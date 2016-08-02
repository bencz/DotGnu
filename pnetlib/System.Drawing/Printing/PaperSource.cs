/*
 * PaperSource.cs - Implementation of the
 *			"System.Drawing.Printing.PaperSource" class.
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

public class PaperSource
{
	// Internal state.
	private PaperSourceKind kind;
	private String name;

	// Constructor.
	internal PaperSource(PaperSourceKind kind, String name)
			{
				this.kind = kind;
				if(name == null)
				{
					this.name = kind.ToString();
				}
				else
				{
					this.name = name;
				}
			}

	// Get the paper kind.
	public PaperSourceKind Kind
			{
				get
				{
					return kind;
				}
			}
	public String SourceName
			{
				get
				{
					return name;
				}
			}

	// Convert this object into a string.
	public override String ToString()
			{
				StringBuilder builder = new StringBuilder();
				builder.Append("[PaperSource ");
				builder.Append(name);
				builder.Append(" Kind=");
				builder.Append(kind.ToString());
				builder.Append(']');
				return builder.ToString();
			}

}; // class PaperSource

}; // namespace System.Drawing.Printing
