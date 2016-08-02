/*
 * SelectionRules.cs - Implementation of "System.Windows.Forms.Design.SelectionRules" class 
 *
 * Copyright (C) 2002  Free Software Foundation, Inc.
 * 
 * Contributions by Adam Ballai <Adam@TheFrontNetworks.net>
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

namespace System.Windows.Forms.Design
{

#if CONFIG_COMPONENT_MODEL_DESIGN
	
using System;
[Flags]
[Serializable]
public enum SelectionRules
{
	AllSizeable = 15,
	BottomSizeable = 2,
	LeftSizeable = 4,
	Locked = -2147483648,
	Moveable = 268435456,
	None = 0,
	RightSizeable = 8,
	TopSizeable = 1,
	Visible = 1073741824
} // enum SelectionRules

#endif // CONFIG_COMPONENT_MODEL_DESIGN

} // namespace System.Windows.Forms.Design
