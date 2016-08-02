/*
 * CID0003.cs - ca culture handler.
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

// Generated from "ca.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0003 : RootCulture
{
	public CID0003() : base(0x0003) {}
	public CID0003(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "ca";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "cat";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "CAT";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "ca";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"dg.", "dl.", "dt.", "dc.", "dj.", "dv.", "ds."};
			dfi.DayNames = new String[] {"diumenge", "dilluns", "dimarts", "dimecres", "dijous", "divendres", "dissabte"};
			dfi.AbbreviatedMonthNames = new String[] {"gen.", "feb.", "mar\u00E7", "abr.", "maig", "juny", "jul.", "ag.", "set.", "oct.", "nov.", "des.", ""};
			dfi.MonthNames = new String[] {"gener", "febrer", "mar\u00E7", "abril", "maig", "juny", "juliol", "agost", "setembre", "octubre", "novembre", "desembre", ""};
			dfi.DateSeparator = "/";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "d MMMM yyyy";
			dfi.LongTimePattern = "HH:mm:ss z";
			dfi.ShortDatePattern = "dd/MM/yy";
			dfi.ShortTimePattern = "HH:mm";
			dfi.FullDateTimePattern = "dddd dd MMMM yyyy HH:mm:ss z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:dd/MM/yy",
				"D:dddd dd MMMM yyyy",
				"f:dddd dd MMMM yyyy HH:mm:ss z",
				"f:dddd dd MMMM yyyy HH:mm:ss z",
				"f:dddd dd MMMM yyyy HH:mm:ss",
				"f:dddd dd MMMM yyyy HH:mm",
				"F:dddd dd MMMM yyyy HH:mm:ss",
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
			case "ca": return "catal\u00E0";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "ES": return "Espanya";
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

}; // class CID0003

public class CNca : CID0003
{
	public CNca() : base() {}

}; // class CNca

}; // namespace I18N.West
