/*
 * CID040e.cs - hu-HU culture handler.
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

// Generated from "hu_HU.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID040e : CID000e
{
	public CID040e() : base(0x040E) {}

	public override String Name
	{
		get
		{
			return "hu-HU";
		}
	}
	public override String Country
	{
		get
		{
			return "HU";
		}
	}

}; // class CID040e

public class CNhu_hu : CID040e
{
	public CNhu_hu() : base() {}

}; // class CNhu_hu

}; // namespace I18N.West
