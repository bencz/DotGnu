/*
 * CID0057.cs - kok culture handler.
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

// Generated from "kok.txt".

namespace I18N.Other
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0057 : RootCulture
{
	public CID0057() : base(0x0057) {}
	public CID0057(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "kok";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "kok";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "KNK";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "kok";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AMDesignator = "\u092e.\u092a\u0942.";
			dfi.PMDesignator = "\u092e.\u0928\u0902.";
			dfi.AbbreviatedDayNames = new String[] {"\u0930\u0935\u093f", "\u0938\u094b\u092e", "\u092e\u0902\u0917\u0933", "\u092c\u0941\u0927", "\u0917\u0941\u0930\u0941", "\u0936\u0941\u0915\u094d\u0930", "\u0936\u0928\u093f"};
			dfi.DayNames = new String[] {"\u0906\u0926\u093f\u0924\u094d\u092f\u0935\u093e\u0930", "\u0938\u094b\u092e\u0935\u093e\u0930", "\u092e\u0902\u0917\u0933\u093e\u0930", "\u092c\u0941\u0927\u0935\u093e\u0930", "\u0917\u0941\u0930\u0941\u0935\u093e\u0930", "\u0936\u0941\u0915\u094d\u0930\u0935\u093e\u0930", "\u0936\u0928\u093f\u0935\u093e\u0930"};
			dfi.AbbreviatedMonthNames = new String[] {"\u091c\u093e\u0928\u0947\u0935\u093e\u0930\u0940", "\u092b\u0947\u092c\u0943\u0935\u093e\u0930\u0940", "\u092e\u093e\u0930\u094d\u091a", "\u090f\u092a\u094d\u0930\u093f\u0932", "\u092e\u0947", "\u091c\u0942\u0928", "\u091c\u0941\u0932\u0948", "\u0913\u0917\u0938\u094d\u091f", "\u0938\u0947\u092a\u094d\u091f\u0947\u0902\u092c\u0930", "\u0913\u0915\u094d\u091f\u094b\u092c\u0930", "\u0928\u094b\u0935\u094d\u0939\u0947\u0902\u092c\u0930", "\u0921\u093f\u0938\u0947\u0902\u092c\u0930", ""};
			dfi.MonthNames = new String[] {"\u091c\u093e\u0928\u0947\u0935\u093e\u0930\u0940", "\u092b\u0947\u092c\u094d\u0930\u0941\u0935\u093e\u0930\u0940", "\u092e\u093e\u0930\u094d\u091a", "\u090f\u092a\u094d\u0930\u093f\u0932", "\u092e\u0947", "\u091c\u0942\u0928", "\u091c\u0941\u0932\u0948", "\u0913\u0917\u0938\u094d\u091f", "\u0938\u0947\u092a\u094d\u091f\u0947\u0902\u092c\u0930", "\u0913\u0915\u094d\u091f\u094b\u092c\u0930", "\u0928\u094b\u0935\u094d\u0939\u0947\u0902\u092c\u0930", "\u0921\u093f\u0938\u0947\u0902\u092c\u0930", ""};
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
			case "aa": return "\u0905\u092b\u093e\u0930";
			case "ab": return "\u0905\u092c\u0916\u0947\u091c\u093c\u093f\u092f\u0928";
			case "af": return "\u0905\u092b\u094d\u0930\u093f\u0915\u093e\u0928\u094d\u0938";
			case "am": return "\u0905\u092e\u0939\u093e\u0930\u093f\u0915\u094d";
			case "ar": return "\u0905\u0930\u0947\u092c\u093f\u0915\u094d";
			case "as": return "\u0905\u0938\u093e\u092e\u0940";
			case "ay": return "\u0910\u092e\u0930\u093e";
			case "az": return "\u0905\u091c\u093c\u0930\u092c\u0948\u091c\u093e\u0928\u0940";
			case "ba": return "\u092c\u0937\u094d\u0915\u093f\u0930";
			case "be": return "\u092c\u0948\u0932\u094b\u0930\u0941\u0938\u093f\u092f\u0928\u094d";
			case "bg": return "\u092c\u0932\u094d\u0917\u0947\u0930\u093f\u092f\u0928";
			case "bh": return "\u092c\u0940\u0939\u093e\u0930\u0940";
			case "bi": return "\u092c\u093f\u0938\u0932\u092e\u093e";
			case "bn": return "\u092c\u0902\u0917\u093e\u0932\u0940";
			case "bo": return "\u0924\u093f\u092c\u0947\u0924\u093f\u092f\u0928";
			case "br": return "\u092c\u094d\u0930\u0947\u091f\u0928";
			case "ca": return "\u0915\u091f\u0932\u093e\u0928";
			case "co": return "\u0915\u094b\u0930\u094d\u0936\u093f\u092f\u0928";
			case "cs": return "\u091c\u093c\u0947\u0915\u094d";
			case "cy": return "\u0935\u0947\u0933\u094d\u0937\u094d";
			case "da": return "\u0921\u093e\u0928\u093f\u0937";
			case "de": return "\u091c\u0930\u094d\u092e\u0928";
			case "dz": return "\u092d\u0942\u091f\u093e\u0928\u0940";
			case "el": return "\u0917\u094d\u0930\u0940\u0915\u094d";
			case "en": return "\u0906\u0902\u0917\u094d\u0932";
			case "eo": return "\u0907\u0938\u094d\u092a\u0930\u093e\u0928\u094d\u091f\u094b";
			case "es": return "\u0938\u094d\u092a\u093e\u0928\u093f\u0937";
			case "et": return "\u0907\u0938\u094d\u091f\u094b\u0928\u093f\u092f\u0928\u094d";
			case "eu": return "\u092c\u093e\u0938\u094d\u0915";
			case "fa": return "\u092a\u0930\u094d\u0937\u093f\u092f\u0928\u094d";
			case "fi": return "\u092b\u093f\u0928\u094d\u0928\u093f\u0937\u094d";
			case "fj": return "\u092b\u093f\u091c\u0940";
			case "fo": return "\u092b\u0947\u0930\u094b\u0938\u094d";
			case "fr": return "\u092b\u094d\u0930\u0947\u0928\u094d\u091a";
			case "fy": return "\u092b\u094d\u0930\u093f\u0936\u093f\u092f\u0928\u094d";
			case "ga": return "\u0910\u0930\u093f\u0937";
			case "gd": return "\u0938\u094d\u0915\u093e\u091f\u0938\u094d \u0917\u0947\u0932\u093f\u0915\u094d";
			case "gl": return "\u0917\u0947\u0932\u0940\u0936\u093f\u092f\u0928";
			case "gn": return "\u0917\u094c\u0930\u093e\u0928\u0940";
			case "gu": return "\u0917\u0941\u091c\u0930\u093e\u0924\u0940";
			case "ha": return "\u0939\u094c\u0938\u093e";
			case "he": return "\u0939\u0947\u092c\u094d\u0930\u0941";
			case "hi": return "\u0939\u093f\u0928\u094d\u0926\u0940";
			case "hr": return "\u0915\u094d\u0930\u094b\u092f\u0947\u0937\u093f\u092f\u0928\u094d";
			case "hu": return "\u0939\u0902\u0917\u0947\u0930\u093f\u092f\u0928\u094d";
			case "hy": return "\u0906\u0930\u094d\u092e\u0940\u0928\u093f\u092f\u0928\u094d";
			case "ia": return "\u0907\u0928\u094d\u091f\u0930\u0932\u093f\u0902\u0917\u094d\u0935\u093e";
			case "id": return "\u0907\u0928\u094d\u0921\u094b\u0928\u0947\u0937\u093f\u092f\u0928";
			case "ie": return "\u0907\u0928\u094d\u091f\u0930\u0932\u093f\u0902\u0917\u094d";
			case "ik": return "\u0907\u0928\u0942\u092a\u0947\u092f\u093e\u0915\u094d";
			case "is": return "\u0906\u0908\u0938\u094d\u0932\u093e\u0928\u094d\u0921\u093f\u0915";
			case "it": return "\u0907\u091f\u093e\u0932\u093f\u092f\u0928";
			case "iu": return "\u0907\u0928\u094d\u092f\u0941\u0915\u091f\u094d\u091f";
			case "ja": return "\u091c\u093e\u092a\u0928\u0940\u0938\u094d";
			case "jv": return "\u091c\u093e\u0935\u0928\u0940\u0938\u094d";
			case "ka": return "\u091c\u093e\u0930\u094d\u091c\u093f\u092f\u0928\u094d";
			case "kk": return "\u0915\u091c\u093c\u0916\u094d";
			case "kl": return "\u0917\u094d\u0930\u0940\u0928\u0932\u093e\u0928\u094d\u0921\u093f\u0915";
			case "km": return "\u0915\u0902\u092c\u094b\u0921\u093f\u092f\u0928";
			case "kn": return "\u0915\u0928\u094d\u0928\u0921\u093e";
			case "ko": return "\u0915\u094b\u0930\u093f\u092f\u0928\u094d";
			case "kok": return "\u0915\u094b\u0902\u0915\u0923\u0940";
			case "ks": return "\u0915\u0936\u094d\u092e\u0940\u0930\u0940";
			case "ku": return "\u0915\u0941\u0930\u094d\u0926\u093f\u0937";
			case "ky": return "\u0915\u093f\u0930\u094d\u0917\u093f\u091c\u093c";
			case "la": return "\u0932\u093e\u091f\u093f\u0928";
			case "ln": return "\u0932\u093f\u0902\u0917\u093e\u0932\u093e";
			case "lo": return "\u0932\u093e\u0913\u0924\u093f\u092f\u0928\u094d";
			case "lt": return "\u0932\u093f\u0925\u0941\u0906\u0928\u093f\u092f\u0928\u094d";
			case "lv": return "\u0932\u093e\u091f\u094d\u0935\u093f\u092f\u0928\u094d (\u0932\u0947\u091f\u094d\u091f\u093f\u0937\u094d)";
			case "mg": return "\u092e\u0932\u093e\u0917\u0938\u0940";
			case "mi": return "\u092e\u093e\u0913\u0930\u0940";
			case "mk": return "\u092e\u0938\u0940\u0921\u094b\u0928\u093f\u092f\u0928\u094d";
			case "ml": return "\u092e\u0933\u093f\u092f\u093e\u0933\u092e";
			case "mn": return "\u092e\u0902\u0917\u094b\u0932\u093f\u092f\u0928\u094d";
			case "mo": return "\u092e\u094b\u0932\u094d\u0921\u093e\u0935\u093f\u092f\u0928\u094d";
			case "mr": return "\u092e\u0930\u093e\u0920\u0940";
			case "ms": return "\u092e\u0932\u092f";
			case "mt": return "\u092e\u093e\u0932\u0924\u0940\u0938\u094d";
			case "my": return "\u092c\u0930\u094d\u092e\u0940\u091c\u093c\u094d";
			case "na": return "\u0928\u094c\u0930\u094b";
			case "ne": return "\u0928\u0947\u092a\u093e\u0933\u0940";
			case "nl": return "\u0921\u091a\u094d";
			case "no": return "\u0928\u094b\u0930\u094d\u0935\u0947\u091c\u093f\u092f\u0928";
			case "oc": return "\u0913\u0938\u093f\u091f\u093e\u0928\u094d";
			case "om": return "\u0913\u0930\u094b\u092e\u094b (\u0905\u092b\u093e\u0928)";
			case "or": return "\u0913\u0930\u093f\u092f\u093e";
			case "pa": return "\u092a\u0902\u091c\u093e\u092c\u0940";
			case "pl": return "\u092a\u094b\u0932\u093f\u0937";
			case "ps": return "\u092a\u093e\u0937\u094d\u091f\u094b (\u092a\u0941\u0937\u094d\u091f\u094b)";
			case "pt": return "\u092a\u094b\u0930\u094d\u091a\u0941\u0917\u0940\u091c\u093c\u094d";
			case "qu": return "\u0915\u094d\u0935\u0947\u091a\u094d\u0935\u093e";
			case "rm": return "\u0930\u0939\u091f\u094b-\u0930\u094b\u092e\u093e\u0928\u094d\u0938\u094d";
			case "rn": return "\u0915\u093f\u0930\u0941\u0928\u094d\u0926\u0940";
			case "ro": return "\u0930\u094b\u092e\u093e\u0928\u093f\u092f\u0928\u094d";
			case "ru": return "\u0930\u0937\u094d\u092f\u0928\u094d";
			case "rw": return "\u0915\u093f\u0928\u094d\u092f\u093e\u0930\u094d\u0935\u093e\u0928\u094d\u0921\u093e";
			case "sa": return "\u0938\u0902\u0938\u094d\u0915\u0943\u0924";
			case "sd": return "\u0938\u093f\u0902\u0927\u0940";
			case "sg": return "\u0938\u093e\u0902\u0917\u094d\u0930\u094b";
			case "sh": return "\u0938\u0947\u0930\u094d\u092c\u094b-\u0915\u094d\u0930\u094b\u092f\u0947\u0937\u093f\u092f\u0928\u094d";
			case "si": return "\u0938\u093f\u0928\u094d\u0939\u0932\u0940\u0938\u094d";
			case "sk": return "\u0938\u094d\u0932\u094b\u0935\u093e\u0915";
			case "sl": return "\u0938\u094d\u0932\u094b\u0935\u0947\u0928\u093f\u092f\u0928\u094d";
			case "sm": return "\u0938\u092e\u094b\u0928";
			case "sn": return "\u0936\u094b\u0928\u093e";
			case "so": return "\u0938\u094b\u092e\u093e\u0933\u0940";
			case "sq": return "\u0906\u0932\u094d\u092c\u0947\u0928\u093f\u092f\u0928\u094d";
			case "sr": return "\u0938\u0947\u0930\u094d\u092c\u093f\u092f\u0928\u094d";
			case "ss": return "\u0938\u093f\u0938\u094d\u0935\u093e\u0924\u0940";
			case "st": return "\u0938\u0947\u0938\u094b\u0925\u094b";
			case "su": return "\u0938\u0941\u0902\u0926\u0928\u0940\u0938";
			case "sv": return "\u0938\u094d\u0935\u0940\u0926\u0940\u0937";
			case "sw": return "\u0938\u094d\u0935\u093e\u0939\u093f\u0932\u0940";
			case "ta": return "\u0924\u092e\u093f\u0933";
			case "te": return "\u0924\u0947\u0932\u0941\u0917\u0942";
			case "tg": return "\u0924\u091c\u093f\u0915";
			case "th": return "\u0925\u093e\u0908";
			case "ti": return "\u0924\u093f\u0917\u094d\u0930\u093f\u0928\u094d\u092f\u093e";
			case "tk": return "\u0924\u0941\u0930\u094d\u0915\u092e\u0928";
			case "tl": return "\u0924\u0917\u093e\u0932\u094b\u0917";
			case "tn": return "\u0938\u0947\u0924\u094d\u0938\u094d\u0935\u093e\u0928\u093e";
			case "to": return "\u0924\u094b\u0902\u0917\u093e";
			case "tr": return "\u0924\u0941\u0930\u094d\u0915\u093f\u0937";
			case "ts": return "\u0924\u094d\u0938\u094b\u0917\u093e";
			case "tt": return "\u0924\u091f\u093e\u0930";
			case "tw": return "\u0924\u094d\u0935\u093f";
			case "ug": return "\u0909\u0927\u0942\u0930";
			case "uk": return "\u092f\u0941\u0915\u094d\u0930\u0947\u0928\u093f\u092f\u0928\u094d";
			case "ur": return "\u0909\u0930\u094d\u0926\u0942";
			case "uz": return "\u0909\u091c\u093c\u092c\u0947\u0915";
			case "vi": return "\u0935\u093f\u092f\u0924\u094d\u0928\u093e\u092e\u0940\u091c\u093c";
			case "vo": return "\u0913\u0932\u093e\u092a\u0941\u0915";
			case "wo": return "\u0909\u0932\u094b\u092b\u093c";
			case "xh": return "\u091d\u093c\u094c\u0938\u093e";
			case "yi": return "\u0907\u0926\u094d\u0926\u093f\u0937\u094d";
			case "yo": return "\u092f\u0942\u0930\u0941\u092c\u093e";
			case "za": return "\u091d\u094d\u0939\u0941\u0928\u094d\u0917";
			case "zh": return "\u091a\u0940\u0928\u0940\u0938\u094d";
			case "zu": return "\u091c\u0941\u0932\u0942";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "IN": return "\u092D\u093E\u0930\u0924";
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

}; // class CID0057

public class CNkok : CID0057
{
	public CNkok() : base() {}

}; // class CNkok

}; // namespace I18N.Other
