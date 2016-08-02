/*
 * FileOptions.cs - Implementation of the "System.IO.FileOptions" class.
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

namespace System.IO
{

#if !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

using System.Runtime.InteropServices;

[ComVisible(true)]
[Serializable]
[Flags]
public enum FileOptions
{
	None			= 0x00000000,
	Encrypted		= 0x00004000,
	DeleteOnClose	= 0x04000000,
	SequentialScan	= 0x08000000,
	RandomAccess	= 0x10000000,
	Asynchronous	= 0x40000000,
	WriteThrough	= 0x80000000

}; // enum FileOptions

#endif // !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

}; // namespace System.IO
