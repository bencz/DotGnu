/*
 * PaperSize.cs - Implementation of the
 *			"System.Drawing.Printing.PaperSize" class.
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

using System.Text;

public class PaperSize
{
	// Internal state.
	private PaperKind kind;
	private String name;
	private int width;
	private int height;

	// Constructors.
	public PaperSize(String name, int width, int height)
			{
				this.kind = PaperKind.Custom;
				this.name = name;
				this.width = width;
				this.height = height;
			}
	internal PaperSize(PaperKind kind)
			{
				this.kind = kind;
				this.name = kind.ToString();
				// TODO: need to add all of the rest of the paper sizes.
				switch(kind)
				{
					case PaperKind.Letter:
					{
						this.width = 850;
						this.height = 1100;
					}
					break;

					case PaperKind.A4:
					{
						this.width = 857;
						this.height = 1212;
					}
					break;

					case PaperKind.Executive:
					{
						this.width = 725;
						this.height = 1050;
					}
					break;

					case PaperKind.Legal:
					{
						this.width = 850;
						this.height = 1400;
					}
					break;

					default:
					{
						// Unknown paper size, so switch to "Letter".
						this.kind = PaperKind.Letter;
						this.width = 850;
						this.height = 1100;
					}
					break;
				}
			}

	// Get or set this object's properties.
	public int Height
			{
				get
				{
					return height;
				}
				set
				{
					if(kind != PaperKind.Custom)
					{
						throw new ArgumentException
							(S._("Arg_PaperSizeNotCustom"));
					}
					height = value;
				}
			}
	public PaperKind Kind
			{
				get
				{
					return kind;
				}
			}
	public String PaperName
			{
				get
				{
					return name;
				}
				set
				{
					if(kind != PaperKind.Custom)
					{
						throw new ArgumentException
							(S._("Arg_PaperSizeNotCustom"));
					}
					name = value;
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
					if(kind != PaperKind.Custom)
					{
						throw new ArgumentException
							(S._("Arg_PaperSizeNotCustom"));
					}
					width = value;
				}
			}

	// Convert this object into a string.
	public override String ToString()
			{
				StringBuilder builder = new StringBuilder();
				builder.Append("[PaperSize ");
				builder.Append(name);
				builder.Append(" Kind=");
				builder.Append(kind.ToString());
				builder.Append(" Height=");
				builder.Append(height.ToString());
				builder.Append(" Width=");
				builder.Append(width.ToString());
				builder.Append(']');
				return builder.ToString();
			}

}; // class PaperSize

}; // namespace System.Drawing.Printing
