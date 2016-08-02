/*
 * CID0804.cs - zh-CN culture handler.
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

// Generated from "zh_CN.txt".

namespace I18N.CJK
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0804 : CID0004
{
	public CID0804() : base(0x0804) {}

	public override String Name
	{
		get
		{
			return "zh-CN";
		}
	}
	public override String Country
	{
		get
		{
			return "CN";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.DateSeparator = "-";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "yyyy'\u5d74'M'\u6708'd'\u65d5'";
			dfi.LongTimePattern = "tthh'\u65F6'mm'\u5206'ss'\u79D2'";
			dfi.ShortDatePattern = "yy-M-d";
			dfi.ShortTimePattern = "tth:mm";
			dfi.FullDateTimePattern = "yyyy'\u5d74'M'\u6708'd'\u65d5' HH'\u65F6'mm'\u5206'ss'\u79D2' z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:yy-M-d",
				"D:yyyy'\u5d74'M'\u6708'd'\u65d5'",
				"f:yyyy'\u5d74'M'\u6708'd'\u65d5' HH'\u65F6'mm'\u5206'ss'\u79D2' z",
				"f:yyyy'\u5d74'M'\u6708'd'\u65d5' tthh'\u65F6'mm'\u5206'ss'\u79D2'",
				"f:yyyy'\u5d74'M'\u6708'd'\u65d5' H:mm:ss",
				"f:yyyy'\u5d74'M'\u6708'd'\u65d5' tth:mm",
				"F:yyyy'\u5d74'M'\u6708'd'\u65d5' HH:mm:ss",
				"g:yy-M-d HH'\u65F6'mm'\u5206'ss'\u79D2' z",
				"g:yy-M-d tthh'\u65F6'mm'\u5206'ss'\u79D2'",
				"g:yy-M-d H:mm:ss",
				"g:yy-M-d tth:mm",
				"G:yy-M-d HH:mm:ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:HH'\u65F6'mm'\u5206'ss'\u79D2' z",
				"t:tthh'\u65F6'mm'\u5206'ss'\u79D2'",
				"t:H:mm:ss",
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

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "CN": return "\u4E2D\u534E\u4EBA\u6C11\u5171\u548C\u56FD";
			case "TW": return "\u53F0\u6E7E";
			case "HK": return "\u9999\u6E2F";
		}
		return base.ResolveCountry(name);
	}

	private class PrivateTextInfo : _I18NTextInfo
	{
		public PrivateTextInfo(int culture) : base(culture) {}

		public override int ANSICodePage
		{
			get
			{
				return 936;
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
				return 10008;
			}
		}
		public override int OEMCodePage
		{
			get
			{
				return 936;
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

}; // class CID0804

public class CNzh_cn : CID0804
{
	public CNzh_cn() : base() {}

}; // class CNzh_cn

}; // namespace I18N.CJK
