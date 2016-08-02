/*
 * CP1361.cs - Code page handling for Korean (Johab).
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

namespace I18N.CJK
{

using System;
using System.Text;
using I18N.Common;

// Note: "johab.table" is generated from "JOHAB.TXT" (ftp.unicode.org).

public class CP1361 : MultiByteEncoding
{
	// Constructor.
	public CP1361() : base(1361, "x-johab", "Korean (Johab)",
						   "x-johab", "x-johab", 949, "johab.table")
			{
				// Nothing to do here.
			}

}; // class CP1361

// Name form of the above class.
public class ENCx_johab : CP1361 {}

}; // namespace I18N.CJK
