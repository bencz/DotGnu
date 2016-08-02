/*
 * LinearGradientBrush.cs - Implementation of the
 *			"System.Drawing.Drawing2D.LinearGradientBrush" class.
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

using System.Drawing.Toolkit;

// Note: if the underlying toolkit does not have support for gradients,
// then this will create a solid brush with "color1" as its argument.

[TODO]
public sealed class LinearGradientBrush : Brush
{
	// Internal state.
	private RectangleF rect;
	private Color color1;
	private Color color2;
	private LinearGradientMode mode;
	private float angle;
	private bool isAngleScaleable;

	// Constructors.
	public LinearGradientBrush(Point point1, Point point2,
							   Color color1, Color color2)
			{
				this.rect = new RectangleF(point1.X, point1.Y,
										   point2.X - point1.X,
										   point2.Y - point1.Y);
				this.color1 = color1;
				this.color2 = color2;
				this.mode = LinearGradientMode.Horizontal;
				this.angle = 0.0f;
				this.isAngleScaleable = false;
			}
	public LinearGradientBrush(PointF point1, PointF point2,
							   Color color1, Color color2)
			{
				this.rect = new RectangleF(point1.X, point1.Y,
										   point2.X - point1.X,
										   point2.Y - point1.Y);
				this.color1 = color1;
				this.color2 = color2;
				this.mode = LinearGradientMode.Horizontal;
				this.angle = 0.0f;
				this.isAngleScaleable = false;
			}
	public LinearGradientBrush(Rectangle rect, Color color1, Color color2,
							   LinearGradientMode linearGradientMode)
			{
				this.rect = rect;
				this.color1 = color1;
				this.color2 = color2;
				this.mode = linearGradientMode;
				this.angle = 0.0f;
				this.isAngleScaleable = false;
			}
	public LinearGradientBrush(RectangleF rect, Color color1, Color color2,
							   LinearGradientMode linearGradientMode)
			{
				this.rect = rect;
				this.color1 = color1;
				this.color2 = color2;
				this.mode = linearGradientMode;
				this.angle = 0.0f;
				this.isAngleScaleable = false;
			}
	public LinearGradientBrush(Rectangle rect, Color color1,
							   Color color2, float angle)
			{
				this.rect = rect;
				this.color1 = color1;
				this.color2 = color2;
				this.mode = (LinearGradientMode)(-1);
				this.angle = angle;
				this.isAngleScaleable = false;
			}
	public LinearGradientBrush(RectangleF rect, Color color1,
							   Color color2, float angle)
			{
				this.rect = rect;
				this.color1 = color1;
				this.color2 = color2;
				this.mode = (LinearGradientMode)(-1);
				this.angle = angle;
				this.isAngleScaleable = false;
			}
	public LinearGradientBrush(Rectangle rect, Color color1,
							   Color color2, float angle,
							   bool isAngleScaleable)
			{
				this.rect = rect;
				this.color1 = color1;
				this.color2 = color2;
				this.mode = (LinearGradientMode)(-1);
				this.angle = angle;
				this.isAngleScaleable = isAngleScaleable;
			}
	public LinearGradientBrush(RectangleF rect, Color color1,
							   Color color2, float angle,
							   bool isAngleScaleable)
			{
				this.rect = rect;
				this.color1 = color1;
				this.color2 = color2;
				this.mode = (LinearGradientMode)(-1);
				this.angle = angle;
				this.isAngleScaleable = isAngleScaleable;
			}

	// TODO: properties and transform methods

	// Properties
	[TODO]
	public Blend Blend
			{
				get
				{
					throw new NotImplementedException("Blend");
				}
				set
				{
					throw new NotImplementedException("Blend");
				}
			}

	[TODO]
	public bool GammaCorrection
			{
				get
				{
					throw new NotImplementedException("GammaCorrection");
				}
				set
				{
					throw new NotImplementedException("GammaCorrection");
				}
			}

	[TODO]
	public ColorBlend InterpolationColors
			{
				get
				{
					throw new NotImplementedException("InterpolationColors");
				}
				set
				{
					throw new NotImplementedException("InterpolationColors");
				}
			}

	[TODO]
	public Color[] LinearColors
			{
				get
				{
					throw new NotImplementedException("LinearColors");
				}
				set
				{
					throw new NotImplementedException("LinearColors");
				}
			}

	[TODO]
	public RectangleF Rectangle
			{
				get
				{
					throw new NotImplementedException("Rectangle");
				}
			}

	[TODO]
	public Matrix Transform
			{
				get
				{
					throw new NotImplementedException("Transform");
				}
				set
				{
					throw new NotImplementedException("Transform");
				}
			}

	[TODO]
	public WrapMode WrapMode
			{
				get
				{
					throw new NotImplementedException("WrapMode");
				}
				set
				{
					throw new NotImplementedException("WrapMode");
				}
			}

	// Clone this brush.
	public override Object Clone()
			{
				if(mode != (LinearGradientMode)(-1))
				{
					return new LinearGradientBrush
						(rect, color1, color2, mode);
				}
				else
				{
					return new LinearGradientBrush
						(rect, color1, color2, angle, isAngleScaleable);
				}
			}

	// Create this brush for a specific toolkit.  Inner part of "GetBrush()".
	internal override IToolkitBrush CreateBrush(IToolkit toolkit)
			{
				IToolkitBrush brush;
				if(mode != (LinearGradientMode)(-1))
				{
					brush = toolkit.CreateLinearGradientBrush
						(rect, color1, color2, mode);
				}
				else
				{
					brush = toolkit.CreateLinearGradientBrush
						(rect, color1, color2, angle, isAngleScaleable);
				}
				if(brush == null)
				{
					// The toolkit doesn't support gradients, so back
					// off to a solid brush using "color1".
					brush = toolkit.CreateSolidBrush(color1);
				}
				return brush;
			}

	[TODO]
	public void MultiplyTransform(Matrix matrix)
			{
				throw new NotImplementedException("MultiplyTransform");
			}

	[TODO]
	public void MultiplyTransform(Matrix matrix, MatrixOrder order)
			{
				throw new NotImplementedException("MultiplyTransform");
			}

	[TODO]
	public void ResetTransform()
			{
				throw new NotImplementedException("ResetTransform");
			}

	[TODO]
	public void RotateTransform(float angle)
			{
				throw new NotImplementedException("RotateTransform");
			}

	[TODO]
	public void RotateTransform(float angle, MatrixOrder order)
			{
				throw new NotImplementedException("RotateTransform");
			}

	[TODO]
	public void ScaleTransform(float sx, float sy)
			{
				throw new NotImplementedException("ScaleTransform");
			}

	[TODO]
	public void ScaleTransform(float sx, float sy, MatrixOrder order)
			{
				throw new NotImplementedException("ScaleTransform");
			}

	[TODO]
	public void SetBlendTriangularShape(float focus)
			{
				throw new NotImplementedException("SetBlendTriangularShape");
			}

	[TODO]
	public void SetBlendTriangularShape(float focus, float scale)
			{
				throw new NotImplementedException("SetBlendTriangularShape");
			}

	[TODO]
	public void SetSigmaBellShape(float focus)
			{
				throw new NotImplementedException("SetSigmaBellShape");
			}

	[TODO]
	public void SetSigmaBellShape(float focus, float scale)
			{
				throw new NotImplementedException("SetSigmaBellShape");
			}

	[TODO]
	public void TranslateTransform(float dx, float dy)
			{
				throw new NotImplementedException("TranslateTransform");
			}

	[TODO]
	public void TranslateTransform(float dx, float dy, MatrixOrder order)
			{
				throw new NotImplementedException("TranslateTransform");
			}

}; // class LinearGradientBrush

}; // namespace System.Drawing.Drawing2D
