/*
 * CID000c.cs - fr culture handler.
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

// Generated from "fr.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID000c : RootCulture
{
	public CID000c() : base(0x000C) {}
	public CID000c(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "fr";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "fra";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "FRA";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "fr";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"dim.", "lun.", "mar.", "mer.", "jeu.", "ven.", "sam."};
			dfi.DayNames = new String[] {"dimanche", "lundi", "mardi", "mercredi", "jeudi", "vendredi", "samedi"};
			dfi.AbbreviatedMonthNames = new String[] {"janv.", "f\u00E9vr.", "mars", "avr.", "mai", "juin", "juil.", "ao\u00FBt", "sept.", "oct.", "nov.", "d\u00E9c.", ""};
			dfi.MonthNames = new String[] {"janvier", "f\u00E9vrier", "mars", "avril", "mai", "juin", "juillet", "ao\u00FBt", "septembre", "octobre", "novembre", "d\u00E9cembre", ""};
			dfi.DateSeparator = "/";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "d MMMM yyyy";
			dfi.LongTimePattern = "HH:mm:ss z";
			dfi.ShortDatePattern = "dd/MM/yy";
			dfi.ShortTimePattern = "HH:mm";
			dfi.FullDateTimePattern = "dddd d MMMM yyyy HH' h 'mm z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:dd/MM/yy",
				"D:dddd d MMMM yyyy",
				"f:dddd d MMMM yyyy HH' h 'mm z",
				"f:dddd d MMMM yyyy HH:mm:ss z",
				"f:dddd d MMMM yyyy HH:mm:ss",
				"f:dddd d MMMM yyyy HH:mm",
				"F:dddd d MMMM yyyy HH:mm:ss",
				"g:dd/MM/yy HH' h 'mm z",
				"g:dd/MM/yy HH:mm:ss z",
				"g:dd/MM/yy HH:mm:ss",
				"g:dd/MM/yy HH:mm",
				"G:dd/MM/yy HH:mm:ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:HH' h 'mm z",
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
			case "root": return "racine";
			case "ab": return "abkhaze";
			case "aa": return "afar";
			case "af": return "afrikaans";
			case "sq": return "albanais";
			case "am": return "amharique";
			case "ar": return "arabe";
			case "hy": return "arm\u00e9nien";
			case "as": return "assamais";
			case "ay": return "aymara";
			case "az": return "az\u00e9ri";
			case "ba": return "bachkir";
			case "eu": return "basque";
			case "bn": return "bengali";
			case "dz": return "dzongkha";
			case "bh": return "bihari";
			case "bi": return "bichlamar";
			case "br": return "breton";
			case "bg": return "bulgare";
			case "my": return "birman";
			case "be": return "bi\u00e9lorusse";
			case "km": return "khmer";
			case "ca": return "catalan";
			case "zh": return "chinois";
			case "co": return "corse";
			case "hr": return "croate";
			case "cs": return "tch\u00e8que";
			case "da": return "danois";
			case "nl": return "hollandais";
			case "en": return "anglais";
			case "eo": return "esp\u00e9ranto";
			case "et": return "estonien";
			case "fo": return "f\u00e9ro\u00efen";
			case "fj": return "fidjien";
			case "fi": return "finnois";
			case "fr": return "fran\u00e7ais";
			case "fy": return "frison";
			case "gl": return "galicien";
			case "ka": return "georgien";
			case "de": return "allemand";
			case "el": return "grec";
			case "kl": return "groenlandais";
			case "gn": return "guarani";
			case "gu": return "goudjrati";
			case "ha": return "haoussa";
			case "he": return "h\u00e9breu";
			case "hi": return "hindi";
			case "hu": return "hongrois";
			case "is": return "islandais";
			case "id": return "indon\u00e9sien";
			case "ia": return "interlingua";
			case "ie": return "interlingue";
			case "iu": return "inuktitut";
			case "ik": return "inupiaq";
			case "ga": return "irlandais";
			case "it": return "italien";
			case "ja": return "japonais";
			case "jv": return "javanais";
			case "kn": return "kannada";
			case "ks": return "kashmiri";
			case "kk": return "kazakh";
			case "rw": return "rwanda";
			case "ky": return "kirghize";
			case "rn": return "rundi";
			case "ko": return "cor\u00e9en";
			case "ku": return "kurde";
			case "lo": return "lao";
			case "la": return "latin";
			case "lv": return "letton";
			case "ln": return "lingala";
			case "lt": return "lithuanien";
			case "mk": return "mac\u00e9donien";
			case "mg": return "malgache";
			case "ms": return "malais";
			case "ml": return "malayalam";
			case "mt": return "maltais";
			case "mi": return "maori";
			case "mr": return "marathe";
			case "mo": return "moldave";
			case "mn": return "mongol";
			case "na": return "nauruan";
			case "ne": return "n\u00e9palais";
			case "no": return "norv\u00e9gien";
			case "oc": return "occitan";
			case "or": return "oriya";
			case "om": return "galla";
			case "ps": return "pachto";
			case "fa": return "persan";
			case "pl": return "polonais";
			case "pt": return "portugais";
			case "pa": return "pendjabi";
			case "qu": return "quechua";
			case "rm": return "rh\u00e9toroman";
			case "ro": return "roumain";
			case "ru": return "russe";
			case "sm": return "samoan";
			case "sg": return "sango";
			case "sa": return "sanscrit";
			case "gd": return "ecossais ga\u00e9lique";
			case "sr": return "serbe";
			case "sh": return "serbo-croate";
			case "st": return "sotho du sud";
			case "tn": return "setswana";
			case "sn": return "shona";
			case "sd": return "sindhi";
			case "si": return "singhalais";
			case "ss": return "swati";
			case "sk": return "slovaque";
			case "sl": return "slov\u00e8ne";
			case "so": return "somali";
			case "es": return "espagnol";
			case "su": return "soundanais";
			case "sw": return "swahili";
			case "sv": return "su\u00e9dois";
			case "tl": return "tagalog";
			case "tg": return "tadjik";
			case "ta": return "tamoul";
			case "tt": return "tatare";
			case "te": return "telugu";
			case "th": return "tha\u00ef";
			case "bo": return "tib\u00e9tain";
			case "ti": return "tigrigna";
			case "to": return "tonga";
			case "ts": return "tsonga";
			case "tr": return "turc";
			case "tk": return "turkm\u00e8ne";
			case "tw": return "twi";
			case "ug": return "ou\u00efgour";
			case "uk": return "ukrainien";
			case "ur": return "ourdou";
			case "uz": return "ouzbek";
			case "vi": return "vietnamien";
			case "vo": return "volap\u00fck";
			case "cy": return "gallois";
			case "wo": return "wolof";
			case "xh": return "xhosa";
			case "yi": return "yiddish";
			case "yo": return "yoruba";
			case "za": return "zhuang";
			case "zu": return "zoulou";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "AD": return "Andorre";
			case "AE": return "Emirats Arabes Unis";
			case "AL": return "Albanie";
			case "AM": return "Arm\u00e9nie";
			case "AN": return "Antilles N\u00e9erlandaises";
			case "AR": return "Argentine";
			case "AS": return "Samoa Am\u00e9ricaines";
			case "AT": return "Autriche";
			case "AU": return "Australie";
			case "AZ": return "Azerba\u00efdjan";
			case "BA": return "Bosnie-Herz\u00e9govine";
			case "BB": return "Barbade";
			case "BE": return "Belgique";
			case "BG": return "Bulgarie";
			case "BH": return "Bahre\u00efn";
			case "BM": return "Bermudes";
			case "BO": return "Bolivie";
			case "BR": return "Br\u00e9sil";
			case "BT": return "Bhoutan";
			case "BY": return "Bi\u00e9lo-Russie";
			case "BZ": return "B\u00e9lize";
			case "CD": return "R\u00e9publique D\u00e9mocratique du Congo";
			case "CF": return "R\u00e9publique Centre-Africaine";
			case "CH": return "Suisse";
			case "CL": return "Chili";
			case "CM": return "Cameroun";
			case "CN": return "Chine";
			case "CO": return "Colombie";
			case "CV": return "Cap Vert";
			case "CY": return "Chypre";
			case "CZ": return "R\u00e9publique Tch\u00e8que";
			case "DE": return "Allemagne";
			case "DK": return "Danemark";
			case "DM": return "Dominique";
			case "DO": return "R\u00e9publique Dominicaine";
			case "DZ": return "Alg\u00e9rie";
			case "EC": return "Equateur";
			case "EE": return "Estonie";
			case "EG": return "Egypte";
			case "EH": return "Sahara Occidental";
			case "ER": return "Erythr\u00e9e";
			case "ES": return "Espagne";
			case "ET": return "Ethiopie";
			case "FI": return "Finlande";
			case "FJ": return "Fidji";
			case "FM": return "Micron\u00e9sie";
			case "GB": return "Royaume-Uni";
			case "GD": return "Grenade";
			case "GE": return "G\u00e9orgie";
			case "GF": return "Guin\u00e9e Fran\u00e7aise";
			case "GL": return "Groenland";
			case "GM": return "Gambie";
			case "GN": return "Guin\u00e9e";
			case "GQ": return "Guin\u00e9e Equatoriale";
			case "GR": return "Gr\u00e8ce";
			case "GW": return "Guin\u00e9e-Bissau";
			case "GY": return "Guyane";
			case "HK": return "Hong-Kong SAR";
			case "HR": return "Croatie";
			case "HT": return "Ha\u00efti";
			case "HU": return "Hongrie";
			case "ID": return "Indon\u00e9sie";
			case "IE": return "Irlande";
			case "IL": return "Isra\u00ebl";
			case "IN": return "Inde";
			case "IS": return "Islande";
			case "IT": return "Italie";
			case "JM": return "Jama\u00efque";
			case "JO": return "Jordanie";
			case "JP": return "Japon";
			case "KH": return "Cambodge";
			case "KM": return "Comores";
			case "KP": return "Cor\u00e9e du Nord";
			case "KR": return "Cor\u00e9e du Sud";
			case "KW": return "Kowe\u00eft";
			case "LB": return "Liban";
			case "LC": return "Sainte-Lucie";
			case "LR": return "Lib\u00e9ria";
			case "LT": return "Lithuanie";
			case "LV": return "Lettonie";
			case "LY": return "Libye";
			case "MA": return "Maroc";
			case "MK": return "Mac\u00e9doine";
			case "MN": return "Mongolie";
			case "MO": return "Macao SAR Chine";
			case "MR": return "Mauritanie";
			case "MT": return "Malte";
			case "MU": return "Maurice";
			case "MX": return "Mexique";
			case "MY": return "Malaisie";
			case "NA": return "Namibie";
			case "NC": return "Nouvelle-Cal\u00e9donie";
			case "NG": return "Nig\u00e9ria";
			case "NL": return "Pays-Bas";
			case "NP": return "N\u00e9pal";
			case "NO": return "Norv\u00e8ge";
			case "NU": return "Niu\u00e9";
			case "NZ": return "Nouvelle-Z\u00e9lande";
			case "PE": return "P\u00e9rou";
			case "PF": return "Polyn\u00e9sie Fran\u00e7aise";
			case "PG": return "Papouasie-Nouvelle-Guin\u00e9e";
			case "PL": return "Pologne";
			case "PM": return "Saint-Pierre-et-Miquelon";
			case "PR": return "Porto Rico";
			case "PW": return "Palaos";
			case "RO": return "Roumanie";
			case "RU": return "Russie";
			case "SA": return "Arabie Saoudite";
			case "SD": return "Soudan";
			case "SE": return "Su\u00e8de";
			case "SG": return "Singapour";
			case "SH": return "Sainte-H\u00e9l\u00e8ne";
			case "SI": return "Slov\u00e9nie";
			case "SK": return "Slovaquie";
			case "SM": return "Saint-Marin";
			case "SN": return "S\u00e9n\u00e9gal";
			case "SO": return "Somalie";
			case "SP": return "Serbie";
			case "ST": return "Sao Tom\u00e9-et-Principe";
			case "SY": return "Syrie";
			case "TD": return "Tchad";
			case "TF": return "Terres Australes Fran\u00e7aises";
			case "TH": return "Tha\u00eflande";
			case "TJ": return "Tadjikistan";
			case "TL": return "Timor Oriental";
			case "TN": return "Tunisie";
			case "TR": return "Turquie";
			case "TT": return "Trinit\u00e9-et-Tobago";
			case "TW": return "Ta\u00efwan, Province de Chine";
			case "TZ": return "Tanzanie";
			case "UG": return "Ouganda";
			case "UM": return "\u00CEles Mineures \u00C9loign\u00e9es des \u00C9tats-Unis";
			case "US": return "\u00C9tats-Unis";
			case "UZ": return "Ouzb\u00e9kistan";
			case "VG": return "Iles Vierges Britanniques";
			case "VI": return "Iles Vierges Am\u00e9ricaines";
			case "WF": return "Wallis et Futuna";
			case "YE": return "Y\u00e9men";
			case "YU": return "Yougoslavie";
			case "ZA": return "Afrique du Sud";
			case "ZM": return "Zambie";
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
				return 20297;
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

}; // class CID000c

public class CNfr : CID000c
{
	public CNfr() : base() {}

}; // class CNfr

}; // namespace I18N.West
