/*
 * LinkLabel.cs - Implementation of the
 *			"System.Windows.Forms.LinkLabel" class.
 *
 * Copyright (C) 2003	Southern Storm Software, Pty Ltd.
 *
 * With contributions from Ian Fung <if26@cornell.edu>
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.	See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA	02111-1307	USA
 */

namespace System.Windows.Forms
{

using System.Drawing;
using System.Drawing.Text;
using System.Collections;

[TODO]
public class LinkLabel : Label, IButtonControl
{
	// Internal state.
	private Color activeLinkColor;
	private Color disabledLinkColor;
	private LinkArea linkArea;
	private LinkBehavior linkBehavior;
	private Color linkColor;
	private LinkCollection links;
	private bool linkVisited;
	private Color visitedLinkColor;

	[TODO]
	public LinkLabel()
	{
	}
	
	// Implement IButtonControl
	public DialogResult DialogResult
	{
		get
		{
			return DialogResult.Abort;
		}
		set
		{
		}
	}
	
	// Get or set this LinkLabel's properties.
	[TODO]
	public Color ActiveLinkColor
	{
		get
		{
			return activeLinkColor;
		}
		set
		{
			activeLinkColor = value;
		}
	}
	[TODO]
	public Color DisabledLinkColor
	{
		get
		{
			return disabledLinkColor;
		}
		set
		{
			disabledLinkColor = value;
		}
	}
	[TODO]
	public LinkArea LinkArea
	{
		get
		{
			return linkArea;
		}
		set
		{
			linkArea = value;
		}
	}

	[TODO]
	public LinkBehavior LinkBehavior
	{
		get
		{
			return linkBehavior;
		}
		set
		{
			linkBehavior = value;
		}
	}

	[TODO]
	public Color LinkColor
	{
		get
		{
			return linkColor;
		}
		set
		{
			linkColor = value;
		}
	}
	
	[TODO]
	public LinkCollection Links
	{
		get
		{
			return links;
		}
		set
		{
			links = value;
		}
	}

	[TODO]
	public override string Text
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
	public Color VisitedLinkColor
	{
		get
		{
			return visitedLinkColor;
		}
		set
		{
			visitedLinkColor = value;
		}
	}
	
	[TODO]
	public bool LinkVisited
	{
		get
		{
			return linkVisited;
		}
		set
		{
			linkVisited = value;
		}
	}

	[TODO]
	public event LinkLabelLinkClickedEventHandler LinkClicked
	{
		add
		{
		}
		remove
		{
		}
	}
	
	[TODO]
	protected override AccessibleObject CreateAccessibilityInstance()
	{
		return null;
	}
	
	[TODO]
	protected override void CreateHandle()
	{
	}

	[TODO]
	protected override void OnEnabledChanged(EventArgs e)
	{
	}
 
	[TODO]
	protected override void OnFontChanged(EventArgs e)
	{
	}
	
	[TODO]
	protected override void OnGotFocus(EventArgs e)
	{
	}

	[TODO]
	protected override void OnKeyDown(KeyEventArgs e)
	{
	}

	[TODO]
	protected virtual void OnLinkClicked(LinkLabelLinkClickedEventArgs e)
	{
	}

	[TODO]
	protected override void OnLostFocus(EventArgs e)
	{
	}

	[TODO]
	protected override void OnMouseDown(MouseEventArgs e)
	{
	}

	[TODO]
	protected override void OnMouseLeave(EventArgs e)
	{
	}

	[TODO]
	protected override void OnMouseMove(MouseEventArgs e)
	{
	}

	[TODO]
	protected override void OnMouseUp(MouseEventArgs e)
	{
	}

	[TODO]
	protected override void OnPaint(PaintEventArgs e)
	{
	}

	[TODO]
	protected override void OnPaintBackground(PaintEventArgs e)
	{
	}
	
	[TODO]
	protected override void OnTextAlignChanged(EventArgs e)
	{
	}

	[TODO]
	protected override void OnTextChanged(EventArgs e)
	{
	}

	[TODO]
	protected Link PointInLink(int x, int y)
	{
		return null;
	}
				
	[TODO]
	protected override bool ProcessDialogKey(Keys keyData)
	{
		return false;
	}

	[TODO]
	protected override void Select(bool directed, bool forward)
	{
	}

	[TODO]
	protected override void SetBoundsCore(int x, int y, int width, int height,
																				BoundsSpecified specified)
	{

	}

	// IButtonControl public methods
	[TODO]
	public virtual void NotifyDefault(bool value) 
	{
	}

	[TODO]
	public virtual void PerformClick()
	{
	}
	
	[TODO]
	public class Link 
	{
		private bool enabled;
		private int length;
		private object linkData;
		private int start;
		private bool visited;

		[TODO]
		public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
			}
		}

		[TODO]
		public int Length
		{
			get
			{
				return length;
			}
			set
			{
				length = value;
			}
		}

		[TODO]
		public object LinkData
		{
			get
			{
				return linkData;
			}
			set
			{
				linkData = value;
			}
		}

		[TODO]
		public int Start
		{
			get
			{
				return start;
			}
			set
			{
				start = value;
			}
		}

		[TODO]
		public bool Visited
		{
			get
			{
				return visited;
			}
			set
			{
				visited = value;
			}
		}
		
	}

	[TODO]
	public class LinkCollection : IList
	{
		private int count;
		private bool isReadOnly;
		
		[TODO]
		public LinkCollection(LinkLabel owner)
		{
		}

		[TODO]
		public virtual int Count
		{
			get
			{
				return count;
			}
		}

		[TODO]
		public virtual bool IsReadOnly
		{
			get
			{
				return isReadOnly;
			}
		}

		[TODO]
		public virtual LinkLabel.Link this[int index]
		{
			get 
			{
				return null;
			}
			set
			{
			}
		}

		[TODO]
		public Link Add(int start, int length)
		{
			return null;
		}
		
		[TODO]
		public Link Add(int start, int length, object linkData)
		{
			return null;
		}

		[TODO]
		public virtual void Clear()
		{
		}

		[TODO]
		public bool Contains(LinkLabel.Link link)
		{
			return false;
		}

		// Implement from IEnumerable
		[TODO]
		public virtual IEnumerator GetEnumerator()
		{
			return null;
		}

		// Implement from ICollection
		[TODO]
		public virtual void CopyTo(Array array, int index)
		{
		}

		[TODO]
		public bool IsSynchronized
		{
			get 
			{
				return false;
			}
		}

		[TODO]
		public object SyncRoot
		{
			get
			{
				return null;
			}
		}

		// Implement from IList
		[TODO]
		public int Add(object value)
		{
			return 0;
		}


		[TODO]
		public bool Contains(object value)
		{
			return false;
		}

		[TODO]
		public int IndexOf(object value)
		{
			return 0;
		}

		[TODO]
		public void Insert(int index, object value)
		{
		}

		[TODO]
		public void Remove(object value)
		{
		}

		[TODO]
		public void RemoveAt(int index)
		{
		}

		[TODO]
		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		[TODO]
		object IList.this[int index]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
	}
	
	#if !CONFIG_COMPACT_FORMS
	// Process a message. 
	protected override void WndProc(ref Message m)
	{
		base.WndProc(ref m);
	}

	#endif // !CONFIG_COMPACT_FORMS

}; // class LinkLabel

}; // namespace System.Windows.Forms
