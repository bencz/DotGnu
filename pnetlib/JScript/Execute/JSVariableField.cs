/*
 * JSVariableField.cs - JScript variable, as a field.
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

public abstract class JSVariableField : JSField
{
	// Internal state.
	private FieldAttributes attributes;
	private String name;
	private FieldInfo realField;
	private Object[] customAttributes;
	protected ScriptObject obj;
	protected Object value;

	// Constructor.
	internal JSVariableField(FieldAttributes attributes, String name,
							 ScriptObject obj, Object value)
			{
				if((attributes & FieldAttributes.FieldAccessMask) ==
						(FieldAttributes)0)
				{
					attributes |= FieldAttributes.Public;
				}
				this.attributes = attributes;
				this.name = name;
				this.realField = null;
				this.customAttributes = null;
				this.obj = obj;
				this.value = value;
			}

	// Get the field's attributes.
	public override FieldAttributes Attributes
			{
				get
				{
					return attributes;
				}
			}

	// Get the declaring type.
	public override Type DeclaringType
			{
				get
				{
					// TODO
					return null;
				}
			}

	// Get the type of this field's value.
	public override Type FieldType
			{
				get
				{
					// TODO
					return typeof(Object);
				}
			}

	// Get the name of this field.
	public override String Name
			{
				get
				{
					return name;
				}
			}

	// Get the custom attributes that are attached to this field.
	public override Object[] GetCustomAttributes(bool inherit)
			{
				if(customAttributes != null)
				{
					return customAttributes;
				}
				return new Object[0];
			}

	// Get the "real" field that underlies this one.
	internal override FieldInfo GetRealField()
			{
				return realField;
			}

}; // class JSVariableField

}; // namespace Microsoft.JScript
