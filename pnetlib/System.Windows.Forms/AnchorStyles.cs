/*
 * AnchorStyles.cs - Implementation of the
 *			"System.Windows.Forms.AnchorStyles" class.
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

using System.ComponentModel;

[Flags]
#if (CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS) && CONFIG_COMPONENT_MODEL_DESIGN
[Editor("System.Windows.Forms.Design.AnchorEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
#endif
public enum AnchorStyles
{
	None	= 0x0000,
	Top		= 0x0001,
	Bottom	= 0x0002,
	Left	= 0x0004,
	Right	= 0x0008

}; // enum AnchorStyles

}; // namespace System.Windows.Forms
