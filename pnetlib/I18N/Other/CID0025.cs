/*
 * CID0025.cs - et culture handler.
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

// Generated from "et.txt".

namespace I18N.Other
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0025 : RootCulture
{
	public CID0025() : base(0x0025) {}
	public CID0025(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "et";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "est";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "ETI";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "et";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"P", "E", "T", "K", "N", "R", "L"};
			dfi.DayNames = new String[] {"p\u00FChap\u00E4ev", "esmasp\u00E4ev", "teisip\u00E4ev", "kolmap\u00E4ev", "neljap\u00E4ev", "reede", "laup\u00E4ev"};
			dfi.AbbreviatedMonthNames = new String[] {"jaan", "veebr", "m\u00E4rts", "apr", "mai", "juuni", "juuli", "aug", "sept", "okt", "nov", "dets", ""};
			dfi.MonthNames = new String[] {"jaanuar", "veebruar", "m\u00E4rts", "aprill", "mai", "juuni", "juuli", "august", "september", "oktoober", "november", "detsember", ""};
			dfi.DateSeparator = ".";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "dddd, d, MMMM yyyy";
			dfi.LongTimePattern = "H:mm:ss z";
			dfi.ShortDatePattern = "dd.MM.yy";
			dfi.ShortTimePattern = "H:mm";
			dfi.FullDateTimePattern = "dddd, d, MMMM yyyy H:mm:ss z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:dd.MM.yy",
				"D:dddd, d, MMMM yyyy",
				"f:dddd, d, MMMM yyyy H:mm:ss z",
				"f:dddd, d, MMMM yyyy H:mm:ss z",
				"f:dddd, d, MMMM yyyy H:mm:ss",
				"f:dddd, d, MMMM yyyy H:mm",
				"F:dddd, d, MMMM yyyy HH:mm:ss",
				"g:dd.MM.yy H:mm:ss z",
				"g:dd.MM.yy H:mm:ss z",
				"g:dd.MM.yy H:mm:ss",
				"g:dd.MM.yy H:mm",
				"G:dd.MM.yy HH:mm:ss",
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
			case "et": return "Eesti";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "EE": return "Eesti";
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
				return 1257;
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
				return 775;
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

}; // class CID0025

public class CNet : CID0025
{
	public CNet() : base() {}

}; // class CNet

}; // namespace I18N.Other
