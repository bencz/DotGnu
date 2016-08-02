/*
 * CID0008.cs - el culture handler.
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

// Generated from "el.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0008 : RootCulture
{
	public CID0008() : base(0x0008) {}
	public CID0008(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "el";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "ell";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "ELL";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "el";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AMDesignator = "\u03C0\u03BC";
			dfi.PMDesignator = "\u03BC\u03BC";
			dfi.AbbreviatedDayNames = new String[] {"\u039A\u03C5\u03C1", "\u0394\u03B5\u03C5", "\u03A4\u03C1\u03B9", "\u03A4\u03B5\u03C4", "\u03A0\u03B5\u03BC", "\u03A0\u03B1\u03C1", "\u03A3\u03B1\u03B2"};
			dfi.DayNames = new String[] {"\u039A\u03C5\u03C1\u03B9\u03B1\u03BA\u03AE", "\u0394\u03B5\u03C5\u03C4\u03AD\u03C1\u03B1", "\u03A4\u03C1\u03AF\u03C4\u03B7", "\u03A4\u03B5\u03C4\u03AC\u03C1\u03C4\u03B7", "\u03A0\u03AD\u03BC\u03C0\u03C4\u03B7", "\u03A0\u03B1\u03C1\u03B1\u03C3\u03BA\u03B5\u03C5\u03AE", "\u03A3\u03AC\u03B2\u03B2\u03B1\u03C4\u03BF"};
			dfi.AbbreviatedMonthNames = new String[] {"\u0399\u03B1\u03BD", "\u03A6\u03B5\u03B2", "\u039C\u03B1\u03C1", "\u0391\u03C0\u03C1", "\u039C\u03B1\u03CA", "\u0399\u03BF\u03C5\u03BD", "\u0399\u03BF\u03C5\u03BB", "\u0391\u03C5\u03B3", "\u03A3\u03B5\u03C0", "\u039F\u03BA\u03C4", "\u039D\u03BF\u03B5", "\u0394\u03B5\u03BA", ""};
			dfi.MonthNames = new String[] {"\u0399\u03B1\u03BD\u03BF\u03C5\u03AC\u03C1\u03B9\u03BF\u03C2", "\u03A6\u03B5\u03B2\u03C1\u03BF\u03C5\u03AC\u03C1\u03B9\u03BF\u03C2", "\u039C\u03AC\u03C1\u03C4\u03B9\u03BF\u03C2", "\u0391\u03C0\u03C1\u03AF\u03BB\u03B9\u03BF\u03C2", "\u039C\u03AC\u03CA\u03BF\u03C2", "\u0399\u03BF\u03CD\u03BD\u03B9\u03BF\u03C2", "\u0399\u03BF\u03CD\u03BB\u03B9\u03BF\u03C2", "\u0391\u03CD\u03B3\u03BF\u03C5\u03C3\u03C4\u03BF\u03C2", "\u03A3\u03B5\u03C0\u03C4\u03AD\u03BC\u03B2\u03C1\u03B9\u03BF\u03C2", "\u039F\u03BA\u03C4\u03CE\u03B2\u03C1\u03B9\u03BF\u03C2", "\u039D\u03BF\u03AD\u03BC\u03B2\u03C1\u03B9\u03BF\u03C2", "\u0394\u03B5\u03BA\u03AD\u03BC\u03B2\u03C1\u03B9\u03BF\u03C2", ""};
			dfi.DateSeparator = "/";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "d MMMM yyyy";
			dfi.LongTimePattern = "h:mm:ss tt z";
			dfi.ShortDatePattern = "d/M/yyyy";
			dfi.ShortTimePattern = "h:mm tt";
			dfi.FullDateTimePattern = "dddd, d MMMM yyyy h:mm:ss tt z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:d/M/yyyy",
				"D:dddd, d MMMM yyyy",
				"f:dddd, d MMMM yyyy h:mm:ss tt z",
				"f:dddd, d MMMM yyyy h:mm:ss tt z",
				"f:dddd, d MMMM yyyy h:mm:ss tt",
				"f:dddd, d MMMM yyyy h:mm tt",
				"F:dddd, d MMMM yyyy HH:mm:ss",
				"g:d/M/yyyy h:mm:ss tt z",
				"g:d/M/yyyy h:mm:ss tt z",
				"g:d/M/yyyy h:mm:ss tt",
				"g:d/M/yyyy h:mm tt",
				"G:d/M/yyyy HH:mm:ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:h:mm:ss tt z",
				"t:h:mm:ss tt z",
				"t:h:mm:ss tt",
				"t:h:mm tt",
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
			case "ar": return "\u0391\u03c1\u03b1\u03b2\u03b9\u03ba\u03ac";
			case "bg": return "\u0392\u03bf\u03c5\u03bb\u03b3\u03b1\u03c1\u03b9\u03ba\u03ac";
			case "ca": return "\u039a\u03b1\u03c4\u03b1\u03bb\u03b1\u03bd\u03b9\u03ba\u03ac";
			case "cs": return "\u03a4\u03c3\u03ad\u03c7\u03b9\u03ba\u03b1";
			case "da": return "\u0394\u03b1\u03bd\u03ad\u03b6\u03b9\u03ba\u03b1";
			case "de": return "\u0393\u03b5\u03c1\u03bc\u03b1\u03bd\u03b9\u03ba\u03ac";
			case "el": return "\u03b5\u03bb\u03bb\u03b7\u03bd\u03b9\u03ba\u03ac";
			case "en": return "\u0391\u03b3\u03b3\u03bb\u03b9\u03ba\u03ac";
			case "es": return "\u0399\u03c3\u03c0\u03b1\u03bd\u03b9\u03ba\u03ac";
			case "fi": return "\u03a6\u03b9\u03bd\u03bb\u03b1\u03bd\u03b4\u03b9\u03ba\u03ac";
			case "fr": return "\u0393\u03b1\u03bb\u03bb\u03b9\u03ba\u03ac";
			case "he": return "\u0395\u03b2\u03c1\u03b1\u03ca\u03ba\u03ac";
			case "hr": return "\u039a\u03c1\u03bf\u03b1\u03c4\u03b9\u03ba\u03ac";
			case "hu": return "\u039f\u03c5\u03b3\u03b3\u03c1\u03b9\u03ba\u03ac";
			case "it": return "\u0399\u03c4\u03b1\u03bb\u03b9\u03ba\u03ac";
			case "mk": return "\u03a3\u03bb\u03b1\u03b2\u03bf\u03bc\u03b1\u03ba\u03b5\u03b4\u03bf\u03bd\u03b9\u03ba\u03ac";
			case "nl": return "\u039f\u03bb\u03bb\u03b1\u03bd\u03b4\u03b9\u03ba\u03ac";
			case "no": return "\u039d\u03bf\u03c1\u03b2\u03b7\u03b3\u03b9\u03ba\u03ac";
			case "pl": return "\u03a0\u03bf\u03bb\u03c9\u03bd\u03b9\u03ba\u03ac";
			case "pt": return "\u03a0\u03bf\u03c1\u03c4\u03bf\u03b3\u03b1\u03bb\u03b9\u03ba\u03ac";
			case "ro": return "\u03a1\u03bf\u03c5\u03bc\u03b1\u03bd\u03b9\u03ba\u03ac";
			case "ru": return "\u03a1\u03c9\u03c3\u03b9\u03ba\u03ac";
			case "sk": return "\u03a3\u03bb\u03bf\u03b2\u03b1\u03ba\u03b9\u03ba\u03ac";
			case "sl": return "\u03a3\u03bb\u03bf\u03b2\u03b5\u03bd\u03b9\u03ba\u03ac";
			case "sq": return "\u0391\u03bb\u03b2\u03b1\u03bd\u03b9\u03ba\u03ac";
			case "sr": return "\u03a3\u03b5\u03c1\u03b2\u03b9\u03ba\u03ac";
			case "sv": return "\u03a3\u03bf\u03c5\u03b7\u03b4\u03b9\u03ba\u03ac";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "AL": return "\u0391\u03bb\u03b2\u03b1\u03bd\u03af\u03b1";
			case "AS": return "\u0391\u03c3\u03af\u03b1 (\u0391\u03b3\u03b3\u03bb\u03b9\u03ba\u03ac)";
			case "AT": return "\u0391\u03c5\u03c3\u03c4\u03c1\u03af\u03b1";
			case "AU": return "\u0391\u03c5\u03c3\u03c4\u03c1\u03b1\u03bb\u03af\u03b1 (\u0391\u03b3\u03b3\u03bb\u03b9\u03ba\u03ac)";
			case "BA": return "\u0392\u03bf\u03c3\u03bd\u03af\u03b1";
			case "BE": return "\u0392\u03ad\u03bb\u03b3\u03b9\u03bf";
			case "BG": return "\u0392\u03bf\u03c5\u03bb\u03b3\u03b1\u03c1\u03af\u03b1";
			case "BR": return "\u0392\u03c1\u03b1\u03b6\u03b9\u03bb\u03af\u03b1";
			case "CA": return "\u039a\u03b1\u03bd\u03b1\u03b4\u03ac\u03c2";
			case "CH": return "\u0395\u03bb\u03b2\u03b5\u03c4\u03af\u03b1";
			case "CN": return "\u039a\u03af\u03bd\u03b1 (\u039b.\u0394.\u039a.)";
			case "CZ": return "\u03a4\u03c3\u03b5\u03c7\u03af\u03b1";
			case "DE": return "\u0393\u03b5\u03c1\u03bc\u03b1\u03bd\u03af\u03b1";
			case "DK": return "\u0394\u03b1\u03bd\u03af\u03b1";
			case "EE": return "\u0395\u03c3\u03b8\u03bf\u03bd\u03af\u03b1";
			case "ES": return "\u0399\u03c3\u03c0\u03b1\u03bd\u03af\u03b1";
			case "FI": return "\u03a6\u03b9\u03bd\u03bb\u03b1\u03bd\u03b4\u03af\u03b1";
			case "FR": return "\u0393\u03b1\u03bb\u03bb\u03af\u03b1";
			case "GB": return "\u0397\u03bd\u03c9\u03bc\u03ad\u03bd\u03bf \u0392\u03b1\u03c3\u03af\u03bb\u03b5\u03b9\u03bf";
			case "GR": return "\u0395\u03bb\u03bb\u03ac\u03b4\u03b1";
			case "HR": return "\u039a\u03c1\u03bf\u03b1\u03c4\u03af\u03b1";
			case "HU": return "\u039f\u03c5\u03b3\u03b3\u03b1\u03c1\u03af\u03b1";
			case "IE": return "\u0399\u03c1\u03bb\u03b1\u03bd\u03b4\u03af\u03b1";
			case "IL": return "\u0399\u03c3\u03c1\u03b1\u03ae\u03bb";
			case "IS": return "\u0399\u03c3\u03bb\u03b1\u03bd\u03b4\u03af\u03b1";
			case "IT": return "\u0399\u03c4\u03b1\u03bb\u03af\u03b1";
			case "JP": return "\u0399\u03b1\u03c0\u03c9\u03bd\u03af\u03b1";
			case "KR": return "\u039a\u03bf\u03c1\u03ad\u03b1";
			case "LA": return "\u039b\u03b1\u03c4\u03b9\u03bd\u03b9\u03ba\u03ae \u0391\u03bc\u03b5\u03c1\u03b9\u03ba\u03ae";
			case "LT": return "\u039b\u03b9\u03b8\u03bf\u03c5\u03b1\u03bd\u03af\u03b1";
			case "LV": return "\u039b\u03b5\u03c4\u03bf\u03bd\u03af\u03b1";
			case "MK": return "\u03a0\u0393\u0394 \u039c\u03b1\u03ba\u03b5\u03b4\u03bf\u03bd\u03af\u03b1\u03c2";
			case "NL": return "\u039f\u03bb\u03bb\u03b1\u03bd\u03b4\u03af\u03b1";
			case "NO": return "\u039d\u03bf\u03c1\u03b2\u03b7\u03b3\u03af\u03b1";
			case "NZ": return "\u039d\u03ad\u03b1 \u0396\u03b7\u03bb\u03b1\u03bd\u03b4\u03af\u03b1";
			case "PL": return "\u03a0\u03bf\u03bb\u03c9\u03bd\u03af\u03b1";
			case "PT": return "\u03a0\u03bf\u03c1\u03c4\u03bf\u03b3\u03b1\u03bb\u03af\u03b1";
			case "RO": return "\u03a1\u03bf\u03c5\u03bc\u03b1\u03bd\u03af\u03b1";
			case "RU": return "\u03a1\u03c9\u03c3\u03af\u03b1";
			case "SE": return "\u03a3\u03bf\u03c5\u03b7\u03b4\u03af\u03b1";
			case "SI": return "\u03a3\u03bb\u03bf\u03b2\u03b5\u03bd\u03af\u03b1";
			case "SK": return "\u03a3\u03bb\u03bf\u03b2\u03b1\u03ba\u03af\u03b1";
			case "SP": return "\u03a3\u03b5\u03c1\u03b2\u03af\u03b1";
			case "TH": return "\u03a4\u03b1\u03ca\u03bb\u03ac\u03bd\u03b4\u03b7";
			case "TR": return "\u03a4\u03bf\u03c5\u03c1\u03ba\u03af\u03b1";
			case "TW": return "\u03a4\u03b1\u03ca\u03b2\u03ac\u03bd (\u0394.\u039a.)";
			case "US": return "\u0397\u03bd\u03c9\u03bc\u03ad\u03bd\u03b5\u03c2 \u03a0\u03bf\u03bb\u03b9\u03c4\u03b5\u03af\u03b5\u03c2 \u0391\u03bc\u03b5\u03c1\u03b9\u03ba\u03ae\u03c2";
			case "ZA": return "\u039d\u03cc\u03c4\u03b9\u03bf\u03c2 \u0391\u03c6\u03c1\u03b9\u03ba\u03ae";
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
				return 1253;
			}
		}
		public override int EBCDICCodePage
		{
			get
			{
				return 20273;
			}
		}
		public override int MacCodePage
		{
			get
			{
				return 10006;
			}
		}
		public override int OEMCodePage
		{
			get
			{
				return 737;
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

}; // class CID0008

public class CNel : CID0008
{
	public CNel() : base() {}

}; // class CNel

}; // namespace I18N.West
