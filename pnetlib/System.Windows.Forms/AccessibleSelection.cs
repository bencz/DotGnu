/*
 * AccessibleSelection.cs - Implementation of the
 *		"System.Windows.Forms.AccessibleSelection" class.
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

namespace System.Windows.Forms
{

#if !CONFIG_COMPACT_FORMS

[Flags]
public enum AccessibleSelection
{
	None				= 0x0000,
	TakeFocus			= 0x0001,
	TakeSelection		= 0x0002,
	ExtendSelection		= 0x0004,
	AddSelection		= 0x0008,
	RemoveSelection		= 0x0010

}; // enum AccessibleSelection

#endif // !CONFIG_COMPACT_FORMS

}; // namespace System.Windows.Forms
