/*
 * CID000f.cs - is culture handler.
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

// Generated from "is.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID000f : RootCulture
{
	public CID000f() : base(0x000F) {}
	public CID000f(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "is";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "isl";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "ISL";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "is";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"sun.", "m\u00E1n.", "\u00FEri.", "mi\u00F0.", "fim.", "f\u00F6s.", "lau."};
			dfi.DayNames = new String[] {"sunnudagur", "m\u00E1nudagur", "\u00FEri\u00F0judagur", "mi\u00F0vikudagur", "fimmtudagur", "f\u00F6studagur", "laugardagur"};
			dfi.AbbreviatedMonthNames = new String[] {"jan.", "feb.", "mar.", "apr.", "ma\u00ED", "j\u00FAn.", "j\u00FAl.", "\u00E1g\u00FA.", "sep.", "okt.", "n\u00F3v.", "des.", ""};
			dfi.MonthNames = new String[] {"jan\u00FAar", "febr\u00FAar", "mars", "apr\u00EDl", "ma\u00ED", "j\u00FAn\u00ED", "j\u00FAl\u00ED", "\u00E1g\u00FAst", "september", "okt\u00F3ber", "n\u00F3vember", "desember", ""};
			dfi.DateSeparator = ".";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "d. MMMM yyyy";
			dfi.LongTimePattern = "HH:mm:ss z";
			dfi.ShortDatePattern = "d.M.yyyy";
			dfi.ShortTimePattern = "HH:mm";
			dfi.FullDateTimePattern = "d. MMMM yyyy HH:mm:ss z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:d.M.yyyy",
				"D:d. MMMM yyyy",
				"f:d. MMMM yyyy HH:mm:ss z",
				"f:d. MMMM yyyy HH:mm:ss z",
				"f:d. MMMM yyyy HH:mm:ss",
				"f:d. MMMM yyyy HH:mm",
				"F:d. MMMM yyyy HH:mm:ss",
				"g:d.M.yyyy HH:mm:ss z",
				"g:d.M.yyyy HH:mm:ss z",
				"g:d.M.yyyy HH:mm:ss",
				"g:d.M.yyyy HH:mm",
				"G:d.M.yyyy HH:mm:ss",
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
			case "is": return "\u00EDslenska";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "IS": return "\u00CDsland";
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
				return 20871;
			}
		}
		public override int MacCodePage
		{
			get
			{
				return 10079;
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

}; // class CID000f

public class CNis : CID000f
{
	public CNis() : base() {}

}; // class CNis

}; // namespace I18N.West
