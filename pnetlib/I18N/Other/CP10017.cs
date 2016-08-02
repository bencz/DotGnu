/*
 * CP10017.cs - Ukraine (Mac) code page.
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

// Generated from "mac-10007.ucm".

namespace I18N.Other
{

using System;
using I18N.Common;

public class CP10017 : ByteEncoding
{
	public CP10017()
		: base(10017, ToChars, "Ukraine (Mac)",
		       "windows-10017", "windows-10017", "windows-10017",
		       false, false, false, false, 1251)
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
		'\u007E', '\u007F', '\u0410', '\u0411', '\u0412', '\u0413', 
		'\u0414', '\u0415', '\u0416', '\u0417', '\u0418', '\u0419', 
		'\u041A', '\u041B', '\u041C', '\u041D', '\u041E', '\u041F', 
		'\u0420', '\u0421', '\u0422', '\u0423', '\u0424', '\u0425', 
		'\u0426', '\u0427', '\u0428', '\u0429', '\u042A', '\u042B', 
		'\u042C', '\u042D', '\u042E', '\u042F', '\u2020', '\u00B0', 
		'\u00A2', '\u00A3', '\u00A7', '\u2022', '\u00B6', '\u0406', 
		'\u00AE', '\u00A9', '\u2122', '\u0402', '\u0452', '\u2260', 
		'\u0403', '\u0453', '\u221E', '\u00B1', '\u2264', '\u2265', 
		'\u0456', '\u00B5', '\u2202', '\u0408', '\u0404', '\u0454', 
		'\u0407', '\u0457', '\u0409', '\u0459', '\u040A', '\u045A', 
		'\u0458', '\u0405', '\u00AC', '\u221A', '\u0192', '\u2248', 
		'\u2206', '\u00AB', '\u00BB', '\u2026', '\u00A0', '\u040B', 
		'\u045B', '\u040C', '\u045C', '\u0455', '\u2013', '\u2014', 
		'\u201C', '\u201D', '\u2018', '\u2019', '\u00F7', '\u201E', 
		'\u040E', '\u045E', '\u040F', '\u045F', '\u2116', '\u0401', 
		'\u0451', '\u044F', '\u0430', '\u0431', '\u0432', '\u0433', 
		'\u0434', '\u0435', '\u0436', '\u0437', '\u0438', '\u0439', 
		'\u043A', '\u043B', '\u043C', '\u043D', '\u043E', '\u043F', 
		'\u0440', '\u0441', '\u0442', '\u0443', '\u0444', '\u0445', 
		'\u0446', '\u0447', '\u0448', '\u0449', '\u044A', '\u044B', 
		'\u044C', '\u044D', '\u044E', '\u00A4', 
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
				case 0x00A2:
				case 0x00A3:
				case 0x00A9:
				case 0x00B1:
				case 0x00B5:
					break;
				case 0x00A0: ch = 0xCA; break;
				case 0x00A4: ch = 0xFF; break;
				case 0x00A7: ch = 0xA4; break;
				case 0x00AB: ch = 0xC7; break;
				case 0x00AC: ch = 0xC2; break;
				case 0x00AE: ch = 0xA8; break;
				case 0x00B0: ch = 0xA1; break;
				case 0x00B6: ch = 0xA6; break;
				case 0x00BB: ch = 0xC8; break;
				case 0x00F7: ch = 0xD6; break;
				case 0x0192: ch = 0xC4; break;
				case 0x0401: ch = 0xDD; break;
				case 0x0402: ch = 0xAB; break;
				case 0x0403: ch = 0xAE; break;
				case 0x0404: ch = 0xB8; break;
				case 0x0405: ch = 0xC1; break;
				case 0x0406: ch = 0xA7; break;
				case 0x0407: ch = 0xBA; break;
				case 0x0408: ch = 0xB7; break;
				case 0x0409: ch = 0xBC; break;
				case 0x040A: ch = 0xBE; break;
				case 0x040B: ch = 0xCB; break;
				case 0x040C: ch = 0xCD; break;
				case 0x040E: ch = 0xD8; break;
				case 0x040F: ch = 0xDA; break;
				case 0x0410:
				case 0x0411:
				case 0x0412:
				case 0x0413:
				case 0x0414:
				case 0x0415:
				case 0x0416:
				case 0x0417:
				case 0x0418:
				case 0x0419:
				case 0x041A:
				case 0x041B:
				case 0x041C:
				case 0x041D:
				case 0x041E:
				case 0x041F:
				case 0x0420:
				case 0x0421:
				case 0x0422:
				case 0x0423:
				case 0x0424:
				case 0x0425:
				case 0x0426:
				case 0x0427:
				case 0x0428:
				case 0x0429:
				case 0x042A:
				case 0x042B:
				case 0x042C:
				case 0x042D:
				case 0x042E:
				case 0x042F:
					ch -= 0x0390;
					break;
				case 0x0430:
				case 0x0431:
				case 0x0432:
				case 0x0433:
				case 0x0434:
				case 0x0435:
				case 0x0436:
				case 0x0437:
				case 0x0438:
				case 0x0439:
				case 0x043A:
				case 0x043B:
				case 0x043C:
				case 0x043D:
				case 0x043E:
				case 0x043F:
				case 0x0440:
				case 0x0441:
				case 0x0442:
				case 0x0443:
				case 0x0444:
				case 0x0445:
				case 0x0446:
				case 0x0447:
				case 0x0448:
				case 0x0449:
				case 0x044A:
				case 0x044B:
				case 0x044C:
				case 0x044D:
				case 0x044E:
					ch -= 0x0350;
					break;
				case 0x044F: ch = 0xDF; break;
				case 0x0451: ch = 0xDE; break;
				case 0x0452: ch = 0xAC; break;
				case 0x0453: ch = 0xAF; break;
				case 0x0454: ch = 0xB9; break;
				case 0x0455: ch = 0xCF; break;
				case 0x0456: ch = 0xB4; break;
				case 0x0457: ch = 0xBB; break;
				case 0x0458: ch = 0xC0; break;
				case 0x0459: ch = 0xBD; break;
				case 0x045A: ch = 0xBF; break;
				case 0x045B: ch = 0xCC; break;
				case 0x045C: ch = 0xCE; break;
				case 0x045E: ch = 0xD9; break;
				case 0x045F: ch = 0xDB; break;
				case 0x2013: ch = 0xD0; break;
				case 0x2014: ch = 0xD1; break;
				case 0x2018: ch = 0xD4; break;
				case 0x2019: ch = 0xD5; break;
				case 0x201C: ch = 0xD2; break;
				case 0x201D: ch = 0xD3; break;
				case 0x201E: ch = 0xD7; break;
				case 0x2020: ch = 0xA0; break;
				case 0x2022: ch = 0xA5; break;
				case 0x2026: ch = 0xC9; break;
				case 0x2116: ch = 0xDC; break;
				case 0x2122: ch = 0xAA; break;
				case 0x2202: ch = 0xB6; break;
				case 0x2206: ch = 0xC6; break;
				case 0x221A: ch = 0xC3; break;
				case 0x221E: ch = 0xB0; break;
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
				case 0x00A2:
				case 0x00A3:
				case 0x00A9:
				case 0x00B1:
				case 0x00B5:
					break;
				case 0x00A0: ch = 0xCA; break;
				case 0x00A4: ch = 0xFF; break;
				case 0x00A7: ch = 0xA4; break;
				case 0x00AB: ch = 0xC7; break;
				case 0x00AC: ch = 0xC2; break;
				case 0x00AE: ch = 0xA8; break;
				case 0x00B0: ch = 0xA1; break;
				case 0x00B6: ch = 0xA6; break;
				case 0x00BB: ch = 0xC8; break;
				case 0x00F7: ch = 0xD6; break;
				case 0x0192: ch = 0xC4; break;
				case 0x0401: ch = 0xDD; break;
				case 0x0402: ch = 0xAB; break;
				case 0x0403: ch = 0xAE; break;
				case 0x0404: ch = 0xB8; break;
				case 0x0405: ch = 0xC1; break;
				case 0x0406: ch = 0xA7; break;
				case 0x0407: ch = 0xBA; break;
				case 0x0408: ch = 0xB7; break;
				case 0x0409: ch = 0xBC; break;
				case 0x040A: ch = 0xBE; break;
				case 0x040B: ch = 0xCB; break;
				case 0x040C: ch = 0xCD; break;
				case 0x040E: ch = 0xD8; break;
				case 0x040F: ch = 0xDA; break;
				case 0x0410:
				case 0x0411:
				case 0x0412:
				case 0x0413:
				case 0x0414:
				case 0x0415:
				case 0x0416:
				case 0x0417:
				case 0x0418:
				case 0x0419:
				case 0x041A:
				case 0x041B:
				case 0x041C:
				case 0x041D:
				case 0x041E:
				case 0x041F:
				case 0x0420:
				case 0x0421:
				case 0x0422:
				case 0x0423:
				case 0x0424:
				case 0x0425:
				case 0x0426:
				case 0x0427:
				case 0x0428:
				case 0x0429:
				case 0x042A:
				case 0x042B:
				case 0x042C:
				case 0x042D:
				case 0x042E:
				case 0x042F:
					ch -= 0x0390;
					break;
				case 0x0430:
				case 0x0431:
				case 0x0432:
				case 0x0433:
				case 0x0434:
				case 0x0435:
				case 0x0436:
				case 0x0437:
				case 0x0438:
				case 0x0439:
				case 0x043A:
				case 0x043B:
				case 0x043C:
				case 0x043D:
				case 0x043E:
				case 0x043F:
				case 0x0440:
				case 0x0441:
				case 0x0442:
				case 0x0443:
				case 0x0444:
				case 0x0445:
				case 0x0446:
				case 0x0447:
				case 0x0448:
				case 0x0449:
				case 0x044A:
				case 0x044B:
				case 0x044C:
				case 0x044D:
				case 0x044E:
					ch -= 0x0350;
					break;
				case 0x044F: ch = 0xDF; break;
				case 0x0451: ch = 0xDE; break;
				case 0x0452: ch = 0xAC; break;
				case 0x0453: ch = 0xAF; break;
				case 0x0454: ch = 0xB9; break;
				case 0x0455: ch = 0xCF; break;
				case 0x0456: ch = 0xB4; break;
				case 0x0457: ch = 0xBB; break;
				case 0x0458: ch = 0xC0; break;
				case 0x0459: ch = 0xBD; break;
				case 0x045A: ch = 0xBF; break;
				case 0x045B: ch = 0xCC; break;
				case 0x045C: ch = 0xCE; break;
				case 0x045E: ch = 0xD9; break;
				case 0x045F: ch = 0xDB; break;
				case 0x2013: ch = 0xD0; break;
				case 0x2014: ch = 0xD1; break;
				case 0x2018: ch = 0xD4; break;
				case 0x2019: ch = 0xD5; break;
				case 0x201C: ch = 0xD2; break;
				case 0x201D: ch = 0xD3; break;
				case 0x201E: ch = 0xD7; break;
				case 0x2020: ch = 0xA0; break;
				case 0x2022: ch = 0xA5; break;
				case 0x2026: ch = 0xC9; break;
				case 0x2116: ch = 0xDC; break;
				case 0x2122: ch = 0xAA; break;
				case 0x2202: ch = 0xB6; break;
				case 0x2206: ch = 0xC6; break;
				case 0x221A: ch = 0xC3; break;
				case 0x221E: ch = 0xB0; break;
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

}; // class CP10017

public class ENCwindows_10017 : CP10017
{
	public ENCwindows_10017() : base() {}

}; // class ENCwindows_10017

}; // namespace I18N.Other
