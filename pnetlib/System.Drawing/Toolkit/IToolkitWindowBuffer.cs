/*
 * IToolkitWindowBuffer.cs - Implementation of the
 *			"System.Drawing.Toolkit.IToolkitWindowBuffer" class.
 *
 * Copyright (C) 2004  Neil Cawse.
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

namespace System.Drawing.Toolkit
{

	// This interface provides the facility to support double buffering when
	// IToolkitWindows are painted.

	[NonStandardExtra]
	public interface IToolkitWindowBuffer : IDisposable
	{
		// Return a buffer toolkit graphics to do the cached drawing.
		IToolkitGraphics BeginDoubleBuffer();
		// Write the cache to the screen and dispose the toolkit graphics.
		void EndDoubleBuffer();
	}
}