/*
 * Debug.cs - Implementation of the
 *			"System.Diagnostics.Debug" class.
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

namespace System.Diagnostics
{

#if ECMA_COMPAT

#define	DEBUG
#define	TRACE

// Replacement for the "System.Diagnostics.Debug" class, which does
// not exist within "System.dll" in ECMA-compatibility mode.

internal sealed class Debug
{
	// This class cannot be instantiated.
	private Debug() {}

	// Write a message to all trace listeners.
	[Conditional("DEBUG")]
	public static void Write(Object value)
			{
				Console.Write(value);
			}
	[Conditional("DEBUG")]
	public static void Write(String message)
			{
				Console.Write(message);
			}
	[Conditional("DEBUG")]
	public static void Write(Object value, String category)
			{
				Console.Write(value);
			}
	[Conditional("DEBUG")]
	public static void Write(String message, String category)
			{
				Console.Write(message);
			}

	// Write a message to all trace listeners.
	[Conditional("DEBUG")]
	public static void WriteLine(Object value)
			{
				Console.WriteLine(value);
			}
	[Conditional("DEBUG")]
	public static void WriteLine(String message)
			{
				Console.WriteLine(message);
			}
	[Conditional("DEBUG")]
	public static void WriteLine(Object value, String category)
			{
				Console.WriteLine(value);
			}
	[Conditional("DEBUG")]
	public static void WriteLine(String message, String category)
			{
				Console.WriteLine(message);
			}

}; // class Debug

#endif // ECMA_COMPAT

}; // namespace System.Diagnostics
