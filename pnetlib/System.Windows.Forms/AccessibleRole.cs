/*
 * AccessibleRole.cs - Implementation of the
 *			"System.Windows.Forms.AccessibleRole" class.
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

public enum AccessibleRole
{
	Default				= -1,
	None				= 0,
	TitleBar			= 1,
	MenuBar				= 2,
	ScrollBar			= 3,
	Grip				= 4,
	Sound				= 5,
	Cursor				= 6,
	Caret				= 7,
	Alert				= 8,
	Window				= 9,
	Client				= 10,
	MenuPopup			= 11,
	MenuItem			= 12,
	ToolTip				= 13,
	Application			= 14,
	Document			= 15,
	Pane				= 16,
	Chart				= 17,
	Dialog				= 18,
	Border				= 19,
	Grouping			= 20,
	Separator			= 21,
	ToolBar				= 22,
	StatusBar			= 23,
	Table				= 24,
	ColumnHeader		= 25,
	RowHeader			= 26,
	Column				= 27,
	Row					= 28,
	Cell				= 29,
	Link				= 30,
	HelpBalloon			= 31,
	Character			= 32,
	List				= 33,
	ListItem			= 34,
	Outline				= 35,
	OutlineItem			= 36,
	PageTab				= 37,
	PropertyPage		= 38,
	Indicator			= 39,
	Graphic				= 40,
	StaticText			= 41,
	Text				= 42,
	PushButton			= 43,
	CheckButton			= 44,
	RadioButton			= 45,
	ComboBox			= 46,
	DropList			= 47,
	ProgressBar			= 48,
	Dial				= 49,
	HotkeyField			= 50,
	Slider				= 51,
	SpinButton			= 52,
	Diagram				= 53,
	Animation			= 54,
	Equation			= 55,
	ButtonDropDown		= 56,
	ButtonMenu			= 57,
	ButtonDropDownGrid	= 58,
	WhiteSpace			= 59,
	PageTabList			= 60,
	Clock				= 61

}; // enum AccessibleRole

#endif // !CONFIG_COMPACT_FORMS

}; // namespace System.Windows.Forms
