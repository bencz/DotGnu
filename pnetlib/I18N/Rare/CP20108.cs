/*
 * CP20108.cs - IA5 Norwegian (7-bit) code page.
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

// Generated from "ia5-20108.ucm".

namespace I18N.Rare
{

using System;
using I18N.Common;

public class CP20108 : ByteEncoding
{
	public CP20108()
		: base(20108, ToChars, "IA5 Norwegian (7-bit)",
		       "windows-20108", "windows-20108", "windows-20108",
		       false, false, false, false, 1252)
	{}

	private static readonly char[] ToChars = {
		'\u0000', '\u0001', '\u0002', '\u0003', '\u0004', '\u0005', 
		'\u0006', '\u0007', '\u0008', '\u0009', '\u000A', '\u000B', 
		'\u000C', '\u000D', '\u000E', '\u000F', '\u0010', '\u0011', 
		'\u0012', '\u0013', '\u0014', '\u0015', '\u0016', '\u0017', 
		'\u0018', '\u0019', '\u001A', '\u001B', '\u001C', '\u001D', 
		'\u001E', '\u001F', '\u0020', '\u0021', '\u0022', '\u00B6', 
		'\u0024', '\u0025', '\u0026', '\u0027', '\u0028', '\u0029', 
		'\u002A', '\u002B', '\u002C', '\u002D', '\u002E', '\u002F', 
		'\u0030', '\u0031', '\u0032', '\u0033', '\u0034', '\u0035', 
		'\u0036', '\u0037', '\u0038', '\u0039', '\u003A', '\u003B', 
		'\u003C', '\u003D', '\u003E', '\u003F', '\u0040', '\u0041', 
		'\u0042', '\u0043', '\u0044', '\u0045', '\u0046', '\u0047', 
		'\u0048', '\u0049', '\u004A', '\u004B', '\u004C', '\u004D', 
		'\u004E', '\u004F', '\u0050', '\u0051', '\u0052', '\u0053', 
		'\u0054', '\u0055', '\u0056', '\u0057', '\u0058', '\u0059', 
		'\u005A', '\u00C6', '\u00D8', '\u00C5', '\u005E', '\u005F', 
		'\u0060', '\u0061', '\u0062', '\u0063', '\u0064', '\u0065', 
		'\u0066', '\u0067', '\u0068', '\u0069', '\u006A', '\u006B', 
		'\u006C', '\u006D', '\u006E', '\u006F', '\u0070', '\u0071', 
		'\u0072', '\u0073', '\u0074', '\u0075', '\u0076', '\u0077', 
		'\u0078', '\u0079', '\u007A', '\u00E6', '\u00F8', '\u00E5', 
		'\u007C', '\u007F', '\u003F', '\u003F', '\u003F', '\u003F', 
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
			if(ch >= 124) switch(ch)
			{
				case 0x007D:
				case 0x007E:
				case 0x007F:
					break;
				case 0x007C: ch = 0x7E; break;
				case 0x00B6: ch = 0x23; break;
				case 0x00C5: ch = 0x5D; break;
				case 0x00C6: ch = 0x5B; break;
				case 0x00D8: ch = 0x5C; break;
				case 0x00E5: ch = 0x7D; break;
				case 0x00E6: ch = 0x7B; break;
				case 0x00F8: ch = 0x7C; break;
				case 0xFF01:
				case 0xFF02:
				case 0xFF03:
				case 0xFF04:
				case 0xFF05:
				case 0xFF06:
				case 0xFF07:
				case 0xFF08:
				case 0xFF09:
				case 0xFF0A:
				case 0xFF0B:
				case 0xFF0C:
				case 0xFF0D:
				case 0xFF0E:
				case 0xFF0F:
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
				case 0xFF1A:
				case 0xFF1B:
				case 0xFF1C:
				case 0xFF1D:
				case 0xFF1E:
				case 0xFF1F:
				case 0xFF20:
				case 0xFF21:
				case 0xFF22:
				case 0xFF23:
				case 0xFF24:
				case 0xFF25:
				case 0xFF26:
				case 0xFF27:
				case 0xFF28:
				case 0xFF29:
				case 0xFF2A:
				case 0xFF2B:
				case 0xFF2C:
				case 0xFF2D:
				case 0xFF2E:
				case 0xFF2F:
				case 0xFF30:
				case 0xFF31:
				case 0xFF32:
				case 0xFF33:
				case 0xFF34:
				case 0xFF35:
				case 0xFF36:
				case 0xFF37:
				case 0xFF38:
				case 0xFF39:
				case 0xFF3A:
				case 0xFF3B:
				case 0xFF3C:
				case 0xFF3D:
				case 0xFF3E:
				case 0xFF3F:
				case 0xFF40:
				case 0xFF41:
				case 0xFF42:
				case 0xFF43:
				case 0xFF44:
				case 0xFF45:
				case 0xFF46:
				case 0xFF47:
				case 0xFF48:
				case 0xFF49:
				case 0xFF4A:
				case 0xFF4B:
				case 0xFF4C:
				case 0xFF4D:
				case 0xFF4E:
				case 0xFF4F:
				case 0xFF50:
				case 0xFF51:
				case 0xFF52:
				case 0xFF53:
				case 0xFF54:
				case 0xFF55:
				case 0xFF56:
				case 0xFF57:
				case 0xFF58:
				case 0xFF59:
				case 0xFF5A:
				case 0xFF5B:
					ch -= 0xFEE0;
					break;
				case 0xFF5C: ch = 0x7E; break;
				case 0xFF5D: ch = 0x7D; break;
				case 0xFF5E: ch = 0x7E; break;
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
			if(ch >= 124) switch(ch)
			{
				case 0x007D:
				case 0x007E:
				case 0x007F:
					break;
				case 0x007C: ch = 0x7E; break;
				case 0x00B6: ch = 0x23; break;
				case 0x00C5: ch = 0x5D; break;
				case 0x00C6: ch = 0x5B; break;
				case 0x00D8: ch = 0x5C; break;
				case 0x00E5: ch = 0x7D; break;
				case 0x00E6: ch = 0x7B; break;
				case 0x00F8: ch = 0x7C; break;
				case 0xFF01:
				case 0xFF02:
				case 0xFF03:
				case 0xFF04:
				case 0xFF05:
				case 0xFF06:
				case 0xFF07:
				case 0xFF08:
				case 0xFF09:
				case 0xFF0A:
				case 0xFF0B:
				case 0xFF0C:
				case 0xFF0D:
				case 0xFF0E:
				case 0xFF0F:
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
				case 0xFF1A:
				case 0xFF1B:
				case 0xFF1C:
				case 0xFF1D:
				case 0xFF1E:
				case 0xFF1F:
				case 0xFF20:
				case 0xFF21:
				case 0xFF22:
				case 0xFF23:
				case 0xFF24:
				case 0xFF25:
				case 0xFF26:
				case 0xFF27:
				case 0xFF28:
				case 0xFF29:
				case 0xFF2A:
				case 0xFF2B:
				case 0xFF2C:
				case 0xFF2D:
				case 0xFF2E:
				case 0xFF2F:
				case 0xFF30:
				case 0xFF31:
				case 0xFF32:
				case 0xFF33:
				case 0xFF34:
				case 0xFF35:
				case 0xFF36:
				case 0xFF37:
				case 0xFF38:
				case 0xFF39:
				case 0xFF3A:
				case 0xFF3B:
				case 0xFF3C:
				case 0xFF3D:
				case 0xFF3E:
				case 0xFF3F:
				case 0xFF40:
				case 0xFF41:
				case 0xFF42:
				case 0xFF43:
				case 0xFF44:
				case 0xFF45:
				case 0xFF46:
				case 0xFF47:
				case 0xFF48:
				case 0xFF49:
				case 0xFF4A:
				case 0xFF4B:
				case 0xFF4C:
				case 0xFF4D:
				case 0xFF4E:
				case 0xFF4F:
				case 0xFF50:
				case 0xFF51:
				case 0xFF52:
				case 0xFF53:
				case 0xFF54:
				case 0xFF55:
				case 0xFF56:
				case 0xFF57:
				case 0xFF58:
				case 0xFF59:
				case 0xFF5A:
				case 0xFF5B:
					ch -= 0xFEE0;
					break;
				case 0xFF5C: ch = 0x7E; break;
				case 0xFF5D: ch = 0x7D; break;
				case 0xFF5E: ch = 0x7E; break;
				default: ch = 0x3F; break;
			}
			bytes[byteIndex++] = (byte)ch;
			--charCount;
		}
	}

}; // class CP20108

public class ENCwindows_20108 : CP20108
{
	public ENCwindows_20108() : base() {}

}; // class ENCwindows_20108

}; // namespace I18N.Rare
