/*
 * XKeymapEvent.cs - Definitions for X event structures.
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
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

namespace Xsharp.Events
{

using System;
using System.Runtime.InteropServices;
using OpenSystem.Platform;
using OpenSystem.Platform.X11;

// Keymap event.
[StructLayout(LayoutKind.Sequential)]
internal struct XKeymapEvent
{
	// Structure fields.
	XAnyEvent			common__;
	public sbyte		key_vector0;
	public sbyte		key_vector1;
	public sbyte		key_vector2;
	public sbyte		key_vector3;
	public sbyte		key_vector4;
	public sbyte		key_vector5;
	public sbyte		key_vector6;
	public sbyte		key_vector7;
	public sbyte		key_vector8;
	public sbyte		key_vector9;
	public sbyte		key_vector10;
	public sbyte		key_vector11;
	public sbyte		key_vector12;
	public sbyte		key_vector13;
	public sbyte		key_vector14;
	public sbyte		key_vector15;
	public sbyte		key_vector16;
	public sbyte		key_vector17;
	public sbyte		key_vector18;
	public sbyte		key_vector19;
	public sbyte		key_vector20;
	public sbyte		key_vector21;
	public sbyte		key_vector22;
	public sbyte		key_vector23;
	public sbyte		key_vector24;
	public sbyte		key_vector25;
	public sbyte		key_vector26;
	public sbyte		key_vector27;
	public sbyte		key_vector28;
	public sbyte		key_vector29;
	public sbyte		key_vector30;
	public sbyte		key_vector31;

	// Access parent class fields.
	public int type           { get { return common__.type; } }
	public uint serial        { get { return common__.serial; } }
	public bool send_event    { get { return common__.send_event; } }
	public IntPtr display     { get { return common__.display; } }
	public XWindow     window { get { return common__.window; } }

	// Convert odd fields into types that are useful.
	public sbyte key_vector(int n)
			{
				switch(n)
				{
					case 0:		return key_vector0;
					case 1:		return key_vector1;
					case 2:		return key_vector2;
					case 3:		return key_vector3;
					case 4:		return key_vector4;
					case 5:		return key_vector5;
					case 6:		return key_vector6;
					case 7:		return key_vector7;
					case 8:		return key_vector8;
					case 9:		return key_vector9;
					case 10:	return key_vector10;
					case 11:	return key_vector11;
					case 12:	return key_vector12;
					case 13:	return key_vector13;
					case 14:	return key_vector14;
					case 15:	return key_vector15;
					case 16:	return key_vector16;
					case 17:	return key_vector17;
					case 18:	return key_vector18;
					case 19:	return key_vector19;
					case 20:	return key_vector20;
					case 21:	return key_vector21;
					case 22:	return key_vector22;
					case 23:	return key_vector23;
					case 24:	return key_vector24;
					case 25:	return key_vector25;
					case 26:	return key_vector26;
					case 27:	return key_vector27;
					case 28:	return key_vector28;
					case 29:	return key_vector29;
					case 30:	return key_vector30;
					case 31:	return key_vector31;
				}
				return 0;
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return common__.ToString();
			}

} // struct XKeymapEvent

} // namespace Xsharp.Events
