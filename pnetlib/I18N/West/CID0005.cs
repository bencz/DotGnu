/*
 * CID0005.cs - cs culture handler.
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

// Generated from "cs.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0005 : RootCulture
{
	public CID0005() : base(0x0005) {}
	public CID0005(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "cs";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "ces";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "CSY";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "cs";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AMDesignator = "dop.";
			dfi.PMDesignator = "odp.";
			dfi.AbbreviatedDayNames = new String[] {"ne", "po", "\u00FAt", "st", "\u010dt", "p\u00E1", "so"};
			dfi.DayNames = new String[] {"ned\u011Ble", "pond\u011Bl\u00ED", "\u00FAter\u00FD", "st\u0159eda", "\u010dtvrtek", "p\u00E1tek", "sobota"};
			dfi.AbbreviatedMonthNames = new String[] {"I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI", "XII", ""};
			dfi.MonthNames = new String[] {"leden", "\u00FAnor", "b\u0159ezen", "duben", "kv\u011Bten", "\u010Derven", "\u010Dervenec", "srpen", "z\u00E1\u0159\u00ED", "\u0159\u00EDjen", "listopad", "prosinec", ""};
			dfi.DateSeparator = ".";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "d. MMMM yyyy";
			dfi.LongTimePattern = "H:mm:ss z";
			dfi.ShortDatePattern = "d.M.yy";
			dfi.ShortTimePattern = "H:mm";
			dfi.FullDateTimePattern = "dddd, d. MMMM yyyy H:mm:ss z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:d.M.yy",
				"D:dddd, d. MMMM yyyy",
				"f:dddd, d. MMMM yyyy H:mm:ss z",
				"f:dddd, d. MMMM yyyy H:mm:ss z",
				"f:dddd, d. MMMM yyyy H:mm:ss",
				"f:dddd, d. MMMM yyyy H:mm",
				"F:dddd, d. MMMM yyyy HH:mm:ss",
				"g:d.M.yy H:mm:ss z",
				"g:d.M.yy H:mm:ss z",
				"g:d.M.yy H:mm:ss",
				"g:d.M.yy H:mm",
				"G:d.M.yy HH:mm:ss",
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
			nfi.CurrencyGroupSeparator = "\u00A0";
			nfi.NumberGroupSeparator = "\u00A0";
			nfi.PercentGroupSeparator = "\u00A0";
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
			case "cs": return "\u010De\u0161tina";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "CZ": return "\u010Cesk\u00E1 republika";
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

}; // class CID0005

public class CNcs : CID0005
{
	public CNcs() : base() {}

}; // class CNcs

}; // namespace I18N.West
