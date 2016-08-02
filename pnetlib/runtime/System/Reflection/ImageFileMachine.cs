/*
 * ImageFileMachine.cs - Implementation of the
 *			"System.Reflection.ImageFileMachine" enumeration.
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

[ComVisible(true)]
[Serializable]
public enum ImageFileMachine
{
	I386			= 0x14C,
	IA64			= 0x200,
	AMD64			= 0x8664

}; // enum ImageFileMachine

#endif // !ECMA_COMPAT &&  CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

}; // namespace System.Reflection
