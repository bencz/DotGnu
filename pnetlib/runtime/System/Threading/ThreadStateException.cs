/*
 * ThreadStateException.cs - Implementation of the
 *			"System.Threading.ThreadStateException" class.
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

namespace System.Threading
{

using System.Runtime.Serialization;

public class ThreadStateException : SystemException
{

	// Constructors.
	public ThreadStateException()
		: base(_("Exception_ThreadState")) {}
	public ThreadStateException(String msg)
		: base(msg) {}
	public ThreadStateException(String msg, Exception inner)
		: base(msg, inner) {}
#if CONFIG_SERIALIZATION
	protected ThreadStateException(SerializationInfo info,
								   StreamingContext context)
		: base(info, context) {}
#endif

	// Get the default message to use for this exception type.
	internal override String MessageDefault
			{
				get
				{
					return _("Exception_ThreadState");
				}
			}

	// Get the default HResult value for this type of exception.
	internal override uint HResultDefault
			{
				get
				{
					return 0x80131520;
				}
			}

}; // class ThreadStateException

}; // namespace System.Threading
