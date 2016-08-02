/*
 * MotifDecorations.cs - Motif window manager decoration flags.
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

namespace Xsharp
{

using System;

/// <summary>
/// <para>The <see cref="T:Xsharp.MotifDecorations"/> enumeration specifies
/// flags for the window manager, to indicate which window decorations
/// are desired by the application.</para>
/// </summary>
///
/// <remarks>
/// <para>If <c>All</c> is specified, then it indicates all decorations
/// except those explicitly listed.  If <c>All</c> is not specified,
/// then it indicates only those decorations that are explicitly listed.</para>
/// </remarks>
[Flags]
public enum MotifDecorations
{
	All				= (1 << 0),
	Border			= (1 << 1),
	ResizeHandles	= (1 << 2),
	Title			= (1 << 3),
	Menu			= (1 << 4),
	Minimize		= (1 << 5),
	Maximize		= (1 << 6),

} // enum MotifDecorations

} // namespace Xsharp
