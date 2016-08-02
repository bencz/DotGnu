/*
 * CP51932.cs - Japanese (EUC) code page.
 *
 * Copyright (c) 2002, 2003  Southern Storm Software, Pty Ltd
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

namespace I18N.CJK
{

using System;
using System.Text;
using I18N.Common;

public unsafe class CP51932 : Encoding
{
	// Magic number used by Windows for the EUC code page.
	private const int EUCJP_CODE_PAGE = 51932;

	// Internal state.
	private JISConvert convert;

	// Constructor.
	public CP51932() : base(EUCJP_CODE_PAGE)
			{
				// Load the JIS conversion tables.
				convert = JISConvert.Convert;
			}

	// Get the number of bytes needed to encode a character buffer.
	public override int GetByteCount(char[] chars, int index, int count)
			{
				// Validate the parameters.
				if(chars == null)
				{
					throw new ArgumentNullException("chars");
				}
				if(index < 0 || index > chars.Length)
				{
					throw new ArgumentOutOfRangeException
						("index", Strings.GetString("ArgRange_Array"));
				}
				if(count < 0 || count > (chars.Length - index))
				{
					throw new ArgumentOutOfRangeException
						("count", Strings.GetString("ArgRange_Array"));
				}

				// Determine the length of the final output.
				int length = 0;
				int ch, value;
				byte *cjkToJis = convert.cjkToJis;
				byte *extraToJis = convert.extraToJis;
				while(count > 0)
				{
					ch = chars[index++];
					--count;
					++length;
					if(ch < 0x0080)
					{
						// Character maps to itself.
						continue;
					}
					else if(ch < 0x0100)
					{
						// Check for special Latin 1 characters that
						// can be mapped to double-byte code points.
						if(ch == 0x00A2 || ch == 0x00A3 || ch == 0x00A7 ||
						   ch == 0x00A8 || ch == 0x00AC || ch == 0x00B0 ||
						   ch == 0x00B1 || ch == 0x00B4 || ch == 0x00B6 ||
						   ch == 0x00D7 || ch == 0x00F7)
						{
							++length;
						}
					}
					else if(ch >= 0x0391 && ch <= 0x0451)
					{
						// Greek subset characters.
						++length;
					}
					else if(ch >= 0x2010 && ch <= 0x9FA5)
					{
						// This range contains the bulk of the CJK set.
						value = (ch - 0x2010) * 2;
						value = ((int)(cjkToJis[value])) |
								(((int)(cjkToJis[value + 1])) << 8);
						if(value >= 0x0100)
						{
							++length;
						}
					}
					else if(ch >= 0xFF01 && ch <= 0xFFEF)
					{
						// This range contains extra characters,
						// including half-width katakana.
						++length;
					}
				}

				// Return the length to the caller.
				return length;
			}

	// Get the bytes that result from encoding a character buffer.
	public override int GetBytes(char[] chars, int charIndex, int charCount,
								 byte[] bytes, int byteIndex)
			{
				// Validate the parameters.
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
						("charIndex", Strings.GetString("ArgRange_Array"));
				}
				if(charCount < 0 || charCount > (chars.Length - charIndex))
				{
					throw new ArgumentOutOfRangeException
						("charCount", Strings.GetString("ArgRange_Array"));
				}
				if(byteIndex < 0 || byteIndex > bytes.Length)
				{
					throw new ArgumentOutOfRangeException
						("byteIndex", Strings.GetString("ArgRange_Array"));
				}

				// Convert the characters into their byte form.
				int posn = byteIndex;
				int byteLength = bytes.Length;
				int ch, value;
				byte *cjkToJis = convert.cjkToJis;
				byte *greekToJis = convert.greekToJis;
				byte *extraToJis = convert.extraToJis;
				while(charCount > 0)
				{
					ch = chars[charIndex++];
					--charCount;
					if(posn >= byteLength)
					{
						throw new ArgumentException
							(Strings.GetString("Arg_InsufficientSpace"),
							 "bytes");
					}
					if(ch < 0x0080)
					{
						// Character maps to itself.
						bytes[posn++] = (byte)ch;
						continue;
					}
					else if(ch < 0x0100)
					{
						// Check for special Latin 1 characters that
						// can be mapped to double-byte code points.
						if(ch == 0x00A2 || ch == 0x00A3 || ch == 0x00A7 ||
						   ch == 0x00A8 || ch == 0x00AC || ch == 0x00B0 ||
						   ch == 0x00B1 || ch == 0x00B4 || ch == 0x00B6 ||
						   ch == 0x00D7 || ch == 0x00F7)
						{
							if((posn + 1) >= byteLength)
							{
								throw new ArgumentException
									(Strings.GetString
										("Arg_InsufficientSpace"), "bytes");
							}
							switch(ch)
							{
								case 0x00A2:
									bytes[posn++] = (byte)0xA1;
									bytes[posn++] = (byte)(0xA0 + 81);
									break;

								case 0x00A3:
									bytes[posn++] = (byte)0xA1;
									bytes[posn++] = (byte)(0xA0 + 82);
									break;

								case 0x00A7:
									bytes[posn++] = (byte)0xA1;
									bytes[posn++] = (byte)(0xA0 + 88);
									break;

								case 0x00A8:
									bytes[posn++] = (byte)0xA1;
									bytes[posn++] = (byte)(0xA0 + 37);
									break;

								case 0x00AC:
									bytes[posn++] = (byte)0xA2;
									bytes[posn++] = (byte)(0xA0 + 44);
									break;

								case 0x00B0:
									bytes[posn++] = (byte)0xA1;
									bytes[posn++] = (byte)(0xA0 + 75);
									break;

								case 0x00B1:
									bytes[posn++] = (byte)0xA1;
									bytes[posn++] = (byte)(0xA0 + 62);
									break;

								case 0x00B4:
									bytes[posn++] = (byte)0xA1;
									bytes[posn++] = (byte)(0xA0 + 76);
									break;

								case 0x00B6:
									bytes[posn++] = (byte)0xA2;
									bytes[posn++] = (byte)(0xA0 + 89);
									break;

								case 0x00D7:
									bytes[posn++] = (byte)0xA1;
									bytes[posn++] = (byte)(0xA0 + 63);
									break;

								case 0x00F7:
									bytes[posn++] = (byte)0xA1;
									bytes[posn++] = (byte)(0xA0 + 64);
									break;
							}
						}
						else if(ch == 0x00A5)
						{
							// Yen sign.
							bytes[posn++] = (byte)0x5C;
						}
						else
						{
							// Invalid character.
							bytes[posn++] = (byte)'?';
						}
						continue;
					}
					else if(ch >= 0x0391 && ch <= 0x0451)
					{
						// Greek subset characters.
						value = (ch - 0x0391) * 2;
						value = ((int)(greekToJis[value])) |
								(((int)(greekToJis[value + 1])) << 8);
					}
					else if(ch >= 0x2010 && ch <= 0x9FA5)
					{
						// This range contains the bulk of the CJK set.
						value = (ch - 0x2010) * 2;
						value = ((int)(cjkToJis[value])) |
								(((int)(cjkToJis[value + 1])) << 8);
					}
					else if(ch >= 0xFF01 && ch <= 0xFFEF)
					{
						// This range contains extra characters,
						// including half-width katakana.
						value = (ch - 0xFF01) * 2;
						value = ((int)(extraToJis[value])) |
								(((int)(extraToJis[value + 1])) << 8);
					}
					else
					{
						// Invalid character.
						value = 0;
					}
					if(value == 0)
					{
						bytes[posn++] = (byte)'?';
					}
					else if((posn + 1) >= byteLength)
					{
						throw new ArgumentException
							(Strings.GetString("Arg_InsufficientSpace"),
							 "bytes");
					}
					else if(value < 0x0100)
					{
						// Half-width katakana.
						bytes[posn++] = (byte)0x8E;
						bytes[posn++] = (byte)value;
					}
					else if(value < 0x8000)
					{
						// JIS X 0208 character.
						value -= 0x0100;
						ch = (value / 94);
						bytes[posn++] = (byte)(ch + 0xA1);
						ch = (value % 94);
						bytes[posn++] = (byte)(ch + 0xA1);
					}
					else
					{
						// JIS X 0212 character.
						if((posn + 2) >= byteLength)
						{
							throw new ArgumentException
								(Strings.GetString("Arg_InsufficientSpace"),
							 	"bytes");
						}
						bytes[posn++] = (byte)0x8F;
						value -= 0x8000;
						ch = (value / 94);
						bytes[posn++] = (byte)(ch + 0xA1);
						ch = (value % 94);
						bytes[posn++] = (byte)(ch + 0xA1);
					}
				}

				// Return the final length to the caller.
				return posn - byteIndex;
			}

	// Get the number of characters needed to decode a byte buffer.
	public override int GetCharCount(byte[] bytes, int index, int count)
			{
				// Validate the parameters.
				if(bytes == null)
				{
					throw new ArgumentNullException("bytes");
				}
				if(index < 0 || index > bytes.Length)
				{
					throw new ArgumentOutOfRangeException
						("index", Strings.GetString("ArgRange_Array"));
				}
				if(count < 0 || count > (bytes.Length - index))
				{
					throw new ArgumentOutOfRangeException
						("count", Strings.GetString("ArgRange_Array"));
				}

				// Determine the total length of the converted string.
				int length = 0;
				int byteval;
				while(count > 0)
				{
					byteval = bytes[index++];
					--count;
					++length;
					if(byteval <= 0x80)
					{
						// Ordinary ASCII/Latin1 character, or the
						// single-byte Yen or overline signs.
						continue;
					}
					else if(byteval >= 0xA1 && byteval <= 0xFE)
					{
						// Two-byte JIS X 0208 character.
						if(count == 0)
						{
							--length;
							continue;
						}
						--count;
						++index;
					}
					else if(byteval == 0x8E)
					{
						// Two-byte half-width katakana.
						if(count == 0)
						{
							--length;
							continue;
						}
						--count;
						++index;
					}
					else if(byteval == 0x8F)
					{
						// Three-byte JIS X 0212 character.
						if(count <= 1)
						{
							--length;
							count = 0;
							continue;
						}
						count -= 2;
						index += 2;
					}
					else
					{
						// Invalid first byte: map to NUL.
						continue;
					}
				}

				// Return the total length.
				return length;
			}

	// Get the characters that result from decoding a byte buffer.
	public override int GetChars(byte[] bytes, int byteIndex, int byteCount,
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
						("byteIndex", Strings.GetString("ArgRange_Array"));
				}
				if(byteCount < 0 || byteCount > (bytes.Length - byteIndex))
				{
					throw new ArgumentOutOfRangeException
						("byteCount", Strings.GetString("ArgRange_Array"));
				}
				if(charIndex < 0 || charIndex > chars.Length)
				{
					throw new ArgumentOutOfRangeException
						("charIndex", Strings.GetString("ArgRange_Array"));
				}

				// Determine the total length of the converted string.
				int charLength = chars.Length;
				int posn = charIndex;
				int length = 0;
				int byteval, value;
				byte *table = convert.jisx0208ToUnicode;
				byte *table2 = convert.jisx0212ToUnicode;
				while(byteCount > 0)
				{
					byteval = bytes[byteIndex++];
					--byteCount;
					++length;
					if(posn >= charLength)
					{
						throw new ArgumentException
							(Strings.GetString("Arg_InsufficientSpace"),
							 "chars");
					}
					if(byteval <= 0x80)
					{
						// Ordinary ASCII/Latin1 character.
						chars[posn++] = (char)byteval;
					}
					else if(byteval >= 0xA1 && byteval <= 0xFE)
					{
						// Two-byte JIS X 0208 character.
						value = (byteval - 0xA1) * 94;
						if(byteCount > 0)
						{
							byteval = bytes[byteIndex++];
							--byteCount;
						}
						else
						{
							break;
						}
						if(byteval >= 0xA1 && byteval <= 0xFE)
						{
							value += (byteval - 0xA1);
							value *= 2;
							value = ((int)(table[value])) |
									(((int)(table[value + 1])) << 8);
							if(value != 0)
							{
								chars[posn++] = (char)value;
							}
							else
							{
								chars[posn++] = '\0';
							}
						}
						else
						{
							// Invalid second byte.
							chars[posn++] = '\0';
						}
					}
					else if(byteval == 0x8E)
					{
						// Two-byte half-width katakana.
						if(byteCount > 0)
						{
							byteval = bytes[byteIndex++];
							--byteCount;
						}
						else
						{
							break;
						}
						if(byteval >= 0xA1 && byteval <= 0xFE)
						{
							chars[posn++] = (char)(byteval - 0xA1 + 0xFF61);
						}
						else
						{
							// Invalid second byte.
							chars[posn++] = '\0';
						}
					}
					else if(byteval == 0x8F)
					{
						// Three-byte JIS X 0212 character.
						if(byteCount > 0)
						{
							byteval = bytes[byteIndex++];
							--byteCount;
						}
						else
						{
							break;
						}
						if(byteval >= 0xA1 && byteval <= 0xFE)
						{
							value = (byteval - 0xA1) * 94;
							if(byteCount > 0)
							{
								byteval = bytes[byteIndex++];
								--byteCount;
							}
							else
							{
								break;
							}
							if(byteval >= 0xA1 && byteval <= 0xFE)
							{
								value += (byteval - 0xA1);
								value *= 2;
								value = ((int)(table2[value])) |
										(((int)(table2[value + 1])) << 8);
								if(value != 0)
								{
									chars[posn++] = (char)value;
								}
								else
								{
									chars[posn++] = '\0';
								}
							}
							else
							{
								// Invalid third byte.
								chars[posn++] = '\0';
							}
						}
						else
						{
							// Invalid second byte.
							chars[posn++] = '\0';
						}
					}
					else if(byteval == 0xA0)
					{
						chars[posn++] = '\uF8F0';
					}
					else if(byteval == 0xFF)
					{
						chars[posn++] = '\uF8F3';
					}
					else
					{
						// Invalid first byte.
						chars[posn++] = '\0';
					}
				}

				// Return the total length.
				return posn - charIndex;
			}

	// Get the maximum number of bytes needed to encode a
	// specified number of characters.
	public override int GetMaxByteCount(int charCount)
			{
				if(charCount < 0)
				{
					throw new ArgumentOutOfRangeException
						("charCount",
						 Strings.GetString("ArgRange_NonNegative"));
				}
				return charCount * 3;
			}

	// Get the maximum number of characters needed to decode a
	// specified number of bytes.
	public override int GetMaxCharCount(int byteCount)
			{
				if(byteCount < 0)
				{
					throw new ArgumentOutOfRangeException
						("byteCount",
						 Strings.GetString("ArgRange_NonNegative"));
				}
				return byteCount;
			}

	// Get a decoder that handles a rolling Shift-JIS state.
	public override Decoder GetDecoder()
			{
				return new CP51932Decoder(convert);
			}

#if !ECMA_COMPAT

	// Get the mail body name for this encoding.
	public override String BodyName
			{
				get
				{
					return "euc-jp";
				}
			}

	// Get the human-readable name for this encoding.
	public override String EncodingName
			{
				get
				{
					return "Japanese (EUC)";
				}
			}

	// Get the mail agent header name for this encoding.
	public override String HeaderName
			{
				get
				{
					return "euc-jp";
				}
			}

	// Determine if this encoding can be displayed in a Web browser.
	public override bool IsBrowserDisplay
			{
				get
				{
					return true;
				}
			}

	// Determine if this encoding can be saved from a Web browser.
	public override bool IsBrowserSave
			{
				get
				{
					return true;
				}
			}

	// Determine if this encoding can be displayed in a mail/news agent.
	public override bool IsMailNewsDisplay
			{
				get
				{
					return true;
				}
			}

	// Determine if this encoding can be saved from a mail/news agent.
	public override bool IsMailNewsSave
			{
				get
				{
					return true;
				}
			}

	// Get the IANA-preferred Web name for this encoding.
	public override String WebName
			{
				get
				{
					return "euc-jp";
				}
			}

	// Get the Windows code page represented by this object.
	public override int WindowsCodePage
			{
				get
				{
					return CP932.SHIFTJIS_CODE_PAGE;
				}
			}

#endif // !ECMA_COMPAT

	// Decoder that handles a rolling EUC-JP state.
	private sealed class CP51932Decoder : Decoder
	{
		private JISConvert convert;
		private int lastByte1;
		private int lastByte2;

		// Constructor.
		public CP51932Decoder(JISConvert convert)
				{
					this.convert = convert;
					this.lastByte1 = 0;
					this.lastByte2 = 0;
				}

		// Override inherited methods.
		public override int GetCharCount(byte[] bytes, int index, int count)
				{
					// Validate the parameters.
					if(bytes == null)
					{
						throw new ArgumentNullException("bytes");
					}
					if(index < 0 || index > bytes.Length)
					{
						throw new ArgumentOutOfRangeException
							("index", Strings.GetString("ArgRange_Array"));
					}
					if(count < 0 || count > (bytes.Length - index))
					{
						throw new ArgumentOutOfRangeException
							("count", Strings.GetString("ArgRange_Array"));
					}

					// Determine the total length of the converted string.
					int length = 0;
					int byteval;
					int last1 = lastByte1;
					int last2 = lastByte2;
					while(count > 0)
					{
						byteval = bytes[index++];
						--count;
						if(last1 == 0)
						{
							if((byteval >= 0xA1 && byteval <= 0xFE) ||
							   byteval == 0x8E || byteval == 0x8F)
							{
								// First byte in a double-byte sequence.
								last1 = byteval;
							}
							else
							{
								++length;
							}
						}
						else if(last1 == 0x8F)
						{
							if(last2 == 0)
							{
								// Second byte in a triple-byte sequence.
								if(byteval >= 0xA1 && byteval <= 0xFE)
								{
									last2 = byteval;
								}
								else
								{
									last1 = 0;
								}
							}
							else
							{
								// Third byte in a triple-byte sequence.
								last1 = 0;
								last2 = 0;
								++length;
							}
						}
						else
						{
							// Second byte in a double-byte sequence.
							last1 = 0;
							last2 = 0;
							++length;
						}
					}
	
					// Return the total length.
					return length;
				}
		public override int GetChars(byte[] bytes, int byteIndex,
									 int byteCount, char[] chars,
									 int charIndex)
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
							("byteIndex", Strings.GetString("ArgRange_Array"));
					}
					if(byteCount < 0 || byteCount > (bytes.Length - byteIndex))
					{
						throw new ArgumentOutOfRangeException
							("byteCount", Strings.GetString("ArgRange_Array"));
					}
					if(charIndex < 0 || charIndex > chars.Length)
					{
						throw new ArgumentOutOfRangeException
							("charIndex", Strings.GetString("ArgRange_Array"));
					}

					// Decode the bytes in the buffer.
					int posn = charIndex;
					int charLength = chars.Length;
					int byteval, value;
					int last1 = lastByte1;
					int last2 = lastByte2;
					byte *table = convert.jisx0208ToUnicode;
					byte *table2 = convert.jisx0212ToUnicode;
					while(byteCount > 0)
					{
						byteval = bytes[byteIndex++];
						--byteCount;
						if(last1 == 0)
						{
							if(posn >= charLength)
							{
								throw new ArgumentException
									(Strings.GetString
										("Arg_InsufficientSpace"), "chars");
							}
							if((byteval >= 0xA1 && byteval <= 0xFE) ||
							   byteval == 0x8E || byteval == 0x8F)
							{
								// First byte in a double-byte sequence.
								last1 = byteval;
							}
							else if(byteval <= 0x80)
							{
								// Ordinary ASCII/Latin1 character.
								chars[posn++] = (char)byteval;
							}
							else if(byteval == 0xA0)
							{
								chars[posn++] = '\uF8F0';
							}
							else if(byteval == 0xFF)
							{
								chars[posn++] = '\uF8F3';
							}
							else
							{
								// Invalid first byte.
								chars[posn++] = '\0';
							}
						}
						else if(last1 == 0x8F)
						{
							if(last2 == 0)
							{
								// Second byte in a triple-byte sequence.
								if(byteval != 0)
								{
									last2 = byteval;
								}
								else
								{
									last2 = 1;
								}
							}
							else
							{
								// Third byte in a triple-byte sequence.
								if(last2 < 0xA1 || last2 > 0xFE)
								{
									// Invalid second byte.
									chars[posn++] = '\0';
									last1 = 0;
									last2 = 0;
									continue;
								}
								if(byteval < 0xA1 || byteval > 0xFE)
								{
									// Invalid third byte.
									chars[posn++] = '\0';
									last1 = 0;
									last2 = 0;
									continue;
								}
								value = (last2 - 0xA1) * 94 + (byteval - 0xA1);
								value *= 2;
								value = ((int)(table2[value])) |
										(((int)(table2[value + 1])) << 8);
								if(value != 0)
								{
									chars[posn++] = (char)value;
								}
								else
								{
									chars[posn++] = '\0';
								}
								last1 = 0;
								last2 = 0;
							}
						}
						else if(last1 == 0x8E)
						{
							// Second byte in a half-width katakana sequence.
							if(byteval >= 0xA1 && byteval <= 0xDF)
							{
								chars[posn++] = (char)(byteval - 0xA1 + 0xFF61);
							}
							else
							{
								// Invalid second byte.
								chars[posn++] = '\0';
							}
							last1 = 0;
						}
						else
						{
							// Second byte in a double-byte sequence.
							if(byteval < 0xA1 || byteval > 0xFE)
							{
								// Invalid second byte.
								chars[posn++] = '\0';
								last1 = 0;
								continue;
							}
							value = (last1 - 0xA1) * 94 + (byteval - 0xA1);
							value *= 2;
							value = ((int)(table[value])) |
									(((int)(table[value + 1])) << 8);
							if(value != 0)
							{
								chars[posn++] = (char)value;
							}
							else
							{
								chars[posn++] = '\0';
							}
							last1 = 0;
						}
					}
					lastByte1 = last1;
					lastByte2 = last2;

					// Return the final length to the caller.
					return posn - charIndex;
				}

	} // class CP51932Decoder

}; // class CP51932

public class ENCeuc_jp : CP51932
{
	public ENCeuc_jp() : base() {}

}; // class ENCeuc_jp

}; // namespace I18N.CJK
