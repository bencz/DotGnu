/*
 * XClientMessageEvent.cs - Definitions for X event structures.
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

// Client message event.
[StructLayout(LayoutKind.Sequential)]
internal struct XClientMessageEvent
{
	// Nested structures for defining different variants of
	// the client message data payload.
	[StructLayout(LayoutKind.Sequential)]
	internal struct ByteMessage
	{
		public sbyte b0;
		public sbyte b1;
		public sbyte b2;
		public sbyte b3;
		public sbyte b4;
		public sbyte b5;
		public sbyte b6;
		public sbyte b7;
		public sbyte b8;
		public sbyte b9;
		public sbyte b10;
		public sbyte b11;
		public sbyte b12;
		public sbyte b13;
		public sbyte b14;
		public sbyte b15;
		public sbyte b16;
		public sbyte b17;
		public sbyte b18;
		public sbyte b19;

	} // struct ByteMessage
	[StructLayout(LayoutKind.Sequential)]
	internal struct ShortMessage
	{
		public short s0;
		public short s1;
		public short s2;
		public short s3;
		public short s4;
		public short s5;
		public short s6;
		public short s7;
		public short s8;
		public short s9;

	} // struct ShortMessage
	[StructLayout(LayoutKind.Sequential)]
	internal struct LongMessage
	{
		public Xlib.Xlong l0;
		public Xlib.Xlong l1;
		public Xlib.Xlong l2;
		public Xlib.Xlong l3;
		public Xlib.Xlong l4;

	} // struct LongMessage
	[StructLayout(LayoutKind.Explicit)]
	internal struct Message
	{
		[FieldOffset(0)] public ByteMessage  b;
		[FieldOffset(0)] public ShortMessage s;
		[FieldOffset(0)] public LongMessage  l;

	} // struct Message

	// Structure fields.
	XAnyEvent			common__;
	public XAtom    	message_type;
	public Xlib.Xint	format__;
	public Message      data__;

	// Access parent class fields.
	public int type           { get { return common__.type; } }
	public uint serial        { get { return common__.serial; } }
	public bool send_event    { get { return common__.send_event; } }
	public IntPtr display     { get { return common__.display; } }
	public XWindow     window { get { return common__.window; } }

	// Convert odd fields into types that are useful.
	public int format         { get { return (int)format__; }
								set { format__ = (Xlib.Xint)value; } }
	public sbyte b(int n)
			{
				switch(n)
				{
					case 0:		return data__.b.b0;
					case 1:		return data__.b.b1;
					case 2:		return data__.b.b2;
					case 3:		return data__.b.b3;
					case 4:		return data__.b.b4;
					case 5:		return data__.b.b5;
					case 6:		return data__.b.b6;
					case 7:		return data__.b.b7;
					case 8:		return data__.b.b8;
					case 9:		return data__.b.b9;
					case 10:	return data__.b.b10;
					case 11:	return data__.b.b11;
					case 12:	return data__.b.b12;
					case 13:	return data__.b.b13;
					case 14:	return data__.b.b14;
					case 15:	return data__.b.b15;
					case 16:	return data__.b.b16;
					case 17:	return data__.b.b17;
					case 18:	return data__.b.b18;
					case 19:	return data__.b.b19;
				}
				return 0;
			}
	public short s(int n)
			{
				switch(n)
				{
					case 0:		return data__.s.s0;
					case 1:		return data__.s.s1;
					case 2:		return data__.s.s2;
					case 3:		return data__.s.s3;
					case 4:		return data__.s.s4;
					case 5:		return data__.s.s5;
					case 6:		return data__.s.s6;
					case 7:		return data__.s.s7;
					case 8:		return data__.s.s8;
					case 9:		return data__.s.s9;
				}
				return 0;
			}
	public int l(int n)
			{
				switch(n)
				{
					case 0:		return (int)(data__.l.l0);
					case 1:		return (int)(data__.l.l1);
					case 2:		return (int)(data__.l.l2);
					case 3:		return (int)(data__.l.l3);
					case 4:		return (int)(data__.l.l4);
				}
				return 0;
			}
	public void setl(int n, int value)
			{
				switch(n)
				{
					case 0:		data__.l.l0 = (Xlib.Xlong)value; break;
					case 1:		data__.l.l1 = (Xlib.Xlong)value; break;
					case 2:		data__.l.l2 = (Xlib.Xlong)value; break;
					case 3:		data__.l.l3 = (Xlib.Xlong)value; break;
					case 4:		data__.l.l4 = (Xlib.Xlong)value; break;
				}
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return common__.ToString() +
					   " message_type=" + ((ulong)message_type).ToString() +
					   " format=" + format.ToString();
			}

} // struct XClientMessageEvent

} // namespace Xsharp.Events
