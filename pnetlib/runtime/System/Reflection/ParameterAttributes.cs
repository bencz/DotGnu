/*
 * ParameterAttributes.cs - Implementation of the
 *		"System.Reflection.ParameterAttributes" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

namespace System.Reflection
{

#if CONFIG_REFLECTION

[Flags]
public enum ParameterAttributes
{
	None				= 0x0000,
	In					= 0x0001,
	Out					= 0x0002,
	Lcid				= 0x0004,
	Retval				= 0x0008,
	Optional			= 0x0010,
	ReservedMask		= 0xF000,
	HasDefault			= 0x1000,
	HasFieldMarshal		= 0x2000,
	Reserved3			= 0x4000,
	Reserved4			= 0x8000

}; // enum ParameterAttributes

#endif // CONFIG_REFLECTION

}; // namespace System.Reflection
