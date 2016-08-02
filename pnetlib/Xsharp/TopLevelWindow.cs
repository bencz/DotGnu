/*
 * TopLevelWindow.cs - Widget handling for top-level application windows.
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
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Xsharp.Types;
using Xsharp.Events;
using OpenSystem.Platform.X11;

/// <summary>
/// <para>The <see cref="T:Xsharp.TopLevelWindow"/> class manages
/// top-level application windows.</para>
/// </summary>
public class TopLevelWindow : InputOutputWidget
{
	// Internal state.
	private String name;
	internal bool iconic;
	internal bool maximized;
	internal bool hasPrimaryFocus;
	internal bool reparented;
	internal bool reparentedNeedMove;
	internal bool sticky;
	internal bool shaded;
	internal bool hidden;
	internal bool fullScreen;
	private bool firstMapDone;
	private IntPtr keyBuffer;
	internal InputOnlyWidget focusWidget;
	private InputOnlyWidget defaultFocus;
	private MotifDecorations decorations;
	private MotifFunctions functions;
	private MotifInputType inputType;
	private OtherHints otherHints;
	private TopLevelWindow transientFor;
	private Timer resizeTimer;
	private int expectedWidth, expectedHeight;
	internal int minWidth, minHeight;
	internal int maxWidth, maxHeight;
	private Image icon;
	internal MdiClientWidget mdiClient;

	/// <summary>
	/// <para>Constructs a new <see cref="T:Xsharp.TopLevelWindow"/>
	/// instance.</para>
	/// </summary>
	///
	/// <param name="name">
	/// <para>The initial name to display in the title bar.</para>
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
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if <paramref name="width"/> or <paramref name="height"/>
	/// is out of range.</para>
	/// </exception>
	///
	/// <remarks>
	/// <para>The new top-level window will be created on the default
	/// screen of the primary display.  If the primary display is
	/// not open, this constructor will open it.</para>
	/// </remarks>
	public TopLevelWindow(String name, int width, int height)
			: this(null, name, width, height)
			{
				// Nothing to do here.
			}

	/// <summary>
	/// <para>Constructs a new <see cref="T:Xsharp.TopLevelWindow"/>
	/// instance.</para>
	/// </summary>
	///
	/// <param name="screen">
	/// <para>The screen to display the new top-level window on, or
	/// <see langword="null"/> to use the default screen of the
	/// primary display.</para>
	/// </param>
	///
	/// <param name="name">
	/// <para>The initial name to display in the title bar.  If this
	/// is <see langword="null"/> then the empty string will be used.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The width of the new widget.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The height of the new widget.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if <paramref name="width"/> or <paramref name="height"/>
	/// is out of range.</para>
	/// </exception>
	public TopLevelWindow(Screen screen, String name, int width, int height)
			: this(GetRoot(screen), name, 0, 0, width, height)
			{
				// Nothing to do here.
			}

	// Internal constructor, that can create a top-level window on either
	// the root window or underneath an MDI client parent.
	public TopLevelWindow(Widget parent, String name,
						  int x, int y, int width, int height)
			: base(parent, x, y, width, height,
			       new Color(StandardColor.Foreground),
			       new Color(StandardColor.Background),
				   true, false)
			{
				// Check the parent.
				if(!(parent is RootWindow) && !(parent is CaptionWidget))
				{
					throw new XInvalidOperationException();
				}

				// Initialize this object's state.
				this.name = ((name != null) ? name : String.Empty);
				this.iconic = false;
				this.maximized = false;
				this.hasPrimaryFocus = false;
				this.reparented = false;
				this.reparentedNeedMove = false;
				this.sticky = false;
				this.shaded = false;
				this.hidden = false;
				this.fullScreen = false;
				this.keyBuffer = IntPtr.Zero;
				this.focusWidget = this;
				this.defaultFocus = null;
				this.decorations = MotifDecorations.All;
				this.functions = MotifFunctions.All;
				this.inputType = MotifInputType.Normal;
				this.transientFor = null;
				this.resizeTimer = null;
				this.expectedWidth = -1;
				this.expectedHeight = -1;

				// Top-level widgets receive all key and focus events.
				SelectInput(EventMask.KeyPressMask |
							EventMask.KeyReleaseMask |
							EventMask.FocusChangeMask |
							EventMask.StructureNotifyMask |
							EventMask.PropertyChangeMask);

				// We don't use WM properties if the parent is a CaptionWidget.
				if(parent is CaptionWidget)
				{
					return;
				}

				// Set the initial WM properties.
				try
				{
					// Lock down the display and get the window handle.
					IntPtr display = dpy.Lock();
					XWindow handle = GetWidgetHandle();

					// Make this the group leader if we don't have one yet.
					bool isFirst = false;
					if(dpy.groupLeader == XWindow.Zero)
					{
						dpy.groupLeader = handle;
						isFirst = true;
					}

					// Set the WM_CLASS hint.
					Application app = Application.Primary;
					if(app != null)
					{
						XClassHint classHint = new XClassHint();
						classHint.res_name = app.ResourceName;
						classHint.res_class = app.ResourceClass;
						Xlib.XSetClassHint(display, handle, ref classHint);
						classHint.Free();
					}

					// Set the title bar and icon names.
					SetWindowName(display, handle, this.name);

					// Ask for "WM_DELETE_WINDOW" and "WM_TAKE_FOCUS".
					SetProtocols(display, handle);

					// Set the window hints.
					if(isFirst && app != null && app.StartIconic)
					{
						// The user supplied "-iconic" on the command-line.
						iconic = true;
					}
					SetWMHints(display, handle);

					// Set some other string properties.
					String cultureName = CultureInfo.CurrentCulture.Name;
					if(cultureName == null || (cultureName.Length == 0))
					{
						cultureName = "en_US";
					}
					else
					{
						cultureName = cultureName.Replace("-", "_");
					}
					SetTextProperty(display, handle, "WM_LOCALE_NAME",
									cultureName);
					String hostname = Application.Hostname;
					if(hostname != null)
					{
						SetTextProperty(display, handle,
										"WM_CLIENT_MACHINE", hostname);
					}
					if(isFirst)
					{
						String[] args = Environment.GetCommandLineArgs();
						if(args != null && args.Length > 0)
						{
							// We put "ilrun" at the start of the command,
							// because the command needs to be in a form
							// that can be directly executed by fork/exec,
							// and IL binaries normally aren't in this form.
							String[] newArgs = new String [args.Length + 1];
							newArgs[0] = "ilrun";
							Array.Copy(args, 0, newArgs, 1, args.Length);
							SetTextProperty(display, handle,
											"WM_COMMAND", newArgs);
						}
					}

				#if CONFIG_EXTENDED_DIAGNOSTICS
					// Put the process ID on the window.
					int pid = Process.GetCurrentProcess().Id;
					if(pid != -1 && pid != 0)
					{
						Xlib.XChangeProperty
							(display, handle,
							 Xlib.XInternAtom(display, "_NET_WM_PID",
							 				  XBool.False),
							 Xlib.XInternAtom(display, "CARDINAL",
							 				  XBool.False),
							 32, 0 /* PropModeReplace */,
							 new Xlib.Xlong [] {(Xlib.Xlong)(pid)}, 1);
					}
				#endif
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Destroy an instance of <see cref="T:Xsharp.TopLevelWindow"/>.
	/// </para>
	/// </summary>
	~TopLevelWindow()
			{
				if(keyBuffer != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(keyBuffer);
					keyBuffer = IntPtr.Zero;
				}
			}

	// Helper method to get the root window of a specified screen.
	internal static Widget GetRoot(Screen screen)
			{
				if(screen == null)
				{
					return Xsharp.Application.Primary
								.Display.DefaultRootWindow;
				}
				else
				{
					return screen.RootWindow;
				}
			}

	// Set the WM protocols for this window.
	private void SetProtocols(IntPtr display, XWindow handle)
			{
				XAtom[] protocols = new XAtom [4];
				int numProtocols = 0;
				protocols[numProtocols++] = dpy.wmDeleteWindow;
				protocols[numProtocols++] = dpy.wmTakeFocus;
				if((otherHints & OtherHints.HelpButton) != 0)
				{
					protocols[numProtocols++] = dpy.wmContextHelp;
				}
				protocols[numProtocols++] = dpy.wmPing;
				Xlib.XSetWMProtocols(display, handle, protocols, numProtocols);
			}

	// Set the window name hints.
	private void SetWindowName(IntPtr display, XWindow handle, String name)
			{
				// Set the ICCCM name hints.
				Xlib.XStoreName(display, handle, name);
				Xlib.XSetIconName(display, handle, name);

				// Set the new-style name hints, in UTF-8.  These are more
				// likely to be rendered properly by newer window managers.
				XAtom utf8String = Xlib.XInternAtom
					(display, "UTF8_STRING", XBool.False);
				XAtom wmName = Xlib.XInternAtom
					(display, "_NET_WM_NAME", XBool.False);
				XAtom wmIconName = Xlib.XInternAtom
					(display, "_NET_WM_ICON_NAME", XBool.False);
				byte[] bytes = Encoding.UTF8.GetBytes(name);
				Xlib.XChangeProperty
					(display, handle, wmName, utf8String,
					 8, 0 /* PropModeReplace */, bytes, bytes.Length);
				Xlib.XChangeProperty
					(display, handle, wmIconName, utf8String,
					 8, 0 /* PropModeReplace */, bytes, bytes.Length);
			}

	// Set the XWMHints structure on this window.
	private void SetWMHints(IntPtr display, XWindow handle)
			{
				XWMHints hints = new XWMHints();
				hints.flags = WMHintsMask.InputHint |
							  WMHintsMask.StateHint |
							  WMHintsMask.WindowGroupHint;
				hints.input = true;
				hints.initial_state = (iconic ? WindowState.IconicState
											  : WindowState.NormalState);
				hints.window_group = (XID)(dpy.groupLeader);
				if(icon != null)
				{
					Pixmap pixmap = icon.Pixmap;
					Bitmap mask = icon.Mask;
					if(mask != null)
					{
						hints.flags |= WMHintsMask.IconPixmapHint |
									   WMHintsMask.IconMaskHint;
						hints.icon_pixmap = pixmap.GetPixmapHandle();
						hints.icon_mask = mask.GetPixmapHandle();
					}
					else
					{
						hints.flags |= WMHintsMask.IconPixmapHint;
						hints.icon_pixmap = pixmap.GetPixmapHandle();
					}
				}
				Xlib.XSetWMHints(display, handle, ref hints);
			}

	// Set a text property hint on this window.
	private void SetTextProperty(IntPtr display, XWindow handle,
								 String property, String value)
			{
				XTextProperty textprop = new XTextProperty();
				if(textprop.SetText(value))
				{
					Xlib.XSetTextProperty
						(display, handle, ref textprop,
						 Xlib.XInternAtom(display, property, XBool.False));
					textprop.Free();
				}
			}
	private void SetTextProperty(IntPtr display, XWindow handle,
								 String property, String[] value)
			{
				XTextProperty textprop = new XTextProperty();
				if(textprop.SetText(value))
				{
					Xlib.XSetTextProperty
						(display, handle, ref textprop,
						 Xlib.XInternAtom(display, property, XBool.False));
					textprop.Free();
				}
			}

	// Set the "_NET_WM_STATE" property, to include extended state requests.
	private void SetNetState(IntPtr display, XWindow handle)
			{
				Xlib.Xlong[] atoms = new Xlib.Xlong [8];
				int numAtoms = 0;

				// Determine if the window should be hidden from the taskbar.
				if((otherHints & OtherHints.HideFromTaskBar) != 0)
				{
					atoms[numAtoms++] =
						(Xlib.Xlong)Xlib.XInternAtom
							(display, "_NET_WM_STATE_SKIP_TASKBAR",
							 XBool.False);

					atoms[numAtoms++] =
						(Xlib.Xlong)Xlib.XInternAtom
							(display, "_NET_WM_STATE_SKIP_PAGER",
							 XBool.False);
				}

				// Determine if the window should be made top-most on-screen.
				if((otherHints & OtherHints.TopMost) != 0)
				{
					atoms[numAtoms++] =
						(Xlib.Xlong)Xlib.XInternAtom
							(display, "_NET_WM_STATE_ABOVE",
						     XBool.False);
				}

				// Determine if we should stick in a fixed position
				if((otherHints & OtherHints.Sticky) != 0)
				{
					atoms[numAtoms++] =
						(Xlib.Xlong)Xlib.XInternAtom
							(display, "_NET_WM_STATE_STICKY",
						    XBool.False);
				}

				// Determine if we should shade
				if((otherHints & OtherHints.Shaded) != 0)
				{
					atoms[numAtoms++] =
						(Xlib.Xlong)Xlib.XInternAtom
							(display, "_NET_WM_STATE_SHADED",
						    XBool.False);
				}

				// Determine if we should hide
				if((otherHints & OtherHints.Hidden) != 0)
				{
					atoms[numAtoms++] =
						(Xlib.Xlong)Xlib.XInternAtom
							(display, "_NET_WM_STATE_HIDDEN",
						    XBool.False);
				}

				// Determine if we should go full screen
				if((otherHints & OtherHints.FullScreen) != 0)
				{
					atoms[numAtoms++] =
						(Xlib.Xlong)Xlib.XInternAtom
							(display, "_NET_WM_STATE_FULLSCREEN",
						    XBool.False);
				}

				// Determine if the window should be maximized by default.
				if(maximized)
				{
					atoms[numAtoms++] =
						(Xlib.Xlong)Xlib.XInternAtom
							(display, "_NET_WM_STATE_MAXIMIZED_VERT",
						     XBool.False);
					atoms[numAtoms++] =
						(Xlib.Xlong)Xlib.XInternAtom
							(display, "_NET_WM_STATE_MAXIMIZED_HORZ",
						     XBool.False);
				}

				// Update the "_NET_WM_STATE" property as appropriate.
				XAtom type = Xlib.XInternAtom
					(display, "ATOM", XBool.False);
				if(numAtoms > 0)
				{
					Xlib.XChangeProperty
						(display, handle, dpy.wmNetState, type,
						 32, 0 /* PropModeReplace */, atoms, numAtoms);
				}
				else
				{
					Xlib.XDeleteProperty(display, handle, dpy.wmNetState);
				}
			}

	// Construct the XSizeHints structure for this window.
	private XSizeHints BuildSizeHints(int x, int y, int width, int height)
			{
				XSizeHints hints = new XSizeHints();
				if(x != 0 || y != 0)
				{
					hints.flags = SizeHintsMask.USPosition |
								  SizeHintsMask.USSize;
					hints.x = x;
					hints.y = y;
				}
				else
				{
					hints.flags = SizeHintsMask.USSize;
				}
				hints.width = width;
				hints.height = height;
				if(minWidth != 0 || minHeight != 0)
				{
					hints.flags |= SizeHintsMask.PMinSize;
					hints.min_width = minWidth;
					hints.min_height = minHeight;
				}
				if(maxWidth != 0 || maxWidth != 0)
				{
					hints.flags |= SizeHintsMask.PMaxSize;
					hints.max_width = maxWidth;
					hints.max_height = maxHeight;
				}
				return hints;
			}
	private XSizeHints BuildSizeHints()
			{
				return BuildSizeHints(x, y, width, height);
			}

	// Perform a MoveResize request.
	internal override void PerformMoveResize
				(IntPtr display, int newX, int newY,
				 int newWidth, int newHeight)
			{
				// If our parent is a caption widget, then let it
				// handle the move/resize operation so that the
				// borders can be adjusted to match the request.
				if(Parent is CaptionWidget)
				{
					((CaptionWidget)Parent).PerformChildResize
						(newX, newY, newWidth, newHeight);
					return;
				}

				XWindow handle = GetWidgetHandle();
				XWindowChanges changes = new XWindowChanges();
				ConfigureWindowMask mask = (ConfigureWindowMask)0;

				// If we haven't mapped the window to the screen yet,
				// then set the size hints and bail out with a normal
				// move/resize event.
				if(!firstMapDone)
				{
					XSizeHints hints = BuildSizeHints
						(newX, newY, newWidth, newHeight);
					Xlib.XSetWMNormalHints(display, handle, ref hints);
					if(newWidth != width || newHeight != height)
					{
						expectedWidth = newWidth;
						expectedHeight = newHeight;
					}
					base.PerformMoveResize
						(display, newX, newY, newWidth, newHeight);
					return;
				}

				// Collect up the changes that need to be performed.
				if(newX != x || newY != y)
				{
					if(newWidth != width || newHeight != height)
					{
						changes.x = newX;
						changes.y = newY;
						changes.width = newWidth;
						changes.height = newHeight;
						expectedWidth = newWidth;
						expectedHeight = newHeight;
						mask = ConfigureWindowMask.CWX |
							   ConfigureWindowMask.CWY |
							   ConfigureWindowMask.CWWidth |
							   ConfigureWindowMask.CWHeight;
					}
					else
					{
						changes.x = newX;
						changes.y = newY;
						mask = ConfigureWindowMask.CWX |
							   ConfigureWindowMask.CWY;
					}
				}
				else if(newWidth != width || newHeight != height)
				{
					changes.width = newWidth;
					changes.height = newHeight;
					expectedWidth = newWidth;
					expectedHeight = newHeight;
					mask = ConfigureWindowMask.CWWidth |
						   ConfigureWindowMask.CWHeight;
				}

				// Send the reconfiguration request to the window manager.
				if(mask != (ConfigureWindowMask)0)
				{
					Xlib.XReconfigureWMWindow
							(display, handle,
							 Screen.ScreenNumber,
						     (uint)mask,
							 ref changes);
				}
			}

	/// <summary>
	/// <para>Determine if this widget is currently iconified.</para>
	/// </summary>
	///
	/// <value>
	/// <para>Returns <see langword="true"/> if the widget is iconified;
	/// <see langword="false"/> otherwise.</para>
	/// </value>
	///
	/// <remarks>
	/// <para>Setting this property is equivalent to calling either
	/// <c>Iconify</c> or <c>Deiconify</c>.</para>
	/// </remarks>
	public bool IsIconic
			{
				get
				{
					return iconic;
				}
				set
				{
					if(value)
					{
						Iconify();
					}
					else
					{
						Deiconify();
					}
				}
			}

	/// <summary>
	/// <para>Determine if this widget is currently maximized.</para>
	/// </summary>
	///
	/// <value>
	/// <para>Returns <see langword="true"/> if the widget is maximized;
	/// <see langword="false"/> otherwise.</para>
	/// </value>
	///
	/// <remarks>
	/// <para>Setting this property is equivalent to calling either
	/// <c>Maximize</c> or <c>Restore</c>.</para>
	/// </remarks>
	public bool IsMaximized
			{
				get
				{
					return maximized;
				}
				set
				{
					if(value)
					{
						Maximize();
					}
					else
					{
						Restore();
					}
				}
			}

	/// <summary>
	/// <para>Map this widget to the screen.</para>
	/// </summary>
	public override void Map()
			{
				try
				{
					IntPtr display = dpy.Lock();
					if(!mapped)
					{
						// Use "XMapRaised" to notify the window manager
						// that we want to be brought to the top.
						if(Caption((firstMapDone
										? CaptionWidget.Operation.Map
										: CaptionWidget.Operation.FirstMap)))
						{
							Xlib.XMapRaised(display, GetWidgetHandle());
						}
						mapped = true;
						firstMapDone = true;
						OnMapStateChanged();
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Unmap this widget from the screen.</para>
	/// </summary>
	public override void Unmap()
			{
				try
				{
					IntPtr display = dpy.Lock();
					if(mapped)
					{
						// Send a "withdraw" message to the window manager,
						// which will take care of unmapping the window for us.
						if(Caption(CaptionWidget.Operation.Unmap))
						{
							Xlib.XWithdrawWindow
								(display, GetWidgetHandle(),
								 screen.ScreenNumber);
						}
						mapped = false;
						OnMapStateChanged();
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Reparenting is not supported for top-level windows.</para>
	/// </summary>
	public override void Reparent(Widget newParent, int x, int y)
			{
				throw new XInvalidOperationException
					(S._("X_NonTopLevelOperation"));
			}

	/// <summary>
	/// <para>Iconify this window.</para>
	/// </summary>
	public virtual void Iconify()
			{
				if(!Caption(CaptionWidget.Operation.Iconify))
				{
					OnIconicStateChanged(iconic);
					return;
				}
				try
				{
					IntPtr display = dpy.Lock();
					if(!iconic)
					{
						if(firstMapDone)
						{
							// Send an "iconify" message to the window manager,
							// which will take care of iconifying the window.
							Xlib.XIconifyWindow
								(display, GetWidgetHandle(),
								 screen.ScreenNumber);
							iconic = true;
							OnIconicStateChanged(true);
						}
						else
						{
							// We haven't been mapped for the first time yet,
							// so merely update the WM_HINTS structure.
							iconic = true;
							SetWMHints(display, GetWidgetHandle());
							OnIconicStateChanged(true);
						}
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>De-iconify this window.</para>
	/// </summary>
	public virtual void Deiconify()
			{
				if(!Caption(CaptionWidget.Operation.Deiconify))
				{
					OnIconicStateChanged(iconic);
					return;
				}
				try
				{
					IntPtr display = dpy.Lock();
					if(iconic)
					{
						// Use "XMapRaised" to notify the window manager
						// that we want to be de-iconified.
						iconic = false;
						if(!firstMapDone)
						{
							// Switch the WM_HINTS back to say "NormalState".
							SetWMHints(display, GetWidgetHandle());
						}
						Xlib.XMapRaised(display, GetWidgetHandle());
						OnIconicStateChanged(false);
						if(!mapped)
						{
							// We are mapped now as well.
							mapped = true;
							firstMapDone = true;
							OnMapStateChanged();
						}
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	// Send a maximize or restore message to the window manager.
	private void SendMaximizeMessage
				(IntPtr display, XWindow handle, bool maximize)
			{
				XEvent xevent = new XEvent();
				xevent.xany.type = (int)(Xsharp.Events.EventType.ClientMessage);
				xevent.xany.window = handle;
				xevent.xclient.message_type = Xlib.XInternAtom
					(display, "_NET_WM_STATE", XBool.False);
				xevent.xclient.format = 32;
				if(maximize)
				{
					xevent.xclient.setl(0, 1 /* _NET_WM_STATE_ADD */ );
				}
				else
				{
					xevent.xclient.setl(0, 0 /* _NET_WM_STATE_REMOVE */ );
				}
				XAtom atom1 = Xlib.XInternAtom
					(display, "_NET_WM_STATE_MAXIMIZED_VERT", XBool.False);
				XAtom atom2 = Xlib.XInternAtom
					(display, "_NET_WM_STATE_MAXIMIZED_HORZ", XBool.False);
				xevent.xclient.setl(1, (int)atom1);
				xevent.xclient.setl(2, (int)atom2);
				Xlib.XSendEvent
					(display, screen.RootWindow.GetWidgetHandle(),
					 XBool.False, (int)(EventMask.NoEventMask),
					 ref xevent);
			}

	/// <summary>
	/// <para>Maximize this window.</para>
	/// </summary>
	public virtual void Maximize()
			{
				if(!Caption(CaptionWidget.Operation.Maximize))
				{
					OnMaximizedStateChanged(maximized);
					return;
				}
				try
				{
					IntPtr display = dpy.Lock();
					if(!maximized)
					{
						if(firstMapDone)
						{
							// Send a "maximize" message to the window manager,
							// which will take care of maximizing the window.
							// Not all window managers support this message.
							SendMaximizeMessage
								(display, GetWidgetHandle(), true);
							maximized = true;
							OnMaximizedStateChanged(true);
						}
						else
						{
							// We haven't been mapped for the first time yet,
							// so merely update the _NET_WM_STATE hint.
							maximized = true;
							SetNetState(display, GetWidgetHandle());
							OnMaximizedStateChanged(true);
						}
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Restore this window from the maximized state.</para>
	/// </summary>
	public virtual void Restore()
			{
				if(!Caption(CaptionWidget.Operation.Restore))
				{
					OnMaximizedStateChanged(maximized);
					return;
				}
				try
				{
					IntPtr display = dpy.Lock();
					if(maximized)
					{
						if(firstMapDone)
						{
							// Send a "restore" message to the window manager,
							// which will take care of restoring the window.
							// Not all window managers support this message.
							SendMaximizeMessage
								(display, GetWidgetHandle(), false);
							maximized = false;
							OnMaximizedStateChanged(false);
						}
						else
						{
							// We haven't been mapped for the first time yet,
							// so merely update the _NET_WM_STATE hint.
							maximized = false;
							SetNetState(display, GetWidgetHandle());
							OnMaximizedStateChanged(false);
						}
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Activate the help feature on this window.</para>
	/// </summary>
	public virtual void Help()
			{
				OnHelp();
			}

	/// <summary>
	/// <para>Destroy this widget if it is currently active.</para>
	/// </summary>
	public override void Destroy()
			{
				if(Caption(CaptionWidget.Operation.Destroy))
				{
					base.Destroy();
				}
			}

	/// <summary>
	/// <para>Close this window, as if the user had clicked the close box.
	/// </para>
	/// </summary>
	///
	/// <returns>
	/// <para>Returns <see langword="true"/> if the window was destroyed,
	/// or <see langword="false"/> if the window is still active
	/// because the <c>OnClose</c> method returned
	/// <see langword="false"/>.</para>
	/// </returns>
	public virtual bool Close()
			{
				// Bail out if the window has already been destroyed.
				if(handle == XDrawable.Zero)
				{
					return true;
				}

				// Ask the "OnClose" method if we can close or not.
				if(!OnClose())
				{
					return false;
				}

				// Destroy the window.
				Destroy();

				// If this was the last undestroyed top-level window
				// that was still mapped, then quit the application.
				if( !(Parent is CaptionWidget))
				{
					Widget child = null;
					if( null != Parent) child = Parent.TopChild;
					TopLevelWindow tchild;
					bool sawActive = false;
					while(child != null)
					{
						tchild = (child as TopLevelWindow);
						if(tchild != null)
						{
							if(tchild.mapped &&
							   tchild.handle != XDrawable.Zero)
							{
								sawActive = true;
								break;
							}
						}
						child = child.NextBelow;
					}
					if(!sawActive)
					{
						dpy.Quit();
					}
				}

				// The window no longer exists.
				return true;
			}

	/// <summary>
	/// <para>Get or set the name in the window's title bar.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The value of this property is the name to display in
	/// the title bar.  If the value is set to <see langword="null"/>,
	/// then the empty string will be used.</para>
	/// </value>
	public virtual String Name
			{
				get
				{
					return name;
				}
				set
				{
					if(name != value)
					{
						name = ((value != null) ? value : String.Empty);
						if(!Caption(CaptionWidget.Operation.Title))
						{
							return;
						}
						try
						{
							// Lock down the display and get the window handle.
							IntPtr display = dpy.Lock();
							XWindow handle = GetWidgetHandle();

							// Set the title bar and icon names.
							SetWindowName(display, handle, name);
						}
						finally
						{
							dpy.Unlock();
						}
					}
				}
			}

	// Update the Motif hint information in the X server.
	private void UpdateMotifHints()
			{
				// Bail out if the caption widget handled the decorations.
				if(!Caption(CaptionWidget.Operation.Decorations))
				{
					return;
				}

				// Build the Motif hint structure.
				Xlib.Xlong[] hint = new Xlib.Xlong [5];
				int flags = 0;
				if(functions != MotifFunctions.All)
				{
					hint[1] = (Xlib.Xlong)(int)functions;
					flags |= 1;
				}
				else
				{
					hint[1] = (Xlib.Xlong)(-1);
				}
				if(decorations != MotifDecorations.All)
				{
					hint[2] = (Xlib.Xlong)(int)decorations;
					flags |= 2;
				}
				else
				{
					hint[2] = (Xlib.Xlong)(-1);
				}
				if(inputType != MotifInputType.Normal)
				{
					hint[3] = (Xlib.Xlong)(int)inputType;
					flags |= 4;
				}
				else
				{
					hint[3] = (Xlib.Xlong)(-1);
				}
				hint[4] = (Xlib.Xlong)(-1);
				hint[0] = (Xlib.Xlong)flags;

				// Set the Motif hint structure on the window.
				try
				{
					IntPtr display = dpy.Lock();
					XWindow handle = GetWidgetHandle();
					Xlib.XChangeProperty
						(display, handle, dpy.wmMwmHints, dpy.wmMwmHints,
						 32, 0 /* PropModeReplace */, hint, 4);
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Get or set the decorations to display on the window's
	/// border.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The value of this property is the set of decorations
	/// that the window desires from the window manager.  The window
	/// manager might ignore this information.</para>
	/// </value>
	public virtual MotifDecorations Decorations
			{
				get
				{
					return decorations;
				}
				set
				{
					if(decorations != value)
					{
						decorations = value;
						UpdateMotifHints();
					}
				}
			}

	/// <summary>
	/// <para>Get or set the functions to display on the window's
	/// context menu.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The value of this property is the set of functions
	/// that the window desires from the window manager.  The window
	/// manager might ignore this information.</para>
	/// </value>
	public virtual MotifFunctions Functions
			{
				get
				{
					return functions;
				}
				set
				{
					if(functions != value)
					{
						functions = value;
						UpdateMotifHints();
					}
				}
			}

	/// <summary>
	/// <para>Get or set the input type for the window.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The value of this property is the input type.  The window
	/// manager might ignore this information.</para>
	/// </value>
	public virtual MotifInputType InputType
			{
				get
				{
					return inputType;
				}
				set
				{
					if(inputType != value)
					{
						inputType = value;
						UpdateMotifHints();
					}
				}
			}

	/// <summary>
	/// <para>Get or set the other hints for the window.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The value of this property is the hint flags.  The window
	/// manager might ignore this information.</para>
	/// </value>
	public virtual OtherHints OtherHints
			{
				get
				{
					return otherHints;
				}
				set
				{
					if(otherHints != value)
					{
						OtherHints prev = otherHints;
						otherHints = value;
						if(!Caption(CaptionWidget.Operation.Decorations))
						{
							return;
						}
						try
						{
							IntPtr display = dpy.Lock();
							XWindow handle = GetWidgetHandle();

							// Change the state of the help button.
							if((prev & OtherHints.HelpButton) !=
									(value & OtherHints.HelpButton))
							{
								SetProtocols(display, handle);
							}

							// Set the window type.
							XAtom type;
							if((value & OtherHints.ToolWindow) != 0)
							{
								type = Xlib.XInternAtom
									(display,
									 "_NET_WM_WINDOW_TYPE_UTILITY",
									 XBool.False);
							}
							else if((value & OtherHints.Dialog) != 0)
							{
								type = Xlib.XInternAtom
									(display,
									 "_NET_WM_WINDOW_TYPE_DIALOG",
									 XBool.False);
							}
							else if((value & OtherHints.Desktop) != 0)
							{
								type = Xlib.XInternAtom
									(display,
									 "_NET_WM_WINDOW_TYPE_DESKTOP",
									 XBool.False);
							}
							else if((value & OtherHints.Dock) != 0)
							{
								type = Xlib.XInternAtom
									(display,
									 "_NET_WM_WINDOW_TYPE_DOCK",
									 XBool.False);
							}
							else if((value & OtherHints.Menu) != 0)
							{
								type = Xlib.XInternAtom
									(display,
									 "_NET_WM_WINDOW_TYPE_MENU",
									 XBool.False);
							}
							else if((value & OtherHints.Splash) != 0)
							{
								type = Xlib.XInternAtom
									(display,
									 "_NET_WM_WINDOW_TYPE_SPLASH",
									 XBool.False);
							}
							else if((value & OtherHints.TopMost) != 0)
							{
								type = Xlib.XInternAtom
									(display,
									 "_NET_WM_STATE_ABOVE",
									 XBool.False);
							}
							else
							{
								type = Xlib.XInternAtom
									(display,
									 "_NET_WM_WINDOW_TYPE_NORMAL",
									 XBool.False);
							}
							XAtom wmType;
							if ((value & OtherHints.TopMost) != 0)
							{
								wmType = Xlib.XInternAtom
									(display, "_NET_WM_STATE",
									XBool.False);
							}
							else
							{
								wmType = Xlib.XInternAtom
									(display, "_NET_WM_WINDOW_TYPE",
									XBool.False);
							}
							XAtom wmAtom = Xlib.XInternAtom
								(display, "ATOM", XBool.False);
							Xlib.Xlong[] data = new Xlib.Xlong [2];
							data[0] = (Xlib.Xlong)type;
							Xlib.XChangeProperty
								(display, handle, wmType, wmAtom,
								 32, 0 /* PropModeReplace */, data, 1);

							// Set the window state, which we can only do
							// before it is first mapped at present.
							if(!firstMapDone)
							{
								SetNetState(display, handle);
							}
						}
						finally
						{
							dpy.Unlock();
						}
					}
				}
			}

	/// <summary>
	/// <para>Get or set the transient parent window.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The transient parent window, or <see langword="null"/>
	/// if there is no transient parent.</para>
	/// </value>
	///
	/// <remarks>
	/// <para>Setting this property to the current widget will have
	/// no effect.  Setting this property to <see langword="null"/>
	/// will make the window transient for the root window.</para>
	/// </remarks>
	public virtual TopLevelWindow TransientFor
			{
				get
				{
					return transientFor;
				}
				set
				{
					if(Parent is CaptionWidget)
					{
						// Cannot set transients for MDI children.
						return;
					}
					if(value != this)
					{
						// Change the "transient for" hint information.
						try
						{
							// Lock down the display and get the handles.
							IntPtr display = dpy.Lock();
							XWindow handle = GetWidgetHandle();
							XWindow thandle;
							if(value != null)
							{
								thandle = value.GetWidgetHandle();
							}
							else
							{
								thandle = screen.RootWindow.GetWidgetHandle();
							}

							// Set the "transient for" hint.
							Xlib.XSetTransientForHint(display, handle, thandle);

							// Record the value for next time.
							transientFor = value;
						}
						finally
						{
							dpy.Unlock();
						}
					}
				}
			}

	/// <summary>
	/// <para>Remove the transient parent window hint.</para>
	/// </summary>
	///
	/// <remarks>
	/// <para>Setting the <c>TransientFor</c> property to
	/// <see langword="null"/> will mark the window as transient, but
	/// for the whole screen instead of an application window.</para>
	///
	/// <para>Calling <c>RemoveTransientFor</c> removes the hint entirely,
	/// resetting the window back to the normal "non-transient" mode.</para>
	/// </remarks>
	public virtual void RemoveTransientFor()
			{
				if(Parent is CaptionWidget)
				{
					// Cannot set transients for MDI children.
					return;
				}
				try
				{
					// Lock down the display and get the handle.
					IntPtr display = dpy.Lock();
					XWindow handle = GetWidgetHandle();

					// Remove the "transient for" hint.
					Xlib.XDeleteProperty
						(display, handle, Xlib.XInternAtom
							(display, "WM_TRANSIENT_FOR", XBool.False));

					// We no longer have a transient parent for this window.
					transientFor = null;
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Get or set the child widget that gets the keyboard focus
	/// by default when the top-level window receives the focus.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The widget that receives the focus, or <see langword="null"/>
	/// if there is no default focus.</para>
	/// </value>
	///
	/// <exception cref="T.Xsharp.XInvalidOperationException">
	/// <para>Raised if the widget is not a child of this top-level window.
	/// </para>
	/// </exception>
	public virtual Widget DefaultFocus
			{
				get
				{
					return defaultFocus;
				}
				set
				{
					if(value == null)
					{
						defaultFocus = null;
					}
					else
					{
						if(!(value is InputOnlyWidget) ||
						   value.TopLevel != this)
						{
							throw new XInvalidOperationException
								(S._("X_InvalidFocusChild"));
						}
					}
				}
			}

	/// <summary>
	/// <para>Set the minimum size for the window.</para>
	/// </summary>
	///
	/// <param name="width">
	/// <para>The new minimum width for the window.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The new minimum width for the window.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if <paramref name="width"/> or <paramref name="height"/>
	/// is out of range.</para>
	/// </exception>
	///
	/// <remarks>
	/// <para>Set both <paramref name="width"/> and <paramref name="height"/>
	/// to disable the minimum size.</para>
	/// </remarks>
	public virtual void SetMinimumSize(int width, int height)
			{
				if(width < 0 || height < 0 || width > 32767 || height > 32767)
				{
					throw new XException(S._("X_InvalidSize"));
				}
				try
				{
					IntPtr display = dpy.Lock();
					XWindow handle = GetWidgetHandle();
					if(width != minWidth || height != minHeight)
					{
						minWidth = width;
						minHeight = height;
					}
					if(Caption(CaptionWidget.Operation.SetMinimumSize))
					{
						XSizeHints hints = BuildSizeHints();
						Xlib.XSetWMNormalHints(display, handle, ref hints);
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Set the maximum size for the window.</para>
	/// </summary>
	///
	/// <param name="width">
	/// <para>The new maximum width for the window.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The new maximum width for the window.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if <paramref name="width"/> or <paramref name="height"/>
	/// is out of range.</para>
	/// </exception>
	///
	/// <remarks>
	/// <para>Set both <paramref name="width"/> and <paramref name="height"/>
	/// to disable the maximum size.</para>
	/// </remarks>
	public virtual void SetMaximumSize(int width, int height)
			{
				if(width < 0 || height < 0 || width > 32767 || height > 32767)
				{
					throw new XException(S._("X_InvalidSize"));
				}
				try
				{
					IntPtr display = dpy.Lock();
					XWindow handle = GetWidgetHandle();
					if(width != maxWidth || height != maxHeight)
					{
						maxWidth = width;
						maxHeight = height;
					}
					if(Caption(CaptionWidget.Operation.SetMaximumSize))
					{
						XSizeHints hints = BuildSizeHints();
						Xlib.XSetWMNormalHints(display, handle, ref hints);
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Get or set the icon associated with this window.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The icon image, or <see langword="null"/> if there is
	/// no icon to be displayed.</para>
	/// </value>
	public virtual Image Icon
			{
				get
				{
					return icon;
				}
				set
				{
					if(icon != value)
					{
						icon = value;
						if(Caption(CaptionWidget.Operation.SetIcon))
						{
							try
							{
								IntPtr display = dpy.Lock();
								XWindow handle = GetWidgetHandle();
								SetWMHints(display, handle);
							}
							finally
							{
								dpy.Unlock();
							}
						}
					}
				}
			}

	
	/// <summary>
	/// <para>Method that is called when the window's iconic state
	/// changes.</para>
	/// </summary>
	protected virtual void OnIconicStateChanged(bool iconfiy)
			{
				// Nothing to do in this base class.
			}

	/// <summary>
	/// <para>Method that is called when the window's maximized state
	/// changes.</para>
	/// </summary>
	protected virtual void OnMaximizedStateChanged(bool maximized)
			{
				// Nothing to do in this base class.
			}

	/// <summary>
	/// <para>Method that is called when the window's close box is
	/// clicked by the user.</para>
	/// </summary>
	///
	/// <returns>
	/// <value>Returns <see langword="true"/> to destroy the window,
	/// or <see langword="false"/> to ignore the close message.</value>
	/// </returns>
	protected virtual bool OnClose()
			{
				return true;
			}

	/// <summary>
	/// <para>Method that is called when the window's "help" box is
	/// clicked by the user.</para>
	/// </summary>
	protected virtual void OnHelp()
			{
				// Nothing to do here.
			}

	/// <summary>
	/// <para>Method that is called when the window gains the primary focus,
	/// just before the actual focus is passed to children.</para>
	/// </summary>
	protected virtual void OnPrimaryFocusIn()
			{
				if(mdiClient != null)
				{
					mdiClient.ChangePrimaryFocus();
				}
			}

	/// <summary>
	/// <para>Method that is called when the window loses the primary focus,
	/// just after the actual focus is removed from children.</para>
	/// </summary>
	protected virtual void OnPrimaryFocusOut()
			{
				if(mdiClient != null)
				{
					mdiClient.ChangePrimaryFocus();
				}
			}

	/// <summary>
	/// <para>Raise this widget to the top of its layer.</para>
	/// </summary>
	public override void Raise()
			{
				if(!Caption(CaptionWidget.Operation.Raise))
				{
					return;
				}
				try
				{
					// Send a message to the window manager to restack us.
					IntPtr display = dpy.Lock();
					XWindow handle = GetWidgetHandle();
					XWindowChanges changes = new XWindowChanges();
					changes.stack_mode = 0;		/* Above */
					Xlib.XReconfigureWMWindow
							(display, handle,
							 Screen.ScreenNumber,
						     (uint)(ConfigureWindowMask.CWStackMode),
							 ref changes);
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Lower this widget to the bottom of its layer.</para>
	/// </summary>
	public override void Lower()
			{
				if(!Caption(CaptionWidget.Operation.Lower))
				{
					return;
				}
				try
				{
					// Send a message to the window manager to restack us.
					IntPtr display = dpy.Lock();
					XWindow handle = GetWidgetHandle();
					XWindowChanges changes = new XWindowChanges();
					changes.stack_mode = 1;		/* Below */
					Xlib.XReconfigureWMWindow
							(display, handle,
							 Screen.ScreenNumber,
						     (uint)(ConfigureWindowMask.CWStackMode),
							 ref changes);
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Move this widget to above one of its siblings.</para>
	/// </summary>
	///
	/// <param name="sibling">
	/// <para>The sibling to move this widget above.</para>
	/// </param>
	public override void MoveToAbove(Widget sibling)
			{
				// TODO: support this in non-top-level mode
				throw new XInvalidOperationException
					(S._("X_NonTopLevelOperation"));
			}

	/// <summary>
	/// <para>Move this widget to above one of its siblings.</para>
	/// </summary>
	///
	/// <param name="sibling">
	/// <para>The sibling to move this widget below.</para>
	/// </param>
	public override void MoveToBelow(Widget sibling)
			{
				// TODO: support this in non-top-level mode
				throw new XInvalidOperationException
					(S._("X_NonTopLevelOperation"));
			}

	// Detect that this top-level window has gained the primary focus.
	private void PrimaryFocusIn()
			{
				if(!hasPrimaryFocus)
				{
					hasPrimaryFocus = true;
					OnPrimaryFocusIn();
					if(defaultFocus != null)
					{
						focusWidget = defaultFocus;
						focusWidget.DispatchFocusIn(null);
					}
					else if(focusWidget != null)
					{
						focusWidget.DispatchFocusIn(null);
					}
				}
			}

	// Set the focus widget to a specific child.
	internal void SetFocus(InputOnlyWidget widget)
			{
				if(!hasPrimaryFocus)
				{
					focusWidget = widget;
					/* 
						Some WindowManager activate and sets the focus to the window only
						if mouse is positioned on it.
						So simulte here, that we got the focus.
					*/
					this.PrimaryFocusIn();
				}
				else if(focusWidget != widget)
				{
					InputOnlyWidget oldFocus = focusWidget;
					focusWidget = widget;
					oldFocus.DispatchFocusOut(widget);
					if(focusWidget == widget)
					{
						widget.DispatchFocusIn(oldFocus);
					}
				}
			}

	// Determine if a specific child currently has the focus.
	internal bool HasFocus(InputOnlyWidget widget)
			{
				return (hasPrimaryFocus && focusWidget == widget);
			}

	// Perform an actual resize operation.  This will be called at the
	// next convenient idle period, to avoid overflowing the event queue.
	private void PerformResize(Object state)
			{
				if(resizeTimer != null)
				{
					resizeTimer.Stop();
					resizeTimer = null;
				}
				expectedWidth = -1;
				expectedHeight = -1;
				if(mdiClient != null)
				{
					mdiClient.PositionControls();
				}
				OnMoveResize(x, y, width, height);
			}

	// Get the contents of a 32-bit window property.
	private Xlib.Xlong[] GetWindowProperty(XAtom name)
			{
				try
				{
					// Lock down the display and get the window handle.
					IntPtr display = dpy.Lock();
					XWindow handle = GetWidgetHandle();

					// Fetch the value of the property.
					XAtom actualTypeReturn;
					Xlib.Xint actualFormatReturn;
					Xlib.Xulong nitemsReturn;
					Xlib.Xulong bytesAfterReturn;
					IntPtr propReturn;
					nitemsReturn = Xlib.Xulong.Zero;
					if(Xlib.XGetWindowProperty
							(display, handle, name, 0, 256,
							 XBool.False, XAtom.Zero,
							 out actualTypeReturn, out actualFormatReturn,
							 out nitemsReturn, out bytesAfterReturn,
							 out propReturn) == XStatus.Zero)
					{
						if(((uint)bytesAfterReturn) > 0)
						{
							// We didn't get everything, so try again.
							if(propReturn != IntPtr.Zero)
							{
								Xlib.XFree(propReturn);
								propReturn = IntPtr.Zero;
							}
							int length = 256 + (((int)bytesAfterReturn) / 4);
							nitemsReturn = Xlib.Xulong.Zero;
							if(Xlib.XGetWindowProperty
									(display, handle, name, 0, length,
									 XBool.False, XAtom.Zero,
									 out actualTypeReturn,
									 out actualFormatReturn,
									 out nitemsReturn, out bytesAfterReturn,
									 out propReturn) != XStatus.Zero)
							{
								propReturn = IntPtr.Zero;
								nitemsReturn = Xlib.Xulong.Zero;
							}
						}
					}
					else
					{
						propReturn = IntPtr.Zero;
						nitemsReturn = Xlib.Xulong.Zero;
					}

					// Convert the property data into an array of longs.
					Xlib.Xlong[] data = new Xlib.Xlong [(int)nitemsReturn];
					int size, posn;
					unsafe
					{
						size = sizeof(Xlib.Xlong);
					}
					for(posn = 0; posn < (int)nitemsReturn; ++posn)
					{
						if(size == 4)
						{
							data[posn] = (Xlib.Xlong)
								Marshal.ReadInt32(propReturn, size * posn);
						}
						else if(size == 8)
						{
							data[posn] = (Xlib.Xlong)
								Marshal.ReadInt64(propReturn, size * posn);
						}
					}

					// Free the property data.
					if(propReturn != IntPtr.Zero)
					{
						Xlib.XFree(propReturn);
					}

					// Return the final data to the caller.
					return data;
				}
				finally
				{
					dpy.Unlock();
				}
			}

	// Determine if a property list contains a "maximized" state atom.
	private bool ContainsMaximizedAtom(Xlib.Xlong[] list)
			{
				try
				{
					IntPtr display = dpy.Lock();
					XAtom atom1 = Xlib.XInternAtom
						(display, "_NET_WM_STATE_MAXIMIZED_VERT",
						 XBool.False);
					XAtom atom2 = Xlib.XInternAtom
						(display, "_NET_WM_STATE_MAXIMIZED_HORZ",
						 XBool.False);
					foreach(Xlib.Xlong value in list)
					{
						if(atom1 == (XAtom)value ||
						   atom2 == (XAtom)value)
						{
							return true;
						}
					}
					return false;
				}
				finally
				{
					dpy.Unlock();
				}
			}

	// Dispatch an event to this widget.
	internal override void DispatchEvent(ref XEvent xevent)
			{
				XKeySym keysym;
				Widget widget;
				InputOnlyWidget io;
				Xlib.Xlong[] data;

				switch((Xsharp.Events.EventType)(xevent.xany.type__))
				{
					case Xsharp.Events.EventType.ClientMessage:
						{
						// Handle messages from the window manager.
						if(xevent.xclient.message_type == dpy.wmProtocols)
						{
							if(xevent.xclient.l(0) == (int)(dpy.wmDeleteWindow))
							{
								// User wants the window to close.
								Close();
							}
							else if(xevent.xclient.l(0) == (int)(dpy.wmTakeFocus))
							{
								// We were given the primary input focus.
								PrimaryFocusIn();
							}
							else if(xevent.xclient.l(0) == (int)(dpy.wmContextHelp))
							{
								// The user pressed the "help" button.
								OnHelp();
							}
							else if(xevent.xclient.l(0) == (int)(dpy.wmPing))
							{
								// The window manager has pinged us to see
								// if we are still responding or are dead.
								// We send the message straight back to the WM.
								try
								{
									IntPtr display = dpy.Lock();
									xevent.xany.window = screen.RootWindow.GetWidgetHandle();
									Xlib.XSendEvent
										(display, xevent.xany.window,
										 XBool.False,
										 (int)(EventMask.NoEventMask),
										 ref xevent);
								}
								finally
								{
									dpy.Unlock();
								}
							}
						}
					}
					break;

				case Xsharp.Events.EventType.PropertyNotify:
					{
						// Handle a property change notification.
						if(xevent.xproperty.atom == dpy.wmState)
						{
							// The "WM_STATE" property has changed.
							if(xevent.xproperty.state == 0)
							{
								// New value for the window state.
								data = GetWindowProperty(dpy.wmState);
								if(data.Length >= 1 && data[0] == (Xlib.Xlong)3)
								{
									// The window is now in the iconic state.
									if(!iconic)
									{
										iconic = true;
										OnIconicStateChanged(true);
									}
								}
								else
								{
									// The window is now in the normal state.
									if(iconic)
									{
										iconic = false;
										OnIconicStateChanged(false);
									}
								}
							}
							else
							{
								// Property removed, so it is "normal" now.
								if(iconic)
								{
									iconic = false;
									OnIconicStateChanged(false);
								}
							}
						}
						else if(xevent.xproperty.atom == dpy.wmNetState)
						{
							// The "_NET_WM_STATE" property has changed.
							if(xevent.xproperty.state == 0)
							{
								// New value: look for maximized state atoms.
								data = GetWindowProperty(dpy.wmNetState);
								if(ContainsMaximizedAtom(data))
								{
									// The window is now maximized.
									if(!maximized)
									{
										maximized = true;
										OnMaximizedStateChanged(true);
									}
								}
								else
								{
									// The window has been restored.
									if(maximized)
									{
										maximized = false;
										OnMaximizedStateChanged(false);
									}
								}
							}
							else
							{
								// Value removed, so not maximized any more.
								if(maximized)
								{
									maximized = false;
									OnMaximizedStateChanged(false);
								}
							}
						}
						else if(xevent.xclient.message_type == dpy.internalBeginInvoke)
						{
							OnBeginInvokeMessage((IntPtr)xevent.xclient.l(0));
						}
					}
					break;

				case Xsharp.Events.EventType.FocusIn:
					{
						// This window has received the focus.
						PrimaryFocusIn();
					}
					break;

				case Xsharp.Events.EventType.FocusOut:
					{
						// This window has lost the focus.
						if(hasPrimaryFocus)
						{
							hasPrimaryFocus = false;
							if(focusWidget != null)
							{
								focusWidget.DispatchFocusOut(null);
							}
							OnPrimaryFocusOut();
						}
					}
					break;

				case Xsharp.Events.EventType.KeyPress:
					{
						// Convert the event into a symbol and a string.
						if(keyBuffer == IntPtr.Zero)
						{
							keyBuffer = Marshal.AllocHGlobal(32);
						}
						keysym = 0;
						int len = Xlib.XLookupString
							(ref xevent.xkey, keyBuffer, 32,
							 ref keysym, IntPtr.Zero);
						String str;
						if(len > 0)
						{
							str = Marshal.PtrToStringAnsi(keyBuffer, len);
						}
						else
						{
							str = null;
						}

						// Special case: check for Alt+F4 to close the window.
						// Some window managers trap Alt+F4 themselves, but not
						// all.  People who are used to System.Windows.Forms
						// under Windows expect Alt+F4 to close the window,
						// irrespective of what key the window manager uses.
						//
						// Note: this check is not foolproof.  The window
						// manager or the kernel may have redirected Alt+F4
						// for some other purpose (e.g. switching between
						// virtual consoles).  On such systems, there is
						// nothing that we can do to get the key event and
						// this code will never be called.
						//
						if((((KeyName)keysym) == KeyName.XK_F4 ||
						    ((KeyName)keysym) == KeyName.XK_KP_F4) &&
						   (xevent.xkey.state & ModifierMask.Mod1Mask) != 0)
						{
							Close();
							break;
						}

						// If we have an MDI client, then give it a chance
						// to process the keypress just in case it is
						// something like Ctrl+F4 or Ctrl+Tab.
						if(mdiClient != null)
						{
							if(mdiClient.DispatchKeyEvent
								((KeyName)keysym, xevent.xkey.state, str))
							{
								break;
							}
						}

						// Dispatch the event.
						widget = focusWidget;
						while(widget != null)
						{
							io = (widget as InputOnlyWidget);
							if(io != null)
							{
								if(io.DispatchKeyEvent
									((KeyName)keysym, xevent.xkey.state, str))
								{
									break;
								}
							}
							if(widget == this)
							{
								break;
							}
							widget = widget.Parent;
						}
					}
					break;

				case Xsharp.Events.EventType.KeyRelease:
					{
						// Convert the event into a symbol and a string.
						if(keyBuffer == IntPtr.Zero)
						{
							keyBuffer = Marshal.AllocHGlobal(32);
						}
						keysym = 0;
						int len = Xlib.XLookupString
							(ref xevent.xkey, keyBuffer, 32,
							 ref keysym, IntPtr.Zero);

						// Dispatch the event.
						widget = focusWidget;
						while(widget != null)
						{
							io = (widget as InputOnlyWidget);
							if(io != null)
							{
								if(io.DispatchKeyReleaseEvent
									((KeyName)keysym, xevent.xkey.state))
								{
									break;
								}
							}
							if(widget == this)
							{
								break;
							}
							widget = widget.Parent;
						}
					}
					break;

				case Xsharp.Events.EventType.ButtonPress:
					{
						if ((xevent.xbutton.button == ButtonName.Button4) ||
						    (xevent.xbutton.button == ButtonName.Button5))
							return;
					}
					break;

				case Xsharp.Events.EventType.ButtonRelease:
					{
						// Handle mouse wheel events.

						// Sanity check
						if ((xevent.xbutton.button != ButtonName.Button4) &&
						    (xevent.xbutton.button != ButtonName.Button5))
							break;

						// Dispatch the event.
						widget = focusWidget;
						while(widget != null)
						{
							io = (widget as InputOnlyWidget);
							if (io != null)
							{
								if (io.DispatchWheelEvent (ref xevent))
								{
									return;
								}
							}
							if (widget == this)
							{
								break;
							}
							widget = widget.Parent;
						}
					}
					break;

				case Xsharp.Events.EventType.ConfigureNotify:
					{
						// The window manager may have caused us to move/resize.
						if(xevent.xconfigure.window != xevent.window)
						{
							// SubstructureNotify - not interesting to us.
							break;
						}
						if(Parent is CaptionWidget)
						{
							// Ignore configure events if we are an MDI child.
							break;
						}
						if(xevent.xconfigure.width != width ||
						   xevent.xconfigure.height != height ||
						   expectedWidth != -1)
						{
							// The size has been changed by the window manager.
							if(expectedWidth == -1)
							{
								// Resize from the window manager, not us.
								width = xevent.xconfigure.width;
								height = xevent.xconfigure.height;
							}
							else if(expectedWidth == xevent.xconfigure.width &&
									expectedHeight == xevent.xconfigure.height)
							{
								// This is the size that we were expecting.
								// Further ConfigureNotify's will be from
								// the window manager instead of from us.
								expectedWidth = -1;
								expectedHeight = -1;
							}
							if(resizeTimer == null)
							{
								resizeTimer = new Timer
									(new TimerCallback(PerformResize), null,
									 0, -1);
							}
						}
						
						if( reparentedNeedMove && 
								(xevent.xconfigure.x != x || xevent.xconfigure.y != y )  )
						{
								x = xevent.xconfigure.x;
								y = xevent.xconfigure.y;
								OnMoveResize(x, y, width, height);
								reparentedNeedMove = false;
						}

						if(xevent.send_event || !reparented)
						{
							// The window manager moved us to a new position.
							if(x != xevent.xconfigure.x ||
							   y != xevent.xconfigure.y)
							{
								x = xevent.xconfigure.x;
								y = xevent.xconfigure.y;
								OnMoveResize(x, y, width, height);
							}
						}
					}
					break;

				case Xsharp.Events.EventType.ReparentNotify:
					{
						// We may have been reparented by the window manager.
						if(xevent.xreparent.window != (XWindow)handle)
						{
							// SubstructureNotify - not interesting to us.
							break;
						}
						if(xevent.xreparent.parent !=
								(XWindow)(screen.RootWindow.handle))
						{
							// Reparented by the window manager.
							reparented = true;
							reparentedNeedMove = true;
						}
						else
						{
							// Window manager crashed: we are back on the root.
							reparented = false;
							reparentedNeedMove = false;
							x = xevent.xreparent.x;
							y = xevent.xreparent.y;
							OnMoveResize(x, y, width, height);
						}
					}
					break;

				case Xsharp.Events.EventType.MapNotify:
					{
						// The window manager mapped us to the screen.
						if(Parent is CaptionWidget)
						{
							break;
						}
						if(iconic)
						{
							iconic = false;
							OnIconicStateChanged(false);
						}
						if(!mapped)
						{
							mapped = true;
							OnMapStateChanged();
						}
					}
					break;

				case Xsharp.Events.EventType.UnmapNotify:
					{
						// We were unmapped from the screen.  If "mapped"
						// is true, then we are being iconified by the window
						// manager.  Otherwise, we asked to be withdrawn.
						if(Parent is CaptionWidget)
						{
							break;
						}
						if(!iconic && mapped)
						{
							iconic = true;
							OnIconicStateChanged(true);
						}
					}
					break;
				}
				base.DispatchEvent(ref xevent);
			}

	// Pass a caption operation up to the caption parent, if there is one.
	// Returns true to perform the normal window manager processing.
	private bool Caption(CaptionWidget.Operation operation)
			{
				Widget parent = Parent;
				if(parent is CaptionWidget)
				{
					return ((CaptionWidget)parent).CaptionOperation(operation);
				}
				else
				{
					return true;
				}
			}

	// Handle a change in the maximized state.
	internal void MaximizeChanged()
			{
				OnMaximizedStateChanged(maximized);
			}

	// Handle a move/resize operation from "CaptionWidget".
	internal void HandleMoveResize(int x, int y, int width, int height)
			{
				bool changed = (x != this.x || y != this.y ||
				                width != this.width || height != this.height);
				this.x = x;
				this.y = y;
				this.width = width;
				this.height = height;
				if(changed)
				{
					// Queue up the "OnMoveResize" for the next idle period.
					if(resizeTimer == null)
					{
						resizeTimer = new Timer
							(new TimerCallback(PerformResize), null,
							 0, -1);
					}
				}
			}

	public double Opacity
	{
		set
		{
			int op = (int)(value * 0xffffffff);
			try
			{	
				IntPtr display = dpy.Lock();
				XWindow handle = GetWidgetHandle();
				
				Xsharp.Xlib.XChangeProperty
									(display, handle,
									Xlib.XInternAtom(display, "_NET_WM_WINDOW_OPACITY",
													XBool.False),
									Xlib.XInternAtom(display, "CARDINAL",
													XBool.False),
									32, 0 /* PropModeReplace */,
									new Xlib.Xlong [] {(Xlib.Xlong)(op)}, 1);
			}
			finally
			{
				dpy.Unlock();
			}
		}
	}

} // class TopLevelWindow

} // namespace Xsharp
