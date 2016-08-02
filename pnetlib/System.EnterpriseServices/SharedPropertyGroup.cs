/*
 * SharedPropertyGroup.cs - Implementation of the
 *			"System.EnterpriseServices.SharedPropertyGroup" class.
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

namespace System.EnterpriseServices
{

using System.Collections;
using System.Runtime.InteropServices;

#if !ECMA_COMPAT
[ComVisible(false)]
#endif
public sealed class SharedPropertyGroup
{
	// Internal state.
	private Hashtable table;

	// Constructor.
	internal SharedPropertyGroup()
			{
				table = new Hashtable();
			}

	// Create a new property within the shared group.
	public SharedProperty CreateProperty(String name, out bool fExists)
			{
				SharedProperty property;
				property = (SharedProperty)(table[name]);
				if(property != null)
				{
					fExists = true;
					return property;
				}
				property = new SharedProperty();
				table[name] = property;
				fExists = false;
				return property;
			}
	public SharedProperty CreatePropertyByPosition
				(int position, out bool fExists)
			{
				SharedProperty property;
				property = (SharedProperty)(table[position]);
				if(property != null)
				{
					fExists = true;
					return property;
				}
				property = new SharedProperty();
				table[position] = property;
				fExists = false;
				return property;
			}

	// Get a property with a particular identifier.
	public SharedProperty Property(String name)
			{
				return (table[name] as SharedProperty);
			}
	public SharedProperty PropertyByPosition(int position)
			{
				return (table[position] as SharedProperty);
			}

}; // class SharedPropertyGroup

}; // namespace System.EnterpriseServices
