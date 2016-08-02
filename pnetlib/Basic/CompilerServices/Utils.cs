/*
 * Utils.cs - Implementation of the
 *			"Microsoft.VisualBasic.Utils" class.
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
using System.Reflection;
using System.Globalization;
using System.Threading;
using System.Text;

[StandardModule]
#if CONFIG_COMPONENT_MODEL
[EditorBrowsable(EditorBrowsableState.Never)]
#endif
public sealed class Utils
{
	// Cannot instantiate this class.
	private Utils() {}

	// Copy the contents of an array.
	public static Array CopyArray(Array arySrc, Array aryDest)
			{
				if(arySrc != null)
				{
					// Check that the arrays have the same rank and dimensions.
					int rank = arySrc.Rank;
					if(rank != aryDest.Rank)
					{
						ThrowExceptionInternal
							(new InvalidCastException
								(S._("VB_MismatchedRanks")));
					}
					for(int dim = 0; dim < rank; ++dim)
					{
						if(arySrc.GetUpperBound(dim) !=
						   aryDest.GetUpperBound(dim))
						{
							ThrowExceptionInternal
								(new ArrayTypeMismatchException
									(S._("VB_MismatchedDimensions")));
						}
					}
					Array.Copy(arySrc, aryDest, arySrc.Length);
				}
				return aryDest;
			}

	// Append the VB form of a type name to a string builder.
	internal static void AppendType(StringBuilder builder, Type type)
			{
				if(type == typeof(System.Byte))
				{
					builder.Append("Byte");
				}
				else if(type == typeof(System.Int16))
				{
					builder.Append("Short");
				}
				else if(type == typeof(System.Int32))
				{
					builder.Append("Integer");
				}
				else if(type == typeof(System.Int64))
				{
					builder.Append("Long");
				}
				else if(type == typeof(System.Single))
				{
					builder.Append("Single");
				}
				else if(type == typeof(System.Double))
				{
					builder.Append("Double");
				}
				else if(type == typeof(System.Boolean))
				{
					builder.Append("Boolean");
				}
				else if(type == typeof(System.Char))
				{
					builder.Append("Char");
				}
				else if(type == typeof(System.String))
				{
					builder.Append("String");
				}
				else if(type == typeof(System.DateTime))
				{
					builder.Append("Date");
				}
				else if(type == typeof(System.Decimal))
				{
					builder.Append("Decimal");
				}
				else if(type == typeof(System.Object))
				{
					builder.Append("Object");
				}
				else if(type.IsArray)
				{
					Type elemType = type.GetElementType();
					while(elemType.IsArray)
					{
						elemType = elemType.GetElementType();
					}
					AppendType(builder, elemType);
					while(type != elemType)
					{
						builder.Append('(');
						int rank = type.GetArrayRank();
						while(rank > 1)
						{
							builder.Append(',');
							--rank;
						}
						builder.Append(')');
						type = type.GetElementType();
					}
				}
				else
				{
					builder.Append(type.FullName);
				}
			}

	// Convert a method name into a string, using VB-style syntax.
	public static String MethodToString(MethodBase Method)
			{
				bool isCtor;
				Type returnType;

				// Determine if the method is a constructor and get
				// the return type if it isn't.
				if(Method is ConstructorInfo)
				{
					isCtor = true;
					returnType = typeof(void);
				}
				else
				{
					isCtor = false;
					returnType = ((MethodInfo)Method).ReturnType;
				}

				// Create a string builder and output the access level.
				StringBuilder builder = new StringBuilder();
				if((Method.Attributes & MethodAttributes.Virtual) != 0)
				{
					if(Method.DeclaringType.IsInterface)
					{
						builder.Append("Overrides ");
					}
				}
				switch(Method.Attributes & MethodAttributes.MemberAccessMask)
				{
					case MethodAttributes.Private:
						builder.Append("Private "); break;

					case MethodAttributes.Assembly:
						builder.Append("Friend "); break;

					case MethodAttributes.Family:
						builder.Append("Protected "); break;

					case MethodAttributes.FamORAssem:
						builder.Append("Protected Friend "); break;

					case MethodAttributes.Public:
						builder.Append("Public "); break;
				}

				// Output the method kind and its name.
				if(isCtor)
				{
					builder.Append("Sub New ");
				}
				else if(returnType == typeof(void))
				{
					builder.Append("Sub ");
				}
				else
				{
					builder.Append("Function ");
				}
				builder.Append(Method.Name);

				// Output the method parameters.
				ParameterInfo[] parameters = Method.GetParameters();
				int index;
				Type type;
				String name;
				Object value;
				builder.Append(" (");
				for(index = 0; index < parameters.Length; ++index)
				{
					if(index > 0)
					{
						builder.Append(", ");
					}
					type = parameters[index].ParameterType;
					if((parameters[index].Attributes
							& ParameterAttributes.Optional) != 0)
					{
						builder.Append("Optional ");
					}
					if(type.IsByRef)
					{
						builder.Append("ByRef ");
					}
					else
					{
						builder.Append("ByVal ");
					}
				#if !ECMA_COMPAT
					if(type.IsArray && parameters[index].IsDefined
							(typeof(ParamArrayAttribute), false))
					{
						builder.Append("ParamArray ");
					}
				#endif
					name = parameters[index].Name;
					if(name != null)
					{
						builder.Append(name);
					}
					else
					{
						builder.Append(String.Format("_p{0}", index));
					}
					builder.Append(" As ");
					AppendType(builder, returnType);
				#if !ECMA_COMPAT
					if(parameters[index].IsOptional)
					{
						builder.Append(" = ");
						value = parameters[index].DefaultValue;
						if(value != null)
						{
							if(value is String)
							{
								builder.Append('"');
								builder.Append(value);
								builder.Append('"');
							}
							else
							{
								builder.Append(value.ToString());
							}
						}
						else
						{
							builder.Append("Nothing");
						}
					}
				#endif
				}
				builder.Append(")");

				// Output the return type for functions.
				if(returnType != typeof(void))
				{
					builder.Append(" As ");
					AppendType(builder, returnType);
				}

				// Return the final string to the caller.
				return builder.ToString();
			}

	// Set the culture information to use.
	public static Object SetCultureInfo(CultureInfo Culture)
			{
				CultureInfo prev = CultureInfo.CurrentCulture;
			#if !ECMA_COMPAT
				Thread.CurrentThread.CurrentCulture = Culture;
			#endif
				return prev;
			}

	// Throw an exception by number.
	public static void ThrowException(int hr)
			{
				Exception exception =
					ErrObject.CreateExceptionFromNumber(hr, null);
				SetErrorNumber(hr);
				throw exception;
			}

	// Throw a particular exception, after setting the error number.
	private static void ThrowExceptionInternal(Exception exception)
			{
				SetErrorNumber(ErrObject.GetNumberForException(exception));
				throw exception;
			}

	// Set a particular error number in the program's error object.
	private static void SetErrorNumber(int number)
			{
				ErrObject error = Information.Err();
				error.Clear();
				error.Number = number;
			}

	// Full width digit characters (0-9, A-F, a-f).
	private static char[] fullWidthDigits =
			{'\uFF10', '\uFF11', '\uFF12', '\uFF13', '\uFF14',
			 '\uFF15', '\uFF16', '\uFF17', '\uFF18', '\uFF19',
			 '\uFF21', '\uFF22', '\uFF23', '\uFF24', '\uFF25', '\uFF26',
			 '\uFF41', '\uFF42', '\uFF43', '\uFF44', '\uFF45', '\uFF46'};

	// Convert full-width digit characters into half-width digits.
	// This is useful in CJK locales where it is common for users to
	// expect that full-width forms of digits can be used interchangeably
	// with half-width forms.
	internal static String FixDigits(String str)
			{
				if(str == null)
				{
					return null;
				}
				if(str.IndexOfAny(fullWidthDigits) == -1)
				{
					return str;
				}
				StringBuilder builder = new StringBuilder(str.Length);
				foreach(char ch in str)
				{
					if(ch >= '\uFF10' && ch <= '\uFF19')
					{
						builder.Append((char)(ch - (0xFF10 + 0x0030)));
					}
					else if(ch >= '\uFF21' && ch <= '\uFF26')
					{
						builder.Append((char)(ch - (0xFF21 + 0x0041)));
					}
					else if(ch >= '\uFF41' && ch <= '\uFF46')
					{
						builder.Append((char)(ch - (0xFF41 + 0x0061)));
					}
					else
					{
						builder.Append(ch);
					}
				}
				return builder.ToString();
			}

	// Convert a string into its wide form.
	internal static String ToWide(String str)
			{
				int posn, len;
				char ch;
				len = str.Length;
				for(posn = 0; posn < len; ++posn)
				{
					ch = str[posn];
					if(ch >= '\u0021' && ch <= '\u007E')
					{
						break;
					}
				}
				if(posn >= len)
				{
					return str;
				}
				StringBuilder builder = new StringBuilder();
				for(posn = 0; posn < len; ++posn)
				{
					ch = str[posn];
					if(ch >= '\u0021' && ch <= '\u007E')
					{
						builder.Append((char)(ch - 0x0021 + 0xFF01));
					}
					else
					{
						builder.Append(ch);
					}
				}
				return builder.ToString();
			}

	// Convert a string into its narrow form.
	internal static String ToNarrow(String str)
			{
				int posn, len;
				char ch;
				len = str.Length;
				for(posn = 0; posn < len; ++posn)
				{
					ch = str[posn];
					if(ch >= '\uFF01' && ch <= '\uFF5E')
					{
						break;
					}
				}
				if(posn >= len)
				{
					return str;
				}
				StringBuilder builder = new StringBuilder();
				for(posn = 0; posn < len; ++posn)
				{
					ch = str[posn];
					if(ch >= '\uFF01' && ch <= '\uFF5E')
					{
						builder.Append((char)(ch - 0xFF01 + 0x0021));
					}
					else
					{
						builder.Append(ch);
					}
				}
				return builder.ToString();
			}

	// Convert katakana characters into hiragana.
	internal static String ToHiragana(String str)
			{
				int posn, len;
				char ch;
				len = str.Length;
				for(posn = 0; posn < len; ++posn)
				{
					ch = str[posn];
					if(ch >= '\u30A1' && ch <= '\u30FE')
					{
						break;
					}
				}
				if(posn >= len)
				{
					return str;
				}
				StringBuilder builder = new StringBuilder();
				for(posn = 0; posn < len; ++posn)
				{
					ch = str[posn];
					if(ch >= '\u30A1' && ch <= '\u30FE')
					{
						builder.Append((char)(ch - 0x30A1 + 0x3041));
					}
					else
					{
						builder.Append(ch);
					}
				}
				return builder.ToString();
			}

	// Convert hiragana characters into katakana.
	internal static String ToKatakana(String str)
			{
				int posn, len;
				char ch;
				len = str.Length;
				for(posn = 0; posn < len; ++posn)
				{
					ch = str[posn];
					if(ch >= '\u3041' && ch <= '\u309E')
					{
						break;
					}
				}
				if(posn >= len)
				{
					return str;
				}
				StringBuilder builder = new StringBuilder();
				for(posn = 0; posn < len; ++posn)
				{
					ch = str[posn];
					if(ch >= '\u3041' && ch <= '\u309E')
					{
						builder.Append((char)(ch - 0x3041 + 0x30A1));
					}
					else
					{
						builder.Append(ch);
					}
				}
				return builder.ToString();
			}

	// Base time for OLE Automation values (Midnight, Dec 30, 1899).
	private static readonly DateTime OLEBaseTime = new DateTime(1899, 12, 30);

	// Convert an OLE Automation date into a DateTime value.
	internal static DateTime FromOADate(double d)
			{
				// Convert the value into ticks, while checking for overflow.
				long ticks;
				checked
				{
					try
					{
						ticks = ((long)(d * (double)TimeSpan.TicksPerDay)) +
								OLEBaseTime.Ticks;
					}
					catch(OverflowException)
					{
						throw new ArgumentOutOfRangeException
							("d", S._("VB_DateTimeRange"));
					}
				}

				// Convert the ticks into a DateTime value, which will
				// perform additional range checking.
				return new DateTime(ticks);
			}

	// Convert this DateTime value into an OLE Automation date.
	internal static double ToOADate(DateTime d)
			{
				long value_ = d.Ticks;
				if(value_ == 0)
				{
					// Special case for uninitialized DateTime values.
					return 0.0;
				}
				else
				{
					return (((double)value_) / ((double)TimeSpan.TicksPerDay))
								- ((double)OLEBaseTime.Ticks);
				}
			}

}; // class Utils

}; // namespace Microsoft.VisualBasic.CompilerServices
