/*
 * CID001d.cs - sv culture handler.
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

// Generated from "sv.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID001d : RootCulture
{
	public CID001d() : base(0x001D) {}
	public CID001d(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "sv";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "swe";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "SVE";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "sv";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"s\u00F6", "m\u00E5", "ti", "on", "to", "fr", "l\u00F6"};
			dfi.DayNames = new String[] {"s\u00F6ndag", "m\u00E5ndag", "tisdag", "onsdag", "torsdag", "fredag", "l\u00F6rdag"};
			dfi.AbbreviatedMonthNames = new String[] {"jan", "feb", "mar", "apr", "maj", "jun", "jul", "aug", "sep", "okt", "nov", "dec", ""};
			dfi.MonthNames = new String[] {"januari", "februari", "mars", "april", "maj", "juni", "juli", "augusti", "september", "oktober", "november", "december", ""};
			dfi.DateSeparator = "-";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "'den 'd MMMM yyyy";
			dfi.LongTimePattern = "HH:mm:ss z";
			dfi.ShortDatePattern = "yyyy-MM-dd";
			dfi.ShortTimePattern = "HH:mm";
			dfi.FullDateTimePattern = "'den 'd MMMM yyyy 'kl 'H:mm z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:yyyy-MM-dd",
				"D:'den 'd MMMM yyyy",
				"f:'den 'd MMMM yyyy 'kl 'H:mm z",
				"f:'den 'd MMMM yyyy HH:mm:ss z",
				"f:'den 'd MMMM yyyy HH:mm:ss",
				"f:'den 'd MMMM yyyy HH:mm",
				"F:'den 'd MMMM yyyy HH:mm:ss",
				"g:yyyy-MM-dd 'kl 'H:mm z",
				"g:yyyy-MM-dd HH:mm:ss z",
				"g:yyyy-MM-dd HH:mm:ss",
				"g:yyyy-MM-dd HH:mm",
				"G:yyyy-MM-dd HH:mm:ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:'kl 'H:mm z",
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
			case "aa": return "afar";
			case "ab": return "abkhaziska";
			case "ace": return "achinese";
			case "ach": return "acholi";
			case "ada": return "adangme";
			case "ae": return "avestiska";
			case "af": return "afrikaans";
			case "ak": return "akan";
			case "akk": return "akkadiska";
			case "ale": return "aleutiska";
			case "am": return "amhariska";
			case "ar": return "arabiska";
			case "arc": return "arameiska";
			case "arn": return "araukanska";
			case "arp": return "arapaho";
			case "arw": return "arawakiska";
			case "as": return "assami";
			case "ast": return "asturiska";
			case "av": return "avariska";
			case "awa": return "awadhi";
			case "ay": return "aymara";
			case "az": return "azerbadzjanska";
			case "ba": return "basjkiriska";
			case "bad": return "banda";
			case "bal": return "baluchi";
			case "bam": return "bambara";
			case "ban": return "balinesiska";
			case "bas": return "basa";
			case "be": return "vitryska";
			case "bej": return "beyja";
			case "bem": return "bemba";
			case "bg": return "bulgariska";
			case "bh": return "bihari";
			case "bho": return "bhojpuri";
			case "bi": return "bislama";
			case "bik": return "bikol";
			case "bin": return "bini";
			case "bla": return "siksika";
			case "bn": return "bengali";
			case "bo": return "tibetanska";
			case "br": return "bretonska";
			case "bra": return "braj";
			case "bs": return "bosniska";
			case "btk": return "batak";
			case "bua": return "buriat";
			case "bug": return "buginesiska";
			case "ca": return "katalanska";
			case "cad": return "caddo";
			case "car": return "karibiska";
			case "ce": return "tjetjenska";
			case "ceb": return "cebuano";
			case "ch": return "chamorro";
			case "chb": return "chibcha";
			case "chg": return "chagatai";
			case "chk": return "chuukesiska";
			case "chm": return "mari";
			case "chn": return "chinook";
			case "cho": return "choctaw";
			case "chr": return "cherokesiska";
			case "chy": return "cheyenne";
			case "co": return "korsiska";
			case "cop": return "koptiska";
			case "cr": return "cree";
			case "cs": return "tjeckiska";
			case "cv": return "tjuvasjiska";
			case "cy": return "walesiska";
			case "da": return "danska";
			case "dak": return "dakota";
			case "day": return "dayak";
			case "de": return "tyska";
			case "del": return "delaware";
			case "dgr": return "dogrib";
			case "din": return "dinka";
			case "doi": return "dogri";
			case "dua": return "duala";
			case "dv": return "maldiviska";
			case "dyu": return "dyula";
			case "dz": return "dzongkha";
			case "ee": return "ewe";
			case "efi": return "efik";
			case "eka": return "ekajuk";
			case "el": return "grekiska";
			case "elx": return "elamitiska";
			case "en": return "engelska";
			case "eo": return "esperanto";
			case "es": return "spanska";
			case "et": return "estniska";
			case "eu": return "baskiska";
			case "ewo": return "ewondo";
			case "fa": return "farsi";
			case "fan": return "fang";
			case "fat": return "fanti";
			case "ff": return "fulani";
			case "fi": return "finska";
			case "fj": return "fidjianska";
			case "fo": return "f\u00E4r\u00F6iska";
			case "fon": return "fon";
			case "fr": return "franska";
			case "fur": return "friuilian";
			case "fy": return "frisiska";
			case "ga": return "irl\u00E4ndsk gaeliska";
			case "gaa": return "g\u00E0";
			case "gay": return "gayo";
			case "gba": return "gbaya";
			case "gd": return "skotsk gaeliska";
			case "gil": return "gilbertesiska; kiribati";
			case "gl": return "galiciska";
			case "gn": return "guaran\u00ED";
			case "gon": return "gondi";
			case "gor": return "gorontalo";
			case "got": return "gotiska";
			case "grb": return "grebo";
			case "gu": return "gujarati";
			case "gv": return "manx gaeliska";
			case "gwi": return "gwich'in";
			case "ha": return "haussa";
			case "hai": return "haida";
			case "haw": return "hawaiiska";
			case "he": return "hebreiska";
			case "hi": return "hindi";
			case "hil": return "hiligaynon";
			case "him": return "himachali";
			case "hmn": return "hmong";
			case "ho": return "hiri motu";
			case "hr": return "kroatiska";
			case "hu": return "ungerska";
			case "hup": return "hupa";
			case "hy": return "armeniska";
			case "hz": return "herero";
			case "iba": return "iban";
			case "id": return "indonesiska";
			case "ig": return "ibo";
			case "ii": return "yi";
			case "ijo": return "ijo";
			case "ik": return "inupiaq";
			case "ilo": return "iloko";
			case "is": return "isl\u00E4ndska";
			case "it": return "italienska";
			case "iu": return "inuktitut";
			case "ja": return "japanska";
			case "jv": return "javanska";
			case "ka": return "georgiska";
			case "kaa": return "karakalpakiska";
			case "kab": return "kabyliska";
			case "kac": return "kachin";
			case "kam": return "kamba";
			case "kar": return "karen";
			case "kaw": return "kawi";
			case "kg": return "kikongo";
			case "kha": return "khasi";
			case "kho": return "sakiska";
			case "ki": return "kikuyu";
			case "kj": return "kuanyama";
			case "kk": return "kazakiska";
			case "kl": return "gr\u00F6nl\u00E4ndska; kalaallisut";
			case "km": return "kambodjanska; khmer";
			case "kmb": return "kinbundu";
			case "kn": return "kanaresiska; kannada";
			case "ko": return "koreanska";
			case "kok": return "konkani";
			case "kos": return "kosreanska";
			case "kpe": return "kpelle";
			case "kr": return "kanuri";
			case "kro": return "kru";
			case "kru": return "kurukh";
			case "ks": return "kashmiri";
			case "ku": return "kurdiska";
			case "kum": return "kumyk";
			case "kut": return "kutenai";
			case "kv": return "kome";
			case "kw": return "korniska";
			case "ky": return "kirgisiska";
			case "la": return "latin";
			case "lah": return "lahnda";
			case "lam": return "lamba";
			case "lb": return "luxemburgiska";
			case "lez": return "lezghien";
			case "lg": return "luganda";
			case "li": return "limburgiska";
			case "ln": return "lingala";
			case "lo": return "laotiska";
			case "lol": return "lolo; mongo";
			case "loz": return "lozi";
			case "lt": return "litauiska";
			case "lu": return "luba-katanga";
			case "lua": return "luba-lulua";
			case "lui": return "luise\u00F1o";
			case "lun": return "lunda";
			case "luo": return "luo";
			case "lus": return "lushai";
			case "lv": return "lettiska";
			case "mad": return "madurese";
			case "mag": return "magahi";
			case "mai": return "maithili";
			case "mak": return "makasar";
			case "man": return "mande";
			case "mas": return "massajiska";
			case "mdr": return "mandar";
			case "men": return "mende";
			case "mg": return "malagassiska";
			case "mh": return "marshalliska";
			case "mi": return "maori";
			case "mic": return "mic-mac";
			case "min": return "minangkabau";
			case "mk": return "makedonska";
			case "ml": return "malayalam";
			case "mn": return "mongoliska";
			case "mnc": return "manchu";
			case "mni": return "manipuri";
			case "mo": return "moldaviska";
			case "moh": return "mohawk";
			case "mos": return "mossi";
			case "mr": return "marathi";
			case "ms": return "malajiska";
			case "mt": return "maltesiska";
			case "mus": return "muskogee";
			case "mwr": return "marwari";
			case "my": return "burmanska";
			case "na": return "nauru";
			case "nah": return "nahuatl; aztekiska";
			case "nap": return "napolitanska";
			case "nb": return "norskt bokm\u00E5l";
			case "nd": return "nord\u00ADndebele";
			case "ne": return "nepali";
			case "new": return "newari";
			case "ng": return "ndonga";
			case "nia": return "nias";
			case "niu": return "niuean";
			case "nl": return "nederl\u00E4ndska; holl\u00E4ndska";
			case "nn": return "ny\u00ADnorsk";
			case "no": return "norska";
			case "nr": return "syd\u00ADndebele";
			case "nso": return "nord\u00ADsotho";
			case "nv": return "navaho";
			case "ny": return "nyanja";
			case "nym": return "nyamwezi";
			case "nyn": return "nyankole";
			case "nyo": return "nyoro";
			case "nzi": return "nzima";
			case "oc": return "provensalska";
			case "oj": return "odjibwa; chippewa";
			case "om": return "oromo; galla";
			case "or": return "oriya";
			case "os": return "ossetiska";
			case "osa": return "osage";
			case "pa": return "panjabi";
			case "pag": return "pangasinan";
			case "pam": return "pampanga";
			case "pap": return "papiamento";
			case "pau": return "palauan";
			case "phn": return "kananeiska; feniciska";
			case "pi": return "pali";
			case "pl": return "polska";
			case "pon": return "ponape";
			case "ps": return "pashto; afghanska";
			case "pt": return "portugisiska";
			case "qu": return "quechua";
			case "raj": return "rajasthani";
			case "rap": return "rapanui";
			case "rar": return "rarotongan";
			case "rm": return "r\u00E4to\u00ADromanska";
			case "rn": return "rundi";
			case "ro": return "rum\u00E4nska";
			case "rom": return "romani";
			case "ru": return "ryska";
			case "rw": return "rwanda; kinjarwanda";
			case "sa": return "sanskrit";
			case "sad": return "sandawe";
			case "sah": return "jakutiska";
			case "sam": return "samaritanska";
			case "sas": return "sasak";
			case "sat": return "santali";
			case "sc": return "sardiska";
			case "sco": return "skotska";
			case "sd": return "sindhi";
			case "se": return "nord\u00ADsamiska";
			case "sel": return "selkup";
			case "sg": return "sango";
			case "shn": return "shan";
			case "si": return "singalesiska";
			case "sid": return "sidamo";
			case "sk": return "slovakiska";
			case "sl": return "slovenska";
			case "sm": return "samoanska";
			case "sn": return "shona; manshona";
			case "snk": return "soninke";
			case "so": return "somali";
			case "sog": return "sogdiska";
			case "son": return "songhai";
			case "sq": return "albanska";
			case "sr": return "serbiska";
			case "srr": return "serer";
			case "ss": return "swati";
			case "st": return "syd\u00ADsotho";
			case "su": return "sundanesiska";
			case "suk": return "sukuma";
			case "sus": return "susu";
			case "sux": return "sumeriska";
			case "sv": return "svenska";
			case "sw": return "swahili";
			case "syr": return "syriska";
			case "ta": return "tamil";
			case "te": return "telugu";
			case "tem": return "temne";
			case "ter": return "tereno";
			case "tet": return "tetum";
			case "tg": return "tadzjikiska";
			case "th": return "thail\u00E4nska";
			case "ti": return "tigrinja";
			case "tig": return "tigr\u00E9";
			case "tiv": return "tivi";
			case "tk": return "turkmeniska";
			case "tkl": return "tokelau";
			case "tl": return "tagalog";
			case "tli": return "tlingit";
			case "tmh": return "tamashek";
			case "tn": return "tswana";
			case "to": return "tonga";
			case "tog": return "tonga-Nyasa";
			case "tpi": return "tok pisin";
			case "tr": return "turkiska";
			case "ts": return "tsonga";
			case "tsi": return "tsimshian";
			case "tt": return "tatariska";
			case "tum": return "tumbuka";
			case "tvl": return "tuvaluan";
			case "tw": return "twi";
			case "ty": return "tahitiska";
			case "tyv": return "tuviniska";
			case "ug": return "uiguriska";
			case "uga": return "ugaritiska";
			case "uk": return "ukrainska";
			case "umb": return "umbundu";
			case "ur": return "urdu";
			case "uz": return "uzbekiska";
			case "vai": return "vai";
			case "ve": return "venda";
			case "vi": return "vietnamesiska";
			case "vot": return "votiska";
			case "wa": return "walloon";
			case "wal": return "walamo";
			case "war": return "waray";
			case "was": return "washo";
			case "wo": return "wolof";
			case "xh": return "xhosa";
			case "yao": return "yao";
			case "yap": return "yap";
			case "yi": return "jiddisch";
			case "yo": return "yoruba";
			case "za": return "zhuang";
			case "zap": return "zapotek";
			case "zen": return "zenaga";
			case "zh": return "kinesiska";
			case "znd": return "zand\u00E9";
			case "zu": return "zulu";
			case "zun": return "zu\u00F1i";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "AE": return "F\u00F6renade Arabemiraten";
			case "AF": return "Afganistan";
			case "AG": return "Antigua och Barbuda";
			case "AL": return "Albanien";
			case "AM": return "Armenien";
			case "AN": return "Nederl\u00E4ndska Antillerna";
			case "AQ": return "Antarktis";
			case "AS": return "Amerikanska Samoa";
			case "AT": return "\u00D6sterrike";
			case "AU": return "Australien";
			case "AZ": return "Azerbajdzjan";
			case "BA": return "Bosnien Herzegovina";
			case "BE": return "Belgien";
			case "BG": return "Bulgarien";
			case "BR": return "Brasilien";
			case "BY": return "Vitryssland";
			case "CA": return "Kanada";
			case "CD": return "Kongo";
			case "CF": return "Centralafrikanska republiken";
			case "CG": return "Kongo";
			case "CH": return "Schweiz";
			case "CI": return "Elfenbenskusten";
			case "CK": return "Cook\u00F6arna";
			case "CM": return "Kamerun";
			case "CN": return "Kina";
			case "CU": return "Kuba";
			case "CV": return "Cap Verde";
			case "CX": return "Jul\u00F6n";
			case "CY": return "Cypern";
			case "CZ": return "Tjeckien";
			case "DE": return "Tyskland";
			case "DK": return "Danmark";
			case "DO": return "Dominikanska republiken";
			case "DZ": return "Algeriet";
			case "EE": return "Estland";
			case "EG": return "Egypten";
			case "EH": return "V\u00E4stra Sahara";
			case "ES": return "Spanien";
			case "ET": return "Etiopien";
			case "FK": return "Falklands\u00F6arna";
			case "FM": return "Mikronesien";
			case "FO": return "F\u00E4r\u00F6arna";
			case "FR": return "Frankrike";
			case "GB": return "Storbritannien";
			case "GE": return "Georgien";
			case "GF": return "Franska Guyana";
			case "GL": return "Gr\u00F6nland";
			case "GP": return "Guadelope";
			case "GQ": return "Ekvatorialguinea";
			case "GR": return "Grekland";
			case "HK": return "Hong Kong";
			case "HR": return "Kroatien";
			case "HU": return "Ungern";
			case "ID": return "Indonesien";
			case "IE": return "Irland";
			case "IN": return "Indien";
			case "IQ": return "Irak";
			case "IS": return "Island";
			case "IT": return "Italien";
			case "JO": return "Jordanien";
			case "KG": return "Kirgisistan";
			case "KH": return "Kambodja";
			case "KM": return "Komorerna";
			case "KN": return "S:t Christopher och Nevis";
			case "KP": return "Nordkorea";
			case "KR": return "Sydkorea";
			case "KY": return "Cayman\u00F6arna";
			case "KZ": return "Kazachstan";
			case "LB": return "Libanon";
			case "LC": return "S:t Lucia";
			case "LT": return "Litauen";
			case "LU": return "Luxemburg";
			case "LV": return "Lettland";
			case "LY": return "Libyen";
			case "MA": return "Marocko";
			case "MD": return "Moldavien";
			case "MG": return "Madagaskar";
			case "MH": return "Marshall\u00F6arna";
			case "MK": return "Makedonien";
			case "MN": return "Mongoliet";
			case "MP": return "Nordmarianerna";
			case "MR": return "Mauretanien";
			case "MV": return "Maldiverna";
			case "MX": return "Mexiko";
			case "NC": return "Nya Caledonien";
			case "NF": return "Norfolk\u00F6n";
			case "NL": return "Nederl\u00E4nderna";
			case "NO": return "Norge";
			case "NZ": return "Nya Zeeland";
			case "PF": return "Franska Polynesien";
			case "PG": return "Papua Nya Guinea";
			case "PH": return "Filippinerna";
			case "PL": return "Polen";
			case "PM": return "S:t Pierre och Miquelon";
			case "RO": return "Rum\u00E4nien";
			case "RU": return "Ryssland";
			case "SA": return "Saudi-Arabien";
			case "SB": return "Salomon\u00F6arna";
			case "SC": return "Seychellerna";
			case "SE": return "Sverige";
			case "SH": return "S:t Helena";
			case "SI": return "Slovenien";
			case "SJ": return "Svalbard och Jan Mayen";
			case "SK": return "Slovakien";
			case "SR": return "Surinam";
			case "ST": return "S\u00E3o Tom\u00E9 och Pr\u00EDncipe";
			case "SY": return "Syrien";
			case "TC": return "Turks- och Caicos\u00F6arna";
			case "TD": return "Tchad";
			case "TJ": return "Tadzjikistan";
			case "TL": return "\u00D6sttimor";
			case "TN": return "Tunisien";
			case "TR": return "Turkiet";
			case "TT": return "Trinidad och Tobago";
			case "UA": return "Ukraina";
			case "US": return "USA";
			case "VA": return "Vatikanstaten";
			case "VC": return "S:t Vincent och Grenadinerna";
			case "WF": return "Wallis och Futuna";
			case "VG": return "Brittiska Jungfru\u00F6arna";
			case "VI": return "Amerikanska Jungfru\u00F6arna";
			case "YE": return "Jemen";
			case "YU": return "Jugoslavien";
			case "ZA": return "Sydafrika";
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

}; // class CID001d

public class CNsv : CID001d
{
	public CNsv() : base() {}

}; // class CNsv

}; // namespace I18N.West
