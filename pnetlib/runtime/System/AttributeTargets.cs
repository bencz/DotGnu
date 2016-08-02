/*
 * AttributeTargets.cs - Implementation of the "System.AttributeTargets" class.
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

namespace System
{

[Flags]
public enum AttributeTargets
{
	Assembly		 = 0x0001,
	Module			 = 0x0002,
	Class			 = 0x0004,
	Struct			 = 0x0008,
	Enum			 = 0x0010,
	Constructor		 = 0x0020,
	Method			 = 0x0040,
	Property		 = 0x0080,
	Field			 = 0x0100,
	Event			 = 0x0200,
	Interface		 = 0x0400,
	Parameter		 = 0x0800,
	Delegate		 = 0x1000,
	ReturnValue		 = 0x2000,
#if CONFIG_FRAMEWORK_2_0
	GenericParameter = 0x4000,
	All				 = 0x7FFF
#else
	All				 = 0x3FFF
#endif

}; // enum AttributeTargets

}; // namespace System
