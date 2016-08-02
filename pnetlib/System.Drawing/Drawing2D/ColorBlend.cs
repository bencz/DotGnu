/*
 * ColorBlend.cs - Implementation of the "System.Drawing.Drawing2D.ColorBlend" class.
 *
 * Copyright (C) 2003  Neil Cawse.
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
using System;
using System.Drawing;

	public sealed class ColorBlend
	{
		private Color[] colors;
	#if CONFIG_EXTENDED_NUMERICS
		private float[] positions;
	#endif

		public Color[] Colors
		{
			get
			{
				return colors;
			}

			set
			{
				colors = value;
			}
		}

	#if CONFIG_EXTENDED_NUMERICS
		public float[] Positions
		{
			get
			{
				return positions;
			}

			set
			{
				positions = value;
			}
		}
	#endif

		public ColorBlend()
		{
			colors = new Color[1];
	#if CONFIG_EXTENDED_NUMERICS
			positions = new float[1];
	#endif
		}

		public ColorBlend(int count)
		{
			colors = new Color[count];
	#if CONFIG_EXTENDED_NUMERICS
			positions = new float[count];
	#endif
		}
	}

}
