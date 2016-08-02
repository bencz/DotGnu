/*
 * BlockingOperations.cs - Class used to abort blocking operations on unixes.
 *
 * Copyright (C) 2007  Southern Storm Software, Pty Ltd.
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

namespace DotGNU.Platform
{

using System;
using System.Threading;
using Platform;

// On unix when thread enters blocking system call (e.g. socket accept), it
// cant be regulary aborted from managed code. On windows you can unblock the
// thread by closing socket's handle.
//
// This class is used to emulate the windows behavior by sending abort signal
// when socket is closed.
//
// This class holds references to threads that are blocked in kernel calls.
// Call NewOp() before you start blocking operation and dispose returned result
// when blocking operation ends. For example:
//
// using(BlockingOperation op = blockingOps.NewOp())
// {
//		some_blocking_operation_e_g_socket.accept();
// }
//
// After you close resource of blocking operation (socket), you can call
// Abort() to unblock all registered operations that are blocking.
public sealed class BlockingOperations
{
	// Internal state.
	private BlockingOperation operations;

	// Constructor.
	public BlockingOperations()
			{
			}

	// Call this method before starting blocking operation.
	// Dispose result after operation is done.
	public BlockingOperation NewOp()
			{
				lock(this)
				{
					// Try to find free handler
					BlockingOperation o = operations;
					while(o != null)
					{
						if(o.Thread == null)
						{
							o.Thread = Thread.CurrentThread;
							return o;
						}
						else
						{
							o = o.Next;
						}
					}

					// Create new handler and append other handlers
					o = operations;
					operations = new BlockingOperation(Thread.CurrentThread);
					operations.Next = o;
					return operations;
				}
			}

	// Abort all blocking operations.
	public void Abort()
			{
				lock(this)
				{
					BlockingOperation o = operations;
					while(o != null)
					{
						if(o.Thread != null)
						{
							o.Abort();
						}
						o = o.Next;
					}
				}
			}

}; // class BlockingOperations

}; // namespace DotGNU.Platform
