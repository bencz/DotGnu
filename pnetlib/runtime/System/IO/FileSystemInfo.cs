/*
 * FileSystemInfo.cs - Implementation of the "System.IO.FileSystemInfo" class.
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

#if !ECMA_COMPAT

using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

[Serializable]
public abstract class FileSystemInfo : MarshalByRefObject
#if CONFIG_SERIALIZATION
	, ISerializable
#endif
{
	// Internal state.
	protected String FullPath;
	protected String OriginalPath;

	// Constructor.
	protected FileSystemInfo()
			{
				FullPath = null;
				OriginalPath = null;
			}
#if CONFIG_SERIALIZATION
	protected FileSystemInfo(SerializationInfo info, StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				OriginalPath = info.GetString("OriginalPath");
				FullPath = info.GetString("FullPath");
			}
#endif

	// Properties.
	public FileAttributes Attributes
			{
				get
				{
					return File.GetAttributes(FullPath);
				}
				set
				{
					File.SetAttributes(FullPath, value);
				}
			}
	public DateTime CreationTime
			{
				get
				{
					return File.GetCreationTime(FullPath);
				}
				set
				{
					File.SetCreationTime(FullPath, value);
				}
			}
	[ComVisible(false)]
	public DateTime CreationTimeUtc
			{
				get
				{
					return File.GetCreationTimeUtc(FullPath);
				}
				set
				{
					File.SetCreationTimeUtc(FullPath, value);
				}
			}
	public abstract bool Exists { get; }
	public String Extension
			{
				get
				{
					return Path.GetExtension(Name);
				}
			}
	public virtual String FullName
			{
				get
				{
					return FullPath;
				}
			}
	public DateTime LastAccessTime
			{
				get
				{
					return File.GetLastAccessTime(FullPath);
				}
				set
				{
					File.SetLastAccessTime(FullPath, value);
				}
			}
	[ComVisible(false)]
	public DateTime LastAccessTimeUtc
			{
				get
				{
					return File.GetLastAccessTimeUtc(FullPath);
				}
				set
				{
					File.SetLastAccessTimeUtc(FullPath, value);
				}
			}
	public DateTime LastWriteTime
			{
				get
				{
					return File.GetLastWriteTime(FullPath);
				}
				set
				{
					File.SetLastWriteTime(FullPath, value);
				}
			}
	[ComVisible(false)]
	public DateTime LastWriteTimeUtc
			{
				get
				{
					return File.GetLastWriteTimeUtc(FullPath);
				}
				set
				{
					File.SetLastWriteTimeUtc(FullPath, value);
				}
			}
	public abstract String Name { get; }

	// Delete the file represented by this object.
	public abstract void Delete();

	// Refresh the cache state information.
	public void Refresh()
			{
				// Nothing to do here - we don't cache values.
			}

#if CONFIG_SERIALIZATION

	// Get the serialization data for this instance.
	[ComVisible(false)]
	public virtual void GetObjectData(SerializationInfo info,
									  StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				info.AddValue("OriginalPath", OriginalPath);
				info.AddValue("FullPath", FullPath);
			}

#endif

}; // class FileSystemInfo

#endif // !ECMA_COMPAT

}; // namespace System.IO
