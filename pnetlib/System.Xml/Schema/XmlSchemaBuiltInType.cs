/*
 * XmlSchemaBuiltInType.cs - Implementation of the
 *		"System.Xml.XmlSchemaBuiltInType" class.
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
 
namespace System.Xml.Schema
{

#if CONFIG_FRAMEWORK_1_2

public enum XmlSchemaBuiltInType
{
	None,
	AnyType,
	AnySimpleType,
	String,
	Bool,
	Float,
	Double,
	Decimal,
	Duration,
	AnyUri,
	Base64Binary,
	Byte,
	Date,
	DateTime,
	GDay,
	GMonth,
	GMonthDay,
	GYear,
	GYearMonth,
	HexBinary,
	Entities,
	Entity,
	Id,
	Idref,
	Idrefs,
	Int,
	Integer,
	Language,
	Long,
	Name,
	NCName,
	NegativeInteger,
	NmToken,
	NmTokens,
	NonNegativeInteger,
	NonPositiveInteger,
	Normalizedstring,
	Notation,
	PositiveInteger,
	QName,
	Short,
	Time,
	Token,
	UnsignedByte,
	UnsignedInt,
	UnsignedLong,
	UnsignedShort

}; // XmlSchemaBuiltInType

#endif // CONFIG_FRAMEWORK_1_2

}; // namespace System.Xml.Schema
