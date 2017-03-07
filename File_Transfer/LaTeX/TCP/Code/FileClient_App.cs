using System;

namespace file_client
{
	public class FileClient_App
	{
		public FileClient_App ()
		{
			
		}


		public static void Main(string[] args)
		{
			Console.WriteLine ("Client starts...");
			//string[] myArgs = { "127.0.0.1", "/root/Documents/test.txt" };
			FileClient fileClient = new FileClient (args);
			try {
				fileClient.ConnectToServer();
				fileClient.receiveFile ();
			} catch (System.Net.Sockets.SocketException ex) {
				Console.WriteLine ("Failed to connect. Error Message: {0}", ex.Message);
			}
		}

	}
}

