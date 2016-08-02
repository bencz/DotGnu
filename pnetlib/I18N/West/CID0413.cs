/*
 * CID0413.cs - nl-NL culture handler.
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

// Generated from "nl_NL.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0413 : CID0013
{
	public CID0413() : base(0x0413) {}

	public override String Name
	{
		get
		{
			return "nl-NL";
		}
	}
	public override String Country
	{
		get
		{
			return "NL";
		}
	}

}; // class CID0413

public class CNnl_nl : CID0413
{
	public CNnl_nl() : base() {}

}; // class CNnl_nl

}; // namespace I18N.West
