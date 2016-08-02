/*
 * Type.cs - Implementation of the "System.Type" class.
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

using System.Private;
using System.Reflection;
using System.Globalization;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Diagnostics;

#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDual)]
#if CONFIG_FRAMEWORK_1_2 && CONFIG_REFLECTION
[ComDefaultInterface(typeof(_Type))]
#endif
#endif
public abstract class Type
#if CONFIG_REFLECTION
	: MemberInfo, IReflect
#if CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2
	, _Type
#endif
#endif
{

	// The only instance of "Missing" in the system.
	public static readonly Object Missing = System.Reflection.Missing.Value;

	// The delimiter for type names.
	public static readonly char Delimiter = '.';

	// An empty array of types.
	public static readonly Type[] EmptyTypes = new Type [0];

#if !ECMA_COMPAT
	// Pre-defined member filters.
	public static readonly MemberFilter FilterName = 
				new MemberFilter(FilterNameImpl);
	public static readonly MemberFilter FilterAttribute = 
				new MemberFilter(FilterAttributeImpl);
	public static readonly MemberFilter FilterNameIgnoreCase = 
				new MemberFilter(FilterNameIgnoreCaseImpl);
#endif

	// Cached copies of useful type descriptors.
	private static readonly Type objectType = typeof(Object);
	private static readonly Type valueType  = typeof(ValueType);
	private static readonly Type enumType   = typeof(Enum);

	// Constructor.
	protected Type() : base() {}

	// Get the rank of this type, if it is an array.
	public virtual int GetArrayRank()
			{
				throw new NotSupportedException(_("NotSupp_NotArrayType"));
			}

	// Get the attribute flags for this type.
#if CONFIG_REFLECTION
	protected abstract TypeAttributes GetAttributeFlagsImpl();
#else
	internal abstract TypeAttributes GetAttributeFlagsImpl();
#endif

	// Get the element type for this type.
#if CONFIG_REFLECTION
	public abstract Type GetElementType();
#else
	internal abstract Type GetElementType();
#endif

	// Get all interfaces that this type implements.
#if CONFIG_REFLECTION
	public abstract Type[] GetInterfaces();
#else
	internal abstract Type[] GetInterfaces();
#endif

	// Implementation of the "IsArray" property.
	protected abstract bool IsArrayImpl();

	// Determine if "this" implements the interface "c".
	private bool IsImplementationOf(Type c)
			{
				Type[] interfaces = GetInterfaces();
				int posn;
				for(posn = 0; posn < interfaces.Length; ++posn)
				{
					if(c == interfaces[posn])
					{
						return true;
					}
				}
				return false;
			}

	// Determine if this type is assignable from another type.
	public virtual bool IsAssignableFrom(Type c)
			{
				if(c == null)
				{
					return false;
				}
				else if(c == this)
				{
					return true;
				}
				else if(c.IsSubclassOf(this))
				{
					return true;
				}
				else if(IsInterface)
				{
					return c.IsImplementationOf(this);
				}
				else if(c.IsInterface)
				{
					return this == typeof(Object);
				}
				else
				{
					return false;
				}
			}

	// Determine if an object is an instance of this type.
	public virtual bool IsInstanceOfType(Object obj)
			{
				if(obj == null)
				{
					return false;
				}
				else
				{
					return IsAssignableFrom(obj.GetType());
				}
			}

	// Implementation of the "IsPointer" property.
	protected abstract bool IsPointerImpl();

	// Implementation of the "IsPrimitive" property.
#if CONFIG_REFLECTION
	protected abstract bool IsPrimitiveImpl();
#else
	internal abstract bool IsPrimitiveImpl();
#endif

	// Determine if the current type is a subclass of another type.
	public virtual bool IsSubclassOf(Type c)
			{
				Type current = this;
				if(c == current)
				{
					return false;
				}
				while(current != null)
				{
					if(current == c)
					{
						return true;
					}
					current = current.BaseType;
				}
				return false;
			}

	// Implementation of the "IsValueType" property.
	protected virtual bool IsValueTypeImpl()
			{
				if(this == valueType || this == enumType)
				{
					return false;
				}
				else
				{
					return IsSubclassOf(valueType);
				}
			}

	// Convert this type into a string.
	public override String ToString()
			{
			#if CONFIG_REFLECTION
				return "Type: " + Name;
			#else
				return "Type: " + FullName;
			#endif
			}

	// Properties that are always accessible, even if no reflection support.
	public abstract String AssemblyQualifiedName { get; }
	public abstract Type BaseType { get; }
	public abstract String FullName { get; }
	public bool IsArray { get { return IsArrayImpl(); } }
	public bool IsClass
			{
				get
				{
					return ((GetAttributeFlagsImpl() &
							 TypeAttributes.ClassSemanticsMask) ==
							 	TypeAttributes.Class) &&
						   !IsSubclassOf(valueType);
				}
			}
	public bool IsEnum
			{
				get
				{
					return IsSubclassOf(enumType);
				}
			}
	public bool IsInterface
			{
				get
				{
					return ((GetAttributeFlagsImpl() &
							 TypeAttributes.ClassSemanticsMask) ==
							 	TypeAttributes.Interface);
				}
			}
	public bool IsPointer { get { return IsPointerImpl(); } }
	public bool IsValueType { get { return IsValueTypeImpl(); } }
#if CONFIG_REFLECTION
	public bool IsPrimitive { get { return IsPrimitiveImpl(); } }
#else
	internal bool IsPrimitive { get { return IsPrimitiveImpl(); } }
#endif

	// Get a type from a runtime type handle.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Type GetTypeFromHandle(RuntimeTypeHandle handle);

#if CONFIG_RUNTIME_INFRA

	// Get the runtime type handle associated with an object.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static RuntimeTypeHandle GetTypeHandle(Object obj);

	// Infrastructure properties.
	public abstract System.Reflection.Assembly Assembly { get; }
	public abstract RuntimeTypeHandle TypeHandle { get; }

#endif // CONFIG_RUNTIME_INFRA

#if CONFIG_REFLECTION

	// Type equality testing.
	public override bool Equals(Object obj)
			{
				if(obj == null || !(obj is Type))
				{
					return false;
				}
				return (UnderlyingSystemType ==
						((Type)obj).UnderlyingSystemType);
			}
	public bool Equals(Type obj)
			{
				if(obj == null)
				{
					return false;
				}
				return (UnderlyingSystemType == obj.UnderlyingSystemType);
			}

	// Validate the "types" argument to a "GetConstructor" or "GetMethod" call.
	private static void ValidateTypes(Type[] types)
			{
				int posn;
				if(types == null)
				{
					throw new ArgumentNullException("types");
				}
				for(posn = 0; posn < types.Length; ++posn)
				{
					if(types[posn] == null)
					{
						throw new ArgumentNullException("types");
					}
				}
			}

	// Get a constructor from this type.
	public ConstructorInfo GetConstructor(BindingFlags bindingAttr,
										  Binder binder, Type[] types,
										  ParameterModifier[] modifiers)
			{
				return GetConstructor(bindingAttr, binder,
									  CallingConventions.Any,
									  types, modifiers);
			}
	public ConstructorInfo GetConstructor(Type[] types)
			{
				return GetConstructor(BindingFlags.Public |
									  BindingFlags.Instance, null,
									  CallingConventions.Any,
									  types, null);
			}
	public ConstructorInfo GetConstructor(BindingFlags bindingAttr,
										  Binder binder,
										  CallingConventions callingConventions,
										  Type[] types,
										  ParameterModifier[] modifiers)
			{
				ValidateTypes(types);
				return GetConstructorImpl(bindingAttr, binder,
										  callingConventions,
										  types, modifiers);
			}

	// Implementation of "GetConstructor" provided by subclasses.
	protected abstract ConstructorInfo
					GetConstructorImpl(BindingFlags bindingAttr,
								       Binder binder,
								       CallingConventions callingConventions,
								       Type[] types,
								       ParameterModifier[] modifiers);

	// Get all constructors for this type.
	public abstract ConstructorInfo[] GetConstructors(BindingFlags bindingAttr);
	public ConstructorInfo[] GetConstructors()
			{
				return GetConstructors(BindingFlags.Instance |
									   BindingFlags.Public);
			}

	// Get the default members within this type.
	public virtual MemberInfo[] GetDefaultMembers()
			{
				// Get the value of the "DefaultMember" attribute.
				DefaultMemberAttribute attr = (DefaultMemberAttribute)
					Attribute.GetCustomAttribute
						(this, typeof(DefaultMemberAttribute));
				if(attr == null)
				{
					return new MemberInfo [0];
				}

				// Find all members with the specified name.
				return GetMember(attr.MemberName);
			}

#if !ECMA_COMPAT

	// Find the interfaces that match a set of filter criteria.
	public virtual Type[] FindInterfaces(TypeFilter filter,
										 Object filterCriteria)
			{
				if(filter == null)
				{
					throw new ArgumentNullException("filter");
				}
				Type[] interfaces = GetInterfaces();
				if(interfaces == null)
				{
					return EmptyTypes;
				}
				int posn, posn2;
				posn = 0;
				posn2 = 0;
				while(posn < interfaces.Length)
				{
					if(filter(interfaces[posn], filterCriteria))
					{
						interfaces[posn2++] = interfaces[posn];
					}
					++posn;
				}
				if(posn2 == interfaces.Length)
				{
					return interfaces;
				}
				else
				{
					Type[] list = new Type [posn2];
					Array.Copy(interfaces, 0, list, 0, posn2);
					return list;
				}
			}

	/* convenience function wrapping over the other abstract stuff */
	public virtual MemberInfo[]	FindMembers	(MemberTypes memberType,
								 BindingFlags bindingAttr,
								 MemberFilter filter,
								 Object filterCriteria)
			{
				ArrayList list=new ArrayList(); 
				if((memberType & MemberTypes.Constructor)!=0)
				{
					ConstructorInfo[] ctors = GetConstructors(bindingAttr);
					foreach(MemberInfo minfo in ctors)
					{
						if(filter==null || filter(minfo,filterCriteria))
						{
							list.Add(minfo);
						}
					}
				}
				if((memberType & MemberTypes.Event)!=0)
				{
					EventInfo[] events = GetEvents(bindingAttr);
					foreach(MemberInfo minfo in events)
					{
						if(filter==null || filter(minfo,filterCriteria))
						{
							list.Add(minfo);
						}
					}
				}
				if((memberType & MemberTypes.Field)!=0)
				{
					FieldInfo[] fields = GetFields(bindingAttr);
					foreach(MemberInfo minfo in fields)
					{
						if(filter==null || filter(minfo,filterCriteria))
						{
							list.Add(minfo);
						}
					}
				}
				if((memberType & MemberTypes.Method)!=0)
				{
					MethodInfo[] mthds = GetMethods(bindingAttr);
					foreach(MemberInfo minfo in mthds)
					{
						if(filter==null || filter(minfo,filterCriteria))
						{
							list.Add(minfo);
						}
					}
				}
				if((memberType & MemberTypes.Property)!=0)
				{
					PropertyInfo[] props= GetProperties(bindingAttr);
					foreach(MemberInfo minfo in props)
					{
						if(filter==null || filter(minfo,filterCriteria))
						{
							list.Add(minfo);
						}
					}
					
				}
				if((memberType & MemberTypes.NestedType)!=0)
				{
					Type[] types= GetNestedTypes(bindingAttr);
					foreach(MemberInfo minfo in types)
					{
						if(filter==null || filter(minfo,filterCriteria))
						{
							list.Add(minfo);
						}
					}
				}
				
				MemberInfo[] retval=new MemberInfo[list.Count];
				list.CopyTo(retval);
				return retval;				
			}
	
	// Filter a member based on its name.
	private static bool FilterNameImpl(MemberInfo m, Object filterCriteria)
			{
				// Validate the filter critiera.
				String filter = (filterCriteria as String);
				if(filter == null)
				{
					throw new InvalidFilterCriteriaException
						(_("Exception_FilterName"));
				}
				filter = filter.Trim();

				// Get the member name.
				String name = m.Name;
				if(m is Type)
				{
					// Trim nested type names down to their last component.
					int index = name.LastIndexOf('+');
					if(index != -1)
					{
						name = name.Substring(index + 1);
					}
				}

				// Check for prefix matches.
				if(filter.EndsWith("*"))
				{
					return name.StartsWith
						(filter.Substring(0, filter.Length - 1));
				}

				// Perform an ordinary match.
				return (name == filter);
			}

	// Filter a member based on its name and ignore case.
	private static bool FilterNameIgnoreCaseImpl(MemberInfo m, 
 												 Object filterCriteria)
			{
				// Validate the filter critiera.
				String filter = (filterCriteria as String);
				if(filter == null)
				{
					throw new InvalidFilterCriteriaException
						(_("Exception_FilterName"));
				}
				filter = filter.Trim();

				// Get the member name.
				String name = m.Name;
				if(m is Type)
				{
					// Trim nested type names down to their last component.
					int index = name.LastIndexOf('+');
					if(index != -1)
					{
						name = name.Substring(index + 1);
					}
				}

				// Check for prefix matches.
				if(filter.EndsWith("*"))
				{
					filter = filter.Substring(0, filter.Length - 1);
					if(name.Length < filter.Length)
					{
						return false;
					}
					return (String.Compare(name, 0, filter, 0,
										   filter.Length, true) == 0);
				}

				// Perform an ordinary match.
				return (String.Compare(name, filter, true) == 0);
			}

	// Filter a member based on its attributes.
	private static bool FilterAttributeImpl(MemberInfo m, 
											Object filterCriteria)
			{
				if(filterCriteria == null)
				{
					throw new InvalidFilterCriteriaException
						(_("Exception_FilterAttribute"));
				}
				if(m is FieldInfo)
				{
					// Validate the field criteria.
					FieldAttributes fattrs;
					if(filterCriteria is int)
					{
						fattrs = (FieldAttributes)(int)filterCriteria;
					}
					else if(filterCriteria is FieldAttributes)
					{
						fattrs = (FieldAttributes)filterCriteria;
					}
					else
					{
						throw new InvalidFilterCriteriaException
							(_("Exception_FilterAttribute"));
					}

					// Check the specified conditions.
					FieldAttributes fmattrs = ((FieldInfo)m).Attributes;
					if((fattrs & FieldAttributes.FieldAccessMask) != 0)
					{
						if((fattrs & FieldAttributes.FieldAccessMask) !=
								(fmattrs & FieldAttributes.FieldAccessMask))
						{
							return false;
						}
					}
					fattrs &= (FieldAttributes.Static |
							   FieldAttributes.InitOnly |
							   FieldAttributes.Literal |
							   FieldAttributes.NotSerialized |
							   FieldAttributes.PinvokeImpl);
					if((fattrs & fmattrs) != fattrs)
					{
						return false;
					}
					return true;
				}
				else if(m is MethodInfo || m is ConstructorInfo)
				{
					// Validate the method criteria.
					MethodAttributes mattrs;
					if(filterCriteria is int)
					{
						mattrs = (MethodAttributes)(int)filterCriteria;
					}
					else if(filterCriteria is MethodAttributes)
					{
						mattrs = (MethodAttributes)filterCriteria;
					}
					else
					{
						throw new InvalidFilterCriteriaException
							(_("Exception_FilterAttribute"));
					}

					// Check the specified conditions.
					MethodAttributes mmattrs = ((MethodBase)m).Attributes;
					if((mattrs & MethodAttributes.MemberAccessMask) != 0)
					{
						if((mattrs & MethodAttributes.MemberAccessMask) !=
								(mmattrs & MethodAttributes.MemberAccessMask))
						{
							return false;
						}
					}
					mattrs &= (MethodAttributes.Static |
							   MethodAttributes.Final |
							   MethodAttributes.Virtual |
							   MethodAttributes.Abstract |
							   MethodAttributes.SpecialName);
					if((mattrs & mmattrs) != mattrs)
					{
						return false;
					}
					return true;
				}
				return false;
			}

