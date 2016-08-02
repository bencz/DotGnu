/*
 * ImageAttributes.cs - Implementation of the
 *			"System.Drawing.Imaging.ImageAttributes" class.
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

namespace System.Drawing.Imaging
{

using System.Drawing.Drawing2D;

public sealed class ImageAttributes : ICloneable, IDisposable
{
	// Attribute information for a particular ColorAdjustType value.
	private sealed class AttributeInfo : ICloneable
	{
		// Accessible state.
		public AttributeInfo next;
		public ColorAdjustType type;
		public Color colorLow;
		public Color colorHigh;
		public ColorMatrix colorMatrix;
		public ColorMatrix grayMatrix;
		public ColorMatrixFlag matrixFlags;
		public float gamma;
		public bool noOp;
		public ColorChannelFlag channelFlags;
		public String profile;
		public ColorMap[] map;
		public float threshold;

		// Constructor.
		public AttributeInfo(AttributeInfo next, ColorAdjustType type)
				{
					this.next = next;
					this.type = type;
				}

		// Clone this object.
		public Object Clone()
				{
					AttributeInfo info = (AttributeInfo)(MemberwiseClone());
					if(next != null)
					{
						info.next = (AttributeInfo)(next.Clone());
					}
					return info;
				}

	}; // class AttributeInfo

	// Internal state.
	private WrapMode mode;
	private Color color;
	private bool clamp;
	private AttributeInfo info;

	// Constructor.
	public ImageAttributes() {}

	// Get the attribute information for a specific ColorAdjustType value.
	private AttributeInfo GetInfo(ColorAdjustType type)
			{
				AttributeInfo current = info;
				while(current != null)
				{
					if(current.type == type)
					{
						return current;
					}
					current = current.next;
				}
				info = new AttributeInfo(info, type);
				return info;
			}

	// Clear the brush remap table.
	public void ClearBrushRemapTable()
			{
				ClearRemapTable(ColorAdjustType.Brush);
			}

	// Clear color keys.
	public void ClearColorKey()
			{
				ClearColorKey(ColorAdjustType.Default);
			}
	public void ClearColorKey(ColorAdjustType type)
			{
				AttributeInfo info = GetInfo(type);
				info.colorLow = Color.Empty;
				info.colorHigh = Color.Empty;
			}

	// Clear color matrices.
	public void ClearColorMatrix()
			{
				ClearColorMatrix(ColorAdjustType.Default);
			}
	public void ClearColorMatrix(ColorAdjustType type)
			{
				AttributeInfo info = GetInfo(type);
				info.colorMatrix = null;
				info.grayMatrix = null;
				info.matrixFlags = ColorMatrixFlag.Default;
			}

	// Disable gamma correction.
	public void ClearGamma()
			{
				ClearGamma(ColorAdjustType.Default);
			}
	public void ClearGamma(ColorAdjustType type)
			{
				GetInfo(type).gamma = 0.0f;
			}

	// Clear the NoOp setting.
	public void ClearNoOp()
			{
				ClearNoOp(ColorAdjustType.Default);
			}
	public void ClearNoOp(ColorAdjustType type)
			{
				GetInfo(type).noOp = false;
			}

	// Clear the output channel setting.
	public void ClearOutputChannel()
			{
				ClearOutputChannel(ColorAdjustType.Default);
			}
	public void ClearOutputChannel(ColorAdjustType type)
			{
				GetInfo(type).channelFlags = ColorChannelFlag.ColorChannelC;
			}

	// Clear the output channel color profile setting.
	public void ClearOutputChannelColorProfile()
			{
				ClearOutputChannelColorProfile(ColorAdjustType.Default);
			}
	public void ClearOutputChannelColorProfile(ColorAdjustType type)
			{
				GetInfo(type).profile = null;
			}

	// Clear the remap table.
	public void ClearRemapTable()
			{
				ClearRemapTable(ColorAdjustType.Default);
			}
	public void ClearRemapTable(ColorAdjustType type)
			{
				GetInfo(type).map = null;
			}

	// Clear the threshold setting.
	public void ClearThreshold()
			{
				ClearThreshold(ColorAdjustType.Default);
			}
	public void ClearThreshold(ColorAdjustType type)
			{
				GetInfo(type).threshold = 0.0f;
			}

	// Clone this object.
	public Object Clone()
			{
				ImageAttributes attrs = (ImageAttributes)(MemberwiseClone());
				if(info != null)
				{
					attrs.info = (AttributeInfo)(info.Clone());
				}
				return attrs;
			}

	// Dispose of this object.
	public void Dispose()
			{
				info = null;
				GC.SuppressFinalize(this);
			}

	// Adjust a palette according to a color adjustment type.
	[TODO]
	public void GetAdjustedPalette(ColorPalette palette, ColorAdjustType type)
			{
				// TODO
			}

	// Set the brush remap table.
	public void SetBrushRemapTable(ColorMap[] map)
			{
				SetRemapTable(map, ColorAdjustType.Brush);
			}

	// Set a color key.
	public void SetColorKey(Color colorLow, Color colorHigh)
			{
				SetColorKey(colorLow, colorHigh, ColorAdjustType.Default);
			}
	public void SetColorKey(Color colorLow, Color colorHigh,
							ColorAdjustType type)
			{
				AttributeInfo info = GetInfo(type);
				info.colorLow = colorLow;
				info.colorHigh = colorHigh;
			}

	// Set color matrices.
	public void SetColorMatrices(ColorMatrix newColorMatrix,
								 ColorMatrix grayMatrix)
			{
				SetColorMatrices(newColorMatrix, grayMatrix,
								 ColorMatrixFlag.Default,
								 ColorAdjustType.Default);
			}
	public void SetColorMatrices(ColorMatrix newColorMatrix,
								 ColorMatrix grayMatrix,
								 ColorMatrixFlag flags)
			{
				SetColorMatrices(newColorMatrix, grayMatrix, flags,
								 ColorAdjustType.Default);
			}
	public void SetColorMatrices(ColorMatrix newColorMatrix,
								 ColorMatrix grayMatrix,
								 ColorMatrixFlag flags,
								 ColorAdjustType type)
			{
				AttributeInfo info = GetInfo(type);
				info.colorMatrix = newColorMatrix;
				info.grayMatrix = grayMatrix;
				info.matrixFlags = flags;
			}

	public void SetColorMatrix(ColorMatrix newColorMatrix)
			{
				SetColorMatrix(newColorMatrix, ColorMatrixFlag.Default);
			}

	public void SetColorMatrix(ColorMatrix newColorMatrix, ColorMatrixFlag flags)
			{
				SetColorMatrix(newColorMatrix, flags, ColorAdjustType.Default);
			}

	public void SetColorMatrix(ColorMatrix newColorMatrix,
	                           ColorMatrixFlag mode, ColorAdjustType type)
			{
				AttributeInfo info = GetInfo(type);
				info.colorMatrix = newColorMatrix;
				info.matrixFlags = mode;
			}

	// Set a gamma setting.
	public void SetGamma(float gamma)
			{
				SetGamma(gamma, ColorAdjustType.Default);
			}
	public void SetGamma(float gamma, ColorAdjustType type)
			{
				GetInfo(type).gamma = gamma;
			}

	// Set the no-operation flag.
	public void SetNoOp()
			{
				SetNoOp(ColorAdjustType.Default);
			}
	public void SetNoOp(ColorAdjustType type)
			{
				GetInfo(type).noOp = true;
			}

	// Set an output channel setting.
	public void SetOutputChannel(ColorChannelFlag flags)
			{
				SetOutputChannel(flags, ColorAdjustType.Default);
			}
	public void SetOutputChannel(ColorChannelFlag flags, ColorAdjustType type)
			{
				GetInfo(type).channelFlags = flags;
			}

	// Set an output channel color profile setting.
	public void SetOutputChannelColorProfile(String colorProfileFilename)
			{
				SetOutputChannelColorProfile(colorProfileFilename,
											 ColorAdjustType.Default);
			}
	public void SetOutputChannelColorProfile(String colorProfileFilename,
											 ColorAdjustType type)
			{
				GetInfo(type).profile = colorProfileFilename;
			}

	// Set a color remap table.
	public void SetRemapTable(ColorMap[] map)
			{
				SetRemapTable(map, ColorAdjustType.Default);
			}
	public void SetRemapTable(ColorMap[] map, ColorAdjustType type)
			{
				GetInfo(type).map = map;
			}

	// Set the threshold setting.
	public void SetThreshold(float threshold)
			{
				SetThreshold(threshold, ColorAdjustType.Default);
			}
	public void SetThreshold(float threshold, ColorAdjustType type)
			{
				GetInfo(type).threshold = threshold;
			}

	// Set the texture wrapping mode.
	public void SetWrapMode(WrapMode mode)
			{
				SetWrapMode(mode, Color.Empty, false);
			}
	public void SetWrapMode(WrapMode mode, Color color)
			{
				SetWrapMode(mode, color, false);
			}
	public void SetWrapMode(WrapMode mode, Color color, bool clamp)
			{
				this.mode = mode;
				this.color = color;
				this.clamp = clamp;
			}

}; // class ImageAttributes

}; // namespace System.Drawing.Imaging
