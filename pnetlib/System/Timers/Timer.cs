/*
 * Timer.cs - Implementation of the
 *		"System.Timers.Timer" class.
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

namespace System.Timers
{

#if !ECMA_COMPAT && CONFIG_COMPONENT_MODEL

using System.Threading;
using System.ComponentModel;

// Don't use this class.  Use "System.Threading.Timer" instead.

[DefaultProperty("Interval")]
[DefaultEvent("Elapsed")]
public class Timer : Component, ISupportInitialize
{
	// Internal state.
	private bool autoReset;
	private bool enabled;
	private double interval;
	private ISynchronizeInvoke synchronizingObject;
	private System.Threading.Timer timer;

	// Constructors.
	public Timer()
			{
				this.autoReset = true;
				this.enabled = false;
				this.interval = 100.0;
				this.synchronizingObject = null;
			}
	public Timer(double interval)
			: this()
			{
				Interval = interval;
			}

	// Get or set this object's properties.
	[TimersDescription("TimerAutoReset")]
	[Category("Behavior")]
	[DefaultValue(true)]
	public bool AutoReset
			{
				get
				{
					return autoReset;
				}
				set
				{
					autoReset = value;
				}
			}
	[TimersDescription("TimerEnabled")]
	[Category("Behavior")]
	[DefaultValue(false)]
	public bool Enabled
			{
				get
				{
					return enabled;
				}
				set
				{
					if(enabled != value)
					{
						enabled = value;
						if(value)
						{
							ActivateTimer();
						}
						else
						{
							if(timer != null)
							{
								timer.Dispose();
								timer = null;
							}
						}
					}
				}
			}
	[TimersDescription("TimerInterval")]
	[Category("Behavior")]
	[DefaultValue(100.0)]
	[RecommendedAsConfigurable(true)]
	public double Interval
			{
				get
				{
					return interval;
				}
				set
				{
					if(value < 0.0)
					{
						throw new ArgumentException
							(S._("ArgRange_NonNegative"));
					}
					interval = value;
				}
			}
	public override ISite Site
			{
				get
				{
					return base.Site;
				}
				set
				{
					base.Site = value;
				}
			}
	[TimersDescription("TimerSynchronizingObject")]
	public ISynchronizeInvoke SynchronizingObject
			{
				get
				{
					return synchronizingObject;
				}
				set
				{
					synchronizingObject = value;
				}
			}

	// Begin initialization.
	public void BeginInit()
			{
				// Not used in this implementation.
			}

	// Close the timer.
	public void Close()
			{
				Dispose(true);
			}

	// End initialization.
	public void EndInit()
			{
				// Not used in this implementation.
			}

	// Start the timer.
	public void Start()
			{
				Enabled = true;
			}

	// Stop the timer.
	public void Stop()
			{
				Enabled = false;
			}

	// Event that is raised when the timer elapses.
	[TimersDescription("TimerIntervalElapsed")]
	[Category("Behavior")]
	public event ElapsedEventHandler Elapsed;

	// Dispose of this timer.
	protected override void Dispose(bool disposing)
			{
				if(timer != null)
				{
					timer.Dispose();
					timer = null;
				}
			}

	// Handle expiry of the timer.
	private static void TimerExpired(Object state)
			{
				Timer timer = (Timer)state;
				if(!(timer.autoReset))
				{
					timer.enabled = false;
					timer.timer = null;
				}
				if(timer.Elapsed != null)
				{
					timer.Elapsed(timer, new ElapsedEventArgs(DateTime.Now));
				}
			}

	// Activate the timer.
	private void ActivateTimer()
			{
				long due = (long)interval;
				long period = (autoReset ? due : -1);
				timer = new System.Threading.Timer
					(new TimerCallback(TimerExpired), this, due, period);
			}

}; // class Timer

#endif // !ECMA_COMPAT && CONFIG_COMPONENT_MODEL

}; // namespace System.Timers
