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
				ReadandSend ("/proc/uptime");
                        break;

			case "L":
			case "l":
				ReadandSend ("/proc/loadavg");
		            break;
			default:
				string errorString = "ERROR!!!!!";
				Byte[] bytesToSend = Encoding.ASCII.GetBytes (errorString);
				_udpServer.Send (bytesToSend, bytesToSend.Length, _remoteIp);
				break;
		    }
		}

		private void ReadandSend(string filepath)
		{
			FileStream fileReader = new FileStream(filepath, FileMode.Open, FileAccess.Read);
			using (MemoryStream ms = new MemoryStream())
			{
				fileReader.CopyTo(ms);
				Byte[] buffer = ms.ToArray();
				_udpServer.Send(buffer, buffer.Length, _remoteIp);
			}
		}


	}
}

