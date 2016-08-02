/*
 * IrDAListener.cs - Implementation of the
 *			"System.Net.IrDAListener" class.
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

namespace System.Net.Sockets
{

using System.IO;

public class IrDAListener
{
	// Internal state.
	private IrDAEndPoint localEP;
	private Socket socket;

	// Constructor.
	public IrDAListener(IrDAEndPoint ep)
			{
				if(ep == null)
				{
					throw new ArgumentNullException("ep");
				}
				this.localEP = ep;
			}
	public IrDAListener(String service)
			{
				this.localEP = new IrDAEndPoint(new byte [4], service);
			}

	// Get the local end point that is being used by this listener.
	public IrDAEndPoint LocalEndpoint
			{
				get
				{
					return localEP;
				}
			}

	// Accept an incoming connection and create a client object.
	public IrDAClient AcceptIrDAClient()
			{
				return new IrDAClient(AcceptSocket());
			}

	// Accept an incoming connection and create a socket object.
	public Socket AcceptSocket()
			{
				if(socket != null)
				{
					return socket.Accept();
				}
				else
				{
					throw new InvalidOperationException
						(S._("IrDA_ListenerNotStarted"));
				}
			}

	// Determine if there is a connection pending.
	public bool Pending()
			{
				if(socket != null)
				{
					return socket.Poll(0, SelectMode.SelectRead);
				}
				else
				{
					throw new InvalidOperationException
						(S._("IrDA_ListenerNotStarted"));
				}
			}

	// Start listening for incoming connections.
	public void Start()
			{
				if(socket == null)
				{
					socket = new Socket(AddressFamily.Irda,
										SocketType.Stream,
										ProtocolType.Unspecified);
					socket.Bind(localEP);
					socket.Listen(Int32.MaxValue);
				}
			}

	// Stop listening for incoming connections.
	public void Stop()
			{
				if(socket != null)
				{
					socket.Close();
					socket = null;
				}
			}

}; // class IrDAListener

}; // namespace System.Net.Sockets
