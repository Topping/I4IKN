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
		public void receiveFile ()
		{
			NetworkStream serverStream = _clientSocket.GetStream ();
			serverStream.ReadTimeout = 10;
			LIB.writeTextTCP (serverStream, _fileOnServer);
			byte[] buffer = new byte[1000];
			int expectedNumOfBytes;
			int.TryParse(LIB.readTextTCP(serverStream), out expectedNumOfBytes);
			Console.WriteLine ("ExpectedNumOfBytes :: {0}", expectedNumOfBytes);
			int totalBytesRead = 0;
		    if (expectedNumOfBytes > 0)
			{
			    FileStream fileWriter = null;
                try
			    {
                    fileWriter = new FileStream(_fileDestination, FileMode.Create);
                }
			    catch (DirectoryNotFoundException e)
			    {
			        Console.WriteLine(e.Message);
                    serverStream.Close();
                    _clientSocket.Close();
			        return;
			    }
                
                do
                {
					var thisTransferBytesRead = serverStream.Read (buffer, 0, buffer.Length);
					if (thisTransferBytesRead == 0)				
						break;
					
					fileWriter.Write (buffer, 0, thisTransferBytesRead);
					totalBytesRead += thisTransferBytesRead;
				} while(serverStream.DataAvailable || expectedNumOfBytes != totalBytesRead);
                Console.WriteLine("ClientProgram :: Done reading {0} KB from Network Stream", totalBytesRead / 1000);
            }
			else
			{
			    Console.WriteLine("File not found on server");
			}
            
			serverStream.Close ();
			_clientSocket.Close ();
		}

	}
}

