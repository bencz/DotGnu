/*
 * Information.cs - Implementation of the
 *			"Microsoft.VisualBasic.Information" class.
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

namespace Microsoft.VisualBasic
{

using System;
using System.Reflection;
using System.Globalization;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;

[StandardModule]
public sealed class Information
{
	// Internal state.
	private static ErrObject err;

	// This class cannot be instantiated.
	private Information() {}

	// Get the ERL code for the current error object.
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Never)]
#endif
	public static int Erl()
			{
				return Err().Erl;
			}

	// Get the current error object.
	public static ErrObject Err()
			{
				if(err == null)
				{
					err = new ErrObject();
				}
				return err;
			}

	// Value type testing methods.
	public static bool IsArray(Object VarName)
			{
				return (VarName is Array);
			}
	public static bool IsDBNull(Object VarName)
			{
			#if !ECMA_COMPAT
				return (VarName is DBNull);
			#else
				return false;
			#endif
			}
	public static bool IsDate(Object VarName)
			{
				if(VarName is DateTime)
				{
					return true;
				}
				else if(VarName is String)
				{
					try
					{
						DateType.FromString((String)VarName);
						return true;
					}
					catch(Exception)
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}
	public static bool IsError(Object VarName)
			{
				return (VarName is Exception);
			}
	public static bool IsNothing(Object VarName)
			{
				return (VarName == null);
			}
	public static bool IsNumeric(Object VarName)
			{
			#if !ECMA_COMPAT
				IConvertible ic = (VarName as IConvertible);
				if(ic != null)
				{
					switch(ic.GetTypeCode())
					{
						case TypeCode.Boolean:
						case TypeCode.Byte:
						case TypeCode.Int16:
						case TypeCode.Int32:
						case TypeCode.Int64:
						case TypeCode.Single:
						case TypeCode.Double:
						case TypeCode.Decimal:	return true;

						case TypeCode.String:
						{
							try
							{
								DoubleType.Parse(ic.ToString(null));
								return true;
							}
							catch(Exception)
							{
								// If we cannot parse, then it isn't numeric.
							}
						}
						break;
					}
				}
				return false;
			#else
				if(VarName != null)
				{
					switch(ObjectType.GetTypeCode(VarName))
					{
						case TypeCode.Boolean:
						case TypeCode.Byte:
						case TypeCode.Int16:
						case TypeCode.Int32:
						case TypeCode.Int64:
						case TypeCode.Single:
						case TypeCode.Double:
						case TypeCode.Decimal:	return true;

						case TypeCode.String:
						{
							try
							{
								DoubleType.Parse(VarName.ToString());
								return true;
							}
							catch(Exception e)
							{
								// If we cannot parse, then it isn't numeric.
							}
						}
						break;
					}
				}
				return false;
			#endif
			}
	public static bool IsReference(Object VarName)
			{
				return !(VarName is ValueType);
			}

	// Get the lower bound of an array.
	public static int LBound
				(Array Array, [Optional] [DefaultValue(1)] int Rank)
			{
				if(Array == null)
				{
					throw new ArgumentNullException("Array");
				}
				else if(Rank < 1 || Rank > Array.Rank)
				{
					throw new RankException(S._("VB_InvalidRank"));
				}
				else
				{
					return Array.GetLowerBound(Rank - 1);
				}
			}

	// Table that maps QB color indexes into RGB values.
	private static int[] qbColors =
			{0x000000, 0x800000, 0x008000, 0x808000,
			 0x000080, 0x800080, 0x008080, 0xC0C0C0,
			 0x808080, 0xFF0000, 0x00FF00, 0xFFFF00,
			 0x0000FF, 0xFF00FF, 0x00FFFF, 0xFFFFFF};

	// Convert a QB color index into an RGB value.
	public static int QBColor(int Color)
			{
				if(Color < 0 || Color > 15)
				{
					throw new ArgumentException
						(S._("VB_InvalidColorIndex"), "Color");
				}
				else
				{
					return qbColors[Color];
				}
			}

	// Build a color value from RGB values.
	public static int RGB(int Red, int Green, int Blue)
			{
				// Validate the parameters.
				if(Red < 0)
				{
					throw new ArgumentException
						(S._("VB_NonNegative"), "Red");
				}
				else if(Red > 255)
				{
					Red = 255;
				}
				if(Green < 0)
				{
					throw new ArgumentException
						(S._("VB_NonNegative"), "Green");
				}
				else if(Green > 255)
				{
					Green = 255;
				}
				if(Blue < 0)
				{
					throw new ArgumentException
						(S._("VB_NonNegative"), "Blue");
				}
				else if(Blue > 255)
				{
					Blue = 255;
				}

				// Build the final color value.
				return (Blue << 16) | (Green << 8) | Red;
			}

	// Get the system type name from a VB type name.
	public static String SystemTypeName(String VbName)
			{
			#if !ECMA_COMPAT
				VbName = (VbName.Trim()).ToLower
					(CultureInfo.InvariantCulture);
			#else
				VbName = (VbName.Trim()).ToLower();
			#endif
				switch(VbName)
				{
					case "boolean":		return "System.Boolean";
					case "char":		return "System.Char";
					case "byte":		return "System.Byte";
					case "short":		return "System.Int16";
					case "integer":		return "System.Int32";
					case "long":		return "System.Int64";
					case "single":		return "System.Single";
					case "double":		return "System.Double";
					case "date":		return "System.DateTime";
					case "decimal":		return "System.Decimal";
					case "string":		return "System.String";
					case "object":		return "System.Object";
				}
				return null;
			}

	// Get the type name for an object.
	public static String TypeName(Object VarName)
			{
				if(VarName == null)
				{
					return "Nothing";
				}
				else
				{
					StringBuilder builder = new StringBuilder();
					Utils.AppendType(builder, VarName.GetType());
					return builder.ToString();
				}
			}

	// Get the upper bound of an array.
	public static int UBound
				(Array Array, [Optional] [DefaultValue(1)] int Rank)
			{
				if(Array == null)
				{
					throw new ArgumentNullException("Array");
				}
				else if(Rank < 1 || Rank > Array.Rank)
				{
					throw new RankException(S._("VB_InvalidRank"));
				}
				else
				{
					return Array.GetUpperBound(Rank - 1);
				}
			}

	// Get the variant type for an object.
	private static VariantType VarTypeForType(Type type)
			{
				if(type.IsArray)
				{
					VariantType vtype = VarTypeForType(type.GetElementType());
					if((vtype & VariantType.Array) != 0)
					{
						// Array of array is transformed into array of object.
						return VariantType.Array | VariantType.Object;
					}
					else
					{
						return VariantType.Array | vtype;
					}
				}
				else if(type.IsEnum)
				{
					// Report the underlying type for enumerations.
					return VarTypeForType(Enum.GetUnderlyingType(type));
				}
				if(type == typeof(bool))
				{
					return VariantType.Boolean;
				}
				else if(type == typeof(char))
				{
					return VariantType.Char;
				}
				else if(type == typeof(byte))
				{
					return VariantType.Byte;
				}
				else if(type == typeof(short))
				{
					return VariantType.Short;
				}
				else if(type == typeof(int))
				{
					return VariantType.Integer;
				}
				else if(type == typeof(long))
				{
					return VariantType.Long;
				}
				else if(type == typeof(float))
				{
					return VariantType.Single;
				}
				else if(type == typeof(double))
				{
					return VariantType.Double;
				}
				else if(type == typeof(Decimal))
				{
					return VariantType.Decimal;
				}
				else if(type == typeof(String))
				{
					return VariantType.String;
				}
				else if(type == typeof(DateTime))
				{
					return VariantType.Date;
				}
			#if !ECMA_COMPAT
				else if(type == typeof(DBNull))
				{
					return VariantType.Null;
				}
				else if(type == typeof(Missing))
				{
					return VariantType.Error;
				}
			#endif
				else if(typeof(Exception).IsAssignableFrom(type))
				{
					return VariantType.Error;
				}
				else if(type.IsValueType)
				{
					return VariantType.UserDefinedType;
				}
				else
				{
					return VariantType.Object;
				}
			}
	public static VariantType VarType(Object VarName)
			{
				if(VarName == null)
				{
					return VariantType.Object;
				}
				else
				{
					return VarTypeForType(VarName.GetType());
				}
			}

	// Convert a system type name into a VB type name.
	public static String VbTypeName(String UrtName)
			{
				// Normalize the name.
			#if !ECMA_COMPAT
				UrtName = (UrtName.Trim()).ToLower
					(CultureInfo.InvariantCulture);
			#else
				UrtName = (UrtName.Trim()).ToLower();
			#endif
				if(UrtName.StartsWith("system."))
				{
					UrtName = UrtName.Substring(7);
				}

				// Check for known runtime engine types with VB equivalents.
				switch(UrtName)
				{
					case "boolean":		return "Boolean";
					case "char":		return "Char";
					case "byte":		return "Byte";
					case "int16":		return "Short";
					case "int32":		return "Integer";
					case "int64":		return "Long";
					case "single":		return "Single";
					case "double":		return "Double";
					case "object":		return "Object";
					case "string":		return "String";
					case "datetime":	return "Date";
					case "decimal":		return "Decimal";
				}
				return null;
			}

}; // class Information

}; // namespace Microsoft.VisualBasic
