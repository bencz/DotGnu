/*
 * CID001c.cs - sq culture handler.
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

// Generated from "sq.txt".

namespace I18N.Other
{

using System;
using System.Globalization;
using I18N.Common;

public class CID001c : RootCulture
{
	public CID001c() : base(0x001C) {}
	public CID001c(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "sq";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "sqi";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "SQI";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "sq";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AMDesignator = "PD";
			dfi.PMDesignator = "MD";
			dfi.AbbreviatedDayNames = new String[] {"Die", "H\u00EBn", "Mar", "M\u00EBr", "Enj", "Pre", "Sht"};
			dfi.DayNames = new String[] {"e diel", "e h\u00EBn\u00EB", "e mart\u00EB", "e m\u00EBrkur\u00EB", "e enjte", "e premte", "e shtun\u00EB"};
			dfi.AbbreviatedMonthNames = new String[] {"Jan", "Shk", "Mar", "Pri", "Maj", "Qer", "Kor", "Gsh", "Sht", "Tet", "N\u00EBn", "Dhj", ""};
			dfi.MonthNames = new String[] {"janar", "shkurt", "mars", "prill", "maj", "qershor", "korrik", "gusht", "shtator", "tetor", "n\u00EBntor", "dhjetor", ""};
			dfi.DateSeparator = "-";
			dfi.TimeSeparator = ".";
			dfi.LongDatePattern = "dd MMMM yyyy";
			dfi.LongTimePattern = "h.mm.ss.tt z";
			dfi.ShortDatePattern = "yy-MM-dd";
			dfi.ShortTimePattern = "h.mm.tt";
			dfi.FullDateTimePattern = "dddd, dd MMMM yyyy h.mm.ss.tt z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:yy-MM-dd",
				"D:dddd, dd MMMM yyyy",
				"f:dddd, dd MMMM yyyy h.mm.ss.tt z",
				"f:dddd, dd MMMM yyyy h.mm.ss.tt z",
				"f:dddd, dd MMMM yyyy h:mm:ss.tt",
				"f:dddd, dd MMMM yyyy h.mm.tt",
				"F:dddd, dd MMMM yyyy HH.mm.ss",
				"g:yy-MM-dd h.mm.ss.tt z",
				"g:yy-MM-dd h.mm.ss.tt z",
				"g:yy-MM-dd h:mm:ss.tt",
				"g:yy-MM-dd h.mm.tt",
				"G:yy-MM-dd HH.mm.ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:h.mm.ss.tt z",
				"t:h.mm.ss.tt z",
				"t:h:mm:ss.tt",
				"t:h.mm.tt",
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
			case "sq": return "shqipe";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "AL": return "Shqip\u00EBria";
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

}; // class CID001c

public class CNsq : CID001c
{
	public CNsq() : base() {}

}; // class CNsq

}; // namespace I18N.Other
