/*
 * CID002b.cs - hy culture handler.
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

// Generated from "hy.txt".

namespace I18N.Other
{

using System;
using System.Globalization;
using I18N.Common;

public class CID002b : RootCulture
{
	public CID002b() : base(0x002B) {}
	public CID002b(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "hy";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "hye";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "HYE";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "hy";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AMDesignator = "\u0531\u057c\u2024";
			dfi.PMDesignator = "\u0535\u0580\u2024";
			dfi.AbbreviatedDayNames = new String[] {"\u053f\u056b\u0580", "\u0535\u0580\u056f", "\u0535\u0580\u0584", "\u0549\u0578\u0580", "\u0540\u0576\u0563", "\u0548\u0582\u0580", "\u0547\u0561\u0562"};
			dfi.DayNames = new String[] {"\u053f\u056b\u0580\u0561\u056f\u056b", "\u0535\u0580\u056f\u0578\u0582\u0577\u0561\u0562\u0569\u056b", "\u0535\u0580\u0565\u0584\u0577\u0561\u0562\u0569\u056b", "\u0549\u0578\u0580\u0565\u0584\u0577\u0561\u0562\u0569\u056b", "\u0540\u056b\u0576\u0563\u0577\u0561\u0562\u0569\u056b", "\u0548\u0582\u0580\u0562\u0561\u0569", "\u0547\u0561\u0562\u0561\u0569"};
			dfi.AbbreviatedMonthNames = new String[] {"\u0545\u0576\u0580", "\u0553\u057f\u0580", "\u0544\u0580\u057f", "\u0531\u057a\u0580", "\u0544\u0575\u057d", "\u0545\u0576\u057d", "\u0545\u056c\u057d", "\u0555\u0563\u057d", "\u054d\u0565\u057a", "\u0540\u0578\u056f", "\u0546\u0578\u0575", "\u0534\u0565\u056f", ""};
			dfi.MonthNames = new String[] {"\u0545\u0578\u0582\u0576\u0578\u0582\u0561\u0580", "\u0553\u0565\u057f\u0580\u0578\u0582\u0561\u0580", "\u0544\u0561\u0580\u057f", "\u0531\u057a\u0580\u056b\u056c", "\u0544\u0561\u0575\u056b\u057d", "\u0545\u0578\u0582\u0576\u056b\u057d", "\u0545\u0578\u0582\u056c\u056b\u057d", "\u0555\u0563\u0578\u057d\u057f\u0578\u057d", "\u054d\u0565\u057a\u057f\u0565\u0574\u0562\u0565\u0580", "\u0540\u0578\u056f\u057f\u0565\u0574\u0562\u0565\u0580", "\u0546\u0578\u0575\u0565\u0574\u0562\u0565\u0580", "\u0534\u0565\u056f\u057f\u0565\u0574\u0562\u0565\u0580", ""};
			dfi.DateSeparator = "/";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "MMMM dd, yyyy";
			dfi.LongTimePattern = "HH:mm:ss z";
			dfi.ShortDatePattern = "MM/dd/yy";
			dfi.ShortTimePattern = "HH:mm";
			dfi.FullDateTimePattern = "dddd,MMMM d, yyyy HH:mm:ss z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:MM/dd/yy",
				"D:dddd,MMMM d, yyyy",
				"f:dddd,MMMM d, yyyy HH:mm:ss z",
				"f:dddd,MMMM d, yyyy HH:mm:ss z",
				"f:dddd,MMMM d, yyyy HH:mm:ss",
				"f:dddd,MMMM d, yyyy HH:mm",
				"F:dddd,MMMM d, yyyy HH:mm:ss",
				"g:MM/dd/yy HH:mm:ss z",
				"g:MM/dd/yy HH:mm:ss z",
				"g:MM/dd/yy HH:mm:ss",
				"g:MM/dd/yy HH:mm",
				"G:MM/dd/yy HH:mm:ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:HH:mm:ss z",
				"t:HH:mm:ss z",
				"t:HH:mm:ss",
				"t:HH:mm",
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

	public override NumberFormatInfo NumberFormat
	{
		get
		{
			NumberFormatInfo nfi = base.NumberFormat;
			nfi.CurrencyDecimalSeparator = ",";
			nfi.CurrencyGroupSeparator = ".";
			nfi.NumberGroupSeparator = ".";
			nfi.PercentGroupSeparator = ".";
			nfi.NegativeSign = "-";
			nfi.NumberDecimalSeparator = ",";
			nfi.PercentDecimalSeparator = ",";
			nfi.PercentSymbol = "%";
			nfi.PerMilleSymbol = "\u2030";
			return nfi;
		}
		set
		{
			base.NumberFormat = value; // not used
		}
	}

	public override String ResolveLanguage(String name)
	{
		switch(name)
		{
			case "hy": return "\u0540\u0561\u0575\u0565\u0580\u0567\u0576";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "AM": return "\u0540\u0561\u0575\u0561\u057D\u057F\u0561\u0576\u056B\u0020";
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
				return 0;
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
				return 2;
			}
		}
		public override int OEMCodePage
		{
			get
			{
				return 1;
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

}; // class CID002b

public class CNhy : CID002b
{
	public CNhy() : base() {}

}; // class CNhy

}; // namespace I18N.Other
