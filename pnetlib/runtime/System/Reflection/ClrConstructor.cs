/*
 * ClrConstructor.cs - Implementation of the
 *		"System.Reflection.ClrConstructor" class.
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
using System.Text;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

internal sealed class ClrConstructor : ConstructorInfo, IClrProgramItem
#if CONFIG_SERIALIZATION
	, ISerializable
#endif
{

	// Private data used by the runtime engine.
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

	// Get the custom attributes attached to this constructor.
	public override Object[] GetCustomAttributes(bool inherit)
			{
				return ClrHelpers.GetCustomAttributes(this, inherit);
			}
	public override Object[] GetCustomAttributes(Type type, bool inherit)
			{
				return ClrHelpers.GetCustomAttributes(this, type, inherit);
			}

	// Determine if custom attributes are defined for this constructor.
	public override bool IsDefined(Type type, bool inherit)
			{
				return ClrHelpers.IsDefined(this, type, inherit);
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

	// Get the parameters for this constructor.
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

	// Get the method attributes.
	public override MethodAttributes Attributes
			{
				get
				{
					return (MethodAttributes)
						ClrHelpers.GetMemberAttrs(privateData);
				}
			}

	// Invoke this constructor.
	public override Object Invoke(Object obj, BindingFlags invokeAttr,
								  Binder binder, Object[] parameters,
								  CultureInfo culture)
			{
				if(obj != null)
				{
					throw new TargetException(_("Reflection_CtorTarget"));
				}
				return Invoke(invokeAttr, binder, parameters, culture);
			}
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public override Object Invoke
				(BindingFlags invokeAttr, Binder binder,
				 Object[] parameters, CultureInfo culture);
	
	// Invoke this constructor.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal override Object InvokeOnEmpty(Object obj, 
								  BindingFlags invokeAttr,
								  Binder binder, Object[] parameters,
								  CultureInfo culture);
								  
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

	// Convert the constructor name into a string.
	public override String ToString()
			{
				StringBuilder builder = new StringBuilder();
				int numParams = ClrHelpers.GetNumParameters(privateData);
				int param;
				ParameterInfo paramInfo;
				builder.Append("Void ");
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

#if CONFIG_SERIALIZATION

	// Get the serialization data for this constructor.
	public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				MemberInfoSerializationHolder.Serialize
					(info, MemberTypes.Constructor,
					 Name, ToString(), ReflectedType);
			}

#endif

}; // class ClrConstructor

#endif // CONFIG_REFLECTION

}; // namespace System.Reflection
