/*
 * EventId.cs - Implementation of the
 *			"System.Windows.Forms.EventId" class.
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

// Internal event identifiers.
internal enum EventId
{
	None,

	// "Control" events.
	BackgroundImageChanged,
	BackColorChanged,
	BindingContextChanged,
	CausesValidationChanged,
	ChangeUICues,
	Click,
	ContextMenuChanged,
	ControlAdded,
	ControlRemoved,
	CursorChanged,
	DockChanged,
	DoubleClick,
	DragDrop,
	DragEnter,
	DragLeave,
	DragOver,
	Enter,
	EnabledChanged,
	FontChanged,
	ForeColorChanged,
	GiveFeedback,
	GotFocus,
	HandleCreated,
	HandleDestroyed,
	HelpRequested,
	ImeModeChanged,
	Invalidated,
	KeyDown,
	KeyPress,
	KeyUp,
	Layout,
	Leave,
	LocationChanged,
	LostFocus,
	MouseDown,
	MouseEnter,
	MouseHover,
	MouseLeave,
	MouseMove,
	MouseUp,
	MouseWheel,
	Move,
	Paint,
	PaintBackground,
	ParentChanged,
	QueryAccessibilityHelp,
	QueryContinueDrag,
	Resize,
	RightToLeftChanged,
	SizeChanged,
	StyleChanged,
	SystemColorsChanged,
	TabIndexChanged,
	TabStopChanged,
	TextChanged,
	Validated,
	Validating,
	VisibleChanged,

	// "CheckBox" events.
	AppearanceChanged,
	CheckedChanged,
	CheckStateChanged,

	// "Label" events.
	AutoSizeChanged,
	TextAlignChanged,

	// "UserControl" events.
	Load,

	// "Form" events.
	Activated,
	Closed,
	Closing,
	Deactivate,
	InputLanguageChanged,
	InputLanguageChanging,
	MaximizedBoundsChanged,
	MaximumSizeChanged,
	MdiChildActivate,
	MenuComplete,
	MenuStart,
	MinimumSizeChanged,

	// "TextBoxBase" events.
	AcceptsTabChanged,
	BorderStyleChanged,
	TextBoxClick,
	HideSelectionChanged,
	ModifiedChanged,
	MultilineChanged,
	ReadOnlyChanged,

	// "ToolBar" events.
	ButtonClick,
	ButtonDropDown,

	// "ScrollBar" events.
	Scroll,
	ValueChanged,

	// "PictureBox" event.
	SizeModeChanged,

	// "ComboBox" events.
	DrawItem,
	DropDown,
	DropDownStyleChanged,
	MeasureItem,
	SelectedIndexChanged,
	SelectionChangeCommitted,

	// "ListControl" events.
	DataSourceChanged,
	DisplayMemberChanged,
	SelectedItemChanged,
	SelectedValueChanged,
	ValueMemberChanged,

	// "PropertyGrid" events,
	PropertyValueChanged,
	PropertyTabChanged,
	SelectedGridItemChanged,
	PropertySortChanged,
	SelectedObjectsChanged,

	// "TreeView" events
	AfterCheck,
	AfterCollapse,
	AfterExpand,
	AfterLabelEdit,
	AfterSelect,
	BeforeCheck,
	BeforeCollapse,
	BeforeExpand,
	BeforeLabelEdit,
	BeforeSelect,
	ItemDrag,

	// MonthCalendar events
	DateChanged,
	DateSelected,

	// Splitter events.
	SplitterMoved,
	SplitterMoving,

	// "DataGrid" events
	AllowNavigationChanged,
	BackButtonClick,
	BackgroundColorChanged,
	CaptionVisibleChanged,
	CurrentCellChanged,
	FlatModeChanged,
	Navigate,
	ParentRowsLabelStyleChanged,
	ParentRowsVisibleChanged,
	RowHeaderClick,
	ShowParentDetailsButtonClick,

	// DataGridColumnStyle events
	AlignmentChanged,
	HeaderTextChanged,
	MappingNameChanged,
	NullTextChanged,
	PropertyDescriptorChanged,
	WidthChanged

}; // enum EventId

}; // namespace System.Windows.Forms
