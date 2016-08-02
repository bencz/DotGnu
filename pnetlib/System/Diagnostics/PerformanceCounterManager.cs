/*
 * PerformanceCounterManager.cs - Implementation of the
 *			"System.Diagnostics.PerformanceCounterManager" class.
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

namespace System.Diagnostics
{

#if CONFIG_EXTENDED_DIAGNOSTICS

using System.Runtime.InteropServices;

[Guid("82840BE1-D273-11D2-B94A-00600893B17A")]
[ComVisible(true)]
public sealed class PerformanceCounterManager : ICollectData
{
	// Constructor.
	public PerformanceCounterManager() {}

	// Implement the ICollectData interface.
	void ICollectData.CloseData()
			{
				// Not used by this implementation.
			}
	void ICollectData.CollectData(int id, IntPtr valueName, IntPtr data,
					 			  int totalBytes, out IntPtr res)
			{
				// Not used by this implementation.
				res = IntPtr.Zero;
			}

}; // class PerformanceCounterManager

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
