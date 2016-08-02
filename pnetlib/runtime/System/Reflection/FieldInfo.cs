/*
 * FieldInfo.cs - Implementation of the "System.Reflection.FieldInfo" class.
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
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.Diagnostics;

#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDual)]
#if CONFIG_FRAMEWORK_1_2 && CONFIG_REFLECTION
[ComDefaultInterface(typeof(_FieldInfo))]
#endif
#endif
public abstract class FieldInfo : MemberInfo
#if CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2
	, _FieldInfo
#endif
{

	// Constructor.
	protected FieldInfo() : base() {}

	// Get the member type for this object.
	public override MemberTypes MemberType
			{
				get
				{
					return MemberTypes.Field;
				}
			}

	// Get the attributes that are associated with this field.
	public abstract FieldAttributes Attributes { get; }

	// Get the type that is associated with this field.
	public abstract Type FieldType { get; }

	// Get the value associated with this field on an object.
	public abstract Object GetValue(Object obj);

	// Set the value associated with this field on an object.
#if !ECMA_COMPAT
	[DebuggerStepThrough]
	[DebuggerHidden]
#endif
	public void SetValue(Object obj, Object value)
			{
				SetValue(obj, value, BindingFlags.Default,
						 Type.DefaultBinder, null);
			}
	public abstract void SetValue(Object obj, Object value,
								  BindingFlags invokeAttr,
								  Binder binder, CultureInfo culture);

#if !ECMA_COMPAT

	// Get a field given its handle.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static FieldInfo GetFieldFromHandle
				(RuntimeFieldHandle handle);

	// Get the handle that is associated with this field.
	public abstract RuntimeFieldHandle FieldHandle { get; }

	// Check for various field attributes.
	public bool IsAssembly
			{
				get
				{
					return ((Attributes & FieldAttributes.FieldAccessMask)
									== FieldAttributes.Assembly);
				}
			}
	public bool IsFamily
			{
				get
				{
					return ((Attributes & FieldAttributes.FieldAccessMask)
									== FieldAttributes.Family);
				}
			}
	public bool IsFamilyAndAssembly
			{
				get
				{
					return ((Attributes & FieldAttributes.FieldAccessMask)
									== FieldAttributes.FamANDAssem);
				}
			}
	public bool IsFamilyOrAssembly
			{
				get
				{
					return ((Attributes & FieldAttributes.FieldAccessMask)
									== FieldAttributes.FamORAssem);
				}
			}
	public bool IsInitOnly
			{
				get
				{
					return ((Attributes & FieldAttributes.InitOnly) != 0);
				}
			}
	public bool IsLiteral
			{
				get
				{
					return ((Attributes & FieldAttributes.Literal) != 0);
				}
			}
	public bool IsNotSerialized
			{
				get
				{
					return ((Attributes & FieldAttributes.NotSerialized) != 0);
				}
			}
	public bool IsPinvokeImpl
			{
				get
				{
					return ((Attributes & FieldAttributes.PinvokeImpl) != 0);
				}
			}
	public bool IsPrivate
			{
				get
				{
					return ((Attributes & FieldAttributes.FieldAccessMask)
									== FieldAttributes.Private);
				}
			}
	public bool IsPublic
			{
				get
				{
					return ((Attributes & FieldAttributes.FieldAccessMask)
									== FieldAttributes.Public);
				}
			}
	public bool IsSpecialName
			{
				get
				{
					return ((Attributes & FieldAttributes.SpecialName) != 0);
				}
			}
	public bool IsStatic
			{
				get
				{
					return ((Attributes & FieldAttributes.Static) != 0);
				}
			}

	// Get the value directly from a typed reference.
	[CLSCompliant(false)]
	public virtual Object GetValueDirect(TypedReference obj)
			{
				throw new NotSupportedException(_("NotSupp_GetValueDirect"));
			}

	// Set the value directly to a typed reference.
	[CLSCompliant(false)]
	public virtual void SetValueDirect(TypedReference obj, Object value)
			{
				throw new NotSupportedException(_("NotSupp_SetValueDirect"));
			}

#endif // !ECMA_COMPAT

}; // class FieldInfo

#endif // CONFIG_REFLECTION

}; // namespace System.Reflection
