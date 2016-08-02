/*
 * MethodImplOptions.cs - Implementation of the
 *			"System.Runtime.CompilerServices.MethodImplOptions" class.
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

namespace System.Runtime.CompilerServices
{
#if !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0
using System.Runtime.InteropServices;

[ComVisible(true)]
#endif
[Flags]
public enum MethodImplOptions
{
	Unmanaged       = 0x0004,
	NoInlining      = 0x0008,
	ForwardRef      = 0x0010,
	Synchronized    = 0x0020,
	NoOptimization	= 0x0040,
	PreserveSig     = 0x0080,
	InternalCall    = 0x1000

}; // enum MethodImplOptions

}; // namespace System.Runtime.CompilerServices
