/*
 * GC.cs - Implementation of the "System.GC" class.
 *
 * Copyright (C) 2001, 2002, 2008  Southern Storm Software, Pty Ltd.
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

using System.Runtime.CompilerServices;

public sealed class GC
{

	// This class cannot be instantiated.
	private GC() {}

	// Perform a full garbage collection.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void CollectInternal(int collectionMode);

#if !ECMA_COMPAT

#if CONFIG_FRAMEWORK_2_0

	// Get the total number of collections done.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static int CollectionCountInternal();

#endif // CONFIG_FRAMEWORK_2_0

	// Get the total amount of memory in use by the heap.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static long GetTotalMemoryInternal(bool forceFullCollection);

#endif // !ECMA_COMPAT

	// Re-register an object for finalization.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void ReRegisterForFinalizeInternal(Object obj);

	// Suppress finalization for an object.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void SuppressFinalizeInternal(Object obj);

	// Wait for all pending finalizers to be run.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void WaitForPendingFinalizersInternal();

	// Perform a full garbage collection.
	public static void Collect()
			{
				CollectInternal(0);
			}

#if !ECMA_COMPAT

	// Perform garbage collection on a range of generations.
	public static void Collect(int generation)
			{
				if(generation != 0)
				{
					throw new ArgumentOutOfRangeException
						("generation", _("ArgRange_GCGeneration"));
				}
				CollectInternal(0);
			}

#if CONFIG_FRAMEWORK_2_0

	// Perform garbage collection on a range of generations with the given
	// collection mode
	public static void Collect(int generation, GCCollectionMode mode)
			{
				if(generation != 0)
				{
					throw new ArgumentOutOfRangeException
						("generation", _("ArgRange_GCGeneration"));
				}
				CollectInternal(0);
			}

	public static int CollectionCount(int generation)
			{
				if(generation != 0)
				{
					throw new ArgumentOutOfRangeException
						("generation", _("ArgRange_GCGeneration"));
				}
				return CollectionCountInternal();
			}

#endif // CONFIG_FRAMEWORK_2_0

	// Get the generation of a specified object.
	public static int GetGeneration(Object obj)
			{
				// We don't currently support generational collection.
				return 0;
			}

	// Get the generation of a weak reference.
	public static int GetGeneration(WeakReference wo)
			{
				Object target = wo.Target;
				if(target == null)
				{
					throw new ArgumentException
						(_("Arg_WeakRefCollected"), "wo");
				}
				return GetGeneration(target);
			}

	// Get the total amount of memory in use by the heap.
	public static long GetTotalMemory(bool forceFullCollection)
			{
				return GetTotalMemoryInternal(forceFullCollection);
			}

#endif // !ECMA_COMPAT

	// Keep an object reference alive.
	// This function does nothing but accessing obj and so preventing it
	// from being collected.
	// Calls to this function MUST NOT be optimized away.
	public static void KeepAlive(Object obj)
			{
			}

#if !ECMA_COMPAT

	// Get the maximum generation currently in use.
	public static int MaxGeneration
			{
				get
				{
					// We don't currently support generational collection.
					return 0;
				}
			}

#endif // !ECMA_COMPAT

	public static void ReRegisterForFinalize(Object obj)
			{
				if(obj == null)
				{
					throw new ArgumentNullException("obj");
				}
				ReRegisterForFinalizeInternal(obj);
			}

	// Suppress finalization for an object.
	public static void SuppressFinalize(Object obj)
			{
				if(obj == null)
				{
					throw new ArgumentNullException("obj");
				}
				SuppressFinalizeInternal(obj);
			}

	// Wait for all pending finalizers to be run.
	public static void WaitForPendingFinalizers()
			{
				WaitForPendingFinalizersInternal();
			}

}; // class GC

}; // namespace System
