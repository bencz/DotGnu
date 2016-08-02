/*
 * AccessibleStates.cs - Implementation of the
 *		"System.Windows.Forms.AccessibleStates" class.
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
public enum AccessibleStates
{
	None			= 0x00000000,
	Unavailable		= 0x00000001,
	Selected		= 0x00000002,
	Focused			= 0x00000004,
	Pressed			= 0x00000008,
	Checked			= 0x00000010,
	Indeterminate	= 0x00000020,
	Mixed			= 0x00000020,
	ReadOnly		= 0x00000040,
	HotTracked		= 0x00000080,
	Default			= 0x00000100,
	Expanded		= 0x00000200,
	Collapsed		= 0x00000400,
	Busy			= 0x00000800,
	Floating		= 0x00001000,
	Marqueed		= 0x00002000,
	Animated		= 0x00004000,
	Invisible		= 0x00008000,
	Offscreen		= 0x00010000,
	Sizeable		= 0x00020000,
	Moveable		= 0x00040000,
	SelfVoicing		= 0x00080000,
	Focusable		= 0x00100000,
	Selectable		= 0x00200000,
	Linked			= 0x00400000,
	Traversed		= 0x00800000,
	MultiSelectable	= 0x01000000,
	ExtSelectable	= 0x02000000,
	AlertLow		= 0x04000000,
	AlertMedium		= 0x08000000,
	AlertHigh		= 0x10000000,
	Protected		= 0x20000000,
	Valid			= 0x3FFFFFFF

}; // enum AccessibleStates

#endif // !CONFIG_COMPACT_FORMS

}; // namespace System.Windows.Forms
