/*
 * CP20261.cs - Implementation of the T.61 encoding.
 *
 * Copyright (c) 2003  Southern Storm Software, Pty Ltd
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

namespace I18N.Rare
{

using System;
using System.Text;
using I18N.Common;

public class CP20261 : Encoding
{
	// Constructor.
	public CP20261() : base(20261) {}

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
						("index", Strings.GetString("ArgRange_Array"));
				}
				if(count < 0 || count > (chars.Length - index))
				{
					throw new ArgumentOutOfRangeException
						("count", Strings.GetString("ArgRange_Array"));
				}
				int length = 0;
				int value;
				char ch;
				while(count > 0)
				{
					ch = chars[index++];
					--count;
					if(ch < 0x0200)
					{
						value = uniToT61A[ch];
					}
					else if(ch >= 0x1E00 && ch <= 0x1EFF)
					{
						value = uniToT61B[ch - 0x1E00];
					}
					else if(ch >= 0xF8D0 && ch <= 0xF8FF)
					{
						value = uniToT61C[ch - 0xF8D0];
					}
					else if(ch == 0x02C7 || ch == 0x02D8 || ch == 0x02D9 ||
							ch == 0x02DA || ch == 0x02DB || ch == 0x02DD)
					{
						value = 0xC100;
					}
					else
					{
						value = 0;
					}
					if(value < 0x0100)
					{
						++length;
					}
					else
					{
						length += 2;
					}
				}
				return length;
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

				// Convert the characters into bytes.
				int posn = byteIndex;
				int byteLength = bytes.Length;
				int value;
				char ch;
				while(charCount > 0)
				{
					ch = chars[charIndex++];
					--charCount;
					if(ch < 0x0200)
					{
						value = uniToT61A[ch];
					}
					else if(ch >= 0x1E00 && ch <= 0x1EFF)
					{
						value = uniToT61B[ch - 0x1E00];
					}
					else if(ch >= 0xF8D0 && ch <= 0xF8FF)
					{
						value = uniToT61C[ch - 0xF8D0];
					}
					else
					{
						switch((int)ch)
						{
							case 0x02C7:	value = 0xCF20; break;
							case 0x02D8:	value = 0xC620; break;
							case 0x02D9:	value = 0xC720; break;
							case 0x02DA:	value = 0xCA20; break;
							case 0x02DB:	value = 0xCE20; break;
							case 0x02DD:	value = 0xCD20; break;
							case 0x2126:	value = 0xE0; break;
							default:		value = '?'; break;
						}
					}
					if(value < 0x0100)
					{
						if(posn >= byteLength)
						{
							throw new ArgumentException
								(Strings.GetString("Arg_InsufficientSpace"),
								 "bytes");
						}
						bytes[posn++] = (byte)value;
					}
					else
					{
						if((posn + 1) >= byteLength)
						{
							throw new ArgumentException
								(Strings.GetString("Arg_InsufficientSpace"),
								 "bytes");
						}
						bytes[posn++] = (byte)(value >> 8);
						bytes[posn++] = (byte)value;
					}
				}
				return posn - byteIndex;
			}

	// Get the number of characters needed to decode a byte buffer.
	public override int GetCharCount(byte[] bytes, int index, int count)
			{
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
				int length = 0;
				int ch;
				while(count > 0)
				{
					ch = bytes[index++];
					--count;
					++length;
					if(ch >= 0xC1 && ch <= 0xCF)
					{
						if(count > 0)
						{
							++index;
							--count;
						}
					}
				}
				return length;
			}

	// Get the characters that result from decoding a byte buffer.
	public override int GetChars(byte[] bytes, int byteIndex, int byteCount,
								 char[] chars, int charIndex)
			{
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
				int posn = charIndex;
				int charLength = chars.Length;
				int ch, ch2;
				while(byteCount > 0)
				{
					ch = bytes[byteIndex++];
					--byteCount;
					if(posn >= charLength)
					{
						throw new ArgumentException
							(Strings.GetString("Arg_InsufficientSpace"),
							 "chars");
					}
					if(ch < 0xC1 || ch > 0xCF)
					{
						chars[posn++] = t61ToUni[ch];
					}
					else
					{
						if(byteCount > 0)
						{
							ch2 = bytes[byteIndex++];
							--byteCount;
						}
						else
						{
							ch2 = 0;
						}
						chars[posn++] = t61ToUniC[ch - 0xC1][ch2];
					}
				}
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
				return charCount * 2;
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

	// Get a decoder that handles a rolling multi-byte state.
	public override Decoder GetDecoder()
			{
				return new CP20261Decoder();
			}

#if !ECMA_COMPAT

	// Get the mail body name for this encoding.
	public override String BodyName
			{
				get
				{
					return "cp20261";
				}
			}

	// Get the human-readable name for this encoding.
	public override String EncodingName
			{
				get
				{
					return "T.61";
				}
			}

	// Get the mail agent header name for this encoding.
	public override String HeaderName
			{
				get
				{
					return "x-cp20261";
				}
			}

	// Determine if this encoding can be displayed in a Web browser.
	public override bool IsBrowserDisplay
			{
				get
				{
					return false;
				}
			}

	// Determine if this encoding can be saved from a Web browser.
	public override bool IsBrowserSave
			{
				get
				{
					return false;
				}
			}

	// Determine if this encoding can be displayed in a mail/news agent.
	public override bool IsMailNewsDisplay
			{
				get
				{
					return false;
				}
			}

	// Determine if this encoding can be saved from a mail/news agent.
	public override bool IsMailNewsSave
			{
				get
				{
					return false;
				}
			}

	// Get the IANA-preferred Web name for this encoding.
	public override String WebName
			{
				get
				{
					return "x-cp20261";
				}
			}

	// Get the Windows code page represented by this object.
	public override int WindowsCodePage
			{
				get
				{
					return 1252;
				}
			}

#endif // !ECMA_COMPAT

	// Decoder that handles a rolling T.61 state.
	private sealed class CP20261Decoder : Decoder
	{
		private int lastByte;

		// Constructor.
		public CP20261Decoder()
				{
					this.lastByte = 0;
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
					int last = lastByte;
					while(count > 0)
					{
						byteval = bytes[index++];
						--count;
						if(last == 0)
						{
							if(byteval >= 0xC1 && byteval <= 0xCF)
							{
								// First byte in a double-byte sequence.
								last = byteval;
							}
							else
							{
								++length;
							}
						}
						else
						{
							// Second byte in a double-byte sequence.
							last = 0;
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
					int byteval;
					char value;
					int last = lastByte;
					while(byteCount > 0)
					{
						byteval = bytes[byteIndex++];
						--byteCount;
						if(last == 0)
						{
							if(byteval < 0xC1 || byteval > 0xCF)
							{
								if(posn >= charLength)
								{
									throw new ArgumentException
										(Strings.GetString
											("Arg_InsufficientSpace"), "chars");
								}
								chars[posn++] = t61ToUni[byteval];
							}
							else
							{
								// First byte in a double-byte sequence.
								last = byteval;
							}
						}
						else
						{
							// Second byte in a double-byte sequence.
							value = t61ToUniC[last - 0xC1][byteval];
							if(posn >= charLength)
							{
								throw new ArgumentException
									(Strings.GetString
										("Arg_InsufficientSpace"), "chars");
							}
							chars[posn++] = value;
							last = 0;
						}
					}
					lastByte = last;

					// Return the final length to the caller.
					return posn - charIndex;
				}

	} // class CP20261Decoder

	// Conversion tables.
	private static readonly ushort[] uniToT61A = { // \u0000 - \u01FF
		0x0000, 0x0001, 0x0002, 0x0003, 0x0004, 0x0005, 0x0006, 0x0007, 
		0x0008, 0x0009, 0x000A, 0x000B, 0x000C, 0x000D, 0x000E, 0x000F, 
		0x0010, 0x0011, 0x0012, 0x0013, 0x0014, 0x0015, 0x0016, 0x0017, 
		0x0018, 0x0019, 0x001A, 0x001B, 0x001C, 0x001D, 0x001E, 0x001F, 
		0x0020, 0x0021, 0x0022, 0x00A6, 0x00A4, 0x0025, 0x0026, 0x0027, 
		0x0028, 0x0029, 0x002A, 0x002B, 0x002C, 0x002D, 0x002E, 0x002F, 
		0x0030, 0x0031, 0x0032, 0x0033, 0x0034, 0x0035, 0x0036, 0x0037, 
		0x0038, 0x0039, 0x003A, 0x003B, 0x003C, 0x003D, 0x003E, 0x003F, 
		0x0040, 0x0041, 0x0042, 0x0043, 0x0044, 0x0045, 0x0046, 0x0047, 
		0x0048, 0x0049, 0x004A, 0x004B, 0x004C, 0x004D, 0x004E, 0x004F, 
		0x0050, 0x0051, 0x0052, 0x0053, 0x0054, 0x0055, 0x0056, 0x0057, 
		0x0058, 0x0059, 0x005A, 0x005B, 0x003F, 0x005D, 0xC320, 0x005F, 
		0xC120, 0x0061, 0x0062, 0x0063, 0x0064, 0x0065, 0x0066, 0x0067, 
		0x0068, 0x0069, 0x006A, 0x006B, 0x006C, 0x006D, 0x006E, 0x006F, 
		0x0070, 0x0071, 0x0072, 0x0073, 0x0074, 0x0075, 0x0076, 0x0077, 
		0x0078, 0x0079, 0x007A, 0x003F, 0x007C, 0x003F, 0xC420, 0x003F, 
		0x0080, 0x0081, 0x0082, 0x0083, 0x0084, 0x0085, 0x0086, 0x0087, 
		0x0088, 0x0089, 0x008A, 0x008B, 0x008C, 0x008D, 0x008E, 0x008F, 
		0x0090, 0x0091, 0x0092, 0x0093, 0x0094, 0x0095, 0x0096, 0x0097, 
		0x0098, 0x0099, 0x009A, 0x009B, 0x009C, 0x009D, 0x009E, 0x009F, 
		0x003F, 0x00A1, 0x00A2, 0x00A3, 0x00A8, 0x00A5, 0x003F, 0x00A7, 
		0xC820, 0x003F, 0x00E3, 0x00AB, 0x003F, 0x003F, 0x003F, 0xC520, 
		0x00B0, 0x00B1, 0x00B2, 0x00B3, 0xC220, 0x00B5, 0x00B6, 0x00B7, 
		0xCB20, 0x003F, 0x00EB, 0x00BB, 0x00BC, 0x00BD, 0x00BE, 0x00BF, 
		0xC141, 0xC241, 0xC341, 0xC441, 0xC841, 0xCA41, 0x00E1, 0xCB43, 
		0xC145, 0xC245, 0xC345, 0xC845, 0xC149, 0xC249, 0xC349, 0xC849, 
		0x00E2, 0xC44E, 0xC14F, 0xC24F, 0xC34F, 0xC44F, 0xC84F, 0x00B4, 
		0x00E9, 0xC155, 0xC255, 0xC355, 0xC855, 0xC259, 0x00EC, 0x00FB, 
		0xC161, 0xC261, 0xC361, 0xC461, 0xC861, 0xCA61, 0x00F1, 0xCB63, 
		0xC165, 0xC265, 0xC365, 0xC865, 0xC169, 0xC269, 0xC369, 0xC869, 
		0x00F3, 0xC46E, 0xC16F, 0xC26F, 0xC36F, 0xC46F, 0xC86F, 0x00B8, 
		0x00F9, 0xC175, 0xC275, 0xC375, 0xC875, 0xC279, 0x00FC, 0xC879, 
		0xC541, 0xC561, 0xC641, 0xC661, 0xCE41, 0xCE61, 0xC243, 0xC263, 
		0xC343, 0xC363, 0xC743, 0xC763, 0xCF43, 0xCF63, 0xCF44, 0xCF64, 
		0x003F, 0x00F2, 0xC545, 0xC565, 0xC645, 0xC665, 0xC745, 0xC765, 
		0xCE45, 0xCE65, 0xCF45, 0xCF65, 0xC347, 0xC367, 0xC647, 0xC667, 
		0xC747, 0xC767, 0xCB47, 0xCB67, 0xC348, 0xC368, 0x00E4, 0x00F4, 
		0xC449, 0xC469, 0xC549, 0xC569, 0xC649, 0xC669, 0xCE49, 0xCE69, 
		0xC749, 0x00F5, 0x00E6, 0x00F6, 0xC34A, 0xC36A, 0xCB4B, 0xCB6B, 
		0x00F0, 0xC24C, 0xC26C, 0xCB4C, 0xCB6C, 0xCF4C, 0xCF6C, 0x00E7, 
		0x00F7, 0x00E8, 0x00F8, 0xC24E, 0xC26E, 0xCB4E, 0xCB6E, 0xCF4E, 
		0xCF6E, 0x00EF, 0x00EE, 0x00FE, 0xC54F, 0xC56F, 0xC64F, 0xC66F, 
		0xCD4F, 0xCD6F, 0x00EA, 0x00FA, 0xC252, 0xC272, 0xCB52, 0xCB72, 
		0xCF52, 0xCF72, 0xC253, 0xC273, 0xC353, 0xC373, 0xCB53, 0xCB73, 
		0xCF53, 0xCF73, 0xCB54, 0xCB74, 0xCF54, 0xCF74, 0x00ED, 0x00FD, 
		0xC455, 0xC475, 0xC555, 0xC575, 0xC655, 0xC675, 0xCA55, 0xCA75, 
		0xCD55, 0xCD75, 0xCE55, 0xCE75, 0xC357, 0xC377, 0xC359, 0xC379, 
		0xC859, 0xC25A, 0xC27A, 0xC75A, 0xC77A, 0xCF5A, 0xCF7A, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0xCF41, 0xCF61, 0xCF49, 
		0xCF69, 0xCF4F, 0xCF6F, 0xCF55, 0xCF75, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0xC5E1, 0xC5F1, 0x003F, 0x003F, 0xCF47, 0xCF67, 
		0xCF4B, 0xCF6B, 0xCE4F, 0xCE6F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0xCF6A, 0x003F, 0x003F, 0x003F, 0xC247, 0xC267, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
	};
	private static readonly ushort[] uniToT61B = { // \u1E00 - \u1EFF
		0x003F, 0x003F, 0xC742, 0xC762, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0xC744, 0xC764, 0x003F, 0x003F, 0x003F, 0x003F, 
		0xCB44, 0xCB64, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0xC746, 0xC766, 
		0xC547, 0xC567, 0xC748, 0xC768, 0x003F, 0x003F, 0xC848, 0xC868, 
		0xCB48, 0xCB68, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0xC24B, 0xC26B, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0xC24D, 0xC26D, 
		0xC74D, 0xC76D, 0x003F, 0x003F, 0xC74E, 0xC76E, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0xC250, 0xC270, 0xC750, 0xC770, 
		0xC752, 0xC772, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0xC753, 0xC773, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0xC754, 0xC774, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0xC456, 0xC476, 0x003F, 0x003F, 
		0xC157, 0xC177, 0xC257, 0xC277, 0xC857, 0xC877, 0xC757, 0xC777, 
		0x003F, 0x003F, 0xC758, 0xC778, 0xC858, 0xC878, 0xC759, 0xC779, 
		0xC35A, 0xC37A, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0xC874, 
		0xCA77, 0xCA79, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0xC445, 0xC465, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0xC159, 0xC179, 0x003F, 0x003F, 0x003F, 0x003F, 
		0xC459, 0xC479, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
	};
	private static readonly ushort[] uniToT61C = { // \uF8D0 - \uF8FF
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 
		0x003F, 0x003F, 0x003F, 0x003F, 0x003F, 0x005C, 0x005E, 0x0060, 
		0x007B, 0x00A0, 0x00A9, 0x00AA, 0x00AC, 0x00AD, 0x00AE, 0x00AF, 
		0x00B9, 0x00BA, 0x00E5, 0x00C0, 0x00D0, 0x00D1, 0x00D2, 0x00D3, 
		0x00D4, 0x00D5, 0x00D6, 0x00D7, 0x00D8, 0x00D9, 0x00DA, 0x00DB, 
		0x00DC, 0x00DD, 0x00DE, 0x00DF, 0x007D, 0x007E, 0x007F, 0x00FF, 
	};
	private static readonly char[] t61ToUni = { // 0x00 - 0xFF
		'\u0000', '\u0001', '\u0002', '\u0003', '\u0004', '\u0005', 
		'\u0006', '\u0007', '\u0008', '\u0009', '\u000A', '\u000B', 
		'\u000C', '\u000D', '\u000E', '\u000F', '\u0010', '\u0011', 
		'\u0012', '\u0013', '\u0014', '\u0015', '\u0016', '\u0017', 
		'\u0018', '\u0019', '\u001A', '\u001B', '\u001C', '\u001D', 
		'\u001E', '\u001F', '\u0020', '\u0021', '\u0022', '\u0023', 
		'\u00A4', '\u0025', '\u0026', '\u0027', '\u0028', '\u0029', 
		'\u002A', '\u002B', '\u002C', '\u002D', '\u002E', '\u002F', 
		'\u0030', '\u0031', '\u0032', '\u0033', '\u0034', '\u0035', 
		'\u0036', '\u0037', '\u0038', '\u0039', '\u003A', '\u003B', 
		'\u003C', '\u003D', '\u003E', '\u003F', '\u0040', '\u0041', 
		'\u0042', '\u0043', '\u0044', '\u0045', '\u0046', '\u0047', 
		'\u0048', '\u0049', '\u004A', '\u004B', '\u004C', '\u004D', 
		'\u004E', '\u004F', '\u0050', '\u0051', '\u0052', '\u0053', 
		'\u0054', '\u0055', '\u0056', '\u0057', '\u0058', '\u0059', 
		'\u005A', '\u005B', '\uF8DD', '\u005D', '\uF8DE', '\u005F', 
		'\uF8DF', '\u0061', '\u0062', '\u0063', '\u0064', '\u0065', 
		'\u0066', '\u0067', '\u0068', '\u0069', '\u006A', '\u006B', 
		'\u006C', '\u006D', '\u006E', '\u006F', '\u0070', '\u0071', 
		'\u0072', '\u0073', '\u0074', '\u0075', '\u0076', '\u0077', 
		'\u0078', '\u0079', '\u007A', '\uF8E0', '\u007C', '\uF8FC', 
		'\uF8FD', '\uF8FE', '\u0080', '\u0081', '\u0082', '\u0083', 
		'\u0084', '\u0085', '\u0086', '\u0087', '\u0088', '\u0089', 
		'\u008A', '\u008B', '\u008C', '\u008D', '\u008E', '\u008F', 
		'\u0090', '\u0091', '\u0092', '\u0093', '\u0094', '\u0095', 
		'\u0096', '\u0097', '\u0098', '\u0099', '\u009A', '\u009B', 
		'\u009C', '\u009D', '\u009E', '\u009F', '\uF8E1', '\u00A1', 
		'\u00A2', '\u00A3', '\u0024', '\u00A5', '\u0023', '\u00A7', 
		'\u00A4', '\uF8E2', '\uF8E3', '\u00AB', '\uF8E4', '\uF8E5', 
		'\uF8E6', '\uF8E7', '\u00B0', '\u00B1', '\u00B2', '\u00B3', 
		'\u00D7', '\u00B5', '\u00B6', '\u00B7', '\u00F7', '\uF8E8', 
		'\uF8E9', '\u00BB', '\u00BC', '\u00BD', '\u00BE', '\u00BF', 
		'\uF8EB', '\u0000', '\u0000', '\u0000', '\u0000', '\u0000', 
		'\u0000', '\u0000', '\u0000', '\u0000', '\u0000', '\u0000', 
		'\u0000', '\u0000', '\u0000', '\u0000', '\uF8EC', '\uF8ED', 
		'\uF8EE', '\uF8EF', '\uF8F0', '\uF8F1', '\uF8F2', '\uF8F3', 
		'\uF8F4', '\uF8F5', '\uF8F6', '\uF8F7', '\uF8F8', '\uF8F9', 
		'\uF8FA', '\uF8FB', '\u2126', '\u00C6', '\u00D0', '\u00AA', 
		'\u0126', '\uF8EA', '\u0132', '\u013F', '\u0141', '\u00D8', 
		'\u0152', '\u00BA', '\u00DE', '\u0166', '\u014A', '\u0149', 
		'\u0138', '\u00E6', '\u0111', '\u00F0', '\u0127', '\u0131', 
		'\u0133', '\u0140', '\u0142', '\u00F8', '\u0153', '\u00DF', 
		'\u00FE', '\u0167', '\u014B', '\uF8FF', 
	};
	private static readonly char[] t61ToUniC1 = { // 0xC100 - 0xC1FF
		'\u0000', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u0060', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u00C0', 
		'\u003F', '\u003F', '\u003F', '\u00C8', '\u003F', '\u003F', 
		'\u003F', '\u00CC', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u00D2', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u00D9', '\u003F', '\u1E80', '\u003F', '\u1EF2', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u00E0', '\u003F', '\u003F', '\u003F', '\u00E8', 
		'\u003F', '\u003F', '\u003F', '\u00EC', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u00F2', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u00F9', '\u003F', '\u1E81', 
		'\u003F', '\u1EF3', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', 
	};
	private static readonly char[] t61ToUniC2 = { // 0xC200 - 0xC2FF
		'\u0000', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u00B4', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u00C1', 
		'\u003F', '\u0106', '\u003F', '\u00C9', '\u003F', '\u01F4', 
		'\u003F', '\u00CD', '\u003F', '\u1E30', '\u0139', '\u1E3E', 
		'\u0143', '\u00D3', '\u1E54', '\u003F', '\u0154', '\u015A', 
		'\u003F', '\u00DA', '\u003F', '\u1E82', '\u003F', '\u00DD', 
		'\u0179', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u00E1', '\u003F', '\u0107', '\u003F', '\u00E9', 
		'\u003F', '\u01F5', '\u003F', '\u00ED', '\u003F', '\u1E31', 
		'\u013A', '\u1E3F', '\u0144', '\u00F3', '\u1E55', '\u003F', 
		'\u0155', '\u015B', '\u003F', '\u00FA', '\u003F', '\u1E83', 
		'\u003F', '\u00FD', '\u017A', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', 
	};
	private static readonly char[] t61ToUniC3 = { // 0xC300 - 0xC3FF
		'\u0000', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u005E', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u00C2', 
		'\u003F', '\u0108', '\u003F', '\u00CA', '\u003F', '\u011C', 
		'\u0124', '\u00CE', '\u0134', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u00D4', '\u003F', '\u003F', '\u003F', '\u015C', 
		'\u003F', '\u00DB', '\u003F', '\u0174', '\u003F', '\u0176', 
		'\u1E90', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u00E2', '\u003F', '\u0109', '\u003F', '\u00EA', 
		'\u003F', '\u011D', '\u0125', '\u00EE', '\u0135', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u00F4', '\u003F', '\u003F', 
		'\u003F', '\u015D', '\u003F', '\u00FB', '\u003F', '\u0175', 
		'\u003F', '\u0177', '\u1E91', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', 
	};
	private static readonly char[] t61ToUniC4 = { // 0xC400 - 0xC4FF
		'\u0000', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u007E', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u00C3', 
		'\u003F', '\u003F', '\u003F', '\u1EBC', '\u003F', '\u003F', 
		'\u003F', '\u0128', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u00D1', '\u00D5', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u0168', '\u1E7C', '\u003F', '\u003F', '\u1EF8', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u00E3', '\u003F', '\u003F', '\u003F', '\u1EBD', 
		'\u003F', '\u003F', '\u003F', '\u0129', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u00F1', '\u00F5', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u0169', '\u1E7D', '\u003F', 
		'\u003F', '\u1EF9', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', 
	};
	private static readonly char[] t61ToUniC5 = { // 0xC500 - 0xC5FF
		'\u0000', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u00AF', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u0100', 
		'\u003F', '\u003F', '\u003F', '\u0112', '\u003F', '\u1E20', 
		'\u003F', '\u012A', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u014C', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u016A', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u0101', '\u003F', '\u003F', '\u003F', '\u0113', 
		'\u003F', '\u1E21', '\u003F', '\u012B', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u014D', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u016B', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u01E2', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u01E3', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', 
	};
	private static readonly char[] t61ToUniC6 = { // 0xC600 - 0xC6FF
		'\u0000', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u02D8', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u0102', 
		'\u003F', '\u003F', '\u003F', '\u0114', '\u003F', '\u011E', 
		'\u003F', '\u012C', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u014E', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u016C', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u0103', '\u003F', '\u003F', '\u003F', '\u0115', 
		'\u003F', '\u011F', '\u003F', '\u012D', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u014F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u016D', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', 
	};
	private static readonly char[] t61ToUniC7 = { // 0xC700 - 0xC7FF
		'\u0000', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u02D9', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u1E02', '\u010A', '\u1E0A', '\u0116', '\u1E1E', '\u0120', 
		'\u1E22', '\u0130', '\u003F', '\u003F', '\u003F', '\u1E40', 
		'\u1E44', '\u003F', '\u1E56', '\u003F', '\u1E58', '\u1E60', 
		'\u1E6A', '\u003F', '\u003F', '\u1E86', '\u1E8A', '\u1E8E', 
		'\u017B', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u1E03', '\u010B', '\u1E0B', '\u0117', 
		'\u1E1F', '\u0121', '\u1E23', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u1E41', '\u1E45', '\u003F', '\u1E57', '\u003F', 
		'\u1E59', '\u1E61', '\u1E6B', '\u003F', '\u003F', '\u1E87', 
		'\u1E8B', '\u1E8F', '\u017C', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', 
	};
	private static readonly char[] t61ToUniC8 = { // 0xC800 - 0xC8FF
		'\u0000', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u00A8', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u00C4', 
		'\u003F', '\u003F', '\u003F', '\u00CB', '\u003F', '\u003F', 
		'\u1E26', '\u00CF', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u00D6', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u00DC', '\u003F', '\u1E84', '\u1E8C', '\u0178', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u00E4', '\u003F', '\u003F', '\u003F', '\u00EB', 
		'\u003F', '\u003F', '\u1E27', '\u00EF', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u00F6', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u1E97', '\u00FC', '\u003F', '\u1E85', 
		'\u1E8D', '\u00FF', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', 
	};
	private static readonly char[] t61ToUniC9 = { // 0xC900 - 0xC9FF
		'\u0000', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', 
	};
	private static readonly char[] t61ToUniCA = { // 0xCA00 - 0xCAFF
		'\u0000', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u02DA', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u00C5', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u016E', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u00E5', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u016F', '\u003F', '\u1E98', 
		'\u003F', '\u1E99', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', 
	};
	private static readonly char[] t61ToUniCB = { // 0xCB00 - 0xCBFF
		'\u0000', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u00B8', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u00C7', '\u1E10', '\u003F', '\u003F', '\u0122', 
		'\u1E28', '\u003F', '\u003F', '\u0136', '\u013B', '\u003F', 
		'\u0145', '\u003F', '\u003F', '\u003F', '\u0156', '\u015E', 
		'\u0162', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u00E7', '\u1E11', '\u003F', 
		'\u003F', '\u0123', '\u1E29', '\u003F', '\u003F', '\u0137', 
		'\u013C', '\u003F', '\u0146', '\u003F', '\u003F', '\u003F', 
		'\u0157', '\u015F', '\u0163', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', 
	};
	private static readonly char[] t61ToUniCC = { // 0xCC00 - 0xCCFF
		'\u0000', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u005F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', 
	};
	private static readonly char[] t61ToUniCD = { // 0xCD00 - 0xCDFF
		'\u0000', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u02DD', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u0150', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u0170', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u0151', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u0171', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', 
	};
	private static readonly char[] t61ToUniCE = { // 0xCE00 - 0xCEFF
		'\u0000', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u02DB', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u0104', 
		'\u003F', '\u003F', '\u003F', '\u0118', '\u003F', '\u003F', 
		'\u003F', '\u012E', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u01EA', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u0172', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u0105', '\u003F', '\u003F', '\u003F', '\u0119', 
		'\u003F', '\u003F', '\u003F', '\u012F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u01EB', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u0173', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', 
	};
	private static readonly char[] t61ToUniCF = { // 0xCF00 - 0xCFFF
		'\u0000', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u02C7', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u01CD', 
		'\u003F', '\u010C', '\u010E', '\u011A', '\u003F', '\u01E6', 
		'\u003F', '\u01CF', '\u003F', '\u01E8', '\u013D', '\u003F', 
		'\u0147', '\u01D1', '\u003F', '\u003F', '\u0158', '\u0160', 
		'\u0164', '\u01D3', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u017D', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u01CE', '\u003F', '\u010D', '\u010F', '\u011B', 
		'\u003F', '\u01E7', '\u003F', '\u01D0', '\u01F0', '\u01E9', 
		'\u013E', '\u003F', '\u0148', '\u01D2', '\u003F', '\u003F', 
		'\u0159', '\u0161', '\u0165', '\u01D4', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u017E', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u003F', 
	};
	private static readonly char[][] t61ToUniC = {
		t61ToUniC1,
		t61ToUniC2,
		t61ToUniC3,
		t61ToUniC4,
		t61ToUniC5,
		t61ToUniC6,
		t61ToUniC7,
		t61ToUniC8,
		t61ToUniC9,
		t61ToUniCA,
		t61ToUniCB,
		t61ToUniCC,
		t61ToUniCD,
		t61ToUniCE,
		t61ToUniCF
	};

}; // class CP20261

// Convenience wrappers for the name forms of the T.61 code page.
public class ENCcp20261 : CP20261 {}
public class ENCx_cp20261 : CP20261 {}

}; // namespace I18N.Rare
