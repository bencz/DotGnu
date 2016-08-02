/*
 * LateBinding.cs - Handle late binding operations.
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
using Microsoft.JScript.Vsa;

public sealed class LateBinding
{
	// Internal state.
	private String name;
	public Object obj;

	// Constructors.
	public LateBinding(String name)
			{
				this.name = name;
				this.obj = null;
			}
	public LateBinding(String name, Object obj)
			{
				this.name = name;
				this.obj = obj;
			}

	// Perform a call on this late binding.
	public Object Call(Object[] arguments, bool construct,
					   bool brackets, VsaEngine engine)
			{
				// TODO
				return null;
			}

	// Perform a late binding call.
	public static Object CallValue(Object thisob, Object val,
								   Object[] arguments, bool construct,
								   bool brackets, VsaEngine engine)
			{
				// TODO
				return null;
			}

	// Perform a late binding call, with reversed arguments.
	public static Object CallValue2(Object val, Object thisob,
								    Object[] arguments, bool construct,
								    bool brackets, VsaEngine engine)
			{
				// TODO
				return null;
			}

	// Delete the named member from the object.
	public bool Delete()
			{
				return DeleteMember(obj, name);
			}
	
	// Delete a named member from a specified object.
	public static bool DeleteMember(Object obj, String name)
			{
				// TODO
				return false;
			}

	// Get the value of this binding, returning "null" for "Missing".
	public Object GetNonMissingValue()
			{
				// TODO
				return false;
			}

	// Get the value of this binding, throwing an exception for "Missing".
	public Object GetValue2()
			{
				// TODO
				return false;
			}

	// Set an indexed property value.
	public static void SetIndexedPropertyValueStatic
				(Object obj, Object[] arguments, Object value)
			{
				// TODO
			}

	// Set the value of this binding.
	public void SetValue(Object value)
			{
				// TODO
			}

}; // class LateBinding

}; // namespace Microsoft.JScript
