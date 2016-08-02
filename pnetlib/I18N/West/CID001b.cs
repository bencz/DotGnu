/*
 * CID001b.cs - sk culture handler.
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

// Generated from "sk.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID001b : RootCulture
{
	public CID001b() : base(0x001B) {}
	public CID001b(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "sk";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "slk";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "SKY";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "sk";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"Ne", "Po", "Ut", "St", "\u0160t", "Pa", "So"};
			dfi.DayNames = new String[] {"Nede\u013Ee", "Pondelok", "Utorok", "Streda", "\u0160tvrtok", "Piatok", "Sobota"};
			dfi.AbbreviatedMonthNames = new String[] {"jan", "feb", "mar", "apr", "m\u00E1j", "j\u00FAn", "j\u00FAl", "aug", "sep", "okt", "nov", "dec", ""};
			dfi.MonthNames = new String[] {"janu\u00E1r", "febru\u00E1r", "marec", "apr\u00EDl", "m\u00E1j", "j\u00FAn", "j\u00FAl", "august", "september", "okt\u00F3ber", "november", "december", ""};
			dfi.DateSeparator = ".";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "d. MMMM yyyy";
			dfi.LongTimePattern = "H:mm:ss z";
			dfi.ShortDatePattern = "d.M.yyyy";
			dfi.ShortTimePattern = "H:mm";
			dfi.FullDateTimePattern = "dddd, d. MMMM yyyy H:mm:ss z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:d.M.yyyy",
				"D:dddd, d. MMMM yyyy",
				"f:dddd, d. MMMM yyyy H:mm:ss z",
				"f:dddd, d. MMMM yyyy H:mm:ss z",
				"f:dddd, d. MMMM yyyy H:mm:ss",
				"f:dddd, d. MMMM yyyy H:mm",
				"F:dddd, d. MMMM yyyy HH:mm:ss",
				"g:d.M.yyyy H:mm:ss z",
				"g:d.M.yyyy H:mm:ss z",
				"g:d.M.yyyy H:mm:ss",
				"g:d.M.yyyy H:mm",
				"G:d.M.yyyy HH:mm:ss",
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
			case "sk": return "Sloven\u010Dina";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "SK": return "Slovensk\u00E1 republika";
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

}; // class CID001b

public class CNsk : CID001b
{
	public CNsk() : base() {}

}; // class CNsk

}; // namespace I18N.West
