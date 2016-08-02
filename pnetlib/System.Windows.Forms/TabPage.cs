/*
 * TabPage.cs - Implementation of the TabPage Control.
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

namespace System.Windows.Forms
{
	using System;
	using System.Drawing;

	public class TabPage : Panel
	{
		private int imageIndex;
		private string toolTipText;
		// The TabPage has independant visibility to the control
		private new bool visible;
		
		public TabPage()
		{
			base.Visible = false; // base is first not visible, TabControl sets it to visible, if TabPage is selected.
			visible = true;
		}

		public TabPage(string text) : this()
		{
			Text = text;
		}

		protected override ControlCollection CreateControlsInstance()
		{
			return new TabPageControlCollection( this );
		}

		// This method is not overridden by Microsoft - but we must.
		protected override void Select(bool directed, bool forward)
		{
			if (Parent.Focused)
				// Move off the tab page onto the first or last child.
				SelectNextControl(this, forward, true, true, true);
			else
			{
				(Parent as TabControl).ActiveControl = Parent;
				// Selecting the Parent will move to the first child.
				// This way we set the focus directly to the parent.
				Parent.Focus();
			}
			return;
		}

		[TODO]
		public static TabPage GetTabPageOfComponent(object comp)
		{
			return null;
		}

		// The size of the tab page is set by the parent
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) 
		{
			if (Parent != null)
			{
				Rectangle fix = (base.Parent as TabControl).DisplayRectangle;
				// Only change the size if we have to.
				if (fix != Bounds)
				{
					base.SetBoundsCore(fix.X, fix.Y, fix.Width, fix.Height, BoundsSpecified.All);
				}
			}
			else
				base.SetBoundsCore(x, y, width, height, specified);
		}
		
		public int ImageIndex {
			get
			{
				return imageIndex;
			}
			set
			{
				imageIndex = value;
				if (base.Parent != null)
					(Parent as TabControl).InvalidateTabs();
			}
		}

		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
				if (base.Parent != null)
					(Parent as TabControl).InvalidateTabs();
			}
		}

		public new bool Visible
		{
			get
			{
				return visible;
			}
			set
			{
				visible = value;
			}
		}


		public string ToolTipText
		{
			get
			{
				return toolTipText;
			}
			set
			{
				toolTipText = value;
			}
		}

		public class TabPageControlCollection : ControlCollection
		{
			public TabPageControlCollection(TabPage owner) : base( owner )
			{}
		}

		public override String ToString()
		{
			return "TabPage: {" + base.Text + "}";
		}
	}
}
