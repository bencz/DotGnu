/*
 * IFormatterConverter.cs - Implementation of the
 *			"System.Runtime.Serialization.IFormatterConverter" interface.
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

[CLSCompliant(false)]
public interface IFormatterConverter
{

	// Convert a value to a specific type.
	Object Convert(Object value, Type type);
	Object Convert(Object value, TypeCode typeCode);

	// Convert a value to specialized types.
	bool     ToBoolean(Object value);
	byte     ToByte(Object value);
	sbyte    ToSByte(Object value);
	short    ToInt16(Object value);
	ushort   ToUInt16(Object value);
	char     ToChar(Object value);
	int      ToInt32(Object value);
	uint     ToUInt32(Object value);
	long     ToInt64(Object value);
	ulong    ToUInt64(Object value);
	float    ToSingle(Object value);
	double   ToDouble(Object value);
	DateTime ToDateTime(Object value);
	Decimal  ToDecimal(Object value);
	String   ToString(Object value);

}; // interface IFormatterConverter

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Serialization
