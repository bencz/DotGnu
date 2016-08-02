/*
 * FieldAttributes.cs - Implementation of the
 *			"System.Reflection.FieldAttributes" class.
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

namespace System.Reflection
{

#if CONFIG_REFLECTION

[Flags]
public enum FieldAttributes
{
	FieldAccessMask			= 0x0007,
	PrivateScope			= 0x0000,
	Private					= 0x0001,
	FamANDAssem				= 0x0002,
	Assembly				= 0x0003,
	Family					= 0x0004,
	FamORAssem				= 0x0005,
	Public					= 0x0006,
	Static					= 0x0010,
	InitOnly				= 0x0020,
	Literal					= 0x0040,
	NotSerialized			= 0x0080,
	SpecialName				= 0x0200,
	PinvokeImpl				= 0x2000,
	ReservedMask			= 0x9500,
	RTSpecialName			= 0x0400,
	HasFieldMarshal			= 0x1000,
	HasDefault				= 0x8000,
	HasFieldRVA				= 0x0100

}; // enum FieldAttributes

#endif // CONFIG_REFLECTION

}; // namespace System.Reflection
