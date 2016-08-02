/*
 * CID0414.cs - nb-NO culture handler.
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

// Generated from "nb_NO.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0414 : CID0014
{
	public CID0414() : base(0x0414) {}

	public override String Name
	{
		get
		{
			return "nb-NO";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "nob";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "nb";
		}
	}
	public override String Country
	{
		get
		{
			return "NO";
		}
	}

}; // class CID0414

public class CNnb_no : CID0414
{
	public CNnb_no() : base() {}

}; // class CNnb_no

public class CNnb : CID0414
{
	public CNnb() : base() {}

}; // class CNnb

}; // namespace I18N.West
