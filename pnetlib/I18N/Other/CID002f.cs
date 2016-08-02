/*
 * CID002f.cs - mk culture handler.
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

// Generated from "mk.txt".

namespace I18N.Other
{

using System;
using System.Globalization;
using I18N.Common;

public class CID002f : RootCulture
{
	public CID002f() : base(0x002F) {}
	public CID002f(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "mk";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "mkd";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "MKI";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "mk";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"\u043D\u0435\u0434.", "\u043F\u043E\u043D.", "\u0432\u0442.", "\u0441\u0440\u0435.", "\u0447\u0435\u0442.", "\u043F\u0435\u0442.", "\u0441\u0430\u0431."};
			dfi.DayNames = new String[] {"\u043D\u0435\u0434\u0435\u043B\u0430", "\u043F\u043E\u043D\u0435\u0434\u0435\u043B\u043D\u0438\u043A", "\u0432\u0442\u043E\u0440\u043D\u0438\u043A", "\u0441\u0440\u0435\u0434\u0430", "\u0447\u0435\u0442\u0432\u0440\u0442\u043E\u043A", "\u043F\u0435\u0442\u043E\u043A", "\u0441\u0430\u0431\u043E\u0442\u0430"};
			dfi.AbbreviatedMonthNames = new String[] {"\u0458\u0430\u043D.", "\u0444\u0435\u0432.", "\u043C\u0430\u0440.", "\u0430\u043F\u0440.", "\u043C\u0430\u0458.", "\u0458\u0443\u043D.", "\u0458\u0443\u043B.", "\u0430\u0432\u0433.", "\u0441\u0435\u043F\u0442.", "\u043E\u043A\u0442.", "\u043D\u043E\u0435\u043C.", "\u0434\u0435\u043A\u0435\u043C.", ""};
			dfi.MonthNames = new String[] {"\u0458\u0430\u043D\u0443\u0430\u0440\u0438", "\u0444\u0435\u0432\u0440\u0443\u0430\u0440\u0438", "\u043C\u0430\u0440\u0442", "\u0430\u043F\u0440\u0438\u043B", "\u043C\u0430\u0458", "\u0458\u0443\u043D\u0438", "\u0458\u0443\u043B\u0438", "\u0430\u0432\u0433\u0443\u0441\u0442", "\u0441\u0435\u043F\u0442\u0435\u043C\u0432\u0440\u0438", "\u043E\u043A\u0442\u043E\u043C\u0432\u0440\u0438", "\u043D\u043E\u0435\u043C\u0432\u0440\u0438", "\u0434\u0435\u043A\u0435\u043C\u0432\u0440\u0438", ""};
			dfi.DateSeparator = ".";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "d, MMMM yyyy";
			dfi.LongTimePattern = "HH:mm:ss z";
			dfi.ShortDatePattern = "d.M.yy";
			dfi.ShortTimePattern = "HH:mm";
			dfi.FullDateTimePattern = "dddd, d, MMMM yyyy HH:mm:ss z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:d.M.yy",
				"D:dddd, d, MMMM yyyy",
				"f:dddd, d, MMMM yyyy HH:mm:ss z",
				"f:dddd, d, MMMM yyyy HH:mm:ss z",
				"f:dddd, d, MMMM yyyy HH:mm:ss",
				"f:dddd, d, MMMM yyyy HH:mm",
				"F:dddd, d, MMMM yyyy HH:mm:ss",
				"g:d.M.yy HH:mm:ss z",
				"g:d.M.yy HH:mm:ss z",
				"g:d.M.yy HH:mm:ss",
				"g:d.M.yy HH:mm",
				"G:d.M.yy HH:mm:ss",
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
			case "mk": return "\u043C\u0430\u043A\u0435\u0434\u043E\u043D\u0441\u043A\u0438";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "MK": return "\u041C\u0430\u043A\u0435\u0434\u043E\u043D\u0438\u0458\u0430";
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
				return 1251;
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
				return 10007;
			}
		}
		public override int OEMCodePage
		{
			get
			{
				return 866;
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

}; // class CID002f

public class CNmk : CID002f
{
	public CNmk() : base() {}

}; // class CNmk

}; // namespace I18N.Other
