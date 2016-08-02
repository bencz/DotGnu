/*
 * MethodInfo.cs - Implementation of the "System.Reflection.MethodInfo" class.
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

using System.Runtime.InteropServices;

#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDual)]
#if CONFIG_FRAMEWORK_1_2 && CONFIG_REFLECTION
[ComDefaultInterface(typeof(_MethodInfo))]
#endif
#endif
public abstract class MethodInfo : MethodBase
#if CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2
	, _MethodInfo
#endif
{

	// Constructor.
	protected MethodInfo() : base() {}

	// Get the member type for this method.
	public override MemberTypes MemberType
			{
				get
				{
					return MemberTypes.Method;
				}
			}

	// Get the return type for this method.
	public abstract Type ReturnType { get; }

	// Get the base definition for this method.
	public abstract MethodInfo GetBaseDefinition();

#if !ECMA_COMPAT

	// Get the custom attribute provider for the return type.
	public abstract ICustomAttributeProvider
				ReturnTypeCustomAttributes { get; }

#endif // !ECMA_COMPAT

	// Determine if this method has generic arguments.
	protected virtual bool HasGenericArgumentsImpl()
			{
				return false;
			}
	public bool HasGenericArguments
			{
				get
				{
					return HasGenericArgumentsImpl();
				}
			}

	// Determine if this method has uninstantiated generic parameters.
	protected virtual bool HasGenericParametersImpl()
			{
				return false;
			}
	public bool HasGenericParameters
			{
				get
				{
					return HasGenericParametersImpl();
				}
			}

	// Get the arguments for this generic method instantiation.
	public virtual Type[] GetGenericArguments()
			{
				throw new NotSupportedException(_("NotSupp_NotGenericType"));
			}

	// Get the generic base method upon this instantiation was based.
	public virtual MethodInfo GetGenericMethodDefinition()
			{
				throw new NotSupportedException(_("NotSupp_NotGenericType"));
			}

	// Bind arguments to this generic method to instantiate it.
	public virtual MethodInfo BindGenericParameters(Type[] typeArgs)
			{
				throw new NotSupportedException(_("NotSupp_NotGenericType"));
			}

}; // class MethodInfo

#endif // CONFIG_REFLECTION

}; // namespace System.Reflection
