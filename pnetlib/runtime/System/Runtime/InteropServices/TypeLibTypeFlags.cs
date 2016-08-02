/*
 * TypeLibTypeFlags.cs - Implementation of the
 *			"System.Runtime.InteropServices.TypeLibTypeFlags" class.
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
[Flags]
public enum TypeLibTypeFlags
{
	FAppObject     = 0x0001,
	FCanCreate     = 0x0002,
	FLicensed      = 0x0004,
	FPreDeclId     = 0x0008,
	FHidden        = 0x0010,
	FControl       = 0x0020,
	FDual          = 0x0040,
	FNonExtensible = 0x0080,
	FOleAutomation = 0x0100,
	FRestricted    = 0x0200,
	FAggregatable  = 0x0400,
	FReplaceable   = 0x0800,
	FDispatchable  = 0x1000,
	FReverseBind   = 0x2000

}; // enum TypeLibTypeFlags

#endif // CONFIG_COM_INTEROP

}; // namespace System.Runtime.InteropServices
