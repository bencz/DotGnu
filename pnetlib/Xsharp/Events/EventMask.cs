/*
 * EventMask.cs - Event mask values.
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

[Flags]
internal enum EventMask
{

	NoEventMask					= 0,
	KeyPressMask				= (1<<0),
	KeyReleaseMask				= (1<<1),
	ButtonPressMask				= (1<<2),
	ButtonReleaseMask			= (1<<3),
	EnterWindowMask				= (1<<4),
	LeaveWindowMask				= (1<<5),
	PointerMotionMask			= (1<<6),
	PointerMotionHintMask		= (1<<7),
	Button1MotionMask			= (1<<8),
	Button2MotionMask			= (1<<9),
	Button3MotionMask			= (1<<10),
	Button4MotionMask			= (1<<11),
	Button5MotionMask			= (1<<12),
	ButtonMotionMask			= (1<<13),
	KeymapStateMask				= (1<<14),
	ExposureMask				= (1<<15),
	VisibilityChangeMask		= (1<<16),
	StructureNotifyMask			= (1<<17),
	ResizeRedirectMask			= (1<<18),
	SubstructureNotifyMask		= (1<<19),
	SubstructureRedirectMask	= (1<<20),
	FocusChangeMask				= (1<<21),
	PropertyChangeMask			= (1<<22),
	ColormapChangeMask			= (1<<23),
	OwnerGrabButtonMask			= (1<<24) 

} // enum EventMask

} // namespace Xsharp.Events
