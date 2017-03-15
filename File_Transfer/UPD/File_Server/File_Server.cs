using System;
using System.Net;
using System.Net.Sockets;

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
		}

		public void ReceiveCommand()
		{
			
		}


	}
}