#endif // !ECMA_COMPAT

	// Get an event from this type.
	public abstract EventInfo GetEvent(String name, BindingFlags bindingAttr);
	public EventInfo GetEvent(String name)
			{
				return GetEvent(name, BindingFlags.Public |
									  BindingFlags.Instance |
									  BindingFlags.Static);
			}

	// Get all events from this type.
	public abstract EventInfo[] GetEvents(BindingFlags bindingAttr);
	public virtual EventInfo[] GetEvents()
			{
				return GetEvents(BindingFlags.Public |
								 BindingFlags.Instance |
								 BindingFlags.Static);
			}

	// Get a field from this type.
	public abstract FieldInfo GetField(String name, BindingFlags bindingAttr);
	public FieldInfo GetField(String name)
			{
				return GetField(name, BindingFlags.Public |
									  BindingFlags.Instance |
									  BindingFlags.Static);
			}

	// Get all fields from this type.
	public abstract FieldInfo[] GetFields(BindingFlags bindingAttr);
	public FieldInfo[] GetFields()
			{
				return GetFields(BindingFlags.Public |
								 BindingFlags.Instance |
								 BindingFlags.Static);
			}

	// Get the hash code for this type.
	public override int GetHashCode()
			{
				Type underlying = UnderlyingSystemType;
				if(underlying != this)
				{
					return underlying.GetHashCode();
				}
				else
				{
					return base.GetHashCode();
				}
			}

	// Get an interface that this type implements.
	public abstract Type GetInterface(String name, bool ignoreCase);
	public Type GetInterface(String name)
			{
				return GetInterface(name, false);
			}

