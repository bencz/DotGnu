/*
 * CID0024.cs - sl culture handler.
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

// Generated from "sl.txt".

namespace I18N.Other
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0024 : RootCulture
{
	public CID0024() : base(0x0024) {}
	public CID0024(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "sl";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "slv";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "SLV";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "sl";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"ned", "pon", "tor", "sre", "\u010det", "pet", "sob"};
			dfi.DayNames = new String[] {"nedelja", "ponedeljek", "torek", "sreda", "\u010detrtek", "petek", "sobota"};
			dfi.AbbreviatedMonthNames = new String[] {"jan", "feb", "mar", "apr", "maj", "jun", "jul", "avg", "sep", "okt", "nov", "dec", ""};
			dfi.MonthNames = new String[] {"januar", "februar", "marec", "april", "maj", "junij", "julij", "avgust", "september", "oktober", "november", "december", ""};
			dfi.DateSeparator = ".";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "dd. MMMM yyyy";
			dfi.LongTimePattern = "H:mm:ss z";
			dfi.ShortDatePattern = "yy.M.d";
			dfi.ShortTimePattern = "H:mm";
			dfi.FullDateTimePattern = "dddd, dd. MMMM yyyy H:mm:ss z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:yy.M.d",
				"D:dddd, dd. MMMM yyyy",
				"f:dddd, dd. MMMM yyyy H:mm:ss z",
				"f:dddd, dd. MMMM yyyy H:mm:ss z",
				"f:dddd, dd. MMMM yyyy H:mm:ss",
				"f:dddd, dd. MMMM yyyy H:mm",
				"F:dddd, dd. MMMM yyyy HH:mm:ss",
				"g:yy.M.d H:mm:ss z",
				"g:yy.M.d H:mm:ss z",
				"g:yy.M.d H:mm:ss",
				"g:yy.M.d H:mm",
				"G:yy.M.d HH:mm:ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:H:mm:ss z",
				"t:H:mm:ss z",
				"t:H:mm:ss",
				"t:H:mm",
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
			case "sl": return "Sloven\u0161\u010Dina";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "SI": return "Slovenija";
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
				return 20880;
			}
		}
		public override int MacCodePage
		{
			get
			{
				return 10029;
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

}; // class CID0024

public class CNsl : CID0024
{
	public CNsl() : base() {}

}; // class CNsl

}; // namespace I18N.Other
