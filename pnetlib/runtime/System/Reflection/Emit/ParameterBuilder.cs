/*
 * ParameterBuilder.cs - Implementation of the
 *			"System.Reflection.Emit.ParameterBuilder" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
 * 
 * Contributed by Gopal.V <gopalv82@symonds.net> 
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

namespace System.Reflection.Emit
{

#if CONFIG_REFLECTION_EMIT

using System;
using System.Runtime.CompilerServices;

public class ParameterBuilder : IClrProgramItem, IDetachItem
{
	// Internal state.
	private TypeBuilder type;
	private MethodBase method;
	private IntPtr privateData;

	// Constructor.
	internal ParameterBuilder(TypeBuilder type, MethodBase method,
							  int position, ParameterAttributes attributes,
							  String strParamName)
			{
				// Initialize the internal state.
				this.type = type;
				this.method = method;

				// Register this item to be detached later.
				type.module.assembly.AddDetach(this);

				// Create the parameter.
				lock(typeof(AssemblyBuilder))
				{
					this.privateData = ClrParameterCreate
						(((IClrProgramItem)method).ClrHandle,
						 position, attributes, strParamName);
				}
			}

	// Get the token for this parameter.
	public virtual ParameterToken GetToken()
			{
				lock(typeof(AssemblyBuilder))
				{
					return new ParameterToken
						(AssemblyBuilder.ClrGetItemToken(privateData));
				}
			}

	// Set a default constant value for this parameter.
	public virtual void SetConstant(Object defaultValue)
			{
				Type paramType;
				int position;
				try
				{
					type.StartSync();
					position = Position;
					if(position == 0)
					{
						if(method is MethodInfo)
						{
							paramType = ((MethodInfo)method).ReturnType;
						}
						else
						{
							paramType = typeof(void);
						}
					}
					else
					{
						paramType = ((method.GetParameters())[position - 1])
										.ParameterType;
					}
					FieldBuilder.ValidateConstant(paramType, defaultValue);
					lock(typeof(AssemblyBuilder))
					{
						FieldBuilder.ClrFieldSetConstant
							(privateData, defaultValue);
					}
				}
				finally
				{
					type.EndSync();
				}
			}

	// Set a custom attribute on this parameter.
	public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
			{
				try
				{
					type.StartSync();
					type.module.assembly.SetCustomAttribute
						(this, customBuilder);
				}
				finally
				{
					type.EndSync();
				}
			}
	public void SetCustomAttribute(ConstructorInfo con, byte[] binaryAttribute)
			{
				try
				{
					type.StartSync();
					type.module.assembly.SetCustomAttribute
						(this, con, binaryAttribute);
				}
				finally
				{
					type.EndSync();
				}
			}

	// Set the marshalling information for this parameter.
	public virtual void SetMarshal(UnmanagedMarshal unmanagedMarshal)
			{
				try
				{
					type.StartSync();
					if(unmanagedMarshal == null)
					{
						throw new ArgumentNullException("unmanagedMarshal");
					}
					lock(typeof(AssemblyBuilder))
					{
						FieldBuilder.ClrFieldSetMarshal
							(privateData, unmanagedMarshal.ToBytes());
					}
				}
				finally
				{
					type.EndSync();
				}
			}

	// Get the attributes for this parameter.
	public virtual int Attributes 
			{
				get
				{
					lock(typeof(AssemblyBuilder))
					{
						return ClrParameterGetAttrs(privateData);
					}
				}
			}

	// Determine if this is an "in" parameter.
	public bool IsIn 
			{ 
				get
				{
					return ((Attributes & 0x0001) != 0);
				}
			}

	// Determine if this is an "optional" parameter.
	public bool IsOptional 
			{
				get
				{
					return ((Attributes & 0x0004) != 0);
				}
			}

	// Determine if this is an "out" parameter.
	public bool IsOut 
			{
				get
				{
					return ((Attributes & 0x0002) != 0);
				}
			}

	// Get the name that is associated with this parameter.
	public virtual String Name 
			{
				get
				{
					lock(typeof(AssemblyBuilder))
					{
						return ClrParameterGetName(privateData);
					}
				}
			}

	// Get the position associated with this parameter.
	public virtual int Position 
			{
				get
				{
					lock(typeof(AssemblyBuilder))
					{
						return ClrParameterGetPosition(privateData);
					}
				}
			}

	// Get the CLR handle for this program item.
	IntPtr IClrProgramItem.ClrHandle
			{
				get
				{
					return privateData;
				}
			}

	// Detach this program item.
	void IDetachItem.Detach()
			{
				privateData = IntPtr.Zero;
			}

	// Create a new parameter and attach it to a particular method.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static IntPtr ClrParameterCreate
			(IntPtr method, int position,
			 ParameterAttributes attributes, String name);

	// Get the attributes associated with a parameter.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static int ClrParameterGetAttrs(IntPtr parameter);

	// Get the name associated with a parameter.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static String ClrParameterGetName(IntPtr parameter);

	// Get the position associated with a parameter.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static int ClrParameterGetPosition(IntPtr parameter);

}; // class ParameterBuilder

#endif // CONFIG_REFLECTION_EMIT

}; // namespace System.Reflection.Emit