#if !ECMA_COMPAT

	// Get an interface mapping for a specific interface type.
	public virtual InterfaceMapping GetInterfaceMap(Type interfaceType)
			{
				return new InterfaceMapping();
			}

#endif

	// Get a member from this type.
	public virtual MemberInfo[] GetMember
				(String name, BindingFlags bindingAttr)
			{
				return GetMember(name, MemberTypes.All, bindingAttr);
			}
	public MemberInfo[] GetMember(String name)
			{
				return GetMember(name, MemberTypes.All,
								 BindingFlags.Public |
								 BindingFlags.Instance |
								 BindingFlags.Static);
			}
#if ECMA_COMPAT
	internal
#else
	public
#endif
	virtual MemberInfo[] GetMember(String name, MemberTypes type,
								   BindingFlags bindingAttr)
			{
				throw new NotSupportedException(_("NotSupp_SubClass"));
			}

	// Get all members from this type.
	public abstract MemberInfo[] GetMembers(BindingFlags bindingAttr);
	public MemberInfo[] GetMembers()
			{
				return GetMembers(BindingFlags.Public |
								  BindingFlags.Instance |
								  BindingFlags.Static);
			}

	// Get a method from this type.
	public MethodInfo GetMethod(String name, BindingFlags bindingAttr)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				return GetMethodImpl(name, bindingAttr, null,
								     CallingConventions.Any, null, null);
			}
	public MethodInfo GetMethod(String name, BindingFlags bindingAttr,
								Binder binder, Type[] types,
								ParameterModifier[] modifiers)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				ValidateTypes(types);
				return GetMethodImpl(name, bindingAttr, binder,
								     CallingConventions.Any,
								     types, modifiers);
			}
	public MethodInfo GetMethod(String name, Type[] types)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				ValidateTypes(types);
				return GetMethodImpl(name, BindingFlags.Public |
									       BindingFlags.Instance |
									       BindingFlags.Static,
								     null, CallingConventions.Any,
								     types, null);
			}
	public MethodInfo GetMethod(String name, Type[] types,
								ParameterModifier[] modifiers)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				ValidateTypes(types);
				return GetMethodImpl(name, BindingFlags.Public |
									       BindingFlags.Instance |
									       BindingFlags.Static,
								     null, CallingConventions.Any,
								     types, modifiers);
			}
	public MethodInfo GetMethod(String name)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				return GetMethodImpl(name, BindingFlags.Public |
									       BindingFlags.Instance |
									       BindingFlags.Static,
								     null, CallingConventions.Any,
								     null, null);
			}
	public MethodInfo GetMethod(String name, BindingFlags bindingAttr,
								Binder binder,
								CallingConventions callingConventions,
								Type[] types,
								ParameterModifier[] modifiers)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				ValidateTypes(types);
				return GetMethodImpl(name, bindingAttr, binder,
									 callingConventions, types, modifiers);
			}

	// Implementation of "GetMethod" provided by subclasses.
	protected abstract MethodInfo
					GetMethodImpl(String name,
								  BindingFlags bindingAttr, Binder binder,
								  CallingConventions callingConventions,
								  Type[] types,
								  ParameterModifier[] modifiers);

	// Get all methods from this type.
	public abstract MethodInfo[] GetMethods(BindingFlags bindingAttr);
	public MethodInfo[] GetMethods()
			{
				return GetMethods(BindingFlags.Public |
								  BindingFlags.Instance |
								  BindingFlags.Static);
			}

	// Get a nested type from this type.
	public abstract Type GetNestedType(String name, BindingFlags bindingAttr);
	public Type GetNestedType(String name)
			{
				return GetNestedType(name,
									 BindingFlags.Public |
								     BindingFlags.Instance |
								     BindingFlags.Static);
			}

	// Get all nested types from this type.
	public abstract Type[] GetNestedTypes(BindingFlags bindingAttr);
	public Type[] GetNestedTypes()
			{
				return GetNestedTypes(BindingFlags.Public |
								      BindingFlags.Instance |
								      BindingFlags.Static);
			}

	// Get a property from this type.
	public PropertyInfo GetProperty(String name)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				return GetPropertyImpl(name,
									   BindingFlags.Public |
									   BindingFlags.Instance |
									   BindingFlags.Static,
									   null, null, null, null);
			}
	public PropertyInfo GetProperty(String name, Type returnType)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				if(returnType == null)
				{
					throw new ArgumentNullException("returnType");
				}
				return GetPropertyImpl(name,
									   BindingFlags.Public |
									   BindingFlags.Instance |
									   BindingFlags.Static,
									   null, returnType, null, null);
			}
	public PropertyInfo GetProperty(String name, Type[] types)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				ValidateTypes(types);
				return GetPropertyImpl(name,
									   BindingFlags.Public |
									   BindingFlags.Instance |
									   BindingFlags.Static,
									   null, null, types, null);
			}
	public PropertyInfo GetProperty(String name, Type returnType, Type[] types)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				if(returnType == null)
				{
					throw new ArgumentNullException("returnType");
				}
				ValidateTypes(types);
				return GetPropertyImpl(name,
									   BindingFlags.Public |
									   BindingFlags.Instance |
									   BindingFlags.Static,
									   null, returnType, types, null);
			}
	public PropertyInfo GetProperty(String name, BindingFlags bindingAttr)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				return GetPropertyImpl(name, bindingAttr,
									   null, null, null, null);
			}
	public PropertyInfo GetProperty(String name, Type returnType,
								    Type[] types,
									ParameterModifier[] modifiers)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				if(returnType == null)
				{
					throw new ArgumentNullException("returnType");
				}
				ValidateTypes(types);
				return GetPropertyImpl(name,
									   BindingFlags.Public |
									   BindingFlags.Instance |
									   BindingFlags.Static,
									   null, returnType, types, modifiers);
			}
	public PropertyInfo GetProperty(String name,
						    		BindingFlags bindingAttr,
									Binder binder,
						    		Type returnType, Type[] types,
									ParameterModifier[] modifiers)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				if(returnType == null)
				{
					throw new ArgumentNullException("returnType");
				}
				ValidateTypes(types);
				return GetPropertyImpl(name, bindingAttr,
									   binder, returnType,
									   types, modifiers);
			}

	// Implementation of "GetProperty" provided by subclasses.
	protected abstract PropertyInfo
					GetPropertyImpl(String name,
									BindingFlags bindingAttr, Binder binder,
								    Type returnType, Type[] types,
								    ParameterModifier[] modifiers);

	// Get all properties from this type.
	public abstract PropertyInfo[] GetProperties(BindingFlags bindingAttr);
	public PropertyInfo[] GetProperties()
			{
				return GetProperties(BindingFlags.Public |
								     BindingFlags.Instance |
								     BindingFlags.Static);
			}

	// Get a type by name.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Type GetType(String name, bool throwOnError,
									  bool ignoreCase);
	public static Type GetType(String name, bool throwOnError)
			{
				return GetType(name, throwOnError, false);
			}
	public static Type GetType(String name)
			{
				return GetType(name, false, false);
			}

	// Get the types of the objects in a specified array.
	public static Type[] GetTypeArray(Object[] args)
			{
				int len, posn;
				Type[] types;
				if(args == null)
				{
					throw new ArgumentNullException("args");
				}
				len = args.Length;
				types = new Type[len];
				for(posn = 0; posn < len; ++posn)
				{
					types[posn] = args[posn].GetType();
				}
				return types;
			}

