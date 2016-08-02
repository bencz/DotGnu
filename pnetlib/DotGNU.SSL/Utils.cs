/*
 * Utils.cs - Implementation of the "DotGNU.SSL.Utils" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * This program is free software, you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY, without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program, if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace DotGNU.SSL
{

using System;
using System.Reflection;

internal sealed class Utils
{
	// This class cannot be instantiated.
	private Utils() {}

	// Get the operating system file descriptor for a "Socket" object.
	// Returns -1 if the socket object doesn't have a valid descriptor.
	internal static int GetSocketFd(Object socket)
			{
				// This is ugly, but is needed until we get recursive
				// assemblies working to break the circular dependency
				// between System.Net and DotGNU.SSL.
				if(socket == null)
				{
					return -1;
				}
			#if CONFIG_REFLECTION
				Object handle = socket.GetType().InvokeMember
						("Handle",
						 BindingFlags.GetProperty |
						 	BindingFlags.Public |
						 	BindingFlags.Instance,
						 null, socket, null, null, null, null);
				if(handle is IntPtr)
				{
					return unchecked((int)(((IntPtr)handle).ToInt64()));
				}
			#endif
				return -1;
			}

	// Helper method for validating buffer arguments.
	internal static void ValidateBuffer(byte[] buffer, int offset, int count)
			{
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				else if(offset < 0 || offset > buffer.Length)
				{
					throw new ArgumentOutOfRangeException();
				}
				else if(count < 0)
				{
					throw new ArgumentOutOfRangeException();
				}
				else if((buffer.Length - offset) < count)
				{
					throw new ArgumentException();
				}
			}

}; // class Utils

}; // namespace DotGNU.SSL
