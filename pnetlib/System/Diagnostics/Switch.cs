/*
 * Switch.cs - Implementation of the "System.Diagnostics.Switch" class.
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

using System.Collections;
using System.Collections.Specialized;

public abstract class Switch
{
	// Internal state.
	private String description;
	private String displayName;
	private int switchSetting;
	private bool switchInitialized;
	private static Hashtable switchValues;

	// Constructor.
	protected Switch(String displayName, String description)
			{
				if(displayName == null)
				{
					this.displayName = String.Empty;
				}
				else
				{
					this.displayName = displayName;
				}
				this.description = description;
				this.switchSetting = 0;
				this.switchInitialized = false;
			}

	// Description of the switch.
	public String Description
			{
				get
				{
					return description;
				}
			}

	// Display name for the switch.
	public String DisplayName
			{
				get
				{
					return displayName;
				}
			}

	// Get or set the switch value.
	protected int SwitchSetting
			{
				get
				{
					if(!switchInitialized)
					{
						switchSetting = GetConfigSetting(displayName);
						switchInitialized = true;
						OnSwitchSettingChanged();
					}
					return switchSetting;
				}
				set
				{
					switchSetting = value;
					switchInitialized = true;
					OnSwitchSettingChanged();
				}
			}

	// Notify the subclass of a switch setting change.
	protected virtual void OnSwitchSettingChanged()
			{
				// Nothing to do here.
			}

	// Load the switch values from the configuration into a hash table.
	private static void LoadSwitchValues(Hashtable values)
			{
				Hashtable switches;
				lock(typeof(Trace))
				{
					Trace.Initialize();
					switches = Trace.switches;
				}
				if(switches != null)
				{
					IDictionaryEnumerator e = switches.GetEnumerator();
					while(e.MoveNext())
					{
						try
						{
							values[e.Key] = Int32.Parse((String)(e.Value));
						}
						catch(Exception)
						{
							// Ignore parsing errors.
						}
					}
				}
			}

	// Get a switch value from the configuration.
	private static int GetConfigSetting(String displayName)
			{
				lock(typeof(Switch))
				{
					if(switchValues == null)
					{
						switchValues = CollectionsUtil
							.CreateCaseInsensitiveHashtable();
						LoadSwitchValues(switchValues);
					}
					Object value = switchValues[displayName];
					if(value != null)
					{
						return (int)value;
					}
					else
					{
						return 0;
					}
				}
			}

}; // class Switch

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
