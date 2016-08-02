/*
 * MemberInfoSerializationHolder.cs - Implementation of the
 *			"System.MemberInfoSerializationHolder" class.
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

namespace System.Reflection
{

#if CONFIG_SERIALIZATION

using System.Runtime.Serialization;
using System.Reflection;
using System.Private;

// This class is used as a proxy to stand in type members such as
// fields, methods, properties, etc.  This class must be named
// "System.Reflection.MemberInfoSerializationHolder" for compatibility
// with other serialization implementations.

internal class MemberInfoSerializationHolder : ISerializable, IObjectReference
{
	// Internal state.
	private MemberTypes memberType;
	private String name;
	private String signature;
	private Type containingType;

	// Constructor.
	public MemberInfoSerializationHolder(SerializationInfo info,
									StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				memberType = (MemberTypes)(info.GetInt32("MemberType"));
				name = info.GetString("Name");
				signature = info.GetString("Signature");
				String assemblyName = info.GetString("AssemblyName");
				String className = info.GetString("ClassName");
				if(assemblyName == null || className == null)
				{
					throw new SerializationException
						(_("Serialize_StateMissing"));
				}
				Assembly assembly = FormatterServices.GetAssemblyByName
					(assemblyName);
				if(assembly == null)
				{
					throw new SerializationException
						(_("Serialize_StateMissing"));
				}
				containingType = FormatterServices.GetTypeFromAssembly
					(assembly, className);
			}

	// Serialize a unity object.
	public static void Serialize(SerializationInfo info,
								 MemberTypes memberType,
								 String name, String signature,
								 Type containingType)
			{
				info.SetType(typeof(MemberInfoSerializationHolder));
				info.AddValue("Name", name);
				info.AddValue("AssemblyName", containingType.Assembly.FullName);
				info.AddValue("ClassName", containingType.FullName);
				info.AddValue("Signature", signature);
				info.AddValue("MemberType", (int)memberType);
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
				if(containingType == null || name == null ||
				   memberType == (MemberTypes)0)
				{
					throw new SerializationException
						(_("Serialize_StateMissing"));
				}
				switch(memberType)
				{
					case MemberTypes.Constructor:
					{
						if(signature == null)
						{
							throw new SerializationException
								(_("Serialize_StateMissing"));
						}
						ConstructorInfo[] ctors;
						ctors = containingType.GetConstructors
							(BindingFlags.Instance |
							 BindingFlags.Static |
							 BindingFlags.Public |
							 BindingFlags.NonPublic);
						foreach(ConstructorInfo ctor in ctors)
						{
							if(ctor.ToString() == signature)
							{
								return ctor;
							}
						}
					}
					break;

					case MemberTypes.Event:
					{
						EventInfo eventInfo = containingType.GetEvent
							(name, BindingFlags.Instance |
								   BindingFlags.Static |
								   BindingFlags.Public |
								   BindingFlags.NonPublic);
						if(eventInfo != null)
						{
							return eventInfo;
						}
					}
					break;

					case MemberTypes.Field:
					{
						FieldInfo field = containingType.GetField
							(name, BindingFlags.Instance |
								   BindingFlags.Static |
								   BindingFlags.Public |
								   BindingFlags.NonPublic);
						if(field != null)
						{
							return field;
						}
					}
					break;

					case MemberTypes.Method:
					{
						if(signature == null)
						{
							throw new SerializationException
								(_("Serialize_StateMissing"));
						}
						MethodInfo[] methods;
						methods = containingType.GetMethods
							(BindingFlags.Instance |
							 BindingFlags.Static |
							 BindingFlags.Public |
							 BindingFlags.NonPublic);
						foreach(MethodInfo method in methods)
						{
							if(method.ToString() == signature)
							{
								return method;
							}
						}
					}
					break;

					case MemberTypes.Property:
					{
						PropertyInfo property = containingType.GetProperty
							(name, BindingFlags.Instance |
								   BindingFlags.Static |
								   BindingFlags.Public |
								   BindingFlags.NonPublic);
						if(property != null)
						{
							return property;
						}
					}
					break;

					default:
					{
						throw new ArgumentException
							(_("Arg_InvalidMemberObject"));
					}
					// Not reached.
				}
				throw new SerializationException
					(_("Arg_InvalidMemberObject"));
			}

}; // class MemberInfoSerializationHolder

#endif // CONFIG_SERIALIZATION

}; // namespace System.Reflection
