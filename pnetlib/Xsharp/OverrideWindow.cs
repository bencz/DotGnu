/*
 * OverrideWindow.cs - Widget handling for override-redirect windows.
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
/// <para>The <see cref="T:Xsharp.OverrideWindow"/> class manages
/// windows that have the override-redirect bit set.</para>
///
/// <para>Applications should inherit from <see cref="T:Xsharp.PopupWindow"/>
/// instead of this class.</para>
/// </summary>
public class OverrideWindow : InputOutputWidget
{
	// Constructor.
	internal OverrideWindow(Widget parent, int x, int y, int width, int height)
			: base(parent, x, y, width, height,
				   new Color(StandardColor.Foreground),
				   new Color(StandardColor.Background),
				   true, true)
			{
				// Nothing to do here.
			}

} // class OverrideWindow

} // namespace Xsharp
