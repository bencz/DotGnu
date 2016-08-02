/*
 * Control.cs - Implementation of the
 *                      "System.Windows.Forms.StatusBar" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * Contributions from Simon Guindon
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

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace System.Windows.Forms
{
	public class StatusBar : Control
	{
		public event StatusBarDrawItemEventHandler DrawItem;
		public event StatusBarPanelClickEventHandler PanelClick;
		
		private StatusBarPanelCollection panels;
		private bool showPanels;
		private bool sizingGrip;

		public StatusBar()
		{
			Name = "StatusBar";
			Height = 22;
			Dock = DockStyle.Bottom;
			showPanels = false;
			sizingGrip = true;
			BackColor = SystemColors.Control;
			panels = new StatusBarPanelCollection(this);
		}

		~StatusBar()
		{
			Dispose(false);
		}

		/// Clean up any resources being used.
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
			}
			base.Dispose(disposing);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			if (showPanels == false)
			{
				DrawSimpleText(e, 0, 0, Width, Height, Text);
			}
			else
			{
				int left = 0;
				Border3DStyle style = Border3DStyle.Sunken;
				if (panels.Count <1)
				{
					int panelWidth;
					if (sizingGrip == true)
					{
						panelWidth = Width - 16;
					}
					else
					{
						panelWidth = Width;
					}
					ControlPaint.DrawBorder3D(g, 0, 2, panelWidth, Height - 2, Border3DStyle.SunkenOuter, Border3DSide.All);
				}
				else
				{
					for (int i=0;i<panels.Count;i++)
					{
						StatusBarPanel panel = panels[i];
						Icon icon = panel.Icon;
						int textLeft = left;

						switch (panel.BorderStyle)
						{
							case StatusBarPanelBorderStyle.None:
								style = Border3DStyle.Flat;
								break;
							case StatusBarPanelBorderStyle.Raised:
								style = Border3DStyle.Raised;
								break;
							case StatusBarPanelBorderStyle.Sunken:
								style = Border3DStyle.SunkenOuter;
								break;
						}
						ControlPaint.DrawBorder3D(g, left, 4, panel.Width, Height -4, style, Border3DSide.All);
						if (icon != null)
						{
							g.DrawIcon(icon, left, (Height - icon.Height) / 2);
							textLeft += icon.Width;
						}
						if (panel.Style == StatusBarPanelStyle.Text)
						{
							StringAlignment align = 0;
							switch (panel.Alignment)
							{
								case HorizontalAlignment.Center:
									align = StringAlignment.Center;
									break;
								case HorizontalAlignment.Left:
									align = StringAlignment.Near;
									break;
								case HorizontalAlignment.Right:
									align = StringAlignment.Far;
									break;
							}
							DrawSimpleText(e, textLeft, 4, left + panel.Width, Height, panel.Text, align);
						}
						else
						{
							// Owner drawn
							StatusBarDrawItemEventArgs args = new StatusBarDrawItemEventArgs(g, this.Font, new Rectangle(0, 0, panel.Width, Height), i, DrawItemState.None, panel);
							OnDrawItem(args);
						}
						left += panel.Width +2;
					}
				}
			}

			if (sizingGrip == true)
			{
				ControlPaint.DrawSizeGrip(g, BackColor, new Rectangle(Width - 16, Height - 16, Width, Height));
			}
			base.OnPaint(e);
		}

		private void DrawSimpleText(PaintEventArgs e, int left, int top, int right, int bottom, string text)
		{
			DrawSimpleText(e, left, top, right, bottom, text, StringAlignment.Near);
		}

		private void DrawSimpleText(PaintEventArgs e, int left, int top, int right, int bottom, string text, StringAlignment align)
		{
			// Draw the text within the statusbar.
			Font font = Font;
			RectangleF layout = (RectangleF)Rectangle.FromLTRB(left, top, right, bottom);

			StringFormat format = new StringFormat();
			format.Alignment = align;
			format.LineAlignment = StringAlignment.Center;

			if(text != null && text != String.Empty)
			{
				if(Enabled)
				{	
					Brush brush = new SolidBrush(ForeColor);					
					e.Graphics.DrawString(text, font, brush, layout, format);					
					brush.Dispose();
				}
				else
				{
					Brush brush = new SolidBrush(ForeColor);
					e.Graphics.DrawString(text, font, brush, layout, format);
					brush.Dispose();
				}
			}
		}

		public override Color BackColor
		{
			get { return base.BackColor; }
			set { base.BackColor = value; }
		}

		public override Image BackgroundImage
		{
			get { return base.BackgroundImage; }
			set { base.BackgroundImage = value; }
		}

		public override DockStyle Dock 
		{
			get { return base.Dock; }
			set { base.Dock = value; }
		}

		public override Font Font 
		{
			get { return base.Font; }
			set { base.Font = value; }
		}

	#if CONFIG_COMPONENT_MODEL
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	#endif
		public StatusBarPanelCollection Panels 
		{
			get { return panels; }
		}

		public bool ShowPanels 
		{
			get { return showPanels; }
			set 
			{
				showPanels = value; 
				Invalidate();
			}
		}

		public bool SizingGrip 
		{
			get { return sizingGrip; }
			set 
			{ 
				sizingGrip = value;
				Invalidate();
			}
		}

		public override string Text 
		{
			get { return base.Text; }
			set 
			{
				base.Text = value;
				Invalidate();
			}
		}

		protected override CreateParams CreateParams 
		{
			get { return base.CreateParams; }
		}

		protected override ImeMode DefaultImeMode 
		{
			get { return base.DefaultImeMode; }
		}

		protected override Size DefaultSize 
		{
			get { return base.DefaultSize; }
		}

		protected override void CreateHandle()
		{
			base.CreateHandle();
		}

		protected virtual void OnDrawItem(StatusBarDrawItemEventArgs e)
		{
			if (DrawItem != null)
			{
				DrawItem(this, e);
			}
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			base.OnHandleDestroyed(e);
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			int left = 0;

			for (int i=0;i < panels.Count;i++)
			{
				if (e.X >= left && e.X < left + panels[i].Width)
				{
					StatusBarPanelClickEventArgs args = new StatusBarPanelClickEventArgs(panels[i], e.Button, e.Clicks, e.X, e.Y);
					OnPanelClick(args);
				}
				left += panels[i].Width + 2;
			}
			base.OnMouseUp(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
		}

		protected virtual void OnPanelClick(StatusBarPanelClickEventArgs e)
		{
			if (PanelClick != null)
			{
				PanelClick(this, e);
			}
		}

		protected override void OnResize(EventArgs e)
		{
			Invalidate();
			base.OnResize(e);
		}

		public override String ToString()
		{
			return base.ToString() + " Panels.Count: " + panels.Count.ToString();
		}
	
		#if !CONFIG_COMPACT_FORMS
		// Process a message.
		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
		}
		#endif // !CONFIG_COMPACT_FORMS

		public class StatusBarPanelCollection : IList	
		{
			private StatusBar owner;
			private ArrayList list;

			public StatusBarPanelCollection(StatusBar owner)
			{
				this.owner = owner;
				list = new ArrayList();
			}

			// Implement the ICollection interface.
			void ICollection.CopyTo(Array array, int index)
			{
				list.CopyTo(array, index);
			}

			public virtual int Count
			{
				get { return list.Count; }
			}

			bool ICollection.IsSynchronized
			{
				get { return false; }
			}

			Object ICollection.SyncRoot
			{
				get { return this; }
			}

			// Implement the IEnumerable interface.
			public IEnumerator GetEnumerator()
			{
				return list.GetEnumerator();
			}

			// Determine if the collection is read-only.
			public bool IsReadOnly
			{
				get { return false; }
			}

			bool IList.IsFixedSize
			{
				get { return false; }
			}

			private static StatusBarPanel ValueAsPanel(object value)
			{
				if(value is StatusBarPanel)
				{
					return (StatusBarPanel)value;
				}
				throw new ArgumentException("value");
			}

			private static void CheckNull(StatusBarPanel value)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
			}

			private void CheckSame(StatusBarPanel value)
			{
				if(list.IndexOf(value) >= 0)
				{
					throw new ArgumentException("value");
				}
			}

			private void CheckAdd(StatusBarPanel value)
			{
				CheckNull(value);
				CheckSame(value);
			}

			// Get the array list that underlies this collection
			Object IList.this[int index]
			{
				get
				{
					return list[index];
				}
				set
				{
					this[index] = ValueAsPanel(value);
				}
			}

			public StatusBarPanel this[int index]
			{
				get
				{
					return (StatusBarPanel)(list[index]);
				}
				set
				{
					StatusBarPanel old = (StatusBarPanel)(list[index]);
					if(old == value)
					{
						return;
					}
					CheckAdd(value);
					list[index] = value;
					old.parent = null;
					value.parent = owner;
					owner.Invalidate();
				}
			}

			protected virtual ArrayList List
			{
				get { return list; }
			}

			int IList.Add(Object value)
			{
				return Add(ValueAsPanel(value));
			}

			public virtual int Add(StatusBarPanel value)
			{
				CheckAdd(value);
				int result = list.Add(value);
				value.parent = owner;
				owner.Invalidate();
				return result;
			}

			public virtual StatusBarPanel Add(string text)
			{
				StatusBarPanel panel = new StatusBarPanel();
				panel.Text = text;
				Add(panel);
				return panel;
			}

			public virtual void AddRange(StatusBarPanel[] panels)
			{
				for(int i = 0; i < panels.Length; i++)
				{
					StatusBarPanel value = panels[i];
					CheckAdd(value);
					value.parent = owner;
					list.Add(value);
				}
				owner.Invalidate();
			}

			public virtual void Clear()
			{
				for(int i = 0; i < list.Count; i++)
				{
					this[i].parent = null;
				}
				list.Clear();
				owner.Invalidate();
			}

			bool IList.Contains(Object value)
			{
				return list.Contains(value);
			}

			public bool Contains(StatusBarPanel panel)
			{
				return list.Contains(panel);
			}

			int IList.IndexOf(Object value)
			{
				return list.IndexOf(value);
			}

			public int IndexOf(StatusBarPanel panel)
			{
				return list.IndexOf(panel);
			}

			void IList.Insert(int index, Object value)
			{
				Insert(index, ValueAsPanel(value));
			}

			public virtual void Insert(int index, StatusBarPanel value)
			{
				CheckAdd(value);
				list.Insert(index, value);
				value.parent = owner;
				owner.Invalidate();
			}

			void IList.Remove(Object value)
			{
				Remove(ValueAsPanel(value));
			}

			public virtual void Remove(StatusBarPanel value)
			{
				CheckNull(value);
				int index = list.IndexOf(value);
				if(index >= 0)
				{
					RemoveAt(index);
				}
			}

			public void RemoveAt(int index)
			{
				StatusBarPanel value = (StatusBarPanel)(list[index]);
				value.parent = null;
				list.RemoveAt(index);
			}
		}
	}
}