#if !ECMA_COMPAT

	// NOTE: this is only a quick fix , more investigation needed
	// to see if we need to handle this in a better (faster) way
	public static TypeCode GetTypeCode(Type type)
			{
				if(type == null)
				{
					return TypeCode.Empty;
				}
				if(type == typeof(Boolean))
				{
					return TypeCode.Boolean;
				}
				if(type == typeof(Byte))
				{
					return TypeCode.Byte;	
				}
				if(type == typeof(SByte))
				{
					return TypeCode.SByte;	
				}
				if(type == typeof(Int16))
				{
					return TypeCode.Int16;	
				}
				if(type == typeof(UInt16))
				{
					return TypeCode.UInt16;
				}
				if(type == typeof(Char))
				{
					return TypeCode.Char;
				}
				if(type == typeof(Int32))
				{
					return TypeCode.Int32;
				}
				if(type == typeof(UInt32))
				{
					return TypeCode.UInt32;
				}
				if(type == typeof(Int64))
				{
					return TypeCode.Int64;	
				}
				if(type == typeof(UInt64))
				{
					return TypeCode.UInt64;
				}
				if(type == typeof(Single))
				{
					return TypeCode.Single;	
				}
				if(type == typeof(Double))
				{
					return TypeCode.Double;
				}
				if(type == typeof(String))
				{
					return TypeCode.String;
				}
				if(type == typeof(Decimal))
				{
					return TypeCode.Decimal;
				}
				if(type.IsEnum)
				{
					return GetTypeCode(Enum.GetUnderlyingType(type));
				}
				if(type == typeof(DateTime))
				{
					return TypeCode.DateTime;
				}
				if(type == typeof(DBNull))
				{
					return TypeCode.DBNull;
				}
				return TypeCode.Object;
			}
	// Get a type from a class identifier.
	//
	// This functionality is not supported, as it is
	// intended for implementing COM, which we don't have.
	public static Type GetTypeFromCLSID(Guid clsid, String server,
										bool throwOnError)
			{
				if(throwOnError)
				{
					throw new TypeLoadException();
				}
				return null;
			}
	public static Type GetTypeFromCLSID(Guid clsid, bool throwOnError)
			{
				return GetTypeFromCLSID(clsid, null, throwOnError);
			}
	public static Type GetTypeFromCLSID(Guid clsid, string server)
			{
				return GetTypeFromCLSID(clsid, server, false);
			}
	public static Type GetTypeFromCLSID(Guid clsid)
			{
				return GetTypeFromCLSID(clsid, null, false);
			}
