/*
 * ASCIIEncoding.cs - Implementation of the "System.Text.ASCIIEncoding" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

using System;

[Serializable]
public class ASCIIEncoding : Encoding
{
	// Magic number used by Windows for "ASCII".
	internal const int ASCII_CODE_PAGE = 20127;

	// Constructor.
	public ASCIIEncoding() : base(ASCII_CODE_PAGE) {}

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
				return count;
			}

	// Convenience wrappers for "GetByteCount".
	public override int GetByteCount(String s)
			{
				if(s == null)
				{
					throw new ArgumentNullException("s");
				}
				return s.Length;
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
				if((bytes.Length - byteIndex) < charCount)
				{
					throw new ArgumentException
						(_("Arg_InsufficientSpace"));
				}
				int count = charCount;
				char ch;
				while(count-- > 0)
				{
					ch = chars[charIndex++];
					if(ch < (char)0x80)
					{
						bytes[byteIndex++] = (byte)ch;
					}
					else
					{
						bytes[byteIndex++] = (byte)'?';
					}
				}
				return charCount;
			}

	// Convenience wrappers for "GetBytes".
	public override int GetBytes(String s, int charIndex, int charCount,
								 byte[] bytes, int byteIndex)
			{
				if(s == null)
				{
					throw new ArgumentNullException("s");
				}
				if(bytes == null)
				{
					throw new ArgumentNullException("bytes");
				}
				if(charIndex < 0 || charIndex > s.Length)
				{
					throw new ArgumentOutOfRangeException
						("charIndex", _("ArgRange_StringIndex"));
				}
				if(charCount < 0 || charCount > (s.Length - charIndex))
				{
					throw new ArgumentOutOfRangeException
						("charCount", _("ArgRange_StringRange"));
				}
				if(byteIndex < 0 || byteIndex > bytes.Length)
				{
					throw new ArgumentOutOfRangeException
						("byteIndex", _("ArgRange_Array"));
				}
				if((bytes.Length - byteIndex) < charCount)
				{
					throw new ArgumentException(_("Arg_InsufficientSpace"));
				}
				int count = charCount;
				char ch;
				while(count-- > 0)
				{
					ch = s[charIndex++];
					if(ch < (char)0x80)
					{
						bytes[byteIndex++] = (byte)ch;
					}
					else
					{
						bytes[byteIndex++] = (byte)'?';
					}
				}
				return charCount;
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
						("index", _("ArgRange_Array"));
				}
				if(count < 0 || count > (bytes.Length - index))
				{
					throw new ArgumentOutOfRangeException
						("count", _("ArgRange_Array"));
				}
				return count;
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
				if((chars.Length - charIndex) < byteCount)
				{
					throw new ArgumentException(_("Arg_InsufficientSpace"));
				}
				int count = byteCount;
				byte ch;
				while(count-- > 0)
				{
					ch = bytes[byteIndex++];
					if(ch < 0x80)
					{
						chars[charIndex++] = (char)ch;
					}
					else
					{
						chars[charIndex++] = '?';
					}
				}
				return byteCount;
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
				return charCount;
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
				return byteCount;
			}

	// Decode a buffer of bytes into a string.
	public override String GetString(byte[] bytes, int index, int count)
			{
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
				if(count == 0)
				{
					return String.Empty;
				}
				String s = String.NewString(count);
				int posn = 0;
				while(count-- > 0)
				{
					s.SetChar(posn++, (char)(bytes[index++]));
				}
				return s;
			}
	public override String GetString(byte[] bytes)
			{
				if(bytes == null)
				{
					throw new ArgumentNullException("bytes");
				}
				if(bytes.Length == 0)
				{
					return String.Empty;
				}
				int count = bytes.Length;
				int posn = 0;
				String s = String.NewString(count);
				while(count-- > 0)
				{
					s.SetChar(posn, (char)(bytes[posn]));
					++posn;
				}
				return s;
			}

#if !ECMA_COMPAT

	// Get the mail body name for this encoding.
	internal override String InternalBodyName
			{
				get
				{
					return "us-ascii";
				}
			}

	// Get the human-readable name for this encoding.
	internal override String InternalEncodingName
			{
				get
				{
					return "US-ASCII";
				}
			}

	// Get the mail agent header name for this encoding.
	internal override String InternalHeaderName
			{
				get
				{
					return "us-ascii";
				}
			}

	// Determine if this encoding can be displayed in a mail/news agent.
	internal override bool InternalIsMailNewsDisplay
			{
				get
				{
					return true;
				}
			}

	// Determine if this encoding can be saved from a mail/news agent.
	internal override bool InternalIsMailNewsSave
			{
				get
				{
					return true;
				}
			}

	// Get the IANA-preferred Web name for this encoding.
	internal override String InternalWebName
			{
				get
				{
					return "us-ascii";
				}
			}

#endif // !ECMA_COMPAT

}; // class ASCIIEncoding

}; // namespace System.Text
