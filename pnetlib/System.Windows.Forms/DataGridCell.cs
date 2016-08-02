/*
 * DataGridCell.cs - Implementation of the
 *		"System.Windows.Forms.DataGridCell" class.
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

public struct DataGridCell
{
	// Internal state.
	private int row;
	private int column;

	// Constructor.
	public DataGridCell(int r, int c)
			{
				row = r;
				column = c;
			}

	// Get or set this object's properties.
	public int ColumnNumber
			{
				get
				{
					return this.column;
				}
				set
				{
					this.column = value;
				}
			}
	public int RowNumber
			{
				get
				{
					return this.row;
				}
				set
				{
					this.row = value;
				}
			}

	// Determine if two objects are equal.
	public override bool Equals(Object o)
			{
				if(o is DataGridCell)
				{
					DataGridCell cell = (DataGridCell)o;
					return (column == cell.column && row == cell.row);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				return ((row << 8) ^ column);
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return String.Format
					("DataGridCell {{RowNumber={0}, ColumnNumber={1}}}",
					 row, column);
			}

}; // struct DataGridCell

}; // namespace System.Windows.Forms
