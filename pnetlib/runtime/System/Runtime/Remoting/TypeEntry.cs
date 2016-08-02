/*
 * TypeEntry.cs - Implementation of the
 *			"System.Runtime.Remoting.TypeEntry" class.
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

namespace System.Runtime.Remoting
{

#if CONFIG_REMOTING

public class TypeEntry
{
	// Internal state.
	internal Type actualType;
	private String assemblyName;
	private String typeName;

	// Constructor.
	protected TypeEntry() {}

	// Get or set the assembly name for this type entry.
	public String AssemblyName
			{
				get
				{
					return assemblyName;
				}
				set
				{
					assemblyName = value;
				}
			}

	// Get or set the type name for this type entry.
	public String TypeName
			{
				get
				{
					return typeName;
				}
				set
				{
					typeName = value;
				}
			}

	// Get the object type.
	internal Type GetObjectType()
			{
				if(actualType != null)
				{
					return actualType;
				}
				else
				{
					return RemotingConfiguration.GetType
						(TypeName, AssemblyName);
				}
			}

}; // class TypeEntry

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting
