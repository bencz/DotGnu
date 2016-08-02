/*
 * PrintPageEventArgs.cs - Implementation of the
 *			"System.Drawing.Printing.PrintPageEventArgs" class.
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

namespace System.Drawing.Printing
{

public class PrintPageEventArgs : EventArgs
{
	// Internal state.
	private bool cancel;
	private bool hasMorePages;
	internal Graphics graphics;
	private Rectangle marginBounds;
	private Rectangle pageBounds;
	private PageSettings pageSettings;

	// Constructor.
	public PrintPageEventArgs(Graphics graphics,
							  Rectangle marginBounds,
							  Rectangle pageBounds,
							  PageSettings pageSettings)
			{
				this.cancel = false;
				this.hasMorePages = false;
				this.graphics = graphics;
				this.marginBounds = marginBounds;
				this.pageBounds = pageBounds;
				this.pageSettings = pageSettings;
			}
	internal PrintPageEventArgs(PageSettings pageSettings)
			{
				this.cancel = false;
				this.hasMorePages = false;
				this.graphics = null;
				this.pageBounds = pageSettings.Bounds;
				Margins margins = pageSettings.Margins;
				this.marginBounds = new Rectangle
					(margins.Left, margins.Top,
					 pageBounds.Width - margins.Left - margins.Right,
					 pageBounds.Height - margins.Top - margins.Bottom);
				this.pageSettings = pageSettings;
			}

	// Event properties.
	public bool Cancel
			{
				get
				{
					return cancel;
				}
				set
				{
					cancel = value;
				}
			}
	public Graphics Graphics
			{
				get
				{
					return graphics;
				}
			}
	public bool HasMorePages
			{
				get
				{
					return hasMorePages;
				}
				set
				{
					hasMorePages = value;
				}
			}
	public Rectangle MarginBounds
			{
				get
				{
					return marginBounds;
				}
			}
	public Rectangle PageBounds
			{
				get
				{
					return pageBounds;
				}
			}
	public PageSettings PageSettings
			{
				get
				{
					return pageSettings;
				}
			}

}; // class PrintPageEventArgs

}; // namespace System.Drawing.Printing
