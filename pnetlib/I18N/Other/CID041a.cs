/*
 * CID041a.cs - hr-HR culture handler.
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

// Generated from "hr_HR.txt".

namespace I18N.Other
{

using System;
using System.Globalization;
using I18N.Common;

public class CID041a : CID001a
{
	public CID041a() : base(0x041A) {}

	public override String Name
	{
		get
		{
			return "hr-HR";
		}
	}
	public override String Country
	{
		get
		{
			return "HR";
		}
	}

}; // class CID041a

public class CNhr_hr : CID041a
{
	public CNhr_hr() : base() {}

}; // class CNhr_hr

}; // namespace I18N.Other
