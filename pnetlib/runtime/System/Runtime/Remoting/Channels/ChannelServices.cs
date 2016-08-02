/*
 * ChannelServices.cs - Implementation of the
 *			"System.Runtime.Remoting.Channels.ChannelServices" class.
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
using System.Runtime.Remoting.Messaging;

public sealed class ChannelServices
{
	// Internal state.
	private static ArrayList channels;

	// This class cannot be instantiated.
	private ChannelServices() {}

	// Get the registered channels.
	public static IChannel[] RegisteredChannels
			{
				get
				{
					lock(typeof(ChannelServices))
					{
						if(channels == null)
						{
							return new IChannel [0];
						}
						IChannel[] array = new IChannel [channels.Count];
						channels.CopyTo(array, 0);
						return array;
					}
				}
			}

	// Asynchronously dispatch a message.
	[TODO]
	public static IMessageCtrl AsyncDispatchMessage
				(IMessage msg, IMessageSink replySink)
			{
				if(msg == null)
				{
					throw new ArgumentNullException("msg");
				}
				// TODO
				return null;
			}

	// Create a server channel sink chain.
	[TODO]
	public static IServerChannelSink CreateServerChannelSinkChain
				(IServerChannelSinkProvider provider, IChannelReceiver channel)
			{
				// TODO
				return null;
			}

	// Dispatch in incoming message.
	[TODO]
	public static ServerProcessing DispatchMessage
				(IServerChannelSinkStack sinkStack, IMessage msg,
		    	 out IMessage replyMsg)
			{
				if(msg == null)
				{
					throw new ArgumentNullException("msg");
				}
				// TODO
				replyMsg = null;
				return ServerProcessing.Complete;
			}

	// Get a registered channel.
	public static IChannel GetChannel(String name)
			{
				lock(typeof(ChannelServices))
				{
					if(channels == null)
					{
						return null;
					}
					foreach(IChannel channel in channels)
					{
						if(channel.ChannelName == name)
						{
							return channel;
						}
					}
					return null;
				}
			}

	// Get the sink properties for a channel.
	[TODO]
	public static IDictionary GetChannelSinkProperties(Object obj)
			{
				// TODO
				return null;
			}

	// Get the URL's that can be used to reach an object.
	[TODO]
	public static String[] GetUrlsForObject(MarshalByRefObject obj)
			{
				// TODO
				return null;
			}

	// Register a channel.
	public static void RegisterChannel(IChannel chnl)
			{
				if(chnl == null)
				{
					throw new ArgumentNullException("chnl");
				}
				String name = chnl.ChannelName;
				lock(typeof(ChannelServices))
				{
					if(channels == null)
					{
						// This is the first channel.
						channels = new ArrayList();
						channels.Add(chnl);
					}
					else if(name == null || name == String.Empty ||
					        channels.IndexOf(name) == -1)
					{
						// Insert the channel in priority order.
						int index = 0;
						while(index < channels.Count)
						{
							if((channels[index] as IChannel).ChannelPriority
									< chnl.ChannelPriority)
							{
								break;
							}
						}
						if(index < channels.Count)
						{
							channels.Insert(index, chnl);
						}
						else
						{
							channels.Add(chnl);
						}
					}
					else
					{
						// The channel is already registered.
						throw new RemotingException
							(_("Remoting_ChannelAlreadyRegistered"));
					}
				}
			}

	// Synchronously dispatch a message.
	[TODO]
	public static IMessage SyncDispatchMessage(IMessage msg)
			{
				if(msg == null)
				{
					throw new ArgumentNullException("msg");
				}
				// TODO
				return null;
			}

	// Unregister a channel.
	public static void UnregisterChannel(IChannel chnl)
			{
				if(chnl != null)
				{
					lock(typeof(ChannelServices))
					{
						if(channels != null)
						{
							int index = channels.IndexOf(chnl);
							if(index != -1)
							{
								channels.RemoveAt(index);
							}
						}
					}
				}
			}

}; // class ChannelServices

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Channels
