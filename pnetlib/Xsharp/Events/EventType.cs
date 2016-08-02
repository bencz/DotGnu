/*
 * EventType.cs - Event type codes.
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

internal enum EventType
{

	KeyPress			= 2,
	KeyRelease			= 3,
	ButtonPress			= 4,
	ButtonRelease		= 5,
	MotionNotify		= 6,
	EnterNotify			= 7,
	LeaveNotify			= 8,
	FocusIn				= 9,
	FocusOut			= 10,
	KeymapNotify		= 11,
	Expose				= 12,
	GraphicsExpose		= 13,
	NoExpose			= 14,
	VisibilityNotify	= 15,
	CreateNotify		= 16,
	DestroyNotify		= 17,
	UnmapNotify			= 18,
	MapNotify			= 19,
	MapRequest			= 20,
	ReparentNotify		= 21,
	ConfigureNotify		= 22,
	ConfigureRequest	= 23,
	GravityNotify		= 24,
	ResizeRequest		= 25,
	CirculateNotify		= 26,
	CirculateRequest	= 27,
	PropertyNotify		= 28,
	SelectionClear		= 29,
	SelectionRequest	= 30,
	SelectionNotify		= 31,
	ColormapNotify		= 32,
	ClientMessage		= 33,
	MappingNotify		= 34

} // enum EventType

} // namespace Xsharp.Events
