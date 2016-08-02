/*
 * ThreadStaticAttribute.cs - Implementation of the
 *			"System.ThreadStaticAttribute" class.
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

// This class is not ECMA-compatible strictly speaking, but it is
// needed to support thread-static variables in the ECMA engine.

#if !ECMA_COMPAT
[Serializable]
#endif
#if CONFIG_FRAMEWORK_2_0
[AttributeUsage(AttributeTargets.Field, AllowMultiple=false, Inherited=false)]
public sealed class ThreadStaticAttribute : Attribute
#else
[AttributeUsage(AttributeTargets.Field, Inherited=false)]
public class ThreadStaticAttribute : Attribute
#endif
{
	// Constructor.
	public ThreadStaticAttribute() {}

}; // class ThreadStaticAttribute

}; // namespace System
