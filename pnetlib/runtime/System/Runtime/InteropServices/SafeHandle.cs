/*
 * SafeHandle.cs - Implementation of the
 *			"System.Runtime.InteropServices.SafeHandle" class.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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

namespace System.Runtime.InteropServices
{

#if !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

using System.Runtime.ConstrainedExecution;

public abstract class SafeHandle
	: CriticalFinalizerObject, IDisposable
{
	// Internal state.
	protected IntPtr handle;
	private bool ownsHandle;
	private bool closed;

	// Constructor.
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	protected SafeHandle(IntPtr invalidHandleValue, bool ownsHandle)
			{
				this.handle = invalidHandleValue;
				this.ownsHandle = ownsHandle;
				this.closed = false;
				if(!ownsHandle)
				{
					GC.SuppressFinalize(this);
				}
			}

	// Destructor.
	~SafeHandle()
			{
				Dispose(false);
			}

	// Dispose
	protected virtual void Dispose(bool disposing)
			{
				if(!IsClosed)
				{
					closed = true;
					if(!IsInvalid)
					{
						ReleaseHandle();
					}
				}
				if(disposing)
				{
					GC.SuppressFinalize(this);
				}
			}

	// Close this handle.
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	public void Close()
			{
				Dispose(true);
			}

	// Perform a reference add.
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	public void DangerousAddRef(ref bool success)
			{
				// Nothing to do in this implementation.
				success = true;
			}

	// Get the handle.
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	public IntPtr DangerousGetHandle()
			{
				return handle;
			}

	// Release the handle.
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	public void DangerousRelease()
			{
				// Nothing to do in this implementation.
			}

	// Implement the IDisposable interface.
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	public void Dispose()
			{
				Dispose(true);
			}

	// Release the handle.
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	protected abstract bool ReleaseHandle();

	// Set the handle.
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	protected void SetHandle(IntPtr handle)
			{
				this.handle = handle;
			}

	// Set the handle to invalid.
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	public void SetHandleAsInvalid()
			{
				this.closed = true;
			}

	// Determine if this handle is closed.
	public bool IsClosed
			{
				[ReliabilityContract(Consistency.WillNotCorruptState,
									 Cer.Success)]
				get
				{
					return closed;
				}
			}

	// Determine if this handle is invalid.
	public abstract bool IsInvalid
			{
				[ReliabilityContract(Consistency.WillNotCorruptState,
									 Cer.Success)]
				get;
			}

}; // class SafeHandle

#endif // !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

}; // namespace System.Runtime.InteropServices
