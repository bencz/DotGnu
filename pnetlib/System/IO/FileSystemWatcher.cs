/*
 * FileSystemWatcher.cs - Implementation of the
 *		"System.IO.FileSystemWatcher" class.
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

namespace System.IO
{

#if CONFIG_WIN32_SPECIFICS && CONFIG_COMPONENT_MODEL

using System.ComponentModel;
using System.Threading;

// File system watching is highly platform-specific, and arguably a
// security weakness.  A program could watch for activity on selected
// files even if they cannot access the file's contents.  This may
// allow the program to perform traffic analysis to determine what is
// actually happening.
//
// Because of this, we allow programs to register to watch filesystem
// events, but we never actually deliver them.  This satisfies the API
// requirements in a platform-neutral fashion without creating a security
// hole at the same time.

[DefaultEvent("Changed")]
public class FileSystemWatcher : Component, ISupportInitialize
{
	// Internal state.
	private String path;
	private String filter;
	private bool enableRaisingEvents;
	private bool includeSubdirectories;
	private int internalBufferSize;
	private NotifyFilters notifyFilter;
	private ISynchronizeInvoke synchronizingObject;

	// Constructors.
	public FileSystemWatcher()
			{
				this.path = String.Empty;
				this.filter = "*.*";
				this.internalBufferSize = 8192;
				this.notifyFilter = NotifyFilters.LastWrite |
									NotifyFilters.FileName |
									NotifyFilters.DirectoryName;
			}
	public FileSystemWatcher(String path)
			{
				if(path == null)
				{
					throw new ArgumentNullException("path");
				}
				else if(path.Length == 0)
				{
					throw new ArgumentException(S._("IO_InvalidPathname"));
				}
				this.filter = "*.*";
				this.internalBufferSize = 8192;
				this.notifyFilter = NotifyFilters.LastWrite |
									NotifyFilters.FileName |
									NotifyFilters.DirectoryName;
			}
	public FileSystemWatcher(String path, String filter)
			{
				if(path == null)
				{
					throw new ArgumentNullException("path");
				}
				else if(path.Length == 0)
				{
					throw new ArgumentException(S._("IO_InvalidPathname"));
				}
				if(filter == null)
				{
					throw new ArgumentNullException("filter");
				}
				this.path = path;
				this.filter = filter;
				this.internalBufferSize = 8192;
				this.notifyFilter = NotifyFilters.LastWrite |
									NotifyFilters.FileName |
									NotifyFilters.DirectoryName;
			}

	// Get or set this object's properties.
	[IODescription("FSW_Enabled")]
	[DefaultValue(false)]
	public bool EnableRaisingEvents
			{
				get
				{
					return enableRaisingEvents;
				}
				set
				{
					enableRaisingEvents = value;
				}
			}
	[IODescription("FSW_Filter")]
	[DefaultValue("*.*")]
	[RecommendedAsConfigurable(true)]
	[TypeConverter
		("System.Diagnostics.Design.StringValueConverter, System.Design")]
	public String Filter
			{
				get
				{
					return filter;
				}
				set
				{
					if(filter == null || filter.Length == 0)
					{
						filter = "*.*";
					}
					else
					{
						filter = value;
					}
				}
			}
	[IODescription("FSW_IncludeSubdirectories")]
	[DefaultValue(false)]
	public bool IncludeSubdirectories
			{
				get
				{
					return includeSubdirectories;
				}
				set
				{
					includeSubdirectories = value;
				}
			}
	[Browsable(false)]
	[DefaultValue(8192)]
	public int InternalBufferSize
			{
				get
				{
					return internalBufferSize;
				}
				set
				{
					internalBufferSize = value;
				}
			}
	[IODescription("FSW_ChangedFilter")]
	public NotifyFilters NotifyFilter
			{
				get
				{
					return notifyFilter;
				}
				set
				{
					notifyFilter = value;
				}
			}
	[IODescription("FSW_Path")]
	[DefaultValue("")]
	[RecommendedAsConfigurable(true)]
	[Editor
		("System.Diagnostics.Design.FSWPathEditor, System.Design",
		 "System.Drawing.Design.UITypeEditor, System.Drawing")]
	[TypeConverter
		("System.Diagnostics.Design.StringValueConverter, System.Design")]
	public String Path
			{
				get
				{
					return path;
				}
				set
				{
					if(value == null || value.Length == 0)
					{
						throw new ArgumentException(S._("IO_InvalidPathname"));
					}
					else
					{
						path = value;
					}
				}
			}
	[Browsable(false)]
	public override ISite Site
			{
				get
				{
					return base.Site;
				}
				set
				{
					base.Site = value;
					if(value != null && DesignMode)
					{
						enableRaisingEvents = true;
					}
				}
			}
	[IODescription("FSW_SynchronizingObject")]
	public ISynchronizeInvoke SynchronizingObject
			{
				get
				{
					return synchronizingObject;
				}
				set
				{
					synchronizingObject = value;
				}
			}

	// Begin initialization of a watcher.
	public void BeginInit() {}

	// End initialization of a watcher.
	public void EndInit() {}

	// Wait for a particular kind of change to occur.
	public WaitForChangedResult WaitForChanged(WatcherChangeTypes changeType)
			{
				return WaitForChanged(changeType, -1);
			}
	public WaitForChangedResult WaitForChanged
				(WatcherChangeTypes changeType, int timeout)
			{
				// Because we aren't actually watching, the wait
				// is equivalent to a simple timeout.
				Thread.Sleep(timeout);
				return new WaitForChangedResult(changeType, true);
			}

	// Events that are emitted for various filesystem operations.
	[IODescription("FSW_Changed")]
	public event FileSystemEventHandler Changed;
	[IODescription("FSW_Created")]
	public event FileSystemEventHandler Created;
	[IODescription("FSW_Deleted")]
	public event FileSystemEventHandler Deleted;
	[Browsable(false)]
	public event ErrorEventHandler Error;
	[IODescription("FSW_Renamed")]
	public event RenamedEventHandler Renamed;

	// Dispose of this object.
	protected override void Dispose(bool disposing)
			{
				// Nothing to do in this implementation.
			}

	// Raise various events.
	protected void OnChanged(FileSystemEventArgs e)
			{
				if(Changed != null)
				{
					Changed(this, e);
				}
			}
	protected void OnCreated(FileSystemEventArgs e)
			{
				if(Created != null)
				{
					Created(this, e);
				}
			}
	protected void OnDeleted(FileSystemEventArgs e)
			{
				if(Deleted != null)
				{
					Deleted(this, e);
				}
			}
	protected void OnError(ErrorEventArgs e)
			{
				if(Error != null)
				{
					Error(this, e);
				}
			}
	protected void OnRenamed(RenamedEventArgs e)
			{
				if(Renamed != null)
				{
					Renamed(this, e);
				}
			}

}; // class FileSystemWatcher

#endif // CONFIG_WIN32_SPECIFICS && CONFIG_COMPONENT_MODEL

}; // namespace System.IO
