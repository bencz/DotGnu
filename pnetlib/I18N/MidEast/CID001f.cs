/*
 * CID001f.cs - tr culture handler.
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

// Generated from "tr.txt".

namespace I18N.MidEast
{

using System;
using System.Globalization;
using I18N.Common;

public class CID001f : RootCulture
{
	public CID001f() : base(0x001F) {}
	public CID001f(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "tr";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "tur";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "TRK";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "tr";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"Paz", "Pzt", "Sal", "\u00C7ar", "Per", "Cum", "Cmt"};
			dfi.DayNames = new String[] {"Pazar", "Pazartesi", "Sal\u0131", "\u00C7ar\u015Famba", "Per\u015Fembe", "Cuma", "Cumartesi"};
			dfi.AbbreviatedMonthNames = new String[] {"Oca", "\u015Eub", "Mar", "Nis", "May", "Haz", "Tem", "A\u011Fu", "Eyl", "Eki", "Kas", "Ara", ""};
			dfi.MonthNames = new String[] {"Ocak", "\u015Eubat", "Mart", "Nisan", "May\u0131s", "Haziran", "Temmuz", "A\u011Fustos", "Eyl\u00FCl", "Ekim", "Kas\u0131m", "Aral\u0131k", ""};
			dfi.DateSeparator = ".";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "dd MMMM yyyy dddd";
			dfi.LongTimePattern = "HH:mm:ss z";
			dfi.ShortDatePattern = "dd.MM.yyyy";
			dfi.ShortTimePattern = "HH:mm";
			dfi.FullDateTimePattern = "dd MMMM yyyy dddd HH:mm:ss z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:dd.MM.yyyy",
				"D:dd MMMM yyyy dddd",
				"f:dd MMMM yyyy dddd HH:mm:ss z",
				"f:dd MMMM yyyy dddd HH:mm:ss z",
				"f:dd MMMM yyyy dddd HH:mm:ss",
				"f:dd MMMM yyyy dddd HH:mm",
				"F:dd MMMM yyyy dddd HH:mm:ss",
				"g:dd.MM.yyyy HH:mm:ss z",
				"g:dd.MM.yyyy HH:mm:ss z",
				"g:dd.MM.yyyy HH:mm:ss",
				"g:dd.MM.yyyy HH:mm",
				"G:dd.MM.yyyy HH:mm:ss",
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
			case "tr": return "T\u00FCrk\u00E7e";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "TR": return "T\u00FCrkiye";
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
				return 1254;
			}
		}
		public override int EBCDICCodePage
		{
			get
			{
				return 20905;
			}
		}
		public override int MacCodePage
		{
			get
			{
				return 10081;
			}
		}
		public override int OEMCodePage
		{
			get
			{
				return 857;
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

}; // class CID001f

public class CNtr : CID001f
{
	public CNtr() : base() {}

}; // class CNtr

}; // namespace I18N.MidEast
