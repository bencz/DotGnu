/*
 * Dns.cs - Implementation of the "System.Net.Dns" class.
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

namespace System.Net
{

using Platform;
using System;
using System.Runtime.CompilerServices;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public sealed class Dns
{
	// Asynchronous operation types.
	private enum DnsOperation
	{
		Resolve ,
		GetHostByName

	}; // enum DnsOperation

	private sealed class DnsAsyncResult : IAsyncResult
	{
		// Internal state.
		private WaitHandle waitHandle;
		private bool completedSynchronously;
		private bool completed;
		private DnsOperation operation;
		private AsyncCallback callback;
		private Object state;
		private IPHostEntry acceptResult;
		private Exception exception;

		// Constructor.
		public DnsAsyncResult(AsyncCallback callback, Object state,
								DnsOperation operation)
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
					this.acceptResult = null;
					this.exception = null;
				}

		// Run the operation thread.
		public void BeginInvoke(String hostName)
				{
					try
					{
						switch(operation)
						{
							case DnsOperation.GetHostByName:
							{
								acceptResult = Dns.GetHostByName(hostName);
							}
							break;

							case DnsOperation.Resolve:
							{
								acceptResult = Dns.Resolve(hostName);
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
				#if ECMA_COMPAT
					SocketMethods.WaitHandleSet(waitHandle);
				#else
					((ManualResetEvent)waitHandle).Set();
				#endif
				}

		public IPHostEntry EndInvoke()
				{
					if(exception != null)
					{
						throw exception;
					}
					if(completed)
					{
						return acceptResult;
					}
					throw new NotImplementedException(
								"TODO: Threaded Asynchronous Operations");
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
	}; // class DnsAsyncResult

	// Cannot instantiate this class.
	private Dns () {}

	// Begin an asynchronous "get host by name" operation.
	[TODO]
	public static IAsyncResult BeginGetHostByName
		(String hostName, AsyncCallback requestedCallback,
		 Object stateObject)
			{
				// TODO
				DnsAsyncResult result=new DnsAsyncResult(requestedCallback,
									stateObject,DnsOperation.GetHostByName);
				result.BeginInvoke(hostName);
				return (IAsyncResult) (result);				
			}

	// Begin an asynchronous name or IP resolution operation.
	[TODO]
	public static IAsyncResult BeginResolve
		(String hostName, AsyncCallback requestedCallback,
		 Object stateObject)
			{
				// TODO
				DnsAsyncResult result=new DnsAsyncResult(requestedCallback,
									stateObject,DnsOperation.Resolve);
				result.BeginInvoke(hostName);
				return (IAsyncResult) (result);				
			}

	// End an asynchronous "get host by name" operation.
	[TODO]
	public static IPHostEntry EndGetHostByName(IAsyncResult asyncResult)
			{
				// TODO
				return ((DnsAsyncResult)(asyncResult)).EndInvoke();
			}

	// End an asynchronous name or IP resolution operation.
	[TODO]
	public static IPHostEntry EndResolve(IAsyncResult asyncResult)
			{
				// TODO
				return ((DnsAsyncResult)(asyncResult)).EndInvoke();
			}

	// Get a host by address synchronously.
	public static IPHostEntry GetHostByAddress(String address)
			{
				//allow Parse to throw FormatException or ArgumentNullException
				IPAddress ip=IPAddress.Parse(address);
				return GetHostByAddress(ip);
			}

	public static IPHostEntry GetHostByAddress(IPAddress address)
			{
				if(address==null)
				{
					throw new ArgumentNullException("address");
				}
				String h_name;
				String [] h_aliases;
				long [] h_addr_list;
				
				if(!DnsMethods.InternalGetHostByAddr(address.Address, out h_name, 
						out h_aliases, out h_addr_list))
				{
					throw new SocketException(); // Hm...	
				}
				return ToIPHostEntry(h_name,h_aliases,h_addr_list);
			}

	// Get a host by name synchronously.
	public static IPHostEntry GetHostByName(String hostName)
			{
				if(hostName==null)
				{
					throw new ArgumentNullException("hostname");
				}
				String h_name;
				String [] h_aliases;
				long [] h_addr_list;
				
				if(!DnsMethods.InternalGetHostByName(hostName, out h_name, 
						out h_aliases, out h_addr_list))
				{
					throw new SocketException(); // Hm...	
				}
				return ToIPHostEntry(h_name,h_aliases,h_addr_list);
			}

	// Get the host name of the local machine.
	public static String GetHostName()
			{
				return DnsMethods.InternalGetHostName();			
			}

	// Resolve a name or IP address synchronously.
	public static IPHostEntry Resolve(String hostName)
			{
				return GetHostByName(hostName);
			}
	
	private static IPHostEntry ToIPHostEntry(String h_name,
				String []h_aliases,long[] h_addr_list)
	{
		IPHostEntry entry=new IPHostEntry();
		entry.HostName=h_name;
		entry.Aliases=h_aliases;
		entry.AddressList=new IPAddress[h_addr_list.Length];
		for(int i=0;i<h_addr_list.Length;i++)
		{
			entry.AddressList[i]=new IPAddress(h_addr_list[i]);
		}
		return entry;
	}

}; // class Dns

}; // namespace System.Net
