/*
 * ProcessModule.cs - Implementation of the
 *			"System.Diagnostics.ProcessModule" class.
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

// We don't support module information blocks, because they aren't
// portable to non-Windows operating systems.  So this class returns
// dummy information.

[Designer("System.Diagnostics.Design.ProcessModuleDesigner, System.Design")]
public class ProcessModule
#if CONFIG_COMPONENT_MODEL
	: Component
#endif
{
	// Internal state.
	private String fileName;
	private String moduleName;

	// Constructor.
	internal ProcessModule(String fileName, String moduleName)
			{
				this.fileName = fileName;
				this.moduleName = moduleName;
			}

	// Module properties.
	[MonitoringDescription("ProcModBaseAddress")]
	public IntPtr BaseAddress
			{
				get
				{
					return IntPtr.Zero;
				}
			}
	[MonitoringDescription("ProcModEntryPointAddress")]
	public IntPtr EntryPointAddress
			{
				get
				{
					return IntPtr.Zero;
				}
			}
	[MonitoringDescription("ProcModFileName")]
	public String FileName
			{
				get
				{
					return fileName;
				}
			}
	[Browsable(false)]
	public FileVersionInfo FileVersionInfo
			{
				get
				{
					return FileVersionInfo.GetVersionInfo(fileName);
				}
			}
	[MonitoringDescription("ProcModModuleMemorySize")]
	public int ModuleMemorySize
			{
				get
				{
					return 0;
				}
			}
	[MonitoringDescription("ProcModModuleName")]
	public String ModuleName
			{
				get
				{
					return moduleName;
				}
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return ModuleName;
			}

}; // class ProcessModule

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
