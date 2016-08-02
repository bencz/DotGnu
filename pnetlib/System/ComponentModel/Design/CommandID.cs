/*
 * CommandID.cs - Implementation of the
 *		"System.ComponentModel.Design.CommandID" class.
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

namespace System.ComponentModel.Design
{

#if CONFIG_COMPONENT_MODEL_DESIGN

using System.Runtime.InteropServices;

[ComVisible(true)]
public class CommandID
{
	// Internal state.
	private Guid menuGroup;
	private int commandID;

	// Constructor.
	public CommandID(Guid menuGroup, int commandID)
			{
				this.menuGroup = menuGroup;
				this.commandID = commandID;
			}

	// Get the command identifier's properties.
	public virtual Guid Guid
			{
				get
				{
					return menuGroup;
				}
			}
	public virtual int ID
			{
				get
				{
					return commandID;
				}
			}

	// Determine if two objects are equal.
	public override bool Equals(Object obj)
			{
				CommandID other = (obj as CommandID);
				if(other != null)
				{
					return menuGroup.Equals(other.menuGroup) &&
						   commandID == other.commandID;
				}
				else
				{
					return false;
				}
			}

	// Get a hash code for this object.
	public override int GetHashCode()
			{
				return commandID;
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return menuGroup.ToString() + " : " + commandID.ToString();
			}

}; // class CommandID

#endif // CONFIG_COMPONENT_MODEL_DESIGN

}; // namespace System.ComponentModel.Design
