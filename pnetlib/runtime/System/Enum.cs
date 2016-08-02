/*
 * Enum.cs - Implementation of the "System.Enum" class.
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

namespace System
{

using System.Reflection;
using System.Private;
using System.Runtime.CompilerServices;

public abstract class Enum : ValueType, IComparable, IFormattable
#if !ECMA_COMPAT
	, IConvertible
#endif
{

	// Constructor.
	protected Enum() : base() {}

	// Implement the IComparable interface.
	public int CompareTo(Object target)
			{
				Type type;
				/* Note: this is what the ECMA spec says about CompareTo
				 * Any positive integer: The value of the current 
				 * instance is greater than the value of target, 
				 * or target is null */
				if(target == null)
				{
					return 1;
				}
				type = GetType();
				if(type != target.GetType())
				{
					throw new ArgumentException(_("Arg_MustBeSameEnum"));
				}
				return ((IComparable)(GetEnumValue())).CompareTo
							(((Enum)target).GetEnumValue());
			}

	// Test for equality.
	public override bool Equals(Object obj)
			{
				Enum oenum = (obj as Enum);
				if(oenum != null)
				{
					Type type = GetType();
					if(type != oenum.GetType())
					{
						return false;
					}
					return (GetEnumValue()).Equals
								(((Enum)obj).GetEnumValue());
				}
				else
				{
					return false;
				}
			}

	// Format an enumerated value.
	public static String Format(Type enumType, Object value, String format)
			{
				Type type;
				String result;

				// Validate the parameters.
				if(enumType == null)
				{
					throw new ArgumentNullException("enumType");
				}
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				if(format == null)
				{
					throw new ArgumentNullException("format");
				}
				if(!enumType.IsEnum)
				{
					throw new ArgumentException(_("Arg_MustBeEnum"));
				}
				type = value.GetType();
				if(type == enumType)
				{
					value = ((Enum)value).GetEnumValue();
				}
				else if(type != GetUnderlyingType(enumType))
				{
					throw new ArgumentException(_("Arg_InvalidEnumValue"));
				}

				// Determine what to do based on the format.
				if(format == "G" || format == "g" || format=="")
				{
				#if CONFIG_REFLECTION
					if(Attribute.IsDefined(enumType, typeof(FlagsAttribute)))
					{
						return FormatEnumWithFlags(enumType, value);
					}
					else
				#endif
					{
						result = GetEnumName(enumType, value);
						if(result != null)
						{
							return result;
						}
						return value.ToString();
					}
				}
				else if(format == "F" || format == "f")
				{
					return FormatEnumWithFlags(enumType, value);
				}
				else if(format == "X" || format == "x")
				{
					/* Note: ECMA says atleast '8' and MS.NET does 
					 * the same as the underlying size */ 
				#if !ECMA_COMPAT
					Type type1=Enum.GetUnderlyingType(enumType);
					if(type1==typeof(Byte) || type1==typeof(SByte)) format="X2";
					if(type1==typeof(Int16) || type1==typeof(UInt16))format="X4";
					if(type1==typeof(Int32) || type1==typeof(UInt32))format="X8";
					if(type1==typeof(Int64) || type1==typeof(UInt64))format="X16";
				#else
					format="X8";
				#endif
					return ((IFormattable)value).ToString(format, null);
				}
				else if(format == "D" || format == "d")
				{
					return value.ToString();
				}
				else
				{
					throw new FormatException(_("Format_Enum"));
				}
			}

	// Get the hash code for this enumerated value.
	public override int GetHashCode()
			{
				return (GetEnumValue()).GetHashCode();
			}

	// Get the name of an enumerated constant value.
	public static String GetName(Type enumType, Object value)
			{
				if(enumType == null)
				{
					throw new ArgumentNullException("enumType");
				}
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				if(!enumType.IsEnum)
				{
					throw new ArgumentException(_("Arg_MustBeEnum"));
				}
				Type type = value.GetType();
				if(type == enumType)
				{
					value = ((Enum)value).GetEnumValue();
				}
			#if ECMA_COMPAT
				else if(type != GetUnderlyingType(enumType))
				{
					throw new ArgumentException(_("Arg_InvalidEnumValue"));
				}
			#else
				/* Note: The spec does mention that "underlying type"
				 * but this seems to be the check for underlying type
				 * they suggest */
				value=ToObject(enumType, value);
			#endif
				return GetEnumName(enumType, value);
			}

