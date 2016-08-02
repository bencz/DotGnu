/*
 * DomainUpDown.cs - Implementation of the
 *			"System.Windows.Forms.DomainUpDown" class.
 *
 * Copyright (C) 2003 Free Software Foundation
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

using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms.Themes;

namespace System.Windows.Forms
{

[TODO]
public class DomainUpDown : UpDownBase
{
	private DomainUpDownItemCollection domainItems;
	private int domainIndex;
	private bool sorted;
	private bool wrap;

	[TODO]
	public class DomainUpDownItemCollection : ArrayList	
 	{
		private DomainUpDown owner;

		internal DomainUpDownItemCollection(DomainUpDown owner)
		{
			this.owner = owner;
		}

		[TODO]
		public override int Add(object item)
		{
			int i = base.Add(item);
			return i;
		}

		[TODO]
		public override void Insert(int index, object item)
		{			
			base.Insert(index, item);
			if (index <= owner.SelectedIndex)
			{
				owner.SelectedIndex++;
			}
		}

		[TODO]
		public override void Remove(object item)
		{
			base.Remove(item);
		}

		[TODO]
		public override void RemoveAt(int item)
		{
			base.RemoveAt(item);
			if (item == owner.SelectedIndex)
			{
				if (Count > item)
				{
					owner.SelectedIndex = item;
				}
				else
				{				
					owner.SelectedIndex = Count - 1;
				}
			}
		}

		[TODO]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public override object this[int index]
		{
			get
			{
				return base[index];
			}
			set
			{
				base[index] = value;
				if (index == owner.SelectedIndex)
				{
					// just to update text
					owner.SelectedIndex = index;
				}
			}
		}
	}; // class  DomainUpDownItemCollection

	private class DomainUpDownCompare : IComparer
	{
		internal DomainUpDownCompare()
		{
		}

		public int Compare(object a, object b)
		{
			return a.ToString().CompareTo(b.ToString());
		}
	}; // class  DomainUpDownCompare
		

	public DomainUpDown() : base()
	{
		domainItems = new DomainUpDownItemCollection(this);
		domainIndex = -1;
	}

	[TODO]
	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return base.CreateAccessibilityInstance();
	}

	[TODO]
	public override void DownButton()
	{
		if (domainIndex >= (domainItems.Count - 1))
		{
			if (!wrap)
			{
				return;
			}
			domainIndex = -1;
		}
		if (domainItems.Count > 0)
		{
			domainIndex++;
			SetText();
			OnSelectedItemChanged(this, new EventArgs());
		}
	}

	[TODO]
	protected override void OnChanged(object source, EventArgs e)
	{
		base.OnChanged(source, e);
	}

	[TODO]
	protected void OnSelectedItemChanged(object source, EventArgs e)
	{
		if (SelectedItemChanged != null)
		{
			SelectedItemChanged(this, e);
		}
	}
	
	[TODO]
	protected override void OnTextBoxKeyDown(object source, KeyEventArgs e)
	{
		base.OnTextBoxKeyDown(source, e);
	}

	public override string ToString()
	{
		return "System.Windows.Forms.DomainUpDown, Items.count: " + domainItems.Count.ToString() +
			", SelectedIndex: " + domainIndex.ToString();
	}

	[TODO]
	public override void UpButton()
	{
		if (domainIndex <= 0)
		{
			if (!wrap)
			{
				return;
			}
			domainIndex = domainItems.Count;
		}
		domainIndex--;
		SetText();
		OnSelectedItemChanged(this, new EventArgs());
	}

	[TODO]
	protected override void UpdateEditText()
	{
	}

	public DomainUpDownItemCollection Items
	{
		get
		{
			return domainItems;
		}
	}

	[TODO]
	[BrowsableAttribute(false)]
	[DefaultValue(-1)]
	public int SelectedIndex
	{
		get
		{
			return domainIndex;
		}
		set
		{
			if ((value < -1) || (value >= domainItems.Count))
				throw new ArgumentException("value", value.ToString());
			domainIndex = value;
			SetText();
			OnSelectedItemChanged(this, new EventArgs());
		}
	}

	[TODO]
	[BrowsableAttribute(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public object SelectedItem
	{
		get
		{
			if (domainIndex >= 0)
			{
				return domainItems[domainIndex];
			}
			return null;
		}
		set
		{
			// FIXME
			if (domainIndex == -1)
			{
				throw new  NullReferenceException("No item Selected");
			}
			domainItems[domainIndex] = value;
			SetText();
			OnSelectedItemChanged(this, new EventArgs());
		}
	}

	[TODO]
	[DefaultValue(false)]	
	public bool Sorted
	{
		get
		{
			return sorted;
		}
		set
		{
			// sorting must be implemented
			if (value)
			{
				domainItems.Sort(new DomainUpDownCompare());
			}
			sorted = value;
		}
	}

	[Localizable(true)]
	[DefaultValue(false)]	
	public bool Wrap
	{
		get
		{
			return wrap;
		}
		set
		{
			wrap = value;
		}
	}

	public event EventHandler SelectedItemChanged;

	private void SetText()
	{
		if (domainIndex >= 0)
		{
			base.Text = domainItems[domainIndex].ToString();
		}
		else
		{
			base.Text = string.Empty;
		}
	}
}; // class DomainUpDown
	
}; // namespace System.Windows.Forms

