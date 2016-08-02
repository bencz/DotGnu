/*
 * NonStandardExtraAttribute.cs - Implementation of the
 *			"System.NonStandardExtraAttribute" class.
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

// This is a pseudo attribute which is used to tag classes that
// are exported from the assembly, but which are not part of the
// standard API.  Classes marked with this attribute are for internal
// use only, and should not be used by user-level applications.

[AttributeUsage(AttributeTargets.Class |
				AttributeTargets.Interface |
				AttributeTargets.Enum |
				AttributeTargets.Struct |
				AttributeTargets.Delegate,
				AllowMultiple=false, Inherited=true)]
internal sealed class NonStandardExtraAttribute : Attribute
{
	public NonStandardExtraAttribute() {}

}; // class NonStandardExtraAttribute

}; // namespace System
