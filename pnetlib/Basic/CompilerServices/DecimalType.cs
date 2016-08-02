/*
 * DecimalType.cs - Implementation of the
 *			"Microsoft.VisualBasic.DecimalType" class.
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
using System.Globalization;

[StandardModule]
#if CONFIG_COMPONENT_MODEL
[EditorBrowsable(EditorBrowsableState.Never)]
#endif
public sealed class DecimalType
{
	// This class cannot be instantiated.
	private DecimalType() {}

	// Convert a boolean value into a decimal value.
	public static Decimal FromBoolean(bool Value)
			{
				return (Value ? -1.0m : 0.0m);
			}

	// Convert an object into a decimal value.
	public static Decimal FromObject(Object Value)
			{
				return FromObject(Value, null);
			}
	public static Decimal FromObject
				(Object Value, NumberFormatInfo NumberFormat)
			{
			#if !ECMA_COMPAT
				if(Value != null)
				{
					IConvertible ic = (Value as IConvertible);
					if(ic != null)
					{
						if(ic.GetTypeCode() != TypeCode.String)
						{
							return ic.ToDecimal(NumberFormat);
						}
						else
						{
							return FromString(ic.ToString(null), NumberFormat);
						}
					}
					else
					{
						throw new InvalidCastException
							(String.Format
								(S._("VB_InvalidCast"),
								 Value.GetType(), "System.Decimal"));
					}
				}
				else
				{
					return 0.0m;
				}
			#else
				if(Value == null)
				{
					return 0;
				}
				Type type = Value.GetType();
				if(type == typeof(byte))
				{
					return Convert.ToDecimal((byte)Value);
				}
				else if(type == typeof(sbyte))
				{
					return Convert.ToDecimal((sbyte)Value);
				}
				else if(type == typeof(short))
				{
					return Convert.ToDecimal((short)Value);
				}
				else if(type == typeof(ushort))
				{
					return Convert.ToDecimal((ushort)Value);
				}
				else if(type == typeof(char))
				{
					return Convert.ToDecimal((char)Value);
				}
				else if(type == typeof(int))
				{
					return Convert.ToDecimal((int)Value);
				}
				else if(type == typeof(uint))
				{
					return Convert.ToDecimal((uint)Value);
				}
				else if(type == typeof(long))
				{
					return Convert.ToDecimal((long)Value);
				}
				else if(type == typeof(ulong))
				{
					return Convert.ToDecimal((ulong)Value);
				}
				else if(type == typeof(float))
				{
					return Convert.ToDecimal((float)Value);
				}
				else if(type == typeof(double))
				{
					return Convert.ToDecimal((double)Value);
				}
				else if(type == typeof(Decimal))
				{
					return (Decimal)Value;
				}
				else if(type == typeof(String))
				{
					return Convert.ToDecimal((String)Value);
				}
				else if(type == typeof(bool))
				{
					return Convert.ToDecimal((bool)Value);
				}
				else
				{
					throw new InvalidCastException
						(String.Format
							(S._("VB_InvalidCast"),
							 Value.GetType(), "System.Decimal"));
				}
			#endif
			}

	// Convert a string into a decimal value.
	public static Decimal FromString(String Value)
			{
				return FromString(Value, null);
			}
	public static Decimal FromString
				(String Value, NumberFormatInfo NumberFormat)
			{
				if(Value == null)
				{
					return 0.0m;
				}
				try
				{
					long lvalue;
					if(LongType.TryConvertHexOct(Value, out lvalue))
					{
						return (decimal)lvalue;
					}
					return Parse(Value, NumberFormat);
				}
				catch(FormatException)
				{
					throw new InvalidCastException
						(String.Format
							(S._("VB_InvalidCast"),
							 "System.String", "System.Decimal"));
				}
			}

	// Parse a string into a decimal value.
	public static Decimal Parse(String Value, NumberFormatInfo NumberFormat)
			{
				return Decimal.Parse(Utils.FixDigits(Value),
									 NumberStyles.Any, NumberFormat);
			}

}; // class DecimalType

}; // namespace Microsoft.VisualBasic.CompilerServices
