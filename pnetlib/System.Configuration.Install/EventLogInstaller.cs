/*
 * EventLogInstaller.cs - Implementation of the
 *	    "System.Configuration.Install.EventLogInstaller" class.
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

public class EventLogInstaller : ComponentInstaller
{
	// Internal state.
	private String log;
	private String source;
	private UninstallAction action;

	// Constructor.
	public EventLogInstaller()
			{
				log = String.Empty;
				source = String.Empty;
				action = UninstallAction.Remove;
			}

	// Get or set this object's properties.
	[TypeConverter
		("System.Diagnostics.Design.StringValueConverter, System.Design")]
	public String Log
			{
				get
				{
					return log;
				}
				set
				{
					log = value;
				}
			}
	[TypeConverter
		("System.Diagnostics.Design.StringValueConverter, System.Design")]
	public String Source
			{
				get
				{
					return source;
				}
				set
				{
					source = value;
				}
			}
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
				EventLog el = (component as EventLog);
				if(el != null)
				{
					if(el.Log == null)
					{
						throw new ArgumentException
							(S._("Installer_LogIsNull"), "component");
					}
					if(el.Source == null)
					{
						throw new ArgumentException
							(S._("Installer_SourceIsNull"), "component");
					}
					log = el.Log;
					source = el.Source;
				}
				else
				{
					throw new ArgumentException
						(S._("Installer_NotAnEventLog"), "component");
				}
			}

	// Determine if another installer is equivalent to this one.
	public override bool IsEquivalentInstaller
				(ComponentInstaller otherInstaller)
			{
				EventLogInstaller other = (otherInstaller as EventLogInstaller);
				if(other != null)
				{
					if(log == other.log && source == other.source)
					{
						return true;
					}
				}
				return false;
			}

	// Installation methods.  We don't support event logs in this
	// implementation as they are highly Windows NT specific.
	public override void Install(IDictionary stateSaver)
			{
				throw new PlatformNotSupportedException
					(S._("Installer_NoEventLogs"));
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

}; // class EventLogInstaller

#endif // CONFIG_COMPONENT_MODEL && CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
