///
/// Copyright (c) 2004 Thong Nguyen (tum@veridicus.com)
///

using System;

namespace ThreadsDemo
{
	/// <summary>
	/// Constructs developers.
	/// </summary>
	public abstract class DeveloperFactory
	{
		private static DeveloperFactory c_Factory;

		static DeveloperFactory()
		{
			c_Factory = new MonitorBasedDeveloperFactory();
		}


		/// <summary>
		/// Get the default developer factory.
		/// </summary>
		public static DeveloperFactory GetFactory()
		{
			return c_Factory;
		}

		/// <summary>
		/// Constructs a new developer.
		/// </summary>
		/// <param name="name">The name of the developer.</param>
		/// <param name="left">The developer to the immediate left.</param>
		/// <param name="right">The developer to the immediate right.</param>
		/// <param name="controller">The controller.</param>
		/// <returns>The newly constructed developer.</returns>
		public abstract Developer NewDeveloper(string name, Developer left, Developer right, DeveloperController controller);
	}
}
