/*
 * CID040b.cs - fi-FI culture handler.
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

// Generated from "fi_FI.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID040b : CID000b
{
	public CID040b() : base(0x040B) {}

	public override String Name
	{
		get
		{
			return "fi-FI";
		}
	}
	public override String Country
	{
		get
		{
			return "FI";
		}
	}

}; // class CID040b

public class CNfi_fi : CID040b
{
	public CNfi_fi() : base() {}

}; // class CNfi_fi

}; // namespace I18N.West
