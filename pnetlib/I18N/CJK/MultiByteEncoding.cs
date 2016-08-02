/*
 * MultiByteEncoding.cs - Common base for multi-byte encoding classes.
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

namespace I18N.CJK
{

using System;
using System.Text;
using I18N.Common;

// Helper class for handling multi-byte encodings, driven by a table.
// The table is assumed to be the output of "pnetlib/I18N/tools/mb2tab".

public unsafe abstract class MultiByteEncoding : Encoding
{
	// Internal state.
	private String bodyName;
	private String encodingName;
	private String headerName;
	private String webName;
	private int windowsCodePage;
	private bool mainEncoding;
	private int lowFirst, highFirst;
	private int lowSecond, highSecond;
	private int lowRangeUpper;
	private int midRangeLower, midRangeUpper;
	private int highRangeLower;
	private byte *dbyteToUnicode;
	private byte *lowUnicodeToDByte;
	private byte *midUnicodeToDByte;
	private byte *highUnicodeToDByte;

	// Section numbers in the resource table.
	private const int Info_Block			= 1;
	private const int DByte_To_Unicode		= 2;
	private const int Low_Unicode_To_DByte	= 3;
	private const int Mid_Unicode_To_DByte	= 4;
	private const int High_Unicode_To_DByte	= 5;

	// Constructor.
	protected MultiByteEncoding(int codePage, String bodyName,
								String encodingName, String headerName,
								String webName, int windowsCodePage,
								String tableName)
			: base(codePage)
			{
				// Initialize this object's state.
				this.bodyName = bodyName;
				this.encodingName = encodingName;
				this.headerName = headerName;
				this.webName = webName;
				this.windowsCodePage = windowsCodePage;
				this.mainEncoding = (codePage == windowsCodePage);

				// Load the conversion rules from the resource table.
				CodeTable table = new CodeTable(tableName);
				byte *info = table.GetSection(Info_Block);
				dbyteToUnicode = table.GetSection(DByte_To_Unicode);
				lowUnicodeToDByte = table.GetSection(Low_Unicode_To_DByte);
				midUnicodeToDByte = table.GetSection(Mid_Unicode_To_DByte);
				highUnicodeToDByte = table.GetSection(High_Unicode_To_DByte);
				table.Dispose();

				// Decode the data in the information header block.
				lowFirst = info[0];
				highFirst = info[1];
				lowSecond = info[2];
				highSecond = info[3];
				lowRangeUpper = (info[4] | (info[5] << 8));
				midRangeLower = (info[6] | (info[7] << 8));
				midRangeUpper = (info[8] | (info[9] << 8));
				highRangeLower = (info[10] | (info[11] << 8));
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
				int lowRangeUpper = this.lowRangeUpper;
				int midRangeLower = this.midRangeLower;
				int midRangeUpper = this.midRangeUpper;
				int highRangeLower = this.highRangeLower;
				byte *lowUnicodeToDByte = this.lowUnicodeToDByte;
				byte *midUnicodeToDByte = this.midUnicodeToDByte;
				byte *highUnicodeToDByte = this.highUnicodeToDByte;
				while(count > 0)
				{
					ch = chars[index++];
					--count;
					if(ch < 0x80)
					{
						// Short-cut for the ASCII subset.
						++length;
						continue;
					}
					else if(ch <= lowRangeUpper)
					{
						value = lowUnicodeToDByte[ch * 2] |
								(lowUnicodeToDByte[ch * 2 + 1] << 8);
					}
					else if(ch >= midRangeLower && ch <= midRangeUpper)
					{
						ch -= midRangeLower;
						value = midUnicodeToDByte[ch * 2] |
								(midUnicodeToDByte[ch * 2 + 1] << 8);
					}
					else if(ch >= highRangeLower)
					{
						ch -= highRangeLower;
						value = highUnicodeToDByte[ch * 2] |
								(highUnicodeToDByte[ch * 2 + 1] << 8);
					}
					else
					{
						value = 0;
					}
					if(value == 0)
					{
						continue;
					}
					else if(value < 0x0100)
					{
						++length;
					}
					else
					{
						length += 2;
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
				int lowRangeUpper = this.lowRangeUpper;
				int midRangeLower = this.midRangeLower;
				int midRangeUpper = this.midRangeUpper;
				int highRangeLower = this.highRangeLower;
				byte *lowUnicodeToDByte = this.lowUnicodeToDByte;
				byte *midUnicodeToDByte = this.midUnicodeToDByte;
				byte *highUnicodeToDByte = this.highUnicodeToDByte;
				while(charCount > 0)
				{
					ch = chars[charIndex++];
					--charCount;
					if(ch < 0x80)
					{
						// Short-cut for the ASCII subset.
						if(posn >= byteLength)
						{
							throw new ArgumentException
								(Strings.GetString("Arg_InsufficientSpace"),
								 "bytes");
						}
						bytes[posn++] = (byte)ch;
						continue;
					}
					else if(ch <= lowRangeUpper)
					{
						value = lowUnicodeToDByte[ch * 2] |
								(lowUnicodeToDByte[ch * 2 + 1] << 8);
					}
					else if(ch >= midRangeLower && ch <= midRangeUpper)
					{
						ch -= midRangeLower;
						value = midUnicodeToDByte[ch * 2] |
								(midUnicodeToDByte[ch * 2 + 1] << 8);
					}
					else if(ch >= highRangeLower)
					{
						ch -= highRangeLower;
						value = highUnicodeToDByte[ch * 2] |
								(highUnicodeToDByte[ch * 2 + 1] << 8);
					}
					else
					{
						value = 0;
					}
					if(value == 0)
					{
						continue;
					}
					else if(value < 0x0100)
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
				int lowFirst = this.lowFirst;
				int highFirst = this.highFirst;
				while(count > 0)
				{
					byteval = bytes[index++];
					--count;
					++length;
					if(byteval >= lowFirst && byteval <= highFirst)
					{
						// First byte of a double-byte sequence.
						if(count > 0)
						{
							++index;
							--count;
						}
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
				int byteval, value;
				int lowFirst = this.lowFirst;
				int highFirst = this.highFirst;
				int lowSecond = this.lowSecond;
				int highSecond = this.highSecond;
				int bankWidth = (highSecond - lowSecond + 1);
				byte *table = this.dbyteToUnicode;
				while(byteCount > 0)
				{
					byteval = bytes[byteIndex++];
					--byteCount;
					if(posn >= charLength)
					{
						throw new ArgumentException
							(Strings.GetString("Arg_InsufficientSpace"),
							 "chars");
					}
					if(byteval < lowFirst || byteval > highFirst)
					{
						// Ordinary ASCII/Latin1 character.
						chars[posn++] = (char)byteval;
					}
					else
					{
						// Double-byte character.
						value = (byteval - lowFirst) * bankWidth;
						if(byteCount > 0)
						{
							byteval = bytes[byteIndex++];
							--byteCount;
						}
						else
						{
							byteval = 0;
						}
						if(byteval >= lowSecond && byteval <= highSecond)
						{
							value += (byteval - lowSecond);
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
				return new MultiByteDecoder(this);
			}

#if !ECMA_COMPAT

	// Get the mail body name for this encoding.
	public override String BodyName
			{
				get
				{
					return bodyName;
				}
			}

	// Get the human-readable name for this encoding.
	public override String EncodingName
			{
				get
				{
					return encodingName;
				}
			}

	// Get the mail agent header name for this encoding.
	public override String HeaderName
			{
				get
				{
					return headerName;
				}
			}

	// Determine if this encoding can be displayed in a Web browser.
	public override bool IsBrowserDisplay
			{
				get
				{
					return mainEncoding;
				}
			}

	// Determine if this encoding can be saved from a Web browser.
	public override bool IsBrowserSave
			{
				get
				{
					return mainEncoding;
				}
			}

	// Determine if this encoding can be displayed in a mail/news agent.
	public override bool IsMailNewsDisplay
			{
				get
				{
					return mainEncoding;
				}
			}

	// Determine if this encoding can be saved from a mail/news agent.
	public override bool IsMailNewsSave
			{
				get
				{
					return mainEncoding;
				}
			}

	// Get the IANA-preferred Web name for this encoding.
	public override String WebName
			{
				get
				{
					return webName;
				}
			}

	// Get the Windows code page represented by this object.
	public override int WindowsCodePage
			{
				get
				{
					return windowsCodePage;
				}
			}

#endif // !ECMA_COMPAT

	// Decoder that handles a rolling multi-byte state.
	private sealed class MultiByteDecoder : Decoder
	{
		private MultiByteEncoding enc;
		private int lastByte;

		// Constructor.
		public MultiByteDecoder(MultiByteEncoding enc)
				{
					this.enc = enc;
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
					int lowFirst = enc.lowFirst;
					int highFirst = enc.highFirst;
					while(count > 0)
					{
						byteval = bytes[index++];
						--count;
						if(last == 0)
						{
							if(byteval >= lowFirst && byteval <= highFirst)
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
					int byteval, value;
					int last = lastByte;
					int lowFirst = enc.lowFirst;
					int highFirst = enc.highFirst;
					int lowSecond = enc.lowSecond;
					int highSecond = enc.highSecond;
					int bankWidth = (highSecond - lowSecond + 1);
					byte *table = enc.dbyteToUnicode;
					while(byteCount > 0)
					{
						byteval = bytes[byteIndex++];
						--byteCount;
						if(last == 0)
						{
							if(posn >= charLength)
							{
								throw new ArgumentException
									(Strings.GetString
										("Arg_InsufficientSpace"), "chars");
							}
							if(byteval < lowFirst || byteval > highFirst)
							{
								// Ordinary ASCII/Latin1 character.
								chars[posn++] = (char)byteval;
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
							if(byteval < lowSecond || byteval > highSecond)
							{
								// Invalid second byte.
								chars[posn++] = '\0';
								last = 0;
								continue;
							}
							value = (last - lowFirst) * bankWidth +
									(byteval - lowSecond);
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
							last = 0;
						}
					}
					lastByte = last;

					// Return the final length to the caller.
					return posn - charIndex;
				}

	} // class MultiByteDecoder

}; // class MultiByteEncoding

}; // namespace I18N.CJK
