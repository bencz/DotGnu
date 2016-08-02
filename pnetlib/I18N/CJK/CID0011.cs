/*
 * CID0011.cs - ja culture handler.
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

// Generated from "ja.txt".

namespace I18N.CJK
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0011 : RootCulture
{
	public CID0011() : base(0x0011) {}
	public CID0011(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "ja";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "jpn";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "JPN";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "ja";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AMDesignator = "\u5348\u524D";
			dfi.PMDesignator = "\u5348\u5F8C";
			dfi.AbbreviatedDayNames = new String[] {"\u65E5", "\u6708", "\u706B", "\u6C34", "\u6728", "\u91D1", "\u571F"};
			dfi.DayNames = new String[] {"\u65E5\u66DC\u65E5", "\u6708\u66DC\u65E5", "\u706B\u66DC\u65E5", "\u6C34\u66DC\u65E5", "\u6728\u66DC\u65E5", "\u91D1\u66DC\u65E5", "\u571F\u66DC\u65E5"};
			dfi.AbbreviatedMonthNames = new String[] {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", ""};
			dfi.MonthNames = new String[] {"1\u6708", "2\u6708", "3\u6708", "4\u6708", "5\u6708", "6\u6708", "7\u6708", "8\u6708", "9\u6708", "10\u6708", "11\u6708", "12\u6708", ""};
			dfi.DateSeparator = "/";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "yyyy/MM/dd";
			dfi.LongTimePattern = "H:mm:ss:z";
			dfi.ShortDatePattern = "yy/MM/dd";
			dfi.ShortTimePattern = "H:mm";
			dfi.FullDateTimePattern = "yyyy'\u5d74'M'\u6708'd'\u65d5' H'\u6642'mm'\u5206'ss'\u79D2'z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:yy/MM/dd",
				"D:yyyy'\u5d74'M'\u6708'd'\u65d5'",
				"f:yyyy'\u5d74'M'\u6708'd'\u65d5' H'\u6642'mm'\u5206'ss'\u79D2'z",
				"f:yyyy'\u5d74'M'\u6708'd'\u65d5' H:mm:ss:z",
				"f:yyyy'\u5d74'M'\u6708'd'\u65d5' H:mm:ss",
				"f:yyyy'\u5d74'M'\u6708'd'\u65d5' H:mm",
				"F:yyyy'\u5d74'M'\u6708'd'\u65d5' HH:mm:ss",
				"g:yy/MM/dd H'\u6642'mm'\u5206'ss'\u79D2'z",
				"g:yy/MM/dd H:mm:ss:z",
				"g:yy/MM/dd H:mm:ss",
				"g:yy/MM/dd H:mm",
				"G:yy/MM/dd HH:mm:ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:H'\u6642'mm'\u5206'ss'\u79D2'z",
				"t:H:mm:ss:z",
				"t:H:mm:ss",
				"t:H:mm",
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
			case "ab": return "\u30a2\u30d6\u30cf\u30ba\u8a9e";
			case "aa": return "\u30a2\u30d5\u30a1\u30fc\u30eb\u8a9e";
			case "af": return "\u30a2\u30d5\u30ea\u30ab\u30fc\u30f3\u30b9\u8a9e";
			case "sq": return "\u30a2\u30eb\u30d0\u30cb\u30a2\u8a9e";
			case "am": return "\u30a2\u30e0\u30cf\u30e9\u8a9e";
			case "ar": return "\u30a2\u30e9\u30d3\u30a2\u8a9e";
			case "hy": return "\u30a2\u30eb\u30e1\u30cb\u30a2\u8a9e";
			case "as": return "\u30a2\u30c3\u30b5\u30e0\u8a9e";
			case "ay": return "\u30a2\u30a4\u30de\u30e9\u8a9e";
			case "az": return "\u30a2\u30bc\u30eb\u30d0\u30a4\u30b8\u30e3\u30f3\u8a9e";
			case "ba": return "\u30d0\u30b7\u30ad\u30fc\u30eb\u8a9e";
			case "eu": return "\u30d0\u30b9\u30af\u8a9e";
			case "bn": return "\u30d9\u30f3\u30ac\u30eb\u8a9e";
			case "dz": return "\u30d6\u30fc\u30bf\u30f3\u8a9e";
			case "bh": return "\u30d3\u30cf\u2015\u30eb\u8a9e";
			case "bi": return "\u30d3\u30b9\u30e9\u30de\u8a9e";
			case "br": return "\u30d6\u30eb\u30c8\u30f3\u8a9e";
			case "bg": return "\u30d6\u30eb\u30ac\u30ea\u30a2\u8a9e";
			case "my": return "\u30d3\u30eb\u30de\u8a9e";
			case "be": return "\u767d\u30ed\u30b7\u30a2\u8a9e";
			case "km": return "\u30ab\u30f3\u30dc\u30b8\u30a2\u8a9e";
			case "ca": return "\u30ab\u30bf\u30ed\u30cb\u30a2\u8a9e";
			case "zh": return "\u4e2d\u56fd\u8a9e";
			case "co": return "\u30b3\u30eb\u30b7\u30ab\u8a9e";
			case "hr": return "\u30af\u30ed\u30a2\u30c1\u30a2\u8a9e";
			case "cs": return "\u30c1\u30a7\u30b3\u8a9e";
			case "da": return "\u30c7\u30f3\u30de\u30fc\u30af\u8a9e";
			case "nl": return "\u30aa\u30e9\u30f3\u30c0\u8a9e";
			case "en": return "\u82f1\u8a9e";
			case "eo": return "\u30a8\u30b9\u30da\u30e9\u30f3\u30c8\u8a9e";
			case "et": return "\u30a8\u30b9\u30c8\u30cb\u30a2\u8a9e";
			case "fo": return "\u30d5\u30a7\u30ed\u30fc\u8a9e";
			case "fj": return "\u30d5\u30a3\u30b8\u30fc\u8a9e";
			case "fi": return "\u30d5\u30a3\u30f3\u30e9\u30f3\u30c9\u8a9e";
			case "fr": return "\u30d5\u30e9\u30f3\u30b9\u8a9e";
			case "fy": return "\u30d5\u30ea\u30b8\u30a2\u8a9e";
			case "gl": return "\u30ac\u30ea\u30b7\u30a2\u8a9e";
			case "ka": return "\u30b0\u30eb\u30b8\u30a2\u8a9e";
			case "de": return "\u30c9\u30a4\u30c4\u8a9e";
			case "el": return "\u30ae\u30ea\u30b7\u30a2\u8a9e";
			case "kl": return "\u30b0\u30ea\u30fc\u30f3\u30e9\u30f3\u30c9\u8a9e";
			case "gn": return "\u30b0\u30ef\u30e9\u30cb\u8a9e";
			case "gu": return "\u30b0\u30b8\u30e3\u30e9\u30fc\u30c8\u8a9e";
			case "ha": return "\u30cf\u30a6\u30b5\u8a9e";
			case "he": return "\u30d8\u30d6\u30e9\u30a4\u8a9e";
			case "hi": return "\u30d2\u30f3\u30c7\u30a3\u30fc\u8a9e";
			case "hu": return "\u30cf\u30f3\u30ac\u30ea\u30fc\u8a9e";
			case "is": return "\u30a2\u30a4\u30b9\u30e9\u30f3\u30c9\u8a9e";
			case "id": return "\u30a4\u30f3\u30c9\u30cd\u30b7\u30a2\u8a9e";
			case "ia": return "\u56fd\u969b\u8a9e";
			case "ie": return "\u56fd\u969b\u8a9e";
			case "iu": return "\u30a4\u30cc\u30af\u30a6\u30c6\u30a3\u30c8\u30c3\u30c8\u8a9e";
			case "ik": return "\u30a4\u30cc\u30d4\u30a2\u30c3\u30af\u8a9e";
			case "ga": return "\u30a2\u30a4\u30eb\u30e9\u30f3\u30c9\u8a9e";
			case "it": return "\u30a4\u30bf\u30ea\u30a2\u8a9e";
			case "ja": return "\u65e5\u672c\u8a9e";
			case "jv": return "\u30b8\u30e3\u30ef\u8a9e";
			case "kn": return "\u30ab\u30f3\u30ca\u30c0\u8a9e";
			case "ks": return "\u30ab\u30b7\u30df\u30fc\u30eb\u8a9e";
			case "kk": return "\u30ab\u30b6\u30d5\u8a9e";
			case "rw": return "\u30eb\u30ef\u30f3\u30c0\u8a9e";
			case "ky": return "\u30ad\u30eb\u30ae\u30b9\u8a9e";
			case "rn": return "\u30eb\u30f3\u30b8\u8a9e";
			case "ko": return "\u97d3\u56fd\u8a9e";
			case "ku": return "\u30af\u30eb\u30c9\u8a9e";
			case "lo": return "\u30e9\u30aa\u8a9e";
			case "la": return "\u30e9\u30c6\u30f3\u8a9e";
			case "lv": return "\u30e9\u30c8\u30d3\u30a2\u8a9e (\u30ec\u30c3\u30c8\u8a9e)";
			case "ln": return "\u30ea\u30f3\u30ac\u30e9\u8a9e";
			case "lt": return "\u30ea\u30c8\u30a2\u30cb\u30a2\u8a9e";
			case "mk": return "\u30de\u30b1\u30c9\u30cb\u30a2\u8a9e";
			case "mg": return "\u30de\u30e9\u30ac\u30b7\u30fc\u8a9e";
			case "ms": return "\u30de\u30e9\u30a4\u8a9e";
			case "ml": return "\u30de\u30e9\u30e4\u2015\u30e9\u30e0\u8a9e";
			case "mt": return "\u30de\u30eb\u30bf\u8a9e";
			case "mi": return "\u30de\u30aa\u30ea\u8a9e";
			case "mr": return "\u30de\u30e9\u30fc\u30c6\u30a3\u30fc\u8a9e";
			case "mo": return "\u30e2\u30eb\u30c0\u30d3\u30a2\u8a9e";
			case "mn": return "\u30e2\u30f3\u30b4\u30eb\u8a9e";
			case "na": return "\u30ca\u30a6\u30eb\u8a9e";
			case "ne": return "\u30cd\u30d1\u30fc\u30eb\u8a9e";
			case "no": return "\u30ce\u30eb\u30a6\u30a7\u30fc\u8a9e";
			case "oc": return "\u30d7\u30ed\u30d0\u30f3\u30b9\u8a9e";
			case "or": return "\u30aa\u30ea\u30e4\u30fc\u8a9e";
			case "om": return "\u30ac\u30e9\u8a9e";
			case "ps": return "\u30d1\u30b7\u30e5\u30c8\u30fc\u8a9e";
			case "fa": return "\u30da\u30eb\u30b7\u30a2\u8a9e";
			case "pl": return "\u30dd\u30fc\u30e9\u30f3\u30c9\u8a9e";
			case "pt": return "\u30dd\u30eb\u30c8\u30ac\u30eb\u8a9e";
			case "pa": return "\u30d1\u30f3\u30b8\u30e3\u30d6\u8a9e";
			case "qu": return "\u30b1\u30c1\u30e5\u30a2\u8a9e";
			case "rm": return "\u30ec\u30c8\uff1d\u30ed\u30de\u30f3\u8a9e";
			case "ro": return "\u30eb\u30fc\u30de\u30cb\u30a2\u8a9e";
			case "ru": return "\u30ed\u30b7\u30a2\u8a9e";
			case "sm": return "\u30b5\u30e2\u30a2\u8a9e";
			case "sg": return "\u30b5\u30f3\u30b4\u8a9e";
			case "sa": return "\u30b5\u30f3\u30b9\u30af\u30ea\u30c3\u30c8\u8a9e";
			case "gd": return "\u30b9\u30b3\u30c3\u30c8\u30e9\u30f3\u30c9\u30fb\u30b2\u30fc\u30eb\u8a9e";
			case "sr": return "\u30bb\u30eb\u30d3\u30a2\u8a9e";
			case "sh": return "\u30bb\u30eb\u30dc\uff1d\u30af\u30ed\u30a2\u30c1\u30a2\u8a9e";
			case "st": return "\u30bb\u30bd\u30c8\u8a9e";
			case "tn": return "\u30c4\u30ef\u30ca\u8a9e";
			case "sn": return "\u30b7\u30e7\u30ca\u8a9e";
			case "sd": return "\u30b7\u30f3\u30c9\u8a9e";
			case "si": return "\u30b7\u30f3\u30cf\u30e9\u8a9e";
			case "ss": return "\u30b7\u30b9\u30ef\u30c6\u30a3\u8a9e";
			case "sk": return "\u30b9\u30ed\u30d0\u30ad\u30a2\u8a9e";
			case "sl": return "\u30b9\u30ed\u30d9\u30cb\u30a2\u8a9e";
			case "so": return "\u30bd\u30de\u30ea\u8a9e";
			case "es": return "\u30b9\u30da\u30a4\u30f3\u8a9e";
			case "su": return "\u30b9\u30f3\u30c0\u8a9e";
			case "sw": return "\u30b9\u30ef\u30d2\u30ea\u8a9e";
			case "sv": return "\u30b9\u30a6\u30a7\u30fc\u30c7\u30f3\u8a9e";
			case "tl": return "\u30bf\u30ac\u30ed\u30b0\u8a9e";
			case "tg": return "\u30bf\u30b8\u30af\u8a9e";
			case "ta": return "\u30bf\u30df\u30fc\u30eb\u8a9e";
			case "tt": return "\u30bf\u30bf\u30fc\u30eb\u8a9e";
			case "te": return "\u30c6\u30eb\u30b0\u8a9e";
			case "th": return "\u30bf\u30a4\u8a9e";
			case "bo": return "\u30c1\u30d9\u30c3\u30c8\u8a9e";
			case "ti": return "\u30c6\u30a3\u30b0\u30ea\u30cb\u30a2\u8a9e";
			case "to": return "\u30c8\u30f3\u30ac\u8a9e";
			case "ts": return "\u30c4\u30a9\u30f3\u30ac\u8a9e";
			case "tr": return "\u30c8\u30eb\u30b3\u8a9e";
			case "tk": return "\u30c8\u30eb\u30af\u30e1\u30f3\u8a9e";
			case "tw": return "\u30c8\u30a5\u30a4\u8a9e";
			case "ug": return "\u30a6\u30a4\u30b0\u30eb\u8a9e";
			case "uk": return "\u30a6\u30af\u30e9\u30a4\u30ca\u8a9e";
			case "ur": return "\u30a6\u30eb\u30c9\u30a5\u30fc\u8a9e";
			case "uz": return "\u30a6\u30ba\u30d9\u30af\u8a9e";
			case "vi": return "\u30d9\u30c8\u30ca\u30e0\u8a9e";
			case "vo": return "\u30dc\u30e9\u30d4\u30e5\u30af\u8a9e";
			case "cy": return "\u30a6\u30a7\u30fc\u30eb\u30ba\u8a9e";
			case "wo": return "\u30a6\u30a9\u30ed\u30d5\u8a9e";
			case "xh": return "\u30b3\u30b5\u8a9e";
			case "yi": return "\u30a4\u30c7\u30a3\u30c3\u30b7\u30e5\u8a9e";
			case "yo": return "\u30e8\u30eb\u30d0\u8a9e";
			case "za": return "\u30c1\u30ef\u30f3\u8a9e";
			case "zu": return "\u30ba\u30fc\u30eb\u30fc\u8a9e";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "AF": return "\u30a2\u30d5\u30ac\u30cb\u30b9\u30bf\u30f3";
			case "AL": return "\u30a2\u30eb\u30d0\u30cb\u30a2";
			case "DZ": return "\u30a2\u30eb\u30b8\u30a7\u30ea\u30a2";
			case "AD": return "\u30a2\u30f3\u30c9\u30e9";
			case "AO": return "\u30a2\u30f3\u30b4\u30e9";
			case "AI": return "\u30a2\u30f3\u30ae\u30e9";
			case "AR": return "\u30a2\u30eb\u30bc\u30f3\u30c1\u30f3";
			case "AM": return "\u30a2\u30eb\u30e1\u30cb\u30a2";
			case "AW": return "\u30a2\u30eb\u30d0\u5cf6";
			case "AU": return "\u30aa\u30fc\u30b9\u30c8\u30e9\u30ea\u30a2";
			case "AT": return "\u30aa\u30fc\u30b9\u30c8\u30ea\u30a2";
			case "AZ": return "\u30a2\u30bc\u30eb\u30d0\u30a4\u30b8\u30e3\u30f3";
			case "BS": return "\u30d0\u30cf\u30de";
			case "BH": return "\u30d0\u30fc\u30ec\u30fc\u30f3";
			case "BD": return "\u30d0\u30f3\u30b0\u30e9\u30c7\u30b7\u30e5";
			case "BB": return "\u30d0\u30eb\u30d0\u30c9\u30b9";
			case "BY": return "\u30d9\u30e9\u30eb\u30fc\u30b7";
			case "BE": return "\u30d9\u30eb\u30ae\u30fc";
			case "BZ": return "\u30d9\u30ea\u30fc\u30ba";
			case "BJ": return "\u30d9\u30cb\u30f3";
			case "BM": return "\u30d0\u30fc\u30df\u30e5\u30fc\u30c0\u8af8\u5cf6";
			case "BT": return "\u30d6\u30fc\u30bf\u30f3";
			case "BO": return "\u30dc\u30ea\u30d3\u30a2";
			case "BA": return "\u30dc\u30b9\u30cb\u30a2\u30fb\u30d8\u30eb\u30c4\u30a7\u30b4\u30d3\u30ca";
			case "BW": return "\u30dc\u30c4\u30ef\u30ca";
			case "BR": return "\u30d6\u30e9\u30b8\u30eb";
			case "BN": return "\u30d6\u30eb\u30cd\u30a4";
			case "BG": return "\u30d6\u30eb\u30ac\u30ea\u30a2";
			case "BF": return "\u30d6\u30eb\u30ad\u30ca\u30d5\u30a1\u30bd";
			case "BI": return "\u30d6\u30eb\u30f3\u30b8";
			case "KH": return "\u30ab\u30f3\u30dc\u30b8\u30a2";
			case "CM": return "\u30ab\u30e1\u30eb\u30fc\u30f3";
			case "CA": return "\u30ab\u30ca\u30c0";
			case "CV": return "\u30ab\u30fc\u30dc\u30d9\u30eb\u30c7";
			case "CF": return "\u4e2d\u592e\u30a2\u30d5\u30ea\u30ab\u5171\u548c\u56fd";
			case "TD": return "\u30c1\u30e3\u30c9";
			case "CL": return "\u30c1\u30ea";
			case "CN": return "\u4e2d\u83ef\u4eba\u6c11\u5171\u548c\u56fd";
			case "CO": return "\u30b3\u30ed\u30f3\u30d3\u30a2";
			case "KM": return "\u30b3\u30e2\u30ed";
			case "CG": return "\u30b3\u30f3\u30b4";
			case "CR": return "\u30b3\u30b9\u30bf\u30ea\u30ab";
			case "CI": return "\u30b3\u30fc\u30c8\u30b8\u30dc\u30a2\u30fc\u30eb";
			case "HR": return "\u30af\u30ed\u30a2\u30c1\u30a2";
			case "CU": return "\u30ad\u30e5\u30fc\u30d0";
			case "CY": return "\u30ad\u30d7\u30ed\u30b9";
			case "CZ": return "\u30c1\u30a7\u30b3";
			case "DK": return "\u30c7\u30f3\u30de\u30fc\u30af";
			case "DJ": return "\u30b8\u30d6\u30c1";
			case "DM": return "\u30c9\u30df\u30cb\u30ab\u56fd";
			case "DO": return "\u30c9\u30df\u30cb\u30ab\u5171\u548c\u56fd";
			case "TL": return "\u6771\u30c6\u30a3\u30e2\u30fc\u30eb";
			case "EC": return "\u30a8\u30af\u30a2\u30c9\u30eb";
			case "EG": return "\u30a8\u30b8\u30d7\u30c8";
			case "SV": return "\u30a8\u30eb\u30b5\u30eb\u30d0\u30c9\u30eb";
			case "GQ": return "\u8d64\u9053\u30ae\u30cb\u30a2";
			case "ER": return "\u30a8\u30ea\u30c8\u30ea\u30a2";
			case "EE": return "\u30a8\u30b9\u30c8\u30cb\u30a2";
			case "ET": return "\u30a8\u30c1\u30aa\u30d4\u30a2";
			case "FJ": return "\u30d5\u30a3\u30b8\u30fc";
			case "FI": return "\u30d5\u30a3\u30f3\u30e9\u30f3\u30c9";
			case "FR": return "\u30d5\u30e9\u30f3\u30b9";
			case "GF": return "\u4ecf\u9818\u30ae\u30a2\u30ca";
			case "PF": return "\u4ecf\u9818\u30dd\u30ea\u30cd\u30b7\u30a2";
			case "TF": return "\u4ecf\u5357\u65b9\u9818";
			case "GA": return "\u30ac\u30dc\u30f3";
			case "GM": return "\u30ac\u30f3\u30d3\u30a2";
			case "GE": return "\u30b0\u30eb\u30b8\u30a2";
			case "DE": return "\u30c9\u30a4\u30c4";
			case "GH": return "\u30ac\u30fc\u30ca";
			case "GR": return "\u30ae\u30ea\u30b7\u30a2";
			case "GP": return "\u30b0\u30a2\u30c9\u30eb\u30fc\u30d7";
			case "GT": return "\u30b0\u30a2\u30c6\u30de\u30e9";
			case "GN": return "\u30ae\u30cb\u30a2";
			case "GW": return "\u30ae\u30cb\u30a2\u30d3\u30b5\u30a6";
			case "GY": return "\u30ac\u30a4\u30a2\u30ca";
			case "HT": return "\u30cf\u30a4\u30c1";
			case "HN": return "\u30db\u30f3\u30b8\u30e5\u30e9\u30b9";
			case "HK": return "\u9999\u6e2f\u7279\u5225\u884c\u653f\u533a";
			case "HU": return "\u30cf\u30f3\u30ac\u30ea\u30fc";
			case "IS": return "\u30a2\u30a4\u30b9\u30e9\u30f3\u30c9";
			case "IN": return "\u30a4\u30f3\u30c9";
			case "ID": return "\u30a4\u30f3\u30c9\u30cd\u30b7\u30a2";
			case "IR": return "\u30a4\u30e9\u30f3";
			case "IQ": return "\u30a4\u30e9\u30af";
			case "IE": return "\u30a2\u30a4\u30eb\u30e9\u30f3\u30c9";
			case "IL": return "\u30a4\u30b9\u30e9\u30a8\u30eb";
			case "IT": return "\u30a4\u30bf\u30ea\u30a2";
			case "JM": return "\u30b8\u30e3\u30de\u30a4\u30ab";
			case "JP": return "\u65e5\u672c";
			case "JO": return "\u30e8\u30eb\u30c0\u30f3";
			case "KZ": return "\u30ab\u30b6\u30d5\u30b9\u30bf\u30f3";
			case "KE": return "\u30b1\u30cb\u30a2";
			case "KI": return "\u30ad\u30ea\u30d0\u30b9";
			case "KP": return "\u671d\u9bae\u6c11\u4e3b\u4e3b\u7fa9\u4eba\u6c11\u5171\u548c\u56fd";
			case "KR": return "\u5927\u97d3\u6c11\u56fd";
			case "KW": return "\u30af\u30a6\u30a7\u30fc\u30c8";
			case "KG": return "\u30ad\u30eb\u30ae\u30b9\u30bf\u30f3";
			case "LA": return "\u30e9\u30aa\u30b9";
			case "LV": return "\u30e9\u30c8\u30d3\u30a2";
			case "LB": return "\u30ec\u30d0\u30ce\u30f3";
			case "LS": return "\u30ec\u30bd\u30c8";
			case "LR": return "\u30ea\u30d9\u30ea\u30a2";
			case "LY": return "\u30ea\u30d3\u30a2";
			case "LI": return "\u30ea\u30d2\u30c6\u30f3\u30b7\u30e5\u30bf\u30a4\u30f3";
			case "LT": return "\u30ea\u30c8\u30a2\u30cb\u30a2";
			case "LU": return "\u30eb\u30af\u30bb\u30f3\u30d6\u30eb\u30af";
			case "MK": return "\u30de\u30b1\u30c9\u30cb\u30a2";
			case "MG": return "\u30de\u30c0\u30ac\u30b9\u30ab\u30eb";
			case "MO": return "\u30de\u30ab\u30aa\u7279\u5225\u884c\u653f\u533a";
			case "MY": return "\u30de\u30ec\u30fc\u30b7\u30a2";
			case "ML": return "\u30de\u30ea";
			case "MT": return "\u30de\u30eb\u30bf";
			case "MQ": return "\u30de\u30eb\u30c6\u30a3\u30cb\u30fc\u30af\u5cf6";
			case "MR": return "\u30e2\u30fc\u30ea\u30bf\u30cb\u30a2";
			case "MU": return "\u30e2\u30fc\u30ea\u30b7\u30e3\u30b9";
			case "YT": return "\u30de\u30e8\u30c3\u30c8\u5cf6";
			case "MX": return "\u30e1\u30ad\u30b7\u30b3";
			case "FM": return "\u30df\u30af\u30ed\u30cd\u30b7\u30a2";
			case "MD": return "\u30e2\u30eb\u30c9\u30d0";
			case "MC": return "\u30e2\u30ca\u30b3";
			case "MN": return "\u30e2\u30f3\u30b4\u30eb";
			case "MS": return "\u30e2\u30f3\u30c8\u30bb\u30e9\u30c8\u5cf6";
			case "MA": return "\u30e2\u30ed\u30c3\u30b3";
			case "MZ": return "\u30e2\u30b6\u30f3\u30d3\u30fc\u30af";
			case "MM": return "\u30df\u30e3\u30f3\u30de\u30fc";
			case "NA": return "\u30ca\u30df\u30d3\u30a2";
			case "NP": return "\u30cd\u30d1\u30fc\u30eb";
			case "NL": return "\u30aa\u30e9\u30f3\u30c0";
			case "AN": return "\u30aa\u30e9\u30f3\u30c0\u9818\u30a2\u30f3\u30c6\u30a3\u30eb\u8af8\u5cf6";
			case "NC": return "\u30cb\u30e5\u30fc\u30ab\u30ec\u30c9\u30cb\u30a2";
			case "NZ": return "\u30cb\u30e5\u30fc\u30b8\u30fc\u30e9\u30f3\u30c9";
			case "NI": return "\u30cb\u30ab\u30e9\u30b0\u30a2";
			case "NE": return "\u30cb\u30b8\u30a7\u30fc\u30eb";
			case "NG": return "\u30ca\u30a4\u30b8\u30a7\u30ea\u30a2";
			case "NU": return "\u30cb\u30a6\u30a8\u5cf6";
			case "NO": return "\u30ce\u30eb\u30a6\u30a7\u30fc";
			case "OM": return "\u30aa\u30de\u30fc\u30f3";
			case "PK": return "\u30d1\u30ad\u30b9\u30bf\u30f3";
			case "PA": return "\u30d1\u30ca\u30de";
			case "PG": return "\u30d1\u30d7\u30a2\u30cb\u30e5\u30fc\u30ae\u30cb\u30a2";
			case "PY": return "\u30d1\u30e9\u30b0\u30a2\u30a4";
			case "PE": return "\u30da\u30eb\u30fc";
			case "PH": return "\u30d5\u30a3\u30ea\u30d4\u30f3";
			case "PL": return "\u30dd\u30fc\u30e9\u30f3\u30c9";
			case "PT": return "\u30dd\u30eb\u30c8\u30ac\u30eb";
			case "PR": return "\u30d7\u30a8\u30eb\u30c8\u30ea\u30b3";
			case "QA": return "\u30ab\u30bf\u30fc\u30eb";
			case "RO": return "\u30eb\u30fc\u30de\u30cb\u30a2";
			case "RU": return "\u30ed\u30b7\u30a2";
			case "RW": return "\u30eb\u30ef\u30f3\u30c0";
			case "SA": return "\u30b5\u30a6\u30b8\u30a2\u30e9\u30d3\u30a2";
			case "SN": return "\u30bb\u30cd\u30ac\u30eb";
			case "SP": return "\u30bb\u30eb\u30d3\u30a2";
			case "SC": return "\u30bb\u30a4\u30b7\u30a7\u30eb";
			case "SL": return "\u30b7\u30a8\u30e9\u30ec\u30aa\u30cd";
			case "SG": return "\u30b7\u30f3\u30ac\u30dd\u30fc\u30eb";
			case "SK": return "\u30b9\u30ed\u30d0\u30ad\u30a2";
			case "SI": return "\u30b9\u30ed\u30d9\u30cb\u30a2";
			case "SO": return "\u30bd\u30de\u30ea\u30a2";
			case "ZA": return "\u5357\u30a2\u30d5\u30ea\u30ab";
			case "ES": return "\u30b9\u30da\u30a4\u30f3";
			case "LK": return "\u30b9\u30ea\u30e9\u30f3\u30ab";
			case "SD": return "\u30b9\u30fc\u30c0\u30f3";
			case "SR": return "\u30b9\u30ea\u30ca\u30e0";
			case "SZ": return "\u30b9\u30ef\u30b8\u30e9\u30f3\u30c9";
			case "SE": return "\u30b9\u30a6\u30a7\u30fc\u30c7\u30f3";
			case "CH": return "\u30b9\u30a4\u30b9";
			case "SY": return "\u30b7\u30ea\u30a2";
			case "TW": return "\u53f0\u6e7e";
			case "TJ": return "\u30bf\u30b8\u30ad\u30b9\u30bf\u30f3";
			case "TZ": return "\u30bf\u30f3\u30b6\u30cb\u30a2";
			case "TH": return "\u30bf\u30a4";
			case "TG": return "\u30c8\u30fc\u30b4";
			case "TK": return "\u30c8\u30b1\u30e9\u30a6\u8af8\u5cf6";
			case "TO": return "\u30c8\u30f3\u30ac";
			case "TT": return "\u30c8\u30ea\u30cb\u30c0\u30fc\u30c9\u30fb\u30c8\u30d0\u30b4";
			case "TN": return "\u30c1\u30e5\u30cb\u30b8\u30a2";
			case "TR": return "\u30c8\u30eb\u30b3";
			case "TM": return "\u30c8\u30eb\u30af\u30e1\u30cb\u30b9\u30bf\u30f3";
			case "UG": return "\u30a6\u30ac\u30f3\u30c0";
			case "UA": return "\u30a6\u30af\u30e9\u30a4\u30ca";
			case "AE": return "\u30a2\u30e9\u30d6\u9996\u9577\u56fd\u9023\u90a6";
			case "GB": return "\u30a4\u30ae\u30ea\u30b9";
			case "US": return "\u30a2\u30e1\u30ea\u30ab\u5408\u8846\u56fd";
			case "UY": return "\u30a6\u30eb\u30b0\u30a2\u30a4";
			case "UZ": return "\u30a6\u30ba\u30d9\u30ad\u30b9\u30bf\u30f3";
			case "VU": return "\u30d0\u30cc\u30a2\u30c4";
			case "VA": return "\u30d0\u30c1\u30ab\u30f3";
			case "VE": return "\u30d9\u30cd\u30ba\u30a8\u30e9";
			case "VN": return "\u30d9\u30c8\u30ca\u30e0";
			case "VG": return "\u82f1\u9818\u30d0\u30fc\u30b8\u30f3\u8af8\u5cf6";
			case "VI": return "\u7c73\u9818\u30d0\u30fc\u30b8\u30f3\u8af8\u5cf6";
			case "EH": return "\u897f\u30b5\u30cf\u30e9";
			case "YE": return "\u30a4\u30a8\u30e1\u30f3";
			case "YU": return "\u30e6\u30fc\u30b4\u30b9\u30e9\u30d3\u30a2\u9023\u90a6";
			case "ZM": return "\u30b6\u30f3\u30d3\u30a2";
			case "ZW": return "\u30b8\u30f3\u30d0\u30d6\u30a8";
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
				return 932;
			}
		}
		public override int EBCDICCodePage
		{
			get
			{
				return 20290;
			}
		}
		public override int MacCodePage
		{
			get
			{
				return 10001;
			}
		}
		public override int OEMCodePage
		{
			get
			{
				return 932;
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

}; // class CID0011

public class CNja : CID0011
{
	public CNja() : base() {}

}; // class CNja

}; // namespace I18N.CJK
