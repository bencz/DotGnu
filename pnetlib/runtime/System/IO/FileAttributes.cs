/*
 * FileAttributes.cs - Implementation of the "System.IO.FileAttributes" class.
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

namespace System.IO
{

#if !ECMA_COMPAT

using System;

[Flags]
public enum FileAttributes
{
	ReadOnly			= 0x00001,
	Hidden				= 0x00002,
	System				= 0x00004,
	Directory			= 0x00010,
	Archive				= 0x00020,
	Device				= 0x00040,
	Normal				= 0x00080,
	Temporary			= 0x00100,
	SparseFile			= 0x00200,
	ReparsePoint		= 0x00400,
	Compressed			= 0x00800,
	Offline				= 0x01000,
	NotContentIndexed	= 0x02000,
	Encrypted			= 0x04000

}; // enum FileAttributes

#endif // !ECMA_COMPAT

}; // namespace System.IO
