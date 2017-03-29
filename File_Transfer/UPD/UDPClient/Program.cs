namespace UDPClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            UDPClient.UdpClient fileClient = new UDPClient.UdpClient(args);
            fileClient.SendCommand();
            fileClient.ReceiveAnswer();
        }
    }
}