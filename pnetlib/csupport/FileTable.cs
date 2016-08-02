/*
 * FileTable.cs - Manage the file descriptor table.
 *
 * This file is part of the Portable.NET "C language support" library.
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace OpenSystem.C
{

using System;
using System.IO;

//
// The file descriptor table maps integer descriptors for C
// into C# stream objects.  File descriptor functions can
// call C# stream methods to perform the required functionality.
//
public sealed class FileTable
{
	// Structure of a stream reference, which also keeps track
	// of the number of times that it has been dup'ed.
	private sealed class StreamRef
	{
		public Stream stream;
		public int count;
		public byte[] buffer;

		public StreamRef(Stream stream)
				{
					this.stream = stream;
					this.count = 1;
					this.buffer = null;
				}

	} // class StreamRef

	// Internal state.
	public const int MaxDescriptors = 256;
	private static StreamRef[] fds = new StreamRef [MaxDescriptors];

	// Set a file descriptor to a specific stream.  The previous
	// association is lost, so it should only be used on a slot
	// that is known to be empty.
	public static void SetFileDescriptor(int fd, Stream stream)
			{
				lock(typeof(FileTable))
				{
					StreamRef[] table = fds;
					StreamRef sref;
					if(fd >= 0 && fd < MaxDescriptors)
					{
						if((sref = table[fd]) == null)
						{
							table[fd] = new StreamRef(stream);
						}
						else
						{
							sref.stream = stream;
							sref.count = 1;
							sref.buffer = null;
						}
					}
				}
			}

	// Get the stream that is associated with a file descriptor.
	// Returns "null" if there is no stream associated.
	public static Stream GetStream(int fd)
			{
				lock(typeof(FileTable))
				{
					if(fd >= 0 && fd < MaxDescriptors)
					{
						StreamRef sref = fds[fd];
						if(sref != null)
						{
							return sref.stream;
						}
						else
						{
							return null;
						}
					}
					else
					{
						return null;
					}
				}
			}

	// Get the stream that is associated with a file descriptor,
	// together with a buffer that can be used for reads and writes.
	// Returns "null" if there is no stream associated.
	public static Stream GetStreamAndBuffer(int fd, out byte[] buffer)
			{
				lock(typeof(FileTable))
				{
					if(fd >= 0 && fd < MaxDescriptors)
					{
						StreamRef sref = fds[fd];
						if(sref != null)
						{
							if(sref.buffer == null)
							{
								sref.buffer = new byte [1024];
							}
							buffer = sref.buffer;
							return sref.stream;
						}
						else
						{
							buffer = null;
							return null;
						}
					}
					else
					{
						buffer = null;
						return null;
					}
				}
			}

	// Find a free file descriptor.  Returns -1 if nothing is free.
	private static int NewFD()
			{
				StreamRef[] table = fds;
				int fd;
				for(fd = 0; fd < MaxDescriptors; ++fd)
				{
					if(table[fd] == null)
					{
						return fd;
					}
				}
				return -1;
			}
	private static int NewFD(int after)
			{
				StreamRef[] table = fds;
				int fd;
				for(fd = after; fd >= 0 && fd < MaxDescriptors; ++fd)
				{
					if(table[fd] == null)
					{
						return fd;
					}
				}
				return -1;
			}

	// Allocate a new file descriptor, with no stream association.
	public static int AllocFD()
			{
				lock(typeof(FileTable))
				{
					int newFD = NewFD();
					if(newFD != -1)
					{
						fds[newFD] = new StreamRef(null);
					}
					return newFD;
				}
			}

	// Release a file descriptor that was allocated with "AllocFD",
	// but is no longer required due to error conditions.
	public static void ReleaseFD(int fd)
			{
				lock(typeof(FileTable))
				{
					if(fd >= 0 && fd < MaxDescriptors)
					{
						fds[fd] = null;
					}
				}
			}

	// Duplicate a file descriptor.  Returns -1 if there are
	// no free descriptors, or -2 if the descriptor is invalid.
	public static int Dup(int fd)
			{
				lock(typeof(FileTable))
				{
					if(fd < 0 || fd >= MaxDescriptors || fds[fd] == null)
					{
						return -2;
					}
					int newFd = NewFD();
					if(newFd == -1)
					{
						return -1;
					}
					StreamRef sref = fds[fd];
					++(sref.count);
					fds[newFd] = sref;
					return newFd;
				}
			}

	// Duplicate a file descriptor, starting at a particular position.
	public static int DupAfter(int fd, int after)
			{
				lock(typeof(FileTable))
				{
					if(fd < 0 || fd >= MaxDescriptors || fds[fd] == null)
					{
						return -2;
					}
					int newFd = NewFD(after);
					if(newFd == -1)
					{
						return -1;
					}
					StreamRef sref = fds[fd];
					++(sref.count);
					fds[newFd] = sref;
					return newFd;
				}
			}

	// Duplicate a file descriptor and replace another one.
	// Returns -1 if either of the descriptors are invalid.
	public static int Dup2(int oldfd, int newfd)
			{
				Stream streamToClose = null;
				lock(typeof(FileTable))
				{
					StreamRef[] table = fds;
					if(oldfd < 0 || oldfd >= MaxDescriptors ||
					   table[oldfd] == null)
					{
						return -1;
					}
					if(newfd < 0 || newfd >= MaxDescriptors)
					{
						return -1;
					}
					StreamRef sref = table[oldfd];
					if(sref == table[newfd])
					{
						// The new stream is already the same as the old.
						return newfd;
					}
					if(table[newfd] != null)
					{
						if(--(table[newfd].count) == 0)
						{
							streamToClose = table[newfd].stream;
						}
					}
					table[newfd] = sref;
					++(sref.count);
				}
				if(streamToClose != null)
				{
					streamToClose.Close();
				}
				return newfd;
			}

	// Close a file descriptor.  Returns 0 if OK, or -1 if
	// the descriptor is invalid.
	public static int Close(int fd)
			{
				Stream streamToClose = null;
				lock(typeof(FileTable))
				{
					StreamRef[] table = fds;
					if(fd < 0 || fd >= MaxDescriptors || table[fd] == null)
					{
						return -1;
					}
					if(--(table[fd].count) == 0)
					{
						streamToClose = table[fd].stream;
					}
					table[fd] = null;
				}
				if(streamToClose != null)
				{
					streamToClose.Close();
				}
				return 0;
			}

} // class FileTable

} // namespace OpenSystem.C
