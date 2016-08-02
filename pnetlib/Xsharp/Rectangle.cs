/*
 * Rectangle.cs - Public rectangle structure type for X#.
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
/// <para>The <see cref="T:Xsharp.Rectangle"/> structure defines
/// a rectangular screen area in pixels.</para>
/// </summary>
public struct Rectangle
{
	/// <summary>
	/// <para>The X co-ordinate of the top-left corner of the rectangle.</para>
	/// </summary>
	public int x;

	/// <summary>
	/// <para>The Y co-ordinate of the top-left corner of the rectangle.</para>
	/// </summary>
	public int y;

	/// <summary>
	/// <para>The width of the rectangle.</para>
	/// </summary>
	public int width;

	/// <summary>
	/// <para>The height of the rectangle.</para>
	/// </summary>
	public int height;

	/// <summary>
	/// <para>Constructs a new rectangle from a set of co-ordinates.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the top-left corner of the rectangle.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the top-left corner of the rectangle.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The width of the rectangle.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The height of the rectangle.</para>
	/// </param>
	public Rectangle(int x, int y, int width, int height)
			{
				this.x = x;
				this.y = y;
				this.width = width;
				this.height = height;
			}

} // struct Rectangle

} // namespace Xsharp
