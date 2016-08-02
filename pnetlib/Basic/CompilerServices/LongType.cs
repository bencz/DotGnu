/*
 * LongType.cs - Implementation of the
 *			"Microsoft.VisualBasic.LongType" class.
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
public sealed class LongType
{
	// This class cannot be instantiated.
	private LongType() {}

	// Convert an object into a long value.
	public static long FromObject(Object Value)
			{
			#if !ECMA_COMPAT
				if(Value != null)
				{
					IConvertible ic = (Value as IConvertible);
					if(ic != null)
					{
						if(ic.GetTypeCode() != TypeCode.String)
						{
							return ic.ToInt64(null);
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
								 Value.GetType(), "System.Int64"));
					}
				}
				else
				{
					return 0;
				}
			#else
				if(Value == null)
				{
					return 0;
				}
				Type type = Value.GetType();
				if(type == typeof(byte))
				{
					return Convert.ToInt64((byte)Value);
				}
				else if(type == typeof(sbyte))
				{
					return Convert.ToInt64((sbyte)Value);
				}
				else if(type == typeof(short))
				{
					return Convert.ToInt64((short)Value);
				}
				else if(type == typeof(ushort))
				{
					return Convert.ToInt64((ushort)Value);
				}
				else if(type == typeof(char))
				{
					return Convert.ToInt64((char)Value);
				}
				else if(type == typeof(int))
				{
					return Convert.ToInt64((int)Value);
				}
				else if(type == typeof(uint))
				{
					return Convert.ToInt64((uint)Value);
				}
				else if(type == typeof(long))
				{
					return (long)Value;
				}
				else if(type == typeof(ulong))
				{
					return Convert.ToInt64((ulong)Value);
				}
#if CONFIG_EXTENDED_NUMERICS
				else if(type == typeof(float))
				{
					return Convert.ToInt64((float)Value);
				}
				else if(type == typeof(double))
				{
					return Convert.ToInt64((double)Value);
				}
				else if(type == typeof(Decimal))
				{
					return Convert.ToInt64((Decimal)Value);
				}
#endif
				else if(type == typeof(String))
				{
					return Convert.ToInt64((String)Value);
				}
				else if(type == typeof(bool))
				{
					return Convert.ToInt64((bool)Value);
				}
				else
				{
					throw new InvalidCastException
						(String.Format
							(S._("VB_InvalidCast"),
							 Value.GetType(), "System.Int64"));
				}
			#endif
			}

	// Try to convert a value using hex or octal forms into "long".
	internal static bool TryConvertHexOct(String str, out long value)
			{
				int len = str.Length;
				int posn = 0;
				char ch;
				while(posn < len)
				{
					ch = str[posn];
					if(ch == '&')
					{
						break;
					}
					else if(ch == ' ' || ch == '\u3000')
					{
						++posn;
					}
					else
					{
						value = 0;
						return false;
					}
				}
				++posn;
				if(posn >= len)
				{
					value = 0;
					return false;
				}
				str = Utils.FixDigits(str);
				ch = str[posn];
				if(ch == 'h' || ch == 'H')
				{
					// Recognize a hexadecimal value.
					++posn;
					value = 0;
					while(posn < len)
					{
						ch = str[posn];
						if(ch >= '0' && ch <= '9')
						{
							value = value * 16 + (int)(ch - '0');
						}
						else if(ch >= 'A' && ch <= 'F')
						{
							value = value * 16 + (int)(ch - 'A' + 10);
						}
						else if(ch >= 'a' && ch <= 'f')
						{
							value = value * 16 + (int)(ch - 'a' + 10);
						}
						else
						{
							break;
						}
						++posn;
					}
					while(posn < len)
					{
						ch = str[posn];
						if(ch != ' ' && ch != '\u3000')
						{
							throw new FormatException
								(S._("VB_InvalidHexOrOct"));
						}
						++posn;
					}
					return true;
				}
				else if(ch == 'o' || ch == 'O')
				{
					// Recognize an octal value.
					++posn;
					value = 0;
					while(posn < len)
					{
						ch = str[posn];
						if(ch >= '0' && ch <= '7')
						{
							value = value * 8 + (int)(ch - '0');
						}
						else
						{
							break;
						}
						++posn;
					}
					while(posn < len)
					{
						ch = str[posn];
						if(ch != ' ' && ch != '\u3000')
						{
							throw new FormatException
								(S._("VB_InvalidHexOrOct"));
						}
						++posn;
					}
					return true;
				}
				else
				{
					// Not a legitimate number format.
					throw new FormatException(S._("VB_InvalidHexOrOct"));
				}
			}

	// Convert a string into a long value.
	public static long FromString(String Value)
			{
				if(Value != null)
				{
					try
					{
						long lvalue;
						if(TryConvertHexOct(Value, out lvalue))
						{
							return lvalue;
						}
						return Convert.ToInt64
							(Math.Round(DoubleType.Parse(Value)));
					}
					catch(OverflowException)
					{
						throw new InvalidCastException
							(String.Format
								(S._("VB_InvalidCast"),
								 "System.String", "System.Int64"));
					}
				}
				else
				{
					return 0;
				}
			}

}; // class LongType

}; // namespace Microsoft.VisualBasic.CompilerServices
