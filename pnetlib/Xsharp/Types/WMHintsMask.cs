/*
 * WMHintsMask.cs - Value mask values for "XSetWMHints".
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
internal enum WMHintsMask
{

	InputHint			= (1<<0),
	StateHint			= (1<<1),
	IconPixmapHint		= (1<<2),
	IconWindowHint		= (1<<3),
	IconPositionHint	= (1<<4),
	IconMaskHint		= (1<<5),
	WindowGroupHint		= (1<<6),
	UrgencyHint			= (1<<8),
	AllHints			= (InputHint | StateHint | IconPixmapHint |
						   IconWindowHint | IconPositionHint |
						   IconMaskHint | WindowGroupHint)

} // enum WMHintsMask

} // namespace Xsharp.Types
