/*
 * ButtonBoxLayout.cs - Implementation of the
 *			"System.Windows.Forms.ButtonBoxLayout" class.
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

namespace System.Windows.Forms
{

using System.Drawing;

// This is a special-purpose control that lays out its children horizontally,
// under the assumption that they are buttons along the bottom of a dialog.
// It is intended for use inside dialog box controls like "MessageBox".

internal class ButtonBoxLayout : HBoxLayout
{
	// Constructor.
	public ButtonBoxLayout() : base()
			{
				UniformSize = true;
			}

	// Lay out the children in this control uniformly.  We leave extra
	// space on the left and right, with the buttons laid out in the
	// middle of the layout area.
	protected override void UniformLayout()
			{
				int numVisible = 0;
				int index;
				int width, height;
				int maxWidth;
				int xposn;
				Size childSize;

				// Count the number of visible child controls
				// and discover the maximum control width.
				maxWidth = 0;
				foreach(Control child1 in Controls)
				{
					if(child1.visible)
					{
						childSize = GetRecommendedSize(child1);
						if(childSize.Width > maxWidth)
						{
							maxWidth = childSize.Width;
						}
						++numVisible;
					}
				}
				if(numVisible == 0)
				{
					return;
				}

				// Determine the optimal width and height.
				width = maxWidth;
				xposn = width * numVisible + (numVisible - 1) * spacing;
				xposn = (ClientSize.Width - xposn) / 2;
				height = ClientSize.Height - margin * 2;

				// Lay out the visible controls.
				index = 0;
				foreach(Control child in Controls)
				{
					if(!(child.visible))
					{
						continue;
					}
					child.SetBounds
						(xposn + index * (width + spacing), margin,
						 width, height);
					++index;
				}
			}

}; // class ButtonBoxLayout

}; // namespace System.Windows.Forms
