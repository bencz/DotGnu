/*
 * ThreadAbortException.cs - Implementation of the
 *			"System.Threading.ThreadAbortException" class.
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

public sealed class ThreadAbortException : SystemException
{
	// Private state.
	private Object stateInfo;

	// Constructor that is called from the runtime engine.
	private ThreadAbortException(Object stateInfo)
			: base(_("Exception_ThreadAbort"))
			{
				this.stateInfo = stateInfo;
			}
#if CONFIG_SERIALIZATION
	internal ThreadAbortException(SerializationInfo info,
								  StreamingContext context)
			: base(info, context) {}
#endif

	// Get the exception state.
	public Object ExceptionState
			{
				get
				{
					return stateInfo;
				}
			}

	// Get the default HResult value for this type of exception.
	internal override uint HResultDefault
			{
				get
				{
					return 0x80131530;
				}
			}

}; // class ThreadAbortException

}; // namespace System.Threading
