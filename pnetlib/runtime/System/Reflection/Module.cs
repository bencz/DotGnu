/*
 * Module.cs - Implementation of the "System.Reflection.Module" class.
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

#if CONFIG_REFLECTION

using System;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;

public class Module : IClrProgramItem, ICustomAttributeProvider
#if CONFIG_SERIALIZATION
	, ISerializable
#endif
{
	// Internal state.
	internal IntPtr privateData;

	// Constructor.
	internal Module() {}

	// Implement the IClrProgramItem interface.
	IntPtr IClrProgramItem.ClrHandle
			{
				get
				{
					return privateData;
				}
			}

	// Get a field from the module type.
	public FieldInfo GetField(String name)
			{
				return GetModuleType().GetField(name);
			}
	public FieldInfo GetField(String name, BindingFlags bindingAttr)
			{
				return GetModuleType().GetField(name, bindingAttr);
			}

	// Get all module type fields that match a specific set of criteria.
	// Note: the first is required by ECMA, but doesn't exist in the SDK,
	// so you should avoid using it if at all possible.
	public FieldInfo[] GetFields(BindingFlags bindingAttr)
			{
				return GetModuleType().GetFields(bindingAttr);
			}
	public FieldInfo[] GetFields()
			{
				return GetModuleType().GetFields();
			}

	// Get a method from the module type.
	public MethodInfo GetMethod(String name)
			{
				return GetMethodImpl(name, BindingFlags.Default,
									 null, CallingConventions.Any,
									 new Type [0], null);
			}
	public MethodInfo GetMethod(String name, Type[] types)
			{
				return GetMethodImpl(name, BindingFlags.Default,
									 null, CallingConventions.Any,
									 types, null);
			}
#if !ECMA_COMPAT
	public MethodInfo GetMethod(String name,
							    BindingFlags bindingAttr, Binder binder,
								CallingConventions callingConventions,
								Type[] types, ParameterModifier[] modifiers)
			{
				return GetMethodImpl(name, bindingAttr, binder,
									 callingConventions, types, modifiers);
			}
#endif // !ECMA_COMPAT

	// Get a method implementation that matches a specific set of criteria.
#if ECMA_COMPAT
	internal
#endif
	protected virtual MethodInfo GetMethodImpl
				(String name, BindingFlags bindingAttr, Binder binder,
				 CallingConventions callingConventions,
				 Type[] types, ParameterModifier[] modifiers)
			{
				return GetModuleType().GetMethod
						(name, bindingAttr, binder,
						 callingConventions, types, modifiers);
			}

	// Get all module type methods that match a specific set of criteria.
	// Note: the first is required by ECMA, but doesn't exist in the SDK,
	// so you should avoid using it if at all possible.
	public MethodInfo[] GetMethods(BindingFlags bindingAttr)
			{
				return GetModuleType().GetMethods(bindingAttr);
			}
	public MethodInfo[] GetMethods()
			{
				return GetModuleType().GetMethods();
			}

	// Convert the module into a string.
	public override String ToString()
			{
				return Name;
			}

#if CONFIG_RUNTIME_INFRA
	// Get the assembly that contains this module.
	public System.Reflection.Assembly Assembly
			{
				get
				{
					return GetAssemblyCore();
				}
			}

	// Workaround the fact that the Assembly property is non-virtual.
	internal virtual System.Reflection.Assembly GetAssemblyCore()
			{
				return GetAssembly();
			}
#endif

	// Get the fully-qualified name for this module.
	public virtual String FullyQualifiedName
			{
				get
				{
					return GetFullName();
				}
			}

	// Get the name of this module.
	public String Name
			{
				get
				{
					return ClrHelpers.GetName(privateData);
				}
			}

#if !ECMA_COMPAT

	// Get the scope name for this module.
	public String ScopeName
			{
				get
				{
					return ClrHelpers.GetName(privateData);
				}
			}

	// Get custom attributes for this module.
	public virtual Object[] GetCustomAttributes(bool inherit)
			{
				return ClrHelpers.GetCustomAttributes(this, inherit);
			}
	public virtual Object[] GetCustomAttributes(Type type, bool inherit)
			{
				return ClrHelpers.GetCustomAttributes(this, type, inherit);
			}
	
	// Determine if a custom attribute is defined for this module.
	public virtual bool IsDefined(Type type, bool inherit)
			{
				return ClrHelpers.IsDefined(this, type, inherit);
			}

	// Get a type from this module.
	public virtual Type GetType(String name)
			{
				return GetType(name, false, false);
			}
	public virtual Type GetType(String name, bool ignoreCase)
			{
				return GetType(name, false, ignoreCase);
			}
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public virtual Type GetType(String name, bool throwOnError,
									   bool ignoreCase);

	// Return all types that are defined in this module.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public virtual Type[] GetTypes();

	// Determine if this module is a resource.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public bool IsResource();

#if CONFIG_X509_CERTIFICATES

	// Get the signer certificate for this module.
	public X509Certificate GetSignerCertificate()
			{
				// Not used in this implementation.
				return null;
			}

#endif

#if CONFIG_SERIALIZATION

	// Get the serialization data for this module.
	public virtual void GetObjectData(SerializationInfo info,
							  		  StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				UnitySerializationHolder.Serialize
					(info, UnitySerializationHolder.UnityType.Module,
					 ScopeName, Assembly);
			}

#endif // CONFIG_SERIALIZATION

	// A type filter that searches on name.
	private static bool FilterTypeNameImpl(Type type, Object criteria)
			{
				String name = (criteria as String);
				if(name != null)
				{
					if(name.EndsWith("*"))
					{
						return type.FullName.StartsWith
							(name.Substring(0, name.Length - 1));
					}
					else
					{
						return (type.FullName == name);
					}
				}
				return false;
			}

	// A type filter that searches on name while ignoring case.
	private static bool FilterTypeNameIgnoreCaseImpl
				(Type type, Object criteria)
			{
				String name = (criteria as String);
				if(name != null)
				{
					if(name.EndsWith("*"))
					{
						name = name.Substring(0, name.Length - 1);
						String fullName = type.FullName;
						if(fullName.Length < name.Length)
						{
							return false;
						}
						return (String.Compare(fullName, 0, name, 0,
											   name.Length, true) == 0);
					}
					else
					{
						return (String.Compare(type.FullName, name, true)
									== 0);
					}
				}
				return false;
			}

	// Declare the standard type filters.
	public static readonly TypeFilter FilterTypeName =
			new TypeFilter(FilterTypeNameImpl);
	public static readonly TypeFilter FilterTypeNameIgnoreCase =
			new TypeFilter(FilterTypeNameIgnoreCaseImpl);

	// Find all types that match a specified filter criteria.
	public virtual Type[] FindTypes(TypeFilter filter,
									Object filterCriteria)
			{
				Type[] types = GetTypes();
				if(filter == null || types == null)
				{
					return types;
				}
				ArrayList list = new ArrayList();
				foreach(Type type in types)
				{
					if(filter(type, filterCriteria))
					{
						list.Add(type);
					}
				}
				return (Type[])(list.ToArray(typeof(Type)));
			}

#else // ECMA_COMPAT

	// Get custom attributes for this module.
	Object[] ICustomAttributeProvider.GetCustomAttributes(bool inherit)
			{
				return ClrHelpers.GetCustomAttributes(this, inherit);
			}
	Object[] ICustomAttributeProvider.GetCustomAttributes
					(Type type, bool inherit)
			{
				return ClrHelpers.GetCustomAttributes(this, type, inherit);
			}
	
	// Determine if a custom attribute is defined for this module.
	bool ICustomAttributeProvider.IsDefined(Type type, bool inherit)
			{
				return ClrHelpers.IsDefined(this, type, inherit);
			}

#endif // ECMA_COMPAT

	// Get the module type.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal virtual Type GetModuleType();

	// Get the assembly that contains this module.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private Assembly GetAssembly();

	// Get the fully-qualified name for this module.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private String GetFullName();

}; // class Module

#endif // CONFIG_REFLECTION

}; // namespace System.Reflection
