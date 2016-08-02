/*
 * ParameterInfo.cs - Implementation of the
 *			"System.Reflection.ParameterInfo" class.
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

public class ParameterInfo : ICustomAttributeProvider
{
	// Parameter information state that is populated by subclasses.
#if !ECMA_COMPAT
	protected ParameterAttributes AttrsImpl;
	protected Type ClassImpl;
	protected Object DefaultValueImpl;
	protected MemberInfo MemberImpl;
	protected String NameImpl;
	protected int PositionImpl;
#else  // ECMA_COMPAT
	internal ParameterAttributes AttrsImpl;
	internal Type ClassImpl;
	internal Object DefaultValueImpl;
	internal MemberInfo MemberImpl;
	internal String NameImpl;
	internal int PositionImpl;
#endif // ECMA_COMPAT

	// Constructor.
	protected ParameterInfo() : base() {}

	// Get the attributes for this parameter.
	public virtual ParameterAttributes Attributes
			{
				get
				{
					return AttrsImpl;
				}
			}

	// Get the name of this parameter.
	public virtual String Name
			{
				get
				{
					return NameImpl;
				}
			}

	// Get the parameter type.
	public virtual Type ParameterType
			{
				get
				{
					return ClassImpl;
				}
			}

#if !ECMA_COMPAT

	// Get the custom attributes that are associated with this parameter.
	public virtual Object[] GetCustomAttributes(bool inherit)
			{
				return null;
			}
	public virtual Object[] GetCustomAttributes(Type type, bool inherit)
			{
				return null;
			}

	// Determine if a custom attribute is currently defined.
	public virtual bool IsDefined(Type type, bool inherit)
			{
				return false;
			}

#else  // ECMA_COMPAT

	// Get the custom attributes that are associated with this parameter.
	Object[] ICustomAttributeProvider.GetCustomAttributes(bool inherit)
			{
				return ClrGetCustomAttributes(inherit);
			}
	Object[] ICustomAttributeProvider.GetCustomAttributes
					(Type type, bool inherit)
			{
				return ClrGetCustomAttributes(type, inherit);
			}

	// Determine if a custom attribute is currently defined.
	bool ICustomAttributeProvider.IsDefined(Type type, bool inherit)
			{
				return ClrIsDefined(type, inherit);
			}

	// Internal interface for getting custom attributes from
	// ClrParameter when the ICustomAttributeProvider interface
	// is declared as private.
	internal virtual Object[] ClrGetCustomAttributes(bool inherit)
			{
				return null;
			}
	internal virtual Object[] ClrGetCustomAttributes
					(Type type, bool inherit)
			{
				return null;
			}
	internal virtual bool ClrIsDefined(Type type, bool inherit)
			{
				return false;
			}

#endif // ECMA_COMPAT

#if !ECMA_COMPAT

	// Get the default value that is associated with this parameter.
	public virtual Object DefaultValue
			{
				get
				{
					return DefaultValueImpl;
				}
			}

	// Determine if this is an input parameter.
	public bool IsIn
			{
				get
				{
					return ((Attributes & ParameterAttributes.In) != 0);
				}
			}

	// Determine if this is a locale identifier parameter.
	public bool IsLcid
			{
				get
				{
					// This is an obsolete parameter type that does
					// not exist in the current ECMA spec.
					return false;
				}
			}

	// Determine if this is an optional parameter.
	public bool IsOptional
			{
				get
				{
					return ((Attributes & ParameterAttributes.Optional) != 0);
				}
			}

	// Determine if this is an output parameter.
	public bool IsOut
			{
				get
				{
					return ((Attributes & ParameterAttributes.Out) != 0);
				}
			}

	// Determine if this is a return value parameter.
	public bool IsRetval
			{
				get
				{
					return ((Attributes & ParameterAttributes.Retval) != 0);
				}
			}

	// Get the member that this parameter is associated with.
	public virtual MemberInfo Member
			{
				get
				{
					return MemberImpl;
				}
			}

	// Get the position of this parameter.
	public virtual int Position
			{
				get
				{
					return PositionImpl;
				}
			}

#endif

}; // class ParameterInfo

#endif // CONFIG_REFLECTION

}; // namespace System.Reflection
