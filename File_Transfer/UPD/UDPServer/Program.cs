namespace UDPServer
{
    public class Program
    {
        public static void Main()
        {
            UDPServer.UdpServer udpServer = new UDPServer.UdpServer(9000);
            while (true)
            {
                udpServer.ReceiveCommand();
            }
            
        }
    }
}