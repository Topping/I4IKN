using System;
using Linklaget;

/// <summary>
/// Transport.
/// </summary>
namespace Transportlaget
{
/// <summary>
/// Transport.
/// </summary>
public class Transport {
/// <summary>
/// The _link.
/// </summary>
private Link _link;
/// <summary>
/// The 1' complements _checksum.
/// </summary>
private Checksum _checksum;
/// <summary>
/// The _buffer.
/// </summary>
private byte[] _buffer;
/// <summary>
/// The seq no.
/// </summary>
private byte _seqNo;
/// <summary>
/// The old_seq no.
/// </summary>
private byte _oldSeqNo;
/// <summary>
/// The error count.
/// </summary>
private int _errorCount;
/// <summary>
/// The DEFAULT_SEQNO.
/// </summary>
private const int DEFAULT_SEQNO = 2;
/// <summary>
/// The data received. True = received data in ReceiveAck, False = not received data in ReceiveAck
/// </summary>
private bool _dataReceived;
/// <summary>
/// The number of data the recveived.
/// </summary>
private int _recvSize = 0;

/// <summary>
/// Initializes a new instance of the <see cref="Transport"/> class.
/// </summary>
public Transport (int BUFSIZE, string APP)
{
	_link = new Link(BUFSIZE+(int)TransSize.ACKSIZE, APP);
	_checksum = new Checksum();
	_buffer = new byte[BUFSIZE+(int)TransSize.ACKSIZE];
	_seqNo = 0;
	_oldSeqNo = DEFAULT_SEQNO;
	_errorCount = 0;
	_dataReceived = false;
}

/// <summary>
/// Receives the ack.
/// </summary>
/// <returns>
/// The ack.
/// </returns>
private bool ReceiveAck()
{
	_recvSize = _link.Receive(ref _buffer);
	_dataReceived = true;

	if (_recvSize == (int)TransSize.ACKSIZE) {
		_dataReceived = false;
		if (!_checksum.checkChecksum (_buffer, (int)TransSize.ACKSIZE) ||
		  _buffer [(int)TransCHKSUM.SEQNO] != _seqNo ||
		  _buffer [(int)TransCHKSUM.TYPE] != (int)TransType.ACK)
		{
			return false;
		}
		_seqNo = (byte)((_buffer[(int)TransCHKSUM.SEQNO] + 1) % 2);
	}

	return true;
}

/// <summary>
/// Sends the ack.
/// </summary>
/// <param name='ackType'>
/// Ack type.
/// </param>
private void SendAck (bool ackType)
{
	byte[] ackBuf = new byte[(int)TransSize.ACKSIZE];
	ackBuf [(int)TransCHKSUM.SEQNO] = (byte)
		(ackType ? (byte)_buffer [(int)TransCHKSUM.SEQNO] : (byte)(_buffer [(int)TransCHKSUM.SEQNO] + 1) % 2);
	ackBuf [(int)TransCHKSUM.TYPE] = (byte)(int)TransType.ACK;
	_checksum.calcChecksum (ref ackBuf, (int)TransSize.ACKSIZE);
	if(++_errorCount == 3) {
		ackBuf[ 1 ]++;
		Console.WriteLine( "Byte 1 spoiled in ACK-package {0}", _errorCount );
	}
	_link.Send(ackBuf, (int)TransSize.ACKSIZE);
}

/// <summary>
/// Send the specified _buffer and size.
/// </summary>
/// <param name='buffer'>
/// Buffer.
/// </param>
/// <param name='size'>
/// Size.
/// </param>
public void Send(byte[] buf, int size)
{
	Array.Copy(buf, 0, _buffer, 4, size);
	_buffer[2] = _seqNo;
	_buffer[3] = 0;

	_checksum.calcChecksum(ref _buffer, _buffer.Length);

	bool success;

	do {
		if (++_errorCount == 3) // Simulate noise in DATA-package
		{
			_buffer[1]++; // Important: Only spoil a checksum-field (buffer[0] or buffer[1])
			Console.WriteLine("Noise on send : " + _errorCount);
		}

		_link.Send( _buffer, _buffer.Length );
		success = ReceiveAck();
	} while (!success);
}

/// <summary>
/// Receive the specified _buffer.
/// </summary>
/// <param name='buf'>
/// Buffer.
/// </param>
public int Receive( ref byte[] buf ) {
	var success = false;
	var receivedBytes = _link.Receive( ref _buffer );

	while ( !success ) {
		if ( _buffer[ (int) TransCHKSUM.SEQNO ] == _oldSeqNo ) {
			SendAck( true );
			return 0;
		}

		if ( _checksum.checkChecksum( _buffer, receivedBytes ) ) {
			Array.Copy( _buffer, 4, buf, 0, receivedBytes - 4 );

			_oldSeqNo = _buffer[ (int) TransCHKSUM.SEQNO ];
			SendAck( true );
			success = true;
		} else {
			Console.WriteLine( "Transport: Checksum nok OK. Sending False Ack: {0}", _seqNo );
			SendAck( false );
		}
	}

	return receivedBytes - 4;
}
}
}