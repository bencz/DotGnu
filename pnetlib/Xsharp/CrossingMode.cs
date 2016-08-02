/*
 * CrossingMode.cs - "Mode" values for Enter/Leave events.
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

namespace Xsharp
{

using System;

/// <summary>
/// <para>The <see cref="T:Xsharp.CrossingMode"/> enumeration specifies
/// notification modes for Enter and Leave events on a widget.
/// </para>
/// </summary>
public enum CrossingMode
{
	NotifyNormal       = 0,
	NotifyGrab         = 1,
	NotifyUngrab       = 2,
	NotifyWhileGrabbed = 3

} // enum CrossingMode

} // namespace Xsharp
