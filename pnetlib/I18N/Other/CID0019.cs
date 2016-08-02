/*
 * CID0019.cs - ru culture handler.
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

// Generated from "ru.txt".

namespace I18N.Other
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0019 : RootCulture
{
	public CID0019() : base(0x0019) {}
	public CID0019(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "ru";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "rus";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "RUS";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "ru";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"\u0412\u0441", "\u041F\u043D", "\u0412\u0442", "\u0421\u0440", "\u0427\u0442", "\u041F\u0442", "\u0421\u0431"};
			dfi.DayNames = new String[] {"\u0432\u043E\u0441\u043A\u0440\u0435\u0441\u0435\u043D\u044C\u0435", "\u043F\u043E\u043D\u0435\u0434\u0435\u043B\u044C\u043D\u0438\u043A", "\u0432\u0442\u043E\u0440\u043D\u0438\u043A", "\u0441\u0440\u0435\u0434\u0430", "\u0447\u0435\u0442\u0432\u0435\u0440\u0433", "\u043F\u044F\u0442\u043D\u0438\u0446\u0430", "\u0441\u0443\u0431\u0431\u043E\u0442\u0430"};
			dfi.AbbreviatedMonthNames = new String[] {"\u044F\u043D\u0432", "\u0444\u0435\u0432", "\u043C\u0430\u0440", "\u0430\u043F\u0440", "\u043C\u0430\u0439", "\u0438\u044E\u043D", "\u0438\u044E\u043B", "\u0430\u0432\u0433", "\u0441\u0435\u043D", "\u043E\u043A\u0442", "\u043D\u043E\u044F", "\u0434\u0435\u043A", ""};
			dfi.MonthNames = new String[] {"\u042F\u043D\u0432\u0430\u0440\u044C", "\u0424\u0435\u0432\u0440\u0430\u043B\u044C", "\u041C\u0430\u0440\u0442", "\u0410\u043F\u0440\u0435\u043B\u044C", "\u041C\u0430\u0439", "\u0418\u044E\u043D\u044C", "\u0418\u044E\u043B\u044C", "\u0410\u0432\u0433\u0443\u0441\u0442", "\u0421\u0435\u043D\u0442\u044F\u0431\u0440\u044C", "\u041E\u043A\u0442\u044F\u0431\u0440\u044C", "\u041D\u043E\u044F\u0431\u0440\u044C", "\u0414\u0435\u043A\u0430\u0431\u0440\u044C", ""};
			dfi.DateSeparator = ".";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "d MMMM yyyy '\u0433.'";
			dfi.LongTimePattern = "H:mm:ss z";
			dfi.ShortDatePattern = "dd.MM.yy";
			dfi.ShortTimePattern = "H:mm";
			dfi.FullDateTimePattern = "d MMMM yyyy '\u0433.' H:mm:ss z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:dd.MM.yy",
				"D:d MMMM yyyy '\u0433.'",
				"f:d MMMM yyyy '\u0433.' H:mm:ss z",
				"f:d MMMM yyyy '\u0433.' H:mm:ss z",
				"f:d MMMM yyyy '\u0433.' H:mm:ss",
				"f:d MMMM yyyy '\u0433.' H:mm",
				"F:d MMMM yyyy '\u0433.' HH:mm:ss",
				"g:dd.MM.yy H:mm:ss z",
				"g:dd.MM.yy H:mm:ss z",
				"g:dd.MM.yy H:mm:ss",
				"g:dd.MM.yy H:mm",
				"G:dd.MM.yy HH:mm:ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:H:mm:ss z",
				"t:H:mm:ss z",
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
			case "ab": return "\u0410\u0431\u0445\u0430\u0437\u0441\u043a\u0438\u0439";
			case "aa": return "\u0410\u0444\u0430\u0440";
			case "af": return "\u0410\u0444\u0440\u0438\u043a\u0430\u0430\u043d\u0441";
			case "sq": return "\u0410\u043b\u0431\u0430\u043d\u0441\u043a\u0438\u0439";
			case "am": return "\u0410\u043c\u0445\u0430\u0440\u0441\u043a\u0438\u0439";
			case "ar": return "\u0410\u0440\u0430\u0431\u0441\u043a\u0438\u0439";
			case "hy": return "\u0410\u0440\u043c\u044f\u043d\u0441\u043a\u0438\u0439";
			case "as": return "\u0410\u0441\u0441\u0430\u043c\u0441\u043a\u0438\u0439";
			case "ay": return "\u0410\u044f\u043c\u0430\u0440\u0430";
			case "az": return "\u0410\u0437\u0435\u0440\u0431\u0430\u0439\u0434\u0436\u0430\u043d\u0441\u043a\u0438\u0439";
			case "ba": return "\u0411\u0430\u0448\u043a\u0438\u0440\u0441\u043a\u0438\u0439";
			case "eu": return "\u0411\u0430\u0441\u043a\u0441\u043a\u0438\u0439";
			case "bn": return "\u0411\u0435\u043d\u0433\u0430\u043b\u044c\u0441\u043a\u0438\u0439";
			case "dz": return "\u0411\u0443\u0442\u0430\u043d\u0441\u043a\u0438\u0439";
			case "bh": return "\u0411\u0438\u0445\u0430\u0440\u0441\u043a\u0438\u0439";
			case "bi": return "\u0411\u0438\u0441\u043b\u0430\u043c\u0430";
			case "br": return "\u0411\u0440\u0435\u0442\u043e\u043d\u0441\u043a\u0438\u0439";
			case "bg": return "\u0411\u043e\u043b\u0433\u0430\u0440\u0441\u043a\u0438\u0439";
			case "my": return "\u0411\u0438\u0440\u043c\u0430\u043d\u0441\u043a\u0438\u0439";
			case "be": return "\u0411\u0435\u043b\u043e\u0440\u0443\u0441\u0441\u043a\u0438\u0439";
			case "km": return "\u041a\u0430\u043c\u0431\u043e\u0434\u0436\u0438\u0439\u0441\u043a\u0438\u0439";
			case "ca": return "\u041a\u0430\u0442\u0430\u043b\u0430\u043d\u0441\u043a\u0438\u0439";
			case "zh": return "\u041a\u0438\u0442\u0430\u0439\u0441\u043a\u0438\u0439";
			case "co": return "\u041a\u043e\u0440\u0441\u0438\u043a\u0430\u043d\u0441\u043a\u0438\u0439";
			case "hr": return "\u0425\u043e\u0440\u0432\u0430\u0442\u0441\u043a\u0438\u0439";
			case "cs": return "\u0427\u0435\u0448\u0441\u043a\u0438\u0439";
			case "da": return "\u0414\u0430\u0442\u0441\u043a\u0438\u0439";
			case "nl": return "\u0413\u043e\u043b\u043b\u0430\u043d\u0434\u0441\u043a\u0438\u0439";
			case "en": return "\u0410\u043d\u0433\u043b\u0438\u0439\u0441\u043a\u0438\u0439";
			case "eo": return "\u042d\u0441\u043f\u0435\u0440\u0430\u043d\u0442\u043e";
			case "et": return "\u042d\u0441\u0442\u043e\u043d\u0441\u043a\u0438\u0439";
			case "fo": return "\u0424\u0430\u0440\u0435\u0440\u0441\u043a\u0438\u0439";
			case "fj": return "\u0424\u0438\u0434\u0436\u0438";
			case "fi": return "\u0424\u0438\u043d\u0441\u043a\u0438\u0439";
			case "fr": return "\u0424\u0440\u0430\u043d\u0446\u0443\u0437\u0441\u043a\u0438\u0439";
			case "fy": return "\u0424\u0440\u0438\u0437\u0441\u043a\u0438\u0439";
			case "gl": return "\u0413\u0430\u043b\u0438\u0446\u0438\u0439\u0441\u043a\u0438\u0439";
			case "ka": return "\u0413\u0440\u0443\u0437\u0438\u043d\u0441\u043a\u0438\u0439";
			case "de": return "\u041d\u0435\u043c\u0435\u0446\u043a\u0438\u0439";
			case "el": return "\u0413\u0440\u0435\u0447\u0435\u0441\u043a\u0438\u0439";
			case "kl": return "\u0413\u0440\u0435\u043d\u043b\u0430\u043d\u0434\u0441\u043a\u0438\u0439";
			case "gn": return "\u0413\u0443\u0430\u0440\u0430\u043d\u0438";
			case "gu": return "\u0413\u0443\u044f\u0440\u0430\u0442\u0438";
			case "ha": return "\u0425\u043e\u0441\u0430";
			case "he": return "\u0418\u0432\u0440\u0438\u0442";
			case "hi": return "\u0425\u0438\u043d\u0434\u0438";
			case "hu": return "\u0412\u0435\u043d\u0433\u0435\u0440\u0441\u043a\u0438\u0439";
			case "is": return "\u0418\u0441\u043b\u0430\u043d\u0434\u0441\u043a\u0438\u0439";
			case "id": return "\u0418\u043d\u0434\u043e\u043d\u0435\u0437\u0438\u0439\u0441\u043a\u0438\u0439";
			case "ia": return "\u0421\u043c\u0435\u0448\u0430\u043d\u043d\u044b\u0439 \u044f\u0437\u044b\u043a";
			case "ie": return "\u0421\u043c\u0435\u0448\u0430\u043d\u043d\u044b\u0439 \u044f\u0437\u044b\u043a";
			case "iu": return "\u0418\u043d\u0430\u043a\u0442\u0438\u0442\u0443\u0442";
			case "ik": return "\u0418\u043d\u0430\u043f\u0438\u0430\u043a";
			case "ga": return "\u0418\u0440\u043b\u0430\u043d\u0434\u0441\u043a\u0438\u0439";
			case "it": return "\u0418\u0442\u0430\u043b\u044c\u044f\u043d\u0441\u043a\u0438\u0439";
			case "ja": return "\u042f\u043f\u043e\u043d\u0441\u043a\u0438\u0439";
			case "jv": return "\u042f\u0432\u0430\u043d\u0441\u043a\u0438\u0439";
			case "kn": return "\u041a\u0430\u043d\u0430\u0434\u0430";
			case "ks": return "\u041a\u0430\u0448\u043c\u0438\u0440\u0441\u043a\u0438\u0439";
			case "kk": return "\u041a\u0430\u0437\u0430\u0445\u0441\u043a\u0438\u0439";
			case "rw": return "\u041a\u0438\u043d\u044f\u0440\u0432\u0430\u043d\u0434\u0430";
			case "ky": return "\u041a\u0438\u0440\u0433\u0438\u0437\u0441\u043a\u0438\u0439";
			case "rn": return "\u041a\u0438\u0440\u0443\u043d\u0434\u0438\u0439\u0441\u043a\u0438\u0439";
			case "ko": return "\u041a\u043e\u0440\u0435\u0439\u0441\u043a\u0438\u0439";
			case "ku": return "\u041a\u0443\u0440\u0434\u0438\u0448";
			case "lo": return "\u041b\u0430\u043e\u0441\u0441\u043a\u0438\u0439";
			case "la": return "\u041b\u0430\u0442\u0438\u043d\u0441\u043a\u0438\u0439";
			case "lv": return "\u041b\u0430\u0442\u0432\u0438\u0439\u0441\u043a\u0438\u0439";
			case "ln": return "\u041b\u0438\u043d\u0433\u0430\u043b\u0430";
			case "lt": return "\u041b\u0438\u0442\u043e\u0432\u0441\u043a\u0438\u0439";
			case "mk": return "\u041c\u0430\u043a\u0435\u0434\u043e\u043d\u0441\u043a\u0438\u0439";
			case "mg": return "\u041c\u0430\u043b\u0430\u0433\u0430\u0441\u0438\u0439\u0441\u043a\u0438\u0439";
			case "ms": return "\u041c\u0430\u043b\u0430\u0439\u0441\u043a\u0438\u0439";
			case "ml": return "\u041c\u0430\u043b\u0430\u044f\u043b\u0430\u043c";
			case "mt": return "\u041c\u0430\u043b\u044c\u0442\u0438\u0439\u0441\u043a\u0438\u0439";
			case "mi": return "\u041c\u0430\u043e\u0440\u0438";
			case "mr": return "\u041c\u0430\u0440\u0430\u0442\u0438\u0439\u0441\u043a\u0438\u0439";
			case "mo": return "\u041c\u043e\u043b\u0434\u0430\u0432\u0441\u043a\u0438\u0439";
			case "mn": return "\u041c\u043e\u043d\u0433\u043e\u043b\u044c\u0441\u043a\u0438\u0439";
			case "na": return "\u041d\u0430\u0443\u0440\u0443";
			case "ne": return "\u041d\u0435\u043f\u0430\u043b\u044c\u0441\u043a\u0438\u0439";
			case "no": return "\u041d\u043e\u0440\u0432\u0435\u0436\u0441\u043a\u0438\u0439";
			case "oc": return "\u041e\u043a\u0438\u0442\u0430\u043d";
			case "or": return "\u041e\u0440\u0438\u044f";
			case "om": return "\u041e\u0440\u043e\u043c\u043e (\u0410\u0444\u0430\u043d)";
			case "ps": return "\u041f\u0430\u0448\u0442\u043e (\u041f\u0443\u0448\u0442\u043e)";
			case "fa": return "\u041f\u0435\u0440\u0441\u0438\u0434\u0441\u043a\u0438\u0439";
			case "pl": return "\u041f\u043e\u043b\u044c\u0441\u043a\u0438\u0439";
			case "pt": return "\u041f\u043e\u0440\u0442\u0443\u0433\u0430\u043b\u044c\u0441\u043a\u0438\u0439";
			case "pa": return "\u041f\u0430\u043d\u0434\u0436\u0430\u0431\u0441\u043a\u0438\u0439";
			case "qu": return "\u041a\u0435\u0447\u0443\u0430";
			case "rm": return "\u0420\u0430\u0435\u0442\u043e-\u0440\u043e\u043c\u0430\u043d\u0441\u043a\u0438\u0439";
			case "ro": return "\u0420\u0443\u043c\u044b\u043d\u0441\u043a\u0438\u0439";
			case "ru": return "\u0440\u0443\u0441\u0441\u043A\u0438\u0439";
			case "sm": return "\u0421\u0430\u043c\u043e\u0430";
			case "sg": return "\u0421\u0430\u043d\u0433\u043e";
			case "sa": return "\u0421\u0430\u043d\u0441\u043a\u0440\u0438\u0442";
			case "gd": return "\u0413\u0430\u044d\u043b\u044c\u0441\u043a\u0438\u0439";
			case "sr": return "\u0421\u0435\u0440\u0431\u0441\u043a\u0438\u0439";
			case "sh": return "\u0421\u0435\u0440\u0431\u0441\u043a\u043e-\u0445\u043e\u0440\u0432\u0430\u0442\u0441\u043a\u0438\u0439";
			case "st": return "\u0421\u0435\u0441\u043e\u0442\u043e";
			case "tn": return "\u0421\u0435\u0442\u0441\u0432\u0430\u043d\u0430";
			case "sn": return "\u0428\u043e\u043d\u0430";
			case "sd": return "\u0421\u0438\u043d\u0434\u0438";
			case "si": return "\u0421\u0438\u043d\u0433\u0430\u043b\u044c\u0441\u043a\u0438\u0439";
			case "ss": return "\u0421\u0438\u0441\u0432\u0430\u0442\u0438";
			case "sk": return "\u0421\u043b\u043e\u0432\u0430\u0446\u043a\u0438\u0439";
			case "sl": return "\u0421\u043b\u043e\u0432\u0435\u043d\u0441\u043a\u0438\u0439";
			case "so": return "\u0421\u043e\u043c\u0430\u043b\u0438";
			case "es": return "\u0418\u0441\u043f\u0430\u043d\u0441\u043a\u0438\u0439";
			case "su": return "\u0421\u0430\u043d\u0434\u0430\u043d\u0438\u0437\u0441\u043a\u0438\u0439";
			case "sw": return "\u0421\u0443\u0430\u0445\u0438\u043b\u0438";
			case "sv": return "\u0428\u0432\u0435\u0434\u0441\u043a\u0438\u0439";
			case "tl": return "\u0422\u0430\u0433\u0430\u043b\u043e\u0433";
			case "tg": return "\u0422\u0430\u0434\u0436\u0438\u043a\u0441\u043a\u0438\u0439";
			case "ta": return "\u0422\u0430\u043c\u0438\u043b\u044c\u0441\u043a\u0438\u0439";
			case "tt": return "\u0422\u0430\u0442\u0430\u0440\u0441\u043a\u0438\u0439";
			case "te": return "\u0422\u0435\u043b\u0443\u0433\u0443";
			case "th": return "\u0422\u0430\u0439\u0441\u043a\u0438\u0439";
			case "bo": return "\u0422\u0438\u0431\u0435\u0442\u0441\u043a\u0438\u0439";
			case "ti": return "\u0422\u0438\u0433\u0440\u0438\u043d\u0438\u0430";
			case "to": return "\u0422\u043e\u043d\u0433\u0430";
			case "ts": return "\u0422\u0441\u043e\u043d\u0433\u0430";
			case "tr": return "\u0422\u0443\u0440\u0435\u0446\u043a\u0438\u0439";
			case "tk": return "\u0422\u0443\u0440\u043a\u043c\u0435\u043d\u0441\u043a\u0438\u0439";
			case "tw": return "\u0422\u0432\u0438";
			case "ug": return "\u0423\u0439\u0433\u0443\u0440\u0441\u043a\u0438\u0439";
			case "uk": return "\u0423\u043a\u0440\u0430\u0438\u043d\u0441\u043a\u0438\u0439";
			case "ur": return "\u0423\u0440\u0434\u0443";
			case "uz": return "\u0423\u0437\u0431\u0435\u043a\u0441\u043a\u0438\u0439";
			case "vi": return "\u0412\u044c\u0435\u0442\u043d\u0430\u043c\u0441\u043a\u0438\u0439";
			case "vo": return "\u0412\u043e\u043b\u0430\u043f\u0430\u043a";
			case "cy": return "\u0412\u0430\u043b\u043b\u0438\u0439\u0441\u043a\u0438\u0439";
			case "wo": return "\u0412\u043e\u043b\u043e\u0444";
			case "xh": return "\u0425\u043e\u0437\u0430";
			case "yi": return "\u0418\u0434\u0438\u0448";
			case "yo": return "\u0419\u043e\u0440\u0443\u0431\u0430";
			case "za": return "\u0417\u0443\u0430\u043d\u0433";
			case "zu": return "\u0417\u0443\u043b\u0443\u0441\u0441\u043a\u0438\u0439";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "AL": return "\u0410\u043b\u0431\u0430\u043d\u0438\u044f";
			case "AS": return "\u0410\u0437\u0438\u044f";
			case "AT": return "\u0410\u0432\u0441\u0442\u0440\u0438\u044f";
			case "AU": return "\u0410\u0432\u0441\u0442\u0440\u0430\u043b\u0438\u044f";
			case "BA": return "\u0411\u043e\u0441\u043d\u0438\u044f";
			case "BE": return "\u0411\u0435\u043b\u044c\u0433\u0438\u044f";
			case "BG": return "\u0411\u043e\u043b\u0433\u0430\u0440\u0438\u044f";
			case "BR": return "\u0411\u0440\u0430\u0437\u0438\u043b\u0438\u044f";
			case "CA": return "\u041a\u0430\u043d\u0430\u0434\u0430";
			case "CH": return "\u0428\u0432\u0435\u0439\u0446\u0430\u0440\u0438\u044f";
			case "CN": return "\u041a\u0438\u0442\u0430\u0439 (\u041a\u041d\u0420)";
			case "CZ": return "\u0427\u0435\u0445\u0438\u044f";
			case "DE": return "\u0413\u0435\u0440\u043c\u0430\u043d\u0438\u044f";
			case "DK": return "\u0414\u0430\u043d\u0438\u044f";
			case "EE": return "\u042d\u0441\u0442\u043e\u043d\u0438\u044f";
			case "ES": return "\u0418\u0441\u043f\u0430\u043d\u0438\u044f";
			case "FI": return "\u0424\u0438\u043d\u043b\u044f\u043d\u0434\u0438\u044f";
			case "FR": return "\u0424\u0440\u0430\u043d\u0446\u0438\u044f";
			case "GB": return "\u0412\u0435\u043b\u0438\u043a\u043e\u0431\u0440\u0438\u0442\u0430\u043d\u0438\u044f";
			case "GR": return "\u0413\u0440\u0435\u0446\u0438\u044f";
			case "HR": return "\u0425\u043e\u0440\u0432\u0430\u0442\u0438\u044f";
			case "HU": return "\u0412\u0435\u043d\u0433\u0440\u0438\u044f";
			case "IE": return "\u0418\u0440\u043b\u0430\u043d\u0434\u0438\u044f";
			case "IL": return "\u255a\u0442\u0401\u0448\u0404";
			case "IS": return "\u0418\u0441\u043b\u0430\u043d\u0434\u0438\u044f";
			case "IT": return "\u0418\u0442\u0430\u043b\u0438\u044f";
			case "JP": return "\u042f\u043f\u043e\u043d\u0438\u044f";
			case "KR": return "\u041a\u043e\u0440\u0435\u044f";
			case "LA": return "\u041b\u0430\u0442\u0438\u043d\u0441\u043a\u0430\u044f \u0410\u043c\u0435\u0440\u0438\u043a\u0430";
			case "LT": return "\u041b\u0438\u0442\u0432\u0430";
			case "LV": return "\u041b\u0430\u0442\u0432\u0438\u044f";
			case "MK": return "\u041c\u0430\u043a\u0435\u0434\u043e\u043d\u0438\u044f";
			case "NL": return "\u041d\u0438\u0434\u0435\u0440\u043b\u0430\u043d\u0434\u044b";
			case "NO": return "\u041d\u043e\u0440\u0432\u0435\u0433\u0438\u044f";
			case "NZ": return "\u041d\u043e\u0432\u0430\u044f \u0417\u0435\u043b\u0430\u043d\u0434\u0438\u044f";
			case "PL": return "\u041f\u043e\u043b\u044c\u0448\u0430";
			case "PT": return "\u041f\u043e\u0440\u0442\u0443\u0433\u0430\u043b\u0438\u044f";
			case "RO": return "\u0420\u0443\u043c\u044b\u043d\u0438\u044f";
			case "RU": return "\u0420\u043e\u0441\u0441\u0438\u044f";
			case "SE": return "\u0428\u0432\u0435\u0446\u0438\u044f";
			case "SI": return "\u0421\u043b\u043e\u0432\u0435\u043d\u0438\u044f";
			case "SK": return "\u0421\u043b\u043e\u0432\u0430\u043a\u0438\u044f";
			case "SP": return "\u0421\u0435\u0440\u0431\u0438\u044f";
			case "TH": return "\u0422\u0430\u0438\u043b\u0430\u043d\u0434";
			case "TR": return "\u0422\u0443\u0440\u0446\u0438\u044f";
			case "TW": return "\u0422\u0430\u0439\u0432\u0430\u043d\u044c";
			case "UA": return "\u0423\u043A\u0440\u0430\u0438\u043D\u0430";
			case "US": return "\u0421\u0428\u0410";
			case "ZA": return "\u042e\u0410\u0420";
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
				return 20880;
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

}; // class CID0019

public class CNru : CID0019
{
	public CNru() : base() {}

}; // class CNru

}; // namespace I18N.Other
