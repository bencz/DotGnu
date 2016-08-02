/*
 * Function.cs - GC function modes.
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
/// <para>The <see cref="T:Xsharp.GCFunction"/> enumeration specifies
/// the function mode for graphics objects
/// (<see cref="T:Xsharp.Graphics"/>).
/// </para>
/// </summary>
public enum GCFunction
{

	GXclear			= 0x0,		/* 0 */
	GXand			= 0x1,		/* src AND dst */
	GXandReverse	= 0x2,		/* src AND NOT dst */
	GXcopy			= 0x3,		/* src */
	GXandInverted	= 0x4,		/* NOT src AND dst */
	GXnoop			= 0x5,		/* dst */
	GXxor			= 0x6,		/* src XOR dst */
	GXor			= 0x7,		/* src OR dst */
	GXnor			= 0x8,		/* NOT src AND NOT dst */
	GXequiv			= 0x9,		/* NOT src XOR dst */
	GXinvert		= 0xa,		/* NOT dst */
	GXorReverse		= 0xb,		/* src OR NOT dst */
	GXcopyInverted	= 0xc,		/* NOT src */
	GXorInverted	= 0xd,		/* NOT src OR dst */
	GXnand			= 0xe,		/* NOT src OR NOT dst */
	GXset			= 0xf		/* 1 */

} // enum Function

} // namespace Xsharp
