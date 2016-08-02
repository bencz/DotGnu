/*
 * ArrayConstructor.cs - Object that represents the "Array" constructor.
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
using System.Globalization;
using Microsoft.JScript.Vsa;

public sealed class ArrayConstructor : ScriptFunction
{
	// Constructor.
	internal ArrayConstructor(FunctionPrototype parent)
			: base(parent, "Array", 1) {}

	// Construct a new array from a list of elements.
	public ArrayObject ConstructArray(Object[] args)
			{
				ArrayObject obj = new ArrayObject
					(EngineInstance.GetEngineInstance(engine)
							.GetArrayPrototype());
				int index, len;
				len = args.Length;
				for(index = 0; index < len; ++index)
				{
					obj.PutIndex(index, args[index]);
				}
				return obj;
			}

	// Construct a new "Array" instance.
	[JSFunction(JSFunctionAttributeEnum.HasVarArgs)]
	public new ArrayObject CreateInstance(params Object[] args)
			{
				return (ArrayObject)Construct(engine, args);
			}

	// Invoke this constructor.
	[JSFunction(JSFunctionAttributeEnum.HasVarArgs)]
	public ArrayObject Invoke(params Object[] args)
			{
				return (ArrayObject)Construct(engine, args);
			}

	// Perform a call on this object.
	internal override Object Call
				(VsaEngine engine, Object thisob, Object[] args)
			{
				return Construct(engine, args);
			}

	// Perform a constructor call on this object.
	internal override Object Construct(VsaEngine engine, Object[] args)
			{
				ArrayObject obj;
				int index, len;
				if(args.Length != 1)
				{
					// Construct an array from a list of values.
					obj = new ArrayObject
						(EngineInstance.GetEngineInstance(engine)
								.GetArrayPrototype(), (uint)(args.Length));
					len = args.Length;
					for(index = 0; index < len; ++index)
					{
						obj.PutIndex(index, args[index]);
					}
				}
				else if(args[0] is Array)
				{
					// Wrap an existing array.
					Array array = (Array)(args[0]);
					if(array.Rank != 1)
					{
						throw new JScriptException(JSError.TypeMismatch);
					}
					obj = new ArrayObject.Wrapper
						(EngineInstance.GetEngineInstance(engine)
								.GetArrayPrototype(), (Array)(args[0]));
				}
				else
				{
					// Construct an array from a length value.
					switch(Support.TypeCodeForObject(args[0]))
					{
						case TypeCode.Byte:
						case TypeCode.SByte: 
						case TypeCode.Char: 
						case TypeCode.Int16: 
						case TypeCode.UInt16:
						case TypeCode.Int32:
						case TypeCode.UInt32:
						case TypeCode.Int64:
						case TypeCode.UInt64:
						case TypeCode.Single:
						case TypeCode.Double:
						case TypeCode.Decimal:
						{
							double num = Convert.ToNumber(args[0]);
							uint inum = Convert.ToUInt32(args[0]);
							if(num == (double)inum)
							{
								return new ArrayObject
									(EngineInstance.GetEngineInstance(engine)
											.GetArrayPrototype(), inum);
							}
							else
							{
								throw new JScriptException
									(JSError.ArrayLengthConstructIncorrect);
							}
						}
						// Not reached.
					}

					// The length is not numeric, so it is actually a value.
					obj = new ArrayObject
						(EngineInstance.GetEngineInstance(engine)
								.GetArrayPrototype());
					obj.PutIndex(0, args[0]);
				}
				return obj;
			}

}; // class ArrayConstructor

}; // namespace Microsoft.JScript
