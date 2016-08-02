/*
 * BinaryReader.cs - Implementation of the "System.IO.BinaryReader" class.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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

namespace System.IO
{

#if !ECMA_COMPAT

using System;
using System.Text;

public class BinaryReader : IDisposable
{
	// Internal state.
	private Stream   input;
	private Encoding encoding;
	private Decoder	 decoder;
	private byte[]   smallBuffer;
	private char[]	 smallCharBuffer;
	private bool	 disposed;

	// Constructors.
	public BinaryReader(Stream input)
			: this(input, Encoding.UTF8)
			{
				// Nothing to do here.
			}
	public BinaryReader(Stream input, Encoding encoding)
			{
				if(input == null)
				{
					throw new ArgumentNullException("input");
				}
				if(encoding == null)
				{
					throw new ArgumentNullException("encoding");
				}
				if(!input.CanRead)
				{
					throw new ArgumentException(_("IO_NotSupp_Read"));
				}
				this.input = input;
				this.encoding = encoding;
				decoder = encoding.GetDecoder();
				smallBuffer = new byte [16];
				smallCharBuffer = new char [1];
				disposed = false;
			}

	// Get the base stream that underlies this binary reader.
	public virtual Stream BaseStream
			{
				get
				{
					return input;
				}
			}

	// Close this stream.
	public virtual void Close()
			{
				if(!disposed)
				{
					input.Close();
				}
				Dispose(true);
			}

	// Implement the IDisposable interface.
	void IDisposable.Dispose()
			{
				Dispose(true);
			}

	// Internal implementation of stream disposal.
	protected virtual void Dispose(bool disposing)
			{
				if(input != null)
				{
					input = null;
				}
				encoding = null;
				decoder = null;
				smallBuffer = null;
				smallCharBuffer = null;
				disposed = true;
			}

	// Read a number of bytes into the small buffer.
	protected virtual void FillBuffer(int num)
			{
				if(disposed)
				{
					throw new ObjectDisposedException("BinaryReader");
				}
				else if(input.Read(smallBuffer, 0, num) != num)
				{
					throw new EndOfStreamException(_("IO_ReadEndOfStream"));
				}
			}

	// Peek at the next character.
	public virtual int PeekChar()
			{
				// Bail out if the stream is invalid.
				if(disposed)
				{
					throw new ObjectDisposedException("BinaryReader");
				}

				// If we cannot seek on the stream, then indicate EOF.
				if(!input.CanSeek)
				{
					return -1;
				}

				// Save the current position, read, and back-track.
				long posn = input.Position;
				int ch = Read();
				input.Position = posn;
				return ch;
			}

	// Read the next character, returning -1 at EOF.
	public virtual int Read()
			{
				int posn;
				int byteval;
				byte[] newBuffer;

				// Bail out if the stream is invalid.
				if(disposed)
				{
					throw new ObjectDisposedException("BinaryReader");
				}

				// Keep reading bytes until we have enough for one character
				// according to the encoding.
				posn = 0;
				do
				{
					if(posn >= smallBuffer.Length)
					{
						newBuffer = new byte [smallBuffer.Length + 16];
						Array.Copy(smallBuffer, newBuffer, smallBuffer.Length);
						smallBuffer = newBuffer;
					}
					byteval = input.ReadByte();
					if(byteval == -1)
					{
						return -1;
					}
					smallBuffer[posn++] = (byte)byteval;
				}
				while(decoder.GetChars(smallBuffer, 0, posn,
									   smallCharBuffer, 0) < 1);

				// Return the character that we read to the caller.
				return smallCharBuffer[0];
			}

	// Read the next character, throwing an exception at EOF.
	public virtual char ReadChar()
			{
				int ch = Read();
				if(ch == -1)
				{
					throw new EndOfStreamException(_("IO_ReadEndOfStream"));
				}
				return (char)ch;
			}

	// Read a buffer of bytes.
	public virtual int Read(byte[] buffer, int index, int count)
			{
				if(disposed)
				{
					throw new ObjectDisposedException("BinaryReader");
				}
				return input.Read(buffer, index, count);
			}

	// Read a buffer of characters.
	public virtual int Read(char[] buffer, int index, int count)
			{
				int posn;
				int byteval;
				int num;
				byte[] newBuffer;

				// Bail out if the stream is invalid.
				if(disposed)
				{
					throw new ObjectDisposedException("BinaryReader");
				}
				// Check args and bail out if they are invalid
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				if(index < 0)
				{
					throw new ArgumentOutOfRangeException("index", _("ArgRange_NonNegative"));
				}
				if(count < 0)
				{
					throw new ArgumentOutOfRangeException("count", _("ArgRange_Array"));
				}
				if((buffer.Length - index) < count)
				{
					throw new ArgumentException(_("Arg_InvalidArrayRange"));
				}
				// Keep reading bytes until EOF or we have enough characters
				// to fill the buffer.
				posn = 0;
				num = 0;
				for(;;)
				{
					if(posn >= smallBuffer.Length)
					{
						newBuffer = new byte [smallBuffer.Length + 16];
						Array.Copy(smallBuffer, newBuffer, smallBuffer.Length);
						smallBuffer = newBuffer;
					}
					byteval = input.ReadByte();
					if(byteval == -1)
					{
						break;
					}
					smallBuffer[posn++] = (byte)byteval;
					if(decoder.GetChars(smallBuffer, 0, posn,
									    buffer, index) != 0)
					{
						posn = 0;
						++num;
						++index;
					}
				}

				// Return the number of characters we have read so far.
				return num;
			}

	// Read boolean values.
	public virtual bool ReadBoolean()
			{
				FillBuffer(1);
				return (smallBuffer[0] != 0);
			}

	// Read various kinds of integer values.
	public virtual byte ReadByte()
			{
				FillBuffer(1);
				return smallBuffer[0];
			}
	[CLSCompliant(false)]
	public virtual sbyte ReadSByte()
			{
				FillBuffer(1);
				return (sbyte)(smallBuffer[0]);
			}
	public virtual short ReadInt16()
			{
				FillBuffer(2);
				return (short)(((int)(smallBuffer[0])) |
					           (((int)(smallBuffer[1])) << 8));
			}
	[CLSCompliant(false)]
	public virtual ushort ReadUInt16()
			{
				FillBuffer(2);
				return (ushort)(((int)(smallBuffer[0])) |
					   		    (((int)(smallBuffer[1])) << 8));
			}
	public virtual int ReadInt32()
			{
				FillBuffer(4);
				return ((int)(smallBuffer[0])) |
					   (((int)(smallBuffer[1])) << 8) |
					   (((int)(smallBuffer[2])) << 16) |
					   (((int)(smallBuffer[3])) << 24);
			}
	[CLSCompliant(false)]
	public virtual uint ReadUInt32()
			{
				FillBuffer(4);
				return ((uint)(smallBuffer[0])) |
					   (((uint)(smallBuffer[1])) << 8) |
					   (((uint)(smallBuffer[2])) << 16) |
					   (((uint)(smallBuffer[3])) << 24);
			}
	public virtual long ReadInt64()
			{
				FillBuffer(8);
				return ((long)(smallBuffer[0])) |
					   (((long)(smallBuffer[1])) << 8) |
					   (((long)(smallBuffer[2])) << 16) |
					   (((long)(smallBuffer[3])) << 24) |
					   (((long)(smallBuffer[4])) << 32) |
					   (((long)(smallBuffer[5])) << 40) |
					   (((long)(smallBuffer[6])) << 48) |
					   (((long)(smallBuffer[7])) << 56);
			}
	[CLSCompliant(false)]
	public virtual ulong ReadUInt64()
			{
				FillBuffer(8);
				return ((ulong)(smallBuffer[0])) |
					   (((ulong)(smallBuffer[1])) << 8) |
					   (((ulong)(smallBuffer[2])) << 16) |
					   (((ulong)(smallBuffer[3])) << 24) |
					   (((ulong)(smallBuffer[4])) << 32) |
					   (((ulong)(smallBuffer[5])) << 40) |
					   (((ulong)(smallBuffer[6])) << 48) |
					   (((ulong)(smallBuffer[7])) << 56);
			}

	// Read floating-point values.
	public virtual float ReadSingle()
			{
				int value;
				FillBuffer(4);
				if(BitConverter.IsLittleEndian)
				{
					value = ((int)(smallBuffer[0])) |
					        (((int)(smallBuffer[1])) << 8) |
					        (((int)(smallBuffer[2])) << 16) |
					        (((int)(smallBuffer[3])) << 24);
				}
				else
				{
					value = ((int)(smallBuffer[3])) |
					        (((int)(smallBuffer[2])) << 8) |
					        (((int)(smallBuffer[1])) << 16) |
					        (((int)(smallBuffer[0])) << 24);
				}
				return BitConverter.Int32BitsToFloat(value);
			}
	public virtual double ReadDouble()
			{
				long value;
				FillBuffer(8);
				if(BitConverter.IsLittleEndian)
				{
					value = ((long)(smallBuffer[0])) |
						    (((long)(smallBuffer[1])) << 8) |
						    (((long)(smallBuffer[2])) << 16) |
						    (((long)(smallBuffer[3])) << 24) |
						    (((long)(smallBuffer[4])) << 32) |
						    (((long)(smallBuffer[5])) << 40) |
						    (((long)(smallBuffer[6])) << 48) |
						    (((long)(smallBuffer[7])) << 56);
				}
				else
				{
					value = ((long)(smallBuffer[7])) |
						    (((long)(smallBuffer[6])) << 8) |
						    (((long)(smallBuffer[5])) << 16) |
						    (((long)(smallBuffer[4])) << 24) |
						    (((long)(smallBuffer[3])) << 32) |
						    (((long)(smallBuffer[2])) << 40) |
						    (((long)(smallBuffer[1])) << 48) |
						    (((long)(smallBuffer[0])) << 56);
				}
				return BitConverter.Int64BitsToDouble(value);
			}

	// Read Decimal values.
	public virtual Decimal ReadDecimal()
			{
				FillBuffer(16);
				int[] bits = new int [4];
				bits[0] = ((int)(smallBuffer[0])) |
					      (((int)(smallBuffer[1])) << 8) |
					      (((int)(smallBuffer[2])) << 16) |
					      (((int)(smallBuffer[3])) << 24);
				bits[1] = ((int)(smallBuffer[4])) |
					      (((int)(smallBuffer[5])) << 8) |
					      (((int)(smallBuffer[6])) << 16) |
					      (((int)(smallBuffer[7])) << 24);
				bits[2] = ((int)(smallBuffer[8])) |
					      (((int)(smallBuffer[9])) << 8) |
					      (((int)(smallBuffer[10])) << 16) |
					      (((int)(smallBuffer[11])) << 24);
				bits[3] = ((int)(smallBuffer[12])) |
					      (((int)(smallBuffer[13])) << 8) |
					      (((int)(smallBuffer[14])) << 16) |
					      (((int)(smallBuffer[15])) << 24);
				return new Decimal(bits);
			}

	// Read a buffer of bytes.
	public virtual byte[] ReadBytes(int count)
			{
				byte[] buffer;
				int result = 0;
				if(count < 0)
				{
					throw new ArgumentOutOfRangeException
						("count", _("ArgRange_NonNegative"));
				}

				buffer = new byte [count];

				do
				{
					int num2 = Read(buffer, result, count);
					if(num2 == 0)
					{
						break;
					}
					result += num2;
					count -= num2;
				}
				while(count > 0);

				if(result != buffer.Length)
				{
					byte[] newBuffer = new byte [result];
					Array.Copy(buffer, newBuffer, result);
					return newBuffer;
				}
				else
				{
					return buffer;
				}
			}

	// Read a buffer of characters.
	public virtual char[] ReadChars(int count)
			{
				byte[] buffer;
				int len;
				if(count < 0)
				{
					throw new ArgumentOutOfRangeException
						("count", _("ArgRange_NonNegative"));
				}
				buffer = ReadBytes(count);
				if(buffer.Length <= 0)
				{
					/* this exception fall through to the ReadString */
					throw new EndOfStreamException(_("IO_ReadEndOfStream"));
				}
				len=encoding.GetCharCount(buffer,0,buffer.Length);
				char[] newBuffer = new char[len];
				encoding.GetChars(buffer,0,buffer.Length,newBuffer,0);
				return newBuffer;
			}

	// Read a prefix-encoded string.
	public virtual String ReadString()
			{
				int len = Read7BitEncodedInt();
				char[] chars;
				if(len == 0)
				{
					return String.Empty;
				}
				else
				{
					chars = ReadChars(len);
					/* an exception is thrown from ReadChars for EndOfStream */
					return new String(chars);
				}
			}

	// Read an integer value that is encoded in a 7-bit format.
	protected int Read7BitEncodedInt()
			{
				int value = 0;
				int byteval;
				int shift = 0;
				while(((byteval = ReadByte()) & 0x80) != 0)
				{
					value |= ((byteval & 0x7F) << shift);
					shift += 7;
				}
				return (value | (byteval << shift));
			}

}; // class BinaryReader

#endif // !ECMA_COMPAT

}; // namespace System.IO
