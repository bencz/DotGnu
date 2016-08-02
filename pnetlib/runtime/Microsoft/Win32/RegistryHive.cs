/*
 * RegistryHive.cs - Implementation of the
 *			"Microsoft.Win32.RegistryHive" class.
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

namespace Microsoft.Win32
{

#if CONFIG_WIN32_SPECIFICS

using System;
using System.Runtime.InteropServices;

[Serializable]
[ComVisible(true)]
public enum RegistryHive
{

	ClassesRoot			= unchecked((int)0x80000000),
	CurrentUser			= unchecked((int)0x80000001),
	LocalMachine		= unchecked((int)0x80000002),
	Users				= unchecked((int)0x80000003),
	PerformanceData		= unchecked((int)0x80000004),
	CurrentConfig		= unchecked((int)0x80000005),
	DynData				= unchecked((int)0x80000006)

}; // enum RegistryHive

#endif // CONFIG_WIN32_SPECIFICS

}; // namespace Microsoft.Win32
