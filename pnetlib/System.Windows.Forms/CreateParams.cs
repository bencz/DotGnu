/*
 * CreateParams.cs - Implementation of the
 *			"System.Windows.Forms.CreateParams" class.
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

public class CreateParams
{
	// Internal state.
	private String caption;
	private String className;
	private int classStyle;
	private int exStyle;
	private int height;
	private Object param;
	private IntPtr parent;
	private int style;
	private int width;
	private int x;
	private int y;

	// Constructor.
	public CreateParams() {}

	// Get or set this object's properties.
	public String Caption
			{
				get
				{
					return caption;
				}
				set
				{
					caption = value;
				}
			}
	public String ClassName
			{
				get
				{
					return className;
				}
				set
				{
					className = value;
				}
			}
	public int ClassStyle
			{
				get
				{
					return classStyle;
				}
				set
				{
					classStyle = value;
				}
			}
	public int ExStyle
			{
				get
				{
					return exStyle;
				}
				set
				{
					exStyle = value;
				}
			}
	public int Height
			{
				get
				{
					return height;
				}
				set
				{
					height = value;
				}
			}
	public Object Param
			{
				get
				{
					return param;
				}
				set
				{
					param = value;
				}
			}
	public IntPtr Parent
			{
				get
				{
					return parent;
				}
				set
				{
					parent = value;
				}
			}
	public int Style
			{
				get
				{
					return style;
				}
				set
				{
					style = value;
				}
			}
	public int Width
			{
				get
				{
					return width;
				}
				set
				{
					width = value;
				}
			}
	public int X
			{
				get
				{
					return x;
				}
				set
				{
					x = value;
				}
			}
	public int Y
			{
				get
				{
					return y;
				}
				set
				{
					y = value;
				}
			}

	public override String ToString()
			{
				return "CreateParams {'" + ClassName + 
					"', '" + Caption + "', 0x" + 
					Style.ToString() + ", 0x" +
			       	ExStyle.ToString() + 
					", {" + X.ToString() + 
					", " + Y.ToString() + 
					", " + Width.ToString() + 
					", " + Height.ToString() + "}}";
			}
}; // class CreateParams

}; // namespace System.Windows.Forms
