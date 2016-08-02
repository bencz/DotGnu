/*
 * Conversion.cs - Implementation of the
 *			"Microsoft.VisualBasic.Conversion" class.
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

namespace Microsoft.VisualBasic
{

using System;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;

[StandardModule]
public sealed class Conversion
{
	// This class cannot be instantiated.
	private Conversion() {}

	// Get the error message for a particular error number.
	public static String ErrorToString()
			{
				String desc = Information.Err().Description;
				if(desc == null || desc == String.Empty)
				{
					return ErrorToString(Information.Err().Number);
				}
				else
				{
					return desc;
				}
			}
	public static String ErrorToString(int errornumber)
			{
				if(errornumber == 0)
				{
					return String.Empty;
				}
				String res = S._(String.Format("VB_Error{0}", errornumber));
				if(res != null)
				{
					return res;
				}
				return S._("VB_ErrorDefault");
			}

	// Get the "fix" integer version of a value.
	public static short Fix(short Number)
			{
				return Number;
			}
	public static int Fix(int Number)
			{
				return Number;
			}
	public static long Fix(long Number)
			{
				return Number;
			}
	public static double Fix(double Number)
			{
				if(Number >= 0.0)
				{
					return Math.Floor(Number);
				}
				else
				{
					return Math.Ceiling(Number);
				}
			}
	public static float Fix(float Number)
			{
				if(Number >= 0.0)
				{
					return (float)(Math.Floor(Number));
				}
				else
				{
					return (float)(Math.Ceiling(Number));
				}
			}
	public static Decimal Fix(Decimal Number)
			{
				return Decimal.Truncate(Number);
			}
	public static Object Fix(Object Number)
			{
				if(Number == null)
				{
					throw new ArgumentNullException("Number");
				}
				switch(ObjectType.GetTypeCode(Number))
				{
					case TypeCode.Boolean:
					case TypeCode.Char:
					case TypeCode.SByte:
					case TypeCode.Byte:
					case TypeCode.Int16:
					case TypeCode.UInt16:
					case TypeCode.Int32:
						return Fix(IntegerType.FromObject(Number));

					case TypeCode.UInt32:
					case TypeCode.Int64:
					case TypeCode.UInt64:
						return Number;

					case TypeCode.Single:
						return Fix(SingleType.FromObject(Number));

					case TypeCode.Double:
						return Fix(DoubleType.FromObject(Number));

					case TypeCode.Decimal:
						return Fix(DecimalType.FromObject(Number));

					case TypeCode.String:
						return Fix(DoubleType.FromString
							(StringType.FromObject(Number)));
				}
				throw new ArgumentException(S._("VB_InvalidNumber"), "Number");
			}

	// Get the hexadecimal form of a value.
	public static String Hex(byte Number)
			{
				return Number.ToString("X");
			}
	public static String Hex(short Number)
			{
				return Number.ToString("X");
			}
	public static String Hex(int Number)
			{
				return Number.ToString("X");
			}
	public static String Hex(long Number)
			{
				return Number.ToString("X");
			}
	public static String Hex(Object Number)
			{
				if(Number == null)
				{
					throw new ArgumentNullException("Number");
				}
				switch(ObjectType.GetTypeCode(Number))
				{
					case TypeCode.Byte:
						return Hex(ByteType.FromObject(Number));

					case TypeCode.Int16:
						return Hex(ShortType.FromObject(Number));

					case TypeCode.Boolean:
					case TypeCode.Char:
					case TypeCode.SByte:
					case TypeCode.UInt16:
					case TypeCode.Int32:
						return Hex(IntegerType.FromObject(Number));

					case TypeCode.Int64:
						return Hex(LongType.FromObject(Number));

					case TypeCode.UInt32:
					case TypeCode.UInt64:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.Decimal:
						return Hex(LongType.FromObject(Number));

					case TypeCode.String:
						return Hex(LongType.FromString
							(StringType.FromObject(Number)));
				}
				throw new ArgumentException(S._("VB_InvalidNumber"), "Number");
			}

	// Get the integer version of a value.
	public static short Int(short Number)
			{
				return Number;
			}
	public static int Int(int Number)
			{
				return Number;
			}
	public static long Int(long Number)
			{
				return Number;
			}
	public static double Int(double Number)
			{
				return Math.Floor(Number);
			}
	public static float Int(float Number)
			{
				return (float)(Math.Floor(Number));
			}
	public static Decimal Int(Decimal Number)
			{
				return Decimal.Floor(Number);
			}
	public static Object Int(Object Number)
			{
				if(Number == null)
				{
					throw new ArgumentNullException("Number");
				}
				switch(ObjectType.GetTypeCode(Number))
				{
					case TypeCode.Boolean:
					case TypeCode.Char:
					case TypeCode.SByte:
					case TypeCode.Byte:
					case TypeCode.Int16:
					case TypeCode.UInt16:
					case TypeCode.Int32:
						return Int(IntegerType.FromObject(Number));

					case TypeCode.UInt32:
					case TypeCode.Int64:
					case TypeCode.UInt64:
						return Number;

					case TypeCode.Single:
						return Int(SingleType.FromObject(Number));

					case TypeCode.Double:
						return Int(DoubleType.FromObject(Number));

					case TypeCode.Decimal:
						return Int(DecimalType.FromObject(Number));

					case TypeCode.String:
						return Int(DoubleType.FromString
							(StringType.FromObject(Number)));
				}
				throw new ArgumentException(S._("VB_InvalidNumber"), "Number");
			}

	// Get the octal form of a value.
	public static String Oct(byte Number)
			{
				return Oct((long)Number);
			}
	public static String Oct(short Number)
			{
				return Oct((long)(ushort)Number);
			}
	public static String Oct(int Number)
			{
				return Oct((long)Number);
			}
	public static String Oct(long Number)
			{
				int numDigits = 1;
				long mask = 7;
				while((Number & ~mask) != 0)
				{
					++numDigits;
					mask |= (mask << 3);
				}
				StringBuilder builder = new StringBuilder();
				while(numDigits > 0)
				{
					--numDigits;
					mask = Number >> (numDigits * 3);
					builder.Append((char)(0x30 + (int)(mask & 7)));
				}
				return builder.ToString();
			}
	public static String Oct(Object Number)
			{
				if(Number == null)
				{
					throw new ArgumentNullException("Number");
				}
				switch(ObjectType.GetTypeCode(Number))
				{
					case TypeCode.Byte:
						return Oct(ByteType.FromObject(Number));

					case TypeCode.Int16:
						return Oct(ShortType.FromObject(Number));

					case TypeCode.Boolean:
					case TypeCode.Char:
					case TypeCode.SByte:
					case TypeCode.UInt16:
					case TypeCode.Int32:
						return Oct(IntegerType.FromObject(Number));

					case TypeCode.Int64:
						return Oct(LongType.FromObject(Number));

					case TypeCode.UInt32:
					case TypeCode.UInt64:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.Decimal:
						return Oct(LongType.FromObject(Number));

					case TypeCode.String:
						return Oct(LongType.FromString
							(StringType.FromObject(Number)));
				}
				throw new ArgumentException(S._("VB_InvalidNumber"), "Number");
			}

	// Convert an object into a string.
	public static String Str(Object Number)
			{
				return StringType.FromObject(Number);
			}

	// Get the numeric value of a string.
	public static double Val(String InputStr)
			{
				return DoubleType.FromString(InputStr);
			}
	public static double Val(Object Expression)
			{
				return Val(Str(Expression));
			}
	public static int Val(char Expression)
			{
				if(Expression >= '0' && Expression <= '9')
				{
					return (int)(Expression - '0');
				}
				else
				{
					return 0;
				}
			}

}; // class Conversion

}; // namespace Microsoft.VisualBasic
