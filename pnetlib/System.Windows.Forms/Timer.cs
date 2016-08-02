/*
 * Timer.cs - Implementation of the
 *			"System.Windows.Forms.Timer" class.
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

namespace System.Windows.Forms
{

using System.Drawing.Toolkit;
using System.ComponentModel;

public class Timer
#if CONFIG_COMPONENT_MODEL
	: Component
#else
	: IDisposable
#endif
{
	// Internal state.
	private bool enabled;
	private int interval;
	private Object timerCookie;
	private EventHandler onTimer;
	
	static private EventHandler EvExpire = new EventHandler(Expire);
	// Constructors.
	public Timer()
			{
				this.enabled = false;
				this.interval = 100;
			}
#if CONFIG_COMPONENT_MODEL
	public Timer(IContainer container) : this()
			{
				container.Add(this);
			}
#else

	// Destructor.
	~Timer()
			{
				Dispose(false);
			}
#endif

	// Enable or disable the timer.
	public virtual bool Enabled
			{
				get
				{
					return enabled;
				}
				set
				{
					if(enabled != value) {
						enabled = value;
						this.CheckTimer();
					}
				}
			}

	// Get or set the timer interval.
	public int Interval
			{
				get
				{
					return interval;
				}
				set
				{
					if(value < 1)
					{
						throw new ArgumentException
							(S._("SWF_InvalidInterval"), "value");
					}
					lock(this)
					{
						if(interval != value)
						{
							bool saveEnabled = enabled;
							Enabled = false;
							interval = value;
							Enabled = saveEnabled;
						}
					}
				}
			}
			
	void CheckTimer() {
		lock(this)
		{
			if( enabled && null != onTimer ) {
				if( null == timerCookie ) {
					timerCookie = ToolkitManager.Toolkit
							.RegisterTimer
							(this, this.interval,EvExpire);
				}
			}
			else {
				if( null != timerCookie ) {
					ToolkitManager.Toolkit.UnregisterTimer
							(timerCookie);
					timerCookie = null;
				}
			}
		}
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

	// Convert this object into a string.
	public override String ToString()
			{
				return base.ToString() + ", Interval: " + interval.ToString();
			}

	// Event that is emitted when the timer expires.
	public event EventHandler Tick {
		add  {
			this.onTimer = (EventHandler)Delegate.Combine(this.onTimer,value);
			this.CheckTimer();
		}
		remove {
			this.onTimer = (EventHandler)Delegate.Remove(this.onTimer,value);
			this.CheckTimer();
		}
	}

	// Dispose of the timer.
#if !CONFIG_COMPONENT_MODEL
	public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
	protected virtual void Dispose(bool disposing)
#else
	protected override void Dispose(bool disposing)
#endif
			{
				Enabled = false;
			}

	// Method that is called by the toolkit when the timer expires.
	static private void Expire(Object sender, EventArgs e)
			{
				Timer timer = (Timer) sender;
				if( null != timer && null != timer.onTimer ) {
					timer.onTimer(timer,e);
				}
			}

}; // class Timer

}; // namespace System.Windows.Forms
