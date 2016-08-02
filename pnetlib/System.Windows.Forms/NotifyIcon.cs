/*
 * NotifyIcon.cs - Implementation of the
 *			"System.Windows.Forms.NotifyIcon" class.
 *
 * Copyright (C) 2004  Deryk Robosson.
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
	using System;
#if CONFIG_COMPONENT_MODEL
	using System.ComponentModel;
#endif
	using System.Drawing;

	public sealed class NotifyIcon : Component
	{
		private ContextMenu contextMenu;
		private Icon icon;
		private string text;
		private bool visable;
		private IContainer container;

		public event EventHandler Click;
		public event EventHandler DoubleClick;
		public event EventHandler MouseDown;
		public event EventHandler MouseMove;
		public event EventHandler MouseUp;

		public NotifyIcon()
		{
			text = String.Empty;
			visable = false;
		}

		public NotifyIcon(IContainer container) : this()
		{
			container = container;
		}

		// Properties

		public ContextMenu ContextMenu
		{
			get
			{
				return contextMenu;
			}
			set
			{
				if(contextMenu != value)
				{
					contextMenu = value;
				}
			}
		}

		public Icon Icon
		{
			get
			{
				return icon;
			}
			set
			{
				if(icon != value)
				{
					icon = value;
				}
			}
		}

		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
			}
		}

		[TODO]
		public bool Visible
		{
			get
			{
				return visable;
			}
			set
			{
				if(visable != value)
				{
					visable = value;
				}
			}
		}

		// Methods
		[TODO]
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		[TODO]
		internal void OnClick(EventArgs e)
		{
			Click(this, EventArgs.Empty);
		}

		[TODO]
		internal void OnDoubleClick(EventArgs e)
		{
			DoubleClick(this, EventArgs.Empty);
		}

		[TODO]
		internal void OnMouseDown(MouseEventArgs e)
		{
			MouseDown(this, e);
		}

		[TODO]
		internal void OnMouseMove(MouseEventArgs e)
		{
			MouseMove(this, e);
		}

		[TODO]
		internal void OnMouseUp(MouseEventArgs e)
		{
			MouseUp(this, e);
		}
	}
}
