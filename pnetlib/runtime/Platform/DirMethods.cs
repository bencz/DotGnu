/*
 * DirMethods.cs - Implementation of the "Platform.DirMethods" class.
 *
 * Copyright (C) 2001, 2002  Southern Storm Software, Pty Ltd.
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

internal class DirMethods
{

	// Get the path character information from the runtime engine.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static PathInfo GetPathInfo();

	// Get the location of the "System" directory.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static String GetSystemDirectory();
	
	// Gets the last access time and date
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Errno GetLastAccess(string path, out long lastac);
	
	// Gets Last Modification(writes, etc.)  time and date
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Errno GetLastModification(string path, out long last_mod);
	
	// Gets Creation Time and Date
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Errno GetCreationTime(string path, out long create_time);
	
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Errno Delete(string path);
	
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Errno Rename(string old_name, string new_name);
	
	// Get the current directory.  Returns null if not possible.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static String GetCurrentDirectory();

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Errno GetFilesInDirectory(string path, out InternalFileInfo[]
	files);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Errno ChangeDirectory(String name);

	// Get a list of the logical drives in this system.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static String[] GetLogicalDrives();

	// Create a Directory
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Errno CreateDirectory(string path);

}; // class DirMethods

}; // namespace Platform
