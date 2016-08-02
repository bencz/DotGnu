///
/// Copyright (c) 2004 Thong Nguyen (tum@veridicus.com)
///

using System;
using System.Threading;

namespace ThreadsDemo
{
	/// <summary>
	/// Implementation of the a developer and the "coding developers" algorithm using monitors.
	/// </summary>
	public class MonitorBasedDeveloper
		: Developer
	{
		public MonitorBasedDeveloper(string name, Developer left, Developer right, DeveloperController controller)
			: base(name, left, right, controller)
		{
		}

		private void Test(MonitorBasedDeveloper developer)
		{
			lock (typeof(Developer))
			{
				if (developer.Left.State != DeveloperState.Coding
					&& developer.State == DeveloperState.Waiting
					&& developer.Right.State != DeveloperState.Coding)
				{
					lock (developer)
					{
						developer.SetState(DeveloperState.Coding);
						Monitor.Pulse(developer);
					}
				}
			}
		}

		protected override void Pickup()
		{
			SetState(DeveloperState.Waiting);
			Test(this);

			lock (this)
			{
				if (State != DeveloperState.Coding)
				{
					Monitor.Wait(this);
				}
			}
		}

		protected override void Putdown()
		{
			SetState(DeveloperState.Thinking);

			Test((MonitorBasedDeveloper)Left);
			Test((MonitorBasedDeveloper)Right);
		}
	}
}
