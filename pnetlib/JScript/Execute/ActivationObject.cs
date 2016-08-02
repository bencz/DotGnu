/*
 * ActivationObject.cs - Structure of an activation object.
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
using System.Collections;
using System.Reflection;

public abstract class ActivationObject : ScriptObject, IActivationObject,
										 IVariableAccess
{
	// Internal state.
	private ScriptObject storage;

	protected ArrayList field_table;

	// Constructor.
	internal ActivationObject(ScriptObject parent, ScriptObject storage)
			: base(parent)
			{
				this.storage = storage;
			}

	// Implement the IActivationObject interface.
	public virtual Object GetDefaultThisObject()
			{
				return ((IActivationObject)parent).GetDefaultThisObject();
			}
	public virtual GlobalScope GetGlobalScope()
			{
				return ((IActivationObject)parent).GetGlobalScope();
			}
	public virtual FieldInfo GetLocalField(String name)
			{
				return storage.GetField(name, BindingFlags.Instance |
									  		  BindingFlags.Static |
									  		  BindingFlags.Public |
									  		  BindingFlags.DeclaredOnly);
			}
	public virtual Object GetMemberValue(String name, int lexlevel)
			{
				if(lexlevel > 0)
				{
					if(storage.HasOwnProperty(name))
					{
						return storage.Get(name);
					}
					else if(parent != null)
					{
						return ((IActivationObject)parent).GetMemberValue
									(name, lexlevel - 1);
					}
				}
				return Missing.Value;
			}
	public virtual FieldInfo GetField(String name, int lexlevel)
			{
				throw new JScriptException(JSError.InternalError);
			}

	// Create a new field within this activation object.
	protected virtual JSVariableField CreateField
				(String name, FieldAttributes attributes, Object value)
			{
				// TODO
				return null;
			}

	// Get a specific member.
	public override MemberInfo[] GetMember
				(String name, BindingFlags bindingAttr)
			{
				// TODO
				return null;
			}

	// Get all members that match specific binding conditions.
	public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
			{
				// TODO
				return null;
			}

	// Implement the internal "IVariableAccess" interface.
	bool IVariableAccess.HasVariable(String name)
			{
				return storage.HasOwnProperty(name);
			}
	Object IVariableAccess.GetVariable(String name)
			{
				if(storage.HasOwnProperty(name))
				{
					return storage.Get(name);
				}
				else
				{
					storage.Put(name, null);
					return null;
				}
			}
	void IVariableAccess.SetVariable(String name, Object value)
			{
				storage.Put(name, value);
			}
	void IVariableAccess.DeclareVariable(String name)
			{
				if(!storage.HasOwnProperty(name))
				{
					storage.Put(name, null);
				}
			}
	IVariableAccess IVariableAccess.GetParentScope()
			{
				return (parent as IVariableAccess);
			}

}; // class ActivationObject

}; // namespace Microsoft.JScript
