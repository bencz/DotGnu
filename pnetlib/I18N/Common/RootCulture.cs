/*
 * RootCulture.cs - root culture handler.
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

// Generated from "root.txt".

namespace I18N.Common
{

using System;
using System.Globalization;

public abstract class RootCulture : CultureInfo
{
	public RootCulture(int culture) : base(0x40000000 + culture) {}

	public override String DisplayName
	{
		get
		{
			return Manager.GetDisplayName(this);
		}
	}
	public override String EnglishName
	{
		get
		{
			return Manager.GetEnglishName(this);
		}
	}
	public override String NativeName
	{
		get
		{
			return Manager.GetNativeName(this);
		}
	}
	public virtual String Country
	{
		get
		{
			return null;
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = new DateTimeFormatInfo();
			dfi.AMDesignator = "AM";
			dfi.PMDesignator = "PM";
			dfi.AbbreviatedDayNames = new String[] {"Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"};
			dfi.DayNames = new String[] {"Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"};
			dfi.AbbreviatedMonthNames = new String[] {"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec", ""};
			dfi.MonthNames = new String[] {"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December", ""};
			dfi.DateSeparator = "/";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "MMMM d, yyyy";
			dfi.LongTimePattern = "h:mm:ss tt z";
			dfi.ShortDatePattern = "M/d/yy";
			dfi.ShortTimePattern = "h:mm tt";
			dfi.FullDateTimePattern = "dddd, MMMM d, yyyy h:mm:ss tt z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:M/d/yy",
				"D:dddd, MMMM d, yyyy",
				"f:dddd, MMMM d, yyyy h:mm:ss tt z",
				"f:dddd, MMMM d, yyyy h:mm:ss tt z",
				"f:dddd, MMMM d, yyyy h:mm:ss tt",
				"f:dddd, MMMM d, yyyy h:mm tt",
				"F:dddd, MMMM d, yyyy HH:mm:ss",
				"g:M/d/yy h:mm:ss tt z",
				"g:M/d/yy h:mm:ss tt z",
				"g:M/d/yy h:mm:ss tt",
				"g:M/d/yy h:mm tt",
				"G:M/d/yy HH:mm:ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:h:mm:ss tt z",
				"t:h:mm:ss tt z",
				"t:h:mm:ss tt",
				"t:h:mm tt",
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
			NumberFormatInfo nfi = new NumberFormatInfo();
			nfi.CurrencyDecimalSeparator = ".";
			RegionNameTable.AddCurrencyInfo(nfi, this);
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

	public virtual String ResolveLanguage(String name)
	{
		switch(name)
		{
			case "root": return "Root";
			case "aa": return "Afar";
			case "ab": return "Abkhazian";
			case "ace": return "Achinese";
			case "ach": return "Acoli";
			case "ada": return "Adangme";
			case "ae": return "Avestan";
			case "af": return "Afrikaans";
			case "afa": return "Afro-Asiatic (Other)";
			case "afh": return "Afrihili";
			case "ak": return "Akan";
			case "akk": return "Akkadien";
			case "ale": return "Aleut";
			case "alg": return "Algonquian Languages";
			case "am": return "Amharic";
			case "ang": return "English, Old (ca.450-1100)";
			case "apa": return "Apache Languages";
			case "ar": return "Arabic";
			case "arc": return "Aramaic";
			case "arn": return "Araucanian";
			case "arp": return "Arapaho";
			case "art": return "Artificial (Other)";
			case "arw": return "Arawak";
			case "as": return "Assamese";
			case "ast": return "Asturian";
			case "ath": return "Athapascan Languages";
			case "aus": return "Australian Languages";
			case "av": return "Avaric";
			case "awa": return "Awadhi";
			case "ay": return "Aymara";
			case "az": return "Azerbaijani";
			case "ba": return "Bashkir";
			case "bad": return "Banda";
			case "bai": return "Bamileke Languages";
			case "bal": return "Baluchi";
			case "bam": return "Bambara";
			case "ban": return "Balinese";
			case "bas": return "Basa";
			case "bat": return "Baltic (Other)";
			case "be": return "Belarusian";
			case "bej": return "Beja";
			case "bem": return "Bemba";
			case "ber": return "Berber";
			case "bg": return "Bulgarian";
			case "bh": return "Bihari";
			case "bho": return "Bhojpuri";
			case "bi": return "Bislama";
			case "bik": return "Bikol";
			case "bin": return "Bini";
			case "bla": return "Siksika";
			case "bm": return "Bambara";
			case "bn": return "Bengali";
			case "bnt": return "Bantu";
			case "bo": return "Tibetan";
			case "br": return "Breton";
			case "bra": return "Braj";
			case "bs": return "Bosnian";
			case "btk": return "Batak";
			case "bua": return "Buriat";
			case "bug": return "Buginese";
			case "ca": return "Catalan";
			case "cad": return "Caddo";
			case "cai": return "Central American Indian (Other)";
			case "car": return "Carib";
			case "cau": return "Caucasian (Other)";
			case "ce": return "Chechen";
			case "ceb": return "Cebuano";
			case "cel": return "Celtic (Other)";
			case "ch": return "Chamorro";
			case "chb": return "Chibcha";
			case "chg": return "Chagatai";
			case "chk": return "Chuukese";
			case "chm": return "Mari";
			case "chn": return "Chinook Jargon";
			case "cho": return "Choctaw";
			case "chp": return "Chipewyan";
			case "chr": return "Cherokee";
			case "chy": return "Cheyenne";
			case "cmc": return "Chamic Languages";
			case "co": return "Corsican";
			case "cop": return "Coptic";
			case "cpe": return "Creoles and Pidgins, English-based (Other)";
			case "cpf": return "Creoles and Pidgins, French-based (Other)";
			case "cpp": return "Creoles and pidgins, Portuguese-based (Other)";
			case "cr": return "Cree";
			case "crp": return "Creoles and Pidgins (Other)";
			case "cs": return "Czech";
			case "cu": return "Church Slavic";
			case "cus": return "Cushitic (Other)";
			case "cv": return "Chuvash";
			case "cy": return "Welsh";
			case "da": return "Danish";
			case "dak": return "Dakota";
			case "dar": return "Dargwa";
			case "day": return "Dayak";
			case "de": return "German";
			case "del": return "Delaware";
			case "den": return "Slave";
			case "dgr": return "Dogrib";
			case "din": return "Dinka";
			case "doi": return "Dogri";
			case "dra": return "Dravidian (Other)";
			case "dua": return "Duala";
			case "dum": return "Dutch, Middle (ca. 1050-1350)";
			case "dv": return "Divehi";
			case "dyu": return "Dyula";
			case "dz": return "Dzongkha";
			case "ee": return "Ewe";
			case "efi": return "Efik";
			case "egy": return "Egyptian (Ancient)";
			case "eka": return "Ekajuk";
			case "el": return "Greek";
			case "elx": return "Elamite";
			case "en": return "English";
			case "enm": return "English, Middle (1100-1500)";
			case "eo": return "Esperanto";
			case "es": return "Spanish";
			case "et": return "Estonian";
			case "eu": return "Basque";
			case "ewo": return "Ewondo";
			case "fa": return "Persian";
			case "fan": return "Fang";
			case "fat": return "Fanti";
			case "ff": return "Fulah";
			case "fi": return "Finnish";
			case "fiu": return "Finno - Ugrian (Other)";
			case "fj": return "Fijian";
			case "fo": return "Faroese";
			case "fon": return "Fon";
			case "fr": return "French";
			case "frm": return "French, Middle (ca.1400-1600)";
			case "fro": return "French, Old (842-ca.1400)";
			case "fur": return "Friulian";
			case "fy": return "Frisian";
			case "ga": return "Irish";
			case "gaa": return "Ga";
			case "gay": return "Gayo";
			case "gba": return "Gbaya";
			case "gd": return "Scottish Gaelic";
			case "gem": return "Germanic (Other)";
			case "gez": return "Geez";
			case "gil": return "Gilbertese";
			case "gl": return "Gallegan";
			case "gla": return "Gaelic (Scots)";
			case "gmh": return "German, Middle High (ca.1050-1500)";
			case "gn": return "Guarani";
			case "goh": return "German, Old High (ca.750-1050)";
			case "gon": return "Gondi";
			case "gor": return "Gorontalo";
			case "got": return "Gothic";
			case "grb": return "Gerbo";
			case "grc": return "Greek, Ancient (to 1453)";
			case "gu": return "Gujarati";
			case "gv": return "Manx";
			case "gwi": return "Gwich'in";
			case "hai": return "Haida";
			case "ha": return "Hausa";
			case "haw": return "Hawaiian";
			case "he": return "Hebrew";
			case "hi": return "Hindi";
			case "hil": return "Hiligaynon";
			case "him": return "Himachali";
			case "hit": return "Hittite";
			case "hmn": return "Hmong";
			case "ho": return "Hiri Motu";
			case "hr": return "Croatian";
			case "hu": return "Hungarian";
			case "hup": return "Hupa";
			case "hy": return "Armenian";
			case "hz": return "Herero";
			case "ia": return "Interlingua";
			case "iba": return "Iban";
			case "id": return "Indonesian";
			case "ie": return "Interlingue";
			case "ig": return "Igbo";
			case "ii": return "Sichuan Yi";
			case "ijo": return "Ijo";
			case "ik": return "Inupiaq";
			case "ilo": return "Iloko";
			case "inc": return "Indic (Other)";
			case "ine": return "Indo-European (Other)";
			case "inh": return "Ingush";
			case "io": return "Ido";
			case "ira": return "Iranian";
			case "iro": return "Iroquoian languages";
			case "is": return "Icelandic";
			case "it": return "Italian";
			case "iu": return "Inukitut";
			case "iw": return "Hebrew";
			case "ja": return "Japanese";
			case "jpr": return "Judeo-Persian";
			case "jrb": return "Judeo-Arabic";
			case "jv": return "Javanese";
			case "ka": return "Georgian";
			case "kaa": return "Kara-Kalpak";
			case "kab": return "Kabyle";
			case "kac": return "Kachin";
			case "kam": return "Kamba";
			case "kar": return "Karen";
			case "kaw": return "Kawi";
			case "kbd": return "Kabardian";
			case "kg": return "Kongo";
			case "kha": return "Khasi";
			case "khi": return "Khoisan (Other)";
			case "kho": return "Khotanese";
			case "ki": return "Kikuyu";
			case "kj": return "Kuanyama";
			case "kk": return "Kazakh";
			case "kl": return "Kalaallisut";
			case "km": return "Khmer";
			case "kmb": return "Kimbundu";
			case "kn": return "Kannada";
			case "ko": return "Korean";
			case "kok": return "Konkani";
			case "kos": return "Kosraean";
			case "kpe": return "Kpelle";
			case "kr": return "Kanuri";
			case "kro": return "Kru";
			case "kru": return "Kurukh";
			case "ks": return "Kashmiri";
			case "ku": return "Kurdish";
			case "kum": return "Kumyk";
			case "kut": return "Kutenai";
			case "kv": return "Komi";
			case "kw": return "Cornish";
			case "ky": return "Kirghiz";
			case "la": return "Latin";
			case "lad": return "Ladino";
			case "lah": return "Lahnda";
			case "lam": return "Lamba";
			case "lb": return "Luxembourgish";
			case "lez": return "Lezghian";
			case "lg": return "Ganda";
			case "lin": return "Lingala";
			case "li": return "Limburgish";
			case "lit": return "Lithuanian";
			case "ln": return "Lingala";
			case "lo": return "Lao";
			case "lol": return "Mongo";
			case "loz": return "Lozi";
			case "lt": return "Lithuanian";
			case "lu": return "Luba-Katanga";
			case "lua": return "Luba-Lulua";
			case "lui": return "Luiseno";
			case "lun": return "Lunda";
			case "luo": return "Luo";
			case "lus": return "Lushai";
			case "lv": return "Latvian";
			case "mad": return "Madurese";
			case "mag": return "Magahi";
			case "mai": return "Maithili";
			case "mak": return "Makasar";
			case "man": return "Mandingo";
			case "map": return "Austronesian";
			case "mas": return "Masai";
			case "mdr": return "Mandar";
			case "men": return "Mende";
			case "mg": return "Malagasy";
			case "mga": return "Irish, Middle (900-1200)";
			case "mh": return "Marshallese";
			case "mi": return "Maori";
			case "mic": return "Micmac";
			case "min": return "Minangkabau";
			case "mis": return "Miscellaneous Languages";
			case "mk": return "Macedonian";
			case "mkh": return "Mon-Khmer (Other)";
			case "ml": return "Malayalam";
			case "mn": return "Mongolian";
			case "mnc": return "Manchu";
			case "mni": return "Manipuri";
			case "mno": return "Manobo Languages";
			case "mo": return "Moldavian";
			case "moh": return "Mohawk";
			case "mos": return "Mossi";
			case "mr": return "Marathi";
			case "ms": return "Malay";
			case "mt": return "Maltese";
			case "mul": return "Multiple Languages";
			case "mun": return "Munda Languages";
			case "mus": return "Creek";
			case "mwr": return "Marwari";
			case "my": return "Burmese";
			case "myn": return "Mayan";
			case "na": return "Nauru";
			case "nah": return "Nahuatl";
			case "nai": return "North American Indian (Other)";
			case "nap": return "Neapolitan";
			case "nb": return "Norwegian Bokm\u00e5l";
			case "nd": return "Ndebele, North";
			case "nds": return "Low German; Low Saxon";
			case "ne": return "Nepali";
			case "new": return "Newari";
			case "ng": return "Ndonga";
			case "nia": return "Nias";
			case "nic": return "Niger - Kordofanian (Other)";
			case "niu": return "Niuean";
			case "nl": return "Dutch";
			case "nn": return "Norwegian Nynorsk";
			case "no": return "Norwegian";
			case "non": return "Norse, Old";
			case "nr": return "Ndebele, South";
			case "nso": return "Sotho, Northern";
			case "nub": return "Nubian Languages";
			case "nv": return "Navajo";
			case "ny": return "Nyanja; Chichewa; Chewa";
			case "nym": return "Nyamwezi";
			case "nyn": return "Nyankole";
			case "nyo": return "Nyoro";
			case "nzi": return "Nzima";
			case "oc": return "Occitan (post 1500); Proven\u00E7al";
			case "oj": return "Ojibwa";
			case "om": return "Oromo";
			case "or": return "Oriya";
			case "os": return "Ossetic";
			case "osa": return "Osage";
			case "ota": return "Turkish, Ottoman (1500-1928)";
			case "oto": return "Otomian Languages";
			case "pa": return "Punjabi";
			case "paa": return "Papuan (Other)";
			case "pag": return "Pangasinan";
			case "pal": return "Pahlavi";
			case "pam": return "Pampanga";
			case "pap": return "Papiamento";
			case "pau": return "Palauan";
			case "peo": return "Persian Old (ca.600-400 B.C.)";
			case "phi": return "Philippine (Other)";
			case "phn": return "Phoenician";
			case "pi": return "Pali";
			case "pl": return "Polish";
			case "pon": return "Pohnpeian";
			case "pra": return "Prakrit Languages";
			case "pro": return "Proven\u00E7al, Old (to 1500)";
			case "ps": return "Pashto (Pushto)";
			case "pt": return "Portuguese";
			case "qu": return "Quechua";
			case "raj": return "Rajasthani";
			case "rap": return "Rapanui";
			case "rar": return "Rarotongan";
			case "rm": return "Rhaeto-Romance";
			case "rn": return "Rundi";
			case "ro": return "Romanian";
			case "roa": return "Romance (Other)";
			case "rom": return "Romany";
			case "ru": return "Russian";
			case "rw": return "Kinyarwanda";
			case "sa": return "Sanskrit";
			case "sad": return "Sandawe";
			case "sah": return "Yakut";
			case "sai": return "South American Indian (Other)";
			case "sal": return "Salishan languages";
			case "sam": return "Samaritan Aramaic";
			case "sas": return "Sasak";
			case "sat": return "Santali";
			case "sc": return "Sardinian";
			case "sco": return "Scots";
			case "sd": return "Sindhi";
			case "se": return "Northern Sami";
			case "sel": return "Selkup";
			case "sem": return "Semitic (Other)";
			case "sg": return "Sango";
			case "sga": return "Irish, Old (to 900)";
			case "sgn": return "Sign Languages";
			case "sh": return "Serbo-Croatian";
			case "shn": return "Shan";
			case "si": return "Sinhalese";
			case "sid": return "Sidamo";
			case "sio": return "Siouan Languages";
			case "sit": return "Sino-Tibetan (Other)";
			case "sk": return "Slovak";
			case "sl": return "Slovenian";
			case "sla": return "Slavic (Other)";
			case "sm": return "Samoan";
			case "sma": return "Southern Sami";
			case "smi": return "Sami languages (Other)";
			case "smj": return "Lule Sami";
			case "smn": return "Inari Sami";
			case "sms": return "Skolt Sami";
			case "sn": return "Shona";
			case "snk": return "Soninke";
			case "so": return "Somali";
			case "sog": return "Sogdien";
			case "son": return "Songhai";
			case "sq": return "Albanian";
			case "sr": return "Serbian";
			case "srr": return "Serer";
			case "ss": return "Swati";
			case "ssa": return "Nilo-Saharam (Other)";
			case "st": return "Sotho, Southern";
			case "su": return "Sundanese";
			case "suk": return "Sukuma";
			case "sus": return "Susu";
			case "sux": return "Sumerian";
			case "sv": return "Swedish";
			case "sw": return "Swahili";
			case "syr": return "Syriac";
			case "ta": return "Tamil";
			case "tai": return "Tai (Other)";
			case "te": return "Telugu";
			case "tem": return "Timne";
			case "ter": return "Tereno";
			case "tet": return "Tetum";
			case "tg": return "Tajik";
			case "th": return "Thai";
			case "tig": return "Tigre";
			case "ti": return "Tigrinya";
			case "tiv": return "Tiv";
			case "tk": return "Turkmen";
			case "tkl": return "Tokelau";
			case "tl": return "Tagalog";
			case "tli": return "Tlingit";
			case "tmh": return "Tamashek";
			case "tn": return "Tswana";
			case "tog": return "Tonga (Nyasa)";
			case "to": return "Tonga (Tonga Islands)";
			case "tpi": return "Tok Pisin";
			case "tr": return "Turkish";
			case "ts": return "Tsonga";
			case "tsi": return "Tsimshian";
			case "tt": return "Tatar";
			case "tum": return "Tumbuka";
			case "tup": return "Tupi languages";
			case "tur": return "Turkish";
			case "tut": return "Altaic (Other)";
			case "tvl": return "Tuvalu";
			case "tw": return "Twi";
			case "ty": return "Tahitian";
			case "tyv": return "Tuvinian";
			case "ug": return "Uighur";
			case "uga": return "Ugaritic";
			case "uk": return "Ukrainian";
			case "umb": return "Umbundu";
			case "und": return "Undetermined";
			case "ur": return "Urdu";
			case "uz": return "Uzbek";
			case "vai": return "Vai";
			case "ve": return "Venda";
			case "vi": return "Vietnamese";
			case "vo": return "Volap\u00FCk";
			case "vot": return "Votic";
			case "wa": return "Walloon";
			case "wak": return "Wakashan Languages";
			case "wal": return "Walamo";
			case "war": return "Waray";
			case "was": return "Washo";
			case "wen": return "Sorbian Languages";
			case "wo": return "Wolof";
			case "xh": return "Xhosa";
			case "yao": return "Yao";
			case "yap": return "Yapese";
			case "yi": return "Yiddish";
			case "yo": return "Yoruba";
			case "ypk": return "Yupik Languages";
			case "za": return "Zhuang";
			case "zap": return "Zapotec";
			case "zen": return "Zenaga";
			case "zh": return "Chinese";
			case "znd": return "Zande";
			case "zu": return "Zulu";
			case "zun": return "Zuni";
		}
		return name;
	}

	public virtual String ResolveCountry(String name)
	{
		switch(name)
		{
			case "AD": return "Andorra";
			case "AE": return "United Arab Emirates";
			case "AF": return "Afghanistan";
			case "AG": return "Antigua and Barbuda";
			case "AI": return "Anguilla";
			case "AL": return "Albania";
			case "AM": return "Armenia";
			case "AN": return "Netherlands Antilles";
			case "AO": return "Angola";
			case "AQ": return "Antarctica";
			case "AR": return "Argentina";
			case "AS": return "American Samoa";
			case "AT": return "Austria";
			case "AU": return "Australia";
			case "AW": return "Aruba";
			case "AZ": return "Azerbaijan";
			case "BA": return "Bosnia and Herzegovina";
			case "BB": return "Barbados";
			case "BD": return "Bangladesh";
			case "BE": return "Belgium";
			case "BF": return "Burkina Faso";
			case "BG": return "Bulgaria";
			case "BH": return "Bahrain";
			case "BI": return "Burundi";
			case "BJ": return "Benin";
			case "BM": return "Bermuda";
			case "BN": return "Brunei";
			case "BO": return "Bolivia";
			case "BR": return "Brazil";
			case "BS": return "Bahamas";
			case "BT": return "Bhutan";
			case "BV": return "Bouvet Island";
			case "BW": return "Botswana";
			case "BY": return "Belarus";
			case "BZ": return "Belize";
			case "CA": return "Canada";
			case "CC": return "Cocos Islands";
			case "CD": return "Democratic Republic of the Congo";
			case "CF": return "Central African Republic";
			case "CG": return "Congo";
			case "CH": return "Switzerland";
			case "CI": return "C\u00F4te d'Ivoire";
			case "CK": return "Cook Islands";
			case "CL": return "Chile";
			case "CM": return "Cameroon";
			case "CN": return "China";
			case "CO": return "Colombia";
			case "CR": return "Costa Rica";
			case "CU": return "Cuba";
			case "CV": return "Cape Verde";
			case "CX": return "Christmas Island";
			case "CY": return "Cyprus";
			case "CZ": return "Czech Republic";
			case "DE": return "Germany";
			case "DJ": return "Djibouti";
			case "DK": return "Denmark";
			case "DM": return "Dominica";
			case "DO": return "Dominican Republic";
			case "DZ": return "Algeria";
			case "EC": return "Ecuador";
			case "EE": return "Estonia";
			case "EG": return "Egypt";
			case "EH": return "Western Sahara";
			case "ER": return "Eritrea";
			case "ES": return "Spain";
			case "ET": return "Ethiopia";
			case "FI": return "Finland";
			case "FJ": return "Fiji";
			case "FK": return "Falkland Islands";
			case "FM": return "Micronesia";
			case "FO": return "Faroe Islands";
			case "FR": return "France";
			case "GA": return "Gabon";
			case "GB": return "United Kingdom";
			case "GD": return "Grenada";
			case "GE": return "Georgia";
			case "GF": return "French Guiana";
			case "GH": return "Ghana";
			case "GI": return "Gibraltar";
			case "GL": return "Greenland";
			case "GM": return "Gambia";
			case "GN": return "Guinea";
			case "GP": return "Guadeloupe";
			case "GQ": return "Equatorial Guinea";
			case "GR": return "Greece";
			case "GS": return "South Georgia and South Sandwich Islands";
			case "GT": return "Guatemala";
			case "GU": return "Guam";
			case "GW": return "Guinea-Bissau";
			case "GY": return "Guyana";
			case "HK": return "Hong Kong S.A.R., China";
			case "HM": return "Heard Island and McDonald Islands";
			case "HN": return "Honduras";
			case "HR": return "Croatia";
			case "HT": return "Haiti";
			case "HU": return "Hungary";
			case "ID": return "Indonesia";
			case "IE": return "Ireland";
			case "IL": return "Israel";
			case "IN": return "India";
			case "IO": return "British Indian Ocean Territory";
			case "IQ": return "Iraq";
			case "IR": return "Iran";
			case "IS": return "Iceland";
			case "IT": return "Italy";
			case "JM": return "Jamaica";
			case "JO": return "Jordan";
			case "JP": return "Japan";
			case "KE": return "Kenya";
			case "KG": return "Kyrgyzstan";
			case "KH": return "Cambodia";
			case "KI": return "Kiribati";
			case "KM": return "Comoros";
			case "KN": return "Saint Kitts and Nevis";
			case "KP": return "North Korea";
			case "KR": return "South Korea";
			case "KW": return "Kuwait";
			case "KY": return "Cayman Islands";
			case "KZ": return "Kazakhstan";
			case "LA": return "Laos";
			case "LB": return "Lebanon";
			case "LC": return "Saint Lucia";
			case "LI": return "Liechtenstein";
			case "LK": return "Sri Lanka";
			case "LR": return "Liberia";
			case "LS": return "Lesotho";
			case "LT": return "Lithuania";
			case "LU": return "Luxembourg";
			case "LV": return "Latvia";
			case "LY": return "Libya";
			case "MA": return "Morocco";
			case "MC": return "Monaco";
			case "MD": return "Moldova";
			case "MG": return "Madagascar";
			case "MH": return "Marshall Islands";
			case "MK": return "Macedonia";
			case "ML": return "Mali";
			case "MM": return "Myanmar";
			case "MN": return "Mongolia";
			case "MO": return "Macao S.A.R. China";
			case "MP": return "Northern Mariana Islands";
			case "MQ": return "Martinique";
			case "MR": return "Mauritania";
			case "MS": return "Montserrat";
			case "MT": return "Malta";
			case "MU": return "Mauritius";
			case "MV": return "Maldives";
			case "MW": return "Malawi";
			case "MX": return "Mexico";
			case "MY": return "Malaysia";
			case "MZ": return "Mozambique";
			case "NA": return "Namibia";
			case "NC": return "New Caledonia";
			case "NE": return "Niger";
			case "NF": return "Norfolk Island";
			case "NG": return "Nigeria";
			case "NI": return "Nicaragua";
			case "NL": return "Netherlands";
			case "NO": return "Norway";
			case "NP": return "Nepal";
			case "NR": return "Nauru";
			case "NU": return "Niue";
			case "NZ": return "New Zealand";
			case "OM": return "Oman";
			case "PA": return "Panama";
			case "PE": return "Peru";
			case "PF": return "French Polynesia";
			case "PG": return "Papua New Guinea";
			case "PH": return "Philippines";
			case "PK": return "Pakistan";
			case "PL": return "Poland";
			case "PM": return "Saint Pierre and Miquelon";
			case "PN": return "Pitcairn";
			case "PR": return "Puerto Rico";
			case "PS": return "Palestinian Territory";
			case "PT": return "Portugal";
			case "PW": return "Palau";
			case "PY": return "Paraguay";
			case "QA": return "Qatar";
			case "RE": return "R\u00E9union";
			case "RO": return "Romania";
			case "RU": return "Russia";
			case "RW": return "Rwanda";
			case "SA": return "Saudi Arabia";
			case "SB": return "Solomon Islands";
			case "SC": return "Seychelles";
			case "SD": return "Sudan";
			case "SE": return "Sweden";
			case "SG": return "Singapore";
			case "SH": return "Saint Helena";
			case "SI": return "Slovenia";
			case "SJ": return "Svalbard and Jan Mayen";
			case "SK": return "Slovakia";
			case "SL": return "Sierra Leone";
			case "SM": return "San Marino";
			case "SN": return "Senegal";
			case "SO": return "Somalia";
			case "SP": return "Serbia";
			case "SR": return "Suriname";
			case "ST": return "Sao Tome and Principe";
			case "SV": return "El Salvador";
			case "SY": return "Syria";
			case "SZ": return "Swaziland";
			case "TC": return "Turks and Caicos Islands";
			case "TD": return "Chad";
			case "TF": return "French Southern Territories";
			case "TG": return "Togo";
			case "TH": return "Thailand";
			case "TJ": return "Tajikistan";
			case "TK": return "Tokelau";
			case "TM": return "Turkmenistan";
			case "TN": return "Tunisia";
			case "TO": return "Tonga";
			case "TL": return "Timor-Leste";
			case "TR": return "Turkey";
			case "TT": return "Trinidad and Tobago";
			case "TV": return "Tuvalu";
			case "TW": return "Taiwan";
			case "TZ": return "Tanzania";
			case "UA": return "Ukraine";
			case "UG": return "Uganda";
			case "UM": return "United States Minor Outlying Islands";
			case "US": return "United States";
			case "UY": return "Uruguay";
			case "UZ": return "Uzbekistan";
			case "VA": return "Vatican";
			case "VC": return "Saint Vincent and the Grenadines";
			case "VE": return "Venezuela";
			case "VG": return "British Virgin Islands";
			case "VI": return "U.S. Virgin Islands";
			case "VN": return "Vietnam";
			case "VU": return "Vanuatu";
			case "WF": return "Wallis and Futuna";
			case "WS": return "Samoa";
			case "YE": return "Yemen";
			case "YT": return "Mayotte";
			case "YU": return "Yugoslavia";
			case "ZA": return "South Africa";
			case "ZM": return "Zambia";
			case "ZW": return "Zimbabwe";
		}
		return name;
	}

}; // class RootCulture

}; // namespace I18N.Common
