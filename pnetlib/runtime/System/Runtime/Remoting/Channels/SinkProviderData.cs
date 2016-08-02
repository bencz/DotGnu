/*
 * SinkProviderData.cs - Implementation of the
 *			"System.Runtime.Remoting.Channels.SinkProviderData" class.
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

namespace System.Runtime.Remoting.Channels
{

#if CONFIG_REMOTING

using System.Collections;

public class SinkProviderData
{
	// Internal state.
	private String name;
	private ArrayList children;
	private Hashtable properties;

	// Constructor.
	public SinkProviderData(String name)
			{
				this.name = name;
				this.children = new ArrayList();
				this.properties = new Hashtable();
			}

	// Get a list of children.
	public IList Children
			{
				get
				{
					return children;
				}
			}

	// Get the name of this sink provider data object.
	public String Name
			{
				get
				{
					return name;
				}
			}

	// Get the properties on this sink provider data object.
	public IDictionary Properties
			{
				get
				{
					return properties;
				}
			}

}; // class SinkProviderData

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Channels
