/*
 * PerformanceCounterInstaller.cs - Implementation of the
 *	    "System.Configuration.Install.PerformanceCounterInstaller" class.
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

#if CONFIG_COMPONENT_MODEL && CONFIG_EXTENDED_DIAGNOSTICS

using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;

public class PerformanceCounterInstaller : ComponentInstaller
{
	// Internal state.
	private String categoryHelp;
	private String categoryName;
	private CounterCreationDataCollection counters;
	private UninstallAction action;

	// Constructor.
	public PerformanceCounterInstaller()
			{
				categoryHelp = String.Empty;
				categoryName = String.Empty;
				counters = new CounterCreationDataCollection();
				action = UninstallAction.Remove;
			}

	// Get or set this object's properties.
	[DefaultValue("")]
	[MonitoringDescription("PCI_CategoryHelp")]
	public String CategoryHelp
			{
				get
				{
					return categoryHelp;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					categoryHelp = value;
				}
			}
	[DefaultValue("")]
	[TypeConverter
		("System.Diagnostics.Design.StringValueConverter, System.Design")]
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
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[MonitoringDescription("PCI_Counters")]
	public CounterCreationDataCollection Counters
			{
				get
				{
					return counters;
				}
			}
	[MonitoringDescription("PCI_UninstallAction")]
	public UninstallAction UninstallAction
			{
				get
				{
					return action;
				}
				set
				{
					action = value;
				}
			}

	// Copy installation properties from a component.
	public override void CopyFromComponent(IComponent component)
			{
				PerformanceCounter pc = (component as PerformanceCounter);
				if(pc != null)
				{
					if(pc.CounterHelp == null)
					{
						throw new ArgumentException
							(S._("Installer_HelpIsNull"), "component");
					}
					if(pc.CategoryName == null)
					{
						throw new ArgumentException
							(S._("Installer_NameIsNull"), "component");
					}
					categoryHelp = pc.CounterHelp;
					categoryName = pc.CategoryName;
				}
				else
				{
					throw new ArgumentException
						(S._("Installer_NotAPerformanceCounter"), "component");
				}
			}

	// Installation methods.  We don't support performance counters
	// in this implementation.
	public override void Install(IDictionary stateSaver)
			{
				// Nothing to do here in this implementation.
			}
	public override void Rollback(IDictionary savedState)
			{
				// Nothing to do here, because there will never
				// be an installation to be rolled back.
			}
	public override void Uninstall(IDictionary savedState)
			{
				// Nothing to do here, because there will never
				// be an installation to be uninstalled.
			}

}; // class PerformanceCounterInstaller

#endif // CONFIG_COMPONENT_MODEL && CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