#if CONFIG_REFLECTION

	// Get the names of all enumerated constants in a type.
	public static String[] GetNames(Type enumType)
			{
				// Validate the parameter.
				if(enumType == null)
				{
					throw new ArgumentNullException("enumType");
				}
				else if(!enumType.IsEnum)
				{
					throw new ArgumentException(_("Arg_MustBeEnum"));
				}

				// Find all public static fields within the type.
				FieldInfo[] fields = enumType.GetFields(BindingFlags.Public |
														BindingFlags.Static);

				// Count the number of fields that are "literal" and
				// have the same type as the enumeration.
				int numLiterals = 0;
				int posn;
				for(posn = 0; posn < fields.Length; ++posn)
				{
					if((fields[posn].Attributes &
							FieldAttributes.Literal) != 0)
					{
						++numLiterals;
					}
					else
					{
						fields[posn] = null;
					}
				}

				// Create and fill the name and value arrays.
				String[] names = new String [numLiterals];
				Array values = Array.CreateInstance(enumType, numLiterals);
				numLiterals = 0;
				for(posn = 0; posn < fields.Length; ++posn)
				{
					if(fields[posn] != null)
					{
						names[numLiterals] = fields[posn].Name;
						values.SetValue(fields[posn].GetValue(null),
										numLiterals);
						++numLiterals;
					}
				}

				// Sort the arrays on value, and then return the names only.
				Array.Sort(values, names);
				return names;
			}

#endif // CONFIG_REFLECTION

	// Get the underlying type for an enumerated type.
	public static Type GetUnderlyingType(Type enumType)
			{
				// Validate the argument.
				if(enumType == null)
				{
					throw new ArgumentNullException("enumType");
				}
				if(!enumType.IsEnum)
				{
					throw new ArgumentException(_("Arg_MustBeEnum"));
				}

#if CONFIG_REFLECTION
				// Search for a public instance field that has the
				// "RTSpecialName" attribute associated with it.
				FieldInfo[] fields = enumType.GetFields(BindingFlags.Public |
													    BindingFlags.Instance);
				int posn;
				for(posn = 0; posn < fields.Length; ++posn)
				{
					if((fields[posn].Attributes &
							FieldAttributes.RTSpecialName) != 0)
					{
						return fields[posn].FieldType;
					}
				}
#endif

				// Use a non-reflection trick to get the type.
				return (ToObject(enumType, 0)).GetType();
			}

#if CONFIG_REFLECTION

	// Get the values of all enumerated constants for an enumerated type.
	public static Array GetValues(Type enumType)
			{
				// Validate the parameter.
				if(enumType == null)
				{
					throw new ArgumentNullException("enumType");
				}
				else if(!enumType.IsEnum)
				{
					throw new ArgumentException(_("Arg_MustBeEnum"));
				}

				// Find all public static fields within the type.
				FieldInfo[] fields = enumType.GetFields(BindingFlags.Public |
														BindingFlags.Static);

				// Count the number of fields that are "literal" and
				// have the same type as the enumeration.
				int numLiterals = 0;
				int posn;
				for(posn = 0; posn < fields.Length; ++posn)
				{
					if((fields[posn].Attributes &
							FieldAttributes.Literal) != 0)
					{
						++numLiterals;
					}
					else
					{
						fields[posn] = null;
					}
				}

				// Create and fill the value array.
				Array values = Array.CreateInstance(enumType, numLiterals);
				numLiterals = 0;
				for(posn = 0; posn < fields.Length; ++posn)
				{
					if(fields[posn] != null)
					{
						values.SetValue(fields[posn].GetValue(null),
										numLiterals);
						++numLiterals;
					}
				}

				// Sort the array on value and return it.
				Array.Sort(values);
				return values;
			}

