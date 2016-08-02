/*
 * ClrMethod.cs - Implementation of the
 *		"System.Reflection.ClrMethod" class.
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

using System;
using System.Globalization;
using System.Text;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

internal sealed class ClrMethod : MethodInfo, IClrProgramItem
#if CONFIG_SERIALIZATION
	, ISerializable
#endif
{

	// Private data used by the runtime engine.  This must be the first field.
	private IntPtr privateData;

	// Cached copy of the parameters.
	private ParameterInfo[] parameters;

	// Implement the IClrProgramItem interface.
	public IntPtr ClrHandle
			{
				get
				{
					return privateData;
				}
			}

	// Get the custom attributes attached to this method.
	public override Object[] GetCustomAttributes(bool inherit)
			{
				return ClrHelpers.GetCustomAttributes(this, inherit);
			}
	public override Object[] GetCustomAttributes(Type type, bool inherit)
			{
				return ClrHelpers.GetCustomAttributes(this, type, inherit);
			}

	// Determine if custom attributes are defined for this method.
	public override bool IsDefined(Type type, bool inherit)
			{
				return ClrHelpers.IsDefined(this, type, inherit);
			}

	// Get the parameters for this method.
	public override ParameterInfo[] GetParameters()
			{
				if(parameters != null)
				{
					return parameters;
				}
				int numParams = ClrHelpers.GetNumParameters(privateData);
				int param;
				parameters = new ParameterInfo [numParams];
				for(param = 0; param < numParams; ++param)
				{
					parameters[param] =
						ClrHelpers.GetParameterInfo(this, this, param + 1);
				}
				return parameters;
			}

	// Invoke this method.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public override Object Invoke
				(Object obj, BindingFlags invokeAttr, Binder binder,
				 Object[] parameters, CultureInfo culture);

	// Get the method attributes.
	public override MethodAttributes Attributes
			{
				get
				{
					return (MethodAttributes)
						ClrHelpers.GetMemberAttrs(privateData);
				}
			}

	// Override inherited properties.
	public override Type DeclaringType
			{
				get
				{
					return ClrHelpers.GetDeclaringType(this);
				}
			}
	public override Type ReflectedType
			{
				get
				{
					return ClrHelpers.GetDeclaringType(this);
				}
			}
	public override String Name
			{
				get
				{
					return ClrHelpers.GetName(this);
				}
			}
	public override Type ReturnType
			{
				get
				{
					return ClrHelpers.GetParameterType(privateData, 0);
				}
			}

	// Get the base definition for this method.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public override MethodInfo GetBaseDefinition();

	// Get the runtime method handle associated with this method.
#if ECMA_COMPAT
	internal
#else
	public
#endif
	override RuntimeMethodHandle MethodHandle
			{
				get
				{
					return new RuntimeMethodHandle(privateData);
				}
			}

#if !ECMA_COMPAT

	// Get the custom attribute provider for the return type.
	public override ICustomAttributeProvider
				ReturnTypeCustomAttributes
			{
				get
				{
					return ClrHelpers.GetParameterInfo(this, this, 0);
				}
			}

	// Get the calling conventions for this method.
	public override CallingConventions CallingConvention
			{
				get
				{
					return ClrHelpers.GetCallConv(privateData);
				}
			}

	// Get the method implementation flags.
	public override MethodImplAttributes GetMethodImplementationFlags()
			{
				return ClrHelpers.GetImplAttrs(privateData);
			}

#endif // !ECMA_COMPAT

	// Convert the method name into a string.
	public override String ToString()
			{
				StringBuilder builder = new StringBuilder();
				int numParams = ClrHelpers.GetNumParameters(privateData);
				int param;
				ParameterInfo paramInfo;
				builder.Append(ReturnType.Name);
				builder.Append(' ');
				builder.Append(Name);
				builder.Append('(');
				for(param = 0; param < numParams; ++param)
				{
					if(param > 0)
					{
						builder.Append(", ");
					}
					paramInfo = ClrHelpers.GetParameterInfo
						(this, this, param + 1);
					builder.Append(paramInfo.ParameterType.Name);
				}
				builder.Append(')');
				return builder.ToString();
			}

	// Determine if this method has generic arguments.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern protected override bool HasGenericArgumentsImpl();

	// Determine if this method has uninstantiated generic parameters.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern protected override bool HasGenericParametersImpl();

	// Get the arguments for this generic method instantiation.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private Type[] GetGenericArgumentsImpl();
	public override Type[] GetGenericArguments()
			{
				if(!HasGenericArgumentsImpl())
				{
					return new Type [0];
				}
				else
				{
					return GetGenericArgumentsImpl();
				}
			}

	// Get the generic base method upon this instantiation was based.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private ClrMethod GetGenericMethodDefinitionImpl();
	public override MethodInfo GetGenericMethodDefinition()
			{
				if(HasGenericArgumentsImpl())
				{
					return GetGenericMethodDefinitionImpl();
				}
				else if(HasGenericParametersImpl())
				{
					// Not instantiated, so the base method is itself.
					return this;
				}
				else
				{
					return null;
				}
			}

	// Get the arity of an uninstantiated generic method.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private int GetArity();

	// Bind arguments to this generic method to instantiate it.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private MethodInfo BindGenericParametersImpl(Type[] typeArgs);
	public override MethodInfo BindGenericParameters(Type[] typeArgs)
			{
				if(typeArgs == null)
				{
					throw new ArgumentNullException("typeArgs");
				}
				ClrMethod method = this;
				if(HasGenericArgumentsImpl())
				{
					// Use the base method, not the instantiated form.
					method = GetGenericMethodDefinitionImpl();
				}
				if(!method.HasGenericParametersImpl())
				{
					throw new ArgumentException
						(_("Arg_NotGenericMethod"));
				}
				else if(method.GetArity() != typeArgs.Length)
				{
					throw new ArgumentException
						(_("Arg_GenericParameterCount"));
				}
				else
				{
					return method.BindGenericParametersImpl(typeArgs);
				}
			}

#if CONFIG_SERIALIZATION

	// Get the serialization data for this method.
	public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				MemberInfoSerializationHolder.Serialize
					(info, MemberTypes.Method, Name, ToString(), ReflectedType);
			}

#endif

}; // class ClrMethod

#endif // CONFIG_REFLECTION

}; // namespace System.Reflection
