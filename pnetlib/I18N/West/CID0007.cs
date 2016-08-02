/*
 * CID0007.cs - de culture handler.
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

// Generated from "de.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0007 : RootCulture
{
	public CID0007() : base(0x0007) {}
	public CID0007(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "de";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "deu";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "DEU";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "de";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AMDesignator = "vorm.";
			dfi.PMDesignator = "nachm.";
			dfi.AbbreviatedDayNames = new String[] {"So", "Mo", "Di", "Mi", "Do", "Fr", "Sa"};
			dfi.DayNames = new String[] {"Sonntag", "Montag", "Dienstag", "Mittwoch", "Donnerstag", "Freitag", "Samstag"};
			dfi.AbbreviatedMonthNames = new String[] {"Jan", "Feb", "Mrz", "Apr", "Mai", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dez", ""};
			dfi.MonthNames = new String[] {"Januar", "Februar", "M\u00E4rz", "April", "Mai", "Juni", "Juli", "August", "September", "Oktober", "November", "Dezember", ""};
			dfi.DateSeparator = ".";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "d. MMMM yyyy";
			dfi.LongTimePattern = "HH:mm:ss z";
			dfi.ShortDatePattern = "dd.MM.yy";
			dfi.ShortTimePattern = "HH:mm";
			dfi.FullDateTimePattern = "dddd, d. MMMM yyyy H:mm' Uhr 'z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:dd.MM.yy",
				"D:dddd, d. MMMM yyyy",
				"f:dddd, d. MMMM yyyy H:mm' Uhr 'z",
				"f:dddd, d. MMMM yyyy HH:mm:ss z",
				"f:dddd, d. MMMM yyyy HH:mm:ss",
				"f:dddd, d. MMMM yyyy HH:mm",
				"F:dddd, d. MMMM yyyy HH:mm:ss",
				"g:dd.MM.yy H:mm' Uhr 'z",
				"g:dd.MM.yy HH:mm:ss z",
				"g:dd.MM.yy HH:mm:ss",
				"g:dd.MM.yy HH:mm",
				"G:dd.MM.yy HH:mm:ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:H:mm' Uhr 'z",
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
			case "ab": return "Abchasisch";
			case "sq": return "Albanisch";
			case "am": return "Amharisch";
			case "ar": return "Arabisch";
			case "hy": return "Armenisch";
			case "az": return "Aserbaidschanisch";
			case "eu": return "Baskisch";
			case "bn": return "Bengalisch";
			case "dz": return "Bhutanisch";
			case "br": return "Bretonisch";
			case "bg": return "Bulgarisch";
			case "my": return "Burmesisch";
			case "be": return "Wei\u00dfrussisch";
			case "km": return "Kkambodschanisch";
			case "ca": return "Katalanisch";
			case "zh": return "Chinesisch";
			case "co": return "Korsisch";
			case "hr": return "Kroatisch";
			case "cs": return "Tschechisch";
			case "da": return "D\u00e4nisch";
			case "nl": return "Holl\u00e4ndisch";
			case "en": return "Englisch";
			case "et": return "Estnisch";
			case "fo": return "F\u00e4r\u00f6isch";
			case "fj": return "Fidschianisch";
			case "fi": return "Finnisch";
			case "fr": return "Franz\u00f6sisch";
			case "fy": return "Frisisch";
			case "gl": return "Galizisch";
			case "ka": return "Georgisch";
			case "de": return "Deutsch";
			case "el": return "Griechisch";
			case "kl": return "Gr\u00f6nl\u00e4ndisch";
			case "he": return "Hebr\u00e4isch";
			case "hu": return "Ungarisch";
			case "is": return "Isl\u00e4ndisch";
			case "id": return "Indonesisch";
			case "ga": return "Irisch";
			case "it": return "Italienisch";
			case "ja": return "Japanisch";
			case "jv": return "Javanesisch";
			case "ks": return "Kaschmirisch";
			case "kk": return "Kasachisch";
			case "ky": return "Kirgisisch";
			case "ko": return "Koreanisch";
			case "ku": return "Kurdisch";
			case "la": return "Latein";
			case "lv": return "Lettisch";
			case "lt": return "Litauisch";
			case "mk": return "Mazedonisch";
			case "mt": return "Maltesisch";
			case "mo": return "Moldawisch";
			case "mn": return "Mongolisch";
			case "na": return "Nauruisch";
			case "nb": return "Norwegisch Bokm\u00e5l";
			case "ne": return "Nepalesisch";
			case "nn": return "Norwegisch Nynorsk";
			case "no": return "Norwegisch";
			case "fa": return "Persisch";
			case "pl": return "Polnisch";
			case "pt": return "Portugiesisch";
			case "rm": return "R\u00e4toromanisch";
			case "ro": return "Rum\u00e4nisch";
			case "ru": return "Russisch";
			case "sm": return "Samoanisch";
			case "gd": return "Schottisch-G\u00e4lisch";
			case "sr": return "Serbisch";
			case "sh": return "Serbo-Kroatisch";
			case "sk": return "Slowakisch";
			case "sl": return "Slowenisch";
			case "so": return "Somalisch";
			case "es": return "Spanisch";
			case "sv": return "Schwedisch";
			case "tg": return "Tadschikisch";
			case "bo": return "Tibetisch";
			case "tr": return "T\u00fcrkisch";
			case "tk": return "T\u00fcrkmenisch";
			case "uk": return "Ukrainisch";
			case "uz": return "Usbekisch";
			case "vi": return "Vietnamesisch";
			case "cy": return "Walisisch";
			case "yi": return "Jiddisch";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "EG": return "\u00c4gypten";
			case "AL": return "Albanien";
			case "DZ": return "Algerien";
			case "AG": return "Antigua und Barbuda";
			case "GQ": return "\u00c4quatorialguinea";
			case "AR": return "Argentinien";
			case "AM": return "Armenien";
			case "AZ": return "Aserbaidschan";
			case "ET": return "\u00c4thiopien";
			case "AU": return "Australien";
			case "BD": return "Bangladesch";
			case "BE": return "Belgien";
			case "BO": return "Bolivien";
			case "BA": return "Bosnien und Herzegowina";
			case "BW": return "Botsuana";
			case "BR": return "Brasilien";
			case "BN": return "Brunei Darussalam";
			case "BG": return "Bulgarien";
			case "CI": return "C\u00f4te d\u0092Ivoire";
			case "DK": return "D\u00e4nemark";
			case "DE": return "Deutschland";
			case "DO": return "Dominikanische Republik";
			case "DJ": return "Dschibuti";
			case "EE": return "Estland";
			case "FJ": return "Fidschi";
			case "FI": return "Finnland";
			case "FR": return "Frankreich";
			case "GA": return "Gabun";
			case "GE": return "Georgien";
			case "GR": return "Griechenland";
			case "IN": return "Indien";
			case "ID": return "Indonesien";
			case "IQ": return "Irak";
			case "IE": return "Irland";
			case "IS": return "Island";
			case "IT": return "Italien";
			case "JM": return "Jamaika";
			case "YE": return "Jemen";
			case "JO": return "Jordanien";
			case "YU": return "Jugoslawien";
			case "KH": return "Kambodscha";
			case "CM": return "Kamerun";
			case "CA": return "Kanada";
			case "CV": return "Kap Verde";
			case "KZ": return "Kasachstan";
			case "QA": return "Katar";
			case "KE": return "Kenia";
			case "KG": return "Kirgisistan";
			case "CO": return "Kolumbien";
			case "KM": return "Komoren";
			case "CG": return "Kongo";
			case "CD": return "Demokratische Republik Kongo";
			case "KP": return "Demokratische Volksrepublik Korea";
			case "KR": return "Republik Korea";
			case "HR": return "Kroatien";
			case "CU": return "Kuba";
			case "LV": return "Lettland";
			case "LB": return "Libanon";
			case "LY": return "Libyen";
			case "LT": return "Litauen";
			case "LU": return "Luxemburg";
			case "MG": return "Madagaskar";
			case "MV": return "Malediven";
			case "MA": return "Marokko";
			case "MH": return "Marshallinseln";
			case "MR": return "Mauretanien";
			case "MK": return "Mazedonien";
			case "MX": return "Mexiko";
			case "FM": return "Mikronesien";
			case "MD": return "Moldawien";
			case "MN": return "Mongolei";
			case "MZ": return "Mosambik";
			case "NZ": return "Neuseeland";
			case "NL": return "Niederlande";
			case "NO": return "Norwegen";
			case "AT": return "\u00d6sterreich";
			case "PG": return "Papua-Neuguinea";
			case "PH": return "Philippinen";
			case "PL": return "Polen";
			case "RW": return "Ruanda";
			case "RO": return "Rum\u00e4nien";
			case "RU": return "Russland";
			case "KN": return "St. Kitts und Nevis";
			case "LC": return "St. Lucia";
			case "VC": return "St. Vincent und die Grenadinen";
			case "SB": return "Salomonen";
			case "ZM": return "Sambia";
			case "ST": return "S\u00e3o Tom\u00e9 und Pr\u00edncipe";
			case "SA": return "Saudi-Arabien";
			case "SE": return "Schweden";
			case "CH": return "Schweiz";
			case "SC": return "Seychellen";
			case "ZW": return "Simbabwe";
			case "SG": return "Singapur";
			case "SK": return "Slowakei";
			case "SI": return "Slowenien";
			case "ES": return "Spanien";
			case "ZA": return "S\u00fcdafrika";
			case "SZ": return "Swasiland";
			case "SY": return "Syrien";
			case "TJ": return "Tadschikistan";
			case "TZ": return "Tansania";
			case "TT": return "Trinidad und Tobago";
			case "TD": return "Tschad";
			case "CZ": return "Tschechien";
			case "TN": return "Tunesien";
			case "TR": return "T\u00fcrkei";
			case "HU": return "Ungarn";
			case "UZ": return "Usbekistan";
			case "VA": return "Vatikanstadt";
			case "AE": return "Vereinigte Arabische Emirate";
			case "GB": return "Vereinigtes K\u00f6nigreich";
			case "US": return "Vereinigte Staaten";
			case "CF": return "Zentralafrikanische Republik";
			case "CY": return "Zypern";
			case "UM": return "Amerikanisch-Ozeanien";
			case "AS": return "Amerikanisch-Samoa";
			case "VI": return "Amerikanische Jungferninseln";
			case "AQ": return "Antarktis";
			case "SJ": return "Svalbard und Jan Mayen";
			case "BV": return "Bouvetinsel";
			case "VG": return "Britische Jungferninseln";
			case "IO": return "Britisches Territorium im Indischen Ozean";
			case "CK": return "Cookinseln";
			case "FK": return "Falklandinseln";
			case "FO": return "F\u00e4r\u00f6er";
			case "GF": return "Franz\u00f6sisch-Guayana";
			case "PF": return "Franz\u00f6sisch-Polynesien";
			case "TF": return "Franz\u00f6sische S\u00fcd- und Antarktisgebiete";
			case "GL": return "Gr\u00f6nland";
			case "HM": return "Heard und McDonaldinseln";
			case "KY": return "Kaimaninseln";
			case "CC": return "Kokosinseln";
			case "MO": return "Macau S.A.R.,China";
			case "NC": return "Neukaledonien";
			case "AN": return "Niederl\u00e4ndische Antillen";
			case "MP": return "N\u00f6rdliche Marianen";
			case "NF": return "Norfolkinsel";
			case "SH": return "St. Helena";
			case "PM": return "St. Pierre und Miquelon";
			case "GS": return "S\u00fcdgeorgien und die S\u00fcdlichen Sandwichinseln";
			case "TC": return "Turks- und Caicosinseln";
			case "WF": return "Wallis und Futuna";
			case "CX": return "Weihnachtsinsel";
			case "SP": return "Serbien";
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
				return 20273;
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

}; // class CID0007

public class CNde : CID0007
{
	public CNde() : base() {}

}; // class CNde

}; // namespace I18N.West
