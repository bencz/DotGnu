/*
 * PathTooLongException.cs - Implementation of the
 *		"System.IO.PathTooLongException" class.
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

public class PathTooLongException : IOException
{
	// Constructors.
	public PathTooLongException()
			: base(Errno.ENAMETOOLONG, _("Exception_PathTooLong")) {}
	public PathTooLongException(String msg)
			: base(Errno.ENAMETOOLONG, msg) {}
	public PathTooLongException(String msg, Exception inner)
			: base(Errno.ENAMETOOLONG, msg, inner) {}
#if CONFIG_SERIALIZATION
	protected PathTooLongException(SerializationInfo info,
								   StreamingContext context)
		: base(info, context) {}
#endif

	// Get the default message to use for this exception type.
	internal override String MessageDefault
			{
				get
				{
					return _("Exception_PathTooLong");
				}
			}

	// Get the default HResult value for this type of exception.
	internal override uint HResultDefault
			{
				get
				{
					return 0x800700ce;
				}
			}

}; // class PathTooLongException

}; // namespace System.IO
