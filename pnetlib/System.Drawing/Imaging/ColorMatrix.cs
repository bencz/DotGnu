/*
 * ColorMatrix.cs - Implementation of the
 *			"System.Drawing.Imaging.ColorMatrix" class.
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

public sealed class ColorMatrix
{
	// Internal state.
	private float m00, m01, m02, m03, m04;
	private float m10, m11, m12, m13, m14;
	private float m20, m21, m22, m23, m24;
	private float m30, m31, m32, m33, m34;
	private float m40, m41, m42, m43, m44;

	// Constructor.
	public ColorMatrix()
			{
				// Set up the 5x5 identity matrix as the default value.
				m00 = 1.0f; m01 = 0.0f; m02 = 0.0f; m03 = 0.0f; m04 = 0.0f;
				m10 = 0.0f; m11 = 1.0f; m12 = 0.0f; m13 = 0.0f; m14 = 0.0f;
				m20 = 0.0f; m21 = 0.0f; m22 = 1.0f; m23 = 0.0f; m24 = 0.0f;
				m30 = 0.0f; m31 = 0.0f; m32 = 0.0f; m33 = 1.0f; m34 = 0.0f;
				m40 = 0.0f; m41 = 0.0f; m42 = 0.0f; m43 = 0.0f; m44 = 1.0f;
			}
	[CLSCompliant(false)]
	public ColorMatrix(float[][] newColorMatrix)
			{
				m00 = newColorMatrix[0][0];
				m01 = newColorMatrix[0][1];
				m02 = newColorMatrix[0][2];
				m03 = newColorMatrix[0][3];
				m04 = newColorMatrix[0][4];
				m10 = newColorMatrix[1][0];
				m11 = newColorMatrix[1][1];
				m12 = newColorMatrix[1][2];
				m13 = newColorMatrix[1][3];
				m14 = newColorMatrix[1][4];
				m20 = newColorMatrix[2][0];
				m21 = newColorMatrix[2][1];
				m22 = newColorMatrix[2][2];
				m23 = newColorMatrix[2][3];
				m24 = newColorMatrix[2][4];
				m30 = newColorMatrix[3][0];
				m31 = newColorMatrix[3][1];
				m32 = newColorMatrix[3][2];
				m33 = newColorMatrix[3][3];
				m34 = newColorMatrix[3][4];
				m40 = newColorMatrix[4][0];
				m41 = newColorMatrix[4][1];
				m42 = newColorMatrix[4][2];
				m43 = newColorMatrix[4][3];
				m44 = newColorMatrix[4][4];
			}

	// Get or set this object's properties.
	public float this[int row, int column]
			{
				get
				{
					switch(row * 5 + column)
					{
						case 0 * 5 + 0:		return m00;
						case 0 * 5 + 1:		return m01;
						case 0 * 5 + 2:		return m02;
						case 0 * 5 + 3:		return m03;
						case 0 * 5 + 4:		return m04;
						case 1 * 5 + 0:		return m10;
						case 1 * 5 + 1:		return m11;
						case 1 * 5 + 2:		return m12;
						case 1 * 5 + 3:		return m13;
						case 1 * 5 + 4:		return m14;
						case 2 * 5 + 0:		return m20;
						case 2 * 5 + 1:		return m21;
						case 2 * 5 + 2:		return m22;
						case 2 * 5 + 3:		return m23;
						case 2 * 5 + 4:		return m24;
						case 3 * 5 + 0:		return m30;
						case 3 * 5 + 1:		return m31;
						case 3 * 5 + 2:		return m32;
						case 3 * 5 + 3:		return m33;
						case 3 * 5 + 4:		return m34;
						case 4 * 5 + 0:		return m40;
						case 4 * 5 + 1:		return m41;
						case 4 * 5 + 2:		return m42;
						case 4 * 5 + 3:		return m43;
						case 4 * 5 + 4:		return m44;
						default:			return 0.0f;
					}
				}
				set
				{
					switch(row * 5 + column)
					{
						case 0 * 5 + 0:		m00 = value; break;
						case 0 * 5 + 1:		m01 = value; break;
						case 0 * 5 + 2:		m02 = value; break;
						case 0 * 5 + 3:		m03 = value; break;
						case 0 * 5 + 4:		m04 = value; break;
						case 1 * 5 + 0:		m10 = value; break;
						case 1 * 5 + 1:		m11 = value; break;
						case 1 * 5 + 2:		m12 = value; break;
						case 1 * 5 + 3:		m13 = value; break;
						case 1 * 5 + 4:		m14 = value; break;
						case 2 * 5 + 0:		m20 = value; break;
						case 2 * 5 + 1:		m21 = value; break;
						case 2 * 5 + 2:		m22 = value; break;
						case 2 * 5 + 3:		m23 = value; break;
						case 2 * 5 + 4:		m24 = value; break;
						case 3 * 5 + 0:		m30 = value; break;
						case 3 * 5 + 1:		m31 = value; break;
						case 3 * 5 + 2:		m32 = value; break;
						case 3 * 5 + 3:		m33 = value; break;
						case 3 * 5 + 4:		m34 = value; break;
						case 4 * 5 + 0:		m40 = value; break;
						case 4 * 5 + 1:		m41 = value; break;
						case 4 * 5 + 2:		m42 = value; break;
						case 4 * 5 + 3:		m43 = value; break;
						case 4 * 5 + 4:		m44 = value; break;
					}
				}
			}
	public float Matrix00
			{
				get
				{
					return m00;
				}
				set
				{
					m00 = value;
				}
			}
	public float Matrix01
			{
				get
				{
					return m01;
				}
				set
				{
					m01 = value;
				}
			}
	public float Matrix02
			{
				get
				{
					return m02;
				}
				set
				{
					m02 = value;
				}
			}
	public float Matrix03
			{
				get
				{
					return m03;
				}
				set
				{
					m03 = value;
				}
			}
	public float Matrix04
			{
				get
				{
					return m04;
				}
				set
				{
					m04 = value;
				}
			}
	public float Matrix10
			{
				get
				{
					return m10;
				}
				set
				{
					m10 = value;
				}
			}
	public float Matrix11
			{
				get
				{
					return m11;
				}
				set
				{
					m11 = value;
				}
			}
	public float Matrix12
			{
				get
				{
					return m12;
				}
				set
				{
					m12 = value;
				}
			}
	public float Matrix13
			{
				get
				{
					return m13;
				}
				set
				{
					m13 = value;
				}
			}
	public float Matrix14
			{
				get
				{
					return m14;
				}
				set
				{
					m14 = value;
				}
			}
	public float Matrix20
			{
				get
				{
					return m20;
				}
				set
				{
					m20 = value;
				}
			}
	public float Matrix21
			{
				get
				{
					return m21;
				}
				set
				{
					m21 = value;
				}
			}
	public float Matrix22
			{
				get
				{
					return m22;
				}
				set
				{
					m22 = value;
				}
			}
	public float Matrix23
			{
				get
				{
					return m23;
				}
				set
				{
					m23 = value;
				}
			}
	public float Matrix24
			{
				get
				{
					return m24;
				}
				set
				{
					m24 = value;
				}
			}
	public float Matrix30
			{
				get
				{
					return m30;
				}
				set
				{
					m30 = value;
				}
			}
	public float Matrix31
			{
				get
				{
					return m31;
				}
				set
				{
					m31 = value;
				}
			}
	public float Matrix32
			{
				get
				{
					return m32;
				}
				set
				{
					m32 = value;
				}
			}
	public float Matrix33
			{
				get
				{
					return m33;
				}
				set
				{
					m33 = value;
				}
			}
	public float Matrix34
			{
				get
				{
					return m34;
				}
				set
				{
					m34 = value;
				}
			}
	public float Matrix40
			{
				get
				{
					return m40;
				}
				set
				{
					m40 = value;
				}
			}
	public float Matrix41
			{
				get
				{
					return m41;
				}
				set
				{
					m41 = value;
				}
			}
	public float Matrix42
			{
				get
				{
					return m42;
				}
				set
				{
					m42 = value;
				}
			}
	public float Matrix43
			{
				get
				{
					return m43;
				}
				set
				{
					m43 = value;
				}
			}
	public float Matrix44
			{
				get
				{
					return m44;
				}
				set
				{
					m44 = value;
				}
			}

}; // class ColorMatrix

}; // namespace System.Drawing.Imaging
