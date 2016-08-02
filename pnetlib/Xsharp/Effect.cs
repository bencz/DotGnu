/*
 * Effect.cs - 3D effect codes for "Graphics.DrawEffect".
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
/// <para>The <see cref="T:Xsharp.Effect"/> enumeration specifies
/// 3D effects (<see cref="T:Xsharp.Graphics"/>).
/// </para>
/// </summary>
[Flags]
public enum Effect
{

	Raised						= (1<<0),
	Indented					= (1<<1),
	Etched						= (1<<2),
	Horizontal					= (1<<3),
	Vertical					= (1<<4),
	RadioBlank					= (1<<5),
	RadioSelected				= (1<<6),
	RadioDisabled				= (1<<7),
	RadioSelectedDisabled		= (1<<8),
	CheckBlank					= (1<<9),
	CheckSelected				= (1<<10),
	CheckDisabled				= (1<<11),
	CheckSelectedDisabled		= (1<<12),
	MenuSelected				= (1<<13),
	MenuSelectedHighlighted		= (1<<14),
	MenuSelectedDisabled		= (1<<15),
	ContentColors				= (1<<16),
	DefaultButton				= (1<<17),
	CaptionButtonRaised			= (1<<18),
	CaptionButtonIndented		= (1<<19),

} // enum Effect

} // namespace Xsharp
