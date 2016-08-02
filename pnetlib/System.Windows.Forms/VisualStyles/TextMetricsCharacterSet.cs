/*
 * TextMetricsCharacterSet.cs - Implementation of the
 *			"System.Windows.Forms.VisualStyles.TextMetricsCharacterSet" class.
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

#if CONFIG_FRAMEWORK_2_0
namespace System.Windows.Forms.VisualStyles
{

public enum TextMetricsCharacterSet
{
	Ansi		= 0,
	Default		= 1,
	Symbol		= 2,
	Mac			= 77,
	ShiftJis	= 128,
	Hangul		= 129,
	Johab		= 130,
	Gb2312		= 134,
	ChineseBig5	= 136,
	Greek		= 161,
	Turkish		= 162,
	Vietnamese	= 163,
	Hebrew		= 177,
	Arabic		= 178,
	Baltic		= 186,
	Russian		= 204,
	Thai		= 222,
	EastEurope	= 238,
	Oem			= 255
}; // enum TextMetricsCharacterSet

}; // namespace System.Windows.Forms.VisualStyles
#endif // CONFIG_FRAMEWORK_2_0

