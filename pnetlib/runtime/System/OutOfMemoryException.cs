/*
 * OutOfMemoryException.cs - Implementation of the
 *		"System.OutOfMemoryException" class.
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

namespace System
{

using System.Runtime.Serialization;

public class OutOfMemoryException : SystemException
{

	// Constructors.
	public OutOfMemoryException()
		: base(_("Exception_NoMemory")) {}
	public OutOfMemoryException(String msg)
		: base(msg) {}
	public OutOfMemoryException(String msg, Exception inner)
		: base(msg, inner) {}
#if CONFIG_SERIALIZATION
	protected OutOfMemoryException(SerializationInfo info,
								   StreamingContext context)
		: base(info, context) {}
#endif

	// Internal constructor used by the runtime engine
	// to make the primary "OutOfMemoryException" object,
	// that has no stack trace associated with it.
	internal OutOfMemoryException(int dummy)
		: base(_("Exception_NoMemory"),
			   null, false) {}

	// Get the default message to use for this exception type.
	internal override String MessageDefault
			{
				get
				{
					return _("Exception_NoMemory");
				}
			}

	// Get the default HResult value for this type of exception.
	internal override uint HResultDefault
			{
				get
				{
					return 0x8007000e;
				}
			}

}; // class OutOfMemoryException

}; // namespace System
