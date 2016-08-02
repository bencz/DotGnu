/*
 * CID0006.cs - da culture handler.
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

// Generated from "da.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0006 : RootCulture
{
	public CID0006() : base(0x0006) {}
	public CID0006(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "da";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "dan";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "DAN";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "da";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"s\u00F8", "ma", "ti", "on", "to", "fr", "l\u00F8"};
			dfi.DayNames = new String[] {"s\u00F8ndag", "mandag", "tirsdag", "onsdag", "torsdag", "fredag", "l\u00F8rdag"};
			dfi.AbbreviatedMonthNames = new String[] {"jan", "feb", "mar", "apr", "maj", "jun", "jul", "aug", "sep", "okt", "nov", "dec", ""};
			dfi.MonthNames = new String[] {"januar", "februar", "marts", "april", "maj", "juni", "juli", "august", "september", "oktober", "november", "december", ""};
			dfi.DateSeparator = "-";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "d. MMMM yyyy";
			dfi.LongTimePattern = "HH:mm:ss z";
			dfi.ShortDatePattern = "dd-MM-yy";
			dfi.ShortTimePattern = "HH:mm";
			dfi.FullDateTimePattern = "d. MMMM yyyy HH:mm:ss z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:dd-MM-yy",
				"D:d. MMMM yyyy",
				"f:d. MMMM yyyy HH:mm:ss z",
				"f:d. MMMM yyyy HH:mm:ss z",
				"f:d. MMMM yyyy HH:mm:ss",
				"f:d. MMMM yyyy HH:mm",
				"F:d. MMMM yyyy HH:mm:ss",
				"g:dd-MM-yy HH:mm:ss z",
				"g:dd-MM-yy HH:mm:ss z",
				"g:dd-MM-yy HH:mm:ss",
				"g:dd-MM-yy HH:mm",
				"G:dd-MM-yy HH:mm:ss",
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
			case "da": return "dansk";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "DK": return "Danmark";
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
				return 20277;
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

}; // class CID0006

public class CNda : CID0006
{
	public CNda() : base() {}

}; // class CNda

}; // namespace I18N.West
