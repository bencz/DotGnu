/*
 * CID0023.cs - be culture handler.
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

// Generated from "be.txt".

namespace I18N.Other
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0023 : RootCulture
{
	public CID0023() : base(0x0023) {}
	public CID0023(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "be";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "bel";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "BEL";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "be";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"\u043d\u0434", "\u043F\u043D", "\u0430\u045e", "\u0441\u0440", "\u0447\u0446", "\u043F\u0442", "\u0441\u0431"};
			dfi.DayNames = new String[] {"\u043D\u044F\u0434\u0437\u0435\u043B\u044F", "\u043F\u0430\u043D\u044F\u0434\u0437\u0435\u043B\u0430\u043A", "\u0430\u045E\u0442\u043E\u0440\u0430\u043A", "\u0441\u0435\u0440\u0430\u0434\u0430", "\u0447\u0430\u0446\u0432\u0435\u0440", "\u043F\u044F\u0442\u043D\u0456\u0446\u0430", "\u0441\u0443\u0431\u043E\u0442\u0430"};
			dfi.AbbreviatedMonthNames = new String[] {"\u0421\u0442\u0443", "\u041b\u044e\u0442", "\u0421\u0430\u043a", "\u041a\u0440\u0430", "\u041c\u0430\u0439", "\u0427\u044d\u0440", "\u041b\u0456\u043f", "\u0416\u043d\u0456", "\u0412\u0435\u0440", "\u041a\u0430\u0441", "\u041b\u0456\u0441", "\u0421\u043d\u0435", ""};
			dfi.MonthNames = new String[] {"\u0421\u0442\u0443\u0434\u0437\u0435\u043d\u044c", "\u041b\u044e\u0442\u044b", "\u0421\u0430\u043a\u0430\u0432\u0456\u043a", "\u041a\u0440\u0430\u0441\u0430\u0432\u0456\u043a", "\u041c\u0430\u0439", "\u0427\u044d\u0440\u0432\u0435\u043d\u044c", "\u041b\u0456\u043f\u0435\u043d\u044c", "\u0416\u043d\u0456\u0432\u0435\u043d\u044c", "\u0412\u0435\u0440\u0430\u0441\u0435\u043d\u044c", "\u041a\u0430\u0441\u0442\u0440\u044b\u0447\u043d\u0456\u043a", "\u041b\u0456\u0441\u0442\u0430\u043f\u0430\u0434", "\u0421\u043d\u0435\u0436\u0430\u043d\u044c", ""};
			dfi.DateSeparator = ".";
			dfi.TimeSeparator = ".";
			dfi.LongDatePattern = "d MMMM yyyy";
			dfi.LongTimePattern = "HH.mm.ss z";
			dfi.ShortDatePattern = "d.M.yy";
			dfi.ShortTimePattern = "HH.mm";
			dfi.FullDateTimePattern = "dddd, d MMMM yyyy HH.mm.ss z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:d.M.yy",
				"D:dddd, d MMMM yyyy",
				"f:dddd, d MMMM yyyy HH.mm.ss z",
				"f:dddd, d MMMM yyyy HH.mm.ss z",
				"f:dddd, d MMMM yyyy HH.mm.ss",
				"f:dddd, d MMMM yyyy HH.mm",
				"F:dddd, d MMMM yyyy HH.mm.ss",
				"g:d.M.yy HH.mm.ss z",
				"g:d.M.yy HH.mm.ss z",
				"g:d.M.yy HH.mm.ss",
				"g:d.M.yy HH.mm",
				"G:d.M.yy HH.mm.ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:HH.mm.ss z",
				"t:HH.mm.ss z",
				"t:HH.mm.ss",
				"t:HH.mm",
				"T:HH.mm.ss",
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
			case "be": return "\u0411\u0435\u043B\u0430\u0440\u0443\u0441\u043A\u0456";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "BY": return "\u0411\u0435\u043B\u0430\u0440\u0443\u0441\u044C";
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

}; // class CID0023

public class CNbe : CID0023
{
	public CNbe() : base() {}

}; // class CNbe

}; // namespace I18N.Other
