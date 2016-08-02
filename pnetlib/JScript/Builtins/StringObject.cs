/*
 * StringObject.cs - Implementation of string objects.
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

public class StringObject : JSObject
{
	// Internal state.
	private String value;

	// Constructor.
	internal StringObject(ScriptObject prototype, String value)
			: base(prototype)
			{
				this.value = value;
				Put("length", length); // thanks to immutable strings
			}

	// Get the length of this string.
	public int length
			{
				get
				{
					return value.Length;
				}
			}
	
	// Determine if two string objects are equal.
	public override bool Equals(Object obj)
			{
				if(obj is StringObject)
				{
					// Unwrap the string object first.
					obj = ((StringObject)obj).value;
				}
				return value.Equals(obj);
			}

	// Get a hash code for this object.
	public override int GetHashCode()
			{
				return value.GetHashCode();
			}

	// Get the type code for this object.
	public new Type GetType()
			{
				return typeof(StringObject);
			}

	// Convert this string object into a raw string.
	public override String ToString()
			{
				return value;
			}

	// Get the internal "[[Class]]" property for this object.
	internal override String Class
			{
				get
				{
					return "String";
				}
			}

	// Get the default value for this object.
	internal override Object DefaultValue(DefaultValueHint hint)
			{
				return value;
			}

}; // class StringObject

}; // namespace Microsoft.JScript
