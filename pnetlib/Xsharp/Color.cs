/*
 * Color.cs - Color structure type for X#.
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
/// <para>The <see cref="T:Xsharp.Color"/> structure defines
/// a specific screen color as red, green, blue values, or as
/// an index into a standard color palette.</para>
/// </summary>
///
/// <remarks>
/// <para>This structure represents colors as a 24-bit RGB specification,
/// or as an index into a standardised palette.  It replaces the raw
/// pixels values that are used by the X protocol.  This makes the
/// application sensitive to theme changes.</para>
///
/// <para>Theoretically, X can handle displays with 48-bit RGB values.
/// Few applications need this level of color precision, and few X
/// servers support more than 24-bit color.</para>
/// </remarks>
public struct Color
{
	// Internal state.
	internal uint value;

	/// <summary>
	/// <para>A <see cref="T:Xsharp.Color"/> value representing
	/// black.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The color value.</para>
	/// </value>
	public static readonly Color Black = new Color(0x00, 0x00, 0x00);

	/// <summary>
	/// <para>A <see cref="T:Xsharp.Color"/> value representing
	/// white.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The color value.</para>
	/// </value>
	public static readonly Color White = new Color(0xFF, 0xFF, 0xFF);

	/// <summary>
	/// <para>Constructs a new <see cref="T:Xsharp.Color"/> value
	/// from red, green, and blue values.</para>
	/// </summary>
	///
	/// <param name="red">
	/// <para>The red component, between 0 and 255.</para>
	/// </param>
	///
	/// <param name="green">
	/// <para>The green component, between 0 and 255.</para>
	/// </param>
	///
	/// <param name="blue">
	/// <para>The blue component, between 0 and 255.</para>
	/// </param>
	public Color(int red, int green, int blue)
			{
				value = (((uint)red) << 16) | (((uint)green) << 8) |
						 ((uint)blue);
			}

	/// <summary>
	/// <para>Constructs a new <see cref="T:Xsharp.Color"/> value
	/// from an index into a standardised palette.</para>
	/// </summary>
	///
	/// <param name="index">
	/// <para>The index into the standard color palette.</para>
	/// </param>
	///
	/// <remarks>
	/// <para>It is recommended that you use standard colors where
	/// possible, so that the application is sensitive to theme changes.</para>
	/// </remarks>
	public Color(StandardColor index)
			{
				value = (((uint)index) << 24);
			}

	/// <summary>
	/// <para>Get the red component from this color.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The red component of the color, or 0 if this value is
	/// an index into the standard color palette.</para>
	/// </value>
	public byte Red
			{
				get
				{
					return (byte)(value >> 16);
				}
			}

	/// <summary>
	/// <para>Get the green component from this color.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The green component of the color, or 0 if this value is
	/// an index into the standard color palette.</para>
	/// </value>
	public byte Green
			{
				get
				{
					return (byte)(value >> 8);
				}
			}

	/// <summary>
	/// <para>Get the blue component from this color.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The blue component of the color, or 0 if this value is
	/// an index into the standard color palette.</para>
	/// </value>
	public byte Blue
			{
				get
				{
					return (byte)value;
				}
			}

	/// <summary>
	/// <para>Get the index into the standard color palette.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The standard color index, or <c>StandardColor.RGB</c> if
	/// this value has red, green, and blue components instead.</para>
	/// </value>
	public StandardColor Index
			{
				get
				{
					return (StandardColor)(value >> 24);
				}
			}

	/// <summary>
	/// <para>Compare two color values for equality.</para>
	/// </summary>
	///
	/// <param name="c1">
	/// <para>The first color value to compare.</para>
	/// </param>
	///
	/// <param name="c2">
	/// <para>The second color value to compare.</para>
	/// </param>
	public static bool operator==(Color c1, Color c2)
			{
				return (c1.value == c2.value);
			}

	/// <summary>
	/// <para>Compare two color values for inequality.</para>
	/// </summary>
	///
	/// <param name="c1">
	/// <para>The first color value to compare.</para>
	/// </param>
	///
	/// <param name="c2">
	/// <para>The second color value to compare.</para>
	/// </param>
	public static bool operator!=(Color c1, Color c2)
			{
				return (c1.value != c2.value);
			}

	/// <summary>
	/// <para>Get the hash code for this color value.</para>
	/// </summary>
	public override int GetHashCode()
			{
				return (int)value;
			}

	/// <summary>
	/// <para>Determine if two color values are equal.</para>
	/// </summary>
	///
	/// <param name="obj">
	/// <para>The color object to compare against.</para>
	/// </param>
	public override bool Equals(Object obj)
			{
				if(obj is Color)
				{
					Color c = (Color)obj;
					return (c.value == value);
				}
				return false;
			}

} // struct Color

} // namespace Xsharp
