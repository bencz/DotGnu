/*
 * MenusCommands.cs - Implementation of "System.Windows.Forms.Design.MenusCommands" class 
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
using System.ComponentModel.Design;

public class MenusCommands :  StandardCommands
{
	public MenusCommands()
			{
				// TODO
			}

	public static readonly CommandID ComponentTrayMenu;
	
	public static readonly CommandID ContainerMenu;

	public static readonly CommandID DesignerProperties;

	public static readonly CommandID KeyCancel;

	public static readonly CommandID KeyDefaultAction;

	public static readonly CommandID KeyMoveDown;

	public static readonly CommandID KeyMoveLeft;

	public static readonly CommandID KeyMoveRight;

	public static readonly CommandID KeyMoveUp;

	public static readonly CommandID KeyNudgeDown;

	public static readonly CommandID KeyNudgeHeightDecrease;

	public static readonly CommandID KeyNudgeHeightIncrease;
	
	public static readonly CommandID KeyNudgeLeft;

	public static readonly CommandID KeyNudgeRight;

	public static readonly CommandID KeyNudgeUp;

	public static readonly CommandID KeyNudgeWidthDecrease;

	public static readonly CommandID KeyNudgeWidthIncrease;

	public static readonly CommandID KeyReverseCancel;

	public static readonly CommandID KeySelectNext;

	public static readonly CommandID KeySelectPrevious;

	public static readonly CommandID KeySizeHeightDecrease;

	public static readonly CommandID KeySizeHeightIncrease;

	public static readonly CommandID KeySizeWidthDecrease;

	public static readonly CommandID KeySizeWidthIncrease;

	public static readonly CommandID KeyTabOrderSelect;

	public static readonly CommandID SelectionMenu;

	public static readonly CommandID TraySelectionMenu;
	
} // class MenusCommands

#endif // CONFIG_COMPONENT_MODEL_DESIGN

} // namespace System.Windows.Forms.Design
