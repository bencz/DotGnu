/*
 * MethodAttributes.cs - Implementation of the
 *			"System.Reflection.MethodAttributes" class.
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
public enum MethodAttributes
{
	MemberAccessMask		= 0x0007,
	PrivateScope			= 0x0000,
	Private					= 0x0001,
	FamANDAssem				= 0x0002,
	Assembly				= 0x0003,
	Family					= 0x0004,
	FamORAssem				= 0x0005,
	Public					= 0x0006,
	Static					= 0x0010,
	Final					= 0x0020,
	Virtual					= 0x0040,
	HideBySig				= 0x0080,
	VtableLayoutMask		= 0x0100,
	ReuseSlot				= 0x0000,
	NewSlot					= 0x0100,
	CheckAccessOnOverride   = 0x0200,
	Abstract				= 0x0400,
	SpecialName				= 0x0800,
	PinvokeImpl				= 0x2000,
	UnmanagedExport			= 0x0008,
	ReservedMask			= 0xD000,
	RTSpecialName			= 0x1000,
	HasSecurity				= 0x4000,
	RequireSecObject		= 0x8000

}; // enum MethodAttributes

#endif // CONFIG_REFLECTION

}; // namespace System.Reflection
