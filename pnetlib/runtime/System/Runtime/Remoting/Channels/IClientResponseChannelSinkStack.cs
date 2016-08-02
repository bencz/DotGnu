/*
 * IClientResponseChannelSinkStack.cs - Implementation of the
 *	"System.Runtime.Remoting.Channels.IClientResponseChannelSinkStack" class.
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

public interface IClientResponseChannelSinkStack
{
	// Request asynchronous processing of a response.
	void AsyncProcessResponse(ITransportHeaders headers, Stream stream);

	// Dispatch an exception on the reply sink.
	void DispatchException(Exception e);

	// Dispatch a reply message on the reply sink.
	void DispatchReplyMessage(IMessage msg);

}; // interface IClientResponseChannelSinkStack

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Channels
