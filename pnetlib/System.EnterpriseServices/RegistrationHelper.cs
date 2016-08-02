/*
 * RegistrationHelper.cs - Implementation of the
 *			"System.EnterpriseServices.RegistrationHelper" class.
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
using System.Security;

#if !ECMA_COMPAT
[Guid("89a86e7b-c229-4008-9baa-2f5c8411d7e0")]
#endif
public sealed class RegistrationHelper
	: MarshalByRefObject, IRegistrationHelper
{
	// Constructor.
	public RegistrationHelper() {}

	// Implement the IRegistrationHelper interface.
	public void InstallAssembly
				(String assembly, ref String application,
				 ref String tlb, InstallationFlags installFlags)
			{
				InstallAssembly(assembly, ref application, null,
								ref tlb, installFlags);
			}
	public void UninstallAssembly(String assembly, String application)
			{
				UninstallAssembly(assembly, application, null);
			}

	// Install an assembly.
	public void InstallAssembly
				(String assembly, ref String application, String partition,
				 ref String tlb, InstallationFlags installFlags)
			{
				RegistrationConfig config = new RegistrationConfig();
				config.AssemblyFile = assembly;
				config.Application = application;
				config.Partition = partition;
				config.TypeLibrary = tlb;
				config.InstallationFlags = installFlags;
				InstallAssemblyFromConfig(ref config);
				application = config.Application;
				tlb = config.TypeLibrary;
			}

	// Uninstall an assembly.
	public void UninstallAssembly
				(String assembly, String application, String partition)
			{
				RegistrationConfig config = new RegistrationConfig();
				config.AssemblyFile = assembly;
				config.Application = application;
				UninstallAssemblyFromConfig(ref config);
			}

	// Install an assembly given configuration information.
	public void InstallAssemblyFromConfig(ref RegistrationConfig regConfig)
			{
				throw new SecurityException();
			}

	// Uninstall an assembly given configuration information.
	public void UninstallAssemblyFromConfig(ref RegistrationConfig regConfig)
			{
				throw new SecurityException();
			}

}; // class RegistrationHelper

}; // namespace System.EnterpriseServices
