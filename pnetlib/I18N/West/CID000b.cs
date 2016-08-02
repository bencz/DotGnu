/*
 * CID000b.cs - fi culture handler.
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

// Generated from "fi.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID000b : RootCulture
{
	public CID000b() : base(0x000B) {}
	public CID000b(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "fi";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "fin";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "FIN";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "fi";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"su", "ma", "ti", "ke", "to", "pe", "la"};
			dfi.DayNames = new String[] {"sunnuntai", "maanantai", "tiistai", "keskiviikko", "torstai", "perjantai", "lauantai"};
			dfi.AbbreviatedMonthNames = new String[] {"tammi", "helmi", "maalis", "huhti", "touko", "kes\u00E4", "hein\u00E4", "elo", "syys", "loka", "marras", "joulu", ""};
			dfi.MonthNames = new String[] {"tammikuu", "helmikuu", "maaliskuu", "huhtikuu", "toukokuu", "kes\u00E4kuu", "hein\u00E4kuu", "elokuu", "syyskuu", "lokakuu", "marraskuu", "joulukuu", ""};
			dfi.DateSeparator = ".";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "d. MMMM'ttt 'yyyy";
			dfi.LongTimePattern = "HH:mm:ss z";
			dfi.ShortDatePattern = "d.M.yyyy";
			dfi.ShortTimePattern = "HH:mm";
			dfi.FullDateTimePattern = "d. MMMM'ttt 'yyyy HH:mm:ss z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:d.M.yyyy",
				"D:d. MMMM'ttt 'yyyy",
				"f:d. MMMM'ttt 'yyyy HH:mm:ss z",
				"f:d. MMMM'ttt 'yyyy HH:mm:ss z",
				"f:d. MMMM'ttt 'yyyy HH:mm:ss",
				"f:d. MMMM'ttt 'yyyy HH:mm",
				"F:d. MMMM'ttt 'yyyy HH:mm:ss",
				"g:d.M.yyyy HH:mm:ss z",
				"g:d.M.yyyy HH:mm:ss z",
				"g:d.M.yyyy HH:mm:ss",
				"g:d.M.yyyy HH:mm",
				"G:d.M.yyyy HH:mm:ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:HH:mm:ss z",
				"t:HH:mm:ss z",
				"t:HH:mm:ss",
				"t:HH:mm",
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
			nfi.CurrencyGroupSeparator = "\u00A0";
			nfi.NumberGroupSeparator = "\u00A0";
			nfi.PercentGroupSeparator = "\u00A0";
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
			case "ar": return "arabia";
			case "az": return "azerbaizani";
			case "ba": return "baski";
			case "be": return "valkoven\u00e4j\u00e4";
			case "bg": return "bulgaria";
			case "bh": return "bihari";
			case "bn": return "bengali";
			case "ca": return "katalaani";
			case "cs": return "tsekki";
			case "da": return "tanska";
			case "de": return "saksa";
			case "el": return "kreikka";
			case "en": return "englanti";
			case "es": return "espanja";
			case "et": return "viro";
			case "fa": return "farsi";
			case "fi": return "suomi";
			case "fr": return "ranska";
			case "he": return "heprea";
			case "hi": return "hindi";
			case "hr": return "kroatia";
			case "hu": return "unkari";
			case "id": return "indonesia";
			case "it": return "italia";
			case "ja": return "japani";
			case "ka": return "georgia";
			case "kk": return "kazakki";
			case "km": return "khmer";
			case "kn": return "kannada";
			case "ko": return "korea";
			case "ku": return "kurdi";
			case "la": return "latinalainen";
			case "lt": return "liettua";
			case "lv": return "latvia";
			case "mk": return "makedonia";
			case "mr": return "marathi";
			case "my": return "burma";
			case "nl": return "hollanti";
			case "no": return "norja";
			case "pl": return "puola";
			case "pt": return "portugali";
			case "ro": return "romania";
			case "ru": return "ven\u00e4j\u00e4";
			case "sk": return "slovakia";
			case "sl": return "slovenia";
			case "sq": return "albania";
			case "sr": return "serbia";
			case "sv": return "ruotsi";
			case "sw": return "swahili";
			case "te": return "telugu";
			case "th": return "thai";
			case "tk": return "tagalog";
			case "tr": return "turkki";
			case "uk": return "ukraina";
			case "ur": return "urdu";
			case "uz": return "uzbekki";
			case "zh": return "kiina";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "AE": return "Yhdistyneet Arabiemiraatit";
			case "AT": return "It\u00e4valta";
			case "BA": return "Bosnia";
			case "BE": return "Belgia";
			case "BR": return "Brasilia";
			case "BY": return "Valko-Ven\u00e4j\u00e4";
			case "CA": return "Kanada";
			case "CH": return "Sveitsi";
			case "CN": return "Kiina";
			case "CO": return "Kolumbia";
			case "CZ": return "Tsekin tasavalta";
			case "DE": return "Saksa";
			case "DK": return "Tanska";
			case "DO": return "Dominikaaninen tasavalta";
			case "EC": return "Equador";
			case "EE": return "Viro";
			case "EG": return "Egypti";
			case "ES": return "Espanja";
			case "FI": return "Suomi";
			case "FR": return "Ranska";
			case "GB": return "Iso-Britannia";
			case "GR": return "Kreikka";
			case "HR": return "Kroatia";
			case "HU": return "Unkari";
			case "HK": return "Hongknog, erit.hall.alue";
			case "IE": return "Irlanti";
			case "IN": return "Intia";
			case "IS": return "Islanti";
			case "IT": return "Italia";
			case "JO": return "Jordania";
			case "JP": return "Japani";
			case "KR": return "Korea";
			case "LA": return "Latinalainen Amerikka";
			case "LB": return "Libanon";
			case "LT": return "Liettua";
			case "LU": return "Luxemburg";
			case "MA": return "Marokko";
			case "MK": return "Makedonia (FYR)";
			case "MO": return "Macao, erit.hall.alue";
			case "MX": return "Meksiko";
			case "NL": return "Alankomaat";
			case "NO": return "Norja";
			case "NZ": return "Uusi Seelanti";
			case "PL": return "Puola";
			case "PT": return "Portugali";
			case "RU": return "Ven\u00e4j\u00e4";
			case "SA": return "Saudi-Arabia";
			case "SE": return "Ruotsi";
			case "SY": return "Syyria";
			case "TH": return "Thaimaa";
			case "TR": return "Turkki";
			case "UA": return "Ukraina";
			case "US": return "Yhdysvallat";
			case "YE": return "Jemen";
			case "ZA": return "Etel\u00e4-Afrikka";
		}
		return base.ResolveCountry(name);
	}

	private class PrivateTextInfo : _I18NTextInfo
	{
		public PrivateTextInfo(int culture) : base(culture) {}

		public override int EBCDICCodePage
		{
			get
			{
				return 20278;
			}
		}
		public override int OEMCodePage
		{
			get
			{
				return 850;
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

}; // class CID000b

public class CNfi : CID000b
{
	public CNfi() : base() {}

}; // class CNfi

}; // namespace I18N.West
