/*
 * AsyncResult.cs - Implementation of the
 *			"System.Runtime.Remoting.Messaging.AsyncResult" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System.Runtime.Remoting.Messaging
{

#if CONFIG_REFLECTION

using System;
using System.Threading;
using System.Runtime.CompilerServices;

// This class is not ECMA-compatible, strictly speaking.  But it
// is required to implement asynchronous delegates.

public class AsyncResult : IAsyncResult
#if CONFIG_REMOTING
	, IMessageSink
#endif
{
	// Internal state.
	private Delegate del;
	private Object[] args;
	private AsyncCallback callback;
	private Object state;
	private Object result;
	private Exception resultException;
	private bool synchronous;
	private bool completed;
	private bool endInvokeCalled;
	private ManualResetEvent waitHandle;
#if CONFIG_REMOTING
	private IMessage replyMessage;
	private IMessageCtrl messageControl;
#endif

	// Construct a new asynchronous result object and begin invocation.
	internal AsyncResult(Delegate del, Object[] args,
						 AsyncCallback callback, Object state)
			{
				// Initialize the fields within this class.
				this.del = del;
				this.args = args;
				this.callback = callback;
				this.state = state;
				this.result = null;
				this.resultException = null;
				this.synchronous = false;
				this.completed = false;
				this.endInvokeCalled = false;

				// If we have threads, then queue the delegate to run
				// on the thread pool's completion worker thread.
				if(Thread.CanStartThreads())
				{
					ThreadPool.QueueCompletionItem
						(new WaitCallback(Run), null);
					return;
				}

				// We don't have threads, so call the delegate synchronously.
				this.synchronous = true;
				try
				{
					this.result = del.DynamicInvoke(args);
				}
				catch(Exception e)
				{
					this.resultException = e;
				}
				this.completed = true;
				if(callback != null)
				{
					callback(this);
				}
			}

	// Get the delegate that was invoked.
	public virtual Object AsyncDelegate
			{
				get
				{
					return del;
				}
			}

	// Get the state information for a BeginInvoke callback.
	public virtual Object AsyncState
			{
				get
				{
					return state;
				}
			}

	// Get a wait handle that can be used to wait for the
	// asynchronous delegate call to complete.
	public virtual WaitHandle AsyncWaitHandle
			{
				get
				{
					lock(this)
					{
						if(waitHandle == null)
						{
							waitHandle = new ManualResetEvent(false);
						}
						return waitHandle;
					}
				}
			}

	// Determine if the call completed synchronously.
	public virtual bool CompletedSynchronously
			{
				get
				{
					lock(this)
					{
						return synchronous;
					}
				}
			}

	// Get or set the state which represents if "EndInvoke"
	// has been called for the delegate.
	public bool EndInvokeCalled
			{
				get
				{
					lock(this)
					{
						return endInvokeCalled;
					}
				}
				set
				{
					lock(this)
					{
						endInvokeCalled = value;
					}
				}
			}

	// Determine if the call has completed yet.
	public virtual bool IsCompleted
			{
				get
				{
					lock(this)
					{
						return completed;
					}
				}
			}

	// Run the delegate on the completion worker thread.
	private void Run(Object state)
			{
				try
				{
					result = del.DynamicInvoke(args);
				}
				catch(Exception e)
				{
					resultException = e;
				}
				completed = true;
				((ISignal)AsyncWaitHandle).Signal();
				if(callback != null)
				{
					callback(this);
				}
			}

	// Set the output parameters for an "EndInvoke" request.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void SetOutParams
				(Delegate del, Object[] args, Object[] outParams);

	// End invocation on the delegate in this object.
	internal Object EndInvoke(Object[] outParams)
			{
				// Check for synchronous returns first.
				lock(this)
				{
					if(synchronous)
					{
						endInvokeCalled = true;
						if(resultException != null)
						{
							throw resultException;
						}
						else
						{
							SetOutParams(del, args, outParams);
							return result;
						}
					}
				}

				// Wait for the worker thread to signal us.
				AsyncWaitHandle.WaitOne();

				// Process the return values.
				lock(this)
				{
					endInvokeCalled = true;
					if(resultException != null)
					{
						throw resultException;
					}
					else
					{
						SetOutParams(del, args, outParams);
						return result;
					}
				}
			}

#if CONFIG_REMOTING

	// Implement the IMessageSink interface.
	public IMessageSink NextSink
			{
				get
				{
					return null;
				}
			}
	public virtual IMessageCtrl AsyncProcessMessage
				(IMessage msg, IMessageSink replySink)
			{
				throw new NotSupportedException
					(_("NotSupp_DelAsyncProcMsg"));
			}
	public virtual IMessage SyncProcessMessage(IMessage msg)
			{
				if(msg != null)
				{
					if(msg is IMethodReturnMessage)
					{
						replyMessage = msg;
					}
					else
					{
						replyMessage = new ReturnMessage
							(new RemotingException(), new NullMessage());
					}
				}
				else
				{
					replyMessage = new ReturnMessage
						(new RemotingException(), new NullMessage());
				}
				completed = true;
				((ISignal)AsyncWaitHandle).Signal();
				if(callback != null)
				{
					callback(this);
				}
				return null;
			}

	// Get the reply message.
	public virtual IMessage GetReplyMessage()
			{
				return replyMessage;
			}

	// Set the message control information for this result.
	public virtual void SetMessageCtrl(IMessageCtrl mc)
			{
				messageControl = mc;
			}

#endif // CONFIG_REMOTING

}; // class AsyncResult

#endif // CONFIG_REFLECTION

}; // namespace System.Runtime.Remoting.Messaging
