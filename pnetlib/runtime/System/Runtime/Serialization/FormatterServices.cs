/*
 * FormatterServices.cs - Implementation of the
 *			"System.Runtime.Serialization.FormatterServices" class.
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

using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters;

public sealed class FormatterServices
{
	// This class cannot be instantiated.
	private FormatterServices() {}

	// Check whether a type can be deserialized.
	public static void CheckTypeSecurity(Type t, TypeFilterLevel securityLevel)
			{
			#if CONFIG_REMOTING
				if(securityLevel == TypeFilterLevel.Low)
				{
					if(typeof(IEnvoyInfo).IsAssignableFrom(t) ||
					   typeof(ISponsor).IsAssignableFrom(t) ||
					   typeof(ObjRef).IsAssignableFrom(t))
					{
						throw new SecurityException
							(String.Format(_("Security_RemotingCheckType"), t));
					}
				}
			#endif
			}

	// Extract data from a specified object.
	[SecurityPermission(SecurityAction.LinkDemand,
						Flags=SecurityPermissionFlag.SerializationFormatter)]
	public static Object[] GetObjectData(Object obj, MemberInfo[] members)
			{
				if(obj == null)
				{
					throw new ArgumentNullException("obj");
				}
				if(members == null)
				{
					throw new ArgumentNullException("members");
				}
				Object[] data = new Object [members.Length];
				int index;
				for(index = 0; index < members.Length; ++index)
				{
					if(members[index] == null)
					{
						throw new ArgumentNullException
							("members[" + index.ToString() + "]");
					}
					if(!(members[index] is FieldInfo))
					{
						throw new SerializationException
							(_("Serialize_NotField"));
					}
					data[index] = ((FieldInfo)(members[index])).GetValue(obj);
				}
				return data;
			}

	// Get all of the serializable members for a specified type.
	[SecurityPermission(SecurityAction.LinkDemand,
						Flags=SecurityPermissionFlag.SerializationFormatter)]
	public static MemberInfo[] GetSerializableMembers(Type type)
			{
				return GetSerializableMembers
					(type, new StreamingContext(StreamingContextStates.All));
			}
	[SecurityPermission(SecurityAction.LinkDemand,
						Flags=SecurityPermissionFlag.SerializationFormatter)]
	public static MemberInfo[] GetSerializableMembers
				(Type type, StreamingContext context)
			{
				if(type == null)
				{
					throw new ArgumentNullException("type");
				}
				else if(!(type is ClrType))
				{
					return new MemberInfo [0];
				}
				else
				{
					return InternalGetSerializableMembers(type);
				}
			}

	// Internal version of "GetSerializableMembers".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static MemberInfo[] InternalGetSerializableMembers
				(Type type);

	// Get an assembly by name.
	internal static Assembly GetAssemblyByName(String assem)
			{
				try
				{
					Assembly assembly = Assembly.Load(assem);
					return assembly;
				}
				catch(Exception)
				{
					return null;
				}
			}

	// Get a named type from a specific assembly
	#if (__CSCC__)
	[SecurityPermission(SecurityAction.LinkDemand,
						Flags=SecurityPermissionFlag.SerializationFormatter)]
	[ReflectionPermission(SecurityAction.LinkDemand, TypeInformation=true)]
	#endif
	public static Type GetTypeFromAssembly(Assembly assem, String name)
			{
				if(assem == null)
				{
					throw new ArgumentNullException("assem");
				}
				return assem.GetType(name);
			}

	// Get an uninitialized instance from the runtime engine.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static Object InternalGetUninitializedObject(Type type);

	// Create a new unitialized instance of a specific object type.
	[SecurityPermission(SecurityAction.LinkDemand,
						Flags=SecurityPermissionFlag.SerializationFormatter)]
	public static Object GetUninitializedObject(Type type)
			{
				if(type == null)
				{
					throw new ArgumentNullException("type");
				}
				else if(!(type is ClrType))
				{
					throw new SerializationException
						(_("Serialize_NotClrType"));
				}
				return InternalGetUninitializedObject(type);
			}
	[SecurityPermission(SecurityAction.LinkDemand,
						Flags=SecurityPermissionFlag.SerializationFormatter)]
	public static Object GetSafeUninitializedObject(Type type)
			{
				return GetUninitializedObject(type);
			}

	// Populate the members of a specific object with given data values.
	[SecurityPermission(SecurityAction.LinkDemand,
						Flags=SecurityPermissionFlag.SerializationFormatter)]
	public static Object PopulateObjectMembers
				(Object obj, MemberInfo[] members, Object[] data)
			{
				// Validate the parameters.
				if(obj == null)
				{
					throw new ArgumentNullException("obj");
				}
				else if(members == null)
				{
					throw new ArgumentNullException("members");
				}
				else if(data == null)
				{
					throw new ArgumentNullException("data");
				}
				else if(members.Length != data.Length)
				{
					throw new ArgumentException
						(_("Serialize_MemberDataMismatch"));
				}

				// Set the member values.
				int index;
				for(index = 0; index < members.Length; ++index)
				{
					if(members[index] == null)
					{
						throw new ArgumentNullException
							("members[" + index.ToString() + "]");
					}
					if(data[index] == null)
					{
						// Skip entries with null values.
						continue;
					}
					if(!(members[index] is FieldInfo))
					{
						throw new SerializationException
							(_("Serialize_NotField"));
					}
					((FieldInfo)(members[index])).SetValue(obj, data[index]);
				}
				return obj;
			}

}; // class FormatterServices

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Serialization
