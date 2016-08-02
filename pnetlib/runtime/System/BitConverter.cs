/*
 * BitConverter.cs - Implementation of the "System.BitConverter" class.
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

using System.Text;
using System.Runtime.CompilerServices;

#if ECMA_COMPAT
internal
#else
public
#endif
sealed class BitConverter
{
	// Cannot instantiate this class.
	private BitConverter() {}

	// Specification of the endian-ness of this platform.
	public static readonly bool IsLittleEndian = GetLittleEndian();

	// Get the endian-ness of the underlying platform.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static bool GetLittleEndian();

	// Convert a double value into a 64-bit integer.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static long DoubleToInt64Bits(double value);

	// Convert a 64-bit integer into a double value.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static double Int64BitsToDouble(long value);

	// Convert a float value into a 32-bit integer.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static int FloatToInt32Bits(float value);

	// Convert a 32-bit integer into a float value.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static float Int32BitsToFloat(int value);

	// Convert a float value into an array of bytes.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static byte[] GetLittleEndianBytes(float value);

	// Convert a double value into an array of bytes.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static byte[] GetLittleEndianBytes(double value);

#if !ECMA_COMPAT

	// Convert a boolean value into an array of bytes.
	public static byte[] GetBytes(bool value)
			{
				byte[] bytes = new byte [1];
				bytes[0] = (byte)(value ? 1 : 0);
				return bytes;
			}

	// Convert a character value into an array of bytes.
	public static byte[] GetBytes(char value)
			{
				byte[] bytes = new byte [2];
				if(IsLittleEndian)
				{
					bytes[0] = (byte)value;
					bytes[1] = (byte)(value >> 8);
				}
				else
				{
					bytes[0] = (byte)(value >> 8);
					bytes[1] = (byte)value;
				}
				return bytes;
			}

	// Convert a short value into an array of bytes.
	public static byte[] GetBytes(short value)
			{
				byte[] bytes = new byte [2];
				if(IsLittleEndian)
				{
					bytes[0] = (byte)value;
					bytes[1] = (byte)(value >> 8);
				}
				else
				{
					bytes[0] = (byte)(value >> 8);
					bytes[1] = (byte)value;
				}
				return bytes;
			}

	// Convert an unsigned short value into an array of bytes.
	[CLSCompliant(false)]
	public static byte[] GetBytes(ushort value)
			{
				byte[] bytes = new byte [2];
				if(IsLittleEndian)
				{
					bytes[0] = (byte)value;
					bytes[1] = (byte)(value >> 8);
				}
				else
				{
					bytes[0] = (byte)(value >> 8);
					bytes[1] = (byte)value;
				}
				return bytes;
			}

	// Convert an integer value into an array of bytes.
	public static byte[] GetBytes(int value)
			{
				byte[] bytes = new byte [4];
				if(IsLittleEndian)
				{
					bytes[0] = (byte)value;
					bytes[1] = (byte)(value >> 8);
					bytes[2] = (byte)(value >> 16);
					bytes[3] = (byte)(value >> 24);
				}
				else
				{
					bytes[0] = (byte)(value >> 24);
					bytes[1] = (byte)(value >> 16);
					bytes[2] = (byte)(value >> 8);
					bytes[3] = (byte)value;
				}
				return bytes;
			}

	// Convert an unsigned integer value into an array of bytes.
	[CLSCompliant(false)]
	public static byte[] GetBytes(uint value)
			{
				byte[] bytes = new byte [4];
				if(IsLittleEndian)
				{
					bytes[0] = (byte)value;
					bytes[1] = (byte)(value >> 8);
					bytes[2] = (byte)(value >> 16);
					bytes[3] = (byte)(value >> 24);
				}
				else
				{
					bytes[0] = (byte)(value >> 24);
					bytes[1] = (byte)(value >> 16);
					bytes[2] = (byte)(value >> 8);
					bytes[3] = (byte)value;
				}
				return bytes;
			}

	// Convert a long value into an array of bytes.
	public static byte[] GetBytes(long value)
			{
				byte[] bytes = new byte [8];
				if(IsLittleEndian)
				{
					bytes[0] = (byte)value;
					bytes[1] = (byte)(value >> 8);
					bytes[2] = (byte)(value >> 16);
					bytes[3] = (byte)(value >> 24);
					bytes[4] = (byte)(value >> 32);
					bytes[5] = (byte)(value >> 40);
					bytes[6] = (byte)(value >> 48);
					bytes[7] = (byte)(value >> 56);
				}
				else
				{
					bytes[0] = (byte)(value >> 56);
					bytes[1] = (byte)(value >> 48);
					bytes[2] = (byte)(value >> 40);
					bytes[3] = (byte)(value >> 32);
					bytes[4] = (byte)(value >> 24);
					bytes[5] = (byte)(value >> 16);
					bytes[6] = (byte)(value >> 8);
					bytes[7] = (byte)value;
				}
				return bytes;
			}

	// Convert an unsigned long value into an array of bytes.
	[CLSCompliant(false)]
	public static byte[] GetBytes(ulong value)
			{
				byte[] bytes = new byte [8];
				if(IsLittleEndian)
				{
					bytes[0] = (byte)value;
					bytes[1] = (byte)(value >> 8);
					bytes[2] = (byte)(value >> 16);
					bytes[3] = (byte)(value >> 24);
					bytes[4] = (byte)(value >> 32);
					bytes[5] = (byte)(value >> 40);
					bytes[6] = (byte)(value >> 48);
					bytes[7] = (byte)(value >> 56);
				}
				else
				{
					bytes[0] = (byte)(value >> 56);
					bytes[1] = (byte)(value >> 48);
					bytes[2] = (byte)(value >> 40);
					bytes[3] = (byte)(value >> 32);
					bytes[4] = (byte)(value >> 24);
					bytes[5] = (byte)(value >> 16);
					bytes[6] = (byte)(value >> 8);
					bytes[7] = (byte)value;
				}
				return bytes;
			}

	// Convert a float value into an array of bytes.
	public static byte[] GetBytes(float value)
			{
				return GetBytes(FloatToInt32Bits(value));
			}

	// Convert a double value into an array of bytes.
	public static byte[] GetBytes(double value)
			{
				return GetBytes(DoubleToInt64Bits(value));
			}

	// Convert a byte within an array into a boolean value.
	public static bool ToBoolean(byte[] value, int startIndex)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				if(startIndex < 0 || startIndex >= value.Length)
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("Arg_InvalidArrayIndex"));
				}
				return (value[startIndex] != 0);
			}

	// Convert bytes within an array into a char value.
	public static char ToChar(byte[] value, int startIndex)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				if(startIndex < 0 || startIndex >= (value.Length - 1))
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("Arg_InvalidArrayIndex"));
				}
				if(IsLittleEndian)
				{
					return (char)(((int)(value[startIndex])) |
								  (((int)(value[startIndex + 1])) << 8));
				}
				else
				{
					return (char)(((int)(value[startIndex + 1])) |
								  (((int)(value[startIndex])) << 8));
				}
			}

	// Convert bytes within an array into a short value.
	public static short ToInt16(byte[] value, int startIndex)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				if(startIndex < 0 || startIndex >= (value.Length - 1))
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("Arg_InvalidArrayIndex"));
				}
				if(IsLittleEndian)
				{
					return (short)(((int)(value[startIndex])) |
								   (((int)(value[startIndex + 1])) << 8));
				}
				else
				{
					return (short)(((int)(value[startIndex + 1])) |
								  (((int)(value[startIndex])) << 8));
				}
			}

	// Convert bytes within an array into an unsigned short value.
	[CLSCompliant(false)]
	public static ushort ToUInt16(byte[] value, int startIndex)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				if(startIndex < 0 || startIndex >= (value.Length - 1))
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("Arg_InvalidArrayIndex"));
				}
				if(IsLittleEndian)
				{
					return (ushort)(((int)(value[startIndex])) |
								    (((int)(value[startIndex + 1])) << 8));
				}
				else
				{
					return (ushort)(((int)(value[startIndex + 1])) |
								   (((int)(value[startIndex])) << 8));
				}
			}

	// Convert bytes within an array into an integer value.
	public static int ToInt32(byte[] value, int startIndex)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				if(startIndex < 0 || startIndex >= (value.Length - 3))
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("Arg_InvalidArrayIndex"));
				}
				if(IsLittleEndian)
				{
					return (((int)(value[startIndex])) |
						    (((int)(value[startIndex + 1])) << 8) |
						    (((int)(value[startIndex + 2])) << 16) |
						    (((int)(value[startIndex + 3])) << 24));
				}
				else
				{
					return (((int)(value[startIndex + 3])) |
						    (((int)(value[startIndex + 2])) << 8) |
						    (((int)(value[startIndex + 1])) << 16) |
						    (((int)(value[startIndex])) << 24));
				}
			}

	// Convert bytes within an array into an unsigned integer value.
	[CLSCompliant(false)]
	public static uint ToUInt32(byte[] value, int startIndex)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				if(startIndex < 0 || startIndex >= (value.Length - 3))
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("Arg_InvalidArrayIndex"));
				}
				if(IsLittleEndian)
				{
					return (uint)(((int)(value[startIndex])) |
						    	  (((int)(value[startIndex + 1])) << 8) |
						    	  (((int)(value[startIndex + 2])) << 16) |
						    	  (((int)(value[startIndex + 3])) << 24));
				}
				else
				{
					return (uint)(((int)(value[startIndex + 3])) |
						    	  (((int)(value[startIndex + 2])) << 8) |
						    	  (((int)(value[startIndex + 1])) << 16) |
						    	  (((int)(value[startIndex])) << 24));
				}
			}

	// Convert bytes within an array into a long value.
	public static long ToInt64(byte[] value, int startIndex)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				if(startIndex < 0 || startIndex >= (value.Length - 7))
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("Arg_InvalidArrayIndex"));
				}
				if(IsLittleEndian)
				{
					return (((long)(value[startIndex])) |
						    (((long)(value[startIndex + 1])) << 8) |
						    (((long)(value[startIndex + 2])) << 16) |
						    (((long)(value[startIndex + 3])) << 24) |
						    (((long)(value[startIndex + 4])) << 32) |
						    (((long)(value[startIndex + 5])) << 40) |
						    (((long)(value[startIndex + 6])) << 48) |
						    (((long)(value[startIndex + 7])) << 56));
				}
				else
				{
					return (((long)(value[startIndex + 7])) |
						    (((long)(value[startIndex + 6])) << 8) |
						    (((long)(value[startIndex + 5])) << 16) |
						    (((long)(value[startIndex + 4])) << 24) |
						    (((long)(value[startIndex + 3])) << 32) |
						    (((long)(value[startIndex + 2])) << 40) |
						    (((long)(value[startIndex + 1])) << 48) |
						    (((long)(value[startIndex])) << 56));
				}
			}

	// Convert bytes within an array into an unsigned long value.
	[CLSCompliant(false)]
	public static ulong ToUInt64(byte[] value, int startIndex)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				if(startIndex < 0 || startIndex >= (value.Length - 7))
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("Arg_InvalidArrayIndex"));
				}
				if(IsLittleEndian)
				{
					return (((ulong)(value[startIndex])) |
						    (((ulong)(value[startIndex + 1])) << 8) |
						    (((ulong)(value[startIndex + 2])) << 16) |
						    (((ulong)(value[startIndex + 3])) << 24) |
						    (((ulong)(value[startIndex + 4])) << 32) |
						    (((ulong)(value[startIndex + 5])) << 40) |
						    (((ulong)(value[startIndex + 6])) << 48) |
				    	    (((ulong)(value[startIndex + 7])) << 56));
				}
				else
				{
					return (((ulong)(value[startIndex + 7])) |
						    (((ulong)(value[startIndex + 6])) << 8) |
						    (((ulong)(value[startIndex + 5])) << 16) |
						    (((ulong)(value[startIndex + 4])) << 24) |
						    (((ulong)(value[startIndex + 3])) << 32) |
						    (((ulong)(value[startIndex + 2])) << 40) |
						    (((ulong)(value[startIndex + 1])) << 48) |
						    (((ulong)(value[startIndex])) << 56));
				}
			}

	// Convert bytes within an array into a float value.
	public static float ToSingle(byte[] value, int startIndex)
			{
				return Int32BitsToFloat(ToInt32(value, startIndex));
			}

	// Convert bytes within an array into a double value.
	public static double ToDouble(byte[] value, int startIndex)
			{
				return Int64BitsToDouble(ToInt64(value, startIndex));
			}

	// Convert bytes within an array into a string of hex values.
	public static String ToString(byte[] value)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				return ToString(value, 0, value.Length);
			}
	public static String ToString(byte[] value, int startIndex)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				return ToString(value, startIndex, value.Length - startIndex);
			}
	public static String ToString(byte[] value, int startIndex, int length)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				else if(startIndex < 0 || startIndex >= value.Length)
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("ArgRange_Array"));
				}
				else if(length < 0)
				{
					throw new ArgumentOutOfRangeException
						("length", _("ArgRange_Array"));
				}
				else if((value.Length - startIndex) < length)
				{
					throw new ArgumentException
						("length", _("ArgRange_Array"));
				}
				if(length == 0)
				{
					return String.Empty;
				}
				StringBuilder builder = new StringBuilder(length * 3 - 1);
				AppendHex(builder, value[startIndex++]);
				--length;
				while(length > 0)
				{
					builder.Append('-');
					AppendHex(builder, value[startIndex++]);
					--length;
				}
				return builder.ToString();
			}

	// Append the hex version of a byte to a string builder.
	internal static void AppendHex(StringBuilder builder, int value)
			{
				int digit = ((value / 16) & 0x0F);
				if(digit < 10)
					builder.Append((char)('0' + digit));
				else
					builder.Append((char)('A' + digit - 10));
				digit = (value & 0x0F);
				if(digit < 10)
					builder.Append((char)('0' + digit));
				else
					builder.Append((char)('A' + digit - 10));
			}

#endif // !ECMA_COMPAT

}; // class BitConverter

}; // namespace System
