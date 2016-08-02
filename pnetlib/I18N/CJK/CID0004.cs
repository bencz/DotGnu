/*
 * CID0004.cs - zh culture handler.
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

// Generated from "zh.txt".

namespace I18N.CJK
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0004 : RootCulture
{
	public CID0004() : base(0x0004) {}
	public CID0004(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "zh";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "zho";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "CHS";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "zh";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AMDesignator = "\u4E0A\u5348";
			dfi.PMDesignator = "\u4E0B\u5348";
			dfi.AbbreviatedDayNames = new String[] {"\u661F\u671F\u65E5", "\u661F\u671F\u4E00", "\u661F\u671F\u4E8C", "\u661F\u671F\u4E09", "\u661F\u671F\u56DB", "\u661F\u671F\u4E94", "\u661F\u671F\u516D"};
			dfi.DayNames = new String[] {"\u661F\u671F\u65E5", "\u661F\u671F\u4E00", "\u661F\u671F\u4E8C", "\u661F\u671F\u4E09", "\u661F\u671F\u56DB", "\u661F\u671F\u4E94", "\u661F\u671F\u516D"};
			dfi.AbbreviatedMonthNames = new String[] {"\u4E00\u6708", "\u4E8C\u6708", "\u4E09\u6708", "\u56DB\u6708", "\u4E94\u6708", "\u516D\u6708", "\u4E03\u6708", "\u516B\u6708", "\u4E5D\u6708", "\u5341\u6708", "\u5341\u4E00\u6708", "\u5341\u4E8C\u6708", ""};
			dfi.MonthNames = new String[] {"\u4E00\u6708", "\u4E8C\u6708", "\u4E09\u6708", "\u56DB\u6708", "\u4E94\u6708", "\u516D\u6708", "\u4E03\u6708", "\u516B\u6708", "\u4E5D\u6708", "\u5341\u6708", "\u5341\u4E00\u6708", "\u5341\u4E8C\u6708", ""};
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
			case "ab": return "\u963f\u5e03\u54c8\u897f\u4e9a\u6587";
			case "aa": return "\u963f\u6cd5\u6587";
			case "af": return "\u5357\u975e\u8377\u5170\u6587";
			case "sq": return "\u963f\u5c14\u5df4\u5c3c\u4e9a\u6587";
			case "am": return "\u963f\u59c6\u54c8\u62c9\u6587";
			case "ar": return "\u963f\u62c9\u4f2f\u6587";
			case "hy": return "\u4e9a\u7f8e\u5c3c\u4e9a\u6587";
			case "as": return "\u963f\u8428\u59c6\u6587";
			case "ay": return "\u827e\u9a6c\u62c9\u6587";
			case "az": return "\u963f\u585e\u62dc\u7586\u6587";
			case "ba": return "\u5df4\u4ec0\u5ba2\u5c14\u6587";
			case "eu": return "\u5df4\u65af\u514b\u6587";
			case "bn": return "\u5b5f\u52a0\u62c9\u6587";
			case "dz": return "\u4e0d\u4e39\u6587";
			case "bh": return "\u6bd4\u54c8\u5c14\u6587";
			case "bi": return "\u6bd4\u65af\u62c9\u9a6c\u6587";
			case "br": return "\u5e03\u91cc\u591a\u5c3c\u6587";
			case "bg": return "\u4fdd\u52a0\u5229\u4e9a\u6587";
			case "my": return "\u7f05\u7538\u6587";
			case "be": return "\u767d\u4fc4\u7f57\u65af\u6587";
			case "km": return "\u67ec\u57d4\u5be8\u6587";
			case "ca": return "\u52a0\u6cf0\u7f57\u5c3c\u4e9a\u6587";
			case "zh": return "\u4e2d\u6587";
			case "co": return "\u79d1\u897f\u5609\u6587";
			case "hr": return "\u514b\u7f57\u5730\u4e9a\u6587";
			case "cs": return "\u6377\u514b\u6587";
			case "da": return "\u4e39\u9ea6\u6587";
			case "nl": return "\u8377\u5170\u6587";
			case "en": return "\u82f1\u6587";
			case "eo": return "\u4e16\u754c\u6587";
			case "et": return "\u7231\u6c99\u5c3c\u4e9a\u6587";
			case "fo": return "\u6cd5\u7f57\u6587";
			case "fj": return "\u6590\u6d4e\u6587";
			case "fi": return "\u82ac\u5170\u6587";
			case "fr": return "\u6cd5\u6587";
			case "fy": return "\u5f17\u91cc\u65af\u5170\u6587";
			case "gl": return "\u52a0\u5229\u897f\u4e9a\u6587";
			case "ka": return "\u683c\u9c81\u5409\u4e9a\u6587";
			case "de": return "\u5fb7\u6587";
			case "el": return "\u5e0c\u814a\u6587";
			case "kl": return "\u683c\u9675\u5170\u6587";
			case "gn": return "\u74dc\u62c9\u5c3c\u6587";
			case "gu": return "\u53e4\u52a0\u62c9\u63d0\u6587";
			case "ha": return "\u8c6a\u6492\u6587";
			case "he": return "\u5e0c\u4f2f\u6765\u6587";
			case "hi": return "\u5370\u5730\u6587";
			case "hu": return "\u5308\u7259\u5229\u6587";
			case "is": return "\u51b0\u5c9b\u6587";
			case "id": return "\u5370\u5ea6\u5c3c\u897f\u4e9a\u6587";
			case "ia": return "\u62c9\u4e01\u56fd\u9645\u6587";
			case "ie": return "\u62c9\u4e01\u56fd\u9645\u6587";
			case "iu": return "\u7231\u65af\u57fa\u6469\u6587";
			case "ik": return "\u4f9d\u5974\u76ae\u7ef4\u514b\u6587";
			case "ga": return "\u7231\u5c14\u5170\u6587";
			case "it": return "\u610f\u5927\u5229\u6587";
			case "ja": return "\u65e5\u6587";
			case "jv": return "\u722a\u54c7\u6587";
			case "kn": return "\u57c3\u7eb3\u5fb7\u6587";
			case "ks": return "\u514b\u4ec0\u7c73\u5c14\u6587";
			case "kk": return "\u54c8\u8428\u514b\u6587";
			case "rw": return "\u5362\u65fa\u8fbe\u6587";
			case "ky": return "\u5409\u5c14\u5409\u65af\u6587";
			case "rn": return "\u57fa\u9686\u8fea\u6587";
			case "ko": return "\u671d\u9c9c\u6587";
			case "ku": return "\u5e93\u5c14\u5fb7\u6587";
			case "lo": return "\u8001\u631d\u6587";
			case "la": return "\u62c9\u4e01\u6587";
			case "lv": return "\u62c9\u6258\u7ef4\u4e9a\u6587(\u5217\u6258)";
			case "ln": return "\u6797\u52a0\u62c9\u6587";
			case "lt": return "\u7acb\u9676\u5b9b\u6587";
			case "mk": return "\u9a6c\u5176\u987f\u6587";
			case "mg": return "\u9a6c\u5c14\u52a0\u4ec0\u6587";
			case "ms": return "\u9a6c\u6765\u6587";
			case "ml": return "\u9a6c\u6765\u4e9a\u62c9\u59c6\u6587";
			case "mt": return "\u9a6c\u8033\u4ed6\u6587";
			case "mi": return "\u6bdb\u5229\u6587";
			case "mr": return "\u9a6c\u62c9\u5730\u6587";
			case "mo": return "\u6469\u5c14\u591a\u74e6\u6587";
			case "mn": return "\u8499\u53e4\u6587";
			case "na": return "\u7459\u9c81\u6587";
			case "ne": return "\u5c3c\u6cca\u5c14\u6587";
			case "no": return "\u632a\u5a01\u6587";
			case "oc": return "\u5965\u897f\u5766\u6587";
			case "or": return "\u6b27\u91cc\u4e9a\u6587";
			case "om": return "\u963f\u66fc\u6587";
			case "ps": return "\u666e\u4ec0\u56fe\u6587";
			case "fa": return "\u6ce2\u65af\u6587";
			case "pl": return "\u6ce2\u5170\u6587";
			case "pt": return "\u8461\u8404\u7259\u6587";
			case "pa": return "\u65c1\u906e\u666e\u6587";
			case "qu": return "\u76d6\u4e18\u4e9a\u6587";
			case "rm": return "\u91cc\u6258\u7f57\u66fc\u65af\u6587";
			case "ro": return "\u7f57\u9a6c\u5c3c\u4e9a\u6587";
			case "ru": return "\u4fc4\u6587";
			case "sm": return "\u8428\u6469\u4e9a\u6587";
			case "sg": return "\u6851\u6208\u6587";
			case "sa": return "\u68b5\u6587";
			case "gd": return "\u82cf\u683c\u5170- \u76d6\u5c14\u6587";
			case "sr": return "\u585e\u5c14\u7ef4\u4e9a\u6587";
			case "sh": return "\u585e\u6ce2\u5c3c\u65af-\u514b\u7f57\u5730\u4e9a\u6587";
			case "st": return "\u585e\u7d22\u6258\u6587";
			case "tn": return "\u7a81\u5c3c\u65af\u6587";
			case "sn": return "\u585e\u5185\u52a0\u5c14\u6587";
			case "sd": return "\u82cf\u4e39\u6587";
			case "si": return "\u50e7\u4f3d\u7f57\u6587";
			case "ss": return "\u8f9b\u8f9b\u90a3\u63d0\u6587";
			case "sk": return "\u65af\u6d1b\u4f10\u514b\u6587";
			case "sl": return "\u65af\u6d1b\u6587\u5c3c\u4e9a\u6587";
			case "so": return "\u7d22\u9a6c\u91cc\u6587";
			case "es": return "\u897f\u73ed\u7259\u6587";
			case "su": return "\u82cf\u4e39\u6587";
			case "sw": return "\u65af\u74e6\u5e0c\u91cc\u6587";
			case "sv": return "\u745e\u5178\u6587";
			case "tl": return "\u5854\u52a0\u8def\u65cf\u6587";
			case "tg": return "\u5854\u5409\u514b\u6587";
			case "ta": return "\u6cf0\u7c73\u5c14\u6587";
			case "tt": return "\u9791\u977c\u6587";
			case "te": return "\u6cf0\u5362\u56fa\u6587";
			case "th": return "\u6cf0\u6587";
			case "bo": return "\u897f\u85cf\u6587";
			case "ti": return "\u63d0\u683c\u91cc\u5c3c\u4e9a\u6587";
			case "to": return "\u6c64\u52a0\u6587";
			case "ts": return "\u7279\u677e\u52a0\u6587";
			case "tr": return "\u571f\u8033\u5176\u6587";
			case "tk": return "\u571f\u5e93\u66fc\u6587";
			case "tw": return "\u53f0\u6e7e\u6587";
			case "ug": return "\u7ef4\u543e\u5c14\u6587";
			case "uk": return "\u4e4c\u514b\u5170\u6587";
			case "ur": return "\u4e4c\u5c14\u90fd\u6587";
			case "uz": return "\u4e4c\u5179\u522b\u514b\u6587";
			case "vi": return "\u8d8a\u5357\u6587";
			case "vo": return "\u6c83\u62c9\u666e\u514b\u6587";
			case "cy": return "\u5a01\u5c14\u58eb\u6587";
			case "wo": return "\u6c83\u5c14\u592b\u6587";
			case "xh": return "\u73ed\u56fe\u6587";
			case "yi": return "\u4f9d\u5730\u6587";
			case "yo": return "\u7ea6\u9c81\u5df4\u6587";
			case "za": return "\u85cf\u6587";
			case "zu": return "\u7956\u9c81\u6587";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "AF": return "\u963f\u5bcc\u6c57";
			case "AL": return "\u963f\u5c14\u5df4\u5c3c\u4e9a";
			case "DZ": return "\u963f\u5c14\u53ca\u5229\u4e9a";
			case "AD": return "\u5b89\u9053\u5c14";
			case "AO": return "\u5b89\u54e5\u62c9";
			case "AI": return "\u5b89\u572d\u62c9";
			case "AR": return "\u963f\u6839\u5ef7";
			case "AM": return "\u4e9a\u7f8e\u5c3c\u4e9a";
			case "AW": return "\u963f\u9c81\u5df4";
			case "AU": return "\u6fb3\u5927\u5229\u4e9a";
			case "AT": return "\u5965\u5730\u5229";
			case "AZ": return "\u963f\u585e\u62dc\u7586";
			case "BS": return "\u5df4\u54c8\u9a6c";
			case "BH": return "\u5df4\u6797";
			case "BD": return "\u5b5f\u52a0\u62c9";
			case "BB": return "\u5df4\u5df4\u591a\u65af";
			case "BY": return "\u767d\u4fc4\u7f57\u65af";
			case "BE": return "\u6bd4\u5229\u65f6";
			case "BZ": return "\u4f2f\u91cc\u5179";
			case "BJ": return "\u8d1d\u5b81";
			case "BM": return "\u767e\u6155\u5927";
			case "BT": return "\u4e0d\u4e39";
			case "BO": return "\u73bb\u5229\u7ef4\u4e9a";
			case "BA": return "\u6ce2\u65af\u5c3c\u4e9a\u548c\u9ed1\u5c71\u5171\u548c\u56fd";
			case "BW": return "\u535a\u8328\u74e6\u7eb3";
			case "BR": return "\u5df4\u897f";
			case "BN": return "\u6587\u83b1";
			case "BG": return "\u4fdd\u52a0\u5229\u4e9a";
			case "BF": return "\u5e03\u57fa\u7eb3\u6cd5\u7d22";
			case "BI": return "\u5e03\u9686\u8fea";
			case "KH": return "\u67ec\u57d4\u5be8";
			case "CM": return "\u5580\u9ea6\u9686";
			case "CA": return "\u52a0\u62ff\u5927";
			case "CV": return "\u4f5b\u5f97\u89d2";
			case "CF": return "\u4e2d\u975e\u5171\u548c\u56fd";
			case "TD": return "\u4e4d\u5f97";
			case "CL": return "\u667a\u5229";
			case "CN": return "\u4E2D\u83EF\u4EBA\u6C11\u5171\u548C\u570B";
			case "CO": return "\u54e5\u4f26\u6bd4\u4e9a";
			case "KM": return "\u79d1\u6469\u7f57";
			case "CG": return "\u521a\u679c";
			case "CR": return "\u54e5\u65af\u8fbe\u9ece\u52a0";
			case "CI": return "\u8c61\u7259\u6d77\u5cb8";
			case "HR": return "\u514b\u7f57\u5730\u4e9a";
			case "CU": return "\u53e4\u5df4";
			case "CY": return "\u585e\u6d66\u8def\u65af";
			case "CZ": return "\u6377\u514b\u5171\u548c\u56fd";
			case "DK": return "\u4e39\u9ea6";
			case "DJ": return "\u5409\u5e03\u63d0";
			case "DM": return "\u591a\u7c73\u5c3c\u52a0\u8054\u90a6";
			case "DO": return "\u591a\u7c73\u5c3c\u52a0\u5171\u548c\u56fd";
			case "TL": return "\u4e1c\u5e1d\u6c76";
			case "EC": return "\u5384\u74dc\u591a\u5c14";
			case "EG": return "\u57c3\u53ca";
			case "SV": return "\u8428\u5c14\u74e6\u591a";
			case "GQ": return "\u8d64\u9053\u51e0\u5185\u4e9a";
			case "ER": return "\u5384\u91cc\u7279\u5c3c\u4e9a";
			case "EE": return "\u7231\u6c99\u5c3c\u4e9a";
			case "ET": return "\u57c3\u585e\u4fc4\u6bd4\u4e9a";
			case "FJ": return "\u6590\u6d4e";
			case "FI": return "\u82ac\u5170";
			case "FR": return "\u6cd5\u56fd";
			case "GF": return "\u6cd5\u5c5e\u572d\u4e9a\u90a3";
			case "PF": return "\u6cd5\u5c5e\u73bb\u5229\u5c3c\u897f\u4e9a";
			case "TF": return "\u6cd5\u5c5e\u5357\u7279\u7acb\u5c3c\u8fbe";
			case "GA": return "\u52a0\u84ec";
			case "GM": return "\u5188\u6bd4\u4e9a";
			case "GE": return "\u683c\u9c81\u5409\u4e9a";
			case "DE": return "\u5fb7\u56fd";
			case "GH": return "\u52a0\u7eb3";
			case "GR": return "\u5e0c\u814a";
			case "GP": return "\u74dc\u5fb7\u7f57\u666e\u5c9b";
			case "GT": return "\u5371\u5730\u9a6c\u62c9";
			case "GN": return "\u51e0\u5185\u4e9a";
			case "GW": return "\u51e0\u5185\u4e9a\u6bd4\u7ecd\u5171\u548c\u56fd";
			case "GY": return "\u572d\u4e9a\u90a3";
			case "HT": return "\u6d77\u5730";
			case "HN": return "\u6d2a\u90fd\u62c9\u65af";
			case "HK": return "\u4e2d\u56fd\u9999\u6e2f\u7279\u522b\u884c\u653f\u533a";
			case "HU": return "\u5308\u7259\u5229";
			case "IS": return "\u51b0\u5c9b";
			case "IN": return "\u5370\u5ea6";
			case "ID": return "\u5370\u5ea6\u5c3c\u897f\u4e9a";
			case "IR": return "\u4f0a\u6717";
			case "IQ": return "\u4f0a\u62c9\u514b";
			case "IE": return "\u7231\u5c14\u5170";
			case "IL": return "\u4ee5\u8272\u5217";
			case "IT": return "\u610f\u5927\u5229";
			case "JM": return "\u7259\u4e70\u52a0";
			case "JP": return "\u65e5\u672c";
			case "JO": return "\u7ea6\u65e6";
			case "KZ": return "\u54c8\u8428\u514b\u65af\u5766";
			case "KE": return "\u80af\u5c3c\u4e9a";
			case "KI": return "\u57fa\u91cc\u5df4\u65af";
			case "KP": return "\u5317\u671d\u9c9c";
			case "KR": return "\u5357\u671d\u9c9c";
			case "KW": return "\u79d1\u5a01\u7279";
			case "KG": return "\u5409\u5c14\u5409\u514b\u65af\u5766";
			case "LA": return "\u8001\u631d";
			case "LV": return "\u62c9\u8131\u7ef4\u4e9a";
			case "LB": return "\u9ece\u5df4\u5ae9";
			case "LS": return "\u83b1\u7d22\u6258";
			case "LR": return "\u5229\u6bd4\u91cc\u4e9a";
			case "LY": return "\u5229\u6bd4\u4e9a";
			case "LI": return "\u5217\u652f\u6566\u58eb\u767b";
			case "LT": return "\u7acb\u9676\u5b9b";
			case "LU": return "\u5362\u68ee\u5821";
			case "MK": return "\u9a6c\u5176\u987f\u738b\u56fd";
			case "MG": return "\u9a6c\u8fbe\u52a0\u65af\u52a0";
			case "MO": return "\u4e2d\u56fd\u6fb3\u95e8\u7279\u522b\u884c\u653f\u533a";
			case "MY": return "\u9a6c\u6765\u897f\u4e9a";
			case "ML": return "\u9a6c\u91cc";
			case "MT": return "\u9a6c\u8033\u4ed6";
			case "MQ": return "\u9a6c\u63d0\u5c3c\u514b\u5c9b";
			case "MR": return "\u6bdb\u91cc\u5854\u5c3c\u4e9a";
			case "MU": return "\u6bdb\u91cc\u6c42\u65af";
			case "YT": return "\u9a6c\u7ea6\u7279\u5c9b";
			case "MX": return "\u58a8\u897f\u54e5";
			case "FM": return "\u5bc6\u514b\u7f57\u5c3c\u897f\u4e9a";
			case "MD": return "\u6469\u5c14\u591a\u74e6";
			case "MC": return "\u6469\u7eb3\u54e5";
			case "MN": return "\u8499\u53e4";
			case "MS": return "\u8499\u7279\u585e\u62c9\u7fa4\u5c9b";
			case "MA": return "\u6469\u6d1b\u54e5";
			case "MZ": return "\u83ab\u6851\u6bd4\u514b";
			case "MM": return "\u7f05\u7538";
			case "NA": return "\u7eb3\u7c73\u6bd4\u4e9a";
			case "NP": return "\u5c3c\u6cca\u5c14";
			case "NL": return "\u8377\u5170";
			case "AN": return "\u8377\u5c5e\u5b89\u7684\u5217\u65af\u7fa4\u5c9b";
			case "NC": return "\u65b0\u514b\u91cc\u591a\u5c3c\u4e9a\u7fa4\u5c9b";
			case "NZ": return "\u65b0\u897f\u5170";
			case "NI": return "\u5c3c\u52a0\u62c9\u74dc";
			case "NE": return "\u5c3c\u65e5\u5c14";
			case "NG": return "\u5c3c\u65e5\u5229\u4e9a";
			case "NU": return "\u7ebd\u57c3\u5c9b";
			case "NO": return "\u632a\u5a01";
			case "OM": return "\u963f\u66fc";
			case "PK": return "\u5df4\u57fa\u65af\u5766";
			case "PA": return "\u5df4\u62ff\u9a6c";
			case "PG": return "\u5df4\u5e03\u4e9a\u65b0\u51e0\u5185\u4e9a";
			case "PY": return "\u5df4\u62c9\u572d";
			case "PE": return "\u79d8\u9c81";
			case "PH": return "\u83f2\u5f8b\u5bbe";
			case "PL": return "\u6ce2\u5170";
			case "PT": return "\u8461\u8404\u7259";
			case "PR": return "\u6ce2\u591a\u9ece\u54e5";
			case "QA": return "\u5361\u5854\u5c14";
			case "RO": return "\u7f57\u9a6c\u5c3c\u4e9a";
			case "RU": return "\u4fc4\u7f57\u65af";
			case "RW": return "\u5362\u65fa\u8fbe";
			case "SA": return "\u6c99\u7279\u963f\u62c9\u4f2f";
			case "SN": return "\u585e\u5185\u52a0\u5c14";
			case "SP": return "\u585e\u5c14\u7ef4\u4e9a";
			case "SC": return "\u585e\u820c\u5c14\u7fa4\u5c9b";
			case "SL": return "\u585e\u62c9\u91cc\u6602";
			case "SG": return "\u65b0\u52a0\u5761";
			case "SK": return "\u65af\u6d1b\u4f10\u514b";
			case "SI": return "\u65af\u6d1b\u6587\u5c3c\u4e9a";
			case "SO": return "\u7d22\u9a6c\u91cc";
			case "ZA": return "\u5357\u975e";
			case "ES": return "\u897f\u73ed\u7259";
			case "LK": return "\u65af\u91cc\u5170\u5361";
			case "SD": return "\u82cf\u4e39";
			case "SR": return "\u82cf\u91cc\u5357";
			case "SZ": return "\u65af\u5a01\u58eb\u5170";
			case "SE": return "\u745e\u5178";
			case "CH": return "\u745e\u58eb";
			case "SY": return "\u53d9\u5229\u4e9a";
			case "TW": return "\u53f0\u6e7e";
			case "TJ": return "\u5854\u5409\u514b\u65af\u5766";
			case "TZ": return "\u5766\u6851\u5c3c\u4e9a";
			case "TH": return "\u6cf0\u56fd";
			case "TG": return "\u591a\u54e5";
			case "TK": return "\u8054\u5408\u7fa4\u5c9b";
			case "TO": return "\u6c64\u52a0";
			case "TT": return "\u7279\u7acb\u5c3c\u8fbe\u548c\u591a\u5df4\u54e5";
			case "TN": return "\u7a81\u5c3c\u65af";
			case "TR": return "\u571f\u8033\u5176";
			case "TM": return "\u571f\u5e93\u66fc\u65af\u5766";
			case "UG": return "\u4e4c\u5e72\u8fbe";
			case "UA": return "\u4e4c\u514b\u5170";
			case "AE": return "\u963f\u62c9\u4f2f\u8054\u5408\u914b\u957f\u56fd";
			case "GB": return "\u82f1\u56fd";
			case "US": return "\u7f8e\u56fd";
			case "UY": return "\u4e4c\u62c9\u572d";
			case "UZ": return "\u4e4c\u5179\u522b\u514b\u65af\u5766";
			case "VU": return "\u74e6\u52aa\u963f\u56fe";
			case "VA": return "\u68b5\u8482\u5188";
			case "VE": return "\u59d4\u5185\u745e\u62c9";
			case "VN": return "\u8d8a\u5357";
			case "VG": return "\u82f1\u5c5e\u7ef4\u4eac\u7fa4\u5c9b";
			case "VI": return "\u7f8e\u5c5e\u7ef4\u4eac\u7fa4\u5c9b";
			case "EH": return "\u897f\u6492\u54c8\u62c9";
			case "YE": return "\u4e5f\u95e8";
			case "YU": return "\u5357\u65af\u62c9\u592b";
			case "ZM": return "\u8d5e\u6bd4\u4e9a";
			case "ZW": return "\u6d25\u5df4\u5e03\u97e6";
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
				return 936;
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
				return 10008;
			}
		}
		public override int OEMCodePage
		{
			get
			{
				return 936;
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

}; // class CID0004

public class CNzh : CID0004
{
	public CNzh() : base() {}

}; // class CNzh

public class CNzh_chs : CID0004
{
	public CNzh_chs() : base() {}

}; // class CNzh_chs

}; // namespace I18N.CJK
