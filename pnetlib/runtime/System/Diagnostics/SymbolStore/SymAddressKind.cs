/*
 * SymAddressKind.cs - Implementation of 
 *			"System.Diagnostics.SymbolStore.SymAddressKind" enum.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * 
 * Contributed by Gopal.V
 *
 * This program is free software, you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY, without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program, if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

#if CONFIG_EXTENDED_DIAGNOSTICS

using System;

namespace System.Diagnostics.SymbolStore
{
	public enum SymAddressKind
	{
		ILOffset 					= 0x01,
		NativeRVA					= 0x02,
		NativeRegister 				= 0x03,
		NativeRegisterRelative		= 0x04,
		NativeOffset				= 0x05,
		NativeRegisterRegister		= 0x06,
		NativeRegisterStack			= 0x07,
		NativeStackRegister			= 0x08,
		BitField					= 0x09
	}
}//namespace

#endif // CONFIG_EXTENDED_DIAGNOSTICS
