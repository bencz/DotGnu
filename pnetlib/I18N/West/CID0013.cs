/*
 * CID0013.cs - nl culture handler.
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

// Generated from "nl.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0013 : RootCulture
{
	public CID0013() : base(0x0013) {}
	public CID0013(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "nl";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "nld";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "NLD";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "nl";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"zo", "ma", "di", "wo", "do", "vr", "za"};
			dfi.DayNames = new String[] {"zondag", "maandag", "dinsdag", "woensdag", "donderdag", "vrijdag", "zaterdag"};
			dfi.AbbreviatedMonthNames = new String[] {"jan", "feb", "mrt", "apr", "mei", "jun", "jul", "aug", "sep", "okt", "nov", "dec", ""};
			dfi.MonthNames = new String[] {"januari", "februari", "maart", "april", "mei", "juni", "juli", "augustus", "september", "oktober", "november", "december", ""};
			dfi.DateSeparator = "-";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "d MMMM yyyy";
			dfi.LongTimePattern = "H:mm:ss z";
			dfi.ShortDatePattern = "d-M-yy";
			dfi.ShortTimePattern = "H:mm";
			dfi.FullDateTimePattern = "dddd d MMMM yyyy H:mm:ss' uur' z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:d-M-yy",
				"D:dddd d MMMM yyyy",
				"f:dddd d MMMM yyyy H:mm:ss' uur' z",
				"f:dddd d MMMM yyyy H:mm:ss z",
				"f:dddd d MMMM yyyy H:mm:ss",
				"f:dddd d MMMM yyyy H:mm",
				"F:dddd d MMMM yyyy HH:mm:ss",
				"g:d-M-yy H:mm:ss' uur' z",
				"g:d-M-yy H:mm:ss z",
				"g:d-M-yy H:mm:ss",
				"g:d-M-yy H:mm",
				"G:d-M-yy HH:mm:ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:H:mm:ss' uur' z",
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
			case "nl": return "Nederlands";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "NL": return "Nederland";
			case "BE": return "Belgi\u00EB";
		}
		return base.ResolveCountry(name);
	}

	private class PrivateTextInfo : _I18NTextInfo
	{
		public PrivateTextInfo(int culture) : base(culture) {}

		public override int EBCDICCodePage
		{
			get
			{
				return 500;
			}
		}
		public override int OEMCodePage
		{
			get
			{
				return 850;
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

}; // class CID0013

public class CNnl : CID0013
{
	public CNnl() : base() {}

}; // class CNnl

}; // namespace I18N.West
