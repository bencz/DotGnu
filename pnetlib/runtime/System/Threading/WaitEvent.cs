/*
 * WaitHandle.cs - Implementation of the "System.Threading.WaitHandle" class.
 *
 *	Copyright (C) 2002 Free Software Foundation
 *
 *	Authors: Thong Nguyen
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
	/// Provides access to internal wait event methods.
	/// </summary>
	internal sealed class WaitEvent
	{
		/// <summary>
		/// Internal call to create an event.
		/// </summary>
		/// <param name="manualReset">If false, the event will automatically reset itself.</param>
		/// <param name="initialState">Initial state of the event (true for signalled)</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr InternalCreateEvent(bool manualReset, bool initialState);

		/// <summary>
		/// Set an event to the signalled state.
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool InternalSetEvent(IntPtr handle);
		
		/// <summary>
		/// Set an event to the unsignalled state.
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool InternalResetEvent(IntPtr handle);
	}
}
