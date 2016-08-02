/*
 * CP709.cs - Arabic - ASMO 449+, BCON V4 code page.
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

// Generated from "iso-9036.ucm".

namespace I18N.Rare
{

using System;
using I18N.Common;

public class CP709 : ByteEncoding
{
	public CP709()
		: base(709, ToChars, "Arabic - ASMO 449+, BCON V4",
		       "windows-709", "windows-709", "windows-709",
		       false, false, false, false, 1256)
	{}

	private static readonly char[] ToChars = {
		'\u0000', '\u0001', '\u0002', '\u0003', '\u0004', '\u0005', 
		'\u0006', '\u0007', '\u0008', '\u0009', '\u000A', '\u000B', 
		'\u000C', '\u000D', '\u000E', '\u000F', '\u0010', '\u0011', 
		'\u0012', '\u0013', '\u0014', '\u0015', '\u0016', '\u0017', 
		'\u0018', '\u0019', '\u001A', '\u001B', '\u001C', '\u001D', 
		'\u001E', '\u001F', '\u0020', '\u0021', '\u0022', '\u0023', 
		'\u00A4', '\u0025', '\u0026', '\u0027', '\u0028', '\u0029', 
		'\u002A', '\u002B', '\u060C', '\u002D', '\u002E', '\u002F', 
		'\u0030', '\u0031', '\u0032', '\u0033', '\u0034', '\u0035', 
		'\u0036', '\u0037', '\u0038', '\u0039', '\u003A', '\u061B', 
		'\u003C', '\u003D', '\u003E', '\u061F', '\u0040', '\u0621', 
		'\u0622', '\u0623', '\u0624', '\u0625', '\u0626', '\u0627', 
		'\u0628', '\u0629', '\u062A', '\u062B', '\u062C', '\u062D', 
		'\u062E', '\u062F', '\u0630', '\u0631', '\u0632', '\u0633', 
		'\u0634', '\u0635', '\u0636', '\u0637', '\u0638', '\u0639', 
		'\u063A', '\u005B', '\u005C', '\u005D', '\u005E', '\u005F', 
		'\u0640', '\u0641', '\u0642', '\u0643', '\u0644', '\u0645', 
		'\u0646', '\u0647', '\u0648', '\u0649', '\u064A', '\u064B', 
		'\u064C', '\u064D', '\u064E', '\u064F', '\u0650', '\u0651', 
		'\u0652', '\u003F', '\u003F', '\u003F', '\u003F', '\u003F', 
		'\u003F', '\u003F', '\u003F', '\u007B', '\u007C', '\u007D', 
		'\u203E', '\u007F', '\u003F', '\u003F', '\u003F', '\u003F', 
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
			if(ch >= 36) switch(ch)
			{
				case 0x0025:
				case 0x0026:
				case 0x0027:
				case 0x0028:
				case 0x0029:
				case 0x002A:
				case 0x002B:
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
				case 0x003C:
				case 0x003D:
				case 0x003E:
				case 0x0040:
				case 0x005B:
				case 0x005C:
				case 0x005D:
				case 0x005E:
				case 0x005F:
				case 0x007B:
				case 0x007C:
				case 0x007D:
				case 0x007F:
					break;
				case 0x00A4: ch = 0x24; break;
				case 0x060C: ch = 0x2C; break;
				case 0x061B: ch = 0x3B; break;
				case 0x061F: ch = 0x3F; break;
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
					ch -= 0x05E0;
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
					ch -= 0x05E0;
					break;
				case 0x203E: ch = 0x7E; break;
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
			if(ch >= 36) switch(ch)
			{
				case 0x0025:
				case 0x0026:
				case 0x0027:
				case 0x0028:
				case 0x0029:
				case 0x002A:
				case 0x002B:
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
				case 0x003C:
				case 0x003D:
				case 0x003E:
				case 0x0040:
				case 0x005B:
				case 0x005C:
				case 0x005D:
				case 0x005E:
				case 0x005F:
				case 0x007B:
				case 0x007C:
				case 0x007D:
				case 0x007F:
					break;
				case 0x00A4: ch = 0x24; break;
				case 0x060C: ch = 0x2C; break;
				case 0x061B: ch = 0x3B; break;
				case 0x061F: ch = 0x3F; break;
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
					ch -= 0x05E0;
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
					ch -= 0x05E0;
					break;
				case 0x203E: ch = 0x7E; break;
				default: ch = 0x3F; break;
			}
			bytes[byteIndex++] = (byte)ch;
			--charCount;
		}
	}

}; // class CP709

public class ENCwindows_709 : CP709
{
	public ENCwindows_709() : base() {}

}; // class ENCwindows_709

}; // namespace I18N.Rare
