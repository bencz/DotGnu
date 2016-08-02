/*
 * RegistrationHelperTx.cs - Implementation of the
 *			"System.EnterpriseServices.RegistrationHelperTx" class.
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
[Guid("9e31421c-2f15-4f35-ad20-66fb9d4cd428")]
#endif
[Transaction(TransactionOption.RequiresNew)]
public sealed class RegistrationHelperTx : ServicedComponent
{
	// Constructor.
	public RegistrationHelperTx() {}

	// Method that is called when the serviced component is activated.
	protected internal override void Activate() {}

	// Method that is called just before the component is deactivated.
	protected internal override void Deactivate() {}

	// Install an assembly.
	public void InstallAssembly
				(String assembly, ref String application,
				 ref String tlb, InstallationFlags installFlags, Object sync)
			{
				InstallAssembly(assembly, ref application, null,
								ref tlb, installFlags, sync);
			}
	public void InstallAssembly
				(String assembly, ref String application, String partition,
				 ref String tlb, InstallationFlags installFlags, Object sync)
			{
				RegistrationConfig config = new RegistrationConfig();
				config.AssemblyFile = assembly;
				config.Application = application;
				config.Partition = partition;
				config.TypeLibrary = tlb;
				config.InstallationFlags = installFlags;
				InstallAssemblyFromConfig(ref config, sync);
				application = config.Application;
				tlb = config.TypeLibrary;
			}

	// Uninstall an assembly.
	public void UninstallAssembly
				(String assembly, String application, Object sync)
			{
				UninstallAssembly(assembly, application, null, sync);
			}
	public void UninstallAssembly
				(String assembly, String application,
				 String partition, Object sync)
			{
				RegistrationConfig config = new RegistrationConfig();
				config.AssemblyFile = assembly;
				config.Application = application;
				UninstallAssemblyFromConfig(ref config, sync);
			}

	// Install an assembly given configuration information.
	public void InstallAssemblyFromConfig
				(ref RegistrationConfig regConfig, Object sync)
			{
				RegistrationHelper helper;
				bool complete = false;
				try
				{
					helper = new RegistrationHelper();
					helper.InstallAssemblyFromConfig(ref regConfig);
					ContextUtil.SetComplete();
					complete = true;
				}
				finally
				{
					if(!complete)
					{
						ContextUtil.SetAbort();
					}
				}
			}

	// Uninstall an assembly given configuration information.
	public void UninstallAssemblyFromConfig
				(ref RegistrationConfig regConfig, Object sync)
			{
				RegistrationHelper helper;
				bool complete = false;
				try
				{
					helper = new RegistrationHelper();
					helper.UninstallAssemblyFromConfig(ref regConfig);
					ContextUtil.SetComplete();
					complete = true;
				}
				finally
				{
					if(!complete)
					{
						ContextUtil.SetAbort();
					}
				}
			}

	// Determine if this component is running inside a transaction.
	public bool IsInTransaction()
			{
				return ContextUtil.IsInTransaction;
			}

}; // class RegistrationHelperTx

}; // namespace System.EnterpriseServices
