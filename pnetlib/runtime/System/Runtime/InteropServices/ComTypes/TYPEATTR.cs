/*
 * TYPEATTR.cs - Implementation of the
 *			"System.Runtime.InteropServices.ComTypes.TYPEATTR" class.
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

namespace System.Runtime.InteropServices.ComTypes
{

#if CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2

[ComVisible(false)]
[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
public struct TYPEATTR
{
	// Accessible state.
	public const int MEMBER_ID_NIL = -1;
	public Guid guid;
	public int lcid;
	public int dwReserved;
	public int memidConstructor;
	public int memidDestructor;
	public IntPtr lpstrSchema;
	public int cbSizeInstance;
	public TYPEKIND typekind;
	public short cFuncs;
	public short cVars;
	public short cImplTypes;
	public short cbSizeVft;
	public short cbAlignment;
	public TYPEFLAGS wTypeFlags;
	public short wMajorVerNum;
	public short wMinorVerNum;
	public TYPEDESC tdescAlias;
	public IDLDESC idldescType;

}; // struct TYPEATTR

#endif // CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2

}; // namespace System.Runtime.InteropServices.ComTypes
