/*
 * FileSystemRights.cs - Implementation of the
 *			"System.Security.AccessControl.FileSystemRights" class.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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

namespace System.Security.AccessControl
{

#if CONFIG_ACCESS_CONTROL

using System.Runtime.InteropServices;

[Flags]
#if !ECMA_COMPAT
[ComVisible(false)]
#endif
public enum FileSystemRights
{
	ListDirectory					= 0x00000001,
	ReadData						= 0x00000001,
	CreateFiles						= 0x00000002,
	WriteData						= 0x00000002,
	CreateDirectories				= 0x00000004,
	AppendData						= 0x00000004,
	ReadExtendedAttributes			= 0x00000008,
	WriteExtendedAttributes			= 0x00000010,
	ExecuteFile						= 0x00000020,
	Traverse						= 0x00000020,
	DeleteDirectoryTree				= 0x00000040,
	ReadAttributes					= 0x00000080,
	WriteAttributes					= 0x00000100,
	Delete							= 0x00010000,
	ReadPermissions					= 0x00020000,
	ChangePermissions				= 0x00040000,
	TakeOwnership					= 0x00080000,
	DeleteSubdirectoriesAndFiles	= 0x00100000,
	FullControl						= 0x001F01FF

}; // enum FileSystemRights

#endif // CONFIG_ACCESS_CONTROL

}; // namespace System.Security.AccessControl
