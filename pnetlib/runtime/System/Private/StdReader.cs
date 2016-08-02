/*
 * StdReader.cs - Implementation of the "System.Private.StdReader" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

namespace System.Private
{

#if !CONFIG_SMALL_CONSOLE

using System;
using System.IO;
using System.Globalization;
using System.Text;
using Platform;

internal sealed class StdReader : TextReader
{
	// Local state.
	private int fd;

	// Constructor.
	public StdReader(int fd) : base()
			{
				this.fd = fd;
			}

	// Destructor.
	~StdReader()
			{
				if(fd != -1)
				{
					Stdio.StdClose(fd);
					fd = -1;
				}
			}

	// Dispose this text reader.
	protected override void Dispose(bool disposing)
			{
				if(fd != -1)
				{
					Stdio.StdClose(fd);
					fd = -1;
				}
			}

	// Peek at the next character.
	public override int Peek()
			{
				if(fd != -1)
				{
					return Stdio.StdPeek(fd);
				}
				else
				{
					return -1;
				}
			}

	// Read the next character.
	public override int Read()
			{
				if(fd != -1)
				{
					return Stdio.StdRead(fd);
				}
				else
				{
					return -1;
				}
			}

	// Read a buffer of characters.
	public override int Read(char[] buffer, int index, int count)
			{
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				if(index < 0)
				{
					throw new ArgumentOutOfRangeException
						("index", _("ArgRange_Array"));
				}
				if(count < 0)
				{
					throw new ArgumentOutOfRangeException
						("count", _("ArgRange_Array"));
				}
				if((buffer.Length - index) < count)
				{
					throw new ArgumentException(_("Arg_InvalidArrayRange"));
				}
				if(fd != -1)
				{
					return Stdio.StdRead(fd, buffer, index, count);
				}
				else
				{
					return 0;
				}
			}

}; // class StdReader

#endif // !CONFIG_SMALL_CONSOLE

}; // namespace System.Private
