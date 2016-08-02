/*
 * CompilationRelaxations.cs - Implementation of the
 *	"System.Runtime.CompilerServices.CompilationRelaxations" class.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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

namespace System.Runtime.CompilerServices
{

#if CONFIG_FRAMEWORK_2_0

[Flags]
public enum CompilationRelaxations
{
	StringInterning					= 0x0004,
	NoStringInterning				= 0x0008,
	StrictNullReferenceException	= 0x0010,
	RelaxedNullReferenceException	= 0x0020,
	StrictInvalidCastException		= 0x0040,
	RelaxedInvalidCastException		= 0x0080,
	StrictArrayExceptions			= 0x0100,
	RelaxedArrayExceptions			= 0x0200,
	StrictOverflowExceptions		= 0x0400,
	RelaxedOverflowExceptions		= 0x0800

}; // enum CompilationRelaxations

#endif // CONFIG_FRAMEWORK_2_0

}; // namespace System.Runtime.CompilerServices
