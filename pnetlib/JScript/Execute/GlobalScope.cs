/*
 * GlobalScope.cs - Structure of the global activation object.
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
using System.Runtime.InteropServices.Expando;
using Microsoft.JScript.Vsa;

public class GlobalScope : ActivationObject
#if !ECMA_COMPAT
	, IExpando
#endif
{
	// Constructor.
	public GlobalScope(GlobalScope parent, ScriptObject storage)
			: base(parent, storage)
			{
				// Nothing else to do here.
			}
	
	public GlobalScope(GlobalScope parent, VsaEngine engine)
			: base(parent, (parent == null && engine != null ?
					engine.LenientGlobalObject.globalObject : null))
			{
				// Nothing else to do here.
			}

	// Override methods in the IActivationObject interface.
	public override Object GetDefaultThisObject()
			{
				return this;
			}
	public override GlobalScope GetGlobalScope()
			{
				return this;
			}
	public override FieldInfo GetLocalField(String name)
			{
				return GetField(name, BindingFlags.Instance |
									  BindingFlags.Static |
									  BindingFlags.Public |
									  BindingFlags.DeclaredOnly);
			}
	public override FieldInfo GetField(String name, int lexlevel)
			{
				return GetField(name, BindingFlags.Instance |
									  BindingFlags.Static |
									  BindingFlags.Public |
									  BindingFlags.DeclaredOnly);
			}

	// Override methods in the IReflect interface.
	public override FieldInfo[] GetFields(BindingFlags bindingAttr)
			{
				return base.GetFields(bindingAttr | BindingFlags.DeclaredOnly);
			}
	public override MemberInfo[] GetMember
				(String name, BindingFlags bindingAttr)
			{
				// TODO
				return null;
			}
	public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
			{
				// TODO
				return null;
			}
	public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
			{
				return base.GetMethods(bindingAttr | BindingFlags.DeclaredOnly);
			}
	public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
			{
				return base.GetProperties
					(bindingAttr | BindingFlags.DeclaredOnly);
			}

	// Implement the IExpando interface.
	public FieldInfo AddField(String name)
			{
				return CreateField(name, FieldAttributes.Public, null);
			}
#if !ECMA_COMPAT
	MethodInfo IExpando.AddMethod(String name, Delegate method)
			{
				// Not used by JScript.
				return null;
			}
	PropertyInfo IExpando.AddProperty(String name)
			{
				// Not used by JScript.
				return null;
			}
	void IExpando.RemoveMember(MemberInfo m)
			{
				// TODO
			}
#endif

}; // class GlobalScope

}; // namespace Microsoft.JScript
