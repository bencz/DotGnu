/*
 * MsgBoxStyle.cs - Implementation of the
 *			"Microsoft.VisualBasic.MsgBoxStyle" class.
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

namespace Microsoft.VisualBasic
{

using System;

[Flags]
public enum MsgBoxStyle
{
	ApplicationModal	= 0x00000000,
	DefaultButton1		= 0x00000000,
	OKOnly				= 0x00000000,
	OKCancel			= 0x00000001,
	AbortRetryIgnore	= 0x00000002,
	YesNoCancel			= 0x00000003,
	YesNo				= 0x00000004,
	RetryCancel			= 0x00000005,
	Critical			= 0x00000010,
	Question			= 0x00000020,
	Exclamation			= 0x00000030,
	Information			= 0x00000040,
	DefaultButton2		= 0x00000100,
	DefaultButton3		= 0x00000200,
	SystemModal			= 0x00001000,
	MsgBoxHelp			= 0x00004000,
	MsgBoxSetForeground	= 0x00010000,
	MsgBoxRight			= 0x00080000,
	MsgBoxRtlReading	= 0x00100000

}; // enum MsgBoxStyle

}; // namespace Microsoft.VisualBasic
