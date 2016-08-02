/*
 * Convert.cs - Implementation of the "System.Convert" class.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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

using System.Private;
using System.Text;
using System.Globalization;

public sealed class Convert
{
#if !ECMA_COMPAT
	internal static readonly Type[] ConvertTypes =
		{typeof(Empty), typeof(Object), typeof(System.DBNull),
		 typeof(Boolean), typeof(Char), typeof(SByte), typeof(Byte),
		 typeof(Int16), typeof(UInt16), typeof(Int32), typeof(UInt32),
		 typeof(Int64), typeof(UInt64), typeof(Single), typeof(Double),
		 typeof(Decimal), typeof(DateTime), typeof(Object), typeof(String)};
	public static readonly Object DBNull = System.DBNull.Value;
#endif // !ECMA_COMPAT

	// This class cannot be instantiated.
	private Convert() {}

	// Convert various types into Boolean.
	public static bool ToBoolean(bool value) { return value; }
	public static bool ToBoolean(byte value) { return (value != 0); }
	[CLSCompliant(false)]
	public static bool ToBoolean(sbyte value) { return (value != 0); }
	public static bool ToBoolean(short value) { return (value != 0); }
	[CLSCompliant(false)]
	public static bool ToBoolean(ushort value) { return (value != 0); }
	public static bool ToBoolean(int value) { return (value != 0); }
	[CLSCompliant(false)]
	public static bool ToBoolean(uint value) { return (value != 0); }
	public static bool ToBoolean(long value) { return (value != 0); }
	[CLSCompliant(false)]
	public static bool ToBoolean(ulong value) { return (value != 0); }
#if CONFIG_EXTENDED_NUMERICS
	public static bool ToBoolean(float value) { return (value != 0.0); }
	public static bool ToBoolean(double value) { return (value != 0.0); }
	public static bool ToBoolean(Decimal value)
			{
				return (value != 0.0m);
			}
#endif
	public static bool ToBoolean(String value)
			{
				return Boolean.Parse(value);
			}
#if !ECMA_COMPAT
	public static bool ToBoolean(char value)
			{
				return ((IConvertible)value).ToBoolean(null);
			}
	public static bool ToBoolean(DateTime value)
			{
				return ((IConvertible)value).ToBoolean(null);
			}
	public static bool ToBoolean(Object value)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToBoolean(null);
				}
				else
				{
					return false;
				}
			}
	public static bool ToBoolean(Object value, IFormatProvider provider)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToBoolean(provider);
				}
				else
				{
					return false;
				}
			}
	public static bool ToBoolean(String value, IFormatProvider provider)
			{
				return Boolean.Parse(value);
			}
#else // ECMA_COMPAT
	internal static bool ToBoolean(Object value)
			{
				if(value == null)
				{
					return false;
				}
				Type type = value.GetType();
				if(type == typeof(byte))
				{
					return ToBoolean((byte)value);
				}
				else if(type == typeof(sbyte))
				{
					return ToBoolean((sbyte)value);
				}
				else if(type == typeof(short))
				{
					return ToBoolean((short)value);
				}
				else if(type == typeof(ushort))
				{
					return ToBoolean((ushort)value);
				}
				else if(type == typeof(char))
				{
					return ToBoolean((char)value);
				}
				else if(type == typeof(int))
				{
					return ToBoolean((int)value);
				}
				else if(type == typeof(uint))
				{
					return ToBoolean((uint)value);
				}
				else if(type == typeof(long))
				{
					return ToBoolean((long)value);
				}
				else if(type == typeof(ulong))
				{
					return ToBoolean((ulong)value);
				}
#if CONFIG_EXTENDED_NUMERICS
				else if(type == typeof(float))
				{
					return ToBoolean((float)value);
				}
				else if(type == typeof(double))
				{
					return ToBoolean((double)value);
				}
				else if(type == typeof(Decimal))
				{
					return ToBoolean((Decimal)value);
				}
#endif
				else if(type == typeof(String))
				{
					return ToBoolean((String)value);
				}
				else if(type == typeof(bool))
				{
					return (bool)value;
				}
				else
				{
					throw new InvalidCastException
						(String.Format
							(_("InvalidCast_FromTo"),
 						     type.FullName, typeof(bool).FullName));
				}
			}
#endif // ECMA_COMPAT

	// Convert various types into Byte.
	public static byte ToByte(bool value) { return (byte)(value ? 1 : 0); }
	public static byte ToByte(byte value) { return value; }
	[CLSCompliant(false)]
	public static byte ToByte(sbyte value)
			{
				if(value >= 0)
				{
					return unchecked((byte)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_Byte"));
				}
			}
	public static byte ToByte(short value)
			{
				if(value >= 0 && value <= 255)
				{
					return unchecked((byte)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_Byte"));
				}
			}
	[CLSCompliant(false)]
	public static byte ToByte(ushort value)
			{
				if(value <= 255)
				{
					return unchecked((byte)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_Byte"));
				}
			}
	public static byte ToByte(int value)
			{
				if(value >= 0 && value <= 255)
				{
					return unchecked((byte)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_Byte"));
				}
			}
	[CLSCompliant(false)]
	public static byte ToByte(uint value)
			{
				if(value <= 255)
				{
					return unchecked((byte)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_Byte"));
				}
			}
	public static byte ToByte(long value)
			{
				if(value >= 0 && value <= 255)
				{
					return unchecked((byte)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_Byte"));
				}
			}
	[CLSCompliant(false)]
	public static byte ToByte(ulong value)
			{
				if(value <= 255)
				{
					return unchecked((byte)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_Byte"));
				}
			}
	public static byte ToByte(char value)
			{
				if(value <= 255)
				{
					return unchecked((byte)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_Byte"));
				}
			}
#if CONFIG_EXTENDED_NUMERICS
	public static byte ToByte(float value)
			{
				return ToByte((double)value);
			}
	public static byte ToByte(double value)
			{
				return ToByte(ToInt32(value));
			}
	public static byte ToByte(Decimal value)
			{
				return Decimal.ToByte(Decimal.Round(value, 0));
			}
#endif
	public static byte ToByte(String value)
			{
				return Byte.Parse(value);
			}
	public static byte ToByte(String value, IFormatProvider provider)
			{
				return Byte.Parse(value, provider);
			}
#if !ECMA_COMPAT
	public static byte ToByte(DateTime value)
			{
				return ((IConvertible)value).ToByte(null);
			}
	public static byte ToByte(Object value)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToByte(null);
				}
				else
				{
					return 0;
				}
			}
	public static byte ToByte(Object value, IFormatProvider provider)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToByte(provider);
				}
				else
				{
					return 0;
				}
			}
	public static byte ToByte(String value, int fromBase)
			{
				if(fromBase == 2 || fromBase == 8 ||
				   fromBase == 10 || fromBase == 16)
				{
					return ToByte(NumberParser.StringToUInt32
										(value, fromBase, 256));
				}
				else
				{
					throw new ArgumentException(_("Arg_InvalidBase"));
				}
			}
#else // ECMA_COMPAT
	internal static byte ToByte(Object value)
			{
				if(value == null)
				{
					return 0;
				}
				Type type = value.GetType();
				if(type == typeof(byte))
				{
					return (byte)value;
				}
				else if(type == typeof(sbyte))
				{
					return ToByte((sbyte)value);
				}
				else if(type == typeof(short))
				{
					return ToByte((short)value);
				}
				else if(type == typeof(ushort))
				{
					return ToByte((ushort)value);
				}
				else if(type == typeof(char))
				{
					return ToByte((char)value);
				}
				else if(type == typeof(int))
				{
					return ToByte((int)value);
				}
				else if(type == typeof(uint))
				{
					return ToByte((uint)value);
				}
				else if(type == typeof(long))
				{
					return ToByte((long)value);
				}
				else if(type == typeof(ulong))
				{
					return ToByte((ulong)value);
				}
#if CONFIG_EXTENDED_NUMERICS
				else if(type == typeof(float))
				{
					return ToByte((float)value);
				}
				else if(type == typeof(double))
				{
					return ToByte((double)value);
				}
				else if(type == typeof(Decimal))
				{
					return ToByte((Decimal)value);
				}
#endif
				else if(type == typeof(String))
				{
					return ToByte((String)value);
				}
				else if(type == typeof(bool))
				{
					return ToByte((bool)value);
				}
				else
				{
					throw new InvalidCastException
						(String.Format
							(_("InvalidCast_FromTo"),
 						     type.FullName, typeof(byte).FullName));
				}
			}
#endif // ECMA_COMPAT

	// Convert various types into SByte.
	[CLSCompliant(false)]
	public static sbyte ToSByte(bool value) { return (sbyte)(value ? 1 : 0); }
	[CLSCompliant(false)]
	public static sbyte ToSByte(byte value)
			{
				if(value <= 127)
				{
					return unchecked((sbyte)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_SByte"));
				}
			}
	[CLSCompliant(false)]
	public static sbyte ToSByte(sbyte value)	{ return value; }
	[CLSCompliant(false)]
	public static sbyte ToSByte(short value)
			{
				if(value >= -128 && value <= 127)
				{
					return unchecked((sbyte)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_SByte"));
				}
			}
	[CLSCompliant(false)]
	public static sbyte ToSByte(ushort value)
			{
				if(value <= 127)
				{
					return unchecked((sbyte)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_SByte"));
				}
			}
	[CLSCompliant(false)]
	public static sbyte ToSByte(int value)
			{
				if(value >= -128 && value <= 127)
				{
					return unchecked((sbyte)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_SByte"));
				}
			}
	[CLSCompliant(false)]
	public static sbyte ToSByte(uint value)
			{
				if(value <= 127)
				{
					return unchecked((sbyte)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_SByte"));
				}
			}
	[CLSCompliant(false)]
	public static sbyte ToSByte(long value)
			{
				if(value >= -127 && value <= 127)
				{
					return unchecked((sbyte)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_SByte"));
				}
			}
	[CLSCompliant(false)]
	public static sbyte ToSByte(ulong value)
			{
				if(value <= 127)
				{
					return unchecked((sbyte)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_SByte"));
				}
			}
	[CLSCompliant(false)]
	public static sbyte ToSByte(char value)
			{
				if(value <= 127)
				{
					return unchecked((sbyte)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_SByte"));
				}
			}
#if CONFIG_EXTENDED_NUMERICS
	[CLSCompliant(false)]
	public static sbyte ToSByte(float value)
			{
				return ToSByte((double)value);
			}
	[CLSCompliant(false)]
	public static sbyte ToSByte(double value)
			{
				return ToSByte(ToInt32(value));
			}
	[CLSCompliant(false)]
	public static sbyte ToSByte(Decimal value)
			{
				return Decimal.ToSByte(Decimal.Round(value, 0));
			}
#endif
	[CLSCompliant(false)]
	public static sbyte ToSByte(String value)
			{
				return SByte.Parse(value);
			}
	[CLSCompliant(false)]
	public static sbyte ToSByte(String value, IFormatProvider provider)
			{
				return SByte.Parse(value, provider);
			}
#if !ECMA_COMPAT
	[CLSCompliant(false)]
	public static sbyte ToSByte(DateTime value)
			{
				return ((IConvertible)value).ToSByte(null);
			}
	[CLSCompliant(false)]
	public static sbyte ToSByte(Object value)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToSByte(null);
				}
				else
				{
					return 0;
				}
			}
	[CLSCompliant(false)]
	public static sbyte ToSByte(Object value, IFormatProvider provider)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToSByte(provider);
				}
				else
				{
					return 0;
				}
			}
	[CLSCompliant(false)]
	public static sbyte ToSByte(String value, int fromBase)
			{
				if(fromBase == 2 || fromBase == 8 ||
				   fromBase == 10 || fromBase == 16)
				{
					return ToSByte(NumberParser.StringToInt32
								   (value, fromBase, 128, 0x80, Byte.MaxValue));
				}
				else
				{
					throw new ArgumentException(_("Arg_InvalidBase"));
				}
			}
#else // ECMA_COMPAT
	internal static sbyte ToSByte(Object value)
			{
				if(value == null)
				{
					return 0;
				}
				Type type = value.GetType();
				if(type == typeof(byte))
				{
					return ToSByte((byte)value);
				}
				else if(type == typeof(sbyte))
				{
					return (sbyte)value;
				}
				else if(type == typeof(short))
				{
					return ToSByte((short)value);
				}
				else if(type == typeof(ushort))
				{
					return ToSByte((ushort)value);
				}
				else if(type == typeof(char))
				{
					return ToSByte((char)value);
				}
				else if(type == typeof(int))
				{
					return ToSByte((int)value);
				}
				else if(type == typeof(uint))
				{
					return ToSByte((uint)value);
				}
				else if(type == typeof(long))
				{
					return ToSByte((long)value);
				}
				else if(type == typeof(ulong))
				{
					return ToSByte((ulong)value);
				}
#if CONFIG_EXTENDED_NUMERICS
				else if(type == typeof(float))
				{
					return ToSByte((float)value);
				}
				else if(type == typeof(double))
				{
					return ToSByte((double)value);
				}
				else if(type == typeof(Decimal))
				{
					return ToSByte((Decimal)value);
				}
#endif
				else if(type == typeof(String))
				{
					return ToSByte((String)value);
				}
				else if(type == typeof(bool))
				{
					return ToSByte((bool)value);
				}
				else
				{
					throw new InvalidCastException
						(String.Format
							(_("InvalidCast_FromTo"),
 						     type.FullName, typeof(sbyte).FullName));
				}
			}
#endif // ECMA_COMPAT

	// Convert various types into Int16.
	public static short ToInt16(bool value) { return (short)(value ? 1 : 0); }
	public static short ToInt16(byte value)
			{
				return unchecked((short)value);
			}
	[CLSCompliant(false)]
	public static short ToInt16(sbyte value)
			{
				return unchecked((short)value);
			}
	public static short ToInt16(short value) { return value; }
	[CLSCompliant(false)]
	public static short ToInt16(ushort value)
			{
				if(value <= 32767)
				{
					return unchecked((short)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_Int16"));
				}
			}
	public static short ToInt16(int value)
			{
				if(value >= -32768 && value <= 32767)
				{
					return unchecked((short)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_Int16"));
				}
			}
	[CLSCompliant(false)]
	public static short ToInt16(uint value)
			{
				if(value <= 32767)
				{
					return unchecked((short)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_Int16"));
				}
			}
	public static short ToInt16(long value)
			{
				if(value >= -32768 && value <= 32767)
				{
					return unchecked((short)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_Int16"));
				}
			}
	[CLSCompliant(false)]
	public static short ToInt16(ulong value)
			{
				if(value <= 32767)
				{
					return unchecked((short)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_Int16"));
				}
			}
	public static short ToInt16(char value)
			{
				if(value <= 32767)
				{
					return unchecked((short)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_Int16"));
				}
			}
#if CONFIG_EXTENDED_NUMERICS
	public static short ToInt16(float value)
			{
				return ToInt16((double)value);
			}
	public static short ToInt16(double value)
			{
				return ToInt16(ToInt32(value));
			}
	public static short ToInt16(Decimal value)
			{
				return Decimal.ToInt16(Decimal.Round(value, 0));
			}
#endif
	public static short ToInt16(String value)
			{
				return Int16.Parse(value);
			}
	public static short ToInt16(String value, IFormatProvider provider)
			{
				return Int16.Parse(value, provider);
			}
#if !ECMA_COMPAT
	public static short ToInt16(DateTime value)
			{
				return ((IConvertible)value).ToInt16(null);
			}
	public static short ToInt16(Object value)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToInt16(null);
				}
				else
				{
					return 0;
				}
			}
	public static short ToInt16(Object value, IFormatProvider provider)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToInt16(provider);
				}
				else
				{
					return 0;
				}
			}
	public static short ToInt16(String value, int fromBase)
			{
				if(fromBase == 2 || fromBase == 8 ||
				   fromBase == 10 || fromBase == 16)
				{
					return ToInt16(NumberParser.StringToInt32
								   (value, fromBase, 32768, 0x8000,
									UInt16.MaxValue));
				}
				else
				{
					throw new ArgumentException(_("Arg_InvalidBase"));
				}
			}
#else // ECMA_COMPAT
	internal static short ToInt16(Object value)
			{
				if(value == null)
				{
					return 0;
				}
				Type type = value.GetType();
				if(type == typeof(byte))
				{
					return ToInt16((byte)value);
				}
				else if(type == typeof(sbyte))
				{
					return ToInt16((sbyte)value);
				}
				else if(type == typeof(short))
				{
					return (short)value;
				}
				else if(type == typeof(ushort))
				{
					return ToInt16((ushort)value);
				}
				else if(type == typeof(char))
				{
					return ToInt16((char)value);
				}
				else if(type == typeof(int))
				{
					return ToInt16((int)value);
				}
				else if(type == typeof(uint))
				{
					return ToInt16((uint)value);
				}
				else if(type == typeof(long))
				{
					return ToInt16((long)value);
				}
				else if(type == typeof(ulong))
				{
					return ToInt16((ulong)value);
				}
#if CONFIG_EXTENDED_NUMERICS
				else if(type == typeof(float))
				{
					return ToInt16((float)value);
				}
				else if(type == typeof(double))
				{
					return ToInt16((double)value);
				}
				else if(type == typeof(Decimal))
				{
					return ToInt16((Decimal)value);
				}
#endif
				else if(type == typeof(String))
				{
					return ToInt16((String)value);
				}
				else if(type == typeof(bool))
				{
					return ToInt16((bool)value);
				}
				else
				{
					throw new InvalidCastException
						(String.Format
							(_("InvalidCast_FromTo"),
 						     type.FullName, typeof(short).FullName));
				}
			}
#endif // ECMA_COMPAT

	// Convert various types into UInt16.
	[CLSCompliant(false)]
	public static ushort ToUInt16(bool value)
			{
				return (ushort)(value ? 1 : 0);
			}
	[CLSCompliant(false)]
	public static ushort ToUInt16(byte value)
			{
				return unchecked((ushort)value);
			}
	[CLSCompliant(false)]
	public static ushort ToUInt16(sbyte value)
			{
				if(value >= 0)
				{
					return unchecked((ushort)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_UInt16"));
				}
			}
	[CLSCompliant(false)]
	public static ushort ToUInt16(short value)
			{
				if(value >= 0)
				{
					return unchecked((ushort)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_UInt16"));
				}
			}
	[CLSCompliant(false)]
	public static ushort ToUInt16(ushort value) { return value; }
	[CLSCompliant(false)]
	public static ushort ToUInt16(int value)
			{
				if(value >= 0 && value <= 65535)
				{
					return unchecked((ushort)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_UInt16"));
				}
			}
	[CLSCompliant(false)]
	public static ushort ToUInt16(uint value)
			{
				if(value <= 65535)
				{
					return unchecked((ushort)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_UInt16"));
				}
			}
	[CLSCompliant(false)]
	public static ushort ToUInt16(long value)
			{
				if(value >= 0 && value <= 65535)
				{
					return unchecked((ushort)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_UInt16"));
				}
			}
	[CLSCompliant(false)]
	public static ushort ToUInt16(ulong value)
			{
				if(value <= 65535)
				{
					return unchecked((ushort)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_UInt16"));
				}
			}
	[CLSCompliant(false)]
	public static ushort ToUInt16(char value)
			{
				return unchecked((ushort)value);
			}
#if CONFIG_EXTENDED_NUMERICS
	[CLSCompliant(false)]
	public static ushort ToUInt16(float value)
			{
				return ToUInt16((double)value);
			}
	[CLSCompliant(false)]
	public static ushort ToUInt16(double value)
			{
				return ToUInt16(ToInt32(value));
			}
	[CLSCompliant(false)]
	public static ushort ToUInt16(Decimal value)
			{
				return Decimal.ToUInt16(Decimal.Round(value, 0));
			}
#endif
	[CLSCompliant(false)]
	public static ushort ToUInt16(String value)
			{
				return UInt16.Parse(value);
			}
	[CLSCompliant(false)]
	public static ushort ToUInt16(String value, IFormatProvider provider)
			{
				return UInt16.Parse(value, provider);
			}
#if !ECMA_COMPAT
	[CLSCompliant(false)]
	public static ushort ToUInt16(DateTime value)
			{
				return ((IConvertible)value).ToUInt16(null);
			}
	[CLSCompliant(false)]
	public static ushort ToUInt16(Object value)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToUInt16(null);
				}
				else
				{
					return 0;
				}
			}
	[CLSCompliant(false)]
	public static ushort ToUInt16(Object value, IFormatProvider provider)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToUInt16(provider);
				}
				else
				{
					return 0;
				}
			}
	[CLSCompliant(false)]
	public static ushort ToUInt16(String value, int fromBase)
			{
				if(fromBase == 2 || fromBase == 8 ||
				   fromBase == 10 || fromBase == 16)
				{
					return ToUInt16(NumberParser.StringToUInt32
										(value, fromBase, 65536));
				}
				else
				{
					throw new ArgumentException(_("Arg_InvalidBase"));
				}
			}
#else // ECMA_COMPAT
	internal static ushort ToUInt16(Object value)
			{
				if(value == null)
				{
					return 0;
				}
				Type type = value.GetType();
				if(type == typeof(byte))
				{
					return ToUInt16((byte)value);
				}
				else if(type == typeof(sbyte))
				{
					return ToUInt16((sbyte)value);
				}
				else if(type == typeof(short))
				{
					return ToUInt16((short)value);
				}
				else if(type == typeof(ushort))
				{
					return (ushort)value;
				}
				else if(type == typeof(char))
				{
					return ToUInt16((char)value);
				}
				else if(type == typeof(int))
				{
					return ToUInt16((int)value);
				}
				else if(type == typeof(uint))
				{
					return ToUInt16((uint)value);
				}
				else if(type == typeof(long))
				{
					return ToUInt16((long)value);
				}
				else if(type == typeof(ulong))
				{
					return ToUInt16((ulong)value);
				}
#if CONFIG_EXTENDED_NUMERICS
				else if(type == typeof(float))
				{
					return ToUInt16((float)value);
				}
				else if(type == typeof(double))
				{
					return ToUInt16((double)value);
				}
				else if(type == typeof(Decimal))
				{
					return ToUInt16((Decimal)value);
				}
#endif
				else if(type == typeof(String))
				{
					return ToUInt16((String)value);
				}
				else if(type == typeof(bool))
				{
					return ToUInt16((bool)value);
				}
				else
				{
					throw new InvalidCastException
						(String.Format
							(_("InvalidCast_FromTo"),
 						     type.FullName, typeof(ushort).FullName));
				}
			}
#endif // ECMA_COMPAT

	// Convert various types into Int32.
	public static int ToInt32(bool value) { return (value ? 1 : 0); }
	public static int ToInt32(byte value)
			{
				return unchecked((int)value);
			}
	[CLSCompliant(false)]
	public static int ToInt32(sbyte value)
			{
				return unchecked((int)value);
			}
	public static int ToInt32(short value)
			{
				return unchecked((int)value);
			}
	[CLSCompliant(false)]
	public static int ToInt32(ushort value)
			{
				return unchecked((int)value);
			}
	public static int ToInt32(int value) { return value; }
	[CLSCompliant(false)]
	public static int ToInt32(uint value)
			{
				if(value <= 2147483647)
				{
					return unchecked((int)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_Int32"));
				}
			}
	public static int ToInt32(long value)
			{
				if(value >= -2147483648 && value <= 2147483647)
				{
					return unchecked((int)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_Int32"));
				}
			}
	[CLSCompliant(false)]
	public static int ToInt32(ulong value)
			{
				if(value <= 2147483647)
				{
					return unchecked((int)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_Int32"));
				}
			}
	public static int ToInt32(char value)
			{
				return unchecked((int)value);
			}
#if CONFIG_EXTENDED_NUMERICS
	public static int ToInt32(float value)
			{
				return ToInt32((double)value);
			}
	public static int ToInt32(double value)
			{
				// Let the runtime engine do the hard work
				// of detecting when overflow occurs.
				try
				{
					int iNum = checked((int)value);
					// Round value to the nearest 32-bit signed int. 
					// if value is halfway between two whole numbers, the even number is returned.
					// Sample: 
					//         4.5 is converted to 4  (!)  
					//         5.5 is converted to 6  
					double d2 = value - iNum;
					if( iNum >= 0 ) {
						if( d2 > 0.5 || ( d2 == 0.5 && ( (iNum & 1) != 0 ) ) ) {
							iNum++;
						}
					}
					else {
						if( d2 < -0.5 || ( d2 == -0.5 && ( (iNum & 1) != 0 ) ) ) {
							iNum--;
						}
					}
					return iNum;
				}
				catch(OverflowException)
				{
					// Convert the runtime exception into
					// one with a localized error string.
					throw new OverflowException(_("Overflow_Int32"));
				}
			}
	public static int ToInt32(Decimal value)
			{
				return Decimal.ToInt32(Decimal.Round(value, 0));
			}
#endif
	public static int ToInt32(String value)
			{
				return Int32.Parse(value);
			}
	public static int ToInt32(String value, IFormatProvider provider)
			{
				return Int32.Parse(value, provider);
			}
#if !ECMA_COMPAT
	public static int ToInt32(DateTime value)
			{
				return ((IConvertible)value).ToInt32(null);
			}
	public static int ToInt32(Object value)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToInt32(null);
				}
				else
				{
					return 0;
				}
			}
	public static int ToInt32(Object value, IFormatProvider provider)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToInt32(provider);
				}
				else
				{
					return 0;
				}
			}
	public static int ToInt32(String value, int fromBase)
			{
				if(fromBase == 2 || fromBase == 8 ||
				   fromBase == 10 || fromBase == 16)
				{
					return NumberParser.StringToInt32(value, fromBase, 0, 0x80000000, UInt32.MaxValue);
				}
				else
				{
					throw new ArgumentException(_("Arg_InvalidBase"));
				}
			}
#else // ECMA_COMPAT
	internal static int ToInt32(Object value)
			{
				if(value == null)
				{
					return 0;
				}
				Type type = value.GetType();
				if(type == typeof(byte))
				{
					return ToInt32((byte)value);
				}
				else if(type == typeof(sbyte))
				{
					return ToInt32((sbyte)value);
				}
				else if(type == typeof(short))
				{
					return ToInt32((short)value);
				}
				else if(type == typeof(ushort))
				{
					return ToInt32((ushort)value);
				}
				else if(type == typeof(char))
				{
					return ToInt32((char)value);
				}
				else if(type == typeof(int))
				{
					return (int)value;
				}
				else if(type == typeof(uint))
				{
					return ToInt32((uint)value);
				}
				else if(type == typeof(long))
				{
					return ToInt32((long)value);
				}
				else if(type == typeof(ulong))
				{
					return ToInt32((ulong)value);
				}
#if CONFIG_EXTENDED_NUMERICS
				else if(type == typeof(float))
				{
					return ToInt32((float)value);
				}
				else if(type == typeof(double))
				{
					return ToInt32((double)value);
				}
				else if(type == typeof(Decimal))
				{
					return ToInt32((Decimal)value);
				}
#endif
				else if(type == typeof(String))
				{
					return ToInt32((String)value);
				}
				else if(type == typeof(bool))
				{
					return ToInt32((bool)value);
				}
				else
				{
					throw new InvalidCastException
						(String.Format
							(_("InvalidCast_FromTo"),
 						     type.FullName, typeof(int).FullName));
				}
			}
#endif // ECMA_COMPAT

	// Convert various types into UInt32.
	[CLSCompliant(false)]
	public static uint ToUInt32(bool value) { return (uint)(value ? 1 : 0); }
	[CLSCompliant(false)]
	public static uint ToUInt32(byte value)
			{
				return unchecked((uint)value);
			}
	[CLSCompliant(false)]
	public static uint ToUInt32(sbyte value)
			{
				if(value >= 0)
				{
					return unchecked((uint)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_UInt32"));
				}
			}
	[CLSCompliant(false)]
	public static uint ToUInt32(short value)
			{
				if(value >= 0)
				{
					return unchecked((uint)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_UInt32"));
				}
			}
	[CLSCompliant(false)]
	public static uint ToUInt32(ushort value)
			{
				return unchecked((uint)value);
			}
	[CLSCompliant(false)]
	public static uint ToUInt32(int value)
			{
				if(value >= 0)
				{
					return unchecked((uint)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_UInt32"));
				}
			}
	[CLSCompliant(false)]
	public static uint ToUInt32(uint value) { return value; }
	[CLSCompliant(false)]
	public static uint ToUInt32(long value)
			{
				if(value >= 0 && value <= 4294967295)
				{
					return unchecked((uint)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_UInt32"));
				}
			}
	[CLSCompliant(false)]
	public static uint ToUInt32(ulong value)
			{
				if(value <= 4294967295)
				{
					return unchecked((uint)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_UInt32"));
				}
			}
	[CLSCompliant(false)]
	public static uint ToUInt32(char value)
			{
				return unchecked((uint)value);
			}
#if CONFIG_EXTENDED_NUMERICS
	[CLSCompliant(false)]
	public static uint ToUInt32(float value)
			{
				return ToUInt32((double)value);
			}
	[CLSCompliant(false)]
	public static uint ToUInt32(double value)
			{
				// Let the runtime engine do the hard work
				// of detecting when overflow occurs.
				try
				{
					uint uiNum = checked((uint)value);
					// Round value to the nearest 32-bit unsigned int. 
					// if value is halfway between two whole numbers, the even number is returned.
					// Sample: 
					//         4.5 is converted to 4  (!)  
					//         5.5 is converted to 6  
					double d2 = value - uiNum;
					if( d2 > 0.5 || ( d2 == 0.5 && ( (uiNum & 1) != 0 ) ) ) {
						uiNum++;
					}
					return uiNum;
				}
				catch(OverflowException)
				{
					// Convert the runtime exception into
					// one with a localized error string.
					throw new OverflowException(_("Overflow_UInt32"));
				}
			}
	[CLSCompliant(false)]
	public static uint ToUInt32(Decimal value)
			{
				return Decimal.ToUInt32(Decimal.Round(value, 0));
			}
#endif
	[CLSCompliant(false)]
	public static uint ToUInt32(String value)
			{
				return UInt32.Parse(value);
			}
	[CLSCompliant(false)]
	public static uint ToUInt32(String value, IFormatProvider provider)
			{
				return UInt32.Parse(value, provider);
			}
#if !ECMA_COMPAT
	[CLSCompliant(false)]
	public static uint ToUInt32(DateTime value)
			{
				return ((IConvertible)value).ToUInt32(null);
			}
	[CLSCompliant(false)]
	public static uint ToUInt32(Object value)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToUInt32(null);
				}
				else
				{
					return 0;
				}
			}
	[CLSCompliant(false)]
	public static uint ToUInt32(Object value, IFormatProvider provider)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToUInt32(provider);
				}
				else
				{
					return 0;
				}
			}
	[CLSCompliant(false)]
	public static uint ToUInt32(String value, int fromBase)
			{
				if(fromBase == 2 || fromBase == 8 ||
				   fromBase == 10 || fromBase == 16)
				{
					return NumberParser.StringToUInt32(value, fromBase, 0);
				}
				else
				{
					throw new ArgumentException(_("Arg_InvalidBase"));
				}
			}
#else // ECMA_COMPAT
	internal static uint ToUInt32(Object value)
			{
				if(value == null)
				{
					return 0;
				}
				Type type = value.GetType();
				if(type == typeof(byte))
				{
					return ToUInt32((byte)value);
				}
				else if(type == typeof(sbyte))
				{
					return ToUInt32((sbyte)value);
				}
				else if(type == typeof(short))
				{
					return ToUInt32((short)value);
				}
				else if(type == typeof(ushort))
				{
					return ToUInt32((ushort)value);
				}
				else if(type == typeof(char))
				{
					return ToUInt32((char)value);
				}
				else if(type == typeof(int))
				{
					return ToUInt32((int)value);
				}
				else if(type == typeof(uint))
				{
					return (uint)value;
				}
				else if(type == typeof(long))
				{
					return ToUInt32((long)value);
				}
				else if(type == typeof(ulong))
				{
					return ToUInt32((ulong)value);
				}
#if CONFIG_EXTENDED_NUMERICS
				else if(type == typeof(float))
				{
					return ToUInt32((float)value);
				}
				else if(type == typeof(double))
				{
					return ToUInt32((double)value);
				}
				else if(type == typeof(Decimal))
				{
					return ToUInt32((Decimal)value);
				}
#endif
				else if(type == typeof(String))
				{
					return ToUInt32((String)value);
				}
				else if(type == typeof(bool))
				{
					return ToUInt32((bool)value);
				}
				else
				{
					throw new InvalidCastException
						(String.Format
							(_("InvalidCast_FromTo"),
 						     type.FullName, typeof(uint).FullName));
				}
			}
#endif // ECMA_COMPAT

	// Convert various types into Int64.
	public static long ToInt64(bool value) { return (value ? 1 : 0); }
	public static long ToInt64(byte value)
			{
				return unchecked((long)value);
			}
	[CLSCompliant(false)]
	public static long ToInt64(sbyte value)
			{
				return unchecked((long)value);
			}
	public static long ToInt64(short value)
			{
				return unchecked((long)value);
			}
	[CLSCompliant(false)]
	public static long ToInt64(ushort value)
			{
				return unchecked((long)value);
			}
	public static long ToInt64(int value)
			{
				return unchecked((long)value);
			}
	[CLSCompliant(false)]
	public static long ToInt64(uint value)
			{
				return unchecked((long)value);
			}
	public static long ToInt64(long value) { return value; }
	[CLSCompliant(false)]
	public static long ToInt64(ulong value)
			{
				if(value <= 9223372036854775807)
				{
					return unchecked((long)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_Int64"));
				}
			}
	public static long ToInt64(char value)
			{
				return unchecked((long)value);
			}
#if CONFIG_EXTENDED_NUMERICS
	public static long ToInt64(float value)
			{
				return ToInt64((double)value);
			}
	public static long ToInt64(double value)
			{
				// Let the runtime engine do the hard work
				// of detecting when overflow occurs.
				try
				{
					long lNum = checked((long)value);
					// Round value to the nearest 64-bit signed int. 
					// if value is halfway between two whole numbers, the even number is returned.
					// Sample: 
					//         4.5 is converted to 4  (!)  
					//         5.5 is converted to 6  
					double d2 = value - lNum;
					if( lNum >= 0 ) {
						if( d2 > 0.5 || ( d2 == 0.5 && ( (lNum & 1) != 0 ) ) ) {
							lNum++;
						}
					}
					else {
						if( d2 < -0.5 || ( d2 == -0.5 && ( (lNum & 1) != 0 ) ) ) {
							lNum--;
						}
					}
					return lNum;
				}
				catch(OverflowException)
				{
					// Convert the runtime exception into
					// one with a localized error string.
					throw new OverflowException(_("Overflow_Int64"));
				}
			}
	public static long ToInt64(Decimal value)
			{
				return Decimal.ToInt64(Decimal.Round(value, 0));
			}
#endif
	public static long ToInt64(String value)
			{
				return Int64.Parse(value);
			}
	public static long ToInt64(String value, IFormatProvider provider)
			{
				return Int64.Parse(value, provider);
			}
#if !ECMA_COMPAT
	public static long ToInt64(DateTime value)
			{
				return ((IConvertible)value).ToInt64(null);
			}
	public static long ToInt64(Object value)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToInt64(null);
				}
				else
				{
					return 0;
				}
			}
	public static long ToInt64(Object value, IFormatProvider provider)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToInt64(provider);
				}
				else
				{
					return 0;
				}
			}
	public static long ToInt64(String value, int fromBase)
			{
				if(fromBase == 2 || fromBase == 8 ||
				   fromBase == 10 || fromBase == 16)
				{
					return NumberParser.StringToInt64(value, fromBase);
				}
				else
				{
					throw new ArgumentException(_("Arg_InvalidBase"));
				}
			}
#else // ECMA_COMPAT
	internal static long ToInt64(Object value)
			{
				if(value == null)
				{
					return 0;
				}
				Type type = value.GetType();
				if(type == typeof(byte))
				{
					return ToInt64((byte)value);
				}
				else if(type == typeof(sbyte))
				{
					return ToInt64((sbyte)value);
				}
				else if(type == typeof(short))
				{
					return ToInt64((short)value);
				}
				else if(type == typeof(ushort))
				{
					return ToInt64((ushort)value);
				}
				else if(type == typeof(char))
				{
					return ToInt64((char)value);
				}
				else if(type == typeof(int))
				{
					return ToInt64((int)value);
				}
				else if(type == typeof(uint))
				{
					return ToInt64((uint)value);
				}
				else if(type == typeof(long))
				{
					return (long)value;
				}
				else if(type == typeof(ulong))
				{
					return ToInt64((ulong)value);
				}
#if CONFIG_EXTENDED_NUMERICS
				else if(type == typeof(float))
				{
					return ToInt64((float)value);
				}
				else if(type == typeof(double))
				{
					return ToInt64((double)value);
				}
				else if(type == typeof(Decimal))
				{
					return ToInt64((Decimal)value);
				}
#endif
				else if(type == typeof(String))
				{
					return ToInt64((String)value);
				}
				else if(type == typeof(bool))
				{
					return ToInt64((bool)value);
				}
				else
				{
					throw new InvalidCastException
						(String.Format
							(_("InvalidCast_FromTo"),
 						     type.FullName, typeof(long).FullName));
				}
			}
#endif // ECMA_COMPAT

	// Convert various types into UInt64.
	[CLSCompliant(false)]
	public static ulong ToUInt64(bool value) { return (ulong)(value ? 1 : 0); }
	[CLSCompliant(false)]
	public static ulong ToUInt64(byte value)
			{
				return unchecked((ulong)value);
			}
	[CLSCompliant(false)]
	public static ulong ToUInt64(sbyte value)
			{
				if(value >= 0)
				{
					return unchecked((ulong)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_UInt64"));
				}
			}
	[CLSCompliant(false)]
	public static ulong ToUInt64(short value)
			{
				if(value >= 0)
				{
					return unchecked((ulong)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_UInt64"));
				}
			}
	[CLSCompliant(false)]
	public static ulong ToUInt64(ushort value)
			{
				return unchecked((ulong)value);
			}
	[CLSCompliant(false)]
	public static ulong ToUInt64(int value)
			{
				if(value >= 0)
				{
					return unchecked((ulong)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_UInt64"));
				}
			}
	[CLSCompliant(false)]
	public static ulong ToUInt64(uint value)
			{
				return unchecked((ulong)value);
			}
	[CLSCompliant(false)]
	public static ulong ToUInt64(long value)
			{
				if(value >= 0)
				{
					return unchecked((ulong)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_UInt64"));
				}
			}
	[CLSCompliant(false)]
	public static ulong ToUInt64(ulong value) { return value; }
	[CLSCompliant(false)]
	public static ulong ToUInt64(char value)
			{
				return unchecked((ulong)value);
			}
#if CONFIG_EXTENDED_NUMERICS
	[CLSCompliant(false)]
	public static ulong ToUInt64(float value)
			{
				return ToUInt64((double)value);
			}
	[CLSCompliant(false)]
	public static ulong ToUInt64(double value)
			{
				// Let the runtime engine do the hard work
				// of detecting when overflow occurs.
				try
				{
					ulong ulNum = checked((ulong)value);
					// Round value to the nearest 64-bit signed int. 
					// if value is halfway between two whole numbers, the even number is returned.
					// Sample: 
					//         4.5 is converted to 4  (!)  
					//         5.5 is converted to 6  
					double d2 = value - ulNum;
					if( d2 > 0.5 || ( d2 == 0.5 && ( (ulNum & 1) != 0 ) ) ) {
						ulNum++;
					}
					return ulNum;
				}
				catch(OverflowException)
				{
					// Convert the runtime exception into
					// one with a localized error string.
					throw new OverflowException(_("Overflow_UInt64"));
				}
			}
	[CLSCompliant(false)]
	public static ulong ToUInt64(Decimal value)
			{
				return Decimal.ToUInt64(Decimal.Round(value, 0));
			}
#endif
	[CLSCompliant(false)]
	public static ulong ToUInt64(String value)
			{
				return UInt64.Parse(value);
			}
	[CLSCompliant(false)]
	public static ulong ToUInt64(String value, IFormatProvider provider)
			{
				return UInt64.Parse(value, provider);
			}
#if !ECMA_COMPAT
	[CLSCompliant(false)]
	public static ulong ToUInt64(DateTime value)
			{
				return ((IConvertible)value).ToUInt64(null);
			}
	[CLSCompliant(false)]
	public static ulong ToUInt64(Object value)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToUInt64(null);
				}
				else
				{
					return 0;
				}
			}
	[CLSCompliant(false)]
	public static ulong ToUInt64(Object value, IFormatProvider provider)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToUInt64(provider);
				}
				else
				{
					return 0;
				}
			}
	[CLSCompliant(false)]
	public static ulong ToUInt64(String value, int fromBase)
			{
				if(fromBase == 2 || fromBase == 8 ||
				   fromBase == 10 || fromBase == 16)
				{
					return NumberParser.StringToUInt64(value, fromBase);
				}
				else
				{
					throw new ArgumentException(_("Arg_InvalidBase"));
				}
			}
#else // ECMA_COMPAT
	internal static ulong ToUInt64(Object value)
			{
				if(value == null)
				{
					return 0;
				}
				Type type = value.GetType();
				if(type == typeof(byte))
				{
					return ToUInt64((byte)value);
				}
				else if(type == typeof(sbyte))
				{
					return ToUInt64((sbyte)value);
				}
				else if(type == typeof(short))
				{
					return ToUInt64((short)value);
				}
				else if(type == typeof(ushort))
				{
					return ToUInt64((ushort)value);
				}
				else if(type == typeof(char))
				{
					return ToUInt64((char)value);
				}
				else if(type == typeof(int))
				{
					return ToUInt64((int)value);
				}
				else if(type == typeof(uint))
				{
					return ToUInt64((uint)value);
				}
				else if(type == typeof(long))
				{
					return ToUInt64((long)value);
				}
				else if(type == typeof(ulong))
				{
					return (ulong)value;
				}
#if CONFIG_EXTENDED_NUMERICS
				else if(type == typeof(float))
				{
					return ToUInt64((float)value);
				}
				else if(type == typeof(double))
				{
					return ToUInt64((double)value);
				}
				else if(type == typeof(Decimal))
				{
					return ToUInt64((Decimal)value);
				}
#endif
				else if(type == typeof(String))
				{
					return ToUInt64((String)value);
				}
				else if(type == typeof(bool))
				{
					return ToUInt64((bool)value);
				}
				else
				{
					throw new InvalidCastException
						(String.Format
							(_("InvalidCast_FromTo"),
 						     type.FullName, typeof(ulong).FullName));
				}
			}
#endif // ECMA_COMPAT

	// Convert various types into Char.
	public static char ToChar(byte value)
			{
				return unchecked((char)value);
			}
	[CLSCompliant(false)]
	public static char ToChar(sbyte value)
			{
				if(value >= 0)
				{
					return unchecked((char)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_Char"));
				}
			}
	public static char ToChar(short value)
			{
				if(value >= 0)
				{
					return unchecked((char)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_Char"));
				}
			}
	[CLSCompliant(false)]
	public static char ToChar(ushort value)
			{
				return unchecked((char)value);
			}
	public static char ToChar(int value)
			{
				if(value >= 0 && value <= 65535)
				{
					return unchecked((char)(ushort)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_Char"));
				}
			}
	[CLSCompliant(false)]
	public static char ToChar(uint value)
			{
				if(value <= 65535)
				{
					return unchecked((char)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_Char"));
				}
			}
	public static char ToChar(long value)
			{
				if(value >= 0 && value <= 65535)
				{
					return unchecked((char)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_Char"));
				}
			}
	[CLSCompliant(false)]
	public static char ToChar(ulong value)
			{
				if(value <= 65535)
				{
					return unchecked((char)value);
				}
				else
				{
					throw new OverflowException(_("Overflow_Char"));
				}
			}
	public static char ToChar(char value) { return value; }
	public static char ToChar(String value)
			{
				if(value != null)
				{
					if(value.Length == 1)
					{
						return value[0];
					}
					else
					{
						throw new FormatException(_("Format_NeedSingleChar"));
					}
				}
				else
				{
					throw new ArgumentNullException("value");
				}
			}
#if !ECMA_COMPAT
#if CONFIG_EXTENDED_NUMERICS
	public static char ToChar(float value)
			{
				return ((IConvertible)value).ToChar(null);
			}
	public static char ToChar(double value)
			{
				return ((IConvertible)value).ToChar(null);
			}
	public static char ToChar(Decimal value)
			{
				return ((IConvertible)value).ToChar(null);
			}
#endif
	public static char ToChar(bool value)
			{
				return ((IConvertible)value).ToChar(null);
			}
	public static char ToChar(DateTime value)
			{
				return ((IConvertible)value).ToChar(null);
			}
	public static char ToChar(Object value)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToChar(null);
				}
				else
				{
					return '\u0000';
				}
			}
	public static char ToChar(Object value, IFormatProvider provider)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToChar(provider);
				}
				else
				{
					return '\u0000';
				}
			}
	public static char ToChar(String value, IFormatProvider provider)
			{
				return ToChar(value);
			}
#else // ECMA_COMPAT
	internal static char ToChar(Object value)
			{
				if(value == null)
				{
					return '\u0000';
				}
				Type type = value.GetType();
				if(type == typeof(byte))
				{
					return ToChar((byte)value);
				}
				else if(type == typeof(sbyte))
				{
					return ToChar((sbyte)value);
				}
				else if(type == typeof(short))
				{
					return ToChar((short)value);
				}
				else if(type == typeof(ushort))
				{
					return ToChar((ushort)value);
				}
				else if(type == typeof(char))
				{
					return (char)value;
				}
				else if(type == typeof(int))
				{
					return ToChar((int)value);
				}
				else if(type == typeof(uint))
				{
					return ToChar((uint)value);
				}
				else if(type == typeof(long))
				{
					return ToChar((long)value);
				}
				else if(type == typeof(ulong))
				{
					return ToChar((ulong)value);
				}
#if CONFIG_EXTENDED_NUMERICS
				else if(type == typeof(float))
				{
					return ToChar((float)value);
				}
				else if(type == typeof(double))
				{
					return ToChar((double)value);
				}
				else if(type == typeof(Decimal))
				{
					return ToChar((Decimal)value);
				}
#endif
				else if(type == typeof(String))
				{
					return ToChar((String)value);
				}
				else if(type == typeof(bool))
				{
					return ToChar((bool)value);
				}
				else
				{
					throw new InvalidCastException
						(String.Format
							(_("InvalidCast_FromTo"),
 						     type.FullName, typeof(char).FullName));
				}
			}
#endif // ECMA_COMPAT

#if CONFIG_EXTENDED_NUMERICS

	// Convert various types into Single.
	public static float ToSingle(bool value) { return (value ? 1.0f : 0.0f); }
	public static float ToSingle(byte value)
			{
				return unchecked((float)value);
			}
	[CLSCompliant(false)]
	public static float ToSingle(sbyte value)
			{
				return unchecked((float)value);
			}
	public static float ToSingle(short value)
			{
				return unchecked((float)value);
			}
	[CLSCompliant(false)]
	public static float ToSingle(ushort value)
			{
				return unchecked((float)value);
			}
	public static float ToSingle(int value)
			{
				return unchecked((float)value);
			}
	[CLSCompliant(false)]
	public static float ToSingle(uint value)
			{
				return unchecked((float)value);
			}
	public static float ToSingle(long value)
			{
				return unchecked((float)value);
			}
	[CLSCompliant(false)]
	public static float ToSingle(ulong value)
			{
				return unchecked((float)value);
			}
	public static float ToSingle(float value) { return value; }
	public static float ToSingle(double value)
			{
				return unchecked((float)value);
			}
	public static float ToSingle(Decimal value)
			{
				return (float)value;
			}
	public static float ToSingle(String value)
			{
				return Single.Parse(value);
			}
	public static float ToSingle(String value, IFormatProvider provider)
			{
				return Single.Parse(value, provider);
			}
#if !ECMA_COMPAT
	public static float ToSingle(char value)
			{
				return unchecked((float)value);
			}
	public static float ToSingle(Object value)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToSingle(null);
				}
				else
				{
					return 0;
				}
			}
	public static float ToSingle(Object value, IFormatProvider provider)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToSingle(provider);
				}
				else
				{
					return 0;
				}
			}
	public static float ToSingle(DateTime value)
			{
				return ((IConvertible)value).ToSingle(null);
			}
#else // ECMA_COMPAT
	internal static float ToSingle(Object value)
			{
				if(value == null)
				{
					return 0;
				}
				Type type = value.GetType();
				if(type == typeof(byte))
				{
					return ToSingle((byte)value);
				}
				else if(type == typeof(sbyte))
				{
					return ToSingle((sbyte)value);
				}
				else if(type == typeof(short))
				{
					return ToSingle((short)value);
				}
				else if(type == typeof(ushort))
				{
					return ToSingle((ushort)value);
				}
				else if(type == typeof(char))
				{
					return ToSingle((char)value);
				}
				else if(type == typeof(int))
				{
					return ToSingle((int)value);
				}
				else if(type == typeof(uint))
				{
					return ToSingle((uint)value);
				}
				else if(type == typeof(long))
				{
					return ToSingle((long)value);
				}
				else if(type == typeof(ulong))
				{
					return ToSingle((ulong)value);
				}
				else if(type == typeof(float))
				{
					return (float)value;
				}
				else if(type == typeof(double))
				{
					return (float)(double)value;
				}
				else if(type == typeof(Decimal))
				{
					return ToSingle((Decimal)value);
				}
				else if(type == typeof(String))
				{
					return ToSingle((String)value);
				}
				else if(type == typeof(bool))
				{
					return ToSingle((bool)value);
				}
				else
				{
					throw new InvalidCastException
						(String.Format
							(_("InvalidCast_FromTo"),
 						     type.FullName, typeof(float).FullName));
				}
			}
#endif // ECMA_COMPAT

	// Convert various types into Double.
	public static double ToDouble(bool value) { return (value ? 1.0d : 0.0d); }
	public static double ToDouble(byte value)
			{
				return unchecked((double)value);
			}
	[CLSCompliant(false)]
	public static double ToDouble(sbyte value)
			{
				return unchecked((double)value);
			}
	public static double ToDouble(short value)
			{
				return unchecked((double)value);
			}
	[CLSCompliant(false)]
	public static double ToDouble(ushort value)
			{
				return unchecked((double)value);
			}
	public static double ToDouble(int value)
			{
				return unchecked((double)value);
			}
	[CLSCompliant(false)]
	public static double ToDouble(uint value)
			{
				return unchecked((double)value);
			}
	public static double ToDouble(long value)
			{
				return unchecked((double)value);
			}
	[CLSCompliant(false)]
	public static double ToDouble(ulong value)
			{
				return unchecked((double)value);
			}
	public static double ToDouble(float value)
			{
				return unchecked((double)value);
			}
	public static double ToDouble(double value) { return value; }
	public static double ToDouble(Decimal value)
			{
				return (double)value;
			}
	public static double ToDouble(String value)
			{
				return Double.Parse(value);
			}
	public static double ToDouble(String value, IFormatProvider provider)
			{
				return Double.Parse(value, provider);
			}
#if !ECMA_COMPAT
	public static double ToDouble(char value)
			{
				return unchecked((double)value);
			}
	public static double ToDouble(Object value)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToDouble(null);
				}
				else
				{
					return 0;
				}
			}
	public static double ToDouble(Object value, IFormatProvider provider)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToDouble(provider);
				}
				else
				{
					return 0;
				}
			}
	public static double ToDouble(DateTime value)
			{
				return ((IConvertible)value).ToDouble(null);
			}
#else // ECMA_COMPAT
	internal static double ToDouble(Object value)
			{
				if(value == null)
				{
					return 0;
				}
				Type type = value.GetType();
				if(type == typeof(byte))
				{
					return ToDouble((byte)value);
				}
				else if(type == typeof(sbyte))
				{
					return ToDouble((sbyte)value);
				}
				else if(type == typeof(short))
				{
					return ToDouble((short)value);
				}
				else if(type == typeof(ushort))
				{
					return ToDouble((ushort)value);
				}
				else if(type == typeof(char))
				{
					return ToDouble((char)value);
				}
				else if(type == typeof(int))
				{
					return ToDouble((int)value);
				}
				else if(type == typeof(uint))
				{
					return ToDouble((uint)value);
				}
				else if(type == typeof(long))
				{
					return ToDouble((long)value);
				}
				else if(type == typeof(ulong))
				{
					return ToDouble((ulong)value);
				}
				else if(type == typeof(float))
				{
					return (double)(float)value;
				}
				else if(type == typeof(double))
				{
					return (double)value;
				}
				else if(type == typeof(Decimal))
				{
					return ToDouble((Decimal)value);
				}
				else if(type == typeof(String))
				{
					return ToDouble((String)value);
				}
				else if(type == typeof(bool))
				{
					return ToDouble((bool)value);
				}
				else
				{
					throw new InvalidCastException
						(String.Format
							(_("InvalidCast_FromTo"),
 						     type.FullName, typeof(double).FullName));
				}
			}
#endif // ECMA_COMPAT

#endif // CONFIG_EXTENDED_NUMERICS

	// Convert various types into String.
	public static String ToString(bool value)
			{
				return value.ToString();
			}
	public static String ToString(byte value)
			{
				return value.ToString();
			}
	public static String ToString(byte value, IFormatProvider provider)
			{
				return value.ToString(provider);
			}
	[CLSCompliant(false)]
	public static String ToString(sbyte value)
			{
				return value.ToString();
			}
	[CLSCompliant(false)]
	public static String ToString(sbyte value, IFormatProvider provider)
			{
				return value.ToString(provider);
			}
	public static String ToString(short value)
			{
				return value.ToString();
			}
	public static String ToString(short value, IFormatProvider provider)
			{
				return value.ToString(provider);
			}
	[CLSCompliant(false)]
	public static String ToString(ushort value)
			{
				return value.ToString();
			}
	[CLSCompliant(false)]
	public static String ToString(ushort value, IFormatProvider provider)
			{
				return value.ToString(provider);
			}
	public static String ToString(int value)
			{
				return value.ToString();
			}
	public static String ToString(int value, IFormatProvider provider)
			{
				return value.ToString(provider);
			}
	[CLSCompliant(false)]
	public static String ToString(uint value)
			{
				return value.ToString();
			}
	[CLSCompliant(false)]
	public static String ToString(uint value, IFormatProvider provider)
			{
				return value.ToString(provider);
			}
	public static String ToString(long value)
			{
				return value.ToString();
			}
	public static String ToString(long value, IFormatProvider provider)
			{
				return value.ToString(provider);
			}
	[CLSCompliant(false)]
	public static String ToString(ulong value)
			{
				return value.ToString();
			}
	[CLSCompliant(false)]
	public static String ToString(ulong value, IFormatProvider provider)
			{
				return value.ToString(provider);
			}
	public static String ToString(char value)
			{
				return value.ToString();
			}
#if CONFIG_EXTENDED_NUMERICS
	public static String ToString(float value)
			{
				return value.ToString();
			}
	public static String ToString(float value, IFormatProvider provider)
			{
				return value.ToString(provider);
			}
	public static String ToString(double value)
			{
				return value.ToString();
			}
	public static String ToString(double value, IFormatProvider provider)
			{
				return value.ToString(provider);
			}
	public static String ToString(Decimal value)
			{
				return value.ToString();
			}
	public static String ToString(Decimal value, IFormatProvider provider)
			{
				return value.ToString(provider);
			}
#endif
	public static String ToString(DateTime value)
			{
				return value.ToString();
			}
	public static String ToString(DateTime value, IFormatProvider provider)
			{
				return value.ToString(provider);
			}
	public static String ToString(String value)
			{
				return value;
			}
#if !ECMA_COMPAT
	public static String ToString(String value, IFormatProvider provider)
			{
				return value;
			}
#endif

#if !ECMA_COMPAT
	// Format an integer in base 10.
	private static String FormatBase10(ulong value, bool isneg)
	{
		String basic;
		if(value == 0)
		{
			basic = "0";
		}
		else
		{
			basic = "";
			while(value != 0)
			{
				basic = ((char)((value % 10) + (int)'0')) + basic;
				value /= 10;
			}
		}
		if(isneg)
		{
			return "-" + basic;
		}
		else
		{
			return basic;
		}
	}

	// Format a number in a specific base.
	private static String FormatInBase(long value, int toBase, int numBits)
	{
		char[] buf;
		int posn;
		int digit;
		if(toBase == 2)
		{
			buf = new char[numBits];
			posn = numBits - 1;
			while(posn >= 0)
			{
				if((value & 1) != 0)
				{
					buf[posn--] = '1';
				}
				else
				{
					buf[posn--] = '0';
				}
				value >>= 1;
			}
		}
		else if(toBase == 8)
		{
			buf = new char[(numBits + 2) / 3];
			posn = ((numBits + 2) / 3) - 1;
			while(posn >= 0)
			{
				buf[posn--] = unchecked((char)((value % 8) + (long)'0'));
				value >>= 3;
			}
		}
		else if(toBase == 10)
		{
			if(value < 0)
			{
				return FormatBase10(unchecked((ulong)(-value)), true);
			}
			else
			{
				return FormatBase10(unchecked((ulong)value), false);
			}
		}
		else if(toBase == 16)
		{
			buf = new char[numBits / 4];
			posn = (numBits / 4) - 1;
			while(posn >= 0)
			{
				digit = unchecked((int)(value % 16));
				if(digit < 10)
				{
					buf[posn--] = unchecked((char)(digit + (int)'0'));
				}
				else
				{
					buf[posn--] = unchecked((char)(digit - 10 + (int)'A'));
				}
				value >>= 4;
			}
		}
		else
		{
			throw new ArgumentException(_("Arg_InvalidBase"));
		}
		return new String(buf);
	}

	public static String ToString(bool value, IFormatProvider provider)
			{
				return value.ToString(provider);
			}
	public static String ToString(char value, IFormatProvider provider)
			{
				return value.ToString(provider);
			}
	public static String ToString(byte value, int toBase)
			{
				return FormatInBase((long)value, toBase, 8);
			}
	public static String ToString(short value, int toBase)
			{
				return FormatInBase((long)value, toBase, 16);
			}
	public static String ToString(int value, int toBase)
			{
				return FormatInBase((long)value, toBase, 32);
			}
	public static String ToString(long value, int toBase)
			{
				return FormatInBase((long)value, toBase, 64);
			}
	public static String ToString(Object value)
			{
				if(value != null)
				{
					return value.ToString();
				}
				else
				{
					return String.Empty;
				}
			}
	public static String ToString(Object value, IFormatProvider provider)
			{
				IConvertible iconv = (value as IConvertible);
				if(iconv != null)
				{
					return iconv.ToString(provider);
				}
				else if(value != null)
				{
					return value.ToString();
				}
				else
				{
					return String.Empty;
				}
			}
#else // ECMA_COMPAT
	internal static String ToString(Object value)
			{
				if(value != null)
				{
					return value.ToString();
				}
				else
				{
					return String.Empty;
				}
			}
#endif // ECMA_COMPAT

	// Convert various types into DateTime.
	public static DateTime ToDateTime(DateTime value) { return value; }
	public static DateTime ToDateTime(String value)
			{
				return DateTime.Parse(value);
			}
	public static DateTime ToDateTime(String value, IFormatProvider provider)
			{
				return DateTime.Parse(value, provider);
			}
#if !ECMA_COMPAT
	public static DateTime ToDateTime(bool value)
			{
				return ((IConvertible)value).ToDateTime(null);
			}
	public static DateTime ToDateTime(byte value)
			{
				return ((IConvertible)value).ToDateTime(null);
			}
	[CLSCompliant(false)]
	public static DateTime ToDateTime(sbyte value)
			{
				return ((IConvertible)value).ToDateTime(null);
			}
	public static DateTime ToDateTime(short value)
			{
				return ((IConvertible)value).ToDateTime(null);
			}
	[CLSCompliant(false)]
	public static DateTime ToDateTime(ushort value)
			{
				return ((IConvertible)value).ToDateTime(null);
			}
	public static DateTime ToDateTime(char value)
			{
				return ((IConvertible)value).ToDateTime(null);
			}
	public static DateTime ToDateTime(int value)
			{
				return ((IConvertible)value).ToDateTime(null);
			}
	[CLSCompliant(false)]
	public static DateTime ToDateTime(uint value)
			{
				return ((IConvertible)value).ToDateTime(null);
			}
	public static DateTime ToDateTime(long value)
			{
				return ((IConvertible)value).ToDateTime(null);
			}
	[CLSCompliant(false)]
	public static DateTime ToDateTime(ulong value)
			{
				return ((IConvertible)value).ToDateTime(null);
			}
#if CONFIG_EXTENDED_NUMERICS
	public static DateTime ToDateTime(float value)
			{
				return ((IConvertible)value).ToDateTime(null);
			}
	public static DateTime ToDateTime(double value)
			{
				return ((IConvertible)value).ToDateTime(null);
			}
	public static DateTime ToDateTime(Decimal value)
			{
				return ((IConvertible)value).ToDateTime(null);
			}
#endif
	public static DateTime ToDateTime(Object value)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToDateTime(null);
				}
				else
				{
					return DateTime.MinValue;
				}
			}
	public static DateTime ToDateTime(Object value, IFormatProvider provider)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToDateTime(provider);
				}
				else
				{
					return DateTime.MinValue;
				}
			}
#endif // !ECMA_COMPAT

#if CONFIG_EXTENDED_NUMERICS

	// Convert various types into Decimal.
	public static Decimal ToDecimal(bool value)
			{
				return (value ? 1.0m : 0.0m);
			}
	public static Decimal ToDecimal(byte value)
			{
				return (Decimal)value;
			}
	[CLSCompliant(false)]
	public static Decimal ToDecimal(sbyte value)
			{
				return (Decimal)value;
			}
	public static Decimal ToDecimal(short value)
			{
				return (Decimal)value;
			}
	[CLSCompliant(false)]
	public static Decimal ToDecimal(ushort value)
			{
				return (Decimal)value;
			}
	public static Decimal ToDecimal(int value)
			{
				return (Decimal)value;
			}
	[CLSCompliant(false)]
	public static Decimal ToDecimal(uint value)
			{
				return (Decimal)value;
			}
	public static Decimal ToDecimal(long value)
			{
				return (Decimal)value;
			}
	[CLSCompliant(false)]
	public static Decimal ToDecimal(ulong value)
			{
				return (Decimal)value;
			}
	public static Decimal ToDecimal(float value)
			{
				return (Decimal)value;
			}
	public static Decimal ToDecimal(double value)
			{
				return (Decimal)value;
			}
	public static Decimal ToDecimal(Decimal value) { return value; }
	public static Decimal ToDecimal(String value)
			{
				return Decimal.Parse(value);
			}
	public static Decimal ToDecimal(String value, IFormatProvider provider)
			{
				return Decimal.Parse(value, provider);
			}
#if !ECMA_COMPAT
	public static Decimal ToDecimal(char value)
			{
				return ((IConvertible)value).ToDecimal(null);
			}
	public static Decimal ToDecimal(Object value)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToDecimal(null);
				}
				else
				{
					return new Decimal(0);
				}
			}
	public static Decimal ToDecimal(Object value, IFormatProvider provider)
			{
				if(value != null)
				{
					return ((IConvertible)value).ToDecimal(provider);
				}
				else
				{
					return new Decimal(0);
				}
			}
	public static Decimal ToDecimal(DateTime value)
			{
				return ((IConvertible)value).ToDecimal(null);
			}
#else // ECMA_COMPAT
	internal static Decimal ToDecimal(Object value)
			{
				if(value == null)
				{
					return 0;
				}
				Type type = value.GetType();
				if(type == typeof(byte))
				{
					return ToDecimal((byte)value);
				}
				else if(type == typeof(sbyte))
				{
					return ToDecimal((sbyte)value);
				}
				else if(type == typeof(short))
				{
					return ToDecimal((short)value);
				}
				else if(type == typeof(ushort))
				{
					return ToDecimal((ushort)value);
				}
				else if(type == typeof(char))
				{
					return ToDecimal((char)value);
				}
				else if(type == typeof(int))
				{
					return ToDecimal((int)value);
				}
				else if(type == typeof(uint))
				{
					return ToDecimal((uint)value);
				}
				else if(type == typeof(long))
				{
					return ToDecimal((long)value);
				}
				else if(type == typeof(ulong))
				{
					return ToDecimal((ulong)value);
				}
				else if(type == typeof(float))
				{
					return ToDecimal((float)value);
				}
				else if(type == typeof(double))
				{
					return ToDecimal((double)value);
				}
				else if(type == typeof(Decimal))
				{
					return (Decimal)value;
				}
				else if(type == typeof(String))
				{
					return ToDecimal((String)value);
				}
				else if(type == typeof(bool))
				{
					return ToDecimal((bool)value);
				}
				else
				{
					throw new InvalidCastException
						(String.Format
							(_("InvalidCast_FromTo"),
 						     type.FullName, typeof(Decimal).FullName));
				}
			}
#endif // !ECMA_COMPAT

#endif // CONFIG_EXTENDED_NUMERICS

#if !ECMA_COMPAT

	// Change the type of an object.
	public static Object ChangeType(Object value, Type conversionType)
			{
				return ChangeType(value, conversionType,
								  CultureInfo.CurrentCulture);
			}
	public static Object ChangeType(Object value, Type conversionType,
									IFormatProvider provider)
			{
				IConvertible iconv = (value as IConvertible);
				if(iconv != null)
				{
					return DefaultToType(iconv, conversionType,
										 provider, false);
				}
				else if(value != null || conversionType != null)
				{
					if(value.GetType() == conversionType)
					{
						return value;
					}
					else
					{
						throw new InvalidCastException
							(_("InvalidCast_IConvertible"));
					}
				}
				else
				{
					return null;
				}
			}
	public static Object ChangeType(Object value, TypeCode typeCode)
			{
				return ChangeType(value, typeCode, CultureInfo.CurrentCulture);
			}
	public static Object ChangeType(Object value, TypeCode typeCode,
									IFormatProvider provider)
			{
				if(value is IConvertible)
				{
					IConvertible iconv = (IConvertible)value;
					switch(typeCode)
					{
						case TypeCode.Empty:
						{
							throw new InvalidCastException
								(_("InvalidCast_Empty"));
						}
						/* Not reached */

						case TypeCode.Object:
						{
							return value;
						}
						/* Not reached */

						case TypeCode.DBNull:
						{
							throw new InvalidCastException
								(_("InvalidCast_DBNull"));
						}
						/* Not reached */

						case TypeCode.Boolean:
						{
							return (Object)(iconv.ToBoolean(provider));
						}
						/* Not reached */

						case TypeCode.Char:
						{
							return (Object)(iconv.ToChar(provider));
						}
						/* Not reached */

						case TypeCode.SByte:
						{
							return (Object)(iconv.ToSByte(provider));
						}
						/* Not reached */

						case TypeCode.Byte:
						{
							return (Object)(iconv.ToByte(provider));
						}
						/* Not reached */

						case TypeCode.Int16:
						{
							return (Object)(iconv.ToInt16(provider));
						}
						/* Not reached */

						case TypeCode.UInt16:
						{
							return (Object)(iconv.ToUInt16(provider));
						}
						/* Not reached */

						case TypeCode.Int32:
						{
							return (Object)(iconv.ToInt32(provider));
						}
						/* Not reached */

						case TypeCode.UInt32:
						{
							return (Object)(iconv.ToUInt32(provider));
						}
						/* Not reached */

						case TypeCode.Int64:
						{
							return (Object)(iconv.ToInt64(provider));
						}
						/* Not reached */

						case TypeCode.UInt64:
						{
							return (Object)(iconv.ToUInt64(provider));
						}
						/* Not reached */

						case TypeCode.Single:
						{
							return (Object)(iconv.ToSingle(provider));
						}
						/* Not reached */

						case TypeCode.Double:
						{
							return (Object)(iconv.ToDouble(provider));
						}
						/* Not reached */

						case TypeCode.Decimal:
						{
							return (Object)(iconv.ToDecimal(provider));
						}
						/* Not reached */

						case TypeCode.DateTime:
						{
							return (Object)(iconv.ToDateTime(provider));
						}
						/* Not reached */

						case TypeCode.String:
						{
							return (Object)(iconv.ToString(provider));
						}
						/* Not reached */

						default:
						{
							throw new ArgumentException
								(_("Arg_UnknownTypeCode"));
						}
						/* Not reached */
					}
				}
				else if(value != null || typeCode != TypeCode.Empty)
				{
					throw new InvalidCastException
						(_("InvalidCast_IConvertible"));
				}
				else
				{
					return null;
				}
			}

	// Default implementation of the "ToType" methods in
	// the primitive classes like Byte, Int32, Boolean, etc.
	internal static Object DefaultToType(IConvertible obj, Type targetType,
										 IFormatProvider provider,
										 bool recursive)
			{
				if(targetType != null)
				{
					if(obj.GetType() == targetType)
					{
						return obj;
					}
					else if(targetType == ConvertTypes[(int)TypeCode.Boolean])
					{
						return (Object)(obj.ToBoolean(provider));
					}
					else if(targetType == ConvertTypes[(int)TypeCode.Char])
					{
						return (Object)(obj.ToChar(provider));
					}
					else if(targetType == ConvertTypes[(int)TypeCode.SByte])
					{
						return (Object)(obj.ToSByte(provider));
					}
					else if(targetType == ConvertTypes[(int)TypeCode.Byte])
					{
						return (Object)(obj.ToByte(provider));
					}
					else if(targetType == ConvertTypes[(int)TypeCode.Int16])
					{
						return (Object)(obj.ToInt16(provider));
					}
					else if(targetType == ConvertTypes[(int)TypeCode.UInt16])
					{
						return (Object)(obj.ToUInt16(provider));
					}
					else if(targetType == ConvertTypes[(int)TypeCode.Int32])
					{
						return (Object)(obj.ToInt32(provider));
					}
					else if(targetType == ConvertTypes[(int)TypeCode.UInt32])
					{
						return (Object)(obj.ToUInt32(provider));
					}
					else if(targetType == ConvertTypes[(int)TypeCode.Int64])
					{
						return (Object)(obj.ToInt64(provider));
					}
					else if(targetType == ConvertTypes[(int)TypeCode.UInt64])
					{
						return (Object)(obj.ToUInt64(provider));
					}
					else if(targetType == ConvertTypes[(int)TypeCode.Single])
					{
						return (Object)(obj.ToSingle(provider));
					}
					else if(targetType == ConvertTypes[(int)TypeCode.Double])
					{
						return (Object)(obj.ToDouble(provider));
					}
					else if(targetType == ConvertTypes[(int)TypeCode.Decimal])
					{
						return (Object)(obj.ToDecimal(provider));
					}
					else if(targetType == ConvertTypes[(int)TypeCode.DateTime])
					{
						return (Object)(obj.ToDateTime(provider));
					}
					else if(targetType == ConvertTypes[(int)TypeCode.String])
					{
						return (Object)(obj.ToString(provider));
					}
					else if(targetType == ConvertTypes[(int)TypeCode.Object])
					{
						return obj;
					}
					else if(targetType == ConvertTypes[(int)TypeCode.Empty])
					{
						throw new InvalidCastException
							(_("InvalidCast_Empty"));
					}
					else if(targetType == ConvertTypes[(int)TypeCode.DBNull])
					{
						throw new InvalidCastException
							(_("InvalidCast_DBNull"));
					}
					else if(recursive)
					{
						throw new InvalidCastException
							(String.Format
								(_("InvalidCast_FromTo"),
		 					     obj.GetType().FullName, targetType.FullName));
					}
					else
					{
						// We weren't called from a "ToType" method,
						// so we can use it to handle the default case.
						return obj.ToType(targetType, provider);
					}
				}
				else
				{
					throw new ArgumentNullException("targetType");
				}
			}

	// Miscellaneous methods.
	public static bool IsDBNull(Object value)
			{
				if(value is IConvertible)
				{
					return (((IConvertible)value).GetTypeCode() ==
									TypeCode.DBNull);
				}
				else
				{
					return false;
				}
			}
	public static TypeCode GetTypeCode(Object value)
			{
				if(value != null)
				{
					if(value is IConvertible)
					{
						return ((IConvertible)value).GetTypeCode();
					}
					else
					{
						return TypeCode.Object;
					}
				}
				else
				{
					return TypeCode.Empty;
				}
			}

	// Characters to use to encode 6-bit values in base64.
	internal const String base64Chars =
		"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

	// Map bytes in base64 to 6-bit values.
	internal static readonly sbyte[] base64Values = {
		-1, -1, -1, -1, -1, -1, -1, -1,   -1, -1, -1, -1, -1, -1, -1, -1, // 00
		-1, -1, -1, -1, -1, -1, -1, -1,   -1, -1, -1, -1, -1, -1, -1, -1, // 10
		-1, -1, -1, -1, -1, -1, -1, -1,   -1, -1, -1, 62, -1, -1, -1, 63, // 20
		52, 53, 54, 55, 56, 57, 58, 59,   60, 61, -1, -1, -1, -1, -1, -1, // 30

		-1, 0,  1,  2,  3,  4,  5,  6,     7,  8,  9, 10, 11, 12, 13, 14, // 40
		15, 16, 17, 18, 19, 20, 21, 22,   23, 24, 25, -1, -1, -1, -1, -1, // 50
		-1, 26, 27, 28, 29, 30, 31, 32,   33, 34, 35, 36, 37, 38, 39, 40, // 60
		41, 42, 43, 44, 45, 46, 47, 48,   49, 50, 51, -1, -1, -1, -1, -1, // 70

		-1, -1, -1, -1, -1, -1, -1, -1,   -1, -1, -1, -1, -1, -1, -1, -1, // 80
		-1, -1, -1, -1, -1, -1, -1, -1,   -1, -1, -1, -1, -1, -1, -1, -1, // 90
		-1, -1, -1, -1, -1, -1, -1, -1,   -1, -1, -1, -1, -1, -1, -1, -1, // A0
		-1, -1, -1, -1, -1, -1, -1, -1,   -1, -1, -1, -1, -1, -1, -1, -1, // B0

		-1, -1, -1, -1, -1, -1, -1, -1,   -1, -1, -1, -1, -1, -1, -1, -1, // C0
		-1, -1, -1, -1, -1, -1, -1, -1,   -1, -1, -1, -1, -1, -1, -1, -1, // D0
		-1, -1, -1, -1, -1, -1, -1, -1,   -1, -1, -1, -1, -1, -1, -1, -1, // E0
		-1, -1, -1, -1, -1, -1, -1, -1,   -1, -1, -1, -1, -1, -1, -1, -1, // F0
	};

	// Convert a set of base64 characters into an array of bytes.
	public static byte[] FromBase64CharArray(char[] inArray, int offset,
											 int length)
			{
				// Validate the parameters.
				if(inArray == null)
				{
					throw new ArgumentNullException("inArray");
				}
				else if(offset < 0 || offset > inArray.Length)
				{
					throw new ArgumentOutOfRangeException
						("offset", _("ArgRange_Array"));
				}
				else if(length < 0 || (inArray.Length - offset) < length)
				{
					throw new ArgumentException
						(_("Arg_InvalidArrayRange"));
				}

				// Determine the length of the result array in 6-bit values.
				int resultLen = 0;
				int index = offset;
				int count = length;
				char ch;
				int byteval;
				int numPadding;
				sbyte[] base64 = base64Values;
				while(count > 0)
				{
					ch = inArray[index++];
					--count;
					if(ch < '\u0100')
					{
						byteval = base64[(int)ch];
					}
					else
					{
						byteval = -1;
					}
					if(byteval != -1)
					{
						++resultLen;
					}
					else if(ch == '=')
					{
						// Process the padding characters.
						numPadding = 1;
						while(count > 0)
						{
							ch = inArray[index++];
							--count;
							if(ch == '=')
							{
								++numPadding;
							}
							else if(ch != ' ' && ch != '\t' &&
									ch != '\r' && ch != '\n')
							{
								throw new FormatException
									(_("Format_Base64ArrayChar"));
							}
						}
					}
					else if(ch != ' ' && ch != '\t' &&
					        ch != '\r' && ch != '\n')
					{
						// Invalid base64 character.
						throw new FormatException
							(_("Format_Base64ArrayChar"));
					}
				}

				// Convert the result length into bytes and allocate the array.
				resultLen = (int)((((long)resultLen) * 6L) / 8L);
				byte[] result = new byte [resultLen];

				// Convert the contents of the array.
				resultLen = 0;
				index = offset;
				count = length;
				int bits = 0;
				int numBits = 0;
				while(count > 0)
				{
					ch = inArray[index++];
					--count;
					if(ch < '\u0100')
					{
						byteval = base64[(int)ch];
					}
					else
					{
						byteval = -1;
					}
					if(byteval != -1)
					{
						bits = (bits << 6) + byteval;
						numBits += 6;
						if(numBits >= 8)
						{
							numBits -= 8;
							result[resultLen++] = (byte)(bits >> numBits);
							bits &= ((1 << numBits) - 1);
						}
					}
				}
				return result;
			}

	// Convert a base64-encoded string into an array of bytes.
	public static byte[] FromBase64String(String s)
			{
				// Validate the parameters.
				if(s == null)
				{
					throw new ArgumentNullException("s");
				}

				// Determine the length of the result array in 6-bit values.
				int resultLen = 0;
				int index = 0;
				int count = s.Length;
				char ch;
				int byteval;
				int numPadding;
				sbyte[] base64 = base64Values;
				while(count > 0)
				{
					ch = s[index++];
					--count;
					if(ch < '\u0100')
					{
						byteval = base64[(int)ch];
					}
					else
					{
						byteval = -1;
					}
					if(byteval != -1)
					{
						++resultLen;
					}
					else if(ch == '=')
					{
						// Process the padding characters.
						numPadding = 1;
						while(count > 0)
						{
							ch = s[index++];
							--count;
							if(ch == '=')
							{
								++numPadding;
							}
							else if(ch != ' ' && ch != '\t' &&
									ch != '\r' && ch != '\n')
							{
								throw new FormatException
									(_("Format_Base64ArrayChar"));
							}
						}
					}
					else if(ch != ' ' && ch != '\t' &&
					        ch != '\r' && ch != '\n')
					{
						// Invalid base64 character.
						throw new FormatException
							(_("Format_Base64ArrayChar"));
					}
				}

				// Convert the result length into bytes and allocate the array.
				resultLen = (int)((((long)resultLen) * 6L) / 8L);
				byte[] result = new byte [resultLen];

				// Convert the contents of the array.
				resultLen = 0;
				index = 0;
				count = s.Length;
				int bits = 0;
				int numBits = 0;
				while(count > 0)
				{
					ch = s[index++];
					--count;
					if(ch < '\u0100')
					{
						byteval = base64[(int)ch];
					}
					else
					{
						byteval = -1;
					}
					if(byteval != -1)
					{
						bits = (bits << 6) + byteval;
						numBits += 6;
						if(numBits >= 8)
						{
							numBits -= 8;
							result[resultLen++] = (byte)(bits >> numBits);
							bits &= ((1 << numBits) - 1);
						}
					}
				}
				return result;
			}

	// Convert an array of bytes into base64 characters.
	public static int ToBase64CharArray(byte[] inArray, int offsetIn,
										int length, char[] outArray,
										int offsetOut)
			{
				// Validate the parameters.
				if(inArray == null)
				{
					throw new ArgumentNullException("inArray");
				}
				if(outArray == null)
				{
					throw new ArgumentNullException("outArray");
				}
				if(offsetIn < 0 || offsetIn > inArray.Length)
				{
					throw new ArgumentOutOfRangeException
						("offsetIn", _("ArgRange_Array"));
				}
				if(length < 0 || length > (inArray.Length - offsetIn))
				{
					throw new ArgumentOutOfRangeException
						("length", _("ArgRange_Array"));
				}
				if(offsetOut < 0 || offsetOut > outArray.Length)
				{
					throw new ArgumentOutOfRangeException
						("outArray", _("ArgRange_Array"));
				}

				// Convert the bytes.
				int bits = 0;
				int numBits = 0;
				String base64 = base64Chars;
				int size = length;
				int posn = offsetOut;
				int outLen = outArray.Length;
				while(size > 0)
				{
					bits = (bits << 8) + inArray[offsetIn++];
					numBits += 8;
					--size;
					while(numBits >= 6)
					{
						numBits -= 6;
						if(posn >= outLen)
						{
							throw new ArgumentOutOfRangeException
								("offsetOut", _("Arg_InsufficientSpace"));
						}
						outArray[posn++] = base64[bits >> numBits];
						bits &= ((1 << numBits) - 1);
					}
				}
				length %= 3;
				if(length == 1)
				{
					if((posn + 3) > outLen)
					{
						throw new ArgumentOutOfRangeException
							("offsetOut", _("Arg_InsufficientSpace"));
					}
					outArray[posn++] = base64[bits << (6 - numBits)];
					outArray[posn++] = '=';
					outArray[posn++] = '=';
				}
				else if(length == 2)
				{
					if((posn + 2) > outLen)
					{
						throw new ArgumentOutOfRangeException
							("offsetOut", _("Arg_InsufficientSpace"));
					}
					outArray[posn++] = base64[bits << (6 - numBits)];
					outArray[posn++] = '=';
				}

				// Finished.
				return posn - offsetOut;
			}

	// Convert an array of bytes into a base64 string.
	public static String ToBase64String(byte[] inArray)
			{
				if(inArray == null)
				{
					throw new ArgumentNullException("inArray");
				}
				return ToBase64String(inArray, 0, inArray.Length);
			}
	public static String ToBase64String(byte[] inArray, int offset, int length)
			{
				// Validate the parameters.
				if(inArray == null)
				{
					throw new ArgumentNullException("inArray");
				}
				if(offset < 0 || offset > inArray.Length)
				{
					throw new ArgumentOutOfRangeException
						("offset", _("ArgRange_Array"));
				}
				if(length < 0 || length > (inArray.Length - offset))
				{
					throw new ArgumentOutOfRangeException
						("length", _("ArgRange_Array"));
				}

				// Convert the bytes.
				StringBuilder builder =
					new StringBuilder
						((int)(((((long)length) + 2L) * 4L) / 3L));
				int bits = 0;
				int numBits = 0;
				String base64 = base64Chars;
				int size = length;
				while(size > 0)
				{
					bits = (bits << 8) + inArray[offset++];
					numBits += 8;
					--size;
					while(numBits >= 6)
					{
						numBits -= 6;
						builder.Append(base64[bits >> numBits]);
						bits &= ((1 << numBits) - 1);
					}
				}
				length %= 3;
				if(length == 1)
				{
					builder.Append(base64[bits << (6 - numBits)]);
					builder.Append('=');
					builder.Append('=');
				}
				else if(length == 2)
				{
					builder.Append(base64[bits << (6 - numBits)]);
					builder.Append('=');
				}

				// Finished.
				return builder.ToString();
			}

