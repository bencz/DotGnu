/*
 * BlockingOperation.cs - Helper class for aborting blocking operation.
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
using System.Runtime.CompilerServices;

// Helper that must be disposed after blocking operation ends.
public sealed class BlockingOperation : IDisposable
{
	// Internal state.
	private volatile Thread thread;
	private BlockingOperation next;

	// Constructor.
	public BlockingOperation(Thread thread)
			{
				this.thread = thread;
			}

	public Thread Thread
			{
				get
				{
					return thread;
				}
				set
				{
					thread = value;
				}
			}

	public BlockingOperation Next
			{
				get
				{
					return next;
				}
				set
				{
					next = value;
				}
			}

	// Send IL_SIG_ABORT to given thread to cancel operation that is blocking
	// in system call.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void ThreadSigAbort(Thread thread);

	// Handle leave from blocking operation.
	public void Dispose()
			{
				thread = null;
			}

	public void Abort()
			{
				Thread t = thread;
				if(t != null)
				{
					thread = null;
					ThreadSigAbort(t);
				}
			}

}; // class BlockingOperation

}; // namespace DotGNU.Platform
