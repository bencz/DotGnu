/*
 * AbandonedMutexException.cs - Implementation of the
 *			"System.Threading.AbandonedMutexException" class.
 *
 * Copyright (C) 2007  Southern Storm Software, Pty Ltd.
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

#if CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

using System.Runtime.InteropServices;

#if CONFIG_SERIALIZATION
using System.Runtime.Serialization;

[Serializable]
#endif // CONFIG_SERIALIZATION
[ComVisible(false)]
public class AbandonedMutexException : SystemException
{
	private Mutex mutex;
	private int mutexIndex;

	// Constructors.
	public AbandonedMutexException() : base(_("Exception_MutexAbandoned"))
	{
		mutex = null;
		mutexIndex = -1;
	}

	public AbandonedMutexException(String msg) : base(msg)
	{
		mutex = null;
		mutexIndex = -1;
	}

	public AbandonedMutexException(int index, WaitHandle handle) :
			base(_("Exception_MutexAbandoned"))
	{
		mutexIndex = index;
		mutex = (Mutex)handle;
	}

	public AbandonedMutexException(String msg, Exception inner) : base(msg, inner)
	{
		mutex = null;
		mutexIndex = -1;
	}

	public AbandonedMutexException(String msg, int index, WaitHandle handle) :
			base(msg)
	{
		mutexIndex = index;
		mutex = (Mutex)handle;
	}

	public AbandonedMutexException(String msg, Exception inner,
								   int index, WaitHandle handle) :
			base(msg, inner)
	{
		mutexIndex = index;
		mutex = (Mutex)handle;
	}

#if CONFIG_SERIALIZATION
	protected AbandonedMutexException(SerializationInfo info,
									  StreamingContext context) :
			base(info, context)
	{
		mutexIndex = info.GetInt32("MutexIndex");
		mutex = (Mutex)(info.GetValue("Mutex", typeof(Mutex)));
	}
#endif

	// Get the abandoned mutex that caused the exception.
	public Mutex Mutex
	{
		get
		{
			return mutex;
		}
	}

	// Get the index of the abandoned mutex that caused the exception.
	public int MutexIndex
	{
		get
		{
			return mutexIndex;
		}
	}

	// Get the default message to use for this exception type.
	internal override String MessageDefault
			{
				get
				{
					return _("Exception_MutexAbandoned");
				}
			}

}; // class AbandonedMutexException

#endif // CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

}; // namespace System.Threading
