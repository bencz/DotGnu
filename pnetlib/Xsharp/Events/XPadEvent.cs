/*
 * XPadEvent.cs - Definitions for X event structures.
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

// Structure that is used to pad an XEvent to its maximum size.
[StructLayout(LayoutKind.Sequential)]
internal struct XPadEvent
{
	public Xlib.Xlong	pad0;
	public Xlib.Xlong	pad1;
	public Xlib.Xlong	pad2;
	public Xlib.Xlong	pad3;
	public Xlib.Xlong	pad4;
	public Xlib.Xlong	pad5;
	public Xlib.Xlong	pad6;
	public Xlib.Xlong	pad7;
	public Xlib.Xlong	pad8;
	public Xlib.Xlong	pad9;
	public Xlib.Xlong	pad10;
	public Xlib.Xlong	pad11;
	public Xlib.Xlong	pad12;
	public Xlib.Xlong	pad13;
	public Xlib.Xlong	pad14;
	public Xlib.Xlong	pad15;
	public Xlib.Xlong	pad16;
	public Xlib.Xlong	pad17;
	public Xlib.Xlong	pad18;
	public Xlib.Xlong	pad19;
	public Xlib.Xlong	pad20;
	public Xlib.Xlong	pad21;
	public Xlib.Xlong	pad22;
	public Xlib.Xlong	pad23;
}

} // namespace Xsharp.Events