#endif // !ECMA_COMPAT

	// Convert an object to a new type.
	internal static Object ConvertObject(Object obj, Type toType)
			{
				// Handle the "null" case.
				if(obj == null && !toType.IsValueType)
				{
					if(!toType.IsValueType)
					{
						return null;
					}
					else
					{
						throw new InvalidCastException
							(String.Format
								(_("InvalidCast_FromTo"),
 							     "null", toType.FullName));
					}
				}

				// If the types are directly assignable,
				// then return the original object as-is.
				Type objType = obj.GetType();
				if(toType.IsAssignableFrom(objType))
				{
					return obj;
				}

			#if !ECMA_COMPAT
				// Try to use "DefaultToType" to do the work.
				IConvertible iconv = (obj as IConvertible);
				if(iconv != null)
				{
					return DefaultToType
						(iconv, toType, CultureInfo.CurrentCulture, false);
				}
			#else // ECMA_COMPAT
				// Perform primitive type conversions the hard way
				// because we don't have support for IConvertible.
			#if CONFIG_EXTENDED_NUMERICS
				if(objType != typeof(String) && objType != typeof(Decimal))
			#else
				if(objType != typeof(String))
			#endif
				{
					try
					{
						if(toType == typeof(bool))
						{
							return ToBoolean(obj);
						}
						else if(toType == typeof(byte))
						{
							return ToByte(obj);
						}
						else if(toType == typeof(sbyte))
						{
							return ToSByte(obj);
						}
						else if(toType == typeof(short))
						{
							return ToInt16(obj);
						}
						else if(toType == typeof(ushort))
						{
							return ToUInt16(obj);
						}
						else if(toType == typeof(int))
						{
							return ToInt32(obj);
						}
						else if(toType == typeof(uint))
						{
							return ToUInt32(obj);
						}
						else if(toType == typeof(long))
						{
							return ToInt64(obj);
						}
						else if(toType == typeof(ulong))
						{
							return ToUInt64(obj);
						}
#if CONFIG_EXTENDED_NUMERICS
						else if(toType == typeof(float))
						{
							return ToSingle(obj);
						}
						else if(toType == typeof(double))
						{
							return ToDouble(obj);
						}
						else if(toType == typeof(Decimal))
						{
							return ToDecimal(obj);
						}
#endif
					}
					catch(OverflowException)
					{
						// Turn OverflowException's into InvalidCastException's.
					}
				}
			#endif // ECMA_COMPAT

				// The conversion is impossible.
				throw new InvalidCastException
					(String.Format
						(_("InvalidCast_FromTo"),
 					     objType.FullName, toType.FullName));
			}

	// Determine if there is a widening conversion between two
	// primitive types.
	internal static bool HasWideningConversion(Type from, Type to)
			{
				if(from == typeof(bool))
				{
					return (to == typeof(bool) ||
							to == typeof(byte) ||
							to == typeof(short) ||
					        to == typeof(ushort) ||
					        to == typeof(char) ||
							to == typeof(int) ||
							to == typeof(uint) ||
							to == typeof(long) ||
							to == typeof(ulong)
						#if CONFIG_EXTENDED_NUMERICS
							||
							to == typeof(float) ||
							to == typeof(double));
						#else
							);
						#endif
				}
				else if(from == typeof(byte))
				{
					return (to == typeof(byte) ||
							to == typeof(short) ||
					        to == typeof(ushort) ||
					        to == typeof(char) ||
							to == typeof(int) ||
							to == typeof(uint) ||
							to == typeof(long) ||
							to == typeof(ulong)
						#if CONFIG_EXTENDED_NUMERICS
							||
							to == typeof(float) ||
							to == typeof(double));
						#else
							);
						#endif
				}
				else if(from == typeof(sbyte))
				{
					return (to == typeof(sbyte) ||
							to == typeof(short) ||
							to == typeof(int) ||
							to == typeof(long)
						#if CONFIG_EXTENDED_NUMERICS
							||
							to == typeof(float) ||
							to == typeof(double));
						#else
							);
						#endif
				}
				else if(from == typeof(short))
				{
					return (to == typeof(short) ||
							to == typeof(int) ||
							to == typeof(long)
						#if CONFIG_EXTENDED_NUMERICS
							||
							to == typeof(float) ||
							to == typeof(double));
						#else
							);
						#endif
				}
				else if(from == typeof(ushort) || from == typeof(char))
				{
					return (to == typeof(ushort) ||
					        to == typeof(char) ||
							to == typeof(int) ||
							to == typeof(uint) ||
							to == typeof(long) ||
							to == typeof(ulong)
						#if CONFIG_EXTENDED_NUMERICS
							||
							to == typeof(float) ||
							to == typeof(double));
						#else
							);
						#endif
				}
				else if(from == typeof(int))
				{
					return (to == typeof(int) ||
							to == typeof(long)
						#if CONFIG_EXTENDED_NUMERICS
							||
							to == typeof(float) ||
							to == typeof(double));
						#else
							);
						#endif
				}
				else if(from == typeof(uint))
				{
					return (to == typeof(uint) ||
							to == typeof(ulong)
						#if CONFIG_EXTENDED_NUMERICS
							||
							to == typeof(float) ||
							to == typeof(double));
						#else
							);
						#endif
				}
				else if(from == typeof(long))
				{
					return (to == typeof(long)
						#if CONFIG_EXTENDED_NUMERICS
							||
							to == typeof(float) ||
							to == typeof(double));
						#else
							);
						#endif
				}
				else if(from == typeof(ulong))
				{
					return (to == typeof(ulong)
						#if CONFIG_EXTENDED_NUMERICS
							||
							to == typeof(float) ||
							to == typeof(double));
						#else
							);
						#endif
				}
			#if CONFIG_EXTENDED_NUMERICS
				else if(from == typeof(float))
				{
					return (to == typeof(float) ||
							to == typeof(double));
				}
				else if(from == typeof(double))
				{
					return (to == typeof(double));
				}
			#endif
				return false;
			}

}; // class Convert

}; // namespace System
