/*
 * IServerChannelSink.cs - Implementation of the
 *			"System.Runtime.Remoting.Channels.IServerChannelSink" class.
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

public interface IServerChannelSink : IChannelSinkBase
{
	// Get the next sink in the chain.
	IServerChannelSink NextChannelSink { get; }

	// Request asynchronous processing of a response.
	void AsyncProcessResponse
			(IServerResponseChannelSinkStack sinkStack,
			 Object state, IMessage msg,
			 ITransportHeaders headers, Stream stream);

	// Get the request stream.
	Stream GetResponseStream
			(IServerResponseChannelSinkStack sinkStack,
			 Object state, IMessage msg, ITransportHeaders headers);

	// Process a message.
	ServerProcessing ProcessMessage
			(IServerChannelSinkStack sinkStack, IMessage requestMsg,
			 ITransportHeaders requestHeaders, Stream requestStream,
			 out IMessage responseMsg, out ITransportHeaders responseHeaders,
			 out Stream responseStream);

}; // interface IServerChannelSink

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Channels
