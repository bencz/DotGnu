/*
 * ControlStyles.cs - Implementation of the
 *			"System.Windows.Forms.ControlStyles" class.
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

namespace System.Windows.Forms
{

[Flags]
public enum ControlStyles
{
	ContainerControl				= 0x00000001,
	UserPaint						= 0x00000002,
	Opaque							= 0x00000004,
	ResizeRedraw					= 0x00000010,
	FixedWidth						= 0x00000020,
	FixedHeight						= 0x00000040,
	StandardClick					= 0x00000100,
	Selectable						= 0x00000200,
	UserMouse						= 0x00000400,
	SupportsTransparentBackColor	= 0x00000800,
	StandardDoubleClick				= 0x00001000,
	AllPaintingInWmPaint			= 0x00002000,
	CacheText						= 0x00004000,
	EnableNotifyMessage				= 0x00008000,
	DoubleBuffer					= 0x00010000

}; // enum ControlStyles

}; // namespace System.Windows.Forms
