/*
 * AttributeCollection.cs - Implementation of the
 *		"System.ComponentModel.ComponentModel.AttributeCollection" class.
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

namespace System.ComponentModel
{

#if CONFIG_COMPONENT_MODEL

using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

[ComVisible(true)]
public class AttributeCollection : ICollection, IEnumerable
{
	// Internal state.
	private ArrayList coll;

	// The empty attribute collection.
	public static readonly AttributeCollection Empty
			= new AttributeCollection(null);

	// Constructor.
	public AttributeCollection(Attribute[] attributes)
			{
				coll = new ArrayList();
				if(attributes != null)
				{
					coll.AddRange(attributes);
				}
			}

	// Implement the ICollection interface.
	public void CopyTo(Array array, int index)
			{
				coll.CopyTo(array, index);
			}
	int ICollection.Count
			{
				get
				{
					return coll.Count;
				}
			}
	bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}
	Object ICollection.SyncRoot
			{
				get
				{
					return this;
				}
			}

	// Implement the IEnumerable interface.
	IEnumerator IEnumerable.GetEnumerator()
			{
				return coll.GetEnumerator();
			}

	// Get the number of elements in this collection.
	public int Count
			{
				get
				{
					return coll.Count;
				}
			}

	// Get an element from this collection, by index.
	public virtual Attribute this[int index]
			{
				get
				{
					return (Attribute)(coll[index]);
				}
			}

	// Get an element from this collection, by type.
	public virtual Attribute this[Type type]
			{
				get
				{
					foreach(Attribute attr in coll)
					{
						if(attr != null && attr.GetType() == type)
						{
							return attr;
						}
					}
					return GetDefaultAttribute(type);
				}
			}

	// Determine if this collection contains a particular attribute.
	public bool Contains(Attribute attr)
			{
				return coll.Contains(attr);
			}

	// Determine if this collection contains a list of attributes.
	public bool Contains(Attribute[] attributes)
			{
				if(attributes != null)
				{
					foreach(Attribute attr in attributes)
					{
						if(!Contains(attr))
						{
							return false;
						}
					}
				}
				return true;
			}

	// Get the default attribute value of a particular type.
	protected Attribute GetDefaultAttribute(Type attributeType)
			{
				FieldInfo field = attributeType.GetField
					("Default", BindingFlags.Public | BindingFlags.Static);
				if(field != null)
				{
					return (Attribute)(field.GetValue(null));
				}
				else
				{
					Attribute attr;
					attr = (Attribute)(Activator.CreateInstance(attributeType));
					if(attr != null && attr.IsDefaultAttribute())
					{
						return attr;
					}
					else
					{
						return null;
					}
				}
			}

	// Get an enumerator for this collection.
	public IEnumerator GetEnumerator()
			{
				return coll.GetEnumerator();
			}

	// Determine if an attribute matches something in the collection.
	public bool Matches(Attribute attr)
			{
				foreach(Attribute attr2 in coll)
				{
					if(attr2 != null && attr2.Match(attr))
					{
						return true;
					}
				}
				return false;
			}

	// Determine if all attributes in a list match something.
	public bool Matches(Attribute[] attributes)
			{
				if(attributes != null)
				{
					foreach(Attribute attr in attributes)
					{
						if(!Matches(attr))
						{
							return false;
						}
					}
				}
				return true;
			}

}; // class AttributeCollection

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
