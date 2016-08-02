/*
 * Keys.cs - Implementation of the
 *			"System.Windows.Forms.Keys" class.
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

using System.Runtime.InteropServices;

#if !ECMA_COMPAT && !CONFIG_COMPACT_FORMS
[ComVisible(true)]
//TODO [TypeConverter(typeof(KeysConverter))]
#endif
[Flags]
public enum Keys
{
	// No key specified.
	None				= 0x00000000,

	// Letter keys.
	A					= 0x00000041,
	B					= 0x00000042,
	C					= 0x00000043,
	D					= 0x00000044,
	E					= 0x00000045,
	F					= 0x00000046,
	G					= 0x00000047,
	H					= 0x00000048,
	I					= 0x00000049,
	J					= 0x0000004A,
	K					= 0x0000004B,
	L					= 0x0000004C,
	M					= 0x0000004D,
	N					= 0x0000004E,
	O					= 0x0000004F,
	P					= 0x00000050,
	Q					= 0x00000051,
	R					= 0x00000052,
	S					= 0x00000053,
	T					= 0x00000054,
	U					= 0x00000055,
	V					= 0x00000056,
	W					= 0x00000057,
	X					= 0x00000058,
	Y					= 0x00000059,
	Z					= 0x0000005A,

	// Digit keys.
	D0					= 0x00000030,
	D1					= 0x00000031,
	D2					= 0x00000032,
	D3					= 0x00000033,
	D4					= 0x00000034,
	D5					= 0x00000035,
	D6					= 0x00000036,
	D7					= 0x00000037,
	D8					= 0x00000038,
	D9					= 0x00000039,

	// Number pad keys.
	NumPad0				= 0x00000060,
	NumPad1				= 0x00000061,
	NumPad2				= 0x00000062,
	NumPad3				= 0x00000063,
	NumPad4				= 0x00000064,
	NumPad5				= 0x00000065,
	NumPad6				= 0x00000066,
	NumPad7				= 0x00000067,
	NumPad8				= 0x00000068,
	NumPad9				= 0x00000069,

	// Function keys.
	F1					= 0x00000070,
	F2					= 0x00000071,
	F3					= 0x00000072,
	F4					= 0x00000073,
	F5					= 0x00000074,
	F6					= 0x00000075,
	F7					= 0x00000076,
	F8					= 0x00000077,
	F9					= 0x00000078,
	F10					= 0x00000079,
	F11					= 0x0000007A,
	F12					= 0x0000007B,
	F13					= 0x0000007C,
	F14					= 0x0000007D,
	F15					= 0x0000007E,
	F16					= 0x0000007F,
	F17					= 0x00000080,
	F18					= 0x00000081,
	F19					= 0x00000082,
	F20					= 0x00000083,
	F21					= 0x00000084,
	F22					= 0x00000085,
	F23					= 0x00000086,
	F24					= 0x00000087,

	// Buttons.
	LButton				= 0x00000001,
	RButton				= 0x00000002,
	MButton				= 0x00000004,
	XButton1			= 0x00000005,
	XButton2			= 0x00000006,

	// Special keys.
	Cancel				= 0x00000003,
	Back				= 0x00000008,
	Tab					= 0x00000009,
	LineFeed			= 0x0000000A,
	Enter				= 0x0000000D,
	Return				= Enter,
	ShiftKey			= 0x00000010,
	ControlKey			= 0x00000011,
	Menu				= 0x00000012,
	Clear				= 0x00000012,
	Pause				= 0x00000013,
	Capital				= 0x00000014,
	CapsLock			= Capital,
	Escape				= 0x0000001B,
	Space				= 0x00000020,
	Prior				= 0x00000021,
	PageUp				= Prior,
	Next				= 0x00000022,
	PageDown			= Next,
	End					= 0x00000023,
	Home				= 0x00000024,
	Left				= 0x00000025,
	Up					= 0x00000026,
	Right				= 0x00000027,
	Down				= 0x00000028,
	Select				= 0x00000029,
	Print				= 0x0000002A,
	Execute				= 0x0000002B,
	PrintScreen			= 0x0000002C,
	Snapshot			= PrintScreen,
	Insert				= 0x0000002D,
	Delete				= 0x0000002E,
	Help				= 0x0000002F,
	LWin				= 0x0000005B,
	RWin				= 0x0000005C,
	Apps				= 0x0000005D,
	Multiply			= 0x0000006A,
	Add					= 0x0000006B,
	Separator			= 0x0000006C,
	Subtract			= 0x0000006D,
	Decimal				= 0x0000006E,
	Divide				= 0x0000006F,
	NumLock				= 0x00000090,
	Scroll				= 0x00000091,
	LShiftKey			= 0x000000A0,
	RShiftKey			= 0x000000A1,
	LControlKey			= 0x000000A2,
	RControlKey			= 0x000000A3,
	LMenu				= 0x000000A4,
	RMenu				= 0x000000A5,
	ProcessKey			= 0x000000E5,
	Attn				= 0x000000F6,
	Crsel				= 0x000000F7,
	Exsel				= 0x000000F8,
	EraseEof			= 0x000000F9,
	Play				= 0x000000FA,
	Zoom				= 0x000000FB,
	NoName				= 0x000000FC,
	Pa1					= 0x000000FD,
	OemClear			= 0x000000FE,

	// Modifier keys.
	Shift				= 0x00010000,
	Control				= 0x00020000,
	Alt					= 0x00040000,

	// Masks.
	KeyCode				= 0x0000FFFF,
	Modifiers			= unchecked((int)0xFFFF0000),

#if !CONFIG_COMPACT_FORMS

	// Other keys.
	HangulMode			= 0x00000015,
	HanguelMode			= HangulMode,
	KanaMode			= HangulMode,
	JunjaMode			= 0x00000017,
	FinalMode			= 0x00000018,
	HanjaMode			= 0x00000019,
	KanjiMode			= HanjaMode,
	IMEConvert			= 0x0000001C,
	IMENonconvert		= 0x0000001D,
	IMEAceept			= 0x0000001E,
	IMEModeChange		= 0x0000001F,
	BrowserBack			= 0x000000A6,
	BrowserForward		= 0x000000A7,
	BrowserRefresh		= 0x000000A8,
	BrowserStop			= 0x000000A9,
	BrowserSearch		= 0x000000AA,
	BrowserFavorites	= 0x000000AB,
	BrowserHome			= 0x000000AC,
	VolumeMute			= 0x000000AD,
	VolumeDown			= 0x000000AE,
	VolumeUp			= 0x000000AF,
	MediaNextTrack		= 0x000000B0,
	MediaPreviousTrack	= 0x000000B1,
	MediaStop			= 0x000000B2,
	MediaPlayPause		= 0x000000B3,
	LaunchMail			= 0x000000B4,
	SelectMedia			= 0x000000B5,
	LaunchApplication1	= 0x000000B6,
	LaunchApplication2	= 0x000000B7,
	OemSemicolon		= 0x000000BA,
	Oemplus				= 0x000000BB,
	Oemcomma			= 0x000000BC,
	OemMinus			= 0x000000BD,
	OemPeriod			= 0x000000BE,
	OemQuestion			= 0x000000BF,
	OemTilde			= 0x000000C0,
	OemOpenBrackets		= 0x000000DB,
	OemPipe				= 0x000000DC,
	OemCloseBrackets	= 0x000000DD,
	OemQuotes			= 0x000000DE,
	Oem8				= 0x000000DF,
	OemBackslash		= 0x000000E2,

#endif // !CONFIG_COMPACT_FORMS

}; // enum Keys

}; // namespace System.Windows.Forms
