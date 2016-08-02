/*
 * CID0018.cs - ro culture handler.
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

// Generated from "ro.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0018 : RootCulture
{
	public CID0018() : base(0x0018) {}
	public CID0018(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "ro";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "ron";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "ROM";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "ro";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"D", "L", "Ma", "Mi", "J", "V", "S"};
			dfi.DayNames = new String[] {"duminic\u0103", "luni", "mar\u0163i", "miercuri", "joi", "vineri", "s\u00EEmb\u0103t\u0103"};
			dfi.AbbreviatedMonthNames = new String[] {"Ian", "Feb", "Mar", "Apr", "Mai", "Iun", "Iul", "Aug", "Sep", "Oct", "Nov", "Dec", ""};
			dfi.MonthNames = new String[] {"ianuarie", "februarie", "martie", "aprilie", "mai", "iunie", "iulie", "august", "septembrie", "octombrie", "noiembrie", "decembrie", ""};
			dfi.DateSeparator = ".";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "d MMMM yyyy";
			dfi.LongTimePattern = "HH:mm:ss z";
			dfi.ShortDatePattern = "dd.MM.yyyy";
			dfi.ShortTimePattern = "HH:mm";
			dfi.FullDateTimePattern = "d MMMM yyyy HH:mm:ss z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:dd.MM.yyyy",
				"D:d MMMM yyyy",
				"f:d MMMM yyyy HH:mm:ss z",
				"f:d MMMM yyyy HH:mm:ss z",
				"f:d MMMM yyyy HH:mm:ss",
				"f:d MMMM yyyy HH:mm",
				"F:d MMMM yyyy HH:mm:ss",
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
			case "ro": return "rom\u00E2n\u0103";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "RO": return "Rom\u00E2nia";
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

}; // class CID0018

public class CNro : CID0018
{
	public CNro() : base() {}

}; // class CNro

}; // namespace I18N.West
