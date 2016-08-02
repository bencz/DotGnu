/*
 * Application.cs - The main application object for a display.
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
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

namespace Xsharp
{

using System;
using System.Collections;
using System.Net;
using System.Runtime.InteropServices;

/// <summary>
/// <para>The <see cref="T:Xsharp.Application"/> class manages
/// initialization and event handling for an application.</para>
/// </summary>
public sealed class Application : IDisposable
{
	// Internal state.
	private String resourceName;
	private String resourceClass;
	private String programName;
	private String[] cmdLineArgs;
	private String displayName;
	private Display display;
	private bool startIconic;
	private String title;
	private String geometry;
	private Font defaultFont;
	private static Application primary;

	/// <summary>
	/// <para>Construct a new application object, process command-line
	/// options, and open the display.</para>
	/// </summary>
	///
	/// <param name="name">
	/// <para>The resource name and class for the application.</para>
	/// </param>
	///
	/// <param name="args">
	/// <para>The arguments that came from the application's "Main"
	/// method.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XCannotConnectException">
	/// <para>A connection to the X display server could not
	/// be established.</para>
	/// </exception>
	public Application(String name, String[] args)
			{
				String[] envCmdLine;
				int firstArg = 0;
				String fontName;
				bool synchronous = false;

				// Set this as the primary application object if necessary.
				lock(typeof(Application))
				{
					if(primary == null)
					{
						primary = this;
					}
				}

				// Choose defaults for the parameters.
				try
				{
					envCmdLine = Environment.GetCommandLineArgs();
				}
				catch(NotSupportedException)
				{
					envCmdLine = null;
				}
				if(envCmdLine != null && envCmdLine.Length > 0)
				{
					programName = envCmdLine[0];
				}
				else
				{
					programName = "Xsharp-application";
				}
				if(name == null)
				{
					// Strip the path from the program name to
					// get the default resource name to use.
					int index = programName.LastIndexOf('/');
					if(index == -1)
					{
						index = programName.LastIndexOf('\\');
					}
					if(index != -1)
					{
						name = programName.Substring(index + 1);
					}
					else
					{
						name = programName;
					}
					int len = name.Length;
					if(len > 4 && name[len - 4] == '.' &&
					   (name[len - 3] == 'e' || name[len - 3] == 'E') &&
					   (name[len - 2] == 'x' || name[len - 2] == 'X') &&
					   (name[len - 1] == 'e' || name[len - 1] == 'E'))
					{
						name = name.Substring(0, len - 4);
					}
				}
				if(args == null)
				{
					if(envCmdLine != null && envCmdLine.Length > 0)
					{
						args = envCmdLine;
						firstArg = 1;
					}
					else
					{
						args = new String [0];
					}
				}

				// Initialize the application state.
				resourceName = name;
				resourceClass = name;
				displayName = null;
				startIconic = false;
				title = null;
				geometry = null;
				fontName = null;

				// Process the standard Xt command-line options.
				ArrayList newArgs = new ArrayList();
				while(firstArg < args.Length)
				{
					switch(args[firstArg])
					{
						case "-display":
						{
							++firstArg;
							if(firstArg < args.Length)
							{
								displayName = args[firstArg];
							}
						}
						break;

						case "-iconic":
						{
							startIconic = true;
						}
						break;

						case "-name":
						{
							++firstArg;
							if(firstArg < args.Length)
							{
								resourceName = args[firstArg];
							}
						}
						break;

						case "-title":
						{
							++firstArg;
							if(firstArg < args.Length)
							{
								title = args[firstArg];
							}
						}
						break;

						case "-fn":
						case "-font":
						{
							++firstArg;
							if(firstArg < args.Length)
							{
								fontName = args[firstArg];
							}
						}
						break;

						case "-geometry":
						{
							++firstArg;
							if(firstArg < args.Length)
							{
								geometry = args[firstArg];
							}
						}
						break;

						case "+synchronous":
						case "-synchronous":
						{
							// Turn on synchronous processing to the X server.
							synchronous = true;
						}
						break;

						// Ignore other Xt toolkit options that aren't
						// relevant to us.  We may add some of these later.
						case "-reverse":
						case "-rv":
						case "+rv":
							break;
						case "-bg":
						case "-background":
						case "-bw":
						case "-borderwidth":
						case "-bd":
						case "-bordercolor":
						case "-fg":
						case "-foreground":
						case "-selectionTimeout":
						case "-xnllanguage":
						case "-xrm":
						case "-xtsessionID":
							++firstArg;
							break;

						default:
						{
							// Unknown option - copy it to "newArgs".
							newArgs.Add(args[firstArg]);
						}
						break;
					}
					++firstArg;
				}
				cmdLineArgs = (String[])(newArgs.ToArray(typeof(String)));

				// Connect to the display.
				if(displayName == null)
				{
					// Xlib will figure it by itself, but classes using displayName can get broken is it's null
					displayName = Environment.GetEnvironmentVariable("DISPLAY");
				}
				if( null == displayName || displayName == string.Empty ) {
					displayName = ":0";
				}

				display = Xsharp.Display.Open(displayName, this, synchronous);

				// Create the default font.
				defaultFont = Font.CreateFromXLFD(fontName);
				defaultFont.GetFontSet(display);
			}

	/// <summary>
	/// <para>Close the application if it is currently active.</para>
	/// </summary>
	~Application()
			{
				Close();
			}

	/// <summary>
	/// <para>Close the application if it is currently active.</para>
	/// </summary>
	///
	/// <remarks>
	/// <para>This method implements the <see cref="T:System.IDisposable"/>
	/// interface.</para>
	/// </remarks>
	public void Dispose()
			{
				Close();
			}

	/// <summary>
	/// <para>Close the application if it is currently active.</para>
	/// </summary>
	public void Close()
			{
				lock(typeof(Application))
				{
					if(display != null)
					{
						display.Close();
						display = null;
					}
					if(primary == this)
					{
						primary = null;
					}
				}

			}

	/// <summary>
	/// <para>Run the main event loop on this application.</para>
	/// </summary>
	///
	/// <remarks>
	/// <para>The main event loop will run until the <c>Quit</c>
	/// method is called on the display.</para>
	/// </remarks>
	public void Run()
			{
				if(display != null)
				{
					display.Run();
				}
			}

	/// <summary>
	/// <para>Process pending events, and return immediately.</para>
	/// </summary>
	///
	/// <returns>
	/// <para>Returns <see langword="true"/> if events were processed,
	/// or <see langword="false"/> if there are no pending events.</para>
	/// </returns>
	public bool ProcessPendingEvents()
			{
				if(display != null)
				{
					return display.ProcessPendingEvents();
				}
				else
				{
					return false;
				}
			}

	/// <summary>
	/// <para>Wait for the next event, process it, and then return.</para>
	/// </summary>
	///
	/// <returns>
	/// <para>Returns <see langword="true"/> if an event was processed,
	/// or <see langword="false"/> if <c>Quit</c> was detected.</para>
	/// </returns>
	public bool WaitForEvent()
			{
				if(display != null)
				{
					return display.WaitForEvent();
				}
				else
				{
					return false;
				}
			}

	/// <summary>
	/// <para>Get the name of this program (i.e. the argv[0] value).</para>
	/// </summary>
	///
	/// <value>
	/// <para>The program name.</para>
	/// </value>
	public String ProgramName
			{
				get
				{
					return programName;
				}
			}

	/// <summary>
	/// <para>Get the resource name of this program's primary window.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The resource name.</para>
	/// </value>
	public String ResourceName
			{
				get
				{
					return resourceName;
				}
			}

	/// <summary>
	/// <para>Get the resource class of this program.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The resource class.</para>
	/// </value>
	public String ResourceClass
			{
				get
				{
					return resourceClass;
				}
			}

	/// <summary>
	/// <para>Get the command-line arguments for this program that
	/// were left after standard options were removed.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The argument array.</para>
	/// </value>
	public String[] Args
			{
				get
				{
					return cmdLineArgs;
				}
			}

	/// <summary>
	/// <para>Get the name of the display that we connected to,
	/// or <see langword="null"/> for the default display.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The display name.</para>
	/// </value>
	public String DisplayName
			{
				get
				{
					return displayName;
				}
			}

	/// <summary>
	/// <para>Get the display that is associated with this application.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The display object.</para>
	/// </value>
	public Display Display
			{
				get
				{
					return display;
				}
			}

	/// <summary>
	/// <para>Determine if the program's primary window should
	/// start in an iconic state.</para>
	/// </summary>
	///
	/// <value>
	/// <para>Returns <see langword="true"/> if the window should
	/// start iconic, or <see langword="false"/> if not.</para>
	/// </value>
	public bool StartIconic
			{
				get
				{
					return startIconic;
				}
			}

	/// <summary>
	/// <para>Get the value of the "-title" command-line option,
	/// or <see langword="null"/> if no title override.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The window title.</para>
	/// </value>
	public String Title
			{
				get
				{
					return title;
				}
			}

	/// <summary>
	/// <para>Get the value of the "-geometry" command-line option,
	/// or <see langword="null"/> if no geometry override.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The window geometry.</para>
	/// </value>
	public String Geometry
			{
				get
				{
					return geometry;
				}
			}

	/// <summary>
	/// <para>Get the application's default font.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The default font object.</para>
	/// </value>
	public Font DefaultFont
			{
				get
				{
					return defaultFont;
				}
			}

	/// <summary>
	/// <para>Get the primary application object.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The application object.</para>
	/// </value>
	public static Application Primary
			{
				get
				{
					return primary;
				}
			}

	/// <summary>
	/// <para>Get the name of the host that this program is running on.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The host name.</para>
	/// </value>
	public static String Hostname
			{
				get
				{
					// Get the hostname via "_XGetHostname" in "Xlib".
					IntPtr buf = Marshal.AllocHGlobal(1024);
					if(buf == IntPtr.Zero)
					{
						return null;
					}
					Xlib._XGetHostname(buf, 1024);
					String host = Marshal.PtrToStringAnsi(buf);
					Marshal.FreeHGlobal(buf);
					if(host == null || (host.Length == 0))
					{
						return null;
					}

				#if false
					// TODO: DNS routines are a little flaky at present.

					// Fully-qualify the name, if possible.
					if(host.IndexOf('.') != -1)
					{
						return null;
					}
					IPHostEntry entry = Dns.Resolve(host);
					if(entry == null)
					{
						return host;
					}
					else
					{
						return entry.HostName;
					}
				#endif
					return host;
				}
			}

} // class Application

} // namespace Xsharp
