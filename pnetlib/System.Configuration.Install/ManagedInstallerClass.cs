/*
 * ManagedInstallerClass.cs - Implementation of the
 *	    "System.Configuration.Install.ManagedInstallerClass" class.
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

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

#if !ECMA_COMPAT
[ComVisible(true)]
[Guid("42EB0342-0393-448f-84AA-D4BEB0283595")]
#endif
public class ManagedInstallerClass : IManagedInstaller
{
	// Internal state.
	private String program;
	private String[] args;
	private String[] options;
	private StringDictionary optionDict;
	private String filename;
	private bool uninstall;
	private InstallContext context;

	// Public constructor, for backwards compatibility.
	// Use the "InstallHelper" method instead.
	public ManagedInstallerClass() {}

	// Internal constructor.
	private ManagedInstallerClass(String[] args)
			{
				this.program = GetProgramName();
				if(args != null)
				{
					this.args = args;
				}
				else
				{
					this.args = new String [0];
				}
			}

	// Get the name of the program that called us.
	private static String GetProgramName()
			{
				String program = Environment.GetCommandLineArgs()[0];
				if(program.EndsWith(".exe") || program.EndsWith(".EXE"))
				{
					program = program.Substring(0, program.Length - 4);
				}
				int index1 = program.LastIndexOf('/');
				int index2 = program.LastIndexOf('\\');
				if(index1 < index2)
				{
					index1 = index2;
				}
				if(index1 != -1)
				{
					program = program.Substring(index1 + 1);
				}
				return program;
			}

	// Print version header information for this program.
	private void VersionHeader()
			{
				Console.WriteLine
					("ILINSTALL {0} - IL Assembly Installer",
					 Environment.Version);
				Console.WriteLine
					("Copyright (c) 2003 Southern Storm Software, Pty Ltd.");
				Console.WriteLine();
			}

	// Print version information for this program.
	private void Version()
			{
				VersionHeader();
				Console.WriteLine
					("ILINSTALL comes with ABSOLUTELY NO WARRANTY.  This is free software,");
				Console.WriteLine
					("and you are welcome to redistribute it under the terms of the");
				Console.WriteLine
					("GNU General Public License.  See the file COPYING for further details.");
				Console.WriteLine();
				Console.WriteLine
					("Use the `--help' option to get help on the command-line options.");
			}

	// Print usage information for this program.
	private void Usage()
			{
				VersionHeader();
				Console.WriteLine
					("Usage: {0} [options] assembly [[options] assembly ...]",
					 program);
				Console.WriteLine();
				Console.WriteLine
					("    --install          or   -i");
				Console.WriteLine
					("        Install the assemblies (default).");
				Console.WriteLine
					("    --uninstall        or   -u");
				Console.WriteLine
					("        Uninstall the assemblies.");
				Console.WriteLine
					("    --logfile=file");
				Console.WriteLine
					("        Specify the name of the log file.");
				Console.WriteLine
					("    --logtoconsole");
				Console.WriteLine
					("        Also log messages to the console.");
				Console.WriteLine
					("    --showcallstack");
				Console.WriteLine
					("        Show the call stack when logging exceptions.");
				Console.WriteLine
					("    --version          or   -v");
				Console.WriteLine
					("        Print the version of this program.");
				Console.WriteLine
					("    --help assembly    or   -h assembly");
				Console.WriteLine
					("        Print help for a specific assembly.");
				Console.WriteLine
					("    --help             or   -h");
				Console.WriteLine
					("        Print this help message.");
				Console.WriteLine();
				Console.WriteLine
					("Options may also be specified with `/'.  e.g. `/uninstall'.");
			}

	// Print help for a specific installer assembly.
	private void PrintHelpFor(String filename)
			{
				AssemblyInstaller inst;
				inst = new AssemblyInstaller(filename, options);
				Console.WriteLine("Help options for {0}:", filename);
				Console.WriteLine();
				Console.Write(inst.HelpText);
			}

	// Run the installation process for a specific assembly.
	private void RunInstall(String filename)
			{
				// Load the installer assembly.
				AssemblyInstaller inst;
				inst = new AssemblyInstaller(filename, options);

				// Wrap the installer in a transaction.
				TransactedInstaller trans;
				trans = new TransactedInstaller();
				trans.Installers.Add(inst);

				// Install the assembly.
				IDictionary dict = new Hashtable();
				trans.Install(dict);

				// Write the state information, for later uninstall.
				// TODO
			}

	// Run the uninstallation process for a specific assembly.
	private void RunUninstall(String filename)
			{
				// Load the installer assembly.
				AssemblyInstaller inst;
				inst = new AssemblyInstaller(filename, options);

				// Wrap the installer in a transaction.
				TransactedInstaller trans;
				trans = new TransactedInstaller();
				trans.Installers.Add(inst);

				// Load the previous state information from the install.
				IDictionary dict = new Hashtable();
				// TODO

				// Install the assembly.
				trans.Uninstall(dict);
			}

	// Run the installation process.
	private int Install()
			{
				// Scan the command-line options in groups.
				int posn = 0;
				int start;
				String arg;
				bool both;
				uninstall = false;
				if(args.Length == 0)
				{
					Usage();
					return 1;
				}
				while(posn < args.Length)
				{
					// Extract the next group of options.
					start = posn;
					while(posn < args.Length)
					{
						arg = args[posn];
						if(arg.Length == 0)
						{
							break;
						}
						if(arg[0] == '-')
						{
							// Option that starts with "-".
							++posn;
							if(arg.Length == 2 && arg[1] == '-')
							{
								// We use "--" to terminate the option
								// list just prior to a filename that
								// starts with "-" or "/".
								break;
							}
						}
						else if(arg[0] == '/')
						{
							// Compatibility option that starts with "/".
							++posn;
						}
						else if(arg[0] == '=')
						{
							// May be specifying a value for a previous option.
							++posn;
						}
						else if(posn > start && args[posn - 1].EndsWith("="))
						{
							// Specifying a value for a previous option name.
							++posn;
						}
						else
						{
							// This is a filename.
							break;
						}
					}

					// Parse the command line options that we just received.
					optionDict = InstallContext.ParseCommandLine
						(args, start, posn - start, out options);

					// Extract the filename.
					if(posn < args.Length)
					{
						filename = args[posn++];
					}
					else
					{
						filename = null;
					}

					// Create an install context for this option group.
					context = new InstallContext(optionDict);

					// Check for the "uninstall" and "install" flags.
					both = false;
					if(context.IsParameterTrue("uninstall") ||
					   context.IsParameterTrue("u"))
					{
						uninstall = true;
						both = true;
					}
					if(context.IsParameterTrue("install") ||
					   context.IsParameterTrue("i"))
					{
						if(both)
						{
						#if !CONFIG_SMALL_CONSOLE
							Console.Error.WriteLine
								("{0}: cannot specify both `--install' and " +
								 "`--uninstall'", program);
						#else
							Console.WriteLine
								("{0}: cannot specify both `--install' and " +
								 "`--uninstall'", program);
						#endif
							return 1;
						}
						uninstall = false;
					}

					// Check for the version flag.
					if(context.IsParameterTrue("version") ||
					   context.IsParameterTrue("v"))
					{
						Version();
						return 0;
					}

					// Check for the help flag.
					if(context.IsParameterTrue("help") ||
					   context.IsParameterTrue("h") ||
					   context.IsParameterTrue("?"))
					{
						if(filename == null)
						{
							Usage();
						}
						else
						{
							PrintHelpFor(filename);
						}
						continue;
					}

					// If we don't have a filename, then print the usage.
					if(filename == null)
					{
						Usage();
						return 1;
					}

					// Run the installation/uninstallation process.
					if(uninstall)
					{
						RunUninstall(filename);
					}
					else
					{
						RunInstall(filename);
					}
				}
				return 0;
			}

	// Entry point that is called by "InstallUtil".
	public static void InstallHelper(String[] args)
			{
				ManagedInstallerClass inst;
				inst = new ManagedInstallerClass(args);
				int exitCode = inst.Install();
				if(exitCode != 0)
				{
					Environment.Exit(exitCode);
				}
			}

	// Perform a managed install - this entry point is not recommended.
	// It is provided for backwards compatibility only.
	int IManagedInstaller.ManagedInstall(String commandLine, int hInstall)
			{
				this.program = GetProgramName();
				if(commandLine != null && commandLine.Length > 0)
				{
					this.args = commandLine.Split(' ');
				}
				else
				{
					this.args = new String [0];
				}
				return Install();
			}

}; // class ManagedInstallerClass

#endif // !ECMA_COMPAT

}; // namespace System.Configuration.Install
