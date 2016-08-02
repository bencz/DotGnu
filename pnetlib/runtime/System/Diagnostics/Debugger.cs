/*
 * Debugger.cs - Implementation of the
 *			"System.Diagnostics.Debugger" class.
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

namespace System.Diagnostics
{

#if !ECMA_COMPAT

using System.Runtime.CompilerServices;

public sealed class Debugger
{

	// Default debugger message category.
	public static readonly String DefaultCategory = null;

	// Determine if a debugger is attached to this process.
	public static bool IsAttached
			{
				get
				{
					return InternalIsAttached();
				}
			}

	// Internal version of "IsAttached".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static bool InternalIsAttached();

	// Cause a breakpoint within the attached debugger.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void Break();

	// Determine if the attached debugger is logging.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static bool IsLogging();

	// Launch a debugger and attach it to this process.
	public static bool Launch()
			{
				if(InternalIsAttached())
				{
					// We already have a debugger attached to this process.
					return true;
				}
				else
				{
					// Launch the debugger.
					return InternalLaunch();
				}
			}

	// Internal version of "Launch".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static bool InternalLaunch();

	// Log a message with the attached debugger.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void Log(int level, String category, String message);

}; // class Debugger

#endif // !ECMA_COMPAT

}; // namespace System.Diagnostics
