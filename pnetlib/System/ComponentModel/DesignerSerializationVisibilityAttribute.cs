/*
 * DesignerSerializationVisibilityAttribute.cs - Implementation of the
 *	"System.ComponentModel.DesignerSerializationVisibilityAttribute" class.
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

namespace System.ComponentModel
{

#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public sealed class DesignerSerializationVisibilityAttribute : Attribute
{
	// Internal state.
	private DesignerSerializationVisibility vis;

	// Pre-defined values.
	public static readonly DesignerSerializationVisibilityAttribute Content =
			new DesignerSerializationVisibilityAttribute
				(DesignerSerializationVisibility.Content);
	public static readonly DesignerSerializationVisibilityAttribute Hidden =
			new DesignerSerializationVisibilityAttribute
				(DesignerSerializationVisibility.Hidden);
	public static readonly DesignerSerializationVisibilityAttribute Visible =
			new DesignerSerializationVisibilityAttribute
				(DesignerSerializationVisibility.Visible);
	public static readonly DesignerSerializationVisibilityAttribute Default =
			Visible;

	// Constructor.
	public DesignerSerializationVisibilityAttribute
				(DesignerSerializationVisibility vis)
			{
				this.vis = vis;
			}

	// Get this attribute's value.
	public DesignerSerializationVisibility Visibility 
			{
				get
				{
					return vis;
				}
			}

	// Determine if two objects are equal.
	public override bool Equals(object obj)
 			{
				DesignerSerializationVisibilityAttribute other;
				other = (obj as DesignerSerializationVisibilityAttribute);
				if(other != null)
				{
					return (other.vis == vis);
				}
				else
				{
					return false;
				}
			}

	// Get a hash code for this object.
	public override int GetHashCode()
			{
				return vis.GetHashCode();
			}

	// Determine if this attribute corresponds to the default value.
	public override bool IsDefaultAttribute()
			{
				return (vis == DesignerSerializationVisibility.Visible);
			}

}; // class DesignerSerializationVisibilityAttribute

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
