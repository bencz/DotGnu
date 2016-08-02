/*
 * CP10006.cs - Greek (Mac) code page.
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

// Generated from "mac-10006.ucm".

namespace I18N.West
{

using System;
using I18N.Common;

public class CP10006 : ByteEncoding
{
	public CP10006()
		: base(10006, ToChars, "Greek (Mac)",
		       "windows-10006", "windows-10006", "windows-10006",
		       false, false, false, false, 1253)
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
		'\u007E', '\u007F', '\u00C4', '\u00B9', '\u00B2', '\u00C9', 
		'\u00B3', '\u00D6', '\u00DC', '\u0385', '\u00E0', '\u00E2', 
		'\u00E4', '\u0384', '\u00A8', '\u00E7', '\u00E9', '\u00E8', 
		'\u00EA', '\u00EB', '\u00A3', '\u2122', '\u00EE', '\u00EF', 
		'\u2022', '\u00BD', '\u2030', '\u00F4', '\u00F6', '\u00A6', 
		'\u00AD', '\u00F9', '\u00FB', '\u00FC', '\u2020', '\u0393', 
		'\u0394', '\u0398', '\u039B', '\u039E', '\u03A0', '\u00DF', 
		'\u00AE', '\u00A9', '\u03A3', '\u03AA', '\u00A7', '\u2260', 
		'\u00B0', '\u0387', '\u0391', '\u00B1', '\u2264', '\u2265', 
		'\u00A5', '\u0392', '\u0395', '\u0396', '\u0397', '\u0399', 
		'\u039A', '\u039C', '\u03A6', '\u03AB', '\u03A8', '\u03A9', 
		'\u03AC', '\u039D', '\u00AC', '\u039F', '\u03A1', '\u2248', 
		'\u03A4', '\u00AB', '\u00BB', '\u2026', '\u00A0', '\u03A5', 
		'\u03A7', '\u0386', '\u0388', '\u0153', '\u2013', '\u2015', 
		'\u201C', '\u201D', '\u2018', '\u2019', '\u00F7', '\u0389', 
		'\u038A', '\u038C', '\u038E', '\u03AD', '\u03AE', '\u03AF', 
		'\u03CC', '\u038F', '\u03CD', '\u03B1', '\u03B2', '\u03C8', 
		'\u03B4', '\u03B5', '\u03C6', '\u03B3', '\u03B7', '\u03B9', 
		'\u03BE', '\u03BA', '\u03BB', '\u03BC', '\u03BD', '\u03BF', 
		'\u03C0', '\u03CE', '\u03C1', '\u03C3', '\u03C4', '\u03B8', 
		'\u03C9', '\u03C2', '\u03C7', '\u03C5', '\u03B6', '\u03CA', 
		'\u03CB', '\u0390', '\u03B0', '\u00FF', 
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
				case 0x00A9:
				case 0x00B1:
				case 0x00FF:
					break;
				case 0x00A0: ch = 0xCA; break;
				case 0x00A3: ch = 0x92; break;
				case 0x00A5: ch = 0xB4; break;
				case 0x00A6: ch = 0x9B; break;
				case 0x00A7: ch = 0xAC; break;
				case 0x00A8: ch = 0x8C; break;
				case 0x00AB: ch = 0xC7; break;
				case 0x00AC: ch = 0xC2; break;
				case 0x00AD: ch = 0x9C; break;
				case 0x00AE: ch = 0xA8; break;
				case 0x00B0: ch = 0xAE; break;
				case 0x00B2: ch = 0x82; break;
				case 0x00B3: ch = 0x84; break;
				case 0x00B9: ch = 0x81; break;
				case 0x00BB: ch = 0xC8; break;
				case 0x00BD: ch = 0x97; break;
				case 0x00C4: ch = 0x80; break;
				case 0x00C9: ch = 0x83; break;
				case 0x00D6: ch = 0x85; break;
				case 0x00DC: ch = 0x86; break;
				case 0x00DF: ch = 0xA7; break;
				case 0x00E0: ch = 0x88; break;
				case 0x00E2: ch = 0x89; break;
				case 0x00E4: ch = 0x8A; break;
				case 0x00E7: ch = 0x8D; break;
				case 0x00E8: ch = 0x8F; break;
				case 0x00E9: ch = 0x8E; break;
				case 0x00EA: ch = 0x90; break;
				case 0x00EB: ch = 0x91; break;
				case 0x00EE: ch = 0x94; break;
				case 0x00EF: ch = 0x95; break;
				case 0x00F4: ch = 0x99; break;
				case 0x00F6: ch = 0x9A; break;
				case 0x00F7: ch = 0xD6; break;
				case 0x00F9: ch = 0x9D; break;
				case 0x00FB: ch = 0x9E; break;
				case 0x00FC: ch = 0x9F; break;
				case 0x0153: ch = 0xCF; break;
				case 0x0384: ch = 0x8B; break;
				case 0x0385: ch = 0x87; break;
				case 0x0386: ch = 0xCD; break;
				case 0x0387: ch = 0xAF; break;
				case 0x0388: ch = 0xCE; break;
				case 0x0389: ch = 0xD7; break;
				case 0x038A: ch = 0xD8; break;
				case 0x038C: ch = 0xD9; break;
				case 0x038E: ch = 0xDA; break;
				case 0x038F: ch = 0xDF; break;
				case 0x0390: ch = 0xFD; break;
				case 0x0391: ch = 0xB0; break;
				case 0x0392: ch = 0xB5; break;
				case 0x0393: ch = 0xA1; break;
				case 0x0394: ch = 0xA2; break;
				case 0x0395: ch = 0xB6; break;
				case 0x0396: ch = 0xB7; break;
				case 0x0397: ch = 0xB8; break;
				case 0x0398: ch = 0xA3; break;
				case 0x0399: ch = 0xB9; break;
				case 0x039A: ch = 0xBA; break;
				case 0x039B: ch = 0xA4; break;
				case 0x039C: ch = 0xBB; break;
				case 0x039D: ch = 0xC1; break;
				case 0x039E: ch = 0xA5; break;
				case 0x039F: ch = 0xC3; break;
				case 0x03A0: ch = 0xA6; break;
				case 0x03A1: ch = 0xC4; break;
				case 0x03A3: ch = 0xAA; break;
				case 0x03A4: ch = 0xC6; break;
				case 0x03A5: ch = 0xCB; break;
				case 0x03A6: ch = 0xBC; break;
				case 0x03A7: ch = 0xCC; break;
				case 0x03A8: ch = 0xBE; break;
				case 0x03A9: ch = 0xBF; break;
				case 0x03AA: ch = 0xAB; break;
				case 0x03AB: ch = 0xBD; break;
				case 0x03AC: ch = 0xC0; break;
				case 0x03AD: ch = 0xDB; break;
				case 0x03AE: ch = 0xDC; break;
				case 0x03AF: ch = 0xDD; break;
				case 0x03B0: ch = 0xFE; break;
				case 0x03B1: ch = 0xE1; break;
				case 0x03B2: ch = 0xE2; break;
				case 0x03B3: ch = 0xE7; break;
				case 0x03B4: ch = 0xE4; break;
				case 0x03B5: ch = 0xE5; break;
				case 0x03B6: ch = 0xFA; break;
				case 0x03B7: ch = 0xE8; break;
				case 0x03B8: ch = 0xF5; break;
				case 0x03B9: ch = 0xE9; break;
				case 0x03BA:
				case 0x03BB:
				case 0x03BC:
				case 0x03BD:
					ch -= 0x02CF;
					break;
				case 0x03BE: ch = 0xEA; break;
				case 0x03BF: ch = 0xEF; break;
				case 0x03C0: ch = 0xF0; break;
				case 0x03C1: ch = 0xF2; break;
				case 0x03C2: ch = 0xF7; break;
				case 0x03C3: ch = 0xF3; break;
				case 0x03C4: ch = 0xF4; break;
				case 0x03C5: ch = 0xF9; break;
				case 0x03C6: ch = 0xE6; break;
				case 0x03C7: ch = 0xF8; break;
				case 0x03C8: ch = 0xE3; break;
				case 0x03C9: ch = 0xF6; break;
				case 0x03CA: ch = 0xFB; break;
				case 0x03CB: ch = 0xFC; break;
				case 0x03CC: ch = 0xDE; break;
				case 0x03CD: ch = 0xE0; break;
				case 0x03CE: ch = 0xF1; break;
				case 0x2013: ch = 0xD0; break;
				case 0x2015: ch = 0xD1; break;
				case 0x2018: ch = 0xD4; break;
				case 0x2019: ch = 0xD5; break;
				case 0x201C: ch = 0xD2; break;
				case 0x201D: ch = 0xD3; break;
				case 0x2020: ch = 0xA0; break;
				case 0x2022: ch = 0x96; break;
				case 0x2026: ch = 0xC9; break;
				case 0x2030: ch = 0x98; break;
				case 0x2122: ch = 0x93; break;
				case 0x2248: ch = 0xC5; break;
				case 0x2260: ch = 0xAD; break;
				case 0x2264: ch = 0xB2; break;
				case 0x2265: ch = 0xB3; break;
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
				case 0x00A9:
				case 0x00B1:
				case 0x00FF:
					break;
				case 0x00A0: ch = 0xCA; break;
				case 0x00A3: ch = 0x92; break;
				case 0x00A5: ch = 0xB4; break;
				case 0x00A6: ch = 0x9B; break;
				case 0x00A7: ch = 0xAC; break;
				case 0x00A8: ch = 0x8C; break;
				case 0x00AB: ch = 0xC7; break;
				case 0x00AC: ch = 0xC2; break;
				case 0x00AD: ch = 0x9C; break;
				case 0x00AE: ch = 0xA8; break;
				case 0x00B0: ch = 0xAE; break;
				case 0x00B2: ch = 0x82; break;
				case 0x00B3: ch = 0x84; break;
				case 0x00B9: ch = 0x81; break;
				case 0x00BB: ch = 0xC8; break;
				case 0x00BD: ch = 0x97; break;
				case 0x00C4: ch = 0x80; break;
				case 0x00C9: ch = 0x83; break;
				case 0x00D6: ch = 0x85; break;
				case 0x00DC: ch = 0x86; break;
				case 0x00DF: ch = 0xA7; break;
				case 0x00E0: ch = 0x88; break;
				case 0x00E2: ch = 0x89; break;
				case 0x00E4: ch = 0x8A; break;
				case 0x00E7: ch = 0x8D; break;
				case 0x00E8: ch = 0x8F; break;
				case 0x00E9: ch = 0x8E; break;
				case 0x00EA: ch = 0x90; break;
				case 0x00EB: ch = 0x91; break;
				case 0x00EE: ch = 0x94; break;
				case 0x00EF: ch = 0x95; break;
				case 0x00F4: ch = 0x99; break;
				case 0x00F6: ch = 0x9A; break;
				case 0x00F7: ch = 0xD6; break;
				case 0x00F9: ch = 0x9D; break;
				case 0x00FB: ch = 0x9E; break;
				case 0x00FC: ch = 0x9F; break;
				case 0x0153: ch = 0xCF; break;
				case 0x0384: ch = 0x8B; break;
				case 0x0385: ch = 0x87; break;
				case 0x0386: ch = 0xCD; break;
				case 0x0387: ch = 0xAF; break;
				case 0x0388: ch = 0xCE; break;
				case 0x0389: ch = 0xD7; break;
				case 0x038A: ch = 0xD8; break;
				case 0x038C: ch = 0xD9; break;
				case 0x038E: ch = 0xDA; break;
				case 0x038F: ch = 0xDF; break;
				case 0x0390: ch = 0xFD; break;
				case 0x0391: ch = 0xB0; break;
				case 0x0392: ch = 0xB5; break;
				case 0x0393: ch = 0xA1; break;
				case 0x0394: ch = 0xA2; break;
				case 0x0395: ch = 0xB6; break;
				case 0x0396: ch = 0xB7; break;
				case 0x0397: ch = 0xB8; break;
				case 0x0398: ch = 0xA3; break;
				case 0x0399: ch = 0xB9; break;
				case 0x039A: ch = 0xBA; break;
				case 0x039B: ch = 0xA4; break;
				case 0x039C: ch = 0xBB; break;
				case 0x039D: ch = 0xC1; break;
				case 0x039E: ch = 0xA5; break;
				case 0x039F: ch = 0xC3; break;
				case 0x03A0: ch = 0xA6; break;
				case 0x03A1: ch = 0xC4; break;
				case 0x03A3: ch = 0xAA; break;
				case 0x03A4: ch = 0xC6; break;
				case 0x03A5: ch = 0xCB; break;
				case 0x03A6: ch = 0xBC; break;
				case 0x03A7: ch = 0xCC; break;
				case 0x03A8: ch = 0xBE; break;
				case 0x03A9: ch = 0xBF; break;
				case 0x03AA: ch = 0xAB; break;
				case 0x03AB: ch = 0xBD; break;
				case 0x03AC: ch = 0xC0; break;
				case 0x03AD: ch = 0xDB; break;
				case 0x03AE: ch = 0xDC; break;
				case 0x03AF: ch = 0xDD; break;
				case 0x03B0: ch = 0xFE; break;
				case 0x03B1: ch = 0xE1; break;
				case 0x03B2: ch = 0xE2; break;
				case 0x03B3: ch = 0xE7; break;
				case 0x03B4: ch = 0xE4; break;
				case 0x03B5: ch = 0xE5; break;
				case 0x03B6: ch = 0xFA; break;
				case 0x03B7: ch = 0xE8; break;
				case 0x03B8: ch = 0xF5; break;
				case 0x03B9: ch = 0xE9; break;
				case 0x03BA:
				case 0x03BB:
				case 0x03BC:
				case 0x03BD:
					ch -= 0x02CF;
					break;
				case 0x03BE: ch = 0xEA; break;
				case 0x03BF: ch = 0xEF; break;
				case 0x03C0: ch = 0xF0; break;
				case 0x03C1: ch = 0xF2; break;
				case 0x03C2: ch = 0xF7; break;
				case 0x03C3: ch = 0xF3; break;
				case 0x03C4: ch = 0xF4; break;
				case 0x03C5: ch = 0xF9; break;
				case 0x03C6: ch = 0xE6; break;
				case 0x03C7: ch = 0xF8; break;
				case 0x03C8: ch = 0xE3; break;
				case 0x03C9: ch = 0xF6; break;
				case 0x03CA: ch = 0xFB; break;
				case 0x03CB: ch = 0xFC; break;
				case 0x03CC: ch = 0xDE; break;
				case 0x03CD: ch = 0xE0; break;
				case 0x03CE: ch = 0xF1; break;
				case 0x2013: ch = 0xD0; break;
				case 0x2015: ch = 0xD1; break;
				case 0x2018: ch = 0xD4; break;
				case 0x2019: ch = 0xD5; break;
				case 0x201C: ch = 0xD2; break;
				case 0x201D: ch = 0xD3; break;
				case 0x2020: ch = 0xA0; break;
				case 0x2022: ch = 0x96; break;
				case 0x2026: ch = 0xC9; break;
				case 0x2030: ch = 0x98; break;
				case 0x2122: ch = 0x93; break;
				case 0x2248: ch = 0xC5; break;
				case 0x2260: ch = 0xAD; break;
				case 0x2264: ch = 0xB2; break;
				case 0x2265: ch = 0xB3; break;
				default: ch = 0x3F; break;
			}
			bytes[byteIndex++] = (byte)ch;
			--charCount;
		}
	}

}; // class CP10006

public class ENCwindows_10006 : CP10006
{
	public ENCwindows_10006() : base() {}

}; // class ENCwindows_10006

}; // namespace I18N.West
