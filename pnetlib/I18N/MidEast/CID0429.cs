/*
 * CID0429.cs - fa-IR culture handler.
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

// Generated from "fa_IR.txt".

namespace I18N.MidEast
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0429 : CID0029
{
	public CID0429() : base(0x0429) {}

	public override String Name
	{
		get
		{
			return "fa-IR";
		}
	}
	public override String Country
	{
		get
		{
			return "IR";
		}
	}

	public override NumberFormatInfo NumberFormat
	{
		get
		{
			NumberFormatInfo nfi = base.NumberFormat;
			nfi.CurrencyDecimalSeparator = "\u066B";
			nfi.CurrencyGroupSeparator = "\u066C";
			nfi.NumberGroupSeparator = "\u066C";
			nfi.PercentGroupSeparator = "\u066C";
			nfi.NegativeSign = "-";
			nfi.NumberDecimalSeparator = "\u066B";
			nfi.PercentDecimalSeparator = "\u066B";
			nfi.PercentSymbol = "\u066A";
			nfi.PerMilleSymbol = "\u2030";
			return nfi;
		}
		set
		{
			base.NumberFormat = value; // not used
		}
	}

}; // class CID0429

public class CNfa_ir : CID0429
{
	public CNfa_ir() : base() {}

}; // class CNfa_ir

}; // namespace I18N.MidEast
