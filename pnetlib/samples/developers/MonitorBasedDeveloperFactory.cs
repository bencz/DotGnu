///
/// Copyright (c) 2004 Thong Nguyen (tum@veridicus.com)
///

using System;

namespace ThreadsDemo
{
	/// <summary>
	/// DeveloperFactory for MonitorBasedDevelopers.
	/// </summary>
	public class MonitorBasedDeveloperFactory
		: DeveloperFactory
	{
		public MonitorBasedDeveloperFactory()
		{
		}

		public override Developer NewDeveloper(string name, Developer left, Developer right, DeveloperController controller)
		{
			return new MonitorBasedDeveloper(name, left, right, controller);
		}

	}
}
