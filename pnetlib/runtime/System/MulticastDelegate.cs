/*
 * MulticastDelegate.cs - Implementation of "System.MulticastDelegate".
 *
 * Copyright (C) 2001, 2002, 2003  Southern Storm Software, Pty Ltd.
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

namespace System
{

// Note: this class is not defined by the ECMA class library
// specification, but it is required by the metadata specification.

using System;
using System.Reflection;
using System.Runtime.Serialization;

public abstract class MulticastDelegate : Delegate
{
	// Previous delegate on the same invocation list.
	internal MulticastDelegate prev;

#if CONFIG_REFLECTION
	// Constructors.
	protected MulticastDelegate(Object target, String method)
			: base(target, method)
			{
				prev = null;
			}
	protected MulticastDelegate(Type target, String method)
			: base(target, method)
			{
				prev = null;
			}
#else // !CONFIG_REFLECTION
	protected MulticastDelegate() {}
#endif // !CONFIG_REFLECTION

	// Determine if two multicast delegates are equal.
	public override bool Equals(Object obj)
			{
				if(!base.Equals(obj))
				{
					return false;
				}
				else if(((Object)prev) != null)
				{
					return prev.Equals(((MulticastDelegate)obj).prev);
				}
				else
				{
					return ReferenceEquals(((MulticastDelegate)obj).prev, null);
				}
			}

	// Get a hash code for a multicast delegate.
	public override int GetHashCode()
			{
				// Use the hash code of the last delegate on the list.
				return base.GetHashCode();
			}

	// Get the invocation list of the current delegate.
	public override Delegate[] GetInvocationList()
			{
				int len = GetLength(this);
				Delegate[] list = new Delegate [len];
				int posn = len - 1;
				MulticastDelegate current = this;
				MulticastDelegate clone;
				while(((Object)current) != null)
				{
					clone = (MulticastDelegate)(current.MemberwiseClone());
					clone.prev = null;
					list[posn--] = clone;
					current = current.prev;
				}
				return list;
			}

	// Operators.
	public static bool operator==(MulticastDelegate d1, MulticastDelegate d2)
			{
				if(((Object)d1) == null)
				{
					return (((Object)d2) == null);
				}
				else
				{
					return d1.Equals(d2);
				}
			}
	public static bool operator!=(MulticastDelegate d1, MulticastDelegate d2)
			{
				if(((Object)d1) == null)
				{
					return (((Object)d2) != null);
				}
				else
				{
					return !(d1.Equals(d2));
				}
			}

	// Implementation of delegate combination.
	protected override Delegate CombineImpl(Delegate d)
			{
				MulticastDelegate list;
				MulticastDelegate current;
				list = (MulticastDelegate)
							(((MulticastDelegate)d).MemberwiseClone());
				current = list;
				while(((Object)(current.prev)) != null)
				{
					current.prev = (MulticastDelegate)
						(current.prev.MemberwiseClone());
					current = current.prev;
				}
				current.prev = this;
				return list;
			}

#if CONFIG_REFLECTION
	// Implementation of dynamic invocation.
	protected override Object DynamicInvokeImpl(Object[] args)
			{
				if(((Object)prev) != null)
				{
					prev.DynamicInvokeImpl(args);
				}
				return base.DynamicInvokeImpl(args);
			}
#endif

	// Determine if we have an invocation list match, where the
	// invocation members of d appear in the list starting at
	// the current position.
	private static bool ListMatch(MulticastDelegate list,
								  MulticastDelegate d)
			{
				while(list.target == d.target &&
					  list.method.Equals(d.method))
				{
					list = list.prev;
					d = d.prev;
					if(((Object)d) == null)
					{
						return true;
					}
					if(((Object)list) == null)
					{
						return false;
					}
				}
				return false;
			}

	// Get the length of a multicast delegate invocation list.
	private static int GetLength(MulticastDelegate d)
			{
				int len = 0;
				while(((Object)d) != null)
				{
					++len;
					d = d.prev;
				}
				return len;
			}

	// Skip past "len" items in a multicast delegate list.
	private static MulticastDelegate Skip(MulticastDelegate d, int len)
			{
				while(len > 0)
				{
					d = d.prev;
					--len;
				}
				return d;
			}

	// Implementation of delegate removal.
	protected override Delegate RemoveImpl(Delegate d)
			{
				MulticastDelegate current, list;
				MulticastDelegate dmulti = (MulticastDelegate)d;
				int len;

				// See if the delegate is actually on the list.
				// If not, then return the list as-is.
				current = this;
				while(((Object)current) != null)
				{
					if(ListMatch(current, dmulti))
					{
						break;
					}
					current = current.prev;
				}
				if(((Object)current) == null)
				{
					return this;
				}

				// Get the length of the invocation list to be removed.
				len = GetLength(dmulti);

				// If the delegate is the first on the list, then
				// find the tail and return that.
				if(ListMatch(this, dmulti))
				{
					return Skip(this, len);
				}

				// Clone a new list with the first instance of the
				// delegate removed from it.
				list = (MulticastDelegate)(MemberwiseClone());
				current = list;
				while(((Object)(current.prev)) != null)
				{
					if(ListMatch(current.prev, dmulti))
					{
						current.prev = Skip(current.prev, len);
						break;
					}
					current.prev = (MulticastDelegate)
							(current.prev.MemberwiseClone());
					current = current.prev;
				}
				return list;
			}

	// Implementation of delegate "all" removal.
	internal override Delegate RemoveAllImpl(Delegate d)
			{
				MulticastDelegate current, list;
				MulticastDelegate dmulti = (MulticastDelegate)d;
				int len;

				// See if the delegate is actually on the list.
				// If not, then return the list as-is.
				current = this;
				while(((Object)current) != null)
				{
					if(ListMatch(current, dmulti))
					{
						break;
					}
					current = current.prev;
				}
				if(((Object)current) == null)
				{
					return this;
				}

				// Get the length of the invocation list to be removed.
				len = GetLength(dmulti);

				// Strip the delegate from the front of the list as
				// many times as is necessary to get to a non-copy.
				list = this;
				while(((Object)list) != null &&
				      ListMatch(list, dmulti))
				{
					return Skip(list, len);
				}
				if(((Object)list) == null)
				{
					return null;
				}

				// Clone a new list with all instances of the
				// delegate removed from it.
				list = (MulticastDelegate)(list.MemberwiseClone());
				current = list;
				while(((Object)(current.prev)) != null)
				{
					if(ListMatch(current.prev, dmulti))
					{
						current.prev = Skip(current.prev, len);
					}
					else
					{
						current.prev = (MulticastDelegate)
								(current.prev.MemberwiseClone());
						current = current.prev;
					}
				}
				return list;
			}

#if CONFIG_SERIALIZATION

	// Get serialization data for this delegate.
	public override void GetObjectData
				(SerializationInfo info, StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				DelegateSerializationHolder.SerializeMulticast(info, this);
			}

#endif // CONFIG_SERIALIZATION

}; // class MulticastDelegate

}; // namespace System
