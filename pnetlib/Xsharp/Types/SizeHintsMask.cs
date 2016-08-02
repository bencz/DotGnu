/*
 * SizeHintsMask.cs - Value mask values for "XSetWMNormalHints".
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

namespace Xsharp.Types
{

using System;

[Flags]
internal enum SizeHintsMask
{

	USPosition		= (1<<0),
	USSize			= (1<<1),
	PPosition		= (1<<2),
	PSize			= (1<<3),
	PMinSize		= (1<<4),
	PMaxSize		= (1<<5),
	PResizeInc		= (1<<6),
	PAspect			= (1<<7),
	PBaseSize		= (1<<8),
	PWinGravity		= (1<<9),
	PAllHints		= (PPosition | PSize | PMinSize |
					   PMaxSize | PResizeInc | PAspect)

} // enum SizeHintsMask

} // namespace Xsharp.Types
