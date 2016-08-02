/*
 * StandardColor.cs - Indexes into the standard color palette.
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
/// <para>The <see cref="T:Xsharp.StandardColor"/> enumeration specifies
/// indexes into the standard color palette, independent of the particulars
/// of specific themes.</para>
/// </summary>
public enum StandardColor
{
	// Explicit RGB value (i.e. not a standard palette index).
	RGB,

	// Color set for drawing widget controls (e.g. buttons, dialogs).
	Foreground,
	Background,
	EndBackground,
	HighlightForeground,
	HighlightBackground,
	HighlightEndBackground,
	TopShadow,
	TopShadowEnhance,
	BottomShadow,
	BottomShadowEnhance,
	Trim,

	// Color set for drawing content widgets (e.g. text boxes, list boxes).
	ContentForeground,
	ContentBackground,
	ContentHighlightForeground,
	ContentHighlightBackground,
	ContentTopShadow,
	ContentTopShadowEnhance,
	ContentBottomShadow,
	ContentBottomShadowEnhance,
	ContentTrim,

	// Special color values for widget backgrounds.
	Inherit,
	Pixmap,

	// Must be the last color index.
	Last

} // enum StandardColor

} // namespace Xsharp
