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
			string[] myArgs = { "127.0.0.1", "/root/Documents/test.txt" };
			FileClient fileClient = new FileClient (myArgs);
			fileClient.receiveFile ("/root/Documents/test2.txt");
		}

	}
}

