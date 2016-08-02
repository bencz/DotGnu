/*
 * VariantType.cs - Implementation of the
 *			"Microsoft.VisualBasic.VariantType" class.
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

public enum VariantType
{
	Empty			= 0x0000,
	Null			= 0x0001,
	Short			= 0x0002,
	Integer			= 0x0003,
	Single			= 0x0004,
	Double			= 0x0005,
	Currency		= 0x0006,
	Date			= 0x0007,
	String			= 0x0008,
	Object			= 0x0009,
	Error			= 0x000A,
	Boolean			= 0x000B,
	Variant			= 0x000C,
	DataObject		= 0x000D,
	Decimal			= 0x000E,
	Byte			= 0x0011,
	Char			= 0x0012,
	Long			= 0x0014,
	UserDefinedType	= 0x0024,
	Array			= 0x2000

}; // enum VariantType

}; // namespace Microsoft.VisualBasic
