/*
 * CrossingDetail.cs - "Detail" values for Enter/Leave events.
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
/// <para>The <see cref="T:Xsharp.CrossingDetail"/> enumeration specifies
/// notification details for Enter and Leave events on a widget.
/// </para>
/// </summary>
public enum CrossingDetail
{
	NotifyAncestor         = 0,
	NotifyVirtual          = 1,
	NotifyInferior         = 2,
	NotifyNonlinear        = 3,
	NotifyNonlinearVirtual = 4,
	NotifyPointer          = 5,
	NotifyPointerRoot      = 6,
	NotifyDetailNone       = 7

} // enum CrossingDetail

} // namespace Xsharp
