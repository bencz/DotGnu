/*
 * CP775.cs - OEM Baltic code page.
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

// Generated from "oem-775.ucm".

namespace I18N.West
{

using System;
using I18N.Common;

public class CP775 : ByteEncoding
{
	public CP775()
		: base(775, ToChars, "OEM Baltic",
		       "iso-8859-4", "windows-775", "windows-775",
		       false, false, false, false, 1257)
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
		'\u007E', '\u007F', '\u0106', '\u00FC', '\u00E9', '\u0101', 
		'\u00E4', '\u0123', '\u00E5', '\u0107', '\u0142', '\u0113', 
		'\u0156', '\u0157', '\u012B', '\u0179', '\u00C4', '\u00C5', 
		'\u00C9', '\u00E6', '\u00C6', '\u014D', '\u00F6', '\u0122', 
		'\u00A2', '\u015A', '\u015B', '\u00D6', '\u00DC', '\u00F8', 
		'\u00A3', '\u00D8', '\u00D7', '\u00A4', '\u0100', '\u012A', 
		'\u00F3', '\u017B', '\u017C', '\u017A', '\u201D', '\u00A6', 
		'\u00A9', '\u00AE', '\u00AC', '\u00BD', '\u00BC', '\u0141', 
		'\u00AB', '\u00BB', '\u2591', '\u2592', '\u2593', '\u2502', 
		'\u2524', '\u0104', '\u010C', '\u0118', '\u0116', '\u2563', 
		'\u2551', '\u2557', '\u255D', '\u012E', '\u0160', '\u2510', 
		'\u2514', '\u2534', '\u252C', '\u251C', '\u2500', '\u253C', 
		'\u0172', '\u016A', '\u255A', '\u2554', '\u2569', '\u2566', 
		'\u2560', '\u2550', '\u256C', '\u017D', '\u0105', '\u010D', 
		'\u0119', '\u0117', '\u012F', '\u0161', '\u0173', '\u016B', 
		'\u017E', '\u2518', '\u250C', '\u2588', '\u2584', '\u258C', 
		'\u2590', '\u2580', '\u00D3', '\u00DF', '\u014C', '\u0143', 
		'\u00F5', '\u00D5', '\u00B5', '\u0144', '\u0136', '\u0137', 
		'\u013B', '\u013C', '\u0146', '\u0112', '\u0145', '\u2019', 
		'\u00AD', '\u00B1', '\u201C', '\u00BE', '\u00B6', '\u00A7', 
		'\u00F7', '\u201E', '\u00B0', '\u2219', '\u00B7', '\u00B9', 
		'\u00B3', '\u00B2', '\u25A0', '\u00A0', 
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
				case 0x00A0: ch = 0xFF; break;
				case 0x00A2: ch = 0x96; break;
				case 0x00A3: ch = 0x9C; break;
				case 0x00A4: ch = 0x9F; break;
				case 0x00A6: ch = 0xA7; break;
				case 0x00A7: ch = 0xF5; break;
				case 0x00A9: ch = 0xA8; break;
				case 0x00AB: ch = 0xAE; break;
				case 0x00AC: ch = 0xAA; break;
				case 0x00AD: ch = 0xF0; break;
				case 0x00AE: ch = 0xA9; break;
				case 0x00B0: ch = 0xF8; break;
				case 0x00B1: ch = 0xF1; break;
				case 0x00B2: ch = 0xFD; break;
				case 0x00B3: ch = 0xFC; break;
				case 0x00B5: ch = 0xE6; break;
				case 0x00B6: ch = 0xF4; break;
				case 0x00B7: ch = 0xFA; break;
				case 0x00B9: ch = 0xFB; break;
				case 0x00BB: ch = 0xAF; break;
				case 0x00BC: ch = 0xAC; break;
				case 0x00BD: ch = 0xAB; break;
				case 0x00BE: ch = 0xF3; break;
				case 0x00C4: ch = 0x8E; break;
				case 0x00C5: ch = 0x8F; break;
				case 0x00C6: ch = 0x92; break;
				case 0x00C9: ch = 0x90; break;
				case 0x00D3: ch = 0xE0; break;
				case 0x00D5: ch = 0xE5; break;
				case 0x00D6: ch = 0x99; break;
				case 0x00D7: ch = 0x9E; break;
				case 0x00D8: ch = 0x9D; break;
				case 0x00DC: ch = 0x9A; break;
				case 0x00DF: ch = 0xE1; break;
				case 0x00E4: ch = 0x84; break;
				case 0x00E5: ch = 0x86; break;
				case 0x00E6: ch = 0x91; break;
				case 0x00E9: ch = 0x82; break;
				case 0x00F3: ch = 0xA2; break;
				case 0x00F5: ch = 0xE4; break;
				case 0x00F6: ch = 0x94; break;
				case 0x00F7: ch = 0xF6; break;
				case 0x00F8: ch = 0x9B; break;
				case 0x00FC: ch = 0x81; break;
				case 0x0100: ch = 0xA0; break;
				case 0x0101: ch = 0x83; break;
				case 0x0104: ch = 0xB5; break;
				case 0x0105: ch = 0xD0; break;
				case 0x0106: ch = 0x80; break;
				case 0x0107: ch = 0x87; break;
				case 0x010C: ch = 0xB6; break;
				case 0x010D: ch = 0xD1; break;
				case 0x0112: ch = 0xED; break;
				case 0x0113: ch = 0x89; break;
				case 0x0116: ch = 0xB8; break;
				case 0x0117: ch = 0xD3; break;
				case 0x0118: ch = 0xB7; break;
				case 0x0119: ch = 0xD2; break;
				case 0x0122: ch = 0x95; break;
				case 0x0123: ch = 0x85; break;
				case 0x012A: ch = 0xA1; break;
				case 0x012B: ch = 0x8C; break;
				case 0x012E: ch = 0xBD; break;
				case 0x012F: ch = 0xD4; break;
				case 0x0136: ch = 0xE8; break;
				case 0x0137: ch = 0xE9; break;
				case 0x013B: ch = 0xEA; break;
				case 0x013C: ch = 0xEB; break;
				case 0x0141: ch = 0xAD; break;
				case 0x0142: ch = 0x88; break;
				case 0x0143: ch = 0xE3; break;
				case 0x0144: ch = 0xE7; break;
				case 0x0145: ch = 0xEE; break;
				case 0x0146: ch = 0xEC; break;
				case 0x014C: ch = 0xE2; break;
				case 0x014D: ch = 0x93; break;
				case 0x0156: ch = 0x8A; break;
				case 0x0157: ch = 0x8B; break;
				case 0x015A: ch = 0x97; break;
				case 0x015B: ch = 0x98; break;
				case 0x0160: ch = 0xBE; break;
				case 0x0161: ch = 0xD5; break;
				case 0x016A: ch = 0xC7; break;
				case 0x016B: ch = 0xD7; break;
				case 0x0172: ch = 0xC6; break;
				case 0x0173: ch = 0xD6; break;
				case 0x0179: ch = 0x8D; break;
				case 0x017A: ch = 0xA5; break;
				case 0x017B: ch = 0xA3; break;
				case 0x017C: ch = 0xA4; break;
				case 0x017D: ch = 0xCF; break;
				case 0x017E: ch = 0xD8; break;
				case 0x2019: ch = 0xEF; break;
				case 0x201C: ch = 0xF2; break;
				case 0x201D: ch = 0xA6; break;
				case 0x201E: ch = 0xF7; break;
				case 0x2219: ch = 0xF9; break;
				case 0x2500: ch = 0xC4; break;
				case 0x2502: ch = 0xB3; break;
				case 0x250C: ch = 0xDA; break;
				case 0x2510: ch = 0xBF; break;
				case 0x2514: ch = 0xC0; break;
				case 0x2518: ch = 0xD9; break;
				case 0x251C: ch = 0xC3; break;
				case 0x2524: ch = 0xB4; break;
				case 0x252C: ch = 0xC2; break;
				case 0x2534: ch = 0xC1; break;
				case 0x253C: ch = 0xC5; break;
				case 0x2550: ch = 0xCD; break;
				case 0x2551: ch = 0xBA; break;
				case 0x2554: ch = 0xC9; break;
				case 0x2557: ch = 0xBB; break;
				case 0x255A: ch = 0xC8; break;
				case 0x255D: ch = 0xBC; break;
				case 0x2560: ch = 0xCC; break;
				case 0x2563: ch = 0xB9; break;
				case 0x2566: ch = 0xCB; break;
				case 0x2569: ch = 0xCA; break;
				case 0x256C: ch = 0xCE; break;
				case 0x2580: ch = 0xDF; break;
				case 0x2584: ch = 0xDC; break;
				case 0x2588: ch = 0xDB; break;
				case 0x258C: ch = 0xDD; break;
				case 0x2590: ch = 0xDE; break;
				case 0x2591: ch = 0xB0; break;
				case 0x2592: ch = 0xB1; break;
				case 0x2593: ch = 0xB2; break;
				case 0x25A0: ch = 0xFE; break;
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
			if(ch >= 128) switch(ch)
			{
				case 0x00A0: ch = 0xFF; break;
				case 0x00A2: ch = 0x96; break;
				case 0x00A3: ch = 0x9C; break;
				case 0x00A4: ch = 0x9F; break;
				case 0x00A6: ch = 0xA7; break;
				case 0x00A7: ch = 0xF5; break;
				case 0x00A9: ch = 0xA8; break;
				case 0x00AB: ch = 0xAE; break;
				case 0x00AC: ch = 0xAA; break;
				case 0x00AD: ch = 0xF0; break;
				case 0x00AE: ch = 0xA9; break;
				case 0x00B0: ch = 0xF8; break;
				case 0x00B1: ch = 0xF1; break;
				case 0x00B2: ch = 0xFD; break;
				case 0x00B3: ch = 0xFC; break;
				case 0x00B5: ch = 0xE6; break;
				case 0x00B6: ch = 0xF4; break;
				case 0x00B7: ch = 0xFA; break;
				case 0x00B9: ch = 0xFB; break;
				case 0x00BB: ch = 0xAF; break;
				case 0x00BC: ch = 0xAC; break;
				case 0x00BD: ch = 0xAB; break;
				case 0x00BE: ch = 0xF3; break;
				case 0x00C4: ch = 0x8E; break;
				case 0x00C5: ch = 0x8F; break;
				case 0x00C6: ch = 0x92; break;
				case 0x00C9: ch = 0x90; break;
				case 0x00D3: ch = 0xE0; break;
				case 0x00D5: ch = 0xE5; break;
				case 0x00D6: ch = 0x99; break;
				case 0x00D7: ch = 0x9E; break;
				case 0x00D8: ch = 0x9D; break;
				case 0x00DC: ch = 0x9A; break;
				case 0x00DF: ch = 0xE1; break;
				case 0x00E4: ch = 0x84; break;
				case 0x00E5: ch = 0x86; break;
				case 0x00E6: ch = 0x91; break;
				case 0x00E9: ch = 0x82; break;
				case 0x00F3: ch = 0xA2; break;
				case 0x00F5: ch = 0xE4; break;
				case 0x00F6: ch = 0x94; break;
				case 0x00F7: ch = 0xF6; break;
				case 0x00F8: ch = 0x9B; break;
				case 0x00FC: ch = 0x81; break;
				case 0x0100: ch = 0xA0; break;
				case 0x0101: ch = 0x83; break;
				case 0x0104: ch = 0xB5; break;
				case 0x0105: ch = 0xD0; break;
				case 0x0106: ch = 0x80; break;
				case 0x0107: ch = 0x87; break;
				case 0x010C: ch = 0xB6; break;
				case 0x010D: ch = 0xD1; break;
				case 0x0112: ch = 0xED; break;
				case 0x0113: ch = 0x89; break;
				case 0x0116: ch = 0xB8; break;
				case 0x0117: ch = 0xD3; break;
				case 0x0118: ch = 0xB7; break;
				case 0x0119: ch = 0xD2; break;
				case 0x0122: ch = 0x95; break;
				case 0x0123: ch = 0x85; break;
				case 0x012A: ch = 0xA1; break;
				case 0x012B: ch = 0x8C; break;
				case 0x012E: ch = 0xBD; break;
				case 0x012F: ch = 0xD4; break;
				case 0x0136: ch = 0xE8; break;
				case 0x0137: ch = 0xE9; break;
				case 0x013B: ch = 0xEA; break;
				case 0x013C: ch = 0xEB; break;
				case 0x0141: ch = 0xAD; break;
				case 0x0142: ch = 0x88; break;
				case 0x0143: ch = 0xE3; break;
				case 0x0144: ch = 0xE7; break;
				case 0x0145: ch = 0xEE; break;
				case 0x0146: ch = 0xEC; break;
				case 0x014C: ch = 0xE2; break;
				case 0x014D: ch = 0x93; break;
				case 0x0156: ch = 0x8A; break;
				case 0x0157: ch = 0x8B; break;
				case 0x015A: ch = 0x97; break;
				case 0x015B: ch = 0x98; break;
				case 0x0160: ch = 0xBE; break;
				case 0x0161: ch = 0xD5; break;
				case 0x016A: ch = 0xC7; break;
				case 0x016B: ch = 0xD7; break;
				case 0x0172: ch = 0xC6; break;
				case 0x0173: ch = 0xD6; break;
				case 0x0179: ch = 0x8D; break;
				case 0x017A: ch = 0xA5; break;
				case 0x017B: ch = 0xA3; break;
				case 0x017C: ch = 0xA4; break;
				case 0x017D: ch = 0xCF; break;
				case 0x017E: ch = 0xD8; break;
				case 0x2019: ch = 0xEF; break;
				case 0x201C: ch = 0xF2; break;
				case 0x201D: ch = 0xA6; break;
				case 0x201E: ch = 0xF7; break;
				case 0x2219: ch = 0xF9; break;
				case 0x2500: ch = 0xC4; break;
				case 0x2502: ch = 0xB3; break;
				case 0x250C: ch = 0xDA; break;
				case 0x2510: ch = 0xBF; break;
				case 0x2514: ch = 0xC0; break;
				case 0x2518: ch = 0xD9; break;
				case 0x251C: ch = 0xC3; break;
				case 0x2524: ch = 0xB4; break;
				case 0x252C: ch = 0xC2; break;
				case 0x2534: ch = 0xC1; break;
				case 0x253C: ch = 0xC5; break;
				case 0x2550: ch = 0xCD; break;
				case 0x2551: ch = 0xBA; break;
				case 0x2554: ch = 0xC9; break;
				case 0x2557: ch = 0xBB; break;
				case 0x255A: ch = 0xC8; break;
				case 0x255D: ch = 0xBC; break;
				case 0x2560: ch = 0xCC; break;
				case 0x2563: ch = 0xB9; break;
				case 0x2566: ch = 0xCB; break;
				case 0x2569: ch = 0xCA; break;
				case 0x256C: ch = 0xCE; break;
				case 0x2580: ch = 0xDF; break;
				case 0x2584: ch = 0xDC; break;
				case 0x2588: ch = 0xDB; break;
				case 0x258C: ch = 0xDD; break;
				case 0x2590: ch = 0xDE; break;
				case 0x2591: ch = 0xB0; break;
				case 0x2592: ch = 0xB1; break;
				case 0x2593: ch = 0xB2; break;
				case 0x25A0: ch = 0xFE; break;
				default: ch = 0x3F; break;
			}
			bytes[byteIndex++] = (byte)ch;
			--charCount;
		}
	}

}; // class CP775

public class ENCwindows_775 : CP775
{
	public ENCwindows_775() : base() {}

}; // class ENCwindows_775

}; // namespace I18N.West
