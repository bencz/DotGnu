/*
 * CP949.cs - Code page handling for Korean.
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

// Note: "ksc.table" is generated from "KSC5601.TXT" (ftp.unicode.org).

public class CP949 : MultiByteEncoding
{
	// Constructor.
	public CP949() : base(949, "ks_c_5601-1987", "Korean",
						  "ks_c_5601-1987", "ks_c_5601-1987",
						  949, "ksc.table")
			{
				// Nothing to do here.
			}

}; // class CP949

// Name form of the above class.
public class ENCks_c_5601_1987 : CP949 {}

}; // namespace I18N.CJK
