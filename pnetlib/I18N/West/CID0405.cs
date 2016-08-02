/*
 * CID0405.cs - cs-CZ culture handler.
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

// Generated from "cs_CZ.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0405 : CID0005
{
	public CID0405() : base(0x0405) {}

	public override String Name
	{
		get
		{
			return "cs-CZ";
		}
	}
	public override String Country
	{
		get
		{
			return "CZ";
		}
	}

}; // class CID0405

public class CNcs_cz : CID0405
{
	public CNcs_cz() : base() {}

}; // class CNcs_cz

}; // namespace I18N.West
