/*
 * Overlapped.cs - Implementation of the
 *		"System.Threading.Overlapped" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

#if !ECMA_COMPAT

// For compatibility only - don't use this.

public class Overlapped
{
	// Internal state.
	private int offsetLo;
	private int offsetHi;
	private int hEvent;
	private IAsyncResult ar;

	// Constructors.
	public Overlapped() {}
	public Overlapped(int offsetLo, int offsetHi, int hEvent, IAsyncResult ar)
			{
				this.offsetLo = offsetLo;
				this.offsetHi = offsetHi;
				this.hEvent = hEvent;
				this.ar = ar;
			}

	// Get or set this object's properties.
	public int OffsetLow
			{
				get
				{
					return offsetLo;
				}
				set
				{
					offsetLo = value;
				}
			}
	public int OffsetHigh
			{
				get
				{
					return offsetHi;
				}
				set
				{
					offsetHi = value;
				}
			}
	public int EventHandle
			{
				get
				{
					return hEvent;
				}
				set
				{
					hEvent = value;
				}
			}
	public IAsyncResult AsyncResult
			{
				get
				{
					return ar;
				}
				set
				{
					ar = value;
				}
			}

	// Pack a native overlapped structure.
	[CLSCompliant(false)]
	public unsafe NativeOverlapped *Pack(IOCompletionCallback cb)
			{
				throw new NotImplementedException();
			}
	[CLSCompliant(false)]
	public unsafe NativeOverlapped *UnsafePack(IOCompletionCallback cb)
			{
				throw new NotImplementedException();
			}

	// Unpack a native overlapped structure.
	[CLSCompliant(false)]
	public static unsafe Overlapped Unpack
				(NativeOverlapped *nativeOverlappedPtr)
			{
				if(((IntPtr)nativeOverlappedPtr) == IntPtr.Zero)
				{
					throw new ArgumentNullException("nativeOverlappedPtr");
				}
				return new Overlapped(nativeOverlappedPtr->OffsetLow,
									  nativeOverlappedPtr->OffsetHigh,
									  nativeOverlappedPtr->EventHandle,
									  null);
			}

	// Free a native overlapped structure.
	[CLSCompliant(false)]
	public static unsafe void Free(NativeOverlapped *nativeOverlappedPtr)
			{
				// Since there is no way to allocate a native overlapped
				// structure in this implementation, the only thing we
				// need to do here is check for null.
				if(((IntPtr)nativeOverlappedPtr) == IntPtr.Zero)
				{
					throw new ArgumentNullException("nativeOverlappedPtr");
				}
			}

}; // class Overlapped

#endif // !ECMA_COMPAT

}; // namespace System.Threading
