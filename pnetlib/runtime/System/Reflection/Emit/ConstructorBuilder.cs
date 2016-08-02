/*
 * ConstructorBuilder.cs - Implementation of the
 *		"System.Reflection.Emit.ConstructorBuilder" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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
using System.Text;
using System.Security;
using System.Reflection;
using System.Globalization;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

public sealed class ConstructorBuilder
		: ConstructorInfo, IClrProgramItem, IDetachItem
{
	// Internal state.
	internal TypeBuilder type;
	private IntPtr privateData;
	internal int numParams;
	private ILGenerator ilGenerator;
	private bool initLocals;
	private SignatureHelper helper;

	// Constructor.
	internal ConstructorBuilder(TypeBuilder type, String name,
								MethodAttributes attributes,
				 				CallingConventions callingConvention,
				 				Type[] parameterTypes)
			{
				// Validate the parameters.
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				else if(name == String.Empty)
				{
					throw new ArgumentException(_("Emit_NameEmpty"));
				}
				if((attributes & MethodAttributes.Static) == 0)
				{
					callingConvention |= CallingConventions.HasThis;
				}
				if((attributes & MethodAttributes.Virtual) != 0)
				{
					throw new ArgumentException(/* TODO */);
				}
				attributes |= MethodAttributes.SpecialName;
				attributes |= MethodAttributes.RTSpecialName;

				// Set the internal state.
				this.type = type;
				this.numParams = (parameterTypes != null
									? parameterTypes.Length : 0);
				this.ilGenerator = null;
				this.initLocals = true;

				// Register this item to be detached later.
				type.module.assembly.AddDetach(this);

				// Create the signature.
				helper = SignatureHelper.GetMethodSigHelper
						(type.module, callingConvention,
						 (CallingConvention)0,
						 typeof(void), parameterTypes);

				// Create the constructor method.
				lock(typeof(AssemblyBuilder))
				{
					this.privateData = MethodBuilder.ClrMethodCreate
						(((IClrProgramItem)type).ClrHandle, name,
						 attributes, helper.sig);
				}

				// Add the constructor to the type for post-processing.
				type.AddMethod(this);
			}

	// Add declarative security to this constructor.
	public void AddDeclarativeSecurity(SecurityAction action, 
										PermissionSet pset)
			{
				try
				{
					type.StartSync();
					type.module.assembly.AddDeclarativeSecurity
						(this, action, pset);
				}
				finally
				{
					type.EndSync();
				}
			}

	// Define a parameter information block for this constructor.
	public ParameterBuilder DefineParameter(int iSequence, 
											ParameterAttributes attributes, 
											String strParamName)
			{
				try
				{
					type.StartSync();
					if(iSequence <= 0 || iSequence > numParams)
					{
						throw new ArgumentOutOfRangeException
							("iSequence", _("Emit_InvalidParamNum"));
					}
					return new ParameterBuilder
						(type, this, iSequence, attributes, strParamName);
				}
				finally
				{
					type.EndSync();
				}
			}

	// Get the custom attributes for this constructor.
	public override Object[] GetCustomAttributes(bool inherit)
			{
				throw new NotSupportedException(_("NotSupp_Builder"));
			}
	public override Object[] GetCustomAttributes(Type attribute_type, 
												 bool inherit)
			{
				throw new NotSupportedException(_("NotSupp_Builder"));
			}

	// Get an IL code generator for this constructor.
	public ILGenerator GetILGenerator()
			{
				if(ilGenerator == null)
				{
					ilGenerator = new ILGenerator(type.module, 64);
				}
				return ilGenerator;
			}

	// Get the implementation attributes for this constructor.
	public override MethodImplAttributes GetMethodImplementationFlags()
			{
				lock(typeof(AssemblyBuilder))
				{
					return ClrHelpers.GetImplAttrs(privateData);
				}
			}

	// Get the method that contains this constructor.
	public Module GetModule()
			{
				return type.module;
			}

	// Get the parameter information for this constructor.
	public override ParameterInfo[] GetParameters()
			{
				lock(typeof(AssemblyBuilder))
				{
					int param;
					ParameterInfo[] parameters = new ParameterInfo [numParams];
					for(param = 0; param < numParams; ++param)
					{
						parameters[param] =
							ClrHelpers.GetParameterInfo(this, this, param + 1);
					}
					return parameters;
				}
			}

	// Get the token for this constructor.
	public MethodToken GetToken()
			{
				lock(typeof(AssemblyBuilder))
				{
					return new MethodToken
						(AssemblyBuilder.ClrGetItemToken(privateData));
				}
			}

	// Invoke this constructor.
	public override Object Invoke(Object obj, BindingFlags invokeAttr, 
								  Binder binder, Object[] parameters, 
								  CultureInfo culture)
			{
				throw new NotSupportedException(_("NotSupp_Builder"));
			}
	public override Object Invoke(BindingFlags invokeAttr, 
								  Binder binder, Object[] parameters, 
								  CultureInfo culture)
			{
				throw new NotSupportedException(_("NotSupp_Builder"));
			}
	
	internal override Object InvokeOnEmpty(Object obj, 
								  BindingFlags invokeAttr,
								  Binder binder, Object[] parameters,
								  CultureInfo culture)
			{
				throw new NotSupportedException(_("NotSupp_Builder"));
			}

	// Determine if an attribute is defined on this constructor.
	public override bool IsDefined(Type attribute_type, bool inherit)
			{
				throw new NotSupportedException(_("NotSupp_Builder"));
			}

	// Set a custom attribute on this constructor.
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

	// Set the method implementation attributes for this constructor.
	public void SetImplementationFlags(MethodImplAttributes attributes)
			{
				try
				{
					type.StartSync();
					lock(typeof(AssemblyBuilder))
					{
						MethodBuilder.ClrMethodSetImplAttrs
							(privateData, attributes);
					}
				}
				finally
				{
					type.EndSync();
				}
			}

	// Set symbol attribute information for this constructor.
	public void SetSymCustomAttribute(String name, byte[] data)
			{
				// We don't support symbols at present - ignored.
			}

	// Convert this constructor into a string.
	public override String ToString()
			{
				StringBuilder builder = new StringBuilder();
				builder.Append("Name: ");
				builder.Append(Name);
				builder.Append(Environment.NewLine);
				builder.Append("Attributes: ");
				builder.Append(Attributes.ToString());
				builder.Append(Environment.NewLine);
				builder.Append("Method Signature: ");
				builder.Append(helper.ToString());
				builder.Append(Environment.NewLine);
				builder.Append(Environment.NewLine);
				return builder.ToString();
			}

	// Get the attributes for this constructor.
	public override MethodAttributes Attributes 
			{
				get
				{
					lock(typeof(AssemblyBuilder))
					{
						return (MethodAttributes)
							ClrHelpers.GetMemberAttrs(privateData);
					}
				}
			}

	// Get the type that declares this constructor.
	public override Type DeclaringType 
			{
				get
				{
					return type;
				}
			}

	// Get or set the initalized locals state.
	public bool InitLocals 
			{
				get
				{
					return initLocals;
				}
				set
				{
					initLocals = value;
				}
			}

	// Get the method handle for this constructor.
	public override RuntimeMethodHandle MethodHandle 
			{
				get
				{
					throw new NotSupportedException(_("NotSupp_Builder"));
				}
			}

	// Get the name of this constructor.
	public override String Name 
			{
				get
				{
					lock(typeof(AssemblyBuilder))
					{
						return ClrHelpers.GetName(this);
					}
				}
			}

	// Get the reflected type that owns this constructor.
	public override Type ReflectedType 
			{
				get
				{
					return type;
				}
			}

	// Get the return type for this constructor.
	public Type ReturnType 
			{
				get
				{
					return typeof(void);
				}
			}

	// Get the signature of this constructor as a string.
	public String Signature 
			{
				get
				{
					return helper.ToString();
				}
			}

	// Get the CLR handle for this constructor.
	IntPtr IClrProgramItem.ClrHandle
			{
				get
				{
					return privateData;
				}
			}

	// Finalize this constructor by writing its code to the output image.
	internal void FinalizeConstructor()
			{
				int rva;
				if(ilGenerator != null)
				{
					rva = ilGenerator.WriteCode(initLocals);
				}
				else
				{
					rva = 0;
				}
				lock(typeof(AssemblyBuilder))
				{
					MethodBuilder.ClrMethodSetRVA(privateData, rva);
				}
			}

	// Detach this item.
	void IDetachItem.Detach()
			{
				privateData = IntPtr.Zero;
			}

}; // class ConstructorBuilder

#endif // CONFIG_REFLECTION_EMIT

}; // namespace System.Reflection.Emit
