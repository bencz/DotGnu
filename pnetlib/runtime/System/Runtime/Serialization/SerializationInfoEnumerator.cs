/*
 * SerializationInfoEnumerator.cs - Implementation of the
 *			"System.Runtime.Serialization.SerializationInfoEnumerator" class.
 *
 * Copyright (C) 2001, 2002  Southern Storm Software, Pty Ltd.
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

public sealed class SerializationInfoEnumerator : IEnumerator
{
	// Internal state.
	private SerializationInfo info;
	private int generation;
	private int posn;

	// Constructor.
	internal SerializationInfoEnumerator(SerializationInfo info)
			{
				this.info = info;
				this.generation = info.generation;
				this.posn = -1;
			}

	// Implement the IEnumerator interface.
	public bool MoveNext()
			{
				if(generation != info.generation)
				{
					throw new InvalidOperationException
						(_("Invalid_CollectionModified"));
				}
				++posn;
				return (posn < info.names.Count);
			}
	public void Reset()
			{
				posn = -1;
			}
	Object IEnumerator.Current
			{
				get
				{
					return (Object)Current;
				}
			}

	// Get the current serialization entry information.
	public SerializationEntry Current
			{
				get
				{
					if(generation != info.generation)
					{
						throw new InvalidOperationException
							(_("Invalid_CollectionModified"));
					}
					else if(posn < 0 || posn >= info.names.Count)
					{
						throw new InvalidOperationException
							(_("Invalid_BadEnumeratorPosition"));
					}
					return new SerializationEntry
					  	((String)(info.names[posn]),
						 (Type)(info.types[posn]),
						 info.values[posn]);
				}
			}

	// Get the name associated with the current entry.
	public String Name
			{
				get
				{
					if(generation != info.generation)
					{
						throw new InvalidOperationException
							(_("Invalid_CollectionModified"));
					}
					else if(posn < 0 || posn >= info.names.Count)
					{
						throw new InvalidOperationException
							(_("Invalid_BadEnumeratorPosition"));
					}
					return (String)(info.names[posn]);
				}
			}

	// Get the object type associated with the current entry.
	public Type ObjectType
			{
				get
				{
					if(generation != info.generation)
					{
						throw new InvalidOperationException
							(_("Invalid_CollectionModified"));
					}
					else if(posn < 0 || posn >= info.names.Count)
					{
						throw new InvalidOperationException
							(_("Invalid_BadEnumeratorPosition"));
					}
					return (Type)(info.types[posn]);
				}
			}

	// Get the value associated with the current entry.
	public Object Value
			{
				get
				{
					if(generation != info.generation)
					{
						throw new InvalidOperationException
							(_("Invalid_CollectionModified"));
					}
					else if(posn < 0 || posn >= info.names.Count)
					{
						throw new InvalidOperationException
							(_("Invalid_BadEnumeratorPosition"));
					}
					return info.values[posn];
				}
			}

}; // class SerializationInfoEnumerator

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Serialization
