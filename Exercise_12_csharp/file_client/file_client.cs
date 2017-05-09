using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;

namespace Application
{
	class file_client
	{
		/// <summary>
		/// The BUFSIZE.
		/// </summary>
		private const int BUFSIZE = 1000;
		private const string APP = "FILE_CLIENT";

		/// <summary>
		/// Initializes a new instance of the <see cref="file_client"/> class.
		/// 
		/// file_client metoden opretter en peer-to-peer forbindelse
		/// Sender en forspÃ¸rgsel for en bestemt fil om denne findes pÃ¥ serveren
		/// Modtager filen hvis denne findes eller en besked om at den ikke findes (jvf. protokol beskrivelse)
		/// Lukker alle streams og den modtagede fil
		/// Udskriver en fejl-meddelelse hvis ikke antal argumenter er rigtige
		/// </summary>
		/// <param name='args'>
		/// Filnavn med evtuelle sti.
		/// </param>
	    private file_client(String[] args)
	    {
			Console.WriteLine ("FileClient Started. Opening file: {0}", args[0]);
			receiveFile(args[0], new Transport(BUFSIZE, APP));
			Console.WriteLine ("FileClient done.");
	    }

		/// <summary>
		/// Receives the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='transport'>
		/// Transportlaget
		/// </param>
		private void receiveFile (String filePath, Transport transport)
		{
			var fileToReceive = Encoding.ASCII.GetBytes (filePath);
			transport.Send (fileToReceive, fileToReceive.Length);
			var bytesToReceive = new byte[BUFSIZE];
			transport.Receive (ref bytesToReceive);

			var fileName = LIB.extractFileName(filePath);
			var file = new FileStream (fileName, FileMode.OpenOrCreate, FileAccess.Write);

			var fileSize = BitConverter.ToInt32 (bytesToReceive, 0);
			var receivedData = new byte[BUFSIZE];

			while(fileSize > 0)
			{
				var receivedBytes = transport.Receive(ref receivedData);

				file.Write (receivedData, 0, receivedBytes);
				fileSize -= receivedBytes;
			}

			file.Close ();
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// First argument: Filname
		/// </param>
		public static void Main (string[] argsx)
		{
			/*
			if(args.Length == 0) {
				Console.WriteLine ("No input argumetns given");
				return;
			}*/
			var args = new string[1];
			args [0] = "/root/Documents/test.txt";
			new file_client(args);
		}
	}
}