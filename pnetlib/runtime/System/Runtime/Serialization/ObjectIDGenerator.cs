/*
 * ObjectIDGenerator.cs - Implementation of the
 *			"System.Runtime.Serialization.ObjectIDGenerator" class.
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
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

using System.Collections;

public class ObjectIDGenerator
{
	// Internal state.
	private Hashtable table;
	private Hashtable typeTable;
	private long nextId;

	// Constructor.
	public ObjectIDGenerator()
			{
				table = new IdentityHashtable();
				typeTable = new IdentityHashtable();
				nextId = 1;
			}

	// Get an identifier for an object.
	public virtual long GetId(Object obj, out bool firstTime)
			{
				if(obj == null)
				{
					throw new ArgumentNullException("obj");
				}
				Object id = table[obj];
				if(id != null)
				{
					firstTime = false;
					return (long)id;
				}
				else
				{
					firstTime = true;
					table[obj] = (Object)(nextId);
					return nextId++;
				}
			}

	// Determine if an object already has an identifier.
	public virtual long HasId(Object obj, out bool firstTime)
			{
				if(obj == null)
				{
					throw new ArgumentNullException("obj");
				}
				Object id = table[obj];
				if(id != null)
				{
					firstTime = false;
					return (long)id;
				}
				else
				{
					firstTime = true;
					return 0;
				}
			}

	// Register a type with a previous object identifier that used the type.
	internal void RegisterType(Type type, long id)
			{
				if(typeTable[type] == null)
				{
					typeTable[type] = (Object)id;
				}
			}

	// Get the object identity of an object of a specific type.
	// This is used to cache type structure information between objects.
	// Returns -1 if there was no previous object with the given type.
	internal long GetIDForType(Type type)
			{
				Object obj = typeTable[type];
				if(obj != null)
				{
					return (long)obj;
				}
				else
				{
					return -1;
				}
			}

	// Modified hash table class that uses object identity for the key.
	// This way, it is possible for two objects that compare as "Equal"
	// to be present in the hash table under separate identifiers.
	private sealed class IdentityHashtable : Hashtable
	{
		// Constructor.
		public IdentityHashtable() : base(128) {} // avoid expansation of hashtable

		// Determine if an item is equal to a key value.
		protected override bool KeyEquals(Object item, Object key)
				{
					if(item is String)
					{
						// Strings with the same value compare as equal.
						return item.Equals(key);
					}
					else
					{
						// Everything else needs object identity.
						return (item == key);
					}
				}

	}; // class IdentityHashtable

}; // class ObjectIDGenerator

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Serialization
