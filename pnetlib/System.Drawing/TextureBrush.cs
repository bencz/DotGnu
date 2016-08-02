/*
 * TextureBrush.cs - Implementation of the "System.Drawing.TextureBrush" class.
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

namespace System.Drawing
{

using System.Drawing.Toolkit;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

public sealed class TextureBrush : Brush
{
	// Internal state.
	private Image image;
	private RectangleF dstRect;
	private WrapMode wrapMode;
	private ImageAttributes imageAttr;
	private Matrix transform;

	// Constructors.
	public TextureBrush(Image image)
			{
				if(image == null)
				{
					throw new ArgumentNullException("image");
				}
				this.image = image;
			}
	public TextureBrush(Image image, Rectangle dstRect)
			{
				if(image == null)
				{
					throw new ArgumentNullException("image");
				}
				this.image = image;
				this.dstRect = (RectangleF)dstRect;
			}
	public TextureBrush(Image image, RectangleF dstRect)
			{
				if(image == null)
				{
					throw new ArgumentNullException("image");
				}
				this.image = image;
				this.dstRect = dstRect;
			}
	public TextureBrush(Image image, WrapMode wrapMode)
			{
				if(image == null)
				{
					throw new ArgumentNullException("image");
				}
				this.image = image;
				this.wrapMode = wrapMode;
			}
	public TextureBrush(Image image, Rectangle dstRect,
						ImageAttributes imageAttr)
			{
				if(image == null)
				{
					throw new ArgumentNullException("image");
				}
				this.image = image;
				this.dstRect = (RectangleF)dstRect;
				this.imageAttr = imageAttr;
			}
	public TextureBrush(Image image, RectangleF dstRect,
						ImageAttributes imageAttr)
			{
				if(image == null)
				{
					throw new ArgumentNullException("image");
				}
				this.image = image;
				this.dstRect = dstRect;
				this.imageAttr = imageAttr;
			}
	public TextureBrush(Image image, WrapMode wrapMode,
						Rectangle dstRect)
			{
				if(image == null)
				{
					throw new ArgumentNullException("image");
				}
				this.image = image;
				this.wrapMode = wrapMode;
				this.dstRect = (RectangleF)dstRect;
			}
	public TextureBrush(Image image, WrapMode wrapMode,
						RectangleF dstRect)
			{
				if(image == null)
				{
					throw new ArgumentNullException("image");
				}
				this.image = image;
				this.wrapMode = wrapMode;
				this.dstRect = dstRect;
			}

	// Get the image associated with this texture brush.
	public Image Image
			{
				get
				{
					return image;
				}
			}

	// Get or set the transformation matrix.
	public Matrix Transform
			{
				get
				{
					lock(this)
					{
						if(transform == null)
						{
							transform = new Matrix();
						}
						return transform;
					}
				}
				set
				{
					lock(this)
					{
						if(value == null)
						{
							Modified();
							transform = new Matrix();
						}
						else if(transform == null ||
								!transform.Equals(value))
						{
							Modified();
							transform = new Matrix(value);
						}
					}
				}
			}

	// Get or set the wrap mode.
	public WrapMode WrapMode
			{
				get
				{
					return wrapMode;
				}
				set
				{
					lock(this)
					{
						if(wrapMode != value)
						{
							Modified();
							wrapMode = value;
						}
					}
				}
			}

	// Clone this brush.
	public override Object Clone()
			{
				lock(this)
				{
					TextureBrush brush = (TextureBrush)(MemberwiseClone());
					brush.toolkit = null;
					brush.toolkitBrush = null;
					return brush;
				}
			}

	// Multiply the transformation by an amount.
	public void MultiplyTransform(Matrix matrix)
			{
				MultiplyTransform(matrix, MatrixOrder.Prepend);
			}
	public void MultiplyTransform(Matrix matrix, MatrixOrder order)
			{
				lock(this)
				{
					if(transform == null)
					{
						transform = new Matrix();
					}
					transform.Multiply(matrix, order);
					Modified();
				}
			}

	// Reset the brush transformation.
	public void ResetTransform()
			{
				lock(this)
				{
					transform = new Matrix();
					Modified();
				}
			}

	// Rotate the transformation by an amount.
	public void RotateTransform(float angle)
			{
				RotateTransform(angle, MatrixOrder.Prepend);
			}
	public void RotateTransform(float angle, MatrixOrder order)
			{
				lock(this)
				{
					if(transform == null)
					{
						transform = new Matrix();
					}
					transform.Rotate(angle, order);
					Modified();
				}
			}

	// Scale the transformation by an amount.
	public void ScaleTransform(float sx, float sy)
			{
				ScaleTransform(sx, sy, MatrixOrder.Prepend);
			}
	public void ScaleTransform(float sx, float sy, MatrixOrder order)
			{
				lock(this)
				{
					if(transform == null)
					{
						transform = new Matrix();
					}
					transform.Scale(sx, sy, order);
					Modified();
				}
			}

	// Translate the transformation by an amount.
	public void TranslateTransform(float dx, float dy)
			{
				TranslateTransform(dx, dy, MatrixOrder.Prepend);
			}
	public void TranslateTransform(float dx, float dy, MatrixOrder order)
			{
				lock(this)
				{
					if(transform == null)
					{
						transform = new Matrix();
					}
					transform.Translate(dx, dy, order);
					Modified();
				}
			}

	// Create this brush for a specific toolkit.  Inner part of "GetBrush()".
	internal override IToolkitBrush CreateBrush(IToolkit toolkit)
			{
				if(image.toolkitImage == null)
				{
					image.toolkitImage = toolkit.CreateImage(image.dgImage, 0);
				}
				return toolkit.CreateTextureBrush(this, image.toolkitImage,
												  dstRect, imageAttr);
			}

}; // class TextureBrush

}; // namespace System.Drawing
