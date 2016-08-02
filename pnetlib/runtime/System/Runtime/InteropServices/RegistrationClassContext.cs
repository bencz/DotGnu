/*
 * RegistrationClassContext.cs - Implementation of the
 *		"System.Runtime.InteropServices.RegistrationClassContext" class 
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

namespace System.Runtime.InteropServices
{

#if CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2

[ComVisible(false)]
public enum RegistrationClassContext
{
	InprocServer					= 0x00000001,
	InprocHandler					= 0x00000002,
	LocalServer						= 0x00000004,
	InprocServer16					= 0x00000008,
	RemoteServer					= 0x00000010,
	InprocHandler16					= 0x00000020,
	Reserved1						= 0x00000040,
	Reserved2						= 0x00000080,
	Reserved3						= 0x00000100,
	Reserved4						= 0x00000200,
	NoCodeDownload					= 0x00000400,
	Reserved5						= 0x00000800,
	NoCustomMarshal					= 0x00001000,
	EnableCodeDownload				= 0x00002000,
	NoFailureLog					= 0x00004000,
	DisableActivateAsActivator		= 0x00008000,
	EnableActivateAsActivator		= 0x00010000,
	FromDefaultContext				= 0x00020000

}; // enum RegistrationClassContext

#endif // CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2

}; // namespace System.Runtime.InteropServices
