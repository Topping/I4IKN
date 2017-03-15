using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace File_Server
{
	public class File_Server
	{
		private IPEndPoint _remoteIp;
		private UdpClient _udpServer;
		public File_Server (int portnumber)
		{
			_udpServer = new UdpClient (portnumber);
			_remoteIp = new IPEndPoint (IPAddress.Any, 0);
            Console.WriteLine(">> Server has started.");
        }

		public void ReceiveCommand()
		{
            Console.WriteLine(">> Waiting for new connection...");
            Byte[] receivedBytes = _udpServer.Receive(ref _remoteIp);
            string bytesAsString = Encoding.ASCII.GetString(receivedBytes);

		    switch (bytesAsString)
		    {
                case "U":
                case "u":
                    FileStream fileReader = new FileStream("/proc/uptime", FileMode.Open);
		            using (MemoryStream ms = new MemoryStream())
		            {
		                fileReader.CopyTo(ms);
		                Byte[] buffer = ms.ToArray();
		                _udpServer.Send(buffer, buffer.Length, _remoteIp);
		            }
                        break;

                case "L":
                case "l":
		            break;
		    }
		}


	}
}

