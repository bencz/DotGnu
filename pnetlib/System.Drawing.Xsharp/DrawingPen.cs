/*
 * DrawingPen.cs - Implementation of pens for System.Drawing.
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
using System.Drawing.Toolkit;
using Xsharp;

internal sealed class DrawingPen : ToolkitPenBase
{
	// Internal state.
	private Pen properties;

	// Constructor.
	public DrawingPen(Pen properties) :
		base(properties.Color, (int)properties.Width)
			{
				this.properties = properties;
			}

	// Map the line style from "System.Drawing" to "Xsharp".
	private static LineStyle MapLineStyle(DashStyle style)
			{
				switch(style)
				{
					case DashStyle.Solid:
					default:
						return LineStyle.LineSolid;
					case DashStyle.Custom:
					case DashStyle.Dash:
					case DashStyle.Dot:
					case DashStyle.DashDot:
					case DashStyle.DashDotDot:
						return LineStyle.LineOnOffDash;
				}
			}

	// Map the cap style from "System.Drawing" to "Xsharp".
	private static CapStyle MapCapStyle(LineCap style)
			{
				switch(style)
				{
					case LineCap.Square:
						return CapStyle.CapProjecting;
					case LineCap.Flat:
						return CapStyle.CapButt;
					case LineCap.Round:
					default:
						return CapStyle.CapRound;
				}
			}

	// Map the join style from "System.Drawing" to "Xsharp".
	private static JoinStyle MapJoinStyle(LineJoin style)
			{
				switch(style)
				{
					case LineJoin.Miter:
					case LineJoin.MiterClipped:
					default:
						return JoinStyle.JoinMiter;
					case LineJoin.Bevel:
						return JoinStyle.JoinBevel;
					case LineJoin.Round:
						return JoinStyle.JoinRound;
				}
			}

	// Standard dash patterns.
	private static readonly byte[] dash = {3, 1};
	private static readonly byte[] dot = {1, 1};
	private static readonly byte[] dashdot = {3, 1, 1, 1};
	private static readonly byte[] dashdotdot = {3, 1, 1, 1, 1, 1};

	// Select this pen into a graphics object.
	public override void Select(IToolkitGraphics _graphics)
			{
				if (_graphics == null)
					return;
				
				if (_graphics is DrawingGraphics)
				{
					DrawingGraphics graphics = _graphics as DrawingGraphics;
					Xsharp.Graphics g = graphics.graphics;
					int width = (int)(properties.Width);
					LineStyle style = MapLineStyle(properties.DashStyle);
					if(style == LineStyle.LineOnOffDash)
					{
						if(width == 1)
						{
							width = 0;
						}
						switch(properties.DashStyle)
						{
							case DashStyle.Dash:
							{
								g.DashPattern = dash;
							}
							break;

							case DashStyle.Dot:
							{
								g.DashPattern = dot;
							}
							break;

							case DashStyle.DashDot:
							{
								g.DashPattern = dashdot;
							}
							break;

							case DashStyle.DashDotDot:
							{
								g.DashPattern = dashdotdot;
							}
							break;
							
							case DashStyle.Custom :
							{
								float [] src = properties.DashPattern;
								int iLen = src.Length;
								byte [] ayCopy = new byte[ iLen ];
								float fWidth = properties.Width;
								float tmp;
								for( int i = 0; i < iLen; i++ ) {
									tmp = src[i]*fWidth;
									     if( tmp < 0    ) tmp = 0;
									else if( tmp > 0xFF ) tmp = 0xFF;
									ayCopy[i] = (byte) ( tmp );
									if( ayCopy[i] == 0 ) ayCopy[i] = 1; // must not be zero
								}
								g.DashPattern = ayCopy; 
							}
							break;
						}
					}
					g.Function = GCFunction.GXcopy;
					g.SubwindowMode = SubwindowMode.ClipByChildren;
					g.LineWidth = width;
					g.LineStyle = style;
					g.CapStyle = MapCapStyle(properties.EndCap);
					g.JoinStyle = MapJoinStyle(properties.LineJoin);
					g.Foreground = DrawingToolkit.DrawingToXColor
						(properties.Color);
					g.SetFillSolid();
					graphics.Pen = this;
				}
				else if (_graphics is DrawingGraphicsImage)
				{
					DrawingGraphicsImage graphics = _graphics as DrawingGraphicsImage;
					graphics.Pen = this;
				}
			}

	// Select a brush-based pen into a graphics object.
	public override void Select(IToolkitGraphics graphics, IToolkitBrush brush)
			{
				// Set the basic line information first.
				Select(graphics);

				// Select the brush details into the graphics context.
				brush.Select(graphics);
			}

	// Dispose of this pen.
	protected override void Dispose(bool disposing)
			{
				// Nothing to do here in this implementation.
			}

}; // class DrawingPen

}; // namespace System.Drawing.Toolkit
