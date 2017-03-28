namespace File_Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            File_Client fileClient = new File_Client(args);
            fileClient.SendCommand();
            fileClient.ReceiveAnswer();
        }
    }
}