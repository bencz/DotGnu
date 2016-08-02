/*
 * AssemblyInstaller.cs - Implementation of the
 *	    "System.Configuration.Install.AssemblyInstaller" class.
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
using System.ComponentModel;
using System.IO;

public class AssemblyInstaller : Installer
{
	// Internal state.
	private Assembly assembly;
	private String assemblyPath;
	private String[] commandLine;
	private bool useNewContext;
	private AssemblyInfo info;
	private static AssemblyInfo[] assemblies;

	internal class AssemblyInfo
	{
		public String filename;
		public Assembly assembly;
		public AssemblyInstaller installer;
		public Exception loadException;

	}; // class AssemblyInfo

	// Constructors.
	public AssemblyInstaller()
			{
				this.useNewContext = true;
			}
	public AssemblyInstaller(Assembly assembly, String[] commandLine)
			{
				this.assembly = assembly;
				this.commandLine = commandLine;
				this.useNewContext = true;
			}
	public AssemblyInstaller(String filename, String[] commandLine)
			{
				this.assemblyPath = filename;
				this.commandLine = commandLine;
				this.useNewContext = true;
			}

	// Get or set this object's properties.
	public Assembly Assembly
			{
				get
				{
					return assembly;
				}
				set
				{
					assembly = value;
				}
			}
	public String[] CommandLine
			{
				get
				{
					return commandLine;
				}
				set
				{
					commandLine = value;
				}
			}
	public override String HelpText
			{
				get
				{
					Initialize();
					String text = String.Empty;
					foreach(Installer inst in Installers)
					{
						text += inst.HelpText + Environment.NewLine;
					}
					return text;
				}
			}
	public String Path
			{
				get
				{
					return assemblyPath;
				}
				set
				{
					assemblyPath = value;
				}
			}
	public bool UseNewContext
			{
				get
				{
					return useNewContext;
				}
				set
				{
					useNewContext = value;
				}
			}

	// Load all installers from a particular assembly.
	private static void LoadInstallers(AssemblyInfo info)
			{
				Type[] types = info.assembly.GetTypes();
				ConstructorInfo ctor;
				foreach(Type type in types)
				{
					// Type must not be abstract and must
					// inherit from the "Installer" class.
					if(!type.IsAbstract &&
					   type.IsSubclassOf(typeof(Installer)))
					{
						// Check for a zero-argument public ctor.
						ctor = type.GetConstructor(Type.EmptyTypes);
						if(ctor == null)
						{
							continue;
						}

					#if !ECMA_COMPAT
						// Check for the "RunInstaller" attribute.
						Object[] attrs =
							type.GetCustomAttributes
								(typeof(RunInstallerAttribute), false);
						if(attrs != null && attrs.Length > 0)
						{
							if(((RunInstallerAttribute)(attrs[0]))
									.RunInstaller)
							{
								// This is a valid installer.
								info.installer.Installers.Add
									(ctor.Invoke(new Object [0])
										as Installer);
							}
						}
					#endif
					}
				}
			}

	// Initialize this object if necessary.
	private void Initialize()
			{
				if(info == null)
				{
					if(Context == null)
					{
						Context = new InstallContext(null, commandLine);
					}
					if(assembly != null)
					{
						info = new AssemblyInfo();
						info.assembly = assembly;
						info.installer = this;
						LoadInstallers(info);
					}
					else
					{
						LoadInstallerAssembly(assemblyPath, Context);
					}
				}
			}

	// Check to see if a particular assembly is installable.
	public static void CheckIfInstallable(String assemblyName)
			{
				AssemblyInfo info = LoadInstallerAssembly
					(assemblyName, new InstallContext());
				if(info.installer.Installers.Count == 0)
				{
					throw new InvalidOperationException
						(S._("Installer_NoInstallersFound"));
				}
			}

    // Commit the installation transaction.
    public override void Commit(IDictionary savedState)
			{
				Initialize();
				base.Commit(savedState);
			}

    // Perform the installation process, saving the previous
    // state in the "stateSaver" object.
    public override void Install(IDictionary stateSaver)
			{
				Initialize();
				base.Install(stateSaver);
			}

    // Roll back the current installation to "savedState".
    public override void Rollback(IDictionary savedState)
			{
				Initialize();
				base.Rollback(savedState);
			}

    // Uninstall and return to a previously saved state.
    public override void Uninstall(IDictionary savedState)
			{
				Initialize();
				base.Uninstall(savedState);
			}

	// Load an assembly by name and get the information object for it.
	internal static AssemblyInfo LoadInstallerAssembly
				(String filename, InstallContext logContext)
			{
				String fullName;
				AssemblyInfo info;
				AssemblyInfo[] newAssemblies;
				int index;
				lock(typeof(AssemblyInstaller))
				{
					try
					{
						// See if we have a cached information block,
						// from when we loaded the assembly previously.
						fullName = IO.Path.GetFullPath(filename);
						if(assemblies != null)
						{
							for(index = 0; index < assemblies.Length; ++index)
							{
								info = assemblies[index];
								if(info.filename == fullName)
								{
									if(info.loadException == null)
									{
										return info;
									}
									throw info.loadException;
								}
							}
							newAssemblies = new AssemblyInfo
								[assemblies.Length + 1];
							Array.Copy(assemblies, 0, newAssemblies, 0,
									   assemblies.Length);
							info = new AssemblyInfo();
							newAssemblies[assemblies.Length] = info;
							assemblies = newAssemblies;
						}
						else
						{
							info = new AssemblyInfo();
							assemblies = new AssemblyInfo [] {info};
						}

						// Try to load the assembly into memory.
						info.filename = fullName;
						try
						{
							info.assembly = Assembly.LoadFrom(fullName);
						}
						catch(SystemException e)
						{
							info.loadException = e;
							throw;
						}

						// Wrap the assembly in an installer.
						info.installer = new AssemblyInstaller();
						info.installer.assemblyPath = filename;
						info.installer.info = info;

						// Search for all public installer types.
						LoadInstallers(info);

						// The assembly is ready to go.
						return info;
					}
					catch(SystemException e)
					{
						if(logContext != null)
						{
							if(logContext.IsParameterTrue("ShowCallStack"))
							{
								logContext.LogLine
									("LoadAssembly: " + e.ToString());
							}
							else
							{
								logContext.LogLine
									("LoadAssembly: " + e.Message);
							}
						}
						throw new InvalidOperationException
							(String.Format
								(S._("Installer_CouldNotLoadAssembly"),
								 filename), e);
					}
				}
			}

}; // class AssemblyInstaller

#endif // !ECMA_COMPAT

}; // namespace System.Configuration.Install
