/*
 * CP20838.cs - IBM EBCDIC (Thai) code page.
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

// Generated from "ibm-838.ucm".

namespace I18N.Other
{

using System;
using I18N.Common;

public class CP20838 : ByteEncoding
{
	public CP20838()
		: base(20838, ToChars, "IBM EBCDIC (Thai)",
		       "IBM838", "IBM838", "IBM838",
		       false, false, false, false, 874)
	{}

	private static readonly char[] ToChars = {
		'\u0000', '\u0001', '\u0002', '\u0003', '\u009C', '\u0009', 
		'\u0086', '\u007F', '\u0097', '\u008D', '\u008E', '\u000B', 
		'\u000C', '\u000D', '\u000E', '\u000F', '\u0010', '\u0011', 
		'\u0012', '\u0013', '\u009D', '\u0085', '\u0008', '\u0087', 
		'\u0018', '\u0019', '\u0092', '\u008F', '\u001C', '\u001D', 
		'\u001E', '\u001F', '\u0080', '\u0081', '\u0082', '\u0083', 
		'\u0084', '\u000A', '\u0017', '\u001B', '\u0088', '\u0089', 
		'\u008A', '\u008B', '\u008C', '\u0005', '\u0006', '\u0007', 
		'\u0090', '\u0091', '\u0016', '\u0093', '\u0094', '\u0095', 
		'\u0096', '\u0004', '\u0098', '\u0099', '\u009A', '\u009B', 
		'\u0014', '\u0015', '\u009E', '\u001A', '\u0020', '\u00A0', 
		'\u0E01', '\u0E02', '\u0E03', '\u0E04', '\u0E05', '\u0E06', 
		'\u0E07', '\u005B', '\u00A2', '\u002E', '\u003C', '\u0028', 
		'\u002B', '\u007C', '\u0026', '\u0E48', '\u0E08', '\u0E09', 
		'\u0E0A', '\u0E0B', '\u0E0C', '\u0E0D', '\u0E0E', '\u005D', 
		'\u0021', '\u0024', '\u002A', '\u0029', '\u003B', '\u00AC', 
		'\u002D', '\u002F', '\u0E0F', '\u0E10', '\u0E11', '\u0E12', 
		'\u0E13', '\u0E14', '\u0E15', '\u005E', '\u00A6', '\u002C', 
		'\u0025', '\u005F', '\u003E', '\u003F', '\u0E3F', '\u0E4E', 
		'\u0E16', '\u0E17', '\u0E18', '\u0E19', '\u0E1A', '\u0E1B', 
		'\u0E1C', '\u0060', '\u003A', '\u0023', '\u0040', '\u0027', 
		'\u003D', '\u0022', '\u0E4F', '\u0061', '\u0062', '\u0063', 
		'\u0064', '\u0065', '\u0066', '\u0067', '\u0068', '\u0069', 
		'\u0E1D', '\u0E1E', '\u0E1F', '\u0E20', '\u0E21', '\u0E22', 
		'\u0E5A', '\u006A', '\u006B', '\u006C', '\u006D', '\u006E', 
		'\u006F', '\u0070', '\u0071', '\u0072', '\u0E23', '\u0E24', 
		'\u0E25', '\u0E26', '\u0E27', '\u0E28', '\u0E5B', '\u007E', 
		'\u0073', '\u0074', '\u0075', '\u0076', '\u0077', '\u0078', 
		'\u0079', '\u007A', '\u0E29', '\u0E2A', '\u0E2B', '\u0E2C', 
		'\u0E2D', '\u0E2E', '\u0E50', '\u0E51', '\u0E52', '\u0E53', 
		'\u0E54', '\u0E55', '\u0E56', '\u0E57', '\u0E58', '\u0E59', 
		'\u0E2F', '\u0E30', '\u0E31', '\u0E32', '\u0E33', '\u0E34', 
		'\u007B', '\u0041', '\u0042', '\u0043', '\u0044', '\u0045', 
		'\u0046', '\u0047', '\u0048', '\u0049', '\u0E49', '\u0E35', 
		'\u0E36', '\u0E37', '\u0E38', '\u0E39', '\u007D', '\u004A', 
		'\u004B', '\u004C', '\u004D', '\u004E', '\u004F', '\u0050', 
		'\u0051', '\u0052', '\u0E3A', '\u0E40', '\u0E41', '\u0E42', 
		'\u0E43', '\u0E44', '\u005C', '\u0E4A', '\u0053', '\u0054', 
		'\u0055', '\u0056', '\u0057', '\u0058', '\u0059', '\u005A', 
		'\u0E45', '\u0E46', '\u0E47', '\u0E48', '\u0E49', '\u0E4A', 
		'\u0030', '\u0031', '\u0032', '\u0033', '\u0034', '\u0035', 
		'\u0036', '\u0037', '\u0038', '\u0039', '\u0E4B', '\u0E4C', 
		'\u0E4D', '\u0E4B', '\u0E4C', '\u009F', 
	};

	protected override void ToBytes(char[] chars, int charIndex, int charCount,
	                                byte[] bytes, int byteIndex)
	{
		int ch;
		while(charCount > 0)
		{
			ch = (int)(chars[charIndex++]);
			if(ch >= 4) switch(ch)
			{
				case 0x000B:
				case 0x000C:
				case 0x000D:
				case 0x000E:
				case 0x000F:
				case 0x0010:
				case 0x0011:
				case 0x0012:
				case 0x0013:
				case 0x0018:
				case 0x0019:
				case 0x001C:
				case 0x001D:
				case 0x001E:
				case 0x001F:
					break;
				case 0x0004: ch = 0x37; break;
				case 0x0005: ch = 0x2D; break;
				case 0x0006: ch = 0x2E; break;
				case 0x0007: ch = 0x2F; break;
				case 0x0008: ch = 0x16; break;
				case 0x0009: ch = 0x05; break;
				case 0x000A: ch = 0x25; break;
				case 0x0014: ch = 0x3C; break;
				case 0x0015: ch = 0x3D; break;
				case 0x0016: ch = 0x32; break;
				case 0x0017: ch = 0x26; break;
				case 0x001A: ch = 0x3F; break;
				case 0x001B: ch = 0x27; break;
				case 0x0020: ch = 0x40; break;
				case 0x0021: ch = 0x5A; break;
				case 0x0022: ch = 0x7F; break;
				case 0x0023: ch = 0x7B; break;
				case 0x0024: ch = 0x5B; break;
				case 0x0025: ch = 0x6C; break;
				case 0x0026: ch = 0x50; break;
				case 0x0027: ch = 0x7D; break;
				case 0x0028: ch = 0x4D; break;
				case 0x0029: ch = 0x5D; break;
				case 0x002A: ch = 0x5C; break;
				case 0x002B: ch = 0x4E; break;
				case 0x002C: ch = 0x6B; break;
				case 0x002D: ch = 0x60; break;
				case 0x002E: ch = 0x4B; break;
				case 0x002F: ch = 0x61; break;
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
					ch += 0x00C0;
					break;
				case 0x003A: ch = 0x7A; break;
				case 0x003B: ch = 0x5E; break;
				case 0x003C: ch = 0x4C; break;
				case 0x003D: ch = 0x7E; break;
				case 0x003E: ch = 0x6E; break;
				case 0x003F: ch = 0x6F; break;
				case 0x0040: ch = 0x7C; break;
				case 0x0041:
				case 0x0042:
				case 0x0043:
				case 0x0044:
				case 0x0045:
				case 0x0046:
				case 0x0047:
				case 0x0048:
				case 0x0049:
					ch += 0x0080;
					break;
				case 0x004A:
				case 0x004B:
				case 0x004C:
				case 0x004D:
				case 0x004E:
				case 0x004F:
				case 0x0050:
				case 0x0051:
				case 0x0052:
					ch += 0x0087;
					break;
				case 0x0053:
				case 0x0054:
				case 0x0055:
				case 0x0056:
				case 0x0057:
				case 0x0058:
				case 0x0059:
				case 0x005A:
					ch += 0x008F;
					break;
				case 0x005B: ch = 0x49; break;
				case 0x005C: ch = 0xE0; break;
				case 0x005D: ch = 0x59; break;
				case 0x005E: ch = 0x69; break;
				case 0x005F: ch = 0x6D; break;
				case 0x0060: ch = 0x79; break;
				case 0x0061:
				case 0x0062:
				case 0x0063:
				case 0x0064:
				case 0x0065:
				case 0x0066:
				case 0x0067:
				case 0x0068:
				case 0x0069:
					ch += 0x0020;
					break;
				case 0x006A:
				case 0x006B:
				case 0x006C:
				case 0x006D:
				case 0x006E:
				case 0x006F:
				case 0x0070:
				case 0x0071:
				case 0x0072:
					ch += 0x0027;
					break;
				case 0x0073:
				case 0x0074:
				case 0x0075:
				case 0x0076:
				case 0x0077:
				case 0x0078:
				case 0x0079:
				case 0x007A:
					ch += 0x002F;
					break;
				case 0x007B: ch = 0xC0; break;
				case 0x007C: ch = 0x4F; break;
				case 0x007D: ch = 0xD0; break;
				case 0x007E: ch = 0xA1; break;
				case 0x007F: ch = 0x07; break;
				case 0x0080:
				case 0x0081:
				case 0x0082:
				case 0x0083:
				case 0x0084:
					ch -= 0x0060;
					break;
				case 0x0085: ch = 0x15; break;
				case 0x0086: ch = 0x06; break;
				case 0x0087: ch = 0x17; break;
				case 0x0088:
				case 0x0089:
				case 0x008A:
				case 0x008B:
				case 0x008C:
					ch -= 0x0060;
					break;
				case 0x008D: ch = 0x09; break;
				case 0x008E: ch = 0x0A; break;
				case 0x008F: ch = 0x1B; break;
				case 0x0090: ch = 0x30; break;
				case 0x0091: ch = 0x31; break;
				case 0x0092: ch = 0x1A; break;
				case 0x0093:
				case 0x0094:
				case 0x0095:
				case 0x0096:
					ch -= 0x0060;
					break;
				case 0x0097: ch = 0x08; break;
				case 0x0098:
				case 0x0099:
				case 0x009A:
				case 0x009B:
					ch -= 0x0060;
					break;
				case 0x009C: ch = 0x04; break;
				case 0x009D: ch = 0x14; break;
				case 0x009E: ch = 0x3E; break;
				case 0x009F: ch = 0xFF; break;
				case 0x00A0: ch = 0x41; break;
				case 0x00A2: ch = 0x4A; break;
				case 0x00A6: ch = 0x6A; break;
				case 0x00AC: ch = 0x5F; break;
				case 0x0E01:
				case 0x0E02:
				case 0x0E03:
				case 0x0E04:
				case 0x0E05:
				case 0x0E06:
				case 0x0E07:
					ch -= 0x0DBF;
					break;
				case 0x0E08:
				case 0x0E09:
				case 0x0E0A:
				case 0x0E0B:
				case 0x0E0C:
				case 0x0E0D:
				case 0x0E0E:
					ch -= 0x0DB6;
					break;
				case 0x0E0F:
				case 0x0E10:
				case 0x0E11:
				case 0x0E12:
				case 0x0E13:
				case 0x0E14:
				case 0x0E15:
					ch -= 0x0DAD;
					break;
				case 0x0E16:
				case 0x0E17:
				case 0x0E18:
				case 0x0E19:
				case 0x0E1A:
				case 0x0E1B:
				case 0x0E1C:
					ch -= 0x0DA4;
					break;
				case 0x0E1D:
				case 0x0E1E:
				case 0x0E1F:
				case 0x0E20:
				case 0x0E21:
				case 0x0E22:
					ch -= 0x0D93;
					break;
				case 0x0E23:
				case 0x0E24:
				case 0x0E25:
				case 0x0E26:
				case 0x0E27:
				case 0x0E28:
					ch -= 0x0D89;
					break;
				case 0x0E29:
				case 0x0E2A:
				case 0x0E2B:
				case 0x0E2C:
				case 0x0E2D:
				case 0x0E2E:
					ch -= 0x0D7F;
					break;
				case 0x0E2F:
				case 0x0E30:
				case 0x0E31:
				case 0x0E32:
				case 0x0E33:
				case 0x0E34:
					ch -= 0x0D75;
					break;
				case 0x0E35:
				case 0x0E36:
				case 0x0E37:
				case 0x0E38:
				case 0x0E39:
					ch -= 0x0D6A;
					break;
				case 0x0E3A: ch = 0xDA; break;
				case 0x0E3F: ch = 0x70; break;
				case 0x0E40:
				case 0x0E41:
				case 0x0E42:
				case 0x0E43:
				case 0x0E44:
					ch -= 0x0D65;
					break;
				case 0x0E45:
				case 0x0E46:
				case 0x0E47:
				case 0x0E48:
				case 0x0E49:
				case 0x0E4A:
					ch -= 0x0D5B;
					break;
				case 0x0E4B: ch = 0xFD; break;
				case 0x0E4C: ch = 0xFE; break;
				case 0x0E4D: ch = 0xFC; break;
				case 0x0E4E: ch = 0x71; break;
				case 0x0E4F: ch = 0x80; break;
				case 0x0E50:
				case 0x0E51:
				case 0x0E52:
				case 0x0E53:
				case 0x0E54:
				case 0x0E55:
				case 0x0E56:
				case 0x0E57:
				case 0x0E58:
				case 0x0E59:
					ch -= 0x0DA0;
					break;
				case 0x0E5A: ch = 0x90; break;
				case 0x0E5B: ch = 0xA0; break;
				case 0xFF01: ch = 0x5A; break;
				case 0xFF02: ch = 0x7F; break;
				case 0xFF03: ch = 0x7B; break;
				case 0xFF04: ch = 0x5B; break;
				case 0xFF05: ch = 0x6C; break;
				case 0xFF06: ch = 0x50; break;
				case 0xFF07: ch = 0x7D; break;
				case 0xFF08: ch = 0x4D; break;
				case 0xFF09: ch = 0x5D; break;
				case 0xFF0A: ch = 0x5C; break;
				case 0xFF0B: ch = 0x4E; break;
				case 0xFF0C: ch = 0x6B; break;
				case 0xFF0D: ch = 0x60; break;
				case 0xFF0E: ch = 0x4B; break;
				case 0xFF0F: ch = 0x61; break;
				case 0xFF10:
				case 0xFF11:
				case 0xFF12:
				case 0xFF13:
				case 0xFF14:
				case 0xFF15:
				case 0xFF16:
				case 0xFF17:
				case 0xFF18:
				case 0xFF19:
					ch -= 0xFE20;
					break;
				case 0xFF1A: ch = 0x7A; break;
				case 0xFF1B: ch = 0x5E; break;
				case 0xFF1C: ch = 0x4C; break;
				case 0xFF1D: ch = 0x7E; break;
				case 0xFF1E: ch = 0x6E; break;
				case 0xFF1F: ch = 0x6F; break;
				case 0xFF20: ch = 0x7C; break;
				case 0xFF21:
				case 0xFF22:
				case 0xFF23:
				case 0xFF24:
				case 0xFF25:
				case 0xFF26:
				case 0xFF27:
				case 0xFF28:
				case 0xFF29:
					ch -= 0xFE60;
					break;
				case 0xFF2A:
				case 0xFF2B:
				case 0xFF2C:
				case 0xFF2D:
				case 0xFF2E:
				case 0xFF2F:
				case 0xFF30:
				case 0xFF31:
				case 0xFF32:
					ch -= 0xFE59;
					break;
				case 0xFF33:
				case 0xFF34:
				case 0xFF35:
				case 0xFF36:
				case 0xFF37:
				case 0xFF38:
				case 0xFF39:
				case 0xFF3A:
					ch -= 0xFE51;
					break;
				case 0xFF3B: ch = 0x49; break;
				case 0xFF3C: ch = 0xE0; break;
				case 0xFF3D: ch = 0x59; break;
				case 0xFF3E: ch = 0x69; break;
				case 0xFF3F: ch = 0x6D; break;
				case 0xFF40: ch = 0x79; break;
				case 0xFF41:
				case 0xFF42:
				case 0xFF43:
				case 0xFF44:
				case 0xFF45:
				case 0xFF46:
				case 0xFF47:
				case 0xFF48:
				case 0xFF49:
					ch -= 0xFEC0;
					break;
				case 0xFF4A:
				case 0xFF4B:
				case 0xFF4C:
				case 0xFF4D:
				case 0xFF4E:
				case 0xFF4F:
				case 0xFF50:
				case 0xFF51:
				case 0xFF52:
					ch -= 0xFEB9;
					break;
				case 0xFF53:
				case 0xFF54:
				case 0xFF55:
				case 0xFF56:
				case 0xFF57:
				case 0xFF58:
				case 0xFF59:
				case 0xFF5A:
					ch -= 0xFEB1;
					break;
				case 0xFF5B: ch = 0xC0; break;
				case 0xFF5C: ch = 0x4F; break;
				case 0xFF5D: ch = 0xD0; break;
				case 0xFF5E: ch = 0xA1; break;
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
			if(ch >= 4) switch(ch)
			{
				case 0x000B:
				case 0x000C:
				case 0x000D:
				case 0x000E:
				case 0x000F:
				case 0x0010:
				case 0x0011:
				case 0x0012:
				case 0x0013:
				case 0x0018:
				case 0x0019:
				case 0x001C:
				case 0x001D:
				case 0x001E:
				case 0x001F:
					break;
				case 0x0004: ch = 0x37; break;
				case 0x0005: ch = 0x2D; break;
				case 0x0006: ch = 0x2E; break;
				case 0x0007: ch = 0x2F; break;
				case 0x0008: ch = 0x16; break;
				case 0x0009: ch = 0x05; break;
				case 0x000A: ch = 0x25; break;
				case 0x0014: ch = 0x3C; break;
				case 0x0015: ch = 0x3D; break;
				case 0x0016: ch = 0x32; break;
				case 0x0017: ch = 0x26; break;
				case 0x001A: ch = 0x3F; break;
				case 0x001B: ch = 0x27; break;
				case 0x0020: ch = 0x40; break;
				case 0x0021: ch = 0x5A; break;
				case 0x0022: ch = 0x7F; break;
				case 0x0023: ch = 0x7B; break;
				case 0x0024: ch = 0x5B; break;
				case 0x0025: ch = 0x6C; break;
				case 0x0026: ch = 0x50; break;
				case 0x0027: ch = 0x7D; break;
				case 0x0028: ch = 0x4D; break;
				case 0x0029: ch = 0x5D; break;
				case 0x002A: ch = 0x5C; break;
				case 0x002B: ch = 0x4E; break;
				case 0x002C: ch = 0x6B; break;
				case 0x002D: ch = 0x60; break;
				case 0x002E: ch = 0x4B; break;
				case 0x002F: ch = 0x61; break;
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
					ch += 0x00C0;
					break;
				case 0x003A: ch = 0x7A; break;
				case 0x003B: ch = 0x5E; break;
				case 0x003C: ch = 0x4C; break;
				case 0x003D: ch = 0x7E; break;
				case 0x003E: ch = 0x6E; break;
				case 0x003F: ch = 0x6F; break;
				case 0x0040: ch = 0x7C; break;
				case 0x0041:
				case 0x0042:
				case 0x0043:
				case 0x0044:
				case 0x0045:
				case 0x0046:
				case 0x0047:
				case 0x0048:
				case 0x0049:
					ch += 0x0080;
					break;
				case 0x004A:
				case 0x004B:
				case 0x004C:
				case 0x004D:
				case 0x004E:
				case 0x004F:
				case 0x0050:
				case 0x0051:
				case 0x0052:
					ch += 0x0087;
					break;
				case 0x0053:
				case 0x0054:
				case 0x0055:
				case 0x0056:
				case 0x0057:
				case 0x0058:
				case 0x0059:
				case 0x005A:
					ch += 0x008F;
					break;
				case 0x005B: ch = 0x49; break;
				case 0x005C: ch = 0xE0; break;
				case 0x005D: ch = 0x59; break;
				case 0x005E: ch = 0x69; break;
				case 0x005F: ch = 0x6D; break;
				case 0x0060: ch = 0x79; break;
				case 0x0061:
				case 0x0062:
				case 0x0063:
				case 0x0064:
				case 0x0065:
				case 0x0066:
				case 0x0067:
				case 0x0068:
				case 0x0069:
					ch += 0x0020;
					break;
				case 0x006A:
				case 0x006B:
				case 0x006C:
				case 0x006D:
				case 0x006E:
				case 0x006F:
				case 0x0070:
				case 0x0071:
				case 0x0072:
					ch += 0x0027;
					break;
				case 0x0073:
				case 0x0074:
				case 0x0075:
				case 0x0076:
				case 0x0077:
				case 0x0078:
				case 0x0079:
				case 0x007A:
					ch += 0x002F;
					break;
				case 0x007B: ch = 0xC0; break;
				case 0x007C: ch = 0x4F; break;
				case 0x007D: ch = 0xD0; break;
				case 0x007E: ch = 0xA1; break;
				case 0x007F: ch = 0x07; break;
				case 0x0080:
				case 0x0081:
				case 0x0082:
				case 0x0083:
				case 0x0084:
					ch -= 0x0060;
					break;
				case 0x0085: ch = 0x15; break;
				case 0x0086: ch = 0x06; break;
				case 0x0087: ch = 0x17; break;
				case 0x0088:
				case 0x0089:
				case 0x008A:
				case 0x008B:
				case 0x008C:
					ch -= 0x0060;
					break;
				case 0x008D: ch = 0x09; break;
				case 0x008E: ch = 0x0A; break;
				case 0x008F: ch = 0x1B; break;
				case 0x0090: ch = 0x30; break;
				case 0x0091: ch = 0x31; break;
				case 0x0092: ch = 0x1A; break;
				case 0x0093:
				case 0x0094:
				case 0x0095:
				case 0x0096:
					ch -= 0x0060;
					break;
				case 0x0097: ch = 0x08; break;
				case 0x0098:
				case 0x0099:
				case 0x009A:
				case 0x009B:
					ch -= 0x0060;
					break;
				case 0x009C: ch = 0x04; break;
				case 0x009D: ch = 0x14; break;
				case 0x009E: ch = 0x3E; break;
				case 0x009F: ch = 0xFF; break;
				case 0x00A0: ch = 0x41; break;
				case 0x00A2: ch = 0x4A; break;
				case 0x00A6: ch = 0x6A; break;
				case 0x00AC: ch = 0x5F; break;
				case 0x0E01:
				case 0x0E02:
				case 0x0E03:
				case 0x0E04:
				case 0x0E05:
				case 0x0E06:
				case 0x0E07:
					ch -= 0x0DBF;
					break;
				case 0x0E08:
				case 0x0E09:
				case 0x0E0A:
				case 0x0E0B:
				case 0x0E0C:
				case 0x0E0D:
				case 0x0E0E:
					ch -= 0x0DB6;
					break;
				case 0x0E0F:
				case 0x0E10:
				case 0x0E11:
				case 0x0E12:
				case 0x0E13:
				case 0x0E14:
				case 0x0E15:
					ch -= 0x0DAD;
					break;
				case 0x0E16:
				case 0x0E17:
				case 0x0E18:
				case 0x0E19:
				case 0x0E1A:
				case 0x0E1B:
				case 0x0E1C:
					ch -= 0x0DA4;
					break;
				case 0x0E1D:
				case 0x0E1E:
				case 0x0E1F:
				case 0x0E20:
				case 0x0E21:
				case 0x0E22:
					ch -= 0x0D93;
					break;
				case 0x0E23:
				case 0x0E24:
				case 0x0E25:
				case 0x0E26:
				case 0x0E27:
				case 0x0E28:
					ch -= 0x0D89;
					break;
				case 0x0E29:
				case 0x0E2A:
				case 0x0E2B:
				case 0x0E2C:
				case 0x0E2D:
				case 0x0E2E:
					ch -= 0x0D7F;
					break;
				case 0x0E2F:
				case 0x0E30:
				case 0x0E31:
				case 0x0E32:
				case 0x0E33:
				case 0x0E34:
					ch -= 0x0D75;
					break;
				case 0x0E35:
				case 0x0E36:
				case 0x0E37:
				case 0x0E38:
				case 0x0E39:
					ch -= 0x0D6A;
					break;
				case 0x0E3A: ch = 0xDA; break;
				case 0x0E3F: ch = 0x70; break;
				case 0x0E40:
				case 0x0E41:
				case 0x0E42:
				case 0x0E43:
				case 0x0E44:
					ch -= 0x0D65;
					break;
				case 0x0E45:
				case 0x0E46:
				case 0x0E47:
				case 0x0E48:
				case 0x0E49:
				case 0x0E4A:
					ch -= 0x0D5B;
					break;
				case 0x0E4B: ch = 0xFD; break;
				case 0x0E4C: ch = 0xFE; break;
				case 0x0E4D: ch = 0xFC; break;
				case 0x0E4E: ch = 0x71; break;
				case 0x0E4F: ch = 0x80; break;
				case 0x0E50:
				case 0x0E51:
				case 0x0E52:
				case 0x0E53:
				case 0x0E54:
				case 0x0E55:
				case 0x0E56:
				case 0x0E57:
				case 0x0E58:
				case 0x0E59:
					ch -= 0x0DA0;
					break;
				case 0x0E5A: ch = 0x90; break;
				case 0x0E5B: ch = 0xA0; break;
				case 0xFF01: ch = 0x5A; break;
				case 0xFF02: ch = 0x7F; break;
				case 0xFF03: ch = 0x7B; break;
				case 0xFF04: ch = 0x5B; break;
				case 0xFF05: ch = 0x6C; break;
				case 0xFF06: ch = 0x50; break;
				case 0xFF07: ch = 0x7D; break;
				case 0xFF08: ch = 0x4D; break;
				case 0xFF09: ch = 0x5D; break;
				case 0xFF0A: ch = 0x5C; break;
				case 0xFF0B: ch = 0x4E; break;
				case 0xFF0C: ch = 0x6B; break;
				case 0xFF0D: ch = 0x60; break;
				case 0xFF0E: ch = 0x4B; break;
				case 0xFF0F: ch = 0x61; break;
				case 0xFF10:
				case 0xFF11:
				case 0xFF12:
				case 0xFF13:
				case 0xFF14:
				case 0xFF15:
				case 0xFF16:
				case 0xFF17:
				case 0xFF18:
				case 0xFF19:
					ch -= 0xFE20;
					break;
				case 0xFF1A: ch = 0x7A; break;
				case 0xFF1B: ch = 0x5E; break;
				case 0xFF1C: ch = 0x4C; break;
				case 0xFF1D: ch = 0x7E; break;
				case 0xFF1E: ch = 0x6E; break;
				case 0xFF1F: ch = 0x6F; break;
				case 0xFF20: ch = 0x7C; break;
				case 0xFF21:
				case 0xFF22:
				case 0xFF23:
				case 0xFF24:
				case 0xFF25:
				case 0xFF26:
				case 0xFF27:
				case 0xFF28:
				case 0xFF29:
					ch -= 0xFE60;
					break;
				case 0xFF2A:
				case 0xFF2B:
				case 0xFF2C:
				case 0xFF2D:
				case 0xFF2E:
				case 0xFF2F:
				case 0xFF30:
				case 0xFF31:
				case 0xFF32:
					ch -= 0xFE59;
					break;
				case 0xFF33:
				case 0xFF34:
				case 0xFF35:
				case 0xFF36:
				case 0xFF37:
				case 0xFF38:
				case 0xFF39:
				case 0xFF3A:
					ch -= 0xFE51;
					break;
				case 0xFF3B: ch = 0x49; break;
				case 0xFF3C: ch = 0xE0; break;
				case 0xFF3D: ch = 0x59; break;
				case 0xFF3E: ch = 0x69; break;
				case 0xFF3F: ch = 0x6D; break;
				case 0xFF40: ch = 0x79; break;
				case 0xFF41:
				case 0xFF42:
				case 0xFF43:
				case 0xFF44:
				case 0xFF45:
				case 0xFF46:
				case 0xFF47:
				case 0xFF48:
				case 0xFF49:
					ch -= 0xFEC0;
					break;
				case 0xFF4A:
				case 0xFF4B:
				case 0xFF4C:
				case 0xFF4D:
				case 0xFF4E:
				case 0xFF4F:
				case 0xFF50:
				case 0xFF51:
				case 0xFF52:
					ch -= 0xFEB9;
					break;
				case 0xFF53:
				case 0xFF54:
				case 0xFF55:
				case 0xFF56:
				case 0xFF57:
				case 0xFF58:
				case 0xFF59:
				case 0xFF5A:
					ch -= 0xFEB1;
					break;
				case 0xFF5B: ch = 0xC0; break;
				case 0xFF5C: ch = 0x4F; break;
				case 0xFF5D: ch = 0xD0; break;
				case 0xFF5E: ch = 0xA1; break;
				default: ch = 0x3F; break;
			}
			bytes[byteIndex++] = (byte)ch;
			--charCount;
		}
	}

}; // class CP20838

public class ENCibm838 : CP20838
{
	public ENCibm838() : base() {}

}; // class ENCibm838

}; // namespace I18N.Other
