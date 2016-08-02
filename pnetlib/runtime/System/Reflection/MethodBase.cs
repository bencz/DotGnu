/*
 * MethodBase.cs - Implementation of the "System.Reflection.MethodBase" class.
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

namespace System.Reflection
{

#if CONFIG_REFLECTION

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Diagnostics;

#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDual)]
#if CONFIG_FRAMEWORK_1_2 && CONFIG_REFLECTION
[ComDefaultInterface(typeof(_MethodBase))]
#endif
#endif
public abstract class MethodBase : MemberInfo
#if CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2
	, _MethodBase
#endif
{

	// Constructor.
	protected MethodBase() : base() {}

#if CONFIG_RUNTIME_INFRA
	// Get a method from the runtime engine given its handle.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static MethodBase GetMethodFromHandle
				(RuntimeMethodHandle handle);
#endif

	// Get the parameters for this method.
	public abstract ParameterInfo[] GetParameters();

	// Invoke this method.
#if !ECMA_COMPAT
	[DebuggerStepThrough]
	[DebuggerHidden]
#endif
	public Object Invoke(Object obj, Object[] parameters)
			{
				return Invoke(obj, BindingFlags.Default,
							  null, parameters, null);
			}
	public abstract Object Invoke(Object obj, BindingFlags invokeAttr,
								  Binder binder, Object[] parameters,
								  CultureInfo culture);

	// Get the method attributes.
	public abstract MethodAttributes Attributes { get; }

#if !ECMA_COMPAT

	// Get the runtime method handle associated with this method.
	public abstract RuntimeMethodHandle MethodHandle { get; }

#else  // ECMA_COMPAT

	// Get the runtime method handle associated with this method.
	internal virtual RuntimeMethodHandle MethodHandle
			{
				get
				{
					return new RuntimeMethodHandle(IntPtr.Zero);
				}
			}

#endif // ECMA_COMPAT

#if !ECMA_COMPAT

	// Get the calling conventions for this method.
	public virtual CallingConventions CallingConvention
			{
				get
				{
					return CallingConventions.Standard;
				}
			}

	// Check for various method attributes.
	public bool IsAbstract
			{
				get 
				{
					return ((Attributes & MethodAttributes.Abstract) != 0);
				}
			}
	public bool IsAssembly
			{
				get 
				{
					return ((Attributes & MethodAttributes.MemberAccessMask)
									== MethodAttributes.Assembly);
				}
			}
	public bool IsConstructor
			{
				get 
				{
					if((Attributes & MethodAttributes.RTSpecialName) == 0)
					{
						return false;
					}
					return (Name == ConstructorInfo.ConstructorName);
				}
			}
	public bool IsFamily
			{
				get 
				{
					return ((Attributes & MethodAttributes.MemberAccessMask)
									== MethodAttributes.Family);
				}
			}
	public bool IsFamilyAndAssembly
			{
				get 
				{
					return ((Attributes & MethodAttributes.MemberAccessMask)
									== MethodAttributes.FamANDAssem);
				}
			}
	public bool IsFamilyOrAssembly
			{
				get 
				{
					return ((Attributes & MethodAttributes.MemberAccessMask)
									== MethodAttributes.FamORAssem);
				}
			}
	public bool IsFinal
			{
				get 
				{
					return ((Attributes & MethodAttributes.Final) != 0);
				}
			}
	public bool IsHideBySig
			{
				get 
				{
					return ((Attributes & MethodAttributes.HideBySig) != 0);
				}
			}
	public bool IsPrivate
			{
				get 
				{
					return ((Attributes & MethodAttributes.MemberAccessMask)
									== MethodAttributes.Private);
				}
			}
	public bool IsPublic
			{
				get 
				{
					return ((Attributes & MethodAttributes.MemberAccessMask)
									== MethodAttributes.Public);
				}
			}
	public bool IsSpecialName
			{
				get 
				{
					return ((Attributes & MethodAttributes.SpecialName) != 0);
				}
			}
	public bool IsStatic
			{
				get 
				{
					return ((Attributes & MethodAttributes.Static) != 0);
				}
			}
	public bool IsVirtual
			{
				get 
				{
					return ((Attributes & MethodAttributes.Virtual) != 0);
				}
			}

	// Get the currently executing method.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static MethodBase GetCurrentMethod();

	// Get the method implementation flags.
	public abstract MethodImplAttributes GetMethodImplementationFlags();

#endif // !ECMA_COMPAT

}; // class MethodBase

#endif // CONFIG_REFLECTION

}; // namespace System.Reflection
