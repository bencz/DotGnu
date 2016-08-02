/*
 * CID0807.cs - de-CH culture handler.
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

// Generated from "de_CH.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0807 : CID0007
{
	public CID0807() : base(0x0807) {}

	public override String Name
	{
		get
		{
			return "de-CH";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "DES";
		}
	}
	public override String Country
	{
		get
		{
			return "CH";
		}
	}

	public override NumberFormatInfo NumberFormat
	{
		get
		{
			NumberFormatInfo nfi = base.NumberFormat;
			nfi.CurrencyDecimalSeparator = ".";
			nfi.CurrencyGroupSeparator = "'";
			nfi.NumberGroupSeparator = "'";
			nfi.PercentGroupSeparator = "'";
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

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "BD": return "Bangladesh";
			case "BW": return "Botswana";
			case "BN": return "Brunei";
			case "DJ": return "Djibouti";
			case "CV": return "Kapverden";
			case "MH": return "Marshall-Inseln";
			case "RW": return "Rwanda";
			case "SB": return "Salomon-Inseln";
			case "ST": return "Sao Tom\u00e9 und Principe";
			case "ZW": return "Zimbabwe";
			case "GB": return "Grossbritannien";
		}
		return base.ResolveCountry(name);
	}

}; // class CID0807

public class CNde_ch : CID0807
{
	public CNde_ch() : base() {}

}; // class CNde_ch

}; // namespace I18N.West
