/*
 * MethodImplAttributes.cs - Implementation of the
 *			"System.Reflection.MethodImplAttributes" class.
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
public enum MethodImplAttributes
{
	CodeTypeMask		= 0x0003,
	IL					= 0x0000,
	Native				= 0x0001,
	OPTIL				= 0x0002,
	Runtime				= 0x0003,
	ManagedMask			= 0x0004,
	Managed				= 0x0000,
	Unmanaged			= 0x0004,
	NoInlining			= 0x0008,
	ForwardRef			= 0x0010,
	Synchronized		= 0x0020,
#if CONFIG_FRAMEWORK_2_0
	NoOptimization		= 0x0040,
#endif
	PreserveSig			= 0x0080,
	InternalCall		= 0x1000,
	MaxMethodImplVal	= 0xFFFF

}; // enum MethodImplAttributes

#endif // CONFIG_REFLECTION

}; // namespace System.Reflection
