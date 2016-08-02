/*
 * UTF32Encoding.cs - Implementation of the "System.Text.UTF32Encoding" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

namespace System.Text
{

#if !ECMA_COMPAT && CONFIG_FRAMEWORK_1_2
public class UTF32Encoding : Encoding
#else
internal class UTF32Encoding : Encoding
#endif
{
	// Code page numbers for UTF32.
	internal const int UTF32_CODE_PAGE = 12000;
	internal const int UTF32_BIG_ENDIAN_CODE_PAGE = 12001;

	// Byte ordering for UCS-4 streams.
	private enum ByteOrder
	{
		Order_1234,
		Order_4321,
		Order_3412,
		Order_2143

	}; // enum ByteOrder

	// Internal state.
	private ByteOrder byteOrder;
	private bool byteOrderMark;
	private bool throwOnInvalidChars;

	// Constructors.
	public UTF32Encoding() : this(false, true, false) {}
	public UTF32Encoding(bool bigEndian, bool byteOrderMark)
			: this(bigEndian, byteOrderMark, false) {}
	public UTF32Encoding(bool bigEndian, bool byteOrderMark,
						 bool throwOnInvalidChars)
			: base(bigEndian ? UTF32_BIG_ENDIAN_CODE_PAGE : UTF32_CODE_PAGE)
			{
				if(bigEndian)
				{
					byteOrder = ByteOrder.Order_1234;
				}
				else
				{
					byteOrder = ByteOrder.Order_4321;
				}
				this.byteOrderMark = byteOrderMark;
				this.throwOnInvalidChars = throwOnInvalidChars;
			}

	// Get the number of bytes needed to encode a character buffer.
	public override int GetByteCount(char[] chars, int index, int count)
			{
				if(chars == null)
				{
					throw new ArgumentNullException("chars");
				}
				if(index < 0 || index > chars.Length)
				{
					throw new ArgumentOutOfRangeException
						("index", _("ArgRange_Array"));
				}
				if(count < 0 || count > (chars.Length - index))
				{
					throw new ArgumentOutOfRangeException
						("count", _("ArgRange_Array"));
				}
				int bytes = 0;
				char ch;
				while(count > 0)
				{
					ch = chars[index++];
					--count;
					if(ch >= '\uD800' && ch <= '\uDBFF')
					{
						// Possibly the start of a surrogate pair.
						if(count > 0)
						{
							ch = chars[index];
							if(ch >= '\uDC00' && ch <= '\uDFFF')
							{
								++index;
								--count;
							}
						}
					}
					bytes += 4;
				}
				return bytes;
			}

	// Get the bytes that result from encoding a character buffer.
	public override int GetBytes(char[] chars, int charIndex, int charCount,
								 byte[] bytes, int byteIndex)
			{
				if(chars == null)
				{
					throw new ArgumentNullException("chars");
				}
				if(bytes == null)
				{
					throw new ArgumentNullException("bytes");
				}
				if(charIndex < 0 || charIndex > chars.Length)
				{
					throw new ArgumentOutOfRangeException
						("charIndex", _("ArgRange_Array"));
				}
				if(charCount < 0 || charCount > (chars.Length - charIndex))
				{
					throw new ArgumentOutOfRangeException
						("charCount", _("ArgRange_Array"));
				}
				if(byteIndex < 0 || byteIndex > bytes.Length)
				{
					throw new ArgumentOutOfRangeException
						("byteIndex", _("ArgRange_Array"));
				}
				int posn = byteIndex;
				int left = bytes.Length - byteIndex;
				uint pair;
				char ch;
				switch(byteOrder)
				{
					case ByteOrder.Order_1234:
					{
						while(charCount-- > 0)
						{
							if(left < 4)
							{
								throw new ArgumentException
									(_("Arg_InsufficientSpace"));
							}
							ch = chars[charIndex++];
							if(ch >= '\uD800' && ch <= '\uDBFF' &&
							   charCount > 0 &&
							   chars[charIndex] >= '\uDC00' &&
							   chars[charIndex] <= '\uDFFF')
							{
								pair = ((uint)(ch - '\uD800')) << 10;
								ch = chars[charIndex++];
								--charCount;
								pair += ((uint)(ch - '\uDC00')) + (uint)0x10000;
								bytes[posn++] = (byte)(pair >> 24);
								bytes[posn++] = (byte)(pair >> 16);
								bytes[posn++] = (byte)(pair >> 8);
								bytes[posn++] = (byte)pair;
							}
							else
							{
								bytes[posn++] = (byte)0;
								bytes[posn++] = (byte)0;
								bytes[posn++] = (byte)(ch >> 8);
								bytes[posn++] = (byte)ch;
							}
							left -= 4;
						}
					}
					break;

					case ByteOrder.Order_4321:
					{
						while(charCount-- > 0)
						{
							if(left < 4)
							{
								throw new ArgumentException
									(_("Arg_InsufficientSpace"));
							}
							ch = chars[charIndex++];
							if(ch >= '\uD800' && ch <= '\uDBFF' &&
							   charCount > 0 &&
							   chars[charIndex] >= '\uDC00' &&
							   chars[charIndex] <= '\uDFFF')
							{
								pair = ((uint)(ch - '\uD800')) << 10;
								ch = chars[charIndex++];
								--charCount;
								pair += ((uint)(ch - '\uDC00')) + (uint)0x10000;
								bytes[posn++] = (byte)pair;
								bytes[posn++] = (byte)(pair >> 8);
								bytes[posn++] = (byte)(pair >> 16);
								bytes[posn++] = (byte)(pair >> 24);
							}
							else
							{
								bytes[posn++] = (byte)ch;
								bytes[posn++] = (byte)(ch >> 8);
								bytes[posn++] = (byte)0;
								bytes[posn++] = (byte)0;
							}
							left -= 4;
						}
					}
					break;

					case ByteOrder.Order_3412:
					{
						while(charCount-- > 0)
						{
							if(left < 4)
							{
								throw new ArgumentException
									(_("Arg_InsufficientSpace"));
							}
							ch = chars[charIndex++];
							if(ch >= '\uD800' && ch <= '\uDBFF' &&
							   charCount > 0 &&
							   chars[charIndex] >= '\uDC00' &&
							   chars[charIndex] <= '\uDFFF')
							{
								pair = ((uint)(ch - '\uD800')) << 10;
								ch = chars[charIndex++];
								--charCount;
								pair += ((uint)(ch - '\uDC00')) + (uint)0x10000;
								bytes[posn++] = (byte)(pair >> 8);
								bytes[posn++] = (byte)pair;
								bytes[posn++] = (byte)(pair >> 24);
								bytes[posn++] = (byte)(pair >> 16);
							}
							else
							{
								bytes[posn++] = (byte)(ch >> 8);
								bytes[posn++] = (byte)ch;
								bytes[posn++] = (byte)0;
								bytes[posn++] = (byte)0;
							}
							left -= 4;
						}
					}
					break;

					case ByteOrder.Order_2143:
					{
						while(charCount-- > 0)
						{
							if(left < 4)
							{
								throw new ArgumentException
									(_("Arg_InsufficientSpace"));
							}
							ch = chars[charIndex++];
							if(ch >= '\uD800' && ch <= '\uDBFF' &&
							   charCount > 0 &&
							   chars[charIndex] >= '\uDC00' &&
							   chars[charIndex] <= '\uDFFF')
							{
								pair = ((uint)(ch - '\uD800')) << 10;
								ch = chars[charIndex++];
								--charCount;
								pair += ((uint)(ch - '\uDC00')) + (uint)0x10000;
								bytes[posn++] = (byte)(pair >> 16);
								bytes[posn++] = (byte)(pair >> 24);
								bytes[posn++] = (byte)pair;
								bytes[posn++] = (byte)(pair >> 8);
							}
							else
							{
								bytes[posn++] = (byte)0;
								bytes[posn++] = (byte)0;
								bytes[posn++] = (byte)ch;
								bytes[posn++] = (byte)(ch >> 8);
							}
							left -= 4;
						}
					}
					break;
				}
				return posn - byteIndex;
			}

	// Read a 4-byte character from an array.
	private static uint ReadChar(ByteOrder byteOrder, byte[] array, int index)
			{
				switch(byteOrder)
				{
					case ByteOrder.Order_1234:
					{
						return (((uint)(array[index])) << 24) |
						       (((uint)(array[index + 1])) << 16) |
						       (((uint)(array[index + 2])) << 8) |
						        ((uint)(array[index + 3]));
					}
					// Not reached.

					case ByteOrder.Order_4321:
					{
						return (((uint)(array[index + 3])) << 24) |
						       (((uint)(array[index + 2])) << 16) |
						       (((uint)(array[index + 1])) << 8) |
						        ((uint)(array[index]));
					}
					// Not reached.

					case ByteOrder.Order_3412:
					{
						return (((uint)(array[index + 2])) << 24) |
						       (((uint)(array[index + 3])) << 16) |
						       (((uint)(array[index])) << 8) |
						        ((uint)(array[index + 1]));
					}
					// Not reached.

					case ByteOrder.Order_2143:
					{
						return (((uint)(array[index + 1])) << 24) |
						       (((uint)(array[index])) << 16) |
						       (((uint)(array[index + 3])) << 8) |
						        ((uint)(array[index + 2]));
					}
					// Not reached.
				}
				return 0;
			}

	// Internal version of "GetCharCount".
	private static int InternalGetCharCount
				(ByteOrder byteOrder, byte[] leftOver, int leftOverLen,
				 byte[] bytes, int index, int count)
			{
				// Validate the parameters.
				if(bytes == null)
				{
					throw new ArgumentNullException("bytes");
				}
				if(index < 0 || index > bytes.Length)
				{
					throw new ArgumentOutOfRangeException
						("index", _("ArgRange_Array"));
				}
				if(count < 0 || count > (bytes.Length - index))
				{
					throw new ArgumentOutOfRangeException
						("count", _("ArgRange_Array"));
				}

				// Handle the left-over buffer.
				int chars = 0;
				uint value;
				if(leftOverLen > 0)
				{
					if((leftOverLen + count) < 4)
					{
						return 0;
					}
					Array.Copy(bytes, index, leftOver, leftOverLen,
							   4 - leftOverLen);
					value = ReadChar(byteOrder, leftOver, 0);
					if(value != (uint)0x0000FEFF)
					{
						if(value > (uint)0x0000FFFF)
						{
							chars += 2;
						}
						else
						{
							++chars;
						}
					}
					index += 4 - leftOverLen;
					count -= 4 - leftOverLen;
				}

				// Handle the main buffer contents.
				while(count >= 4)
				{
					value = ReadChar(byteOrder, bytes, index);
					if(value != (uint)0x0000FEFF)
					{
						if(value > (uint)0x0000FFFF)
						{
							chars += 2;
						}
						else
						{
							++chars;
						}
					}
					index += 4;
					count -= 4;
				}
				return chars;
			}

	// Get the number of characters needed to decode a byte buffer.
	public override int GetCharCount(byte[] bytes, int index, int count)
			{
				return InternalGetCharCount(byteOrder, null, 0,
											bytes, index, count);
			}

	// Internal version of "GetChars".
	private static int InternalGetChars(ByteOrder byteOrder,
										byte[] leftOver, ref int leftOverLen,
										byte[] bytes, int byteIndex,
										int byteCount,
								 		char[] chars, int charIndex)
			{
				// Validate the parameters.
				if(bytes == null)
				{
					throw new ArgumentNullException("bytes");
				}
				if(chars == null)
				{
					throw new ArgumentNullException("chars");
				}
				if(byteIndex < 0 || byteIndex > bytes.Length)
				{
					throw new ArgumentOutOfRangeException
						("byteIndex", _("ArgRange_Array"));
				}
				if(byteCount < 0 || byteCount > (bytes.Length - byteIndex))
				{
					throw new ArgumentOutOfRangeException
						("byteCount", _("ArgRange_Array"));
				}
				if(charIndex < 0 || charIndex > chars.Length)
				{
					throw new ArgumentOutOfRangeException
						("charIndex", _("ArgRange_Array"));
				}

				// Handle the left-over buffer.
				uint value;
				int start = charIndex;
				int charCount = chars.Length - charIndex;
				if(leftOverLen > 0)
				{
					if((leftOverLen + byteCount) < 4)
					{
						Array.Copy(bytes, byteIndex, leftOver,
								   leftOverLen, byteCount);
						leftOverLen += byteCount;
						return 0;
					}
					Array.Copy(bytes, byteIndex, leftOver, leftOverLen,
							   4 - leftOverLen);
					value = ReadChar(byteOrder, leftOver, 0);
					if(value != (uint)0x0000FEFF)
					{
						if(value > (uint)0x0000FFFF)
						{
							if(charCount < 2)
							{
								throw new ArgumentException
									(_("Arg_InsufficientSpace"));
							}
							value -= (uint)0x10000;
							chars[charIndex++] =
								(char)((value >> 10) + (uint)0xD800);
							chars[charIndex++] =
								(char)((value & (uint)0x03FF) + (uint)0xDC00);
							charCount -= 2;
						}
						else
						{
							if(charCount < 1)
							{
								throw new ArgumentException
									(_("Arg_InsufficientSpace"));
							}
							chars[charIndex++] = (char)value;
							--charCount;
						}
					}
					byteIndex += 4 - leftOverLen;
					byteCount -= 4 - leftOverLen;
					leftOverLen = 0;
				}

				// Handle the main buffer contents.
				while(byteCount >= 4)
				{
					value = ReadChar(byteOrder, bytes, byteIndex);
					if(value != (uint)0x0000FEFF)
					{
						if(value > (uint)0x0000FFFF)
						{
							if(charCount < 2)
							{
								throw new ArgumentException
									(_("Arg_InsufficientSpace"));
							}
							value -= (uint)0x10000;
							chars[charIndex++] =
								(char)((value >> 10) + (uint)0xD800);
							chars[charIndex++] =
								(char)((value & (uint)0x03FF) + (uint)0xDC00);
							charCount -= 2;
						}
						else
						{
							if(charCount < 1)
							{
								throw new ArgumentException
									(_("Arg_InsufficientSpace"));
							}
							chars[charIndex++] = (char)value;
							--charCount;
						}
					}
					byteIndex += 4;
					byteCount -= 4;
				}
				if(byteCount > 0)
				{
					Array.Copy(bytes, byteIndex, leftOver, 0, byteCount);
					leftOverLen = byteCount;
				}
				return charIndex - start;
			}

	// Get the characters that result from decoding a byte buffer.
	public override int GetChars(byte[] bytes, int byteIndex, int byteCount,
								 char[] chars, int charIndex)
			{
				int leftOverLen = 0;
				return InternalGetChars(byteOrder, null, ref leftOverLen,
									    bytes, byteIndex, byteCount,
										chars, charIndex);
			}

	// Decode a buffer of bytes into a string.
	public override String GetString(byte[] bytes, int index, int count)
			{
				return base.GetString(bytes, index, count);
			}

	// Get the maximum number of bytes needed to encode a
	// specified number of characters.
	public override int GetMaxByteCount(int charCount)
			{
				if(charCount < 0)
				{
					throw new ArgumentOutOfRangeException
						("charCount", _("ArgRange_NonNegative"));
				}
				return charCount * 4;
			}

	// Get the maximum number of characters needed to decode a
	// specified number of bytes.
	public override int GetMaxCharCount(int byteCount)
			{
				if(byteCount < 0)
				{
					throw new ArgumentOutOfRangeException
						("byteCount", _("ArgRange_NonNegative"));
				}
				// We may need to account for surrogate pairs,
				// so use / 2 rather than / 4.
				return byteCount / 2;
			}

	// Get a UCS4-specific encoder that is attached to this instance.
	public override Encoder GetEncoder()
			{
				return base.GetEncoder();
			}

	// Get a UCS4-specific decoder that is attached to this instance.
	public override Decoder GetDecoder()
			{
				return new UCS4Decoder(byteOrder);
			}

	// Get the UCS4 preamble.
	public override byte[] GetPreamble()
			{
				if(!byteOrderMark)
				{
					return new byte [0];
				}
				byte[] preamble = new byte[4];
				switch(byteOrder)
				{
					case ByteOrder.Order_1234:
					{
						preamble[0] = (byte)0x00;
						preamble[1] = (byte)0x00;
						preamble[2] = (byte)0xFE;
						preamble[3] = (byte)0xFF;
					}
					break;

					case ByteOrder.Order_4321:
					{
						preamble[0] = (byte)0xFF;
						preamble[1] = (byte)0xFE;
						preamble[2] = (byte)0x00;
						preamble[3] = (byte)0x00;
					}
					break;

					case ByteOrder.Order_3412:
					{
						preamble[0] = (byte)0xFE;
						preamble[1] = (byte)0xFF;
						preamble[2] = (byte)0x00;
						preamble[3] = (byte)0x00;
					}
					break;

					case ByteOrder.Order_2143:
					{
						preamble[0] = (byte)0x00;
						preamble[1] = (byte)0x00;
						preamble[2] = (byte)0xFF;
						preamble[3] = (byte)0xFE;
					}
					break;
				}
				return preamble;
			}

	// Determine if this object is equal to another.
	public override bool Equals(Object value)
			{
				UTF32Encoding enc = (value as UTF32Encoding);
				if(enc != null)
				{
					return (byteOrder == enc.byteOrder);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				return base.GetHashCode();
			}

#if !ECMA_COMPAT

	// Get the mail body name for this encoding.
	internal override String InternalBodyName
			{
				get
				{
					switch(byteOrder)
					{
						case ByteOrder.Order_1234:
							return "ucs-4-be";

						case ByteOrder.Order_4321:
							return "ucs-4";

						case ByteOrder.Order_3412:
							return "ucs-4-3412";

						case ByteOrder.Order_2143:
							return "ucs-4-2143";
					}
					return null;
				}
			}

	// Get the human-readable name for this encoding.
	internal override String InternalEncodingName
			{
				get
				{
					switch(byteOrder)
					{
						case ByteOrder.Order_1234:
							return "Unicode (UCS-4 Big-Endian)";

						case ByteOrder.Order_4321:
							return "Unicode (UCS-4)";

						case ByteOrder.Order_3412:
							return "Unicode (UCS-4 Order 3412)";

						case ByteOrder.Order_2143:
							return "Unicode (UCS-4 Order 2143)";
					}
					return null;
				}
			}

	// Get the mail agent header name for this encoding.
	internal override String InternalHeaderName
			{
				get
				{
					return BodyName;
				}
			}

	// Determine if this encoding can be displayed in a Web browser.
	internal override bool InternalIsBrowserDisplay
			{
				get
				{
					return false;
				}
			}

	// Determine if this encoding can be saved from a Web browser.
	internal override bool InternalIsBrowserSave
			{
				get
				{
					return false;
				}
			}

	// Determine if this encoding can be displayed in a mail/news agent.
	internal override bool InternalIsMailNewsDisplay
			{
				get
				{
					return false;
				}
			}

	// Determine if this encoding can be saved from a mail/news agent.
	internal override bool InternalIsMailNewsSave
			{
				get
				{
					return false;
				}
			}

	// Get the IANA-preferred Web name for this encoding.
	internal override String InternalWebName
			{
				get
				{
					return BodyName;
				}
			}

	// Get the Windows code page represented by this object.
	internal override int InternalWindowsCodePage
			{
				get
				{
					// Use UCS-2 as the underlying "real" code page.
					if(byteOrder == ByteOrder.Order_1234)
					{
						return 1201;
					}
					else
					{
						return 1200;
					}
				}
			}

#endif // !ECMA_COMPAT

	// UCS4 decoder implementation.
	private sealed class UCS4Decoder : Decoder
	{
		// Internal state.
		private ByteOrder byteOrder;
		private byte[] buffer;
		private int bufferUsed;

		// Constructor.
		public UCS4Decoder(ByteOrder order)
				{
					byteOrder = order;
					buffer = new byte [4];
					bufferUsed = 0;
				}

		// Override inherited methods.
		public override int GetCharCount(byte[] bytes, int index, int count)
				{
					return InternalGetCharCount(byteOrder, buffer, bufferUsed,
												bytes, index, count);
				}
		public override int GetChars(byte[] bytes, int byteIndex,
									 int byteCount, char[] chars,
									 int charIndex)
				{
					return InternalGetChars(byteOrder, buffer,
											ref bufferUsed,
											bytes, byteIndex, byteCount,
											chars, charIndex);
				}

	} // class UCS4Decoder

}; // class UTF32Encoding

}; // System.Text
