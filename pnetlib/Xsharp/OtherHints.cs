/*
 * OtherHints.cs - Other window manager hint flags.
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
/// <para>The <see cref="T:Xsharp.OtherHints"/> enumeration specifies
/// hint flags for the window manager, to specify physical styles.</para>
/// </summary>
[Flags]
public enum OtherHints
{
	None				= 0,
	ToolWindow			= (1 << 0),
	HideFromTaskBar		= (1 << 1),
	HelpButton			= (1 << 2),
	Dialog				= (1 << 3),
	TopMost				= (1 << 4),
	Desktop				= (1 << 5),
	Dock				= (1 << 6),
	Menu				= (1 << 7),
	Splash				= (1 << 8),
	Sticky				= (1 << 9),
	Shaded				= (1 << 10),
	Hidden				= (1 << 11),
	FullScreen			= (1 << 12)

} // enum OtherHints

} // namespace Xsharp
