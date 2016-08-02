/*
 * CP20106.cs - IA5 German (7-bit) code page.
 *
 * Copyright (c) 2002  Southern Storm Software, Pty Ltd
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

// Generated from "ia5-20106.ucm".

namespace I18N.Rare
{

using System;
using I18N.Common;

public class CP20106 : ByteEncoding
{
	public CP20106()
		: base(20106, ToChars, "IA5 German (7-bit)",
		       "windows-20106", "windows-20106", "windows-20106",
		       false, false, false, false, 1252)
	{}

	private static readonly char[] ToChars = {
		'\u0000', '\u0001', '\u0002', '\u0003', '\u0004', '\u0005', 
		'\u0006', '\u0007', '\u0008', '\u0009', '\u000A', '\u000B', 
		'\u000C', '\u000D', '\u000E', '\u000F', '\u0010', '\u0011', 
		'\u0012', '\u0013', '\u0014', '\u0015', '\u0016', '\u0017', 
		'\u0018', '\u0019', '\u001A', '\u001B', '\u001C', '\u001D', 
		'\u001E', '\u001F', '\u0020', '\u0021', '\u0022', '\u0023', 
		'\u0024', '\u0025', '\u0026', '\u0027', '\u0028', '\u0029', 
		'\u002A', '\u002B', '\u002C', '\u002D', '\u002E', '\u002F', 
		'\u0030', '\u0031', '\u0032', '\u0033', '\u0034', '\u0035', 
		'\u0036', '\u0037', '\u0038', '\u0039', '\u003A', '\u003B', 
		'\u003C', '\u003D', '\u003E', '\u003F', '\u00A7', '\u0041', 
		'\u0042', '\u0043', '\u0044', '\u0045', '\u0046', '\u0047', 
		'\u0048', '\u0049', '\u004A', '\u004B', '\u004C', '\u004D', 
		'\u004E', '\u004F', '\u0050', '\u0051', '\u0052', '\u0053', 
		'\u0054', '\u0055', '\u0056', '\u0057', '\u0058', '\u0059', 
		'\u005A', '\u00C4', '\u00D6', '\u00DC', '\u005E', '\u005F', 
		'\u0060', '\u0061', '\u0062', '\u0063', '\u0064', '\u0065', 
		'\u0066', '\u0067', '\u0068', '\u0069', '\u006A', '\u006B', 
		'\u006C', '\u006D', '\u006E', '\u006F', '\u0070', '\u0071', 
		'\u0072', '\u0073', '\u0074', '\u0075', '\u0076', '\u0077', 
		'\u0078', '\u0079', '\u007A', '\u00E4', '\u00F6', '\u00FC', 
		'\u00DF', '\u007F', '\u003F', '\u003F', '\u003F', '\u003F', 
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

	protected override void ToBytes(char[] chars, int charIndex, int charCount,
	                                byte[] bytes, int byteIndex)
	{
		int ch;
		while(charCount > 0)
		{
			ch = (int)(chars[charIndex++]);
			if(ch >= 128) switch(ch)
			{
				case 0x00A7: ch = 0x40; break;
				case 0x00C4: ch = 0x5B; break;
				case 0x00D6: ch = 0x5C; break;
				case 0x00DC: ch = 0x5D; break;
				case 0x00DF: ch = 0x7E; break;
				case 0x00E4: ch = 0x7B; break;
				case 0x00F6: ch = 0x7C; break;
				case 0x00FC: ch = 0x7D; break;
				default:
				{
					if(ch >= 0xFF01 && ch <= 0xFF5E)
						ch -= 0xFEE0;
					else
						ch = 0x3F;
				}
				break;
			}
			bytes[byteIndex++] = (byte)ch;
			--charCount;
		}
	}

	protected override void ToBytes(String s, int charIndex, int charCount,
	                                byte[] bytes, int byteIndex)
	{
		int ch;
		while(charCount > 0)
		{
			ch = (int)(s[charIndex++]);
			if(ch >= 128) switch(ch)
			{
				case 0x00A7: ch = 0x40; break;
				case 0x00C4: ch = 0x5B; break;
				case 0x00D6: ch = 0x5C; break;
				case 0x00DC: ch = 0x5D; break;
				case 0x00DF: ch = 0x7E; break;
				case 0x00E4: ch = 0x7B; break;
				case 0x00F6: ch = 0x7C; break;
				case 0x00FC: ch = 0x7D; break;
				default:
				{
					if(ch >= 0xFF01 && ch <= 0xFF5E)
						ch -= 0xFEE0;
					else
						ch = 0x3F;
				}
				break;
			}
			bytes[byteIndex++] = (byte)ch;
			--charCount;
		}
	}

}; // class CP20106

public class ENCwindows_20106 : CP20106
{
	public ENCwindows_20106() : base() {}

}; // class ENCwindows_20106

}; // namespace I18N.Rare
