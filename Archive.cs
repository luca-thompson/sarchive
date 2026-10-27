using System.Text;

static class Archiver {
    public static void Archive(string fn_out, string[] fns_in)
    {
        //overwrites existing
        if (File.Exists(fn_out))
        {
            File.Delete(fn_out);
        }

        FileStream fs_out = File.OpenWrite(fn_out);

        foreach (string fn in fns_in)
        {
            using (FileStream fs_in = File.OpenRead(fn))
            {
                write_header(fs_in, fs_out);

                write_data(fs_in, fs_out);
            }
        }

        write_eof(fs_out);

        fs_out.Close();
    }

    private static byte[] pad_or_truncate_to_256b(byte[] bytes_in)
    {
        byte[] blocked_bytes = new byte[256];

        //truncate
        if (bytes_in.Length > 256)
        {
            return bytes_in[0..255];
        }

        //pad
        for (int i = 0; i < bytes_in.Length; i++)
        {
            blocked_bytes[i] = bytes_in[i];
        }

        return blocked_bytes;
    }

    private static byte[] pad_remainder_to_512b(byte[] bytes_in)
    {
        byte[] blocked_bytes = new byte[512];

        //pad
        for (int i = 0; i < bytes_in.Length; i++)
        {
            blocked_bytes[i] = bytes_in[i];
        }

        return blocked_bytes;
    }

    private static void write_header(FileStream fs_in, FileStream fs_out)
    {
        byte[] length_as_bytes = pad_or_truncate_to_256b(BitConverter.GetBytes(fs_in.Length));

        byte[] name_as_bytes = pad_or_truncate_to_256b(Encoding.ASCII.GetBytes(fs_in.Name));

        fs_out.Write(name_as_bytes, 0, 256);
        fs_out.Write(length_as_bytes, 0, 256);
    }

    static void write_data(FileStream fs_in, FileStream fs_out)
    {
        int block_count = (int)Math.Ceiling(Convert.ToDouble(fs_in.Length / 512));
    
        byte[] buffer = new byte[512];
    
        for (int i = 0; i < block_count - 1; i++)
        {
            fs_in.ReadExactly(buffer);
            fs_out.Write(buffer, 0, 512);
        }
    
        int bytes_remaining = (int)fs_in.Length - (512 * block_count);
        buffer = new byte[bytes_remaining];
    
        fs_in.ReadExactly(buffer);
    
        byte[] padded_final_block = pad_remainder_to_512b(buffer);
        fs_out.Write(padded_final_block, 0, 512);
    }
    
    static void write_eof(FileStream fs_out){
    
        byte[] emptyByteArray = new byte[512];
    
        fs_out.Write(emptyByteArray, 0, 512);
    }
}
