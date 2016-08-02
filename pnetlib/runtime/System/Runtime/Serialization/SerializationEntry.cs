/*
 * SerializationEntry.cs - Implementation of the
 *			"System.Runtime.Serialization.SerializationEntry" structure.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System.Runtime.Serialization
{

#if CONFIG_SERIALIZATION

public struct SerializationEntry
{
	// Internal state.
	private String name;
	private Type type;
	private Object value;

	// Constructor.
	internal SerializationEntry(String name, Type type, Object value)
			{
				this.name = name;
				this.type = type;
				this.value = value;
			}

	// Get the name of the object.
	public String Name
			{
				get
				{
					return name;
				}
			}

	// Get the type of the object.
	public Type ObjectType
			{
				get
				{
					return type;
				}
			}

	// Get the value contained in the object.
	public Object Value
			{
				get
				{
					return value;
				}
			}

}; // struct SerializationEntry

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Serialization
