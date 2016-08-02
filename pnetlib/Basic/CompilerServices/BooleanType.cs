/*
 * BooleanType.cs - Implementation of the
 *			"Microsoft.VisualBasic.BooleanType" class.
 *
 * Copyright (C) 2003, 2004  Southern Storm Software, Pty Ltd.
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

[StandardModule]
#if CONFIG_COMPONENT_MODEL
[EditorBrowsable(EditorBrowsableState.Never)]
#endif
public sealed class BooleanType
{
	// This class cannot be instantiated.
	private BooleanType() {}

	// Convert an object into a boolean value.
	public static bool FromObject(Object Value)
			{
			#if !ECMA_COMPAT
				if(Value != null)
				{
					IConvertible ic = (Value as IConvertible);
					if(ic != null)
					{
						if(ic.GetTypeCode() != TypeCode.String)
						{
							return ic.ToBoolean(null);
						}
						else
						{
							return FromString(ic.ToString(null));
						}
					}
					else
					{
						throw new InvalidCastException
							(String.Format
								(S._("VB_InvalidCast"),
								 Value.GetType(), "System.Boolean"));
					}
				}
				else
				{
					return false;
				}
			#else
				if(Value == null)
				{
					return false;
				}
				Type type = Value.GetType();
				if(type == typeof(byte))
				{
					return Convert.ToBoolean((byte)Value);
				}
				else if(type == typeof(sbyte))
				{
					return Convert.ToBoolean((sbyte)Value);
				}
				else if(type == typeof(short))
				{
					return Convert.ToBoolean((short)Value);
				}
				else if(type == typeof(ushort))
				{
					return Convert.ToBoolean((ushort)Value);
				}
				else if(type == typeof(char))
				{
					return Convert.ToBoolean((char)Value);
				}
				else if(type == typeof(int))
				{
					return Convert.ToBoolean((int)Value);
				}
				else if(type == typeof(uint))
				{
					return Convert.ToBoolean((uint)Value);
				}
				else if(type == typeof(long))
				{
					return Convert.ToBoolean((long)Value);
				}
				else if(type == typeof(ulong))
				{
					return Convert.ToBoolean((ulong)Value);
				}
#if CONFIG_EXTENDED_NUMERICS
				else if(type == typeof(float))
				{
					return Convert.ToBoolean((float)Value);
				}
				else if(type == typeof(double))
				{
					return Convert.ToBoolean((double)Value);
				}
				else if(type == typeof(Decimal))
				{
					return Convert.ToBoolean((Decimal)Value);
				}
#endif
				else if(type == typeof(String))
				{
					return Convert.ToBoolean((String)Value);
				}
				else if(type == typeof(bool))
				{
					return (bool)Value;
				}
				else
				{
					throw new InvalidCastException
						(String.Format
							(S._("VB_InvalidCast"),
							 Value.GetType(), "System.Boolean"));
				}
			#endif
			}

	// Convert a string into a boolean value.
	public static bool FromString(String Value)
			{
				if(Value == null)
				{
					Value = String.Empty;
				}
				try
				{
					return Boolean.Parse(Value);
				}
				catch(FormatException)
				{
					return (DoubleType.Parse(Value) != 0.0);
				}
			}

}; // class BooleanType

}; // namespace Microsoft.VisualBasic.CompilerServices
