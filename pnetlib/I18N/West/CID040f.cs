/*
 * CID040f.cs - is-IS culture handler.
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

// Generated from "is_IS.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID040f : CID000f
{
	public CID040f() : base(0x040F) {}

	public override String Name
	{
		get
		{
			return "is-IS";
		}
	}
	public override String Country
	{
		get
		{
			return "IS";
		}
	}

}; // class CID040f

public class CNis_is : CID040f
{
	public CNis_is() : base() {}

}; // class CNis_is

}; // namespace I18N.West
