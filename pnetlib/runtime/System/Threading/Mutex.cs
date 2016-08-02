/*
 * Mutex.cs - Implementation of the "System.Threading.Mutex" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

public sealed class Mutex : WaitHandle
{
	// Constructors.
	public Mutex()
			{
				bool temp;
				SetHandle(InternalCreateMutex(false, null, out temp));
			}
	public Mutex(bool initiallyOwned)
			{
				bool temp;
				SetHandle(InternalCreateMutex(initiallyOwned, null, out temp));
			}
	public Mutex(bool initiallyOwned, String name)
			{
				bool temp;
				SetHandle(InternalCreateMutex(initiallyOwned, name, out temp));
			}
	public Mutex(bool initiallyOwned, String name, out bool gotOwnership)
			{
				SetHandle(InternalCreateMutex(initiallyOwned, name,
											  out gotOwnership));
			}

	// Internal method to create a mutex.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static IntPtr InternalCreateMutex(bool initiallyOwned,
													 String name,
													 out bool gotOwnership);

	// Internal method to release a mutex.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void InternalReleaseMutex(IntPtr mutex);

	// Release this mutex.
	public void ReleaseMutex()
			{
				InternalReleaseMutex(Handle);
			}

}; // class Mutex

}; // namespace System.Threading