#endif // CONFIG_REFLECTION

	// Determine if a specified constant is defined in an enumerated type.
	public static bool IsDefined(Type enumType, Object value)
			{
				if(enumType == null)
				{
					throw new ArgumentNullException("enumType");
				}
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				if(!enumType.IsEnum)
				{
					throw new ArgumentException(_("Arg_MustBeEnum"));
				}
				/* Note: Ecma doesn't specify this , but MS seems to have
				 *       it */
				#if !ECMA_COMPAT
				if(value is String)
				{
					return (GetEnumValueFromName(enumType, 
												(value as String),
												false) != null);
				}
				#endif
				Type type = value.GetType();
				if(type == enumType)
				{
					value = ((Enum)value).GetEnumValue();
				}
				else if(type != GetUnderlyingType(enumType))
				{
					throw new ArgumentException(_("Arg_InvalidEnumValue"));
				}
				return IsEnumValue(enumType, value);
			}

	// Parse an enumerated constant specification.
	public static Object Parse(Type enumType, String value)
			{
				return Parse(enumType, value, false);
			}
	public static Object Parse(Type enumType, String value, bool ignoreCase)
			{
				// Validate the parameters.
				if(enumType == null)
				{
					throw new ArgumentNullException("enumType");
				}
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				if(!enumType.IsEnum)
				{
					throw new ArgumentException(_("Arg_MustBeEnum"));
				}

				value = value.Trim();
				
				Object finalValue = null;
				Object newValue;
				string name;
				
				string [] split = value.Split( ',' );
				
				foreach( string s in split ) 
				{
					name = s.Trim();
					// Convert the name into a value.
					newValue = GetEnumValueFromName(enumType, name, ignoreCase);
					if(newValue == null)
					{
						throw new ArgumentException
							(_("Arg_InvalidEnumName"));
					}
				
					// Combine the current value with the new one.
					if(finalValue == null)
					{
						finalValue = newValue;
					}
					else
					{
						finalValue = EnumValueOr(finalValue, newValue);
					}
				}

				// If we didn't see any names, then the string is invalid.
				if(finalValue == null)
				{
					throw new ArgumentException
						(_("Arg_InvalidEnumName"));
				}
				return finalValue;
			}

	// Convert a constant into an enumerated value object.
	public static Object ToObject(Type enumType, Object value)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				Type type = value.GetType();
				if(type == typeof(System.Int32))
				{
					// This is the most common case, so we get it first.
					return ToObject(enumType, (int)value);
				}
				else if(type == typeof(System.SByte))
				{
					return ToObject(enumType, (sbyte)value);
				}
				else if(type == typeof(System.Byte))
				{
					return ToObject(enumType, (byte)value);
				}
				else if(type == typeof(System.Int16))
				{
					return ToObject(enumType, (short)value);
				}
				else if(type == typeof(System.UInt16))
				{
					return ToObject(enumType, (ushort)value);
				}
				else if(type == typeof(System.UInt32))
				{
					return ToObject(enumType, (uint)value);
				}
				else if(type == typeof(System.Int64))
				{
					return ToObject(enumType, (long)value);
				}
				else if(type == typeof(System.UInt64))
				{
					return ToObject(enumType, (ulong)value);
				}
				else
				{
					throw new ArgumentException(_("Arg_InvalidEnumValue"));
				}
			}
	[CLSCompliant(false)]
	public static Object ToObject(Type enumType, sbyte value)
			{
				if(enumType == null)
				{
					throw new ArgumentNullException("enumType");
				}
				if(!enumType.IsEnum)
				{
					throw new ArgumentException(_("Arg_MustBeEnum"));
				}
				return EnumIntToObject(enumType, value);
			}
	public static Object ToObject(Type enumType, byte value)
			{
				if(enumType == null)
				{
					throw new ArgumentNullException("enumType");
				}
				if(!enumType.IsEnum)
				{
					throw new ArgumentException(_("Arg_MustBeEnum"));
				}
				return EnumIntToObject(enumType, value);
			}
	public static Object ToObject(Type enumType, short value)
			{
				if(enumType == null)
				{
					throw new ArgumentNullException("enumType");
				}
				if(!enumType.IsEnum)
				{
					throw new ArgumentException(_("Arg_MustBeEnum"));
				}
				return EnumIntToObject(enumType, value);
			}
	[CLSCompliant(false)]
	public static Object ToObject(Type enumType, ushort value)
			{
				if(enumType == null)
				{
					throw new ArgumentNullException("enumType");
				}
				if(!enumType.IsEnum)
				{
					throw new ArgumentException(_("Arg_MustBeEnum"));
				}
				return EnumIntToObject(enumType, value);
			}
	public static Object ToObject(Type enumType, int value)
			{
				if(enumType == null)
				{
					throw new ArgumentNullException("enumType");
				}
				if(!enumType.IsEnum)
				{
					throw new ArgumentException(_("Arg_MustBeEnum"));
				}
				return EnumIntToObject(enumType, value);
			}
	[CLSCompliant(false)]
	public static Object ToObject(Type enumType, uint value)
			{
				if(enumType == null)
				{
					throw new ArgumentNullException("enumType");
				}
				if(!enumType.IsEnum)
				{
					throw new ArgumentException(_("Arg_MustBeEnum"));
				}
				return EnumIntToObject(enumType, unchecked((int)value));
			}
	public static Object ToObject(Type enumType, long value)
			{
				if(enumType == null)
				{
					throw new ArgumentNullException("enumType");
				}
				if(!enumType.IsEnum)
				{
					throw new ArgumentException(_("Arg_MustBeEnum"));
				}
				return EnumLongToObject(enumType, value);
			}
	[CLSCompliant(false)]
	public static Object ToObject(Type enumType, ulong value)
			{
				if(enumType == null)
				{
					throw new ArgumentNullException("enumType");
				}
				if(!enumType.IsEnum)
				{
					throw new ArgumentException(_("Arg_MustBeEnum"));
				}
				return EnumLongToObject(enumType, unchecked((long)value));
			}

	// String conversion.
	public override String ToString()
			{
				return ToString(null, null);
			}
	public String ToString(String format)
			{
				return ToString(format, null);
			}
	public String ToString(IFormatProvider provider)
			{
				return ToString(null, provider);
			}
	public String ToString(String format, IFormatProvider provider)
			{
				if(format == null)
				{
					return Format(GetType(), this, "G");
				}
				else
				{
					return Format(GetType(), this, format);
				}
			}

