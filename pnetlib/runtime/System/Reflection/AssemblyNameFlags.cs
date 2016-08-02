/*
 * AssemblyNameFlags.cs - Implementation of the
 *			"System.Reflection.AssemblyNameFlags" class.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2009  Free Software Foundation Inc.
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

namespace System.Reflection
{

#if !ECMA_COMPAT
#if CONFIG_FRAMEWORK_2_0
using System.Runtime.InteropServices;

[ComVisible(true)]
#endif // CONFIG_FRAMEWORK_2_0
[Serializable]
[Flags]
public enum AssemblyNameFlags
{
	None         = 0x0000,
	PublicKey    = 0x0001,
	Retargetable = 0x0100
#if CONFIG_FRAMEWORK_2_0
	,
	EnableJITcompileOptimizer	= 0x4000,
	EnableJITcompileTracking	= 0x8000
#endif // CONFIG_FRAMEWORK_2_0

}; // enum AssemblyNameFlags

#endif // ECMA_COMPAT

}; // namespace System.Reflection
