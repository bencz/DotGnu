/*
 * SignatureHelper.cs - Implementation of the
 *		"System.Reflection.Emit.SignatureHelper" class.
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
using System.Reflection;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

public sealed class SignatureHelper : IDetachItem
{
	// Internal state.
	private Module mod;
	private IntPtr context;
	internal IntPtr sig;
	internal int numArgs;
	private CallingConvention callConv;
	private bool field;
	private long bytesOffset;

	// Constructor.
	private SignatureHelper(Module mod, IntPtr context)
			{
				this.mod = mod;
				this.context = context;
				this.sig = IntPtr.Zero;
				this.numArgs = 0;
				this.callConv = (CallingConvention)0;
				this.field = true;
				this.bytesOffset = -1;
				((ModuleBuilder)mod).assembly.AddDetach(this);
			}
	private SignatureHelper(Module mod, IntPtr context, IntPtr sig)
			{
				this.mod = mod;
				this.context = context;
				this.sig = sig;
				this.numArgs = 0;
				this.callConv = (CallingConvention)0;
				this.field = false;
				this.bytesOffset = -1;
				((ModuleBuilder)mod).assembly.AddDetach(this);
			}
	private SignatureHelper(Module mod, IntPtr context, IntPtr sig,
							CallingConvention callConv)
			{
				this.mod = mod;
				this.context = context;
				this.sig = sig;
				this.numArgs = 0;
				this.callConv = callConv;
				this.field = false;
				this.bytesOffset = -1;
				((ModuleBuilder)mod).assembly.AddDetach(this);
			}

	// Convert a module into an ILContext value.
	private static IntPtr ModuleToContext(Module mod)
			{
				if(mod == null)
				{
					throw new ArgumentNullException("mod");
				}
				else if(!(mod is ModuleBuilder))
				{
					throw new NotSupportedException
						(_("Emit_NeedDynamicModule"));
				}
				return ClrSigModuleToContext(mod.privateData);
			}

	// Convert a C# type into an ILType value.
	private static IntPtr CSToILType(Module mod, IntPtr context, Type type)
			{
				if(type == null)
				{
					return ClrSigCreatePrimitive(context, typeof(void));
				}
				else if(type.IsPrimitive || type == typeof(void))
				{
					return ClrSigCreatePrimitive(context, type);
				}
				else if(type.IsArray)
				{
					return ClrSigCreateArray
						(context, type.GetArrayRank(),
						 CSToILType(mod, context, type.GetElementType()));
				}
				else if(type.IsPointer)
				{
					return ClrSigCreatePointer
						(context,
						 CSToILType(mod, context, type.GetElementType()));
				}
				else if(type.IsByRef)
				{
					return ClrSigCreateByRef
						(context,
						 CSToILType(mod, context, type.GetElementType()));
				}
				else if(type.HasGenericArguments ||
						type.HasGenericParameters)
				{
					throw new NotSupportedException
						(_("Emit_GenericsNotSupported"));
				}
				else if(type.IsValueType)
				{
					return ClrSigCreateValueType
						(mod.privateData,
						 (((ModuleBuilder)mod).GetTypeToken(type)).Token);
				}
				else
				{
					return ClrSigCreateClass
						(mod.privateData,
						 (((ModuleBuilder)mod).GetTypeToken(type)).Token);
				}
			}
	internal static IntPtr CSToILType(Module mod, Type type)
			{
				lock(typeof(AssemblyBuilder))
				{
					return CSToILType(mod, ModuleToContext(mod), type);
				}
			}

	// Create a signature helper for a field signature.
	public static SignatureHelper GetFieldSigHelper(Module mod)
			{
				lock(typeof(AssemblyBuilder))
				{
					IntPtr context = ModuleToContext(mod);
					return new SignatureHelper(mod, context);
				}
			}

	// Create a signature helper for a local variable signature.
	public static SignatureHelper GetLocalVarSigHelper(Module mod)
			{
				lock(typeof(AssemblyBuilder))
				{
					IntPtr context = ModuleToContext(mod);
					return new SignatureHelper
						(mod, context, ClrSigCreateLocal(context));
				}
			}

	// Create a signature helper for a method signature.
	internal static SignatureHelper GetMethodSigHelper
				(Module mod, CallingConventions callConv,
				 CallingConvention unmanagedCallConv, Type returnType,
				 Type[] parameterTypes)
			{
				lock(typeof(AssemblyBuilder))
				{
					// Convert the module into a signature create context.
					IntPtr context = ModuleToContext(mod);

					// Determine the calling convention flags to use.
					int conv = 0;		/* default */
					if((callConv & CallingConventions.VarArgs) != 0)
					{
						conv = 0x05;	/* vararg */
					}
					if((callConv & CallingConventions.HasThis) != 0)
					{
						conv |= 0x20;	/* hasthis */
					}
					if((callConv & CallingConventions.ExplicitThis) != 0)
					{
						conv |= 0x40;	/* explicitthis */
					}

					// Create the basic signature helper.
					IntPtr sig = ClrSigCreateMethod
						(context, conv, CSToILType(mod, context, returnType));
					SignatureHelper helper = new SignatureHelper
						(mod, context, sig, unmanagedCallConv);

					// Add the parameters to the helper.
					if(parameterTypes != null)
					{
						foreach(Type type in parameterTypes)
						{
							helper.AddArgument(type);
						}
					}
					return helper;
				}
			}
	public static SignatureHelper GetMethodSigHelper
				(Module mod, CallingConvention unmanagedCallConv,
				 Type returnType)
			{
				return GetMethodSigHelper
					(mod, CallingConventions.Standard,
					 unmanagedCallConv, returnType, null);
			}
	public static SignatureHelper GetMethodSigHelper
				(Module mod, CallingConventions callingConvention,
				 Type returnType)
			{
				return GetMethodSigHelper
					(mod, callingConvention, (CallingConvention)0,
					 returnType, null);
			}
	public static SignatureHelper GetMethodSigHelper
				(Module mod, Type returnType, Type[] parameterTypes)
			{
				return GetMethodSigHelper
					(mod, CallingConventions.Standard,
					 (CallingConvention)0, returnType, parameterTypes);
			}
	internal static SignatureHelper GetMethodSigHelper
				(Module mod, MethodToken token)
			{
				lock(typeof(AssemblyBuilder))
				{
					IntPtr context = ModuleToContext(mod);
					IntPtr sig = ClrSigCreateMethodCopy
						(context, mod.privateData, token.Token);
					return new SignatureHelper(mod, context, sig);
				}
			}

	// Create a signature helper for a property signature.
	public static SignatureHelper GetPropertySigHelper
				(Module mod, Type returnType, Type[] parameterTypes)
			{
				lock(typeof(AssemblyBuilder))
				{
					// Convert the module into a signature create context.
					IntPtr context = ModuleToContext(mod);

					// Create the basic signature helper.
					IntPtr sig = ClrSigCreateProperty
						(context, CSToILType(mod, context, returnType));
					SignatureHelper helper = new SignatureHelper
						(mod, context, sig);

					// Add the parameters to the helper.
					if(parameterTypes != null)
					{
						foreach(Type type in parameterTypes)
						{
							helper.AddArgument(type);
						}
					}
					return helper;
				}
			}

	// Add an argument type to a signature.
	public void AddArgument(Type clsArgument)
			{
				lock(typeof(AssemblyBuilder))
				{
					if (bytesOffset != -1)
					{
						throw new ArgumentException(/* TODO */);
					}
					if(clsArgument == null)
					{
						throw new ArgumentNullException("clsArgument");
					}
					IntPtr type = CSToILType(mod, context, clsArgument);
					if (field && sig == IntPtr.Zero)
					{
						sig = type;
					}
					else if (field)
					{
						throw new InvalidOperationException
							(_("Emit_InvalidSigArgument"));
					}
					else if(!ClrSigAddArgument(context, sig, type))
					{
						throw new InvalidOperationException
							(_("Emit_InvalidSigArgument"));
					}
					++numArgs;
				}
			}

	// Add a vararg sentinel to a signature.
	public void AddSentinel()
			{
				lock(typeof(AssemblyBuilder))
				{
					if (bytesOffset != -1)
					{
						throw new ArgumentException(/* TODO */);
					}
					if (field)
					{
						throw new InvalidOperationException
							(_("Emit_InvalidSigArgument"));
					}
					if(!ClrSigAddSentinel(context, sig))
					{
						throw new InvalidOperationException
							(_("Emit_InvalidSigArgument"));
					}
				}
			}

	// Determine if two signatures are equal.
	public override bool Equals(Object obj)
			{
				lock(typeof(AssemblyBuilder))
				{
					SignatureHelper helper = (obj as SignatureHelper);
					if(helper != null &&
					   helper.mod == mod &&
					   helper.field == field)
					{
						return ClrSigIdentical(sig, helper.sig);
					}
					else
					{
						return false;
					}
				}
			}

	// Get the hash code for a signature.
	public override int GetHashCode()
			{
				lock(typeof(AssemblyBuilder))
				{
					return ClrSigGetHashCode(sig);
				}
			}

	// Convert the signature into an array of bytes.
	public byte[] GetSignature()
			{
				lock(typeof(AssemblyBuilder))
				{
					if (bytesOffset == -1)
					{
						bytesOffset = ClrSigFinalize
							(mod.privateData, sig, field);
					}
					return ClrSigGetBytes
						(mod.privateData, bytesOffset);
				}
			}

	// Get a token for a stand alone signature made from this signature.
	internal int StandAloneToken()
			{
				if (sig == IntPtr.Zero)
				{
					return 0;
				}
				return ClrStandAloneToken(mod.privateData, sig);
			}

	// Convert this signature into a string.
	public override String ToString()
			{
				byte[] bytes = GetSignature();
				StringBuilder builder = new StringBuilder();
				builder.Append("Length: " + bytes.Length.ToString() +
							   Environment.NewLine);
				if(bytes[0] == 0x06)	/* field */
				{
					builder.Append("Field Signature" + Environment.NewLine);
				}
				else
				{
					builder.Append("Arguments: " + numArgs.ToString() +
								   Environment.NewLine);
				}
				builder.Append("Signature:" + Environment.NewLine);
				foreach(byte val in bytes)
				{
					builder.Append(val.ToString() + " ");
				}
				builder.Append(Environment.NewLine);
				return builder.ToString();
			}

	// Detach this item.
	void IDetachItem.Detach()
			{
				context = IntPtr.Zero;
				sig = IntPtr.Zero;
			}

	// Internal version of "ModuleToContext".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static IntPtr ClrSigModuleToContext(IntPtr module);

	// Create a primitive type value.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static IntPtr ClrSigCreatePrimitive
			(IntPtr context, Type type);

	// Create an array type value.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static IntPtr ClrSigCreateArray
			(IntPtr context, int rank, IntPtr elemType);

	// Create a pointer type value.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static IntPtr ClrSigCreatePointer
			(IntPtr context, IntPtr elemType);

	// Create a by-reference type value.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static IntPtr ClrSigCreateByRef
			(IntPtr context, IntPtr elemType);

	// Create a value type value.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static IntPtr ClrSigCreateValueType
			(IntPtr module, int typeToken);

	// Create a class type value.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static IntPtr ClrSigCreateClass
			(IntPtr module, int typeToken);

	// Create a local variable signature type value.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static IntPtr ClrSigCreateLocal(IntPtr context);

	// Create a method signature type value.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static IntPtr ClrSigCreateMethod
			(IntPtr context, int callConv, IntPtr returnType);

	// Create a method signature type value as a copy of another.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static IntPtr ClrSigCreateMethodCopy
			(IntPtr context, IntPtr module, int methodToken);

	// Create a property signature type value.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static IntPtr ClrSigCreateProperty
			(IntPtr context, IntPtr returnType);

	// Add an argument to a signature.  Returns false if args not allowed.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static bool ClrSigAddArgument
			(IntPtr context, IntPtr sig, IntPtr arg);

	// Add a sentinel to a signature.  Returns false if args not allowed.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static bool ClrSigAddSentinel(IntPtr context, IntPtr sig);

	// Determine if two signatures are identical.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static bool ClrSigIdentical(IntPtr sig1, IntPtr sig2);

	// Get the hash code for a signature.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static int ClrSigGetHashCode(IntPtr sig);

	// Write the signature to the blob and get its offset.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static long ClrSigFinalize
			(IntPtr module, IntPtr sig, bool field);

	// Get the bytes of a signature from the blob.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static byte[] ClrSigGetBytes(IntPtr module, long offset);

	// Create an ILStandAloneSig from the signature and return its token.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static int ClrStandAloneToken(IntPtr module, IntPtr sig);

}; // class SignatureHelper

#endif // CONFIG_REFLECTION_EMIT

}; // namespace System.Reflection.Emit
