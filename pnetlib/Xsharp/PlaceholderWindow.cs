/*
 * PlaceholderWindow.cs - Placeholder for unparented widgets.
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

// Special window that holds child widgets that don't yet have real parents.

internal class PlaceholderWindow : InputOutputWidget
{
	// Constructor.
	public PlaceholderWindow(Widget parent)
			: base(parent, 0, 0, 1, 1,
			       new Color(StandardColor.Foreground),
			       new Color(StandardColor.Background),
				   true, true)
			{
				autoMapChildren = false;
			}

	// Disable certain widget operations which we don't want.
	public override void Destroy()
			{
				// Nothing to do here.
			}
	public override void Map()
			{
				throw new XInvalidOperationException
					(S._("X_NonPlaceholderOperation"));
			}
	public override void Unmap()
			{
				throw new XInvalidOperationException
					(S._("X_NonPlaceholderOperation"));
			}
	public override void Reparent(Widget newParent, int x, int y)
			{
				throw new XInvalidOperationException
					(S._("X_NonPlaceholderOperation"));
			}

} // class PlaceholderWindow

} // namespace Xsharp
