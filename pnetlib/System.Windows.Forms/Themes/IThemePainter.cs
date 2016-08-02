/*
 * IThemePainter.cs - Implementation of the
 *			"System.Windows.Forms.Themes.IThemePainter" class.
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

namespace System.Windows.Forms.Themes
{

using System.Drawing;
using System.Drawing.Drawing2D;

public interface IThemePainter
{
	// Get the width of the specified border type
	int GetBorderWidth(BorderStyle border);

	// Draw a simple button border.
	void DrawBorder(Graphics graphics, Rectangle bounds,
				 	Color color, ButtonBorderStyle style);
	void DrawBorder(Graphics graphics, Rectangle bounds, Color leftColor,
			        int leftWidth, ButtonBorderStyle leftStyle, Color topColor,
				    int topWidth, ButtonBorderStyle topStyle, Color rightColor,
				    int rightWidth, ButtonBorderStyle rightStyle,
				    Color bottomColor, int bottomWidth,
				    ButtonBorderStyle bottomStyle);

	// Draw a 3D border within a rectangle.
	void DrawBorder3D(Graphics graphics, int x, int y,
					  int width, int height,
					  Color foreColor,
					  Color backColor,
					  Border3DStyle style,
					  Border3DSide sides);

	// Draw a button control.
	void DrawButton(Graphics graphics, int x, int y, int width, int height,
				    ButtonState state, Color foreColor, Color backColor,
				    bool isDefault);

	// Draw a caption button control.
	void DrawCaptionButton
				(Graphics graphics, int x, int y, int width, int height,
				 CaptionButton button, ButtonState state);

	// Draw a progress bar control.
	void DrawProgressBar(Graphics graphics, int x, int y,
						 int width, int height, 
						 int steps, int step,
						 int value, bool enabled);

	// Draw a progress bar block.
	void DrawBlock(Graphics graphics, int x, int y,
				  int width, int height, Color color);

	// Draw a check box control.
	void DrawCheckBox(Graphics graphics, int x, int y,
				      int width, int height, ButtonState state);

	// Draw a combo box's drop down button control.
	void DrawComboButton(Graphics graphics, int x, int y,
				         int width, int height, ButtonState state);

	// Draw a container grab handle.
	void DrawContainerGrabHandle(Graphics graphics, Rectangle rectangle);

	// Draw a focus rectangle.
	void DrawFocusRectangle(Graphics graphics, Rectangle rectangle,
				 			Color foreColor, Color backColor);

	// Draw a grab handle.
	void DrawGrabHandle(Graphics graphics, Rectangle rectangle,
				 		bool primary, bool enabled);

	// Draw a group box.
	void DrawGroupBox(Graphics graphics, Rectangle bounds, Color foreColor,
	                  Color backColor, Brush backgroundBrush, bool enabled,
	                  bool entered, FlatStyle style, String text, Font font,
	                  StringFormat format);

	// Draw a grid of dots.
	void DrawGrid(Graphics graphics, Rectangle area,
				  Size pixelsBetweenDots, Color backColor);

	// Draw an image in its disabled state.
	void DrawImageDisabled(Graphics graphics, Image image,
				 		   int x, int y, Color background);

	// Draw a locked selection frame.
	void DrawLockedFrame(Graphics graphics, Rectangle rectangle, bool primary);

	// Draw a menu glyph.
	void DrawMenuGlyph(Graphics graphics, int x, int y, int width,
				 	   int height, MenuGlyph glyph);

	// Draw a three-state check box control.
	void DrawMixedCheckBox(Graphics graphics, int x, int y, int width,
				 		   int height, ButtonState state);

	// Draw a radio button control.
	void DrawRadioButton
				(Graphics graphics, int x, int y, int width, int height,
				 ButtonState state, Color foreColor, Color backColor,
				 Brush backgroundBrush);

	// Draw a scroll bar control.
	void DrawScrollBar
				(Graphics graphics, Rectangle bounds,
				 Rectangle drawBounds,
				 Color foreColor, Color backColor,
				 bool vertical, bool enabled,
				 Rectangle bar, Rectangle track,
				 Rectangle decrement, bool decDown,
				 Rectangle increment, bool incDown);

	// Draw a scroll button control.
	void DrawScrollButton
				(Graphics graphics, int x, int y, int width, int height,
				 ScrollButton button, ButtonState state,
				 Color foreColor, Color backColor);

	// Draw a selection frame.
	void DrawSelectionFrame
				(Graphics graphics, bool active, Rectangle outsideRect,
				 Rectangle insideRect, Color backColor);

	// Draw a size grip.
	void DrawSizeGrip(Graphics graphics, Color backColor, Rectangle drawBounds);

	// Draw a list box.
	void DrawListBox(Graphics graphics,
		int x, int y, int width, int height, bool corner,
		int cornerHeight, int cornerWidth);

	// Draw a disabled string.
	void DrawStringDisabled
				(Graphics graphics, String s, Font font,
			     Color color, RectangleF layoutRectangle,
				 StringFormat format);

	// Draw a menu separator line.
	void DrawSeparator(Graphics graphics, Rectangle rectangle);

	// Draw a scroll bar control.
	void DrawTrackBar(Graphics graphics, Rectangle clientRect,
			Rectangle barRect, Orientation orientation, 
			bool enabled, int ticks, TickStyle style);

}; // class IThemePainter

}; // namespace System.Windows.Forms.Themes
