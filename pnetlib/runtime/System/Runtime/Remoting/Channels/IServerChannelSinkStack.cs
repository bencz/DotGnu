/*
 * IServerChannelSinkStack.cs - Implementation of the
 *			"System.Runtime.Remoting.Channels.IServerChannelSinkStack" class.
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

public interface IServerChannelSinkStack
	: IServerResponseChannelSinkStack
{
	// Pop an item from the stack.
	Object Pop(IServerChannelSink sink);

	// Push an item onto the stack.
	void Push(IServerChannelSink sink, Object state);

	// Handle a server callback.
	void ServerCallback(IAsyncResult ar);

	// Store into this sink stack.
	void Store(IServerChannelSink sink, Object state);

	// Store into this sink stack and then dispatch.
	void StoreAndDispatch(IServerChannelSink sink, Object state);

}; // interface IServerChannelSinkStack

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Channels
