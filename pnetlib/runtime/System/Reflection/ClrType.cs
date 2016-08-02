/*
 * ClrType.cs - Implementation of the "System.Reflection.ClrType" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

using System;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

internal class ClrType : Type, ICloneable, IClrProgramItem
#if CONFIG_SERIALIZATION
	, ISerializable
#endif
{
	// Internal state.
	internal IntPtr privateData;

#if CONFIG_REFLECTION
	private static IComparer memberNameComparer =
			new MemberNameComparer();

	// Comparer class for member names.
	private sealed class MemberNameComparer : IComparer
	{
		public int Compare(Object x, Object y)
				{
					return String.CompareOrdinal
						(((MemberInfo)x).Name, ((MemberInfo)y).Name);
				}

	}; // class MemberNameComparer
#endif

	// Constructor.  This class is normally instantiated
	// by the runtime engine, not by the class library.
	internal ClrType() : base() { privateData = IntPtr.Zero; }

	// Implement the ICloneable interface.
	public Object Clone()
			{
				return this;
			}

	// Implement the IClrProgramItem interface.
	public IntPtr ClrHandle
			{
				get
				{
					return privateData;
				}
			}

	// Get the rank of this type, if it is an array.
	public override int GetArrayRank()
			{
				if(IsArray)
				{
					return GetClrArrayRank();
				}
				else
				{
					throw new NotSupportedException(_("NotSupp_NotArrayType"));
				}
			}
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private int GetClrArrayRank();

	// Get the attribute flags for this type.
#if CONFIG_REFLECTION
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern protected override TypeAttributes GetAttributeFlagsImpl();
#else
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal override TypeAttributes GetAttributeFlagsImpl();
#endif

	// Get the category of this type.  Array, pointer, byref, primitive, etc.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private ClrTypeCategory GetClrTypeCategory();

	// Get the element type for this type.
#if CONFIG_REFLECTION
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public override Type GetElementType();
#else
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal override Type GetElementType();
#endif

	// Get all interfaces that this type implements.
#if CONFIG_REFLECTION
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public override Type[] GetInterfaces();
#else
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal override Type[] GetInterfaces();
#endif

	// Implementation of the "IsArray" property.
	protected override bool IsArrayImpl()
			{
				return (GetClrTypeCategory() == ClrTypeCategory.Array);
			}

	// Implementation of the "IsPointer" property.
	protected override bool IsPointerImpl()
			{
				return (GetClrTypeCategory() == ClrTypeCategory.Pointer);
			}

	// Implementation of the "IsPrimitive" property.
#if CONFIG_REFLECTION
	protected override bool IsPrimitiveImpl()
#else
	internal override bool IsPrimitiveImpl()
#endif
			{
				return (GetClrTypeCategory() == ClrTypeCategory.Primitive);
			}

	// Determine if the current type is a subclass of another type.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public override bool IsSubclassOf(Type c);

	// Convert this type into a string.
	public override String ToString()
			{
				return GetClrFullName();
			}

#if CONFIG_RUNTIME_INFRA
	// Internal methods for supporting the properties.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private System.Reflection.Assembly GetClrAssembly();
#endif

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private Type GetClrBaseType();

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private String GetClrFullName();

	// Properties that are accessible even if no reflection.
	public override String AssemblyQualifiedName
			{
				get
				{
#if CONFIG_RUNTIME_INFRA
					return FullName + ", " + Assembly.ToString();
#else
					return FullName;
#endif
				}
			}
	public override Type BaseType
			{
				get
				{
					return GetClrBaseType();
				}
			}
	public override String FullName
			{
				get
				{
					return GetClrFullName();
				}
			}

#if CONFIG_RUNTIME_INFRA

	// Runtime infrastructure properties.
	public override RuntimeTypeHandle TypeHandle
			{
				get
				{
					return new RuntimeTypeHandle(privateData);
				}
			}
	public override System.Reflection.Assembly Assembly
			{
				get
				{
					return GetClrAssembly();
				}
			}

#endif // CONFIG_RUNTIME_INFRA

#if CONFIG_REFLECTION

	// Get the custom attributes for this type.
	public override Object[] GetCustomAttributes(bool inherit)
			{
				return ClrHelpers.GetCustomAttributes(this, inherit);
			}
	public override Object[] GetCustomAttributes(Type type, bool inherit)
			{
				return ClrHelpers.GetCustomAttributes(this, type, inherit);
			}

	// Determine if custom attributes are defined for this type.
	public override bool IsDefined(Type type, bool inherit)
			{
				return ClrHelpers.IsDefined(this, type, inherit);
			}

	// Get the hash code for this type.
	public override int GetHashCode()
			{
				// We convert via long to prevent IntPtr throwing
				// an overflow exception on 64-bit platforms.
				return unchecked((int)(privateData.ToInt64()));
			}

	// Get an interface that this type implements.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public override Type GetInterface(String name, bool ignoreCase);

	// Get a member from this type.  The member could be a field,
	// method, constructor, event, property, or nested type.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private MemberInfo GetMemberImpl
				(String name, MemberTypes memberTypes,
			     BindingFlags bindingAttr, Binder binder,
				 CallingConventions callingConventions,
				 Type[] types, ParameterModifier[] modifiers);

	// Get a list of members from this type.  Returns an array,
	// which must then be cast to the correct type.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private Object GetMembersImpl(MemberTypes memberTypes,
										 BindingFlags bindingAttr,
										 Type arrayType,
										 String name);
	
	
	[Flags]
	private enum MemberComparison
	{
		None = 0x01,
		Candidate = 0x02,
		Override = 0x04,
	}
	
	/* 
	 * Check if m1 overrides m2
	 */
	private MemberComparison CompareMembers(MemberInfo m1, MemberInfo m2)
			{
				if(m1.MemberType != m2.MemberType || m1.Name!=m2.Name)
				{	
					return MemberComparison.None;
				}
				if(m1.MemberType==MemberTypes.Field)
				{
					FieldInfo f1 = (FieldInfo)m1;
					FieldInfo f2 = (FieldInfo)m2;

					if(f1.Attributes == f2.Attributes)
					{
						if(((f1.Attributes & FieldAttributes.FieldAccessMask) & FieldAttributes.Private) != 0)
						{
							// Private members never override each other.
							// Very important for serialization.
							// TODO: more research on protected and internal stuff here.
							return MemberComparison.None;
						}
						goto overrideCheck;
					}
				}
				if(m1.MemberType == MemberTypes.NestedType)
				{
					Type t1 = (Type)m1;
					Type t2 = (Type)m2;
					if(t1.Attributes == t2.Attributes) 
					{
						// TODO: big question, right ?
						// same as above
						if(t1.IsNestedPrivate)
						{
							return MemberComparison.None;
						}
					}
					goto overrideCheck;
				}
				MethodBase method1 = null;
				MethodBase method2 = null;
				switch(m1.MemberType)
				{
					case MemberTypes.Constructor:
					{
						method1=(MethodBase)m1;
						method2=(MethodBase)m2;
					}
					break;

					case MemberTypes.Method:
					{
						method1=(MethodBase)m1;
						method2=(MethodBase)m2;
					}
					break;
					
					case MemberTypes.Property:
					{
						if(((PropertyInfo)m1).GetAccessors().Length==0 &&
						   ((PropertyInfo)m2).GetAccessors().Length==0)
						{
							if(((PropertyInfo)m1).Attributes == 
									((PropertyInfo)m2).Attributes)
							{
								goto overrideCheck;
							}
						}

						method1=((PropertyInfo)m1).GetGetMethod() != null ?
									((PropertyInfo)m1).GetGetMethod() :
									((PropertyInfo)m1).GetSetMethod() ;
						
						method2=((PropertyInfo)m2).GetGetMethod() != null ?
									((PropertyInfo)m2).GetGetMethod() :
									((PropertyInfo)m2).GetSetMethod() ;
					}
					break;
					
					case MemberTypes.Event:
					{
						method1=((EventInfo)m1).GetAddMethod() != null ?
									((EventInfo)m1).GetAddMethod() :
									((EventInfo)m1).GetRemoveMethod() ;
						
						method2=((EventInfo)m2).GetAddMethod() != null ?
									((EventInfo)m2).GetAddMethod() :
									((EventInfo)m2).GetRemoveMethod() ;
					}
					break;
				}
				if((method1.Attributes | MethodAttributes.NewSlot) 
							!= (method1.Attributes | MethodAttributes.NewSlot))
				{
					return MemberComparison.None;
				}

				ParameterInfo [] pinfo1 = method1.GetParameters();
				ParameterInfo [] pinfo2 = method2.GetParameters();
				if(pinfo1.Length != pinfo2.Length)
				{
					return MemberComparison.None;
				}
				for(int i=0;i<pinfo1.Length;i++)
				{
					if(pinfo1[i].Attributes!=pinfo2[i].Attributes ||
						pinfo1[i].ParameterType != pinfo2[i].ParameterType)
					{
						return MemberComparison.None;
					}
				}

				if(method1 is MethodInfo)
				{
					if(!((MethodInfo)method2).ReturnType.Equals(
								((MethodInfo)method1).ReturnType))
					{
						return MemberComparison.None;
					}
				}
			overrideCheck:
				if(m1.DeclaringType.IsSubclassOf(m2.DeclaringType))
				{
					return MemberComparison.Candidate 
							| MemberComparison.Override;
				}
				return MemberComparison.Candidate;
			}

	/// <summary>
	/// This function removes overrides from an array of
	/// MemberInfo[] while retaining original order of
	/// the members in the array.
	/// </summary>
	private Object TrimMembers(Object memberArray, Type type)
			{
				MemberInfo[] members=(MemberInfo[])(memberArray);
				BitArray visibleMask = new BitArray(members.Length, false);
				int count = 0;
				int best;

				// Technically speaking, this algorithm
				// should not touch a type twice in the
				// inner loop. Need to fix that up
				// Real Soon Now (tm).
				for(int i = 0; i < members.Length; i++)
				{
					best = i;
					for(int j = 0; j < members.Length; j++)
					{
						if(i == j || members[j].Name != members[best].Name)
						{
							continue;
						}
						MemberComparison cmp = CompareMembers(members[j],
																members[best]);
						if(cmp == MemberComparison.None)
						{
							break;
						}
						if((cmp & MemberComparison.Candidate) != 0)
						{
							if((cmp & MemberComparison.Override) != 0)
							{
								best = j;
							}
						}
					}
					/* speed up, use get/set instead of indexer
					if(visibleMask[best] == false)
					{
						visibleMask[best] = true;
						count++;
					}
					*/
					if(visibleMask.Get(best) == false)
					{
						visibleMask.Set(best, true);
						count++;
					}
				}
				MemberInfo [] newMembers = (MemberInfo[])Array.CreateInstance(type,count);
				for(int i = 0, j = 0; i < visibleMask.Count; i++)
				{
					/* speed up, use get/set instead of indexer
					if(visibleMask[i] == true)
					{
						newMembers[j++] = members[i];
					}
					*/
					if(visibleMask.Get(i) == true)
					{
						newMembers[j++] = members[i];
					}
				}
				return newMembers;
			}
	
	// Implementation of "GetConstructor" provided by subclasses.
	protected override ConstructorInfo
					GetConstructorImpl(BindingFlags bindingAttr,
								       Binder binder,
								       CallingConventions callingConventions,
								       Type[] types,
								       ParameterModifier[] modifiers)
			{
				if(bindingAttr == 0)
				{
					bindingAttr = BindingFlags.Public |
								  BindingFlags.Instance;
				}
				return (ConstructorInfo)GetMemberImpl
							(".ctor", MemberTypes.Constructor,
							 bindingAttr | BindingFlags.DeclaredOnly,
							 binder, callingConventions,
							 types, modifiers);
			}

	// Get all constructors for this type.
	public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
			{
				if(bindingAttr == 0)
				{
					bindingAttr = BindingFlags.Public |
								  BindingFlags.Instance;
				}
				return (ConstructorInfo[])TrimMembers(GetMembersImpl
							(MemberTypes.Constructor,
							 bindingAttr | BindingFlags.DeclaredOnly,
							 typeof(ConstructorInfo[]), null),
							 typeof(ConstructorInfo));
			}

	// Get an event from this type.
	public override EventInfo GetEvent(String name, BindingFlags bindingAttr)
			{
				return (EventInfo)GetMemberImpl
							(name, MemberTypes.Event,
							 bindingAttr, null, CallingConventions.Any,
							 null, null);
			}

	// Get all events from this type.
	public override EventInfo[] GetEvents(BindingFlags bindingAttr)
			{
				return (EventInfo[])TrimMembers(GetMembersImpl
							(MemberTypes.Event, bindingAttr,
							 typeof(EventInfo[]), null),
							 typeof(EventInfo));
			}

	// Get a field from this type.
	public override FieldInfo GetField(String name, BindingFlags bindingAttr)
			{
				return (FieldInfo)GetMemberImpl
							(name, MemberTypes.Field,
							 bindingAttr, null, CallingConventions.Any,
							 null, null);
			}

	// Get all fields from this type.
	public override FieldInfo[] GetFields(BindingFlags bindingAttr)
			{
				return (FieldInfo[]) TrimMembers(GetMembersImpl
										(MemberTypes.Field, bindingAttr,
										 typeof(FieldInfo[]), null), 
										 typeof(FieldInfo));
			}

	// Get a member from this type.
