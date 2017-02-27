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
			BinaryReader binaryReader = new BinaryReader (serverStream);
			BinaryWriter binaryWriter = new BinaryWriter(File.Open(fileName, FileMode.Create));
			byte[] buffer;
			buffer = binaryReader.ReadBytes (1000);
			Console.WriteLine ("Buffer length after initial read: {0}", buffer.Length);
			int fileOffset = 0;// Første 2 bytes = længde på buffer + null
			while (buffer.Length > 0) 
			{
				Console.WriteLine (buffer.Length);
				binaryWriter.Write (buffer, fileOffset, buffer.Length-fileOffset);
				buffer = binaryReader.ReadBytes (1000);
			}
			serverStream.Close ();
			_clientSocket.Close ();

		}

	}
}

