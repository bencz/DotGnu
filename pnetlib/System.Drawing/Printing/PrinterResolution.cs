/*
 * PrinterResolution.cs - Implementation of the
 *			"System.Drawing.Printing.PrinterResolution" class.
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

namespace System.Drawing.Printing
{

public class PrinterResolution
{
	// Internal state.
	private PrinterResolutionKind kind;
	private int x;
	private int y;

	// Constructors.
	internal PrinterResolution(PrinterResolutionKind kind, int x, int y)
			{
				this.kind = kind;
				this.x = x;
				this.y = y;
			}

	// Get this object's properties.
	public PrinterResolutionKind Kind
			{
				get
				{
					return kind;
				}
			}
	public int X
			{
				get
				{
					return x;
				}
			}
	public int Y
			{
				get
				{
					return y;
				}
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return "[PrinterResolution " + kind.ToString() + "]";
			}

}; // class PrinterResolution

}; // namespace System.Drawing.Printing
