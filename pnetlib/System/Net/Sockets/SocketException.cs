/*
 * SocketException.cs - Implementation of the "System.Net.Sockets.SocketException" class.
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
*
 * This program is free software, you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY, without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program, if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace System.Net.Sockets
{

using System;
using Platform;
using System.ComponentModel;
using System.Runtime.Serialization;

#if !ECMA_COMPAT
[Serializable]
#endif
public class SocketException :
#if !ECMA_COMPAT
	Win32Exception
#else
	SystemException
#endif
{
	// Internal state.
	private Errno errno;

	// Constructors.
	public SocketException()
		: base(S._("IO_Socket"))
		{
			errno = Errno.EREMOTEIO;
		}
#if CONFIG_SERIALIZATION
	protected SocketException(SerializationInfo info, StreamingContext context)
		: base(info, context)
		{
			errno = Errno.EREMOTEIO;
		}
#endif
	internal SocketException(String msg)
		: base(msg)
		{
			errno = Errno.EREMOTEIO;
		}
	internal SocketException(String msg, Exception inner)
		: base(msg, inner)
		{
			errno = Errno.EREMOTEIO;
		}
#if !ECMA_COMPAT
	public SocketException(int errorCode)
		: this((Errno)errorCode)
		{
		}
#endif

	// Internal constructors that are used to set correct error codes.
	internal SocketException(Errno errno)
#if !ECMA_COMPAT
		: base((int)errno, DefaultMessage(null, errno))
#else
		: base(DefaultMessage(null, errno))
#endif
		{
			this.errno = errno;
		}
	internal SocketException(Errno errno, String msg)
		: base(DefaultMessage(msg, errno))
		{
			this.errno = errno;
		}
	internal SocketException(Errno errno, String msg, Exception inner)
		: base(DefaultMessage(msg, errno), inner)
		{
			this.errno = errno;
		}

	// Get the platform error code associated with this exception.
	internal Errno Errno
			{
				get
				{
					return errno;
				}
			}

	// Get the message that corresponds to a platform error number.
	internal static String GetErrnoMessage(Errno errno)
			{
				// Try getting a message from the underlying platform.
				String str = SocketMethods.GetErrnoMessage(errno);
				if(str != null)
				{
					return str;
				}

				// Use the default Socket exception string.
				return S._("IO_Socket");
			}

	// Get the default message to use for this exception type.
	private static String DefaultMessage(String msg, Errno errno)
			{
				if(msg != null)
				{
					return msg;
				}
				else if(errno == Errno.EREMOTEIO)
				{
					return S._("IO_Socket");
				}
				else
				{
					return GetErrnoMessage(errno);
				}
			}

#if !ECMA_COMPAT

	// Get the error code.
	public override int ErrorCode
			{
				get
				{
					return NativeErrorCode;
				}
			}

#endif

}; // class SocketException

}; // namespace System.Net.Sockets

