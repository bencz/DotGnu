/*
 * ObjectType.cs - Implementation of the
 *			"Microsoft.VisualBasic.ObjectType" class.
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

#if CONFIG_COMPONENT_MODEL
[EditorBrowsable(EditorBrowsableState.Never)]
#endif
public sealed class ObjectType
{
	// Constructor.
	public ObjectType() {}

	// Get the common type for two objects.
	internal static TypeCode CommonType(Object o1, Object o2,
									    TypeCode tc2, bool compare)
			{
				TypeCode tc1;

				// Get the type codes for the arguments.
				if(o1 == null)
				{
					o1 = o2;
				}
				else if(o2 == null)
				{
					o2 = o1;
				}
			#if !ECMA_COMPAT
				if(o1 is IConvertible)
				{
					tc1 = ((IConvertible)o1).GetTypeCode();
				}
				else if(o1 is char[])
				{
					tc1 = TypeCode.String;
				}
				else
				{
					tc1 = TypeCode.Empty;
				}
				if(tc2 == (TypeCode)(-1))
				{
					if(o2 is IConvertible)
					{
						tc2 = ((IConvertible)o2).GetTypeCode();
					}
					else if(o2 is char[])
					{
						tc2 = TypeCode.String;
					}
					else
					{
						tc2 = TypeCode.Empty;
					}
				}
			#else
				if(o1 is char[])
				{
					tc1 = TypeCode.String;
				}
				else
				{
					tc1 = GetTypeCode(o1);
				}
				if(tc2 == (TypeCode)(-1))
				{
					if(o2 is char[])
					{
						tc2 = TypeCode.String;
					}
					else
					{
						tc2 = GetTypeCode(o2);
					}
				}
			#endif

				// Handle the special case of string comparisons.
				if(compare)
				{
					if(tc1 == TypeCode.String || tc2 == TypeCode.String)
					{
						return TypeCode.String;
					}
				}

				// Determine the common numeric type to use.
				switch(tc1)
				{
					case TypeCode.Boolean:
					{
						switch(tc2)
						{
							case TypeCode.Boolean:	return TypeCode.Boolean;
							case TypeCode.Byte:
							case TypeCode.Int16:
							case TypeCode.Int32:
							case TypeCode.Int64:
							case TypeCode.Single:
							case TypeCode.Double:
							case TypeCode.Decimal:	return tc2;
							case TypeCode.String:	return TypeCode.Double;
						}
					}
					break;

					case TypeCode.Byte:
					{
						switch(tc2)
						{
							case TypeCode.Boolean:	return TypeCode.Byte;
							case TypeCode.Byte:
							case TypeCode.Int16:
							case TypeCode.Int32:
							case TypeCode.Int64:
							case TypeCode.Single:
							case TypeCode.Double:
							case TypeCode.Decimal:	return tc2;
							case TypeCode.String:	return TypeCode.Double;
						}
					}
					break;

					case TypeCode.Int16:
					{
						switch(tc2)
						{
							case TypeCode.Boolean:
							case TypeCode.Byte:
							case TypeCode.Int16:	return TypeCode.Int16;
							case TypeCode.Int32:
							case TypeCode.Int64:
							case TypeCode.Single:
							case TypeCode.Double:
							case TypeCode.Decimal:	return tc2;
							case TypeCode.String:	return TypeCode.Double;
						}
					}
					break;

					case TypeCode.Int32:
					{
						switch(tc2)
						{
							case TypeCode.Boolean:
							case TypeCode.Byte:
							case TypeCode.Int16:
							case TypeCode.Int32:	return TypeCode.Int32;
							case TypeCode.Int64:
							case TypeCode.Single:
							case TypeCode.Double:
							case TypeCode.Decimal:	return tc2;
							case TypeCode.String:	return TypeCode.Double;
						}
					}
					break;

					case TypeCode.Int64:
					{
						switch(tc2)
						{
							case TypeCode.Boolean:
							case TypeCode.Byte:
							case TypeCode.Int16:
							case TypeCode.Int32:
							case TypeCode.Int64:	return TypeCode.Int64;
							case TypeCode.Single:
							case TypeCode.Double:
							case TypeCode.Decimal:	return tc2;
							case TypeCode.String:	return TypeCode.Double;
						}
					}
					break;

					case TypeCode.Single:
					{
						switch(tc2)
						{
							case TypeCode.Boolean:
							case TypeCode.Byte:
							case TypeCode.Int16:
							case TypeCode.Int32:
							case TypeCode.Int64:
							case TypeCode.Single:	return TypeCode.Single;
							case TypeCode.Double:
							case TypeCode.Decimal:	return tc2;
							case TypeCode.String:	return TypeCode.Double;
						}
					}
					break;

					case TypeCode.Double:
					case TypeCode.String:
					{
						switch(tc2)
						{
							case TypeCode.Boolean:
							case TypeCode.Byte:
							case TypeCode.Int16:
							case TypeCode.Int32:
							case TypeCode.Int64:
							case TypeCode.Single:
							case TypeCode.Double:
							case TypeCode.Decimal:
							case TypeCode.String:	return TypeCode.Double;
						}
					}
					break;

					case TypeCode.Decimal:
					{
						switch(tc2)
						{
							case TypeCode.Boolean:
							case TypeCode.Byte:
							case TypeCode.Int16:
							case TypeCode.Int32:
							case TypeCode.Int64:
							case TypeCode.Decimal:	return TypeCode.Decimal;
							case TypeCode.Single:
							case TypeCode.Double:
							case TypeCode.String:	return TypeCode.Double;
						}
					}
					break;
				}

				// We could not determine a common type.
				return TypeCode.Empty;
			}
	private static TypeCode CommonType(Object o1, Object o2, bool compare)
			{
				return CommonType(o1, o2, (TypeCode)(-1), compare);
			}

	// Get the common type for two objects, and determine the enum type.
	internal static TypeCode CommonType(Object o1, Object o2, out Type enumType)
			{
				if(o1 is Enum)
				{
					enumType = o1.GetType();
				}
				else if(o2 is Enum)
				{
					enumType = o2.GetType();
				}
				else
				{
					enumType = null;
				}
				return CommonType(o1, o2, (TypeCode)(-1), false);
			}

	// Get the type code for an object.
	internal static TypeCode GetTypeCode(Object obj)
			{
			#if !ECMA_COMPAT
				IConvertible ic = (obj as IConvertible);
				if(ic != null)
				{
					return ic.GetTypeCode();
				}
				else
				{
					return TypeCode.Empty;
				}
			#else
				if(obj == null)
				{
					return TypeCode.Empty;
				}
				if(obj is Boolean)
				{
					return TypeCode.Boolean;
				}
				else if(obj is Char)
				{
					return TypeCode.Char;
				}
				else if(obj is SByte)
				{
					return TypeCode.SByte;
				}
				else if(obj is Byte)
				{
					return TypeCode.Byte;
				}
				else if(obj is Int16)
				{
					return TypeCode.Int16;
				}
				else if(obj is UInt16)
				{
					return TypeCode.UInt16;
				}
				else if(obj is Int32)
				{
					return TypeCode.Int32;
				}
				else if(obj is UInt32)
				{
					return TypeCode.UInt32;
				}
				else if(obj is Int64)
				{
					return TypeCode.Int64;
				}
				else if(obj is UInt64)
				{
					return TypeCode.UInt64;
				}
				else if(obj is Single)
				{
					return TypeCode.Single;
				}
				else if(obj is Double)
				{
					return TypeCode.Double;
				}
				else if(obj is Decimal)
				{
					return TypeCode.Decimal;
				}
				else if(obj is DateTime)
				{
					return TypeCode.DateTime;
				}
				else if(obj is String)
				{
					return TypeCode.String;
				}
				else
				{
					return TypeCode.Empty;
				}
			#endif
			}

	// Add two objects.
	public static Object AddObj(Object o1, Object o2)
			{
				switch(CommonType(o1, o2, false))
				{
					case TypeCode.Boolean:
					{
						return (short)((BooleanType.FromObject(o1) ? -1 : 0) +
							           (BooleanType.FromObject(o2) ? -1 : 0));
					}
					// Not reached.

					case TypeCode.Byte:
					{
						int bp = ByteType.FromObject(o1) +
								 ByteType.FromObject(o2);
						if(bp <= 255)
						{
							return (byte)bp;
						}
						else if(bp <= 32767)
						{
							return (short)bp;
						}
						else
						{
							return bp;
						}
					}
					// Not reached.

					case TypeCode.Int16:
					{
						int sp = ShortType.FromObject(o1) +
								 ShortType.FromObject(o2);
						if(sp >= -32768 && sp <= 32767)
						{
							return (short)sp;
						}
						else
						{
							return sp;
						}
					}
					// Not reached.

					case TypeCode.Int32:
					{
						int i1 = IntegerType.FromObject(o1);
						int i2 = IntegerType.FromObject(o2);
						try
						{
							checked
							{
								return i1 + i2;
							}
						}
						catch(OverflowException)
						{
							return ((long)i1) + ((long)i2);
						}
					}
					// Not reached.

					case TypeCode.Int64:
					{
						long l1 = LongType.FromObject(o1);
						long l2 = LongType.FromObject(o2);
						try
						{
							checked
							{
								return l1 + l2;
							}
						}
						catch(OverflowException)
						{
							try
							{
								return ((decimal)l1) + ((decimal)l2);
							}
							catch(OverflowException)
							{
								return ((double)l1) + ((double)l2);
							}
						}
					}
					// Not reached.

					case TypeCode.Single:
					{
						return SingleType.FromObject(o1) +
							   SingleType.FromObject(o2);
					}
					// Not reached.

					case TypeCode.Double:
					case TypeCode.String:
					{
						return DoubleType.FromObject(o1) +
							   DoubleType.FromObject(o2);
					}
					// Not reached.

					case TypeCode.Decimal:
					{
						decimal d1 = DecimalType.FromObject(o1);
						decimal d2 = DecimalType.FromObject(o2);
						try
						{
							checked
							{
								return d1 + d2;
							}
						}
						catch(OverflowException)
						{
							return ((double)d1) + ((double)d2);
						}
					}
					// Not reached.
				}
				throw new InvalidCastException(S._("VB_InvalidAddArguments"));
			}

	// Bitwise AND of two objects.
	public static Object BitAndObj(Object o1, Object o2)
			{
				Type enumType;
				Object value;
				switch(CommonType(o1, o2, out enumType))
				{
					case TypeCode.Boolean:
					{
						return BooleanType.FromObject(o1) &&
							   BooleanType.FromObject(o2);
					}
					break;

					case TypeCode.Byte:
					{
						value = (byte)(ByteType.FromObject(o1) &
								       ByteType.FromObject(o2));
					}
					break;

					case TypeCode.Int16:
					{
						value = (short)(ShortType.FromObject(o1) &
										ShortType.FromObject(o2));
					}
					break;

					case TypeCode.Int32:
					{
						value = (int)(IntegerType.FromObject(o1) &
									  IntegerType.FromObject(o2));
					}
					break;

					case TypeCode.Int64:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.String:
					case TypeCode.Decimal:
					{
						value = (long)(LongType.FromObject(o1) &
									   LongType.FromObject(o2));
					}
					break;

					default:
					{
						throw new InvalidCastException
							(S._("VB_InvalidBitAndArguments"));
					}
					// Not reached.
				}
				if(enumType == null)
				{
					return value;
				}
				else
				{
					return Enum.ToObject(enumType, value);
				}
			}

	// Bitwise OR of two objects.
	public static Object BitOrObj(Object o1, Object o2)
			{
				Type enumType;
				Object value;
				switch(CommonType(o1, o2, out enumType))
				{
					case TypeCode.Boolean:
					{
						return BooleanType.FromObject(o1) ||
							   BooleanType.FromObject(o2);
					}
					break;

					case TypeCode.Byte:
					{
						value = (byte)(ByteType.FromObject(o1) |
								       ByteType.FromObject(o2));
					}
					break;

					case TypeCode.Int16:
					{
						value = (short)(ShortType.FromObject(o1) |
										ShortType.FromObject(o2));
					}
					break;

					case TypeCode.Int32:
					{
						value = (int)(IntegerType.FromObject(o1) |
									  IntegerType.FromObject(o2));
					}
					break;

					case TypeCode.Int64:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.String:
					case TypeCode.Decimal:
					{
						value = (long)(LongType.FromObject(o1) |
									   LongType.FromObject(o2));
					}
					break;

					default:
					{
						throw new InvalidCastException
							(S._("VB_InvalidBitOrArguments"));
					}
					// Not reached.
				}
				if(enumType == null)
				{
					return value;
				}
				else
				{
					return Enum.ToObject(enumType, value);
				}
			}

	// Bitwise XOR of two objects.
	public static Object BitXorObj(Object o1, Object o2)
			{
				Type enumType;
				Object value;
				switch(CommonType(o1, o2, out enumType))
				{
					case TypeCode.Boolean:
					{
						return (((BooleanType.FromObject(o1) ? -1 : 0) ^
							     (BooleanType.FromObject(o2) ? -1 : 0)) != 0);
					}
					break;

					case TypeCode.Byte:
					{
						value = (byte)(ByteType.FromObject(o1) ^
								       ByteType.FromObject(o2));
					}
					break;

					case TypeCode.Int16:
					{
						value = (short)(ShortType.FromObject(o1) ^
										ShortType.FromObject(o2));
					}
					break;

					case TypeCode.Int32:
					{
						value = (int)(IntegerType.FromObject(o1) ^
									  IntegerType.FromObject(o2));
					}
					break;

					case TypeCode.Int64:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.String:
					case TypeCode.Decimal:
					{
						value = (long)(LongType.FromObject(o1) ^
									   LongType.FromObject(o2));
					}
					break;

					default:
					{
						throw new InvalidCastException
							(S._("VB_InvalidBitXorArguments"));
					}
					// Not reached.
				}
				if(enumType == null)
				{
					return value;
				}
				else
				{
					return Enum.ToObject(enumType, value);
				}
			}

	// Divide two objects.
	public static Object DivObj(Object o1, Object o2)
			{
				switch(CommonType(o1, o2, false))
				{
					case TypeCode.Boolean:
					{
						return (BooleanType.FromObject(o1) ? -1.0 : 0.0) *
							   (BooleanType.FromObject(o2) ? -1.0 : 0.0);
					}
					// Not reached.

					case TypeCode.Single:
					{
						return SingleType.FromObject(o1) /
							   SingleType.FromObject(o2);
					}
					// Not reached.

					case TypeCode.Byte:
					case TypeCode.Int16:
					case TypeCode.Int32:
					case TypeCode.Int64:
					case TypeCode.Double:
					case TypeCode.String:
					{
						return DoubleType.FromObject(o1) /
							   DoubleType.FromObject(o2);
					}
					// Not reached.

					case TypeCode.Decimal:
					{
						decimal d1 = DecimalType.FromObject(o1);
						decimal d2 = DecimalType.FromObject(o2);
						try
						{
							checked
							{
								return d1 / d2;
							}
						}
						catch(OverflowException)
						{
							return ((float)d1) / ((float)d2);
						}
					}
					// Not reached.
				}
				throw new InvalidCastException(S._("VB_InvalidDivArguments"));
			}

	// Convert an object into its primitive form.
	public static Object GetObjectValuePrimitive(Object o)
			{
			#if !ECMA_COMPAT
				if(o == null)
				{
					return null;
				}
				switch(GetTypeCode(o))
				{
					case TypeCode.Boolean:
						return ((IConvertible)o).ToBoolean(null);

					case TypeCode.Char:
						return ((IConvertible)o).ToChar(null);

					case TypeCode.Byte:
						return ((IConvertible)o).ToByte(null);

					case TypeCode.SByte:
						return ((IConvertible)o).ToSByte(null);

					case TypeCode.Int16:
						return ((IConvertible)o).ToInt16(null);

					case TypeCode.UInt16:
						return ((IConvertible)o).ToUInt16(null);

					case TypeCode.Int32:
						return ((IConvertible)o).ToInt32(null);

					case TypeCode.UInt32:
						return ((IConvertible)o).ToUInt32(null);

					case TypeCode.Int64:
						return ((IConvertible)o).ToInt64(null);

					case TypeCode.UInt64:
						return ((IConvertible)o).ToUInt64(null);

					case TypeCode.Single:
						return ((IConvertible)o).ToSingle(null);

					case TypeCode.Double:
						return ((IConvertible)o).ToDouble(null);

					case TypeCode.Decimal:
						return ((IConvertible)o).ToDecimal(null);

					case TypeCode.String:
						return ((IConvertible)o).ToString(null);

					case TypeCode.DateTime:
						return ((IConvertible)o).ToDateTime(null);
				}
			#endif // !ECMA_COMPAT
				return o;
			}

	// Integer divide two objects.
	public static Object IDivObj(Object o1, Object o2)
			{
				switch(CommonType(o1, o2, false))
				{
					case TypeCode.Boolean:
					{
						return (short)((BooleanType.FromObject(o1) ? -1 : 0) /
							           (BooleanType.FromObject(o2) ? -1 : 0));
					}
					// Not reached.

					case TypeCode.Byte:
					{
						return (byte)(ByteType.FromObject(o1) /
								      ByteType.FromObject(o2));
					}
					// Not reached.

					case TypeCode.Int16:
					{
						return (short)(ShortType.FromObject(o1) /
								 	   ShortType.FromObject(o2));
					}
					// Not reached.

					case TypeCode.Int32:
					{
						return (int)(IntegerType.FromObject(o1) /
									 IntegerType.FromObject(o2));
					}
					// Not reached.

					case TypeCode.Int64:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.String:
					case TypeCode.Decimal:
					{
						return (long)(LongType.FromObject(o1) /
									  LongType.FromObject(o2));
					}
					// Not reached.
				}
				throw new InvalidCastException(S._("VB_InvalidIDivArguments"));
			}

	// Determine if two objects are alike.
	public static bool LikeObj(Object vLeft, Object vRight,
							   CompareMethod CompareOption)
			{
				return StringType.StrLike(StringType.FromObject(vLeft),
										  StringType.FromObject(vRight),
										  CompareOption);
			}

	// Modulus two objects.
	public static Object ModObj(Object o1, Object o2)
			{
				switch(CommonType(o1, o2, false))
				{
					case TypeCode.Boolean:
					{
						return (short)((BooleanType.FromObject(o1) ? -1 : 0) %
							           (BooleanType.FromObject(o2) ? -1 : 0));
					}
					// Not reached.

					case TypeCode.Byte:
					{
						return (byte)(ByteType.FromObject(o1) %
							   		  ByteType.FromObject(o2));
					}
					// Not reached.

					case TypeCode.Int16:
					{
						return (short)(ShortType.FromObject(o1) %
								 	   ShortType.FromObject(o2));
					}
					// Not reached.

					case TypeCode.Int32:
					{
						return IntegerType.FromObject(o1) %
							   IntegerType.FromObject(o2);
					}
					// Not reached.

					case TypeCode.Int64:
					{
						return LongType.FromObject(o1) %
							   LongType.FromObject(o2);
					}
					// Not reached.

					case TypeCode.Single:
					{
						return SingleType.FromObject(o1) %
							   SingleType.FromObject(o2);
					}
					// Not reached.

					case TypeCode.Double:
					case TypeCode.String:
					{
						return DoubleType.FromObject(o1) %
							   DoubleType.FromObject(o2);
					}
					// Not reached.

					case TypeCode.Decimal:
					{
						return DecimalType.FromObject(o1) %
							   DecimalType.FromObject(o2);
					}
					// Not reached.
				}
				throw new InvalidCastException(S._("VB_InvalidModArguments"));
			}

	// Multiply two objects.
	public static Object MulObj(Object o1, Object o2)
			{
				switch(CommonType(o1, o2, false))
				{
					case TypeCode.Boolean:
					{
						return (short)((BooleanType.FromObject(o1) ? -1 : 0) *
							           (BooleanType.FromObject(o2) ? -1 : 0));
					}
					// Not reached.

					case TypeCode.Byte:
					{
						int bp = ByteType.FromObject(o1) *
								 ByteType.FromObject(o2);
						if(bp <= 255)
						{
							return (byte)bp;
						}
						else if(bp <= 32767)
						{
							return (short)bp;
						}
						else
						{
							return bp;
						}
					}
					// Not reached.

					case TypeCode.Int16:
					{
						int sp = ShortType.FromObject(o1) *
								 ShortType.FromObject(o2);
						if(sp >= -32768 && sp <= 32767)
						{
							return (short)sp;
						}
						else
						{
							return sp;
						}
					}
					// Not reached.

					case TypeCode.Int32:
					{
						int i1 = IntegerType.FromObject(o1);
						int i2 = IntegerType.FromObject(o2);
						try
						{
							checked
							{
								return i1 * i2;
							}
						}
						catch(OverflowException)
						{
							return ((long)i1) * ((long)i2);
						}
					}
					// Not reached.

					case TypeCode.Int64:
					{
						long l1 = LongType.FromObject(o1);
						long l2 = LongType.FromObject(o2);
						try
						{
							checked
							{
								return l1 * l2;
							}
						}
						catch(OverflowException)
						{
							try
							{
								return ((decimal)l1) * ((decimal)l2);
							}
							catch(OverflowException)
							{
								return ((double)l1) * ((double)l2);
							}
						}
					}
					// Not reached.

					case TypeCode.Single:
					{
						return SingleType.FromObject(o1) *
							   SingleType.FromObject(o2);
					}
					// Not reached.

					case TypeCode.Double:
					case TypeCode.String:
					{
						return DoubleType.FromObject(o1) *
							   DoubleType.FromObject(o2);
					}
					// Not reached.

					case TypeCode.Decimal:
					{
						decimal d1 = DecimalType.FromObject(o1);
						decimal d2 = DecimalType.FromObject(o2);
						try
						{
							checked
							{
								return d1 * d2;
							}
						}
						catch(OverflowException)
						{
							return ((double)d1) * ((double)d2);
						}
					}
					// Not reached.
				}
				throw new InvalidCastException(S._("VB_InvalidMulArguments"));
			}

	// Negate an object.
	public static Object NegObj(Object o)
			{
				if(o == null)
				{
					return 0;
				}
				switch(GetTypeCode(o))
				{
					case TypeCode.Boolean:
						return (short)(BooleanType.FromObject(o) ? 1 : 0);

					case TypeCode.Byte:
						return (short)(-(ByteType.FromObject(o)));

					case TypeCode.Int16:
					{
						int svalue = -(ShortType.FromObject(o));
						if(svalue >= -32768 && svalue <= 32767)
						{
							return (short)svalue;
						}
						else
						{
							return svalue;
						}
					}
					// Not reached.

					case TypeCode.Int32:
					{
						int ivalue = IntegerType.FromObject(o);
						if(ivalue != Int32.MinValue)
						{
							return -ivalue;
						}
						else
						{
							return -((long)ivalue);
						}
					}
					// Not reached.

					case TypeCode.Int64:
					{
						long lvalue = LongType.FromObject(o);
						if(lvalue != Int64.MinValue)
						{
							return -lvalue;
						}
						else
						{
							return -((decimal)lvalue);
						}
					}
					// Not reached.

					case TypeCode.Single:
						return -(SingleType.FromObject(o));

					case TypeCode.Double:
					case TypeCode.String:
						return -(DoubleType.FromObject(o));

					case TypeCode.Decimal:
						return -(DecimalType.FromObject(o));
				}
				throw new InvalidCastException(S._("VB_InvalidNegArgument"));
			}

	// Bitwise NOT an object.
	public static Object NotObj(Object o)
			{
				Type enumType;
				Object value;
				switch(CommonType(o, o, out enumType))
				{
					case TypeCode.Boolean:
					{
						return !BooleanType.FromObject(o);
					}
					break;

					case TypeCode.Byte:
					{
						value = (byte)(~(ByteType.FromObject(o)));
					}
					break;

					case TypeCode.Int16:
					{
						value = (short)(~(ShortType.FromObject(o)));
					}
					break;

					case TypeCode.Int32:
					{
						value = ~(IntegerType.FromObject(o));
					}
					break;

					case TypeCode.Int64:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.String:
					case TypeCode.Decimal:
					{
						value = ~(LongType.FromObject(o));
					}
					break;

					default:
					{
						throw new InvalidCastException
							(S._("VB_InvalidNotArgument"));
					}
					// Not reached.
				}
				if(enumType == null)
				{
					return value;
				}
				else
				{
					return Enum.ToObject(enumType, value);
				}
			}

	// Test two objects and return -1, 0, or 1.
	public static int ObjTst(Object o1, Object o2, bool TextCompare)
			{
				switch(CommonType(o1, o2, true))
				{
					case TypeCode.Boolean:
					{
						int b1 = (BooleanType.FromObject(o1) ? -1 : 0);
						int b2 = (BooleanType.FromObject(o2) ? -1 : 0);
						if(b1 < b2)
						{
							return -1;
						}
						else if(b1 > b2)
						{
							return 1;
						}
						else
						{
							return 0;
						}
					}
					// Not reached.

					case TypeCode.Byte:
					{
						byte by1 = ByteType.FromObject(o1);
						byte by2 = ByteType.FromObject(o2);
						if(by1 < by2)
						{
							return -1;
						}
						else if(by1 > by2)
						{
							return 1;
						}
						else
						{
							return 0;
						}
					}
					// Not reached.

					case TypeCode.Int16:
					{
						short s1 = ShortType.FromObject(o1);
						short s2 = ShortType.FromObject(o2);
						if(s1 < s2)
						{
							return -1;
						}
						else if(s1 > s2)
						{
							return 1;
						}
						else
						{
							return 0;
						}
					}
					// Not reached.

					case TypeCode.Int32:
					{
						int i1 = IntegerType.FromObject(o1);
						int i2 = IntegerType.FromObject(o2);
						if(i1 < i2)
						{
							return -1;
						}
						else if(i1 > i2)
						{
							return 1;
						}
						else
						{
							return 0;
						}
					}
					// Not reached.

					case TypeCode.Int64:
					{
						long l1 = LongType.FromObject(o1);
						long l2 = LongType.FromObject(o2);
						if(l1 < l2)
						{
							return -1;
						}
						else if(l1 > l2)
						{
							return 1;
						}
						else
						{
							return 0;
						}
					}
					// Not reached.

					case TypeCode.Single:
					{
						float f1 = SingleType.FromObject(o1);
						float f2 = SingleType.FromObject(o2);
						if(f1 < f2)
						{
							return -1;
						}
						else if(f1 > f2)
						{
							return 1;
						}
						else
						{
							return 0;
						}
					}
					// Not reached.

					case TypeCode.Double:
					{
						double d1 = DoubleType.FromObject(o1);
						double d2 = DoubleType.FromObject(o2);
						if(d1 < d2)
						{
							return -1;
						}
						else if(d1 > d2)
						{
							return 1;
						}
						else
						{
							return 0;
						}
					}
					// Not reached.

					case TypeCode.String:
					{
						if(!TextCompare)
						{
							return String.CompareOrdinal
								(StringType.FromObject(o1),
								 StringType.FromObject(o2));
						}
						else
						{
							return CultureInfo.CurrentCulture.CompareInfo
								.Compare(StringType.FromObject(o1),
										 StringType.FromObject(o2),
										 CompareOptions.IgnoreCase |
										 CompareOptions.IgnoreKanaType |
										 CompareOptions.IgnoreWidth);
						}
					}
					// Not reached.

					case TypeCode.Decimal:
					{
						decimal D1 = DecimalType.FromObject(o1);
						decimal D2 = DecimalType.FromObject(o2);
						if(D1 < D2)
						{
							return -1;
						}
						else if(D1 > D2)
						{
							return 1;
						}
						else
						{
							return 0;
						}
					}
					// Not reached.
				}
				throw new InvalidCastException(S._("VB_InvalidTstArguments"));
			}

	// Unary plus an object.
	public static Object PlusObj(Object o)
			{
				if(o == null)
				{
					return 0;
				}
				switch(GetTypeCode(o))
				{
					case TypeCode.Boolean:
						return (short)(BooleanType.FromObject(o) ? -1 : 0);

					case TypeCode.Byte:
					case TypeCode.Int16:
					case TypeCode.Int32:
					case TypeCode.Int64:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.Decimal:
					case TypeCode.String:
						return DoubleType.FromObject(o);
				}
				throw new InvalidCastException(S._("VB_InvalidPlusArgument"));
			}

	// Power two objects.
	public static Object PowObj(Object o1, Object o2)
			{
				switch(CommonType(o1, o2, false))
				{
					case TypeCode.Boolean:
					case TypeCode.Byte:
					case TypeCode.Int16:
					case TypeCode.Int32:
					case TypeCode.Int64:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.String:
					case TypeCode.Decimal:
					{
						return Math.Pow(DoubleType.FromObject(o1),
							   			DoubleType.FromObject(o2));
					}
					// Not reached.
				}
				throw new InvalidCastException(S._("VB_InvalidPowArguments"));
			}

	// Shift an object left by an amount.
	public static Object ShiftLeftObj(Object o1, int amount)
			{
				switch(GetTypeCode(o1))
				{
					case TypeCode.Boolean:
					{
						return (short)((BooleanType.FromObject(o1) ? -1 : 0) <<
							           (amount & 0x0F));
					}
					// Not reached.

					case TypeCode.Byte:
					{
						return (byte)(ByteType.FromObject(o1) <<
									  (amount & 0x07));
					}
					// Not reached.

					case TypeCode.Int16:
					{
						return (short)(ShortType.FromObject(o1) <<
								 	   (amount & 0x0F));
					}
					// Not reached.

					case TypeCode.Int32:
					{
						return (int)(IntegerType.FromObject(o1) <<
									 (amount & 0x1F));
					}
					// Not reached.

					case TypeCode.Int64:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.String:
					case TypeCode.Decimal:
					{
						return (long)(LongType.FromObject(o1) <<
									  (amount & 0x3F));
					}
					// Not reached.
				}
				throw new InvalidCastException
					(S._("VB_InvalidShiftLeftArguments"));
			}

	// Shift an object right by an amount.
	public static Object ShiftRightObj(Object o1, int amount)
			{
				switch(GetTypeCode(o1))
				{
					case TypeCode.Boolean:
					{
						return (short)((BooleanType.FromObject(o1) ? -1 : 0) >>
							           (amount & 0x0F));
					}
					// Not reached.

					case TypeCode.Byte:
					{
						return (byte)(ByteType.FromObject(o1) >>
									  (amount & 0x07));
					}
					// Not reached.

					case TypeCode.Int16:
					{
						return (short)(ShortType.FromObject(o1) >>
								 	   (amount & 0x0F));
					}
					// Not reached.

					case TypeCode.Int32:
					{
						return (int)(IntegerType.FromObject(o1) >>
									 (amount & 0x1F));
					}
					// Not reached.

					case TypeCode.Int64:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.String:
					case TypeCode.Decimal:
					{
						return (long)(LongType.FromObject(o1) >>
									  (amount & 0x3F));
					}
					// Not reached.
				}
				throw new InvalidCastException
					(S._("VB_InvalidShiftRightArguments"));
			}

	// Concatenate two objects.
	public static Object StrCatObj(Object o1, Object o2)
			{
			#if !ECMA_COMPAT
				if(o1 is DBNull)
				{
					o1 = String.Empty;
				}
				if(o2 is DBNull)
				{
					o2 = String.Empty;
				}
			#endif
				return StringType.FromObject(o1) + StringType.FromObject(o2);
			}

	// Subtract two objects.
	public static Object SubObj(Object o1, Object o2)
			{
				switch(CommonType(o1, o2, false))
				{
					case TypeCode.Boolean:
					{
						return (short)((BooleanType.FromObject(o1) ? -1 : 0) -
							           (BooleanType.FromObject(o2) ? -1 : 0));
					}
					// Not reached.

					case TypeCode.Byte:
					{
						int bp = ByteType.FromObject(o1) -
								 ByteType.FromObject(o2);
						if(bp <= 255)
						{
							return (byte)bp;
						}
						else if(bp <= 32767)
						{
							return (short)bp;
						}
						else
						{
							return bp;
						}
					}
					// Not reached.

					case TypeCode.Int16:
					{
						int sp = ShortType.FromObject(o1) -
								 ShortType.FromObject(o2);
						if(sp >= -32768 && sp <= 32767)
						{
							return (short)sp;
						}
						else
						{
							return sp;
						}
					}
					// Not reached.

					case TypeCode.Int32:
					{
						int i1 = IntegerType.FromObject(o1);
						int i2 = IntegerType.FromObject(o2);
						try
						{
							checked
							{
								return i1 - i2;
							}
						}
						catch(OverflowException)
						{
							return ((long)i1) - ((long)i2);
						}
					}
					// Not reached.

					case TypeCode.Int64:
					{
						long l1 = LongType.FromObject(o1);
						long l2 = LongType.FromObject(o2);
						try
						{
							checked
							{
								return l1 - l2;
							}
						}
						catch(OverflowException)
						{
							try
							{
								return ((decimal)l1) - ((decimal)l2);
							}
							catch(OverflowException)
							{
								return ((double)l1) - ((double)l2);
							}
						}
					}
					// Not reached.

					case TypeCode.Single:
					{
						return SingleType.FromObject(o1) -
							   SingleType.FromObject(o2);
					}
					// Not reached.

					case TypeCode.Double:
					case TypeCode.String:
					{
						return DoubleType.FromObject(o1) -
							   DoubleType.FromObject(o2);
					}
					// Not reached.

					case TypeCode.Decimal:
					{
						decimal d1 = DecimalType.FromObject(o1);
						decimal d2 = DecimalType.FromObject(o2);
						try
						{
							checked
							{
								return d1 - d2;
							}
						}
						catch(OverflowException)
						{
							return ((double)d1) - ((double)d2);
						}
					}
					// Not reached.
				}
				throw new InvalidCastException(S._("VB_InvalidSubArguments"));
			}

	// XOR two objects as boolean values.
	public static Object XorObj(Object o1, Object o2)
			{
				if(o1 == null && o2 == null)
				{
					return false;
				}
				switch(CommonType(o1, o2, false))
				{
					case TypeCode.Boolean:
					case TypeCode.Byte:
					case TypeCode.Int16:
					case TypeCode.Int32:
					case TypeCode.Int64:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.String:
					case TypeCode.Decimal:
					{
						bool b1 = BooleanType.FromObject(o1);
						bool b2 = BooleanType.FromObject(o2);
						return ((b1 && !b2) || (!b1 && b2));
					}
					// Not reached.
				}
				throw new InvalidCastException(S._("VB_InvalidXorArguments"));
			}

}; // class ObjectType

}; // namespace Microsoft.VisualBasic.CompilerServices
