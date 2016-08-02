/*
 * RenamedEventArgs.cs - Implementation of the
 *		"System.IO.RenamedEventArgs" class.
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

public class RenamedEventArgs : FileSystemEventArgs
{
	// Internal state.
	private String oldName;

	// Constructor.
	public RenamedEventArgs(WatcherChangeTypes changeType,
							String directory, String name,
							String oldName)
			: base(changeType, directory, name)
			{
				this.oldName = oldName;
			}

	// Get this object's properties.
	public String OldFullPath
			{
				get
				{
					if(directory.EndsWith
						(Path.DirectorySeparatorChar.ToString()) ||
					   directory.EndsWith
						(Path.AltDirectorySeparatorChar.ToString()))
					{
						return directory + oldName;
					}
					else
					{
						return directory +
							   Path.DirectorySeparatorChar.ToString() +
							   oldName;
					}
				}
			}
	public String OldName
			{
				get
				{
					return oldName;
				}
			}

}; // class RenamedEventArgs

#endif // CONFIG_WIN32_SPECIFICS

}; // namespace System.IO
