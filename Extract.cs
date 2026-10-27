static class Extractor {
    public static void Extract(string fn_in, string fn_out)
    {
        FileStream fs_in = File.OpenRead(fn_in);

        while (true)
        {
            byte[] header_bytes = read_block(fs_in);

            if (is_end_of_file(header_bytes))
            {
                break;
            }

            FileHeader f_info = extract_file_info(header_bytes);

            extract_file(fs_in, f_info);
        }

        fs_in.Close();
    }

    private static byte[] read_block(FileStream fs_in)
    {
        byte[] buffer = new byte[512];

        fs_in.ReadExactly(buffer);

        return buffer;
    }

    private static bool is_end_of_file(byte[] block)
    {

        if (block[0] != 0)
        {
            return false;
        }

        for (int i = 0; i < block.Length - 1; i++)
        {
            if (block[i] != block[i + 1])
            {
                return false;
            }
        }

        return true;
    }

    private static FileHeader extract_file_info(byte[] header)
    {
        string path = System.Text.Encoding.UTF8.GetString(header[0..256]).Replace("\0", string.Empty);
        int size = BitConverter.ToInt32(header[256..288]);

        return new FileHeader(path, size);
    }

    private static void extract_file(FileStream fs_in, FileHeader fh_out)
    {
        int block_count = (int)Math.Ceiling(Convert.ToDouble(fh_out.size / 512));

        byte[] buffer; ;

        using (FileStream fs_out = File.OpenWrite(fh_out.path))
        {
            for (int i = 0; i < block_count - 1; i++)
            {
                buffer = read_block(fs_in);

                fs_out.Write(buffer, 0, 512);
            }

            int bytes_remaining = (int)fh_out.size - (512 * block_count);
            buffer = new byte[bytes_remaining];
            fs_in.ReadExactly(buffer);
            fs_out.Write(buffer, 0, bytes_remaining);

            //need to move head to end of block
            for (int i = 0; i < (512 - bytes_remaining); i++)
            {
                fs_in.ReadByte();
            }
        }
    }

    struct FileHeader
    {
        public FileHeader(string path, int size)
        {
            this.path = path;
            this.size = size;
        }
    
        public string path { get; init; }
        public int size { get; init; }
    }
}
