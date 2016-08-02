/*
 * UnmanagedType.cs - Implementation of the
 *			"System.Runtime.InteropServices.UnmanagedType" class.
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

namespace System.Runtime.InteropServices
{

#if CONFIG_RUNTIME_INFRA

public enum UnmanagedType
{
	Bool				= 0x02,
	I1					= 0x03,
	U1					= 0x04,
	I2					= 0x05,
	U2					= 0x06,
	I4					= 0x07,
	U4					= 0x08,
	I8					= 0x09,
	U8					= 0x0A,
	R4					= 0x0B,
	R8					= 0x0C,
	Currency			= 0x0F,
	BStr				= 0x13,
	LPStr				= 0x14,
	LPWStr				= 0x15,
	LPTStr				= 0x16,
	ByValTStr			= 0x17,
	IUnknown			= 0x19,
	IDispatch			= 0x1A,
	Struct				= 0x1B,
	Interface			= 0x1C,
	SafeArray			= 0x1D,
	ByValArray			= 0x1E,
	SysInt				= 0x1F,
	SysUInt				= 0x20,
	VBByRefStr			= 0x22,
	AnsiBStr			= 0x23,
	TBStr				= 0x24,
	VariantBool			= 0x25,
	FunctionPtr			= 0x26,
	AsAny				= 0x28,
	LPArray				= 0x2A,
	LPStruct			= 0x2B,
	CustomMarshaler		= 0x2C,
	Error				= 0x2D
	
}; // enum UnmanagedType

#endif // CONFIG_RUNTIME_INFRA

}; // namespace System.Runtime.InteropServices
