/*
 * TabCompleteEventArgs.cs - Implementation of the
 *			"DotGNU.Terminal.TabCompleteEventArgs" class.
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

namespace DotGNU.Terminal
{

using System;

public class TabCompleteEventArgs : EventArgs
{
	// Internal state.
	private String prefix;
	private String suffix;
	private String insert;
	private String[] alternatives;

	// Constructor.
	public TabCompleteEventArgs(String prefix, String suffix)
			{
				this.prefix = prefix;
				this.suffix = suffix;
				this.insert = null;
				this.alternatives = null;
			}

	// Get the line prefix (i.e. all characters before the current position).
	public String Prefix
			{
				get
				{
					return prefix;
				}
			}

	// Get the line suffix (i.e. all characters after the current position).
	public String Suffix
			{
				get
				{
					return suffix;
				}
			}

	// Get or set the extra string to be inserted into the line.
	public String Insert
			{
				get
				{
					return insert;
				}
				set
				{
					insert = value;
				}
			}

	// Get or set the list of strings to be displayed as alternatives.
	public String[] Alternatives
			{
				get
				{
					return alternatives;
				}
				set
				{
					alternatives = value;
				}
			}

}; // class TabCompleteEventArgs

}; // namespace DotGNU.Terminal
