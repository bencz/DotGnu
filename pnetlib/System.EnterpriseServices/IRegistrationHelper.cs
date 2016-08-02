/*
 * IRegistrationHelper.cs - Implementation of the
 *			"System.EnterpriseServices.IRegistrationHelper" class.
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

namespace System.EnterpriseServices
{

using System.Runtime.InteropServices;

#if CONFIG_COM_INTEROP
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
#endif
#if !ECMA_COMPAT
[Guid("55e3ea25-55cb-4650-8887-18e8d30bb4bc")]
#endif
public interface IRegistrationHelper
{
	// Install an assembly into the catalog.
	void InstallAssembly
			([In] String assembly, [In,Out] ref String application,
			 [In,Out] ref String tlb, [In] InstallationFlags installFlags);

	// Uninstall an assembly from the catalog.
	void UninstallAssembly
			([In] String assembly, [In] String application);

}; // interface IRegistrationHelper

}; // namespace System.EnterpriseServices
