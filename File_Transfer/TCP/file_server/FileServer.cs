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
		/// <summary>
		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// Opretter en socket.
		/// Venter på en connect fra en klient.
		/// Modtager filnavn
		/// Finder filstørrelsen
		/// Kalder metoden sendFile
		/// Lukker socketen og programmet
		/// </summary>
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
				    try
				    {
                        FileStream fileReader = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        BufferedStream bufferStream = new BufferedStream(fileReader, _bufferSize);
                        LIB.writeTextTCP(networkStream, fileReader.Length.ToString());
                        do
                        {
                            bufferStream.CopyTo(networkStream);
                        } while (fileReader.Position != fileReader.Length);
                    }
				    catch (IOException e)
				    {
				        Console.WriteLine(e.Message);
                        CloseConnection();
				        return;
				    }
					
				} else {
					LIB.writeTextTCP (networkStream, "0");
				}
			}
			CloseConnection ();
		}


		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name='fileName'>
		/// The filename.
		/// </param>
		/// <param name='fileSize'>
		/// The filesize.
		/// </param>
		/// <param name='io'>
		/// Network stream for writing to the client.
		/// </param>
		private void sendFile (String fileName, long fileSize, NetworkStream io)
		{
			// TO DO Your own code
		}
	}
}

