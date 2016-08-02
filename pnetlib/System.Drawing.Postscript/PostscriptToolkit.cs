/*
 * PostscriptToolkit.cs - Implementation of the
 *			"System.Drawing.Toolkit.PostscriptToolkit" class.
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

using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

internal sealed class PostscriptToolkit : NullToolkit
{
	// Create a solid toolkit brush.
	public override IToolkitBrush CreateSolidBrush(Color color)
			{
				return null;
			}

	// Create a hatched toolkit brush.
	public override IToolkitBrush CreateHatchBrush
				(HatchStyle style, Color foreColor, Color backColor)
			{
				return null;
			}

	// Create a linear gradient brush.  Returns null if the
	// toolkit does not support linear gradient brushes.
	public override IToolkitBrush CreateLinearGradientBrush
				(RectangleF rect, Color color1, Color color2,
				 LinearGradientMode mode)
			{
				return null;
			}
	public override IToolkitBrush CreateLinearGradientBrush
				(RectangleF rect, Color color1, Color color2,
				 float angle, bool isAngleScaleable)
			{
				return null;
			}

	// Create a texture brush.
	public override IToolkitBrush CreateTextureBrush
				(TextureBrush properties, IToolkitImage image,
				 RectangleF dstRect, ImageAttributes imageAttr)
			{
				return null;
			}

	// Create a toolkit pen from the properties in the specified object.
	// If the toolkit does not support the precise combination of pen
	// properties, it will return the closest matching pen.
	public override IToolkitPen CreatePen(Pen pen)
			{
				return new PostscriptPen(pen);
			}

	// Create a toolkit font from the properties in the specified object.
	public override IToolkitFont CreateFont(Font font, float dpi)
			{
				return null;
			}

	// Create a toolkit image from the properties in the specified object.
	public override IToolkitImage CreateImage
				(DotGNU.Images.Image image, int frame)
			{
				return new PostscriptImage(image, frame);
			}

}; // class PostscriptToolkit

}; // namespace System.Drawing.Postscript
