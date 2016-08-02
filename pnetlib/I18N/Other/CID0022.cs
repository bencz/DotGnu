/*
 * CID0022.cs - uk culture handler.
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

// Generated from "uk.txt".

namespace I18N.Other
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0022 : RootCulture
{
	public CID0022() : base(0x0022) {}
	public CID0022(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "uk";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "ukr";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "UKR";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "uk";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"\u041D\u0434", "\u041F\u043D", "\u0412\u0442", "\u0421\u0440", "\u0427\u0442", "\u041F\u0442", "\u0421\u0431"};
			dfi.DayNames = new String[] {"\u041D\u0435\u0434\u0456\u043B\u044F", "\u041F\u043E\u043D\u0435\u0434\u0456\u043B\u043E\u043A", "\u0412\u0456\u0432\u0442\u043E\u0440\u043E\u043A", "\u0421\u0435\u0440\u0435\u0434\u0430", "\u0427\u0435\u0442\u0432\u0435\u0440", "\u041F\u044F\u0442\u043D\u0438\u0446\u044F", "\u0421\u0443\u0431\u043E\u0442\u0430"};
			dfi.AbbreviatedMonthNames = new String[] {"\u0421\u0456\u0447", "\u041B\u044E\u0442", "\u0411\u0435\u0440", "\u041A\u0432\u0456\u0442", "\u0422\u0440\u0430\u0432", "\u0427\u0435\u0440\u0432", "\u041B\u0438\u043F", "\u0421\u0435\u0440\u043F", "\u0412\u0435\u0440", "\u0416\u043E\u0432\u0442", "\u041B\u0438\u0441\u0442", "\u0413\u0440\u0443\u0434", ""};
			dfi.MonthNames = new String[] {"\u0421\u0456\u0447\u043D\u044F", "\u041B\u044E\u0442\u043E\u0433\u043E", "\u0411\u0435\u0440\u0435\u0436\u043D\u044F", "\u041A\u0432\u0456\u0442\u043D\u044F", "\u0422\u0440\u0430\u0432\u043D\u044F", "\u0427\u0435\u0440\u0432\u043D\u044F", "\u041B\u0438\u043F\u043D\u044F", "\u0421\u0435\u0440\u043F\u043D\u044F", "\u0412\u0435\u0440\u0435\u0441\u043D\u044F", "\u0416\u043E\u0432\u0442\u043D\u044F", "\u041B\u0438\u0441\u0442\u043E\u043F\u0430\u0434\u0430", "\u0413\u0440\u0443\u0434\u043D\u044F", ""};
			dfi.DateSeparator = "/";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "dddd, d, MMMM yyyy";
			dfi.LongTimePattern = "HH:mm:ss z";
			dfi.ShortDatePattern = "d/M/yy";
			dfi.ShortTimePattern = "HH:mm";
			dfi.FullDateTimePattern = "dddd, d, MMMM yyyy HH:mm:ss z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:d/M/yy",
				"D:dddd, d, MMMM yyyy",
				"f:dddd, d, MMMM yyyy HH:mm:ss z",
				"f:dddd, d, MMMM yyyy HH:mm:ss z",
				"f:dddd, d, MMMM yyyy HH:mm:ss",
				"f:dddd, d, MMMM yyyy HH:mm",
				"F:dddd, d, MMMM yyyy HH:mm:ss",
				"g:d/M/yy HH:mm:ss z",
				"g:d/M/yy HH:mm:ss z",
				"g:d/M/yy HH:mm:ss",
				"g:d/M/yy HH:mm",
				"G:d/M/yy HH:mm:ss",
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
			case "uk": return "\u0443\u043A\u0440\u0430\u0457\u043D\u0441\u044C\u043A\u0430";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "UA": return "\u0423\u043A\u0440\u0430\u0457\u043D\u0430";
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
				return 10017;
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

}; // class CID0022

public class CNuk : CID0022
{
	public CNuk() : base() {}

}; // class CNuk

}; // namespace I18N.Other
