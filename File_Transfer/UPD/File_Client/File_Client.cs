using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace File_Client
{
	public class File_Client
	{
	    private UdpClient _udpClient;
	    private string _hostIp;
		public File_Client (string[] args)
		{
		    _hostIp = args[0];
		    _udpClient = new UdpClient(9000);
		}

	    public void SendCommand()
	    {
	        string cmdToSend = Console.ReadLine();
            _udpClient.Connect(_hostIp,9000);
	        Byte[] bytesToSend = Encoding.ASCII.GetBytes(cmdToSend);
	        _udpClient.Send(bytesToSend, bytesToSend.Length);
	    }

	    public void ReceiveAnswer()
	    {
	        IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
	        Byte[] receivedBytes = _udpClient.Receive(ref remoteIpEndPoint);
            string returnData = Encoding.ASCII.GetString(receivedBytes);
	        Console.WriteLine(returnData);
        }


	}
}

