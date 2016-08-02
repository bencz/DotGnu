/*
 * MethodBuilder.cs - Implementation of the
 *		"System.Reflection.Emit.MethodBuilder" class.
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
using System.Security;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

public sealed class MethodBuilder : MethodInfo, IClrProgramItem, IDetachItem
{
	// Internal state.
	internal TypeBuilder type;
	private IntPtr privateData;
	private bool bodySet;
	private bool initLocals;
	private byte[] explicitBody;
	private ILGenerator ilGenerator;
	private Type returnType;
	private ParameterBuilder returnBuilder;
	private SignatureHelper helper;
	internal int numParams;

	// Constructor.
	internal MethodBuilder(TypeBuilder type, String name,
						   MethodAttributes attributes,
						   CallingConventions callingConvention,
						   Type returnType, Type[] parameterTypes)
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
				if(returnType == null)
				{
					returnType = typeof(void);
				}
				if((attributes & MethodAttributes.Static) == 0)
				{
					callingConvention |= CallingConventions.HasThis;
				}
				else if((attributes & MethodAttributes.Virtual) != 0)
				{
					throw new ArgumentException
						(_("Emit_BothStaticAndVirtual"));
				}
				if((type.Attributes & TypeAttributes.ClassSemanticsMask)
						== TypeAttributes.Interface &&
				   (attributes & MethodAttributes.SpecialName) == 0)
				{
					if((attributes & (MethodAttributes.Virtual |
									  MethodAttributes.Abstract))
						!= (MethodAttributes.Virtual |
							MethodAttributes.Abstract))
					{
						throw new ArgumentException
							(_("Emit_InterfaceMethodAttrs"));
					}
				}

				// Set the local state.
				this.type = type;
				this.bodySet = false;
				this.initLocals = true;
				this.explicitBody = null;
				this.ilGenerator = null;
				this.returnType = returnType;
				this.returnBuilder = null;
				this.numParams = (parameterTypes != null
									? parameterTypes.Length : 0);

				// Register this item to be detached later.
				type.module.assembly.AddDetach(this);

				// Create the method signature.
				helper = SignatureHelper.GetMethodSigHelper
						(type.module, callingConvention,
						 (CallingConvention)0,
						 returnType, parameterTypes);

				// Create the method.
				lock(typeof(AssemblyBuilder))
				{
					this.privateData = ClrMethodCreate
						(((IClrProgramItem)type).ClrHandle, name,
						 attributes, helper.sig);
				}

				// Add the method to the type for post-processing.
				type.AddMethod(this);
			}

	// Add declarative security to this method.
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

	// Create the method body from a literal array of IL instructions.
	public void CreateMethodBody(byte[] il, int count)
			{
				try
				{
					type.StartSync();
					if(bodySet)
					{
						throw new InvalidOperationException
							(_("Emit_BodyAlreadySet"));
					}
					if(il == null)
					{
						explicitBody = null;
					}
					else if(count < 0 || count > il.Length)
					{
						throw new ArgumentOutOfRangeException
							("count", _("ArgRange_Array"));
					}
					else
					{
						explicitBody = new byte [count];
						Array.Copy(il, explicitBody, count);
						bodySet = true;
					}
				}
				finally
				{
					type.EndSync();
				}
			}

	// Define a parameter builder for a particular parameter.
	public ParameterBuilder DefineParameter(int position, 
											ParameterAttributes attributes, 
											String strParamName)
			{
				try
				{
					type.StartSync();
					if(position <= 0 || position > numParams)
					{
						throw new ArgumentOutOfRangeException
							("position", _("Emit_InvalidParamNum"));
					}
					return new ParameterBuilder
						(type, this, position, attributes, strParamName);
				}
				finally
				{
					type.EndSync();
				}
			}

	// Return the base definition for a method.
	public override MethodInfo GetBaseDefinition()
			{
				return this;
			}

	// Get custom attributes for this method.
	public override Object[] GetCustomAttributes(bool inherit)
			{
				throw new NotSupportedException(_("NotSupp_Builder"));
			}
	public override Object[] GetCustomAttributes(Type attributeType,
												 bool inherit)
			{
				throw new NotSupportedException(_("NotSupp_Builder"));
			}

	// Get the IL generator to use for the method body.
	public ILGenerator GetILGenerator()
			{
				return GetILGenerator(64);
			}
	public ILGenerator GetILGenerator(int size)
			{
				MethodImplAttributes attrs = GetMethodImplementationFlags();
				if((attrs & (MethodImplAttributes.PreserveSig |
							 MethodImplAttributes.Unmanaged)) != 0 ||
				   (attrs & MethodImplAttributes.CodeTypeMask) !=
				   		MethodImplAttributes.IL ||
				   (Attributes & MethodAttributes.PinvokeImpl) != 0)
				{
					throw new InvalidOperationException
						(_("Emit_CannotHaveBody"));
				}
				if(ilGenerator == null)
				{
					ilGenerator = new ILGenerator(type.module, size);
				}
				return ilGenerator;
			}

	// Get the method implementation attributes for this method.
	public override MethodImplAttributes GetMethodImplementationFlags()
			{
				lock(typeof(AssemblyBuilder))
				{
					return ClrHelpers.GetImplAttrs(privateData);
				}
			}

	// Get the module that owns this method.
	public Module GetModule()
			{
				return type.module;
			}

	// Get the parameters for this method.
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

	// Get the token code for this method.
	public MethodToken GetToken()
			{
				lock(typeof(AssemblyBuilder))
				{
					return new MethodToken
						(AssemblyBuilder.ClrGetItemToken(privateData));
				}
			}

	// Invoke this method.
	public override Object Invoke(Object obj, BindingFlags invokeAttr, 
								  Binder binder, Object[] parameters, 
								  CultureInfo culture)
			{
				throw new NotSupportedException(_("NotSupp_Builder"));
			}

	// Determine if a particular attribute is defined on this method.
	public override bool IsDefined(Type attribute_type, bool inherit)
			{
				throw new NotSupportedException(_("NotSupp_Builder"));
			}

	// Set an attribute on this method.
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

	// Set the implementation flags for this method.
	public void SetImplementationFlags(MethodImplAttributes attributes)
			{
				try
				{
					type.StartSync();
					lock(typeof(AssemblyBuilder))
					{
						ClrMethodSetImplAttrs(privateData, attributes);
					}
				}
				finally
				{
					type.EndSync();
				}
			}

	// Set the marshalling information for the return type.
	public void SetMarshal(UnmanagedMarshal unmanagedMarshal)
			{
				try
				{
					type.StartSync();
					if(returnBuilder != null)
					{
						returnBuilder = new ParameterBuilder
							(type, this, 0, ParameterAttributes.None, null);
					}
					returnBuilder.SetMarshal(unmanagedMarshal);
				}
				finally
				{
					type.EndSync();
				}
			}

	// Determine if two method builders are equal.
	public override bool Equals(Object obj)
			{
				MethodBuilder mb = (obj as MethodBuilder);
				if(mb != null)
				{
					if(Name != mb.Name)
					{
						return false;
					}
					if(Attributes != mb.Attributes)
					{
						return false;
					}
					if(ReturnType != mb.ReturnType)
					{
						return false;
					}
					ParameterInfo[] params1 = GetParameters();
					ParameterInfo[] params2 = mb.GetParameters();
					if(params1.Length != params2.Length)
					{
						return false;
					}
					int index;
					for(index = 0; index < params1.Length; ++index)
					{
						if(params1[index].Attributes !=
						   params2[index].Attributes)
						{
							return false;
						}
						if(params1[index].ParameterType !=
						   params2[index].ParameterType)
						{
							return false;
						}
					}
					return true;
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this method.
	public override int GetHashCode()
			{
				return Name.GetHashCode();
			}

	// Convert this method into a string.
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

	// Set symbol attribute information for this method.
	public void SetSymCustomAttribute(String name, byte[] data)
			{
				// We don't support symbols at present - ignored.
			}

	// Get the attributes for this method.
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

	// Get the calling conventions for this method.
	public override CallingConventions CallingConvention 
			{
				get
				{
					lock(typeof(AssemblyBuilder))
					{
						return ClrHelpers.GetCallConv(privateData);
					}
				}
			}

	// Get the type that declares this method.
	public override Type DeclaringType 
			{
				get
				{
					return type;
				}
			}

	// Get or set the "initialize locals" flag for this method.
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

	// Get the handle for this method.
	public override RuntimeMethodHandle MethodHandle
			{
				get
				{
					throw new NotSupportedException(_("NotSupp_Builder"));
				}
			}

	// Get the name of this method.
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

	// Get the reflected type that contains this method.
	public override Type ReflectedType 
			{
				get
				{
					return type;
				}
			}

	// Get the return type for this method.
	public override Type ReturnType 
			{
				get
				{
					return returnType;
				}
			}

	// Get the string form of the signature of this method.
	public String Signature 
			{
				get
				{
					return helper.ToString();
				}
			}

	// Get the custom attribute information for the return type.
	public override ICustomAttributeProvider ReturnTypeCustomAttributes
			{
				get
				{
					// Nothing to do here.
					return null;
				}
			}

	// Get the CLR handle for this method.
	IntPtr IClrProgramItem.ClrHandle
			{
				get
				{
					return privateData;
				}
			}

	// Finalize this method by writing its code to the output image.
	internal void FinalizeMethod()
			{
				int rva;
				if(bodySet)
				{
					if(explicitBody == null)
					{
						rva = 0;
					}
					else
					{
						rva = ILGenerator.WriteExplicitCode
							(type.module, explicitBody, initLocals);
					}
				}
				else if(ilGenerator != null)
				{
					rva = ilGenerator.WriteCode(initLocals);
				}
				else
				{
					rva = 0;
				}
				lock(typeof(AssemblyBuilder))
				{
					ClrMethodSetRVA(privateData, rva);
				}
			}

	// Detach this item.
	void IDetachItem.Detach()
			{
				privateData = IntPtr.Zero;
			}

	// Create a new method and attach it to a particular class.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static IntPtr ClrMethodCreate
			(IntPtr classInfo, String name,
			 MethodAttributes attributes, IntPtr signature);

	// Set the implementation attributes for a method item.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static void ClrMethodSetImplAttrs
			(IntPtr item, MethodImplAttributes attributes);

	// Create a member reference for a vararg method call.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static int ClrMethodCreateVarArgRef
			(IntPtr module, int methodToken, IntPtr signature);

	// Set the RVA for a method's code.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static void ClrMethodSetRVA(IntPtr method, int rva);

	// Add a PInvoke declaration to a method.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static void ClrMethodAddPInvoke
			(IntPtr method, int pinvAttrs, String dllName, String entryName);

}; // class MethodBuilder

#endif // CONFIG_REFLECTION_EMIT

}; // namespace System.Reflection.Emit
