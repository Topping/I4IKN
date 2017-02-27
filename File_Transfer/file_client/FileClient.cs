/* TODO 
 * Få opdateret så de parametre man taster ind i commandline rent faktisk er til den fil man vil have.
 * I stedet for hard coded
 * 
 * Få fixet filnavnet til den fil der bliver kopieret over, så det ikke bare er "copy.txt"
 */


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
		string _filename;

		TcpClient _clientSocket = new TcpClient();

		public FileClient (string[] args)
		{
			Console.WriteLine ("ClientProgram :: Starting new client");
			Console.WriteLine ("ClientProgram :: Attempting to connect to: {0}:9000", args[0]);
			_hostIP = args [0];
			_filename = args [1];
			_clientSocket.Connect (_hostIP, 9000);
			Console.WriteLine ("ClientProgram :: Client conncted to host at: {0}:9000", _hostIP);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="file_client"/> class.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments. First ip-adress of the server. Second the filename
		/// </param>


		/// <summary>
		/// Receives the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='io'>
		/// Network stream for reading from the server
		/// </param>
		public void receiveFile (string fileName)
		{
			NetworkStream serverStream = _clientSocket.GetStream ();
			System.Threading.Thread.Sleep (1000);
			LIB.writeTextTCP (serverStream, _filename);
			byte[] buffer = new byte[4096];
			FileStream fileWriter = new FileStream ("/root/Documents/copy.txt", FileMode.Create);
			int expectedNumOfBytes = int.Parse(LIB.readTextTCP(serverStream));
			int totalBytesRead = 0;
			int thisTransferBytesRead = 0;
			do {
				thisTransferBytesRead = serverStream.Read (buffer, 0, buffer.Length);
				if (thisTransferBytesRead > 0) {
					Console.WriteLine ("ClientProgram :: Read {0} Bytes from Network Stream...", thisTransferBytesRead);
					fileWriter.Write(buffer, 0, thisTransferBytesRead);
					totalBytesRead += thisTransferBytesRead;
				}
			} while(serverStream.DataAvailable || expectedNumOfBytes != totalBytesRead);
			Console.WriteLine ("ClientProgram :: Done reading {0} KB from Network Stream", totalBytesRead / 1000);
			serverStream.Close ();
			_clientSocket.Close ();

		}

	}
}

