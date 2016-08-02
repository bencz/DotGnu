/*
 * ListViewItem.cs - Implementation of the
 *			"System.Windows.Forms.ListViewItem" class.
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
using System.Collections;
using System.Drawing;
#if CONFIG_SERIALIZATION
using System.Runtime.Serialization;
#endif

	public class ListViewItem: ICloneable
#if CONFIG_SERIALIZATION
		, ISerializable
#endif
	{
		private int stateImageIndex;
		private int imageIndex;
		private object tag;
		internal ListView listView;
		private ListViewSubItem[] subItems;
		private int subItemsCount;
		private bool useItemStyleForSubItems;
		private ListViewSubItemCollection listViewSubItemCollection;
		private int index;
		private bool selected;

		public Color BackColor
		{
			get
			{
				if (subItemsCount == 0)
				{
					if (listView == null)
					{
						return SystemColors.Window; 
					}
					return listView.BackColor; 
				}
				return SubItems[0].BackColor;
			}

			set
			{
				SubItems[0].BackColor = value;
			}
		}

		public Rectangle Bounds
		{
			get
			{
				if (listView == null)
				{
					return Rectangle.Empty;
				}
				else
				{
					return listView.GetItemRect(Index);
				}
			}
		}

		public bool Checked
		{
			get
			{
				return (imageIndex > 0);
			}

			set
			{
				if (Checked == value)
				{
					return;
				}

				if (value)
				{
					imageIndex = 1;
				}
				else
				{
					imageIndex = 0;
				}
			}
		}

		public bool Focused
		{
			get
			{
				return (listView.FocusedItem == this);
			}

			set
			{
				listView.FocusedItemInternal = this;
			}
		}

		public Font Font
		{
			get
			{
				if (subItemsCount == 0)
				{
					if (listView == null)
					{
						return Control.DefaultFont; 
					}
					return listView.Font; 
				}
				return SubItems[0].Font; 

			}

			set
			{
				SubItems[0].Font = value;
			}
		}

		public Color ForeColor
		{
			get
			{
				if (subItemsCount == 0)
				{
					if (listView == null)
					{
						return SystemColors.WindowText; 
					}
					return listView.ForeColor; 
				}
				return SubItems[0].ForeColor;
			}

			set
			{
				SubItems[0].ForeColor = value;
			}
		}

		[TODO]
		public int ImageIndex
		{
			get
			{
				if (ImageList != null && imageIndex != -1 && imageIndex >= ImageList.Images.Count)
				{
					return ImageList.Images.Count - 1;
				}
				return imageIndex;
			}

			set
			{
				imageIndex = value;
				if (listView.IsHandleCreated)
				{
					// Fill in here
				}
			}
		}

		public ImageList ImageList
		{
			get
			{
				if (listView == null)
				{
					return null;
				}
				switch (listView.View)
				{
					case View.LargeIcon:
						return listView.LargeImageList;
					case View.Details:
					case View.SmallIcon:
					case View.List:
						return listView.SmallImageList;

					default:
						return null;
				}
			}
		}

		public int Index
		{
			get
			{
				return index;
			}
		}

		public ListView ListView
		{
			get
			{
				return listView;
			}
		}

		[TODO]
		public bool Selected
		{
			get
			{
				return selected;
			}

			set
			{
				if (value == selected)
				{
					return;
				}
				selected = value;
				// Fill in here
			}
		}

		[TODO]
		public int StateImageIndex
		{
			get
			{
				return stateImageIndex;
			}

			set
			{
				if (stateImageIndex == value)
				{
					return;
				}
				if (value < -1 || value > 14)
				{
					throw new ArgumentOutOfRangeException();
				}
				// Fill in here
			}
		}

		public ListViewSubItemCollection SubItems
		{
			get
			{
				if (subItemsCount == 0)
				{
					subItemsCount = 1;
					subItems = new ListViewSubItem[subItemsCount];
					subItems[0] = new ListViewSubItem(this, string.Empty);
				}
				if (listViewSubItemCollection == null)
				{
					listViewSubItemCollection = new ListViewSubItemCollection(this);
				}
				return listViewSubItemCollection;
			}
		}

		public object Tag
		{
			get
			{
				return tag;
			}

			set
			{
				tag = value;
			}
		}

		public string Text
		{
			get
			{
				if (subItemsCount == 0)
				{
					return string.Empty; 
				}
				return SubItems[0].Text; 
			}

			set
			{
				SubItems[0].Text = value;
			}
		}

		public bool UseItemStyleForSubItems
		{
			get
			{
				return useItemStyleForSubItems;
			}

			set
			{
				useItemStyleForSubItems = value;
			}
		}

		public ListViewItem()
		{
			listViewSubItemCollection = null;
			index = -1;
			imageIndex = -1;
			selected = false;
			useItemStyleForSubItems = true;
			imageIndex = -1;
			subItemsCount = 0;
		}

		public ListViewItem(string text) : this(text, -1)
		{
		}

		public ListViewItem(string text, int imageIndex) : this()
		{
			this.imageIndex = imageIndex;
			Text = text;
		}

		public ListViewItem(string[] items) : this(items, -1)
		{
		}

		public ListViewItem(string[] items, int imageIndex, Color foreColor, Color backColor, Font font) : this(items, imageIndex)
		{
			ForeColor = foreColor;
			BackColor = backColor;
			Font = font;
		}

		public ListViewItem(string[] items, int imageIndex) : this()
		{
			this.imageIndex = imageIndex;
			if (items == null || items.Length > 0)
			{
				return;
			}
			subItemsCount = items.Length;
			subItems = new ListViewSubItem[subItemsCount];
			for (int i = 0; i < items.Length; i++)
			{
				subItems[i] = new ListViewSubItem(this, items[i]);
			}
		}

		public ListViewItem(ListViewSubItem[] subItems, int imageIndex) : this()
		{
			this.imageIndex = imageIndex;
			this.subItems = subItems;
			subItemsCount = subItems.Length;
			for (int i = 0; i < subItemsCount; i++)
			{
				subItems[i].owner = this;
			}
		}

		public virtual object Clone()
		{
			// Deep clone the sub items.
			ListViewSubItem[] newSubItems = new ListViewSubItem[subItemsCount];
			for (int i = 0; i < subItemsCount; i++)
			{
				ListViewSubItem item = subItems[i];
				newSubItems[i] = new ListViewSubItem(null, item.Text, item.ForeColor, item.BackColor, item.Font);
			}
			
			ListViewItem items = new ListViewItem(newSubItems, imageIndex);

			items.Checked = Checked;
			items.useItemStyleForSubItems = useItemStyleForSubItems;
			items.tag = tag;
			return items;
		}

		public void BeginEdit()
		{
			if (Index == 0)
			{
				return;
			}
			if (!ListView.LabelEdit)
			{
				throw new InvalidOperationException();
			}

			ListView.BeginEdit(this);
		}

		public virtual void EnsureVisible()
		{
			if (listView != null)
			{
				listView.EnsureVisible(Index);
			}
		}

		public Rectangle GetBounds(ItemBoundsPortion portion)
		{
			if (listView != null)
			{
				return listView.GetItemRect(Index, portion);
			}
			else
			{
				return Rectangle.Empty;
			}
		}

		public virtual void Remove()
		{
			if (listView != null)
			{
				listView.Items.RemoveAt(Index);
			}
		}

		public override string ToString()
		{
			return "ListViewItem: \"" + Text + "\"";
		}

#if CONFIG_SERIALIZATION
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			Serialize(info, context);
		}

		protected virtual void Deserialize(SerializationInfo info, StreamingContext context)
		{
			int itemLength = 0;
			SerializationInfoEnumerator serializationInfos = info.GetEnumerator();
			while (serializationInfos.MoveNext())
			{
				SerializationEntry entry = serializationInfos.Current;
				if (entry.Name == "Text")
				{
					Text = entry.Value as String;
				}
				else if (entry.Name == "ImageIndex")
				{
					imageIndex = (int)entry.Value;
				}
				else if (entry.Name == "SubItemCount")
				{
					itemLength = (int)entry.Value;
				}
				else if (entry.Name == "BackColor")
				{
					BackColor = (System.Drawing.Color)entry.Value;
				}
				else if (entry.Name == "Checked")
				{
					Checked = (bool)entry.Value;
				}
				else if (entry.Name == "Font")
				{
					Font = (Font)entry.Value;
				}
				else if (entry.Name == "ForeColor")
				{
					ForeColor = (System.Drawing.Color)entry.Value;
				}
				else if (entry.Name == "UseItemStyleForSubItems")
				{
					UseItemStyleForSubItems = (bool)entry.Value;
				}
			}
			if (itemLength > 0)
			{
				ListViewSubItem[] subItems = new ListViewSubItem[itemLength];
				for (int i = 1; i < itemLength; i++)
				{
					ListViewSubItem item = info.GetValue("SubItem" + i, typeof(ListViewSubItem)) as ListViewSubItem;
					item.owner = this;
					subItems[i] = item;
				}
				subItems[0] = this.subItems[0];
				this.subItems = subItems;
			}

		}

		protected virtual void Serialize(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Text", Text);
			info.AddValue("ImageIndex", imageIndex);
			if (subItemsCount > 1)
			{
				info.AddValue("SubItemCount", subItemsCount);
				for (int i = 1; i < subItemsCount; i++)
				{
					info.AddValue("SubItem" + i.ToString(), SubItems[i], typeof(ListViewSubItem));
				}
 
			}
			info.AddValue("BackColor", BackColor);
			info.AddValue("Checked", Checked);
			info.AddValue("Font", Font);
			info.AddValue("ForeColor", ForeColor);
			info.AddValue("UseItemStyleForSubItems", UseItemStyleForSubItems);
		}

#endif
	
		public class ListViewSubItem
		{
			internal ListViewItem owner;
			private string text;
			internal Properties properties;

			internal class Properties
			{
				public Color backColor = Color.Empty;
				public Color foreColor = Color.Empty;
				public Font font = null;
			}

			public Color BackColor
			{
				get
				{
					if (properties != null && properties.backColor != Color.Empty)
					{
						return properties.backColor; 
					}
					if (owner == null || owner.listView == null)
					{
						return SystemColors.Window;
					}
					return owner.listView.BackColor; 
				}

				set
				{
					if (properties == null)
					{
						properties = new Properties();
					}
					if (properties.backColor == value)
					{
						return;
					}
					properties.backColor = value;
					if (owner != null && owner.listView != null)
					{
						owner.listView.Invalidate();
					}
				}
			}

			public Font Font
			{
				get
				{
					if (properties != null && properties.font != null)
					{
						return properties.font; 
					}
					if (owner == null || owner.listView == null)
					{
						return Control.DefaultFont;
					}
					return owner.listView.Font;
				}

				set
				{
					if (properties == null)
					{
						properties = new Properties();
 
					}
					if (properties.font != value)
					{
						properties.font = value;
						if (owner != null && owner.listView != null)
						{
							owner.listView.Invalidate();
						}
					}
				}
			}

			public Color ForeColor
			{
				get
				{
					if (properties != null && properties.foreColor != Color.Empty)
					{
						return properties.foreColor; 
					}
					if (owner == null || owner.listView == null)
					{
						return SystemColors.WindowText;
					}
					return owner.listView.ForeColor;
				}

				set
				{
					if (properties == null)
					{
						properties = new Properties();
 
					}
					if (properties.foreColor != value)
					{
						properties.foreColor = value;
						if (owner != null && owner.listView != null)
						{
							owner.listView.Invalidate();
						}
					}
				}
			}

			[TODO]
			public string Text
			{
				get
				{
					if (text != null)
					{
						return text;
					}
					else
					{
						return String.Empty;
					}
				}

				set
				{
					text = value;
					if (owner != null)
					{
						// Fill in here
					}
				}
			}

			public ListViewSubItem(ListViewItem owner, string text)
			{
				this.owner = owner;
				this.text = text;
			}

			public ListViewSubItem(ListViewItem owner, string text, Color foreColor, Color backColor, Font font)
			{
				this.owner = owner;
				this.text = text;
				properties = new Properties();
				properties.foreColor = foreColor;
				properties.backColor = backColor;
				properties.font = font;
			}

			public ListViewSubItem()
			{}

			public void ResetStyle()
			{
				if (properties != null)
				{
					properties = null;
					if (owner != null && owner.listView != null)
					{
						owner.listView.Invalidate();
					}
				}
			}

			public override string ToString()
			{
				return "ListViewSubItem: \"" + Text + "\"";
			}
		}


		public class ListViewSubItemCollection: IList
		{
			private ListViewItem owner;

			public virtual int Count
			{
				get
				{
					return owner.subItemsCount;
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
			public ListViewSubItem this[int index]
			{
				get
				{
					return owner.subItems[index];
				}

				set
				{
					owner.subItems[index] = value;
					// Fill in here
				}
			}

			public ListViewSubItemCollection(ListViewItem owner)
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
					this[index] = value as ListViewSubItem;
				}
			}

			public void AddRange(string[] items)
			{
				SizeSubItemArray(items.Length, -1);
				for (int i = 0; i < items.Length; i++)
				{
					String item = items[i];
					if (item != null)
					{
						owner.subItems[++owner.subItemsCount] = new ListViewSubItem(owner, item);
					}
				}
				//TODO:

			}

			public void AddRange(ListViewSubItem[] items)
			{
				SizeSubItemArray(items.Length, -1);
				for (int i = 0; i < items.Length; i++)
				{
					ListViewSubItem item = items[i];
					if (item != null)
					{
						owner.subItems[owner.subItemsCount++] = item;
					}
				}
				//TODO:
			}

			public void AddRange(string[] items, Color foreColor, Color backColor, Font font)
			{
				this.SizeSubItemArray(items.Length, -1);
				for (int i = 0; i < items.Length; i++)
				{
					String item = items[i];
					if (item != null)
					{
						this.owner.subItems[owner.subItemsCount++] = new ListViewSubItem(owner, item, foreColor, backColor, font);
					}
				}
				//TODO:
			}

			public ListViewSubItem Add(ListViewSubItem item)
			{
				SizeSubItemArray(1, -1);
				owner.subItems[owner.subItemsCount++] = item;
				//TODO
				return item;
			}

			public ListViewSubItem Add(string text)
			{
				ListViewSubItem item = new ListViewSubItem(owner, text);
				Add(item);
				return item;
			}

			public ListViewSubItem Add(string text, Color foreColor, Color backColor, Font font)
			{
				ListViewSubItem item = new ListViewSubItem(owner, text, foreColor, backColor, font);
				Add(item);
				return item;
			}

			int IList.Add(object item)
			{
				return IndexOf(Add(item as ListViewSubItem));
			}

			public virtual void Clear()
			{
				owner.subItemsCount = 0;
				//TODO
			}

			public bool Contains(ListViewSubItem subItem)
			{
				return (IndexOf(subItem) != -1); 
			}

			bool IList.Contains(object subItem)
			{
				return (IndexOf(subItem as ListViewSubItem) != -1);
			}

			public void Insert(int index, ListViewSubItem item)
			{
				SizeSubItemArray(1, index);
				item.owner = owner;
				owner.subItems[index] = item;
				owner.subItemsCount++;
				//TODO
			}

			void IList.Insert(int index, object item)
			{
				Insert(index, item as ListViewSubItem);
			}

			public void Remove(ListViewSubItem item)
			{
				int pos = this.IndexOf(item);
				if (pos != -1)
				{
					RemoveAt(pos);
				}
			}
			
			void IList.Remove(object item)
			{
				Remove(item as ListViewSubItem);
			}

			public virtual void RemoveAt(int index)
			{
				for (int i = index; i < owner.subItemsCount - 1; i++)
				{
					owner.subItems[i] = owner.subItems[i + 1];
				}
				owner.subItemsCount++;
				//TODO:
			}

			void ICollection.CopyTo(Array dest, int index)
			{
				Array.Copy(owner.subItems, 0, dest, index, owner.subItemsCount);
			}

			public virtual IEnumerator GetEnumerator()
			{
				if (owner.subItems != null)
				{
					return new ArrayEnumerator(owner.subItems, owner.subItemsCount); 
				}
				return new ListViewSubItem[0].GetEnumerator();
			}

			public int IndexOf(ListViewSubItem subItem)
			{
				for (int i = 0;i < owner.subItemsCount; i++)
				{
					if (owner.subItems[i] == subItem)
					{
						return i; 
					}
				}
				return -1;
			}

			int IList.IndexOf(object subItem)
			{
				ListViewSubItem listViewSubItem = subItem as ListViewSubItem;
				if (listViewSubItem != null)
				{
					return IndexOf(listViewSubItem); 
				}
				return -1;
			}

			// Make sure we have enough space in the array. insert at "index" or at the end if index is -1.
			private void SizeSubItemArray(int size, int index)
			{
				// Do we need more space?
				if (owner.subItems.Length < owner.subItemsCount + size)
				{
					int newLength = owner.subItems.Length * 2;
					int minLength = owner.subItemsCount + size;
					for (; newLength < minLength ; newLength *= 2)
					{
					}
					ListViewSubItem[] newSubItems = new ListViewSubItem[newLength];
					if (index != -1)
					{
						Array.Copy(owner.subItems, 0, newSubItems, 0, index);
						Array.Copy(owner.subItems, index, newSubItems, index + size, owner.subItemsCount - index);
					}
					else
					{
							Array.Copy(owner.subItems, newSubItems, owner.subItemsCount);
					}
					owner.subItems = newSubItems;
					return; 
				}

				// Move items after index up to make space for the new items.
				if (index != -1)
				{
					for (int i = owner.subItemsCount - 1; i >= index; i--)
					{
						owner.subItems[(i + size)] = owner.subItems[i];
					}
				}
 
			}

			private class ArrayEnumerator : IEnumerator
			{
				private object[] array;
				private int count;
				private int current;

				public ArrayEnumerator(object[] array, int count)
				{
					this.array = array;
					this.count = count;
					current = -1;
				}

				public object Current
				{
					get
					{
						if (current == -1)
						{
							return null;
						}
						return array[current];
					}
				}

				public bool MoveNext()
				{
					if (current < count - 1)
					{
						current++;
						return true;
					}
					else
					{
						return false;
					}
				}

				public void Reset()
				{
					current = -1;
				}

			}

		}
	}
}
