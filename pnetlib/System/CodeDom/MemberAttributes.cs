/*
 * MemberAttributes.cs - Implementation of the
 *		System.CodeDom.MemberAttributes class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System.CodeDom
{

#if CONFIG_CODEDOM

using System.Runtime.InteropServices;

[Serializable]
#if CONFIG_COM_INTEROP
[ComVisible(true)]
#endif
public enum MemberAttributes
{
	Abstract          = 0x0001,
	Final             = 0x0002,
	Static            = 0x0003,
	Override          = 0x0004,
	Const             = 0x0005,
	ScopeMask         = 0x000F,
	New               = 0x0010,
	VTableMask        = 0x00F0,
	Overloaded        = 0x0100,
	Assembly          = 0x1000,
	FamilyAndAssembly = 0x2000,
	Family            = 0x3000,
	FamilyOrAssembly  = 0x4000,
	Private           = 0x5000,
	Public            = 0x6000,
	AccessMask        = 0xF000

}; // enum MemberAttributes

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom
