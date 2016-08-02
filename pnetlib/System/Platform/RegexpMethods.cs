/*
 * RegexpMethods.cs - Internal calls for the Posix regexp methods
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

using System;
using System.Runtime.CompilerServices;
using System.Private;

namespace Platform
{
internal sealed class RegexpMethods 
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static IntPtr CompileInternal(String pattern,int flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static IntPtr CompileWithSyntaxInternal
			(String pattern, int syntax);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static int ExecInternal
			(IntPtr compiled, String input,int flags);
	
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static Array MatchInternal
			(IntPtr compiled, String input,
			 int maxMatches, int flags, Type elemType);
	
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static void FreeInternal(IntPtr compiled);

}; // class RegexpMethods
}; // namespace Platform
