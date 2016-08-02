/*
 * GraphicsPathIterator.cs - Implementation of the
 *			"System.Drawing.Drawing2D.GraphicsPathIterator" class.
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

	public sealed class GraphicsPathIterator : MarshalByRefObject
	{

		[TODO]
		public int Count
		{
			get
			{
				return 0;
			}
		}

		[TODO]
		public int SubpathCount
		{
			get
			{
				return 0;
			}
		}

		[TODO]
		public GraphicsPathIterator(GraphicsPath path)
		{
		}

		[TODO]
		public int NextSubpath(out int startIndex, out int endIndex, out bool isClosed)
		{
			startIndex = 0;
			endIndex = 0;
			isClosed = false;
			return 0;
		}

		[TODO]
		public int NextSubpath(GraphicsPath path, out bool isClosed)
		{
			isClosed = false;
			return 0;
		}

		[TODO]
		public int NextPathType(out byte pathType, out int startIndex, out int endIndex)
		{
			pathType = 0;
			startIndex = 0;
			endIndex = 0;
			return 0;
		}

		[TODO]
		public int NextMarker(out int startIndex, out int endIndex)
		{
			startIndex = 0;
			endIndex = 0;
			return 0;
		}

		[TODO]
		public int NextMarker(GraphicsPath path)
		{
			return 0;
		}

		[TODO]
		public bool HasCurve()
		{
			return false;
		}

		[TODO]
		public void Rewind()
		{
			return;
		}

		[TODO]
		public int Enumerate(ref PointF[] points, ref byte[] types)
		{
			return 0;
		}

		[TODO]
		public int CopyData(ref PointF[] points, ref byte[] types, int startIndex, int endIndex)
		{
			return 0;
		}
	}

}
