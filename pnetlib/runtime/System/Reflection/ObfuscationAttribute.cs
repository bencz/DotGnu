/*
 * ObfuscationAttribute.cs - Implementation of the
 *			"System.Reflection.ObfuscationAttribute" class.
 *
 * Copyright (C) 2009  Free Software Foundation Inc.
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

#if !ECMA_COMPAT &&  CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK
using System.Runtime.InteropServices;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Assembly |
				AttributeTargets.Class |
				AttributeTargets.Struct |
				AttributeTargets.Enum |
				AttributeTargets.Method |
				AttributeTargets.Property |
				AttributeTargets.Field |
				AttributeTargets.Event |
				AttributeTargets.Interface |
				AttributeTargets.Parameter |
				AttributeTargets.Delegate,
				AllowMultiple = true, Inherited = false)]
public sealed class ObfuscationAttribute : Attribute
{
	private bool applyToMembers;
	private bool exclude;
	private bool stripAfterObfuscation;
	private string feature;

	public ObfuscationAttribute()
	{
		applyToMembers = true;
		exclude = true;
		stripAfterObfuscation = true;
		feature = "all";
	}

	public bool ApplyToMembers
	{
		get
		{
			return applyToMembers;
		}
		set
		{
			applyToMembers = value;
		}
	}

	public bool Exclude
	{
		get
		{
			return exclude;
		}
		set
		{
			exclude = value;
		}
	}

	public string Feature
	{
		get
		{
			return feature;
		}
		set
		{
			feature = value;
		}
	}

	public bool StripAfterObfuscation
	{
		get
		{
			return stripAfterObfuscation;
		}
		set
		{
			stripAfterObfuscation = value;
		}
	}

}; // class ObfuscationAttribute

#endif // !ECMA_COMPAT &&  CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

}; // namespace System.Reflection
