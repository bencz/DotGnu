/*
 * CID001a.cs - hr culture handler.
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

// Generated from "hr.txt".

namespace I18N.Other
{

using System;
using System.Globalization;
using I18N.Common;

public class CID001a : RootCulture
{
	public CID001a() : base(0x001A) {}
	public CID001a(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "hr";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "hrv";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "HRV";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "hr";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"ned", "pon", "uto", "sri", "\u010Det", "pet", "sub"};
			dfi.DayNames = new String[] {"nedjelja", "ponedjeljak", "utorak", "srijeda", "\u010Detvrtak", "petak", "subota"};
			dfi.AbbreviatedMonthNames = new String[] {"sij", "vel", "o\u017Eu", "tra", "svi", "lip", "srp", "kol", "ruj", "lis", "stu", "pro", ""};
			dfi.MonthNames = new String[] {"sije\u010Danj", "velja\u010Da", "o\u017Eujak", "travanj", "svibanj", "lipanj", "srpanj", "kolovoz", "rujan", "listopad", "studeni", "prosinac", ""};
			dfi.DateSeparator = ".";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "yyyy. MMMM dd";
			dfi.LongTimePattern = "HH:mm:ss z";
			dfi.ShortDatePattern = "yyyy.MM.dd";
			dfi.ShortTimePattern = "HH:mm";
			dfi.FullDateTimePattern = "yyyy. MMMM dd HH:mm:ss z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:yyyy.MM.dd",
				"D:yyyy. MMMM dd",
				"f:yyyy. MMMM dd HH:mm:ss z",
				"f:yyyy. MMMM dd HH:mm:ss z",
				"f:yyyy. MMMM dd HH:mm:ss",
				"f:yyyy. MMMM dd HH:mm",
				"F:yyyy. MMMM dd HH:mm:ss",
				"g:yyyy.MM.dd HH:mm:ss z",
				"g:yyyy.MM.dd HH:mm:ss z",
				"g:yyyy.MM.dd HH:mm:ss",
				"g:yyyy.MM.dd HH:mm",
				"G:yyyy.MM.dd HH:mm:ss",
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
			case "hr": return "hrvatski";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "HR": return "Hrvatska";
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
				return 1250;
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
				return 10082;
			}
		}
		public override int OEMCodePage
		{
			get
			{
				return 852;
			}
		}
		public override String ListSeparator
		{
			get
			{
				return ";";
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

}; // class CID001a

public class CNhr : CID001a
{
	public CNhr() : base() {}

}; // class CNhr

}; // namespace I18N.Other
