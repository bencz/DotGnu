/*
 * AccessibleEvents.cs - Implementation of the
 *		"System.Windows.Forms.AccessibleEvents" class.
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

#if !CONFIG_COMPACT_FORMS

public enum AccessibleEvents
{
	SystemSound					= 1,
	SystemAlert					= 2,
	SystemForeground			= 3,
	SystemMenuStart				= 4,
	SystemMenuEnd				= 5,
	SystemMenuPopupStart		= 6,
	SystemMenuPopupEnd			= 7,
	SystemCaptureStart			= 8,
	SystemCaptureEnd			= 9,
	SystemMoveSizeStart			= 10,
	SystemMoveSizeEnd			= 11,
	SystemContextHelpStart		= 12,
	SystemContextHelpEnd		= 13,
	SystemDragDropStart			= 14,
	SystemDragDropEnd			= 15,
	SystemDialogStart			= 16,
	SystemDialogEnd				= 17,
	SystemScrollingStart		= 18,
	SystemScrollingEnd			= 19,
	SystemSwitchStart			= 20,
	SystemSwitchEnd				= 21,
	SystemMinimizeStart			= 22,
	SystemMinimizeEnd			= 23,
	DefaultActionChange			= 32765,
	Create						= 32768,
	Destroy						= 32769,
	Show						= 32770,
	Hide						= 32771,
	Reorder						= 32772,
	Focus						= 32773,
	Selection					= 32774,
	SelectionAdd				= 32775,
	SelectionRemove				= 32776,
	SelectionWithin				= 32777,
	StateChange					= 32778,
	LocationChange				= 32779,
	NameChange					= 32780,
	DescriptionChange			= 32781,
	ValueChange					= 32782,
	ParentChange				= 32783,
	HelpChange					= 32784,
	AcceleratorChange			= 32786

}; // enum AccessibleEvents

#endif // !CONFIG_COMPACT_FORMS

}; // namespace System.Windows.Forms
