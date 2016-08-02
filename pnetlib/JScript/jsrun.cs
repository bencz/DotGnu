/*
 * jsrun.cs - Run scripts from the command-line.
 *
 * Copyright (C) 2003 Southern Storm Software, Pty Ltd.
 *
 * Contributions by Carl-Adam Brengesjo <ca.brengesjo@telia.com>
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

using System;
using System.IO;
using System.Text;
using System.Collections.Specialized;
using Microsoft.JScript;
using Microsoft.JScript.Vsa;
using Microsoft.Vsa;

public sealed class JSRun
{
	private static Version jsrunVersion = new Version("1.5");

	private static void Version()
			{
				Console.WriteLine("JSRUN {0} - JScript Commandline Executer", jsrunVersion);
				Console.WriteLine("Copyright (C) 2003 Southern Storm Software, Pty Ltd.");
				Console.WriteLine();
				Console.WriteLine("JSRUN comes with ABSOLUTELY NO WARRANTY.  This is free software,");
				Console.WriteLine("and you are welcome to redistribute it under the terms of the");
				Console.WriteLine("GNU General Public License.  See the file COPYING for further details.");
			}

	private static void Usage()
			{
				Console.WriteLine("JSRUN {0} - JScript Commandline Executer", jsrunVersion);
				Console.WriteLine("Copyright (C) 2003 Southern Storm Software, Pty Ltd.");
				Console.WriteLine();
				Console.WriteLine("Usage: jsrun [options] script [args]");
				Console.WriteLine();
				Console.WriteLine("  -h  --help	 Prints this help and exits.");
				Console.WriteLine("  -v  --version  Prints version and exts.");
			}

	// Main entry point for the application.
	public static int Main(String[] args)
			{
#if CONFIG_EXTENDED_NUMERICS && CONFIG_REFLECTION
				StreamReader reader = null;
				int len;
				VsaEngine engine;
				IVsaCodeItem item;
				StringCollection scripts = new StringCollection();

				// Create an engine instance and add the script to it.
				engine = VsaEngine.CreateEngine();
				engine.SetOption("print", true);

				// get command-line arguments
				int i = 0;
				String arg = args.Length == 0 ? null : args[0];
				while(arg != null)
				{
					String next = null;
					if (arg.StartsWith("-") && !arg.StartsWith("--") &&
							arg.Length > 2)
					{
						next = "-" + arg.Substring(2, arg.Length - 2);
						arg = arg.Substring(0,2);
					}
					switch(arg)
					{
					case "-h":
					case "--help":
						Usage();
						return 0;
					case "-v":
					case "--version":
						Version();
						return 0;
					default:
						// matches both short and long options. (-/--)
						if (arg.StartsWith("-"))
						{
#if !CONFIG_SMALL_CONSOLE
							Console.Error.WriteLine
#else
							Console.WriteLine
#endif
								("jsrun: unkown option `{0}'", arg);
							return 1;
						}
						// not an option - assumes script path
						else
						{
							// To prevent a relative and a absolute pathname to same file!
							FileInfo fi = new FileInfo(arg);
							if(!fi.Exists)
							{
#if !CONFIG_SMALL_CONSOLE
								Console.Error.WriteLine
#else
								Console.WriteLine
#endif
									("jsrun: {0}: No such file or directory", arg);
								return 1;
							}
							// Cannot load same script source twice!
							if(scripts.Contains(fi.FullName))
							{
#if !CONFIG_SMALL_CONSOLE
								Console.Error.WriteLine
#else
								Console.WriteLine
#endif
								("jsrun: {0}: use of duplicate sources illegal.", fi.FullName);
							}
							else
							{
								scripts.Add(fi.FullName);
							}
							// Load script file
							try
							{
								reader = new StreamReader(arg);
							}
							catch(Exception e)
							{
#if !CONFIG_SMALL_CONSOLE
								Console.Error.WriteLine
#else
								Console.WriteLine
#endif
									("jsrun: ({0}): {1}", e.GetType(), e.Message);
							}

							// Load the script into memory as a string.
							StringBuilder scriptBuilder = new StringBuilder();
							char[] scriptBuffer = new char [512];
							while((len = reader.Read(scriptBuffer, 0, 512)) > 0)
							{
								scriptBuilder.Append(scriptBuffer, 0, len);
							}
							reader.Close();

							item = (IVsaCodeItem)(engine.Items.CreateItem
									(String.Concat("script",engine.Items.Count+1),
									VsaItemType.Code, VsaItemFlag.None));
							item.SourceText = scriptBuilder.ToString();
							item.SetOption("codebase", arg);
						}
						break;
					} // switch(arg)

					if(next != null)
					{
						arg = next;
					}
					else if(i + 1 >= args.Length)
					{
						arg = null;
					}
					else
					{
						i = i + 1;
						arg = args[i];
					}
				} // for each in args

				// We need at least one item.
				if(engine.Items.Count == 0)
				{
					Usage();
					return 1;
				}

				// Compile and run the script.
				if(!engine.Compile())
				{
#if !CONFIG_SMALL_CONSOLE
					Console.Error.WriteLine
#else
					Console.WriteLine
#endif
						("jsrun: Could not compile script");
					return 1;
				}
				engine.Run();

				// Shut down the engine and exit.
				engine.Close();
				return 0;
#else
				// Use error output and exit status in case any
				// script/program is depending on output.
#if !CONFIG_SMALL_CONSOLE
				Console.Error.WriteLine
#else
				Console.WriteLine
#endif
					("JScript is not available in this configuration " +
					 "because the library does\n" +
					 "not have sufficient features to support JScript.");
				return 1;
#endif
			}

}; // class JSRun
