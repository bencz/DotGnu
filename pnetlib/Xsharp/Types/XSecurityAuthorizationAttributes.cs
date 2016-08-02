/*
 * XSecurityAuthorizationAttributes.cs - Definition of security attributes.
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

namespace Xsharp.Types
{

using System;
using System.Runtime.InteropServices;
using OpenSystem.Platform.X11;

[StructLayout(LayoutKind.Sequential)]
internal struct XSecurityAuthorizationAttributes
{

	// Structure fields.
	public Xlib.Xuint		timeout__;
	public Xlib.Xuint		trust_level__;
	public XID				group;
	public Xlib.Xlong		event_mask__;

	// Convert odd fields into types that are useful.
	public uint timeout
			{ get { return (uint)timeout__; }
			  set { timeout__ = (Xlib.Xuint)value; } }
	public uint trust_level
			{ get { return (uint)trust_level__; }
			  set { trust_level__ = (Xlib.Xuint)value; } }
	public long event_mask
			{ get { return (long)event_mask__; }
			  set { event_mask__ = (Xlib.Xlong)value; } }

} // struct XSecurityAuthorizationAttributes

} // namespace Xsharp.Types
