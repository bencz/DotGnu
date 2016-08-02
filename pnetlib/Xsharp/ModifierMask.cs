/*
 * ModifierMask.cs - Key modifier mask values.
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
/// <para>The <see cref="T:Xsharp.ModifierMask"/> enumeration specifies
/// masks for key modifiers such as Shift, Control, etc.</para>
/// </summary>
[Flags]
public enum ModifierMask
{

	ShiftMask		= (1<<0),
	LockMask		= (1<<1),
	ControlMask		= (1<<2),
	Mod1Mask		= (1<<3),
	Mod2Mask		= (1<<4),
	Mod3Mask		= (1<<5),
	Mod4Mask		= (1<<6),
	Mod5Mask		= (1<<7),
	Button1Mask		= (1<<8),
	Button2Mask		= (1<<9),
	Button3Mask		= (1<<10),
	Button4Mask		= (1<<11),
	Button5Mask		= (1<<12),
	AnyModifier		= (1<<15),
	AllButtons		= Button1Mask | Button2Mask | Button3Mask |
					  Button4Mask | Button5Mask

} // enum ModifierMask

} // namespace Xsharp
