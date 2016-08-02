/*
 * TaskMethods.cs - Implementation of the "Platform.TaskMethods" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

internal class TaskMethods
{

	// Exit from the current process.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void Exit(int exitCode);

	// Set the exit code for when this process returns
	// from its "Main" method.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void SetExitCode(int exitCode);

	// Get the command line arguments for the current process.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static String[] GetCommandLineArgs();

	// Get the value of an environment variable.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static String GetEnvironmentVariable(String variable);

	// Get the number of environment variables.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static int GetEnvironmentCount();

	// Get the key for a numbered environment variable.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static String GetEnvironmentKey(int posn);

	// Get the value for a numbered environment variable.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static String GetEnvironmentValue(int posn);

}; // class TaskMethods

}; // namespace Platform
