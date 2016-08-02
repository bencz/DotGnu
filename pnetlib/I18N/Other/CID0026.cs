/*
 * CID0026.cs - lv culture handler.
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

// Generated from "lv.txt".

namespace I18N.Other
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0026 : RootCulture
{
	public CID0026() : base(0x0026) {}
	public CID0026(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "lv";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "lav";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "LVI";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "lv";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"Sv", "P", "O", "T", "C", "Pk", "S"};
			dfi.DayNames = new String[] {"sv\u0113tdiena", "pirmdiena", "otrdiena", "tre\u0161diena", "ceturtdiena", "piektdiena", "sestdiena"};
			dfi.AbbreviatedMonthNames = new String[] {"Jan", "Feb", "Mar", "Apr", "Maijs", "J\u016Bn", "J\u016Bl", "Aug", "Sep", "Okt", "Nov", "Dec", ""};
			dfi.MonthNames = new String[] {"janv\u0101ris", "febru\u0101ris", "marts", "apr\u012Blis", "maijs", "j\u016Bnijs", "j\u016Blijs", "augusts", "septembris", "oktobris", "novembris", "decembris", ""};
			dfi.DateSeparator = ".";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "dddd, yyyy, d MMMM";
			dfi.LongTimePattern = "HH:mm:ss z";
			dfi.ShortDatePattern = "yy.d.M";
			dfi.ShortTimePattern = "HH:mm";
			dfi.FullDateTimePattern = "dddd, yyyy, d MMMM HH:mm:ss z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:yy.d.M",
				"D:dddd, yyyy, d MMMM",
				"f:dddd, yyyy, d MMMM HH:mm:ss z",
				"f:dddd, yyyy, d MMMM HH:mm:ss z",
				"f:dddd, yyyy, d MMMM HH:mm:ss",
				"f:dddd, yyyy, d MMMM HH:mm",
				"F:dddd, yyyy, d MMMM HH:mm:ss",
				"g:yy.d.M HH:mm:ss z",
				"g:yy.d.M HH:mm:ss z",
				"g:yy.d.M HH:mm:ss",
				"g:yy.d.M HH:mm",
				"G:yy.d.M HH:mm:ss",
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
			case "lv": return "Latvie\u0161u";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "LV": return "Latvija";
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
				return 1257;
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
				return 10029;
			}
		}
		public override int OEMCodePage
		{
			get
			{
				return 775;
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

}; // class CID0026

public class CNlv : CID0026
{
	public CNlv() : base() {}

}; // class CNlv

}; // namespace I18N.Other
