/*
 * CID0049.cs - ta culture handler.
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

// Generated from "ta.txt".

namespace I18N.Other
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0049 : RootCulture
{
	public CID0049() : base(0x0049) {}
	public CID0049(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "ta";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "tam";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "TAM";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "ta";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AMDesignator = "\u0b95\u0bbe\u0bb2\u0bc8";
			dfi.PMDesignator = "\u0bae\u0bbe\u0bb2\u0bc8";
			dfi.AbbreviatedDayNames = new String[] {"\u0b9e\u0bbe", "\u0ba4\u0bbf", "\u0b9a\u0bc6", "\u0baa\u0bc1", "\u0bb5\u0bbf", "\u0bb5\u0bc6", "\u0b9a"};
			dfi.DayNames = new String[] {"\u0b9e\u0bbe\u0baf\u0bbf\u0bb1\u0bc1", "\u0ba4\u0bbf\u0b99\u0bcd\u0b95\u0bb3\u0bcd", "\u0b9a\u0bc6\u0bb5\u0bcd\u0bb5\u0bbe\u0baf\u0bcd", "\u0baa\u0bc1\u0ba4\u0ba9\u0bcd", "\u0bb5\u0bbf\u0baf\u0bbe\u0bb4\u0ba9\u0bcd", "\u0bb5\u0bc6\u0bb3\u0bcd\u0bb3\u0bbf", "\u0b9a\u0ba9\u0bbf"};
			dfi.AbbreviatedMonthNames = new String[] {"\u0b9c\u0ba9.", "\u0baa\u0bc6\u0baa\u0bcd.", "\u0bae\u0bbe\u0bb0\u0bcd.", "\u0b8f\u0baa\u0bcd.", "\u0bae\u0bc7", "\u0b9c\u0bc2\u0ba9\u0bcd", "\u0b9c\u0bc2\u0bb2\u0bc8", "\u0b86\u0b95.", "\u0b9a\u0bc6\u0baa\u0bcd.", "\u0b85\u0b95\u0bcd.", "\u0ba8\u0bb5.", "\u0b9f\u0bbf\u0b9a.", ""};
			dfi.MonthNames = new String[] {"\u0b9c\u0ba9\u0bb5\u0bb0\u0bbf", "\u0baa\u0bc6\u0baa\u0bcd\u0bb0\u0bb5\u0bb0\u0bbf", "\u0bae\u0bbe\u0bb0\u0bcd\u0b9a\u0bcd", "\u0b8f\u0baa\u0bcd\u0bb0\u0bb2\u0bcd", "\u0bae\u0bc7", "\u0b9c\u0bc2\u0ba9\u0bcd", "\u0b9c\u0bc2\u0bb2\u0bc8", "\u0b86\u0b95\u0bb8\u0bcd\u0b9f\u0bcd", "\u0b9a\u0bc6\u0baa\u0bcd\u0b9f\u0bae\u0bcd\u0baa\u0bb0\u0bcd", "\u0b85\u0b95\u0bcd\u0b9f\u0bcb\u0baa\u0bb0\u0bcd", "\u0ba8\u0bb5\u0bae\u0bcd\u0baa\u0bb0\u0bcd", "\u0b9f\u0bbf\u0b9a\u0bae\u0bcd\u0baa\u0bb0\u0bcd", ""};
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
			case "root": return "\u0ba4\u0bae\u0bbf\u0bb4\u0bcd";
			case "aa": return "\u0b85\u0baa\u0bbe\u0bb0\u0bcd";
			case "ab": return "\u0b85\u0baa\u0bcd\u0b95\u0bbe\u0bb8\u0bbf\u0ba9\u0bcd";
			case "af": return "\u0b86\u0baa\u0bcd\u0bb0\u0bbf\u0b95\u0ba9\u0bcd\u0bb8\u0bcd";
			case "am": return "\u0b85\u0bae\u0bcd\u0bb9\u0bbe\u0bb0\u0bbf\u0b95\u0bcd";
			case "ar": return "\u0b85\u0bb0\u0baa\u0bbf\u0b95\u0bcd";
			case "as": return "\u0b85\u0bb8\u0bbe\u0bae\u0bc0\u0bb8\u0bcd";
			case "ay": return "\u0b85\u0baf\u0bae\u0bb0\u0bbe";
			case "az": return "\u0b85\u0b9a\u0bb0\u0bcd\u0baa\u0bbe\u0baf\u0bcd\u0b9c\u0bbe\u0ba9\u0bbf";
			case "ba": return "\u0baa\u0bbe\u0bb7\u0bcd\u0b95\u0bbf\u0bb0\u0bcd0";
			case "be": return "\u0baa\u0bc8\u0bb2\u0bcb\u0bb0\u0bc1\u0bb7\u0bcd\u0ba9\u0bcd";
			case "bg": return "\u0baa\u0bb2\u0bcd\u0b95\u0bc6\u0bb0\u0bbf\u0baf\u0ba9\u0bcd";
			case "bh": return "\u0baa\u0bbf\u0bb9\u0bbe\u0bb0\u0bbf";
			case "bi": return "\u0baa\u0bbf\u0bb8\u0bcd\u0bb2\u0bbe\u0bae\u0bbe";
			case "bn": return "\u0baa\u0bc6\u0b99\u0bcd\u0b95\u0bbe\u0bb2\u0bbf";
			case "bo": return "\u0ba4\u0bbf\u0baa\u0bc6\u0ba4\u0bcd\u0ba4\u0bbf\u0baf\u0ba9\u0bcd";
			case "br": return "\u0baa\u0bbf\u0bb0\u0bbf\u0b9f\u0ba9\u0bcd";
			case "ca": return "\u0b95\u0bbe\u0b9f\u0bb2\u0bbe\u0ba9\u0bcd";
			case "co": return "\u0b95\u0bbe\u0bb0\u0bcd\u0b9a\u0bbf\u0baf\u0ba9\u0bcd";
			case "cs": return "\u0b9a\u0bc6\u0b95\u0bcd";
			case "cy": return "\u0bb5\u0bc6\u0bb2\u0bcd\u0bb7\u0bcd";
			case "da": return "\u0b9f\u0bbe\u0ba9\u0bbf\u0bb7\u0bcd";
			case "de": return "\u0b9c\u0bc6\u0bb0\u0bcd\u0bae\u0ba9\u0bcd";
			case "dz": return "\u0baa\u0bc1\u0b9f\u0bbe\u0ba9\u0bbf";
			case "el": return "\u0b95\u0bbf\u0bb0\u0bbf\u0b95\u0bcd";
			case "en": return "\u0b86\u0b99\u0bcd\u0b95\u0bbf\u0bb2\u0bae\u0bcd";
			case "eo": return "\u0b8e\u0bb8\u0bcd\u0baa\u0bb0\u0bc7\u0ba9\u0bcd\u0b9f\u0bcb";
			case "es": return "\u0bb8\u0bcd\u0baa\u0bc7\u0ba9\u0bbf\u0bb7\u0bcd";
			case "et": return "\u0b8e\u0bb8\u0bcd\u0b9f\u0bcb\u0ba9\u0bbf\u0baf\u0ba9\u0bcd";
			case "eu": return "\u0baa\u0bb8\u0bcd\u0b95\u0bcd";
			case "fa": return "\u0baa\u0bb0\u0bcd\u0bb8\u0bbf\u0baf\u0ba9\u0bcd";
			case "fi": return "\u0baa\u0bbf\u0ba9\u0bcd\u0bb7\u0bcd";
			case "fj": return "\u0baa\u0bbf\u0b9c\u0bbf";
			case "fo": return "\u0baa\u0bc8\u0bb0\u0bcb\u0bb8\u0bbf";
			case "fr": return "\u0baa\u0bbf\u0bb0\u0ba9\u0bcd\u0b9a\u0bcd";
			case "fy": return "\u0baa\u0bbf\u0bb0\u0bbf\u0bb7\u0bbf\u0baf\u0ba9\u0bcd";
			case "ga": return "\u0b90\u0bb0\u0bbf\u0bb7\u0bcd";
			case "gd": return "\u0bb8\u0bcd\u0b95\u0bbe\u0b9f\u0bcd\u0bb8\u0bcd \u0b95\u0bbe\u0bb2\u0bc6\u0b95\u0bcd";
			case "gl": return "\u0b95\u0bc6\u0bb2\u0bbf\u0bb8\u0bbf\u0baf\u0ba9\u0bcd";
			case "gn": return "\u0b95\u0bc1\u0bb0\u0bbe\u0ba9\u0bbf";
			case "gu": return "\u0b95\u0bc1\u0b9c\u0bb0\u0bbe\u0ba4\u0bcd\u0ba4\u0bbf";
			case "ha": return "\u0bb9\u0bca\u0bb8\u0bbe";
			case "he": return "\u0bb9\u0bc1\u0baa\u0bcd\u0bb0\u0bc1";
			case "hi": return "\u0b87\u0ba8\u0bcd\u0ba4\u0bbf";
			case "hr": return "\u0b95\u0bb0\u0bcb\u0bb7\u0bbf\u0baf\u0ba9\u0bcd";
			case "hu": return "\u0bb9\u0b99\u0bcd\u0b95\u0bc7\u0bb0\u0bbf\u0baf\u0ba9\u0bcd";
			case "hy": return "\u0b86\u0bb0\u0bcd\u0bae\u0bc7\u0ba9\u0bbf\u0baf\u0ba9\u0bcd";
			case "ia": return "\u0b87\u0ba9\u0bcd\u0b9f\u0bb0\u0bcd\u0bb2\u0bbf\u0b99\u0bcd\u0b95\u0bc1\u0bb5\u0bbe";
			case "id": return "\u0b87\u0ba8\u0bcd\u0ba4\u0bcb\u0ba9\u0bc7\u0bb7\u0bbf\u0baf\u0ba9\u0bcd";
			case "ie": return "\u0b87\u0ba9\u0bcd\u0b9f\u0bb0\u0bcd\u0bb2\u0bbf\u0b99\u0bcd\u0b95\u0bc1\u0bb5\u0bbe";
			case "ik": return "\u0b87\u0ba9\u0bc1\u0baa\u0bc6\u0b95\u0bcd";
			case "is": return "\u0b90\u0bb8\u0bcd\u0bb2\u0bc6\u0ba9\u0bcd\u0b9f\u0bbf\u0b95\u0bcd";
			case "it": return "\u0b87\u0ba4\u0bcd\u0ba4\u0bbe\u0bb2\u0bbf\u0baf\u0ba9\u0bcd";
			case "iu": return "\u0b87\u0ba9\u0bc1\u0b95\u0bbf\u0b9f\u0b9f\u0bcd";
			case "ja": return "\u0b9c\u0bbe\u0baa\u0ba9\u0bc0\u0bb8\u0bcd";
			case "jv": return "\u0b9c\u0bbe\u0bb5\u0bbe\u0ba9\u0bc0\u0bb8\u0bcd";
			case "ka": return "\u0b9c\u0bbe\u0bb0\u0bcd\u0b9c\u0bbf\u0baf\u0ba9\u0bcd";
			case "kk": return "\u0b95\u0b9a\u0bbe\u0b95\u0bcd";
			case "kl": return "\u0b95\u0bbf\u0bb0\u0bbf\u0ba9\u0bcd\u0bb2\u0bc6\u0ba9\u0bcd\u0b9f\u0bbf\u0b95\u0bcd";
			case "km": return "\u0b95\u0bae\u0bcd\u0baa\u0bcb\u0b9f\u0bbf\u0baf\u0ba9\u0bcd";
			case "kn": return "\u0b95\u0ba9\u0bcd\u0ba9\u0b9f\u0bbe";
			case "ko": return "\u0b95\u0bca\u0bb0\u0bbf\u0baf\u0ba9\u0bcd";
			case "kok": return "\u0b95\u0bcb\u0b99\u0bcd\u0b95\u0bcd\u0b95\u0ba9\u0bbf";
			case "ks": return "\u0b95\u0bbe\u0bb7\u0bcd\u0bae\u0bbf\u0bb0\u0bbf";
			case "ku": return "\u0b95\u0bc1\u0bb0\u0bcd\u0ba4\u0bbf\u0bb7\u0bcd";
			case "ky": return "\u0b95\u0bbf\u0bb0\u0bcd\u0b95\u0bbf\u0bb7\u0bcd";
			case "la": return "\u0bb2\u0bbe\u0ba4\u0bbf\u0ba9\u0bcd";
			case "ln": return "\u0bb2\u0bbf\u0b99\u0bcd\u0b95\u0bbe\u0bb2\u0bbe";
			case "lo": return "\u0bb2\u0bcb\u0ba4\u0bcd\u0ba4\u0bbf\u0baf\u0ba9\u0bcd";
			case "lt": return "\u0bb2\u0bc1\u0ba4\u0bcd\u0ba4\u0bc7\u0ba9\u0bbf\u0baf\u0ba9\u0bcd";
			case "lv": return "\u0bb2\u0bc7\u0b9f\u0bcd\u0bb5\u0bbf\u0baf\u0ba9\u0bcd (\u0bb2\u0bc7\u0b9f\u0bcd\u0b9f\u0bbf\u0bb7\u0bcd)";
			case "mg": return "\u0bae\u0bb2\u0b95\u0bc6\u0bb8\u0bbf";
			case "mi": return "\u0bae\u0bcb\u0bb0\u0bbf";
			case "mk": return "\u0bae\u0bc6\u0b95\u0bcd\u0b95\u0b9f\u0bcb\u0ba9\u0bbf\u0baf\u0ba9\u0bcd";
			case "ml": return "\u0bae\u0bb2\u0baf\u0bbe\u0bb4\u0bae\u0bcd";
			case "mn": return "\u0bae\u0b99\u0bcd\u0b95\u0bcb\u0bb2\u0bbf\u0baf\u0ba9\u0bcd";
			case "mo": return "\u0bae\u0bcb\u0bb2\u0bcd\u0b9f\u0bc7\u0bb5\u0bbf\u0baf\u0ba9\u0bcd";
			case "mr": return "\u0bae\u0bb0\u0bbe\u0ba4\u0bcd\u0ba4\u0bbf";
			case "ms": return "\u0bae\u0bb2\u0bbe\u0baf\u0bcd";
			case "mt": return "\u0bae\u0bbe\u0bb2\u0bcd\u0b9f\u0bbf\u0bb8\u0bcd";
			case "my": return "\u0baa\u0bb0\u0bcd\u0bae\u0bbf\u0bb8\u0bcd";
			case "na": return "\u0ba8\u0bbe\u0bb0\u0bc2";
			case "ne": return "\u0ba8\u0bc7\u0baa\u0bcd\u0baa\u0bbe\u0bb2\u0bbf";
			case "nl": return "\u0b9f\u0b9a\u0bcd";
			case "no": return "\u0ba8\u0bbe\u0bb0\u0bcd\u0bb5\u0bc7\u0b95\u0bbf\u0baf\u0ba9\u0bcd";
			case "oc": return "\u0b86\u0b95\u0bbf\u0b9f\u0bbf\u0baf\u0ba9\u0bcd";
			case "om": return "\u0b92\u0bb0\u0bcb\u0bae (\u0b85\u0baa\u0ba9\u0bcd)";
			case "or": return "\u0b92\u0bb0\u0bbf\u0baf\u0bbe";
			case "pa": return "\u0baa\u0b9e\u0bcd\u0b9a\u0bbe\u0baa\u0bbf";
			case "pl": return "\u0baa\u0bcb\u0bb2\u0bbf\u0bb7\u0bcd";
			case "ps": return "\u0baa\u0bc7\u0bb7\u0bcd\u0b9f\u0bcb (\u0baa\u0bc1\u0bb7\u0bcd\u0b9f\u0bcb)";
			case "pt": return "\u0baa\u0bcb\u0bb0\u0bcd\u0b9a\u0bcd\u0b9a\u0bbf\u0b95\u0bc0\u0bb8\u0bcd";
			case "qu": return "\u0b95\u0bbf\u0baf\u0bc1\u0b9a\u0bbeQuechua";
			case "rm": return "\u0bb0\u0bc8\u0b9f\u0bcd\u0b9f\u0bcb-\u0bb0\u0bcb\u0bae\u0bc6\u0ba9\u0bcd\u0bb8\u0bcd";
			case "rn": return "\u0b95\u0bbf\u0bb0\u0bc1\u0ba8\u0bcd\u0ba4\u0bbf";
			case "ro": return "\u0bb0\u0bcb\u0bae\u0bc7\u0ba9\u0bbf\u0baf\u0ba9\u0bcd";
			case "ru": return "\u0bb0\u0bb7\u0bbf\u0baf\u0ba9\u0bcd";
			case "rw": return "\u0b95\u0bbf\u0ba9\u0bcd\u0baf\u0bb0\u0bcd\u0bb5\u0bc6\u0ba9\u0bcd\u0b9f\u0bbe";
			case "sa": return "\u0b9a\u0bae\u0bb8\u0bcd\u0b95\u0bbf\u0bb0\u0bbf\u0ba4\u0bae\u0bcd";
			case "sd": return "\u0b9a\u0bbf\u0ba8\u0bcd\u0ba4\u0bbf";
			case "sg": return "\u0b9a\u0bc6\u0ba9\u0bcd\u0b95\u0bcd\u0bb0\u0bcb";
			case "sh": return "\u0b9a\u0bc6\u0bb0\u0bcd\u0baa\u0bcb-\u0b95\u0bcd\u0bb0\u0bcb\u0bb7\u0bbf\u0baf\u0ba9\u0bcd";
			case "si": return "\u0b9a\u0bbf\u0b99\u0bcd\u0b95\u0bb3\u0bbf\u0bb8\u0bcd";
			case "sk": return "\u0bb8\u0bcd\u0bb2\u0bcb\u0bb5\u0bc6\u0b95\u0bcd";
			case "sl": return "\u0bb8\u0bcd\u0bb2\u0bcb\u0bb5\u0bbf\u0ba9\u0bc7\u0baf\u0bbf\u0ba9\u0bcd";
			case "sm": return "\u0bb8\u0bc6\u0bae\u0bcb\u0ba9\u0bcd";
			case "sn": return "\u0bb7\u0bcb\u0ba9\u0bbe";
			case "so": return "\u0b9a\u0bcb\u0bae\u0bbe\u0bb2\u0bbf";
			case "sq": return "\u0b85\u0bb2\u0bcd\u0baa\u0bc6\u0ba9\u0bbf\u0baf\u0ba9\u0bcd";
			case "sr": return "\u0b9a\u0bb0\u0bcd\u0baa\u0bbf\u0baf\u0ba9\u0bcd";
			case "ss": return "\u0bb7\u0bbf\u0bb8\u0bcd\u0bb5\u0bbe\u0ba4\u0bbf";
			case "st": return "\u0bb7\u0bc6\u0bb8\u0bcd\u0bb8\u0bcb\u0ba4\u0bcb";
			case "su": return "\u0b9a\u0bc1\u0b9f\u0bbe\u0ba9\u0bc0\u0bb8\u0bcd";
			case "sv": return "\u0bb7\u0bc0\u0bb5\u0bbf\u0b9f\u0bbf\u0bb8\u0bcd";
			case "sw": return "\u0bb8\u0bcd\u0bb5\u0bc6\u0bb9\u0bbf\u0bb2\u0bbf";
			case "ta": return "\u0ba4\u0bae\u0bbf\u0bb4\u0bcd";
			case "te": return "\u0ba4\u0bc6\u0bb2\u0bc1\u0b99\u0bcd\u0b95\u0bc1";
			case "tg": return "\u0ba4\u0bbe\u0b9c\u0bbf\u0b95\u0bcd";
			case "th": return "\u0ba4\u0bbe\u0baf\u0bcd";
			case "ti": return "\u0b9f\u0bbf\u0b95\u0bcd\u0bb0\u0bbf\u0ba9\u0bcd\u0baf\u0bbe";
			case "tk": return "\u0b9f\u0bb0\u0bcd\u0b95\u0bcd\u0bae\u0bc6\u0ba9\u0bcd";
			case "tl": return "\u0b9f\u0bbe\u0b95\u0bbe\u0bb2\u0bcb\u0b95\u0bcd";
			case "tn": return "\u0bb8\u0bc6\u0b9f\u0bcd\u0bb8\u0bcd\u0bb5\u0bbe\u0ba9\u0bbe";
			case "to": return "\u0b9f\u0bcb\u0b99\u0bcd\u0b95\u0bbe";
			case "tr": return "\u0b9f\u0bb0\u0bcd\u0b95\u0bbf\u0bb7\u0bcd";
			case "ts": return "\u0bb8\u0bcb\u0b99\u0bcd\u0b95\u0bbe";
			case "tt": return "\u0b9f\u0bbe\u0b9f\u0bb0\u0bcd";
			case "tw": return "\u0ba4\u0bcd\u0ba4\u0bbf\u0bb5\u0bbf";
			case "ug": return "\u0baf\u0bc1\u0b95\u0bc1\u0bb0\u0bcd";
			case "uk": return "\u0b89\u0b95\u0bcd\u0bb0\u0bc7\u0ba9\u0bbf\u0baf\u0ba9\u0bcd";
			case "ur": return "\u0b89\u0bb0\u0bc1\u0ba4\u0bc1";
			case "uz": return "\u0b89\u0bb8\u0bcd\u0baa\u0bc6\u0b95\u0bcd";
			case "vi": return "\u0bb5\u0bbf\u0baf\u0b9f\u0bcd\u0ba8\u0bbe\u0bae\u0bbf\u0bb8\u0bcd";
			case "vo": return "\u0b92\u0bb2\u0baa\u0bc1\u0b95\u0bcdVolapuk";
			case "wo": return "\u0b92\u0bb2\u0bcb\u0baa\u0bcdWolof";
			case "xh": return "\u0bb9\u0bcb\u0bb7\u0bbeXhosa";
			case "yi": return "\u0b88\u0ba4\u0bcd\u0ba4\u0bbf\u0bb7";
			case "yo": return "\u0baf\u0bcb\u0bb0\u0bc1\u0baa\u0bcd\u0baa\u0bbe";
			case "za": return "\u0b9c\u0bc1\u0bb5\u0bbe\u0b99\u0bcd";
			case "zh": return "\u0b9a\u0baf\u0ba9\u0bc0\u0bb8\u0bcd";
			case "zu": return "\u0b9c\u0bc2\u0bb2\u0bc2";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "IN": return "\u0b87\u0ba8\u0bcd\u0ba4\u0bbf\u0baf\u0bbe";
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
				return 0;
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
				return 2;
			}
		}
		public override int OEMCodePage
		{
			get
			{
				return 1;
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

}; // class CID0049

public class CNta : CID0049
{
	public CNta() : base() {}

}; // class CNta

}; // namespace I18N.Other
