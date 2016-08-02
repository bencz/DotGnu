/*
 * CP10005.cs - Hebrew (Mac) code page.
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

// Generated from "mac-10005.ucm".

namespace I18N.MidEast
{

using System;
using I18N.Common;

public class CP10005 : ByteEncoding
{
	public CP10005()
		: base(10005, ToChars, "Hebrew (Mac)",
		       "windows-10005", "windows-10005", "windows-10005",
		       false, false, false, false, 1255)
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
		'\u003C', '\u003D', '\u003E', '\u003F', '\u0040', '\u0041', 
		'\u0042', '\u0043', '\u0044', '\u0045', '\u0046', '\u0047', 
		'\u0048', '\u0049', '\u004A', '\u004B', '\u004C', '\u004D', 
		'\u004E', '\u004F', '\u0050', '\u0051', '\u0052', '\u0053', 
		'\u0054', '\u0055', '\u0056', '\u0057', '\u0058', '\u0059', 
		'\u005A', '\u005B', '\u005C', '\u005D', '\u005E', '\u005F', 
		'\u0060', '\u0061', '\u0062', '\u0063', '\u0064', '\u0065', 
		'\u0066', '\u0067', '\u0068', '\u0069', '\u006A', '\u006B', 
		'\u006C', '\u006D', '\u006E', '\u006F', '\u0070', '\u0071', 
		'\u0072', '\u0073', '\u0074', '\u0075', '\u0076', '\u0077', 
		'\u0078', '\u0079', '\u007A', '\u007B', '\u007C', '\u007D', 
		'\u007E', '\u007F', '\u00C4', '\u05F2', '\u00C7', '\u00C9', 
		'\u00D1', '\u00D6', '\u00DC', '\u00E1', '\u00E0', '\u00E2', 
		'\u00E4', '\u00E3', '\u00E5', '\u00E7', '\u00E9', '\u00E8', 
		'\u00EA', '\u00EB', '\u00ED', '\u00EC', '\u00EE', '\u00EF', 
		'\u00F1', '\u00F3', '\u00F2', '\u00F4', '\u00F6', '\u00F5', 
		'\u00FA', '\u00F9', '\u00FB', '\u00FC', '\u0020', '\u0021', 
		'\u0022', '\u0023', '\u0024', '\u0025', '\u20AA', '\u0027', 
		'\u0029', '\u0028', '\u002A', '\u002B', '\u002C', '\u002D', 
		'\u002E', '\u002F', '\u0030', '\u0031', '\u0032', '\u0033', 
		'\u0034', '\u0035', '\u0036', '\u0037', '\u0038', '\u0039', 
		'\u003A', '\u003B', '\u003C', '\u003D', '\u003E', '\u003F', 
		'\uF86A', '\u201E', '\uF89B', '\uF89C', '\uF89D', '\uF89E', 
		'\u05BC', '\uFB4B', '\uFB35', '\u2026', '\u00A0', '\u05B8', 
		'\u05B7', '\u05B5', '\u05B6', '\u05B4', '\u2013', '\u2014', 
		'\u201C', '\u201D', '\u2018', '\u2019', '\uFB2A', '\uFB2B', 
		'\u05BF', '\u05B0', '\u05B2', '\u05B1', '\u05BB', '\u05B9', 
		'\u05B8', '\u05B3', '\u05D0', '\u05D1', '\u05D2', '\u05D3', 
		'\u05D4', '\u05D5', '\u05D6', '\u05D7', '\u05D8', '\u05D9', 
		'\u05DA', '\u05DB', '\u05DC', '\u05DD', '\u05DE', '\u05DF', 
		'\u05E0', '\u05E1', '\u05E2', '\u05E3', '\u05E4', '\u05E5', 
		'\u05E6', '\u05E7', '\u05E8', '\u05E9', '\u05EA', '\u007D', 
		'\u005D', '\u007B', '\u005B', '\u007C', 
	};

	protected override void ToBytes(char[] chars, int charIndex, int charCount,
	                                byte[] bytes, int byteIndex)
	{
		int ch;
		while(charCount > 0)
		{
			ch = (int)(chars[charIndex++]);
			if(ch >= 0) switch(ch)
			{
				case 0x0026:
				case 0x0040:
				case 0x0041:
				case 0x0042:
				case 0x0043:
				case 0x0044:
				case 0x0045:
				case 0x0046:
				case 0x0047:
				case 0x0048:
				case 0x0049:
				case 0x004A:
				case 0x004B:
				case 0x004C:
				case 0x004D:
				case 0x004E:
				case 0x004F:
				case 0x0050:
				case 0x0051:
				case 0x0052:
				case 0x0053:
				case 0x0054:
				case 0x0055:
				case 0x0056:
				case 0x0057:
				case 0x0058:
				case 0x0059:
				case 0x005A:
				case 0x005C:
				case 0x005E:
				case 0x005F:
				case 0x0060:
				case 0x0061:
				case 0x0062:
				case 0x0063:
				case 0x0064:
				case 0x0065:
				case 0x0066:
				case 0x0067:
				case 0x0068:
				case 0x0069:
				case 0x006A:
				case 0x006B:
				case 0x006C:
				case 0x006D:
				case 0x006E:
				case 0x006F:
				case 0x0070:
				case 0x0071:
				case 0x0072:
				case 0x0073:
				case 0x0074:
				case 0x0075:
				case 0x0076:
				case 0x0077:
				case 0x0078:
				case 0x0079:
				case 0x007A:
				case 0x007E:
					break;
				case 0x0020:
				case 0x0021:
				case 0x0022:
				case 0x0023:
				case 0x0024:
				case 0x0025:
					ch += 0x0080;
					break;
				case 0x0027: ch = 0xA7; break;
				case 0x0028: ch = 0xA9; break;
				case 0x0029: ch = 0xA8; break;
				case 0x002A:
				case 0x002B:
				case 0x002C:
				case 0x002D:
				case 0x002E:
				case 0x002F:
				case 0x0030:
				case 0x0031:
				case 0x0032:
				case 0x0033:
				case 0x0034:
				case 0x0035:
				case 0x0036:
				case 0x0037:
				case 0x0038:
				case 0x0039:
				case 0x003A:
				case 0x003B:
				case 0x003C:
				case 0x003D:
				case 0x003E:
				case 0x003F:
					ch += 0x0080;
					break;
				case 0x005B: ch = 0xFE; break;
				case 0x005D: ch = 0xFC; break;
				case 0x007B: ch = 0xFD; break;
				case 0x007C: ch = 0xFF; break;
				case 0x007D: ch = 0xFB; break;
				case 0x00A0: ch = 0xCA; break;
				case 0x00C4: ch = 0x80; break;
				case 0x00C7: ch = 0x82; break;
				case 0x00C9: ch = 0x83; break;
				case 0x00D1: ch = 0x84; break;
				case 0x00D6: ch = 0x85; break;
				case 0x00DC: ch = 0x86; break;
				case 0x00E0: ch = 0x88; break;
				case 0x00E1: ch = 0x87; break;
				case 0x00E2: ch = 0x89; break;
				case 0x00E3: ch = 0x8B; break;
				case 0x00E4: ch = 0x8A; break;
				case 0x00E5: ch = 0x8C; break;
				case 0x00E7: ch = 0x8D; break;
				case 0x00E8: ch = 0x8F; break;
				case 0x00E9: ch = 0x8E; break;
				case 0x00EA: ch = 0x90; break;
				case 0x00EB: ch = 0x91; break;
				case 0x00EC: ch = 0x93; break;
				case 0x00ED: ch = 0x92; break;
				case 0x00EE: ch = 0x94; break;
				case 0x00EF: ch = 0x95; break;
				case 0x00F1: ch = 0x96; break;
				case 0x00F2: ch = 0x98; break;
				case 0x00F3: ch = 0x97; break;
				case 0x00F4: ch = 0x99; break;
				case 0x00F5: ch = 0x9B; break;
				case 0x00F6: ch = 0x9A; break;
				case 0x00F9: ch = 0x9D; break;
				case 0x00FA: ch = 0x9C; break;
				case 0x00FB: ch = 0x9E; break;
				case 0x00FC: ch = 0x9F; break;
				case 0x05B0: ch = 0xD9; break;
				case 0x05B1: ch = 0xDB; break;
				case 0x05B2: ch = 0xDA; break;
				case 0x05B3: ch = 0xDF; break;
				case 0x05B4: ch = 0xCF; break;
				case 0x05B5: ch = 0xCD; break;
				case 0x05B6: ch = 0xCE; break;
				case 0x05B7: ch = 0xCC; break;
				case 0x05B8: ch = 0xDE; break;
				case 0x05B9: ch = 0xDD; break;
				case 0x05BB: ch = 0xDC; break;
				case 0x05BC: ch = 0xC6; break;
				case 0x05BF: ch = 0xD8; break;
				case 0x05D0:
				case 0x05D1:
				case 0x05D2:
				case 0x05D3:
				case 0x05D4:
				case 0x05D5:
				case 0x05D6:
				case 0x05D7:
				case 0x05D8:
				case 0x05D9:
				case 0x05DA:
				case 0x05DB:
				case 0x05DC:
				case 0x05DD:
				case 0x05DE:
				case 0x05DF:
				case 0x05E0:
				case 0x05E1:
				case 0x05E2:
				case 0x05E3:
				case 0x05E4:
				case 0x05E5:
				case 0x05E6:
				case 0x05E7:
				case 0x05E8:
				case 0x05E9:
				case 0x05EA:
					ch -= 0x04F0;
					break;
				case 0x05F2: ch = 0x81; break;
				case 0x2013: ch = 0xD0; break;
				case 0x2014: ch = 0xD1; break;
				case 0x2018: ch = 0xD4; break;
				case 0x2019: ch = 0xD5; break;
				case 0x201C: ch = 0xD2; break;
				case 0x201D: ch = 0xD3; break;
				case 0x201E: ch = 0xC1; break;
				case 0x2026: ch = 0xC9; break;
				case 0x20AA: ch = 0xA6; break;
				case 0xF86A: ch = 0xC0; break;
				case 0xF89B:
				case 0xF89C:
				case 0xF89D:
				case 0xF89E:
					ch -= 0xF7D9;
					break;
				case 0xFB2A: ch = 0xD6; break;
				case 0xFB2B: ch = 0xD7; break;
				case 0xFB35: ch = 0xC8; break;
				case 0xFB4B: ch = 0xC7; break;
				default: ch = 0x3F; break;
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
			if(ch >= 0) switch(ch)
			{
				case 0x0026:
				case 0x0040:
				case 0x0041:
				case 0x0042:
				case 0x0043:
				case 0x0044:
				case 0x0045:
				case 0x0046:
				case 0x0047:
				case 0x0048:
				case 0x0049:
				case 0x004A:
				case 0x004B:
				case 0x004C:
				case 0x004D:
				case 0x004E:
				case 0x004F:
				case 0x0050:
				case 0x0051:
				case 0x0052:
				case 0x0053:
				case 0x0054:
				case 0x0055:
				case 0x0056:
				case 0x0057:
				case 0x0058:
				case 0x0059:
				case 0x005A:
				case 0x005C:
				case 0x005E:
				case 0x005F:
				case 0x0060:
				case 0x0061:
				case 0x0062:
				case 0x0063:
				case 0x0064:
				case 0x0065:
				case 0x0066:
				case 0x0067:
				case 0x0068:
				case 0x0069:
				case 0x006A:
				case 0x006B:
				case 0x006C:
				case 0x006D:
				case 0x006E:
				case 0x006F:
				case 0x0070:
				case 0x0071:
				case 0x0072:
				case 0x0073:
				case 0x0074:
				case 0x0075:
				case 0x0076:
				case 0x0077:
				case 0x0078:
				case 0x0079:
				case 0x007A:
				case 0x007E:
					break;
				case 0x0020:
				case 0x0021:
				case 0x0022:
				case 0x0023:
				case 0x0024:
				case 0x0025:
					ch += 0x0080;
					break;
				case 0x0027: ch = 0xA7; break;
				case 0x0028: ch = 0xA9; break;
				case 0x0029: ch = 0xA8; break;
				case 0x002A:
				case 0x002B:
				case 0x002C:
				case 0x002D:
				case 0x002E:
				case 0x002F:
				case 0x0030:
				case 0x0031:
				case 0x0032:
				case 0x0033:
				case 0x0034:
				case 0x0035:
				case 0x0036:
				case 0x0037:
				case 0x0038:
				case 0x0039:
				case 0x003A:
				case 0x003B:
				case 0x003C:
				case 0x003D:
				case 0x003E:
				case 0x003F:
					ch += 0x0080;
					break;
				case 0x005B: ch = 0xFE; break;
				case 0x005D: ch = 0xFC; break;
				case 0x007B: ch = 0xFD; break;
				case 0x007C: ch = 0xFF; break;
				case 0x007D: ch = 0xFB; break;
				case 0x00A0: ch = 0xCA; break;
				case 0x00C4: ch = 0x80; break;
				case 0x00C7: ch = 0x82; break;
				case 0x00C9: ch = 0x83; break;
				case 0x00D1: ch = 0x84; break;
				case 0x00D6: ch = 0x85; break;
				case 0x00DC: ch = 0x86; break;
				case 0x00E0: ch = 0x88; break;
				case 0x00E1: ch = 0x87; break;
				case 0x00E2: ch = 0x89; break;
				case 0x00E3: ch = 0x8B; break;
				case 0x00E4: ch = 0x8A; break;
				case 0x00E5: ch = 0x8C; break;
				case 0x00E7: ch = 0x8D; break;
				case 0x00E8: ch = 0x8F; break;
				case 0x00E9: ch = 0x8E; break;
				case 0x00EA: ch = 0x90; break;
				case 0x00EB: ch = 0x91; break;
				case 0x00EC: ch = 0x93; break;
				case 0x00ED: ch = 0x92; break;
				case 0x00EE: ch = 0x94; break;
				case 0x00EF: ch = 0x95; break;
				case 0x00F1: ch = 0x96; break;
				case 0x00F2: ch = 0x98; break;
				case 0x00F3: ch = 0x97; break;
				case 0x00F4: ch = 0x99; break;
				case 0x00F5: ch = 0x9B; break;
				case 0x00F6: ch = 0x9A; break;
				case 0x00F9: ch = 0x9D; break;
				case 0x00FA: ch = 0x9C; break;
				case 0x00FB: ch = 0x9E; break;
				case 0x00FC: ch = 0x9F; break;
				case 0x05B0: ch = 0xD9; break;
				case 0x05B1: ch = 0xDB; break;
				case 0x05B2: ch = 0xDA; break;
				case 0x05B3: ch = 0xDF; break;
				case 0x05B4: ch = 0xCF; break;
				case 0x05B5: ch = 0xCD; break;
				case 0x05B6: ch = 0xCE; break;
				case 0x05B7: ch = 0xCC; break;
				case 0x05B8: ch = 0xDE; break;
				case 0x05B9: ch = 0xDD; break;
				case 0x05BB: ch = 0xDC; break;
				case 0x05BC: ch = 0xC6; break;
				case 0x05BF: ch = 0xD8; break;
				case 0x05D0:
				case 0x05D1:
				case 0x05D2:
				case 0x05D3:
				case 0x05D4:
				case 0x05D5:
				case 0x05D6:
				case 0x05D7:
				case 0x05D8:
				case 0x05D9:
				case 0x05DA:
				case 0x05DB:
				case 0x05DC:
				case 0x05DD:
				case 0x05DE:
				case 0x05DF:
				case 0x05E0:
				case 0x05E1:
				case 0x05E2:
				case 0x05E3:
				case 0x05E4:
				case 0x05E5:
				case 0x05E6:
				case 0x05E7:
				case 0x05E8:
				case 0x05E9:
				case 0x05EA:
					ch -= 0x04F0;
					break;
				case 0x05F2: ch = 0x81; break;
				case 0x2013: ch = 0xD0; break;
				case 0x2014: ch = 0xD1; break;
				case 0x2018: ch = 0xD4; break;
				case 0x2019: ch = 0xD5; break;
				case 0x201C: ch = 0xD2; break;
				case 0x201D: ch = 0xD3; break;
				case 0x201E: ch = 0xC1; break;
				case 0x2026: ch = 0xC9; break;
				case 0x20AA: ch = 0xA6; break;
				case 0xF86A: ch = 0xC0; break;
				case 0xF89B:
				case 0xF89C:
				case 0xF89D:
				case 0xF89E:
					ch -= 0xF7D9;
					break;
				case 0xFB2A: ch = 0xD6; break;
				case 0xFB2B: ch = 0xD7; break;
				case 0xFB35: ch = 0xC8; break;
				case 0xFB4B: ch = 0xC7; break;
				default: ch = 0x3F; break;
			}
			bytes[byteIndex++] = (byte)ch;
			--charCount;
		}
	}

}; // class CP10005

public class ENCwindows_10005 : CP10005
{
	public ENCwindows_10005() : base() {}

}; // class ENCwindows_10005

}; // namespace I18N.MidEast
