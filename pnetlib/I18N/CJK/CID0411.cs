/*
 * CID0411.cs - ja-JP culture handler.
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

// Generated from "ja_JP.txt".

namespace I18N.CJK
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0411 : CID0011
{
	public CID0411() : base(0x0411) {}

	public override String Name
	{
		get
		{
			return "ja-JP";
		}
	}
	public override String Country
	{
		get
		{
			return "JP";
		}
	}

}; // class CID0411

public class CNja_jp : CID0411
{
	public CNja_jp() : base() {}

}; // class CNja_jp

}; // namespace I18N.CJK
