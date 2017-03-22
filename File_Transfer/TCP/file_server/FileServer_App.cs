using System;

namespace file_server
{
	public class FileServer_App
	{

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>

		public static void Main (string[] args)
		{
			Console.WriteLine ("Server starts...");
			FileServer fileServer = new FileServer (9000);
		    while (true)
		    {
                fileServer.WaitNewConnection();
            }
		}
	}
}

