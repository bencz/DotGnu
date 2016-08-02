/*
 * CID000e.cs - hu culture handler.
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

// Generated from "hu.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID000e : RootCulture
{
	public CID000e() : base(0x000E) {}
	public CID000e(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "hu";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "hun";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "HUN";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "hu";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AMDesignator = "DE";
			dfi.PMDesignator = "DU";
			dfi.AbbreviatedDayNames = new String[] {"V", "H", "K", "Sze", "Cs", "P", "Szo"};
			dfi.DayNames = new String[] {"vas\u00E1rnap", "h\u00E9tf\u0151", "kedd", "szerda", "cs\u00FCt\u00F6rt\u00F6k", "p\u00E9ntek", "szombat"};
			dfi.AbbreviatedMonthNames = new String[] {"jan.", "febr.", "m\u00E1rc.", "\u00E1pr.", "m\u00E1j.", "j\u00FAn.", "j\u00FAl.", "aug.", "szept.", "okt.", "nov.", "dec.", ""};
			dfi.MonthNames = new String[] {"janu\u00E1r", "febru\u00E1r", "m\u00E1rcius", "\u00E1prilis", "m\u00E1jus", "j\u00FAnius", "j\u00FAlius", "augusztus", "szeptember", "okt\u00F3ber", "november", "december", ""};
			dfi.DateSeparator = ".";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "yyyy. MMMM d.";
			dfi.LongTimePattern = "H:mm:ss z";
			dfi.ShortDatePattern = "yyyy.MM.dd.";
			dfi.ShortTimePattern = "H:mm";
			dfi.FullDateTimePattern = "yyyy. MMMM d. H:mm:ss z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:yyyy.MM.dd.",
				"D:yyyy. MMMM d.",
				"f:yyyy. MMMM d. H:mm:ss z",
				"f:yyyy. MMMM d. H:mm:ss z",
				"f:yyyy. MMMM d. H:mm:ss",
				"f:yyyy. MMMM d. H:mm",
				"F:yyyy. MMMM d. HH:mm:ss",
				"g:yyyy.MM.dd. H:mm:ss z",
				"g:yyyy.MM.dd. H:mm:ss z",
				"g:yyyy.MM.dd. H:mm:ss",
				"g:yyyy.MM.dd. H:mm",
				"G:yyyy.MM.dd. HH:mm:ss",
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
			case "hu": return "magyar";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "HU": return "Magyarorsz\u00E1g";
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

}; // class CID000e

public class CNhu : CID000e
{
	public CNhu() : base() {}

}; // class CNhu

}; // namespace I18N.West
