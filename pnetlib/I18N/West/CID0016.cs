/*
 * CID0016.cs - pt culture handler.
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

// Generated from "pt.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0016 : RootCulture
{
	public CID0016() : base(0x0016) {}
	public CID0016(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "pt";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "por";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "PTB";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "pt";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "S\u00E1b"};
			dfi.DayNames = new String[] {"Domingo", "Segunda-feira", "Ter\u00E7a-feira", "Quarta-feira", "Quinta-feira", "Sexta-feira", "S\u00E1bado"};
			dfi.AbbreviatedMonthNames = new String[] {"Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez", ""};
			dfi.MonthNames = new String[] {"Janeiro", "Fevereiro", "Mar\u00E7o", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro", ""};
			dfi.DateSeparator = "-";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "d' de 'MMMM' de 'yyyy";
			dfi.LongTimePattern = "HH:mm:ss z";
			dfi.ShortDatePattern = "dd-MM-yyyy";
			dfi.ShortTimePattern = "HH:mm";
			dfi.FullDateTimePattern = "dddd, d' de 'MMMM' de 'yyyy HH'H'mm'm' z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:dd-MM-yyyy",
				"D:dddd, d' de 'MMMM' de 'yyyy",
				"f:dddd, d' de 'MMMM' de 'yyyy HH'H'mm'm' z",
				"f:dddd, d' de 'MMMM' de 'yyyy HH:mm:ss z",
				"f:dddd, d' de 'MMMM' de 'yyyy HH:mm:ss",
				"f:dddd, d' de 'MMMM' de 'yyyy HH:mm",
				"F:dddd, d' de 'MMMM' de 'yyyy HH:mm:ss",
				"g:dd-MM-yyyy HH'H'mm'm' z",
				"g:dd-MM-yyyy HH:mm:ss z",
				"g:dd-MM-yyyy HH:mm:ss",
				"g:dd-MM-yyyy HH:mm",
				"G:dd-MM-yyyy HH:mm:ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:HH'H'mm'm' z",
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
			case "ar": return "\u00c1rabe";
			case "az": return "Azerbaij\u00e3o";
			case "ba": return "Basco";
			case "be": return "Bielo-russo";
			case "bg": return "B\u00falgaro";
			case "bn": return "Bengala";
			case "ca": return "Catal\u00e3o";
			case "cs": return "Tcheco";
			case "da": return "Dinamarqu\u00eas";
			case "de": return "Alem\u00e3o";
			case "el": return "Grego";
			case "en": return "Ingl\u00eas";
			case "es": return "Espanhol";
			case "et": return "Est\u00f4nio";
			case "fa": return "Farsi";
			case "fi": return "Finland\u00eas";
			case "fr": return "Franc\u00eas";
			case "gu": return "Guzarate";
			case "he": return "Hebreu";
			case "hr": return "Croata";
			case "hu": return "H\u00fangaro";
			case "id": return "Indon\u00e9sio";
			case "it": return "Italiano";
			case "ja": return "Japon\u00eas";
			case "ka": return "Georgiano";
			case "km": return "Cmer";
			case "kn": return "Canad\u00e1";
			case "ko": return "Coreano";
			case "ku": return "Curdo";
			case "la": return "Latino";
			case "lt": return "Lituano";
			case "lv": return "Let\u00e3o";
			case "mk": return "Maced\u00f4nio";
			case "mr": return "Marati";
			case "my": return "Birman\u00eas";
			case "nl": return "Holand\u00eas";
			case "no": return "Noruegu\u00eas";
			case "pl": return "Polon\u00eas";
			case "pt": return "Portugu\u00eas";
			case "ro": return "Romeno";
			case "ru": return "Russo";
			case "sk": return "Eslovaco";
			case "sl": return "Esloveno";
			case "sq": return "Alban\u00eas";
			case "sr": return "S\u00e9rvio";
			case "sv": return "Su\u00e9co";
			case "te": return "T\u00e9lugu";
			case "th": return "Tailand\u00eas";
			case "tk": return "Tagalo";
			case "tr": return "Turco";
			case "uk": return "Ucraniano";
			case "uz": return "Usbeque";
			case "zh": return "Chin\u00eas";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "AE": return "Rep\u00fablica \u00c1rabe Unida";
			case "AL": return "Alb\u00e2nia";
			case "AS": return "\u00c1sia";
			case "AT": return "\u00c1ustria";
			case "AU": return "Austr\u00e1lia";
			case "BA": return "B\u00f3snia";
			case "BE": return "B\u00e9lgica";
			case "BG": return "Bulg\u00e1ria";
			case "BH": return "Bar\u00e1in";
			case "BO": return "Bol\u00edvia";
			case "BR": return "Brasil";
			case "CA": return "Canad\u00e1";
			case "CH": return "Su\u00ed\u00e7a";
			case "CN": return "China (PRC)";
			case "CO": return "Col\u00f4mbia";
			case "CZ": return "Rep\u00fablica Tcheca";
			case "DE": return "Alemanha";
			case "DK": return "Dinamarca";
			case "DO": return "Rep\u00fablica Dominicana";
			case "DZ": return "Arg\u00e9lia";
			case "EC": return "Equador";
			case "EE": return "Est\u00f4nia";
			case "EG": return "Egito";
			case "ES": return "Espanha";
			case "FI": return "Finl\u00e2ndia";
			case "FR": return "Fran\u00e7a";
			case "GB": return "Reino Unido";
			case "GR": return "Gr\u00e9cia";
			case "HR": return "Cro\u00e1cia";
			case "HU": return "Hungria";
			case "ID": return "Indon\u00e9sia";
			case "IE": return "Irlanda";
			case "IN": return "\u00cdndia";
			case "IS": return "Isl\u00e2ndia";
			case "IT": return "It\u00e1lia";
			case "JO": return "Jord\u00e2nia";
			case "JP": return "Jap\u00e3o";
			case "KR": return "Cor\u00e9ia";
			case "LA": return "Am\u00e9rica Latina";
			case "LB": return "L\u00edbano";
			case "LT": return "Litu\u00e2nia";
			case "LU": return "Luxemburgo";
			case "LV": return "Let\u00f4nia";
			case "MA": return "Marrocos";
			case "MK": return "Maced\u00f4nia FYR";
			case "MX": return "M\u00e9xico";
			case "NI": return "Nicar\u00e1gua";
			case "NL": return "Pa\u00edses Baixos";
			case "NO": return "Noruega";
			case "NZ": return "Nova Zel\u00e2ndia";
			case "OM": return "Om\u00e3";
			case "PA": return "Panam\u00e1";
			case "PK": return "Paquist\u00e3o";
			case "PL": return "Pol\u00f4nia";
			case "PY": return "Paraguai";
			case "QA": return "Catar";
			case "RO": return "Rom\u00eania";
			case "RU": return "R\u00fassia";
			case "SA": return "Ar\u00e1bia Saudita";
			case "SE": return "Su\u00e9cia";
			case "SG": return "Cingapura";
			case "SI": return "Eslov\u00eania";
			case "SK": return "Eslov\u00e1quia";
			case "SP": return "S\u00e9rvia";
			case "SY": return "S\u00edria";
			case "TH": return "Tail\u00e2ndia";
			case "TN": return "Tun\u00edsia";
			case "TR": return "Turquia";
			case "UA": return "Ucr\u00e2nia";
			case "US": return "Estados Unidos";
			case "UY": return "Uruguai";
			case "ZA": return "\u00c1frica do Sul";
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
				return 500;
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

}; // class CID0016

public class CNpt : CID0016
{
	public CNpt() : base() {}

}; // class CNpt

}; // namespace I18N.West
