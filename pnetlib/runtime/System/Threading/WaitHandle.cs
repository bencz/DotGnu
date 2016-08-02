/*
 * WaitHandle.cs - Implementation of the "System.Threading.WaitHandle" class.
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

using System.Runtime.CompilerServices;

public abstract class WaitHandle : MarshalByRefObject, IDisposable
{
	// Private data used by the runtime engine to
	// represent the underlying wait object handle.
	// This must be the first field in the object.
	private IntPtr privateData;

	/// <summary>
	/// Constant that specifies a timeout occured within the 
	/// WaitAny or WaitAll methods.
	/// </summary>
	public const int WaitTimeout = 258;

	// Constructors.
	public WaitHandle()
			{
				privateData = IntPtr.Zero;
			}

	// Destructor.
	~WaitHandle()
			{
				if(privateData != IntPtr.Zero)
				{
					Dispose(false);
				}
			}

	// Close the handle and release all resources.
	public virtual void Close()
			{
				Dispose(true);
			}

	// Internal version of "Close".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void InternalClose(IntPtr privateData);

	// Dispose this wait handle.
	protected virtual void Dispose(bool explicitDisposing)
			{
				lock(this)
				{
					if(privateData != IntPtr.Zero)
					{
						InternalClose(privateData);
						privateData = IntPtr.Zero;
					}
				}
			}

	// Implement the IDisposable interface.
	void IDisposable.Dispose()
			{
				Dispose(true);
			}

	// Validate the contents of a "waitHandles" array and
	// return a new array of low-level handles.
	private static IntPtr[] ValidateHandles(WaitHandle[] waitHandles)
			{
				int posn, posn2;
				WaitHandle handle;
				IntPtr[] lowLevel;
				if(waitHandles == null)
				{
					throw new ArgumentNullException("waitHandles");
				}
				if(waitHandles.Length > 64)
				{
					throw new NotSupportedException
						(_("NotSupp_MaxWaitHandles"));
				}
				lowLevel = new IntPtr [waitHandles.Length];
				for(posn = waitHandles.Length - 1; posn >= 0; --posn)
				{
					if((handle = waitHandles[posn]) == null ||
					   handle.privateData == IntPtr.Zero)
					{
						throw new ArgumentNullException
							("waitHandles[" + posn + "]");
					}
					for(posn2 = posn - 1; posn2 >= 0; --posn2)
					{
						if(handle == waitHandles[posn2])
						{
							throw new DuplicateWaitObjectException();
						}
					}
					lowLevel[posn] = handle.privateData;
				}
				return lowLevel;
			}

	// Wait for all elements in an array to receive a signal.
	public static bool WaitAll(WaitHandle[] waitHandles)
			{
				IntPtr[] lowLevel = ValidateHandles(waitHandles);
				return InternalWaitAll(lowLevel, -1, true);
			}
	public static bool WaitAll(WaitHandle[] waitHandles,
							   int millisecondsTimeout,
							   bool exitContext)
			{
				IntPtr[] lowLevel = ValidateHandles(waitHandles);
				if(millisecondsTimeout < -1)
				{
					throw new ArgumentOutOfRangeException
						("millisecondsTimeout",
						 _("ArgRange_NonNegOrNegOne"));
				}
				return InternalWaitAll(lowLevel, millisecondsTimeout,
								       exitContext);
			}
	public static bool WaitAll(WaitHandle[] waitHandles,
							   TimeSpan timeout, bool exitContext)
			{
				IntPtr[] lowLevel = ValidateHandles(waitHandles);
				return InternalWaitAll(lowLevel,
									   Monitor.TimeSpanToMS(timeout),
								       exitContext);
			}

	// Internal version of "WaitAll".  A timeout of -1 indicates infinite,
	// and zero indicates "test and return immediately".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static bool InternalWaitAll
				(IntPtr[] waitHandles, int timeout, bool exitContext);

	// Wait for any element in an array to receive a signal.
	public static int WaitAny(WaitHandle[] waitHandles)
			{
				IntPtr[] lowLevel = ValidateHandles(waitHandles);
				return InternalWaitAny(lowLevel, -1, true);
			}
	public static int WaitAny(WaitHandle[] waitHandles,
							  int millisecondsTimeout,
							  bool exitContext)
			{
				IntPtr[] lowLevel = ValidateHandles(waitHandles);
				if(millisecondsTimeout < -1)
				{
					throw new ArgumentOutOfRangeException
						("millisecondsTimeout",
						 _("ArgRange_NonNegOrNegOne"));
				}
				return InternalWaitAny(lowLevel, millisecondsTimeout,
								       exitContext);
			}
	public static int WaitAny(WaitHandle[] waitHandles,
							  TimeSpan timeout, bool exitContext)
			{
				IntPtr[] lowLevel = ValidateHandles(waitHandles);
				return InternalWaitAny(lowLevel,
									   Monitor.TimeSpanToMS(timeout),
								       exitContext);
			}

	// Internal version of "WaitAny".  A timeout of -1 indicates
	// infinite, and zero indicates "test and return immediately".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static int InternalWaitAny
				(IntPtr[] waitHandles, int timeout, bool exitContext);

	// Wait until this handle receives a signal.
	public virtual bool WaitOne()
			{
				if( privateData == IntPtr.Zero ) 
				{
					throw new ObjectDisposedException(_("WaitHandle"));
				}
				
				return InternalWaitOne(privateData, -1);
			}
	public virtual bool WaitOne(int millisecondsTimeout,bool exitContext)
			{
				if( privateData == IntPtr.Zero ) 
				{
					throw new ObjectDisposedException(_("WaitHandle"));
				}
				
				if(millisecondsTimeout < -1)
				{
					throw new ArgumentOutOfRangeException
						("millisecondsTimeout",
						 _("ArgRange_NonNegOrNegOne"));
				}
				
				return InternalWaitOne(privateData, millisecondsTimeout);
			}
	public virtual bool WaitOne(TimeSpan timeout, bool exitContext)
			{
				if( privateData == IntPtr.Zero ) 
				{
					throw new ObjectDisposedException(_("WaitHandle"));
				}

				return InternalWaitOne(privateData,
									   Monitor.TimeSpanToMS(timeout));
			}

	// Internal version of "WaitOne".  A timeout of -1 indicates
	// infinite, and zero indicates "test and return immediately".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static bool InternalWaitOne(IntPtr privateData,
											   int timeout);

	// Value of an invalid handle.
	protected static readonly IntPtr InvalidHandle = IntPtr.Zero;

	// Get the private handle associated with this object.
	// Note: Microsoft's implementation allows the handle
	// to be set here, but that's too dangerous to allow.
	public virtual IntPtr Handle
			{
				get
				{
					return privateData;
				}
				set
				{
					throw new NotSupportedException
						(_("NotSupp_SetWaitHandle"));
				}
			}

	// Set the handle from a subclass.
	internal void SetHandle(IntPtr handle)
			{
				privateData = handle;
			}

}; // class WaitHandle

}; // namespace System.Threading
