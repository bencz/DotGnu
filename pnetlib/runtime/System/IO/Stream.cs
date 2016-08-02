/*
 * Stream.cs - Implementation of the "System.IO.Stream" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

namespace System.IO
{

using System;
using System.Threading;

public abstract class Stream : MarshalByRefObject, IDisposable
{
	// Standard null stream.
	public static readonly Stream Null = new NullStream();

	// Constructor.
	protected Stream() {}

	// Asynchronous read/write control class.
	private sealed class AsyncControl : IAsyncResult
	{
		// Internal state.
		private ManualResetEvent waitHandle;
		private bool completedSynchronously;
		private bool completed;
		private bool reading;
		private AsyncCallback callback;
		private Object state;
		private Stream stream;
		private byte[] buffer;
		private int offset;
		private int count;
		private int result;
		private Exception exception;

		// Constructor.
		public AsyncControl(AsyncCallback callback, Object state,
							Stream stream, byte[] buffer, int offset,
							int count, bool reading)
				{
					this.waitHandle = new ManualResetEvent(false);
					this.completedSynchronously = false;
					this.completed = false;
					this.reading = reading;
					this.callback = callback;
					this.state = state;
					this.stream = stream;
					this.buffer = buffer;
					this.offset = offset;
					this.count = count;
					this.result = -1;
					this.exception = null;
				}

		// Run the operation thread.
		private void Run(Object state)
				{
					try
					{
						if(reading)
						{
							result = stream.Read(buffer, offset, count);
						}
						else
						{
							stream.Write(buffer, offset, count);
						}
					}
					catch(Exception e)
					{
						// Save the exception to be thrown in EndRead/EndWrite.
						exception = e;
					}
					completed = true;
					if(callback != null)
					{
						callback(this);
					}
					waitHandle.Set();
				}

		// Start the async thread, or perform the operation synchronously.
		public void Start()
				{
					if(Thread.CanStartThreads())
					{
						ThreadPool.QueueCompletionItem
							(new WaitCallback(Run), null);
					}
					else
					{
						completedSynchronously = true;
						Run(null);
					}
				}

		// End an asynchronous read operation.
		public int EndRead(Stream check)
				{
					if(stream != check || !reading)
					{
						throw new ArgumentException(_("Arg_InvalidAsync"));
					}
					WaitHandle handle = waitHandle;
					if(handle != null)
					{
						if(!completed)
						{
							handle.WaitOne();
						}
						((IDisposable)handle).Dispose();
					}
					waitHandle = null;
					if(exception != null)
					{
						throw exception;
					}
					else
					{
						return result;
					}
				}

		// End an asynchronous write operation.
		public void EndWrite(Stream check)
				{
					if(stream != check || reading)
					{
						throw new ArgumentException(_("Arg_InvalidAsync"));
					}
					WaitHandle handle = waitHandle;
					if(handle != null)
					{
						if(!completed)
						{
							handle.WaitOne();
						}
						((IDisposable)handle).Dispose();
					}
					waitHandle = null;
					if(exception != null)
					{
						throw exception;
					}
				}

		// Implement the IAsyncResult interface.
		public Object AsyncState
				{
					get
					{
						return state;
					}
				}
		public WaitHandle AsyncWaitHandle
				{
					get
					{
						return waitHandle;
					}
				}
		public bool CompletedSynchronously
				{
					get
					{
						return completedSynchronously;
					}
				}
		public bool IsCompleted
				{
					get
					{
						return completed;
					}
				}

	}; // class AsyncControl

	// Begin an asynchronous read operation.
	public virtual IAsyncResult BeginRead
				(byte[] buffer, int offset, int count,
				 AsyncCallback callback, Object state)
			{
				// Validate the parameters and the stream.
				ValidateBuffer(buffer, offset, count);
				if(!CanRead)
				{
					throw new NotSupportedException(_("IO_NotSupp_Read"));
				}

				// Create the result object.
				AsyncControl async = new AsyncControl
					(callback, state, this, buffer, offset, count, true);

				// Start the background read.
				async.Start();
				return async;
			}

	// Wait for an asynchronous read operation to end.
	public virtual int EndRead(IAsyncResult asyncResult)
			{
				if(asyncResult == null)
				{
					throw new ArgumentNullException("asyncResult");
				}
				else if(!(asyncResult is AsyncControl))
				{
					throw new ArgumentException(_("Arg_InvalidAsync"));
				}
				else
				{
					return ((AsyncControl)asyncResult).EndRead(this);
				}
			}

	// Begin an asychronous write operation.
	public virtual IAsyncResult BeginWrite
				(byte[] buffer, int offset, int count,
				 AsyncCallback callback, Object state)
			{
				// Validate the parameters and the stream.
				ValidateBuffer(buffer, offset, count);
				if(!CanWrite)
				{
					throw new NotSupportedException(_("IO_NotSupp_Write"));
				}

				// Create the result object.
				AsyncControl async = new AsyncControl
					(callback, state, this, buffer, offset, count, false);

				// Start the background write.
				async.Start();
				return async;
			}

	// Wait for an asynchronous write operation to end.
	public virtual void EndWrite(IAsyncResult asyncResult)
			{
				if(asyncResult == null)
				{
					throw new ArgumentNullException("asyncResult");
				}
				else if(!(asyncResult is AsyncControl))
				{
					throw new ArgumentException(_("Arg_InvalidAsync"));
				}
				else
				{
					((AsyncControl)asyncResult).EndWrite(this);
				}
			}

	// Close the stream.
	public virtual void Close()
			{
				// Nothing to do here.
			}

	// Create a wait handle for asynchronous operations.
	protected virtual WaitHandle CreateWaitHandle()
			{
				return new ManualResetEvent(false);
			}

	// Flush the pending contents in this stream.
	public abstract void Flush();

	// Read data from this stream.
	public abstract int Read(byte[] buffer, int offset, int count);

	// Read a single byte from this stream.
	public virtual int ReadByte()
			{
				byte[] bytes = new byte[1];
				if(Read(bytes, 0, 1) == 1)
				{
					return (int)(bytes[0]);
				}
				else
				{
					return -1;
				}
			}

	// Seek to a new position within this stream.
	public abstract long Seek(long offset, SeekOrigin origin);

	// Set the length of this stream.
	public abstract void SetLength(long value);

	// Implement the IDisposable interface.
	void IDisposable.Dispose()
			{
				Close();
			}

	// Write a buffer of bytes to this stream.
	public abstract void Write(byte[] buffer, int offset, int count);

	// Write a single byte to this stream.
	public virtual void WriteByte(byte value)
			{
				byte[] bytes = new byte[1];
				bytes[0] = value;
				Write(bytes, 0, 1);
			}

	// Determine if it is possible to read from this stream.
	public abstract bool CanRead { get; }

	// Determine if it is possible to seek within this stream.
	public abstract bool CanSeek { get; }

	// Determine if it is possible to write to this stream.
	public abstract bool CanWrite { get; }

	// Get the length of this stream.
	public abstract long Length { get; }

	// Get the current position within the stream.
	public abstract long Position { get; set; }

	// Helper methods for validating buffer arguments.
	internal static void ValidateBuffer(byte[] buffer, int offset, int count)
			{
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				else if(offset < 0)
				{
					throw new ArgumentOutOfRangeException
						("offset", _("ArgRange_Array"));
				}
				else if(count < 0)
				{
					throw new ArgumentOutOfRangeException
						("count", _("ArgRange_Array"));
				}
				else if((buffer.Length - offset) < count)
				{
					throw new ArgumentException(_("Arg_InvalidArrayRange"));
				}
			}
	internal static void ValidateBuffer(char[] buffer, int offset, int count)
			{
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				else if(offset < 0)
				{
					throw new ArgumentOutOfRangeException
						("offset", _("ArgRange_Array"));
				}
				else if(count < 0)
				{
					throw new ArgumentOutOfRangeException
						("count", _("ArgRange_Array"));
				}
				else if((buffer.Length - offset) < count)
				{
					throw new ArgumentException(_("Arg_InvalidArrayRange"));
				}
			}

	// Private class that implements null streams.
	private sealed class NullStream : Stream
	{
		// Constructor.
		public NullStream() {}

		// Stub out all stream functionality.
		public override void Flush() {}
		public override int Read(byte[] buffer, int offset, int count)
				{
					ValidateBuffer(buffer, offset, count);
					return 0;
				}
		public override int ReadByte() { return -1; }
		public override long Seek(long offset, SeekOrigin origin)
				{
					throw new NotSupportedException(_("IO_NotSupp_Seek"));
				}
		public override void SetLength(long value)
				{
					throw new NotSupportedException(_("IO_NotSupp_SetLength"));
				}
		public override void Write(byte[] buffer, int offset, int count)
				{
					ValidateBuffer(buffer, offset, count);
				}
		public override void WriteByte(byte value) {}
		public override bool CanRead { get { return true; } }
		public override bool CanSeek { get { return false; } }
		public override bool CanWrite { get { return true; } }
		public override long Length
				{
					get
					{
						throw new NotSupportedException(_("IO_NotSupp_Seek"));
					}
				}
		public override long Position
				{
					get
					{
						throw new NotSupportedException(_("IO_NotSupp_Seek"));
					}
					set
					{
						throw new NotSupportedException(_("IO_NotSupp_Seek"));
					}
				}

	}; // class NullStream

}; // class Stream

}; // namespace System.IO
