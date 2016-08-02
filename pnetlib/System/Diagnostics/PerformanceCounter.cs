/*
 * PerformanceCounter.cs - Implementation of the
 *			"System.Diagnostics.PerformanceCounter" class.
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

using System.ComponentModel;

// Note: this class is mostly a place-holder.  We don't actually read
// the system performance counters as they are highly Windows-specific
// and probably insecure to access anyway.

#if CONFIG_COMPONENT_MODEL
[Designer("Microsoft.VisualStudio.Install.PerformanceCounterDesigner, Microsoft.VisualStudio")]
[InstallerType("System.Diagnostics.PerformanceCounterInstaller,System.Configuration.Install")]
#endif
public sealed class PerformanceCounter
#if CONFIG_COMPONENT_MODEL
	: Component, ISupportInitialize
#endif
{
	// Internal state.
	private String categoryName;
	private String counterHelp;
	private String counterName;
	private PerformanceCounterType counterType;
	private String instanceName;
	private String machineName;
	private long rawValue;
	private bool readOnly;

	// The default file mapping size to use.
	public static int DefaultFileMappingSize = 0x80000;

	// Constructor.
	public PerformanceCounter() {}
	public PerformanceCounter(String categoryName, String counterName)
			: this(categoryName, counterName, "", ".") {}
	public PerformanceCounter(String categoryName, String counterName,
							  bool readOnly)
			: this(categoryName, counterName, "", ".")
			{
				this.readOnly = readOnly;
			}
	public PerformanceCounter(String categoryName, String counterName,
							  String instanceName)
			: this(categoryName, counterName, instanceName, ".") {}
	public PerformanceCounter(String categoryName, String counterName,
							  String instanceName, bool readOnly)
			: this(categoryName, counterName, instanceName, ".")
			{
				this.readOnly = readOnly;
			}
	public PerformanceCounter(String categoryName, String counterName,
							  String instanceName, String machineName)
			{
				if(categoryName == null)
				{
					throw new ArgumentNullException("categoryName");
				}
				if(counterName == null)
				{
					throw new ArgumentNullException("counterName");
				}
				if(instanceName == null)
				{
					throw new ArgumentNullException("instanceName");
				}
				if(machineName == null)
				{
					throw new ArgumentNullException("machineName");
				}
				this.categoryName = categoryName;
				this.counterName = counterName;
				this.instanceName = instanceName;
				this.machineName = machineName;
				this.counterType = (PerformanceCounterType)
					Enum.Parse(typeof(PerformanceCounterType), counterName);
			}

	// Counter properties.
	[ReadOnly(true)]
	[RecommendedAsConfigurable(true)]
	[TypeConverter
		("System.Diagnostics.Design.CategoryValueConverter, System.Design")]
	[DefaultValue("")]
	public String CategoryName
			{
				get
				{
					return categoryName;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					categoryName = value;
				}
			}
	[MonitoringDescription("PC_CounterHelp")]
	[ReadOnly(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public String CounterHelp
			{
				get
				{
					if(categoryName == null)
					{
						throw new InvalidOperationException
							(S._("Invalid_CounterNotInited"));
					}
					return counterHelp;
				}
			}
	[TypeConverter
		("System.Diagnostics.Design.CounterNameConverter, System.Design")]
	[ReadOnly(true)]
	[RecommendedAsConfigurable(true)]
	[DefaultValue("")]
	public String CounterName
			{
				get
				{
					return counterName;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					counterName = value;
				}
			}
	[MonitoringDescription("PC_CounterType")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public PerformanceCounterType CounterType
			{
				get
				{
					if(categoryName == null)
					{
						throw new InvalidOperationException
							(S._("Invalid_CounterNotInited"));
					}
					return counterType;
				}
			}
	[TypeConverter
		("System.Diagnostics.Design.InstanceNameConverter, System.Design")]
	[ReadOnly(true)]
	[DefaultValue("")]
	[RecommendedAsConfigurable(true)]
	public String InstanceName
			{
				get
				{
					return instanceName;
				}
				set
				{
					instanceName = value;
				}
			}
	[Browsable(false)]
	[DefaultValue(".")]
	[RecommendedAsConfigurable(true)]
	public String MachineName
			{
				get
				{
					return machineName;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					machineName = value;
				}
			}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	[MonitoringDescription("PC_RawValue")]
	public long RawValue
			{
				get
				{
					if(readOnly)
					{
						throw new InvalidOperationException
							(S._("Invalid_ReadOnly"));
					}
					return rawValue;
				}
				set
				{
					rawValue = value;
				}
			}
	[DefaultValue(true)]
	[Browsable(false)]
	[MonitoringDescription("PC_ReadOnly")]
	public bool ReadOnly
			{
				get
				{
					return readOnly;
				}
				set
				{
					if(readOnly != value)
					{
						readOnly = value;
						Close();
					}
				}
			}

	// Implement the ISupportsInitialize interface.
	public void BeginInit()
			{
				// Nothing to do here.
			}
	public void EndInit()
			{
				// Nothing to do here.
			}

	// Close the performance counter.
	public void Close()
			{
				// Nothing to do here.
			}

	// Decrement the performance counter's raw value.
	public long Decrement()
			{
				if(readOnly)
				{
					throw new InvalidOperationException
						(S._("Invalid_ReadOnly"));
				}
				else if(categoryName == null)
				{
					throw new InvalidOperationException
						(S._("Invalid_CounterNotInited"));
				}
				lock(this)
				{
					--rawValue;
					return rawValue;
				}
			}

	// Increment the performance counter's raw value.
	public long Increment()
			{
				if(readOnly)
				{
					throw new InvalidOperationException
						(S._("Invalid_ReadOnly"));
				}
				else if(categoryName == null)
				{
					throw new InvalidOperationException
						(S._("Invalid_CounterNotInited"));
				}
				lock(this)
				{
					++rawValue;
					return rawValue;
				}
			}

	// Increment the performance counter's raw value by a particular amount.
	public long IncrementBy(long value)
			{
				if(readOnly)
				{
					throw new InvalidOperationException
						(S._("Invalid_ReadOnly"));
				}
				else if(categoryName == null)
				{
					throw new InvalidOperationException
						(S._("Invalid_CounterNotInited"));
				}
				lock(this)
				{
					rawValue += value;
					return rawValue;
				}
			}

	// Get the next counter sample.
	public CounterSample NextSample()
			{
				if(categoryName == null)
				{
					throw new InvalidOperationException
						(S._("Invalid_CounterNotInited"));
				}
				return new CounterSample(rawValue, 0, 0, 0, 0, 0, CounterType);
			}

	// Get the next counter sample value.
	public float NextValue()
			{
				return CounterSampleCalculator
					.ComputeCounterValue(NextSample());
			}

	// Remove a particular instance from this counter.
	public void RemoveInstance()
			{
				if(readOnly)
				{
					throw new InvalidOperationException
						(S._("Invalid_ReadOnly"));
				}
				else if(categoryName == null)
				{
					throw new InvalidOperationException
						(S._("Invalid_CounterNotInited"));
				}
			}

	// Dispose of this object.
	protected override void Dispose(bool disposing)
			{
				Close();
			}

	// Close resources that are shared between performance counters.
	public static void CloseSharedResources()
			{
				// Nothing to do in this implementation.
			}

}; // class PerformanceCounter

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
