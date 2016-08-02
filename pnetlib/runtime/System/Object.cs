/*
 * Object.cs - Implementation of the "System.Object" class.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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

using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Resources;

#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDual)]
#endif
public class Object
{

	// Default constructor for the Object class.
	public Object() {}

	// Default definition of the object finalizer.
	protected virtual void Finalize() {}

	// Default implementation of the ToString method.
	public virtual String ToString()
			{ return GetType().ToString(); }

	// Internal methods that are provided by the runtime engine.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public Type GetType();

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public virtual int GetHashCode();

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public virtual bool Equals(Object obj);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern protected Object MemberwiseClone();

	// Compare two objects for equality.
	public static bool Equals(Object objA, Object objB)
			{
				if(objA == objB)
				{
					return true;
				}
				else if(objA != null && objB != null)
				{
					return objA.Equals(objB);
				}
				else
				{
					return false;
				}
			}

	// Determine if two object references are equal.
	public static bool ReferenceEquals(Object objA, Object objB)
			{
				return (objA == objB);
			}

#if CONFIG_RUNTIME_INFRA

	// Cached copy of the resources for this assembly.
	private static ResourceManager resources = null;
	private static bool reflectionMissing = false;

	// Helper for obtaining string resources for this assembly.
	internal static String _(String tag)
			{
				lock(typeof(Object))
				{
					if(reflectionMissing)
					{
						return tag;
					}
					else
					{
						try
						{
							if(resources == null)
							{
								resources = new ResourceManager
									("runtime",
									 Assembly.GetExecutingAssembly());
							}
							return resources.GetString(tag, null);
						}
						catch(NotImplementedException)
						{
							reflectionMissing = true;
							return tag;
						}
					}
				}
			}

#else // !CONFIG_RUNTIME_INFRA

	// We don't have sufficient runtime support for loading resources.
	internal static String _(String tag)
			{
				return tag;
			}

#endif // !CONFIG_RUNTIME_INFRA

}; // class Object

}; // namespace System
