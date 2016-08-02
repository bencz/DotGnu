/*
 * ListView.cs - Implementation of the
 *			"System.Windows.Forms.ListView" class.
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

namespace System.Windows.Forms
{
using System;
using System.Drawing;
using System.Collections;

	public class ListView : Control
	{
		private ItemActivation activation;
		private ListViewAlignment alignStyle;
		private BorderStyle borderStyle;
		private ColumnHeaderStyle headerStyle;
		private SortOrder sorting;
		private View viewStyle;
		private bool allowColumnReorder;
		private bool autoArrange;
		private bool checkBoxes;
		private bool fullRowSelect;
		private bool gridLines;
		private bool hideSelection;
		private bool labelEdit;
		private bool labelWrap;
		private bool multiSelect;
		private bool scrollable;
		private bool hoverSelection;
		private ListViewItemCollection items;
		private ColumnHeaderCollection columns;
		private CheckedIndexCollection checkedIndices;
		private CheckedListViewItemCollection checkedItems;
		private SelectedListViewItemCollection selectedItems;
		internal ColumnHeader[] columnHeaders;
		internal ArrayList listItems;
		private SelectedIndexCollection selectedIndices;
		private IComparer listViewItemSorter;
		private ImageList largeImageList;
		private ImageList smallImageList;
		private ImageList stateImageList;
		private int updating;
		private bool inLabelEdit;

		public event LabelEditEventHandler AfterLabelEdit;
		public event LabelEditEventHandler BeforeLabelEdit;
		public event ColumnClickEventHandler ColumnClick;
		public event EventHandler ItemActivate;
		public event ItemCheckEventHandler ItemCheck;
		public event ItemDragEventHandler ItemDrag;
		
		public ListView()
		{
			items = new ListViewItemCollection(this);
			columns = new ColumnHeaderCollection(this);
			listItems = new ArrayList();
			autoArrange = true;
			hideSelection = true;
			labelWrap = true;
			multiSelect = true;
			scrollable = true;
			activation = ItemActivation.Standard;
			alignStyle = ListViewAlignment.Top;
			borderStyle = BorderStyle.Fixed3D;
			headerStyle = ColumnHeaderStyle.Clickable;
			sorting = SortOrder.None;
			viewStyle = View.LargeIcon;
		}

		[TODO]
		public ItemActivation Activation
		{
			get
			{
				return activation;
			}

			set
			{
				if (activation != value)
				{
					activation = value;
				}
			}
		}

		[TODO]
		public ListViewAlignment Alignment
		{
			get
			{
				return alignStyle;
			}

			set
			{
				if (alignStyle != value)
				{
					alignStyle = value;
				}
			}
		}

		[TODO]
		public bool AllowColumnReorder
		{
			get
			{
				return allowColumnReorder;
			}

			set
			{
				if (allowColumnReorder != value)
				{
					allowColumnReorder = value;
				}
			}
		}

		[TODO]
		public bool AutoArrange
		{
			get
			{
				return autoArrange;
			}

			set
			{
				if (value != autoArrange)
				{
					autoArrange = value;
				}
			}
		}

		[TODO]
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}

			set
			{
				base.BackColor = value;
			}
		}

		public override Image BackgroundImage
		{
			get
			{
				return base.BackgroundImage;
			}

			set
			{
				base.BackgroundImage = value;
			}
		}

		internal void BeginEdit(ListViewItem item)
		{
		}

		[TODO]
		public BorderStyle BorderStyle
		{
			get
			{
				return borderStyle;
			}

			set
			{
				if (borderStyle != value)
				{
					borderStyle = value;
				}
			}
		}

		[TODO]
		public bool CheckBoxes
		{
			get
			{
				return checkBoxes;
			}

			set
			{
				if (checkBoxes != value)
				{
					checkBoxes = value;
				}
			}
		}

		public CheckedIndexCollection CheckedIndices
		{
			get
			{
				if (checkedIndices == null)
				{
					checkedIndices = new CheckedIndexCollection(this);
				}
				return checkedIndices;
			}
		}

		public CheckedListViewItemCollection CheckedItems
		{
			get
			{
				if (checkedItems == null)
				{
					checkedItems = new CheckedListViewItemCollection(this);
				}
				return checkedItems;
			}
		}

		public ColumnHeaderCollection Columns
		{
			get
			{
				return columns;
			}
		}

		protected override CreateParams CreateParams
		{
			get
			{
				return base.CreateParams;
			}
		}

		[TODO]
		public ListViewItem FocusedItem
		{
			get
			{
				return null;
			}
		}

		[TODO]
		internal ListViewItem FocusedItemInternal
		{
			set
			{

			}
		}

		protected override Size DefaultSize
		{
			get
			{
				return new Size(121, 97);
			}
		}

		[TODO]
		public override Color ForeColor
		{
			get
			{
				return base.ForeColor;
			}

			set
			{
				base.ForeColor = value;
			}
		}

		[TODO]
		public bool FullRowSelect
		{
			get
			{
				return fullRowSelect;
			}

			set
			{
				if (fullRowSelect != value)
				{
					fullRowSelect = value;
				}
			}
		}

		[TODO]
		public ColumnHeaderStyle HeaderStyle
		{
			get
			{
				return headerStyle;
			}

			set
			{
				if (headerStyle != value)
				{
					headerStyle = value;
				}
			}
		}

		[TODO]
		public bool GridLines
		{
			get
			{
				return gridLines;
			}

			set
			{
				if (gridLines != value)
				{
					gridLines = value;
				}
			}
		}

		[TODO]
		public bool HideSelection
		{
			get
			{
				return hideSelection;
			}

			set
			{
				if (value != hideSelection)
				{
					hideSelection = value;
				}
			}
		}

		[TODO]
		public bool HoverSelection
		{
			get
			{
				return hoverSelection;
			}

			set
			{
				if (hoverSelection != value)
				{
					hoverSelection = value;
				}
			}
		}

		[TODO]
		public bool LabelEdit
		{
			get
			{
				return labelEdit;
			}

			set
			{
				if (value != labelEdit)
				{
					labelEdit = value;
				}
			}
		}

		[TODO]
		public bool LabelWrap
		{
			get
			{
				return labelWrap;
			}

			set
			{
				if (value != labelWrap)
				{
					labelWrap = value;
				}
			}
		}

		[TODO]
		public ImageList LargeImageList
		{
			get
			{
				return largeImageList;
			}

			set
			{
				if (value != largeImageList)
				{
					if (largeImageList != null)
					{
						largeImageList.Dispose();
					}
					largeImageList = value;
				}
		}
		}

		public ListViewItemCollection Items
		{
			get
			{
				return items;
			}
		}

		public IComparer ListViewItemSorter
		{
			get
			{
				return listViewItemSorter;
			}

			set
			{
				if (listViewItemSorter != value)
				{
					listViewItemSorter = value;
					Sort();
				}
			}
		}

		[TODO]
		public bool MultiSelect
		{
			get
			{
				return multiSelect;
			}

			set
			{
				if (value != multiSelect)
				{
					multiSelect = value;
				}
			}
		}

		[TODO]
		public bool Scrollable
		{
			get
			{
				return scrollable;
			}

			set
			{
				if (value != scrollable)
				{
					scrollable = value;
				}
			}
		}

		public SelectedIndexCollection SelectedIndices
		{
			get
			{
				if (selectedIndices == null)
				{
					selectedIndices = new SelectedIndexCollection(this);
				}
				return selectedIndices;
			}
		}

		public SelectedListViewItemCollection SelectedItems
		{
			get
			{
				if (selectedItems == null)
				{
					selectedItems = new SelectedListViewItemCollection(this);
				}
				return selectedItems;
			}
		}

		[TODO]
		public ImageList SmallImageList
		{
			get
			{
				return smallImageList;
			}

			set
			{
				if (value != smallImageList)
				{
					if (smallImageList != null)
					{
						smallImageList.Dispose();
					}
					smallImageList = value;
				}
			}
		}

		[TODO]
		public SortOrder Sorting
		{
			get
			{
				return sorting;
			}

			set
			{
				if (sorting != value)
				{
					sorting = value;
				}
			}
		}

		[TODO]
		public ImageList StateImageList
		{
			get
			{
				return null;
			}

			set
			{
				if (stateImageList == value)
				{
					return;
				}
				if (stateImageList != null)
				{
					stateImageList.Dispose();
				}
				stateImageList = value;
			}
		}

		[TODO]
		public override string Text
		{
			get
			{
				return base.Text;
			}

			set
			{
				base.Text = value;
			}
		}

		[TODO]
		public ListViewItem TopItem
		{
			get
			{
				return null;
			}
		}

		[TODO]
		public View View
		{
			get
			{
				return viewStyle;
			}

			set
			{
				if (viewStyle != value)
				{
					viewStyle = value;
				}
			}
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			base.OnEnabledChanged(e);
		}

		[TODO]
		public void ArrangeIcons(ListViewAlignment value)
		{
			if (viewStyle == View.Details)
			{
				return; 
			}
			if (sorting != SortOrder.None)
			{
				Sort();
			}
		}

		public void ArrangeIcons()
		{
			ArrangeIcons(ListViewAlignment.Default);
		}

		public void BeginUpdate()
		{
			updating++;
		}

		public void Clear()
		{
			Items.Clear();
			Columns.Clear();
		}

		protected override void CreateHandle()
		{
			base.CreateHandle();
		}

		protected override void Dispose(bool disposing)
		{
			if (columnHeaders != null)
			{
				// More efficient to remove from the back.
				for (int i = columnHeaders.Length - 1; i >= 0; i--)
				{
					columnHeaders[i].Dispose();
				}
				columnHeaders = null;
			}
			base.Dispose(disposing);
		}

		public void EndUpdate()
		{
			if (--updating <= 0)
			{
				Invalidate();
			}
		}

		[TODO]
		public void EnsureVisible(int index)
		{
		}

		[TODO]
		public ListViewItem GetItemAt(int x, int y)
		{
			return null;
		}

		public Rectangle GetItemRect(int index)
		{
			return GetItemRect(index, ItemBoundsPortion.Entire);
		}

		[TODO]
		public Rectangle GetItemRect(int index, ItemBoundsPortion portion)
		{
			return Rectangle.Empty;
		}

		protected override bool IsInputKey(Keys keyData)
		{
			if ((keyData & Keys.Alt) != 0)
			{
				return false;
			}
			Keys key = keyData & Keys.KeyCode;
			if (key == Keys.Prior || key == Keys.Next || key == Keys.Home || key == Keys.End || base.IsInputKey(keyData))
			{
				return true;
			}
			if (inLabelEdit)
			{
				if (key == Keys.Return || key == Keys.Escape)
				{
					return true;
				}
			}
			return false;
		}

		protected virtual void OnAfterLabelEdit(LabelEditEventArgs e)
		{
			if (AfterLabelEdit != null)
			{
				AfterLabelEdit(this, e);
			}
		}

		protected virtual void OnBeforeLabelEdit(LabelEditEventArgs e)
		{
			if (BeforeLabelEdit != null)
			{
				BeforeLabelEdit(this, e);
			}
		}

		protected virtual void OnColumnClick(ColumnClickEventArgs e)
		{
			if (ColumnClick != null)
			{
				ColumnClick(this, e);
			}
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			Invalidate();
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			base.OnHandleDestroyed(e);
		}

		protected virtual void OnItemActivate(EventArgs e)
		{
			if (ItemActivate != null)
			{
				ItemActivate(this, e);
			}
		}

		protected virtual void OnItemCheck(ItemCheckEventArgs ice)
		{
			if (ItemCheck != null)
			{
				ItemCheck(this, ice);
			}
		}

		protected virtual void OnItemDrag(ItemDragEventArgs e)
		{
			if (ItemDrag != null)
			{
				ItemDrag(this, e);
			}
		}

		protected virtual void OnSelectedIndexChanged(EventArgs e)
		{
			EventHandler handler = GetHandler(EventId.SelectedIndexChanged) as EventHandler;
			if(handler != null)
			{
				handler(this, e);
			}
		}

		protected override void OnSystemColorsChanged(EventArgs e)
		{
			base.OnSystemColorsChanged(e);
		}

		protected void RealizeProperties()
		{
			// Not required in this implementation.
		}

		[TODO]
		public void Sort()
		{
		}

		public override string ToString()
		{
			String s = base.ToString();
			if (listItems != null)
			{
				s += ", Count: " + listItems.Count;
				if (listItems.Count > 0)
				{
					String s1 = ", Items[0]: " + listItems[0];
					if (s1.Length > 50)
					{
						s1 = s1.Substring(0, 50);
					}
					s += s1;
				}
			}
			return s;
		}

		protected void UpdateExtendedStyles()
		{
		}
#if !CONFIG_COMPACT_FORMS
		protected override void WndProc(ref Message m)
		{
		}
#endif

		public event EventHandler SelectedIndexChanged
		{
			add
			{
				AddHandler(EventId.SelectedIndexChanged, value);
			}
			remove
			{
				RemoveHandler(EventId.SelectedIndexChanged, value);
			}
		}

		public class CheckedIndexCollection: IList
		{
			private ListView owner;

			public virtual int Count
			{
				get
				{
					if (!owner.CheckBoxes)
					{
						return 0;
					}
					int count = 0;
					for (int i = 0; i < owner.listItems.Count; i++)
					{
						if ((owner.listItems[i] as ListViewItem).Checked)
						{
							count++;
						}
					}
					return count;
				}
			}

			public int this[int index]
			{
				get
				{
					int pos = 0;
					for (int i = 0; i < owner.listItems.Count; i++)
					{
						if ((owner.listItems[i] as ListViewItem).Checked)
						{
							if (pos == index)
							{
								return pos;
							}
							pos++;
						}
					}
					throw new ArgumentOutOfRangeException();
				}
			}

			object IList.this[int index]
			{
				get
				{
					return this[index];
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			public virtual bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public CheckedIndexCollection(ListView owner)
			{
				this.owner = owner;
			}

			object ICollection.SyncRoot
			{
				get
				{
					return this;
				}
			}

			bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}

			bool IList.IsFixedSize
			{
				get
				{
					return true;
				}
			}

			public bool Contains(int checkedIndex)
			{
				return (owner.listItems[checkedIndex] as ListViewItem).Checked;
			}

			bool IList.Contains(object checkedIndex)
			{
				if (checkedIndex is int)
				{
					return Contains((int)checkedIndex);
				}
				else
				{
					return false;
				}
			}

			public int IndexOf(int checkedIndex)
			{
				int pos = 0;
				for (int i = 0; i < owner.listItems.Count; i++)
				{
					ListViewItem item = owner.listItems[i] as ListViewItem;
					if (item.Checked)
					{
						if (i == checkedIndex)
						{
							return pos;
						}
						pos++;
					}
				}
				return -1;
			}

			int IList.IndexOf(object checkedIndex)
			{
				if (checkedIndex is Int32)
				{
					return IndexOf((int)checkedIndex);
				}
				else
				{
					return -1;
				}
			}

			int IList.Add(object value)
			{
				throw new NotSupportedException();
			}

			void IList.Clear()
			{
				throw new NotSupportedException();
			}

			void IList.Insert(int index, object value)
			{
				throw new NotSupportedException();
			}

			void IList.Remove(object value)
			{
				throw new NotSupportedException();
			}

			void IList.RemoveAt(int index)
			{
				throw new NotSupportedException();
			}

			void ICollection.CopyTo(Array dest, int index)
			{
				for (int i = 0; i < owner.listItems.Count; i++)
				{
					if ((owner.listItems[i] as ListViewItem).Checked)
					{
						dest.SetValue(i, index++);
					}
				}
			}

			public virtual IEnumerator GetEnumerator()
			{
				return GenerateCheckedIndexes().GetEnumerator(); 
			}

			private int[] GenerateCheckedIndexes()
			{
				int[] indexes =new int[Count];
				int pos = 0;
				for (int i = 0; i < owner.listItems.Count; i++)
				{
					if ((owner.listItems[i] as ListViewItem).Checked)
					{
						indexes[pos++] = i;
					}
				}
				return indexes;
			}
		}

		public class CheckedListViewItemCollection: IList
		{
			private ListView owner;

			public virtual int Count
			{
				get
				{
					return owner.CheckedIndices.Count;
				}
			}

			private ListViewItem[] GenerateCheckedItems()
			{
				ListViewItem[] indexes =new ListViewItem[owner.CheckedIndices.Count];
				int pos = 0;
				for (int i = 0; i < owner.listItems.Count; i++)
				{
					ListViewItem item = owner.listItems[i] as ListViewItem;
					if (item.Checked)
					{
						indexes[pos++] = item;
					}
				}
				return indexes;
			}

			public ListViewItem this[int index]
			{
				get
				{
					int pos = 0;
					for (int i = 0; i < owner.listItems.Count; i++)
					{
						ListViewItem item = owner.listItems[i] as ListViewItem;
						if (item.Checked)
						{
							if (pos == index)
							{
								return item;
							}
							pos++;
						}
					}
					return null;
				}
			}

			object IList.this[int index]
			{
				get
				{
					return this[index];
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			public virtual bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public CheckedListViewItemCollection(ListView owner)
			{
				this.owner = owner;
			}

			object ICollection.SyncRoot
			{
				get
				{
					return this;
				}
			}

			bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}

			bool IList.IsFixedSize
			{
				get
				{
					return true;
				}
			}

			public bool Contains(ListViewItem item)
			{
				if (item != null && item.ListView == owner && item.Checked)
				{
					return true;
				}
				else
				{
					return false;
				}
			}

			bool IList.Contains(object item)
			{
				if (item is ListViewItem)
				{
					return Contains(item as ListViewItem);
				}
				else
				{
					return false;
				}
			}

			public int IndexOf(ListViewItem item)
			{
				int pos = 0;
				for (int i = 0; i < owner.listItems.Count; i++)
				{
					ListViewItem itemArray = owner.listItems[i] as ListViewItem;
					if (item.Checked)
					{
						if (item == itemArray)
						{
							return pos;
						}
						pos++;
					}
				}
				return -1;
			}

			int IList.IndexOf(object item)
			{
				ListViewItem listViewItem = item as ListViewItem;
				if (listViewItem != null)
				{
					return IndexOf(listViewItem);
				}
				else
				{
					return -1;
				}
			}

			void IList.Clear()
			{
				throw new NotSupportedException();
			}

			int IList.Add(object value)
			{
				throw new NotSupportedException();
			}

			void IList.Insert(int index, object value)
			{
				throw new NotSupportedException();
			}

			void IList.RemoveAt(int index)
			{
				throw new NotSupportedException();
			}

			void IList.Remove(object value)
			{
				throw new NotSupportedException();
			}

			public virtual void CopyTo(Array dest, int index)
			{
				for (int i = 0; i < owner.listItems.Count; i++)
				{
					ListViewItem item = owner.listItems[i] as ListViewItem;
					if (item.Checked)
					{
						dest.SetValue(item, index++);
					}
				}
			}

			public virtual IEnumerator GetEnumerator()
			{
				return GenerateCheckedItems().GetEnumerator();
			}
		}
		public class SelectedIndexCollection: IList
		{
			private ListView owner;

			public virtual int Count
			{
				get
				{
					int pos = 0;
					for (int i = 0; i < owner.listItems.Count; i++)
					{
						ListViewItem listViewItem = owner.listItems[i] as ListViewItem;
						if (listViewItem.Selected)
						{
							pos++;
						}
					}
					return pos;
				}
			}

			public int this[int index]
			{
				get
				{
					int pos = 0;
					for (int i = 0; i < owner.listItems.Count; i++)
					{
						ListViewItem listViewItem = owner.listItems[i] as ListViewItem;
						if (listViewItem.Selected)
						{
							if (pos == index)
							{
								return i;
							}
							pos++;
						}
					}
					throw new ArgumentOutOfRangeException();
				}
			}

			public virtual bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public SelectedIndexCollection(ListView owner)
			{
				this.owner = owner;
			}

			object IList.this[int index]
			{
				get
				{
					return this[index];
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			object ICollection.SyncRoot
			{
				get
				{
					return this;
				}
			}

			bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}

			bool IList.IsFixedSize
			{
				get
				{
					return true;
				}
			}

			public bool Contains(int selectedIndex)
			{
				return owner.Items[selectedIndex].Selected;
			}

			bool IList.Contains(object selectedIndex)
			{
				if (selectedIndex is Int32)
				{
					return Contains((int)selectedIndex);
				}
				else
				{
					return false;
				}
			}

			public int IndexOf(int selectedIndex)
			{
				int pos = 0;
				for (int i = 0; i < owner.listItems.Count; i++)
				{
					ListViewItem listViewItem = owner.listItems[i] as ListViewItem;
					if (listViewItem.Selected)
					{
						if (selectedIndex == i)
						{
							return pos;
						}
						pos++;
					}
				}
				return -1;
			}

			int IList.IndexOf(object selectedIndex)
			{
				if (selectedIndex is Int32)
				{
					return IndexOf((int)selectedIndex);
				}
				else
				{
					return -1;
				}
			}

			int IList.Add(object value)
			{
				throw new NotSupportedException();
			}

			void IList.Clear()
			{
				throw new NotSupportedException();
			}

			void IList.Insert(int index, object value)
			{
				throw new NotSupportedException();
			}

			void IList.RemoveAt(int index)
			{
				throw new NotSupportedException();
			}

			void IList.Remove(object value)
			{
				throw new NotSupportedException();
			}

			public virtual void CopyTo(Array dest, int index)
			{
				for (int i = 0; i < owner.listItems.Count; i++)
				{
					if ((owner.listItems[i] as ListViewItem).Selected)
					{
						dest.SetValue(i, index++);
					}
				}
			}

			public virtual IEnumerator GetEnumerator()
			{
				return GenerateSelectedIndexArray().GetEnumerator();	
			}

			private int[] GenerateSelectedIndexArray()
			{
				int[] selectedIndexes = new int[Count];
				int pos = 0;
				for (int i = 0; i < owner.listItems.Count; i++)
				{
					if ((owner.listItems[i] as ListViewItem).Selected)
					{
						selectedIndexes[pos++] = i;
					}
				}
				return selectedIndexes;
			}
		}

		public class SelectedListViewItemCollection: IList
		{
			private ListView owner;

			public virtual int Count
			{
				get
				{
					return owner.selectedIndices.Count;
				}
			}

			public ListViewItem this[int index]
			{
				get
				{
					int pos = 0;
					for (int i = 0;i < owner.listItems.Count; i++)
					{
						ListViewItem listViewItem = owner.listItems[i] as ListViewItem;
						if (listViewItem.Selected)
						{
							if (index == pos)
							{
								return listViewItem;
							}
							pos++;
						}
					}
					throw new ArgumentOutOfRangeException();
				}
			}

			public virtual bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public SelectedListViewItemCollection(ListView owner)
			{
				this.owner = owner;
			}

			object IList.this[int index]
			{
				get
				{
					return this[index];
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			bool IList.IsFixedSize
			{
				get
				{
					return true;
				}
			}

			object ICollection.SyncRoot
			{
				get
				{
					return this;
				}
			}

			bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}

			int IList.Add(object value)
			{
				throw new NotSupportedException();
			}

			void IList.Insert(int index, object value)
			{
				throw new NotSupportedException();
			}

			void IList.RemoveAt(int index)
			{
				throw new NotSupportedException();
			}

			void IList.Remove(object value)
			{
				throw new NotSupportedException();
			}

			public virtual void Clear()
			{
				for (int i = 0; i < owner.listItems.Count; i++)
				{
					ListViewItem listViewItem = owner.listItems[i] as ListViewItem;
					if (listViewItem.Selected)
					{
						listViewItem.Selected = false;
					}
				}
			}

			public bool Contains(ListViewItem item)
			{
				return IndexOf(item) != -1;
			}

			bool IList.Contains(object item)
			{
				ListViewItem listViewItem = item as ListViewItem;
				if (listViewItem != null)
				{
					return Contains(item as ListViewItem);
				}
				else
				{
					return false;
				}
			}

			public virtual void CopyTo(Array dest, int index)
			{
				for (int i = 0; i < owner.listItems.Count; i++)
				{
					ListViewItem listViewItem = owner.listItems[i] as ListViewItem;
					if (listViewItem.Selected)
					{
						dest.SetValue(listViewItem, index++);
					}
				}
			}

			public int IndexOf(ListViewItem item)
			{
				int pos = 0;
				for (int i = 0; i < owner.listItems.Count; i++)
				{
					ListViewItem arrayItem = owner.listItems[i] as ListViewItem;
					
					if (arrayItem.Selected)
					{
						if (arrayItem == item)
						{
							return pos;
						}
						pos++;
					}
				}
				return -1;
			}

			public virtual IEnumerator GetEnumerator()
			{
				return GenerateSelectedItemArray().GetEnumerator();
			}

			private ListViewItem[] GenerateSelectedItemArray()
			{
				ListViewItem[] selectedItems = new ListViewItem[Count];
				int pos = 0;
				for (int i = 0; i < owner.listItems.Count; i++)
				{
					ListViewItem item = owner.listItems[i] as ListViewItem;
					if (item.Selected)
					{
						selectedItems[pos++] = item;
					}
				}
				return selectedItems;
			}

			int IList.IndexOf(object item)
			{
				ListViewItem listViewItem = item as ListViewItem;
				if (listViewItem != null)
				{
					return IndexOf(listViewItem);
				}
				return -1;
			}
		}

		public class ListViewItemCollection: IList
		{
			private ListView owner;

			public virtual int Count
			{
				get
				{
					return owner.listItems.Count;
				}
			}

			public virtual bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			[TODO]
			public virtual ListViewItem this[int displayIndex]
			{
				get
				{
					return owner.listItems[displayIndex] as ListViewItem;
				}

				set
				{
					owner.listItems[displayIndex] = value;
				}
			}

			public ListViewItemCollection(ListView owner)
			{
				this.owner = owner;
			}

			object ICollection.SyncRoot
			{
				get
				{
					return this;
				}
			}

			bool ICollection.IsSynchronized
			{
				get
				{
					return true;
				}
			}

			bool IList.IsFixedSize
			{
				get
				{
					return false;
				}
			}

			object IList.this[int index]
			{
				get
				{
					return this[index];
				}
				set
				{
					if (value is ListViewItem)
					{
						this[index] = value as ListViewItem;
					}
					else if (value != null)
					{
						this[index] = new ListViewItem(value.ToString());
					}
				}
			}

			public virtual ListViewItem Add(string text)
			{
				return Add(text, -1);
			}

			int IList.Add(object item)
			{
				if (item is ListViewItem)
				{
					return IndexOf(Add(item as ListViewItem));
				}
				if (item != null)
				{
					return IndexOf(Add(item.ToString()));
				}
				return -1;
			}

			public virtual ListViewItem Add(string text, int imageIndex)
			{
				ListViewItem listViewItem = new ListViewItem(text, imageIndex);
				Add(listViewItem);
				return listViewItem;
			}

			[TODO]
			public virtual ListViewItem Add(ListViewItem value)
			{
				owner.listItems.Add(value);
				owner.Sort();
				return value;
			}

			[TODO]
			public void AddRange(ListViewItem[] values)
			{
				owner.listItems.AddRange(values);
				owner.Sort();
			}

			[TODO]
			public virtual void Clear()
			{
				if (owner.listItems.Count > 0)
				{
					owner.listItems.Clear();
				}
			}

			public bool Contains(ListViewItem item)
			{
				return IndexOf(item) != -1;
			}

			bool IList.Contains(object item)
			{
				if (item is ListViewItem)
				{
					return Contains(item as ListViewItem);
				}
				else
				{
					return false;
				}
			}

			public virtual void CopyTo(Array dest, int index)
			{
				for (int i = 0; i < owner.listItems.Count; i++)
				{
					dest.SetValue(owner.listItems[i], index++);
				}
			}

			public virtual IEnumerator GetEnumerator()
			{
				ListViewItem[] listViewItems = new ListViewItem[owner.listItems.Count];
				CopyTo(listViewItems, 0);
				return listViewItems.GetEnumerator();
			}

			public int IndexOf(ListViewItem item)
			{
				for (int i = 0; i < Count; i++)
				{
					if (this[i] == item)
					{
						return i;
					}
				}
				return -1;
			}

			int IList.IndexOf(object item)
			{
				ListViewItem listViewItem = item as ListViewItem;
				if (item != null)
				{
					return IndexOf(listViewItem);
				}
				else
				{
					return -1;
				}
			}

			void IList.Insert(int index, object item)
			{
				ListViewItem listViewItem = item as ListViewItem;
				if (listViewItem != null)
				{
					Insert(index, listViewItem);
				}
				else if (item != null)
				{
					Insert(index, item.ToString());
				}
			}

			[TODO]
			public virtual void RemoveAt(int index)
			{
				owner.listItems.RemoveAt(index);
			}

			[TODO]
			public virtual void Remove(ListViewItem item)
			{
				Remove(item);
			}

			void IList.Remove(object item)
			{
				if (item == null || !(item is ListViewItem))
				{
					return;
				}
				Remove(item as ListViewItem);
			}

			[TODO]
			public ListViewItem Insert(int index, ListViewItem item)
			{
				owner.listItems.Insert(index, item);
				return item;
			}

			public ListViewItem Insert(int index, string text)
			{
				return Insert(index, new ListViewItem(text));
			}

			public ListViewItem Insert(int index, string text, int imageIndex)
			{
				return Insert(index, new ListViewItem(text, imageIndex));
			}
		}

		public class ColumnHeaderCollection: IList
		{
			private ListView owner;

			public virtual ColumnHeader this[int index]
			{
				get
				{
					return owner.columnHeaders[index];
				}
			}

			public virtual int Count
			{
				get
				{
					if (owner.columnHeaders != null)
					{
						return owner.columnHeaders.Length;
					}
					else
					{
						return 0;
					}
				}
			}

			public virtual bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public ColumnHeaderCollection(ListView owner)
			{
				this.owner = owner;
			}

			object IList.this[int index]
			{
				get
				{
					return this[index];
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			object ICollection.SyncRoot
			{
				get
				{
					return this;
				}
			}

			bool ICollection.IsSynchronized
			{
				get
				{
					return true;
				}
			}

			bool IList.IsFixedSize
			{
				get
				{
					return false;
				}
			}

			public virtual ColumnHeader Add(string str, int width, HorizontalAlignment textAlign)
			{
				ColumnHeader columnHeader = new ColumnHeader();
				columnHeader.text = str;
				columnHeader.width = width;
				columnHeader.textAlign = textAlign;
				Add(columnHeader);
				return columnHeader;
			}

			[TODO]
			public virtual int Add(ColumnHeader value)
			{
				return Count;
			}

			public virtual void AddRange(ColumnHeader[] values)
			{
				for (int i = 0; i < values.Length; i++)
					Add(values[i]);
			}

			int IList.Add(object value)
			{
				return Add(value as ColumnHeader);
			}

			[TODO]
			public virtual void Clear()
			{
				if (owner.columnHeaders != null)
				{
					owner.columnHeaders = null;
				}
			}

			void ICollection.CopyTo(Array dest, int index)
			{
				Array.Copy(owner.columnHeaders, 0, dest, index, owner.columnHeaders.Length);
			}

			public int IndexOf(ColumnHeader value)
			{
				for (int i = 0; i < owner.columnHeaders.Length; i++)
				{
					if (this[i] == value)
					{
						return i;
					}
				}
				return -1;
			}

			int IList.IndexOf(object value)
			{
				ColumnHeader columnHeader = value as ColumnHeader;
				if (columnHeader != null)
				{
					return IndexOf(columnHeader);
				}
				else
				{
					return -1;
				}
			}

			public bool Contains(ColumnHeader value)
			{
				return IndexOf(value) != -1;
			}

			bool IList.Contains(object value)
			{
				ColumnHeader columnHeader = value as ColumnHeader;
				if (columnHeader != null)
				{
					return Contains(columnHeader);
				}
				else
				{
					return false;
				}
			}

			public virtual void Remove(ColumnHeader column)
			{
				int pos = IndexOf(column);
				if (pos != -1)
				{
					RemoveAt(pos);
				}
			}

			void IList.Remove(object value)
			{
				if (value is ColumnHeader)
				{
					Remove(value as ColumnHeader);
				}
			}

			public virtual void RemoveAt(int index)
			{

				owner.columnHeaders[index].listView = null;
				int newLen = owner.columnHeaders.Length - 1;
				if (newLen == 0)
				{
					owner.columnHeaders = null;
					return; 
				}
				ColumnHeader[] columnHeaders = new ColumnHeader[newLen];
				Array.Copy(owner.columnHeaders, 0, columnHeaders, 0, index);
				if (index < newLen)
				{
					Array.Copy(owner.columnHeaders, index + 1, columnHeaders, index, newLen - index);
				}
				owner.columnHeaders = columnHeaders;

			}

			public virtual IEnumerator GetEnumerator()
			{
				if (owner.columnHeaders != null)
				{
					return owner.columnHeaders.GetEnumerator();
				}
				else
				{
					return new ColumnHeader[0].GetEnumerator();
				}
			}
			
			public void Insert(int index, ColumnHeader value)
			{
				value.listView = owner;
				int oldLen = 0;
				if (owner.columnHeaders == null)
				{
					owner.columnHeaders = new ColumnHeader[1];
				}
				else
				{
					oldLen = owner.columnHeaders.Length;
					ColumnHeader[] newColumnHeaders = new ColumnHeader[oldLen + 1];
					Array.Copy(owner.columnHeaders, newColumnHeaders, oldLen);
					owner.columnHeaders = newColumnHeaders;
				}
				Array.Copy(owner.columnHeaders, index, owner.columnHeaders, index + 1, oldLen - index);
				owner.columnHeaders[index] = value;
				//TODO
			}

			void IList.Insert(int index, object value)
			{
				ColumnHeader columnHeader = value as ColumnHeader;
				if (columnHeader != null)
				{
					Insert(index, columnHeader);
				}
			}

			public void Insert(int index, string str, int width, HorizontalAlignment textAlign)
			{
				ColumnHeader columnHeader = new ColumnHeader();
				columnHeader.text = str;
				columnHeader.width = width;
				columnHeader.textAlign = textAlign;
				Insert(index, columnHeader);
			}
		}
	}


}
