/*
 * CID0010.cs - it culture handler.
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

// Generated from "it.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0010 : RootCulture
{
	public CID0010() : base(0x0010) {}
	public CID0010(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "it";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "ita";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "ITA";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "it";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AMDesignator = "m.";
			dfi.PMDesignator = "p.";
			dfi.AbbreviatedDayNames = new String[] {"dom", "lun", "mar", "mer", "gio", "ven", "sab"};
			dfi.DayNames = new String[] {"domenica", "luned\u00EC", "marted\u00EC", "mercoled\u00EC", "gioved\u00EC", "venerd\u00EC", "sabato"};
			dfi.AbbreviatedMonthNames = new String[] {"gen", "feb", "mar", "apr", "mag", "giu", "lug", "ago", "set", "ott", "nov", "dic", ""};
			dfi.MonthNames = new String[] {"gennaio", "febbraio", "marzo", "aprile", "maggio", "giugno", "luglio", "agosto", "settembre", "ottobre", "novembre", "dicembre", ""};
			dfi.DateSeparator = "/";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "dd MMMM yyyy";
			dfi.LongTimePattern = "HH:mm:ss z";
			dfi.ShortDatePattern = "dd/MM/yy";
			dfi.ShortTimePattern = "HH:mm";
			dfi.FullDateTimePattern = "dddd d MMMM yyyy HH:mm:ss z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:dd/MM/yy",
				"D:dddd d MMMM yyyy",
				"f:dddd d MMMM yyyy HH:mm:ss z",
				"f:dddd d MMMM yyyy HH:mm:ss z",
				"f:dddd d MMMM yyyy HH:mm:ss",
				"f:dddd d MMMM yyyy HH:mm",
				"F:dddd d MMMM yyyy HH:mm:ss",
				"g:dd/MM/yy HH:mm:ss z",
				"g:dd/MM/yy HH:mm:ss z",
				"g:dd/MM/yy HH:mm:ss",
				"g:dd/MM/yy HH:mm",
				"G:dd/MM/yy HH:mm:ss",
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
			case "it": return "italiano";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "IT": return "Italia";
			case "CH": return "Svizzera";
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
				return 20280;
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

}; // class CID0010

public class CNit : CID0010
{
	public CNit() : base() {}

}; // class CNit

}; // namespace I18N.West
