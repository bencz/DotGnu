/*
 * ToolkitPenBase.cs - Implementation of the
 *			"System.Drawing.Toolkit.ToolkitPenBase" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

namespace System.Drawing.Toolkit
{
	using DotGNU.Images;

	[NonStandardExtra]
	public abstract class ToolkitPenBase : IToolkitPen
	{
		private Color color;
		protected internal int width;

		public ToolkitPenBase(Color color, int width)
				{
					this.color = color;
					this.width = width;
				}

		public void Dispose()
				{
					Dispose(true);
					GC.SuppressFinalize(this);
				}

		protected abstract void Dispose(bool disposing);

		~ToolkitPenBase()
			{
				Dispose(false);
			}

		public abstract void Select(IToolkitGraphics graphics);
		public abstract void Select(IToolkitGraphics graphics, IToolkitBrush brush);

		public Color Color
			{
				get
				{
					return color;
				}
			}
	}
}
