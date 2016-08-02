/*
 * ProcessorArchitecture.cs - Implementation of the
 *			"System.Reflection.ProcessorArchitecture" enumeration.
 *
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

#if !ECMA_COMPAT &&  CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK
using System.Runtime.InteropServices;

[Serializable]
[ComVisible(true)]
public enum ProcessorArchitecture
{
	None	= 0,
	MSIL	= 1,
	X86		= 2,
	IA64	= 3,
	Amd64	= 4

}; // enum ProcessorArchitecture

#endif // !ECMA_COMPAT &&  CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

}; // namespace System.Reflection
