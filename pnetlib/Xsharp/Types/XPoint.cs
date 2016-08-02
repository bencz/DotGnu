/*
 * XPoint.cs - Definition of the X point type structure.
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
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

namespace Xsharp.Types
{

using System;
using System.Runtime.InteropServices;

// X point structure.
[StructLayout(LayoutKind.Sequential)]
internal struct XPoint
{
	public short x;
	public short y;

	// Constructor.
	public XPoint(int x, int y)
			{
				try
				{
					checked
					{
						this.x = (short)x;
						this.y = (short)y;
					}
				}
				catch(OverflowException)
				{
					throw new XException(S._("X_PointCoordRange"));
				}
			}

} // struct XPoint

} // namespace Xsharp.Types
