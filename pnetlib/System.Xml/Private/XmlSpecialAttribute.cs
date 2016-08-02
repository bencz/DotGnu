/*
 * XmlSpecialAttribute.cs - Implementation of the
 *		"System.Xml.Private.XmlSpecialAttribute" class.
 *
 * Copyright (C) 2002 Southern Storm Software, Pty Ltd.
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

namespace System.Xml.Private
{

using System;
using System.Xml;

// Special attributes are used by "XmlTextReader" to access
// attributes on "XmlDeclaration" and similar nodes that don't
// normally have an attribute collection.

internal class XmlSpecialAttribute : XmlAttribute
{
	// Constructor.
	internal XmlSpecialAttribute(XmlNode parent, String name)
			: base(parent, ConvertName(parent, name))
			{
				// Nothing to do here.
			}

	// Convert a special name into a full "NameInfo" object.
	private static NameCache.NameInfo ConvertName(XmlNode parent, String name)
			{
				NameCache cache = parent.FindOwnerQuick().nameCache;
				return cache.Add(name, String.Empty, String.Empty);
			}

	// Determine if the attribute value was explictly specified.
	public override bool Specified
			{
				get
				{
					return (Value != String.Empty);
				}
			}

	// Get or set the value associated with this node.
	public override String Value
			{
				get
				{
					return parent.GetSpecialAttribute(LocalName);
				}
				set
				{
					parent.SetSpecialAttribute(LocalName, value);
				}
			}

	// Clone this node in either shallow or deep mode.
	public override XmlNode CloneNode(bool deep)
			{
				// This should never be called in regular usage.
				throw new NotImplementedException();
			}

	// Writes the contents of this node to a specified XmlWriter.
	public override void WriteContentTo(XmlWriter w)
			{
				// This should never be called in regular usage.
				throw new NotImplementedException();
			}

	// Write this node and all of its contents to a specified XmlWriter.
	public override void WriteTo(XmlWriter w)
			{
				// This should never be called in regular usage.
				throw new NotImplementedException();
			}

}; // class XmlSpecialAttribute

}; // namespace System.Xml.Private
