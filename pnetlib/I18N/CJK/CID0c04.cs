/*
 * CID0c04.cs - zh-HK culture handler.
 *
 * Copyright (c) 2003  Southern Storm Software, Pty Ltd
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

// Generated from "zh_HK.txt".

namespace I18N.CJK
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0c04 : CID0004
{
	public CID0c04() : base(0x0C04) {}

	public override String Name
	{
		get
		{
			return "zh-HK";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "ZHH";
		}
	}
	public override String Country
	{
		get
		{
			return "HK";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"\u65E5", "\u4E00", "\u4E8C", "\u4E09", "\u56DB", "\u4E94", "\u516D"};
			dfi.AbbreviatedMonthNames = new String[] {"1\u6708", "2\u6708", "3\u6708", "4\u6708", "5\u6708", "6\u6708", "7\u6708", "8\u6708", "9\u6708", "10\u6708", "11\u6708", "12\u6708", ""};
			dfi.DateSeparator = "'";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "yyyy'\u5d74'MM'\u6708'dd'\u65d5' dddd";
			dfi.LongTimePattern = "tthh'\u6642'mm'\u5206'ss'\u79D2'";
			dfi.ShortDatePattern = "yy'\u5d74'M'\u6708'd'\u65d5'";
			dfi.ShortTimePattern = "tth:mm";
			dfi.FullDateTimePattern = "yyyy'\u5d74'MM'\u6708'dd'\u65d5' dddd tthh'\u6642'mm'\u5206'ss'\u79D2' z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:yy'\u5d74'M'\u6708'd'\u65d5'",
				"D:yyyy'\u5d74'MM'\u6708'dd'\u65d5' dddd",
				"f:yyyy'\u5d74'MM'\u6708'dd'\u65d5' dddd tthh'\u6642'mm'\u5206'ss'\u79D2' z",
				"f:yyyy'\u5d74'MM'\u6708'dd'\u65d5' dddd tthh'\u6642'mm'\u5206'ss'\u79D2'",
				"f:yyyy'\u5d74'MM'\u6708'dd'\u65d5' dddd tthh:mm:ss",
				"f:yyyy'\u5d74'MM'\u6708'dd'\u65d5' dddd tth:mm",
				"F:yyyy'\u5d74'MM'\u6708'dd'\u65d5' dddd HH:mm:ss",
				"g:yy'\u5d74'M'\u6708'd'\u65d5' tthh'\u6642'mm'\u5206'ss'\u79D2' z",
				"g:yy'\u5d74'M'\u6708'd'\u65d5' tthh'\u6642'mm'\u5206'ss'\u79D2'",
				"g:yy'\u5d74'M'\u6708'd'\u65d5' tthh:mm:ss",
				"g:yy'\u5d74'M'\u6708'd'\u65d5' tth:mm",
				"G:yy'\u5d74'M'\u6708'd'\u65d5' HH:mm:ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:tthh'\u6642'mm'\u5206'ss'\u79D2' z",
				"t:tthh'\u6642'mm'\u5206'ss'\u79D2'",
				"t:tthh:mm:ss",
				"t:tth:mm",
				"T:HH:mm:ss",
				"u:yyyy'-'MM'-'dd HH':'mm':'ss'Z'",
				"U:dddd, dd MMMM yyyy HH:mm:ss",
				"y:yyyy MMMM",
				"Y:yyyy MMMM",
			});
			return dfi;
		}
		set
		{
			base.DateTimeFormat = value; // not used
		}
	}

	private class PrivateTextInfo : _I18NTextInfo
	{
		public PrivateTextInfo(int culture) : base(culture) {}

		public override int ANSICodePage
		{
			get
			{
				return 950;
			}
		}
		public override int EBCDICCodePage
		{
			get
			{
				return 500;
			}
		}
		public override int MacCodePage
		{
			get
			{
				return 10002;
			}
		}
		public override int OEMCodePage
		{
			get
			{
				return 950;
			}
		}

	}; // class PrivateTextInfo

	public override TextInfo TextInfo
	{
		get
		{
			return new PrivateTextInfo(LCID);
		}
	}

}; // class CID0c04

public class CNzh_hk : CID0c04
{
	public CNzh_hk() : base() {}

}; // class CNzh_hk

}; // namespace I18N.CJK
