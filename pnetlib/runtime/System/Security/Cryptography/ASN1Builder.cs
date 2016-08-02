/*
 * ASN1Builder.cs - Implementation of the
 *		"System.Security.Cryptography.ASN1Builder" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System.Security.Cryptography
{

#if CONFIG_CRYPTO

using System;
using System.Text;
using System.Collections;

// This class is used to help with building byte arrays that are
// stored in the Abstract Syntax Notation One (ASN.1) encoding.

internal class ASN1Builder
{
	// Internal state.
	protected ASN1Type  type;
	protected ArrayList list;

	// Constructors.
	public ASN1Builder() : this(ASN1Type.Sequence) {}
	public ASN1Builder(ASN1Type type)
			{
				this.type = type;
				this.list = new ArrayList();
			}
	protected ASN1Builder(ASN1Type type, int dummy)
			{
				this.type = type;
				this.list = null;
			}

	// Add a container to this builder.
	public ASN1Builder AddContainer(ASN1Type type)
			{
				ASN1Builder container = new ASN1Builder(type);
				list.Add(container);
				return container;
			}

	// Add a sequence to this builder.
	public ASN1Builder AddSequence(ASN1Type type)
			{
				return AddContainer(type);
			}
	public ASN1Builder AddSequence()
			{
				return AddContainer(ASN1Type.Sequence);
			}

	// Add a set to this builder.
	public ASN1Builder AddSet(ASN1Type type)
			{
				return AddContainer(type);
			}
	public ASN1Builder AddSet()
			{
				return AddContainer(ASN1Type.Set);
			}

	// Add a byte array field to this builder.
	public void AddByteArray(ASN1Type type, byte[] value)
			{
				list.Add(new ASN1ByteBuilder(type, value));
			}

	// Add a bit string to this builder.
	public void AddBitString(ASN1Type type, byte[] value)
			{
				list.Add(new ASN1BitStringBuilder(type, value));
			}
	public void AddBitString(byte[] value)
			{
				list.Add(new ASN1BitStringBuilder(ASN1Type.BitString, value));
			}
	public ASN1Builder AddBitStringContents()
			{
				return new ASN1BitStringContentsBuilder(ASN1Type.BitString);
			}

	// Add a string to this builder.
	public void AddString(ASN1Type type, String value)
			{
				list.Add(new ASN1ByteBuilder
					(type, Encoding.UTF8.GetBytes(value)));
			}
	public void AddString(String value)
			{
				AddString(ASN1Type.IA5String, value);
			}
	public void AddPrintableString(String value)
			{
				AddString(ASN1Type.PrintableString, value);
			}

	// Add a 32-bit integer to this builder.
	public void AddInt32(ASN1Type type, int value)
			{
				list.Add(new ASN1Int32Builder(type, value));
			}
	public void AddInt32(int value)
			{
				AddInt32(ASN1Type.Integer, value);
			}

	// Add a 64-bit integer to this builder.
	public void AddInt64(ASN1Type type, long value)
			{
				list.Add(new ASN1Int64Builder(type, value));
			}
	public void AddInt64(long value)
			{
				AddInt64(ASN1Type.Integer, value);
			}

	// Add a big integer to this builder.
	public void AddBigInt(ASN1Type type, byte[] value)
			{
				list.Add(new ASN1BigIntBuilder(type, value));
			}
	public void AddBigInt(byte[] value)
			{
				AddBigInt(ASN1Type.Integer, value);
			}

	// Add an object identifier to this builder.
	public void AddObjectIdentifier(ASN1Type type, byte[] value)
			{
				AddByteArray(type, value);
			}
	public void AddObjectIdentifier(byte[] value)
			{
				AddByteArray(ASN1Type.ObjectIdentifier, value);
			}

	// Add an octet string to this builder.
	public void AddOctetString(ASN1Type type, byte[] value)
			{
				AddByteArray(type, value);
			}
	public void AddOctetString(byte[] value)
			{
				AddByteArray(ASN1Type.OctetString, value);
			}

	// Add a UTCTime value to this builder.
	public void AddUTCTime(ASN1Type type, String value)
			{
				AddString(type, value);
			}
	public void AddUTCTime(String value)
			{
				AddString(ASN1Type.UTCTime, value);
			}

	// Add a null value to this builder.
	public void AddNull()
			{
				AddByteArray(ASN1Type.Null, new byte [0]);
			}

	// Convert this builder into a byte array.
	public byte[] ToByteArray()
			{
				int length = GetLength();
				byte[] result = new byte [length];
				Encode(result, 0);
				return result;
			}

	// Get the length of this builder when it is encoded using DER.
	protected virtual int GetLength()
			{
				int len = 0;
				foreach(ASN1Builder builder in list)
				{
					len += builder.GetLength();
				}
				return 1 + GetBytesForLength(len) + len;
			}

	// Encode this builder in a byte array as DER.  Returns the length.
	protected virtual int Encode(byte[] result, int offset)
			{
				int start = offset;
				int len = 0;
				foreach(ASN1Builder builder in list)
				{
					len += builder.GetLength();
				}
				result[offset++] = (byte)type;
				offset += EncodeLength(result, offset, len);
				foreach(ASN1Builder builder in list)
				{
					offset += builder.Encode(result, offset);
				}
				return offset - start;
			}

	// Get the number of bytes that are needed to store a length value.
	private static int GetBytesForLength(int length)
			{
				if(length < (1 << 7))
				{
					return 1;
				}
				else if(length < (1 << 14))
				{
					return 2;
				}
				else if(length < (1 << 21))
				{
					return 3;
				}
				else if(length < (1 << 28))
				{
					return 4;
				}
				else
				{
					return 5;
				}
			}

	// Encode a length value and return the number of bytes used.
	private static int EncodeLength(byte[] result, int offset, int length)
			{
				if(length < (1 << 7))
				{
					result[offset] = (byte)length;
					return 1;
				}
				else if(length < (1 << 14))
				{
					result[offset] = (byte)(0x80 | (length >> 7));
					result[offset + 1] = (byte)(length & 0x7F);
					return 2;
				}
				else if(length < (1 << 21))
				{
					result[offset] = (byte)(0x80 | (length >> 14));
					result[offset + 1] = (byte)((length >> 7) | 0x80);
					result[offset + 2] = (byte)(length & 0x7F);
					return 3;
				}
				else if(length < (1 << 28))
				{
					result[offset] = (byte)(0x80 | (length >> 21));
					result[offset + 1] = (byte)((length >> 14) | 0x80);
					result[offset + 2] = (byte)((length >> 7) | 0x80);
					result[offset + 3] = (byte)(length & 0x7F);
					return 4;
				}
				else
				{
					result[offset] = (byte)(0x80 | (length >> 28));
					result[offset + 1] = (byte)((length >> 21) | 0x80);
					result[offset + 2] = (byte)((length >> 14) | 0x80);
					result[offset + 3] = (byte)((length >> 7) | 0x80);
					result[offset + 4] = (byte)(length & 0x7F);
					return 5;
				}
			}

	// Builder node that stores a byte array.
	private class ASN1ByteBuilder : ASN1Builder
	{
		// Internal state.
		private byte[] value;

		// Constructor.
		public ASN1ByteBuilder(ASN1Type type, byte[] value)
			: base(type, 0)
			{
				this.value = value;
			}

		// Get the length of this builder when it is encoded using DER.
		protected override int GetLength()
			{
				return 1 + GetBytesForLength(value.Length) + value.Length;
			}

		// Encode this builder in a byte array as DER.  Returns the length.
		protected override int Encode(byte[] result, int offset)
			{
				int start = offset;
				result[offset++] = (byte)type;
				offset += EncodeLength(result, offset, value.Length);
				Array.Copy(value, 0, result, offset, value.Length);
				return (offset + value.Length - start);
			}

	}; // class ASN1ByteBuilder

	// Builder node that stores a 32-bit integer.
	private class ASN1Int32Builder : ASN1Builder
	{
		// Internal state.
		private int value;

		// Constructor.
		public ASN1Int32Builder(ASN1Type type, int value)
			: base(type, 0)
			{
				this.value = value;
			}

		// Get the length of this builder when it is encoded using DER.
		protected override int GetLength()
			{
				if(value >= -0x80 && value < 0x80)
				{
					return 3;
				}
				else if(value >= -0x8000 && value < 0x8000)
				{
					return 4;
				}
				else if(value >= -0x800000 && value < 0x800000)
				{
					return 5;
				}
				else
				{
					return 6;
				}
			}

		// Encode this builder in a byte array as DER.  Returns the length.
		protected override int Encode(byte[] result, int offset)
			{
				if(value >= -0x80 && value < 0x80)
				{
					result[offset] = (byte)type;
					result[offset + 1] = (byte)1;
					result[offset + 2] = (byte)value;
					return 3;
				}
				else if(value >= -0x8000 && value < 0x8000)
				{
					result[offset] = (byte)type;
					result[offset + 1] = (byte)2;
					result[offset + 2] = (byte)(value >> 8);
					result[offset + 3] = (byte)value;
					return 4;
				}
				else if(value >= -0x800000 && value < 0x800000)
				{
					result[offset] = (byte)type;
					result[offset + 1] = (byte)3;
					result[offset + 2] = (byte)(value >> 16);
					result[offset + 3] = (byte)(value >> 8);
					result[offset + 4] = (byte)value;
					return 5;
				}
				else
				{
					result[offset] = (byte)type;
					result[offset + 1] = (byte)4;
					result[offset + 2] = (byte)(value >> 24);
					result[offset + 3] = (byte)(value >> 16);
					result[offset + 4] = (byte)(value >> 8);
					result[offset + 5] = (byte)value;
					return 6;
				}
			}

	}; // class ASN1Int32Builder

	// Builder node that stores a 64-bit integer.
	private class ASN1Int64Builder : ASN1Builder
	{
		// Internal state.
		private long value;

		// Constructor.
		public ASN1Int64Builder(ASN1Type type, long value)
			: base(type, 0)
			{
				this.value = value;
			}

		// Get the length of this builder when it is encoded using DER.
		protected override int GetLength()
			{
				if(value >= -0x80L && value < 0x80L)
				{
					return 3;
				}
				else if(value >= -0x8000L && value < 0x8000L)
				{
					return 4;
				}
				else if(value >= -0x800000L && value < 0x800000L)
				{
					return 5;
				}
				else if(value >= -0x80000000L && value < 0x80000000L)
				{
					return 6;
				}
				else if(value >= -0x8000000000L && value < 0x8000000000L)
				{
					return 7;
				}
				else if(value >= -0x800000000000L && value < 0x800000000000L)
				{
					return 8;
				}
				else if(value >= -0x80000000000000L &&
						value < 0x80000000000000L)
				{
					return 9;
				}
				else
				{
					return 10;
				}
			}

		// Encode this builder in a byte array as DER.  Returns the length.
		protected override int Encode(byte[] result, int offset)
			{
				if(value >= -0x80L && value < 0x80L)
				{
					result[offset] = (byte)type;
					result[offset + 1] = (byte)1;
					result[offset + 2] = (byte)value;
					return 3;
				}
				else if(value >= -0x8000L && value < 0x8000L)
				{
					result[offset] = (byte)type;
					result[offset + 1] = (byte)2;
					result[offset + 2] = (byte)(value >> 8);
					result[offset + 3] = (byte)value;
					return 4;
				}
				else if(value >= -0x800000L && value < 0x800000L)
				{
					result[offset] = (byte)type;
					result[offset + 1] = (byte)3;
					result[offset + 2] = (byte)(value >> 16);
					result[offset + 3] = (byte)(value >> 8);
					result[offset + 4] = (byte)value;
					return 5;
				}
				else if(value >= -0x80000000L && value < 0x80000000L)
				{
					result[offset] = (byte)type;
					result[offset + 1] = (byte)4;
					result[offset + 2] = (byte)(value >> 24);
					result[offset + 3] = (byte)(value >> 16);
					result[offset + 4] = (byte)(value >> 8);
					result[offset + 5] = (byte)value;
					return 6;
				}
				else if(value >= -0x8000000000L && value < 0x8000000000L)
				{
					result[offset] = (byte)type;
					result[offset + 1] = (byte)5;
					result[offset + 2] = (byte)(value >> 32);
					result[offset + 3] = (byte)(value >> 24);
					result[offset + 4] = (byte)(value >> 16);
					result[offset + 5] = (byte)(value >> 8);
					result[offset + 6] = (byte)value;
					return 7;
				}
				else if(value >= -0x800000000000L && value < 0x800000000000L)
				{
					result[offset] = (byte)type;
					result[offset + 1] = (byte)6;
					result[offset + 2] = (byte)(value >> 40);
					result[offset + 3] = (byte)(value >> 32);
					result[offset + 4] = (byte)(value >> 24);
					result[offset + 5] = (byte)(value >> 16);
					result[offset + 6] = (byte)(value >> 8);
					result[offset + 7] = (byte)value;
					return 8;
				}
				else if(value >= -0x80000000000000L &&
						value < 0x80000000000000L)
				{
					result[offset] = (byte)type;
					result[offset + 1] = (byte)7;
					result[offset + 2] = (byte)(value >> 48);
					result[offset + 3] = (byte)(value >> 40);
					result[offset + 4] = (byte)(value >> 32);
					result[offset + 5] = (byte)(value >> 24);
					result[offset + 6] = (byte)(value >> 16);
					result[offset + 7] = (byte)(value >> 8);
					result[offset + 8] = (byte)value;
					return 9;
				}
				else
				{
					result[offset] = (byte)type;
					result[offset + 1] = (byte)8;
					result[offset + 2] = (byte)(value >> 56);
					result[offset + 3] = (byte)(value >> 48);
					result[offset + 4] = (byte)(value >> 40);
					result[offset + 5] = (byte)(value >> 32);
					result[offset + 6] = (byte)(value >> 24);
					result[offset + 7] = (byte)(value >> 16);
					result[offset + 8] = (byte)(value >> 8);
					result[offset + 9] = (byte)value;
					return 10;
				}
			}

	}; // class ASN1Int64Builder

	// Builder node that stores a big integer.
	private class ASN1BigIntBuilder : ASN1Builder
	{
		// Internal state.
		private byte[] value;

		// Constructor.
		public ASN1BigIntBuilder(ASN1Type type, byte[] value)
			: base(type, 0)
			{
				this.value = value;
			}

		// Get the length of this builder when it is encoded using DER.
		protected override int GetLength()
			{
				if((value[0] & 0x80) != 0)
				{
					// We need to add an extra leading zero byte.
					return 2 + GetBytesForLength(value.Length) + value.Length;
				}
				else
				{
					// No leading zero required.
					return 1 + GetBytesForLength(value.Length) + value.Length;
				}
			}

		// Encode this builder in a byte array as DER.  Returns the length.
		protected override int Encode(byte[] result, int offset)
			{
				int start = offset;
				result[offset++] = (byte)type;
				if((value[0] & 0x80) != 0)
				{
					// We need to add an extra leading zero byte.
					offset += EncodeLength(result, offset, value.Length + 1);
					result[offset] = (byte)0;
					Array.Copy(value, 0, result, offset + 1, value.Length);
					offset += value.Length + 1;
				}
				else
				{
					// No leading zero required.
					offset += EncodeLength(result, offset, value.Length);
					Array.Copy(value, 0, result, offset, value.Length);
					offset += value.Length;
				}
				return (offset - start);
			}

	}; // class ASN1BigIntBuilder

	// Builder node that stores a bit string.
	private class ASN1BitStringBuilder : ASN1Builder
	{
		// Internal state.
		private byte[] value;

		// Constructor.
		public ASN1BitStringBuilder(ASN1Type type, byte[] value)
			: base(type, 0)
			{
				this.value = value;
			}

		// Get the length of this builder when it is encoded using DER.
		protected override int GetLength()
			{
				return 2 + GetBytesForLength(value.Length) + value.Length;
			}

		// Encode this builder in a byte array as DER.  Returns the length.
		protected override int Encode(byte[] result, int offset)
			{
				int start = offset;
				result[offset++] = (byte)type;
				offset += EncodeLength(result, offset, value.Length + 1);
				result[offset] = (byte)0;
				Array.Copy(value, 0, result, offset + 1, value.Length);
				offset += value.Length + 1;
				return (offset - start);
			}

	}; // class ASN1BitStringBuilder

	// Builder node that stores a bit string in "contents" mode.
	private class ASN1BitStringContentsBuilder : ASN1Builder
	{
		// Constructor.
		public ASN1BitStringContentsBuilder(ASN1Type type) : base(type) {}

		// Get the length of this builder when it is encoded using DER.
		protected override int GetLength()
			{
				int len = 1;
				foreach(ASN1Builder builder in list)
				{
					len += builder.GetLength();
				}
				return 1 + GetBytesForLength(len) + len;
			}

		// Encode this builder in a byte array as DER.  Returns the length.
		protected override int Encode(byte[] result, int offset)
			{
				int start = offset;
				int len = 1;
				foreach(ASN1Builder builder in list)
				{
					len += builder.GetLength();
				}
				result[offset++] = (byte)type;
				offset += EncodeLength(result, offset, len);
				result[offset++] = (byte)0;
				foreach(ASN1Builder builder in list)
				{
					offset += builder.Encode(result, offset);
				}
				return offset - start;
			}

	}; // class ASN1BitStringContentsBuilder

}; // class ASN1Builder

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
