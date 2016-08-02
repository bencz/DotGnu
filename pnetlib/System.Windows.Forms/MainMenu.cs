/*
 * MainMenu.cs - Implementation of the
 *			"System.Windows.Forms.MainMenu" class.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2004  Neil Cawse.
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

	public class MainMenu : Menu
	{
		// Internal state.
		private RightToLeft rightToLeft;
		internal Form ownerForm;
		private ContextMenu menuPopup;
		private int currentMouseItem = -1;
		// Has the menu been clicked.
		private bool clicked = false;

		// Constructors.
		public MainMenu() : base(null)
		{
			rightToLeft = RightToLeft.Inherit;
		}
		public MainMenu(MenuItem[] items) : base(items)
		{
			rightToLeft = RightToLeft.Inherit;
		}

		// Get or set the right-to-left property.
		public virtual RightToLeft RightToLeft
		{
			get
			{
				if(rightToLeft != RightToLeft.Inherit)
				{
					return rightToLeft;
				}
				else if(ownerForm != null)
				{
					return ownerForm.RightToLeft;
				}
				else
				{
					return RightToLeft.No;
				}
			}
			set
			{
				if(rightToLeft != value)
				{
					SuppressUpdates();
					rightToLeft = value;
					AllowUpdates();
				}
			}
		}

		[TODO]
		// Clone this main menu.
		public virtual MainMenu CloneMenu()
		{
			return this;
		}

		// Get the form that owns this menu.
		public Form GetForm()
		{
			return ownerForm;
		}

		// Convert this object into a string.
		public override String ToString()
		{
			if(ownerForm != null)
			{
				return base.ToString() + ", GetForm: " +
					ownerForm.ToString();
			}
			else
			{
				return base.ToString();
			}
		}

#if CONFIG_COMPONENT_MODEL

	// Dispose of this menu.
	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

#endif

		protected internal override void RepaintAndRecalc()
		{
			itemBounds = null;
			if(ownerForm != null)
			{
				OnPaint();
			}
		}

		// Add this main menu to a form.
		internal void AddToForm(Form form)
		{
			ownerForm = form;
			if (!ownerForm.IsHandleCreated)
			{
				return;
			}
			// Cause the form to reposition its controls 
			// now that the client area has changed
			form.PerformLayout();
		}

		// Remove this main menu from its owning form.
		internal void RemoveFromForm()
		{
			// Cause the form to reposition its controls 
			// now that the client area has changed
			ownerForm.PerformLayout();
			ownerForm = null;
		}

		internal void OnPaint()
		{
			using (Graphics g = ownerForm.CreateNonClientGraphics())
			{
				int i = 0;

				// Measure the menus if they need to be.
				if (itemBounds == null)
				{
					MeasureItemBounds(g);
				}
				for (i = 0; i < MenuItems.Count; i++)
				{
					DrawMenuItem(g, i, false);
				}
				// Fill in the rest of the menu, or the whole lot if no items
				if(i > 0)
				{
					i--;
					g.FillRectangle(SystemBrushes.Menu, new Rectangle(
								itemBounds[i].X + itemBounds[i].Width, 0,
								ownerForm.Width - MenuPaddingSize.Width,
								SystemInformation.MenuHeight));
				}
				else
				{
					g.FillRectangle(SystemBrushes.Menu, new Rectangle
							(MenuPaddingOrigin.X, MenuPaddingOrigin.Y,
							 ownerForm.Width - MenuPaddingSize.Width,
							 ownerForm.Height - MenuPaddingSize.Height));
				}
			}
		}

		// Calculates the position of each MenuItem.
		private void MeasureItemBounds(Graphics g)
		{
			// get the menu items
			MenuItemCollection menuItems = MenuItems;

			// get the padding origin
			Point padOrigin = MenuPaddingOrigin;

			// get the padding size
			Size padSize = MenuPaddingSize;

			// get the x coordinate from the padding origin
			int x = padOrigin.X;

			// get the bounds of the menu
			Rectangle menuBounds = new Rectangle
				(x, padOrigin.Y,
				 ownerForm.Width - padSize.Width,
				 ownerForm.Height - padSize.Height);

			// create the item bounds array
			itemBounds = new Rectangle[menuItems.Count];

			// get system menu information
			int sysMenuHeight = SystemInformation.MenuHeight;
			Font sysMenuFont = SystemInformation.MenuFont;

			// get the item padding width
			int itemPadWidth = ItemPaddingSize.Width;

			// calculate the bounds for each menu item
			int i = 0;
			foreach(MenuItem item in menuItems)
			{
				int width;
				if (item.OwnerDraw)
				{
					MeasureItemEventArgs measureItem = new MeasureItemEventArgs
						(g, i, sysMenuHeight);
					item.OnMeasureItem(measureItem);
					width = measureItem.ItemWidth;
				}
				else
				{
					// Do the default handling
					SizeF size = g.MeasureString(item.Text, sysMenuFont);
					width = Size.Truncate(size).Width;
				}
				Rectangle bounds =  new Rectangle
					(x, 0, width + itemPadWidth, sysMenuHeight);
				bounds.IntersectsWith(menuBounds);
				itemBounds[i++] = bounds;
				x += bounds.Width;
			}
		}

		private void OnMouseDownMenus(object sender, MouseEventArgs e)
		{
			OnMouseDown(e);
		}

		internal void OnMouseDown( MouseEventArgs e)
		{
			// Get the mouse coordinate relative to the control
			int y = e.Y + SystemInformation.MenuHeight;
			// Search the main menu
			int item = ItemFromPoint(new Point(e.X, y));
			if (item != -1)
			{
				MenuItem menuItem = ItemSelected(item);
				if (menuItem.MenuItems.Count == 0)
				{
					clicked = false;
					menuItem.PerformClick();
				}
				else
				{
					clicked = true;
				}
			}
			else
			{
				// We have clicked outside of any menus that are up so pop down all menus.
				if (menuPopup != null)
				{
					menuPopup.PopDown();
					clicked = false;
				}
			}
		}

		private MenuItem ItemSelected(int item)
		{
			// Remove any exisiting menus.
			if (menuPopup != null)
			{
				menuPopup.PopDown();
				menuPopup = null;
			}

			MenuItem menuItem = MenuItems[item];
			if (menuItem.MenuItems.Count > 0)
			{
				menuPopup = new ContextMenu(menuItem.itemList);
				Point pt = new Point(itemBounds[item].Left, 0);
				menuPopup.MouseMove +=new MouseEventHandler(OnMouseMoveMenus);
				menuPopup.MouseDown +=new MouseEventHandler(OnMouseDownMenus);
				menuPopup.Show(ownerForm, pt);
			}
			return menuItem;
		}

		private void OnMouseMoveMenus(object sender, MouseEventArgs e)
		{
			OnMouseMove(e);
		}
		
		internal void OnMouseMove(MouseEventArgs e)
		{
			// Get the mouse coordinate relative to the control.
			int y = e.Y + SystemInformation.MenuHeight;
			int newMouseItem = ItemFromPoint(new Point(e.X, y));
			// Dont worry if the mouse is still on the same item.
			if (newMouseItem == currentMouseItem)
			{
				return;
			}
			// Draw by removing the previous highlight and drawing the new one.
			using (Graphics g = ownerForm.CreateNonClientGraphics())
			{
				if (currentMouseItem != -1)
				{
					DrawMenuItem(g, currentMouseItem, false);
				}
				if (newMouseItem != -1)
				{
					DrawMenuItem(g, newMouseItem, true);
				}
			}
			currentMouseItem = newMouseItem;
			if (clicked)
			{
				StartTimer(newMouseItem);
			}
		}

		protected internal override void ItemSelectTimerTick(object sender, EventArgs e)
		{
			base.ItemSelectTimerTick (sender, e);
			if(currentMouseItem != -1)
			{
				ItemSelected(currentMouseItem);
			}
		}

		internal void OnMouseLeave()
		{
			if (currentMouseItem == -1)
			{
				return;
			}
			using (Graphics g = ownerForm.CreateNonClientGraphics())
			{
				DrawMenuItem(g, currentMouseItem, false);
			}
			currentMouseItem = -1;
		}
	
}; // class MainMenu

}; // namespace System.Windows.Forms
