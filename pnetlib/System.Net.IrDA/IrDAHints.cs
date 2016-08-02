/*
 * IrDAHints.cs - Implementation of the
 *			"System.Net.IrDAHints" class.
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

namespace System.Net.Sockets
{

[Flags]
public enum IrDAHints
{
	None			= 0x0000,
	PnP				= 0x0001,
	PdaAndPalmtop	= 0x0002,
	Computer		= 0x0004,
	Printer			= 0x0008,
	Modem			= 0x0010,
	Fax				= 0x0020,
	LanAccess		= 0x0040,
	Telephony		= 0x0100,
	FileServer		= 0x0200

}; // enum IrDAHints

}; // namespace System.Net.Sockets
