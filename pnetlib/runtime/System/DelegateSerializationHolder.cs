/*
 * DelegateSerializationHolder.cs - Implementation of the
 *			"System.DelegateSerializationHolder" class.
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

namespace System
{

#if CONFIG_SERIALIZATION

using System.Runtime.Serialization;
using System.Reflection;
using System.Private;

// This class is used as a proxy to stand in for delegate objects.
// This class must be named "System.DelegateSerializationHolder" for
// compatibility with other serialization implementations.

internal class DelegateSerializationHolder : ISerializable, IObjectReference
{
	// Entry class that is used to hold the information about a delegate.
	[Serializable]
	internal class DelegateEntry
	{
		public String type;
		public String assembly;
		public Object target;
		public String targetTypeAssembly;
		public String targetTypeName;
		public String methodName;
		public DelegateEntry delegateEntry;

		// Convert a delegate serialization entry into a delegate.
		public Delegate ToDelegate()
				{
					Assembly assem;
					Type delegateType;
					Type targetType;

					// Validate the entry state.
					if(methodName == null || methodName.Length == 0)
					{
						throw new SerializationException
							(_("Serialize_StateMissing"));
					}

					// Fetch the delegate type.
					assem = FormatterServices.GetAssemblyByName(assembly);
					if(assem == null)
					{
						throw new SerializationException
							(_("Serialize_UnknownAssembly"));
					}
					delegateType = assem.GetType(type);
					if(delegateType == null)
					{
						throw new SerializationException
							(_("Serialize_StateMissing"));
					}

					// Fetch the target type.
					assem = FormatterServices.GetAssemblyByName
						(targetTypeAssembly);
					if(assem == null)
					{
						throw new SerializationException
							(_("Serialize_UnknownAssembly"));
					}
					targetType = assem.GetType(targetTypeName);
					if(targetType == null)
					{
						throw new SerializationException
							(_("Serialize_StateMissing"));
					}

					// Check that the target is an instance of
					// the specified target type.
					if(target != null)
					{
						if(!targetType.IsInstanceOfType(target))
						{
							throw new SerializationException
								(_("Serialize_DelegateTargetMismatch"));
						}
					}

					// Create the Delegate.
					Delegate del;
					if(target != null)
					{
						del = Delegate.CreateDelegate
							(delegateType, target, methodName);
					}
					else
					{
						del = Delegate.CreateDelegate
							(delegateType, targetType, methodName);
					}

					// Fail if the method is non-public.
					MethodInfo method = del.Method;
					if(method != null && !(method.IsPublic))
					{
						throw new SerializationException
							(_("Serialize_DelegateNotPublic"));
					}
					return del;
				}

	}; // class DelegateEntry

	// Internal state.
	private DelegateEntry entry;

	// Constructor.
	public DelegateSerializationHolder(SerializationInfo info,
									   StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				bool needsResolve = false;
				try
				{
					// Try the new serialization format first.
					entry = (DelegateEntry)(info.GetValue
						("Delegate", typeof(DelegateEntry)));
					needsResolve = true;
				}
				catch(Exception)
				{
					// Try the old-style serialization format (deprecated).
					entry = new DelegateEntry();
					entry.type = info.GetString("DelegateType");
					entry.assembly = info.GetString("DelegateAssembly");
					entry.target = info.GetValue("Target", typeof(Object));
					entry.targetTypeAssembly =
						info.GetString("TargetTypeAssembly");
					entry.targetTypeName = info.GetString("TargetTypeName");
					entry.methodName = info.GetString("MethodName");
				}

				// Resolve targets specified as field names.
				if(needsResolve)
				{
					DelegateEntry e = entry;
					while(e != null)
					{
						if(e.target is String)
						{
							e.target = info.GetValue
								((String)(e.target), typeof(Object));
						}
						e = e.delegateEntry;
					}
				}
			}

	// Serialize a delegate object.
	internal static DelegateEntry Serialize
				(SerializationInfo info, int listPosition,
				 Type delegateType, Object target, MethodBase method)
			{
				// Create the new delegate entry block.
				DelegateEntry entry = new DelegateEntry();
				entry.type = delegateType.FullName;
				entry.assembly = delegateType.Assembly.FullName;
				entry.target = target;
				entry.targetTypeAssembly =
						method.ReflectedType.Assembly.FullName;
				entry.targetTypeName = method.ReflectedType.FullName;
				entry.methodName = method.Name;

				// Add the block if this is the first in the list.
				if(info.MemberCount == 0)
				{
					info.SetType(typeof(DelegateSerializationHolder));
					info.AddValue("Delegate", entry, typeof(DelegateEntry));
				}

				// Add the target object to the top level of the info block.
				// Needed to get around order of fixup problems in some
				// third party serialization implementations.
				if(target != null)
				{
					String name = "target" + listPosition.ToString();
					info.AddValue(name, target, typeof(Object));
					entry.target = name;
				}

				// Return the entry to the caller so that we can chain
				// multiple entries together for multicast delegates.
				return entry;
			}

	// Serialize a multicast delegate object.
	internal static void SerializeMulticast
				(SerializationInfo info, MulticastDelegate del)
			{
				DelegateEntry entry, next;
				int index = 0;

				// Serialize the first entry on the multicast list.
				entry = Serialize(info, index++, del.GetType(), del.target,
								  MethodBase.GetMethodFromHandle(del.method));

				// Serialize the rest of the multicast chain.
				del = del.prev;
				while(del != null)
				{
					next = Serialize
						(info, index++, del.GetType(), del.target,
						 MethodBase.GetMethodFromHandle(del.method));
					entry.delegateEntry = next;
					entry = next;
					del = del.prev;
				}
			}

	// Implement the ISerializable interface.
	public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				// This method should never be called.
				throw new NotSupportedException();
			}

	// Implement the IObjectReference interface.
	public Object GetRealObject(StreamingContext context)
			{
				// Get the first delegate in the chain.
				Delegate del = entry.ToDelegate();

				// Bail out early if this is a unicast delegate.
				if(!(del is MulticastDelegate) || entry.delegateEntry == null)
				{
					return del;
				}

				// Build the multicast delegate chain.
				Delegate end = del;
				Delegate newDelegate;
				DelegateEntry e = entry.delegateEntry;
				while(e != null)
				{
					newDelegate = e.ToDelegate();
					if(newDelegate.GetType() != del.GetType())
					{
						throw new SerializationException
							(_("Arg_DelegateMismatch"));
					}
					((MulticastDelegate)end).prev =
						(newDelegate as MulticastDelegate);
					end = newDelegate;
					e = e.delegateEntry;
				}
				return del;
			}

}; // class DelegateSerializationHolder

#endif // CONFIG_SERIALIZATION

}; // namespace System