#endif // !ECMA_COMPAT

	// Get a type from a program identifier.
	//
	// This functionality is not supported, as it is
	// intended for implementing COM, which we don't have.
	public static Type GetTypeFromProgID(String progID, String server,
										 bool throwOnError)
			{
				if(throwOnError)
				{
					throw new TypeLoadException();
				}
				return null;
			}
	public static Type GetTypeFromProgID(String progID, bool throwOnError)
			{
				return GetTypeFromProgID(progID, null, throwOnError);
			}
	public static Type GetTypeFromProgID(String progID, string server)
			{
				return GetTypeFromProgID(progID, server, false);
			}
	public static Type GetTypeFromProgID(String progID)
			{
				return GetTypeFromProgID(progID, null, false);
			}

	// Implementation of the "HasElementType" property.
	protected abstract bool HasElementTypeImpl();

	// Invoke a member.
	public abstract Object InvokeMember
						(String name, BindingFlags invokeAttr,
					     Binder binder, Object target, Object[] args,
					     ParameterModifier[] modifiers,
					     CultureInfo culture, String[] namedParameters);
#if !ECMA_COMPAT
	[DebuggerStepThrough]
	[DebuggerHidden]
#endif
	public Object InvokeMember(String name, BindingFlags invokeAttr,
					     	   Binder binder, Object target, Object[] args)
			{
				return InvokeMember(name, invokeAttr, binder, target, args,
									null, null, null);
			}
