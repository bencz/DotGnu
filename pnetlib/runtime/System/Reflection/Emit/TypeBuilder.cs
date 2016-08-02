/*
 * TypeBuilder.cs - Implementation of the
 *		"System.Reflection.Emit.TypeBuilder" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System.Reflection.Emit
{

#if CONFIG_REFLECTION_EMIT

using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;

public sealed class TypeBuilder : Type, IClrProgramItem, IDetachItem
{
	// Internal state.
	private IntPtr privateData;			// Must be the first field.
	internal ModuleBuilder module;
	private String name;
	private String nspace;
	internal TypeAttributes attr;
	private Type parent;
	private Type[] interfaces;
	private TypeBuilder declaringType;
	private ClrType type;
	private Type underlyingSystemType;
	private ArrayList methods;
	internal bool needsDefaultConstructor;

	// Constants.
	public const int UnspecifiedTypeSize = 0;

	// Constructor.
	internal TypeBuilder(ModuleBuilder module, String name, String nspace,
						 TypeAttributes attr, Type parent, Type[] interfaces,
						 PackingSize packingSize, int typeSize,
						 TypeBuilder declaringType)
			{
				// Validate the parameters.
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				else if(name == String.Empty)
				{
					throw new ArgumentException(_("Emit_NameEmpty"));
				}
				if(nspace == null)
				{
					nspace = String.Empty;
				}

				// Initialize the internal state.
				this.module = module;
				this.name = name;
				this.nspace = nspace;
				this.attr = attr;
				this.parent = parent;
				this.interfaces = null;
				this.declaringType = declaringType;
				this.type = null;
				this.underlyingSystemType = null;
				this.methods = new ArrayList();
				this.needsDefaultConstructor = true;

				// We need the AssemblyBuilder lock for the next part.
				lock(typeof(AssemblyBuilder))
				{
					// Determine the scope to use to declare the type.
					IntPtr scope;
					if(declaringType == null)
					{
						scope = IntPtr.Zero;
					}
					else
					{
						scope = ((IClrProgramItem)declaringType).ClrHandle;
					}

					// Create the type.
					privateData = ClrTypeCreate
						(((IClrProgramItem)module).ClrHandle, scope, name,
					 	(nspace == String.Empty ? null : nspace), attr,
					 	(parent == null
							? new System.Reflection.Emit.TypeToken(0)
					 		: module.GetTypeToken(parent)));
					if(privateData == IntPtr.Zero)
					{
						throw new ArgumentException
							(_("Emit_TypeAlreadyExists"));
					}
					module.assembly.AddDetach(this);
					if(packingSize != PackingSize.Unspecified)
					{
						ClrTypeSetPackingSize(privateData, (int)packingSize);
					}
					if(typeSize != UnspecifiedTypeSize)
					{
						ClrTypeSetClassSize(privateData, typeSize);
					}
				}

				// Add the interfaces to the type.
				if(interfaces != null)
				{
					foreach(Type iface in interfaces)
					{
						AddInterfaceImplementation(iface);
					}
				}
			}

	// Get the attribute flags for this type.
	protected override TypeAttributes GetAttributeFlagsImpl()
			{
				return attr;
			}

	// Get the assembly associated with this type.
	public override Assembly Assembly
			{
				get
				{
					return module.Assembly;
				}
			}

	// Get the full qualified assembly name of this type.
	public override String AssemblyQualifiedName
			{
				get
				{
					return Assembly.CreateQualifiedName
						(module.Assembly.FullName, name);
				}
			}

	// Get the base type for this type.
	public override Type BaseType
			{
				get
				{
					return parent;
				}
			}

	// Get the declaring type for this type.
	public override Type DeclaringType
			{
				get
				{
					return declaringType;
				}
			}

	// Get the full name of this type.
	public override String FullName
			{
				get
				{
					if(declaringType != null)
					{
						return declaringType.FullName + "+" + name;
					}
					else if(nspace != null)
					{
						return nspace + "." + name;
					}
					else
					{
						return name;
					}
				}
			}

	// Get the GUID of this type.
	public override Guid GUID
			{
				get
				{
					throw new NotSupportedException(_("NotSupp_Builder"));
				}
			}

	// Retrieve the module that this type is defined within.
	public override Module Module
			{
				get
				{
					return module;
				}
			}

	// Get the name of this type.
	public override String Name
			{
				get
				{
					return name;
				}
			}

	// Get the namespace of this type.
	public override String Namespace
			{
				get
				{
					return nspace;
				}
			}

	// Get the packing size of this type.
	public PackingSize PackingSize
			{
				get
				{
					lock(typeof(AssemblyBuilder))
					{
						return (PackingSize)
							(ClrTypeGetPackingSize(privateData));
					}
				}
			}

	// Get the reflected type for this type.
	public override Type ReflectedType
			{
				get
				{
					return declaringType;
				}
			}

	// Get the total size of this type.
	public int Size
			{
				get
				{
					lock(typeof(AssemblyBuilder))
					{
						return ClrTypeGetClassSize(privateData);
					}
				}
			}

	// Get the type handle for this type.
	public override RuntimeTypeHandle TypeHandle
			{
				get
				{
					throw new NotSupportedException(_("NotSupp_Builder"));
				}
			}

	// Get the type token for this type.
	public TypeToken TypeToken
			{
				get
				{
					lock(typeof(AssemblyBuilder))
					{
						return new System.Reflection.Emit.TypeToken
							(AssemblyBuilder.ClrGetItemToken(privateData));
					}
				}
			}

	// Get the underlying system type for this type.
	public override Type UnderlyingSystemType
			{
				get
				{
					if(type != null)
					{
						return type.UnderlyingSystemType;
					}
					else if(IsEnum)
					{
						if(underlyingSystemType != null)
						{
							return underlyingSystemType;
						}
						else
						{
							throw new InvalidOperationException
								(_("Emit_UnderlyingNotSet"));
						}
					}
					return this;
				}
			}

	// Check that the type has not yet been created.
	internal void CheckNotCreated()
			{
				if(type != null)
				{
					throw new NotSupportedException(_("NotSupp_TypeCreated"));
				}
			}

	// Check that the type has been created.
	internal void CheckCreated()
			{
				if(type == null)
				{
					throw new NotSupportedException
						(_("NotSupp_TypeNotCreated"));
				}
			}

	// Start a synchronized type builder operation.
	internal void StartSync()
			{
				module.StartSync();
				if(type != null)
				{
					throw new NotSupportedException(_("NotSupp_TypeCreated"));
				}
			}

	// End a synchronized type builder operation.
	internal void EndSync()
			{
				module.EndSync();
			}

	// Add declarative security to this type.
	public void AddDeclarativeSecurity(SecurityAction action,
									   PermissionSet pset)
			{
				try
				{
					StartSync();
					module.assembly.AddDeclarativeSecurity
						(this, action, pset);
				}
				finally
				{
					EndSync();
				}
			}

	// Add an interface to this type's implementation list.
	public void AddInterfaceImplementation(Type interfaceType)
			{
				try
				{
					// Start builder synchronization.
					StartSync();

					// Validate the parameters.
					if(interfaceType == null)
					{
						throw new ArgumentNullException("interfaceType");
					}

					// Bail out if the supplied parameter is not an interface.
					// We should probably throw an exception, but MS doesn't.
					if(!interfaceType.IsInterface)
					{
						return;
					}

					// Bail out if this type is inherited by the interface
					// so that we cannot create circular class structures.
					if(IsAssignableFrom(interfaceType))
					{
						return;
					}

					// Determine if we already have this interface.
					if(interfaces != null)
					{
						int index;
						for(index = 0; index < interfaces.Length; ++index)
						{
							if(interfaceType.Equals(interfaces[index]))
							{
								return;
							}
						}
					}

					// Convert the interface into a token, which may throw
					// an exception if it cannot be imported.
					TypeToken token = module.GetTypeToken(interfaceType);

					// Add the interface to the list.
					Type[] newInterfaces;
					if(interfaces != null)
					{
						newInterfaces = new Type [interfaces.Length + 1];
						Array.Copy(interfaces, newInterfaces,
								   interfaces.Length);
						newInterfaces[interfaces.Length] = interfaceType;
					}
					else
					{
						newInterfaces = new Type [1];
						newInterfaces[0] = interfaceType;
					}
					interfaces = newInterfaces;

					// Call the runtime engine to add the interface.
					lock(typeof(AssemblyBuilder))
					{
						ClrTypeAddInterface(privateData, token);
					}
				}
				finally
				{
					EndSync();
				}
			}

	// Create this type.
	public Type CreateType()
			{
				try
				{
					// Synchronize access and make sure we aren't created.
					StartSync();

					// If nested, the nesting parent must be created first.
					if(declaringType != null)
					{
						if(declaringType.type == null)
						{
							throw new InvalidOperationException
								(_("Emit_NestingParentNotCreated"));
						}
					}

					// Define a default constructor if necessary.
					if(needsDefaultConstructor && !IsInterface && !IsValueType)
					{
						if(IsAbstract)
						{
							DefineDefaultConstructor(MethodAttributes.Family);
						}
						else
						{
							DefineDefaultConstructor(MethodAttributes.Public);
						}
					}

					// Finalize the methods and constructors.
					MethodBuilder mb;
					foreach(MethodBase method in methods)
					{
						mb = (method as MethodBuilder);
						if(mb != null)
						{
							mb.FinalizeMethod();
						}
						else
						{
							((ConstructorBuilder)method).FinalizeConstructor();
						}
					}

					// Wrap "privateData" in a "ClrType" object and return it.
					lock(typeof(AssemblyBuilder))
					{
						if(privateData == IntPtr.Zero)
						{
							throw new InvalidOperationException
								(_("Emit_TypeInvalid"));
						}
						ClrType clrType = new ClrType();
						clrType.privateData = privateData;
						type = clrType;
						return type;
					}
				}
				finally
				{
					EndSync();
				}
			}

	// Define a constructor for this class.
	public ConstructorBuilder DefineConstructor
				(MethodAttributes attributes,
				 CallingConventions callingConvention,
				 Type[] parameterTypes)
			{
				try
				{
					StartSync();
					String name;
					if((attributes & MethodAttributes.Static) != 0)
					{
						name = ".cctor";
					}
					else
					{
						name = ".ctor";
					}
					attributes |= MethodAttributes.SpecialName;
					ConstructorBuilder con = new ConstructorBuilder
						(this, name, attributes,
						 callingConvention, parameterTypes);
					needsDefaultConstructor = false;
					return con;
				}
				finally
				{
					EndSync();
				}
			}

	// Define a default constructor for this class.
	public ConstructorBuilder DefineDefaultConstructor
				(MethodAttributes attributes)
			{
				return DefineConstructor(attributes,
										 CallingConventions.Standard,
										 null);
			}

	// Define an event for this class.
	public EventBuilder DefineEvent
				(String name, EventAttributes attributes, Type eventType)
			{
				try
				{
					StartSync();
					return new EventBuilder(this, name, attributes, eventType);
				}
				finally
				{
					EndSync();
				}
			}

	// Define a field for this class.
	public FieldBuilder DefineField
				(String name, Type type, FieldAttributes attributes)
			{
				try
				{
					StartSync();
					if(IsEnum && underlyingSystemType == null &&
					   (attributes & FieldAttributes.Static) == 0)
					{
						underlyingSystemType = type;
					}
					return new FieldBuilder(this, name, type, attributes);
				}
				finally
				{
					EndSync();
				}
			}

	// Define a data field within this class.
	private FieldBuilder DefineData(String name, byte[] data,
								    int size, FieldAttributes attributes)
			{
				// Validate the parameters.
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				else if(name == String.Empty)
				{
					throw new ArgumentException(_("Emit_NameEmpty"));
				}
				else if(size <= 0 || size > 0x003EFFFF)
				{
					throw new ArgumentException(_("Emit_DataSize"));
				}

				// We must not have created the type yet.
				CheckNotCreated();

				// Look for or create a value type for the field.
				String typeName = "$ArrayType$" + size.ToString();
				Type type = module.GetType(typeName);
				if(type == null)
				{
					TypeBuilder builder;
					builder = module.DefineType
							(typeName,
							 TypeAttributes.Public |
							 TypeAttributes.Sealed |
							 TypeAttributes.ExplicitLayout,
							 typeof(System.ValueType),
							 PackingSize.Size1, size);
					type = builder.CreateType();
				}

				// Define the field and set the data on it.
				FieldBuilder field = DefineField
					(name, type, attributes | FieldAttributes.Static);
				field.SetData(data, size);
				return field;
			}

	// Define static initialized data within this class.
	public FieldBuilder DefineInitializedData
				(String name, byte[] data, FieldAttributes attributes)
			{
				try
				{
					StartSync();
					if(data == null)
					{
						throw new ArgumentNullException("data");
					}
					return DefineData(name, data, data.Length, attributes);
				}
				finally
				{
					EndSync();
				}
			}

	// Define a method for this class.
	public MethodBuilder DefineMethod
				(String name, MethodAttributes attributes,
				 CallingConventions callingConvention,
				 Type returnType, Type[] parameterTypes)
			{
				try
				{
					StartSync();
					return new MethodBuilder
						(this, name, attributes, callingConvention,
						 returnType, parameterTypes);
				}
				finally
				{
					EndSync();
				}
			}
	public MethodBuilder DefineMethod
				(String name, MethodAttributes attributes,
				 Type returnType, Type[] parameterTypes)
			{
				return DefineMethod(name, attributes,
									CallingConventions.Standard,
									returnType, parameterTypes);
			}

	// Define a method override declaration for this class.
	public void DefineMethodOverride
				(MethodInfo methodInfoBody, MethodInfo methodInfoDeclaration)
			{
				try
				{
					StartSync();
					
					// Validate the parameters.
					if(methodInfoBody == null)
					{
						throw new ArgumentNullException("methodInfoBody");
					}
					if(methodInfoDeclaration == null)
					{
						throw new ArgumentNullException
							("methodInfoDeclaration");
					}
					if(methodInfoBody.DeclaringType != this)
					{
						throw new ArgumentException
							(_("Emit_OverrideBodyNotInType"));
					}
					MethodToken bodyToken = module.GetMethodToken
						(methodInfoBody);
					MethodToken declToken = module.GetMethodToken
						(methodInfoDeclaration);
					lock(typeof(AssemblyBuilder))
					{
						ClrTypeAddOverride
							(module.privateData,
							 bodyToken.Token, declToken.Token);
					}
				}
				finally
				{
					EndSync();
				}
			}

	// Define a nested type within this class.
	private TypeBuilder DefineNestedType
				(String name, TypeAttributes attr, Type parent,
				 Type[] interfaces, int typeSize, PackingSize packingSize)
			{
				try
				{
					StartSync();
					return new TypeBuilder(module,
										   name, null, attr, parent,
										   interfaces, packingSize,
										   typeSize, this);
				}
				finally
				{
					EndSync();
				}
			}
	public TypeBuilder DefineNestedType(String name)
			{
				return DefineNestedType(name,
									    TypeAttributes.NestedPrivate,
										null, null, 0,
										PackingSize.Unspecified);
			}
	public TypeBuilder DefineNestedType(String name, TypeAttributes attr)
			{
				return DefineNestedType(name, attr, null, null, 0,
										PackingSize.Unspecified);
			}
	public TypeBuilder DefineNestedType(String name, TypeAttributes attr,
										Type parent)
			{
				return DefineNestedType(name, attr, parent, null, 0,
										PackingSize.Unspecified);
			}
	public TypeBuilder DefineNestedType(String name, TypeAttributes attr,
										Type parent, int typeSize)
			{
				return DefineNestedType(name, attr, parent, null, typeSize,
										PackingSize.Unspecified);
			}
	public TypeBuilder DefineNestedType(String name, TypeAttributes attr,
										Type parent, PackingSize packSize)
			{
				return DefineNestedType(name, attr, parent, null, 0, packSize);
			}
	public TypeBuilder DefineNestedType(String name, TypeAttributes attr,
										Type parent, Type[] interfaces)
			{
				return DefineNestedType(name, attr, parent, interfaces,
										0, PackingSize.Unspecified);
			}

	// Define a PInvoke method for this class.
	public MethodBuilder DefinePInvokeMethod
				(String name, String dllName, String entryName,
				 MethodAttributes attributes,
				 CallingConventions callingConvention,
				 Type returnType, Type[] parameterTypes,
				 CallingConvention nativeCallConv,
				 CharSet nativeCharSet)
			{
				try
				{
					// Lock down the assembly while we do this.
					StartSync();

					// Validate the parameters.
					if(name == null)
					{
						throw new ArgumentNullException("name");
					}
					if(name == String.Empty)
					{
						throw new ArgumentException(_("Emit_NameEmpty"));
					}
					if(dllName == null)
					{
						throw new ArgumentNullException("dllName");
					}
					if(dllName == String.Empty)
					{
						throw new ArgumentException(_("Emit_NameEmpty"));
					}
					if(entryName == null)
					{
						throw new ArgumentNullException("entryName");
					}
					if(entryName == String.Empty)
					{
						throw new ArgumentException(_("Emit_NameEmpty"));
					}
					if((type.Attributes & TypeAttributes.ClassSemanticsMask)
							== TypeAttributes.Interface)
					{
						throw new ArgumentException
							(_("Emit_PInvokeInInterface"));
					}
					if((attributes & MethodAttributes.Abstract) != 0)
					{
						throw new ArgumentException
							(_("Emit_PInvokeAbstract"));
					}

					// Create the underlying method.
					MethodBuilder method = new MethodBuilder
							(this, name,
							 attributes | MethodAttributes.PinvokeImpl,
							 callingConvention, returnType, parameterTypes);

					// Build the attributes for the PInvoke declaration.
					int pinvAttrs = (((int)nativeCallConv) << 8);
					switch(nativeCharSet)
					{
						case CharSet.Ansi:		pinvAttrs |= 0x0002; break;
						case CharSet.Unicode:	pinvAttrs |= 0x0004; break;
						case CharSet.Auto:		pinvAttrs |= 0x0006; break;
						default:				break;
					}

					// Create the PInvoke declaration on the method.
					if(entryName == name)
					{
						entryName = null;
					}
					lock(typeof(AssemblyBuilder))
					{
						MethodBuilder.ClrMethodAddPInvoke
							(((IClrProgramItem)method).ClrHandle,
							 pinvAttrs, dllName, entryName);
					}

					// Return the method to the caller.
					return method;
				}
				finally
				{
					EndSync();
				}
			}
	public MethodBuilder DefinePInvokeMethod
				(String name, String dllName,
				 MethodAttributes attributes,
				 CallingConventions callingConvention,
				 Type returnType, Type[] parameterTypes,
				 CallingConvention nativeCallConv,
				 CharSet nativeCharSet)
			{
				return DefinePInvokeMethod(name, dllName, name,
										   attributes, callingConvention,
										   returnType, parameterTypes,
										   nativeCallConv, nativeCharSet);
			}

	// Define a property for this class.
	public PropertyBuilder DefineProperty
				(String name, PropertyAttributes attributes,
				 Type returnType, Type[] parameterTypes)
			{
				try
				{
					StartSync();
					return new PropertyBuilder
						(this, name, attributes, returnType, parameterTypes);
				}
				finally
				{
					EndSync();
				}
			}

	// Define a type initializer for this class.
	public ConstructorBuilder DefineTypeInitializer()
			{
				return DefineConstructor(MethodAttributes.Private |
										 MethodAttributes.Static |
										 MethodAttributes.SpecialName,
										 CallingConventions.Standard,
										 null);
			}

	// Define uninitialized data within this class.
	public FieldBuilder DefineUninitializedData
				(String name, int size, FieldAttributes attributes)
			{
				try
				{
					StartSync();
					return DefineData(name, null, size, attributes);
				}
				finally
				{
					EndSync();
				}
			}

	// Implementation of "GetConstructor" provided by subclasses.
	protected override ConstructorInfo
					GetConstructorImpl(BindingFlags bindingAttr,
								       Binder binder,
								       CallingConventions callingConventions,
								       Type[] types,
								       ParameterModifier[] modifiers)
			{
				CheckCreated();
				return type.GetConstructor(bindingAttr, binder,
										   callingConventions,
										   types, modifiers);
			}

	// Get all constructors for this type.
	public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
			{
				CheckCreated();
				return type.GetConstructors(bindingAttr);
			}

	// Get the custom attributes that are associated with this member.
	public override Object[] GetCustomAttributes(bool inherit)
			{
				CheckCreated();
				return type.GetCustomAttributes(inherit);
			}
	public override Object[] GetCustomAttributes(Type type, bool inherit)
			{
				CheckCreated();
				return this.type.GetCustomAttributes(type, inherit);
			}

	// Determine if custom attributes are defined for this member.
	public override bool IsDefined(Type type, bool inherit)
			{
				CheckCreated();
				return this.type.IsDefined(type, inherit);
			}

	// Get the element type.
	public override Type GetElementType()
			{
				throw new NotSupportedException(_("NotSupp_Builder"));
			}

	// Get an event from this type.
	public override EventInfo GetEvent(String name, BindingFlags bindingAttr)
			{
				CheckCreated();
				return type.GetEvent(name, bindingAttr);
			}

	// Get the list of all events within this type.
	public override EventInfo[] GetEvents()
			{
				CheckCreated();
				return type.GetEvents();
			}
	public override EventInfo[] GetEvents(BindingFlags bindingAttr)
			{
				CheckCreated();
				return type.GetEvents(bindingAttr);
			}

	// Get a field from this type.
	public override FieldInfo GetField(String name, BindingFlags bindingAttr)
			{
				CheckCreated();
				return type.GetField(name, bindingAttr);
			}

	// Get the list of all fields within this type.
	public override FieldInfo[] GetFields(BindingFlags bindingAttr)
			{
				CheckCreated();
				return type.GetFields(bindingAttr);
			}

	// Get an interface from within this type.
	public override Type GetInterface(String name, bool ignoreCase)
			{
				CheckCreated();
				return type.GetInterface(name, ignoreCase);
			}

	// Get an interface mapping for this type.
	public override InterfaceMapping GetInterfaceMap(Type interfaceType)
			{
				CheckCreated();
				return type.GetInterfaceMap(interfaceType);
			}

	// Get the list of all interfaces that are implemented by this type.
	public override Type[] GetInterfaces()
			{
				if(type != null)
				{
					return type.GetInterfaces();
				}
				else if(interfaces == null)
				{
					return new Type [0];
				}
				else
				{
					return (Type[])(interfaces.Clone());
				}
			}

	// Get a list of members that have a specific name.
	public override MemberInfo[] GetMember
				(String name, MemberTypes type, BindingFlags bindingAttr)
			{
				CheckCreated();
				return this.type.GetMember(name, type, bindingAttr);
			}

	// Get a list of all members in this type.
	public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
			{
				CheckCreated();
				return type.GetMembers(bindingAttr);
			}

	// Implementation of "GetMethod".
	protected override MethodInfo GetMethodImpl
				(String name, BindingFlags bindingAttr,
				 Binder binder, CallingConventions callConvention,
				 Type[] types, ParameterModifier[] modifiers)
			{
				CheckCreated();
				return type.GetMethod(name, bindingAttr, binder,
									  callConvention, types, modifiers);
			}

	// Get a list of all methods in this type.
	public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
			{
				CheckCreated();
				return type.GetMethods(bindingAttr);
			}

	// Get a nested type that is contained within this type.
	public override Type GetNestedType(String name, BindingFlags bindingAttr)
			{
				CheckCreated();
				return type.GetNestedType(name, bindingAttr);
			}

	// Get a list of all nested types in this type.
	public override Type[] GetNestedTypes(BindingFlags bindingAttr)
			{
				CheckCreated();
				return type.GetNestedTypes(bindingAttr);
			}

	// Get a list of all properites in this type.
	public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
			{
				CheckCreated();
				return type.GetProperties(bindingAttr);
			}

	// Get a specific property from within this type.
	protected override PropertyInfo GetPropertyImpl
				(String name, BindingFlags bindingAttr, Binder binder,
				 Type returnType, Type[] types, ParameterModifier[] modifiers)
			{
				CheckCreated();
				return type.GetProperty(name, bindingAttr, binder,
										returnType, types, modifiers);
			}

	// Determine if this type has an element type.
	protected override bool HasElementTypeImpl()
			{
				throw new NotSupportedException(_("NotSupp_Builder"));
			}

	// Invoke a member of this type.
	public override Object InvokeMember
				(String name, BindingFlags invokeAttr, Binder binder,
				 Object target, Object[] args, ParameterModifier[] modifiers,
				 CultureInfo culture, String[] namedParameters)
			{
				CheckCreated();
				return type.InvokeMember(name, invokeAttr, binder,
										 target, args, modifiers,
										 culture, namedParameters);
			}

	// Determine if this type is an array.
	protected override bool IsArrayImpl()
			{
				return false;
			}

	// Determine if this type is assignable from another type.
	[TODO]
	public override bool IsAssignableFrom(Type c)
			{
				// TODO
				return base.IsAssignableFrom(c);
			}

	// Determine if this type is a "by reference" type.
	protected override bool IsByRefImpl()
			{
				return false;
			}
	
	// Determine if this type imports a COM type.
	protected override bool IsCOMObjectImpl()
			{
				return false;
			}

	// Determine if this is a pointer type.
	protected override bool IsPointerImpl()
			{
				return false;
			}

	// Determine if this is a primitive type.
	protected override bool IsPrimitiveImpl()
			{
				return false;
			}

	// Determine if this type is a subclass of "c".
	public override bool IsSubclassOf(Type c)
			{
				if(c == null)
				{
					// Can never be a subclass of "null".
					return false;
				}
				else if(type != null)
				{
					// We have been created, so use the underlying type.
					if(c is TypeBuilder)
					{
						Type otherType = ((TypeBuilder)c).type;
						if(otherType != null)
						{
							return type.IsSubclassOf(otherType);
						}
						else
						{
							return false;
						}
					}
					else
					{
						return type.IsSubclassOf(c);
					}
				}
				else
				{
					// This type isn't created yet, so scan up the
					// tree looking for the specified type.
					if(this == c || parent == c)
					{
						return true;
					}
					else if(parent != null)
					{
						return parent.IsSubclassOf(c);
					}
					else
					{
						return false;
					}
				}
			}

	// Set a custom attribute on this type builder.
	public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
			{
				try
				{
					StartSync();
					module.assembly.SetCustomAttribute(this, customBuilder);
				}
				finally
				{
					EndSync();
				}
			}
	public void SetCustomAttribute(ConstructorInfo con,
								   byte[] binaryAttribute)
			{
				try
				{
					StartSync();
					module.assembly.SetCustomAttribute
						(this, con, binaryAttribute);
				}
				finally
				{
					EndSync();
				}
			}

	// Set the parent of this type.
	public void SetParent(Type parent)
			{
				if(parent == null)
				{
					throw new ArgumentNullException("parent");
				}
				try
				{
					StartSync();
					lock(typeof(AssemblyBuilder))
					{
						ClrTypeSetParent
							(privateData, module.GetTypeToken(parent));
					}
					this.parent = parent;
				}
				finally
				{
					EndSync();
				}
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return name;
			}

	// Implement the IClrProgramItem interface.
	IntPtr IClrProgramItem.ClrHandle
			{
				get
				{
					return privateData;
				}
			}

	// Add a method to this type for later processing during "CreateType".
	internal void AddMethod(MethodBase method)
			{
				methods.Add(method);
			}

	// Detach this item.
	void IDetachItem.Detach()
			{
				privateData = IntPtr.Zero;
			}

	// Create a new type.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static IntPtr ClrTypeCreate
			(IntPtr module, IntPtr nestedParent, String name, String nspace,
			 TypeAttributes attr, TypeToken parent);

	// Set the parent of a type to a new value.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void ClrTypeSetParent
			(IntPtr classInfo, TypeToken parent);

	// Add an interface to a type.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void ClrTypeAddInterface
			(IntPtr classInfo, TypeToken iface);

	// Get the packing size for a type.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static int ClrTypeGetPackingSize(IntPtr classInfo);

	// Set the packing size for a type.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void ClrTypeSetPackingSize
			(IntPtr classInfo, int packingSize);

	// Get the class size for a type.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static int ClrTypeGetClassSize(IntPtr classInfo);

	// Set the class size for a type.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void ClrTypeSetClassSize
			(IntPtr classInfo, int classSize);

	// Import a class information structure into a module.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static int ClrTypeImport
			(IntPtr module, IntPtr classInfo);

	// Import a member information structure into a module.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static int ClrTypeImportMember
			(IntPtr module, IntPtr memberInfo);

	// Add an override declaration.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void ClrTypeAddOverride
			(IntPtr module, int bodyToken, int declToken);

}; // class TypeBuilder

#endif // CONFIG_REFLECTION_EMIT

}; // namespace System.Reflection.Emit
