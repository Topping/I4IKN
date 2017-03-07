using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using tcp;

namespace file_client
{
	public class FileClient
	{
string _hostIP;
string _fileOnServer;
string _fileDestination;

TcpClient _clientSocket = new TcpClient();

public FileClient (string[] args)
{
	Console.WriteLine ("ClientProgram :: Starting new client");
	Console.WriteLine ("ClientProgram :: Attempting to connect to: {0}:9000", args[0]);
	_hostIP = args [0];
	_fileOnServer = args [1];
	_fileDestination = Path.Combine (args [2], Path.GetFileName (_fileOnServer));
	Console.WriteLine ("ClientProgram :: Client conncted to host at: {0}:9000", _hostIP);
}

public void ConnectToServer()
{
	_clientSocket.Connect(_hostIP, 9000);
}

public void receiveFile ()
{
	NetworkStream serverStream = _clientSocket.GetStream ();
	serverStream.ReadTimeout = 10;
	LIB.writeTextTCP (serverStream, _fileOnServer);
	byte[] buffer = new byte[1000];
	FileStream fileWriter = new FileStream (_fileDestination, FileMode.Create);
	int expectedNumOfBytes;
	int.TryParse(LIB.readTextTCP(serverStream), out expectedNumOfBytes);
	Console.WriteLine ("ExpectedNumOfBytes :: {0}", expectedNumOfBytes);
	int totalBytesRead = 0;
	int thisTransferBytesRead = 0;
	if (expectedNumOfBytes > 0) {
		do {
			thisTransferBytesRead = serverStream.Read (buffer, 0, buffer.Length);
			if (thisTransferBytesRead == 0)				
				break;
			
			fileWriter.Write (buffer, 0, thisTransferBytesRead);
			totalBytesRead += thisTransferBytesRead;
		} while(serverStream.DataAvailable || expectedNumOfBytes != totalBytesRead);
	}
	Console.WriteLine ("ClientProgram :: Done reading {0} KB from Network Stream", totalBytesRead / 1000);
	serverStream.Close ();
	_clientSocket.Close ();
}

}
}

