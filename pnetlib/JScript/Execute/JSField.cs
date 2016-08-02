/*
 * JSField.cs - Extended information for a JScript field.
 *
 * Copyright (C) 2003 Southern Storm Software, Pty Ltd.
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
 
namespace Microsoft.JScript
{

using System;
using System.Reflection;

public abstract class JSField : FieldInfo
{

	// Get the field's attributes.
	public override FieldAttributes Attributes
			{
				get
				{
					return (FieldAttributes)0;
				}
			}

	// Get the declaring type.
	public override Type DeclaringType
			{
				get
				{
					return null;
				}
			}

#if !ECMA_COMPAT
	// Get the handle for the underlying "real" field.
	public override RuntimeFieldHandle FieldHandle
			{
				get
				{
					return GetRealField().FieldHandle;
				}
			}
#endif

	// Get the type of this field's value.
	public override Type FieldType
			{
				get
				{
					return typeof(Object);
				}
			}

	// Get the member type.
	public override MemberTypes MemberType
			{
				get
				{
					return MemberTypes.Field;
				}
			}

	// Get the name of this field.
	public override String Name
			{
				get
				{
					return String.Empty;
				}
			}

	// Get the reflected type.
	public override Type ReflectedType
			{
				get
				{
					return DeclaringType;
				}
			}

	// Get the custom attributes that are attached to this field.
	public override Object[] GetCustomAttributes(bool inherit)
			{
				return new Object[0];
			}
	public override Object[] GetCustomAttributes(Type type, bool inherit)
			{
				return new Object[0];
			}
	public override bool IsDefined(Type type, bool inherit)
			{
				return false;
			}

	// Get the "real" field that underlies this one.
	internal virtual FieldInfo GetRealField()
			{
				throw new JScriptException(JSError.InternalError);
			}

}; // class JSField

}; // namespace Microsoft.JScript
