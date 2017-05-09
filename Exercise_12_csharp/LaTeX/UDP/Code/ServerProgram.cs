namespace File_Server
{
    public class Program
    {
        public static void Main()
        {
            File_Server fileServer = new File_Server(9000);
            while (true)
            {
                fileServer.ReceiveCommand();
            }
            
        }
    }
}