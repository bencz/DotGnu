/*
 * CP865.cs - Nordic (DOS) code page.
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

// Generated from "ibm-865.ucm".

namespace I18N.West
{

using System;
using I18N.Common;

public class CP865 : ByteEncoding
{
	public CP865()
		: base(865, ToChars, "Nordic (DOS)",
		       "IBM863", "IBM865", "IBM865",
		       false, false, false, false, 1252)
	{}

	private static readonly char[] ToChars = {
		'\u0000', '\u0001', '\u0002', '\u0003', '\u0004', '\u0005', 
		'\u0006', '\u0007', '\u0008', '\u0009', '\u000A', '\u000B', 
		'\u000C', '\u000D', '\u000E', '\u000F', '\u0010', '\u0011', 
		'\u0012', '\u0013', '\u0014', '\u0015', '\u0016', '\u0017', 
		'\u0018', '\u0019', '\u001C', '\u001B', '\u007F', '\u001D', 
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
		'\u007E', '\u001A', '\u00C7', '\u00FC', '\u00E9', '\u00E2', 
		'\u00E4', '\u00E0', '\u00E5', '\u00E7', '\u00EA', '\u00EB', 
		'\u00E8', '\u00EF', '\u00EE', '\u00EC', '\u00C4', '\u00C5', 
		'\u00C9', '\u00E6', '\u00C6', '\u00F4', '\u00F6', '\u00F2', 
		'\u00FB', '\u00F9', '\u00FF', '\u00D6', '\u00DC', '\u00F8', 
		'\u00A3', '\u00D8', '\u20A7', '\u0192', '\u00E1', '\u00ED', 
		'\u00F3', '\u00FA', '\u00F1', '\u00D1', '\u00AA', '\u00BA', 
		'\u00BF', '\u2310', '\u00AC', '\u00BD', '\u00BC', '\u00A1', 
		'\u00AB', '\u00A4', '\u2591', '\u2592', '\u2593', '\u2502', 
		'\u2524', '\u2561', '\u2562', '\u2556', '\u2555', '\u2563', 
		'\u2551', '\u2557', '\u255D', '\u255C', '\u255B', '\u2510', 
		'\u2514', '\u2534', '\u252C', '\u251C', '\u2500', '\u253C', 
		'\u255E', '\u255F', '\u255A', '\u2554', '\u2569', '\u2566', 
		'\u2560', '\u2550', '\u256C', '\u2567', '\u2568', '\u2564', 
		'\u2565', '\u2559', '\u2558', '\u2552', '\u2553', '\u256B', 
		'\u256A', '\u2518', '\u250C', '\u2588', '\u2584', '\u258C', 
		'\u2590', '\u2580', '\u03B1', '\u00DF', '\u0393', '\u03C0', 
		'\u03A3', '\u03C3', '\u03BC', '\u03C4', '\u03A6', '\u0398', 
		'\u03A9', '\u03B4', '\u221E', '\u03C6', '\u03B5', '\u2229', 
		'\u2261', '\u00B1', '\u2265', '\u2264', '\u2320', '\u2321', 
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
			if(ch >= 26) switch(ch)
			{
				case 0x001B:
				case 0x001D:
				case 0x001E:
				case 0x001F:
				case 0x0020:
				case 0x0021:
				case 0x0022:
				case 0x0023:
				case 0x0024:
				case 0x0025:
				case 0x0026:
				case 0x0027:
				case 0x0028:
				case 0x0029:
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
				case 0x005B:
				case 0x005C:
				case 0x005D:
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
				case 0x007B:
				case 0x007C:
				case 0x007D:
				case 0x007E:
					break;
				case 0x001A: ch = 0x7F; break;
				case 0x001C: ch = 0x1A; break;
				case 0x007F: ch = 0x1C; break;
				case 0x00A0: ch = 0xFF; break;
				case 0x00A1: ch = 0xAD; break;
				case 0x00A3: ch = 0x9C; break;
				case 0x00A4: ch = 0xAF; break;
				case 0x00A7: ch = 0x15; break;
				case 0x00AA: ch = 0xA6; break;
				case 0x00AB: ch = 0xAE; break;
				case 0x00AC: ch = 0xAA; break;
				case 0x00B0: ch = 0xF8; break;
				case 0x00B1: ch = 0xF1; break;
				case 0x00B2: ch = 0xFD; break;
				case 0x00B6: ch = 0x14; break;
				case 0x00B7: ch = 0xFA; break;
				case 0x00BA: ch = 0xA7; break;
				case 0x00BC: ch = 0xAC; break;
				case 0x00BD: ch = 0xAB; break;
				case 0x00BF: ch = 0xA8; break;
				case 0x00C4: ch = 0x8E; break;
				case 0x00C5: ch = 0x8F; break;
				case 0x00C6: ch = 0x92; break;
				case 0x00C7: ch = 0x80; break;
				case 0x00C9: ch = 0x90; break;
				case 0x00D1: ch = 0xA5; break;
				case 0x00D6: ch = 0x99; break;
				case 0x00D8: ch = 0x9D; break;
				case 0x00DC: ch = 0x9A; break;
				case 0x00DF: ch = 0xE1; break;
				case 0x00E0: ch = 0x85; break;
				case 0x00E1: ch = 0xA0; break;
				case 0x00E2: ch = 0x83; break;
				case 0x00E4: ch = 0x84; break;
				case 0x00E5: ch = 0x86; break;
				case 0x00E6: ch = 0x91; break;
				case 0x00E7: ch = 0x87; break;
				case 0x00E8: ch = 0x8A; break;
				case 0x00E9: ch = 0x82; break;
				case 0x00EA: ch = 0x88; break;
				case 0x00EB: ch = 0x89; break;
				case 0x00EC: ch = 0x8D; break;
				case 0x00ED: ch = 0xA1; break;
				case 0x00EE: ch = 0x8C; break;
				case 0x00EF: ch = 0x8B; break;
				case 0x00F1: ch = 0xA4; break;
				case 0x00F2: ch = 0x95; break;
				case 0x00F3: ch = 0xA2; break;
				case 0x00F4: ch = 0x93; break;
				case 0x00F6: ch = 0x94; break;
				case 0x00F7: ch = 0xF6; break;
				case 0x00F8: ch = 0x9B; break;
				case 0x00F9: ch = 0x97; break;
				case 0x00FA: ch = 0xA3; break;
				case 0x00FB: ch = 0x96; break;
				case 0x00FC: ch = 0x81; break;
				case 0x00FF: ch = 0x98; break;
				case 0x0192: ch = 0x9F; break;
				case 0x0393: ch = 0xE2; break;
				case 0x0398: ch = 0xE9; break;
				case 0x03A3: ch = 0xE4; break;
				case 0x03A6: ch = 0xE8; break;
				case 0x03A9: ch = 0xEA; break;
				case 0x03B1: ch = 0xE0; break;
				case 0x03B4: ch = 0xEB; break;
				case 0x03B5: ch = 0xEE; break;
				case 0x03BC: ch = 0xE6; break;
				case 0x03C0: ch = 0xE3; break;
				case 0x03C3: ch = 0xE5; break;
				case 0x03C4: ch = 0xE7; break;
				case 0x03C6: ch = 0xED; break;
				case 0x2022: ch = 0x07; break;
				case 0x203C: ch = 0x13; break;
				case 0x207F: ch = 0xFC; break;
				case 0x20A7: ch = 0x9E; break;
				case 0x2190: ch = 0x1B; break;
				case 0x2191: ch = 0x18; break;
				case 0x2192: ch = 0x1A; break;
				case 0x2193: ch = 0x19; break;
				case 0x2194: ch = 0x1D; break;
				case 0x2195: ch = 0x12; break;
				case 0x21A8: ch = 0x17; break;
				case 0x2219: ch = 0xF9; break;
				case 0x221A: ch = 0xFB; break;
				case 0x221E: ch = 0xEC; break;
				case 0x221F: ch = 0x1C; break;
				case 0x2229: ch = 0xEF; break;
				case 0x2248: ch = 0xF7; break;
				case 0x2261: ch = 0xF0; break;
				case 0x2264: ch = 0xF3; break;
				case 0x2265: ch = 0xF2; break;
				case 0x2302: ch = 0x7F; break;
				case 0x2310: ch = 0xA9; break;
				case 0x2320: ch = 0xF4; break;
				case 0x2321: ch = 0xF5; break;
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
				case 0x25AC: ch = 0x16; break;
				case 0x25B2: ch = 0x1E; break;
				case 0x25BA: ch = 0x10; break;
				case 0x25BC: ch = 0x1F; break;
				case 0x25C4: ch = 0x11; break;
				case 0x25CB: ch = 0x09; break;
				case 0x25D8: ch = 0x08; break;
				case 0x25D9: ch = 0x0A; break;
				case 0x263A: ch = 0x01; break;
				case 0x263B: ch = 0x02; break;
				case 0x263C: ch = 0x0F; break;
				case 0x2640: ch = 0x0C; break;
				case 0x2642: ch = 0x0B; break;
				case 0x2660: ch = 0x06; break;
				case 0x2663: ch = 0x05; break;
				case 0x2665: ch = 0x03; break;
				case 0x2666: ch = 0x04; break;
				case 0x266A: ch = 0x0D; break;
				case 0x266B: ch = 0x0E; break;
				case 0xFFE8: ch = 0xB3; break;
				case 0xFFE9: ch = 0x1B; break;
				case 0xFFEA: ch = 0x18; break;
				case 0xFFEB: ch = 0x1A; break;
				case 0xFFEC: ch = 0x19; break;
				case 0xFFED: ch = 0xFE; break;
				case 0xFFEE: ch = 0x09; break;
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
			if(ch >= 26) switch(ch)
			{
				case 0x001B:
				case 0x001D:
				case 0x001E:
				case 0x001F:
				case 0x0020:
				case 0x0021:
				case 0x0022:
				case 0x0023:
				case 0x0024:
				case 0x0025:
				case 0x0026:
				case 0x0027:
				case 0x0028:
				case 0x0029:
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
				case 0x005B:
				case 0x005C:
				case 0x005D:
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
				case 0x007B:
				case 0x007C:
				case 0x007D:
				case 0x007E:
					break;
				case 0x001A: ch = 0x7F; break;
				case 0x001C: ch = 0x1A; break;
				case 0x007F: ch = 0x1C; break;
				case 0x00A0: ch = 0xFF; break;
				case 0x00A1: ch = 0xAD; break;
				case 0x00A3: ch = 0x9C; break;
				case 0x00A4: ch = 0xAF; break;
				case 0x00A7: ch = 0x15; break;
				case 0x00AA: ch = 0xA6; break;
				case 0x00AB: ch = 0xAE; break;
				case 0x00AC: ch = 0xAA; break;
				case 0x00B0: ch = 0xF8; break;
				case 0x00B1: ch = 0xF1; break;
				case 0x00B2: ch = 0xFD; break;
				case 0x00B6: ch = 0x14; break;
				case 0x00B7: ch = 0xFA; break;
				case 0x00BA: ch = 0xA7; break;
				case 0x00BC: ch = 0xAC; break;
				case 0x00BD: ch = 0xAB; break;
				case 0x00BF: ch = 0xA8; break;
				case 0x00C4: ch = 0x8E; break;
				case 0x00C5: ch = 0x8F; break;
				case 0x00C6: ch = 0x92; break;
				case 0x00C7: ch = 0x80; break;
				case 0x00C9: ch = 0x90; break;
				case 0x00D1: ch = 0xA5; break;
				case 0x00D6: ch = 0x99; break;
				case 0x00D8: ch = 0x9D; break;
				case 0x00DC: ch = 0x9A; break;
				case 0x00DF: ch = 0xE1; break;
				case 0x00E0: ch = 0x85; break;
				case 0x00E1: ch = 0xA0; break;
				case 0x00E2: ch = 0x83; break;
				case 0x00E4: ch = 0x84; break;
				case 0x00E5: ch = 0x86; break;
				case 0x00E6: ch = 0x91; break;
				case 0x00E7: ch = 0x87; break;
				case 0x00E8: ch = 0x8A; break;
				case 0x00E9: ch = 0x82; break;
				case 0x00EA: ch = 0x88; break;
				case 0x00EB: ch = 0x89; break;
				case 0x00EC: ch = 0x8D; break;
				case 0x00ED: ch = 0xA1; break;
				case 0x00EE: ch = 0x8C; break;
				case 0x00EF: ch = 0x8B; break;
				case 0x00F1: ch = 0xA4; break;
				case 0x00F2: ch = 0x95; break;
				case 0x00F3: ch = 0xA2; break;
				case 0x00F4: ch = 0x93; break;
				case 0x00F6: ch = 0x94; break;
				case 0x00F7: ch = 0xF6; break;
				case 0x00F8: ch = 0x9B; break;
				case 0x00F9: ch = 0x97; break;
				case 0x00FA: ch = 0xA3; break;
				case 0x00FB: ch = 0x96; break;
				case 0x00FC: ch = 0x81; break;
				case 0x00FF: ch = 0x98; break;
				case 0x0192: ch = 0x9F; break;
				case 0x0393: ch = 0xE2; break;
				case 0x0398: ch = 0xE9; break;
				case 0x03A3: ch = 0xE4; break;
				case 0x03A6: ch = 0xE8; break;
				case 0x03A9: ch = 0xEA; break;
				case 0x03B1: ch = 0xE0; break;
				case 0x03B4: ch = 0xEB; break;
				case 0x03B5: ch = 0xEE; break;
				case 0x03BC: ch = 0xE6; break;
				case 0x03C0: ch = 0xE3; break;
				case 0x03C3: ch = 0xE5; break;
				case 0x03C4: ch = 0xE7; break;
				case 0x03C6: ch = 0xED; break;
				case 0x2022: ch = 0x07; break;
				case 0x203C: ch = 0x13; break;
				case 0x207F: ch = 0xFC; break;
				case 0x20A7: ch = 0x9E; break;
				case 0x2190: ch = 0x1B; break;
				case 0x2191: ch = 0x18; break;
				case 0x2192: ch = 0x1A; break;
				case 0x2193: ch = 0x19; break;
				case 0x2194: ch = 0x1D; break;
				case 0x2195: ch = 0x12; break;
				case 0x21A8: ch = 0x17; break;
				case 0x2219: ch = 0xF9; break;
				case 0x221A: ch = 0xFB; break;
				case 0x221E: ch = 0xEC; break;
				case 0x221F: ch = 0x1C; break;
				case 0x2229: ch = 0xEF; break;
				case 0x2248: ch = 0xF7; break;
				case 0x2261: ch = 0xF0; break;
				case 0x2264: ch = 0xF3; break;
				case 0x2265: ch = 0xF2; break;
				case 0x2302: ch = 0x7F; break;
				case 0x2310: ch = 0xA9; break;
				case 0x2320: ch = 0xF4; break;
				case 0x2321: ch = 0xF5; break;
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
				case 0x25AC: ch = 0x16; break;
				case 0x25B2: ch = 0x1E; break;
				case 0x25BA: ch = 0x10; break;
				case 0x25BC: ch = 0x1F; break;
				case 0x25C4: ch = 0x11; break;
				case 0x25CB: ch = 0x09; break;
				case 0x25D8: ch = 0x08; break;
				case 0x25D9: ch = 0x0A; break;
				case 0x263A: ch = 0x01; break;
				case 0x263B: ch = 0x02; break;
				case 0x263C: ch = 0x0F; break;
				case 0x2640: ch = 0x0C; break;
				case 0x2642: ch = 0x0B; break;
				case 0x2660: ch = 0x06; break;
				case 0x2663: ch = 0x05; break;
				case 0x2665: ch = 0x03; break;
				case 0x2666: ch = 0x04; break;
				case 0x266A: ch = 0x0D; break;
				case 0x266B: ch = 0x0E; break;
				case 0xFFE8: ch = 0xB3; break;
				case 0xFFE9: ch = 0x1B; break;
				case 0xFFEA: ch = 0x18; break;
				case 0xFFEB: ch = 0x1A; break;
				case 0xFFEC: ch = 0x19; break;
				case 0xFFED: ch = 0xFE; break;
				case 0xFFEE: ch = 0x09; break;
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

}; // class CP865

public class ENCibm865 : CP865
{
	public ENCibm865() : base() {}

}; // class ENCibm865

}; // namespace I18N.West
