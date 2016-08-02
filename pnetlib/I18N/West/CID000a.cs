/*
 * CID000a.cs - es culture handler.
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

// Generated from "es.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID000a : RootCulture
{
	public CID000a() : base(0x000A) {}
	public CID000a(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "es";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "spa";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "ESP";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "es";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"dom", "lun", "mar", "mi\u00E9", "jue", "vie", "s\u00E1b"};
			dfi.DayNames = new String[] {"domingo", "lunes", "martes", "mi\u00E9rcoles", "jueves", "viernes", "s\u00E1bado"};
			dfi.AbbreviatedMonthNames = new String[] {"ene", "feb", "mar", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic", ""};
			dfi.MonthNames = new String[] {"enero", "febrero", "marzo", "abril", "mayo", "junio", "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre", ""};
			dfi.DateSeparator = "/";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "d' de 'MMMM' de 'yyyy";
			dfi.LongTimePattern = "HH:mm:ss z";
			dfi.ShortDatePattern = "d/MM/yy";
			dfi.ShortTimePattern = "HH:mm";
			dfi.FullDateTimePattern = "dddd d' de 'MMMM' de 'yyyy HH'H'mm'' z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:d/MM/yy",
				"D:dddd d' de 'MMMM' de 'yyyy",
				"f:dddd d' de 'MMMM' de 'yyyy HH'H'mm'' z",
				"f:dddd d' de 'MMMM' de 'yyyy HH:mm:ss z",
				"f:dddd d' de 'MMMM' de 'yyyy HH:mm:ss",
				"f:dddd d' de 'MMMM' de 'yyyy HH:mm",
				"F:dddd d' de 'MMMM' de 'yyyy HH:mm:ss",
				"g:d/MM/yy HH'H'mm'' z",
				"g:d/MM/yy HH:mm:ss z",
				"g:d/MM/yy HH:mm:ss",
				"g:d/MM/yy HH:mm",
				"G:d/MM/yy HH:mm:ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:HH'H'mm'' z",
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

	public override String ResolveLanguage(String name)
	{
		switch(name)
		{
			case "root": return "ra\u00EDz";
			case "af": return "afrikaans";
			case "am": return "amharic";
			case "ar": return "\u00E1rabe";
			case "az": return "azerbayano";
			case "be": return "bielorruso";
			case "bg": return "b\u00FAlgaro";
			case "bh": return "bihari";
			case "bn": return "bengal\u00ED";
			case "ca": return "catal\u00E1n";
			case "cs": return "checo";
			case "da": return "dan\u00E9s";
			case "de": return "alem\u00E1n";
			case "el": return "griego";
			case "en": return "ingl\u00E9s";
			case "eo": return "esperanto";
			case "es": return "espa\u00F1ol";
			case "et": return "estonio";
			case "eu": return "vasco";
			case "fa": return "farsi";
			case "fi": return "finland\u00E9s";
			case "fo": return "faro\u00E9s";
			case "fr": return "franc\u00E9s";
			case "ga": return "irland\u00E9s";
			case "gl": return "gallego";
			case "gu": return "goujarat\u00ED";
			case "he": return "hebreo";
			case "hi": return "hindi";
			case "hr": return "croata";
			case "hu": return "h\u00FAngaro";
			case "hy": return "armenio";
			case "id": return "indonesio";
			case "is": return "island\u00e9s";
			case "it": return "italiano";
			case "ja": return "japon\u00E9s";
			case "ka": return "georgiano";
			case "kk": return "kazajo";
			case "kl": return "groenland\u00E9s";
			case "km": return "kmer";
			case "kn": return "canara";
			case "ko": return "coreano";
			case "ku": return "kurdo";
			case "kw": return "c\u00F3rnico";
			case "ky": return "kirghiz";
			case "la": return "lat\u00EDn";
			case "lt": return "lituano";
			case "lv": return "let\u00F3n";
			case "mk": return "macedonio";
			case "mn": return "mongol";
			case "mr": return "marathi";
			case "ms": return "malaisio";
			case "mt": return "malt\u00e9s";
			case "my": return "birmano";
			case "nl": return "holand\u00E9s";
			case "no": return "noruego";
			case "pa": return "punjab\u00ED";
			case "pl": return "polaco";
			case "pt": return "portugu\u00E9s";
			case "ro": return "rumano";
			case "ru": return "ruso";
			case "sh": return "serbo-croata";
			case "sk": return "eslovaco";
			case "sl": return "esloveno";
			case "so": return "somal\u00ED";
			case "sq": return "alban\u00E9s";
			case "sr": return "servio";
			case "sv": return "sueco";
			case "sw": return "swahili";
			case "te": return "telugu";
			case "th": return "tailand\u00E9s";
			case "ti": return "tigrinya";
			case "tr": return "turco";
			case "tt": return "tataro";
			case "vi": return "vietnam\u00E9s";
			case "uk": return "ucraniano";
			case "ur": return "urdu";
			case "uz": return "uzbeko";
			case "zh": return "chino";
			case "zu": return "zul\u00FA";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "AE": return "Emiratos \u00C1rabes Unidos";
			case "AS": return "Samoa Americana";
			case "BE": return "B\u00E9lgica";
			case "BH": return "Bahr\u00E1in";
			case "BR": return "Brasil";
			case "BZ": return "Belice";
			case "BY": return "Bielorrusia";
			case "CA": return "Canad\u00E1";
			case "CH": return "Suiza";
			case "CZ": return "Chequia";
			case "DE": return "Alemania";
			case "DK": return "Dinamarca";
			case "DO": return "Rep\u00FAblica Dominicana";
			case "DZ": return "Argelia";
			case "EG": return "Egipto";
			case "ES": return "Espa\u00F1a";
			case "FI": return "Finlandia";
			case "FO": return "Islas Feroe";
			case "FR": return "Francia";
			case "GB": return "Reino Unido";
			case "GL": return "Groenlanida";
			case "GR": return "Grecia";
			case "HR": return "Croacia";
			case "HU": return "Hungr\u00EDa";
			case "IE": return "Irlanda";
			case "IQ": return "Irak";
			case "IR": return "Ir\u00E1n";
			case "IS": return "Islandia";
			case "IT": return "Italia";
			case "JO": return "Jordania";
			case "JP": return "Jap\u00F3n";
			case "KE": return "Kenia";
			case "KP": return "Corea del Norte";
			case "KR": return "Corea del Sur";
			case "LB": return "L\u00EDbano";
			case "LT": return "Lituania";
			case "LU": return "Luxemburgo";
			case "LV": return "Letonia";
			case "MA": return "Marruecos";
			case "MH": return "Islas Marshall";
			case "MP": return "Islas Marianas del Norte";
			case "MX": return "M\u00E9xico";
			case "NL": return "Pa\u00EDses Bajos";
			case "NO": return "Noruega";
			case "NZ": return "Nueva Zelanda";
			case "OM": return "Om\u00E1n";
			case "PA": return "Panam\u00E1";
			case "PE": return "Per\u00FA";
			case "PH": return "Islas Filipinas";
			case "PK": return "Pakist\u00E1n";
			case "PL": return "Polonia";
			case "RO": return "Rumania";
			case "RU": return "Rusia";
			case "SA": return "Arabia Saud\u00ED";
			case "SD": return "Sud\u00E1n";
			case "SE": return "Suecia";
			case "SG": return "Singapur";
			case "SI": return "Eslovenia";
			case "SK": return "Eslovaquia";
			case "SP": return "Servia";
			case "SY": return "Siria";
			case "TH": return "Tailandia";
			case "TN": return "T\u00FAnez";
			case "TR": return "Turqu\u00EDa";
			case "TT": return "Trinidad y Tabago";
			case "TW": return "Taiw\u00E1n";
			case "UA": return "Ucraina";
			case "UM": return "Islas Perif\u00E9ricas Menores de los Estados Unidos";
			case "US": return "Estados Unidos";
			case "VI": return "Islas V\u00EDrgenes de los Estados Unidos";
			case "ZA": return "Sud\u00E1frica";
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
				return 20284;
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

}; // class CID000a

public class CNes : CID000a
{
	public CNes() : base() {}

}; // class CNes

}; // namespace I18N.West
