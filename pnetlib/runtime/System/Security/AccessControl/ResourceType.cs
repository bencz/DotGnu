/*
 * ResourceType.cs - Implementation of the
 *			"System.Security.AccessControl.ResourceType" class.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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

namespace System.Security.AccessControl
{

#if CONFIG_ACCESS_CONTROL

using System.Runtime.InteropServices;

#if !ECMA_COMPAT
[ComVisible(false)]
#endif
public enum ResourceType
{
	Unknown				= 0,
	FileObject			= 1,
	Service				= 2,
	Printer				= 3,
	RegistryKey			= 4,
	LMShare				= 5,
	KernelObject		= 6,
	WindowObject		= 7,
	DSObject			= 8,
	DSObjectAll			= 9,
	ProviderDefined		= 10,
	WmiGuidObject		= 11,
	RegistryWow6432Key	= 12

}; // enum ResourceType

#endif // CONFIG_ACCESS_CONTROL

}; // namespace System.Security.AccessControl
