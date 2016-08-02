/*
 * AutoResetEvent.cs - Implementation of the "System.Threading.AutoResetEvent" class.
 *
 * Copyright (C) 2002 Free Software Foundation
 *
 * Authors: Thong Nguyen
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
	using System.Runtime.CompilerServices;

	/// <summary>
	/// See ECMA specs.
	/// </summary>
#if !ECMA_COMPAT
	public
#else
	internal
#endif
	sealed class AutoResetEvent : WaitHandle, ISignal
	{
		/// <summary>
		/// See ECMA specs.
		/// </summary>
		public AutoResetEvent(bool initialState)
		{
			IntPtr handle;

			handle = WaitEvent.InternalCreateEvent(false, initialState);

			SetHandle(handle);
		}

		/// <summary>
		/// See ECMA specs.
		/// </summary>
		public bool Set()
		{
			if( Handle == IntPtr.Zero ) 
			{
				throw new ObjectDisposedException(_("AutoResetEvent"));
			}
			return WaitEvent.InternalSetEvent(Handle);
		}

		/// <summary>		
		/// See ECMA specs.
		/// </summary>
		public bool Reset()
		{
		if( Handle == IntPtr.Zero ) 
			{
				throw new ObjectDisposedException(_("AutoResetEvent"));
			}
			return WaitEvent.InternalResetEvent(Handle);
		}

		// Implement the ISignal interface.
		void ISignal.Signal()
		{
			Set();
		}
	}
}
