/*
 * HBoxLayout.cs - Implementation of the
 *			"System.Windows.Forms.HBoxLayout" class.
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

// This is a special-purpose control that lays out its children horizontally.
// It is intended for use inside dialog box controls like "MessageBox".

internal class HBoxLayout : Control, IRecommendedSize
{
	// Internal state.
	private bool uniformSize;
	private Control stretchControl;
	protected int margin, spacing;

	// Constructor.
	public HBoxLayout()
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

	// Get the recommended size for a control.
	internal static Size GetRecommendedSize(Control control)
			{
				if(control is IRecommendedSize)
				{
					return ((IRecommendedSize)control).RecommendedSize;
				}
				else if(control is Label)
				{
					return (control as Label).GetPreferredSize();
				}
				else
				{
					return control.Size;
				}
			}

	// Get the recommended client size for this control.
	public Size RecommendedSize
			{
				get
				{
					int width = 0;
					int maxWidth = 0;
					int maxHeight = 0;
					int numVisible = 0;
					int xextra, yextra;
					Size childSize;
					foreach(Control child in Controls)
					{
						if(child.visible)
						{
							childSize = GetRecommendedSize(child);
							width += childSize.Width;
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
					if(numVisible >= 2)
					{
						xextra += (numVisible - 1) * spacing;
					}
					yextra = margin * 2;
					if(uniformSize)
					{
						return new Size
							(maxWidth * numVisible + xextra,
							 maxHeight + yextra);
					}
					else
					{
						return new Size
							(width + xextra, maxHeight + yextra);
					}
				}
			}

	// Lay out the children in this control uniformly.
	protected virtual void UniformLayout()
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
				width -= (numVisible - 1) * spacing;
				width = width / numVisible;
				height = ClientSize.Height - margin * 2;

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
							(index * (width + spacing), 0,
							 ClientSize.Width - index * width, height);
					}
					else
					{
						// Lay out some control other than the last.
						child.SetBounds
							(margin + index * (width + spacing), margin,
							 width, height);
					}
					++index;
				}
			}

	// Lay out the children in this control non-uniformly.
	protected virtual void NonUniformLayout()
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
						childSize = GetRecommendedSize(child);
						child.SetBounds
							(posn, margin, childSize.Width,
							 clientSize.Height - 2 * margin);
						posn += childSize.Width + spacing;
					}
					++index;
				}

				// Lay out the children after the stretched control.
				posn2 = clientSize.Width - margin;
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
						childSize = GetRecommendedSize(child);
						posn2 -= childSize.Width;
						child.SetBounds
							(posn2, margin, childSize.Width,
							 clientSize.Height - 2 * margin);
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
					(posn, margin, posn2 - posn,
					 clientSize.Height - 2 * margin);
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

}; // class HBoxLayout

}; // namespace System.Windows.Forms
