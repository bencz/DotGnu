/*
 * BindableAttribute.cs - Implementation of the
 *			"System.ComponentModel.BindableAttribute" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System.ComponentModel
{

#if CONFIG_COMPONENT_MODEL

[AttributeUsage(AttributeTargets.All)]
public sealed class BindableAttribute : Attribute
{
	// Internal state.
	private bool bind;
	private bool def;

	// Constructor.
	public BindableAttribute(BindableSupport flags)
			{
				bind = (flags != BindableSupport.No);
				def = (flags == BindableSupport.Default);
			}
	public BindableAttribute(bool bindable)
			{
				bind = bindable;
				def = false;
			}

	// Get the attribute's value.
	public bool Bindable
			{
				get
				{
					return bind;
				}
			}

	// Determine if two instances of this class are equal.
	public override bool Equals(Object obj)
			{
				BindableAttribute other = (obj as BindableAttribute);
				if(other != null)
				{
					return (bind == other.bind);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this attribute.
	public override int GetHashCode()
			{
				return bind.GetHashCode();
			}

	// Determine if this is the default attribute value.
	public override bool IsDefaultAttribute()
			{
				return (def || Equals(Default));
			}

	// Predefined instances of this class.
	public static readonly BindableAttribute No =
		new BindableAttribute(false);
	public static readonly BindableAttribute Yes =
		new BindableAttribute(true);
	public static readonly BindableAttribute Default = No;

}; // class BindableAttribute

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
