/*
 * IOException.cs - Implementation of the "System.IO.IOException" class.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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
using Platform;
using System.Runtime.Serialization;

public class IOException : SystemException
{
	// Internal state.
	private Errno errno;

	// Constructors.
	public IOException()
		: base(_("Exception_IO"))
		{
			errno = Errno.EIO;
		}
	public IOException(String msg)
		: base(msg)
		{
			errno = Errno.EIO;
		}
	public IOException(String msg, Exception inner)
		: base(msg, inner)
		{
			errno = Errno.EIO;
		}
#if !ECMA_COMPAT
	public IOException(String msg, int hresult)
		: base(msg)
		{
			errno = Errno.EIO;
			HResult = hresult;
		}
#endif
#if CONFIG_SERIALIZATION
	protected IOException(SerializationInfo info, StreamingContext context)
		: base(info, context) {}
#endif

	// Internal constructors that are used to set correct error codes.
	internal IOException(Errno errno)
		: base(null)
		{
			this.errno = errno;
		}
	internal IOException(Errno errno, String msg)
		: base(msg)
		{
			this.errno = errno;
		}
	internal IOException(Errno errno, String msg, Exception inner)
		: base(msg, inner)
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
				String str = FileMethods.GetErrnoMessage(errno);
				if(str != null)
				{
					return str;
				}

				// Use the default I/O exception string.
				return _("Exception_IO");
			}

	// Get the default message to use for I/O exceptions.
	internal override String MessageDefault
			{
				get
				{
					if(errno == Errno.EIO)
					{
						return _("Exception_IO");
					}
					else
					{
						return GetErrnoMessage(errno);
					}
				}
			}

	// Get the default HResult value for this type of exception.
	internal override uint HResultDefault
			{
				get
				{
					return 0x80131620;
				}
			}

}; // class IOException

}; // namespace System.IO
