/*
 * ChannelDataStore.cs - Implementation of the
 *			"System.Runtime.Remoting.Channels.ChannelDataStore" class.
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

[Serializable]
public class ChannelDataStore : IChannelDataStore
{
	// Internal state.
	private String[] channelURIs;
	private Hashtable items;

	// Constructor.
	public ChannelDataStore(String[] channelURIs)
			{
				this.channelURIs = channelURIs;
			}

	// Get or set the channel URI's for this data store.
	public String[] ChannelUris
			{
				get
				{
					return channelURIs;
				}
				set
				{
					channelURIs = value;
				}
			}

	// Get or set an item in this data store.
	public Object this[Object key]
			{
				get
				{
					if(items == null)
					{
						return null;
					}
					else
					{
						return items[key];
					}
				}
				set
				{
					if(items == null)
					{
						items = new Hashtable();
					}
					items[key] = value;
				}
			}

}; // class ChannelDataStore

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Channels
