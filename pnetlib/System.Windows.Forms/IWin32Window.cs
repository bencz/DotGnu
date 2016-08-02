/*
 * IWin32Window.cs - Implementation of the
 *			"System.Windows.Forms.IWin32Window" class.
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

namespace System.Windows.Forms
{

using System.Runtime.InteropServices;

#if !ECMA_COMPAT
[ComVisible(true)]
#endif
#if CONFIG_COM_INTEROP
[Guid("458AB8A2-A1EA-4d7b-8EBE-DEE5D3D9442C")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
#endif
public interface IWin32Window
{
	// Get the operating system handle for this window.
	IntPtr Handle { get; }

}; // interface IWin32Window

}; // namespace System.Windows.Forms