#if ECMA_COMPAT
	internal
#else
	public
#endif
	override MemberInfo[] GetMember
				(String name, MemberTypes type, BindingFlags bindingAttr)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				return (MemberInfo[]) TrimMembers(GetMembersImpl
										(type, bindingAttr, 
										typeof(MemberInfo[]), name),
										typeof(MemberInfo));
			}

	// Get all members from this type.
	public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
			{
				return (MemberInfo[]) TrimMembers(GetMembersImpl
										(MemberTypes.All, 
										bindingAttr, 
										typeof(MemberInfo[]), null),
										typeof(MemberInfo));
			}

	// Implementation of "GetMethod" provided by subclasses.
	protected override MethodInfo
					GetMethodImpl(String name,
								  BindingFlags bindingAttr, Binder binder,
								  CallingConventions callingConventions,
								  Type[] types,
								  ParameterModifier[] modifiers)
			{
				return (MethodInfo)GetMemberImpl
							(name, MemberTypes.Method,
							 bindingAttr, binder, callingConventions,
							 types, modifiers);
			}

	// Get all methods from this type.
	public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
			{
				return (MethodInfo[]) TrimMembers(GetMembersImpl
									(MemberTypes.Method, bindingAttr,
									 typeof(MethodInfo[]), null), 
									 typeof(MethodInfo));
			}

	// Get a nested type from this type.
	public override Type GetNestedType(String name, BindingFlags bindingAttr)
			{
				return (Type)GetMemberImpl
							(name, MemberTypes.NestedType,
							 bindingAttr, null, CallingConventions.Any,
							 null, null);
			}

	// Get all nested types from this type.
	public override Type[] GetNestedTypes(BindingFlags bindingAttr)
			{
				return (Type[])GetMembersImpl
							(MemberTypes.NestedType, bindingAttr,
							 typeof(Type[]), null);
			}

	// Implementation of "GetProperty" provided by subclasses.
	protected override PropertyInfo
					GetPropertyImpl(String name,
									BindingFlags bindingAttr, Binder binder,
								    Type returnType, Type[] types,
								    ParameterModifier[] modifiers)
			{
				return (PropertyInfo)GetMemberImpl
							(name, MemberTypes.Property,
							 bindingAttr, binder, CallingConventions.Any,
							 types, modifiers);
			}

	// Get all properties from this type.
	public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
			{
				return (PropertyInfo[])TrimMembers(GetMembersImpl
							(MemberTypes.Property, bindingAttr,
							 typeof(PropertyInfo[]), null),typeof(PropertyInfo));
			}

	// Implementation of the "HasElementType" property.
	protected override bool HasElementTypeImpl()
			{
				return (IsArray || IsPointer || IsByRef);
			}

	// Search a parameter list for a particular name.
	private static int GetParamByName(ParameterInfo[] parameters,
									  String name)
			{
				int param;
				for(param = 0; param < parameters.Length; ++param)
				{
					if(parameters[param].Name == name)
					{
						return param;
					}
				}
				return -1;
			}

	// Repack a set of arguments based on a named parameter list.
	private static Object[] RepackArgs(Object[] args, String[] namedParameters,
									   ParameterInfo[] parameters)
			{
				if(args == null || args.Length == 0)
				{
					if(namedParameters.Length != 0 || parameters.Length != 0)
					{
						throw new ArgumentException
							(_("Reflection_InvalidArgs"));
					}
					return args;
				}
				else if(namedParameters.Length > args.Length)
				{
					throw new ArgumentException
						(_("Reflection_InvalidArgs"));
				}
				else if(args.Length != parameters.Length)
				{
					throw new ArgumentException
						(_("Reflection_InvalidArgs"));
				}
				else
				{
					// Construct an array of remapping indices.
					int numArgs = args.Length;
					int[] remapArgs = new int [numArgs];
					int arg, param;
					String name;
					for(arg = 0; arg < numArgs; ++arg)
					{
						remapArgs[arg] = -1;
					}
					for(arg = 0; arg < numArgs; ++arg)
					{
						if(arg < namedParameters.Length)
						{
							// Search for the parameter by name.
							name = namedParameters[arg];
							if(name == null)
							{
								throw new ArgumentNullException
								  ("namedParameters[" + arg.ToString() + "]");
							}
							param = GetParamByName(parameters, name);
							if(param == -1)
							{
								throw new ArgumentException
									(_("Reflection_ParamName"));
							}
							remapArgs[param] = arg;
						}
						else
						{
							// Allocate this argument to first unnamed slot.
							for(param = 0; param < numArgs; ++param)
							{
								if(remapArgs[param] == -1)
								{
									remapArgs[param] = arg;
									break;
								}
							}
						}
					}

					// Construct the new argument array.
					Object[] newArgs = new Object [numArgs];
					for(arg = 0; arg < numArgs; ++arg)
					{
						newArgs[arg] = args[remapArgs[arg]];
					}

					// Return the new argument array.
					return newArgs;
				}
			}

	// Convert a list of arguments into a list of types.
	private static Type[] ArgsToTypes(Object[] args)
			{
				Type[] types;
				int posn;
				Object value;

				// The test is easy if there are no arguments.
				if(args == null)
				{
					return new Type [0];
				}

				// Convert the object types into a type array.
				types = new Type [args.Length];
				for(posn = 0; posn < args.Length; ++posn)
				{
					value = args[posn];
					if(value == null)
					{
						types[posn] = null;
					}
					else
					{
						types[posn] = value.GetType();
					}
				}

				// Done.
				return types;
			}

	// Invoke a member.
	public override Object InvokeMember
						(String name, BindingFlags invokeAttr,
					     Binder binder, Object target, Object[] args,
					     ParameterModifier[] modifiers,
					     CultureInfo culture, String[] namedParameters)
			{
				MemberInfo member;
				MemberTypes types;
				MemberTypes otherTypes = (MemberTypes)0;
				Type[] argTypes;

				// Validate the parameters against the invocation type.
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				
				// Specification say that if type of look up is absent
				// the lookup is treated as Public Instance by default
				if((invokeAttr & BindingFlags.NonPublic) == 0)
				{
					invokeAttr |= BindingFlags.Public;
				}
				if((invokeAttr & BindingFlags.Static) == 0)
				{
					invokeAttr |= BindingFlags.Instance;
				}
				
				if((invokeAttr & BindingFlags.CreateInstance) != 0)
				{
					if((invokeAttr & (BindingFlags.InvokeMethod |
									  BindingFlags.GetField |
									  BindingFlags.SetField |
									  BindingFlags.GetProperty |
									  BindingFlags.SetProperty)) != 0)
					{
						throw new ArgumentException
							(_("Reflection_InvokeAttr"));
					}
					types = MemberTypes.Constructor;
					invokeAttr |= BindingFlags.DeclaredOnly;
					name = ".ctor";
					if(target != null)
					{
						throw new TargetException(_("Reflection_CtorTarget"));
					}
					argTypes = ArgsToTypes(args);
				}
				else if((invokeAttr & BindingFlags.InvokeMethod) != 0)
				{
					if((invokeAttr & (BindingFlags.SetField |
									  BindingFlags.SetProperty)) != 0)
					{
						throw new ArgumentException
							(_("Reflection_InvokeAttr"));
					}
					types = MemberTypes.Method;
					argTypes = ArgsToTypes(args);
				}
				else if((invokeAttr & BindingFlags.GetField) != 0)
				{
					if((invokeAttr & (BindingFlags.SetField |
									  BindingFlags.SetProperty)) != 0)
					{
						throw new ArgumentException
							(_("Reflection_InvokeAttr"));
					}
					if((invokeAttr & BindingFlags.GetProperty) != 0)
					{
						// It is possible for callers to ask for
						// "either a field or a property with this name".
						otherTypes = MemberTypes.Property;
					}
					types = MemberTypes.Field;
					if(args != null && args.Length != 0)
					{
						throw new ArgumentException
							(_("Reflection_GetField"));
					}
					argTypes = null;
				}
				else if((invokeAttr & BindingFlags.SetField) != 0)
				{
					if((invokeAttr & (BindingFlags.GetField |
									  BindingFlags.GetProperty)) != 0)
					{
						throw new ArgumentException
							(_("Reflection_InvokeAttr"));
					}
					if((invokeAttr & BindingFlags.SetProperty) != 0)
					{
						// It is possible for callers to ask for
						// "either a field or a property with this name".
						otherTypes = MemberTypes.Property;
					}
					types = MemberTypes.Field;
					if(args == null || args.Length != 1)
					{
						throw new ArgumentException
							(_("Reflection_SetField"));
					}
					argTypes = null;
				}
				else if((invokeAttr & BindingFlags.GetProperty) != 0)
				{
					if((invokeAttr & BindingFlags.SetProperty) != 0)
					{
						throw new ArgumentException
							(_("Reflection_InvokeAttr"));
					}
					types = MemberTypes.Property;
					argTypes = ArgsToTypes(args);
				}
				else if((invokeAttr & BindingFlags.SetProperty) != 0)
				{
					if((invokeAttr & BindingFlags.GetProperty) != 0)
					{
						throw new ArgumentException
							(_("Reflection_InvokeAttr"));
					}
					types = MemberTypes.Property;
					argTypes = ArgsToTypes(args);
				}
				else
				{
					throw new ArgumentException
						(_("Reflection_InvokeAttr"));
				}

				// Get the default member name if necessary.
				if(name == String.Empty &&
				   Attribute.IsDefined(this, typeof(DefaultMemberAttribute)))
				{
					DefaultMemberAttribute attr =
						(DefaultMemberAttribute)
							Attribute.GetCustomAttribute
								(this, typeof(DefaultMemberAttribute));
					name = attr.MemberName;
				}

				// Get the member from the type.
				member = GetMemberImpl(name, types, invokeAttr, binder,
									   CallingConventions.Any,
									   argTypes, modifiers);
				if(member == null && otherTypes != (MemberTypes)0)
				{
					types = otherTypes;
					member = GetMemberImpl(name, types, invokeAttr, binder,
										   CallingConventions.Any,
										   argTypes, modifiers);
				}
				if(member == null)
				{
					if(types == MemberTypes.Field)
					{
						throw new MissingFieldException
							(_("Reflection_MissingField"));
					}
					else if(types == MemberTypes.Property)
					{
						throw new MissingFieldException
							(_("Reflection_MissingProperty"));
					}
					else
					{
						throw new MissingMethodException
							(_("Reflection_MissingMethod"));
					}
				}

				// Invoke the member.
				if(types == MemberTypes.Constructor)
				{
					// Invoke a constructor.
					if(namedParameters != null)
					{
						args = RepackArgs(args, namedParameters,
								  ((ConstructorInfo)member).GetParameters());
					}
					return ((ConstructorInfo)member).Invoke
								(invokeAttr, binder, args, culture);
				}
				else if(types == MemberTypes.Method)
				{
					// Invoke a method.
					if(namedParameters != null)
					{
						args = RepackArgs(args, namedParameters,
								  ((MethodInfo)member).GetParameters());
					}
					return ((MethodInfo)member).Invoke
								(target, invokeAttr, binder, args, culture);
				}
				else if(types == MemberTypes.Field)
				{
					// Invoke a field.
					if((invokeAttr & BindingFlags.GetField) != 0)
					{
						// Get the value of a field.
						return ((FieldInfo)member).GetValue(target);
					}
					else
					{
						// Set the value of a field.
						((FieldInfo)member).SetValue(target, args[0],
													 invokeAttr, binder,
													 culture);
						return null;
					}
				}
				else
				{
					// Invoke a property.
					if((invokeAttr & BindingFlags.GetProperty) != 0)
					{
						// Get the value of a property.
						if(namedParameters != null)
						{
							args = RepackArgs(args, namedParameters,
							  ((PropertyInfo)member).GetIndexParameters());
						}
						return ((PropertyInfo)member).GetValue
							(target, invokeAttr, binder, args, culture);
					}
					else
					{
						// Set the value of a property.
						MethodInfo setter;
						setter = ((PropertyInfo)member).GetSetMethod();
						if(setter == null)
						{
							throw new MissingMethodException
								(_("Reflection_NoPropertySet"));
						}
						if(namedParameters != null)
						{
							args = RepackArgs(args, namedParameters,
											  setter.GetParameters());
						}
						return setter.Invoke(target, invokeAttr, binder,
											 args, culture);
					}
				}
			}

	// Implementation of the "IsByRef" property.
	protected override bool IsByRefImpl()
			{
				return (GetClrTypeCategory() == ClrTypeCategory.ByRef);
			}

	// Implementation of the "IsCOMObject" property.
	protected override bool IsCOMObjectImpl()
			{
				return (GetClrTypeCategory() == ClrTypeCategory.COMObject);
			}

	// Implementation of the "IsContextful" property.
	protected override bool IsContextfulImpl()
			{
				return IsSubclassOf(typeof(System.ContextBoundObject));
			}

	// Implementation of the "IsMarshalByRef" property.
	protected override bool IsMarshalByRefImpl()
			{
				return IsSubclassOf(typeof(System.MarshalByRefObject));
			}

	// Determine if this is a nested type.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private bool IsClrNestedType();

