/*
 * NumberObject.cs - The JScript Number object.
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
using System.Globalization;

public class NumberObject : JSObject
{
	// underlying value
	internal Object value;
	
	// internal constructor
	internal NumberObject(ScriptObject parent, Object value)
		: base(parent)
	{
		switch(Type.GetTypeCode(value.GetType()))
		{
		case TypeCode.SByte:
		case TypeCode.Byte:
		case TypeCode.Int16:
		case TypeCode.UInt16:
		case TypeCode.Int32:
		case TypeCode.UInt32:
		case TypeCode.Int64: 
		case TypeCode.UInt64:
		case TypeCode.Single:
		case TypeCode.Double:
			this.value = value;
			break;
		default:
			throw new JScriptException(JSError.TypeMismatch);
		}
	}
	
	// Determine if two numbers are equal
	public override bool Equals(Object obj)
	{
		if(obj == null || obj is Missing)
		{
			return false;
		}
		if(obj is NumberObject)
		{
			// Unwrap the value object first
			obj = ((NumberObject)obj).value;
		}
		return value.Equals(obj);
	}
	
	// Get a hash code for this object.
	public override int GetHashCode()
	{
		return value.GetHashCode();
	}
	
	// Get the type for this object
	public new Type GetType()
	{
		return typeof(NumberObject);
	}
	
	// Return a string representation
	public override String ToString()
	{
		return value.ToString();
	}
	
	// Get the internal "[[Class]]" property for this object.
	internal override String Class
	{
		get
		{
			return "Number";
		}
	}
	
	// Get the default value for this object.
	internal override Object DefaultValue(DefaultValueHint hint)
	{
		return value;
	}
		
}; // class NumberObject

}; // namespace Microsoft.JScript
