/*
 * FileSystemEventArgs.cs - Implementation of the
 *		"System.IO.FileSystemEventArgs" class.
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

#if CONFIG_WIN32_SPECIFICS

public class FileSystemEventArgs : EventArgs
{
	// Internal state.
	private WatcherChangeTypes changeType;
	internal String directory;
	private String name;

	// Constructor.
	public FileSystemEventArgs(WatcherChangeTypes changeType,
							   String directory, String name)
			{
				this.changeType = changeType;
				this.directory = directory;
				this.name = name;
			}

	// Get this object's properties.
	public WatcherChangeTypes ChangeType
			{
				get
				{
					return changeType;
				}
			}
	public String FullPath
			{
				get
				{
					if(directory.EndsWith
						(Path.DirectorySeparatorChar.ToString()) ||
					   directory.EndsWith
						(Path.AltDirectorySeparatorChar.ToString()))
					{
						return directory + name;
					}
					else
					{
						return directory +
							   Path.DirectorySeparatorChar.ToString() +
							   name;
					}
				}
			}
	public String Name
			{
				get
				{
					return name;
				}
			}

}; // class FileSystemEventArgs

#endif // CONFIG_WIN32_SPECIFICS

}; // namespace System.IO
