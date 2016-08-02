/*
 * socket-glue.cs - Glue between libc/socket and the C# system library.
 *
 * This file is part of the Portable.NET C library.
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace OpenSystem.C
{

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

[GlobalScope]
public class LibCSocket
{
	// The stream object that wraps up a socket.
	private class SocketStream : Stream, IFDOperations
	{
		// Internal state.
		private Socket socket;
		private bool connected;
		private bool listening;

		// Constructor.
		public SocketStream(Socket socket)
				{
					this.socket = socket;
					this.connected = false;
					this.listening = false;
				}

		// Get the socket that underlies this stream.
		public Socket Socket
				{
					get
					{
						return socket;
					}
				}

		// Get or set the "connected" state on this socket.
		public bool Connected
				{
					get
					{
						return connected;
					}
					set
					{
						connected = value;
					}
				}

		// Get or set the "listening" state on this socket.
		public bool Listening
				{
					get
					{
						return listening;
					}
					set
					{
						listening = value;
					}
				}

		// Implement the LibCFDOperations interface.
		public bool NonBlocking
				{
					get
					{
						return socket.Blocking;
					}
					set
					{
						socket.Blocking = value;
					}
				}
		public int NativeFd
				{
					get
					{
						return (int)(socket.Handle);
					}
				}
		public int SelectFd
				{
					get
					{
						return (int)(socket.Handle);
					}
				}

		// Implement pass throughs for "Stream" methods.
		public override void Close()
				{
					socket.Close();
				}
		public override void Flush()
				{
					// Nothing to do here.
				}
		public override int Read(byte[] buffer, int offset, int count)
				{
					return socket.Receive(buffer, offset, count,
										  SocketFlags.None);
				}
		public override int ReadByte()
				{
					byte[] buf = new byte [1];
					int len = socket.Receive(buf, 0, 1, SocketFlags.None);
					if(len > 0)
					{
						return (int)(buf[0]);
					}
					else
					{
						return -1;
					}
				}
		public override long Seek(long offset, SeekOrigin origin)
				{
					throw new NotSupportedException();
				}
		public override void SetLength(long value)
				{
					throw new NotSupportedException();
				}
		public override void Write(byte[] buffer, int offset, int count)
				{
					socket.Send(buffer, offset, count, SocketFlags.None);
				}
		public override void WriteByte(byte value)
				{
					byte[] buf = new byte [1];
					buf[0] = value;
					socket.Send(buf, 0, 1, SocketFlags.None);
				}
		public override bool CanRead
				{
					get
					{
						return true;
					}
				}
		public override bool CanSeek
				{
					get
					{
						return false;
					}
				}
		public override bool CanWrite
				{
					get
					{
						return true;
					}
				}
		public override long Length
				{
					get
					{
						throw new NotSupportedException();
					}
				}
		public override long Position
				{
					get
					{
						throw new NotSupportedException();
					}
					set
					{
						throw new NotSupportedException();
					}
				}
	
	} // class SocketStream

	// Create a new socket.
	public static int __syscall_socket(int domain, int type)
			{
				AddressFamily family;
				SocketType socketType;
				ProtocolType protocol;

				// Convert the parametrs into their C# equivalents.
				if(domain == 2 /*AF_INET*/)
				{
					family = AddressFamily.InterNetwork;
				}
				else if(domain == 10 /*AF_INET6*/)
				{
					family = AddressFamily.InterNetworkV6;
				}
				else
				{
					return -22;		/* EINVAL */
				}
				if(type == 1 /*SOCK_STREAM*/)
				{
					socketType = SocketType.Stream;
					protocol = ProtocolType.Tcp;
				}
				else if(type == 2 /*SOCK_DGRAM*/)
				{
					socketType = SocketType.Dgram;
					protocol = ProtocolType.Udp;
				}
				else
				{
					return -22;		/* EINVAL */
				}

				// Create the socket.
				Socket socket;
				try
				{
					socket = new Socket(family, socketType, protocol);
				}
				catch(SocketException)
				{
					return -22;		/* EINVAL */
				}

				// Wrap the socket within a stream.
				Stream stream = new SocketStream(socket);

				// Associate the stream with a file descriptor.
				int fd = FileTable.AllocFD();
				if(fd == -1)
				{
					stream.Close();
					return fd;
				}
				FileTable.SetFileDescriptor(fd, stream);
				return fd;
			}

	// Wrap a socket that was just accepted with a file descriptor.
	public static int __syscall_wrap_accept(Socket socket)
			{
				// Wrap the socket within a stream.
				SocketStream stream = new SocketStream(socket);
				stream.Connected = true;

				// Associate the stream with a file descriptor.
				int fd = FileTable.AllocFD();
				if(fd == -1)
				{
					stream.Close();
					return fd;
				}
				FileTable.SetFileDescriptor(fd, stream);
				return fd;
			}

	// Get the socket object for a file descriptor.  Returns null
	// if an error occurred (which is written to "errno").
	public unsafe static Socket __syscall_get_socket(int fd, int *errno)
			{
				Stream stream = FileTable.GetStream(fd);
				if(stream == null)
				{
					*errno = 9;		/* EBADF */
					return null;
				}
				SocketStream sstream = (stream as SocketStream);
				if(sstream != null)
				{
					return sstream.Socket;
				}
				else
				{
					*errno = 88;	/* ENOTSOCK */
					return null;
				}
			}

	// Create an end point object for an IPv4 socket address.
	public static EndPoint __create_ipv4_endpoint(uint address, int port)
			{
				return new IPEndPoint((long)address, port);
			}

	// Create an end point object for an IPv6 socket address.
	public static EndPoint __create_ipv6_endpoint
				(IntPtr address, uint scope, int port)
			{
				byte[] addr = new byte [16];
				Marshal.Copy(address, addr, 0, 16);
				return new IPEndPoint
					(new IPAddress(addr, (long)scope), port);
			}

	// Decode an end point for an IPv4 socket address into its components.
	public static int __decode_ipv4_endpoint
				(EndPoint ep, ref uint address, ref int port)
			{
				if(!(ep is IPEndPoint) ||
				   ep.AddressFamily != AddressFamily.InterNetwork)
				{
					address = 0;
					port = 0;
					return 0;
				}
				IPEndPoint iep = (IPEndPoint)ep;
				address = (uint)(iep.Address.Address);
				port = iep.Port;
				return 1;
			}

	// Decode an end point for an IPv6 socket address into its components.
	public static int __decode_ipv6_endpoint
				(EndPoint ep, IntPtr address, ref uint scope, ref int port)
			{
				if(!(ep is IPEndPoint) ||
				   ep.AddressFamily != AddressFamily.InterNetworkV6)
				{
					scope = 0;
					port = 0;
					return 0;
				}
				IPEndPoint iep = (IPEndPoint)ep;
				byte[] addr = iep.Address.GetAddressBytes();
				Marshal.Copy(addr, 0, address, 16);
				scope = (uint)(iep.Address.ScopeId);
				port = iep.Port;
				return 1;
			}

	// Get the address family for a socket file descriptor.
	// Returns a negative errno code if not a socket.
	public static int __syscall_socket_family(int fd)
			{
				Stream stream = FileTable.GetStream(fd);
				if(stream == null)
				{
					return -9;		/* EBADF */
				}
				SocketStream sstream = (stream as SocketStream);
				if(sstream == null)
				{
					return -88;		/* ENOTSOCK */
				}
				AddressFamily family = sstream.Socket.AddressFamily;
				if(family == AddressFamily.InterNetwork)
				{
					return 2;		/* AF_INET */
				}
				else if(family == AddressFamily.InterNetworkV6)
				{
					return 10;		/* AF_INET6 */
				}
				else
				{
					return -22;		/* EINVAL */
				}
			}

	// Determine if a socket has been marked for listening.
	public static int __syscall_is_listening(int fd)
			{
				Stream stream = FileTable.GetStream(fd);
				if(stream is SocketStream)
				{
					return (((SocketStream)stream).Listening ? 1 : 0);
				}
				else
				{
					return 0;
				}
			}

	// Set the listening state on a socket.
	public static void __syscall_set_listening(int fd, int value)
			{
				Stream stream = FileTable.GetStream(fd);
				if(stream is SocketStream)
				{
					((SocketStream)stream).Listening = (value != 0);
				}
			}

	// Determine if a socket has been marked as connected.
	public static int __syscall_is_connected(int fd)
			{
				Stream stream = FileTable.GetStream(fd);
				if(stream is SocketStream)
				{
					return (((SocketStream)stream).Connected ? 1 : 0);
				}
				else
				{
					return 0;
				}
			}

	// Set the connected state on a socket.
	public static void __syscall_set_connected(int fd, int value)
			{
				Stream stream = FileTable.GetStream(fd);
				if(stream is SocketStream)
				{
					((SocketStream)stream).Connected = (value != 0);
				}
			}

	// Structure that mirrors "struct hostent".
	[StructLayout(LayoutKind.Sequential)]
	private unsafe struct HostEnt
	{
		public IntPtr h_name;
		public void **h_aliases;
		public int h_addrtype;
		public int h_length;
		public void **h_addr_list;

	} // struct HostEnt

	// Convert an IPHostEntry object into a hostent structure.
	public unsafe static void __syscall_convert_hostent
				(IPHostEntry input, IntPtr output)
			{
				HostEnt *ent = (HostEnt *)(void *)output;

				// Convert the primary host name.
				ent->h_name = Marshal.StringToHGlobalAnsi(input.HostName);

				// Convert the alias list.
				String[] aliases = input.Aliases;
				int posn;
				if(aliases != null)
				{
					ent->h_aliases = (void **)(void *)Marshal.AllocHGlobal
						(sizeof(void *) * (aliases.Length + 1));
					for(posn = 0; posn < aliases.Length; ++posn)
					{
						ent->h_aliases[posn] =
							(void *)Marshal.StringToHGlobalAnsi(aliases[posn]);
					}
					ent->h_aliases[posn] = null;
				}
				else
				{
					ent->h_aliases = (void **)(void *)
						Marshal.AllocHGlobal(sizeof(void *));
					ent->h_aliases[0] = null;
				}

				// Set the address type and length.
				IPAddress[] addrlist = input.AddressList;
				if(addrlist[0].AddressFamily == AddressFamily.InterNetwork)
				{
					ent->h_addrtype = 2;	/* AF_INET */
					ent->h_length = 4;
				}
				else if(addrlist[0].AddressFamily ==
							AddressFamily.InterNetworkV6)
				{
					ent->h_addrtype = 10;	/* AF_INET6 */
					ent->h_length = 16;
				}
				else
				{
					ent->h_addrtype = 0;
					ent->h_length = 0;
				}

				// Convert the IP address length.
				ent->h_addr_list = (void **)(void *)Marshal.AllocHGlobal
					(sizeof(void *) * (addrlist.Length + 1));
				for(posn = 0; posn < addrlist.Length; ++posn)
				{
					byte[] addr = addrlist[posn].GetAddressBytes();
					ent->h_addr_list[posn] = (void *)
						Marshal.AllocHGlobal(addr.Length);
					Marshal.Copy(addr, 0, (IntPtr)(ent->h_addr_list[posn]),
								 addr.Length);
				}
				ent->h_addr_list[0] = null;
			}

} // class LibCSocket

} // namespace OpenSystem.C
