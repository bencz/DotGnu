/*
 * PaintEventArgs.cs - Implementation of the
 *			"System.Windows.Forms.PaintEventArgs" class.
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

namespace System.Windows.Forms
{

using System.Drawing;

public class PaintEventArgs : EventArgs
#if !CONFIG_COMPACT_FORMS
	, IDisposable
#endif
{
	// Internal state.
	private Graphics graphics;
	private Rectangle clipRect;

	// Constructor.
	public PaintEventArgs(Graphics graphics, Rectangle clipRect)
			{
				this.graphics = graphics;
				this.clipRect = clipRect;
			}

	// Get the clipping rectangle.
	public Rectangle ClipRectangle
			{
				get
				{
					return clipRect;
				}
			}

	// Get the graphics object that should be used for painting.
	public Graphics Graphics
			{
				get
				{
					return graphics;
				}
			}

#if !CONFIG_COMPACT_FORMS

	// Destructor.
	~PaintEventArgs()
			{
				Dispose(false);
			}

	// Dispose of this object.
	public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
	protected virtual void Dispose(bool disposing)
			{
				if(graphics != null)
				{
					graphics.Dispose();
					graphics = null;
				}
			}

#else // CONFIG_COMPACT_FORMS

	// Dispose of this object.
	internal void Dispose()
			{
				if(graphics != null)
				{
					graphics.Dispose();
					graphics = null;
				}
			}

#endif // CONFIG_COMPACT_FORMS

}; // class PaintEventArgs

}; // namespace System.Windows.Forms
