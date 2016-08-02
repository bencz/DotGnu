/*
 * CID0009.cs - en culture handler.
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

// Generated from "en.txt".

namespace I18N.Common
{

using System;
using System.Globalization;

public class CID0009 : RootCulture
{
	public CID0009() : base(0x0009) {}
	public CID0009(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "en";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "eng";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "ENU";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "en";
		}
	}

}; // class CID0009

public class CNen : CID0009
{
	public CNen() : base() {}

}; // class CNen

}; // namespace I18N.Common
