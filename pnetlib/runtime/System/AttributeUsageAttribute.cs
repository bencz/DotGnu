/*
 * AttributeUsageAttribute.cs - Implementation of the
 *			"System.AttributeUsageAttribute" class.
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

#if CONFIG_FRAMEWORK_2_0
[AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
#else
[AttributeUsage(AttributeTargets.Class, Inherited=true)]
#endif
public sealed class AttributeUsageAttribute : Attribute
{
	// Internal state.
	private int flags;

	// Extra flags that are used internally.
	private const int InheritedFlag = 0x00010000;
	private const int AllowMultFlag = 0x00020000;

	// Constructor.
	public AttributeUsageAttribute(AttributeTargets targets)
			{
				flags = (((int)targets) | InheritedFlag);
			}

	// Properties.
	public bool AllowMultiple
		{
			get
			{
				return ((flags & AllowMultFlag) != 0);
			}
			set
			{
				flags = ((flags & ~AllowMultFlag) |
						 (value ? AllowMultFlag : 0));
			}
		}
	public bool Inherited
		{
			get
			{
				return ((flags & InheritedFlag) != 0);
			}
			set
			{
				flags = ((flags & ~InheritedFlag) |
						 (value ? InheritedFlag : 0));
			}
		}
	public AttributeTargets ValidOn
		{
			get
			{
				return unchecked((AttributeTargets)
									(flags & (int)(AttributeTargets.All)));
			}
		}

}; // class AttributeUsageAttribute

}; // namespace System
