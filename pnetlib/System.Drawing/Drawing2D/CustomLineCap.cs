/*
 * CustomLineCap.cs - Implementation of the
 *			"System.Drawing.Drawing2D.CustomLineCap" class.
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

public class CustomLineCap : MarshalByRefObject, ICloneable, IDisposable
{
	// Internal state.
	private GraphicsPath fillPath;
	private GraphicsPath strokePath;
	private LineCap baseCap;
	private float baseInset;
	private LineJoin strokeJoin;
	private float widthScale;
	private LineCap endCap;
	private LineCap startCap;

	// Constructors.
	public CustomLineCap(GraphicsPath fillPath,
						 GraphicsPath strokePath)
			: this(fillPath, strokePath, LineCap.Flat, 0.0f) {}
	public CustomLineCap(GraphicsPath fillPath,
						 GraphicsPath strokePath,
						 LineCap baseCap)
			: this(fillPath, strokePath, baseCap, 0.0f) {}
	public CustomLineCap(GraphicsPath fillPath,
						 GraphicsPath strokePath,
						 LineCap baseCap, float baseInset)
			{
				this.fillPath = fillPath;
				this.strokePath = strokePath;
				this.baseCap = baseCap;
				this.baseInset = baseInset;
			}

	// Destructor.
	~CustomLineCap()
			{
				Dispose(false);
			}

	// Get or set this object's properties.
	public LineCap BaseCap
			{
				get
				{
					return baseCap;
				}
				set
				{
					baseCap = value;
				}
			}
	public float BaseInset
			{
				get
				{
					return baseInset;
				}
				set
				{
					baseInset = value;
				}
			}
	public LineJoin StrokeJoin
			{
				get
				{
					return strokeJoin;
				}
				set
				{
					strokeJoin = value;
				}
			}
	public float WidthScale
			{
				get
				{
					return widthScale;
				}
				set
				{
					widthScale = value;
				}
			}

	// Clone this object.
	public Object Clone()
			{
				return MemberwiseClone();
			}

	// Dispose of this object.
	public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
	protected virtual void Dispose(bool disposing)
			{
				// Nothing to do here.
			}

	// Get the stroke capabilities.
	public void GetStrokeCaps(out LineCap startCap, out LineCap endCap)
			{
				startCap = this.startCap;
				endCap = this.endCap;
			}

	// Set the stroke capabilities.
	public void SetStrokeCaps(LineCap startCap, LineCap endCap)
			{
				this.startCap = startCap;
				this.endCap = endCap;
			}

}; // class CustomLineCap

}; // namespace System.Drawing.Drawing2D
