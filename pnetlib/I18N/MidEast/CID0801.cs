/*
 * CID0801.cs - ar-IQ culture handler.
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

// Generated from "ar_IQ.txt".

namespace I18N.MidEast
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0801 : CID0001
{
	public CID0801() : base(0x0801) {}

	public override String Name
	{
		get
		{
			return "ar-IQ";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "ARI";
		}
	}
	public override String Country
	{
		get
		{
			return "IQ";
		}
	}

}; // class CID0801

public class CNar_iq : CID0801
{
	public CNar_iq() : base() {}

}; // class CNar_iq

}; // namespace I18N.MidEast
