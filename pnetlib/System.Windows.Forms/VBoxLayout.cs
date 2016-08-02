/*
 * VBoxLayout.cs - Implementation of the
 *			"System.Windows.Forms.VBoxLayout" class.
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

// This is a special-purpose control that lays out its children vertically.
// It is intended for use inside dialog box controls like "MessageBox".

internal class VBoxLayout : Control, IRecommendedSize
{
	// Internal state.
	private bool uniformSize;
	private Control stretchControl;
	private int margin, spacing;

	// Constructor.
	public VBoxLayout()
			{
				uniformSize = false;
				stretchControl = null;
				margin = 4;
				spacing = 4;
				TabStop = false;
			}

	// Get or set whether all controls will have a uniform size.
	public bool UniformSize
			{
				get
				{
					return uniformSize;
				}
				set
				{
					uniformSize = value;
				}
			}

	// Get or set the control to be stretched in non-uniform layouts.
	// If this is not set, then the last control will be stretched.
	public Control StretchControl
			{
				get
				{
					return stretchControl;
				}
				set
				{
					stretchControl = value;
				}
			}

	// Get or set the margin.
	public int Margin
			{
				get
				{
					return margin;
				}
				set
				{
					margin = value;
				}
			}

	// Get or set the spacing between items.
	public int Spacing
			{
				get
				{
					return spacing;
				}
				set
				{
					spacing = value;
				}
			}

	// Get the recommended client size for this control.
	public Size RecommendedSize
			{
				get
				{
					int height = 0;
					int maxWidth = 0;
					int maxHeight = 0;
					int numVisible = 0;
					int xextra, yextra;
					Size childSize;
					foreach(Control child in Controls)
					{
						if(child.visible)
						{
							childSize = HBoxLayout.GetRecommendedSize(child);
							height += childSize.Height;
							if(childSize.Width > maxWidth)
							{
								maxWidth = childSize.Width;
							}
							if(childSize.Height > maxHeight)
							{
								maxHeight = childSize.Height;
							}
							++numVisible;
						}
					}
					xextra = margin * 2;
					yextra = margin * 2;
					if(numVisible >= 2)
					{
						yextra += (numVisible - 1) * spacing;
					}
					if(uniformSize)
					{
						return new Size
							(maxWidth + xextra,
							 maxHeight * numVisible + yextra);
					}
					else
					{
						return new Size
							(maxWidth + xextra, height + yextra);
					}
				}
			}

	// Lay out the children in this control uniformly.
	private void UniformLayout()
			{
				int numVisible = 0;
				int index;
				int width, height;

				// Count the number of visible child controls.
				foreach(Control child1 in Controls)
				{
					if(child1.visible)
					{
						++numVisible;
					}
				}
				if(numVisible == 0)
				{
					return;
				}

				// Determine the optimal width and height.
				width = ClientSize.Width - margin * 2;
				height = ClientSize.Height - margin * 2;
				height -= (numVisible - 1) * spacing;
				height = height / numVisible;

				// Lay out the visible controls.
				index = 0;
				foreach(Control child in Controls)
				{
					if(!(child.visible))
					{
						continue;
					}
					if(index == (numVisible - 1) && margin == 0)
					{
						// Lay out the final control to the full width,
						// to undo the effect of slight rounding errors.
						child.SetBounds
							(0, index * (height + spacing), width,
							 ClientSize.Height - index * (height + spacing));
					}
					else
					{
						// Lay out some control other than the last.
						child.SetBounds
							(margin, margin + index * (height + spacing),
							 width, height);
					}
					++index;
				}
			}

	// Lay out the children in this control non-uniformly.
	private void NonUniformLayout()
			{
				ControlCollection controls = Controls;
				int count, index;
				Control stretch;
				Control child;
				int posn, posn2;
				Size clientSize;
				Size childSize;

				// Find the control to be stretched.
				if(stretchControl != null && stretchControl.visible)
				{
					stretch = stretchControl;
				}
				else
				{
					stretch = null;
					foreach(Control child1 in controls)
					{
						if(child1.visible)
						{
							stretch = child1;
						}
					}
					if(stretch == null)
					{
						// Abort layout - none of the children are visible.
						return;
					}
				}

				// Lay out the children before the stretched control.
				count = controls.Count;
				index = 0;
				posn = margin;
				clientSize = ClientSize;
				while(index < count)
				{
					child = controls[index];
					if(child == stretch)
					{
						break;
					}
					if(child.visible)
					{
						childSize = HBoxLayout.GetRecommendedSize(child);
						child.SetBounds
							(margin, posn, clientSize.Width - 2 * margin,
							 childSize.Height);
						posn += childSize.Height + spacing;
					}
					++index;
				}

				// Lay out the children after the stretched control.
				posn2 = clientSize.Height - margin;
				index = count - 1;
				while(index >= 0)
				{
					child = controls[index];
					if(child == stretch)
					{
						break;
					}
					if(child.visible)
					{
						childSize = HBoxLayout.GetRecommendedSize(child);
						posn2 -= childSize.Height;
						child.SetBounds
							(margin, posn2, clientSize.Width - 2 * margin,
							 childSize.Height);
						posn2 -= spacing;
					}
					--index;
				}

				// Lay out the stretched control.
				if(posn2 < posn)
				{
					posn2 = posn;
				}
				stretch.SetBounds
					(margin, posn, clientSize.Width - 2 * margin,
					 posn2 - posn);
			}

	// Lay out the children in this control.
	protected override void OnLayout(LayoutEventArgs e)
			{
				if(uniformSize)
				{
					UniformLayout();
				}
				else
				{
					NonUniformLayout();
				}
			}

}; // class VBoxLayout

}; // namespace System.Windows.Forms
