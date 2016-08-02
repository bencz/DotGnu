/*
 * SerialPort.cs - Implementation of the "System.IO.Ports.SerialPort" class.
 *
 * Copyright (C) 2003-2004  Southern Storm Software, Pty Ltd.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace System.IO.Ports
{

#if CONFIG_SERIAL_PORTS

using System.ComponentModel;
using System.Security;
using System.Text;
using Platform;

public class SerialPort
#if CONFIG_COMPONENT_MODEL
	: Component
#endif
{
	// Internal state.
	private String resource;
	private int portType;
	private int portNumber;
	private Encoding encoding;
	private IntPtr handle;
	private PortMethods.Parameters parameters;
	private Stream baseStream;
	private String newLine;
	private byte[] newLineBuffer;
	private SerialPinChangedEventHandler pinChanged;
	private SerialReceivedEventHandler received;
	private static readonly int[] validBaudRates =
		{50, 75, 110, 134, 150, 200, 300, 600, 1200,
		 1800, 2400, 4800, 9600, 19200, 38400, 57600,
		 115200, 230400, 460800, 500000, 576000, 921600,
		 1000000, 1152000, 1500000, 2000000, 2500000,
		 3000000, 3500000, 4000000};

	// Timeout value that indicates an infinite timeout.
	public static readonly int InfiniteTimeout = 0;

	// Constructors.
	public SerialPort()
			{
				resource = null;
				portType = PortMethods.SERIAL_REGULAR;
				portNumber = 0;
				encoding = new UTF8Encoding();
				handle = IntPtr.Zero;
				parameters = new PortMethods.Parameters();
				parameters.baudRate = 9600;
				parameters.parity = (int)(Parity.None);
				parameters.dataBits = 8;
				parameters.stopBits = (int)(StopBits.One);
				parameters.handshake = (int)(Handshake.None);
				parameters.parityReplace = (byte)0;
				parameters.discardNull = false;
				PortMethods.GetRecommendedBufferSizes
					(out parameters.readBufferSize,
					 out parameters.writeBufferSize,
					 out parameters.receivedBytesThreshold);
				parameters.readTimeout = InfiniteTimeout;
				parameters.writeTimeout = InfiniteTimeout;
				baseStream = null;
				newLine = "\n";  // MS defaults to \n
				newLineBuffer = null;
			}
	[TODO]
	public SerialPort(IContainer container) : this()
			{
				// TODO
			}
	public SerialPort(String resource) : this()
			{
				PortName = resource;
			}
	public SerialPort(String resource, int baudRate)
			: this(resource, baudRate, Parity.None, 8, StopBits.One) {}
	public SerialPort(String resource, int baudRate, Parity parity)
			: this(resource, baudRate, parity, 8, StopBits.One) {}
	public SerialPort(String resource, int baudRate,
					  Parity parity, int dataBits)
			: this(resource, baudRate, parity, dataBits, StopBits.One) {}
	public SerialPort(String resource, int baudRate,
					  Parity parity, int dataBits, StopBits stopBits)
			: this()
			{
				PortName = resource;
				BaudRate = baudRate;
				Parity = parity;
				DataBits = dataBits;
				StopBits = stopBits;
			}

#if !CONFIG_COMPONENT_MODEL
	// Destructor.
	~SerialPort()
			{
				Dispose(false);
			}
#endif

	// Get the base stream that wraps the serial port.
	public Stream BaseStream
			{
				get
				{
					return baseStream;
				}
			}

	// Get or set the serial port's properties.
	public int BaudRate
			{
				get
				{
					return parameters.baudRate;
				}
				set
				{
					if(Array.IndexOf(validBaudRates, value) == -1)
					{
						throw new ArgumentException
							(S._("Arg_PortBaudRate"), "value");
					}
					lock(this)
					{
						if(parameters.baudRate != value)
						{
							parameters.baudRate = value;
							if(handle != IntPtr.Zero)
							{
								PortMethods.Modify(handle, parameters);
							}
						}
					}
				}
			}
	public int DataBits
			{
				get
				{
					return parameters.dataBits;
				}
				set
				{
					if(value != 7 && value != 8)
					{
						throw new ArgumentException
							(S._("Arg_PortDataBits"), "value");
					}
					lock(this)
					{
						if(parameters.dataBits != value)
						{
							parameters.dataBits = value;
							if(handle != IntPtr.Zero)
							{
								PortMethods.Modify(handle, parameters);
							}
						}
					}
				}
			}
	public Parity Parity
			{
				get
				{
					return (Parity)(parameters.parity);
				}
				set
				{
					if(((int)value) < ((int)(Parity.None)) ||
					   ((int)value) > ((int)(Parity.Space)))
					{
						throw new ArgumentException
							(S._("Arg_PortParity"), "value");
					}
					lock(this)
					{
						if(parameters.parity != (int)value)
						{
							parameters.parity = (int)value;
							if(handle != IntPtr.Zero)
							{
								PortMethods.Modify(handle, parameters);
							}
						}
					}
				}
			}
	public StopBits StopBits
			{
				get
				{
					return (StopBits)(parameters.stopBits);
				}
				set
				{
					if(((int)value) < ((int)(StopBits.One)) ||
					   ((int)value) > ((int)(StopBits.OnePointFive)))
					{
						throw new ArgumentException
							(S._("Arg_PortStopBits"), "value");
					}
					lock(this)
					{
						if(parameters.stopBits != (int)value)
						{
							parameters.stopBits = (int)value;
							if(handle != IntPtr.Zero)
							{
								PortMethods.Modify(handle, parameters);
							}
						}
					}
				}
			}

	// Get the number of bytes that can be read or written.
	public int BytesToRead
			{
				get
				{
					lock(this)
					{
						if(handle != IntPtr.Zero)
						{
							return PortMethods.GetBytesToRead(handle);
						}
						else
						{
							throw new InvalidOperationException
								(S._("Invalid_PortNotOpen"));
						}
					}
				}
			}
	public int BytesToWrite
			{
				get
				{
					lock(this)
					{
						if(handle != IntPtr.Zero)
						{
							return PortMethods.GetBytesToWrite(handle);
						}
						else
						{
							throw new InvalidOperationException
								(S._("Invalid_PortNotOpen"));
						}
					}
				}
			}

	// Read a serial port pin.
	private bool ReadPin(int pin)
			{
				lock(this)
				{
					if(handle != IntPtr.Zero)
					{
						return ((PortMethods.ReadPins(handle) & pin) != 0);
					}
					else
					{
						throw new InvalidOperationException
							(S._("Invalid_PortNotOpen"));
					}
				}
			}

	// Write a serial port pin.
	private void WritePin(int pin, bool value)
			{
				lock(this)
				{
					if(handle != IntPtr.Zero)
					{
						if(value)
						{
							PortMethods.WritePins(handle, pin, pin);
						}
						else
						{
							PortMethods.WritePins(handle, pin, 0);
						}
					}
					else
					{
						throw new InvalidOperationException
							(S._("Invalid_PortNotOpen"));
					}
				}
			}

	// Check or set the state of various pins.
	public bool BreakState
			{
				get
				{
					return ReadPin(PortMethods.PIN_BREAK);
				}
				set
				{
					WritePin(PortMethods.PIN_BREAK, value);
				}
			}
	public bool CDHolding
			{
				get
				{
					return ReadPin(PortMethods.PIN_CD);
				}
			}
	public bool CtsHolding
			{
				get
				{
					return ReadPin(PortMethods.PIN_CTS);
				}
			}
	public bool DsrHolding
			{
				get
				{
					return ReadPin(PortMethods.PIN_DSR);
				}
			}
	public bool DtrEnable
			{
				get
				{
					return ReadPin(PortMethods.PIN_DTR);
				}
				set
				{
					WritePin(PortMethods.PIN_DTR, value);
				}
			}
	public bool RtsEnable
			{
				get
				{
					return ReadPin(PortMethods.PIN_RTS);
				}
				set
				{
					WritePin(PortMethods.PIN_RTS, value);
				}
			}

	// Get or set the "discard NUL" flag.
	public bool DiscardNull
			{
				get
				{
					return parameters.discardNull;
				}
				set
				{
					lock(this)
					{
						if(parameters.discardNull != value)
						{
							parameters.discardNull = value;
							if(handle != IntPtr.Zero)
							{
								PortMethods.Modify(handle, parameters);
							}
						}
					}
				}
			}

	// Get or set the encoding used for text conversion.
	public Encoding Encoding
			{
				get
				{
					return encoding;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					encoding = value;
					newLineBuffer = null;
				}
			}

	// Get or set the handshaking mode.
	public Handshake Handshake
			{
				get
				{
					return (Handshake)(parameters.handshake);
				}
				set
				{
					if(((int)value) < ((int)(Handshake.None)) ||
					   ((int)value) > ((int)(Handshake.RequestToSendXOnXOff)))
					{
						throw new ArgumentException
							(S._("Arg_PortHandshake"), "value");
					}
					lock(this)
					{
						if(parameters.handshake != (int)value)
						{
							parameters.handshake = (int)value;
							if(handle != IntPtr.Zero)
							{
								PortMethods.Modify(handle, parameters);
							}
						}
					}
				}
			}

	// Determine if the serial port is currently open.
	public bool IsOpen
			{
				get
				{
					lock(this)
					{
						return (handle != IntPtr.Zero);
					}
				}
			}

	// Get or set the newline sequence to use for text writes.
	public String NewLine
			{
				get
				{
					return newLine;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					newLine = value;
					newLineBuffer = null;
				}
			}

	// Get or set the byte value to use to replace parity errors.
	public byte ParityReplace
			{
				get
				{
					return parameters.parityReplace;
				}
				set
				{
					lock(this)
					{
						if(parameters.parityReplace != value)
						{
							parameters.parityReplace = value;
							if(handle != IntPtr.Zero)
							{
								PortMethods.Modify(handle, parameters);
							}
						}
					}
				}
			}

	// Get or set the name of the port.  We only allow port names
	// of the form "COMn", "COMn:", "IRCOMMn", "IRCOMMn:", "USBn",
	// and "USBn:" in this implementation.  "COM" ports are regular
	// serial ports, "IRCOMM" ports are infrared ports, and "USB"
	// are ports on the USB bus.  The numbers are 1-based.
	public String PortName
			{
				get
				{
					return resource;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					int posn;
					int type;
					if(value.Length > 3 &&
					   (value[0] == 'c' || value[0] == 'C') &&
					   (value[1] == 'o' || value[1] == 'O') &&
					   (value[2] == 'm' || value[2] == 'M'))
					{
						posn = 3;
						type = PortMethods.SERIAL_REGULAR;
					}
					else if(value.Length > 6 &&
					        (value[0] == 'i' || value[0] == 'I') &&
					        (value[1] == 'r' || value[1] == 'R') &&
					        (value[2] == 'c' || value[2] == 'C') &&
					        (value[3] == 'o' || value[3] == 'O') &&
					        (value[4] == 'm' || value[4] == 'M') &&
					        (value[5] == 'm' || value[5] == 'M'))
					{
						posn = 6;
						type = PortMethods.SERIAL_INFRARED;
					}
					else if(value.Length > 6 &&
					        (value[0] == 'r' || value[0] == 'R') &&
					        (value[1] == 'f' || value[1] == 'F') &&
					        (value[2] == 'c' || value[2] == 'C') &&
					        (value[3] == 'o' || value[3] == 'O') &&
					        (value[4] == 'm' || value[4] == 'M') &&
					        (value[5] == 'm' || value[5] == 'M'))
					{
						posn = 6;
						type = PortMethods.SERIAL_RFCOMM;
					}
					else if(value.Length > 3 &&
					        (value[0] == 'u' || value[0] == 'U') &&
					        (value[1] == 's' || value[1] == 'S') &&
					        (value[2] == 'b' || value[2] == 'B'))
					{
						posn = 3;
						type = PortMethods.SERIAL_USB;
					}
					else
					{
						throw new ArgumentException
							(S._("Arg_PortName"), "value");
					}
					if(value[posn] < '0' || value[posn] > '9')
					{
						throw new ArgumentException
							(S._("Arg_PortName"), "value");
					}
					int number = (int)(value[posn] - '0');
					++posn;
					while(posn < value.Length &&
					      value[posn] >= '0' && value[posn] <= '9')
					{
						number = number * 10 + (int)(value[posn] - '0');
						++posn;
					}
					if(posn == value.Length ||
					   ((posn + 1) == value.Length && value[posn] == ':'))
					{
						if(!PortMethods.IsValid(type, number))
						{
							throw new ArgumentException
								(S._("Arg_PortName"), "value");
						}
						lock(this)
						{
							if(handle != IntPtr.Zero)
							{
								throw new InvalidOperationException
									(S._("Invalid_PortOpen"));
							}
							portType = type;
							portNumber = number;
							resource = value;
						}
					}
					else
					{
						throw new ArgumentException
							(S._("Arg_PortName"), "value");
					}
				}
			}

	// Get or set the buffer sizes.
	public int ReadBufferSize
			{
				get
				{
					return parameters.readBufferSize;
				}
				set
				{
					if(value < 0)
					{
						throw new ArgumentOutOfRangeException
							("value", S._("ArgRange_NonNegative"));
					}
					lock(this)
					{
						if(parameters.readBufferSize != value)
						{
							parameters.readBufferSize = value;
							if(handle != IntPtr.Zero)
							{
								PortMethods.Modify(handle, parameters);
							}
						}
					}
				}
			}
	public int WriteBufferSize
			{
				get
				{
					return parameters.writeBufferSize;
				}
				set
				{
					if(value < 0)
					{
						throw new ArgumentOutOfRangeException
							("value", S._("ArgRange_NonNegative"));
					}
					lock(this)
					{
						if(parameters.writeBufferSize != value)
						{
							parameters.writeBufferSize = value;
							if(handle != IntPtr.Zero)
							{
								PortMethods.Modify(handle, parameters);
							}
						}
					}
				}
			}
	public int ReceivedBytesThreshold
			{
				get
				{
					return parameters.receivedBytesThreshold;
				}
				set
				{
					if(value < 0)
					{
						throw new ArgumentOutOfRangeException
							("value", S._("ArgRange_NonNegative"));
					}
					lock(this)
					{
						if(parameters.receivedBytesThreshold != value)
						{
							parameters.receivedBytesThreshold = value;
							if(handle != IntPtr.Zero)
							{
								PortMethods.Modify(handle, parameters);
							}
						}
					}
				}
			}

	// Get or set the timeout values.
	public int ReadTimeout
			{
				get
				{
					return parameters.readTimeout;
				}
				set
				{
					if(value < -1)
					{
						throw new ArgumentOutOfRangeException
							("value", S._("ArgRange_NonNegOrNegOne"));
					}
					lock(this)
					{
						if(parameters.readTimeout != value)
						{
							parameters.readTimeout = value;
							if(handle != IntPtr.Zero)
							{
								PortMethods.Modify(handle, parameters);
							}
						}
					}
				}
			}
	public int WriteTimeout
			{
				get
				{
					return parameters.writeTimeout;
				}
				set
				{
					if(value < -1)
					{
						throw new ArgumentOutOfRangeException
							("value", S._("ArgRange_NonNegOrNegOne"));
					}
					lock(this)
					{
						if(parameters.writeTimeout != value)
						{
							parameters.writeTimeout = value;
							if(handle != IntPtr.Zero)
							{
								PortMethods.Modify(handle, parameters);
							}
						}
					}
				}
			}

	// Close this serial port.
	public void Close()
			{
				lock(this)
				{
					if(handle != IntPtr.Zero)
					{
						PortMethods.Close(handle);
						handle = IntPtr.Zero;
					}
					baseStream = null;
				}
			}

	// Discard the contents of the input buffer.
	public void DiscardInBuffer()
			{
				lock(this)
				{
					if(handle != IntPtr.Zero)
					{
						PortMethods.DiscardInBuffer(handle);
					}
					else
					{
						throw new InvalidOperationException
							(S._("Invalid_PortNotOpen"));
					}
				}
			}

	// Discard the contents of the output buffer.
	public void DiscardOutBuffer()
			{
				lock(this)
				{
					if(handle != IntPtr.Zero)
					{
						PortMethods.DiscardOutBuffer(handle);
					}
					else
					{
						throw new InvalidOperationException
							(S._("Invalid_PortNotOpen"));
					}
				}
			}

	// Dispose of this serial port.
#if !CONFIG_COMPONENT_MODEL
	public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
#endif
	protected override void Dispose(bool disposing)
			{
				Close();
			}

	// Open the serial port with the current parameters.
	public void Open()
			{
				lock(this)
				{
					if(handle != IntPtr.Zero)
					{
						throw new InvalidOperationException
							(S._("Invalid_PortOpen"));
					}
					if(resource == null)
					{
						throw new ArgumentException
							(S._("Arg_PortNameNotSet"), "PortName");
					}
					if(!PortMethods.IsAccessible(portType, portNumber))
					{
						throw new SecurityException
							(S._("Arg_CannotAccessPort"));
					}
					handle = PortMethods.Open(portType, portNumber, parameters);
					if(handle == IntPtr.Zero)
					{
						throw new InvalidOperationException
							(S._("Invalid_CannotOpenPort"));
					}
					baseStream = new PortStream(this);
				}
			}

	// Read data from the serial port.
	public int Read(byte[] buffer, int offset, int count)
			{
				lock(this)
				{
					if(baseStream != null)
					{
						return baseStream.Read(buffer, offset, count);
					}
					else
					{
						throw new InvalidOperationException
							(S._("Invalid_PortNotOpen"));
					}
				}
			}
	[TODO]
	public int Read(char[] buffer, int offset, int count)
			{
				// TODO
				return 0;
			}

	// Read a single byte from the serial port.
	public int ReadByte()
			{
				lock(this)
				{
					if(baseStream != null)
					{
						return baseStream.ReadByte();
					}
					else
					{
						throw new InvalidOperationException
							(S._("Invalid_PortNotOpen"));
					}
				}
			}

	// Read a single character from the serial port.
	[TODO]
	public int ReadChar()
			{
				// TODO
				return -1;
			}

	// ???
	[TODO]
	public String ReadExisting()
			{
				// TODO
				int toRead = BytesToRead;
				if(toRead > 0)
				{
					byte[] buffer = new byte[toRead];
					if(Read(buffer, 0, toRead) > 0)
					{
						return encoding.GetString(buffer);
					}
				}
				return null;
			}

	// Read a single line from the serial port.
	[TODO]
	public String ReadLine()
			{
				lock(this)
				{
					if(baseStream != null)
					{
						int[] backbuffer = new int[this.newLine.Length];
						int bytesinbuffer = 0;
						int outbufferpos = 0;
						bool pendingeol = false;
						bool eolfailed = false;
						StringBuilder inLine = new StringBuilder();
						while(true)
						{
							int retByte;
							
							// read the next byte
							if(bytesinbuffer > 0 && eolfailed == true)
							{
								retByte = backbuffer[outbufferpos++];
								if(--bytesinbuffer == 0)
								{
									// backbuffer is flushed. reset for next multibyte eol search
									eolfailed = false;
									outbufferpos = 0;
								}
							}
							else
							{
								retByte = baseStream.ReadByte();
							}
							
							// is this an eol byte?
							if(retByte == this.newLine[bytesinbuffer])
							{
								backbuffer[bytesinbuffer++] = retByte;
								if(this.newLine.Length == bytesinbuffer)
								{
									bytesinbuffer = 0;
									return(inLine.ToString());
								}
								else
								{
									pendingeol = true;
								}
							}
							else
							{
								if(bytesinbuffer > 0)
								{
									// eol search failed after finding at least one of a multi
									// byte eol marker.  set eolfailed so that we flush the bytes
									// that we stored during failed multi byte eol match
									eolfailed = true;
								}
								
							 	if(retByte == -1)
								{
									// FIXME need to store the inLine data so that we do not lose it
									// if another ReadLine is issued after this timeout
									return null;
								}
								
								inLine.Append((char) retByte);						
							}
						}
					}
					else
					{
						throw new InvalidOperationException
							(S._("Invalid_PortNotOpen"));
					}
				}
				// TODO
				return null;
			}

	// Read until we encounter a specific value.
	[TODO]
	public String ReadTo(String value)
			{
				// TODO
				return null;
			}

	// Write a string to the serial port.
	public void Write(String str)
			{
				if(str != null)
				{
					byte[] bytes = encoding.GetBytes(str);
					Write(bytes, 0, bytes.Length);
				}
			}

	// Write a buffer to the serial port.
	public void Write(byte[] buffer, int offset, int count)
			{
				lock(this)
				{
					if(baseStream != null)
					{
						baseStream.Write(buffer, offset, count);
					}
					else
					{
						throw new InvalidOperationException
							(S._("Invalid_PortNotOpen"));
					}
				}
			}
	public void Write(char[] buffer, int offset, int count)
			{
				byte[] bytes = encoding.GetBytes(buffer, offset, count);
				Write(bytes, 0, bytes.Length);
			}

	// Write a string followed by a newline to the serial port.
	public void WriteLine(String str)
			{
				Write(str);
				lock(this)
				{
					if(newLineBuffer == null)
					{
						newLineBuffer = encoding.GetBytes(newLine);
					}
					Write(newLineBuffer, 0, newLineBuffer.Length);
				}
			}

	// Event that is emitted when an error occurs (ignored in this version).
	public event SerialErrorEventHandler ErrorEvent;

	// Event that is emitted when incoming serial pins change state.
	[TODO]
	public event SerialPinChangedEventHandler PinChangedEvent
			{
				add
				{
					lock(this)
					{
						bool empty = (pinChanged == null);
						pinChanged = pinChanged + value;
						if(empty && pinChanged != null)
						{
							// TODO: launch the pin notification thread.
						}
					}
				}
				remove
				{
					lock(this)
					{
						pinChanged = pinChanged - value;
						if(pinChanged == null)
						{
							// TODO: shut down the pin notification thread.
						}
					}
				}
			}

	// Event that is emitted when data is received.
	[TODO]
	public event SerialReceivedEventHandler ReceivedEvent
			{
				add
				{
					lock(this)
					{
						bool empty = (received == null);
						received = received + value;
						if(empty && received != null)
						{
							// TODO: launch the data received thread.
						}
					}
				}
				remove
				{
					lock(this)
					{
						received = received - value;
						if(received == null)
						{
							// TODO: shut down the data received thread.
						}
					}
				}
			}

	// Stream class that wraps up a serial port.
	private sealed class PortStream : Stream
	{
		// Internal state.
		private SerialPort port;
		private byte[] byteBuffer;

		// Constructor.
		public PortStream(SerialPort port)
				{
					this.port = port;
					this.byteBuffer = new byte [1];
				}

		// Close the stream.
		public override void Close()
				{
					port.Close();
				}

		// Flush the pending contents in this stream.
		public override void Flush()
				{
					lock(port)
					{
						if(port.handle != IntPtr.Zero)
						{
							PortMethods.DrainOutBuffer(port.handle);
						}
						else
						{
							throw new InvalidOperationException
								(S._("Invalid_PortNotOpen"));
						}
					}
				}

		// Read data from this stream.
		public override int Read(byte[] buffer, int offset, int count)
				{
					ValidateBuffer(buffer, offset, count);
					lock(port)
					{
						if(port.handle != IntPtr.Zero)
						{
							return PortMethods.Read
								(port.handle, buffer, offset, count);
						}
						else
						{
							throw new InvalidOperationException
								(S._("Invalid_PortNotOpen"));
						}
					}
				}

		// Read a single byte from this stream.
		public override int ReadByte()
				{
					lock(port)
					{
						if(port.handle != IntPtr.Zero)
						{
							int count = PortMethods.Read
								(port.handle, byteBuffer, 0, 1);
							if(count > 0)
							{
								return (int)(byteBuffer[0]);
							}
							else
							{
								// Timeout probably.
								return -1;
							}
						}
						else
						{
							throw new InvalidOperationException
								(S._("Invalid_PortNotOpen"));
						}
					}
				}

		// Seek to a new position within this stream.
		public override long Seek(long offset, SeekOrigin origin)
				{
					throw new NotSupportedException(S._("IO_NotSupp_Seek"));
				}

		// Set the length of this stream.
		public override void SetLength(long value)
				{
					throw new NotSupportedException
						(S._("IO_NotSupp_SetLength"));
				}

		// Throw exception according to retval.
		// Timeout has value 0, otherwise it's generic IO exception.
		private void ThrowPortException(int retval)
				{
					if(retval == 0)
					{
						// TODO: throw System.ServiceProcess.TimeoutException
						throw new SystemException(S._("IO_Timeout"));
					}
					else
					{
						// This exception is not in docs but is useful
						Errno errno = SocketMethods.GetErrno();
						String message = SocketMethods.GetErrnoMessage(errno);
						throw new IOException(message);
					}
				}

		// Write a buffer of bytes to this stream.
		public override void Write(byte[] buffer, int offset, int count)
				{
					ValidateBuffer(buffer, offset, count);
					lock(port)
					{
						if(port.handle != IntPtr.Zero)
						{
							int retval = PortMethods.Write
								(port.handle, buffer, offset, count);
							if(retval <= 0)
							{
								ThrowPortException(retval);
							}
						}
						else
						{
							throw new InvalidOperationException
								(S._("Invalid_PortNotOpen"));
						}
					}
				}

		// Write a single byte to this stream.
		public override void WriteByte(byte value)
				{
					lock(port)
					{
						if(port.handle != IntPtr.Zero)
						{
							byteBuffer[0] = value;
							int retval = PortMethods.Write
								(port.handle, byteBuffer, 0, 1);
							if(retval <= 0)
							{
								ThrowPortException(retval);
							}
						}
						else
						{
							throw new InvalidOperationException
								(S._("Invalid_PortNotOpen"));
						}
					}
				}

		// Determine if it is possible to read from this stream.
		public override bool CanRead
				{
					get
					{
						return true;
					}
				}

		// Determine if it is possible to seek within this stream.
		public override bool CanSeek
				{
					get
					{
						return false;
					}
				}

		// Determine if it is possible to write to this stream.
		public override bool CanWrite
				{
					get
					{
						return true;
					}
				}

		// Get the length of this stream.
		public override long Length
				{
					get
					{
						throw new NotSupportedException
							(S._("IO_NotSupp_Seek"));
					}
				}

		// Get the current position within the stream.
		public override long Position
				{
					get
					{
						throw new NotSupportedException
							(S._("IO_NotSupp_Seek"));
					}
					set
					{
						throw new NotSupportedException
							(S._("IO_NotSupp_Seek"));
					}
				}

	}; // class PortStream

	// Helper methods for validating buffer arguments.
	internal static void ValidateBuffer(byte[] buffer, int offset, int count)
			{
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				else if(offset < 0 || offset > buffer.Length)
				{
					throw new ArgumentOutOfRangeException
						("offset", S._("ArgRange_Array"));
				}
				else if(count < 0)
				{
					throw new ArgumentOutOfRangeException
						("count", S._("ArgRange_Array"));
				}
				else if((buffer.Length - offset) < count)
				{
					throw new ArgumentException(S._("Arg_InvalidArrayRange"));
				}
			}
	internal static void ValidateBuffer(char[] buffer, int offset, int count)
			{
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				else if(offset < 0 || offset > buffer.Length)
				{
					throw new ArgumentOutOfRangeException
						("offset", S._("ArgRange_Array"));
				}
				else if(count < 0)
				{
					throw new ArgumentOutOfRangeException
						("count", S._("ArgRange_Array"));
				}
				else if((buffer.Length - offset) < count)
				{
					throw new ArgumentException(S._("Arg_InvalidArrayRange"));
				}
			}

}; // class SerialPort

#endif // CONFIG_SERIAL_PORTS

}; // namespace System.IO.Ports
