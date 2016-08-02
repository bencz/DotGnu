/*
 * Timer.cs - Implementation of the "System.Threading.Timer" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * Author: Russell Stuart <russell-savannah@stuart.id.au>
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

	using System;
	using System.Collections;

	public sealed class Timer : MarshalByRefObject, IDisposable
	{
		//
		// Static data used by the background thread that implements the timers.
		//
		private static AlarmClock	alarmClock;		// Implements timeouts
		private static Queue		disposeQueue;	// Timers waiting to die
		private static long			now;			// The current time
		private static AutoResetEvent threadWakeup;	// Event the thread sleeps on
		private static Thread		timerThread; 	// Thread that fires timers
		//
		// Internal object data.
		//
		private AlarmClock.IAlarm	alarm;			// Thing that does the work
		private bool				disposed;		// True if Dispose() called
		private TimerCallback		callback;		// Thing to fire when timer expires
		private Object				state;			// State info to pass to Callback
		private WaitHandle			notifyObject;	// Who do notify when object disposed

		//
		// Private constructor - used to ensure all constructors initialise
		// the object in the same way.
		//
		private Timer(TimerCallback callback, Object state)
		{
			//
			// Validate the parameters.
			//
			if (callback == null)
				throw new ArgumentNullException("callback");
			//
			// If this is the first timer constructed allocate resources
			// for the timer thread and start it.
			//
			lock (typeof(Timer))
			{
				if (Timer.alarmClock == null)
				{
					if (!Thread.CanStartThreads())
						throw new NotImplementedException();
					Timer.alarmClock = new AlarmClock();
					Timer.disposeQueue = new System.Collections.Queue();
					Timer.now = Timer.UtcMilliseconds();
					Timer.threadWakeup = new AutoResetEvent(false);
					Timer.timerThread = new Thread(new ThreadStart(Timer.Run));
					Timer.timerThread.IsBackground = true;
					Timer.timerThread.Start();
				}
			}
			//
			// Initialize the timer state.
			//
			lock (this)
			{
				this.disposed = false;
				this.alarm = Timer.alarmClock.CreateAlarm(
					new AlarmClock.AlarmExpiredHandler(this.fireTimer));
				this.callback = callback;
				this.state = state;
			}
		}

		//
		// Constructors.
		//
		public Timer(TimerCallback callback, Object state, int dueTime, int period)
			: this(callback, state)
		{
			this.Change(dueTime, period);
		}

		public Timer(TimerCallback callback, Object state,
					TimeSpan dueTime, TimeSpan period)
			: this(callback, state)
		{
			this.Change(dueTime, period);
		}

	#if !ECMA_COMPAT
		[CLSCompliant(false)]
		public Timer(TimerCallback callback, Object state,
					uint dueTime, uint period)
			: this(callback, state)
		{
			this.Change(dueTime, period);
		}

		public Timer(TimerCallback callback, Object state,
					long dueTime, long period)
			: this(callback, state)
		{
			this.Change(dueTime, period);
		}
	#endif // !ECMA_COMPAT

		//
		// Destructor.
		//
		~Timer()
		{
			DisposeInternal(null, false);
		}

		//
		// Change the current timer parameters.
		//
		public bool Change(int dueTime, int period)
		{
			//
			// Validate the parameters.
			//
			if (dueTime < -1)
			{
				throw new ArgumentOutOfRangeException
					("dueTime", _("ArgRange_NonNegOrNegOne"));
			}
			if (period < -1)
			{
				throw new ArgumentOutOfRangeException
					("period", _("ArgRange_NonNegOrNegOne"));
			}
			//
			// Apparently trying to change the state of a disposed timer
			// is legal.
			//
			lock (this)
			{
				if (this.disposed)
					return false;
			}
			//
			// Set the new state.
			//
			long due = dueTime == -1 ? AlarmClock.INFINITE : dueTime;
			long interval = period <= 0 ? AlarmClock.INFINITE : period;
			Timer.AdvanceTime(period);
			this.alarm.SetAlarm(due, interval);
			//
			// Wake up the background thread so it sees the new state.
			//
			Timer.threadWakeup.Set();
			return true;
		}

		public bool Change(TimeSpan dueTime, TimeSpan period)
		{
			return Change((long)dueTime.TotalMilliseconds, (long)period.TotalMilliseconds);
		}

	#if !ECMA_COMPAT
		[CLSCompliant(false)]
		public bool Change(uint dueTime, uint period)
		{
			return Change(UIntToMS(dueTime), UIntToMS(period));
		}

		public bool Change(long dueTime, long period)
		{
			return Change(LongToMS(dueTime), LongToMS(period));
		}
	#endif // !ECMA_COMPAT

		//
		// Dispose of this object.
		//
		public void Dispose()
		{
			DisposeInternal(null, true);
		}

		//
		// Dispose of this object and signal a particular wait
		// handle once the timer has been disposed.
		//
		public bool Dispose(WaitHandle notifyObject)
		{
			if (notifyObject == null)
				throw new ArgumentNullException("notifyObject");
			return DisposeInternal(notifyObject, true);
		}

		//
		// Internal version of "Dispose".
		//
		private bool DisposeInternal(WaitHandle notifyObject, bool disposing)
		{
			if(disposing)
			{
				// Add the timer to the dispose list only if it is disposed
				// by the application.
				lock (this)
				{
					if (this.disposed)
						return false;
					this.disposed = true;
					this.notifyObject = notifyObject;
				}
				this.alarm.SetAlarm(AlarmClock.INFINITE, AlarmClock.INFINITE);
				if (this.notifyObject != null)
				{
					lock (typeof(Timer))
						Timer.disposeQueue.Enqueue(this);
					Timer.threadWakeup.Set();
				}
				GC.SuppressFinalize(this);
			}
			else
			{
				// disable the timer
				// Hopefully the GC sets the references of garbage collected
				// objects to null in circular references like the delegate
				// in the alarm here. So we have to check if the alarm is
				// not null here before disabling it.
				if(this.alarm != null)
				{
					this.alarm.SetAlarm(AlarmClock.INFINITE,
										AlarmClock.INFINITE);
				}
			}
			return true;
		}

		//
		// Convert an unsigned integer value into a milliseconds value.
		//
		internal static int UIntToMS(uint value)
		{
			if (value > (uint)(Int32.MaxValue))
			{
				throw new ArgumentOutOfRangeException
					("value", _("ArgRange_NonNegOrNegOne"));
			}
			return unchecked((int)value);
		}

		//
		// Convert a long integer value into a milliseconds value.
		//
		internal static int LongToMS(long value)
		{
			if (value < (-1L) || value > (long)(Int32.MaxValue))
			{
				throw new ArgumentOutOfRangeException
					("value", _("ArgRange_NonNegOrNegOne"));
			}
			return unchecked((int)value);
		}

		//
		// Return the current UTC time in milliseconds.
		//
		private static long UtcMilliseconds()
		{
			return DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
		}

		//
		// Method that runs the timer thread.
		//
		private static void Run()
		{
			for (;;)
			{
				//
				// Free up any Timer's awaiting destruction.  Once this
				// is done the Timer is guarenteed to never fire again.
				//
				for (;;)
				{
					Timer timer;
					lock (typeof(Timer))
					{
						if (Timer.disposeQueue.Count == 0)
							break;
						timer = (Timer)Timer.disposeQueue.Dequeue();
					}
					WaitHandle notifyObject;
					lock (timer)
						notifyObject = timer.notifyObject;
					(notifyObject as ISignal).Signal();
				}
				//
				// Sleep until an alarm is due to go off.
				//
				long longMs = Timer.alarmClock.TimeTillAlarm;
				int ms = longMs > int.MaxValue ? int.MaxValue : (int)longMs;
				Timer.threadWakeup.WaitOne(ms, false);
				Timer.AdvanceTime(ms);
			}
		}

		//
		// Advance the time in the Alarm object so it is the same as the
		// real time.
		//
		private static void AdvanceTime(long timerPeriod)
		{
			long elapsed;
			lock (typeof(Timer))
			{
				long was = Timer.now;
				Timer.now = Timer.UtcMilliseconds();
				elapsed = Timer.now - was;
			}
			// if elapsed is less then zero the system time might have been changed to the past.
			// so dont sleep. This works well.
			if( elapsed < 0 ) {
				elapsed = 0;
			}
			// if elapsed is greater then x*timerPeriod the system time might have been changed to the future.
			// This is a workaround. but works.
			else if( timerPeriod > 0 && elapsed > 10*timerPeriod ) {
				elapsed = 0;
			}
			Timer.alarmClock.Sleep(elapsed);
		}

		//
		// Called by the Alarm object when a time expires.  Fire the
		// real Timer's callback.
		//
		private void fireTimer()
		{
			TimerCallback	callback;
			Object			state;
			lock (this)
			{
				callback = this.callback;
				state = this.state;
			}
			callback(state);
		}

		/// <summary>A generalised timeout class</summary>
		/// <remarks>
		/// <para>
		///     Think of an <see cref="AlarmClock"/> as an implementation of a
		///     real alarm clock - ie something that keeps track of time.  Use
		///     <see cref="AlarmClock.CreateAlarm"/> to create
		///     any number of alarms - ie things that call their
		///     <see cref="AlarmClock.AlarmHandler"/> event when the alarm clock's
		///     time reaches a certain value.   This value is set with
		///     <see cref="AlarmClock.Alarm.SetAlarm"/> - you can choose to have
		///     the alarm to fire just once or periodically.
		/// </para>
		///
		/// <para>
		///     The only thing this alarm clock lacks is a timer - ie something
		///     that advances the time.  You must do that for it by calling
		///     <see cref="AlarmClock.Sleep"/>, which advances the time by the
		///     number of "time units" passed.  Calling <c>Sleep</c> fires any
		///		alarms set in the elapsed period.  A time unit is what you
		///     define it to be - a tick, a second, a day or whatever.  You
		///     must to be careful to ensure you use the same definition when
		///     passing time intervals to <c>SetAlarm</c> and to <c>Sleep</c>.
		///     <see cref="AlarmClock.TimeTillAlarm"/> tells you how long until
		///     the next alarm will fire.
		/// </para>
		///
		/// <para>
		///     Note: timers / alarms are all pretty much identical, except
		///     in how they advance time.  Threads, message loops, busy waits
		///     are all possibilities.  Because this class omits that piece of
		///     the puzzle it can be used as the basis for all the others.
		/// </para>
		///
		/// <para>
		///     The time need to manipulate N alarms is O(log2(N)), so this
		///     should be efficient for huge numbers of alarms.
		/// </para>
		/// </remarks>
		internal class AlarmClock
		{
			/// <summary>Disable a alarm.</summary>
			public const long INFINITE = long.MaxValue;

			/// <summary>A priority queue of Alarms.</summary>
			/// <remarks>
			///     The sort algroithm used by the queue is guarenteed to be
			///     "stable", meaning that alarms due to go off at the same
			///     time will fire in the order they were added.
			/// </remarks>
			private class PriorityQueue
			{
				//
				// Priority queue represented as a balanced binary heap: the
				// two children of queue[n] are queue[2*n] and queue[2*n+1].
				// The priority queue is ordered on the nextExecutionTime
				// field: The Alarm with the lowest ExpiryTime is in queue[1]
				// (assuming the queue is nonempty).  For each node n in the
				// heap, and each descendant of n, d:
				//   n.ExpiryTime <= d.ExpiryTime. 
				//
				private Alarm[] queue = new Alarm[4];

				//
				// The number of timeouts in the priority queue.  (The Alarms
				// are stored in queue[1] up to queue[count]).
				//
				private int count = 0;

				/// <summary>Adds an alarm to the priority queue.</summary>
				public void Add(Alarm alarm)
				{
					//
					// Grow backing store if necessary
					//
					if (++this.count == this.queue.Length)
					{
						Alarm[] newQueue = new Alarm[2 * this.count];
						Array.Copy(this.queue, 1, newQueue, 1, this.count - 1);
						this.queue = newQueue;
					}
					this.queue[this.count] = alarm;
					this.fixUp(this.count);
				}

				/// <summary>Get the alarm next due to fire.</summary>
				/// <remarks>
				///     Return the "head Alarm" of the priority queue.  This
				///     is the Alarm with the lowest ExpiryTime.  <c>null</c>
				///     is returned if there is no alarm pending.
				/// </remarks>
				public Alarm Head
				{
					get { return this.queue[1]; }
				}

				/// <summary>Remove an alarm from the priority queue.</summary>
				public void Remove(Alarm alarm)
				{
					int index =
						System.Array.IndexOf(this.queue, alarm, 1, this.count);
					if (index == -1)		// Alarm not in the queue?
						return;
					int oldCount = this.count;
					this.count -= 1;
					if (index != oldCount)	// Not last alarm in the queue?
					{
						//
						// Delete the alarm by overwriting it with the last one
						// in the queue, then re-adjusting the heap to maintain
						// the invariant.
						//
						this.queue[index] = this.queue[oldCount];
						this.fixDown(index);
					}
					this.queue[oldCount] = null; // Prevent memory leaks!
				}

				//
				// Establishes the heap invariant (described above) assuming
				// the heap satisfies the invariant except possibly for the
				// leaf-node indexed by k (which may have a ExpiryTime less
				// than its parent's).
				//
				// This method functions by "promoting" queue[k] up the
				// hierarchy (by swapping it with its parent) repeatedly until
				// queue[k]'s expireTime is greater than or equal to that of
				// its parent.
				//
				private void fixUp(int k)
				{
					while (k > 1)
					{
						int j = k >> 1;
						if (this.queue[j].ExpiryTime <= this.queue[k].ExpiryTime)
							break;
						Alarm tmp = this.queue[j];
						this.queue[j] = this.queue[k];
						this.queue[k] = tmp;
						k = j;
					}
				}

				//
				// Establishes the heap invariant (described above) in the
				// subtree rooted at k, which is assumed to satisfy the heap
				// invariant except possibly for node k itself (which may have
				// a ExpiryTime greater than its children's).
				// 
				// This method functions by "demoting" queue[k] down the
				// hierarchy (by swapping it with its smaller child) repeatedly
				// until queue[k]'s expireTime is less than or equal to those
				// of its children.
				//
				private void fixDown(int k)
				{
					int j;
					while ((j = k << 1) <= this.count)
					{
						if (j < this.count && this.queue[j].ExpiryTime > this.queue[j+1].ExpiryTime)
							j += 1; // j indexes smallest kid
						if (this.queue[k].ExpiryTime <= this.queue[j].ExpiryTime)
							break;
						Alarm tmp = this.queue[j];
						this.queue[j] = this.queue[k];
						this.queue[k] = tmp;
						k = j;
					}
				}
			}

			public interface IAlarm
			{
				/// <summary>Set the Alarm.</summary>
				/// <param name="dueTime">The amount of time to delay before
				///     firing the event for the first time.  If this is 0, the
				///     event is fired immediately.  If this is
				///     <see cref="INFINITE"/> the Alarm is disabled, meaning
				///     it never fires.</param>
				/// <param name="period">After the event is fired, the time to
				///     delay before firing it again.  If this is
				///     <see cref="INFINITE"/> is will never be fired
				///     again.</param>
				///
				/// <remarks>
				///     The event to fire is passed to
				///     <see cref="CreateAlarm"/>.  The units used to measure
				///     time is up to you.  Just be sure they are consistent
				///     with what is passed to <see cref="Sleep"/>.
				/// </remarks>
				void SetAlarm(long dueTime, long period);
			}

			/// <summary>Fired when an alarm expires.</summary>
			public delegate void AlarmExpiredHandler();

			/// <summary>Implementation of an IAlarm.</summary>
			/// <remarks>
			///     No one is meant to look inside of this - except the Alarm
			///     Clock.
			/// </remarks>
			private class Alarm : IAlarm
			{
				public long					ExpiryTime;  // DISABLED or when next due
				public readonly AlarmExpiredHandler	AlarmExpired;
				private readonly AlarmClock	alarmClock;	// Our owner
				private long				period;		// Periodic interval

				private const long			DISABLED = -1;

				/// <summary>Create a new Alarm.</summary>
				public Alarm(AlarmClock alarmClock, AlarmExpiredHandler alarmExpired)
				{
					lock (this)
					{
						this.alarmClock = alarmClock;
						this.AlarmExpired = alarmExpired;
						this.ExpiryTime = DISABLED;
					}
				}

				public void SetAlarm(long dueTime, long period)
				{
					//
					// Check arguments.
					//
					if (dueTime < 0)
					{
						throw new ArgumentOutOfRangeException(
							"dueTime", _("ArgRange_NonNegative"));
					}
					if (period <= 0)
					{
						throw new ArgumentOutOfRangeException(
							"period", _("ArgRange_PositiveNonZero"));
					}
					//
					// Lock the clock while the alarm is scheduled for the
					// first time.
					//
					lock (this)
					{
						//
						// First a quick check to see if anything has changed.
						// This has to be done, because this routine may be
						// called by a finalizer wishing to clean up.  The
						// the finalizer will of been called by the garbage
						// collector because it was asked for more memory,
						// and there isn't any.  The thread asking for memory
						// may be in here, allocating a new Alarm, say.
						// Because it is in here, the AlarmClock object may
						// be locked.  If we should then try to lock the
						// AlarmClock, the finializer will block, which means
						// the memory won't be freed, which means the other
						// thread will remain blocked, which means we managed
						// to dead lock the garbage collector.  Oh joy.  A
						// new reason to hate threads.  And yes, this really
						// did happen during testing.
						//
						// The key to this mess is to realise that if this
						// object is being garbage collected, there must be
						// no references to it.  In particular, it is not in
						// the alarm queue, so its ExpiryTime must be DISABLED.
						// Also, since this object is being garbage collected,
						// the only reasonable thing the finalizer can be doing
						// in here it ensuring this alarm is disabled.
						//
						// So, if this call is disabling an already disabled
						// alarm, then under no circumstances must we try to
						// acquire a lock on AlarmClock, as doing so may
						// deadlock the garbage collector.
						//
						if (dueTime == INFINITE && this.ExpiryTime == DISABLED)
							return;
					}
					//
					// The locks must be released and re-acquired in the same
					// order as Sleep/Advance does it below, or again we risk
					// deadlock.
					//
					lock (this.alarmClock)
					{
						lock (this)
						{
							if (this.ExpiryTime != DISABLED)
								this.alarmClock.pending.Remove(this);
							if (dueTime == INFINITE)
							{
								this.ExpiryTime = DISABLED;
								return;
							}
							this.period = period;
							this.ExpiryTime = this.alarmClock.now + dueTime;
							this.alarmClock.pending.Add(this);
						}
					}
				}

				/// <summary>Advance the time for this alarm.</summary>
				/// <param name="to">The proposed new value for the
				///     time.</param>
				/// <returns>The time this alarm is due to go off.</returns>
				///
				/// <remarks>
				///     Must be called while the lock on the clock is held.
				///     If the Alarm would expire in the interval passed it
				///     is rescheduled or disabled, depending on its settings.
				/// </remarks>
				public long Advance(long to)
				{
					long oldExpiryTime;
					lock (this)
					{
						oldExpiryTime = this.ExpiryTime;
						if (this.ExpiryTime <= to)
						{
							this.alarmClock.pending.Remove(this);
							if (this.period == INFINITE)
								this.ExpiryTime = DISABLED;
							else
							{
								this.ExpiryTime += this.period;
								this.alarmClock.pending.Add(this);
							}
						}
					}
					return oldExpiryTime;
				}
			};

			//
			// The current time.
			//
			private long			now = 0;

			//
			// A list of all enabled Alarm's.
			//
			private PriorityQueue	pending = new PriorityQueue();

			/// <summary>Create a new IAlarm.</summary>
			/// <remarks>
			///     The IAlarm is initially disabled.
			/// </remarks>
			public IAlarm CreateAlarm(AlarmExpiredHandler alarmExpired)
			{
				if (alarmExpired == null)
					throw new ArgumentNullException("alarmExpired");
				return new Alarm(this, alarmExpired);
			}

			/// <summary>Advance the time.</summary>
			/// <param name="period">The amount to advance the time by.</param>
			///
			/// <remarks>
			///     Advance the time, and fire all <see cref="IAlarm"/>'s that
			///     exipred during that period.  Because this method may fire
			///     events it must be called *OUTSIDE OF ALL LOCKS*.  The units
			///     used to measure time are up to you - just be sure to keep
			///     them consistent with <see cref="Alarm.SetAlarm"/>.  
			/// </remarks>
			public void Sleep(long period)
			{
				for (;;)
				{
					Alarm nextAlarm;
					lock (this)
					{
						//
						// Find the event to occur.
						//
						long endSleep = this.now + period;
						nextAlarm = this.pending.Head;
						//
						// If there another alarm in the queue?
						//
						if (nextAlarm == null)
						{
							this.now = endSleep;
							return;
						}
						//
						// Is the alarm due to go off?
						//
						long nextExpiryTime = nextAlarm.Advance(endSleep);
						if (nextExpiryTime > endSleep)
						{
							this.now = endSleep;
							return;
						}
						//
						// An alarm has expired.  Advance the time to equal
						// its current expiry time.
						//
						period -= nextExpiryTime - this.now;
						this.now = nextExpiryTime;
					}
					//
					// Must be called outside of the lock to avoid potential
					// deadlocks.
					//
					nextAlarm.AlarmExpired();
				}
			}

			/// <summary>The delay until the next alarm will fire.</summary>
			/// <returns>
			///     The amount of time until the next <see cref="IAlarm"/>
			///     will fire, or <see cref="INFINITE"/> if there is no
			///     <c>Alarm</c>'s pending.  The units are the same as for the
			///     period passed to <see cref="Sleep"/>.
			/// </returns>
			public long TimeTillAlarm
			{
				get
				{
					lock (this)
					{
						Alarm nextAlarm = this.pending.Head;
						if (nextAlarm == null)
							return INFINITE;
						return nextAlarm.ExpiryTime - this.now;
					}
				}
			}
		}
	}; // class Timer
}; // namespace System.Threading
