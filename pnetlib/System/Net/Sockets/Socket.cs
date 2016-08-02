/*
 * Socket.cs - Implementation of the "System.Net.Sockets.Socket" class.
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

using Platform;
using DotGNU.Platform;
using System;
using System.Private;
using System.Collections;
using System.Security;
using System.Threading;

public class Socket : IDisposable
{
	// Internal state.
	private IntPtr handle;
	private AddressFamily family;
	private SocketType socketType;
	private ProtocolType protocol;
	private bool blocking;
	private bool connected;
	private EndPoint localEP;
	private EndPoint remoteEP;
	private Object readLock;
	private BlockingOperations blockingOps;

	// Invalid socket handle.
	private static readonly IntPtr InvalidHandle =
		SocketMethods.GetInvalidHandle();

	// Constructor.
	public Socket(AddressFamily addressFamily, SocketType socketType,
				  ProtocolType protocolType)
			{
				// Validate the parameters.
				if(addressFamily == AddressFamily.Unspecified)
				{
					addressFamily = AddressFamily.InterNetwork;
				}
				else if(!SocketMethods.AddressFamilySupported
							((int)addressFamily))
				{
					throw new SocketException(Errno.EINVAL);
				}
				
				switch( socketType ) {
					
					case SocketType.Stream :
						if(protocolType == ProtocolType.Unspecified)
						{
							if(addressFamily == AddressFamily.InterNetwork ||
												  addressFamily == AddressFamily.InterNetworkV6)
							{
								protocolType = ProtocolType.Tcp;
							}
						}
						else if(protocolType != ProtocolType.Tcp)
						{
							throw new SocketException(Errno.EPROTONOSUPPORT);
						}
						break;
					
					case SocketType.Dgram :
						if(protocolType == ProtocolType.Unspecified)
						{
							if(addressFamily == AddressFamily.InterNetwork ||
												  addressFamily == AddressFamily.InterNetworkV6)
							{
								protocolType = ProtocolType.Udp;
							}
						}
						else if(protocolType != ProtocolType.Udp)
						{
							throw new SocketException(Errno.EPROTONOSUPPORT);
						}
						break;
					
					case SocketType.Raw:
						if(protocolType == ProtocolType.Unspecified)
						{
							if(addressFamily == AddressFamily.InterNetwork ||
												  addressFamily == AddressFamily.InterNetworkV6)
							{
								protocolType = ProtocolType.Raw;
							}
						}
					break;
					
					case 	SocketType.Rdm : case SocketType.Seqpacket :
						break;
						
					default: 
						throw new SocketException(Errno.EPROTONOSUPPORT);
						break;
				}
				
				// Initialize the local state.
				this.handle = InvalidHandle;
				this.family = addressFamily;
				this.socketType = socketType;
				this.protocol = protocolType;
				this.blocking = true;
				this.connected = false;
				this.localEP = null;
				this.remoteEP = null;
				this.readLock = new Object();
				this.blockingOps = new BlockingOperations();

				// Attempt to create the socket.  This may bail out for
				// some address families, even if "AddressFamilySupported"
				// returned true.  This can happen, for example, if the user
				// space definitions are available for IrDA, but the kernel
				// drivers are not.  "AddressFamilySupported" may not be
				// able to detect the kernel capabilities on all platforms.
				if(!SocketMethods.Create((int)addressFamily, (int)socketType,
										 (int)protocolType, out handle))
				{
					throw new SocketException(this.GetErrno());
				}
			}
	private Socket(AddressFamily addressFamily, SocketType socketType,
				   ProtocolType protocolType, IntPtr handle, bool blocking,
				   EndPoint remoteEP)
			{
				// Set up for a new socket that has just been accepted.
				this.handle = handle;
				this.family = addressFamily;
				this.socketType = socketType;
				this.protocol = protocolType;
				this.blocking = blocking;
				this.connected = true;
				this.localEP = null;
				this.remoteEP = remoteEP;
				this.readLock = new Object();
				this.blockingOps = new BlockingOperations();
			}

	// Destructor.
	~Socket()
			{
				Dispose(false);
			}

	// Implement the IDisposable interface.
	void IDisposable.Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

	// Get the current errno condition.
	private Errno GetErrno()
			{
				// get the errno condition
				Errno errno = SocketMethods.GetErrno();

				// disconnect as required
				switch(errno)
				{
					case Errno.EPIPE:
					case Errno.ECONNABORTED:
					case Errno.ECONNRESET:
					case Errno.ETIMEDOUT:
					case Errno.ECONNREFUSED:
					case Errno.EHOSTDOWN:
					{
						connected = false;
					}
					break;
				}

				// return the errno condition
				return errno;
			}

	// Accept an incoming connection on this socket.
	public Socket Accept()
			{
				IntPtr currentHandle;
				IntPtr newHandle;
				EndPoint remoteEP;

				// Get the socket handle, synchronized against "Close".
				lock(this)
				{
					// Bail out if the socket has been closed.
					if(handle == InvalidHandle)
					{
						throw new ObjectDisposedException
							(S._("Exception_Disposed"));
					}
					currentHandle = handle;
				}

				// Create the sockaddr buffer from the local end point.
				byte[] addrReturn = LocalEndPoint.Serialize().Array;
				Array.Clear(addrReturn, 0, addrReturn.Length);

				// Accept a new connection on the socket.  We do this outside
				// of the lock's protection so that multiple threads can
				// wait for incoming connections on the same socket.
				using(BlockingOperation op = blockingOps.NewOp())
				{
					if(!SocketMethods.Accept
						(currentHandle, addrReturn, out newHandle))
					{
						throw new SocketException(this.GetErrno());
					}
				}

				// Create the end-point object for the remote side.
				remoteEP = LocalEndPoint.Create(new SocketAddress(addrReturn));

				// Create and return a new socket object.
				return new Socket(family, socketType, protocol,
								  newHandle, blocking, remoteEP);
			}

	// Asynchronous operation types.
	private enum AsyncOperation
	{
		Accept,
		Connect,
		Receive,
		ReceiveFrom,
		Send,
		SendTo

	}; // enum AsyncOperation

	// Asynchronous operation control class.
	private sealed class AsyncControl : IAsyncResult
	{
		// Internal state.
		private WaitHandle waitHandle;
		private bool completedSynchronously;
		private bool completed;
		private AsyncOperation operation;
		private AsyncCallback callback;
		private Object state;
		private Socket socket;
		private byte[] buffer;
		private int offset;
		private int count;
		private SocketFlags flags;
		public int result;
		public Socket acceptResult;
		public EndPoint remoteEP;
		private Exception exception;

		// Constructor.
		public AsyncControl(AsyncCallback callback, Object state,
							Socket socket, byte[] buffer, int offset,
							int count, SocketFlags flags, EndPoint remoteEP,
							AsyncOperation operation)
				{
				#if ECMA_COMPAT
					this.waitHandle = SocketMethods.CreateManualResetEvent();
				#else
					this.waitHandle = new ManualResetEvent(false);
				#endif
					this.completedSynchronously = false;
					this.completed = false;
					this.operation = operation;
					this.callback = callback;
					this.state = state;
					this.socket = socket;
					this.buffer = buffer;
					this.offset = offset;
					this.count = count;
					this.flags = flags;
					this.result = -1;
					this.acceptResult = null;
					this.remoteEP = remoteEP;
					this.exception = null;
				}

		// Run the operation thread.
		private void Run(IAsyncResult state)
				{
					try
					{
						switch(operation)
						{
							case AsyncOperation.Accept:
							{
								acceptResult = socket.Accept();
							}
							break;

							case AsyncOperation.Connect:
							{
								socket.Connect(remoteEP);
							}
							break;

							case AsyncOperation.Receive:
							{
								result = socket.Receive
									(buffer, offset, count, flags);
							}
							break;

							case AsyncOperation.ReceiveFrom:
							{
								result = socket.ReceiveFrom
									(buffer, offset, count, flags,
									 ref remoteEP);
							}
							break;

							case AsyncOperation.Send:
							{
								result = socket.Send
									(buffer, offset, count, flags);
							}
							break;

							case AsyncOperation.SendTo:
							{
								result = socket.SendTo
									(buffer, offset, count, flags, remoteEP);
							}
							break;
						}
					}
					catch(Exception e)
					{
						// Save the exception to be thrown in EndXXX.
						exception = e;
					}
					completed = true;
					if(callback != null)
					{
						callback(this);
					}
					if(waitHandle != null)
					{
				#if ECMA_COMPAT
						SocketMethods.WaitHandleSet(waitHandle);
				#else
						((ManualResetEvent)waitHandle).Set();
				#endif
					}
				}

		// Start the async thread, or perform the operation synchronously.
		public void Start()
				{
					if(SocketMethods.CanStartThreads())
					{
						SocketMethods.QueueCompletionItem
							(new AsyncCallback(Run), this);
					}
					else
					{
						completedSynchronously = true;
						Run(null);
					}
				}

		// Wait for an asynchronous operation to complete.
		// Also handles exceptions that were thrown by the operation.
		public void Wait(Socket check, AsyncOperation oper)
				{
					if(socket != check || operation != oper)
					{
						throw new ArgumentException(S._("Arg_InvalidAsync"));
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

	// Begin an asynchronous operation to accept an incoming connection.
	public IAsyncResult BeginAccept(AsyncCallback callback, Object state)
			{
				// Create the result object.
				AsyncControl async = new AsyncControl
					(callback, state, this, null, 0, 0,
					 SocketFlags.None, null, AsyncOperation.Accept);

				// Start the background process.
				async.Start();
				return async;
			}

	// End an asynchronous accept operation.
	public Socket EndAccept(IAsyncResult asyncResult)
			{
				if(asyncResult == null)
				{
					throw new ArgumentNullException("asyncResult");
				}
				else if(!(asyncResult is AsyncControl))
				{
					throw new ArgumentException(S._("Arg_InvalidAsync"));
				}
				else
				{
					AsyncControl async = (AsyncControl)asyncResult;
					async.Wait(this, AsyncOperation.Accept);
					return async.acceptResult;
				}
			}

	// Begin an asynchronous operation to connect on this socket.
	public IAsyncResult BeginConnect(EndPoint remoteEP,
									 AsyncCallback callback,
									 Object state)
			{
				// Validate the parameters.
				if(remoteEP == null)
				{
					throw new ArgumentNullException("remoteEP");
				}
				else if(remoteEP.AddressFamily != family)
				{
					throw new SocketException(Errno.EINVAL);
				}

				// Create the result object.
				AsyncControl async = new AsyncControl
					(callback, state, this, null, 0, 0,
					 SocketFlags.None, remoteEP, AsyncOperation.Connect);

				// Start the background process.
				async.Start();
				return async;
			}

	// End an asynchronous connect operation.
	public void EndConnect(IAsyncResult asyncResult)
			{
				if(asyncResult == null)
				{
					throw new ArgumentNullException("asyncResult");
				}
				else if(!(asyncResult is AsyncControl))
				{
					throw new ArgumentException(S._("Arg_InvalidAsync"));
				}
				else
				{
					((AsyncControl)asyncResult).Wait
						(this, AsyncOperation.Connect);
				}
			}

	// Begin an asynchronous operation to receive on this socket.
	public IAsyncResult BeginReceive(byte[] buffer, int offset, int size,
									 SocketFlags socketFlags,
									 AsyncCallback callback,
									 Object state)
			{
				// Validate the parameters.
				ValidateBuffer(buffer, offset, size);

				// Create the result object.
				AsyncControl async = new AsyncControl
					(callback, state, this, buffer, offset, size,
					 socketFlags, null, AsyncOperation.Receive);

				// Start the background process.
				async.Start();
				return async;
			}

	// End an asynchronous receive operation.
	public int EndReceive(IAsyncResult asyncResult)
			{
				if(asyncResult == null)
				{
					throw new ArgumentNullException("asyncResult");
				}
				else if(!(asyncResult is AsyncControl))
				{
					throw new ArgumentException(S._("Arg_InvalidAsync"));
				}
				else
				{
					AsyncControl async = (AsyncControl)asyncResult;
					async.Wait(this, AsyncOperation.Receive);
					return async.result;
				}
			}

	// Begin an asynchronous operation to receive from this socket.
	public IAsyncResult BeginReceiveFrom(byte[] buffer, int offset, int size,
									 	 SocketFlags socketFlags,
										 ref EndPoint remoteEP,
									 	 AsyncCallback callback,
									 	 Object state)
			{
				// Validate the parameters.
				ValidateBuffer(buffer, offset, size);
				if(remoteEP == null)
				{
					throw new ArgumentNullException("remoteEP");
				}
				else if(remoteEP.AddressFamily != family)
				{
					throw new SocketException(Errno.EINVAL);
				}

				// Create the result object.
				AsyncControl async = new AsyncControl
					(callback, state, this, buffer, offset, size,
					 socketFlags, remoteEP, AsyncOperation.ReceiveFrom);

				// Start the background process.
				async.Start();
				return async;
			}

	// End an asynchronous receive from operation.
	public int EndReceiveFrom(IAsyncResult asyncResult, ref EndPoint endPoint)
			{
				if(asyncResult == null)
				{
					throw new ArgumentNullException("asyncResult");
				}
				else if(!(asyncResult is AsyncControl))
				{
					throw new ArgumentException(S._("Arg_InvalidAsync"));
				}
				else
				{
					AsyncControl async = (AsyncControl)asyncResult;
					async.Wait(this, AsyncOperation.ReceiveFrom);
					endPoint = async.remoteEP;
					return async.result;
				}
			}

	// Begin an asynchronous operation to send on this socket.
	public IAsyncResult BeginSend(byte[] buffer, int offset, int size,
								  SocketFlags socketFlags,
								  AsyncCallback callback,
								  Object state)
			{
				// Validate the parameters.
				ValidateBuffer(buffer, offset, size);

				// Create the result object.
				AsyncControl async = new AsyncControl
					(callback, state, this, buffer, offset, size,
					 socketFlags, null, AsyncOperation.Send);

				// Start the background process.
				async.Start();
				return async;
			}

	// End an asynchronous send operation.
	public int EndSend(IAsyncResult asyncResult)
			{
				if(asyncResult == null)
				{
					throw new ArgumentNullException("asyncResult");
				}
				else if(!(asyncResult is AsyncControl))
				{
					throw new ArgumentException(S._("Arg_InvalidAsync"));
				}
				else
				{
					AsyncControl async = (AsyncControl)asyncResult;
					async.Wait(this, AsyncOperation.Send);
					return async.result;
				}
			}

	// Begin an asynchronous operation to send from this socket.
	public IAsyncResult BeginSendTo(byte[] buffer, int offset, int size,
								 	SocketFlags socketFlags,
									EndPoint remoteEP,
								 	AsyncCallback callback,
								 	Object state)
			{
				// Validate the parameters.
				ValidateBuffer(buffer, offset, size);
				if(remoteEP == null)
				{
					throw new ArgumentNullException("remoteEP");
				}
				else if(remoteEP.AddressFamily != family)
				{
					throw new SocketException(Errno.EINVAL);
				}

				// Create the result object.
				AsyncControl async = new AsyncControl
					(callback, state, this, buffer, offset, size,
					 socketFlags, remoteEP, AsyncOperation.SendTo);

				// Start the background process.
				async.Start();
				return async;
			}

	// End an asynchronous send to operation.
	public int EndSendTo(IAsyncResult asyncResult)
			{
				if(asyncResult == null)
				{
					throw new ArgumentNullException("asyncResult");
				}
				else if(!(asyncResult is AsyncControl))
				{
					throw new ArgumentException(S._("Arg_InvalidAsync"));
				}
				else
				{
					AsyncControl async = (AsyncControl)asyncResult;
					async.Wait(this, AsyncOperation.SendTo);
					return async.result;
				}
			}

	// Bind this socket to a specific end-point.
	public void Bind(EndPoint localEP)
			{
				// Validate the parameter.
				if(localEP == null)
				{
					throw new ArgumentNullException("localEP");
				}
				else if(localEP.AddressFamily != family)
				{
					throw new SocketException(Errno.EINVAL);
				}

				// Convert the end point into a sockaddr buffer.
				byte[] addr = localEP.Serialize().Array;

				// Lock down the socket object while we do the bind.
				lock(this)
				{
					// Bail out if the socket has been closed.
					if(handle == InvalidHandle)
					{
						throw new ObjectDisposedException
							(S._("Exception_Disposed"));
					}

					// Bind the address to the socket.
					if(!SocketMethods.Bind(handle, addr))
					{
						throw new SocketException(this.GetErrno());
					}

					// Record the local end point for later.
					this.localEP = localEP;
				}
			}

	// Close this socket.
	public void Close()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

	// Connect to a remote end-point.
	public void Connect(EndPoint remoteEP)
			{
				// Validate the parameter.
				if(remoteEP == null)
				{
					throw new ArgumentNullException("remoteEP");
				}
				else if(remoteEP.AddressFamily != family)
				{
					throw new SocketException(Errno.EINVAL);
				}

				// Convert the end point into a sockaddr buffer.
				byte[] addr = remoteEP.Serialize().Array;

				// Lock down the socket object while we do the connect.
				lock(this)
				{
					// Bail out if the socket has been closed.
					if(handle == InvalidHandle)
					{
						throw new ObjectDisposedException
							(S._("Exception_Disposed"));
					}

					// Connect to the foreign location.
					using(BlockingOperation op = blockingOps.NewOp())
					{
						if(!SocketMethods.Connect(handle, addr))
						{
							throw new SocketException(this.GetErrno());
						}
					}
					connected = true;
					this.remoteEP = remoteEP;
				}
			}

	// Dispose this socket.
	protected virtual void Dispose(bool disposing)
			{
				lock(this)
				{
					if(handle != InvalidHandle)
					{
						SocketMethods.Close(handle);
						handle = InvalidHandle;
						blockingOps.Abort();
					}
				}
			}

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				return base.GetHashCode();
			}

	// Get a raw numeric socket option.
	private int GetSocketOptionRaw(SocketOptionLevel optionLevel,
								   SocketOptionName optionName)
			{
				int optionValue;
				lock(this)
				{
					if(handle == InvalidHandle)
					{
						throw new ObjectDisposedException
							(S._("Exception_Disposed"));
					}
					if(!SocketMethods.GetSocketOption
							(handle, (int)optionLevel, (int)optionName,
							 out optionValue))
					{
						throw new SocketException(this.GetErrno());
					}
					return optionValue;
				}
			}

	// Get an option on this socket.
	public Object GetSocketOption(SocketOptionLevel optionLevel,
								  SocketOptionName optionName)
			{
				bool enabled;
				int seconds;

				// Validate the option information to ensure that the
				// caller is not trying to get something that is insecure.
				if(optionLevel == SocketOptionLevel.Socket)
				{
					if(optionName == SocketOptionName.KeepAlive ||
					   optionName == SocketOptionName.ReceiveTimeout ||
					   optionName == SocketOptionName.SendTimeout ||
					   optionName == SocketOptionName.ReceiveBuffer ||
					   optionName == SocketOptionName.SendBuffer ||
					   optionName == SocketOptionName.ReuseAddress)
					{
						return GetSocketOptionRaw(optionLevel, optionName);
					}
					if(optionName == SocketOptionName.DontLinger)
					{
						// Get the linger information and test it.
						lock(this)
						{
							if(handle == InvalidHandle)
							{
								throw new ObjectDisposedException
									(S._("Exception_Disposed"));
							}
							if(!SocketMethods.GetLingerOption
									(handle, out enabled, out seconds))
							{
								throw new SocketException
									(this.GetErrno());
							}
							return ((enabled && seconds == 0) ? 1 : 0);
						}
					}
					if(optionName == SocketOptionName.Linger)
					{
						// Get the linger information.
						lock(this)
						{
							if(handle == InvalidHandle)
							{
								throw new ObjectDisposedException
									(S._("Exception_Disposed"));
							}
							if(!SocketMethods.GetLingerOption
									(handle, out enabled, out seconds))
							{
								throw new SocketException
									(this.GetErrno());
							}
							return new LingerOption(enabled, seconds);
						}
					}
					if(optionName == SocketOptionName.ExclusiveAddressUse)
					{
						// Flip the option and turn it into "ReuseAddress".
						int reuse = GetSocketOptionRaw
							(optionLevel, SocketOptionName.ReuseAddress);
						return ((reuse != 0) ? 0 : 1);
					}
					if(optionName == SocketOptionName.Type)
					{
						// Return the socket type.
						return (int)socketType;
					}
				}
				else if(optionLevel == SocketOptionLevel.Tcp)
				{
					if(optionName == SocketOptionName.NoDelay ||
					   optionName == SocketOptionName.Expedited)
					{
						return GetSocketOptionRaw(optionLevel, optionName);
					}
				}
				else if(optionLevel == SocketOptionLevel.Udp)
				{
					if(optionName == SocketOptionName.NoChecksum ||
					   optionName == SocketOptionName.ChecksumCoverage)
					{
						return GetSocketOptionRaw(optionLevel, optionName);
					}
				}
				else if(optionLevel == SocketOptionLevel.IP)
				{
					if(optionName == SocketOptionName.AddMembership ||
					   optionName == SocketOptionName.DropMembership)
					{
						// Get the contents of the multicast membership set.
						byte[] group;
						byte[] mcint;
						lock(this)
						{
							if(handle == InvalidHandle)
							{
								throw new ObjectDisposedException
									(S._("Exception_Disposed"));
							}
							group = new byte [32];
							mcint = new byte [32];
							if(!SocketMethods.GetMulticastOption
									(handle, (int)family, (int)optionName,
									 group, mcint))
							{
								throw new SocketException
									(this.GetErrno());
							}
							return new MulticastOption
								((new SocketAddress(group)).IPAddress,
								 (new SocketAddress(mcint)).IPAddress);
						}
					}
				}
				throw new SecurityException(S._("Arg_SocketOption"));
			}
	public void GetSocketOption(SocketOptionLevel optionLevel,
								SocketOptionName optionName,
								byte[] optionValue)
			{
				// This version is not portable - do not use.
				throw new SecurityException(S._("Arg_SocketOption"));
			}
	public byte[] GetSocketOption(SocketOptionLevel optionLevel,
								  SocketOptionName optionName,
								  int optionLength)
			{
				// Check for the IrDA device discovery option.
				if(optionLevel == (SocketOptionLevel)255 &&
				   optionName == (SocketOptionName)16)
				{
					lock(this)
					{
						if(handle == InvalidHandle)
						{
							throw new ObjectDisposedException
								(S._("Exception_Disposed"));
						}
						else if(family != AddressFamily.Irda)
						{
							throw new SocketException(Errno.EINVAL);
						}
						byte[] data = new byte [optionLength];
						using(BlockingOperation op = blockingOps.NewOp())
						{
							if(!SocketMethods.DiscoverIrDADevices
									(handle, data))
							{
								throw new SocketException
									(this.GetErrno());
							}
						}
						return data;
					}
				}

				// Everything else is non-portable - do not use.
				throw new SecurityException(S._("Arg_SocketOption"));
			}

	// Perform an "ioctl" operation on this socket.
	public int IOControl(int ioControlCode, byte[] optionInValue,
						 byte[] optionOutValue)
			{
				// We don't support any "ioctl" operations in this
				// implementation because they aren't portable or they
				// are inherently insecure (e.g. changing process groups).
				// The two most interesting ones (FIONBIO and FIONREAD)
				// can be accessed using "Blocking" and "Available" instead.
				throw new SocketException(Errno.EINVAL);
			}

	// Perform a listen operation on this socket.
	public void Listen(int backlog)
			{
				lock(this)
				{
					// Bail out if the socket has been closed.
					if(handle == InvalidHandle)
					{
						throw new ObjectDisposedException
							(S._("Exception_Disposed"));
					}

					// Perform a listen on the socket.
					if(!SocketMethods.Listen(handle, backlog))
					{
						throw new SocketException(this.GetErrno());
					}
				}
			}

	// Poll the select status of the socket.
	public bool Poll(int microSeconds, SelectMode mode)
			{
				IntPtr[] array;
				int result;

				// Create an array that contains the socket's handle.
				array = new IntPtr [1];
				array[0] = GetHandle(this);

				// Perform the select.
				using(BlockingOperation op = blockingOps.NewOp())
				{
					switch(mode)
					{
						case SelectMode.SelectRead:
						{ 
							result = SocketMethods.Select
								(array, null, null, (long)microSeconds);
						}
						break;
	
						case SelectMode.SelectWrite:
						{
							result = SocketMethods.Select
								(null, array, null, (long)microSeconds);
						}
						break;
	
						case SelectMode.SelectError:
						{
							result = SocketMethods.Select
								(null, null, array, (long)microSeconds);
						}
						break;
	
						default:
						{
							throw new NotSupportedException
								(S._("NotSupp_SelectMode"));
						}
						// Not reached.
					}
				}

				// Decode the result and return.
				if(result == 0)
				{
					return false;
				}
				else if(result < 0)
				{
					throw new SocketException(this.GetErrno());
				}
				else
				{
					return true;
				}
			}

	// Receive data on this socket.
	public int Receive(byte[] buffer, int offset, int size,
					   SocketFlags socketFlags)
			{
				int result;

				// Validate the arguments.
				ValidateBuffer(buffer, offset, size);

				// Perform the receive operation.
				lock(readLock)
				{
					if(handle == InvalidHandle)
					{
						throw new ObjectDisposedException
							(S._("Exception_Disposed"));
					}
					using(BlockingOperation op = blockingOps.NewOp())
					{
						result = SocketMethods.Receive
							(handle, buffer, offset, size, (int)socketFlags);
					}
					if(result < 0)
					{
						throw new SocketException(this.GetErrno());
					}
					else
					{
						return result;
					}
				}
			}
	public int Receive(byte[] buffer, int size, SocketFlags socketFlags)
			{
				return Receive(buffer, 0, size, socketFlags);
			}
	public int Receive(byte[] buffer, SocketFlags socketFlags)
			{
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				return Receive(buffer, 0, buffer.Length, socketFlags);
			}
	public int Receive(byte[] buffer)
			{
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				return Receive(buffer, 0, buffer.Length, SocketFlags.None);
			}

	// Receive data on this socket and record where it came from.
	public int ReceiveFrom(byte[] buffer, int offset, int size,
					       SocketFlags socketFlags, ref EndPoint remoteEP)
			{
				int result;
				byte[] addrReturn;

				// Validate the arguments.
				ValidateBuffer(buffer, offset, size);
				if(remoteEP == null)
				{
					throw new ArgumentNullException("remoteEP");
				}
				else if(remoteEP.AddressFamily != family)
				{
					throw new SocketException(Errno.EINVAL);
				}

				// Create a sockaddr buffer to write the address into.
				addrReturn = remoteEP.Serialize().Array;
				Array.Clear(addrReturn, 0, addrReturn.Length);

				// Perform the receive operation.
				lock(readLock)
				{
					if(handle == InvalidHandle)
					{
						throw new ObjectDisposedException
							(S._("Exception_Disposed"));
					}
					using(BlockingOperation op = blockingOps.NewOp())
					{
						result = SocketMethods.ReceiveFrom
							(handle, buffer, offset, size,
							(int)socketFlags, addrReturn);
					}
					if(result < 0)
					{
						throw new SocketException(this.GetErrno());
					}
					else
					{
						remoteEP = remoteEP.Create
							(new SocketAddress(addrReturn));
						return result;
					}
				}
			}
	public int ReceiveFrom(byte[] buffer, int size, SocketFlags socketFlags,
						   ref EndPoint remoteEP)
			{
				return ReceiveFrom(buffer, 0, size, socketFlags, ref remoteEP);
			}
	public int ReceiveFrom(byte[] buffer, SocketFlags socketFlags,
						   ref EndPoint remoteEP)
			{
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				return ReceiveFrom(buffer, 0, buffer.Length,
								   socketFlags, ref remoteEP);
			}
	public int ReceiveFrom(byte[] buffer, ref EndPoint remoteEP)
			{
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				return ReceiveFrom(buffer, 0, buffer.Length,
								   SocketFlags.None, ref remoteEP);
			}

	// Perform a select operation on a group of sockets.
	public static void Select(IList checkRead, IList checkWrite,
							  IList checkError, int microSeconds)
			{
				int posn, posn2, result;
				IntPtr[] readArray;
				IntPtr[] writeArray;
				IntPtr[] errorArray;

				// Validate the parameters.
				if((checkRead == null || checkRead.Count == 0) &&
				   (checkWrite == null || checkWrite.Count == 0) &&
				   (checkError == null || checkError.Count == 0))
				{
					throw new ArgumentNullException("all");
				}

				// Convert the lists into socket handle arrays.
				if(checkRead != null)
				{
					readArray = new IntPtr [checkRead.Count];
					for(posn = 0; posn < checkRead.Count; ++posn)
					{
						readArray[posn] = GetHandle(checkRead[posn]);
					}
				}
				else
				{
					readArray = null;
				}
				if(checkWrite != null)
				{
					writeArray = new IntPtr [checkWrite.Count];
					for(posn = 0; posn < checkWrite.Count; ++posn)
					{
						writeArray[posn] = GetHandle(checkWrite[posn]);
					}
				}
				else
				{
					writeArray = null;
				}
				if(checkError != null)
				{
					errorArray = new IntPtr [checkError.Count];
					for(posn = 0; posn < checkError.Count; ++posn)
					{
						errorArray[posn] = GetHandle(checkError[posn]);
					}
				}
				else
				{
					errorArray = null;
				}

				// Perform the select.
				result = SocketMethods.Select
					(readArray, writeArray, errorArray, (long)microSeconds);

				// Decode the result.  If the list is fixed-size,
				// then we set the removed elements to null; otherwise
				// we remove the elements with "IList.RemoveAt".
				if(result == 0)
				{
					// A timeout occurred, so clear all return sets.
					if(checkRead != null)
					{
						if(!(checkRead.IsFixedSize))
						{
							checkRead.Clear();
						}
						else
						{
							for(posn = 0; posn < checkRead.Count; ++posn)
							{
								checkRead[posn] = null;
							}
						}
					}
					if(checkWrite != null)
					{
						if(!(checkWrite.IsFixedSize))
						{
							checkWrite.Clear();
						}
						else
						{
							for(posn = 0; posn < checkWrite.Count; ++posn)
							{
								checkWrite[posn] = null;
							}
						}
					}
					if(checkError != null)
					{
						if(!(checkError.IsFixedSize))
						{
							checkError.Clear();
						}
						else
						{
							for(posn = 0; posn < checkError.Count; ++posn)
							{
								checkError[posn] = null;
							}
						}
					}
					return;
				}
				else if(result < 0)
				{
					// Some kind of error occurred.
					throw new SocketException(SocketMethods.GetErrno());
				}

				// Ordinary return: update the sets to reflect the result.
				if(checkRead != null)
				{
					if(!(checkRead.IsFixedSize))
					{
						posn2 = 0;
						for(posn = 0; posn < readArray.Length; ++posn)
						{
							if(readArray[posn] == InvalidHandle)
							{
								checkRead.RemoveAt(posn2);
							}
							else
							{
								++posn2;
							}
						}
					}
					else
					{
						for(posn = 0; posn < readArray.Length; ++posn)
						{
							if(readArray[posn] == InvalidHandle)
							{
								checkRead[posn] = null;
							}
						}
					}
				}
				if(checkWrite != null)
				{
					if(!(checkWrite.IsFixedSize))
					{
						posn2 = 0;
						for(posn = 0; posn < writeArray.Length; ++posn)
						{
							if(writeArray[posn] == InvalidHandle)
							{
								checkWrite.RemoveAt(posn2);
							}
							else
							{
								++posn2;
							}
						}
					}
					else
					{
						for(posn = 0; posn < writeArray.Length; ++posn)
						{
							if(writeArray[posn] == InvalidHandle)
							{
								checkWrite[posn] = null;
							}
						}
					}
				}
				if(checkError != null)
				{
					if(!(checkError.IsFixedSize))
					{
						posn2 = 0;
						for(posn = 0; posn < errorArray.Length; ++posn)
						{
							if(errorArray[posn] == InvalidHandle)
							{
								checkError.RemoveAt(posn2);
							}
							else
							{
								++posn2;
							}
						}
					}
					else
					{
						for(posn = 0; posn < errorArray.Length; ++posn)
						{
							if(errorArray[posn] == InvalidHandle)
							{
								checkError[posn] = null;
							}
						}
					}
				}
			}

	// Send data on this socket.
	public int Send(byte[] buffer, int offset, int size,
					SocketFlags socketFlags)
			{
				int result;

				// Validate the arguments.
				ValidateBuffer(buffer, offset, size);

				// Perform the send operation.
				lock(this)
				{
					if(handle == InvalidHandle)
					{
						throw new ObjectDisposedException
							(S._("Exception_Disposed"));
					}
					using(BlockingOperation op = blockingOps.NewOp())
					{
						result = SocketMethods.Send
							(handle, buffer, offset, size, (int)socketFlags);
					}
					if(result < 0)
					{
						throw new SocketException(this.GetErrno());
					}
					else
					{
						return result;
					}
				}
			}
	public int Send(byte[] buffer, int size, SocketFlags socketFlags)
			{
				return Send(buffer, 0, size, socketFlags);
			}
	public int Send(byte[] buffer, SocketFlags socketFlags)
			{
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				return Send(buffer, 0, buffer.Length, socketFlags);
			}
	public int Send(byte[] buffer)
			{
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				return Send(buffer, 0, buffer.Length, SocketFlags.None);
			}

	// Send data on this socket to a specific location.
	public int SendTo(byte[] buffer, int offset, int size,
					  SocketFlags socketFlags, EndPoint remoteEP)
			{
				int result;
				byte[] addr;

				// Validate the arguments.
				ValidateBuffer(buffer, offset, size);
				if(remoteEP == null)
				{
					throw new ArgumentNullException("remoteEP");
				}
				else if(remoteEP.AddressFamily != family)
				{
					throw new SocketException(Errno.EINVAL);
				}

				// Convert the end point into a sockaddr buffer.
				addr = remoteEP.Serialize().Array;

				// Perform the send operation.
				lock(this)
				{
					if(handle == InvalidHandle)
					{
						throw new ObjectDisposedException
							(S._("Exception_Disposed"));
					}
					using(BlockingOperation op = blockingOps.NewOp())
					{
						result = SocketMethods.SendTo
							(handle, buffer, offset, size,
							 (int)socketFlags, addr);
					}
					if(result < 0)
					{
						throw new SocketException(this.GetErrno());
					}
					else
					{
						return result;
					}
				}
			}
	public int SendTo(byte[] buffer, int size, SocketFlags socketFlags,
					  EndPoint remoteEP)
			{
				return SendTo(buffer, 0, size, socketFlags, remoteEP);
			}
	public int SendTo(byte[] buffer, SocketFlags socketFlags,
					  EndPoint remoteEP)
			{
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				return SendTo(buffer, 0, buffer.Length,
							   socketFlags, remoteEP);
			}
	public int SendTo(byte[] buffer, EndPoint remoteEP)
			{
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				return SendTo(buffer, 0, buffer.Length,
							  SocketFlags.None, remoteEP);
			}

	// Set a raw numeric socket option.
	private void SetSocketOptionRaw(SocketOptionLevel optionLevel,
									SocketOptionName optionName,
									int optionValue)
			{
				lock(this)
				{
					if(handle == InvalidHandle)
					{
						throw new ObjectDisposedException
							(S._("Exception_Disposed"));
					}
					if(!SocketMethods.SetSocketOption
							(handle, (int)optionLevel, (int)optionName,
							 optionValue))
					{
						throw new SocketException(this.GetErrno());
					}
				}
			}

	// Set an option on this socket.
	public void SetSocketOption(SocketOptionLevel optionLevel,
								SocketOptionName optionName,
								int optionValue)
			{
				// Validate the option information to ensure that the
				// caller is not trying to set something that is insecure.
				if(optionLevel == SocketOptionLevel.Socket)
				{
					if(optionName == SocketOptionName.KeepAlive ||
					   optionName == SocketOptionName.ReceiveBuffer ||
					   optionName == SocketOptionName.SendBuffer ||
					   optionName == SocketOptionName.ReuseAddress ||
					   optionName == SocketOptionName.Broadcast ||
					   optionName == SocketOptionName.ReceiveTimeout ||
					   optionName == SocketOptionName.SendTimeout)
					{
						SetSocketOptionRaw(optionLevel, optionName,
										   optionValue);
						return;
					}
					if(optionName == SocketOptionName.DontLinger)
					{
						// Convert "DontLinger" into a "Linger" call.
						LingerOption linger =
							new LingerOption(optionValue != 0, 0);
						SetSocketOption(optionLevel,
										SocketOptionName.Linger,
										linger);
						return;
					}
					if(optionName == SocketOptionName.ExclusiveAddressUse)
					{
						// Flip the option and turn it into "ReuseAddress".
						SetSocketOptionRaw(optionLevel,
										   SocketOptionName.ReuseAddress,
										   (optionValue == 0) ? 1 : 0);
						return;
					}
				}
				else if(optionLevel == SocketOptionLevel.Tcp)
				{
					if(optionName == SocketOptionName.NoDelay ||
					   optionName == SocketOptionName.Expedited)
					{
						SetSocketOptionRaw(optionLevel, optionName,
										   optionValue);
						return;
					}
				}
				else if(optionLevel == SocketOptionLevel.Udp)
				{
					if(optionName == SocketOptionName.NoChecksum ||
					   optionName == SocketOptionName.ChecksumCoverage)
					{
						SetSocketOptionRaw(optionLevel, optionName,
										   optionValue);
						return;
					}
				}
				throw new SecurityException(S._("Arg_SocketOption"));
			}
	public void SetSocketOption(SocketOptionLevel optionLevel,
								SocketOptionName optionName,
								byte[] optionValue)
			{
				// We don't support any options with byte[] values.
				throw new SecurityException(S._("Arg_SocketOption"));
			}
	public void SetSocketOption(SocketOptionLevel optionLevel,
								SocketOptionName optionName,
								Object optionValue)
			{
				// Validate the option information to ensure that the
				// caller is not trying to set something that is insecure.
				if(optionValue == null)
				{
					throw new ArgumentNullException("optionValue");
				}
				if(optionLevel == SocketOptionLevel.Socket)
				{
					if(optionName == SocketOptionName.Linger)
					{
						// Modify the linger option.
						if(!(optionValue is LingerOption))
						{
							throw new ArgumentException
								(S._("Arg_SocketOptionValue"));
						}
						LingerOption linger = (LingerOption)optionValue;
						if(linger.LingerTime < 0 ||
						   linger.LingerTime > UInt16.MaxValue)
						{
							throw new ArgumentException
								(S._("Arg_SocketOptionValue"));
						}
						lock(this)
						{
							if(handle == InvalidHandle)
							{
								throw new ObjectDisposedException
									(S._("Exception_Disposed"));
							}
							if(!SocketMethods.SetLingerOption
									(handle, linger.Enabled,
									 linger.LingerTime))
							{
								throw new SocketException
									(this.GetErrno());
							}
						}
						return;
					}
				}
				else if(optionLevel == SocketOptionLevel.IP)
				{
					if(optionName == SocketOptionName.AddMembership ||
					   optionName == SocketOptionName.DropMembership)
					{
						// Modify the multicast membership set.
						if(!(optionValue is MulticastOption))
						{
							throw new ArgumentException
								(S._("Arg_SocketOptionValue"));
						}
						MulticastOption multicast =
							(MulticastOption)optionValue;
						byte[] group =
							(new IPEndPoint(multicast.Group, 0))
								.Serialize().Array;
						byte[] mcint =
							(new IPEndPoint(multicast.LocalAddress, 0))
								.Serialize().Array;
						lock(this)
						{
							if(handle == InvalidHandle)
							{
								throw new ObjectDisposedException
									(S._("Exception_Disposed"));
							}
							if(!SocketMethods.SetMulticastOption
									(handle, (int)family, (int)optionName,
									 group, mcint))
							{
								throw new SocketException
									(this.GetErrno());
							}
						}
						return;
					}
				}
				throw new ArgumentException(S._("Arg_SocketOption"));
			}

	// Perform a shutdown on one or either socket direction.
	public void Shutdown(SocketShutdown how)
			{
				lock(this)
				{
					if(handle == InvalidHandle)
					{
						throw new ObjectDisposedException
							(S._("Exception_Disposed"));
					}
					if(!SocketMethods.Shutdown(handle, (int)how))
					{
						throw new SocketException(this.GetErrno());
					}
				}
			}
		
	// Determine if IPv4 or IPv6 is supported.
	public static bool SupportsIPv4
			{
				get
				{
					return SocketMethods.AddressFamilySupported
						((int)AddressFamily.InterNetwork);
				}
			}
	public static bool SupportsIPv6
			{
				get
				{
					return SocketMethods.AddressFamilySupported
						((int)AddressFamily.InterNetworkV6);
				}
			}

	// Get the address family for this socket.
	public AddressFamily AddressFamily
			{
				get
				{
					return family;
				}
			}

	// Get the number of bytes that are available for reading.
	public int Available
			{
				get
				{
					lock(readLock)
					{
						if(handle == InvalidHandle)
						{
							throw new ObjectDisposedException
								(S._("Exception_Disposed"));
						}
						int result = SocketMethods.GetAvailable(handle);
						if(result < 0)
						{
							throw new SocketException
								(this.GetErrno());
						}
						else
						{
							return result;
						}
					}
				}
			}

	// Get or set the blocking state on this socket.
	public bool Blocking
			{
				get
				{
					return blocking;
				}
				set
				{
					lock(this)
					{
						if(handle == InvalidHandle)
						{
							throw new ObjectDisposedException
								(S._("Exception_Disposed"));
						}
						if(blocking != value)
						{
							blocking = value;
							SocketMethods.SetBlocking(handle, value);
						}
					}
				}
			}

	// Determine if this socket is connected.
	public bool Connected
			{
				get
				{
					return connected;
				}
			}

	// Get the operating system handle for this socket.
	public IntPtr Handle
			{
				get
				{
					return handle;
				}
			}

	// Get the operating system handle for a socket object,
	// bailing out if the object is not a valid socket.
	private static IntPtr GetHandle(Object obj)
			{
				Socket socket = (obj as Socket);
				if(socket == null)
				{
					throw new ArgumentException(S._("Arg_NotSocket"));
				}
				lock(socket)
				{
					if(socket.handle == InvalidHandle)
					{
						throw new ArgumentException(S._("Arg_NotSocket"));
					}
					return socket.handle;
				}
			}

	// Get the local end-point in use by this socket.
	public EndPoint LocalEndPoint
			{
				get
				{
					byte[] addrReturn;
					EndPoint ep;
					lock(this)
					{
						if(handle == InvalidHandle)
						{
							throw new ObjectDisposedException
								(S._("Exception_Disposed"));
						}
						if(localEP != null)
						{
							return localEP;
						}

						// Get a sockaddr buffer of the right size.
						ep = remoteEP;
						if(ep == null)
						{
							// We don't have a remote end-point to guess
							// the size from, so check for known families.
							if(family == AddressFamily.InterNetwork)
							{
								ep = new IPEndPoint(IPAddress.Any, 0);
							}
							else if(family == AddressFamily.InterNetworkV6)
							{
								ep = new IPEndPoint(IPAddress.IPv6Any, 0);
							}
							else
							{
								throw new SocketException(Errno.EINVAL);
							}
						}
						addrReturn = ep.Serialize().Array;
						Array.Clear(addrReturn, 0, addrReturn.Length);

						// Get the name of the socket.
						if(!SocketMethods.GetSockName(handle, addrReturn))
						{
							throw new SocketException
								(this.GetErrno());
						}

						// Create a new end-point object using addrReturn.
						localEP = ep.Create(new SocketAddress(addrReturn));
						return localEP;
					}
				}
			}

	// Get the protocol type in use by this socket.
	public ProtocolType ProtocolType
			{
				get
				{
					return protocol;
				}
			}

	// Get the remote end-point in use by this socket.
	public EndPoint RemoteEndPoint
			{
				get
				{
					lock(this)
					{
						if(handle == InvalidHandle)
						{
							throw new ObjectDisposedException
								(S._("Exception_Disposed"));
						}
						if(remoteEP == null)
						{
							throw new SocketException(Errno.ENOTCONN);
						}
						return remoteEP;
					}
				}
			}

	// Get the type of this socket.
	public SocketType SocketType
			{
				get
				{
					return socketType;
				}
			}

	// Helper function for validating buffer arguments.
	private static void ValidateBuffer
				(byte[] buffer, int offset, int size)
			{
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				else if(offset < 0 || offset > buffer.Length)
				{
					throw new ArgumentOutOfRangeException
						("offset", S._("ArgRange_Array"));
				}
				else if(size < 0)
				{
					throw new ArgumentOutOfRangeException
						("size", S._("ArgRange_Array"));
				}
				else if((buffer.Length - offset) < size)
				{
					throw new ArgumentException(S._("Arg_InvalidArrayRange"));
				}
			}

}; // class Socket

}; // namespace System.Net.Sockets
