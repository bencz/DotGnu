/*
 * getenv-glue.cs - Implementation of getenv() in C# for pnetC
 *
 * This file is part of the Portable.NET C library.
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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
using System.Runtime.InteropServices;

[GlobalScope]
public class LibCEnviron
{

	// Get the value of the environment variable with the given name
	public static IntPtr __syscall_getenv(IntPtr name)
			{
				String vname, rv;
				vname = Marshal.PtrToStringAnsi(name);
				rv = Environment.GetEnvironmentVariable(vname);
				if (rv == null) return IntPtr.Zero;
				return Marshal.StringToHGlobalAnsi(rv);
			}

} // class LibCEnviron

} // namespace OpenSystem.C
