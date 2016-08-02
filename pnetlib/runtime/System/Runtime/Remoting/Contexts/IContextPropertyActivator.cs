/*
 * IContextPropertyActivator.cs - Implementation of the
 *			"System.Runtime.Remoting.Contexts.IContextPropertyActivator" class.
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

namespace System.Runtime.Remoting.Contexts
{

#if CONFIG_REMOTING

using System.Runtime.Remoting.Activation;

public interface IContextPropertyActivator
{
	// Collect construction information from a client message.
	void CollectFromClientContext(IConstructionCallMessage msg);

	// Collection construction return information from a server message.
	void CollectFromServerContext(IConstructionReturnMessage msg);

	// Deliver client context information to the server.
	bool DeliverClientContextToServerContext(IConstructionCallMessage msg);

	// Deliver server context information to the client.
	bool DeliverServerContextToClientContext(IConstructionReturnMessage msg);

	// Determine if it is OK to activate a given call message.
	bool IsOKToActivate(IConstructionCallMessage msg);

}; // interface IContextPropertyActivator

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Contexts
