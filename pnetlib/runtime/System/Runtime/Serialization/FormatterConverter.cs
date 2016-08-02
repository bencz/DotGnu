/*
 * FormatterConverter.cs - Implementation of the
 *			"System.Runtime.Serialization.FormatterConverter" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System.Runtime.Serialization
{

#if CONFIG_SERIALIZATION

public class FormatterConverter : IFormatterConverter
{
	// Constructor.
	public FormatterConverter() {}

	// Convert a value to a specific type.
	public Object Convert(Object value, Type type)
			{
				return System.Convert.ChangeType(value, type);
			}
	public Object Convert(Object value, TypeCode typeCode)
			{
				return System.Convert.ChangeType(value, typeCode);
			}

	// Convert a value to specialized types.
	public bool ToBoolean(Object value)
			{
				return System.Convert.ToBoolean(value);
			}
	public byte ToByte(Object value)
			{
				return System.Convert.ToByte(value);
			}
	[CLSCompliant(false)]
	public sbyte ToSByte(Object value)
			{
				return System.Convert.ToSByte(value);
			}
	public short ToInt16(Object value)
			{
				return System.Convert.ToInt16(value);
			}
	[CLSCompliant(false)]
	public ushort ToUInt16(Object value)
			{
				return System.Convert.ToUInt16(value);
			}
	public char ToChar(Object value)
			{
				return System.Convert.ToChar(value);
			}
	public int ToInt32(Object value)
			{
				return System.Convert.ToInt32(value);
			}
	[CLSCompliant(false)]
	public uint ToUInt32(Object value)
			{
				return System.Convert.ToUInt32(value);
			}
	public long ToInt64(Object value)
			{
				return System.Convert.ToInt64(value);
			}
	[CLSCompliant(false)]
	public ulong ToUInt64(Object value)
			{
				return System.Convert.ToUInt16(value);
			}
	public float ToSingle(Object value)
			{
				return System.Convert.ToSingle(value);
			}
	public double ToDouble(Object value)
			{
				return System.Convert.ToDouble(value);
			}
	public DateTime ToDateTime(Object value)
			{
				return System.Convert.ToDateTime(value);
			}
	public Decimal ToDecimal(Object value)
			{
				return System.Convert.ToDecimal(value);
			}
	public String ToString(Object value)
			{
				return System.Convert.ToString(value);
			}

}; // class FormatterConverter

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Serialization
