/*
 * ClientChannelSinkStack.cs - Implementation of the
 *			"System.Runtime.Remoting.Channels.ClientChannelSinkStack" class.
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

public class ClientChannelSinkStack
	: IClientChannelSinkStack, IClientResponseChannelSinkStack
{
	// Internal state.
	private IMessageSink replySink;
	private SinkStackEntry top;

	// Structure of a sink state entry.
	private sealed class SinkStackEntry
	{
		// Internal state.
		public IClientChannelSink sink;
		public Object state;
		public SinkStackEntry below;

		// Constructor.
		public SinkStackEntry(IClientChannelSink sink, Object state,
							  SinkStackEntry below)
				{
					this.sink = sink;
					this.state = state;
					this.below = below;
				}

	}; // class SinkStackEntry

	// Constructors.
	public ClientChannelSinkStack() {}
	public ClientChannelSinkStack(IMessageSink replySink)
			{
				this.replySink = replySink;
			}

	// Pop an item from the stack.
	public Object Pop(IClientChannelSink sink)
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
	public void Push(IClientChannelSink sink, Object state)
			{
				top = new SinkStackEntry(sink, state, top);
			}

	// Request asynchronous processing of a response.
	public void AsyncProcessResponse(ITransportHeaders headers, Stream stream)
			{
				if(replySink != null)
				{
					if(top == null)
					{
						throw new RemotingException
							(_("Remoting_SinkStackEmpty"));
					}
					SinkStackEntry entry = top;
					top = top.below;
					entry.sink.AsyncProcessResponse
						(this, entry.state, headers, stream);
				}
			}

	// Dispatch an exception on the reply sink.
	public void DispatchException(Exception e)
			{
				if(replySink != null)
				{
					replySink.SyncProcessMessage(new ReturnMessage(e, null));
				}
			}

	// Dispatch a reply message on the reply sink.
	public void DispatchReplyMessage(IMessage msg)
			{
				if(replySink != null)
				{
					replySink.SyncProcessMessage(msg);
				}
			}

}; // class ClientChannelSinkStack

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Channels
