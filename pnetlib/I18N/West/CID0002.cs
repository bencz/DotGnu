/*
 * CID0002.cs - bg culture handler.
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

// Generated from "bg.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0002 : RootCulture
{
	public CID0002() : base(0x0002) {}
	public CID0002(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "bg";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "bul";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "BGR";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "bg";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"\u043D\u0434", "\u043F\u043D", "\u0432\u0442", "\u0441\u0440", "\u0447\u0442", "\u043F\u0442", "\u0441\u0431"};
			dfi.DayNames = new String[] {"\u043D\u0435\u0434\u0435\u043B\u044F", "\u043F\u043E\u043D\u0435\u0434\u0435\u043B\u043D\u0438\u043A", "\u0432\u0442\u043E\u0440\u043D\u0438\u043A", "\u0441\u0440\u044F\u0434\u0430", "\u0447\u0435\u0442\u0432\u044A\u0440\u0442\u044A\u043A", "\u043F\u0435\u0442\u044A\u043A", "\u0441\u044A\u0431\u043E\u0442\u0430"};
			dfi.AbbreviatedMonthNames = new String[] {"\u044f\u043d.", "\u0444\u0435\u0432.", "\u043c\u0430\u0440\u0442.", "\u0430\u043f\u0440.", "\u043c\u0430\u0439.", "\u044e\u043d\u0438.", "\u044e\u043b\u0438.", "\u0430\u0432\u0433.", "\u0441\u0435\u043f.", "\u043e\u043a\u0442.", "\u043d\u043e\u0435\u043c.", "\u0434\u0435\u043a.", ""};
			dfi.MonthNames = new String[] {"\u042F\u043D\u0443\u0430\u0440\u0438", "\u0424\u0435\u0432\u0440\u0443\u0430\u0440\u0438", "\u041C\u0430\u0440\u0442", "\u0410\u043F\u0440\u0438\u043B", "\u041C\u0430\u0439", "\u042E\u043D\u0438", "\u042E\u043B\u0438", "\u0410\u0432\u0433\u0443\u0441\u0442", "\u0421\u0435\u043F\u0442\u0435\u043C\u0432\u0440\u0438", "\u041E\u043A\u0442\u043E\u043C\u0432\u0440\u0438", "\u041D\u043E\u0435\u043C\u0432\u0440\u0438", "\u0414\u0435\u043A\u0435\u043C\u0432\u0440\u0438", ""};
			dfi.DateSeparator = ".";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "dd MMMM yyyy";
			dfi.LongTimePattern = "HH:mm:ss z";
			dfi.ShortDatePattern = "dd.MM.yy";
			dfi.ShortTimePattern = "HH:mm";
			dfi.FullDateTimePattern = "dd MMMM yyyy, dddd HH:mm:ss z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:dd.MM.yy",
				"D:dd MMMM yyyy, dddd",
				"f:dd MMMM yyyy, dddd HH:mm:ss z",
				"f:dd MMMM yyyy, dddd HH:mm:ss z",
				"f:dd MMMM yyyy, dddd HH:mm:ss",
				"f:dd MMMM yyyy, dddd HH:mm",
				"F:dd MMMM yyyy, dddd HH:mm:ss",
				"g:dd.MM.yy HH:mm:ss z",
				"g:dd.MM.yy HH:mm:ss z",
				"g:dd.MM.yy HH:mm:ss",
				"g:dd.MM.yy HH:mm",
				"G:dd.MM.yy HH:mm:ss",
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
			case "bg": return "\u0431\u044A\u043B\u0433\u0430\u0440\u0441\u043A\u0438";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "BG": return "\u0411\u044A\u043B\u0433\u0430\u0440\u0438\u044F";
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
				return 20420;
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

}; // class CID0002

public class CNbg : CID0002
{
	public CNbg() : base() {}

}; // class CNbg

}; // namespace I18N.West
