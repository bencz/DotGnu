/*
 * DBNull.cs - Implementation of the "System.DBNull" class.
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

using System.Runtime.Serialization;

public sealed class DBNull : IConvertible
#if CONFIG_SERIALIZATION
	, ISerializable
#endif
{

	// The only DBNull object in the system.
	public readonly static DBNull Value = new DBNull();

	// Constructors.
	private DBNull() {}

	// Override inherited methods.
	public override String ToString() { return String.Empty; }

	// Implement the IConvertible interface.
	public TypeCode GetTypeCode() { return TypeCode.DBNull; }
	Object IConvertible.ToType(Type ct, IFormatProvider provider)
			{
				return Convert.DefaultToType(this, ct, provider, true);
			}
	Boolean IConvertible.ToBoolean(IFormatProvider provider)
			{
				throw new InvalidCastException(_("InvalidCast_DBNull"));
			}
	Byte IConvertible.ToByte(IFormatProvider provider)
			{
				throw new InvalidCastException(_("InvalidCast_DBNull"));
			}
	SByte IConvertible.ToSByte(IFormatProvider provider)
			{
				throw new InvalidCastException(_("InvalidCast_DBNull"));
			}
	Int16 IConvertible.ToInt16(IFormatProvider provider)
			{
				throw new InvalidCastException(_("InvalidCast_DBNull"));
			}
	UInt16 IConvertible.ToUInt16(IFormatProvider provider)
			{
				throw new InvalidCastException(_("InvalidCast_DBNull"));
			}
	Int32 IConvertible.ToInt32(IFormatProvider provider) 
			{
				throw new InvalidCastException(_("InvalidCast_DBNull"));
			}
	UInt32 IConvertible.ToUInt32(IFormatProvider provider)
			{
				throw new InvalidCastException(_("InvalidCast_DBNull"));
			}
	Int64 IConvertible.ToInt64(IFormatProvider provider)
			{
				throw new InvalidCastException(_("InvalidCast_DBNull"));
			}
	UInt64 IConvertible.ToUInt64(IFormatProvider provider)
			{
				throw new InvalidCastException(_("InvalidCast_DBNull"));
			}
	Char IConvertible.ToChar(IFormatProvider provider)
			{
				throw new InvalidCastException(_("InvalidCast_DBNull"));
			}
	Single IConvertible.ToSingle(IFormatProvider provider)
			{
				throw new InvalidCastException(_("InvalidCast_DBNull"));
			}
	Double IConvertible.ToDouble(IFormatProvider provider)
			{
				throw new InvalidCastException(_("InvalidCast_DBNull"));
			}
	Decimal IConvertible.ToDecimal(IFormatProvider provider)
			{
				throw new InvalidCastException(_("InvalidCast_DBNull"));
			}
	DateTime IConvertible.ToDateTime(IFormatProvider provider)
			{
				throw new InvalidCastException(_("InvalidCast_DBNull"));
			}
	public String ToString(IFormatProvider provider)
			{
				return String.Empty;
			}

#if CONFIG_SERIALIZATION

	// Get the serialization data for this object.
	public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				UnitySerializationHolder.Serialize
					(info, UnitySerializationHolder.UnityType.DBNull,
					 null, null);
			}

#endif

}; // class DBNull

#endif // !ECMA_COMPAT

}; // namespace System
