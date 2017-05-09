using System;
using System.IO.Ports;
using System.Linq;

/// <summary>
/// Link.
/// </summary>
namespace Linklaget
{
	/// <summary>
	/// Link.
	/// </summary>
	public class Link
	{
		/// <summary>
		/// The DELIMITE for slip protocol.
		/// </summary>
		const byte DELIMITER = (byte)'A';
		/// <summary>
		/// The _buffer for link.
		/// </summary>
		private byte[] _buffer;
		/// <summary>
		/// The serial port.
		/// </summary>
		SerialPort _serialPort;

		/// <summary>
		/// Initializes a new instance of the <see cref="link"/> class.
		/// </summary>
		public Link (int BUFSIZE, string APP)
		{
			// Create a new SerialPort object with default settings.
			#if DEBUG
				if(APP.Equals("FILE_SERVER"))
				{
					_serialPort = new SerialPort("/dev/ttyS0",115200,Parity.None,8,StopBits.One);
				}
				else
				{
					_serialPort = new SerialPort("/dev/ttyS1",115200,Parity.None,8,StopBits.One);
				}
			#else
				_serialPort = new SerialPort("/dev/ttyS1",115200,Parity.None,8,StopBits.One);
			#endif
			if(!_serialPort.IsOpen)
				_serialPort.Open();

			_buffer = new byte[(BUFSIZE*2)];

			// Uncomment the next line to use timeout
			//_serialPort.ReadTimeout = 500;

			_serialPort.DiscardInBuffer ();
			_serialPort.DiscardOutBuffer ();
		}

		/// <summary>
		/// Send the specified buf and size.
		/// </summary>
		/// <param name='buf'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public void Send (byte[] buf, int size)
		{
		    //Every occurance of A or B must be replaced with BC or BD
		    //Increase size by one for every occurance of A or B to accomodate the extra character
		    //Add +2 to account for the additional start and stop byte
		    size += buf.Count(t => t == 'A' || t == 'B') + 2;

		    var transmitArray = new byte[size];

		    //Transmitions must start and stop with the character A
		    transmitArray[0] = Convert.ToByte('A');
		    transmitArray[size - 1] = Convert.ToByte('A');

		    //Run through the _buffer and replace all occurances of A and B with the correct characters
		    var buffCounter = 0;
		    for (var s = 1; s < size - 1; s++) {
		        switch ((char)buf[buffCounter]) {
		            case 'A':
		                transmitArray[s] = Convert.ToByte('B');
		                transmitArray[s + 1] = Convert.ToByte('C');
		                s++;
		                break;
		            case 'B':
		                transmitArray[s] = Convert.ToByte('B');
		                transmitArray[s + 1] = Convert.ToByte('D');
		                s++;
		                break;
		            default:
		                transmitArray[s] = buf[buffCounter];
		                break;
		        }

		        buffCounter++;
		    }

		    _serialPort.Write(transmitArray, 0, transmitArray.Length);
        }

		/// <summary>
		/// Receive the specified buf and size.
		/// </summary>
		/// <param name='buf'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public int Receive (ref byte[] buf)
		{
		    int readByte;
		    var byteIndex = 0;

		    var inFrame = false;

		    // Read all bytes
		    while ((readByte = _serialPort.ReadByte()) != -1) {
		        if (!inFrame && (byte)readByte == DELIMITER) // Begin frame and skip delimiter
		        {
		            inFrame = true;
		            continue;
		        }

		        if (inFrame && (byte)readByte == DELIMITER)    // End frame
		            break;

		        _buffer[byteIndex] = (byte)readByte;
		        byteIndex++;
		    }

		    var bufIndex = 0;

		    for (var i = 0; i < byteIndex; i++) {

		        //If we find a B, and it is not the last character, check the following characters and replace accordingly
		        if (_buffer[i] == 'B' && !(i + 1 >= _buffer.Length)) {
		            switch ((char)_buffer[i + 1]) {
		                case 'C':
		                    buf[bufIndex] = (byte)'A';
		                    break;

		                case 'D':
		                    buf[bufIndex] = (byte)'B';
		                    break;
		            }

		            i++;
		            bufIndex++;
		            continue;
		        }

		        buf[bufIndex] = _buffer[i];
		        bufIndex++;
		    }

		    return bufIndex;
        }
	}
}
