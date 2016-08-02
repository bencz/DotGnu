/*
 * ISymUnmanaged.cs - Implementation of unmanaged classes for C++ interop.
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


#if CONFIG_EXTENDED_DIAGNOSTICS

using System;
using Microsoft.VisualC;
using System.Runtime.InteropServices;

// We don't use these types - they are for backwards-compatibility only.

[StructLayout(LayoutKind.Sequential, Size=4, Pack=1)]
[DebugInfoInPDB]
[MiscellaneousBits(0x0041)]
public struct ISymUnmanagedBinder
{
}; // struct ISymUnmanagedBinder

[StructLayout(LayoutKind.Sequential, Size=4, Pack=1)]
[DebugInfoInPDB]
[MiscellaneousBits(0x0041)]
public struct ISymUnmanagedDocument
{
}; // struct ISymUnmanagedDocument

[StructLayout(LayoutKind.Sequential, Size=4, Pack=1)]
[DebugInfoInPDB]
[MiscellaneousBits(0x0041)]
public struct ISymUnmanagedDocumentWriter
{
}; // struct ISymUnmanagedDocumentWriter

[StructLayout(LayoutKind.Sequential, Size=4, Pack=1)]
[DebugInfoInPDB]
[MiscellaneousBits(0x0041)]
public struct ISymUnmanagedMethod
{
}; // struct ISymUnmanagedMethod

[StructLayout(LayoutKind.Sequential, Size=4, Pack=1)]
[DebugInfoInPDB]
[MiscellaneousBits(0x0041)]
public struct ISymUnmanagedReader
{
}; // struct ISymUnmanagedReader

[StructLayout(LayoutKind.Sequential, Size=4, Pack=1)]
[DebugInfoInPDB]
[MiscellaneousBits(0x0041)]
public struct ISymUnmanagedScope
{
}; // struct ISymUnmanagedScope

[StructLayout(LayoutKind.Sequential, Size=4, Pack=1)]
[DebugInfoInPDB]
[MiscellaneousBits(0x0041)]
public struct ISymUnmanagedVariable
{
}; // struct ISymUnmanagedVariable

[StructLayout(LayoutKind.Sequential, Size=4, Pack=1)]
[DebugInfoInPDB]
[MiscellaneousBits(0x0041)]
public struct ISymUnmanagedWriter
{
}; // struct ISymUnmanagedWriter

[StructLayout(LayoutKind.Sequential, Size=4, Pack=1)]
[DebugInfoInPDB]
[MiscellaneousBits(0x0041)]
public struct IUnknown
{
}; // struct IUnknown

[StructLayout(LayoutKind.Sequential, Size=4, Pack=1)]
[DebugInfoInPDB]
[MiscellaneousBits(0x0041)]
public struct IStream
{
}; // struct IStream

[StructLayout(LayoutKind.Sequential, Size=16, Pack=1)]
[DebugInfoInPDB]
[MiscellaneousBits(0x0041)]
public struct _GUID
{
}; // struct _GUID

[StructLayout(LayoutKind.Sequential, Size=16, Pack=1)]
[DebugInfoInPDB]
[MiscellaneousBits(0x0041)]
public struct tagPROPVARIANT
{
}; // struct tagPROPVARIANT

[StructLayout(LayoutKind.Sequential, Size=16, Pack=1)]
[DebugInfoInPDB]
[MiscellaneousBits(0x0041)]
public struct tagVARIANT
{
}; // struct tagVARIANT

#if __CSCC__

__module
{
	// Some global rubbish to keep Managed C++ applications happy.

	public static _GUID CLSID_CorSymBinder_SxS;
	public static _GUID CLSID_CorSymWriter_SxS;
	public static _GUID IID_ISymUnmanagedBinder;
	public static _GUID IID_ISymUnmanagedWriter;
	public unsafe static int CoCreateInstance
				(_GUID *A_0, IUnknown *A_1, uint A_2,
				 _GUID *A_3, void **A_4)
			{
				return 0;
			}
	public unsafe static void @delete(void *A_0) {}
	public unsafe static void *@new(uint A_0)
			{
				return null;
			}

}; // __module

#endif // __CSCC__

#endif // CONFIG_EXTENDED_DIAGNOSTICS
