/*
 * LateBinding.cs - Implementation of the
 *			"Microsoft.VisualBasic.LateBinding" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

namespace Microsoft.VisualBasic.CompilerServices
{

using System;
using System.ComponentModel;
using System.Reflection;
using System.Diagnostics;

[StandardModule]
#if CONFIG_COMPONENT_MODEL
[EditorBrowsable(EditorBrowsableState.Never)]
#endif
public sealed class LateBinding
{
	// Cannot instantiate this class.
	private LateBinding() {}

	// Perform a late call.
#if !ECMA_COMPAT
	[DebuggerStepThrough]
	[DebuggerHidden]
#endif
	public static void LateCall(Object o, Type objType,
								String name, Object[] args,
								String[] paramnames,
								bool[] CopyBack)
			{
				LateCallWithResult
					(o, objType, name, args, paramnames, CopyBack);
			}
#if !ECMA_COMPAT
	[DebuggerStepThrough]
	[DebuggerHidden]
#endif
	internal static Object LateCallWithResult
				(Object o, Type objType, String name, Object[] args,
				 String[] paramnames, bool[] CopyBack)
			{
			#if CONFIG_REFLECTION
				if(objType == null)
				{
					objType = o.GetType();
				}
				return objType.InvokeMember
					(name, BindingFlags.InvokeMethod |
						   BindingFlags.Public |
						   BindingFlags.Static |
						   BindingFlags.Instance, null, o, args,
					 null, null, paramnames);
			#else
				throw new NotImplementedException();
			#endif
			}

	// Perform a late property get.
#if !ECMA_COMPAT
	[DebuggerStepThrough]
	[DebuggerHidden]
#endif
	public static Object LateGet(Object o, Type objType,
								 String name, Object[] args,
								 String[] paramnames, bool[] CopyBack)
			{
			#if CONFIG_REFLECTION
				if(objType == null)
				{
					objType = o.GetType();
				}
				return objType.InvokeMember
					(name, BindingFlags.GetField |
						   BindingFlags.GetProperty |
						   BindingFlags.Public |
						   BindingFlags.Static |
						   BindingFlags.Instance, null, o, args,
					 null, null, paramnames);
			#else
				throw new NotImplementedException();
			#endif
			}

	// Perform a late index get.
#if !ECMA_COMPAT
	[DebuggerStepThrough]
	[DebuggerHidden]
#endif
	public static Object LateIndexGet(Object o, Object[] args,
									  String[] paramnames)
			{
			#if CONFIG_REFLECTION
				return o.GetType().InvokeMember
					("", BindingFlags.GetProperty |
						 BindingFlags.Public |
						 BindingFlags.Static |
						 BindingFlags.Instance, null, o, args,
					 null, null, paramnames);
			#else
				throw new NotImplementedException();
			#endif
			}

	// Perform a late index set.
#if !ECMA_COMPAT
	[DebuggerStepThrough]
	[DebuggerHidden]
#endif
	public static void LateIndexSet(Object o, Object[] args,
									String[] paramnames)
			{
			#if CONFIG_REFLECTION
				o.GetType().InvokeMember
					("", BindingFlags.SetProperty |
						 BindingFlags.Public |
						 BindingFlags.Static |
						 BindingFlags.Instance, null, o, args,
				     null, null, paramnames);
			#else
				throw new NotImplementedException();
			#endif
			}

	// Perform a complex late index set.
#if !ECMA_COMPAT
	[DebuggerStepThrough]
	[DebuggerHidden]
#endif
	public static void LateIndexSetComplex(Object o, Object[] args,
										   String[] paramnames,
										   bool OptimisticSet,
										   bool RValueBase)
			{
				LateIndexSet(o, args, paramnames);
			}

	// Perform a late property set.
#if !ECMA_COMPAT
	[DebuggerStepThrough]
	[DebuggerHidden]
#endif
	public static void LateSet(Object o, Type objType, String name,
							   Object[] args, String[] paramnames)
			{
			#if CONFIG_REFLECTION
				if(objType == null)
				{
					objType = o.GetType();
				}
				objType.InvokeMember
					(name, BindingFlags.SetField |
						   BindingFlags.SetProperty |
						   BindingFlags.Public |
						   BindingFlags.Static |
						   BindingFlags.Instance, null, o, args,
				     null, null, paramnames);
			#else
				throw new NotImplementedException();
			#endif
			}

	// Perform a complex late property set.
#if !ECMA_COMPAT
	[DebuggerStepThrough]
	[DebuggerHidden]
#endif
	public static void LateSetComplex(Object o, Type objType, String name,
							          Object[] args, String[] paramnames,
									  bool OptimisticSet, bool RValueBase)
			{
				LateSet(o, objType, name, args, paramnames);
			}

}; // class LateBinding

}; // namespace Microsoft.VisualBasic.CompilerServices