#if !ECMA_COMPAT

	// Implementation of the IConvertible interface.
	public TypeCode GetTypeCode()
			{
				Type type = GetUnderlyingType(GetType());
				return Type.GetTypeCode(type);
			}
	bool IConvertible.ToBoolean(IFormatProvider provider)
			{
				/* Note: apparently the use of IConvertible is 
				 * to convert what can't be casted . */
				return Convert.ToBoolean(GetEnumValue());
			}
	byte IConvertible.ToByte(IFormatProvider provider)
			{
				return Convert.ToByte(GetEnumValue());
			}
	sbyte IConvertible.ToSByte(IFormatProvider provider)
			{
				return Convert.ToSByte(GetEnumValue());
			}
	short IConvertible.ToInt16(IFormatProvider provider)
			{
				return Convert.ToInt16(GetEnumValue());
			}
	ushort IConvertible.ToUInt16(IFormatProvider provider)
			{
				return Convert.ToUInt16(GetEnumValue());
			}
	char IConvertible.ToChar(IFormatProvider provider)
			{
				return Convert.ToChar(GetEnumValue());
			}
	int IConvertible.ToInt32(IFormatProvider provider)
			{
				return Convert.ToInt32(GetEnumValue());
			}
	uint IConvertible.ToUInt32(IFormatProvider provider)
			{
				return Convert.ToUInt32(GetEnumValue());
			}
	long IConvertible.ToInt64(IFormatProvider provider)
			{
				return Convert.ToInt64(GetEnumValue());
			}
	ulong IConvertible.ToUInt64(IFormatProvider provider)
			{
				return Convert.ToUInt64(GetEnumValue());
			}
	float IConvertible.ToSingle(IFormatProvider provider)
			{
				return  Convert.ToSingle(GetEnumValue());
			}
	double IConvertible.ToDouble(IFormatProvider provider)
			{
				return  Convert.ToDouble(GetEnumValue());
			}
	Decimal IConvertible.ToDecimal(IFormatProvider provider)
			{
				return  Convert.ToDecimal(GetEnumValue());
			}
	DateTime IConvertible.ToDateTime(IFormatProvider provider)
			{
				throw new InvalidCastException
					(String.Format
						(_("InvalidCast_FromTo"), "Byte", "DateTime"));
			}
	Object IConvertible.ToType(Type conversionType, IFormatProvider provider)
			{
				return Convert.DefaultToType(this, conversionType,
											 provider, true);
			}

#endif // !ECMA_COMPAT

	// Get the boxed version of an enumerated value in the underlying type.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private Object GetEnumValue();

	// Get the name of an underlying enumerated value for a type.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static String GetEnumName(Type enumType, Object value);

	// Determine if a underlying value is a member of an enumerated type.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static bool IsEnumValue(Type enumType, Object value);

	// Get an enumerated value by name.  The name may either be
	// a constant name, or an integer value.  The return type is
	// either an enumerated value in the specified type, or null
	// if the name is invalid.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static Object GetEnumValueFromName
				(Type enumType, String name, bool ignoreCase);

	// Or two enumerated values together to form a new value.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static Object EnumValueOr(Object value1, Object value2);

	// Convert an integer value into an enumerated value in a specific type.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static Object EnumIntToObject(Type enumType, int value);

	// Convert a long value into an enumerated value in a specific type.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static Object EnumLongToObject(Type enumType, long value);

	// Format an enumerated value when flags are involved.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static String FormatEnumWithFlags
				(Type enumType, Object value);

}; // class Enum

}; // namespace System
