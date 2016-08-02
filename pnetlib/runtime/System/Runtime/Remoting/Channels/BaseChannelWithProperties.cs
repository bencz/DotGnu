/*
 * BaseChannelWithProperties.cs - Implementation of the
 *	"System.Runtime.Remoting.Channels.BaseChannelWithProperties" class.
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

public abstract class BaseChannelWithProperties
	: BaseChannelObjectWithProperties
{
	// Accessible state.
	protected IChannelSinkBase SinksWithProperties;

	// Constructor.
	protected BaseChannelWithProperties() {}

	// Get the properties associated with this object.
	public override IDictionary Properties
			{
				get
				{
					// Collect up all dictionaries from the attached sinks.
					ArrayList members = new ArrayList();
					IChannelSinkBase sink;
					IDictionary dict;
					sink = SinksWithProperties;
					while(sink != null)
					{
						dict = sink.Properties;
						if(dict != null)
						{
							members.Add(dict);
						}
						if(sink is IServerChannelSink)
						{
							sink = ((IServerChannelSink)sink).NextChannelSink;
						}
						else
						{
							sink = ((IClientChannelSink)sink).NextChannelSink;
						}
					}
					return new CombinedDictionary(members);
				}
			}

}; // class BaseChannelWithProperties

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Channels
