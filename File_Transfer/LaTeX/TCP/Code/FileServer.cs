using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using tcp;

namespace file_server
{
public class FileServer
{
	private int _bufferSize;
	private TcpListener _serverSocket;
	private TcpClient _clientSocket = new TcpClient();
}

public FileServer(int port, int buffersize = 4096)
{
	_bufferSize = buffersize;
	_serverSocket = new TcpListener (port);
	_serverSocket.Start();
	Console.WriteLine (">> Server has started.");
}

public void WaitNewConnection()
{
	Console.WriteLine (">> Waiting for new connection...");
	_clientSocket = _serverSocket.AcceptTcpClient ();
	Console.WriteLine (">> Client with IP: {0} connected to the server", _clientSocket.Client.RemoteEndPoint.ToString());
	Send_en_fil_FINDETNAVN ();
}

public void CloseConnection()
{
	_clientSocket.Close ();
	Console.WriteLine (">> Connection to client closed.");
}

public void Send_en_fil_FINDETNAVN()
{
	using (	NetworkStream networkStream = _clientSocket.GetStream() ) 
	{
		string filename = LIB.readTextTCP (networkStream);
		if (File.Exists (filename)) {
			FileStream fileReader = new FileStream (filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			BufferedStream bufferStream = new BufferedStream (fileReader, _bufferSize);
			LIB.writeTextTCP (networkStream, fileReader.Length.ToString ());
			do {
				bufferStream.CopyTo (networkStream);
			} while (fileReader.Position != fileReader.Length);
		} else {
			LIB.writeTextTCP (networkStream, "0");
		}
	}
	CloseConnection ();
	WaitNewConnection ();
}
}
}

