/*
 * CP737.cs - OEM Greek code page.
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

// Generated from "oem-737.ucm".

namespace I18N.West
{

using System;
using I18N.Common;

public class CP737 : ByteEncoding
{
	public CP737()
		: base(737, ToChars, "OEM Greek",
		       "iso-8859-7", "windows-737", "windows-737",
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
		'\u007E', '\u007F', '\u0391', '\u0392', '\u0393', '\u0394', 
		'\u0395', '\u0396', '\u0397', '\u0398', '\u0399', '\u039A', 
		'\u039B', '\u039C', '\u039D', '\u039E', '\u039F', '\u03A0', 
		'\u03A1', '\u03A3', '\u03A4', '\u03A5', '\u03A6', '\u03A7', 
		'\u03A8', '\u03A9', '\u03B1', '\u03B2', '\u03B3', '\u03B4', 
		'\u03B5', '\u03B6', '\u03B7', '\u03B8', '\u03B9', '\u03BA', 
		'\u03BB', '\u03BC', '\u03BD', '\u03BE', '\u03BF', '\u03C0', 
		'\u03C1', '\u03C3', '\u03C2', '\u03C4', '\u03C5', '\u03C6', 
		'\u03C7', '\u03C8', '\u2591', '\u2592', '\u2593', '\u2502', 
		'\u2524', '\u2561', '\u2562', '\u2556', '\u2555', '\u2563', 
		'\u2551', '\u2557', '\u255D', '\u255C', '\u255B', '\u2510', 
		'\u2514', '\u2534', '\u252C', '\u251C', '\u2500', '\u253C', 
		'\u255E', '\u255F', '\u255A', '\u2554', '\u2569', '\u2566', 
		'\u2560', '\u2550', '\u256C', '\u2567', '\u2568', '\u2564', 
		'\u2565', '\u2559', '\u2558', '\u2552', '\u2553', '\u256B', 
		'\u256A', '\u2518', '\u250C', '\u2588', '\u2584', '\u258C', 
		'\u2590', '\u2580', '\u03C9', '\u03AC', '\u03AD', '\u03AE', 
		'\u03CA', '\u03AF', '\u03CC', '\u03CD', '\u03CB', '\u03CE', 
		'\u0386', '\u0388', '\u0389', '\u038A', '\u038C', '\u038E', 
		'\u038F', '\u00B1', '\u2265', '\u2264', '\u03AA', '\u03AB', 
		'\u00F7', '\u2248', '\u00B0', '\u2219', '\u00B7', '\u221A', 
		'\u207F', '\u00B2', '\u25A0', '\u00A0', 
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
				case 0x00B0: ch = 0xF8; break;
				case 0x00B1: ch = 0xF1; break;
				case 0x00B2: ch = 0xFD; break;
				case 0x00B7: ch = 0xFA; break;
				case 0x00F7: ch = 0xF6; break;
				case 0x0386: ch = 0xEA; break;
				case 0x0388: ch = 0xEB; break;
				case 0x0389: ch = 0xEC; break;
				case 0x038A: ch = 0xED; break;
				case 0x038C: ch = 0xEE; break;
				case 0x038E: ch = 0xEF; break;
				case 0x038F: ch = 0xF0; break;
				case 0x0391:
				case 0x0392:
				case 0x0393:
				case 0x0394:
				case 0x0395:
				case 0x0396:
				case 0x0397:
				case 0x0398:
				case 0x0399:
				case 0x039A:
				case 0x039B:
				case 0x039C:
				case 0x039D:
				case 0x039E:
				case 0x039F:
				case 0x03A0:
				case 0x03A1:
					ch -= 0x0311;
					break;
				case 0x03A3:
				case 0x03A4:
				case 0x03A5:
				case 0x03A6:
				case 0x03A7:
				case 0x03A8:
				case 0x03A9:
					ch -= 0x0312;
					break;
				case 0x03AA: ch = 0xF4; break;
				case 0x03AB: ch = 0xF5; break;
				case 0x03AC: ch = 0xE1; break;
				case 0x03AD: ch = 0xE2; break;
				case 0x03AE: ch = 0xE3; break;
				case 0x03AF: ch = 0xE5; break;
				case 0x03B1:
				case 0x03B2:
				case 0x03B3:
				case 0x03B4:
				case 0x03B5:
				case 0x03B6:
				case 0x03B7:
				case 0x03B8:
				case 0x03B9:
				case 0x03BA:
				case 0x03BB:
				case 0x03BC:
				case 0x03BD:
				case 0x03BE:
				case 0x03BF:
				case 0x03C0:
				case 0x03C1:
					ch -= 0x0319;
					break;
				case 0x03C2: ch = 0xAA; break;
				case 0x03C3: ch = 0xA9; break;
				case 0x03C4:
				case 0x03C5:
				case 0x03C6:
				case 0x03C7:
				case 0x03C8:
					ch -= 0x0319;
					break;
				case 0x03C9: ch = 0xE0; break;
				case 0x03CA: ch = 0xE4; break;
				case 0x03CB: ch = 0xE8; break;
				case 0x03CC: ch = 0xE6; break;
				case 0x03CD: ch = 0xE7; break;
				case 0x03CE: ch = 0xE9; break;
				case 0x207F: ch = 0xFC; break;
				case 0x2219: ch = 0xF9; break;
				case 0x221A: ch = 0xFB; break;
				case 0x2248: ch = 0xF7; break;
				case 0x2264: ch = 0xF3; break;
				case 0x2265: ch = 0xF2; break;
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
				case 0x2552: ch = 0xD5; break;
				case 0x2553: ch = 0xD6; break;
				case 0x2554: ch = 0xC9; break;
				case 0x2555: ch = 0xB8; break;
				case 0x2556: ch = 0xB7; break;
				case 0x2557: ch = 0xBB; break;
				case 0x2558: ch = 0xD4; break;
				case 0x2559: ch = 0xD3; break;
				case 0x255A: ch = 0xC8; break;
				case 0x255B: ch = 0xBE; break;
				case 0x255C: ch = 0xBD; break;
				case 0x255D: ch = 0xBC; break;
				case 0x255E: ch = 0xC6; break;
				case 0x255F: ch = 0xC7; break;
				case 0x2560: ch = 0xCC; break;
				case 0x2561: ch = 0xB5; break;
				case 0x2562: ch = 0xB6; break;
				case 0x2563: ch = 0xB9; break;
				case 0x2564: ch = 0xD1; break;
				case 0x2565: ch = 0xD2; break;
				case 0x2566: ch = 0xCB; break;
				case 0x2567: ch = 0xCF; break;
				case 0x2568: ch = 0xD0; break;
				case 0x2569: ch = 0xCA; break;
				case 0x256A: ch = 0xD8; break;
				case 0x256B: ch = 0xD7; break;
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
				case 0x00B0: ch = 0xF8; break;
				case 0x00B1: ch = 0xF1; break;
				case 0x00B2: ch = 0xFD; break;
				case 0x00B7: ch = 0xFA; break;
				case 0x00F7: ch = 0xF6; break;
				case 0x0386: ch = 0xEA; break;
				case 0x0388: ch = 0xEB; break;
				case 0x0389: ch = 0xEC; break;
				case 0x038A: ch = 0xED; break;
				case 0x038C: ch = 0xEE; break;
				case 0x038E: ch = 0xEF; break;
				case 0x038F: ch = 0xF0; break;
				case 0x0391:
				case 0x0392:
				case 0x0393:
				case 0x0394:
				case 0x0395:
				case 0x0396:
				case 0x0397:
				case 0x0398:
				case 0x0399:
				case 0x039A:
				case 0x039B:
				case 0x039C:
				case 0x039D:
				case 0x039E:
				case 0x039F:
				case 0x03A0:
				case 0x03A1:
					ch -= 0x0311;
					break;
				case 0x03A3:
				case 0x03A4:
				case 0x03A5:
				case 0x03A6:
				case 0x03A7:
				case 0x03A8:
				case 0x03A9:
					ch -= 0x0312;
					break;
				case 0x03AA: ch = 0xF4; break;
				case 0x03AB: ch = 0xF5; break;
				case 0x03AC: ch = 0xE1; break;
				case 0x03AD: ch = 0xE2; break;
				case 0x03AE: ch = 0xE3; break;
				case 0x03AF: ch = 0xE5; break;
				case 0x03B1:
				case 0x03B2:
				case 0x03B3:
				case 0x03B4:
				case 0x03B5:
				case 0x03B6:
				case 0x03B7:
				case 0x03B8:
				case 0x03B9:
				case 0x03BA:
				case 0x03BB:
				case 0x03BC:
				case 0x03BD:
				case 0x03BE:
				case 0x03BF:
				case 0x03C0:
				case 0x03C1:
					ch -= 0x0319;
					break;
				case 0x03C2: ch = 0xAA; break;
				case 0x03C3: ch = 0xA9; break;
				case 0x03C4:
				case 0x03C5:
				case 0x03C6:
				case 0x03C7:
				case 0x03C8:
					ch -= 0x0319;
					break;
				case 0x03C9: ch = 0xE0; break;
				case 0x03CA: ch = 0xE4; break;
				case 0x03CB: ch = 0xE8; break;
				case 0x03CC: ch = 0xE6; break;
				case 0x03CD: ch = 0xE7; break;
				case 0x03CE: ch = 0xE9; break;
				case 0x207F: ch = 0xFC; break;
				case 0x2219: ch = 0xF9; break;
				case 0x221A: ch = 0xFB; break;
				case 0x2248: ch = 0xF7; break;
				case 0x2264: ch = 0xF3; break;
				case 0x2265: ch = 0xF2; break;
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
				case 0x2552: ch = 0xD5; break;
				case 0x2553: ch = 0xD6; break;
				case 0x2554: ch = 0xC9; break;
				case 0x2555: ch = 0xB8; break;
				case 0x2556: ch = 0xB7; break;
				case 0x2557: ch = 0xBB; break;
				case 0x2558: ch = 0xD4; break;
				case 0x2559: ch = 0xD3; break;
				case 0x255A: ch = 0xC8; break;
				case 0x255B: ch = 0xBE; break;
				case 0x255C: ch = 0xBD; break;
				case 0x255D: ch = 0xBC; break;
				case 0x255E: ch = 0xC6; break;
				case 0x255F: ch = 0xC7; break;
				case 0x2560: ch = 0xCC; break;
				case 0x2561: ch = 0xB5; break;
				case 0x2562: ch = 0xB6; break;
				case 0x2563: ch = 0xB9; break;
				case 0x2564: ch = 0xD1; break;
				case 0x2565: ch = 0xD2; break;
				case 0x2566: ch = 0xCB; break;
				case 0x2567: ch = 0xCF; break;
				case 0x2568: ch = 0xD0; break;
				case 0x2569: ch = 0xCA; break;
				case 0x256A: ch = 0xD8; break;
				case 0x256B: ch = 0xD7; break;
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

}; // class CP737

public class ENCwindows_737 : CP737
{
	public ENCwindows_737() : base() {}

}; // class ENCwindows_737

}; // namespace I18N.West
