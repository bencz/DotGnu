/*
 * DefaultThemePainter.cs - Implementation of the
 *			"System.Windows.Forms.Themes.DefaultThemePainter" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2003  Free Software Foundation, Inc.
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

namespace System.Windows.Forms.Themes
{

using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Toolkit;
using System.Windows.Forms;

// This class implements the default theme functionality for the
// System.Windows.Forms controls.  Other themes can inherit from
// this to provide new themes, while deferring most things to the
// base class to be handled.

public class DefaultThemePainter : IThemePainter
{

	// Cached values
	protected bool gridDark = true;
	protected Size gridSpacing = Size.Empty;
	protected Brush gridBrush = null;

	// Draw a simple bitmap-based glyph within a rectangle.
	private static void DrawGlyph
				(Graphics graphics, int x, int y, int width, int height,
				 byte[] bits, int bitsWidth, int bitsHeight, Color color)
			{
				x += (width - bitsWidth) / 2;
				y += (height - bitsHeight) / 2;
				ToolkitManager.DrawGlyph
					(graphics, x, y, bits, bitsWidth, bitsHeight, color);
			}

	// Get the width of the specified border type
	public virtual int GetBorderWidth(BorderStyle border)
			{
				switch(border)
				{
					case BorderStyle.Fixed3D:
						return 2;
					case BorderStyle.FixedSingle:
						return 1;
					case BorderStyle.None:
						return 0;
					default:
						return 0;
				}
			}

	// Draw a simple button border.
	public virtual void DrawBorder
				(Graphics graphics, Rectangle bounds,
				 Color color, ButtonBorderStyle style)
			{
				Pen pen;
				switch(style)
				{
					case ButtonBorderStyle.Dotted:
					{
						pen = new Pen(color, 1.0f);
						pen.EndCap = LineCap.Square;
						pen.DashStyle = DashStyle.Dot;
						graphics.DrawRectangle(pen, bounds);
						pen.Dispose();
					}
					break;

					case ButtonBorderStyle.Dashed:
					{
						pen = new Pen(color, 1.0f);
						pen.EndCap = LineCap.Square;
						pen.DashStyle = DashStyle.Dash;
						graphics.DrawRectangle(pen, bounds);
						pen.Dispose();
					}
					break;

					case ButtonBorderStyle.Solid:
					{
						pen = new Pen(color, 1.0f);
						pen.EndCap = LineCap.Square;
						Rectangle r = new Rectangle(bounds.X,
							bounds.Y, bounds.Width - 1, bounds.Height - 1);
						graphics.DrawRectangle(pen, r);
						pen.Color = ControlPaint.LightLight(color);
						graphics.DrawLine(pen, bounds.X + 1,
							bounds.Y + bounds.Height - 1,
							bounds.X + bounds.Width - 1,
							bounds.Y + bounds.Height - 1);
						graphics.DrawLine(pen, bounds.X + bounds.Width - 1,
							bounds.Y + bounds.Height - 2,
							bounds.X + bounds.Width - 1,
							bounds.Y + 1);
						pen.Dispose();
					}
					break;

					case ButtonBorderStyle.Inset:
					{
						pen = new Pen(ControlPaint.DarkDark(color), 1.0f);
						pen.EndCap = LineCap.Square;
						graphics.DrawLine(pen, bounds.X,
										  bounds.Y + bounds.Height - 1,
										  bounds.X, bounds.Y);
						graphics.DrawLine(pen, bounds.X + 1, bounds.Y,
										  bounds.X + bounds.Width - 1,
										  bounds.Y);
						pen.Color = ControlPaint.LightLight(color);
						graphics.DrawLine(pen, bounds.X + 1,
										  bounds.Y + bounds.Height - 1,
										  bounds.X + bounds.Width - 1,
										  bounds.Y + bounds.Height - 1);
						graphics.DrawLine(pen, bounds.X + bounds.Width - 1,
										  bounds.Y + bounds.Height - 2,
										  bounds.X + bounds.Width - 1,
										  bounds.Y + 1);
						pen.Color = ControlPaint.Light(color);
						graphics.DrawLine(pen, bounds.X + 1,
										  bounds.Y + bounds.Height - 2,
										  bounds.X + 1, bounds.Y + 1);
						graphics.DrawLine(pen, bounds.X + 2, bounds.Y + 1,
										  bounds.X + bounds.Width - 2,
										  bounds.Y + 1);
						if(color.ToKnownColor() == KnownColor.Control)
						{
							pen.Color = SystemColors.ControlLight;
							graphics.DrawLine(pen, bounds.X + 1,
											  bounds.Y + bounds.Height - 2,
											  bounds.X + bounds.Width - 2,
											  bounds.Y + bounds.Height - 2);
							graphics.DrawLine(pen, bounds.X + bounds.Width - 2,
											  bounds.Y + bounds.Height - 3,
											  bounds.X + bounds.Width - 2,
											  bounds.Y + 1);
						}
						pen.Dispose();
					}
					break;

					case ButtonBorderStyle.Outset:
					{
						pen = new Pen(ControlPaint.LightLight(color), 1.0f);
						pen.EndCap = LineCap.Square;
						graphics.DrawLine(pen, bounds.X,
										  bounds.Y + bounds.Height - 2,
										  bounds.X, bounds.Y);
						graphics.DrawLine(pen, bounds.X + 1, bounds.Y,
										  bounds.X + bounds.Width - 2,
										  bounds.Y);
						pen.Color = ControlPaint.DarkDark(color);
						graphics.DrawLine(pen, bounds.X,
										  bounds.Y + bounds.Height - 1,
										  bounds.X + bounds.Width - 1,
										  bounds.Y + bounds.Height - 1);
						graphics.DrawLine(pen, bounds.X + bounds.Width - 1,
										  bounds.Y + bounds.Height - 2,
										  bounds.X + bounds.Width - 1,
										  bounds.Y);
						pen.Color = ControlPaint.Light(color);
						graphics.DrawLine(pen, bounds.X + 1,
										  bounds.Y + bounds.Height - 3,
										  bounds.X + 1, bounds.Y + 1);
						graphics.DrawLine(pen, bounds.X + 1, bounds.Y + 1,
										  bounds.X + bounds.Width - 3,
										  bounds.Y + 1);
						pen.Color = ControlPaint.Dark(color);
						graphics.DrawLine(pen, bounds.X + 1,
										  bounds.Y + bounds.Height - 2,
										  bounds.X + bounds.Width - 2,
										  bounds.Y + bounds.Height - 2);
						graphics.DrawLine(pen, bounds.X + bounds.Width - 2,
										  bounds.Y + bounds.Height - 3,
										  bounds.X + bounds.Width - 2,
										  bounds.Y + 1);
						pen.Dispose();
					}
					break;
				}
			}
	public virtual void DrawBorder
				(Graphics graphics, Rectangle bounds, Color leftColor,
			     int leftWidth, ButtonBorderStyle leftStyle, Color topColor,
				 int topWidth, ButtonBorderStyle topStyle, Color rightColor,
				 int rightWidth, ButtonBorderStyle rightStyle,
				 Color bottomColor, int bottomWidth,
				 ButtonBorderStyle bottomStyle)
			{
				Pen pen;
				float percent, change;

				// Paint the left side of the border.
				if(leftWidth > 0)
				{
					switch(leftStyle)
					{
						case ButtonBorderStyle.Dotted:
						{
							pen = new Pen(leftColor, 1.0f);
							pen.EndCap = LineCap.Square;
							pen.DashStyle = DashStyle.Dot;
							while(leftWidth > 0)
							{
								--leftWidth;
								graphics.DrawLine
									(pen, bounds.X + leftWidth,
									 bounds.Y + leftWidth + 1,
									 bounds.X + leftWidth,
									 bounds.Y + bounds.Height - leftWidth - 2);
							}
							pen.Dispose();
						}
						break;

						case ButtonBorderStyle.Dashed:
						{
							pen = new Pen(leftColor, 1.0f);
							pen.EndCap = LineCap.Square;
							pen.DashStyle = DashStyle.Dash;
							while(leftWidth > 0)
							{
								--leftWidth;
								graphics.DrawLine
									(pen, bounds.X + leftWidth,
									 bounds.Y + leftWidth + 1,
									 bounds.X + leftWidth,
									 bounds.Y + bounds.Height - leftWidth - 2);
							}
							pen.Dispose();
						}
						break;

						case ButtonBorderStyle.Solid:
						{
							pen = new Pen(leftColor, 1.0f);
							pen.EndCap = LineCap.Square;
							while(leftWidth > 0)
							{
								--leftWidth;
								graphics.DrawLine
									(pen, bounds.X + leftWidth,
									 bounds.Y + leftWidth + 1,
									 bounds.X + leftWidth,
									 bounds.Y + bounds.Height - leftWidth - 2);
							}
							pen.Dispose();
						}
						break;

						case ButtonBorderStyle.Inset:
						{
							change = (1.0f / leftWidth);
							percent = 1.0f;
							while(leftWidth > 0)
							{
								--leftWidth;
								pen = new Pen(ControlPaint.Dark
												(leftColor, percent), 1.0f);
								pen.EndCap = LineCap.Square;
								graphics.DrawLine
									(pen, bounds.X + leftWidth,
									 bounds.Y + leftWidth + 1,
									 bounds.X + leftWidth,
									 bounds.Y + bounds.Height - leftWidth - 2);
								pen.Dispose();
								percent -= change;
							}
						}
						break;

						case ButtonBorderStyle.Outset:
						{
							change = (1.0f / leftWidth);
							percent = change;
							while(leftWidth > 0)
							{
								--leftWidth;
								pen = new Pen(ControlPaint.Light
												(leftColor, percent), 1.0f);
								pen.EndCap = LineCap.Square;
								graphics.DrawLine
									(pen, bounds.X + leftWidth,
									 bounds.Y + leftWidth + 1,
									 bounds.X + leftWidth,
									 bounds.Y + bounds.Height - leftWidth - 2);
								pen.Dispose();
								percent += change;
							}
						}
						break;
					}
				}

				// Paint the top side of the border.
				if(topWidth > 0)
				{
					switch(topStyle)
					{
						case ButtonBorderStyle.Dotted:
						{
							pen = new Pen(topColor, 1.0f);
							pen.EndCap = LineCap.Square;
							pen.DashStyle = DashStyle.Dot;
							while(topWidth > 0)
							{
								--topWidth;
								graphics.DrawLine
									(pen, bounds.X + topWidth,
									 bounds.Y + topWidth,
									 bounds.X + bounds.Width - 1 - topWidth,
									 bounds.Y + topWidth);
							}
							pen.Dispose();
						}
						break;

						case ButtonBorderStyle.Dashed:
						{
							pen = new Pen(topColor, 1.0f);
							pen.EndCap = LineCap.Square;
							pen.DashStyle = DashStyle.Dash;
							while(topWidth > 0)
							{
								--topWidth;
								graphics.DrawLine
									(pen, bounds.X + topWidth,
									 bounds.Y + topWidth,
									 bounds.X + bounds.Width - 1 - topWidth,
									 bounds.Y + topWidth);
							}
							pen.Dispose();
						}
						break;

						case ButtonBorderStyle.Solid:
						{
							pen = new Pen(topColor, 1.0f);
							pen.EndCap = LineCap.Square;
							while(topWidth > 0)
							{
								--topWidth;
								graphics.DrawLine
									(pen, bounds.X + topWidth,
									 bounds.Y + topWidth,
									 bounds.X + bounds.Width - 1 - topWidth,
									 bounds.Y + topWidth);
							}
							pen.Dispose();
						}
						break;

						case ButtonBorderStyle.Inset:
						{
							change = (1.0f / topWidth);
							percent = 1.0f;
							while(topWidth > 0)
							{
								--topWidth;
								pen = new Pen(ControlPaint.Dark
												(leftColor, percent), 1.0f);
								pen.EndCap = LineCap.Square;
								graphics.DrawLine
									(pen, bounds.X + topWidth,
									 bounds.Y + topWidth,
									 bounds.X + bounds.Width - 1 - topWidth,
									 bounds.Y + topWidth);
								pen.Dispose();
								percent -= change;
							}
						}
						break;

						case ButtonBorderStyle.Outset:
						{
							change = (1.0f / topWidth);
							percent = change;
							while(topWidth > 0)
							{
								--topWidth;
								pen = new Pen(ControlPaint.Light
												(leftColor, percent), 1.0f);
								pen.EndCap = LineCap.Square;
								graphics.DrawLine
									(pen, bounds.X + topWidth,
									 bounds.Y + topWidth,
									 bounds.X + bounds.Width - 1 - topWidth,
									 bounds.Y + topWidth);
								pen.Dispose();
								percent += change;
							}
						}
						break;
					}
				}

				// Paint the right side of the border.
				if(rightWidth > 0)
				{
					switch(rightStyle)
					{
						case ButtonBorderStyle.Dotted:
						{
							pen = new Pen(rightColor, 1.0f);
							pen.EndCap = LineCap.Square;
							pen.DashStyle = DashStyle.Dot;
							while(rightWidth > 0)
							{
								--rightWidth;
								graphics.DrawLine
									(pen,
									 bounds.X + bounds.Width - 1 - rightWidth,
									 bounds.Y + rightWidth,
									 bounds.X + bounds.Width - 1 - rightWidth,
									 bounds.Y + bounds.Height - 1 - rightWidth);
							}
							pen.Dispose();
						}
						break;

						case ButtonBorderStyle.Dashed:
						{
							pen = new Pen(rightColor, 1.0f);
							pen.EndCap = LineCap.Square;
							pen.DashStyle = DashStyle.Dash;
							while(rightWidth > 0)
							{
								--rightWidth;
								graphics.DrawLine
									(pen,
									 bounds.X + bounds.Width - 1 - rightWidth,
									 bounds.Y + rightWidth,
									 bounds.X + bounds.Width - 1 - rightWidth,
									 bounds.Y + bounds.Height - 1 - rightWidth);
							}
							pen.Dispose();
						}
						break;

						case ButtonBorderStyle.Solid:
						{
							pen = new Pen(rightColor, 1.0f);
							pen.EndCap = LineCap.Square;
							while(rightWidth > 0)
							{
								--rightWidth;
								graphics.DrawLine
									(pen,
									 bounds.X + bounds.Width - 1 - rightWidth,
									 bounds.Y + rightWidth,
									 bounds.X + bounds.Width - 1 - rightWidth,
									 bounds.Y + bounds.Height - 1 - rightWidth);
							}
							pen.Dispose();
						}
						break;

						case ButtonBorderStyle.Inset:
						{
							change = (1.0f / rightWidth);
							percent = 1.0f;
							while(rightWidth > 0)
							{
								--rightWidth;
								pen = new Pen(ControlPaint.Dark
												(leftColor, percent), 1.0f);
								pen.EndCap = LineCap.Square;
								graphics.DrawLine
									(pen,
									 bounds.X + bounds.Width - 1 - rightWidth,
									 bounds.Y + rightWidth,
									 bounds.X + bounds.Width - 1 - rightWidth,
									 bounds.Y + bounds.Height - 1 - rightWidth);
								pen.Dispose();
								percent -= change;
							}
						}
						break;

						case ButtonBorderStyle.Outset:
						{
							change = (1.0f / rightWidth);
							percent = change;
							while(rightWidth > 0)
							{
								--rightWidth;
								pen = new Pen(ControlPaint.Light
												(leftColor, percent), 1.0f);
								pen.EndCap = LineCap.Square;
								graphics.DrawLine
									(pen,
									 bounds.X + bounds.Width - 1 - rightWidth,
									 bounds.Y + rightWidth,
									 bounds.X + bounds.Width - 1 - rightWidth,
									 bounds.Y + bounds.Height - 1 - rightWidth);
								pen.Dispose();
								percent += change;
							}
						}
						break;
					}
				}

				// Paint the bottom side of the border.
				if(bottomWidth > 0)
				{
					switch(bottomStyle)
					{
						case ButtonBorderStyle.Dotted:
						{
							pen = new Pen(bottomColor, 1.0f);
							pen.EndCap = LineCap.Square;
							pen.DashStyle = DashStyle.Dot;
							while(bottomWidth > 0)
							{
								--bottomWidth;
								graphics.DrawLine
									(pen,
									 bounds.X + bottomWidth,
									 bounds.Y + bounds.Height - 1 - bottomWidth,
									 bounds.X + bounds.Width - 2 - bottomWidth,
									 bounds.Y +
									 	bounds.Height - 1 - bottomWidth);
							}
							pen.Dispose();
						}
						break;

						case ButtonBorderStyle.Dashed:
						{
							pen = new Pen(bottomColor, 1.0f);
							pen.EndCap = LineCap.Square;
							pen.DashStyle = DashStyle.Dash;
							while(bottomWidth > 0)
							{
								--bottomWidth;
								graphics.DrawLine
									(pen,
									 bounds.X + bottomWidth,
									 bounds.Y + bounds.Height - 1 - bottomWidth,
									 bounds.X + bounds.Width - 2 - bottomWidth,
									 bounds.Y +
									 	bounds.Height - 1 - bottomWidth);
							}
							pen.Dispose();
						}
						break;

						case ButtonBorderStyle.Solid:
						{
							pen = new Pen(bottomColor, 1.0f);
							pen.EndCap = LineCap.Square;
							while(bottomWidth > 0)
							{
								--bottomWidth;
								graphics.DrawLine
									(pen,
									 bounds.X + bottomWidth,
									 bounds.Y + bounds.Height - 1 - bottomWidth,
									 bounds.X + bounds.Width - 2 - bottomWidth,
									 bounds.Y +
									 	bounds.Height - 1 - bottomWidth);
							}
							pen.Dispose();
						}
						break;

						case ButtonBorderStyle.Inset:
						{
							change = (1.0f / bottomWidth);
							percent = 1.0f;
							while(bottomWidth > 0)
							{
								--bottomWidth;
								pen = new Pen(ControlPaint.Dark
												(leftColor, percent), 1.0f);
								pen.EndCap = LineCap.Square;
								graphics.DrawLine
									(pen,
									 bounds.X + bottomWidth,
									 bounds.Y + bounds.Height - 1 - bottomWidth,
									 bounds.X + bounds.Width - 2 - bottomWidth,
									 bounds.Y +
									 	bounds.Height - 1 - bottomWidth);
								pen.Dispose();
								percent -= change;
							}
						}
						break;

						case ButtonBorderStyle.Outset:
						{
							change = (1.0f / bottomWidth);
							percent = change;
							while(bottomWidth > 0)
							{
								--bottomWidth;
								pen = new Pen(ControlPaint.Light
												(leftColor, percent), 1.0f);
								pen.EndCap = LineCap.Square;
								graphics.DrawLine
									(pen,
									 bounds.X + bottomWidth,
									 bounds.Y + bounds.Height - 1 - bottomWidth,
									 bounds.X + bounds.Width - 2 - bottomWidth,
									 bounds.Y +
									 	bounds.Height - 1 - bottomWidth);
								pen.Dispose();
								percent += change;
							}
						}
						break;
					}
				}
			}

	// Draw a 3D border within a rectangle.
	public virtual void DrawBorder3D(Graphics graphics, int x, int y,
									 int width, int height,
									 Color foreColor,
									 Color backColor,
									 Border3DStyle style,
									 Border3DSide sides)
			{
				Color dark, light, darkdark, lightlight;
				Color top1, top2 , left1, left2;
				Color bottom1, bottom2, right1, right2;
				Pen pen;
				bool doubleBorder=false;
				top2 = left2 = bottom2 = right2 = Color.Black;

				dark=ControlPaint.Dark(backColor);
				light=ControlPaint.Light(backColor);
				darkdark=ControlPaint.DarkDark(backColor);
				lightlight=ControlPaint.LightLight(backColor);
								
				switch(style)
				{
					case Border3DStyle.Flat:
						top1=dark;
						left1=dark;
						right1=dark;
						bottom1=dark;
						doubleBorder=false;
						break;
					case Border3DStyle.Raised:
						top1=light;
						top2=lightlight;
						left1=light;
						left2=lightlight;
						bottom1=dark;
						bottom2=darkdark;
						right1=dark;
						right2=darkdark;
						doubleBorder=true;
						break;
					case Border3DStyle.RaisedInner:
						top1=lightlight;
						left1=lightlight;
						bottom1=dark;
						right1=dark;
						doubleBorder=false;
						break;
					case Border3DStyle.RaisedOuter:
						top1=light;
						left1=light;
						bottom1=darkdark;
						right1=darkdark;
						doubleBorder=false;
						break;
					case Border3DStyle.Etched:
						top1=dark;
						top2=lightlight;
						left1=dark;
						left2=lightlight;
						bottom1=dark;
						bottom2=lightlight;
						right1=dark;
						right2=lightlight;
						doubleBorder=true;
						break;
					case Border3DStyle.Sunken:
						top1=dark;
						top2=darkdark;
						left1=dark;
						left2=darkdark;
						right1=light;
						right2=lightlight;
						bottom1=light;
						bottom2=lightlight;
						doubleBorder=true;
						break;
					case Border3DStyle.SunkenOuter:
						top1=dark;
						left1=dark;
						bottom1=lightlight;
						right1=lightlight;
						doubleBorder=false;
						break;
					case Border3DStyle.SunkenInner:
						top1=dark;
						left1=dark;
						bottom1=light;
						right1=light;
						doubleBorder=false;
						break;					
					case Border3DStyle.Bump:
						top1=light;
						top2=darkdark;
						left1=light;
						left2=darkdark;
						right1=light;
						right2=darkdark;
						bottom1=light;
						bottom2=darkdark;
						doubleBorder=true;
						break;
					default:
						top1=foreColor;
						bottom1=foreColor;
						left1=foreColor;
						right1=foreColor;
						doubleBorder=false;
						break;
				}

				if(doubleBorder && (width >= 4 && height >= 4))
				{
					pen = new Pen(left1, 1.0f);
					pen.EndCap = LineCap.Square;
					if((sides & Border3DSide.Left )!=0)
					{
						graphics.DrawLine(pen, x, y + height - 2, x, y);
						pen.Color = left2;
						graphics.DrawLine(pen, x + 1, y + height - 3,
									  x + 1, y + 1);
					}
					if((sides & Border3DSide.Right )!=0)
					{
						pen.Color = right1;
						graphics.DrawLine(pen, x + width - 2, y + 1,
									  x + width - 2, y + height - 2);
						pen.Color = right2;
						graphics.DrawLine(pen, x + width - 1, y,
									  x + width - 1, y + height - 1);
					}
					if((sides & Border3DSide.Top)!=0)
					{
						pen.Color = top1;
						graphics.DrawLine(pen, x + 1, y, x + width - 2, y);
						pen.Color = top2;
						graphics.DrawLine(pen, x + 2, y + 1,
									  x + width - 3, y + 1);
					}
					if((sides & Border3DSide.Bottom)!=0)
					{
						pen.Color = bottom1;
						graphics.DrawLine(pen, x + width - 3, y + height - 2,
									  x + 1, y + height - 2);
						pen.Color = bottom2;
						graphics.DrawLine(pen, x + width - 2, y + height - 1,
									  x, y + height - 1);
					}
					pen.Dispose();
					x += 2;
					y += 2;
					width -= 4;
					height -= 4;
				}
				else if(!doubleBorder && width >= 2 && height >= 2)
				{
					pen = new Pen(left1, 1.0f);
					pen.EndCap = LineCap.Square;
					if((sides & Border3DSide.Left )!=0)
					{
						graphics.DrawLine(pen, x, y + height - 2, x, y);
					}
					if((sides & Border3DSide.Right )!=0)
					{
						pen.Color = right1;
						graphics.DrawLine(pen, x + width - 1, y,
									  x + width - 1, y + height - 1);
					}
					if((sides & Border3DSide.Top)!=0)
					{
						pen.Color = top1;
						graphics.DrawLine(pen, x + 1, y, x + width - 2, y);
					}
					if((sides & Border3DSide.Bottom)!=0)
					{
						pen.Color = bottom1;
						graphics.DrawLine(pen, x + width - 2, y + height - 1,
									  x, y + height - 1);
					}
					pen.Dispose();
					x+=1;
					y+=1;
					width -=2;
					height -=2;
				}
			}

	// Draw a button control.
	public virtual void DrawButton
				(Graphics graphics, int x, int y, int width, int height,
				 ButtonState state, Color foreColor, Color backColor,
				 bool isDefault)
			{
				Color light, lightlight, dark, darkdark;
				Pen pen;
				Brush brush;

				// Draw the border around the edges of the button.
				if((state & ButtonState.Flat) == 0)
				{
					if((state & ButtonState.Pushed) != 0)
					{
						if(isDefault)
						{
							lightlight = ControlPaint.Dark(backColor);
							darkdark = lightlight;
							light = backColor;
							dark = backColor;
						}
						else
						{
							lightlight = ControlPaint.DarkDark(backColor);
							darkdark = ControlPaint.LightLight(backColor);
							light = ControlPaint.Dark(backColor);
							dark = ControlPaint.Light(backColor);
						}
					}
					else
					{
						lightlight = ControlPaint.LightLight(backColor);
						darkdark = ControlPaint.DarkDark(backColor);
						light = ControlPaint.Light(backColor);
						dark = ControlPaint.Dark(backColor);
					}
					if(isDefault && width >= 2 && height >= 2)
					{
						pen = new Pen(foreColor, 1.0f);
						pen.EndCap = LineCap.Square;
						graphics.DrawRectangle(pen, x, y, width, height);
						pen.Dispose();
						++x;
						++y;
						width -= 2;
						height -= 2;
					}
					if(width >= 4 && height >= 4)
					{
						pen = new Pen(lightlight, 1.0f);
						pen.EndCap = LineCap.Square;
						graphics.DrawLine(pen, x, y + height - 2, x, y);
						graphics.DrawLine(pen, x + 1, y, x + width - 2, y);
						pen.Color = darkdark;
						graphics.DrawLine(pen, x + width - 1, y,
										  x + width - 1, y + height - 1);
						graphics.DrawLine(pen, x + width - 2, y + height - 1,
										  x, y + height - 1);
						pen.Color = light;
						graphics.DrawLine(pen, x + 1, y + height - 3,
										  x + 1, y + 1);
						graphics.DrawLine(pen, x + 2, y + 1,
										  x + width - 3, y + 1);
						pen.Color = dark;
						graphics.DrawLine(pen, x + width - 2, y + 1,
										  x + width - 2, y + height - 2);
						graphics.DrawLine(pen, x + width - 3, y + height - 2,
										  x + 1, y + height - 2);
						pen.Dispose();
						x += 2;
						y += 2;
						width -= 4;
						height -= 4;
					}
				}

				// Fill the button contents with the background color.
				if(width > 0 && height > 0)
				{
					brush = new SolidBrush(backColor);
					graphics.FillRectangle(brush, x, y, width, height);
					brush.Dispose();
				}
			}

	// Draw a caption button control.
	public virtual void DrawCaptionButton
				(Graphics graphics, int x, int y, int width, int height,
				 CaptionButton button, ButtonState state)
			{
				Color color;

				// Draw the border on the button.
				DrawButton(graphics, x, y, width, height, state,
						   SystemColors.ControlText, SystemColors.Control,
						   false);

				// Adjust the glyph position if necessary.
				if((state & ButtonState.Pushed) != 0)
				{
					++x;
					++y;
				}

				// Draw the inactive drop shadow glyph on the button.
				if((state & ButtonState.Inactive) != 0)
				{
					color = SystemColors.ControlLightLight;
					switch(button)
					{
						case CaptionButton.Close:
						{
							DrawGlyph(graphics, x + 1, y + 1, width, height,
									  Glyphs.close_button_bits,
									  Glyphs.close_button_width,
									  Glyphs.close_button_height, color);
						}
						break;

						case CaptionButton.Minimize:
						{
							DrawGlyph(graphics, x + 1, y + 1, width, height,
									  Glyphs.minimize_button_bits,
									  Glyphs.minimize_button_width,
									  Glyphs.minimize_button_height, color);
						}
						break;

						case CaptionButton.Maximize:
						{
							DrawGlyph(graphics, x + 1, y + 1, width, height,
									  Glyphs.maximize_button_bits,
									  Glyphs.maximize_button_width,
									  Glyphs.maximize_button_height, color);
						}
						break;

						case CaptionButton.Restore:
						{
							DrawGlyph(graphics, x + 1, y + 1, width, height,
									  Glyphs.restore_button_bits,
									  Glyphs.restore_button_width,
									  Glyphs.restore_button_height, color);
						}
						break;

						case CaptionButton.Help:
						{
							DrawGlyph(graphics, x + 1, y + 1, width, height,
									  Glyphs.help_button_bits,
									  Glyphs.help_button_width,
									  Glyphs.help_button_height, color);
						}
						break;
					}
				}

				// Determine the color to use for the glyph.
				if((state & ButtonState.Inactive) != 0)
				{
					color = SystemColors.ControlDark;
				}
				else
				{
					color = SystemColors.ControlText;
				}

				// Draw the glyph on the button.
				switch(button)
				{
					case CaptionButton.Close:
					{
						DrawGlyph(graphics, x, y, width, height,
								  Glyphs.close_button_bits,
								  Glyphs.close_button_width,
								  Glyphs.close_button_height, color);
					}
					break;

					case CaptionButton.Minimize:
					{
						DrawGlyph(graphics, x, y, width, height,
								  Glyphs.minimize_button_bits,
								  Glyphs.minimize_button_width,
								  Glyphs.minimize_button_height, color);
					}
					break;

					case CaptionButton.Maximize:
					{
						DrawGlyph(graphics, x, y, width, height,
								  Glyphs.maximize_button_bits,
								  Glyphs.maximize_button_width,
								  Glyphs.maximize_button_height, color);
					}
					break;

					case CaptionButton.Restore:
					{
						DrawGlyph(graphics, x, y, width, height,
								  Glyphs.restore_button_bits,
								  Glyphs.restore_button_width,
								  Glyphs.restore_button_height, color);
					}
					break;

					case CaptionButton.Help:
					{
						DrawGlyph(graphics, x, y, width, height,
								  Glyphs.help_button_bits,
								  Glyphs.help_button_width,
								  Glyphs.help_button_height, color);
					}
					break;
				}
			}

	// Draw a progress bar control.
	[TODO]
	public virtual void DrawProgressBar(Graphics graphics, int x, int y,
										int width, int height, 
										int steps, int step,
										int value, bool enabled)
			{
				// TODO : handle large number of blocks ie merge cases
				int blockWidth, blockHeight, xSpacing, ySpacing;
				DrawBorder3D(graphics,x,y,width,height, 
							 SystemColors.InactiveBorder,
							 SystemColors.Control,
							 Border3DStyle.SunkenInner,
							 Border3DSide.All);
				width-=4;
				height-=4;
				x+=2;
				y+=2;

				xSpacing=2;
				ySpacing=2;
				width=width-((steps-1)*xSpacing);
				blockWidth=width/steps;
				blockHeight=height-ySpacing-1;
						
				x+=xSpacing;

				for(int i=0;i<steps;i++)
				{
					if((i*step) < value)
					{
						DrawBlock(graphics, x, y+ySpacing, 
						          blockWidth,
						          blockHeight,
						          enabled ? SystemColors.Highlight
						                  : SystemColors.ControlDark);
					}
					x+=blockWidth+xSpacing;
				}
			}

	// Draw a progress bar block.
	public virtual void DrawBlock(Graphics graphics, int x, int y,
								  int width, int height,
								  Color color)
			{
				using (Brush brush = new SolidBrush(color))
				{
					graphics.FillRectangle(brush, x, y, width, height);
					brush.Dispose();
				}
			}

	// Draw a check box control.
	public virtual void DrawCheckBox(Graphics graphics, int x, int y,
								     int width, int height, ButtonState state)
			{
				// Draw the border around the outside of the checkbox.
				if((state & ButtonState.Flat) == 0)
				{
					ControlPaint.DrawBorder3D
						(graphics, x, y, width, height,
						 Border3DStyle.Sunken, Border3DSide.All);
					x += 2;
					y += 2;
					width -= 4;
					height -= 4;
				}
				else
				{
					if((state & ButtonState.Inactive) != 0)
					{
						graphics.DrawRectangle
							(SystemPens.ControlDark, x, y,
							 width - 1, height - 1);
					}
					else
					{
						graphics.DrawRectangle
							(SystemPens.ControlText, x, y,
							 width - 1, height - 1);
					}
					x += 1;
					y += 1;
					width -= 2;
					height -= 2;
				}

				// Fill the background of the checkbox.
				if((state & (ButtonState.Inactive | ButtonState.Pushed)) != 0)
				{
					graphics.FillRectangle
						(SystemBrushes.Control, x, y, width, height);
				}
				else if((state & (ButtonState)0x10000) != 0)
				{
					// Checkbox is in the indeterminate state.
					graphics.FillRectangle
						(SystemBrushes.ControlLightLight, x, y, width, height);
				}
				else
				{
					graphics.FillRectangle
						(SystemBrushes.Window, x, y, width, height);
				}

				// Draw the checkmark in the checkbox.
				if((state & ButtonState.Checked) != 0)
				{
					Color color;
					if((state & ButtonState.Inactive) != 0)
					{
						color = SystemColors.ControlDark;
					}
					else if((state & (ButtonState)0x10000) != 0)
					{
						// Checkbox is in the indeterminate state.
						color = SystemColors.ControlDark;
					}
					else
					{
						color = SystemColors.ControlText;
					}
					DrawGlyph(graphics, x, y, width, height,
							  Glyphs.checkmark_bits,
							  Glyphs.checkmark_width,
							  Glyphs.checkmark_height, color);
				}

#if false	// May need later for non-standard glyph sizes.
				Brush brush = new SolidBrush(Color.White);
				ControlPaint.DrawBorder3D(graphics, x, y, width, height, Border3DStyle.Sunken, Border3DSide.All);
				graphics.FillRectangle(brush, x + 2, y + 2, width - 2, height - 2);
				brush.Dispose();
				
				if (state == ButtonState.Checked)
				{
					Brush blackBrush = new SolidBrush(Color.Black);
					Pen pen = new Pen(blackBrush, 2);

					GraphicsPath path = new GraphicsPath();
					path.AddLine(x + 3, y + 5, x + 4, y + 7);
					path.AddLine(x + 5, y + 8, x + 9, y + 3);
					graphics.DrawPath(pen, path);
					blackBrush.Dispose();
					pen.Dispose();
				}
#endif
			}

	// Draw a combo box's drop down button control.
	public virtual void DrawComboButton(Graphics graphics, int x, int y,
								        int width, int height,
									    ButtonState state)
			{
				// Draw the border part of the button.
				DrawButton(graphics, x, y, width, height, state,
				           SystemColors.ControlText, SystemColors.Control,
				           false);

				// Get the drawing color and adjust for the pressed state.
				Color color;
				if((state & ButtonState.Pushed) != 0)
				{
					color = SystemColors.ControlText;
					++x;
					++y;
				}
				else if((state & ButtonState.Inactive) != 0)
				{
					color = SystemColors.ControlDark;
					DrawGlyph(graphics, x + 1, y + 1, width, height,
							  Glyphs.drop_down_arrow_bits,
							  Glyphs.drop_down_arrow_width,
							  Glyphs.drop_down_arrow_height,
							  SystemColors.ControlLightLight);
				}
				else
				{
					color = SystemColors.ControlText;
				}

				// Draw the glyph on the button.
				DrawGlyph(graphics, x, y, width, height,
						  Glyphs.drop_down_arrow_bits,
						  Glyphs.drop_down_arrow_width,
						  Glyphs.drop_down_arrow_height, color);

#if false	// May need later for non-standard glyph sizes.
				x += width / 2 + 2;
				y += height * 2 / 3 + 2;
				Brush arrow = (state == ButtonState.Inactive) ?
					SystemBrushes.ControlDark : SystemBrushes.ControlText;
				graphics.FillPolygon(arrow, new PointF[]
						{new Point(x, y), new Point(x - 4, y - 4),
						 new Point(x + 4, y - 4)});
#endif
			}

	// Draw a container grab handle.
	public virtual void DrawContainerGrabHandle
				(Graphics graphics, Rectangle rectangle)
			{
				Pen black = new Pen(Color.Black, 1.0f);
				black.EndCap = LineCap.Square;
				graphics.FillRectangle(Brushes.White, rectangle);
				graphics.DrawRectangle(black, rectangle);
				graphics.DrawLine
					(black,
					 rectangle.X + rectangle.Width / 2,
					 rectangle.Y + 2,
					 rectangle.X + rectangle.Width / 2,
					 rectangle.Y + rectangle.Height - 3);
				graphics.DrawLine
					(black,
					 rectangle.X + 2,
					 rectangle.Y + rectangle.Height / 2,
					 rectangle.X + rectangle.Width - 3,
					 rectangle.Y + rectangle.Height / 2);
				graphics.DrawLine
					(black,
					 rectangle.X + rectangle.Width / 2 - 1,
					 rectangle.Y + 3,
					 rectangle.X + rectangle.Width / 2 + 1,
					 rectangle.Y + 3);
				graphics.DrawLine
					(black,
					 rectangle.X + rectangle.Width / 2 - 1,
					 rectangle.Y + rectangle.Height - 4,
					 rectangle.X + rectangle.Width / 2 + 1,
					 rectangle.Y + rectangle.Height - 4);
				graphics.DrawLine
					(black,
					 rectangle.X + 3,
					 rectangle.Y + rectangle.Height / 2 - 1,
					 rectangle.X + 3,
					 rectangle.Y + rectangle.Height / 2 + 1);
				graphics.DrawLine
					(black,
					 rectangle.X + rectangle.Width - 4,
					 rectangle.Y + rectangle.Height / 2 - 1,
					 rectangle.X + rectangle.Width - 4,
					 rectangle.Y + rectangle.Height / 2 + 1);
				black.Dispose();
			}

	// Draw a focus rectangle.
	public virtual void DrawFocusRectangle
				(Graphics graphics, Rectangle rectangle,
				 Color foreColor, Color backColor)
			{
				if(graphics == null)
				{
					throw new ArgumentNullException("graphics", "Argument cannot be null");
				}
				Pen pen = new Pen(foreColor, 1.0f);
				pen.EndCap = LineCap.Square;
				pen.DashStyle = DashStyle.Dot;
				graphics.DrawRectangle(pen, rectangle);
				pen.Dispose();
			}

	// Draw a grab handle.
	public virtual void DrawGrabHandle
				(Graphics graphics, Rectangle rectangle,
				 bool primary, bool enabled)
			{
				Brush background;
				Pen border;
				if(primary)
				{
					if(enabled)
					{
						background = Brushes.White;
						border = Pens.Black;
					}
					else
					{
						background = SystemBrushes.Control;
						border = Pens.Black;
					}
				}
				else
				{
					if(enabled)
					{
						background = Brushes.Black;
						border = Pens.White;
					}
					else
					{
						background = SystemBrushes.Control;
						border = Pens.White;
					}
				}
				graphics.FillRectangle(background, rectangle);
				graphics.DrawRectangle(border, rectangle);
			}

	// Draw a group box.
	public virtual void DrawGroupBox
				(Graphics graphics, Rectangle bounds, Color foreColor,
				 Color backColor, Brush backgroundBrush, bool enabled,
				 bool entered, FlatStyle style, String text, Font font,
				 StringFormat format)
			{
				int x = bounds.X;
				int y = bounds.Y;
				int width = bounds.Width;
				int height = bounds.Height;
				int textOffset = 8;
				bool flat = (style == FlatStyle.Flat) ||
				            (!entered && style == FlatStyle.Popup);
				bool rtl = (format.FormatFlags &
				            StringFormatFlags.DirectionRightToLeft) != 0;

				// fill in the background
				graphics.FillRectangle(backgroundBrush, x, y, width, height);

				// prepare bounds for use as text border
				bounds.X = x + textOffset;
				bounds.Width = width - textOffset*2;

				// simplifies things
				if(text == null) { text = ""; }

				// measure text
			#if CONFIG_EXTENDED_NUMERICS
				Size textSize = Size.Ceiling(graphics.MeasureString(text, font,
				                                                    bounds.Width,
				                                                    format));
			#else
				SizeF textSizeF = graphics.MeasureString(text, font,
		                                                 bounds.Width,
		                                                 format);
				Size textSize = new Size((int)(textSizeF.Width),
										 (int)(textSizeF.Height));
			#endif

				// draw text
				if(enabled)
				{
					using(Brush brush = new SolidBrush(foreColor))
					{
						graphics.DrawString(text, font, brush,
						                    (RectangleF)bounds, format);
					}
				}
				else
				{
					DrawStringDisabled(graphics, text, font, backColor,
					                   (RectangleF)bounds, format);
				}

				// setup for drawing the box
				int left = x;
				int right = x + width - 1;
				int top = y + font.Height/2 - 1;
				int bottom = y + height - 1;
				int textLeft;
				int textRight;
				if(text == "")
				{
					textLeft = textRight = left;
				}
				else if(rtl)
				{
					textRight = right - textOffset + 1;
					textLeft = textRight - textSize.Width - 1;
				}
				else
				{
					textLeft = left + textOffset - 1;
					textRight = textLeft + textSize.Width + 1;
				}

				// draw the box
				if(flat)
				{
					Color color;
					if(enabled)
					{
						color = ControlPaint.Light(foreColor);
					}
					else
					{
						color = ControlPaint.LightLight(foreColor);
					}
					using(Pen pen = new Pen(color))
					{
						// draw left side of box
						graphics.DrawLine(pen, left, top, left, bottom);
						++left;

						// draw right side of box
						graphics.DrawLine(pen, right, top, right, bottom);
						--right;

						// draw bottom of box
						graphics.DrawLine(pen, left, bottom, right, bottom);

						// draw top of box, left of text
						graphics.DrawLine(pen, left, top, textLeft, top);

						// draw top of box, right of text
						graphics.DrawLine(pen, textRight, top, right, top);
					}
				}
				else
				{
					Pen dark;
					Pen light;

					// create pens for drawing the box
					if(enabled)
					{
						dark = new Pen(ControlPaint.DarkDark(backColor));
						light = new Pen(ControlPaint.LightLight(backColor));
					}
					else
					{
						dark = new Pen(ControlPaint.Dark(backColor));
						light = new Pen(ControlPaint.Light(backColor));
					}

					// draw left side of box
					graphics.DrawLine(dark, left, top, left, bottom);
					graphics.DrawLine(light, left+1, top+1, left+1, bottom-1);

					// draw right side of box
					graphics.DrawLine(dark, right-1, top+2, right-1, bottom-2);
					graphics.DrawLine(light, right, top+2, right, bottom-1);

					// draw bottom of box
					graphics.DrawLine(dark, left+2, bottom-1, right-1, bottom-1);
					graphics.DrawLine(light, left+1, bottom, right, bottom);

					// draw top of box, left of text
					graphics.DrawLine(dark, left+1, top, textLeft, top);
					graphics.DrawLine(light, left+2, top+1, textLeft, top+1);

					// draw top of box, right of text
					graphics.DrawLine(dark, textRight, top, right, top);
					graphics.DrawLine(light, textRight, top+1, right, top+1);

					// cleanup
					dark.Dispose();
					light.Dispose();
				}
			}

	// Draw a grid of dots.
	public virtual void DrawGrid
				(Graphics graphics, Rectangle area,
				 Size pixelsBetweenDots, Color backColor)
			{
				// this is called from ControlPaint static methods
				// which are supposed to be thread safe, so lock
				// this while changing state
				lock(this)
				{
					bool dark = (backColor.GetBrightness() >= 0.5f);
					if (gridBrush == null ||
					    gridDark != dark ||
					    gridSpacing != pixelsBetweenDots)
					{
						if (gridBrush != null)
						{
							gridBrush.Dispose();
							gridBrush = null;
						}

						gridDark = dark;
						Color color;
						if (gridDark)
						{
							color = Color.Black;
						}
						else
						{
							color = Color.White;
						}

						gridSpacing = pixelsBetweenDots;
						int spaceX = gridSpacing.Width;
						int spaceY = gridSpacing.Height;
						using (Bitmap bmp = new Bitmap(spaceX+1,spaceY+1))
						{
							for (int y = 0; y < bmp.Height; y++)
							{
								for (int x = 0; x < bmp.Width; x++)
								{
									bmp.SetPixel(x, y, backColor);
								}
							}
							bmp.SetPixel(0, 0, color);
							gridBrush = new TextureBrush(bmp);
							graphics.FillRectangle(gridBrush, area);
						}
					}
				}
			}

	// Draw an image in its disabled state.
	public virtual void DrawImageDisabled
				(Graphics graphics, Image image,
				 int x, int y, Color background)
			{
				ColorMatrix colorMatrix = new ColorMatrix(new float[][] { new float[]{ 0.5f,0.5f,0.5f,0,0 },
											new float[]{ 0.5f,0.5f,0.5f,0,0 },
											new float[]{ 0.5f,0.5f,0.5f,0,0 },
											new float[]{ 0,0,0,1,0,0 },
											new float[]{ 0,0,0,0,1,0 },
											new float[]{ 0,0,0,0,0,1 }
											}
									);

				ImageAttributes imageAttributes = new ImageAttributes();
				imageAttributes.SetColorMatrix(colorMatrix);
				graphics.DrawImage(image,
						new Rectangle(x, y, image.Width, image.Height),
						0, 0,
						image.Width,
						image.Height,
						GraphicsUnit.Pixel,
						imageAttributes);
			}

	// Draw a locked selection frame.
	public virtual void DrawLockedFrame
				(Graphics graphics, Rectangle rectangle, bool primary)
			{
				Pen outer;
				Pen inner;
				if(primary)
				{
					outer = Pens.White;
					inner = Pens.Black;
				}
				else
				{
					outer = Pens.Black;
					inner = Pens.White;
				}
				graphics.DrawRectangle(outer, rectangle);
				graphics.DrawRectangle
					(outer, rectangle.X + 1, rectangle.Y + 1,
					 rectangle.Width - 2, rectangle.Height - 2);
				graphics.DrawRectangle
					(inner, rectangle.X + 2, rectangle.Y + 2,
					 rectangle.Width - 4, rectangle.Height - 4);
			}

	// Draw a menu glyph.
	public virtual void DrawMenuGlyph
				(Graphics graphics, int x, int y, int width,
				 int height, MenuGlyph glyph)
			{
				switch(glyph)
				{
					case MenuGlyph.Arrow:
					{
						DrawGlyph(graphics, x, y, width, height,
								  Glyphs.menu_arrow_bits,
								  Glyphs.menu_arrow_width,
								  Glyphs.menu_arrow_height,
								  SystemColors.MenuText);
					}
					break;

					case MenuGlyph.Checkmark:
					{
						DrawGlyph(graphics, x, y, width, height,
								  Glyphs.menu_checkmark_bits,
								  Glyphs.menu_checkmark_width,
								  Glyphs.menu_checkmark_height,
								  SystemColors.MenuText);
					}
					break;

					case MenuGlyph.Bullet:
					{
						DrawGlyph(graphics, x, y, width, height,
								  Glyphs.menu_bullet_bits,
								  Glyphs.menu_bullet_width,
								  Glyphs.menu_bullet_height,
								  SystemColors.MenuText);
						
					}
					break;
				}
			}

	// Draw a three-state check box control.
	public virtual void DrawMixedCheckBox
				(Graphics graphics, int x, int y, int width,
				 int height, ButtonState state)
			{
				DrawCheckBox(graphics, x, y, width, height,
							 state | (ButtonState)0x10000);
			}

	// Draw a radio button control.
	[TODO]
	public virtual void DrawRadioButton
				(Graphics graphics, int x, int y, int width, int height,
				 ButtonState state, Color foreColor, Color backColor,
				 Brush backgroundBrush)
			{
				// Fill in the background.
				graphics.FillRectangle(backgroundBrush, x, y, width, height);

				// Collect up the colors that we need to draw the button.
				Color lightlight = ControlPaint.LightLight(backColor);
				Color darkdark = ControlPaint.DarkDark(backColor);
				Color light = ControlPaint.Light(backColor);
				Color dark = ControlPaint.Dark(backColor);
				Color fillColor;
				Color bulletColor;
				if((state & ButtonState.Inactive) != 0)
				{
					fillColor = light;
					bulletColor = dark;
				}
				else if((state & ButtonState.Pushed) != 0)
				{
					fillColor = light;
					bulletColor = foreColor;
				}
				else
				{
					fillColor = lightlight;
					bulletColor = foreColor;
				}

				// Draw the glyphs that make up the components.
				int radio_width = Glyphs.radio_width;
				int radio_height = Glyphs.radio_height;
				if((state & ButtonState.Flat) == 0)
				{
					DrawGlyph(graphics, x, y, width, height,
							  Glyphs.radio1_bits,
							  radio_width, radio_height, dark);
					DrawGlyph(graphics, x, y, width, height,
							  Glyphs.radio2_bits,
							  radio_width, radio_height, darkdark);
					DrawGlyph(graphics, x, y, width, height,
							  Glyphs.radio3_bits,
							  radio_width, radio_height, lightlight);
					DrawGlyph(graphics, x, y, width, height,
							  Glyphs.radio4_bits,
							  radio_width, radio_height, light);
					DrawGlyph(graphics, x, y, width, height,
							  Glyphs.radio5_bits,
							  radio_width, radio_height, fillColor);
				}
				else
				{
					DrawGlyph(graphics, x, y, width, height,
							  Glyphs.rflat1_bits,
							  radio_width, radio_height, bulletColor);
					DrawGlyph(graphics, x, y, width, height,
							  Glyphs.rflat2_bits,
							  radio_width, radio_height, fillColor);
				}

				// Draw the glyph for the checked state.
				if((state & ButtonState.Checked) != 0)
				{
					DrawGlyph(graphics, x, y, width, height,
							  Glyphs.radio_bullet_bits,
							  radio_width, radio_height, bulletColor);
				}

#if false	// May need later for non-standard glyph sizes.
				// fill in center area
				using (Brush b = new SolidBrush(fillColor))
				{
					graphics.FillEllipse(b, x+1, y+1, width-1, width-1);
				}

				if ((state & ButtonState.Flat) == 0)
				{
					// draw inner border
					using (Pen p = new Pen(darkdark))
					{
						graphics.DrawArc(p, x+2, y+2, width-2, width-2, 140, 190);
					}
					using (Pen p = new Pen(light))
					{
						graphics.DrawArc(p, x+2, y+2, width-2, width-2, 320, 180);
					}

					// draw outer border
					using (Pen p = new Pen(dark))
					{
						graphics.DrawArc(p, x+1, y+1, width, width, 140, 190);
					}
					using (Pen p = new Pen(lightlight))
					{
						graphics.DrawArc(p, x+1, y+1, width, width, 320, 180);
					}
				}
				else
				{
					// draw border
					using (Pen p = new Pen(darkdark))
					{
						graphics.DrawArc(p, x+1, y+1, width-1, width-1, 0, 360);
					}
				}

				// draw check
				if ((state & ButtonState.Checked) != 0)
				{
					Color checkColor = foreColor;
					if ((state & ButtonState.Inactive) != 0)
					{
						checkColor = ControlPaint.Light(foreColor);
					}

					using (Brush b = new SolidBrush(checkColor))
					{
						if ((state & ButtonState.Flat) != 0)
						{
							graphics.FillEllipse(b, x+4, y+4, 6, 6);
						}
						else
						{
							graphics.FillEllipse(b, x+5, y+5, 5, 5);
						}
					}
				}
#endif
			}

	private Brush hatchBrush;
	private Color hatchForeColor = Color.Empty;
	private Color hatchBackColor = Color.Empty;

	// Draw a scroll bar control.
	public virtual void DrawScrollBar
				(Graphics graphics, Rectangle bounds,
				 Rectangle drawBounds,
				 Color foreColor, Color backColor,
				 bool vertical, bool enabled,
				 Rectangle bar, Rectangle track,
				 Rectangle decrement, bool decDown,
				 Rectangle increment, bool incDown)
			{
				
				               
	
				// fill in the background
				//graphics.FillRectangle(backgroundBrush, bounds);
				if (hatchBrush != null && (foreColor != hatchForeColor ||
					backColor != hatchBackColor))
						hatchBrush.Dispose();
				if (hatchBrush == null)
					hatchBrush = new HatchBrush
						  (HatchStyle.Percent50,
						   SystemColors.ScrollBar,
						   SystemColors.ControlLightLight);

				Color color;
				if (backColor.GetBrightness() > 0.5f)
				{
					color = Color.White;
				}
				else
				{
					color = Color.Black;
				}
				Region r = new Region(track);
				r.Intersect(drawBounds);
				r.Exclude(bar);
				r.Exclude(increment);
				r.Exclude(decrement);
				graphics.FillRegion(hatchBrush, r);
				
				// workaround some strange visual bugs with the
				// hatch... comment these out to see... take a
				// screen shot and zoom in on decrement and bar
				//graphics.FillRectangle(backgroundBrush, bar);
				//graphics.FillRectangle(backgroundBrush, decrement);
				//graphics.FillRectangle(backgroundBrush, increment);

				// setup the arrow directions for the scroll buttons
				ScrollButton decButton;
				ScrollButton incButton;
				if (vertical)
				{
					decButton = ScrollButton.Up;
					incButton = ScrollButton.Down;
				}
				else
				{
					decButton = ScrollButton.Left;
					incButton = ScrollButton.Right;
				}

				// setup the states for the scroll buttons
				ButtonState decState;
				ButtonState incState;
				if (!enabled)
				{
					decState = ButtonState.Inactive;
					incState = ButtonState.Inactive;
				}
				else
				{
					decState = decDown ?
					           ButtonState.Pushed :
					           ButtonState.Normal;
					incState = incDown ?
					           ButtonState.Pushed :
					           ButtonState.Normal;
				}
				// draw the scroll button
				if (decrement.IntersectsWith(drawBounds))
					DrawScrollButton(graphics,
				                 decrement.X, decrement.Y,
				                 decrement.Width, decrement.Height,
				                 decButton, decState,
				                 foreColor, backColor);
				
				// draw the scroll button
				if (increment.IntersectsWith(drawBounds))
					DrawScrollButton(graphics,
				                 increment.X, increment.Y,
				                 increment.Width, increment.Height,
				                 incButton, incState,
				                 foreColor, backColor);

				// calculate the state for the scroll bar
				ButtonState barState = enabled ?
				                       ButtonState.Normal :
				                       ButtonState.Inactive;
				
				// draw the scroll bar
				if (track.Height > 0 && bar.IntersectsWith(drawBounds))
					DrawButton(graphics,
				           bar.X, bar.Y,
				           bar.Width, bar.Height,
				           barState,
				           foreColor, backColor,
				           false);   
			}

	// Draw a scroll button control.
	public virtual void DrawScrollButton
				(Graphics graphics, int x, int y, int width, int height,
				 ScrollButton button, ButtonState state,
				 Color foreColor, Color backColor)
			{
				
			        // draw the button border
				
				DrawButton(graphics,
				           x, y, width, height,
				           state,
				           foreColor, backColor,
				           false);
#if false
				// Get the drawing color and adjust for the pressed state.
				Color color;
				Color inactiveColor = ControlPaint.LightLight(backColor);
				if((state & ButtonState.Pushed) != 0)
				{
					color = foreColor;
					++x;
					++y;
				}
				else if((state & ButtonState.Inactive) != 0)
				{
					color = ControlPaint.Dark(backColor);
				}
				else
				{
					color = foreColor;
				}

				// Draw the inactive drop-down glyph for the button.
				
				if(width==0)width=-1;
				
				if((state & ButtonState.Inactive) != 0)
				{
				
					switch (button)
					{
						case ScrollButton.Up:
						{
						
								DrawGlyph(graphics, x + 1, y + 1, width, height,
									  Glyphs.up_arrow_bits,
									  Glyphs.up_arrow_width,
									  Glyphs.up_arrow_height, inactiveColor);

						}
						break;
	
						case ScrollButton.Down:
						{
						
								DrawGlyph(graphics, x + 1, y + 1, width, height,
									  Glyphs.down_arrow_bits,
									  Glyphs.down_arrow_width,
									  Glyphs.down_arrow_height, inactiveColor);
						
							}
						break;
	                                        
						case ScrollButton.Left:
						{
							DrawGlyph(graphics, x + 1, y + 1, width, height,
									  Glyphs.left_arrow_bits,
									  Glyphs.left_arrow_width,
									  Glyphs.left_arrow_height, inactiveColor);
						}
						break;
	
						case ScrollButton.Right:
						{
							DrawGlyph(graphics, x + 1, y + 1, width, height,
									  Glyphs.right_arrow_bits,
									  Glyphs.right_arrow_width,
									  Glyphs.right_arrow_height, inactiveColor);
						}
						break;
					}
				}

				// Draw the glyph for the button.
				switch (button)
				{
					case ScrollButton.Up:
					{      
						
							DrawGlyph(graphics, x, y, width, height,
								  Glyphs.up_arrow_bits,
								  Glyphs.up_arrow_width,
								  Glyphs.up_arrow_height, color);
						
					}
					break;
					 
					case ScrollButton.Down:
					{       
						
							DrawGlyph(graphics, x, y, width, height,
								  Glyphs.down_arrow_bits,
								  Glyphs.down_arrow_width,
								  Glyphs.down_arrow_height, color);

					}
					break;

					case ScrollButton.Left:
					{
					
					     
					   							
						DrawGlyph(graphics, x, y, width, height,
								  Glyphs.left_arrow_bits,
								  Glyphs.left_arrow_width,
					     			  Glyphs.left_arrow_height, color);
					        							
					}
					break;

					case ScrollButton.Right:                                                
					{
						DrawGlyph(graphics, x, y, width, height,
								  Glyphs.right_arrow_bits,
								  Glyphs.right_arrow_width,
								  Glyphs.right_arrow_height, color);
					}
					break;
				}
#endif

				// Set non-standard glyph sizes.
				// The glyphs are resizable using the space left for the increment and decrement buttons.
				x += 2; // skip border
				y += 2; // skip border
				width -= 4; // skip border
				height -= 4; // skip border
				if ((state & ButtonState.Pushed) == 0)
				{
					// the center calculation is off by one
					// so if we don't need "click shift"
					// then minus one from our x and y
					// offsets instead of adding one
					//
					// TODO: "click shift" should probably
					// be set via a protected field or
					// property so themes which don't have
					// it can more easily use this as a base
					x -= 1;
					y -= 1;
				}

				int glyphWidth = 3; // Glyph width size
				int glyphHeight = 3; // Glyph height size
				switch (button)
				{
					case ScrollButton.Up:
				{
						if (height<1)  // Same but for height (UP and DOWN buttons) 
						{
							glyphWidth = 0;
							glyphHeight = 0;				    
				}
						else if (height<=4) // if the glyph is small we draw a single point
						{
							glyphWidth = 0;
							glyphHeight = 1;
						}
						else if (height<=12) // We resize the glyph picture using its logical size.
						{
							glyphWidth = (int)(3.0*((double)height/12.0));
							glyphHeight = (int)(3.0*((double)height/12.0));
						}
					}
					break;	


					case ScrollButton.Down:
					{
						if (height<1)  // Same but for height (UP and DOWN buttons) 
						{
							glyphWidth = 0;
							glyphHeight = 0;				    
						}
						else if (height<=4) // if the glyph is small we draw a single point
						{
							glyphWidth = 0;
							glyphHeight = 1;
						}
						else if (height<=12) // We resize the glysh picture using its logical size.
				{
							glyphWidth = (int)(3.0*((double)height/12.0));
							glyphHeight = (int)(3.0*((double)height/12.0));
				}
					}
					break;	
					
					case ScrollButton.Right:
					{
						if (width<1) // If the glyph width is too small we don't draw it (LEFT and RIGHT buttons)
						{
							glyphWidth = 0;
							glyphHeight = 0;
						}
						else if (width<=4) // if the glyph is small we draw a single point
						{
							glyphWidth = 1;
							glyphHeight = 0;
						}
						else if (width<=12) // We resize the glysh picture using its logical size.
						{
							glyphWidth = (int)(3.0*((double)width/12.0));
							glyphHeight = (int)(3.0*((double)width/12.0));
						}
					}
					break;


					case ScrollButton.Left:
					{
						if (width<1) // If the glyph width is too small we don't draw it (LEFT and RIGHT buttons)
						{
							glyphWidth = 0;
							glyphHeight = 0;
						}
						else if (width<=4) // if the glyph is small we draw a single point
						{
							glyphWidth = 1;
							glyphHeight = 0;
						}
						else if (width<=12) // We resize the glyph using its logical size.
						{
							glyphWidth = (int)(3.0*((double)width/12.0));
							glyphHeight = (int)(3.0*((double)width/12.0));
						}
					}
					break;

					default:
					{
						throw new ArgumentException("Invalid scroll button");
					}
				}


				

					

				
				// setup the glyph shape

				Point[] glyph = new Point[3];
				int offsetX;
				int offsetY;

				switch (button)
				{
					case ScrollButton.Up:
					{
						offsetX = x+width/2-glyphWidth;
						offsetY = y+(height+1)/2;

						glyph[0] = new Point(offsetX,
						                     offsetY);
						glyph[1] = new Point(offsetX+glyphWidth*2,
						                     offsetY);
						glyph[2] = new Point(offsetX+glyphWidth,
						                     offsetY-glyphHeight);
					}
					break;

					case ScrollButton.Down:
					{
						offsetX = x+width/2-glyphWidth;
	  					offsetY = y+(height+1)/2-glyphHeight/2;

						glyph[0] = new Point(offsetX,
						                     offsetY);
						glyph[1] = new Point(offsetX+glyphWidth*2,
						                     offsetY);
						glyph[2] = new Point(offsetX+glyphWidth,
						                     offsetY+glyphHeight);
					}
					break;

					case ScrollButton.Left:
					{

						offsetX = x+(width+1)/2-glyphWidth/2;

						offsetY = y+((height-glyphHeight*2)/2);

						glyph[0] = new Point(offsetX+glyphWidth,
						                     offsetY);
						glyph[1] = new Point(offsetX+glyphWidth,
						                     offsetY+glyphHeight*2);
						glyph[2] = new Point(offsetX,
						                     offsetY+glyphHeight);
					}
					break;

					case ScrollButton.Right:
					{
						offsetX = x+(width+1)/2;
						offsetY = y+((height-glyphHeight*2)/2);
						glyph[0] = new Point(offsetX,
						                     offsetY);
						glyph[1] = new Point(offsetX,
						                     offsetY+glyphHeight*2);
						glyph[2] = new Point(offsetX+glyphWidth,
						                     offsetY+glyphHeight);
					}
					break;

					default:
					{
						throw new ArgumentException("Invalid scroll button");
					}
				}
				
				// draw the arrow
				Color color = foreColor;
				if ((state & ButtonState.Inactive) != 0)
				{
					color = ControlPaint.LightLight(foreColor);
				}
				using (Brush brush = new SolidBrush(color))
				{
					graphics.FillPolygon(brush,glyph);
				}
				using (Pen pen = new Pen(color))
				{
					graphics.DrawPolygon(pen,glyph);
				}


			}

	// Draw a selection frame.
	[TODO]
	public virtual void DrawSelectionFrame
				(Graphics graphics, bool active, Rectangle outsideRect,
				 Rectangle insideRect, Color backColor)
			{
				// TODO
			}

	// Draw a size grip.
	public virtual void DrawSizeGrip
				(Graphics graphics, Color backColor, Rectangle drawBounds)
			{
				Pen light = new Pen(Color.White, 1);
				Pen dark = new Pen(Color.Black, 1);

				// Backfill accordingly
				graphics.FillRectangle(new SolidBrush(backColor),
							drawBounds.X, drawBounds.Y,
							drawBounds.Width, drawBounds.Height);

				// Top most line
				graphics.DrawLine(SystemPens.ControlDark,
						drawBounds.Left + 1,
						drawBounds.Height - 1,
						drawBounds.Width - 1 ,
						drawBounds.Top + 1
					);
				graphics.DrawLine(SystemPens.ControlLightLight,
						drawBounds.Left,
						drawBounds.Height - 1,
						drawBounds.Width - 1,
						drawBounds.Top
					);

				// Middle line
				graphics.DrawLine(SystemPens.ControlDark,
						drawBounds.Left + 5,
						drawBounds.Height - 1,
						drawBounds.Width - 1,
						drawBounds.Top + 5
					);
				graphics.DrawLine(SystemPens.ControlLightLight,
						drawBounds.Left + 4,
						drawBounds.Height - 1,
						drawBounds.Width - 1,
						drawBounds.Top + 4
					);

				// Bottom line
				graphics.DrawLine(SystemPens.ControlDark,
						drawBounds.Left + 10,
						drawBounds.Height - 1,
						drawBounds.Width - 1,
						drawBounds.Top + 10
					);
				graphics.DrawLine(SystemPens.ControlLightLight,
						drawBounds.Left + 9,
						drawBounds.Height - 1,
						drawBounds.Width - 1,
						drawBounds.Top  + 9
					);
			}

	// Draw a list box
	public void DrawListBox(Graphics graphics,
		int x, int y, int width, int height, bool corner,
		int cornerHeight, int cornerWidth)
			{
				// If both scroll bars are visible, then paint the region
				// that is blank where the two meet (lower-right corner)
				if(corner)
				{
					graphics.FillRectangle(
						new SolidBrush(SystemColors.Control),
						width - cornerWidth,
						height - cornerHeight,
						cornerWidth,
						cornerHeight);
				}
			}


	// Draw a disabled string.
	public virtual void DrawStringDisabled
				(Graphics graphics, String s, Font font,
			     Color color, RectangleF layoutRectangle,
				 StringFormat format)
			{
				SolidBrush brush = new SolidBrush(ControlPaint.Light(color));
				layoutRectangle.Offset(1.0f, 1.0f);
				graphics.DrawString(s, font, brush, layoutRectangle, format);
				brush.Color = ControlPaint.Dark(ControlPaint.Dark(ControlPaint.Dark(color)));
				layoutRectangle.Offset(-1.0f, -1.0f);
				graphics.DrawString(s, font, brush, layoutRectangle, format);
				brush.Dispose();
			}

	// Draw a menu separator line.
	public virtual void DrawSeparator(Graphics graphics, Rectangle rectangle)
			{
				int x = rectangle.X + 2;
				int width = rectangle.Width - 4;
				int y = rectangle.Y + (rectangle.Height / 2);
				graphics.DrawLine
					(SystemPens.ControlDark, x, y, x + width - 1, y);
				graphics.DrawLine
					(SystemPens.ControlLightLight, x, y + 1,
					 x + width - 1, y + 1);
			}

	// Draw a track bar control.
	public virtual void DrawTrackBar(Graphics graphics, Rectangle clientRect,
					Rectangle barRect, Orientation orientation, bool enabled,
					int ticks, TickStyle style)
	{
		int x = 0;
		int lineheight = 4;
		int linewidth = 4;

		if(orientation == Orientation.Horizontal) 
		{ // Draw horizontal
			x = (clientRect.Width / ticks) + 1;

			if(style == TickStyle.BottomRight) 
			{
				for(int cnt = x; cnt <= clientRect.Width; cnt += x) 
				{
					// right tick
					DrawBorder3D(graphics,
						clientRect.Left + cnt,
						clientRect.Height / 2,
						linewidth,
						clientRect.Height / lineheight,
						SystemColors.ControlText, SystemColors.Control,
						Border3DStyle.Sunken, Border3DSide.All);
				}
			}
			else if(style == TickStyle.TopLeft) 
			{
				for(int cnt = x; cnt <= clientRect.Width; cnt += x) 
				{
					// left tick
					DrawBorder3D(graphics,
						clientRect.Left + cnt,
						clientRect.Top + clientRect.Height / lineheight,
						linewidth,
						clientRect.Height / lineheight,
						SystemColors.ControlText, SystemColors.Control,
						Border3DStyle.Sunken, Border3DSide.All);
				}
			}
			else if(style == TickStyle.Both) 
			{
				for(int cnt = x; cnt <= clientRect.Width; cnt += x) 
				{
					// left tick
					DrawBorder3D(graphics,
						clientRect.Left + cnt,
						clientRect.Height / 2 - lineheight,
						linewidth,
						lineheight * 2 + lineheight / 2,
						SystemColors.ControlText, SystemColors.Control,
						Border3DStyle.Sunken, Border3DSide.All);
				}
			}
		}
		else 
		{ // Draw vertical
			x = (clientRect.Height / ticks) + 1;

			if(style == TickStyle.BottomRight) 
			{
				for(int cnt = x; cnt < clientRect.Height; cnt += x) 
				{
					// right tick
					DrawBorder3D(graphics,
						clientRect.Width / 2,
						clientRect.Top + cnt,
						clientRect.Width / linewidth,
						lineheight,
						SystemColors.ControlText, SystemColors.Control,
						Border3DStyle.Sunken, Border3DSide.All);
				}
			}
			else if(style == TickStyle.TopLeft) 
			{
				for(int cnt = x; cnt < clientRect.Height; cnt += x) 
				{
					// left tick
					DrawBorder3D(graphics,
						clientRect.Left + clientRect.Width / linewidth,
						clientRect.Top + cnt,
						clientRect.Width / linewidth,
						lineheight,
						SystemColors.ControlText, SystemColors.Control,
						Border3DStyle.Sunken, Border3DSide.All);
				}
			}
			else if(style == TickStyle.Both) 
			{
				for(int cnt = x; cnt < clientRect.Height; cnt += x) 
				{
					// left tick
					DrawBorder3D(graphics,
						clientRect.Width / 2 - linewidth,
						clientRect.Top + cnt,
						linewidth * 2 + linewidth / 2,
						lineheight,
						SystemColors.ControlText, SystemColors.Control,
						Border3DStyle.Sunken, Border3DSide.All);
				}
			}
		}

		// draw end lines if we have a tickstyle
		if(style != TickStyle.None) 
		{
			if(orientation == Orientation.Horizontal) 
			{
				// Draw middle bar
				DrawBorder3D(graphics,
					clientRect.Left, (clientRect.Height / 2) - (lineheight / 2),
					clientRect.Width, lineheight,
					SystemColors.ControlText, SystemColors.Control,
					Border3DStyle.Sunken, Border3DSide.All);

				// left vertical bar
				DrawBorder3D(graphics,
					clientRect.Left, clientRect.Top,
					linewidth, clientRect.Height,
					SystemColors.ControlText, SystemColors.Control,
					Border3DStyle.Sunken, Border3DSide.All);

				// right vertical bar
				DrawBorder3D(graphics,
					clientRect.Right - linewidth, clientRect.Top,
					linewidth, clientRect.Height,
					SystemColors.ControlText, SystemColors.Control,
					Border3DStyle.Sunken, Border3DSide.All);
			}
			else 
			{
				// Draw middle bar
				DrawBorder3D(graphics,
					clientRect.Width / 2 - 1, clientRect.Y,
					linewidth, clientRect.Height,
					SystemColors.ControlText, SystemColors.Control,
					Border3DStyle.Sunken, Border3DSide.All);

				// top horizontal bar
				DrawBorder3D(graphics,
					clientRect.Left, clientRect.Top,
					clientRect.Width, lineheight,
					SystemColors.ControlText, SystemColors.Control,
					Border3DStyle.Sunken, Border3DSide.All);

				// bottom horizontal bar
				DrawBorder3D(graphics,
					clientRect.Left, clientRect.Bottom - lineheight,
					clientRect.Width, lineheight,
					SystemColors.ControlText, SystemColors.Control,
					Border3DStyle.Sunken, Border3DSide.All);
			}
		}

		// draw the button
		if (((barRect.Height > 0) ||
			(barRect.Width > 0)) && barRect.IntersectsWith(clientRect))
		{
			DrawButton(graphics,
				barRect.X, barRect.Y,
				barRect.Width,
				barRect.Height,
				(enabled ? ButtonState.Normal : ButtonState.Inactive),
				SystemColors.ControlText, SystemColors.Control,
				false);
		}
	}
}; // class DefaultThemePainter

}; // namespace System.Windows.Forms.Themes
