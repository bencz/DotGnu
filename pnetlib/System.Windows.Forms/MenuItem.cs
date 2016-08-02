/*
 * MenuItem.cs - Implementation of the "System.Windows.Forms.MenuItem" class.
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

public class MenuItem : Menu
{
	// Internal state.
	internal Menu parent;
	private ItemFlags flags;
	private int mergeOrder;
	private MenuMerge mergeType;
	private Shortcut shortcut;
	private String text;

	// Flag bits for the menu item's state.
	[Flags]
	private enum ItemFlags
	{
		BarBreak		= 0x0001,
		Break			= 0x0002,
		Checked			= 0x0004,
		DefaultItem		= 0x0008,
		Enabled			= 0x0010,
		MdiList			= 0x0020,
		OwnerDraw		= 0x0040,
		RadioCheck		= 0x0080,
		ShowShortcut	= 0x0100,
		Visible			= 0x0200,
		Default			= Enabled | ShowShortcut | Visible,

	}; // enum ItemFlags

	// Constructors.
	public MenuItem()
			: this(MenuMerge.Add, 0, Shortcut.None, null,
			       null, null, null, null) {}
	public MenuItem(String text)
			: this(MenuMerge.Add, 0, Shortcut.None, text,
				   null, null, null, null) {}
	public MenuItem(String text, EventHandler onClick)
			: this(MenuMerge.Add, 0, Shortcut.None, text,
				   onClick, null, null, null) {}
	public MenuItem(String text, MenuItem[] items)
			: this(MenuMerge.Add, 0, Shortcut.None, text,
				   null, null, null, items) {}
	public MenuItem(String text, EventHandler onClick, Shortcut shortcut)
			: this(MenuMerge.Add, 0, shortcut, text,
				   onClick, null, null, null) {}
	public MenuItem(MenuMerge mergeType, int mergeOrder, Shortcut shortcut,
					String text, EventHandler onClick, EventHandler onPopup,
					EventHandler onSelect, MenuItem[] items)
			: base(items)
			{
				this.flags = ItemFlags.Default;
				this.mergeType = mergeType;
				this.mergeOrder = mergeOrder;
				this.shortcut = shortcut;
				this.text = text;
				if(onClick != null)
				{
					Click += onClick;
				}
				if(onPopup != null)
				{
					Popup += onPopup;
				}
				if(onSelect != null)
				{
					Select += onSelect;
				}
			}

	// Update this menu item after a non-trivial state change.
	[TODO]
	private void UpdateMenuItem()
			{
				return;
			}

	// Get the value of a menu item flag.
	private bool GetFlag(ItemFlags flag)
			{
				return ((flags & flag) != 0);
			}

	// Set the value of a menu item flag.
	private void SetFlag(ItemFlags flag, bool value)
			{
				if(value != ((flags & flag) != 0))
				{
					// The flag has been changed.
					if(value)
					{
						flags |= flag;
					}
					else
					{
						flags &= ~flag;
					}
					UpdateMenuItem();
				}
			}

	// Get or set this item's properties.
	public bool BarBreak
			{
				get
				{
					return GetFlag(ItemFlags.BarBreak);
				}
				set
				{
					SetFlag(ItemFlags.BarBreak, value);
				}
			}
	public bool Break
			{
				get
				{
					return GetFlag(ItemFlags.Break);
				}
				set
				{
					SetFlag(ItemFlags.Break, value);
				}
			}
	public bool Checked
			{
				get
				{
					return GetFlag(ItemFlags.Checked);
				}
				set
				{
					SetFlag(ItemFlags.Checked, value);
				}
			}
	public bool DefaultItem
			{
				get
				{
					return GetFlag(ItemFlags.DefaultItem);
				}
				set
				{
					SetFlag(ItemFlags.DefaultItem, value);
				}
			}
	public bool Enabled
			{
				get
				{
					return GetFlag(ItemFlags.Enabled);
				}
				set
				{
					SetFlag(ItemFlags.Enabled, value);
				}
			}
	public int Index
			{
				get
				{
					if(parent != null)
					{
						return parent.MenuItems.IndexOf(this);
					}
					else
					{
						return -1;
					}
				}
				set
				{
					if(parent != null)
					{
						parent.MenuItems.Move(this, value);
					}
				}
			}
	public override bool IsParent
			{
				get
				{
					if(GetFlag(ItemFlags.MdiList))
					{
						return true;
					}
					else
					{
						return base.IsParent;
					}
				}
			}
	public bool MdiList
			{
				get
				{
					return GetFlag(ItemFlags.MdiList);
				}
				set
				{
					SetFlag(ItemFlags.MdiList, value);
				}
			}
	public int MergeOrder
			{
				get
				{
					return mergeOrder;
				}
				set
				{
					mergeOrder = value;
				}
			}
	public MenuMerge MergeType
			{
				get
				{
					return mergeType;
				}
				set
				{
					mergeType = value;
				}
			}
	public char Mnemonic
			{
				get
				{
					String text = Text;
					if(text != null)
					{
						int index = text.IndexOf('&');
						if(index != -1 && (index + 1) < text.Length)
						{
							return Char.ToUpper(text[index + 1]);
						}
					}
					return '\0';
				}
			}
	public bool OwnerDraw
			{
				get
				{
					return GetFlag(ItemFlags.OwnerDraw);
				}
				set
				{
					SetFlag(ItemFlags.OwnerDraw, value);
				}
			}
	public Menu Parent
			{
				get
				{
					return parent;
				}
			}
	public bool RadioCheck
			{
				get
				{
					return GetFlag(ItemFlags.RadioCheck);
				}
				set
				{
					SetFlag(ItemFlags.RadioCheck, value);
				}
			}
	public Shortcut Shortcut
			{
				get
				{
					return shortcut;
				}
				set
				{
					if(shortcut != value)
					{
						shortcut = value;
						UpdateMenuItem();
					}
				}
			}
	public bool ShowShortcut
			{
				get
				{
					return GetFlag(ItemFlags.ShowShortcut);
				}
				set
				{
					SetFlag(ItemFlags.ShowShortcut, value);
				}
			}
	public String Text
			{
				get
				{
					return text;
				}
				set
				{
					if(text != value)
					{
						text = value;
						UpdateMenuItem();
					}
				}
			}
	public bool Visible
			{
				get
				{
					return GetFlag(ItemFlags.Visible);
				}
				set
				{
					SetFlag(ItemFlags.Visible, value);
				}
			}
	protected int MenuID
			{
				get
				{
					// Menu identifiers are not used, so just return the index.
					return Index;
				}
			}

	[TODO]
	// Create a copy of the current menu item.
	public virtual MenuItem CloneMenu()
			{
				MenuItem clone = new MenuItem();
				clone.text = this.text;
				clone.flags = this.flags;
				clone.mergeOrder = this.mergeOrder;
				clone.mergeType = this.mergeType;
				clone.shortcut = this.shortcut;

				// Fix: Clone all other stuff to be cloned.

				return clone;				
			}

	[TODO]
	protected void CloneMenu(MenuItem itemSrc)
			{
				return;
			}

	[TODO]
	// Merge this menu item with another.
	public virtual MenuItem MergeMenu()
			{
				return this;
			}

	[TODO]
	public void MergeMenu(MenuItem itemSrc)
			{
				return;
			}

	// Generate a "Click" event from this menu item.
	public void PerformClick()
			{
				OnClick(EventArgs.Empty);
			}

	// Generate a "Select" event from this menu item.
	public virtual void PerformSelect()
			{
				OnSelect(EventArgs.Empty);
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return base.ToString() + ", Text: " + text;
			}

	// Event that is raised when this menu item is clicked.
	public event EventHandler Click;

	// Event that is raised when this owner-draw item must be drawn.
	public event DrawItemEventHandler DrawItem;

	// Event that is raised when this owner-draw item must be measured.
	public event MeasureItemEventHandler MeasureItem;

	// Event that is raised just before a menu is popped up.
	public event EventHandler Popup;

	// Event that is raised when the cursor moves over a menu item.
	public event EventHandler Select;

	// Raise the "Click" event.
	protected virtual void OnClick(EventArgs e)
			{
				if(Click != null)
				{
					Click(this, e);
				}
			}

	// Raise the "DrawItem" event.
	protected internal virtual void OnDrawItem(DrawItemEventArgs e)
			{
				if(DrawItem != null)
				{
					DrawItem(this, e);
				}
			}

	// Raise the "MeasureItem" event.
	protected internal virtual void OnMeasureItem(MeasureItemEventArgs e)
			{
				if(MeasureItem != null)
				{
					MeasureItem(this, e);
				}
			}

	// Raise the "Popup" event.
	protected virtual void OnPopup(EventArgs e)
			{
				if(Popup != null)
				{
					Popup(this, e);
				}
			}

	// Raise the "Select" event.
	protected virtual void OnSelect(EventArgs e)
			{
				if(Select != null)
				{
					Select(this, e);
				}
			}

}; // class MenuItem

}; // namespace System.Windows.Forms
