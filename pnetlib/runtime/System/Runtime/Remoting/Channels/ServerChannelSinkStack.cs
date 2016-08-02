/*
 * ServerChannelSinkStack.cs - Implementation of the
 *			"System.Runtime.Remoting.Channels.ServerChannelSinkStack" class.
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

using System.IO;
using System.Runtime.Remoting.Messaging;

public class ServerChannelSinkStack
	: IServerChannelSinkStack, IServerResponseChannelSinkStack
{
	// Internal state.
	private SinkStackEntry top;
	private SinkStackEntry storeTop;

	// Structure of a sink state entry.
	private sealed class SinkStackEntry
	{
		// Internal state.
		public IServerChannelSink sink;
		public Object state;
		public SinkStackEntry below;

		// Constructor.
		public SinkStackEntry(IServerChannelSink sink, Object state,
							  SinkStackEntry below)
				{
					this.sink = sink;
					this.state = state;
					this.below = below;
				}

	}; // class SinkStackEntry

	// Constructors.
	public ServerChannelSinkStack() {}

	// Process a response asynchronously.
	public void AsyncProcessResponse
		(IMessage msg, ITransportHeaders headers, Stream stream)
			{
				if(top == null)
				{
					throw new RemotingException
						(_("Remoting_SinkStackEmpty"));
				}
				SinkStackEntry entry = top;
				top = top.below;
				entry.sink.AsyncProcessResponse
					(this, entry.state, msg, headers, stream);
			}

	// Get the response stream.
	public Stream GetResponseStream(IMessage msg, ITransportHeaders headers)
			{
				if(top == null)
				{
					throw new RemotingException
						(_("Remoting_SinkStackEmpty"));
				}

				// Remove the sink from the stack temporarily.
				SinkStackEntry entry = top;
				top = top.below;

				// Get the stream.
				Stream stream = entry.sink.GetResponseStream
					(this, entry.state, msg, headers);

				// Push the sink back onto the stack.
				entry.below = top;
				top = entry;
				return stream;
			}

	// Pop an item from the stack.
	public Object Pop(IServerChannelSink sink)
			{
				while(top != null)
				{
					if(top.sink == sink)
					{
						break;
					}
					top = top.below;
				}
				if(top == null)
				{
					throw new RemotingException
						(_("Remoting_SinkNotFoundOnStack"));
				}
				Object state = top.state;
				top = top.below;
				return state;
			}

	// Push an item onto the stack.
	public void Push(IServerChannelSink sink, Object state)
			{
				top = new SinkStackEntry(sink, state, top);
			}

	// Handle a server callback.
	public void ServerCallback(IAsyncResult ar)
			{
				// Not used in this implementation.
			}

	// Store into this sink stack.
	public void Store(IServerChannelSink sink, Object state)
			{
				// Find the entry on the stack.
				while(top != null)
				{
					if(top.sink == sink)
					{
						break;
					}
					top = top.below;
				}
				if(top == null)
				{
					throw new RemotingException
						(_("Remoting_SinkNotFoundOnStack"));
				}

				// Remove the entry from the main stack.
				SinkStackEntry entry = top;
				top = top.below;

				// Push the entry onto the store stack.
				entry.below = storeTop;
				entry.state = state;
				storeTop = entry;
			}

	// Store into this sink stack and then dispatch async messages.
	[TODO]
	public void StoreAndDispatch(IServerChannelSink sink, Object state)
			{
				// Find the entry on the stack.
				while(top != null)
				{
					if(top.sink == sink)
					{
						break;
					}
					top = top.below;
				}
				if(top == null)
				{
					throw new RemotingException
						(_("Remoting_SinkNotFoundOnStack"));
				}

				// This entry must be the only one on the stack.
				if(top.below != null)
				{
					throw new RemotingException
						(_("Remoting_SinkStackNonEmpty"));
				}

				// Update the entry with the new state information.
				top.state = state;

				// TODO: dispatch async messages
			}

}; // class ServerChannelSinkStack

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Channels
