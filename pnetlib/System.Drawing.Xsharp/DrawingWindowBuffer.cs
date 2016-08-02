/*
 * DrawingWindowBuffer.cs - Window Double Buffer class.
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
namespace System.Drawing.Toolkit
{

using System;
using Xsharp;

internal class DrawingWindowBuffer : IToolkitWindowBuffer, IDisposable
{
	// Internal state.
	private IToolkit toolkit;
	private InputOutputWidget widget;
	private DoubleBuffer buffer;
	private DrawingGraphics graphics;

	// Constructor.
	public DrawingWindowBuffer(IToolkitWindow windowToBuffer)
			{
				toolkit = windowToBuffer.Toolkit;
				widget = windowToBuffer as InputOutputWidget;
				buffer = null;
				graphics = null;
			}

	// Create the buffer object for the widget.
	private void CreateBuffer(int width, int height)
			{
				DeleteBuffer();
				buffer = new DoubleBuffer(widget);
			}

	// Delete the buffer object.
	private void DeleteBuffer()
			{
				// Make sure that we dispose of the X graphics object
				// before we dispose of the buffer.
				if(graphics != null)
				{
					graphics.graphics.Dispose();
				}
				if(buffer != null)
				{
					buffer.Dispose();
					buffer = null;
				}
			}

	// Begin a double buffer operation.
	public IToolkitGraphics BeginDoubleBuffer()
			{
				// Create the double buffer if necessary.
				if(buffer == null)
				{
					CreateBuffer(widget.Width, widget.Height);
				}

				// Create a graphics object for the buffer and return it.
				graphics = new DrawingGraphics
					(toolkit, new Xsharp.Graphics(buffer));
				return graphics;
			}

	// End a double buffer operation, flusing the buffer back to the widget.
	public void EndDoubleBuffer()
			{
				if(graphics != null)
				{
					// Dispose of the IToolkitGraphics object and
					// the underlying double-buffered "Xsharp.Graphics"
					// object.  Dispose that will update the screen.
					graphics.Dispose();
					graphics = null;
				}
			}

	// Dispose of this object.
	public void Dispose()
			{
				DeleteBuffer();
			}

}; // class DrawingWindowBuffer

}; // namespace System.Drawing.Toolkit
