/*
 * MethodRental.cs - Implementation of the
 *		"System.Reflection.Emit.MethodRental" class.
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

namespace System.Reflection.Emit
{

#if CONFIG_REFLECTION_EMIT

using System;

public sealed class MethodRental
{
	// Cannot instantiate this class.
	private MethodRental() {}

	// JIT flags for "SwapMethodBody".
	public const int JitOnDemand  = 0x0000;
	public const int JitImmediate = 0x0001;

	// Swap a method's current body for a new one.
	public static void SwapMethodBody
				(Type cls, int methodtoken, IntPtr rgIL,
				 int methodsize, int flags)
			{
				// This is very insecure, as it allows method bodies
				// to be modified after TypeBuilder.CreateType() has
				// been called.  For now, we just ignore this.
			}

}; // class MethodRental

#endif // CONFIG_REFLECTION_EMIT

}; // namespace System.Reflection.Emit
