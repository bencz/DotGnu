/*
 * TYPEFLAGS.cs - Implementation of the
 *			"System.Runtime.InteropServices.TYPEFLAGS" class.
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

[ComVisible(false)]
[Serializable]
[Flags]
#if CONFIG_FRAMEWORK_1_2
[Obsolete("Use the class in System.Runtime.InteropServices.ComTypes instead")]
#endif
public enum TYPEFLAGS : short
{
	TYPEFLAG_FAPPOBJECT     = 0x0001,
	TYPEFLAG_FCANCREATE     = 0x0002,
	TYPEFLAG_FLICENSED      = 0x0004,
	TYPEFLAG_FPREDECLID     = 0x0008,
	TYPEFLAG_FHIDDEN        = 0x0010,
	TYPEFLAG_FCONTROL       = 0x0020,
	TYPEFLAG_FDUAL          = 0x0040,
	TYPEFLAG_FNONEXTENSIBLE = 0x0080,
	TYPEFLAG_FOLEAUTOMATION = 0x0100,
	TYPEFLAG_FRESTRICTED    = 0x0200,
	TYPEFLAG_FAGGREGATABLE  = 0x0400,
	TYPEFLAG_FREPLACEABLE   = 0x0800,
	TYPEFLAG_FDISPATCHABLE  = 0x1000,
	TYPEFLAG_FREVERSEBIND   = 0x2000,
	TYPEFLAG_FPROXY         = 0x4000

}; // enum TYPEFLAGS

#endif // CONFIG_COM_INTEROP

}; // namespace System.Runtime.InteropServices
