/*
 * CID0816.cs - pt-PT culture handler.
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

// Generated from "pt_PT.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0816 : CID0016
{
	public CID0816() : base(0x0816) {}

	public override String Name
	{
		get
		{
			return "pt-PT";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "PTG";
		}
	}
	public override String Country
	{
		get
		{
			return "PT";
		}
	}

	public override NumberFormatInfo NumberFormat
	{
		get
		{
			NumberFormatInfo nfi = base.NumberFormat;
			nfi.CurrencyDecimalSeparator = "$";
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

}; // class CID0816

public class CNpt_pt : CID0816
{
	public CNpt_pt() : base() {}

}; // class CNpt_pt

}; // namespace I18N.West
