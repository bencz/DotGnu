/*
 * Pen.cs - Implementation of the "System.Drawing.Pen" class.
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

using System.Drawing.Drawing2D;
using System.Drawing.Toolkit;

public sealed class Pen : MarshalByRefObject, ICloneable, IDisposable
{
	// Internal state.
	private Brush brush;
	private Color color;
	private float width;
	private PenAlignment alignment;
	private float[] compoundArray;
	private CustomLineCap customEndCap;
	private CustomLineCap customStartCap;
	private DashCap dashCap;
	private float dashOffset;
	private float[] dashPattern;
	private DashStyle dashStyle;
	private LineCap endCap;
	private LineJoin lineJoin;
	private float miterLimit;
	private LineCap startCap;
	private Matrix transform;
	private IToolkit toolkit;
	private IToolkitPen toolkitPen;

	// Constructors.
	public Pen(Brush brush)
			{
				if(brush == null)
				{
					throw new ArgumentNullException("brush");
				}
				this.brush = brush;
				this.width = 1.0f;
				this.miterLimit = 10.0f;
			}
	public Pen(Color color)
			{
				this.color = color;
				this.width = 1.0f;
				this.miterLimit = 10.0f;
			}
	public Pen(Brush brush, float width)
			{
				if(brush == null)
				{
					throw new ArgumentNullException("brush");
				}
				this.brush = brush;
				this.width = width;
				this.miterLimit = 10.0f;
			}
	public Pen(Color color, float width)
			{
				this.color = color;
				this.width = width;
				this.miterLimit = 10.0f;
			}

	// Destructor.
	~Pen()
			{
				Dispose();
			}

	// Get or set the pen properties.
	public PenAlignment Alignment
			{
				get
				{
					return alignment;
				}
				set
				{
					if(alignment != value)
					{
						Dispose();
						alignment = value;
					}
				}
			}
	public Brush Brush
			{
				get
				{
					if(brush == null)
					{
						brush = new SolidBrush(color);
					}
					return brush;
				}
				set
				{
					if(brush != value)
					{
						Dispose();
						brush = value;
					}
				}
			}
	public Color Color
			{
				get
				{
					if(brush is SolidBrush)
					{
						return ((SolidBrush)brush).Color;
					}
					return color;
				}
				set
				{
					if(color != value)
					{
						Dispose();
						color = value;
						brush = null;
					}
				}
			}
	public float[] CompoundArray
			{
				get
				{
					return compoundArray;
				}
				set
				{
					Dispose();
					compoundArray = value;
				}
			}
	public CustomLineCap CustomEndCap
			{
				get
				{
					return customEndCap;
				}
				set
				{
					Dispose();
					customEndCap = value;
				}
			}
	public CustomLineCap CustomStartCap
			{
				get
				{
					return customStartCap;
				}
				set
				{
					Dispose();
					customStartCap = value;
				}
			}
	public DashCap DashCap
			{
				get
				{
					return dashCap;
				}
				set
				{
					if(dashCap != value)
					{
						Dispose();
						dashCap = value;
					}
				}
			}
	public float DashOffset
			{
				get
				{
					return dashOffset;
				}
				set
				{
					if(dashOffset != value)
					{
						Dispose();
						dashOffset = value;
					}
				}
			}
	public float[] DashPattern
			{
				get
				{
					return dashPattern;
				}
				set
				{
					Dispose();
					dashPattern = value;
				}
			}
	public DashStyle DashStyle
			{
				get
				{
					return dashStyle;
				}
				set
				{
					if(dashStyle != value)
					{
						Dispose();
						dashStyle = value;
					}
				}
			}
	public LineCap EndCap
			{
				get
				{
					return endCap;
				}
				set
				{
					if(endCap != value)
					{
						Dispose();
						endCap = value;
					}
				}
			}
	public LineJoin LineJoin
			{
				get
				{
					return lineJoin;
				}
				set
				{
					if(lineJoin != value)
					{
						Dispose();
						lineJoin = value;
					}
				}
			}
	public float MiterLimit
			{
				get
				{
					return miterLimit;
				}
				set
				{
					if(miterLimit != value)
					{
						Dispose();
						miterLimit = value;
					}
				}
			}
	public PenType PenType
			{
				get
				{
					if(brush == null || brush is SolidBrush)
					{
						return PenType.SolidColor;
					}
					else if(brush is TextureBrush)
					{
						return PenType.TextureFill;
					}
					else if(brush is HatchBrush)
					{
						return PenType.HatchFill;
					}
					else if(brush is PathGradientBrush)
					{
						return PenType.PathGradient;
					}
					else if(brush is LinearGradientBrush)
					{
						return PenType.LinearGradient;
					}
					else
					{
						return PenType.SolidColor;
					}
				}
			}
	public LineCap StartCap
			{
				get
				{
					return startCap;
				}
				set
				{
					if(startCap != value)
					{
						Dispose();
						startCap = value;
					}
				}
			}
	public Matrix Transform
			{
				get
				{
					return transform;
				}
				set
				{
					Dispose();
					if(value != null)
					{
						// Make a copy of the matrix so that modifications
						// to the original don't affect the pen settings.
						transform = Matrix.Clone(value);
					}
					else
					{
						transform = null;
					}
				}
			}
	public float Width
			{
				get
				{
					return width;
				}
				set
				{
					if(width != value)
					{
						Dispose();
						width = value;
					}
				}
			}

	// Clone this pen.
	public Object Clone()
			{
				lock(this)
				{
					Pen pen = (Pen)(MemberwiseClone());
					pen.toolkit = null;
					pen.toolkitPen = null;
					return pen;
				}
			}

	// Dispose of this pen.
	public void Dispose()
			{
				lock(this)
				{
					if(toolkitPen != null)
					{
						toolkitPen.Dispose();
						toolkitPen = null;
					}
					toolkit = null;
					if(brush != null)
					{
						brush.Modified();
					}
				}
			}

	// Set the line capabilities.
	public void SetLineCap(LineCap startCap, LineCap endCap, DashCap dashCap)
			{
				Dispose();
				this.startCap = startCap;
				this.endCap = endCap;
				this.dashCap = dashCap;
			}

	// Get the toolkit version of this pen for a specific toolkit.
	internal IToolkitPen GetPen(IToolkit toolkit)
			{
				lock(this)
				{
					if(this.toolkitPen == null)
					{
						// We don't yet have a toolkit pen yet.
						this.toolkitPen = toolkit.CreatePen(this);
						this.toolkit = toolkit;
						return this.toolkitPen;
					}
					else if(this.toolkit == toolkit)
					{
						// Same toolkit - return the cached pen information.
						return this.toolkitPen;
					}
					else
					{
						// We have a pen for another toolkit,
						// so dispose it and create for this toolkit.
						// We null out "toolkitPen" before calling
						// "CreatePen()" just in case an exception
						// is thrown while creating the toolkit pen.
						this.toolkitPen.Dispose();
						this.toolkitPen = null;
						this.toolkitPen = toolkit.CreatePen(this);
						this.toolkit = toolkit;
						return this.toolkitPen;
					}
				}
			}

}; // class Pen

}; // namespace System.Drawing
