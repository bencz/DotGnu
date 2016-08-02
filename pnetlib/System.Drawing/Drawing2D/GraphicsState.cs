/*
 * GraphicsState.cs - Implementation of the
 *			"System.Drawing.Drawing2D.GraphicsState" class.
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

namespace System.Drawing.Drawing2D
{

using System.Drawing.Text;

public sealed class GraphicsState : MarshalByRefObject
{
	// Internal state.
	private GraphicsContainer container;

	// Constructor.  Call this with the context lock held.
	internal GraphicsState(Graphics graphics)
			{
				container = new GraphicsContainer(graphics);
			}

	// Restore the previous settings of this state.  Call with context lock.
	internal void Restore(Graphics graphics)
			{
				container.Restore(graphics);
			}

}; // class GraphicsState

}; // namespace System.Drawing.Drawing2D
