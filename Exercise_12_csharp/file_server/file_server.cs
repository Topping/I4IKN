using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;

namespace Application
{
	class file_server
	{
		/// <summary>
		/// The BUFSIZE
		/// </summary>
		private const int BUFSIZE = 1000;
		private const string APP = "FILE_SERVER";

		/// <summary>
		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// </summary>
		private file_server ()
		{
			Console.WriteLine ("FileServer: Started");
			var transport = new Transport (BUFSIZE, APP);

			var fileByte = new byte[BUFSIZE];
			transport.Receive (ref fileByte);

			var filePath = Encoding.Default.GetString (fileByte);
			filePath = filePath.TrimEnd ('\0');

			var fileName = LIB.extractFileName (filePath);
			var fileSize = LIB.check_File_Exists (fileName);
			Console.WriteLine ("Found file named: {0} \n File Size: {1}, fileName, fileSize");

			if (fileSize <= 0) {
				Console.WriteLine ("File not found. Closing connection");
			} else {
				sendFile (fileName, fileSize, transport);
			}
			Console.WriteLine ("File transfer done. Closing connection");
		}

		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='fileSize'>
		/// File size.
		/// </param>
		/// <param name='tl'>
		/// Tl.
		/// </param>
		private void sendFile(String fileName, long fileSize, Transport transport)
		{
			var stream = new FileStream (fileName, FileMode.Open);
			var transmissions = Convert.ToInt32 (
				                    Math.Ceiling (Convert.ToDouble (stream.Length) /
				                    Convert.ToDouble (BUFSIZE)));

			//Hvor mange bytes er der at sende
			var streamLength = (int)stream.Length;
			//Send hvor stor filen der skal sendes er.
			var bytesInFile = BitConverter.GetBytes (fileSize);
			transport.Send (bytesInFile, bytesInFile.Length);

			//Fortsæt så længe der er bytes at sende
			for (var i = 0; i < transmissions; i++) {
				// Skal checke hvor stor en packet der skal sendes.
				// Hvis de BUFSIZE er større end antallet af resterende bytes
				// Sørges for at der kun sendes de resterende bytes.
				int lengthOfPacket;
				if (streamLength < BUFSIZE) {
					lengthOfPacket = BUFSIZE;
					streamLength -= lengthOfPacket;
				} else {
					lengthOfPacket = streamLength;
				}

				var transmitBuffer = new byte[lengthOfPacket];
				stream.Read (transmitBuffer, 0, lengthOfPacket);
				transport.Send (transmitBuffer, transmitBuffer.Length);
			}

			stream.Close ();
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			new file_server();
		}
	}
}