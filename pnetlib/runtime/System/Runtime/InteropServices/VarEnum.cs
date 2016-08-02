/*
 * VarEnum.cs - Implementation of the
 *			"System.Runtime.InteropServices.VarEnum" class.
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

namespace System.Runtime.InteropServices
{

#if CONFIG_COM_INTEROP

[Serializable]
public enum VarEnum
{
	VT_EMPTY           = 0,
	VT_NULL            = 1,
	VT_I2              = 2,
	VT_I4              = 3,
	VT_R4              = 4,
	VT_R8              = 5,
	VT_CY              = 6,
	VT_DATE            = 7,
	VT_BSTR            = 8,
	VT_DISPATCH        = 9,
	VT_ERROR           = 10,
	VT_BOOL            = 11,
	VT_VARIANT         = 12,
	VT_UNKNOWN         = 13,
	VT_DECIMAL         = 14,
	VT_I1              = 16,
	VT_UI1             = 17,
	VT_UI2             = 18,
	VT_UI4             = 19,
	VT_I8              = 20,
	VT_UI8             = 21,
	VT_INT             = 22,
	VT_UINT            = 23,
	VT_VOID            = 24,
	VT_HRESULT         = 25,
	VT_PTR             = 26,
	VT_SAFEARRAY       = 27,
	VT_CARRAY          = 28,
	VT_USERDEFINED     = 29,
	VT_LPSTR           = 30,
	VT_LPWSTR          = 31,
	VT_RECORD          = 36,
	VT_FILETIME        = 64,
	VT_BLOB            = 65,
	VT_STREAM          = 66,
	VT_STORAGE         = 67,
	VT_STREAMED_OBJECT = 68,
	VT_STORED_OBJECT   = 69,
	VT_BLOB_OBJECT     = 70,
	VT_CF              = 71,
	VT_CLSID           = 72,
	VT_VECTOR          = 0x1000,
	VT_ARRAY           = 0x2000,
	VT_BYREF           = 0x4000

}; // enum VarEnum

#endif // CONFIG_COM_INTEROP

}; // namespace System.Runtime.InteropServices
