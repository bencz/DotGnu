/*
 * CounterCreationData.cs - Implementation of the
 *			"System.Diagnostics.CounterCreationData" class.
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

[Serializable]
[TypeConverter
	("System.Diagnostics.Design.CounterCreationDataConverter, System.Design")]
public class CounterCreationData
{
	// Internal state.
	private String counterName;
	private String counterHelp;
	private PerformanceCounterType counterType;

	// Constructor.
	public CounterCreationData()
			: this(String.Empty, String.Empty,
				   PerformanceCounterType.NumberOfItems32)
			{
				// Nothing to do here.
			}
	public CounterCreationData(String counterName, String counterHelp,
							   PerformanceCounterType counterType)
			{
				this.counterName = counterName;
				this.counterHelp = counterHelp;
				this.counterType = counterType;
			}

	// Get or set the object properties.
	[DefaultValue("")]
	[MonitoringDescription("CounterHelp")]
	public String CounterHelp
			{
				get
				{
					return counterHelp;
				}
				set
				{
					counterHelp = value;
				}
			}
	[DefaultValue("")]
	[TypeConverter
		("System.Diagnostics.Design.StringValueConverter, System.Design")]
	[MonitoringDescription("CounterName")]
	public String CounterName
			{
				get
				{
					return counterName;
				}
				set
				{
					counterName = value;
				}
			}
	[MonitoringDescription("CounterType")]
	public PerformanceCounterType CounterType
			{
				get
				{
					return counterType;
				}
				set
				{
					counterType = value;
				}
			}

}; // class CounterCreationData

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
