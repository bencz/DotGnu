/*
 * SafeHandleMinusOneIsInvalid.cs - Implementation of the
 *	"Microsoft.Win32.SafeHandles.SafeHandleMinusOneIsInvalid" class.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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

namespace Microsoft.Win32.SafeHandles
{

#if CONFIG_WIN32_SPECIFICS
#if !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

using System;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;

public abstract class SafeHandleMinusOneIsInvalid : SafeHandle
{
	// Constructor.
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
	protected SafeHandleMinusOneIsInvalid(bool ownsHandle)
			: base(new IntPtr(-1L), ownsHandle) {}

	// Determine if the handle is invalid.
	public override bool IsInvalid
			{
				get
				{
					return (handle == new IntPtr(-1L));
				}
			}

}; // class SafeHandleMinusOneIsInvalid

#endif // !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK
#endif // CONFIG_WIN32_SPECIFICS

}; // namespace Microsoft.Win32.SafeHandles
