class Sarchive {
    
    static int Main(string[] args){
        
        switch (args[0]){
            case "create":
                Archiver.Archive(args[1], args[2..]);
                return 0;
            case "extract":
                Extractor.Extract(args[1], args[2]);
                return 0;
            default:
                Console.WriteLine("you did something wrong");
                return 1;
        }
    }
}
