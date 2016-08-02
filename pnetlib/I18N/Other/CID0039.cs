/*
 * CID0039.cs - hi culture handler.
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

// Generated from "hi.txt".

namespace I18N.Other
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0039 : RootCulture
{
	public CID0039() : base(0x0039) {}
	public CID0039(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "hi";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "hin";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "HIN";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "hi";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AMDesignator = "\u092a\u0942\u0930\u094d\u0935\u093e\u0939\u094d\u0928";
			dfi.PMDesignator = "\u0905\u092a\u0930\u093e\u0939\u094d\u0928";
			dfi.AbbreviatedDayNames = new String[] {"\u0930\u0935\u093f", "\u0938\u094b\u092e", "\u092e\u0902\u0917\u0932", "\u092c\u0941\u0927", "\u0917\u0941\u0930\u0941", "\u0936\u0941\u0915\u094d\u0930", "\u0936\u0928\u093f"};
			dfi.DayNames = new String[] {"\u0930\u0935\u093f\u0935\u093e\u0930", "\u0938\u094b\u092e\u0935\u093e\u0930", "\u092e\u0902\u0917\u0932\u0935\u093e\u0930", "\u092c\u0941\u0927\u0935\u093e\u0930", "\u0917\u0941\u0930\u0941\u0935\u093e\u0930", "\u0936\u0941\u0915\u094d\u0930\u0935\u093e\u0930", "\u0936\u0928\u093f\u0935\u093e\u0930"};
			dfi.AbbreviatedMonthNames = new String[] {"\u091c\u0928\u0935\u0930\u0940", "\u092b\u0930\u0935\u0930\u0940", "\u092e\u093e\u0930\u094d\u091a", "\u0905\u092a\u094d\u0930\u0948\u0932", "\u092e\u0908", "\u091c\u0942\u0928", "\u091c\u0941\u0932\u093e\u0908", "\u0905\u0917\u0938\u094d\u0924", "\u0938\u093F\u0924\u092E\u094D\u092C\u0930", "\u0905\u0915\u094d\u0924\u0942\u092c\u0930", "\u0928\u0935\u092E\u094D\u092C\u0930", "\u0926\u093F\u0938\u092E\u094D\u092C\u0930", ""};
			dfi.MonthNames = new String[] {"\u091c\u0928\u0935\u0930\u0940", "\u092b\u0930\u0935\u0930\u0940", "\u092e\u093e\u0930\u094d\u091a", "\u0905\u092a\u094d\u0930\u0948\u0932", "\u092e\u0908", "\u091c\u0942\u0928", "\u091c\u0941\u0932\u093e\u0908", "\u0905\u0917\u0938\u094d\u0924", "\u0938\u093F\u0924\u092E\u094D\u092C\u0930", "\u0905\u0915\u094d\u0924\u0942\u092c\u0930", "\u0928\u0935\u092E\u094D\u092C\u0930", "\u0926\u093F\u0938\u092E\u094D\u092C\u0930", ""};
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
			nfi.CurrencyDecimalSeparator = ".";
			nfi.CurrencyGroupSeparator = ",";
			nfi.NumberGroupSeparator = ",";
			nfi.PercentGroupSeparator = ",";
			nfi.NegativeSign = "-";
			nfi.NumberDecimalSeparator = ".";
			nfi.PercentDecimalSeparator = ".";
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
			case "root": return "\u0936\u093F\u0916\u0930";
			case "aa": return "\u0905\u092B\u093C\u093E\u0930";
			case "ab": return "\u0905\u092C\u094D\u0916\u093E\u095B\u093F\u092F\u0928\u094D";
			case "af": return "\u0905\u092B\u094D\u0930\u0940\u0915\u0940";
			case "am": return "\u0905\u092E\u094D\u0939\u093E\u0930\u093F\u0915\u094D";
			case "ar": return "\u0905\u0930\u092C\u0940";
			case "as": return "\u0905\u0938\u093E\u092E\u0940";
			case "ay": return "\u0906\u092F\u092E\u093E\u0930\u093E";
			case "az": return "\u0905\u095B\u0930\u092C\u0948\u0902\u091C\u093E\u0928\u0940";
			case "ba": return "\u092C\u0936\u0959\u093F\u0930";
			case "be": return "\u092C\u0948\u0932\u094B\u0930\u0942\u0936\u093F\u092F\u0928\u094D";
			case "bg": return "\u092C\u0932\u094D\u0917\u0947\u0930\u093F\u092F\u0928\u094D";
			case "bh": return "\u092C\u093F\u0939\u093E\u0930\u0940";
			case "bi": return "\u092C\u093F\u0938\u094D\u0932\u093E\u092E\u093E";
			case "bn": return "\u092C\u0901\u0917\u093E\u0932\u0940";
			case "bo": return "\u0924\u093F\u092C\u094D\u092C\u0924\u0940";
			case "br": return "\u092C\u094D\u0930\u0947\u091F\u0928";
			case "ca": return "\u0915\u093E\u0924\u093E\u0932\u093E\u0928";
			case "co": return "\u0915\u094B\u0930\u094D\u0938\u0940\u0915\u0928";
			case "cs": return "\u091A\u0947\u0915";
			case "cy": return "\u0935\u0947\u0932\u094D\u0936";
			case "da": return "\u0921\u0948\u0928\u0940\u0936";
			case "de": return "\u095B\u0930\u094D\u092E\u0928";
			case "dz": return "\u092D\u0941\u091F\u093E\u0928\u0940";
			case "el": return "\u0917\u094D\u0930\u0940\u0915";
			case "en": return "\u0905\u0902\u0917\u094D\u0930\u0947\u091C\u0940";
			case "eo": return "\u090F\u0938\u094D\u092A\u0947\u0930\u093E\u0928\u094D\u0924\u094B";
			case "es": return "\u0938\u094D\u092A\u0947\u0928\u093F\u0936";
			case "et": return "\u0910\u0938\u094D\u0924\u094B\u0928\u093F\u092F\u0928\u094D";
			case "eu": return "\u092C\u093E\u0938\u094D\u0915\u094D";
			case "fa": return "\u092A\u0930\u094D\u0936\u093F\u092F\u0928\u094D";
			case "fi": return "\u092B\u093F\u0928\u093F\u0936";
			case "fj": return "\u095E\u0940\u091C\u0940";
			case "fo": return "\u092B\u093F\u0930\u094B\u095B\u0940";
			case "fr": return "\u092B\u094D\u0930\u0947\u0902\u091A";
			case "fy": return "\u092B\u094D\u0930\u0940\u091C\u093C\u0928\u094D";
			case "ga": return "\u0906\u0908\u0930\u093F\u0936";
			case "gd": return "\u0938\u094D\u0915\u093E\u091F\u094D\u0938\u094D \u0917\u093E\u092F\u0947\u0932\u093F\u0915\u094D";
			case "gl": return "\u0917\u0948\u0932\u093F\u0936\u093F\u092F\u0928\u094D";
			case "gn": return "\u0917\u0941\u0906\u0930\u093E\u0928\u0940";
			case "gu": return "\u0917\u0941\u095B\u0930\u093E\u0924\u0940";
			case "ha": return "\u0939\u094B\u0909\u0938\u093E";
			case "he": return "\u0939\u093F\u092C\u094D\u0930\u0940\u090A";
			case "hi": return "\u0939\u093f\u0902\u0926\u0940";
			case "hr": return "\u0915\u094D\u0930\u094B\u090F\u0936\u0928\u094D";
			case "hu": return "\u0939\u0902\u0917\u0947\u0930\u0940\u000D";
			case "hy": return "\u0905\u0930\u092E\u0947\u0928\u093F\u092F\u0928\u094D";
			case "ia": return "\u0908\u0928\u094D\u091F\u0930\u0932\u093F\u0902\u0917\u0941\u0906";
			case "id": return "\u0907\u0928\u094D\u0921\u094B\u0928\u0947\u0936\u093F\u092F\u0928\u094D";
			case "ie": return "\u0908\u0928\u094D\u091F\u0930\u0932\u093F\u0902\u0917\u0941\u0907";
			case "ik": return "\u0907\u0928\u0941\u092A\u093F\u092F\u093E\u0915\u094D";
			case "is": return "\u0906\u0908\u0938\u094D\u0932\u0948\u0902\u0921\u093F\u0915\u094D";
			case "it": return "\u0908\u091F\u093E\u0932\u093F\u092F\u0928\u094D";
			case "iu": return "\u0907\u0928\u0942\u0915\u0940\u091F\u0942\u0924\u094D";
			case "ja": return "\u091C\u093E\u092A\u093E\u0928\u0940";
			case "jv": return "\u091C\u093E\u0935\u093E\u0928\u0940\u0938";
			case "ka": return "\u091C\u0949\u0930\u094D\u091C\u0940\u092F\u0928\u094D";
			case "kk": return "\u0915\u095B\u093E\u0916";
			case "kl": return "\u0917\u094D\u0930\u0940\u0928\u0932\u0948\u0902\u0921\u093F\u0915";
			case "km": return "\u0915\u0948\u092E\u094D\u092C\u094B\u0921\u093F\u092F\u0928\u094D";
			case "kn": return "\u0915\u0928\u094D\u0928\u0921\u093C";
			case "ko": return "\u0915\u094B\u0930\u0940\u092F\u0928\u094D";
			case "kok": return "\u0915\u094B\u0902\u0915\u0923\u0940";
			case "ks": return "\u0915\u093E\u0936\u094D\u092E\u093F\u0930\u0940";
			case "ku": return "\u0915\u0941\u0930\u0926\u0940\u0936";
			case "ky": return "\u0915\u093F\u0930\u0918\u093F\u095B";
			case "la": return "\u0932\u0948\u091F\u0940\u0928";
			case "ln": return "\u0932\u093F\u0902\u0917\u093E\u0932\u093E";
			case "lo": return "\u0932\u093E\u0913\u0925\u0940\u092F\u0928\u094D";
			case "lt": return "\u0932\u093F\u0925\u0941\u0928\u093F\u092F\u0928\u094D";
			case "lv": return "\u0932\u093E\u091F\u0935\u093F\u092F\u0928\u094D (\u0932\u0947\u091F\u094D\u091F\u0940\u0936)";
			case "mg": return "\u092E\u093E\u0932\u093E\u0917\u093E\u0938\u0940";
			case "mi": return "\u092E\u0947\u0913\u0930\u0940";
			case "mk": return "\u092E\u0948\u0938\u0947\u0921\u094B\u0928\u093F\u092F\u0928\u094D";
			case "ml": return "\u092E\u0932\u092F\u093E\u0932\u092E";
			case "mn": return "\u092E\u094B\u0902\u0917\u094B\u0932\u093F\u092F\u0928";
			case "mo": return "\u092E\u094B\u0932\u0921\u093E\u0935\u093F\u092F\u0928\u094D";
			case "mr": return "\u092E\u0930\u093E\u0920\u0940";
			case "ms": return "\u092E\u0932\u092F";
			case "mt": return "\u092E\u093E\u0932\u091F\u093F\u0938\u094D";
			case "my": return "\u092C\u0930\u094D\u0932\u093F\u0938";
			case "na": return "\u0928\u093E\u092F\u0930\u0942";
			case "ne": return "\u0928\u0947\u092A\u093E\u0932\u0940";
			case "nl": return "\u0921\u091A\u094D";
			case "no": return "\u0928\u093E\u0930\u094D\u0935\u0947\u091C\u0940\u092F\u0928\u094D";
			case "oc": return "\u0913\u0938\u0940\u091F\u093E\u0928";
			case "om": return "\u0913\u0930\u094B\u092E\u094B (\u0905\u092B\u093C\u093E\u0928)";
			case "or": return "\u0909\u0921\u093C\u093F\u092F\u093E";
			case "pa": return "\u092A\u0902\u091C\u093E\u092C\u0940";
			case "pl": return "\u092A\u0949\u0932\u093F\u0936";
			case "ps": return "\u092A\u0949\u0936\u0924\u094B (\u092A\u0941\u0936\u0924\u094B)";
			case "pt": return "\u092A\u0941\u0930\u094D\u0924\u0941\u0917\u0940";
			case "qu": return "\u0915\u094D\u0935\u0947\u0936\u0941\u0906";
			case "raj": return "\u0930\u093E\u091C\u0947\u0938\u094D\u0925\u093E\u0928\u0940";
			case "rm": return "\u0930\u0939\u0947\u092F\u094D\u091F\u094B-\u0930\u094B\u092E\u093E\u0928\u094D\u0938";
			case "rn": return "\u0915\u093F\u0930\u0942\u0928\u094D\u0926\u0940";
			case "ro": return "\u0930\u0942\u092E\u093E\u0928\u0940\u092F\u0928\u094D";
			case "ru": return "\u0930\u0941\u0938\u0940";
			case "rw": return "\u0915\u093F\u0928\u094D\u092F\u093E\u0930\u0935\u093E\u0923\u094D\u0921\u093E";
			case "sa": return "\u0938\u0902\u0938\u094D\u0915\u0943\u0924";
			case "sd": return "\u0938\u093F\u0928\u094D\u0927\u0940";
			case "sg": return "\u0938\u093E\u0901\u0917\u094D\u0930\u094B";
			case "sh": return "\u0938\u0947\u0930\u094D\u092C\u094B-\u0915\u094D\u0930\u094B\u090F\u0936\u0928\u094D";
			case "si": return "\u0936\u093F\u0902\u0918\u093E\u0932\u0940\u0938\u094D";
			case "sk": return "\u0938\u094D\u0932\u094B\u0935\u093E\u0915\u094D";
			case "sl": return "\u0938\u094D\u0932\u094B\u0935\u0947\u0928\u093F\u092F\u0928\u094D";
			case "sm": return "\u0938\u093E\u092E\u094B\u0928";
			case "sn": return "\u0938\u094B\u0923\u093E";
			case "so": return "\u0938\u094B\u092E\u093E\u0932\u0940";
			case "sq": return "\u0905\u0932\u094D\u092C\u0947\u0928\u093F\u092F\u0928\u094D";
			case "sr": return "\u0938\u0930\u094D\u092C\u093F\u092F\u0928\u094D";
			case "ss": return "\u0938\u0940\u0938\u094D\u0935\u093E\u091F\u093F";
			case "st": return "\u0938\u0947\u0938\u094B\u0925\u094B";
			case "su": return "\u0938\u0941\u0928\u094D\u0926\u093E\u0928\u0940\u0938";
			case "sv": return "\u0938\u094D\u0935\u093F\u0921\u093F\u0936";
			case "sw": return "\u0938\u094D\u0935\u093E\u0939\u093F\u0932\u0940";
			case "ta": return "\u0924\u092E\u093F\u0932";
			case "te": return "\u0924\u0947\u0932\u0947\u0917\u0941";
			case "tg": return "\u0924\u093E\u091C\u093F\u0915\u094D";
			case "th": return "\u0925\u093E\u0908";
			case "ti": return "\u0924\u093F\u0917\u094D\u0930\u0940\u0928\u094D\u092F\u093E";
			case "tk": return "\u0924\u0941\u0915\u094D\u0930\u092E\u0947\u0928";
			case "tl": return "\u0924\u093E\u0917\u093E\u0932\u094B\u0917";
			case "tn": return "\u0938\u0947\u0924\u094D\u0938\u094D\u0935\u093E\u0928\u093E";
			case "to": return "\u091F\u094B\u0902\u0917\u093E";
			case "tr": return "\u0924\u0941\u0915\u094D\u0930\u0940\u0936";
			case "ts": return "\u0938\u094B\u0902\u0917\u093E";
			case "tt": return "\u091F\u093E\u091F\u0930";
			case "tw": return "\u091F\u094D\u0935\u0940";
			case "ug": return "\u0909\u0908\u0918\u0941\u0930";
			case "uk": return "\u092F\u0942\u0915\u094D\u0930\u0947\u0928\u093F\u092F\u0928\u094D";
			case "ur": return "\u090A\u0930\u094D\u0926\u0941";
			case "uz": return "\u0909\u095B\u092C\u0947\u0915\u094D";
			case "vi": return "\u0935\u093F\u092F\u0947\u0924\u0928\u093E\u092E\u0940\u000D";
			case "vo": return "\u0935\u094B\u0932\u093E\u092A\u0941\u0915";
			case "wo": return "\u0935\u094B\u0932\u094B\u092B";
			case "xh": return "\u0937\u094B\u0938\u093E";
			case "yi": return "\u092F\u0947\u0939\u0941\u0926\u0940";
			case "yo": return "\u092F\u094B\u0930\u0942\u092C\u093E";
			case "za": return "\u095B\u0941\u0906\u0902\u0917";
			case "zh": return "\u091A\u0940\u0928\u0940";
			case "zu": return "\u095B\u0941\u0932\u0942";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "IN": return "\u092d\u093e\u0930\u0924";
			case "GR": return "\u0917\u094D\u0930\u0940\u0938";
			case "GT": return "\u0917\u094b\u0924\u0947\u0926\u093e\u0932\u093e";
			case "UY": return "\u0909\u0930\u0942\u0917\u0941\u090F";
			case "GW": return "\u0917\u0940\u0928\u0940-\u092c\u093f\u0938\u093e\u0909";
			case "GY": return "\u0917\u0941\u092f\u093e\u0928\u093e";
			case "VA": return "\u0935\u093e\u0945\u091f\u093f\u0915\u0928";
			case "TM": return "\u0924\u0941\u0915\u094d\u0930\u092e\u0947\u0928\u093f\u0938\u094d\u0924\u093e\u0928";
			case "VG": return "\u092c\u094d\u0930\u093f\u091f\u093f\u0936 ";
			case "VI": return "\u0908\u0909, \u090f\u0938 ";
			case "TL": return "\u0908\u0938\u094d\u091f \u091f\u093f\u092e\u094b\u0930";
			case "VU": return "\u0938\u093e\u0928\u0941\u0905\u0924\u0941";
			case "HN": return "\u0939\u093e\u0945\u0928\u0921\u0941\u0930\u093e\u0938";
			case "HR": return "\u0915\u094d\u0930\u094b\u0936\u0940\u092f\u093e";
			case "HT": return "\u0939\u093e\u0908\u091f\u0940";
			case "HU": return "\u0939\u0902\u0917\u0947\u0930\u0940";
			case "PE": return "\u092a\u0947\u0930\u0942";
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

}; // class CID0039

public class CNhi : CID0039
{
	public CNhi() : base() {}

}; // class CNhi

}; // namespace I18N.Other
