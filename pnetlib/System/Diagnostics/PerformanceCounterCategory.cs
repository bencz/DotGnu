/*
 * PerformanceCounterCategory.cs - Implementation of the
 *			"System.Diagnostics.PerformanceCounterCategory" class.
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

namespace System.Diagnostics
{

#if CONFIG_EXTENDED_DIAGNOSTICS

public sealed class PerformanceCounterCategory
{
	// Internal state.
	private String categoryHelp;
	private String categoryName;
	private String machineName;

	// Constructor.
	public PerformanceCounterCategory()
			{
				machineName = ".";
			}
	public PerformanceCounterCategory(String categoryName)
			: this(categoryName, ".") {}
	public PerformanceCounterCategory(String categoryName, String machineName)
			{
				if(categoryName == null)
				{
					throw new ArgumentNullException("categoryName");
				}
				else if(categoryName == String.Empty)
				{
					throw new ArgumentException
						(S._("Arg_CounterCategory"));
				}
				if(machineName == null)
				{
					throw new ArgumentNullException("machineName");
				}
				this.categoryName = categoryName;
				this.machineName = machineName;
			}

	// Category properties.
	public String CategoryHelp
			{
				get
				{
					if(categoryName == null)
					{
						throw new InvalidOperationException
							(S._("Invalid_CategoryNotInited"));
					}
					return categoryHelp;
				}
			}
	public String CategoryName
			{
				get
				{
					return categoryName;
				}
				set
				{
					if(categoryName == null)
					{
						throw new ArgumentNullException("categoryName");
					}
					else if(categoryName == String.Empty)
					{
						throw new ArgumentException
							(S._("Arg_CounterCategory"));
					}
					categoryName = value;
				}
			}
	public String MachineName
			{
				get
				{
					return machineName;
				}
				set
				{
					if(machineName == null)
					{
						throw new ArgumentNullException("machineName");
					}
					machineName = value;
				}
			}

	// Determine if a particular counter exists.
	public bool CounterExists(String counterName)
			{
				if(counterName == null)
				{
					throw new ArgumentNullException("counterName");
				}
				if(categoryName == null)
				{
					throw new InvalidOperationException
						(S._("Invalid_CategoryNotInited"));
				}
				return false;	// We don't support performance counters.
			}
	public static bool CounterExists(String counterName,
									 String categoryName)
			{
				return (new PerformanceCounterCategory(categoryName))
							.CounterExists(counterName);
			}
	public static bool CounterExists(String counterName,
									 String categoryName,
									 String machineName)
			{
				return (new PerformanceCounterCategory
								(categoryName, machineName))
							.CounterExists(counterName);
			}

	// Create a performance counter category.
	public static PerformanceCounterCategory Create
				(String categoryName, String categoryHelp,
				 CounterCreationDataCollection counterData)
			{
				// Dummy only: we don't support performance counters.
				return new PerformanceCounterCategory(categoryName);
			}
	public static PerformanceCounterCategory Create
				(String categoryName, String categoryHelp,
				 String counterName, String counterHelp)
			{
				// Dummy only: we don't support performance counters.
				return new PerformanceCounterCategory(categoryName);
			}

	// Delete a particular performance counter category.
	public static void Delete(String categoryName)
			{
				if(categoryName == null)
				{
					throw new ArgumentNullException("categoryName");
				}
			}

	// Determine if a particluar category has been registered.
	public static bool Exists(String categoryName)
			{
				return Exists(categoryName, ".");
			}
	public static bool Exists(String categoryName, String machineName)
			{
				if(categoryName == null)
				{
					throw new ArgumentNullException("categoryName");
				}
				else if(categoryName == String.Empty)
				{
					throw new ArgumentException
						(S._("Arg_CounterCategory"));
				}
				if(machineName == null)
				{
					throw new ArgumentNullException("machineName");
				}
				return false;	// We don't support performance counters.
			}

	// Get a list of all registered categories.
	public static PerformanceCounterCategory[] GetCategories()
			{
				return GetCategories(".");
			}
	public static PerformanceCounterCategory[] GetCategories
				(String machineName)
			{
				if(machineName == null)
				{
					throw new ArgumentNullException("machineName");
				}
				return new PerformanceCounterCategory [0];
			}

	// Get the performance counters in this category.
	public PerformanceCounter[] GetCounters()
			{
				return new PerformanceCounter [0];
			}
	public PerformanceCounter[] GetCounters(String instanceName)
			{
				if(instanceName == null)
				{
					throw new ArgumentNullException("instanceName");
				}
				return new PerformanceCounter [0];
			}

	// Get the instance names associated with this category.
	public String[] GetInstanceNames()
			{
				if(categoryName == null)
				{
					throw new InvalidOperationException
						(S._("Invalid_CategoryNotInited"));
				}
				return new String [0];
			}

	// Determine if a particular instance name exists.
	public bool InstanceExists(String instanceName)
			{
				if(instanceName == null)
				{
					throw new ArgumentNullException("instanceName");
				}
				return false;	// We don't support performance counters.
			}
	public static bool InstanceExists(String instanceName, String categoryName)
			{
				return (new PerformanceCounterCategory(categoryName))
							.InstanceExists(instanceName);
			}
	public static bool InstanceExists(String instanceName, String categoryName,
							          String machineName)
			{
				return (new PerformanceCounterCategory
							(categoryName, machineName))
								.InstanceExists(instanceName);
			}

	// Read all of the instance data associated with a category.
	public InstanceDataCollectionCollection ReadCategory()
			{
				// We don't support performance counters,
				// so return an empty result collection.
				if(categoryName == null)
				{
					throw new InvalidOperationException
						(S._("Invalid_CategoryNotInited"));
				}
				return new InstanceDataCollectionCollection();
			}

}; // class PerformanceCounterCategory

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
