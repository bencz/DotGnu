/*
 * GraphicsContainer.cs - Implementation of the
 *			"System.Drawing.Drawing2D.GraphicsContainer" class.
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

public sealed class GraphicsContainer : MarshalByRefObject
{
	// Internal state.
	private Graphics graphics;
	private GraphicsContainer next;
	private Region clip;
	private CompositingMode compositingMode;
	private CompositingQuality compositingQuality;
	private InterpolationMode interpolationMode;
	private float pageScale;
	private GraphicsUnit pageUnit;
	private PixelOffsetMode pixelOffsetMode;
	private Point renderingOrigin;
	private SmoothingMode smoothingMode;
	private int textContrast;
	private TextRenderingHint textRenderingHint;
	private Matrix transform;

	// Constructor, which saves away all of the important information.
	// We assume that the lock on the "graphics" object is held by the caller.
	internal GraphicsContainer(Graphics graphics)
			{
				// Push this container onto the stack.
				this.graphics = graphics;
				next = graphics.stackTop;
				graphics.stackTop = this;

				// Save the graphics state information.
				clip = graphics.Clip;
				if(clip != null)
				{
					clip = clip.Clone();
				}
				compositingMode = graphics.CompositingMode;
				compositingQuality = graphics.CompositingQuality;
				interpolationMode = graphics.InterpolationMode;
				pageScale = graphics.PageScale;
				pageUnit = graphics.PageUnit;
				pixelOffsetMode = graphics.PixelOffsetMode;
				renderingOrigin = graphics.RenderingOrigin;
				smoothingMode = graphics.SmoothingMode;
				textContrast = graphics.TextContrast;
				textRenderingHint = graphics.TextRenderingHint;
				if (graphics.transform == null)
				{
					transform = null;
				}
				else
				{
					transform = Matrix.Clone(graphics.transform);
				}
			}

	// Restore a graphics object back to the state of this container.
	// We assume that the lock on the "graphics" object is held by the caller.
	internal void Restore(Graphics graphics)
			{
				// Bail out if the container applies to something else.
				if(graphics != this.graphics)
				{
					return;
				}

				// Make sure that we are on the stack, and pop it and all
				// of the items above it.  Bail out if not on the stack.
				GraphicsContainer container = graphics.stackTop;
				while(container != null && container != this)
				{
					container = container.next;
				}
				if(container == null)
				{
					return;
				}
				graphics.stackTop = next;

				// Restore the "graphics" object to its previous state.
				graphics.Clip = clip;
				graphics.CompositingMode = compositingMode;
				graphics.CompositingQuality = compositingQuality;
				graphics.InterpolationMode = interpolationMode;
				graphics.PageScale = pageScale;
				graphics.PageUnit = pageUnit;
				graphics.PixelOffsetMode = pixelOffsetMode;
				graphics.RenderingOrigin = renderingOrigin;
				graphics.SmoothingMode = smoothingMode;
				graphics.TextContrast = textContrast;
				graphics.TextRenderingHint = textRenderingHint;
				graphics.Transform = transform;
			}

}; // class GraphicsContainer

}; // namespace System.Drawing.Drawing2D
