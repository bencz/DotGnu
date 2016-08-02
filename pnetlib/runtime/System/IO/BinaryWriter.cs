/*
 * BinaryWriter.cs - Implementation of the "System.IO.BinaryWriter" class.
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

using System;
using System.Text;

#if ECMA_COMPAT
internal
#else
public
#endif
class BinaryWriter : IDisposable
{
	// The null binary writer.
	public static readonly BinaryWriter Null = new BinaryWriter(Stream.Null);

	// Internal state.
	private Encoding encoding;
	private Encoder	 encoder;
	private byte[]   smallBuffer;
	private char[]   smallCharBuffer;
	private byte[]   largeBuffer;
	private bool	 disposed;

	// Accessible internal state.
	protected Stream OutStream;

	// Constructors.
	protected BinaryWriter()
			: this(Stream.Null, Encoding.UTF8)
			{
				// Nothing to do here
			}
	public BinaryWriter(Stream output)
			: this(output, Encoding.UTF8)
			{
				// Nothing to do here.
			}
	public BinaryWriter(Stream output, Encoding encoding)
			{
				if(output == null)
				{
					throw new ArgumentNullException("output");
				}
				if(encoding == null)
				{
					throw new ArgumentNullException("encoding");
				}
				if(!output.CanWrite)
				{
					throw new ArgumentException(_("IO_NotSupp_Write"));
				}
				OutStream = output;
				this.encoding = encoding;
				encoder = encoding.GetEncoder();
				smallBuffer = new byte [16];
				smallCharBuffer = new char [1];
				disposed = false;
			}

	// Get the base stream that underlies this binary writer.
	public virtual Stream BaseStream
			{
				get
				{
					return OutStream;
				}
			}

	// Close this stream.
	public virtual void Close()
			{
				if(!disposed)
				{
					OutStream.Close();
					Dispose(true);
				}
				else
				{
					throw new ObjectDisposedException("BinaryWriter");
				}
			}

	// Implement the IDisposable interface.
	void IDisposable.Dispose()
			{
				Dispose(true);
			}

	// Internal implementation of stream disposal.
	protected virtual void Dispose(bool disposing)
			{
				encoding = null;
				encoder = null;
				smallBuffer = null;
				smallCharBuffer = null;
				largeBuffer = null;
				disposed = true;
			}

	// Flush this writer.
	public virtual void Flush()
			{
				if(!disposed)
				{
					OutStream.Flush();
				}
			}

	// Write the contents of the small buffer to the output stream.
	private void WriteBuffer(int num)
			{
				if(disposed)
				{
					throw new ObjectDisposedException("BinaryWriter");
				}
				OutStream.Write(smallBuffer, 0, num);
			}

	// Seek to a new position within the underlying stream.
	// Note: the offset value should be "long", but Microsoft's
	// .NET Beta2 SDK defines this as "int".
	public virtual long Seek(int offset, SeekOrigin origin)
			{
				if(disposed)
				{
					throw new ObjectDisposedException("BinaryWriter");
				}
				return OutStream.Seek(offset, origin);
			}

	// Write a boolean value to the output.
	public virtual void Write(bool value)
			{
				smallBuffer[0] = (byte)(value ? 1 : 0);
				WriteBuffer(1);
			}

	// Write integer numeric values to the output.
	public virtual void Write(byte value)
			{
				smallBuffer[0] = value;
				WriteBuffer(1);
			}
	[CLSCompliant(false)]
	public virtual void Write(sbyte value)
			{
				smallBuffer[0] = (byte)value;
				WriteBuffer(1);
			}
	public virtual void Write(short value)
			{
				smallBuffer[0] = (byte)value;
				smallBuffer[1] = (byte)(value >> 8);
				WriteBuffer(2);
			}
	[CLSCompliant(false)]
	public virtual void Write(ushort value)
			{
				smallBuffer[0] = (byte)value;
				smallBuffer[1] = (byte)(value >> 8);
				WriteBuffer(2);
			}
	public virtual void Write(int value)
			{
				smallBuffer[0] = (byte)value;
				smallBuffer[1] = (byte)(value >> 8);
				smallBuffer[2] = (byte)(value >> 16);
				smallBuffer[3] = (byte)(value >> 24);
				WriteBuffer(4);
			}
	[CLSCompliant(false)]
	public virtual void Write(uint value)
			{
				smallBuffer[0] = (byte)value;
				smallBuffer[1] = (byte)(value >> 8);
				smallBuffer[2] = (byte)(value >> 16);
				smallBuffer[3] = (byte)(value >> 24);
				WriteBuffer(4);
			}
	public virtual void Write(long value)
			{
				smallBuffer[0] = (byte)value;
				smallBuffer[1] = (byte)(value >> 8);
				smallBuffer[2] = (byte)(value >> 16);
				smallBuffer[3] = (byte)(value >> 24);
				smallBuffer[4] = (byte)(value >> 32);
				smallBuffer[5] = (byte)(value >> 40);
				smallBuffer[6] = (byte)(value >> 48);
				smallBuffer[7] = (byte)(value >> 56);
				WriteBuffer(8);
			}
	[CLSCompliant(false)]
	public virtual void Write(ulong value)
			{
				smallBuffer[0] = (byte)value;
				smallBuffer[1] = (byte)(value >> 8);
				smallBuffer[2] = (byte)(value >> 16);
				smallBuffer[3] = (byte)(value >> 24);
				smallBuffer[4] = (byte)(value >> 32);
				smallBuffer[5] = (byte)(value >> 40);
				smallBuffer[6] = (byte)(value >> 48);
				smallBuffer[7] = (byte)(value >> 56);
				WriteBuffer(8);
			}

#if CONFIG_EXTENDED_NUMERICS

	// Write floating-point values to the output.
	public virtual void Write(float value)
			{
				int bits = BitConverter.FloatToInt32Bits(value);
				if(BitConverter.IsLittleEndian)
				{
					smallBuffer[0] = (byte)bits;
					smallBuffer[1] = (byte)(bits >> 8);
					smallBuffer[2] = (byte)(bits >> 16);
					smallBuffer[3] = (byte)(bits >> 24);
				}
				else
				{
					smallBuffer[3] = (byte)bits;
					smallBuffer[2] = (byte)(bits >> 8);
					smallBuffer[1] = (byte)(bits >> 16);
					smallBuffer[0] = (byte)(bits >> 24);
				}
				WriteBuffer(4);
			}
	public virtual void Write(double value)
			{
				long bits = BitConverter.DoubleToInt64Bits(value);
				if(BitConverter.IsLittleEndian)
				{
					smallBuffer[0] = (byte)bits;
					smallBuffer[1] = (byte)(bits >> 8);
					smallBuffer[2] = (byte)(bits >> 16);
					smallBuffer[3] = (byte)(bits >> 24);
					smallBuffer[4] = (byte)(bits >> 32);
					smallBuffer[5] = (byte)(bits >> 40);
					smallBuffer[6] = (byte)(bits >> 48);
					smallBuffer[7] = (byte)(bits >> 56);
				}
				else
				{
					smallBuffer[7] = (byte)bits;
					smallBuffer[6] = (byte)(bits >> 8);
					smallBuffer[5] = (byte)(bits >> 16);
					smallBuffer[4] = (byte)(bits >> 24);
					smallBuffer[3] = (byte)(bits >> 32);
					smallBuffer[2] = (byte)(bits >> 40);
					smallBuffer[1] = (byte)(bits >> 48);
					smallBuffer[0] = (byte)(bits >> 56);
				}
				WriteBuffer(8);
			}

	// Write a Decimal value to the output.
	public virtual void Write(Decimal value)
			{
				int[] bits = Decimal.GetBits(value);
				smallBuffer[0]  = (byte)(bits[0]);
				smallBuffer[1]  = (byte)(bits[0] >> 8);
				smallBuffer[2]  = (byte)(bits[0] >> 16);
				smallBuffer[3]  = (byte)(bits[0] >> 24);
				smallBuffer[4]  = (byte)(bits[1]);
				smallBuffer[5]  = (byte)(bits[1] >> 8);
				smallBuffer[6]  = (byte)(bits[1] >> 16);
				smallBuffer[7]  = (byte)(bits[1] >> 24);
				smallBuffer[8]  = (byte)(bits[2]);
				smallBuffer[9]  = (byte)(bits[2] >> 8);
				smallBuffer[10] = (byte)(bits[2] >> 16);
				smallBuffer[11] = (byte)(bits[2] >> 24);
				smallBuffer[12] = (byte)(bits[3]);
				smallBuffer[13] = (byte)(bits[3] >> 8);
				smallBuffer[14] = (byte)(bits[3] >> 16);
				smallBuffer[15] = (byte)(bits[3] >> 24);
				WriteBuffer(16);
			}

#endif // CONFIG_EXTENDED_NUMERICS

	// Write an array of bytes to the output.
	public virtual void Write(byte[] buffer)
			{
				if(disposed)
				{
					throw new ObjectDisposedException("BinaryWriter");
				}
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				OutStream.Write(buffer, 0, buffer.Length);
			}
	public virtual void Write(byte[] buffer, int index, int count)
			{
				if(disposed)
				{
					throw new ObjectDisposedException("BinaryWriter");
				}
				OutStream.Write(buffer, index, count);
			}

	// Write a character to the output.
	public virtual void Write(char ch)
			{
				smallCharBuffer[0] = ch;
				Write(smallCharBuffer, 0, 1);
			}

	// Write an array of characters to the output.
	public virtual void Write(char[] chars)
			{
				if(chars == null)
				{
					throw new ArgumentNullException("chars");
				}
				Write(chars, 0, chars.Length);
			}
	public virtual void Write(char[] chars, int index, int count)
			{
				int byteCount;
				byteCount = encoder.GetByteCount(chars, index, count, true);
				if(byteCount <= smallBuffer.Length)
				{
					encoder.GetBytes(chars, index, count,
									 smallBuffer, 0, true);
					WriteBuffer(byteCount);
				}
				else
				{
					if(disposed)
					{
						throw new ObjectDisposedException("BinaryWriter");
					}
					if(largeBuffer == null || byteCount > largeBuffer.Length)
					{
						largeBuffer = new byte [byteCount];
					}
					encoder.GetBytes(chars, index, count,
									 largeBuffer, 0, true);
					OutStream.Write(largeBuffer, 0, byteCount);
				}
			}

	// Write a string to the output.
	public virtual void Write(String value)
			{
				char[] chars;
				int byteCount;
				if(disposed)
				{
					throw new ObjectDisposedException("BinaryWriter");
				}
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				chars = value.ToCharArray();
				byteCount = encoder.GetByteCount(chars, 0, chars.Length, true);
				Write7BitEncodedInt(byteCount); /* byteCount no charCount */ 
				Write(chars, 0, chars.Length);
			}

	// Write a 7-bit encoded integer value to the output.
	protected void Write7BitEncodedInt(int value)
			{
				uint temp = (uint)value;
				while(temp >= 128)
				{
					Write((byte)(temp | 0x80));
					temp >>= 7;
				}
				Write((byte)temp);
			}
	internal void Write7BitEncoded(int value)
			{
				Write7BitEncodedInt(value);
			}

}; // class BinaryWriter

}; // namespace System.IO
