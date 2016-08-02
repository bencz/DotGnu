/*
 * IFDOperations.cs - Special operations on file descriptors.
 *
 * This file is part of the Portable.NET "C language support" library.
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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

public interface IFDOperations
{

	// Get or set the non-blocking flag.  Throws "NotSupportedException"
	// if an attempt is made to modify blocking on a descriptor that
	// cannot support such a modification.
	bool NonBlocking { get; set; }

	// Get the native operating system file descriptor.  -1 if unknown.
	int NativeFd { get; }

	// Get the native operating system "select" descriptor.  -1 if unknown.
	int SelectFd { get; }

} // interface IFDOperations

} // namespace OpenSystem.C
