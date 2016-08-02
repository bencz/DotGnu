/*
 * EmbeddedApplication.cs - Widget handling for embedding X apps in each other.
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

namespace Xsharp
{

// We need "extended diagnostics" to get the process launch capabilities.
#if CONFIG_EXTENDED_DIAGNOSTICS

using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Xsharp.Types;
using Xsharp.Events;
using OpenSystem.Platform.X11;

/// <summary>
/// <para>The <see cref="T:Xsharp.EmbeddedApplication"/> class manages
/// embedding a foreign X application as a child of this application's
/// widget tree.</para>
/// </summary>
///
/// <remarks>
/// <para>This class uses the <c>XC-APPGROUP</c> extension that is present
/// in common X servers to force a child application to embed itself
/// in our widget tree.</para>
///
/// <para>If the extension is not present, or it cannot
/// be properly initialized for some reason, then the child application
/// will be displayed in its own top-level window rather than being
/// embedded.</para>
///
/// <para>The <c>CanEmbed</c> method can be used to determine if
/// a display supports embedding.</para>
///
/// <para>If the parent application is executed within an ssh account,
/// with X11 forwarding enabled, then the <c>XREALDISPLAY</c> environment
/// variable must be set to indicate the real display server.  This is
/// needed because ssh does not support proxying of <c>XC-APPGROUP</c>
/// requests.</para>
/// </remarks>
public class EmbeddedApplication : InputOutputWidget
{
	// Internal state.
	private String program;
	private String args;
	private Process process;
	private String redirectDisplay;
	private String authorityFile;
	private XAppGroup group;
	private AppGroupWidget groupWrapper;
	private XWindow child;
	private bool closeEventSent;
	private bool closeNotifySent;
	private static bool errorReported;
	private static bool displayProbed;
	private static bool displayExists;

	/// <summary>
	/// <para>Constructs a new <see cref="T:Xsharp.EmbeddedApplication"/>
	/// instance.</para>
	/// </summary>
	///
	/// <param name="parent">
	/// <para>The parent of the new widget.</para>
	/// </param>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the top-left corner of
	/// the new widget.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the top-left corner of
	/// the new widget.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The width of the new window.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The height of the new window.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="parent"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if <paramref name="x"/>, <paramref name="y"/>,
	/// <paramref name="width"/>, or <paramref name="height"/> are
	/// out of range.</para>
	/// </exception>
	///
	/// <exception cref="T.Xsharp.XInvalidOperationException">
	/// <para>Raised if <paramref name="parent"/> is disposed, the
	/// root window, or an input-only window.</para>
	/// </exception>
	public EmbeddedApplication(Widget parent, int x, int y,
							   int width, int height)
			: this(parent, x, y, width, height, null, null)
			{
				// Nothing to do here.
			}

	/// <summary>
	/// <para>Constructs a new <see cref="T:Xsharp.EmbeddedApplication"/>
	/// instance.</para>
	/// </summary>
	///
	/// <param name="parent">
	/// <para>The parent of the new widget.</para>
	/// </param>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the top-left corner of
	/// the new widget.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the top-left corner of
	/// the new widget.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The width of the new window.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The height of the new window.</para>
	/// </param>
	///
	/// <param name="program">
	/// <para>The name of the program to execute.</para>
	/// </param>
	///
	/// <param name="args">
	/// <para>The command-line arguments to pass to the program.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="parent"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if <paramref name="x"/>, <paramref name="y"/>,
	/// <paramref name="width"/>, or <paramref name="height"/> are
	/// out of range.</para>
	/// </exception>
	///
	/// <exception cref="T.Xsharp.XInvalidOperationException">
	/// <para>Raised if <paramref name="parent"/> is disposed, the
	/// root window, or an input-only window.</para>
	/// </exception>
	public EmbeddedApplication(Widget parent, int x, int y,
							   int width, int height,
							   String program, String args)
			: base(parent, x, y, width, height)
			{
				this.program = program;
				this.args = args;
				SelectInput(EventMask.SubstructureNotifyMask);
			}

	/// <summary>
	/// <para>Get or set the name of the program to be executed.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The name of the program to be executed.</para>
	/// </value>
	public String Program
			{
				get
				{
					return program;
				}
				set
				{
					program = value;
				}
			}

	/// <summary>
	/// <para>Get or set the command-line arguments for the program to
	/// be executed.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The command-line arguments for the program to be executed.</para>
	/// </value>
	public String Arguments
			{
				get
				{
					return args;
				}
				set
				{
					args = value;
				}
			}

	/// <summary>
	/// <para>Get the process object for the embedded application.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The process object, or <see langword="null"/> if the
	/// application has not been launched yet.</para>
	/// </value>
	public Process Process
			{
				get
				{
					return process;
				}
			}

	// Called when the child window is destroyed.
	private void ChildDestroyed()
			{
				// Clear the child window and destroy the application group.
				child = XWindow.Zero;
				try
				{
					IntPtr d = dpy.Lock();
					if(group != XAppGroup.Zero)
					{
						Xlib.XagDestroyApplicationGroup(d, group);
						group = XAppGroup.Zero;
					}
				}
				finally
				{
					dpy.Unlock();
				}

				// Wait for the child process to exit properly.
				if(process != null && !(process.HasExited))
				{
					if(!process.WaitForExit(5000))
					{
						// Process is still alive!  Kill it the hard way.
						process.Kill();
						process = null;
					}
				}

				// Delete the authority file, which we no longer require.
				if(authorityFile != null)
				{
					File.Delete(authorityFile);
					authorityFile = null;
				}

				// Notify subclasses that the child was destroyed.
				if(!closeNotifySent)
				{
					closeNotifySent = true;
					OnClose();
				}
			}

	// Disassociate this widget instance and all of its children
	// from their X window handles, as the mirror copy in the X
	// server has been lost.
	internal override void Disassociate()
			{
				// Perform the basic disassociation tasks first.
				base.Disassociate();

				// Send a close message and destroy the application group.
				try
				{
					IntPtr d = dpy.Lock();
					if(child != XWindow.Zero && !closeEventSent)
					{
						closeEventSent = true;
						Xlib.XSharpSendClose(d, child);
						Xlib.XFlush(d);
					}
					if(group != XAppGroup.Zero)
					{
						Xlib.XagDestroyApplicationGroup(d, group);
						group = XAppGroup.Zero;
					}
				}
				finally
				{
					dpy.Unlock();
				}

				// Clean up the child process.
				ChildDestroyed();
			}

	/// <summary>
	/// <para>Close an embedded application.</para>
	/// </summary>
	///
	/// <remarks>
	/// <para>This sends a <c>WM_DELETE_WINDOW</c> event to the embedded
	/// application, or an <c>XKillClient</c> if the embedded application
	/// does not support <c>WM_DELETE_WINDOW</c>.</para>
	///
	/// <para>This is the recommended way to shut down an embedded
	/// application.  If the close fails, then <c>Terminate</c> will
	/// perform a hard shutdown of the child process.</para>
	/// </remarks>
	public void Close()
			{
				try
				{
					IntPtr display = dpy.Lock();
					if(child != XWindow.Zero && !closeEventSent)
					{
						closeEventSent = true;
						Xlib.XSharpSendClose(display, child);
						Xlib.XFlush(display);
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>This method is called when the client application window
	/// closes and the application is no longer running.</para>
	/// </summary>
	protected virtual void OnClose()
			{
				// Nothing to do here: normally overridden in subclasses.
			}

	/// <summary>
	/// <para>Terminate an embedded application.</para>
	/// </summary>
	///
	/// <remarks>
	/// <para>This sends a KILL signal to the child process, terminating it
	/// abruptly.</para>
	/// </remarks>
	public void Terminate()
			{
				if(process != null && !(process.HasExited))
				{
					process.Kill();
					process = null;
					ChildDestroyed();
				}
			}

	/// <summary>
	/// <para>Launch the embedded application.</para>
	/// </summary>
	public void Launch()
			{
				// Bail out if the application was already launched.
				if(this.process != null)
				{
					return;
				}

				// Create an application group in the X server, which is
				// used to redirect window create requests away from the
				// window manager and to us instead.
				CreateApplicationGroup();

				// Create a new process object.
				Process process = new Process();

				// Build the process start information.
				ProcessStartInfo info = process.StartInfo;
				if(program != null)
				{
					info.FileName = program;
				}
				if(args != null)
				{
					info.Arguments = args;
				}
				if(redirectDisplay != null)
				{
					info.EnvironmentVariables["DISPLAY"] = redirectDisplay;
				}
				if(authorityFile != null)
				{
					info.EnvironmentVariables["XAUTHORITY"] = authorityFile;
				}
				ModifyProcessStartInfo(info);

				// Launch the child process.
				process.Start();

				// Set the local process copy - we do this last just
				// in case "Start()" throws an exception.
				this.process = process;
			}

	/// <summary>
	/// <para>Modify the process start information just before launch.</para>
	/// </summary>
	///
	/// <param name="info">
	/// <para>The process start information to be modified.</para>
	/// </param>
	///
	/// <remarks>
	/// <para>This method is called from <c>Launch</c> just before the
	/// child process is started.  It gives subclasses a chance to modify
	/// the environment, redirect stdin/stdout, etc.</para>
	/// </remarks>
	protected virtual void ModifyProcessStartInfo(ProcessStartInfo info)
			{
				// Nothing to do here: overridden by subclasses.
			}

	/// <summary>
	/// <para>Determine if the X server can support embedding.</para>
	/// </summary>
	///
	/// <param name="dpy">
	/// <para>The display to check.</para>
	/// </param>
	///
	/// <returns>
	/// <para>Returns <see langword="true"/> if he display supports
	/// embedding, or <see langword="false"/> otherwise.</para>
	/// </returns>
	///
	/// <remarks>
	/// <para>If this method returns <see langword="false"/>, then
	/// calling <c>Launch</c> will cause the child application to be
	/// displayed in its own top-level window, rather than being
	/// embedded directly as a child widget.</para>
	/// </remarks>
	public static bool CanEmbed(Display dpy)
			{
				String displayName;
				if(dpy == null)
				{
					return false;
				}
				return CanEmbed(dpy, false, out displayName);
			}

	// Probe an X display to see if it is usable.
	private static bool ProbeDisplay(String displayName, bool reportErrors)
			{
				if(!displayProbed)
				{
					// Probe the new display to see if it can be used.
					displayProbed = true;
					IntPtr probe = Xlib.XOpenDisplay(displayName);
					if(probe == IntPtr.Zero)
					{
						if(reportErrors && !errorReported)
						{
							Console.Error.WriteLine
								("The X server at `{0}' is not " +
								 "accessible.  You may need",
								 displayName);
							Console.Error.WriteLine
								("to use `xhost +' to permit access.");
							errorReported = true;
						}
						displayExists = false;
						return false;
					}
					Xlib.XCloseDisplay(probe);
					displayExists = true;
				}
				return displayExists;
			}

	// Determine if the X server supports embedding - inner version.
	// "displayName" will be set to a non-null value if it is necessary
	// to redirect the DISPLAY environment variable elsewhere.
	private static bool CanEmbed(Display display, bool reportErrors,
								 out String displayName)
			{
				IntPtr dpy;
				Xlib.Xint major, minor;
				String client;
				int index;
				displayName = null;
				try
				{
					dpy = display.Lock();

					// See if the X server supports XC-APPGROUP and SECURITY.
					if(Xlib.XagQueryVersion(dpy, out major, out minor)
							== XBool.False)
					{
						if(reportErrors && !errorReported)
						{
							Console.Error.WriteLine
								("The X server `{0}' does not support the " +
								 "XC-APPGROUP extension,",
								 display.displayName);
							Console.Error.WriteLine
								("which is required for application " +
								 "embedding.");
							errorReported = true;
						}
						return false;
					}
					if(Xlib.XSecurityQueryExtension(dpy, out major, out minor)
							== XBool.False)
					{
						if(reportErrors && !errorReported)
						{
							Console.Error.WriteLine
								("The X server `{0}' does not support the " +
								 "SECURITY extension,",
								 display.displayName);
							Console.Error.WriteLine
								("which is required for for application " +
								 "embedding.");
							errorReported = true;
						}
						return false;
					}

					// If we are in an ssh shell account, then we cannot
					// connect via ssh's X11 forwarding mechanism as it
					// does not know how to proxy appgroup security tokens.
					// Try to discover where the ssh client actually lives.
					displayName = Environment.GetEnvironmentVariable
							("XREALDISPLAY");
					client = Environment.GetEnvironmentVariable("SSH_CLIENT");
					if(displayName != null && displayName.Length > 0)
					{
						// The user specified a display override with
						// the XREALDISPLAY environment variable.
						if(!ProbeDisplay(displayName, reportErrors))
						{
							displayName = null;
							return false;
						}
					}
					else if(client != null && client.Length > 0)
					{
						// Synthesize a display name from the ssh client name.
						index = client.IndexOf(' ');
						if(index == -1)
						{
							index = client.Length;
						}
						displayName = client.Substring(0, index) + ":0.0";
						if(!ProbeDisplay(displayName, reportErrors))
						{
							displayName = null;
							return false;
						}
					}
					else if(Environment.GetEnvironmentVariable("SSH_ASKPASS")
								!= null ||
					   	    Environment.GetEnvironmentVariable("SSH_TTY")
								!= null)
					{
						// Older versions of bash do not export SSH_CLIENT
						// within an ssh login session.
						if(reportErrors && !errorReported)
						{
							Console.Error.WriteLine
								("The `SSH_CLIENT' environment variable " +
								 "is not exported from the shell.");
							Console.Error.WriteLine
								("Either export `SSH_CLIENT' or set the " +
								 "`XREALDISPLAY' environment");
							Console.Error.WriteLine
								("variable to the name of the real " +
								 "X display.");
							errorReported = true;
						}
						displayName = null;
						return false;
					}
					else
					{
						// No ssh, so use the original "DISPLAY" value as-is.
						displayName = null;
					}
				}
				catch(MissingMethodException)
				{
					displayName = null;
					return false;
				}
				catch(DllNotFoundException)
				{
					displayName = null;
					return false;
				}
				catch(EntryPointNotFoundException)
				{
					displayName = null;
					return false;
				}
				finally
				{
					display.Unlock();
				}
				return true;
			}

	// Create a new application group for embedding a child application.
	private unsafe void CreateApplicationGroup()
			{
				String displayName;
				Xauth *auth;
				Xauth *authReturn;
				XSecurityAuthorizationAttributes xsa;
				Xlib.XSecurityAuthorization xs;

				// Check that we can create application groups.
				if(!CanEmbed(dpy, true, out displayName))
				{
					return;
				}
				try
				{
					// Lock down the display while we do this.
					IntPtr display = dpy.Lock();

					// Create the application group identifier.
					if(Xlib.XagCreateEmbeddedApplicationGroup
							(display, XVisualID.Zero,
							 Xlib.XDefaultColormapOfScreen(screen.screen),
							 Xlib.XBlackPixelOfScreen(screen.screen),
							 Xlib.XWhitePixelOfScreen(screen.screen),
							 out group) == XStatus.Zero)
					{
						return;
					}

					// Generate an authentication token for the group.
					auth = Xlib.XSecurityAllocXauth();
					if(auth == null)
					{
						return;
					}
					auth->name = Marshal.StringToHGlobalAnsi
						("MIT-MAGIC-COOKIE-1");
					auth->name_length = 18;
					xsa = new XSecurityAuthorizationAttributes();
					xsa.timeout = 300;
					xsa.trust_level = 0;	// XSecurityClientTrusted
					xsa.group = (XID)group;
					xsa.event_mask = 0;
					authReturn = Xlib.XSecurityGenerateAuthorization
						(display, auth,
						 (uint)(XSecurityAttributeMask.XSecurityTimeout |
						 	    XSecurityAttributeMask.XSecurityTrustLevel |
						 	    XSecurityAttributeMask.XSecurityGroup),
						 ref xsa, out xs);
					if(authReturn == null)
					{
						Xlib.XSecurityFreeXauth(auth);
						return;
					}

					// Write the credentials to a temporary X authority file.
					String authFile = Path.GetTempFileName();
					FileStream stream = new FileStream
						(authFile, FileMode.Create, FileAccess.Write);
					WriteShort(stream, 65535);	// family = FamilyWild
					WriteShort(stream, 0);		// address_length
					WriteShort(stream, 0);		// number_length
					WriteShort(stream, authReturn->name_length);
					WriteBytes(stream, authReturn->name,
							   authReturn->name_length);
					WriteShort(stream, authReturn->data_length);
					WriteBytes(stream, authReturn->data,
							   authReturn->data_length);
					stream.Close();

					// Free the Xauth structures that we don't need any more.
					Xlib.XSecurityFreeXauth(auth);
					Xlib.XSecurityFreeXauth(authReturn);

					// Record the app group information.
					redirectDisplay = displayName;
					authorityFile = authFile;

					// Create a wrapper around the appgroup to get events.
					groupWrapper = new AppGroupWidget(dpy, screen, group, this);
				}
				catch(MissingMethodException)
				{
					return;
				}
				catch(DllNotFoundException)
				{
					return;
				}
				catch(EntryPointNotFoundException)
				{
					return;
				}
				finally
				{
					dpy.Unlock();
				}
			}

	// Write a 16-bit short value to a stream.
	private static void WriteShort(Stream stream, int value)
			{
				stream.WriteByte((byte)(value >> 8));
				stream.WriteByte((byte)value);
			}

	// Write a buffer of data to a stream.
	private static void WriteBytes(Stream stream, IntPtr data, int len)
			{
				int offset;
				for(offset = 0; offset < len; ++offset)
				{
					stream.WriteByte(Marshal.ReadByte(data, offset));
				}
			}

	// Dispatch an event to this widget.
	internal override void DispatchEvent(ref XEvent xevent)
			{
				if (xevent.type == Xsharp.Events.EventType.DestroyNotify &&
				   xevent.xdestroywindow.window == child)
				{
					// The child window has been destroyed.
					ChildDestroyed();
				}
				base.DispatchEvent(ref xevent);
			}

	/// <summary>
	/// <para>Method that is called when the widget is moved to a
	/// new position or given a new size.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the new top-left widget corner.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the new top-left widget corner.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The new width for the widget.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The new width for the widget.</para>
	/// </param>
	protected override void OnMoveResize(int x, int y, int width, int height)
			{
				if(child != XWindow.Zero)
				{
					// Resize the embedded child window to match the
					// dimensions of the embedding parent window.
					try
					{
						IntPtr display = dpy.Lock();
						Xlib.XResizeWindow
							(display, child, (uint)width, (uint)height);
					}
					finally
					{
						dpy.Unlock();
					}
				}
			}

	// Simple widget handler for intercepting events from an app group.
	// Whenever a top-level window operation occurs in the embedded
	// application, it first sends the event to us, and then to the root
	// window if we didn't want it.
	private sealed class AppGroupWidget : Widget
	{
		// Internal state.
		private EmbeddedApplication embedParent;

		// Constructor.
		public AppGroupWidget(Display dpy, Screen screen, XAppGroup group,
							  EmbeddedApplication parent)
				: base(dpy, screen, DrawableKind.Widget, null)
				{
					embedParent = parent;
					handle = (XDrawable)group;
					dpy.handleMap[(XWindow)handle] = this;
				}

		// Determine if we want a particular widget in a map request.
		private bool WantThisWindow(IntPtr dpy, XWindow window)
				{
					// Bail out if the parent already has an embedded child.
					if(embedParent.child != XWindow.Zero)
					{
						return false;
					}

					// Ignore the window if it is a transient, because
					// we don't want dialog boxes that are displayed before
					// the main window to get accidentally reparented.
					XWindow transientFor;
					if(Xlib.XGetTransientForHint(dpy, window, out transientFor)
							!= XStatus.Zero)
					{
						// KDE apps that act like a top-level dialog
						// (kcalc, kfind, etc) specific the root window
						// as their parent.  We assume that such windows
						// are actually the main window of the app.
						if(transientFor !=
								Xlib.XRootWindowOfScreen(screen.screen))
						{
							return false;
						}
					}

					// We want this widget.
					return true;
				}

		// Dispatch an event to this widget.
		internal override void DispatchEvent(ref XEvent xevent)
				{
					IntPtr display;
					XWindow child;
					if (xevent.type == Xsharp.Events.EventType.MapRequest)
					{
						// This may be notification of a new window
						// that we need to take control of.
						try
						{
							display = dpy.Lock();
							child = xevent.xmaprequest.window;
							if(WantThisWindow(display, child))
							{
								// This is the top-level child window that
								// we have been waiting for.
								XWindowChanges wc = new XWindowChanges();
								wc.width = embedParent.Width;
								wc.height = embedParent.Height;
								wc.border_width = 0;
								Xlib.XConfigureWindow
									(display, child,
									 (int)(ConfigureWindowMask.CWWidth |
									 	   ConfigureWindowMask.CWHeight |
									 	   ConfigureWindowMask.CWBorderWidth),
									 ref wc);
								Xlib.XReparentWindow
									(display, child,
									 embedParent.GetWidgetHandle(), 0, 0);
								Xlib.XMapWindow(display, child);
								embedParent.child = child;
							}
							else
							{
								// We don't want this window, or we already
								// know about it, so replay the map request.
								Xlib.XMapWindow(display, child);
							}
						}
						finally
						{
							dpy.Unlock();
						}
					}
					else if (xevent.type == Xsharp.Events.EventType.ConfigureRequest)
					{
						// Replay the configure event direct to the X server.
						XWindowChanges wc = new XWindowChanges();
						wc.x = xevent.xconfigurerequest.x;
						wc.y = xevent.xconfigurerequest.y;
						wc.width = xevent.xconfigurerequest.width;
						wc.height = xevent.xconfigurerequest.height;
						wc.border_width = xevent.xconfigurerequest.border_width;
						wc.sibling = xevent.xconfigurerequest.above;
						wc.stack_mode = xevent.xconfigurerequest.detail;
						try
						{
							display = dpy.Lock();
							Xlib.XConfigureWindow
								(display,
								 xevent.xconfigurerequest.window,
								 xevent.xconfigurerequest.value_mask, ref wc);
						}
						finally
						{
							dpy.Unlock();
						}
					}
					base.DispatchEvent(ref xevent);
				}

	}; // class AppGroupWidget

} // class EmbeddedApplication

#endif // CONFIG_EXTENDED_DIAGNOSTICS

} // namespace Xsharp
