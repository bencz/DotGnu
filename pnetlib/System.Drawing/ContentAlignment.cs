/*
 * ContentAlignment.cs - Implementation of the
 *			"System.Drawing.ContentAlignment" class.
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

namespace System.Drawing
{

using System.ComponentModel;
using System.Drawing.Design;

#if CONFIG_COMPONENT_MODEL_DESIGN
[Editor("System.Drawing.Design.ContentAlignmentEditor, System.Drawing.Design",
		typeof(UITypeEditor))]
#endif
public enum ContentAlignment
{
	None			= 0x0000,
	TopLeft			= 0x0001,
	TopCenter		= 0x0002,
	TopRight		= 0x0004,
	MiddleLeft		= 0x0010,
	MiddleCenter	= 0x0020,
	MiddleRight		= 0x0040,
	BottomLeft		= 0x0100,
	BottomCenter	= 0x0200,
	BottomRight		= 0x0400

}; // enum ContentAlignment

}; // namespace System.Drawing
