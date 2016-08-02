/*
 * ToolboxBitmapAttribute.cs - Implementation of the
 *		"System.Drawing.ToolboxBitmapAttribute" class.
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

namespace System.Drawing
{

[AttributeUsage(AttributeTargets.Class)]
public class ToolboxBitmapAttribute : Attribute
{
	// Internal state.
	private String imageFile;
	private Type t;
	private String name;

	// The default toolbox bitmap value.
	public static readonly ToolboxBitmapAttribute Default =
			new ToolboxBitmapAttribute((String)null);

	// Constructors.
	public ToolboxBitmapAttribute(String imageFile)
			{
				this.imageFile = imageFile;
			}
	public ToolboxBitmapAttribute(Type t)
			{
				this.t = t;
			}
	public ToolboxBitmapAttribute(Type t, String name)
			{
				this.t = t;
				this.name = name;
			}

	// Determine if two objects are equal.
	public override bool Equals(Object obj)
			{
				ToolboxBitmapAttribute other = (obj as ToolboxBitmapAttribute);
				if(other != null)
				{
					return (imageFile == other.imageFile &&
							t == other.t && name == other.name);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				if(imageFile != null)
				{
					return imageFile.GetHashCode();
				}
				else if(name != null)
				{
					return name.GetHashCode();
				}
				else if(t != null)
				{
					return t.GetHashCode();
				}
				else
				{
					return 0;
				}
			}

	// Get an image from this attribute.
	public Image GetImage(Object component)
			{
				return GetImage(component, false);
			}
	public Image GetImage(Type type)
			{
				return GetImage(type, false);
			}
	[TODO]
	public Image GetImage(Object component, bool large)
			{
				// TODO
				return null;
			}
	[TODO]
	public Image GetImage(Type type, bool large)
			{
				// TODO
				return null;
			}
	[TODO]
	public Image GetImage(Type type, String imgName, bool large)
			{
				// TODO
				return null;
			}

}; // class ToolboxBitmapAttribute

}; // namespace System.Drawing