#if !ECMA_COMPAT
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private Guid GetClrGUID();
#endif

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private System.Reflection.Module GetClrModule();

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private Type GetClrNestedDeclaringType();

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private String GetClrName();

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private String GetClrNamespace();

	// Implement overridden properties.
#if !ECMA_COMPAT
	public override Guid GUID
			{
				get
				{
					return GetClrGUID();
				}
			}
#endif
	public override System.Reflection.Module Module
			{
				get
				{
					return GetClrModule();
				}
			}
	public override MemberTypes MemberType
			{
				get
				{
					if(IsClrNestedType())
					{
						return MemberTypes.NestedType;
					}
					else
					{
						return MemberTypes.TypeInfo;
					}
				}
			}
	public override String Name
			{
				get
				{
					return GetClrName();
				}
			}
	public override String Namespace
			{
				get
				{
					return GetClrNamespace();
				}
			}
	public override Type UnderlyingSystemType
			{
				get
				{
					return this;
				}
			}
	public override Type DeclaringType
			{
				get
				{
					if(IsClrNestedType())
					{
						return GetClrNestedDeclaringType();
					}
					else
					{
						return null;
					}
				}
			}
	public override Type ReflectedType
			{
				get
				{
					if(IsClrNestedType())
					{
						return GetClrNestedDeclaringType();
					}
					else
					{
						return null;
					}
				}
			}

	// Internal methods that support generic types.

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern protected override bool HasGenericArgumentsImpl();

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern protected override bool HasGenericParametersImpl();

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public override Type[] GetGenericArguments();

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public override Type BindGenericParameters(Type[] inst);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public override Type GetGenericTypeDefinition();

#endif // CONFIG_REFLECTION

#if CONFIG_SERIALIZATION

	// Get the serialization data for this type.
	public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				UnitySerializationHolder.Serialize
					(info, UnitySerializationHolder.UnityType.ClrType,
					 FullName, Assembly);
			}

#endif

}; // class ClrType

}; // namespace System.Reflection
