/*
 * WithScope.cs - Structure of an activation object for "with" contexts.
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

internal sealed class WithScope : ScriptObject, IActivationObject,
								  IVariableAccess
{
	// Internal state.
	private Object withObject;

	// Constructor.
	internal WithScope(ScriptObject parent, Object withObject)
			: base(parent)
			{
				this.withObject = withObject;
			}

	// Implement the IActivationObject interface.
	public Object GetDefaultThisObject()
			{
				return withObject;
			}
	public GlobalScope GetGlobalScope()
			{
				return ((IActivationObject)parent).GetGlobalScope();
			}
	public FieldInfo GetLocalField(String name)
			{
				// This method is not used.
				return null;
			}
	public Object GetMemberValue(String name, int lexlevel)
			{
				// TODO
				return Missing.Value;
			}
	public FieldInfo GetField(String name, int lexlevel)
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
				return ((ScriptObject)withObject).HasProperty(name);
			}
	Object IVariableAccess.GetVariable(String name)
			{
				if(((ScriptObject)withObject).HasProperty(name))
				{
					return ((ScriptObject)withObject).Get(name);
				}
				else
				{
					((ScriptObject)withObject).Put(name, null);
					return null;
				}
			}
	void IVariableAccess.SetVariable(String name, Object value)
			{
				((ScriptObject)withObject).Put(name, value);
			}
	void IVariableAccess.DeclareVariable(String name)
			{
				if(!((ScriptObject)withObject).HasProperty(name))
				{
					((ScriptObject)withObject).Put(name, null);
				}
			}
	IVariableAccess IVariableAccess.GetParentScope()
			{
				return (parent as IVariableAccess);
			}

}; // class WithScope

}; // namespace Microsoft.JScript
