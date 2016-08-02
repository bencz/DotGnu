/*
 * ContextMenu.cs - Implementation of the
 *			"System.Windows.Forms.ContextMenu" class.
 *
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
using System.Windows.Forms.Themes;
using System.ComponentModel;

#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
[DefaultEvent("Popup")]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
public class ContextMenu : Menu
{
	// Internal state.
	private RightToLeft rightToLeft;
	private Control sourceControl;
	private ContextMenu nextPopupMenu;
	internal PopupControl popupControl;
	private Control associatedControl;
	private int currentMouseItem;

	internal event MouseEventHandler MouseMove;
	internal event MouseEventHandler MouseDown;

	// Constructors.
	public ContextMenu() : base(null)
	{
		rightToLeft = RightToLeft.Inherit;
	}
	public ContextMenu(MenuItem[] items) : base(items)
	{
		rightToLeft = RightToLeft.Inherit;
	}

	// Get or set the right-to-left property.
#if CONFIG_COMPONENT_MODEL
	[Localizable(true)]
#endif // CONFIG_COMPONENT_MODEL
	public virtual RightToLeft RightToLeft
	{
		get
		{
			if(rightToLeft != RightToLeft.Inherit)
			{
				return rightToLeft;
			}
			else if(sourceControl != null)
			{
				return sourceControl.RightToLeft;
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

	// Get the control that owns this context menu.
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif // CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	public Control SourceControl
	{
		get
		{
			return sourceControl;
		}
	}

	protected virtual void OnPopup(EventArgs e)
	{
		if (Popup != null)
		{
			this.Popup(this, e);
		}
	}

	// Show this context menu at the specified control-relative co-ordinates.
	public void Show(Control control, Point pos)
	{
		associatedControl = control;
		currentMouseItem = -1; // Not over anything
		popupControl = new PopupControl();
		// We need the following events from popupControl.
		popupControl.MouseMove +=new MouseEventHandler(OnMouseMove);
		popupControl.MouseDown +=new MouseEventHandler(OnMouseDown);
		popupControl.Paint +=new PaintEventHandler(popupControl_Paint);

		OnPopup(EventArgs.Empty);

		// Figure out where we need to put the popup and its size.
		Point pt = control.PointToScreen(new Point(0,0));
		Rectangle rcWork = Screen.PrimaryScreen.WorkingArea;
		using (Graphics g = popupControl.CreateGraphics())
		{
			Size size = MeasureItemBounds(g);
			size.Height -= 1;
			//align it to control
			if (pt.X < rcWork.Left)
			{
				pt.X += size.Width;
			}
			if (pt.X > rcWork.Right - size.Width)
			{
				pt.X -= size.Width;
			}
			if (pt.Y < rcWork.Top)
			{
				pt.Y += size.Height;
			}
			if (pt.Y > rcWork.Bottom - size.Height)
			{
				pt.Y -= size.Height;
			}
			//add offset pos
			pt.X += pos.X;
			pt.Y += pos.Y;
			//ensure that it is completely visible on screen
			if (pt.X < rcWork.Left)
			{
				pt.X = rcWork.Left;
			}
			if (pt.X > rcWork.Right - size.Width)
			{
				pt.X = rcWork.Right - size.Width;
			}
			if (pt.Y < rcWork.Top)
			{
				pt.Y = rcWork.Top;
			}
			if (pt.Y > rcWork.Bottom + size.Height)
			{
				pt.Y = rcWork.Bottom - size.Height;
			}
			popupControl.Bounds = new Rectangle( pt, size);
		}
		popupControl.Show();
	}

	private void popupControl_Paint(Object sender, PaintEventArgs e)
	{
		Graphics g = e.Graphics;
		Rectangle rect = popupControl.ClientRectangle;
		ThemeManager.MainPainter.DrawButton
			(g, rect.X, rect.Y, rect.Width, rect.Height,
			ButtonState.Normal, SystemColors.MenuText,
			SystemColors.Menu, false);

		// Draw the menu items.
		int count = MenuItems.Count;
		for (int i = 0; i < count; i++)
		{
			DrawMenuItem(g, i, false);
		}
	}

	private void OnMouseDown(Object s, MouseEventArgs e)
	{
		// What item are we over?
		int item = ItemFromPoint(new Point(e.X, e.Y));
		if (item != -1)
		{
			MenuItem menuItem = ItemSelected(item);
			// If there were no sub items, then we need to "PerformClick".
			if (menuItem.MenuItems.Count == 0)
			{
				PopDown();
				menuItem.PerformClick();
			}
			else
			{
				return;
			}
		}
		// Do we need to pass the mouse down along?
		if (MouseDown != null)
		{
			// Convert the mouse co-ordinates relative to the associated control (form).
			MouseDown(this, CreateParentMouseArgs(e));
		}
	}

	private MenuItem ItemSelected(int item)
	{
		MenuItem menuItem = MenuItems[item];
		// Remove any currently "popped up" child menus.
		if (nextPopupMenu != null)
		{
			nextPopupMenu.PopDown();
			nextPopupMenu = null;
		}
		// If there are sub menus then show the next child menu.
		if (menuItem.MenuItems.Count > 0)
		{
			nextPopupMenu = new ContextMenu(menuItem.itemList);
			Point location = new Point(itemBounds[item].Right + 1, itemBounds[item].Y - 1);
			location = popupControl.PointToScreen(location);
			location = associatedControl.PointToClient(location);

			nextPopupMenu.MouseMove +=new MouseEventHandler(nextPopupMenu_MouseMove);
			nextPopupMenu.MouseDown +=new MouseEventHandler(nextPopupMenu_MouseDown);
			nextPopupMenu.Show( associatedControl, location);
		}
		return menuItem;
	}

	protected internal override void ItemSelectTimerTick(object sender, EventArgs e)
	{
		base.ItemSelectTimerTick (sender, e);
		if(currentMouseItem != -1)
		{
			ItemSelected(currentMouseItem);
		}
	}

	private void OnMouseMove(Object s, MouseEventArgs e)
	{
		// What item are we over?
		int newMouseItem = ItemFromPoint(new Point(e.X, e.Y));
		// Dont worry if the mouse is still on the same item.
		if (newMouseItem != currentMouseItem)
		{
			// Draw the menu by un-highlighting the old and highlighting the new.
			using (Graphics g = popupControl.CreateGraphics())
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

			StartTimer(newMouseItem);
		}
		// Do we need to pass the mouse move along?
		if (MouseMove != null)
		{
			// Convert the mouse co-ordinates relative to the associated control (form).
			MouseMove(this, CreateParentMouseArgs(e));
		}
	}

	// Create the correct mouse args relative to our "associated control" ie the form.
	private MouseEventArgs CreateParentMouseArgs(MouseEventArgs e)
	{
		// Convert the mouse co-ordinates relative to the associated control (form).
		Point pt = associatedControl.PointToClient(popupControl.Location);
		pt.X += e.X;
		pt.Y += e.Y;
		return new MouseEventArgs(e.Button, e.Clicks, pt.X, pt.Y, e.Delta);
	}

	// Create the correct mouse args relative to this popup.
	private MouseEventArgs CreateLocalMouseArgs(MouseEventArgs e)
	{
		// Convert the associated control (the form) mouse position to be relative to this ContextMenu.
		Point pt = new Point(e.X, e.Y);
		pt = associatedControl.PointToScreen(pt);
		pt.X -= popupControl.Left;
		pt.Y -= popupControl.Top;
		return new MouseEventArgs(e.Button, e.Clicks, pt.X, pt.Y, e.Delta);
	}

	// The mouse move from our child.
	private void nextPopupMenu_MouseMove(Object sender, MouseEventArgs e)
	{
		OnMouseMove(sender, CreateLocalMouseArgs(e));	
	}

	// The mouse down from our child.
	private void nextPopupMenu_MouseDown(Object sender, MouseEventArgs e)
	{
		OnMouseDown(sender, CreateLocalMouseArgs(e));
	}


	// Calculates the position of each MenuItem,
	// returns the bounds of all MenuItems.
	private Size MeasureItemBounds(Graphics g)
	{
		Size outside = Size.Empty;
		itemBounds = new Rectangle[MenuItems.Count];
		int y = MenuPaddingOrigin.Y;
		for (int i = 0; i < MenuItems.Count; i++)
		{
			MenuItem item = MenuItems[i];
			int width, height;
			if (item.Text == "-")
			{
				height = -1;
				width = 0;
			}
			else
			{
				if (item.OwnerDraw)
				{
					MeasureItemEventArgs measureItem = new MeasureItemEventArgs(g, i);
					item.OnMeasureItem(measureItem);
					width = measureItem.ItemWidth;
					height = measureItem.ItemHeight;
				}
				else
				{
					// Do the default handling
					SizeF size = g.MeasureString(item.Text, SystemInformation.MenuFont);
					width = Size.Truncate(size).Width;
					height = Size.Truncate(size).Height;
				}
			}
			width += ItemPaddingSize.Width;
			Rectangle bounds =  new Rectangle(MenuPaddingOrigin.X, y, 0, height + ItemPaddingSize.Height);
			itemBounds[i] = bounds;
			y += bounds.Height;
			if (outside.Width < width)
			{
				outside.Width = width;
			}
		}
		if (outside.Width < MinimumItemWidth)
		{
			outside.Width = MinimumItemWidth;
		}
		for (int i = 0; i < MenuItems.Count; i++)
		{
			itemBounds[i].Width = outside.Width;
		}
		outside.Height = y + MenuPaddingSize.Height;
		outside.Width += MenuPaddingSize.Width;
		return outside;
	}

	protected int MinimumItemWidth
	{
		get
		{
			return 100;
		}
	}

	// Add this main menu to a control.
	internal void AddToControl(Control control)
	{
		sourceControl = control;
	}

	// Remove this main menu from its owning control.
	internal void RemoveFromControl()
	{
		sourceControl = null;
	}

	// Event that is emitted just before the menu pops up.
	public event EventHandler Popup;

	// Pop down this context menu and its children.
	internal void PopDown()
	{
		if (nextPopupMenu != null)
		{
			nextPopupMenu.PopDown();
			nextPopupMenu = null;
		}
		popupControl.Hide();
	}

}; // class ContextMenu

}; // namespace System.Windows.Forms
