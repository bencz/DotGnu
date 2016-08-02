/*
 * CP10004.cs - Arabic (Mac) code page.
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

// Generated from "mac-10004.ucm".

namespace I18N.MidEast
{

using System;
using I18N.Common;

public class CP10004 : ByteEncoding
{
	public CP10004()
		: base(10004, ToChars, "Arabic (Mac)",
		       "windows-10004", "windows-10004", "windows-10004",
		       false, false, false, false, 1256)
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
		'\u007E', '\u007F', '\u00C4', '\u00A0', '\u00C7', '\u00C9', 
		'\u00D1', '\u00D6', '\u00DC', '\u00E1', '\u00E0', '\u00E2', 
		'\u00E4', '\u06BA', '\u00AB', '\u00E7', '\u00E9', '\u00E8', 
		'\u00EA', '\u00EB', '\u00ED', '\u2026', '\u00EE', '\u00EF', 
		'\u00F1', '\u00F3', '\u00BB', '\u00F4', '\u00F6', '\u00F7', 
		'\u00FA', '\u00F9', '\u00FB', '\u00FC', '\u0020', '\u0021', 
		'\u0022', '\u0023', '\u0024', '\u066A', '\u0026', '\u0027', 
		'\u0028', '\u0029', '\u002A', '\u002B', '\u060C', '\u002D', 
		'\u002E', '\u002F', '\u0660', '\u0661', '\u0662', '\u0663', 
		'\u0664', '\u0665', '\u0666', '\u0667', '\u0668', '\u0669', 
		'\u003A', '\u061B', '\u003C', '\u003D', '\u003E', '\u061F', 
		'\u274A', '\u0621', '\u0622', '\u0623', '\u0624', '\u0625', 
		'\u0626', '\u0627', '\u0628', '\u0629', '\u062A', '\u062B', 
		'\u062C', '\u062D', '\u062E', '\u062F', '\u0630', '\u0631', 
		'\u0632', '\u0633', '\u0634', '\u0635', '\u0636', '\u0637', 
		'\u0638', '\u0639', '\u063A', '\u005B', '\u005C', '\u005D', 
		'\u005E', '\u005F', '\u0640', '\u0641', '\u0642', '\u0643', 
		'\u0644', '\u0645', '\u0646', '\u0647', '\u0648', '\u0649', 
		'\u064A', '\u064B', '\u064C', '\u064D', '\u064E', '\u064F', 
		'\u0650', '\u0651', '\u0652', '\u067E', '\u0679', '\u0686', 
		'\u06D5', '\u06A4', '\u06AF', '\u0688', '\u0691', '\u007B', 
		'\u007C', '\u007D', '\u0698', '\u06D2', 
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
				case 0x0025:
				case 0x002C:
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
				case 0x003B:
				case 0x003F:
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
					ch += 0x0080;
					break;
				case 0x0026:
				case 0x0027:
				case 0x0028:
				case 0x0029:
				case 0x002A:
				case 0x002B:
					ch += 0x0080;
					break;
				case 0x002D: ch = 0xAD; break;
				case 0x002E: ch = 0xAE; break;
				case 0x002F: ch = 0xAF; break;
				case 0x003A: ch = 0xBA; break;
				case 0x003C: ch = 0xBC; break;
				case 0x003D: ch = 0xBD; break;
				case 0x003E: ch = 0xBE; break;
				case 0x005B:
				case 0x005C:
				case 0x005D:
				case 0x005E:
				case 0x005F:
					ch += 0x0080;
					break;
				case 0x007B: ch = 0xFB; break;
				case 0x007C: ch = 0xFC; break;
				case 0x007D: ch = 0xFD; break;
				case 0x00A0: ch = 0x81; break;
				case 0x00AB: ch = 0x8C; break;
				case 0x00BB: ch = 0x98; break;
				case 0x00C4: ch = 0x80; break;
				case 0x00C7: ch = 0x82; break;
				case 0x00C9: ch = 0x83; break;
				case 0x00D1: ch = 0x84; break;
				case 0x00D6: ch = 0x85; break;
				case 0x00DC: ch = 0x86; break;
				case 0x00E0: ch = 0x88; break;
				case 0x00E1: ch = 0x87; break;
				case 0x00E2: ch = 0x89; break;
				case 0x00E4: ch = 0x8A; break;
				case 0x00E7: ch = 0x8D; break;
				case 0x00E8: ch = 0x8F; break;
				case 0x00E9: ch = 0x8E; break;
				case 0x00EA: ch = 0x90; break;
				case 0x00EB: ch = 0x91; break;
				case 0x00ED: ch = 0x92; break;
				case 0x00EE: ch = 0x94; break;
				case 0x00EF: ch = 0x95; break;
				case 0x00F1: ch = 0x96; break;
				case 0x00F3: ch = 0x97; break;
				case 0x00F4: ch = 0x99; break;
				case 0x00F6: ch = 0x9A; break;
				case 0x00F7: ch = 0x9B; break;
				case 0x00F9: ch = 0x9D; break;
				case 0x00FA: ch = 0x9C; break;
				case 0x00FB: ch = 0x9E; break;
				case 0x00FC: ch = 0x9F; break;
				case 0x060C: ch = 0xAC; break;
				case 0x061B: ch = 0xBB; break;
				case 0x061F: ch = 0xBF; break;
				case 0x0621:
				case 0x0622:
				case 0x0623:
				case 0x0624:
				case 0x0625:
				case 0x0626:
				case 0x0627:
				case 0x0628:
				case 0x0629:
				case 0x062A:
				case 0x062B:
				case 0x062C:
				case 0x062D:
				case 0x062E:
				case 0x062F:
				case 0x0630:
				case 0x0631:
				case 0x0632:
				case 0x0633:
				case 0x0634:
				case 0x0635:
				case 0x0636:
				case 0x0637:
				case 0x0638:
				case 0x0639:
				case 0x063A:
					ch -= 0x0560;
					break;
				case 0x0640:
				case 0x0641:
				case 0x0642:
				case 0x0643:
				case 0x0644:
				case 0x0645:
				case 0x0646:
				case 0x0647:
				case 0x0648:
				case 0x0649:
				case 0x064A:
				case 0x064B:
				case 0x064C:
				case 0x064D:
				case 0x064E:
				case 0x064F:
				case 0x0650:
				case 0x0651:
				case 0x0652:
					ch -= 0x0560;
					break;
				case 0x0660:
				case 0x0661:
				case 0x0662:
				case 0x0663:
				case 0x0664:
				case 0x0665:
				case 0x0666:
				case 0x0667:
				case 0x0668:
				case 0x0669:
					ch -= 0x05B0;
					break;
				case 0x066A: ch = 0xA5; break;
				case 0x0679: ch = 0xF4; break;
				case 0x067E: ch = 0xF3; break;
				case 0x0686: ch = 0xF5; break;
				case 0x0688: ch = 0xF9; break;
				case 0x0691: ch = 0xFA; break;
				case 0x0698: ch = 0xFE; break;
				case 0x06A4: ch = 0xF7; break;
				case 0x06AF: ch = 0xF8; break;
				case 0x06BA: ch = 0x8B; break;
				case 0x06D2: ch = 0xFF; break;
				case 0x06D5: ch = 0xF6; break;
				case 0x2026: ch = 0x93; break;
				case 0x274A: ch = 0xC0; break;
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
				case 0x0025:
				case 0x002C:
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
				case 0x003B:
				case 0x003F:
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
					ch += 0x0080;
					break;
				case 0x0026:
				case 0x0027:
				case 0x0028:
				case 0x0029:
				case 0x002A:
				case 0x002B:
					ch += 0x0080;
					break;
				case 0x002D: ch = 0xAD; break;
				case 0x002E: ch = 0xAE; break;
				case 0x002F: ch = 0xAF; break;
				case 0x003A: ch = 0xBA; break;
				case 0x003C: ch = 0xBC; break;
				case 0x003D: ch = 0xBD; break;
				case 0x003E: ch = 0xBE; break;
				case 0x005B:
				case 0x005C:
				case 0x005D:
				case 0x005E:
				case 0x005F:
					ch += 0x0080;
					break;
				case 0x007B: ch = 0xFB; break;
				case 0x007C: ch = 0xFC; break;
				case 0x007D: ch = 0xFD; break;
				case 0x00A0: ch = 0x81; break;
				case 0x00AB: ch = 0x8C; break;
				case 0x00BB: ch = 0x98; break;
				case 0x00C4: ch = 0x80; break;
				case 0x00C7: ch = 0x82; break;
				case 0x00C9: ch = 0x83; break;
				case 0x00D1: ch = 0x84; break;
				case 0x00D6: ch = 0x85; break;
				case 0x00DC: ch = 0x86; break;
				case 0x00E0: ch = 0x88; break;
				case 0x00E1: ch = 0x87; break;
				case 0x00E2: ch = 0x89; break;
				case 0x00E4: ch = 0x8A; break;
				case 0x00E7: ch = 0x8D; break;
				case 0x00E8: ch = 0x8F; break;
				case 0x00E9: ch = 0x8E; break;
				case 0x00EA: ch = 0x90; break;
				case 0x00EB: ch = 0x91; break;
				case 0x00ED: ch = 0x92; break;
				case 0x00EE: ch = 0x94; break;
				case 0x00EF: ch = 0x95; break;
				case 0x00F1: ch = 0x96; break;
				case 0x00F3: ch = 0x97; break;
				case 0x00F4: ch = 0x99; break;
				case 0x00F6: ch = 0x9A; break;
				case 0x00F7: ch = 0x9B; break;
				case 0x00F9: ch = 0x9D; break;
				case 0x00FA: ch = 0x9C; break;
				case 0x00FB: ch = 0x9E; break;
				case 0x00FC: ch = 0x9F; break;
				case 0x060C: ch = 0xAC; break;
				case 0x061B: ch = 0xBB; break;
				case 0x061F: ch = 0xBF; break;
				case 0x0621:
				case 0x0622:
				case 0x0623:
				case 0x0624:
				case 0x0625:
				case 0x0626:
				case 0x0627:
				case 0x0628:
				case 0x0629:
				case 0x062A:
				case 0x062B:
				case 0x062C:
				case 0x062D:
				case 0x062E:
				case 0x062F:
				case 0x0630:
				case 0x0631:
				case 0x0632:
				case 0x0633:
				case 0x0634:
				case 0x0635:
				case 0x0636:
				case 0x0637:
				case 0x0638:
				case 0x0639:
				case 0x063A:
					ch -= 0x0560;
					break;
				case 0x0640:
				case 0x0641:
				case 0x0642:
				case 0x0643:
				case 0x0644:
				case 0x0645:
				case 0x0646:
				case 0x0647:
				case 0x0648:
				case 0x0649:
				case 0x064A:
				case 0x064B:
				case 0x064C:
				case 0x064D:
				case 0x064E:
				case 0x064F:
				case 0x0650:
				case 0x0651:
				case 0x0652:
					ch -= 0x0560;
					break;
				case 0x0660:
				case 0x0661:
				case 0x0662:
				case 0x0663:
				case 0x0664:
				case 0x0665:
				case 0x0666:
				case 0x0667:
				case 0x0668:
				case 0x0669:
					ch -= 0x05B0;
					break;
				case 0x066A: ch = 0xA5; break;
				case 0x0679: ch = 0xF4; break;
				case 0x067E: ch = 0xF3; break;
				case 0x0686: ch = 0xF5; break;
				case 0x0688: ch = 0xF9; break;
				case 0x0691: ch = 0xFA; break;
				case 0x0698: ch = 0xFE; break;
				case 0x06A4: ch = 0xF7; break;
				case 0x06AF: ch = 0xF8; break;
				case 0x06BA: ch = 0x8B; break;
				case 0x06D2: ch = 0xFF; break;
				case 0x06D5: ch = 0xF6; break;
				case 0x2026: ch = 0x93; break;
				case 0x274A: ch = 0xC0; break;
				default: ch = 0x3F; break;
			}
			bytes[byteIndex++] = (byte)ch;
			--charCount;
		}
	}

}; // class CP10004

public class ENCwindows_10004 : CP10004
{
	public ENCwindows_10004() : base() {}

}; // class ENCwindows_10004

}; // namespace I18N.MidEast
