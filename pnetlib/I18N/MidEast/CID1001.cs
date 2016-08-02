/*
 * CID1001.cs - ar-LY culture handler.
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

// Generated from "ar_LY.txt".

namespace I18N.MidEast
{

using System;
using System.Globalization;
using I18N.Common;

public class CID1001 : CID0001
{
	public CID1001() : base(0x1001) {}

	public override String Name
	{
		get
		{
			return "ar-LY";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "ARL";
		}
	}
	public override String Country
	{
		get
		{
			return "LY";
		}
	}

}; // class CID1001

public class CNar_ly : CID1001
{
	public CNar_ly() : base() {}

}; // class CNar_ly

}; // namespace I18N.MidEast
