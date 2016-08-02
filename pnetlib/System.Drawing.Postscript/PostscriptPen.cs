/*
 * PostscriptPen.cs - Implementation of pens for System.Drawing.
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
using System.Drawing;
using System.Drawing.Drawing2D;

internal sealed class PostscriptPen : IToolkitPen
{
	// Internal state.
	private Pen properties;
	private PostscriptGraphics selectedInto;

	// Constructor.
	public PostscriptPen(Pen properties)
			{
				this.properties = properties;
				this.selectedInto = null;
			}

	// Select this pen into a graphics object.
	public void Select(IToolkitGraphics _graphics)
			{
				PostscriptGraphics graphics = (_graphics as PostscriptGraphics);
				if(graphics != null)
				{
					if(selectedInto == graphics &&
					   graphics.selectObject == this)
					{
						// Same pen as last time, so don't output a
						// redundant version of the Postscript definitions.
						return;
					}
					if(selectedInto != null)
					{
						selectedInto.selectObject = null;
						selectedInto = null;
					}
					graphics.writer.WriteLine("grestore gsave");
					switch(properties.PenType)
					{
						case PenType.SolidColor:
						{
							Color color = properties.Color;
						#if CONFIG_EXTENDED_NUMERICS
							graphics.writer.WriteLine
								("{0} {1} {2} setrgbcolor",
								 ((double)(color.R)) / 255.0,
								 ((double)(color.G)) / 255.0,
								 ((double)(color.B)) / 255.0);
						#endif
						}
						break;

						// TODO: other pen types
					}
				#if CONFIG_EXTENDED_NUMERICS
					graphics.writer.WriteLine("{0} setlinewidth",
											  properties.Width);
				#else
					graphics.writer.WriteLine("{0} setlinewidth",
											  (int)(properties.Width));
				#endif
					// TODO: caps, joins, miters, etc
					selectedInto = graphics;
					graphics.selectObject = this;
				}
			}

	// Select a brush-based pen into a graphics object.
	public void Select(IToolkitGraphics graphics, IToolkitBrush brush)
			{
				Select(graphics);
				// TODO: select the brush information
			}

	// Dispose of this pen.
	public void Dispose()
			{
				// Nothing to do here in this implementation.
			}

}; // class PostscriptPen

}; // namespace System.Drawing.Toolkit
