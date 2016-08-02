/*
 * GridLayout.cs - Implementation of the
 *			"System.Windows.Forms.GridLayout" class.
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

// This is a special-purpose control that lays out its children in a grid.
// It is intended for use inside dialog box controls like "FileDialog".

internal class GridLayout : Control, IRecommendedSize
{
	// Internal state.
	private int columns;
	private int rows;
	private int stretchColumn;
	private int stretchRow;
	private int margin;
	private int colSpacing;
	private int rowSpacing;
	private Control[] children;

	// Constructor.
	public GridLayout(int columns, int rows)
			{
				this.columns = columns;
				this.rows = rows;
				this.stretchColumn = columns - 1;
				this.stretchRow = rows - 1;
				this.margin = 4;
				this.colSpacing = 4;
				this.rowSpacing = 4;
				this.children = new Control [columns * rows];
				this.TabStop = false;
			}

	// Get or set the column to be stretched.
	public int StretchColumn
			{
				get
				{
					return stretchColumn;
				}
				set
				{
					stretchColumn = value;
				}
			}

	// Get or set the row to be stretched.
	public int StretchRow
			{
				get
				{
					return stretchRow;
				}
				set
				{
					stretchRow = value;
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

	// Get or set the spacing between columns.
	public int ColumnSpacing
			{
				get
				{
					return colSpacing;
				}
				set
				{
					colSpacing = value;
				}
			}

	// Get or set the spacing between rows.
	public int RowSpacing
			{
				get
				{
					return rowSpacing;
				}
				set
				{
					rowSpacing = value;
				}
			}

	// Get the child control at a particular location.
	public Control GetControl(int column, int row)
			{
				return children[column * rows + row];
			}

	// Set the child control at a particular location.  Locations can be blank.
	public void SetControl(int column, int row, Control child)
			{
				if(child.Parent != this)
				{
					Controls.Add(child);
				}
				children[column * rows + row] = child;
			}

	// Get the recommended client size for this control.
	public Size RecommendedSize
			{
				get
				{
					int width = 0;
					int height = 0;
					int maxWidth;
					int maxHeight;
					int xextra, yextra;
					Size childSize;
					int x, y;
					Control child;

					// Scan the columns, looking for maximums.
					for(x = 0; x < columns; ++x)
					{
						maxWidth = 0;
						for(y = 0; y < rows; ++y)
						{
							child = GetControl(x, y);
							if(child != null && child.visible)
							{
								childSize =
									HBoxLayout.GetRecommendedSize(child);
								if(childSize.Width > maxWidth)
								{
									maxWidth = childSize.Width;
								}
							}
						}
						width += maxWidth;
					}

					// Scan the rows, looking for maximums.
					for(y = 0; y < rows; ++y)
					{
						maxHeight = 0;
						for(x = 0; x < columns; ++x)
						{
							child = GetControl(x, y);
							if(child != null && child.visible)
							{
								childSize =
									HBoxLayout.GetRecommendedSize(child);
								if(childSize.Height > maxHeight)
								{
									maxHeight = childSize.Height;
								}
							}
						}
						height += maxHeight;
					}

					// Add the margins and return the final size.
					xextra = margin * 2;
					if(columns >= 2)
					{
						xextra += (columns - 1) * colSpacing;
					}
					yextra = margin * 2;
					if(rows >= 2)
					{
						yextra += (rows - 1) * rowSpacing;
					}
					return new Size(width + xextra, height + yextra);
				}
			}

	// Lay out the children in this control.
	protected override void OnLayout(LayoutEventArgs e)
			{
				int[] columnOffsets = new int [columns];
				int[] columnWidths = new int [columns];
				int[] rowOffsets = new int [rows];
				int[] rowHeights = new int [rows];
				int x, y;
				Control child;
				int posnLower, posnUpper;
				Size childSize;

				// Compute the offset and width of all columns.
				posnLower = margin;
				posnUpper = ClientSize.Width - margin;
				for(x = 0; x < stretchColumn; ++x)
				{
					columnOffsets[x] = posnLower;
					columnWidths[x] = 0;
					for(y = 0; y < rows; ++y)
					{
						child = GetControl(x, y);
						if(child != null && child.visible)
						{
							childSize = HBoxLayout.GetRecommendedSize(child);
							if(childSize.Width > columnWidths[x])
							{
								columnWidths[x] = childSize.Width;
							}
						}
					}
					posnLower += columnWidths[x] + colSpacing;
				}
				for(x = columns - 1; x > stretchColumn; --x)
				{
					columnWidths[x] = 0;
					for(y = 0; y < rows; ++y)
					{
						child = GetControl(x, y);
						if(child != null && child.visible)
						{
							childSize = HBoxLayout.GetRecommendedSize(child);
							if(childSize.Width > columnWidths[x])
							{
								columnWidths[x] = childSize.Width;
							}
						}
					}
					posnUpper -= columnWidths[x];
					columnOffsets[x] = posnUpper;
					posnUpper -= colSpacing;
				}
				columnOffsets[stretchColumn] = posnLower;
				columnWidths[stretchColumn] = posnUpper - posnLower;

				// Compute the offset and height of all rows.
				posnLower = margin;
				posnUpper = ClientSize.Height - margin;
				for(y = 0; y < stretchRow; ++y)
				{
					rowOffsets[y] = posnLower;
					rowHeights[y] = 0;
					for(x = 0; x < columns; ++x)
					{
						child = GetControl(x, y);
						if(child != null && child.visible)
						{
							childSize = HBoxLayout.GetRecommendedSize(child);
							if(childSize.Height > rowHeights[y])
							{
								rowHeights[y] = childSize.Height;
							}
						}
					}
					posnLower += rowHeights[y] + rowSpacing;
				}
				for(y = rows - 1; y > stretchRow; --y)
				{
					rowHeights[y] = 0;
					for(x = 0; x < columns; ++x)
					{
						child = GetControl(x, y);
						if(child != null && child.visible)
						{
							childSize = HBoxLayout.GetRecommendedSize(child);
							if(childSize.Height > rowHeights[y])
							{
								rowHeights[y] = childSize.Height;
							}
						}
					}
					posnUpper -= rowHeights[y];
					rowOffsets[y] = posnUpper;
					posnUpper -= rowSpacing;
				}
				rowOffsets[stretchRow] = posnLower;
				rowHeights[stretchRow] = posnUpper - posnLower;

				// Place the controls in their final locations.
				for(y = 0; y < rows; ++y)
				{
					for(x = 0; x < columns; ++x)
					{
						child = GetControl(x, y);
						if(child != null && child.visible)
						{
							child.SetBounds
								(columnOffsets[x], rowOffsets[y],
								 columnWidths[x], rowHeights[y]);
						}
					}
				}
			}

}; // class GridLayout

}; // namespace System.Windows.Forms
