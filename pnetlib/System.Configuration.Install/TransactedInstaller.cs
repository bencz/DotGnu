/*
 * TransactedInstaller.cs - Implementation of the
 *	    "System.Configuration.Install.TransactedInstaller" class.
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

namespace System.Configuration.Install
{

#if !ECMA_COMPAT

using System.Reflection;
using System.Collections;

public class TransactedInstaller : Installer
{
	// Constructor.
	public TransactedInstaller() {}

    // Perform the installation process, saving the previous
    // state in the "stateSaver" object.
    public override void Install(IDictionary stateSaver)
			{
				// Make sure that we have a context.
				if(Context == null)
				{
					Context = new InstallContext("con", new String [0]);
				}

				// Log the start of the transaction.
				Context.LogLine(S._("Installer_BeginInstallTransaction"));
				try
				{
					// Run the installation process.
					try
					{
						Context.LogLine(S._("Installer_BeginInstall"));
						base.Install(stateSaver);
					}
					catch(SystemException)
					{
						// Roll back the transaction.
						Context.LogLine(S._("Installer_BeginRollback"));
						try
						{
							Rollback(stateSaver);
						}
						catch(SystemException)
						{
							// Ignore errors during rollback.
						}
						Context.LogLine(S._("Installer_EndRollback"));
	
						// Notify the caller about the rollback.
						throw new InvalidOperationException
							(S._("Installer_RollbackPerformed"));
					}
	
					// Commit the transaction.
					Context.LogLine(S._("Installer_BeginCommit"));
					try
					{
						Commit(stateSaver);
					}
					finally
					{
						Context.LogLine(S._("Installer_EndCommit"));
					}
				}
				finally
				{
					Context.LogLine(S._("Installer_EndInstallTransaction"));
				}
			}
	
    // Uninstall and return to a previously saved state.
    public override void Uninstall(IDictionary savedState)
			{
				// Make sure that we have a context.
				if(Context == null)
				{
					Context = new InstallContext("con", new String [0]);
				}

				// Log the start of the transaction.
				Context.LogLine(S._("Installer_BeginUninstallTransaction"));
				try
				{
					// Run the uninstallation process.
					base.Uninstall(savedState);
				}
				finally
				{
					Context.LogLine
						(S._("Installer_EndUninstallTransaction"));
				}
			}

}; // class TransactedInstaller

#endif // !ECMA_COMPAT

}; // namespace System.Configuration.Install
