/*
 * ConsoleColor.cs - Implementation of the "System.ConsoleColor" class.
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

namespace System
{

#if CONFIG_EXTENDED_CONSOLE

public enum ConsoleColor
{
	Black				= 0x00,
	DarkBlue			= 0x01,
	DarkGreen			= 0x02,
	DarkCyan			= 0x03,
	DarkRed				= 0x04,
	DarkMagenta			= 0x05,
	DarkYellow			= 0x06,
	Gray				= 0x07,
	DarkGray			= 0x08,
	Blue				= 0x09,
	Green				= 0x0A,
	Cyan				= 0x0B,
	Red					= 0x0C,
	Magenta				= 0x0D,
	Yellow				= 0x0E,
	White				= 0x0F

}; // enum ConsoleColor

#endif // CONFIG_EXTENDED_CONSOLE

}; // namespace System
