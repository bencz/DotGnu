/*
 * CID0441.cs - sw-KE culture handler.
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

// Generated from "sw_KE.txt".

namespace I18N.Other
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0441 : CID0041
{
	public CID0441() : base(0x0441) {}

	public override String Name
	{
		get
		{
			return "sw-KE";
		}
	}
	public override String Country
	{
		get
		{
			return "KE";
		}
	}

}; // class CID0441

public class CNsw_ke : CID0441
{
	public CNsw_ke() : base() {}

}; // class CNsw_ke

}; // namespace I18N.Other
