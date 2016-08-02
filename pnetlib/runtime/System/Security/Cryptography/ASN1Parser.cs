/*
 * ASN1Parser.cs - Implementation of the
 *		"System.Security.Cryptography.ASN1Parser" class.
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

#if CONFIG_CRYPTO || CONFIG_X509_CERTIFICATES

using System;
using System.Text;

// This class is used to help with parsing byte arrays that are
// stored in the Abstract Syntax Notation One (ASN.1) encoding.

internal sealed class ASN1Parser
{
	// Internal state.
	private byte[] buffer;
	private int offset;
	private int count;

	// Constructors.
	public ASN1Parser(byte[] buffer)
			{
				this.buffer = buffer;
				this.offset = 0;
				this.count  = buffer.Length;
			}
	public ASN1Parser(byte[] buffer, int offset, int count)
			{
				this.buffer = buffer;
				this.offset = offset;
				this.count  = count;
			}

	// Determine if we are at the end of the input data.
	public bool IsAtEnd()
			{
				return (count == 0);
			}

	// Get the tag value for the next ASN.1 field.
	public ASN1Type Type
			{
				get
				{
					if(count < 1)
					{
						// Insufficient octets for the tag value.
						throw new CryptographicException
							(_("Crypto_InvalidASN1"));
					}
					int byteval = buffer[offset];
					if((byteval & 0x1F) == 0x1F)
					{
						// High tag number forms are not supported.
						throw new CryptographicException
							(_("Crypto_InvalidASN1"));
					}
					return (ASN1Type)(int)(buffer[offset]);
				}
			}

	// Get the length of the next ASN.1 field.
	public int Length
			{
				get
				{
					int len, posn, ct, byteval;

					// We must have at least 2 bytes for the field.
					if(count < 2)
					{
						// Insufficient octets for the length value.
						throw new CryptographicException
							(_("Crypto_InvalidASN1"));
					}
					else if(buffer[offset] == 0x80)
					{
						// Indefinite-length fields are not supported.
						throw new CryptographicException
							(_("Crypto_InvalidASN1"));
					}

					// Parse the length value.
					len = 0;
					posn = offset + 1;
					ct = count - 1;
					do
					{
						if(ct <= 0)
						{
							// Insufficient octets for the length value.
							throw new CryptographicException
								(_("Crypto_InvalidASN1"));
						}
						byteval = buffer[posn++];
						len = (len << 7) | (byteval & 0x7F);
						--ct;
					}
					while((byteval & 0x80) != 0);
					return len;
				}
			}

	// Get the offset of the ASN.1 field's contents.  This
	// assumes that the "Length" property has already been
	// used to validate the header information.
	private int Offset
			{
				get
				{
					int posn, ct, byteval;
					posn = offset + 1;
					ct = count - 1;
					do
					{
						byteval = buffer[posn++];
						--ct;
					}
					while((byteval & 0x80) != 0);
					return (posn - offset);
				}
			}

	// Skip the next ASN.1 field.
	public void Skip()
			{
				int adjust = Length + Offset;
				offset += adjust;
				count -= adjust;
			}
	public void Skip(ASN1Type type)
			{
				if(Type != type)
				{
					// Not the expected type.
					throw new CryptographicException
						(_("Crypto_InvalidASN1"));
				}
				Skip();
			}

	// Determine if the next ASN.1 field has a specific type.
	public bool IsInteger()
			{
				return (Type == ASN1Type.Integer);
			}
	public bool IsBitString()
			{
				return (Type == ASN1Type.BitString);
			}
	public bool IsOctetString()
			{
				return (Type == ASN1Type.OctetString);
			}
	public bool IsNull()
			{
				return (Type == ASN1Type.Null);
			}
	public bool IsObjectIdentifier()
			{
				return (Type == ASN1Type.ObjectIdentifier);
			}
	public bool IsSequence()
			{
				return (Type == ASN1Type.Sequence);
			}
	public bool IsSet()
			{
				return (Type == ASN1Type.Set);
			}
	public bool IsString()
			{
				ASN1Type type = Type;
				return (type == ASN1Type.PrintableString ||
						type == ASN1Type.IA5String);
			}
	public bool IsUTCTime()
			{
				return (Type == ASN1Type.UTCTime);
			}

	// Get a parser for the contents of a field.
	public ASN1Parser GetContents(ASN1Type type, int adjust)
			{
				if(Type != type)
				{
					// Not the expected type.
					throw new CryptographicException
						(_("Crypto_InvalidASN1"));
				}
				int len = Length;
				int ofs = Offset;
				if(len < adjust)
				{
					// Contents are no long enough.
					throw new CryptographicException
						(_("Crypto_InvalidASN1"));
				}
				ASN1Parser parser =
					new ASN1Parser(buffer, offset + ofs + adjust, len - adjust);
				offset += len + ofs;
				count -= len + ofs;
				return parser;
			}
	public ASN1Parser GetContents(ASN1Type type)
			{
				return GetContents(type, 0);
			}

	// Get a byte array that contains the contents of a field.
	public byte[] GetContentsAsArray(ASN1Type type, int adjust)
			{
				if(Type != type)
				{
					// Not the expected type.
					throw new CryptographicException
						(_("Crypto_InvalidASN1"));
				}
				int len = Length;
				int ofs = Offset;
				if(len < adjust)
				{
					// Contents are no long enough.
					throw new CryptographicException
						(_("Crypto_InvalidASN1"));
				}
				byte[] result = new byte [len - adjust];
				Array.Copy(buffer, offset + ofs + adjust,
						   result, 0, len - adjust);
				offset += len + ofs;
				count -= len + ofs;
				return result;
			}
	public byte[] GetContentsAsArray(ASN1Type type)
			{
				return GetContentsAsArray(type, 0);
			}

	// Get a whole field, including the header, as a byte array.
	public byte[] GetWholeAsArray()
			{
				int len = Length;
				int ofs = Offset;
				byte[] result = new byte [len + ofs];
				Array.Copy(buffer, offset, result, 0, len + ofs);
				offset += len + ofs;
				count -= len + ofs;
				return result;
			}

	// Get a parser for the contents of a sequence or set.
	public ASN1Parser GetSequence()
			{
				return GetContents(ASN1Type.Sequence);
			}
	public ASN1Parser GetSet()
			{
				return GetContents(ASN1Type.Set);
			}

	// Extract a bit string.
	public byte[] GetBitString(ASN1Type type)
			{
				return GetContentsAsArray(type, 1);
			}
	public byte[] GetBitString()
			{
				return GetContentsAsArray(ASN1Type.BitString, 1);
			}
	public ASN1Parser GetBitStringContents()
			{
				return GetContents(ASN1Type.BitString, 1);
			}

	// Extract a string.
	private String GetStringChecked()
			{
				int len = Length;
				int ofs = Offset;
				String str;
				str = Encoding.UTF8.GetString(buffer, offset + ofs, len);
				offset += len + ofs;
				count -= len + ofs;
				return str;
			}
	public String GetString(ASN1Type type)
			{
				if(Type != type)
				{
					// Not the expected type.
					throw new CryptographicException
						(_("Crypto_InvalidASN1"));
				}
				return GetStringChecked();
			}
	public String GetString()
			{
				ASN1Type type = Type;
				if(type != ASN1Type.PrintableString &&
				   type != ASN1Type.IA5String)
				{
					// Not one of the standard string types.
					throw new CryptographicException
						(_("Crypto_InvalidASN1"));
				}
				return GetStringChecked();
			}

	// Extract a 32-bit integer value.
	public int GetInt32(ASN1Type type)
			{
				if(Type != type)
				{
					// Not the expected type.
					throw new CryptographicException
						(_("Crypto_InvalidASN1"));
				}
				int len = Length;
				int ofs = Offset;
				int posn = 0;
				int value, byteval;
				if(len < 1)
				{
					// Need at least 1 octet.
					throw new CryptographicException
						(_("Crypto_InvalidASN1"));
				}
				byteval = buffer[offset + ofs + posn];
				if((byteval & 0x80) != 0)
				{
					value = (-1 << 8) | byteval;
				}
				else
				{
					value = byteval;
				}
				++posn;
				while(posn < len)
				{
					byteval = buffer[offset + ofs + posn];
					value = (value << 8) | byteval;
					++posn;
				}
				offset += len + ofs;
				count -= len + ofs;
				return value;
			}
	public int GetInt32()
			{
				return GetInt32(ASN1Type.Integer);
			}

	// Extract a 64-bit integer value.
	public long GetInt64(ASN1Type type)
			{
				if(Type != type)
				{
					// Not the expected type.
					throw new CryptographicException
						(_("Crypto_InvalidASN1"));
				}
				int len = Length;
				int ofs = Offset;
				int posn = 0;
				long value;
				int byteval;
				if(len < 1)
				{
					// Need at least 1 octet.
					throw new CryptographicException
						(_("Crypto_InvalidASN1"));
				}
				byteval = buffer[offset + ofs + posn];
				if((byteval & 0x80) != 0)
				{
					value = (-1L << 8) | (long)(uint)byteval;
				}
				else
				{
					value = (long)byteval;
				}
				++posn;
				while(posn < len)
				{
					byteval = buffer[offset + ofs + posn];
					value = (value << 8) | (long)(uint)byteval;
					++posn;
				}
				offset += len + ofs;
				count -= len + ofs;
				return value;
			}
	public long GetInt64()
			{
				return GetInt64(ASN1Type.Integer);
			}

	// Extract a big integer value.
	public byte[] GetBigInt(ASN1Type type)
			{
				if(Type != type)
				{
					// Not the expected type.
					throw new CryptographicException
						(_("Crypto_InvalidASN1"));
				}
				int len = Length;
				int ofs = Offset;
				byte[] value;
				if(len > 1 && buffer[offset + ofs] == 0)
				{
					// Strip the leading zero byte.
					value = new byte [len - 1];
					Array.Copy(buffer, offset + ofs + 1, value, 0, len - 1);
				}
				else
				{
					// The first byte is non-zero.
					value = new byte [len];
					Array.Copy(buffer, offset + ofs, value, 0, len);
				}
				offset += len + ofs;
				count -= len + ofs;
				return value;
			}
	public byte[] GetBigInt()
			{
				return GetBigInt(ASN1Type.Integer);
			}

	// Extract an object identifier.
	public byte[] GetObjectIdentifier(ASN1Type type)
			{
				return GetContentsAsArray(type);
			}
	public byte[] GetObjectIdentifier()
			{
				return GetContentsAsArray(ASN1Type.ObjectIdentifier);
			}

	// Extract an octet string.
	public byte[] GetOctetString(ASN1Type type)
			{
				return GetContentsAsArray(type);
			}
	public byte[] GetOctetString()
			{
				return GetContentsAsArray(ASN1Type.OctetString);
			}

	// Get a UTC time value.
	public String GetUTCTime(ASN1Type type)
			{
				return GetString(type);
			}
	public String GetUTCTime()
			{
				return GetString(ASN1Type.UTCTime);
			}

	// Get a null value.
	public void GetNull()
			{
				if(Type != ASN1Type.Null)
				{
					throw new CryptographicException
						(_("Crypto_InvalidASN1"));
				}
				Skip();
			}

	// Check that we are at the end of the ASN.1 stream.
	public void AtEnd()
			{
				if(count != 0)
				{
					// There is trailing information that we didn't expect.
					throw new CryptographicException
						(_("Crypto_InvalidASN1"));
				}
			}

	// Determine if two object identifier buffers are equal.
	public static bool IsObjectID(byte[] o1, byte[] o2)
			{
				if(o1.Length != o2.Length)
				{
					return false;
				}
				for(int index = 0; index < o1.Length; ++index)
				{
					if(o1[index] != o2[index])
					{
						return false;
					}
				}
				return true;
			}

	// Convert user-defined tag numbers into ASN1Type values.
	public static ASN1Type Universal(int n)
			{
				return (ASN1Type)n;
			}
	public static ASN1Type Application(int n)
			{
				return (ASN1Type)(n | 0x40);
			}
	public static ASN1Type ContextSpecific(int n)
			{
				return (ASN1Type)(n | 0x80);
			}
	public static ASN1Type Private(int n)
			{
				return (ASN1Type)(n | 0xC0);
			}

}; // class ASN1Parser

#endif // CONFIG_CRYPTO || CONFIG_X509_CERTIFICATES

}; // namespace System.Security.Cryptography
