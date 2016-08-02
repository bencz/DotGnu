/*
 * Convert.cs - Convert between various script types.
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
using System.Globalization;
using Microsoft.JScript.Vsa;

public sealed class Convert
{

	// Check to see if a floating-point value is actually an integer.
	public static double CheckIfDoubleIsInteger(double d)
			{
				if(d == Math.Round(d))
				{
					return d;
				}
				else
				{
					throw new JScriptException(JSError.TypeMismatch);
				}
			}
	public static float CheckIfSingleIsInteger(float s)
			{
				if(s == Math.Round(s))
				{
					return s;
				}
				else
				{
					throw new JScriptException(JSError.TypeMismatch);
				}
			}

	// Coerce a value to a new type, throwing an exception if not possible.
	public static Object Coerce(Object value, Object type)
			{
				// TODO
				return value;
			}

	// Coerce to an explicit type.
	public static Object CoerceT(Object value, Type t, bool explicitOK)
			{
				// TODO
				return value;
			}

	// Coerce to a type that is specified using a type code.
	public static Object Coerce2(Object value, TypeCode target,
								 bool truncationPermitted)
			{
				// TODO
				return value;
			}

	// Determine if an abstract syntax tree is a bad index.
	// Not used - for backwards-compatibility only.
	public static bool IsBadIndex(AST ast)
			{
				throw new NotImplementedException();
			}

	// Throw a type mismatch exception.
	public static void ThrowTypeMismatch(Object val)
			{
				throw new JScriptException(JSError.TypeMismatch,
										   new Context(val.ToString()));
			}

	// Extract values of various types from an object that is known
	// to have a specific type code.
	internal static bool ExtractBoolean(Object value)
			{
			#if ECMA_COMPAT
				return (bool)value;
			#else
				return ((IConvertible)value).ToBoolean(null);
			#endif
			}
	internal static char ExtractChar(Object value)
			{
			#if ECMA_COMPAT
				return (char)value;
			#else
				return ((IConvertible)value).ToChar(null);
			#endif
			}
	internal static int ExtractInt32Smaller(Object value)
			{
			#if ECMA_COMPAT
				if(value is SByte)
				{
					return (int)(sbyte)value;
				}
				else if(value is Byte)
				{
					return (int)(byte)value;
				}
				else if(value is Int16)
				{
					return (int)(short)value;
				}
				else if(value is UInt16)
				{
					return (int)(ushort)value;
				}
				else
				{
					return (int)(char)value;
				}
			#else
				return ((IConvertible)value).ToInt32(null);
			#endif
			}
	internal static int ExtractInt32(Object value)
			{
			#if ECMA_COMPAT
				return (int)value;
			#else
				return ((IConvertible)value).ToInt32(null);
			#endif
			}
	internal static long ExtractInt64(Object value)
			{
			#if ECMA_COMPAT
				if(value is long)
				{
					return (long)value;
				}
				else
				{
					return (long)(uint)value;
				}
			#else
				return ((IConvertible)value).ToInt64(null);
			#endif
			}
	internal static ulong ExtractUInt64(Object value)
			{
			#if ECMA_COMPAT
				return (ulong)value;
			#else
				return ((IConvertible)value).ToUInt64(null);
			#endif
			}
	internal static double ExtractDouble(Object value)
			{
			#if ECMA_COMPAT
				if(value is Double)
				{
					return (double)value;
				}
				else
				{
					return (double)(float)value;
				}
			#else
				return ((IConvertible)value).ToDouble(null);
			#endif
			}
	internal static Decimal ExtractDecimal(Object value)
			{
			#if ECMA_COMPAT
				return (Decimal)value;
			#else
				return ((IConvertible)value).ToDecimal(null);
			#endif
			}
	internal static String ExtractString(Object value)
			{
			#if ECMA_COMPAT
				return (String)value;
			#else
				return ((IConvertible)value).ToString(null);
			#endif
			}
	internal static DateTime ExtractDateTime(Object value)
			{
			#if ECMA_COMPAT
				return (DateTime)value;
			#else
				return ((IConvertible)value).ToDateTime(null);
			#endif
			}

	// Convert a value to boolean.
	public static bool ToBoolean(Object value)
			{
				if(value is Boolean)
				{
					return (bool)value;
				}
				switch(Support.TypeCodeForObject(value))
				{
					case TypeCode.Empty:
					case TypeCode.DBNull:		return false;

					case TypeCode.Object:
					{
						if(value is Missing
					#if ECMA_COMPAT
						  )
					#else
						   || value is System.Reflection.Missing)
					#endif
						{
							return false;
						}
						// TODO: look for "op_True" and use that if present
						return true;
					}
					// Not reached.

					case TypeCode.Boolean:
						return ExtractBoolean(value);
					case TypeCode.Char:
						return (ExtractChar(value) != '\0');

					case TypeCode.SByte:
					case TypeCode.Byte:
					case TypeCode.Int16:
					case TypeCode.UInt16:
						return (ExtractInt32Smaller(value) != 0);

					case TypeCode.Int32:
						return (ExtractInt32(value) != 0);

					case TypeCode.UInt32:
					case TypeCode.Int64:
						return (ExtractInt64(value) != (long)0);

					case TypeCode.UInt64:
						return (ExtractUInt64(value) != (ulong)0);

					case TypeCode.Single:
					case TypeCode.Double:
					{
						double dvalue = ExtractDouble(value);
						if(Double.IsNaN(dvalue) || dvalue == 0.0)
						{
							return false;
						}
						else
						{
							return true;
						}
					}
					// Not reached.

					case TypeCode.Decimal:
						return (ExtractDecimal(value) != 0.0m);

					case TypeCode.DateTime:	return true;

					case TypeCode.String:
						return (ExtractString(value).Length != 0);

					default: return true;
				}
			}
	public static bool ToBoolean(Object value, bool explicitConversion)
			{
			#if false
				if(!explicitConversion && value is BooleanObject)
				{
					return ((BooleanObject)value).value;
				}
			#endif
				return ToBoolean(value);
			}

	// Convert a value into an object suitable for "for ... in".
	public static Object ToForInObject(Object value, VsaEngine engine)
			{
				// TODO
				return value;
			}

	// Normalize a value, to remove JScript object wrappers.
	private static Object Normalize(Object value)
			{
				if(value is ScriptObject)
				{
					return ((ScriptObject)value).DefaultValue
								(DefaultValueHint.None);
				}
				else if(value is DateTime)
				{
					return ((DateTime)value).ToString();
				}
				else
				{
					return value;
				}
			}

	// Convert a value into an int32 value.
	public static int ToInt32(Object value)
			{
				Object nvalue;
				if(value is Int32)
				{
					return (int)value;
				}
				else if(value is Double)
				{
					return (int)(double)value;
				}
				switch(Support.TypeCodeForObject(value))
				{
					case TypeCode.Empty:
					case TypeCode.DBNull:		return 0;

					case TypeCode.Object:
					{
						nvalue = Normalize(value);
						if(nvalue != value)
						{
							return ToInt32(nvalue);
						}
						else
						{
							return 0;
						}
					}
					// Not reached.

					case TypeCode.Boolean:
						return (ExtractBoolean(value) ? 1 : 0);

					case TypeCode.SByte:
					case TypeCode.Byte:
					case TypeCode.Int16:
					case TypeCode.UInt16:
					case TypeCode.Char:
						return ExtractInt32Smaller(value);

					case TypeCode.Int32:
						return ExtractInt32(value);

					case TypeCode.UInt32:
					case TypeCode.Int64:
						return (int)(ExtractInt64(value));

					case TypeCode.UInt64:
						return (int)(ExtractUInt64(value));

					case TypeCode.Single:
					case TypeCode.Double:
						return (int)(ExtractDouble(value));

					case TypeCode.Decimal:
						return (int)(double)(ExtractDecimal(value));

					case TypeCode.DateTime:
					{
						value = ExtractDateTime(value);
						nvalue = Normalize(value);
						if(nvalue != value)
						{
							return ToInt32(nvalue);
						}
						else
						{
							return 0;
						}
					}
					// Not reached

					case TypeCode.String:
						return (int)(ToNumber(ExtractString(value)));

					default: return 0;
				}
			}

	// Convert a value into a UInt32 value.
	internal static uint ToUInt32(Object value)
			{
				Object nvalue;
				if(value is UInt32)
				{
					return (uint)value;
				}
				else if(value is Int32)
				{
					return (uint)(int)value;
				}
				else if(value is Double)
				{
					return (uint)(double)value;
				}
				switch(Support.TypeCodeForObject(value))
				{
					case TypeCode.Empty:
					case TypeCode.DBNull:		return 0;

					case TypeCode.Object:
					{
						nvalue = Normalize(value);
						if(nvalue != value)
						{
							return ToUInt32(nvalue);
						}
						else
						{
							return 0;
						}
					}
					// Not reached.

					case TypeCode.Boolean:
						return (ExtractBoolean(value) ? (uint)1 : (uint)0);

					case TypeCode.SByte:
					case TypeCode.Byte:
					case TypeCode.Int16:
					case TypeCode.UInt16:
					case TypeCode.Char:
						return (uint)(ExtractInt32Smaller(value));

					case TypeCode.Int32:
						return (uint)(ExtractInt32(value));

					case TypeCode.UInt32:
					case TypeCode.Int64:
						return (uint)(ExtractInt64(value));

					case TypeCode.UInt64:
						return (uint)(ExtractUInt64(value));

					case TypeCode.Single:
					case TypeCode.Double:
						return (uint)(ExtractDouble(value));

					case TypeCode.Decimal:
						return (uint)(double)(ExtractDecimal(value));

					case TypeCode.DateTime:
					{
						value = ExtractDateTime(value);
						nvalue = Normalize(value);
						if(nvalue != value)
						{
							return ToUInt32(nvalue);
						}
						else
						{
							return 0;
						}
					}
					// Not reached.

					case TypeCode.String:
						return (uint)(ToNumber(ExtractString(value)));

					default: return 0;
				}
			}

	// Convert a JScript array object into a native array.
	public static Object ToNativeArray(Object value, RuntimeTypeHandle handle)
			{
				// TODO
				return value;
			}

	// Convert a value into a number.
	public static double ToNumber(Object value)
			{
				Object nvalue;
				if(value is Double)
				{
					return (double)value;
				}
				else if(value is Int32)
				{
					return (double)(int)value;
				}
				switch(Support.TypeCodeForObject(value))
				{
					case TypeCode.Empty:		return Double.NaN;
					case TypeCode.DBNull:		return 0.0;

					case TypeCode.Object:
					{
						nvalue = Normalize(value);
						if(nvalue != value)
						{
							return ToNumber(nvalue);
						}
						else
						{
							return Double.NaN;
						}
					}
					// Not reached.

					case TypeCode.Boolean:
						return (ExtractBoolean(value) ? 1.0 : 0.0);

					case TypeCode.SByte:
					case TypeCode.Byte:
					case TypeCode.Int16:
					case TypeCode.UInt16:
					case TypeCode.Char:
						return (double)(ExtractInt32Smaller(value));

					case TypeCode.Int32:
						return (double)(ExtractInt32(value));

					case TypeCode.UInt32:
					case TypeCode.Int64:
						return (double)(ExtractInt64(value));

					case TypeCode.UInt64:
						return (double)(ExtractUInt64(value));

					case TypeCode.Single:
					case TypeCode.Double:
						return ExtractDouble(value);

					case TypeCode.Decimal:
						return (double)(ExtractDecimal(value));

					case TypeCode.DateTime:
					{
						value = ExtractDateTime(value);
						nvalue = Normalize(value);
						if(nvalue != value)
						{
							return ToNumber(nvalue);
						}
						else
						{
							return Double.NaN;
						}
					}
					// Not reached

					case TypeCode.String:
						return ToNumber(ExtractString(value));

					default: return 0.0;
				}
			}
	public static double ToNumber(String value)
			{
				// TODO
				return 0;
			}

	// Convert a value into a JScript object, throwing an exception
	// if the conversion is not possible.
	public static Object ToObject(Object value, VsaEngine engine)
			{
				EngineInstance instance = EngineInstance.GetEngineInstance(engine);
				if(value is String)
				{
					return instance.GetStringConstructor().Construct(
														engine,
														new Object[] { value });
				}
				else if(value is Array)
				{
					return instance.GetArrayConstructor().Construct(
														engine,
														new Object[] { value });
				}
				// TODO
				return value;
			}

	// Convert a value into a JScript object, returning "null"
	// if the conversion is not possible.
	public static Object ToObject2(Object value, VsaEngine engine)
			{
				// TODO
				return value;
			}

	// Convert an object into a string.
	public static String ToString(Object value, bool explicitOK)
			{
				// Bail out immediately if it is already a string.
				String s = (value as String);
				if(s != null)
				{
					return s;
				}

				// Handle conversions of null values.
				if(value == null)
				{
					return (explicitOK ? "undefined" : null);
				}
				else if(DBNull.IsDBNull(value))
				{
					return (explicitOK ? "null" : null);
				}

				// Use the standard "Object.ToString()" method.
				s = value.ToString();
				if(s != null)
				{
					return s;
				}
				else
				{
					return (explicitOK ? "undefined" : null);
				}
			}
	internal static String ToString(Object value)
			{
				return ToString(value, true);
			}

	// Convert primitive values into strings.
	public static String ToString(bool b)
			{
				return (b ? "true" : "false");
			}
	public static String ToString(double d)
			{
				long longValue = (long)d;
				if(d == (double)longValue)
				{
					return longValue.ToString();
				}
				else if(Double.IsNaN(d))
				{
					return "NaN";
				}
				else if(Double.IsPositiveInfinity(d))
				{
					return "Infinity";
				}
				else if(Double.IsNegativeInfinity(d))
				{
					return "-Infinity";
				}
				else
				{
					return d.ToString("e", CultureInfo.InvariantCulture);
				}
			}

	// Convert an object into a primitive value.
	internal static Object ToPrimitive(Object value, DefaultValueHint hint)
			{
				if(value is ScriptObject)
				{
					// Let the object handle conversion for JScript objects.
					return ((ScriptObject)value).DefaultValue(hint);
				}
				else
				{
					// Handle non-JScript objects.
					switch(hint)
					{
						case DefaultValueHint.None:
						{
							switch(Support.TypeCodeForObject(value))
							{
								case TypeCode.SByte:
								case TypeCode.Byte:
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
									value = ToNumber(value);
								}
								break;

								default:
								{
									value = ToString(value);
								}
								break;
							}
						}
						break;

						case DefaultValueHint.Number:
						{
							value = ToNumber(value);
						}
						break;

						case DefaultValueHint.String:
						case DefaultValueHint.LocaleString:
						{
							value = ToString(value);
						}
						break;
					}
					return value;
				}
			}

	// Normalize a value down to primitive, but don't apply hard conversions.
	internal static Object NormalizePrimitive(Object value)
			{
				switch(Support.TypeCodeForObject(value))
				{
					case TypeCode.Char:
						return Convert.ToString(value);

					case TypeCode.SByte:
					case TypeCode.Byte:
					case TypeCode.Int16:
					case TypeCode.UInt16:
					case TypeCode.Int32:
					case TypeCode.UInt32:
					case TypeCode.Int64:
					case TypeCode.UInt64:
					case TypeCode.Single:
					case TypeCode.Double:
						return Convert.ToNumber(value);

					default: break;
				}
				return value;
			}

}; // class Convert

}; // namespace Microsoft.JScript
