/*
 * CID001e.cs - th culture handler.
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

// Generated from "th.txt".

namespace I18N.Other
{

using System;
using System.Globalization;
using I18N.Common;

public class CID001e : RootCulture
{
	public CID001e() : base(0x001E) {}
	public CID001e(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "th";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "tha";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "THA";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "th";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AMDesignator = "\u0E01\u0E48\u0E2D\u0E19\u0E40\u0E17\u0E35\u0E48\u0E22\u0E07";
			dfi.PMDesignator = "\u0E2B\u0E25\u0E31\u0E07\u0E40\u0E17\u0E35\u0E48\u0E22\u0E07";
			dfi.AbbreviatedDayNames = new String[] {"\u0E2D\u0E32.", "\u0E08.", "\u0E2D.", "\u0E1E.", "\u0E1E\u0E24.", "\u0E28.", "\u0E2A."};
			dfi.DayNames = new String[] {"\u0E27\u0E31\u0E19\u0E2D\u0E32\u0E17\u0E34\u0E15\u0E22\u0E4C", "\u0E27\u0E31\u0E19\u0E08\u0E31\u0E19\u0E17\u0E23\u0E4C", "\u0E27\u0E31\u0E19\u0E2D\u0E31\u0E07\u0E04\u0E32\u0E23", "\u0E27\u0E31\u0E19\u0E1E\u0E38\u0E18", "\u0E27\u0E31\u0E19\u0E1E\u0E24\u0E2B\u0E31\u0E2A\u0E1A\u0E14\u0E35", "\u0E27\u0E31\u0E19\u0E28\u0E38\u0E01\u0E23\u0E4C", "\u0E27\u0E31\u0E19\u0E40\u0E2A\u0E32\u0E23\u0E4C"};
			dfi.AbbreviatedMonthNames = new String[] {"\u0E21.\u0E04.", "\u0E01.\u0E1E.", "\u0E21\u0E35.\u0E04.", "\u0E40\u0E21.\u0E22.", "\u0E1E.\u0E04.", "\u0E21\u0E34.\u0E22.", "\u0E01.\u0E04.", "\u0E2A.\u0E04.", "\u0E01.\u0E22.", "\u0E15.\u0E04.", "\u0E1E.\u0E22.", "\u0E18.\u0E04.", ""};
			dfi.MonthNames = new String[] {"\u0E21\u0E01\u0E23\u0E32\u0E04\u0E21", "\u0E01\u0E38\u0E21\u0E20\u0E32\u0E1E\u0E31\u0E19\u0E18\u0E4C", "\u0E21\u0E35\u0E19\u0E32\u0E04\u0E21", "\u0E40\u0E21\u0E29\u0E32\u0E22\u0E19", "\u0E1E\u0E24\u0E29\u0E20\u0E32\u0E04\u0E21", "\u0E21\u0E34\u0E16\u0E38\u0E19\u0E32\u0E22\u0E19", "\u0E01\u0E23\u0E01\u0E0E\u0E32\u0E04\u0E21", "\u0E2A\u0E34\u0E07\u0E2B\u0E32\u0E04\u0E21", "\u0E01\u0E31\u0E19\u0E22\u0E32\u0E22\u0E19", "\u0E15\u0E38\u0E25\u0E32\u0E04\u0E21", "\u0E1E\u0E24\u0E28\u0E08\u0E34\u0E01\u0E32\u0E22\u0E19", "\u0E18\u0E31\u0E19\u0E27\u0E32\u0E04\u0E21", ""};
			dfi.DateSeparator = "/";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "dddd'\u0d17\u0d35\u0d48 'd MMMM G yyyy";
			dfi.LongTimePattern = "\u0d34\u0d19\u0d32\u0d17\u0d35'";
			dfi.ShortDatePattern = "d MMM yyyy";
			dfi.ShortTimePattern = "H:mm:ss";
			dfi.FullDateTimePattern = "H:mm H' \u0d19\u0d32\u0d2C\u0d34\u0d01\u0d32 'm' \u0d19\u0d32\u0d17\u0d35 'ss' \u0d27";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:d MMM yyyy",
				"D:H:mm",
				"f:H:mm H' \u0d19\u0d32\u0d2C\u0d34\u0d01\u0d32 'm' \u0d19\u0d32\u0d17\u0d35 'ss' \u0d27",
				"f:H:mm \u0d34\u0d19\u0d32\u0d17\u0d35'",
				"f:H:mm H' \u0d19\u0d32\u0d2C\u0d34\u0d01\u0d32 'm' \u0d19\u0d32\u0d17\u0d35'",
				"f:H:mm H:mm:ss",
				"F:H:mm HH:mm:ss",
				"g:d MMM yyyy H' \u0d19\u0d32\u0d2C\u0d34\u0d01\u0d32 'm' \u0d19\u0d32\u0d17\u0d35 'ss' \u0d27",
				"g:d MMM yyyy \u0d34\u0d19\u0d32\u0d17\u0d35'",
				"g:d MMM yyyy H' \u0d19\u0d32\u0d2C\u0d34\u0d01\u0d32 'm' \u0d19\u0d32\u0d17\u0d35'",
				"g:d MMM yyyy H:mm:ss",
				"G:d MMM yyyy HH:mm:ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:H' \u0d19\u0d32\u0d2C\u0d34\u0d01\u0d32 'm' \u0d19\u0d32\u0d17\u0d35 'ss' \u0d27",
				"t:\u0d34\u0d19\u0d32\u0d17\u0d35'",
				"t:H' \u0d19\u0d32\u0d2C\u0d34\u0d01\u0d32 'm' \u0d19\u0d32\u0d17\u0d35'",
				"t:H:mm:ss",
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

	public override String ResolveLanguage(String name)
	{
		switch(name)
		{
			case "ab": return "\u0e41\u0e2d\u0e1a\u0e01\u0e32\u0e40\u0e0b\u0e35\u0e22";
			case "aa": return "\u0e2d\u0e32\u0e1f\u0e32";
			case "af": return "\u0e41\u0e2d\u0e1f\u0e23\u0e34\u0e01\u0e31\u0e19";
			case "sq": return "\u0e41\u0e2d\u0e25\u0e40\u0e1a\u0e40\u0e19\u0e35\u0e22";
			case "am": return "\u0e2d\u0e31\u0e21\u0e2e\u0e32\u0e23\u0e34\u0e04";
			case "ar": return "\u0e2d\u0e32\u0e23\u0e30\u0e1a\u0e34\u0e04";
			case "hy": return "\u0e2d\u0e32\u0e23\u0e4c\u0e21\u0e35\u0e40\u0e19\u0e35\u0e22";
			case "as": return "\u0e2d\u0e31\u0e2a\u0e2a\u0e31\u0e21\u0e21\u0e34\u0e2a";
			case "ay": return "\u0e44\u0e2d\u0e21\u0e32\u0e23\u0e32";
			case "az": return "\u0e2d\u0e32\u0e40\u0e0b\u0e2d\u0e23\u0e4c\u0e44\u0e1a\u0e08\u0e32\u0e19\u0e35";
			case "ba": return "\u0e1a\u0e32\u0e2a\u0e0a\u0e4c\u0e01\u0e35\u0e23\u0e4c";
			case "eu": return "\u0e41\u0e1a\u0e2a\u0e01\u0e4c";
			case "bn": return "\u0e40\u0e1a\u0e19\u0e01\u0e32\u0e23\u0e35";
			case "dz": return "\u0e20\u0e39\u0e10\u0e32\u0e19\u0e35";
			case "bh": return "\u0e1a\u0e34\u0e2e\u0e32\u0e23\u0e35";
			case "bi": return "\u0e1a\u0e34\u0e2a\u0e25\u0e32\u0e21\u0e32";
			case "br": return "\u0e1a\u0e23\u0e35\u0e17\u0e31\u0e19";
			case "bg": return "\u0e1a\u0e31\u0e25\u0e41\u0e01\u0e40\u0e23\u0e35\u0e22";
			case "my": return "\u0e1e\u0e21\u0e48\u0e32";
			case "be": return "\u0e1a\u0e32\u0e22\u0e42\u0e25\u0e23\u0e31\u0e2a\u0e40\u0e0b\u0e35\u0e22";
			case "km": return "\u0e40\u0e02\u0e21\u0e23";
			case "ca": return "\u0e41\u0e04\u0e15\u0e32\u0e41\u0e25\u0e19";
			case "zh": return "\u0e08\u0e35\u0e19";
			case "co": return "\u0e04\u0e2d\u0e23\u0e4c\u0e0b\u0e34\u0e01\u0e32";
			case "hr": return "\u0e42\u0e04\u0e23\u0e40\u0e2d\u0e40\u0e17\u0e35\u0e22";
			case "cs": return "\u0e40\u0e0a\u0e47\u0e04";
			case "da": return "\u0e40\u0e14\u0e19\u0e21\u0e32\u0e23\u0e4c\u0e01";
			case "nl": return "\u0e2e\u0e2d\u0e25\u0e31\u0e19\u0e14\u0e32";
			case "en": return "\u0e2d\u0e31\u0e07\u0e01\u0e24\u0e29";
			case "eo": return "\u0e40\u0e2d\u0e2a\u0e40\u0e1b\u0e2d\u0e23\u0e31\u0e19\u0e42\u0e15";
			case "et": return "\u0e40\u0e2d\u0e2a\u0e42\u0e15\u0e40\u0e19\u0e35\u0e22";
			case "fo": return "\u0e1f\u0e32\u0e42\u0e23\u0e2a";
			case "fj": return "\u0e1f\u0e34\u0e08\u0e34";
			case "fi": return "\u0e1f\u0e34\u0e19";
			case "fr": return "\u0e1d\u0e23\u0e31\u0e48\u0e07\u0e40\u0e28\u0e2a";
			case "fy": return "\u0e1f\u0e23\u0e35\u0e2a\u0e41\u0e25\u0e19\u0e14\u0e4c";
			case "gl": return "\u0e01\u0e30\u0e25\u0e35\u0e40\u0e0a\u0e35\u0e22";
			case "ka": return "\u0e08\u0e2d\u0e23\u0e4c\u0e40\u0e08\u0e35\u0e22\u0e19";
			case "de": return "\u0e40\u0e22\u0e2d\u0e23\u0e21\u0e31\u0e19";
			case "el": return "\u0e01\u0e23\u0e35\u0e01";
			case "kl": return "\u0e01\u0e23\u0e35\u0e19\u0e41\u0e25\u0e19\u0e14\u0e4c\u0e14\u0e34\u0e04";
			case "gn": return "\u0e01\u0e31\u0e27\u0e23\u0e32\u0e19\u0e35";
			case "gu": return "\u0e01\u0e39\u0e08\u0e32\u0e23\u0e32\u0e15\u0e34";
			case "ha": return "\u0e42\u0e2e\u0e0b\u0e32";
			case "he": return "\u0e22\u0e34\u0e27";
			case "hi": return "\u0e2e\u0e35\u0e19\u0e14\u0e34";
			case "hu": return "\u0e2e\u0e31\u0e07\u0e01\u0e32\u0e23\u0e35";
			case "is": return "\u0e44\u0e2d\u0e0b\u0e4c\u0e41\u0e25\u0e19\u0e14\u0e4c\u0e14\u0e34\u0e04";
			case "id": return "\u0e2d\u0e34\u0e19\u0e42\u0e14\u0e19\u0e35\u0e40\u0e0a\u0e35\u0e22";
			case "ia": return "\u0e2d\u0e34\u0e19\u0e40\u0e15\u0e2d\u0e23\u0e4c\u0e25\u0e34\u0e07\u0e01\u0e27\u0e32";
			case "ie": return "\u0e2d\u0e34\u0e19\u0e40\u0e15\u0e2d\u0e23\u0e4c\u0e25\u0e34\u0e07\u0e04\u0e4c";
			case "iu": return "\u0e44\u0e2d\u0e19\u0e38\u0e01\u0e15\u0e34\u0e15\u0e31\u0e17";
			case "ik": return "\u0e44\u0e2d\u0e19\u0e39\u0e40\u0e1b\u0e35\u0e22\u0e01";
			case "ga": return "\u0e44\u0e2d\u0e23\u0e34\u0e0a";
			case "it": return "\u0e2d\u0e34\u0e15\u0e32\u0e25\u0e35";
			case "ja": return "\u0e0d\u0e35\u0e48\u0e1b\u0e38\u0e48\u0e19";
			case "jv": return "\u0e0a\u0e27\u0e32";
			case "kn": return "\u0e01\u0e32\u0e19\u0e32\u0e14\u0e32";
			case "ks": return "\u0e04\u0e31\u0e0a\u0e21\u0e35\u0e23\u0e35";
			case "kk": return "\u0e04\u0e32\u0e0b\u0e31\u0e04";
			case "rw": return "\u0e04\u0e34\u0e19\u0e22\u0e32\u0e27\u0e31\u0e19\u0e14\u0e32";
			case "ky": return "\u0e40\u0e04\u0e2d\u0e23\u0e4c\u0e01\u0e34\u0e0b";
			case "rn": return "\u0e04\u0e34\u0e23\u0e31\u0e19\u0e14\u0e35";
			case "ko": return "\u0e40\u0e01\u0e32\u0e2b\u0e25\u0e35";
			case "ku": return "\u0e40\u0e04\u0e34\u0e14";
			case "lo": return "\u0e25\u0e32\u0e27";
			case "la": return "\u0e25\u0e30\u0e15\u0e34\u0e19";
			case "lv": return "\u0e41\u0e25\u0e15\u0e40\u0e27\u0e35\u0e22 (\u0e40\u0e25\u0e17\u0e17\u0e34\u0e2a\u0e0a\u0e4c)";
			case "ln": return "\u0e25\u0e34\u0e07\u0e01\u0e32\u0e25\u0e32";
			case "lt": return "\u0e25\u0e34\u0e18\u0e31\u0e27\u0e40\u0e19\u0e35\u0e22";
			case "mk": return "\u0e41\u0e21\u0e0b\u0e35\u0e42\u0e14\u0e40\u0e19\u0e35\u0e22";
			case "mg": return "\u0e21\u0e32\u0e25\u0e32\u0e01\u0e32\u0e0b\u0e35";
			case "ms": return "\u0e21\u0e25\u0e32\u0e22\u0e39";
			case "ml": return "\u0e41\u0e21\u0e25\u0e30\u0e22\u0e32\u0e25\u0e31\u0e21";
			case "mt": return "\u0e21\u0e2d\u0e25\u0e15\u0e32";
			case "mi": return "\u0e40\u0e21\u0e32\u0e23\u0e35";
			case "mr": return "\u0e21\u0e32\u0e23\u0e32\u0e17\u0e35";
			case "mo": return "\u0e42\u0e21\u0e14\u0e32\u0e40\u0e27\u0e35\u0e22";
			case "mn": return "\u0e21\u0e2d\u0e07\u0e42\u0e01\u0e25";
			case "na": return "\u0e19\u0e2d\u0e23\u0e39";
			case "ne": return "\u0e40\u0e19\u0e1b\u0e32\u0e25";
			case "no": return "\u0e19\u0e2d\u0e23\u0e4c\u0e40\u0e27\u0e22\u0e4c";
			case "oc": return "\u0e2d\u0e2d\u0e01\u0e0b\u0e34\u0e17\u0e31\u0e19";
			case "or": return "\u0e42\u0e2d\u0e23\u0e34\u0e22\u0e32";
			case "om": return "\u0e42\u0e2d\u0e42\u0e23\u0e42\u0e21 (\u0e2d\u0e32\u0e1f\u0e32\u0e19)";
			case "ps": return "\u0e1e\u0e32\u0e2a\u0e0a\u0e4c\u0e42\u0e15 (\u0e1e\u0e38\u0e2a\u0e0a\u0e4c\u0e42\u0e15)";
			case "fa": return "\u0e40\u0e1b\u0e2d\u0e23\u0e4c\u0e40\u0e0b\u0e35\u0e22";
			case "pl": return "\u0e42\u0e1b\u0e41\u0e25\u0e19\u0e14\u0e4c";
			case "pt": return "\u0e42\u0e1b\u0e23\u0e15\u0e38\u0e40\u0e01\u0e2a";
			case "pa": return "\u0e1b\u0e31\u0e0d\u0e08\u0e32\u0e1b";
			case "qu": return "\u0e04\u0e34\u0e27\u0e0a\u0e31\u0e27";
			case "rm": return "\u0e40\u0e23\u0e42\u0e15-\u0e42\u0e23\u0e41\u0e21\u0e19\u0e0b\u0e4c";
			case "ro": return "\u0e42\u0e23\u0e21\u0e31\u0e19";
			case "ru": return "\u0e23\u0e31\u0e2a\u0e40\u0e0b\u0e35\u0e22";
			case "sm": return "\u0e0b\u0e32\u0e21\u0e31\u0e27";
			case "sg": return "\u0e2a\u0e31\u0e19\u0e42\u0e04";
			case "sa": return "\u0e2a\u0e31\u0e19\u0e2a\u0e01\u0e24\u0e15";
			case "gd": return "\u0e2a\u0e01\u0e47\u0e2d\u0e15\u0e2a\u0e4c\u0e40\u0e01\u0e25\u0e34\u0e04";
			case "sr": return "\u0e40\u0e0b\u0e2d\u0e23\u0e4c\u0e40\u0e1a\u0e35\u0e22";
			case "sh": return "\u0e40\u0e0b\u0e2d\u0e23\u0e4c\u0e42\u0e1a-\u0e42\u0e04\u0e23\u0e40\u0e2d\u0e40\u0e17\u0e35\u0e22\u0e19";
			case "st": return "\u0e40\u0e0b\u0e42\u0e2a\u0e42\u0e17";
			case "tn": return "\u0e40\u0e0b\u0e15\u0e2a\u0e27\u0e32\u0e19\u0e32";
			case "sn": return "\u0e42\u0e0b\u0e19\u0e32";
			case "sd": return "\u0e0b\u0e34\u0e19\u0e14\u0e34";
			case "si": return "\u0e2a\u0e34\u0e07\u0e2b\u0e25";
			case "ss": return "\u0e0b\u0e35\u0e2a\u0e27\u0e32\u0e15\u0e34";
			case "sk": return "\u0e2a\u0e42\u0e25\u0e27\u0e31\u0e04";
			case "sl": return "\u0e2a\u0e42\u0e25\u0e40\u0e27\u0e40\u0e19\u0e35\u0e22";
			case "so": return "\u0e42\u0e0b\u0e21\u0e32\u0e25\u0e35";
			case "es": return "\u0e2a\u0e40\u0e1b\u0e19";
			case "su": return "\u0e0b\u0e31\u0e19\u0e14\u0e32\u0e19\u0e35\u0e2a";
			case "sw": return "\u0e0b\u0e27\u0e32\u0e2e\u0e34\u0e23\u0e35";
			case "sv": return "\u0e2a\u0e27\u0e35\u0e40\u0e14\u0e19";
			case "tl": return "\u0e15\u0e32\u0e01\u0e32\u0e25\u0e47\u0e2d\u0e01";
			case "tg": return "\u0e17\u0e32\u0e08\u0e34\u0e04";
			case "ta": return "\u0e17\u0e21\u0e34\u0e2c";
			case "tt": return "\u0e15\u0e32\u0e14";
			case "te": return "\u0e17\u0e34\u0e25\u0e39\u0e01\u0e39";
			case "th": return "\u0e44\u0e17\u0e22";
			case "bo": return "\u0e17\u0e34\u0e40\u0e1a\u0e15";
			case "ti": return "\u0e17\u0e34\u0e01\u0e23\u0e34\u0e19\u0e22\u0e32";
			case "to": return "\u0e17\u0e2d\u0e07\u0e01\u0e49\u0e32";
			case "ts": return "\u0e0b\u0e2d\u0e07\u0e01\u0e32";
			case "tr": return "\u0e15\u0e38\u0e23\u0e01\u0e35";
			case "tk": return "\u0e40\u0e15\u0e34\u0e23\u0e4c\u0e01\u0e40\u0e21\u0e19";
			case "tw": return "\u0e17\u0e27\u0e35";
			case "ug": return "\u0e2d\u0e38\u0e22\u0e01\u0e31\u0e27";
			case "uk": return "\u0e22\u0e39\u0e40\u0e04\u0e23\u0e19";
			case "ur": return "\u0e2d\u0e34\u0e23\u0e14\u0e39";
			case "uz": return "\u0e2d\u0e38\u0e2a\u0e40\u0e1a\u0e04";
			case "vi": return "\u0e40\u0e27\u0e35\u0e22\u0e14\u0e19\u0e32\u0e21";
			case "vo": return "\u0e42\u0e27\u0e25\u0e32\u0e1e\u0e38\u0e01";
			case "cy": return "\u0e40\u0e27\u0e25\u0e2a\u0e4c";
			case "wo": return "\u0e27\u0e39\u0e25\u0e2d\u0e1f";
			case "xh": return "\u0e42\u0e0b\u0e2a\u0e32";
			case "yi": return "\u0e22\u0e35\u0e14\u0e34\u0e0a";
			case "yo": return "\u0e42\u0e22\u0e23\u0e39\u0e1a\u0e32";
			case "za": return "\u0e08\u0e27\u0e07";
			case "zu": return "\u0e0b\u0e39\u0e25\u0e39";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "AF": return "\u0e2d\u0e31\u0e1f\u0e01\u0e32\u0e19\u0e34\u0e2a\u0e16\u0e32\u0e19";
			case "AL": return "\u0e41\u0e2d\u0e25\u0e40\u0e1a\u0e40\u0e19\u0e35\u0e22";
			case "DZ": return "\u0e41\u0e2d\u0e25\u0e08\u0e35\u0e40\u0e23\u0e35\u0e22";
			case "AD": return "\u0e2d\u0e31\u0e19\u0e14\u0e2d\u0e23\u0e4c\u0e23\u0e32";
			case "AO": return "\u0e2d\u0e31\u0e19\u0e42\u0e01\u0e25\u0e32";
			case "AI": return "\u0e2d\u0e31\u0e19\u0e01\u0e34\u0e25\u0e48\u0e32";
			case "AR": return "\u0e2d\u0e32\u0e23\u0e4c\u0e40\u0e08\u0e19\u0e15\u0e34\u0e19\u0e48\u0e32";
			case "AM": return "\u0e2d\u0e32\u0e23\u0e4c\u0e21\u0e35\u0e40\u0e19\u0e35\u0e22";
			case "AW": return "\u0e2d\u0e32\u0e23\u0e39\u0e1a\u0e32";
			case "AU": return "\u0e2d\u0e2d\u0e2a\u0e40\u0e15\u0e23\u0e40\u0e25\u0e35\u0e22";
			case "AT": return "\u0e2d\u0e2d\u0e2a\u0e40\u0e15\u0e23\u0e35\u0e22";
			case "AZ": return "\u0e2d\u0e32\u0e40\u0e0b\u0e2d\u0e23\u0e4c\u0e44\u0e1a\u0e08\u0e31\u0e19";
			case "BS": return "\u0e1a\u0e32\u0e2e\u0e32\u0e21\u0e32\u0e2a";
			case "BH": return "\u0e1a\u0e32\u0e2b\u0e4c\u0e40\u0e23\u0e19";
			case "BD": return "\u0e1a\u0e31\u0e07\u0e04\u0e25\u0e32\u0e40\u0e17\u0e28";
			case "BB": return "\u0e1a\u0e32\u0e23\u0e4c\u0e1a\u0e32\u0e14\u0e2d\u0e2a";
			case "BY": return "\u0e40\u0e1a\u0e25\u0e25\u0e32\u0e23\u0e31\u0e2a";
			case "BE": return "\u0e40\u0e1a\u0e25\u0e40\u0e22\u0e35\u0e48\u0e22\u0e21";
			case "BZ": return "\u0e40\u0e1a\u0e25\u0e34\u0e0b";
			case "BJ": return "\u0e40\u0e1a\u0e19\u0e34\u0e19";
			case "BM": return "\u0e40\u0e1a\u0e2d\u0e23\u0e4c\u0e21\u0e34\u0e27\u0e14\u0e49\u0e32";
			case "BT": return "\u0e20\u0e39\u0e10\u0e32\u0e19";
			case "BO": return "\u0e42\u0e1a\u0e25\u0e34\u0e40\u0e27\u0e35\u0e22";
			case "BA": return "\u0e1a\u0e2d\u0e2a\u0e40\u0e19\u0e35\u0e22 \u0e41\u0e25\u0e30 \u0e40\u0e2e\u0e34\u0e23\u0e4c\u0e0b\u0e42\u0e01\u0e27\u0e34\u0e40\u0e19\u0e35\u0e22";
			case "BW": return "\u0e1a\u0e2d\u0e15\u0e2a\u0e27\u0e32\u0e19\u0e32";
			case "BR": return "\u0e1a\u0e23\u0e32\u0e0b\u0e34\u0e25";
			case "BN": return "\u0e1a\u0e23\u0e39\u0e44\u0e19";
			case "BG": return "\u0e1a\u0e31\u0e25\u0e41\u0e01\u0e40\u0e23\u0e35\u0e22";
			case "BF": return "\u0e40\u0e1a\u0e2d\u0e23\u0e4c\u0e01\u0e34\u0e19\u0e32\u0e1f\u0e32\u0e42\u0e0b";
			case "BI": return "\u0e1a\u0e39\u0e23\u0e31\u0e19\u0e14\u0e34";
			case "KH": return "\u0e01\u0e31\u0e21\u0e1e\u0e39\u0e0a\u0e32";
			case "CM": return "\u0e04\u0e32\u0e40\u0e21\u0e23\u0e39\u0e19";
			case "CA": return "\u0e41\u0e04\u0e19\u0e32\u0e14\u0e32";
			case "CV": return "\u0e40\u0e04\u0e1e\u0e40\u0e27\u0e2d\u0e23\u0e4c\u0e14";
			case "CF": return "\u0e2a\u0e32\u0e18\u0e32\u0e23\u0e13\u0e23\u0e31\u0e10\u0e41\u0e2d\u0e1f\u0e23\u0e34\u0e01\u0e32\u0e01\u0e25\u0e32\u0e07";
			case "TD": return "\u0e0a\u0e32\u0e14";
			case "CL": return "\u0e0a\u0e34\u0e25\u0e35";
			case "CN": return "\u0e08\u0e35\u0e19";
			case "CO": return "\u0e42\u0e04\u0e25\u0e31\u0e21\u0e40\u0e1a\u0e35\u0e22";
			case "KM": return "\u0e42\u0e04\u0e42\u0e21\u0e23\u0e2d\u0e2a";
			case "CG": return "\u0e04\u0e2d\u0e07\u0e42\u0e01";
			case "CR": return "\u0e04\u0e2d\u0e2a\u0e15\u0e32\u0e23\u0e34\u0e01\u0e49\u0e32";
			case "CI": return "\u0e1d\u0e31\u0e48\u0e07\u0e17\u0e30\u0e40\u0e25\u0e44\u0e2d\u0e27\u0e2d\u0e23\u0e34";
			case "HR": return "\u0e42\u0e04\u0e23\u0e40\u0e2d\u0e40\u0e0a\u0e35\u0e22";
			case "CU": return "\u0e04\u0e34\u0e27\u0e1a\u0e32";
			case "CY": return "\u0e44\u0e0b\u0e1b\u0e23\u0e31\u0e2a";
			case "CZ": return "\u0e2a\u0e32\u0e18\u0e32\u0e23\u0e13\u0e23\u0e31\u0e10\u0e40\u0e0a\u0e47\u0e04";
			case "DK": return "\u0e40\u0e14\u0e19\u0e21\u0e32\u0e23\u0e4c\u0e01";
			case "DJ": return "\u0e14\u0e34\u0e42\u0e1a\u0e15\u0e34";
			case "DM": return "\u0e42\u0e14\u0e21\u0e34\u0e19\u0e34\u0e01\u0e49\u0e32";
			case "DO": return "\u0e2a\u0e32\u0e18\u0e32\u0e23\u0e13\u0e23\u0e31\u0e10\u0e42\u0e14\u0e21\u0e34\u0e19\u0e34\u0e01\u0e31\u0e19";
			case "TL": return "\u0e15\u0e34\u0e21\u0e2d\u0e23\u0e4c\u0e15\u0e30\u0e27\u0e31\u0e19\u0e2d\u0e2d\u0e01";
			case "EC": return "\u0e40\u0e2d\u0e01\u0e27\u0e32\u0e14\u0e2d\u0e23\u0e4c";
			case "EG": return "\u0e2d\u0e35\u0e22\u0e34\u0e1b\u0e15\u0e4c";
			case "SV": return "\u0e40\u0e2d\u0e25\u0e0b\u0e32\u0e27\u0e32\u0e14\u0e2d\u0e23\u0e4c";
			case "GQ": return "\u0e40\u0e2d\u0e04\u0e27\u0e32\u0e42\u0e17\u0e40\u0e23\u0e35\u0e22\u0e25\u0e01\u0e34\u0e19\u0e35";
			case "ER": return "\u0e2d\u0e34\u0e23\u0e34\u0e17\u0e23\u0e35";
			case "EE": return "\u0e40\u0e2d\u0e2a\u0e42\u0e15\u0e40\u0e19\u0e35\u0e22";
			case "ET": return "\u0e40\u0e2d\u0e18\u0e34\u0e42\u0e2d\u0e40\u0e1b\u0e35\u0e22";
			case "FJ": return "\u0e1f\u0e34\u0e08\u0e34";
			case "FI": return "\u0e1f\u0e34\u0e19\u0e41\u0e25\u0e19\u0e14\u0e4c";
			case "FR": return "\u0e1d\u0e23\u0e31\u0e48\u0e07\u0e40\u0e28\u0e2a";
			case "GF": return "\u0e40\u0e1f\u0e23\u0e47\u0e19\u0e0a\u0e01\u0e34\u0e27\u0e19\u0e48\u0e32";
			case "PF": return "\u0e40\u0e1f\u0e23\u0e47\u0e19\u0e0a\u0e42\u0e1e\u0e25\u0e34\u0e19\u0e35\u0e40\u0e0b\u0e35\u0e22";
			case "TF": return "\u0e2d\u0e32\u0e13\u0e32\u0e40\u0e02\u0e15\u0e17\u0e32\u0e07\u0e43\u0e15\u0e49\u0e02\u0e2d\u0e07\u0e1d\u0e23\u0e31\u0e48\u0e07\u0e40\u0e28\u0e2a";
			case "GA": return "\u0e01\u0e32\u0e1a\u0e2d\u0e19";
			case "GM": return "\u0e41\u0e01\u0e21\u0e40\u0e1a\u0e35\u0e22";
			case "GE": return "\u0e08\u0e2d\u0e23\u0e4c\u0e40\u0e08\u0e35\u0e22";
			case "DE": return "\u0e40\u0e22\u0e2d\u0e23\u0e21\u0e19\u0e35";
			case "GH": return "\u0e01\u0e32\u0e19\u0e48\u0e32";
			case "GR": return "\u0e01\u0e23\u0e35\u0e0b";
			case "GP": return "\u0e01\u0e31\u0e27\u0e40\u0e14\u0e2d\u0e25\u0e39\u0e1b";
			case "GT": return "\u0e01\u0e31\u0e27\u0e40\u0e15\u0e21\u0e32\u0e25\u0e32";
			case "GN": return "\u0e01\u0e34\u0e27\u0e19\u0e35";
			case "GW": return "\u0e01\u0e34\u0e27\u0e19\u0e35-\u0e1a\u0e34\u0e2a\u0e42\u0e0b";
			case "GY": return "\u0e01\u0e39\u0e22\u0e32\u0e19\u0e48\u0e32";
			case "HT": return "\u0e44\u0e2e\u0e15\u0e35";
			case "HN": return "\u0e2e\u0e2d\u0e19\u0e14\u0e39\u0e23\u0e31\u0e2a";
			case "HK": return "\u0e2e\u0e48\u0e2d\u0e07\u0e01\u0e07";
			case "HU": return "\u0e2e\u0e31\u0e07\u0e01\u0e32\u0e23\u0e35";
			case "IS": return "\u0e44\u0e2d\u0e0b\u0e41\u0e25\u0e19\u0e14\u0e4c";
			case "IN": return "\u0e2d\u0e34\u0e19\u0e40\u0e14\u0e35\u0e22";
			case "ID": return "\u0e2d\u0e34\u0e19\u0e42\u0e14\u0e19\u0e35\u0e40\u0e0b\u0e35\u0e22";
			case "IR": return "\u0e2d\u0e34\u0e2b\u0e23\u0e48\u0e32\u0e19";
			case "IQ": return "\u0e2d\u0e34\u0e23\u0e31\u0e01";
			case "IE": return "\u0e44\u0e2d\u0e23\u0e4c\u0e41\u0e25\u0e19\u0e14\u0e4c";
			case "IL": return "\u0e2d\u0e34\u0e2a\u0e23\u0e32\u0e40\u0e2d\u0e25";
			case "IT": return "\u0e2d\u0e34\u0e15\u0e32\u0e25\u0e35";
			case "JM": return "\u0e08\u0e32\u0e44\u0e21\u0e01\u0e49\u0e32";
			case "JP": return "\u0e0d\u0e35\u0e48\u0e1b\u0e38\u0e48\u0e19";
			case "JO": return "\u0e08\u0e2d\u0e23\u0e4c\u0e41\u0e14\u0e19";
			case "KZ": return "\u0e04\u0e32\u0e0b\u0e31\u0e04\u0e2a\u0e16\u0e32\u0e19";
			case "KE": return "\u0e40\u0e04\u0e19\u0e22\u0e48\u0e32";
			case "KI": return "\u0e04\u0e34\u0e23\u0e35\u0e1a\u0e32\u0e15\u0e34";
			case "KP": return "\u0e40\u0e01\u0e32\u0e2b\u0e25\u0e35\u0e40\u0e2b\u0e19\u0e37\u0e2d";
			case "KR": return "\u0e40\u0e01\u0e32\u0e2b\u0e25\u0e35\u0e43\u0e15\u0e49";
			case "KW": return "\u0e04\u0e39\u0e40\u0e27\u0e15";
			case "KG": return "\u0e40\u0e04\u0e2d\u0e23\u0e4c\u0e01\u0e34\u0e2a\u0e16\u0e32\u0e19";
			case "LA": return "\u0e25\u0e32\u0e27";
			case "LV": return "\u0e25\u0e32\u0e15\u0e40\u0e27\u0e35\u0e22";
			case "LB": return "\u0e40\u0e25\u0e1a\u0e32\u0e19\u0e2d\u0e19";
			case "LS": return "\u0e40\u0e25\u0e42\u0e0b\u0e42\u0e17";
			case "LR": return "\u0e25\u0e34\u0e40\u0e1a\u0e2d\u0e23\u0e4c\u0e40\u0e25\u0e35\u0e22";
			case "LY": return "\u0e25\u0e34\u0e40\u0e1a\u0e35\u0e22";
			case "LI": return "\u0e44\u0e25\u0e40\u0e17\u0e19\u0e2a\u0e44\u0e15\u0e19\u0e4c";
			case "LT": return "\u0e25\u0e34\u0e40\u0e17\u0e2d\u0e23\u0e4c\u0e40\u0e19\u0e35\u0e22";
			case "LU": return "\u0e25\u0e31\u0e01\u0e0b\u0e4c\u0e40\u0e0b\u0e21\u0e40\u0e1a\u0e2d\u0e23\u0e4c\u0e01";
			case "MK": return "\u0e41\u0e21\u0e0b\u0e35\u0e42\u0e14\u0e40\u0e19\u0e35\u0e22";
			case "MG": return "\u0e21\u0e32\u0e14\u0e32\u0e01\u0e32\u0e2a\u0e01\u0e49\u0e32";
			case "MO": return "\u0e21\u0e32\u0e40\u0e01\u0e4a\u0e32";
			case "MY": return "\u0e21\u0e32\u0e40\u0e25\u0e40\u0e0b\u0e35\u0e22";
			case "ML": return "\u0e21\u0e32\u0e25\u0e35";
			case "MT": return "\u0e21\u0e31\u0e25\u0e15\u0e49\u0e32";
			case "MQ": return "\u0e21\u0e32\u0e23\u0e4c\u0e15\u0e34\u0e19\u0e34\u0e01";
			case "MR": return "\u0e21\u0e2d\u0e23\u0e34\u0e17\u0e32\u0e40\u0e19\u0e35\u0e22";
			case "MU": return "\u0e21\u0e2d\u0e23\u0e34\u0e40\u0e15\u0e35\u0e22\u0e2a";
			case "YT": return "\u0e21\u0e32\u0e22\u0e2d\u0e15";
			case "MX": return "\u0e41\u0e21\u0e47\u0e01\u0e0b\u0e34\u0e42\u0e01";
			case "FM": return "\u0e44\u0e21\u0e42\u0e04\u0e23\u0e19\u0e34\u0e40\u0e0b\u0e35\u0e22";
			case "MD": return "\u0e42\u0e21\u0e25\u0e42\u0e14\u0e27\u0e32";
			case "MC": return "\u0e42\u0e21\u0e19\u0e32\u0e42\u0e04";
			case "MN": return "\u0e21\u0e2d\u0e07\u0e42\u0e01\u0e40\u0e25\u0e35\u0e22";
			case "MS": return "\u0e21\u0e2d\u0e19\u0e15\u0e4c\u0e40\u0e0b\u0e2d\u0e23\u0e32\u0e15";
			case "MA": return "\u0e42\u0e21\u0e23\u0e2d\u0e04\u0e42\u0e04";
			case "MZ": return "\u0e42\u0e21\u0e41\u0e0b\u0e21\u0e1a\u0e34\u0e04";
			case "MM": return "\u0e2a\u0e2b\u0e20\u0e32\u0e1e\u0e1e\u0e21\u0e48\u0e32";
			case "NA": return "\u0e19\u0e32\u0e21\u0e34\u0e40\u0e1a\u0e35\u0e22";
			case "NP": return "\u0e40\u0e19\u0e1b\u0e32\u0e25";
			case "NL": return "\u0e40\u0e19\u0e40\u0e18\u0e2d\u0e23\u0e4c\u0e41\u0e25\u0e19\u0e14\u0e4c";
			case "AN": return "\u0e40\u0e19\u0e40\u0e18\u0e2d\u0e23\u0e4c\u0e41\u0e25\u0e19\u0e14\u0e4c\u0e41\u0e2d\u0e19\u0e17\u0e34\u0e25\u0e25\u0e4c";
			case "NC": return "\u0e19\u0e34\u0e27\u0e04\u0e32\u0e25\u0e34\u0e42\u0e14\u0e40\u0e19\u0e35\u0e22";
			case "NZ": return "\u0e19\u0e34\u0e27\u0e0b\u0e35\u0e41\u0e25\u0e19\u0e14\u0e4c";
			case "NI": return "\u0e19\u0e34\u0e04\u0e32\u0e23\u0e32\u0e01\u0e31\u0e27";
			case "NE": return "\u0e44\u0e19\u0e40\u0e08\u0e2d\u0e23\u0e4c";
			case "NG": return "\u0e44\u0e19\u0e08\u0e35\u0e40\u0e23\u0e35\u0e22";
			case "NU": return "\u0e19\u0e35\u0e22\u0e39";
			case "NO": return "\u0e19\u0e2d\u0e23\u0e4c\u0e40\u0e27\u0e22\u0e4c";
			case "OM": return "\u0e42\u0e2d\u0e21\u0e32\u0e19";
			case "PK": return "\u0e1b\u0e32\u0e01\u0e35\u0e2a\u0e16\u0e32\u0e19";
			case "PA": return "\u0e1b\u0e32\u0e19\u0e32\u0e21\u0e32";
			case "PG": return "\u0e1b\u0e32\u0e1b\u0e31\u0e27\u0e19\u0e34\u0e27\u0e01\u0e35\u0e19\u0e35";
			case "PY": return "\u0e1b\u0e32\u0e23\u0e32\u0e01\u0e27\u0e31\u0e22";
			case "PE": return "\u0e40\u0e1b\u0e23\u0e39";
			case "PH": return "\u0e1f\u0e34\u0e25\u0e34\u0e1b\u0e1b\u0e34\u0e19\u0e2a\u0e4c";
			case "PL": return "\u0e42\u0e1b\u0e41\u0e25\u0e19\u0e14\u0e4c";
			case "PT": return "\u0e42\u0e1b\u0e15\u0e38\u0e01\u0e31\u0e25";
			case "PR": return "\u0e40\u0e1b\u0e2d\u0e23\u0e4c\u0e42\u0e15\u0e23\u0e34\u0e42\u0e01";
			case "QA": return "\u0e01\u0e32\u0e15\u0e32\u0e23\u0e4c";
			case "RO": return "\u0e23\u0e39\u0e40\u0e21\u0e40\u0e19\u0e35\u0e22";
			case "RU": return "\u0e23\u0e31\u0e2a\u0e40\u0e0b\u0e35\u0e22";
			case "RW": return "\u0e23\u0e32\u0e27\u0e31\u0e25\u0e14\u0e32";
			case "SA": return "\u0e0b\u0e32\u0e2d\u0e38\u0e14\u0e34\u0e2d\u0e32\u0e23\u0e30\u0e40\u0e1a\u0e35\u0e22";
			case "SN": return "\u0e0b\u0e34\u0e19\u0e35\u0e01\u0e31\u0e25";
			case "SP": return "\u0e40\u0e0b\u0e2d\u0e23\u0e4c\u0e40\u0e1a\u0e35\u0e22";
			case "SC": return "\u0e40\u0e0b\u0e22\u0e4c\u0e41\u0e0a\u0e25\u0e25\u0e4c";
			case "SL": return "\u0e40\u0e0b\u0e35\u0e22\u0e23\u0e4c\u0e23\u0e48\u0e32\u0e25\u0e35\u0e2d\u0e2d\u0e19";
			case "SG": return "\u0e2a\u0e34\u0e07\u0e04\u0e42\u0e1b\u0e23\u0e4c";
			case "SK": return "\u0e2a\u0e42\u0e25\u0e27\u0e32\u0e40\u0e01\u0e35\u0e22";
			case "SI": return "\u0e2a\u0e42\u0e25\u0e27\u0e34\u0e40\u0e19\u0e35\u0e22";
			case "SO": return "\u0e42\u0e0b\u0e21\u0e32\u0e40\u0e25\u0e35\u0e22";
			case "ZA": return "\u0e41\u0e2d\u0e1f\u0e23\u0e34\u0e01\u0e32\u0e43\u0e15\u0e49";
			case "ES": return "\u0e2a\u0e40\u0e1b\u0e19";
			case "LK": return "\u0e28\u0e23\u0e35\u0e25\u0e31\u0e07\u0e01\u0e32";
			case "SD": return "\u0e0b\u0e39\u0e14\u0e32\u0e19";
			case "SR": return "\u0e0b\u0e39\u0e23\u0e34\u0e19\u0e32\u0e21\u0e34";
			case "SZ": return "\u0e2a\u0e27\u0e32\u0e0b\u0e34\u0e41\u0e25\u0e19\u0e14\u0e4c";
			case "SE": return "\u0e2a\u0e27\u0e35\u0e40\u0e14\u0e19";
			case "CH": return "\u0e2a\u0e27\u0e34\u0e2a\u0e40\u0e0b\u0e2d\u0e23\u0e4c\u0e41\u0e25\u0e19\u0e14\u0e4c";
			case "SY": return "\u0e0b\u0e35\u0e40\u0e23\u0e35\u0e22";
			case "TW": return "\u0e44\u0e15\u0e49\u0e2b\u0e27\u0e31\u0e19";
			case "TJ": return "\u0e17\u0e32\u0e08\u0e34\u0e01\u0e34\u0e2a\u0e16\u0e32\u0e19";
			case "TZ": return "\u0e17\u0e32\u0e19\u0e0b\u0e32\u0e40\u0e19\u0e35\u0e22";
			case "TH": return "\u0e1b\u0e23\u0e30\u0e40\u0e17\u0e28\u0e44\u0e17\u0e22";
			case "TG": return "\u0e42\u0e15\u0e42\u0e01";
			case "TK": return "\u0e42\u0e17\u0e01\u0e34\u0e42\u0e25";
			case "TO": return "\u0e17\u0e2d\u0e07\u0e01\u0e49\u0e32";
			case "TT": return "\u0e17\u0e23\u0e34\u0e19\u0e34\u0e41\u0e14\u0e14 \u0e41\u0e25\u0e30\u0e42\u0e17\u0e1a\u0e32\u0e42\u0e01";
			case "TN": return "\u0e15\u0e39\u0e19\u0e34\u0e40\u0e0b\u0e35\u0e22";
			case "TR": return "\u0e15\u0e38\u0e23\u0e01\u0e35";
			case "TM": return "\u0e40\u0e15\u0e34\u0e23\u0e4c\u0e01\u0e40\u0e21\u0e19\u0e34\u0e2a\u0e16\u0e32\u0e19";
			case "UG": return "\u0e2d\u0e39\u0e01\u0e32\u0e19\u0e14\u0e32";
			case "UA": return "\u0e22\u0e39\u0e40\u0e04\u0e23\u0e19";
			case "AE": return "\u0e2a\u0e2b\u0e23\u0e31\u0e10\u0e2d\u0e32\u0e2b\u0e23\u0e31\u0e1a\u0e40\u0e2d\u0e21\u0e34\u0e40\u0e23\u0e15\u0e2a\u0e4c";
			case "GB": return "\u0e2a\u0e2b\u0e23\u0e32\u0e0a\u0e2d\u0e32\u0e13\u0e32\u0e08\u0e31\u0e01\u0e23";
			case "US": return "\u0e2a\u0e2b\u0e23\u0e31\u0e10\u0e2d\u0e40\u0e21\u0e23\u0e34\u0e01\u0e32";
			case "UY": return "\u0e2d\u0e38\u0e23\u0e39\u0e01\u0e27\u0e31\u0e22";
			case "UZ": return "\u0e2d\u0e38\u0e0b\u0e40\u0e1a\u0e01\u0e34\u0e2a\u0e16\u0e32\u0e19";
			case "VU": return "\u0e27\u0e32\u0e19\u0e31\u0e27\u0e15\u0e39";
			case "VA": return "\u0e27\u0e32\u0e15\u0e34\u0e01\u0e31\u0e19";
			case "VE": return "\u0e40\u0e27\u0e40\u0e19\u0e0b\u0e39\u0e40\u0e2d\u0e25\u0e48\u0e32";
			case "VN": return "\u0e40\u0e27\u0e35\u0e22\u0e14\u0e19\u0e32\u0e21";
			case "VG": return "\u0e1a\u0e23\u0e34\u0e17\u0e34\u0e0a\u0e40\u0e27\u0e2d\u0e23\u0e4c\u0e08\u0e34\u0e19\u0e44\u0e2d\u0e2a\u0e4c\u0e41\u0e25\u0e19\u0e14\u0e4c";
			case "VI": return "\u0e22\u0e39\u0e40\u0e2d\u0e2a\u0e40\u0e27\u0e2d\u0e23\u0e4c\u0e08\u0e34\u0e19\u0e44\u0e2d\u0e2a\u0e4c\u0e41\u0e25\u0e19\u0e14\u0e4c";
			case "EH": return "\u0e0b\u0e32\u0e2e\u0e32\u0e23\u0e48\u0e32\u0e15\u0e30\u0e27\u0e31\u0e19\u0e15\u0e01";
			case "YE": return "\u0e40\u0e22\u0e40\u0e21\u0e19";
			case "YU": return "\u0e22\u0e39\u0e42\u0e01\u0e2a\u0e25\u0e32\u0e40\u0e27\u0e35\u0e22";
			case "ZM": return "\u0e41\u0e0b\u0e21\u0e40\u0e1a\u0e35\u0e22";
			case "ZW": return "\u0e0b\u0e34\u0e21\u0e1a\u0e32\u0e1a\u0e40\u0e27";
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
				return 874;
			}
		}
		public override int EBCDICCodePage
		{
			get
			{
				return 20838;
			}
		}
		public override int MacCodePage
		{
			get
			{
				return 10021;
			}
		}
		public override int OEMCodePage
		{
			get
			{
				return 874;
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

}; // class CID001e

public class CNth : CID001e
{
	public CNth() : base() {}

}; // class CNth

}; // namespace I18N.Other
