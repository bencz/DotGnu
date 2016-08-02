/*
 * TraceSwitch.cs - Implementation of the
 *			"System.Diagnostics.TraceSwitch" class.
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

public class TraceSwitch : Switch
{
	// Constructor.
	public TraceSwitch(String displayName, String description)
			: base(displayName, description) {}

	// Get or set the trace level.
	public TraceLevel Level
			{
				get
				{
					return (TraceLevel)SwitchSetting;
				}
				set
				{
					if(((int)value) < ((int)(TraceLevel.Off)) ||
					   ((int)value) > ((int)(TraceLevel.Verbose)))
					{
						throw new ArgumentException(S._("Arg_TraceLevel"));
					}
					SwitchSetting = (int)value;
				}
			}

	// Determine if the level is one of the special values.
	public bool TraceError
			{
				get
				{
					return (Level == TraceLevel.Error);
				}
			}
	public bool TraceInfo
			{
				get
				{
					return (Level == TraceLevel.Info);
				}
			}
	public bool TraceVerbose
			{
				get
				{
					return (Level == TraceLevel.Verbose);
				}
			}
	public bool TraceWarning
			{
				get
				{
					return (Level == TraceLevel.Warning);
				}
			}

	// Notify the subclass of a switch setting change.
	protected override void OnSwitchSettingChanged()
			{
				// Validate values changed through "Switch.SwitchSetting".
				int setting = SwitchSetting;
				if(setting < ((int)(TraceLevel.Off)) ||
				   setting > ((int)(TraceLevel.Verbose)))
				{
					Trace.WriteLine(String.Format(S._("ArgRange_SwitchValue"),
												  setting, DisplayName));
					SwitchSetting = (int)(TraceLevel.Off);
				}
			}

}; // class TraceSwitch

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
