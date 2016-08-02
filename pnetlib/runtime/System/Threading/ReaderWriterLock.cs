/*
 * ReaderWriterLock.cs - Implementation of the
 *		"System.Threading.ReaderWriterLock" class.
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

public sealed class ReaderWriterLock
{
	// Lock information for a thread.
	private sealed class LockInfo
	{
		public LockInfo next;
		public Thread thread;
		public int numReadLocks;
		public int numWriteLocks;

		public LockInfo(Thread thread, LockInfo next)
				{
					this.next = next;
					this.thread = thread;
					this.numReadLocks = 0;
					this.numWriteLocks = 0;
				}

	}; // class LockInfo

	// Internal state.
	private int numReadLocks;
	private int numWriteLocks;
	private int seqNum;
	private int lastWriteSeqNum;
	private LockInfo lockList;

	// Constructor.
	public ReaderWriterLock()
			{
				numReadLocks = 0;
				numWriteLocks = 0;
				seqNum = 0;
				lastWriteSeqNum = -1;
				lockList = null;
			}

	// Get the lock information for the current thread.
	private LockInfo GetLockInfo()
			{
				Thread thread = Thread.CurrentThread;
				LockInfo info = lockList;
				while(info != null)
				{
					if(info.thread == thread)
					{
						return info;
					}
					info = info.next;
				}
				return null;
			}

	// Get the lock information for the current thread, creating
	// a new information object if one doesn't exist yet.
	private LockInfo GetOrCreateLockInfo()
			{
				Thread thread = Thread.CurrentThread;
				LockInfo info = lockList;
				while(info != null)
				{
					if(info.thread == thread)
					{
						return info;
					}
					info = info.next;
				}
				info = new LockInfo(thread, lockList);
				lockList = info;
				return info;
			}

	// Acquire the read lock.
	public void AcquireReaderLock(int millisecondsTimeout)
			{
				if(millisecondsTimeout < -1)
				{
					throw new ArgumentOutOfRangeException
						("millisecondsTimeout",
						 _("ArgRange_NonNegOrNegOne"));
				}
				lock(this)
				{
					// Get the lock information for this thread.
					LockInfo info = GetOrCreateLockInfo();

					// Block if some other thread has the writer lock.
					if(millisecondsTimeout == -1)
					{
						while(info.numWriteLocks == 0 && numWriteLocks > 0)
						{
							if(!Monitor.Wait(this))
							{
								return;
							}
						}
					}
					else
					{
						DateTime expire = DateTime.UtcNow +
							new TimeSpan(millisecondsTimeout *
										 TimeSpan.TicksPerMillisecond);
						DateTime now;
						int ms;
						while(info.numWriteLocks == 0 && numWriteLocks > 0)
						{
							now = DateTime.UtcNow;
							if(now >= expire)
							{
								return;
							}
							ms = (int)((expire - now).Ticks /
										TimeSpan.TicksPerMillisecond);
							if(!Monitor.Wait(this, ms))
							{
								return;
							}
						}
					}

					// Update the thread and global read lock counts.
					++(info.numReadLocks);
					++numReadLocks;
				}
			}
	public void AcquireReaderLock(TimeSpan timeout)
			{
				AcquireReaderLock(Monitor.TimeSpanToMS(timeout));
			}

	// Acquire the write lock.
	public void AcquireWriterLock(int millisecondsTimeout)
			{
				if(millisecondsTimeout < -1)
				{
					throw new ArgumentOutOfRangeException
						("millisecondsTimeout",
						 _("ArgRange_NonNegOrNegOne"));
				}
				lock(this)
				{
					// Get the lock information for this thread.
					LockInfo info = GetOrCreateLockInfo();

					// Bail out early if we already have the writer lock.
					if(info.numWriteLocks > 0)
					{
						++(info.numWriteLocks);
						++numWriteLocks;
						lastWriteSeqNum = ++seqNum;
						return;
					}

					// Block while some other thread has the read or write lock.
					if(millisecondsTimeout == -1)
					{
						while(numReadLocks > 0 || numWriteLocks > 0)
						{
							if(!Monitor.Wait(this))
							{
								return;
							}
						}
					}
					else
					{
						DateTime expire = DateTime.UtcNow +
							new TimeSpan(millisecondsTimeout *
										 TimeSpan.TicksPerMillisecond);
						DateTime now;
						int ms;
						while(numReadLocks > 0 || numWriteLocks > 0)
						{
							now = DateTime.UtcNow;
							if(now >= expire)
							{
								return;
							}
							ms = (int)((expire - now).Ticks /
										TimeSpan.TicksPerMillisecond);
							if(!Monitor.Wait(this, ms))
							{
								return;
							}
						}
					}

					// Update the thread and global write lock counts.
					++(info.numWriteLocks);
					++numWriteLocks;
					lastWriteSeqNum = ++seqNum;
				}
			}
	public void AcquireWriterLock(TimeSpan timeout)
			{
				AcquireWriterLock(Monitor.TimeSpanToMS(timeout));
			}

	// Determine if there have been any writers since a particular seqnum.
	public bool AnyWritersSince(int seqNum)
			{
				lock(this)
				{
					if(seqNum >= 0 && seqNum < lastWriteSeqNum)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}

	// Downgrade the current thread from a writer lock.
	public void DowngradeFromWriterLock(ref LockCookie lockCookie)
			{
				lock(this)
				{
					// Get the lock information for this thread.
					LockInfo info = GetLockInfo();
					if(info == null)
					{
						return;
					}

					// Bail out if the cookie is not "Upgrade".
					if(lockCookie.type != LockCookie.CookieType.Upgrade ||
					   lockCookie.thread != Thread.CurrentThread)
					{
						return;
					}

					// Restore the thread to its previous lock state.
					RestoreLockState(info, lockCookie.readCount,
									 lockCookie.writeCount);
				}
			}

	// Release all locks for the current thread and save them.
	public LockCookie ReleaseLock()
			{
				lock(this)
				{
					// Get the lock information for this thread.
					LockInfo info = GetLockInfo();
					if(info == null)
					{
						return new LockCookie
							(LockCookie.CookieType.None,
							 Thread.CurrentThread, 0, 0);
					}

					// Bail out if the thread doesn't have any locks.
					if(info.numReadLocks == 0 && info.numWriteLocks == 0)
					{
						return new LockCookie
							(LockCookie.CookieType.None,
							 Thread.CurrentThread, 0, 0);
					}

					// Copy the lock infomation into the cookie.
					LockCookie cookie = new LockCookie
						(LockCookie.CookieType.Saved,
						 Thread.CurrentThread,
						 info.numReadLocks, info.numWriteLocks);

					// Release the active locks.
					numReadLocks -= info.numReadLocks;
					numWriteLocks -= info.numWriteLocks;
					info.numReadLocks = 0;
					info.numWriteLocks = 0;

					// Determine if we need to wake up a waiting thread.
					if(numReadLocks == 0 || numWriteLocks == 0)
					{
						Monitor.Pulse(this);
					}

					// Return the cookie to the caller.
					return cookie;
				}
			}

	// Release the read lock.
	public void ReleaseReaderLock()
			{
				lock(this)
				{
					// Get the lock information for this thread.
					LockInfo info = GetLockInfo();
					if(info == null)
					{
						return;
					}

					// Save the global write lock count.
					int saveRead = numReadLocks;
					int saveWrite = numWriteLocks;

					// Update the thread and global lock count values.
					if(info.numReadLocks > 0)
					{
						--(info.numReadLocks);
						--numReadLocks;
					}

					// Determine if we need to wake up a waiting thread.
					if(saveRead > numReadLocks && numReadLocks == 0)
					{
						Monitor.Pulse(this);
					}
					else if(saveWrite > numWriteLocks && numWriteLocks == 0)
					{
						Monitor.Pulse(this);
					}
				}
			}

	// Release the write lock.
	public void ReleaseWriterLock()
			{
				lock(this)
				{
					// Get the lock information for this thread.
					LockInfo info = GetLockInfo();
					if(info == null)
					{
						return;
					}

					// Bail out with an exception if we have read locks.
					if(info.numReadLocks > 0)
					{
						throw new ApplicationException(_("Invalid_RWLock"));
					}

					// Update the thread and global lock count values.
					if(info.numWriteLocks == 0)
					{
						return;
					}
					--(info.numWriteLocks);
					--numWriteLocks;

					// Determine if we need to wake up a waiting thread.
					if(numWriteLocks == 0)
					{
						Monitor.Pulse(this);
					}
				}
			}

	// Restore the lock state for the current thread.
	private void RestoreLockState(LockInfo info, int readCount, int writeCount)
			{
				// Save the current global lock state.
				int saveRead = numReadLocks;
				int saveWrite = numWriteLocks;

				// Remove the locks that are currently held by the thread.
				numReadLocks -= info.numReadLocks;
				numWriteLocks -= info.numWriteLocks;
				info.numReadLocks = 0;
				info.numWriteLocks = 0;

				// Wake up any waiting threads.
				if(saveRead > numReadLocks && numReadLocks == 0)
				{
					Monitor.Pulse(this);
				}
				else if(saveWrite > numWriteLocks && numWriteLocks == 0)
				{
					Monitor.Pulse(this);
				}

				// Re-acquire the locks based upon the type required.
				if(readCount > 0 && writeCount == 0)
				{
					// Re-acquire read locks only.
					while(numWriteLocks > 0)
					{
						Monitor.Wait(this);
					}
					info.numReadLocks += readCount;
					numReadLocks += readCount;
				}
				else if(readCount == 0 && writeCount > 0)
				{
					// Re-acquire write locks only.
					while(numReadLocks > 0 && numWriteLocks > 0)
					{
						Monitor.Wait(this);
					}
					info.numWriteLocks += writeCount;
					numWriteLocks += writeCount;
				}
				else if(readCount > 0 && writeCount > 0)
				{
					// Re-acquire both read and write locks.
					while(numWriteLocks > 0)
					{
						Monitor.Wait(this);
					}
					info.numReadLocks += readCount;
					numReadLocks += readCount;
					info.numWriteLocks += writeCount;
					numWriteLocks += writeCount;
				}
			}

	// Restore all locks for the curent thread to a previous "Release" value.
	public void RestoreLock(ref LockCookie lockCookie)
			{
				lock(this)
				{
					// Get the lock information for this thread.
					LockInfo info = GetLockInfo();
					if(info == null)
					{
						return;
					}

					// Bail out if the cookie is not "Saved" or if
					// we have prevailing locks at the moment.
					if(lockCookie.type != LockCookie.CookieType.Saved ||
					   lockCookie.thread != Thread.CurrentThread ||
					   info.numReadLocks > 0 ||
					   info.numWriteLocks > 0)
					{
						return;
					}

					// Restore the thread to its previous lock state.
					RestoreLockState(info, lockCookie.readCount,
									 lockCookie.writeCount);
				}
			}

	// Update the current thread to a writer lock.
	public LockCookie UpgradeToWriterLock(int millisecondsTimeout)
			{
				LockCookie cookie;
				if(millisecondsTimeout < -1)
				{
					throw new ArgumentOutOfRangeException
						("millisecondsTimeout",
						 _("ArgRange_NonNegOrNegOne"));
				}
				lock(this)
				{
					// Get the lock information for this thread.
					LockInfo info = GetOrCreateLockInfo();

					// Bail out early if we already have the writer lock.
					if(info.numWriteLocks > 0)
					{
						cookie = new LockCookie
							(LockCookie.CookieType.Upgrade,
							 Thread.CurrentThread,
							 info.numReadLocks, info.numWriteLocks);
						++(info.numWriteLocks);
						++numWriteLocks;
						lastWriteSeqNum = ++seqNum;
						return cookie;
					}

					// If we have the read lock, then upgrade it.
					if(info.numReadLocks > 0)
					{
						cookie = new LockCookie
							(LockCookie.CookieType.Upgrade,
							 Thread.CurrentThread,
							 info.numReadLocks, info.numWriteLocks);
						info.numWriteLocks += info.numReadLocks;
						numReadLocks -= info.numReadLocks;
						numWriteLocks -= info.numReadLocks;
						info.numReadLocks = 0;
						lastWriteSeqNum = ++seqNum;
						return cookie;
					}

					// Block while some other thread has the read or write lock.
					if(millisecondsTimeout == -1)
					{
						while(numReadLocks > 0 || numWriteLocks > 0)
						{
							if(!Monitor.Wait(this))
							{
								return new LockCookie
									(LockCookie.CookieType.None,
									 Thread.CurrentThread, 0, 0);
							}
						}
					}
					else
					{
						DateTime expire = DateTime.UtcNow +
							new TimeSpan(millisecondsTimeout *
										 TimeSpan.TicksPerMillisecond);
						DateTime now;
						int ms;
						while(numReadLocks > 0 || numWriteLocks > 0)
						{
							now = DateTime.UtcNow;
							if(now >= expire)
							{
								return new LockCookie
									(LockCookie.CookieType.None,
									 Thread.CurrentThread, 0, 0);
							}
							ms = (int)((expire - now).Ticks /
										TimeSpan.TicksPerMillisecond);
							if(!Monitor.Wait(this, ms))
							{
								return new LockCookie
									(LockCookie.CookieType.None,
									 Thread.CurrentThread, 0, 0);
							}
						}
					}

					// Update the thread and global write lock counts.
					cookie = new LockCookie
						(LockCookie.CookieType.Upgrade,
						 Thread.CurrentThread,
						 info.numReadLocks, info.numWriteLocks);
					++(info.numWriteLocks);
					++numWriteLocks;
					lastWriteSeqNum = ++seqNum;
					return cookie;
				}
			}
	public LockCookie UpgradeToWriterLock(TimeSpan timeout)
			{
				return UpgradeToWriterLock(Monitor.TimeSpanToMS(timeout));
			}

	// Determine if the read lock is held by the current thread.
	public bool IsReaderLockHeld
			{
				get
				{
					lock(this)
					{
						LockInfo info = GetLockInfo();
						if(info != null)
						{
							return (info.numReadLocks > 0);
						}
						else
						{
							return false;
						}
					}
				}
			}

	// Determine if the write lock is held by the current thread.
	public bool IsWriterLockHeld
			{
				get
				{
					lock(this)
					{
						LockInfo info = GetLockInfo();
						if(info != null)
						{
							return (info.numWriteLocks > 0);
						}
						else
						{
							return false;
						}
					}
				}
			}

	// Get the writer sequence number.
	public int WriterSeqNum
			{
				get
				{
					lock(this)
					{
						return seqNum;
					}
				}
			}

}; // class ReaderWriterLock

#endif // !ECMA_COMPAT

}; // namespace System.Threading
