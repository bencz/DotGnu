/*
 * InfoMethods.cs - Implementation of the "Platform.InfoMethods" class.
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

namespace Platform
{

using System;
using System.Runtime.CompilerServices;

// This class returns random information about the environment which
// doesn't fit any of the other categories.

internal class InfoMethods
{

	// Get the version number for the runtime engine as a string.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static String GetRuntimeVersion();

	// Get the NetBIOS machine name of the current machine.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static String GetNetBIOSMachineName();

	// Get the domain name of the current machine.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static String GetUserDomainName();

	// Determine if this application is running in an interactive environment.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static bool IsUserInteractive();

	// Get the name of the current user.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static String GetUserName();

	// Get the working set size of the current process, or zero if not avail.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static long GetWorkingSet();

#if !ECMA_COMPAT

	// Get the platform identifier for the OS that we are running on.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static PlatformID GetPlatformID();

	// Get the autoconf name for the platform that we are running on.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static String GetPlatformName();

#endif // !ECMA_COMPAT

	// Get the global configuration directory.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static String GetGlobalConfigDir();

	// Get the user storage directory (e.g. "$HOME/.cli").
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static String GetUserStorageDir();

	// Get the number of processors on this machine.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static int GetProcessorCount();

}; // class InfoMethods

}; // namespace Platform
