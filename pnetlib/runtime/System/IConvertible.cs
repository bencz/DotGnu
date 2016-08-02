/*
 * IConvertible.cs - Implementation of the "System.IConvertible" interface.
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

#if !ECMA_COMPAT

[CLSCompliant(false)]
public interface IConvertible
{

	TypeCode GetTypeCode();
	bool ToBoolean(IFormatProvider provider);
	byte ToByte(IFormatProvider provider);
	sbyte ToSByte(IFormatProvider provider);
	short ToInt16(IFormatProvider provider);
	ushort ToUInt16(IFormatProvider provider);
	char ToChar(IFormatProvider provider);
	int ToInt32(IFormatProvider provider);
	uint ToUInt32(IFormatProvider provider);
	long ToInt64(IFormatProvider provider);
	ulong ToUInt64(IFormatProvider provider);
	float ToSingle(IFormatProvider provider);
	double ToDouble(IFormatProvider provider);
	Decimal ToDecimal(IFormatProvider provider);
	DateTime ToDateTime(IFormatProvider provider);
	String ToString(IFormatProvider provider);
	Object ToType(Type conversionType, IFormatProvider provider);

}; // interface IConvertible

#endif // !ECMA_COMPAT

}; // namespace System
