/*
 * InstanceData.cs - Implementation of the
 *			"System.Diagnostics.InstanceData" class.
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

public class InstanceData
{
	// Internal state.
	private String instanceName;
	private CounterSample sample;

	// Constructor.
	public InstanceData(String instanceName, CounterSample sample)
			{
				this.instanceName = instanceName;
				this.sample = sample;
			}

	// Properties.
	public String InstanceName
			{
				get
				{
					return instanceName;
				}
			}
	public long RawValue
			{
				get
				{
					return sample.RawValue;
				}
			}
	public CounterSample Sample
			{
				get
				{
					return sample;
				}
			}

}; // class InstanceData

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
