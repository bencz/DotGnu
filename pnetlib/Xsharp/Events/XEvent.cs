/*
 * XEvent.cs - Definitions for X event structures.
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

namespace Xsharp.Events
{

using System;
using System.Runtime.InteropServices;
using OpenSystem.Platform;
using OpenSystem.Platform.X11;

// Union that combines all of the events into one structure.
[StructLayout(LayoutKind.Explicit)]
internal struct XEvent
{
	[FieldOffset(0)] public XAnyEvent                 xany;
	[FieldOffset(0)] public XKeyEvent                 xkey;
	[FieldOffset(0)] public XButtonEvent              xbutton;
	[FieldOffset(0)] public XMotionEvent              xmotion;
	[FieldOffset(0)] public XCrossingEvent            xcrossing;
	[FieldOffset(0)] public XFocusChangeEvent         xfocus;
	[FieldOffset(0)] public XKeymapEvent              xkeymap;
	[FieldOffset(0)] public XExposeEvent              xexpose;
	[FieldOffset(0)] public XGraphicsExposeEvent      xgraphicsexpose;
	[FieldOffset(0)] public XNoExposeEvent            xnoexpose;
	[FieldOffset(0)] public XVisibilityEvent          xvisibility;
	[FieldOffset(0)] public XCreateWindowEvent        xcreatewindow;
	[FieldOffset(0)] public XDestroyWindowEvent       xdestroywindow;
	[FieldOffset(0)] public XUnmapEvent               xunmap;
	[FieldOffset(0)] public XMapEvent                 xmap;
	[FieldOffset(0)] public XMapRequestEvent          xmaprequest;
	[FieldOffset(0)] public XReparentEvent            xreparent;
	[FieldOffset(0)] public XConfigureEvent           xconfigure;
	[FieldOffset(0)] public XGravityEvent             xgravity;
	[FieldOffset(0)] public XResizeRequestEvent       xresizerequest;
	[FieldOffset(0)] public XConfigureRequestEvent    xconfigurerequest;
	[FieldOffset(0)] public XCirculateEvent           xcirculate;
	[FieldOffset(0)] public XCirculateRequestEvent    xcirculaterequest;
	[FieldOffset(0)] public XPropertyEvent            xproperty;
	[FieldOffset(0)] public XSelectionClearEvent      xselectionclear;
	[FieldOffset(0)] public XSelectionRequestEvent    xselectionrequest;
	[FieldOffset(0)] public XSelectionEvent           xselection;
	[FieldOffset(0)] public XColormapEvent            xcolormap;
	[FieldOffset(0)] public XClientMessageEvent       xclient;
	[FieldOffset(0)] public XMappingEvent             xmapping;
	[FieldOffset(0)] public XPadEvent                 pad;

	// Access common fields.
	public EventType type     { get { return (EventType)(xany.type); } }
	public uint serial        { get { return xany.serial; } }
	public bool send_event    { get { return xany.send_event; } }
	public IntPtr display     { get { return xany.display; } }
	public XWindow     window { get { return xany.window; } }

	// Convert this object into a string.
	public override String ToString()
			{
				switch(type)
				{
					case EventType.KeyPress:
					case EventType.KeyRelease:
						return xkey.ToString();

					case EventType.ButtonPress:
					case EventType.ButtonRelease:
						return xbutton.ToString();

					case EventType.MotionNotify:
						return xmotion.ToString();

					case EventType.EnterNotify:
					case EventType.LeaveNotify:
						return xcrossing.ToString();

					case EventType.FocusIn:
					case EventType.FocusOut:
						return xfocus.ToString();

					case EventType.KeymapNotify:
						return xkeymap.ToString();

					case EventType.Expose:
						return xexpose.ToString();

					case EventType.GraphicsExpose:
						return xgraphicsexpose.ToString();

					case EventType.NoExpose:
						return xnoexpose.ToString();

					case EventType.VisibilityNotify:
						return xvisibility.ToString();

					case EventType.CreateNotify:
						return xcreatewindow.ToString();

					case EventType.DestroyNotify:
						return xdestroywindow.ToString();

					case EventType.UnmapNotify:
						return xunmap.ToString();

					case EventType.MapNotify:
						return xmap.ToString();

					case EventType.MapRequest:
						return xmaprequest.ToString();

					case EventType.ReparentNotify:
						return xreparent.ToString();

					case EventType.ConfigureNotify:
						return xconfigure.ToString();

					case EventType.GravityNotify:
						return xgravity.ToString();

					case EventType.ResizeRequest:
						return xresizerequest.ToString();

					case EventType.ConfigureRequest:
						return xconfigurerequest.ToString();

					case EventType.CirculateNotify:
						return xcirculate.ToString();

					case EventType.CirculateRequest:
						return xcirculaterequest.ToString();

					case EventType.PropertyNotify:
						return xproperty.ToString();

					case EventType.SelectionClear:
						return xselectionclear.ToString();

					case EventType.SelectionRequest:
						return xselectionrequest.ToString();

					case EventType.SelectionNotify:
						return xselection.ToString();

					case EventType.ColormapNotify:
						return xcolormap.ToString();

					case EventType.ClientMessage:
						return xclient.ToString();

					case EventType.MappingNotify:
						return xmapping.ToString();

					default:
						return xany.ToString();
				}
			}

} // struct XEvent

} // namespace Xsharp.Events
