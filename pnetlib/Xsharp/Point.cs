/*
 * Point.cs - Public point structure type for X#.
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

namespace Xsharp
{

using System;

/// <summary>
/// <para>The <see cref="T:Xsharp.Point"/> structure defines
/// a specific screen location in pixels.</para>
/// </summary>
public struct Point
{
	/// <summary>
	/// <para>The X co-ordinate of the point.</para>
	/// </summary>
	public int x;

	/// <summary>
	/// <para>The Y co-ordinate of the point.</para>
	/// </summary>
	public int y;

	/// <summary>
	/// <para>Constructs a new point from a pair of co-ordinates.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the point.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the point.</para>
	/// </param>
	public Point(int x, int y)
			{
				this.x = x;
				this.y = y;
			}

} // struct Point

} // namespace Xsharp
