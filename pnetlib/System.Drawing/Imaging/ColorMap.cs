/*
 * ColorMap.cs - Implementation of the
 *			"System.Drawing.Imaging.ColorMap" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

namespace System.Drawing.Imaging
{

public sealed class ColorMap
{
	// Internal state.
	private Color newColor;
	private Color oldColor;

	// Constructor.
	public ColorMap() {}

	// Get or set this object's properties.
	public Color NewColor
			{
				get
				{
					return newColor;
				}
				set
				{
					newColor = value;
				}
			}
	public Color OldColor
			{
				get
				{
					return oldColor;
				}
				set
				{
					oldColor = value;
				}
			}

}; // class ColorMap

}; // namespace System.Drawing.Imaging
