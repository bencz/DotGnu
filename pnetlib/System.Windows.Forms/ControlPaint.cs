/*
 * ControlPaint.cs - Implementation of the
 *			"System.Windows.Forms.ControlPaint" class.
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

namespace System.Windows.Forms
{

using System.Windows.Forms.Themes;
using System.Drawing;
using System.Drawing.Drawing2D;

// The real work is done in "System.Windows.Forms.Themes.DefaultThemePainter",
// where it can be easily overridden for theme-specific rendering.

public sealed class ControlPaint
{
	// This class cannot be instantiated.
	private ControlPaint() {}

	// Get the ControlDark color, taking contrast into account.
	public static Color ContrastControlDark
			{
				get
				{
					return SystemColors.ControlDark;
				}
			}

	// Convert HSB values into an RGB value.
	// Adapted from the algoritm in Foley and Van-Dam.
	private static Color FromHSB(float hue, float saturation, float brightness)
	{

		float r=0, g=0, b=0;
		if(brightness != 0)
		{
			if(saturation==0)
			{
				r=g=b=brightness;
			} 
			else 
			{
				float temp2;
				if (brightness<=0.5)
					temp2 = brightness * (1.0f + saturation);
				else
					temp2 = brightness + saturation - brightness * saturation;

				float temp1 = 2.0f * brightness - temp2;
				float t1 = hue + 120.0f;
				float t2 = hue;
				float t3 = hue - 120.0f;
				if (t3 < 0)
					t3 += 360.0f;
				r = g = b = temp1;

				if(t1 < 180.0f)
					r = temp2;
				else if(t1 < 240.0f)
					r += (temp2 - temp1) * (240.0f - t1) / 60.0f;

				if(t2 < 60.0f)
					g += (temp2 - temp1) * t2 / 60.0f;
				else if(t2 < 180.0f)
					g = temp2;
				else if(t2 < 240.0f)
					g = temp1 + (temp2 - temp1) * (240.0f - t2) / 60.0f;

				if(t3 < 60.0f)
					b += (temp2 - temp1) * t3 / 60.0f;
				else if(t3 < 180.0f)
					b = temp2;
				else if(t3 < 240.0f)
					b += (temp2 - temp1) * (240.0f - t3) / 60.0f;
			}
		}
		return Color.FromArgb((int)(255*r),(int)(255*g),(int)(255*b)); 
	}

	// Get the "dark" version of a color.
	public static Color Dark(Color baseColor)
			{
				return Dark(baseColor, 0.5f);
			}
	public static Color DarkDark(Color baseColor)
			{
				return Dark(baseColor, 1.0f);
			}
	public static Color Dark(Color baseColor, float percOfDarkDark)
			{
				if(baseColor.ToKnownColor() == KnownColor.Control)
				{
					if(percOfDarkDark <= 0.0f)
					{
						return SystemColors.ControlDark;
					}
					else if(percOfDarkDark >= 1.0f)
					{
						return SystemColors.ControlDarkDark;
					}
					else
					{
						Color dark = SystemColors.ControlDark;
						Color darkdark = SystemColors.ControlDarkDark;
						int redDiff = darkdark.R - dark.R;
						int greenDiff = darkdark.G - dark.G;
						int blueDiff = darkdark.B - dark.B;
						return Color.FromArgb
							(dark.R + (int)(redDiff * percOfDarkDark),
							 dark.G + (int)(greenDiff * percOfDarkDark),
							 dark.B + (int)(blueDiff * percOfDarkDark));
					}
				}
				float hue = baseColor.GetHue();
				float saturation = baseColor.GetSaturation();
				float brightness = baseColor.GetBrightness();
				brightness -= brightness * percOfDarkDark * 0.333f;
				if(brightness < 0.0f)
				{
					brightness = 0.0f;
				}
				return FromHSB(hue, saturation, brightness);
			}

	// Get the "light" version of a color.
	public static Color Light(Color baseColor)
			{
				return Light(baseColor, 0.5f);
			}
	public static Color LightLight(Color baseColor)
			{
				return Light(baseColor, 1.0f);
			}
	public static Color Light(Color baseColor, float percOfLightLight)
			{
				if(baseColor.ToKnownColor() == KnownColor.Control)
				{
					if(percOfLightLight <= 0.0f)
					{
						return SystemColors.ControlLight;
					}
					else if(percOfLightLight >= 1.0f)
					{
						return SystemColors.ControlLightLight;
					}
					else
					{
						Color light = SystemColors.ControlLight;
						Color lightlight = SystemColors.ControlLightLight;
						int redDiff = lightlight.R - light.R;
						int greenDiff = lightlight.G - light.G;
						int blueDiff = lightlight.B - light.B;
						return Color.FromArgb
							(light.R + (int)(redDiff * percOfLightLight),
							 light.G + (int)(greenDiff * percOfLightLight),
							 light.B + (int)(blueDiff * percOfLightLight));
					}
				}
				float hue = baseColor.GetHue();
				float saturation = baseColor.GetSaturation();
				float brightness = baseColor.GetBrightness();
				brightness += brightness * percOfLightLight * 0.5f;
				if(brightness > 1.0f)
				{
					brightness = 1.0f;
				}
				return  FromHSB(hue, saturation, brightness);
			}

	// Draw a simple button border.
	public static void DrawBorder
				(Graphics graphics, Rectangle bounds,
				 Color color, ButtonBorderStyle style)
			{
				ThemeManager.MainPainter.DrawBorder
					(graphics, bounds, color, style);
			}
	public static void DrawBorder
				(Graphics graphics, Rectangle bounds, Color leftColor,
			     int leftWidth, ButtonBorderStyle leftStyle, Color topColor,
				 int topWidth, ButtonBorderStyle topStyle, Color rightColor,
				 int rightWidth, ButtonBorderStyle rightStyle,
				 Color bottomColor, int bottomWidth,
				 ButtonBorderStyle bottomStyle)
			{
				ThemeManager.MainPainter.DrawBorder
					(graphics, bounds, leftColor, leftWidth, leftStyle,
					 topColor, topWidth, topStyle,
					 rightColor, rightWidth, rightStyle,
					 bottomColor, bottomWidth, bottomStyle);
			}

	// Draw a 3D border within a rectangle.
	public static void DrawBorder3D(Graphics graphics, Rectangle rectangle)
			{
				DrawBorder3D(graphics, rectangle.X, rectangle.Y,
							 rectangle.Width, rectangle.Height,
							 Border3DStyle.Etched,
							 Border3DSide.Left | Border3DSide.Top |
							 Border3DSide.Right | Border3DSide.Bottom);
			}
	public static void DrawBorder3D(Graphics graphics, Rectangle rectangle,
									Border3DStyle style)
			{
				DrawBorder3D(graphics, rectangle.X, rectangle.Y,
							 rectangle.Width, rectangle.Height, style,
							 Border3DSide.Left | Border3DSide.Top |
							 Border3DSide.Right | Border3DSide.Bottom);
			}
	public static void DrawBorder3D(Graphics graphics, Rectangle rectangle,
									Border3DStyle style, Border3DSide sides)
			{
				DrawBorder3D(graphics, rectangle.X, rectangle.Y,
							 rectangle.Width, rectangle.Height, style, sides);
			}
	public static void DrawBorder3D(Graphics graphics, int x, int y,
									int width, int height)
			{
				DrawBorder3D(graphics, x, y, width, height,
							 Border3DStyle.Etched,
							 Border3DSide.Left | Border3DSide.Top |
							 Border3DSide.Right | Border3DSide.Bottom);
			}
	public static void DrawBorder3D(Graphics graphics, int x, int y,
									int width, int height,
									Border3DStyle style)
			{
				DrawBorder3D(graphics, x, y, width, height, style,
							 Border3DSide.Left | Border3DSide.Top |
							 Border3DSide.Right | Border3DSide.Bottom);
			}
	public static void DrawBorder3D(Graphics graphics, int x, int y,
									int width, int height,
									Border3DStyle style,
									Border3DSide sides)
			{
				ThemeManager.MainPainter.DrawBorder3D
					(graphics, x, y, width, height,
					 SystemColors.InactiveBorder,
					 SystemColors.Control,
					 style, sides);
			}

	// Draw a button control.
	public static void DrawButton(Graphics graphics, Rectangle rectangle,
								  ButtonState state)
			{
				ThemeManager.MainPainter.DrawButton
					(graphics, rectangle.X, rectangle.Y,
					 rectangle.Width, rectangle.Height, state,
					 SystemColors.ControlText,
					 SystemColors.Control, false);
			}
	public static void DrawButton(Graphics graphics, int x, int y,
								  int width, int height, ButtonState state)
			{
				ThemeManager.MainPainter.DrawButton
					(graphics, x, y, width, height, state,
					 SystemColors.ControlText,
					 SystemColors.Control, false);
			}

	// Draw a caption button control.
	public static void DrawCaptionButton
				(Graphics graphics, Rectangle rectangle,
				 CaptionButton button, ButtonState state)
			{
				DrawCaptionButton(graphics, rectangle.X, rectangle.Y,
						   		  rectangle.Width, rectangle.Height,
								  button, state);
			}
	public static void DrawCaptionButton
				(Graphics graphics, int x, int y, int width, int height,
				 CaptionButton button, ButtonState state)
			{
				ThemeManager.MainPainter.DrawCaptionButton
					(graphics, x, y, width, height, button, state);
			}

	// Draw a check box control.
	public static void DrawCheckBox(Graphics graphics, Rectangle rectangle,
								    ButtonState state)
			{
				DrawCheckBox(graphics, rectangle.X, rectangle.Y,
						     rectangle.Width, rectangle.Height, state);
			}
	public static void DrawCheckBox(Graphics graphics, int x, int y,
								    int width, int height, ButtonState state)
			{
				ThemeManager.MainPainter.DrawCheckBox
					(graphics, x, y, width, height, state);
			}

	// Draw a combo box's drop down button control.
	public static void DrawComboButton(Graphics graphics, Rectangle rectangle,
								       ButtonState state)
			{
				DrawComboButton(graphics, rectangle.X, rectangle.Y,
						        rectangle.Width, rectangle.Height, state);
			}
	public static void DrawComboButton(Graphics graphics, int x, int y,
								       int width, int height,
									   ButtonState state)
			{
				ThemeManager.MainPainter.DrawComboButton
					(graphics, x, y, width, height, state);
			}

	// Draw a container grab handle.
	public static void DrawContainerGrabHandle
				(Graphics graphics, Rectangle rectangle)
			{
				ThemeManager.MainPainter.DrawContainerGrabHandle
					(graphics, rectangle);
			}

	// Draw a focus rectangle.
	public static void DrawFocusRectangle
				(Graphics graphics, Rectangle rectangle)
			{
				DrawFocusRectangle(graphics, rectangle,
								   SystemColors.ControlText,
								   SystemColors.Control);
			}
	public static void DrawFocusRectangle
				(Graphics graphics, Rectangle rectangle,
				 Color foreColor, Color backColor)
			{
				ThemeManager.MainPainter.DrawFocusRectangle
					(graphics, rectangle, foreColor, backColor);
			}

	// Draw a grab handle.
	public static void DrawGrabHandle
				(Graphics graphics, Rectangle rectangle,
				 bool primary, bool enabled)
			{
				ThemeManager.MainPainter.DrawGrabHandle
					(graphics, rectangle, primary, enabled);
			}

	// Draw a grid of dots.
	public static void DrawGrid(Graphics graphics, Rectangle area,
								Size pixelsBetweenDots, Color backColor)
			{
				ThemeManager.MainPainter.DrawGrid
					(graphics, area, pixelsBetweenDots, backColor);
			}

	// Draw an image in its disabled state.
	public static void DrawImageDisabled
				(Graphics graphics, Image image,
				 int x, int y, Color background)
			{
				ThemeManager.MainPainter.DrawImageDisabled
					(graphics, image, x, y, background);
			}

	// Draw a locked selection frame.
	public static void DrawLockedFrame
				(Graphics graphics, Rectangle rectangle, bool primary)
			{
				ThemeManager.MainPainter.DrawLockedFrame
					(graphics, rectangle, primary);
			}

	// Draw a menu glyph.
	public static void DrawMenuGlyph
				(Graphics graphics, Rectangle rectangle, MenuGlyph glyph)
			{
				DrawMenuGlyph(graphics, rectangle.X, rectangle.Y,
						      rectangle.Width, rectangle.Height, glyph);
			}
	public static void DrawMenuGlyph
				(Graphics graphics, int x, int y, int width,
				 int height, MenuGlyph glyph)
			{
				ThemeManager.MainPainter.DrawMenuGlyph
					(graphics, x, y, width, height, glyph);
			}

	// Draw a three-state check box control.
	public static void DrawMixedCheckBox
				(Graphics graphics, Rectangle rectangle, ButtonState state)
			{
				DrawMixedCheckBox(graphics, rectangle.X, rectangle.Y,
						     	  rectangle.Width, rectangle.Height, state);
			}
	public static void DrawMixedCheckBox
				(Graphics graphics, int x, int y, int width,
				 int height, ButtonState state)
			{
				ThemeManager.MainPainter.DrawMixedCheckBox
					(graphics, x, y, width, height, state);
			}

	// Draw a radio button control.
	public static void DrawRadioButton
				(Graphics graphics, Rectangle rectangle, ButtonState state)
			{
				DrawRadioButton(graphics, rectangle.X, rectangle.Y,
						     	rectangle.Width, rectangle.Height, state);
			}
	public static void DrawRadioButton
				(Graphics graphics, int x, int y, int width,
				 int height, ButtonState state)
			{
				using (Brush bg = new SolidBrush(SystemColors.Control))
				{
					ThemeManager.MainPainter.DrawRadioButton
						(graphics, x, y, width, height, state,
						 SystemColors.ControlText, SystemColors.Control, bg);
				}
			}

	// Draw a reversible frame.
	public static void DrawReversibleFrame
				(Rectangle rectangle, Color backColor, FrameStyle style)
			{
				// This is too dangerous: not supported in this implementation.
			}

	// Draw a reversible line.
	public static void DrawReversibleLine
				(Point start, Point end, Color backColor)
			{
				// This is too dangerous: not supported in this implementation.
			}

	// Draw a scroll button control.
	public static void DrawScrollButton
				(Graphics graphics, Rectangle rectangle,
				 ScrollButton button, ButtonState state)
			{
				ThemeManager.MainPainter.DrawScrollButton
					(graphics,
					 rectangle.X, rectangle.Y,
					 rectangle.Width, rectangle.Height,
					 button, state,
					 SystemColors.ControlText,
					 SystemColors.Control);
			}
	public static void DrawScrollButton
				(Graphics graphics, int x, int y, int width, int height,
				 ScrollButton button, ButtonState state)
			{
				ThemeManager.MainPainter.DrawScrollButton
					(graphics,
					 x, y, width, height,
					 button, state,
					 SystemColors.ControlText,
					 SystemColors.Control);
			}

	// Draw a selection frame.
	public static void DrawSelectionFrame
				(Graphics graphics, bool active, Rectangle outsideRect,
				 Rectangle insideRect, Color backColor)
			{
				ThemeManager.MainPainter.DrawSelectionFrame
					(graphics, active, outsideRect, insideRect, backColor);
			}

	// Draw a size grip.
	public static void DrawSizeGrip
				(Graphics graphics, Color backColor, Rectangle rectangle)
			{
				ThemeManager.MainPainter.DrawSizeGrip(graphics, backColor, rectangle);
			}
	public static void DrawSizeGrip
				(Graphics graphics, Color backColor,
				 int x, int y, int width, int height)
			{
				ThemeManager.MainPainter.DrawSizeGrip
					(graphics, backColor, new Rectangle(x, y, width, height));
			}

	// Draw a disabled string.
	public static void DrawStringDisabled
				(Graphics graphics, String s, Font font,
				 Color color, RectangleF layoutRectangle,
				 StringFormat format)
			{
				ThemeManager.MainPainter.DrawStringDisabled
					(graphics, s, font, color, layoutRectangle, format);
			}

	// Draw a filled reversible rectangle.
	public static void FillReversibleRectangle
				(Rectangle rectangle, Color backColor)
			{
				// This is too dangerous: not supported in this implementation.
			}

}; // class ControlPaint

}; // namespace System.Windows.Forms
