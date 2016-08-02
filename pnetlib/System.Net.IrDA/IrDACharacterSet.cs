/*
 * IrDACharacterSet.cs - Implementation of the
 *			"System.Net.IrDACharacterSet" class.
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

public enum IrDACharacterSet
{
	ASCII			= 0,
	ISO8859Latin1	= 1,
	ISO8859Latin2	= 2,
	ISO8859Latin3	= 3,
	ISO8859Latin4	= 4,
	ISO8859Cyrillic	= 5,
	ISO8859Arabic	= 6,
	ISO8859Greek	= 7,
	ISO8859Hebrew	= 8,
	ISO8859Latin5	= 9,
	Unicode			= 10

}; // enum IrDACharacterSet

}; // namespace System.Net.Sockets
