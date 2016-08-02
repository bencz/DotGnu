/*
 * ComponentCollection.cs - Implementation of the
 *		"System.ComponentModel.ComponentModel.ComponentCollection" class.
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

using System.Collections;
using System.Runtime.InteropServices;

[ComVisible(true)]
public class ComponentCollection : ReadOnlyCollectionBase
{
	// Constructor.
	public ComponentCollection(IComponent[] components)
			{
				InnerList.AddRange(components);
			}

	// Get a collection element by index.
	public virtual IComponent this[int index]
			{
				get
				{
					return (IComponent)(InnerList[index]);
				}
			}

	// Get a collection element by name.
	public virtual IComponent this[String name]
			{
				get
				{
					if(name == null)
					{
						return null;
					}
					ISite site;
					foreach(IComponent component in InnerList)
					{
						site = component.Site;
						if(site != null && site.Name != null &&
						   String.Compare(name, site.Name, true) == 0)
						{
							return component;
						}
					}
					return null;
				}
			}

	// Copy the contents of this collection to an array.
	public void CopyTo(IComponent[] array, int index)
			{
				InnerList.CopyTo(array, index);
			}

}; // class ComponentCollection

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