#if !ECMA_COMPAT
	[DebuggerStepThrough]
	[DebuggerHidden]
#endif
	public Object InvokeMember(String name, BindingFlags invokeAttr,
					     	   Binder binder, Object target, Object[] args,
							   CultureInfo culture)
			{
				return InvokeMember(name, invokeAttr, binder, target, args,
									null, culture, null);
			}

	// Implementation of the "IsByRef" property.
	protected abstract bool IsByRefImpl();

	// Implementation of the "IsCOMObject" property.
	protected abstract bool IsCOMObjectImpl();

	// Implementation of the "IsContextful" property.
	protected virtual bool IsContextfulImpl()
			{
				return (typeof(ContextBoundObject)).IsAssignableFrom(this);
			}

	// Implementation of the "IsMarshalByRef" property.
	protected virtual bool IsMarshalByRefImpl()
			{
				return (typeof(MarshalByRefObject)).IsAssignableFrom(this);
			}

	// Abstract properties.
#if !ECMA_COMPAT
	public abstract Guid GUID { get; }
#endif
	public abstract System.Reflection.Module Module { get; }
	public abstract String Namespace { get; }
	public abstract Type UnderlyingSystemType { get; }

	// Implemented properties.
	public override Type DeclaringType { get { return this; } }
	public bool HasElementType { get { return HasElementTypeImpl(); } }
	public bool IsByRef { get { return IsByRefImpl(); } }
	public bool IsCOMObject { get { return IsCOMObjectImpl(); } }
	public bool IsContextful { get { return IsContextfulImpl(); } }
	public bool IsMarshalByRef { get { return IsMarshalByRefImpl(); } }
	public override Type ReflectedType { get { return this; } }
	public TypeAttributes Attributes
				{ get { return GetAttributeFlagsImpl(); } }

	// Test for various type attributes.
	public bool IsAbstract
			{
				get
				{
					return ((GetAttributeFlagsImpl() &
							 TypeAttributes.Abstract) != 0);
				}
			}
	public bool IsAnsiClass
			{
				get
				{
					return ((GetAttributeFlagsImpl() &
							 TypeAttributes.StringFormatMask) ==
							 	TypeAttributes.AnsiClass);
				}
			}
	public bool IsAutoClass
			{
				get
				{
					return ((GetAttributeFlagsImpl() &
							 TypeAttributes.StringFormatMask) ==
							 	TypeAttributes.AutoClass);
				}
			}
	public bool IsAutoLayout
			{
				get
				{
					return ((GetAttributeFlagsImpl() &
							 TypeAttributes.LayoutMask) ==
							 	TypeAttributes.AutoLayout);
				}
			}
	public bool IsExplicitLayout
			{
				get
				{
					return ((GetAttributeFlagsImpl() &
							 TypeAttributes.LayoutMask) ==
							 	TypeAttributes.ExplicitLayout);
				}
			}
	public bool IsImport
			{
				get
				{
					return ((GetAttributeFlagsImpl() &
							 TypeAttributes.Import) != 0);
				}
			}
	public bool IsLayoutSequential
			{
				get
				{
					return ((GetAttributeFlagsImpl() &
							 TypeAttributes.LayoutMask) ==
							 	TypeAttributes.SequentialLayout);
				}
			}
	public bool IsNestedAssembly
			{
				get
				{
					return ((GetAttributeFlagsImpl() &
							 TypeAttributes.VisibilityMask) ==
							 	TypeAttributes.NestedAssembly);
				}
			}
	public bool IsNestedFamANDAssem
			{
				get
				{
					return ((GetAttributeFlagsImpl() &
							 TypeAttributes.VisibilityMask) ==
							 	TypeAttributes.NestedFamANDAssem);
				}
			}
	public bool IsNestedFamORAssem
			{
				get
				{
					return ((GetAttributeFlagsImpl() &
							 TypeAttributes.VisibilityMask) ==
							 	TypeAttributes.NestedFamORAssem);
				}
			}
	public bool IsNestedFamily
			{
				get
				{
					return ((GetAttributeFlagsImpl() &
							 TypeAttributes.VisibilityMask) ==
							 	TypeAttributes.NestedFamily);
				}
			}
	public bool IsNestedPrivate
			{
				get
				{
					return ((GetAttributeFlagsImpl() &
							 TypeAttributes.VisibilityMask) ==
							 	TypeAttributes.NestedPrivate);
				}
			}
	public bool IsNestedPublic
			{
				get
				{
					return ((GetAttributeFlagsImpl() &
							 TypeAttributes.VisibilityMask) ==
							 	TypeAttributes.NestedPublic);
				}
			}
	public bool IsNotPublic
			{
				get
				{
					return ((GetAttributeFlagsImpl() &
							 TypeAttributes.VisibilityMask) ==
							 	TypeAttributes.NotPublic);
				}
			}
	public bool IsPublic
			{
				get
				{
					return ((GetAttributeFlagsImpl() &
							 TypeAttributes.VisibilityMask) ==
							 	TypeAttributes.Public);
				}
			}
	public bool IsSealed
			{
				get
				{
					return ((GetAttributeFlagsImpl() &
							 TypeAttributes.Sealed) != 0);
				}
			}
	public bool IsSerializable
			{
				get
				{
					return ((GetAttributeFlagsImpl() &
							 TypeAttributes.Serializable) != 0);
				}
			}
	public bool IsSpecialName
			{
				get
				{
					return ((GetAttributeFlagsImpl() &
							 TypeAttributes.SpecialName) != 0);
				}
			}
	public bool IsUnicodeClass
			{
				get
				{
					return ((GetAttributeFlagsImpl() &
							 TypeAttributes.StringFormatMask) ==
							 	TypeAttributes.UnicodeClass);
				}
			}

	// Get the type of this member.
	public override MemberTypes MemberType
			{
				get
				{
					return MemberTypes.TypeInfo;
				}
			}

	// Cached copy of the default binder.
	private static Binder defaultBinder;

	// Get the default binder in use by the system.
	public static Binder DefaultBinder
			{
				get
				{
					if(defaultBinder == null)
					{
						lock(typeof(Type))
						{
							if(defaultBinder == null)
							{
								defaultBinder = new DefaultBinder();
							}
						}
					}
					return defaultBinder;
				}
			}

	// Get the type initializer for this type.
	public ConstructorInfo TypeInitializer
			{
				get
				{
					return GetConstructorImpl(BindingFlags.Public |
											  BindingFlags.NonPublic |
											  BindingFlags.Static,
											  null,
											  CallingConventions.Any,
											  EmptyTypes,
											  null);
				}
			}

	// Support for generic types follows.  Not strictly speaking
	// ECMA-compatible, but will probably be ECMA eventually.

	// Determine if this type has generic arguments.
	protected virtual bool HasGenericArgumentsImpl()
			{
				return false;
			}

	// Determine if this type has generic parameters.
	protected virtual bool HasGenericParametersImpl()
			{
				return false;
			}

	// Get the type parameters that were used to instantiate this type.
	public virtual Type[] GetGenericArguments()
			{
				throw new NotSupportedException(_("NotSupp_NotGenericType"));
			}

	// Instantiate this generic type with a group of parameters.
	public virtual Type BindGenericParameters(Type[] inst)
			{
				throw new NotSupportedException(_("NotSupp_NotGenericType"));
			}

	// Get the generic type that underlies this instantiated type.
	public virtual Type GetGenericTypeDefinition()
			{
				throw new NotSupportedException(_("NotSupp_NotGenericType"));
			}

	// Properties that wrap up the above.
	public bool HasGenericArguments
			{
				get
				{
					return HasGenericArgumentsImpl();
				}
			}
	public bool HasGenericParameters
			{
				get
				{
					return HasGenericParametersImpl();
				}
			}

#endif // CONFIG_REFLECTION

}; // class Type

}; // namespace System
