/*
 * CID0012.cs - ko culture handler.
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

// Generated from "ko.txt".

namespace I18N.CJK
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0012 : RootCulture
{
	public CID0012() : base(0x0012) {}
	public CID0012(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "ko";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "kor";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "KOR";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "ko";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AMDesignator = "\uC624\uC804";
			dfi.PMDesignator = "\uC624\uD6C4";
			dfi.AbbreviatedDayNames = new String[] {"\uC77C", "\uC6D4", "\uD654", "\uC218", "\uBAA9", "\uAE08", "\uD1A0"};
			dfi.DayNames = new String[] {"\uC77C\uC694\uC77C", "\uC6D4\uC694\uC77C", "\uD654\uC694\uC77C", "\uC218\uC694\uC77C", "\uBAA9\uC694\uC77C", "\uAE08\uC694\uC77C", "\uD1A0\uC694\uC77C"};
			dfi.AbbreviatedMonthNames = new String[] {"1\uC6D4", "2\uC6D4", "3\uC6D4", "4\uC6D4", "5\uC6D4", "6\uC6D4", "7\uC6D4", "8\uC6D4", "9\uC6D4", "10\uC6D4", "11\uC6D4", "12\uC6D4", ""};
			dfi.MonthNames = new String[] {"1\uC6D4", "2\uC6D4", "3\uC6D4", "4\uC6D4", "5\uC6D4", "6\uC6D4", "7\uC6D4", "8\uC6D4", "9\uC6D4", "10\uC6D4", "11\uC6D4", "12\uC6D4", ""};
			dfi.DateSeparator = "-";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "yyyy'\uB144' M'\uC6D4' d'\uC77C' dd";
			dfi.LongTimePattern = "tt hh'\uC2DC'mm'\uBD84'ss'\uCD08'";
			dfi.ShortDatePattern = "yy-MM-dd";
			dfi.ShortTimePattern = "tt h:mm";
			dfi.FullDateTimePattern = "yyyy'\uB144' M'\uC6D4' d'\uC77C' dddd tt hh'\uC2DC'mm'\uBD84'ss'\uCD08' z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:yy-MM-dd",
				"D:yyyy'\uB144' M'\uC6D4' d'\uC77C' dddd",
				"f:yyyy'\uB144' M'\uC6D4' d'\uC77C' dddd tt hh'\uC2DC'mm'\uBD84'ss'\uCD08' z",
				"f:yyyy'\uB144' M'\uC6D4' d'\uC77C' dddd tt hh'\uC2DC'mm'\uBD84'ss'\uCD08'",
				"f:yyyy'\uB144' M'\uC6D4' d'\uC77C' dddd tt h:mm:ss",
				"f:yyyy'\uB144' M'\uC6D4' d'\uC77C' dddd tt h:mm",
				"F:yyyy'\uB144' M'\uC6D4' d'\uC77C' dddd HH:mm:ss",
				"g:yy-MM-dd tt hh'\uC2DC'mm'\uBD84'ss'\uCD08' z",
				"g:yy-MM-dd tt hh'\uC2DC'mm'\uBD84'ss'\uCD08'",
				"g:yy-MM-dd tt h:mm:ss",
				"g:yy-MM-dd tt h:mm",
				"G:yy-MM-dd HH:mm:ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:tt hh'\uC2DC'mm'\uBD84'ss'\uCD08' z",
				"t:tt hh'\uC2DC'mm'\uBD84'ss'\uCD08'",
				"t:tt h:mm:ss",
				"t:tt h:mm",
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
			case "ab": return "\uc555\uce74\uc988\uc5b4";
			case "aa": return "\uc544\ud30c\ub974\uc5b4";
			case "af": return "\ub0a8\uc544\uacf5 \uacf5\uc6a9\uc5b4";
			case "sq": return "\uc54c\ubc14\ub2c8\uc544\uc5b4";
			case "am": return "\uc554\ud558\ub77c\uc5b4";
			case "ar": return "\uc544\ub78d\uc5b4";
			case "hy": return "\uc544\ub974\uba54\ub2c8\uc544\uc5b4";
			case "as": return "\uc544\uc0d8\uc5b4";
			case "ay": return "\uc544\uc774\ub9c8\ub77c\uc5b4";
			case "az": return "\uc544\uc81c\ub974\ubc14\uc774\uc794\uc5b4";
			case "ba": return "\ubc14\uc288\ud0a4\ub974\uc5b4";
			case "eu": return "\ubc14\uc2a4\ud06c\uc5b4";
			case "bn": return "\ubcb5\uace8\uc5b4";
			case "dz": return "\ubd80\ud0c4\uc5b4";
			case "bh": return "\ube44\ud558\ub974\uc5b4";
			case "bi": return "\ube44\uc2ac\ub77c\ub9c8\uc5b4";
			case "br": return "\ube0c\ub974\ud0c0\ub274\uc5b4";
			case "bg": return "\ubd88\uac00\ub9ac\uc544\uc5b4";
			case "my": return "\ubc84\ub9c8\uc5b4";
			case "be": return "\ubca8\ub85c\ub8e8\uc2dc\uc5b4";
			case "km": return "\uce84\ubcf4\ub514\uc544\uc5b4";
			case "ca": return "\uce74\ud0c8\ub85c\ub2c8\uc544\uc5b4";
			case "zh": return "\uc911\uad6d\uc5b4";
			case "co": return "\ucf54\ub974\uc2dc\uce74\uc5b4";
			case "hr": return "\ud06c\ub85c\uc544\ud2f0\uc544\uc5b4";
			case "cs": return "\uccb4\ucf54\uc5b4";
			case "da": return "\ub374\ub9c8\ud06c\uc5b4";
			case "nl": return "\ub124\ub35c\ub780\ub4dc\uc5b4";
			case "en": return "\uc601\uc5b4";
			case "eo": return "\uc5d0\uc2a4\ud398\ub780\ud1a0\uc5b4";
			case "et": return "\uc5d0\uc2a4\ud1a0\ub2c8\uc544\uc5b4";
			case "fo": return "\ud398\ub85c\uc2a4\uc5b4";
			case "fj": return "\ud53c\uc9c0\uc5b4";
			case "fi": return "\ud540\ub780\ub4dc\uc5b4";
			case "fr": return "\ud504\ub791\uc2a4\uc5b4";
			case "fy": return "\ud504\ub9ac\uc9c0\uc544\uc5b4";
			case "gl": return "\uac08\ub9ac\uc2dc\uc544\uc5b4";
			case "ka": return "\uadf8\ub8e8\uc9c0\uc57c\uc5b4";
			case "de": return "\ub3c5\uc77c\uc5b4";
			case "el": return "\uadf8\ub9ac\uc2a4\uc5b4";
			case "kl": return "\uadf8\ub9b0\ub79c\ub4dc\uc5b4";
			case "gn": return "\uad6c\uc544\ub77c\ub2c8\uc5b4";
			case "gu": return "\uad6c\uc790\ub77c\ud2b8\uc5b4";
			case "ha": return "\ud558\uc6b0\uc790\uc5b4";
			case "he": return "\ud788\ube0c\ub9ac\uc5b4";
			case "hi": return "\ud78c\ub514\uc5b4";
			case "hu": return "\ud5dd\uac00\ub9ac\uc5b4";
			case "is": return "\uc544\uc774\uc2ac\ub780\ub4dc\uc5b4";
			case "id": return "\uc778\ub3c4\ub124\uc2dc\uc544\uc5b4";
			case "ia": return "\uc778\ud130\ub9c1\uac70";
			case "ie": return "\uc778\ud130\ub9c1\uac8c\uc5b4";
			case "iu": return "\uc774\ub205\ud2f0\ud22c\ud2b8\uc5b4";
			case "ik": return "\uc774\ub204\ud53c\uc544\ud06c\uc5b4";
			case "ga": return "\uc544\uc77c\ub79c\ub4dc\uc5b4";
			case "it": return "\uc774\ud0c8\ub9ac\uc544\uc5b4";
			case "ja": return "\uc77c\ubcf8\uc5b4";
			case "jv": return "\uc790\ubc14\uc5b4";
			case "kn": return "\uce74\ub098\ub2e4\uc5b4";
			case "ks": return "\uce74\uc288\ubbf8\ub974\uc5b4";
			case "kk": return "\uce74\uc790\ud750\uc5b4";
			case "rw": return "\ubc18\ud22c\uc5b4(\ub8e8\uc644\ub2e4)";
			case "ky": return "\ud0a4\ub974\uae30\uc2a4\uc5b4";
			case "rn": return "\ubc18\ud22c\uc5b4(\ubd80\ub8ec\ub514)";
			case "ko": return "\ud55c\uad6d\uc5b4";
			case "ku": return "\ud06c\ub974\ub4dc\uc5b4";
			case "lo": return "\ub77c\uc624\uc5b4";
			case "la": return "\ub77c\ud2f4\uc5b4";
			case "lv": return "\ub77c\ud2b8\ube44\uc544\uc5b4 (\ub808\ud2b8\uc5b4)";
			case "ln": return "\ub9c1\uac08\ub77c\uc5b4";
			case "lt": return "\ub9ac\ud22c\uc544\ub2c8\uc544\uc5b4";
			case "mk": return "\ub9c8\ucf00\ub3c4\ub2c8\uc544\uc5b4";
			case "mg": return "\ub9c8\ub2e4\uac00\uc2a4\uce74\ub974\uc5b4";
			case "ms": return "\ub9d0\ub808\uc774\uc5b4";
			case "ml": return "\ub9d0\ub77c\uc584\ub78c\uc5b4";
			case "mt": return "\ubab0\ud0c0\uc5b4";
			case "mi": return "\ub9c8\uc624\ub9ac\uc5b4";
			case "mr": return "\ub9c8\ub77c\ud2f0\uc5b4";
			case "mo": return "\ubab0\ub2e4\ube44\uc544\uc5b4";
			case "mn": return "\ubabd\uace8\uc5b4";
			case "na": return "\ub098\uc6b0\ub8e8\uc5b4";
			case "ne": return "\ub124\ud314\uc5b4";
			case "no": return "\ub178\ub974\uc6e8\uc774\uc5b4";
			case "oc": return "\uc625\uc2dc\ud2b8\uc5b4";
			case "or": return "\uc624\ub9ac\uc57c\uc5b4";
			case "om": return "\uc624\ub85c\ubaa8\uc5b4 (\uc544\ud310)";
			case "ps": return "\ud30c\uc2dc\ud1a0\uc5b4 (\ud478\uc2dc\ud1a0)";
			case "fa": return "\uc774\ub780\uc5b4";
			case "pl": return "\ud3f4\ub780\ub4dc\uc5b4";
			case "pt": return "\ud3ec\ub974\ud22c\uce7c\uc5b4";
			case "pa": return "\ud380\uc7a1\uc5b4";
			case "qu": return "\ucf00\ucd94\uc544\uc5b4";
			case "rm": return "\ub808\ud1a0\ub85c\ub9cc\uc5b4";
			case "ro": return "\ub8e8\ub9c8\ub2c8\uc544\uc5b4";
			case "ru": return "\ub7ec\uc2dc\uc544\uc5b4";
			case "sm": return "\uc0ac\ubaa8\uc544\uc5b4";
			case "sg": return "\uc0b0\uace0\uc5b4";
			case "sa": return "\uc0b0\uc2a4\ud06c\ub9ac\ud2b8\uc5b4";
			case "gd": return "\uc2a4\ucf54\uac24\ub9ad\uc5b4";
			case "sr": return "\uc138\ub974\ube44\uc544\uc5b4";
			case "sh": return "\uc138\ub974\ubcf4\ud06c\ub85c\uc544\ud2f0\uc544\uc5b4";
			case "st": return "\uc138\uc18c\ud1a0\uc5b4";
			case "tn": return "\uc138\uce20\uc640\ub098\uc5b4";
			case "sn": return "\uc1fc\ub098\uc5b4";
			case "sd": return "\uc2e0\ub514\uc5b4";
			case "si": return "\uc2a4\ub9ac\ub791\uce74\uc5b4";
			case "ss": return "\uc2dc\uc2a4\uc640\ud2f0\uc5b4";
			case "sk": return "\uc2ac\ub85c\ubc14\ud0a4\uc544\uc5b4";
			case "sl": return "\uc2ac\ub85c\ubca0\ub2c8\uc544\uc5b4";
			case "so": return "\uc18c\ub9d0\ub9ac\uc544\uc5b4";
			case "es": return "\uc2a4\ud398\uc778\uc5b4";
			case "su": return "\uc21c\ub2e8\uc5b4";
			case "sw": return "\uc2a4\uc640\ud790\ub9ac\uc5b4";
			case "sv": return "\uc2a4\uc6e8\ub374\uc5b4";
			case "tl": return "\ud0c0\uac08\ub85c\uadf8\uc5b4";
			case "tg": return "\ud0c0\uc9c0\ud0a4\uc2a4\ud0c4\uc5b4";
			case "ta": return "\ud0c0\ubc00\uc5b4";
			case "tt": return "\ud0c0\ud0c0\ub974\uc5b4";
			case "te": return "\ud154\ub8e8\uad6c\uc5b4";
			case "th": return "\ud0dc\uad6d\uc5b4";
			case "bo": return "\ud2f0\ubca0\ud2b8\uc5b4";
			case "ti": return "\ud2f0\uadf8\ub9ac\ub0d0\uc5b4";
			case "to": return "\ud1b5\uac00\uc5b4";
			case "ts": return "\ud1b5\uac00\uc5b4";
			case "tr": return "\ud130\ud0a4\uc5b4";
			case "tk": return "\ud22c\ub974\ud06c\uba58\uc5b4";
			case "tw": return "\ud2b8\uc704\uc5b4";
			case "ug": return "\uc704\uad6c\ub974\uc5b4";
			case "uk": return "\uc6b0\ud06c\ub77c\uc774\ub098\uc5b4";
			case "ur": return "\uc6b0\ub974\ub450\uc5b4";
			case "uz": return "\uc6b0\uc988\ubca0\ud06c\uc5b4";
			case "vi": return "\ubca0\ud2b8\ub0a8\uc5b4";
			case "vo": return "\ubcfc\ub77c\ud4cc\ud06c\uc5b4";
			case "cy": return "\uc6e8\uc77c\uc2a4\uc5b4";
			case "wo": return "\uc62c\ub85c\ud504\uc5b4";
			case "xh": return "\ubc18\ud22c\uc5b4(\ub0a8\uc544\ud504\ub9ac\uce74)";
			case "yi": return "\uc774\ub514\uc2dc\uc5b4";
			case "yo": return "\uc694\ub8e8\ubc14\uc5b4";
			case "za": return "\uc8fc\uc559\uc5b4";
			case "zu": return "\uc904\ub8e8\uc5b4";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "AF": return "\uc544\ud504\uac00\ub2c8\uc2a4\ud0c4";
			case "AL": return "\uc54c\ubc14\ub2c8\uc544";
			case "DZ": return "\uc54c\uc81c\ub9ac";
			case "AD": return "\uc548\ub3c4\ub77c";
			case "AO": return "\uc559\uace8\ub77c";
			case "AI": return "\uc548\uae38\ub77c";
			case "AR": return "\uc544\ub974\ud5e8\ud2f0\ub098";
			case "AM": return "\uc544\ub974\uba54\ub2c8\uc544";
			case "AW": return "\uc544\ub8e8\ubc14";
			case "AU": return "\uc624\uc2a4\ud2b8\ub808\uc77c\ub9ac\uc544";
			case "AT": return "\uc624\uc2a4\ud2b8\ub9ac\uc544";
			case "AZ": return "\uc544\uc81c\ub974\ubc14\uc774\uc794";
			case "BS": return "\ubc14\ud558\ub9c8";
			case "BH": return "\ubc14\ub808\uc778";
			case "BD": return "\ubc29\uae00\ub77c\ub370\uc2dc";
			case "BB": return "\ubc14\ubca0\uc774\ub3c4\uc2a4";
			case "BY": return "\ubca8\ub77c\ub8e8\uc2a4";
			case "BE": return "\ubca8\uae30\uc5d0";
			case "BZ": return "\ubca8\ub9ac\uc988";
			case "BJ": return "\ubca0\ub139";
			case "BM": return "\ubc84\ubba4\ub2e4";
			case "BT": return "\ubd80\ud0c4";
			case "BO": return "\ubcfc\ub9ac\ube44\uc544";
			case "BA": return "\ubcf4\uc2a4\ub2c8\uc544 \ud5e4\ub974\uccb4\uace0\ube44\ub098";
			case "BW": return "\ubcf4\uce20\uc640\ub098";
			case "BR": return "\ube0c\ub77c\uc9c8";
			case "BN": return "\ube0c\ub8e8\ub098\uc774";
			case "BG": return "\ubd88\uac00\ub9ac\uc544";
			case "BF": return "\ubd80\ub974\ud0a4\ub098\ud30c\uc18c";
			case "BI": return "\ubd80\ub8ec\ub514";
			case "KH": return "\uce84\ubcf4\ub514\uc544";
			case "CM": return "\uce74\uba54\ub8ec";
			case "CA": return "\uce90\ub098\ub2e4";
			case "CV": return "\uae4c\ubf40\ubca0\ub974\ub370";
			case "CF": return "\uc911\uc559 \uc544\ud504\ub9ac\uce74";
			case "TD": return "\ucc28\ub4dc";
			case "CL": return "\uce60\ub808";
			case "CN": return "\uc911\uad6d";
			case "CO": return "\ucf5c\ub86c\ube44\uc544";
			case "KM": return "\ucf54\ubaa8\ub974";
			case "CG": return "\ucf69\uace0";
			case "CR": return "\ucf54\uc2a4\ud0c0\ub9ac\uce74";
			case "CI": return "\ucf54\ud2b8\ub514\ubd80\uc640\ub974";
			case "HR": return "\ud06c\ub85c\uc544\ud2f0\uc544";
			case "CU": return "\ucfe0\ubc14";
			case "CY": return "\uc0ac\uc774\ud504\ub7ec\uc2a4";
			case "CZ": return "\uccb4\ucf54";
			case "DK": return "\ub374\ub9c8\ud06c";
			case "DJ": return "\uc9c0\ubd80\ud2f0";
			case "DM": return "\ub3c4\ubbf8\ub2c8\uce74";
			case "DO": return "\ub3c4\ubbf8\ub2c8\uce74 \uacf5\ud654\uad6d";
			case "TL": return "\ub3d9\ud2f0\ubaa8\ub974";
			case "EC": return "\uc5d0\ucfe0\uc544\ub3c4\ub974";
			case "EG": return "\uc774\uc9d1\ud2b8";
			case "SV": return "\uc5d8\uc0b4\ubc14\ub3c4\ub974";
			case "GQ": return "\uc801\ub3c4 \uae30\ub2c8";
			case "ER": return "\uc5d0\ub9ac\ud2b8\ub9ac\uc544";
			case "EE": return "\uc5d0\uc2a4\ud1a0\ub2c8\uc544";
			case "ET": return "\uc774\ub514\uc624\ud53c\uc544";
			case "FJ": return "\ud53c\uc9c0";
			case "FI": return "\ud540\ub780\ub4dc";
			case "FR": return "\ud504\ub791\uc2a4";
			case "GF": return "\ud504\ub791\uc2a4\ub839 \uae30\uc544\ub098";
			case "PF": return "\ud504\ub791\uc2a4\ub839 \ud3f4\ub9ac\ub124\uc2dc\uc544";
			case "TF": return "\ud504\ub791\uc2a4 \ub0a8\ubd80 \uc9c0\ubc29";
			case "GA": return "\uac00\ubd09";
			case "GM": return "\uac10\ube44\uc544";
			case "GE": return "\uadf8\ub8e8\uc9c0\uc57c";
			case "DE": return "\ub3c5\uc77c";
			case "GH": return "\uac00\ub098";
			case "GR": return "\uadf8\ub9ac\uc2a4";
			case "GP": return "\uacfc\ub2ec\ub85c\ud504";
			case "GT": return "\uacfc\ud14c\ub9d0\ub77c";
			case "GN": return "\uae30\ub2c8";
			case "GW": return "\uae30\ub124\ube44\uc3d8";
			case "GY": return "\uac00\uc774\uc544\ub098";
			case "HT": return "\ud558\uc774\ud2f0";
			case "HN": return "\uc628\ub450\ub77c\uc2a4";
			case "HK": return "\ud64d\ucf69 S.A.R.";
			case "HU": return "\ud5dd\uac00\ub9ac";
			case "IS": return "\uc544\uc774\uc2ac\ub780\ub4dc";
			case "IN": return "\uc778\ub3c4";
			case "ID": return "\uc778\ub3c4\ub124\uc2dc\uc544";
			case "IR": return "\uc774\ub780";
			case "IQ": return "\uc774\ub77c\ud06c";
			case "IE": return "\uc544\uc77c\ub79c\ub4dc";
			case "IL": return "\uc774\uc2a4\ub77c\uc5d8";
			case "IT": return "\uc774\ud0c8\ub9ac\uc544";
			case "JM": return "\uc790\uba54\uc774\uce74";
			case "JP": return "\uc77c\ubcf8";
			case "JO": return "\uc694\ub974\ub2e8";
			case "KZ": return "\uce74\uc790\ud750\uc2a4\ud0c4";
			case "KE": return "\ucf00\ub0d0";
			case "KI": return "\ud0a4\ub9ac\ubc14\uc2dc";
			case "KP": return "\uC870\uC120 \uBBFC\uC8FC\uC8FC\uC758 \uC778\uBBFC \uACF5\uD654\uAD6D";
			case "KR": return "\ub300\ud55c\ubbfc\uad6d";
			case "KW": return "\ucfe0\uc6e8\uc774\ud2b8";
			case "KG": return "\ud0a4\ub974\uae30\uc2a4\uc2a4\ud0c4";
			case "LA": return "\ub77c\uc624\uc2a4";
			case "LV": return "\ub77c\ud2b8\ube44\uc544";
			case "LB": return "\ub808\ubc14\ub17c";
			case "LS": return "\ub808\uc18c\ud1a0";
			case "LR": return "\ub77c\uc774\ubca0\ub9ac\uc544";
			case "LY": return "\ub9ac\ube44\uc544";
			case "LI": return "\ub9ac\ud788\ud150\uc288\ud0c0\uc778";
			case "LT": return "\ub9ac\ud22c\uc544\ub2c8\uc544";
			case "LU": return "\ub8e9\uc148\ubd80\ub974\ud06c";
			case "MK": return "\ub9c8\ucf00\ub3c4\ub2c8\uc544\uc5b4";
			case "MG": return "\ub9c8\ub2e4\uac00\uc2a4\uce74\ub974";
			case "MO": return "\ub9c8\uce74\uc624 S.A.R.";
			case "MY": return "\ub9d0\ub808\uc774\uc9c0\uc544";
			case "ML": return "\ub9d0\ub9ac";
			case "MT": return "\ubab0\ud0c0";
			case "MQ": return "\ub9d0\ud2f0\ub2c8\ud06c";
			case "MR": return "\ubaa8\ub9ac\ud0c0\ub2c8";
			case "MU": return "\ubaa8\ub9ac\uc154\uc2a4";
			case "YT": return "\ub9c8\uc694\ud2f0";
			case "MX": return "\uba55\uc2dc\ucf54";
			case "FM": return "\ub9c8\uc774\ud06c\ub85c\ub124\uc2dc\uc544";
			case "MD": return "\ubab0\ub3c4\ubc14";
			case "MC": return "\ubaa8\ub098\ucf54";
			case "MN": return "\ubabd\uace8";
			case "MS": return "\ubaac\ud2b8\uc138\ub77c\ud2b8";
			case "MA": return "\ubaa8\ub85c\ucf54";
			case "MZ": return "\ubaa8\uc7a0\ube44\ud06c";
			case "MM": return "\ubbf8\uc580\ub9c8";
			case "NA": return "\ub098\ubbf8\ube44\uc544";
			case "NP": return "\ub124\ud314";
			case "NL": return "\ub124\ub35c\ub780\ub4dc";
			case "AN": return "\ub124\ub35c\ub780\ub4dc\ub839 \uc548\ud2f8\ub808\uc2a4";
			case "NC": return "\ub274 \uce7c\ub808\ub3c4\ub2c8\uc544";
			case "NZ": return "\ub274\uc9c8\ub79c\ub4dc";
			case "NI": return "\ub2c8\uce74\ub77c\uacfc";
			case "NE": return "\ub2c8\uc81c\ub974";
			case "NG": return "\ub098\uc774\uc9c0\ub9ac\uc544";
			case "NU": return "\ub2c8\uc6b0\uc5d0";
			case "NO": return "\ub178\ub974\uc6e8\uc774";
			case "OM": return "\uc624\ub9cc";
			case "PK": return "\ud30c\ud0a4\uc2a4\ud0c4";
			case "PA": return "\ud30c\ub098\ub9c8";
			case "PG": return "\ud30c\ud478\uc544\ub274\uae30\ub2c8";
			case "PY": return "\ud30c\ub77c\uacfc\uc774";
			case "PE": return "\ud398\ub8e8";
			case "PH": return "\ud544\ub9ac\ud540";
			case "PL": return "\ud3f4\ub780\ub4dc";
			case "PT": return "\ud3ec\ub974\ud2b8\uce7c";
			case "PR": return "\ud478\uc5d0\ub974\ud1a0\ub9ac\ucf54";
			case "QA": return "\uce74\ud0c0\ub974";
			case "RO": return "\ub8e8\ub9c8\ub2c8\uc544";
			case "RU": return "\ub7ec\uc2dc\uc544";
			case "RW": return "\ub974\uc644\ub2e4";
			case "SA": return "\uc0ac\uc6b0\ub514\uc544\ub77c\ube44\uc544";
			case "SN": return "\uc138\ub124\uac08";
			case "SP": return "\uc138\ub974\ube44\uc544";
			case "SC": return "\uc250\uc774\uc258";
			case "SL": return "\uc2dc\uc5d0\ub77c\ub9ac\uc628";
			case "SG": return "\uc2f1\uac00\ud3ec\ub974";
			case "SK": return "\uc2ac\ub85c\ubc14\ud0a4\uc544";
			case "SI": return "\uc2ac\ub85c\ubca0\ub2c8\uc544";
			case "SO": return "\uc18c\ub9d0\ub9ac\uc544";
			case "ZA": return "\ub0a8\uc544\ud504\ub9ac\uce74";
			case "ES": return "\uc2a4\ud398\uc778";
			case "LK": return "\uc2a4\ub9ac\ub791\uce74";
			case "SD": return "\uc218\ub2e8";
			case "SR": return "\uc218\ub9ac\ub0a8";
			case "SZ": return "\uc2a4\uc640\uc9c8\ub79c\ub4dc";
			case "SE": return "\uc2a4\uc6e8\ub374";
			case "CH": return "\uc2a4\uc704\uc2a4";
			case "SY": return "\uc2dc\ub9ac\uc544";
			case "TW": return "\ub300\ub9cc";
			case "TJ": return "\ud0c0\uc9c0\ud0a4\uc2a4\ud0c4";
			case "TZ": return "\ud0c4\uc790\ub2c8\uc544";
			case "TH": return "\ud0dc\uad6d";
			case "TG": return "\ud1a0\uace0";
			case "TK": return "\ud1a0\ucf08\ub77c\uc6b0";
			case "TO": return "\ud1b5\uac00";
			case "TT": return "\ud2b8\ub9ac\ub2c8\ub2e4\ub4dc \ud1a0\ubc14\uace0";
			case "TN": return "\ud280\ub2c8\uc9c0";
			case "TR": return "\ud130\ud0a4";
			case "TM": return "\ud22c\ub974\ud06c\uba54\ub2c8\uc2a4\ud0c4";
			case "UG": return "\uc6b0\uac04\ub2e4";
			case "UA": return "\uc6b0\ud06c\ub77c\uc774\ub098";
			case "AE": return "\uc544\ub78d\uc5d0\ubbf8\ub9ac\ud2b8";
			case "GB": return "\uc601\uad6d";
			case "US": return "\ubbf8\uad6d";
			case "UY": return "\uc6b0\ub8e8\uacfc\uc774";
			case "UZ": return "\uc6b0\uc988\ubca0\ud0a4\uc2a4\ud0c4";
			case "VU": return "\ubc14\ub204\uc544\ud22c";
			case "VA": return "\ubc14\ud2f0\uce78";
			case "VE": return "\ubca0\ub124\uc218\uc5d8\ub77c";
			case "VN": return "\ubca0\ud2b8\ub0a8";
			case "VG": return "\uc601\uad6d\ub839 \ubc84\uc9c4 \uc544\uc77c\ub79c\ub4dc";
			case "VI": return "\ubbf8\uad6d\ub839 \ubc84\uc9c4 \uc544\uc77c\ub79c\ub4dc";
			case "EH": return "\uc11c\uc0ac\ud558\ub77c";
			case "YE": return "\uc608\uba58";
			case "YU": return "\uc720\uace0\uc2ac\ub77c\ube44\uc544";
			case "ZM": return "\uc7a0\ube44\uc544";
			case "ZW": return "\uc9d0\ubc14\ube0c\uc6e8";
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
				return 949;
			}
		}
		public override int EBCDICCodePage
		{
			get
			{
				return 20833;
			}
		}
		public override int MacCodePage
		{
			get
			{
				return 10003;
			}
		}
		public override int OEMCodePage
		{
			get
			{
				return 949;
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

}; // class CID0012

public class CNko : CID0012
{
	public CNko() : base() {}

}; // class CNko

}; // namespace I18N.CJK
