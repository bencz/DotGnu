/*
 * TabControl.cs - Implementation of the TabControl. Requires TabPage.cs.
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
 * 
 * The following are not complete:
 *	Proper alignment for bottom tabs
 *	Left and Right aligned tabs.
 *  Images in tabs.
 *  Disabled Control or disabled tabs.
 *	TabStyle of button or flat.
 *	Auto- repeat on the button when moving tabs left or right
 *  Tooltips
 *  Binding? Component?
 *  
 * At the moment drawing of the tabs is done by ControlPaint.DrawBorder3D.
 * This should be changed to allow better control over the placement of
 * the pixels.
 * Also all painting happens in the form paint event - if we painted
 * outside of this event, we could reduce flicker
 */

namespace System.Windows.Forms
{
	using System;
	using System.Drawing;
	using System.Collections;
	using d = System.Diagnostics.Debug;

	public class TabControl : ContainerControl
	{
		private TabAlignment alignment;
		private TabAppearance appearance;
		private TabDrawMode drawMode;
		private bool hotTrack;
		private ImageList imageList;
		private Size itemSize;
		private bool multiline;
		private Point padding;
		private int selectedIndex;
		private int prevSelectedIndex;
		private int hotTrackIndex;
		private bool showToolTips;
		private TabSizeMode sizeMode;
		private TabPageCollection tabPageCollection;
		private TabPositionInfo positionInfo;
		// Only if not multiline and too many tabs to display on a row
		private bool moveButtonsShowing;
		// Do the move buttons cover the last tab?
		private bool moveButtonsCovered;
		// The virtual start of the tabs if there are too many to show in one row
		private int moveButtonsTabOffset;
		// Tab move buttons when too many tabs on a row
		private Rectangle moveButtonLeftBounds;
		private ButtonState moveButtonLeftState;
		private Rectangle moveButtonRightBounds;
		private ButtonState moveButtonRightState;
		private Size moveButtonSize;

		// Margins for the text on a tab
		private const int tabTextWidthMargin = 5;
		private const int tabTextHeightMargin = 2;
		private const int minimumTabSize = 45;
		// Indent of the actual tabs from the side of the control
		private const int indent = 2;
		// Used to track first paint call, hack to mimmick MS implementation
		// of setting first added tab as active

		// Tab events
		public event DrawItemEventHandler DrawItem;
		public event EventHandler SelectedIndexChanged;
		public TabControl()
		{
			selectedIndex = -1;

			alignment = TabAlignment.Top;
			drawMode = TabDrawMode.Normal;
			itemSize = new Size(42, 21);
			padding = new Point(6, 3);
			moveButtonSize = new Size (17, 17);
			sizeMode = TabSizeMode.Normal;
			appearance = TabAppearance.Normal;
			tabPageCollection = new TabPageCollection(this);
			SetStyle(ControlStyles.UserPaint, true);

			moveButtonLeftState = ButtonState.Normal;
			moveButtonRightState = ButtonState.Normal;
			hotTrackIndex = -1;
			SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
		}

		public Control GetControl(int index)
		{
			return tabPageCollection[index];
		}

		protected string GetToolTipText(object item)
		{
			if( selectedIndex < 0 ) return string.Empty;
			return (tabPageCollection[selectedIndex] as TabPage).ToolTipText;
		}

		protected override Size DefaultSize
		{
			get
			{
				return new Size(200, 100);
			}
		}

		protected virtual void OnSelectedIndexChanged(EventArgs e)
		{
			//redundant check, is done is SelectedIndex already
			//if (selectedIndex == -1 || selectedIndex >= TabCount)
			//{
			//	return;
			//}
			SuspendLayout();
			Control prevPage = GetChildByIndex( prevSelectedIndex );
			if(prevPage != null)
			{
				prevPage.Visible = false;
			}

			Control selectedPage = GetChildByIndex( selectedIndex );
			selectedPage.Visible = true;
			SetTabPageBounds();
			
			if (SelectedIndexChanged != null)
			{
				SelectedIndexChanged( this, EventArgs.Empty );
			}
			ResumeLayout();
			InvalidateTabs();

		}

		protected void RemoveAll()
		{
			tabPageCollection.Clear();
		}
		
		public TabAlignment Alignment
		{
			get
			{
				return alignment;
			}
			set
			{
				alignment = value;
				PerformLayout(this, "Alignment");
			}
		}

		public TabAppearance Appearance
		{
			get
			{
				return appearance;
			}
			set
			{
				appearance = value;
				PerformLayout(this, "Appearance");
			}
		}

		public TabDrawMode DrawMode
		{ 
			get
			{
				return drawMode;
			} 
			set
			{
				drawMode = value;
				PerformLayout(this, "DrawMode");
			}
		}

		public bool HotTrack
		{ 
			get
			{
				return hotTrack;
			}
			set
			{
				if (value != hotTrack)
				{
					hotTrackIndex = -1;
					hotTrack = value;
					InvalidateTabs();
				}
			}
		}

		public ImageList ImageList
		{
			get
			{
				return imageList;
			}
			set
			{
				imageList = value;
				InvalidateTabs();
			}
		}

		public Size ItemSize {
			get
			{
				return itemSize;
			}
			set
			{
				itemSize = value;
				PerformLayout(this, "ItemSize");
			}
		}

		public bool Multiline 
		{ 
			get
			{
				return multiline;
			}
			set
			{
				multiline = value;
				PerformLayout(this, "Multiline");
			}
		}

		public Point Padding
		{
			get
			{
				return padding;
			}
			set
			{
				padding = value;
				PerformLayout(this, "Padding");
			}
		}

		public int RowCount
		{
			get
			{
				return PositionInfo.totalRows;
			}
		}

		public TabPage SelectedTab 
		{
			get
			{
				if (SelectedIndex == -1)	// use SelectedIndex instead of selectedIndex to active a first tab page, if none was selected
				{
					return null;
				}
				else
				{
					return (TabPage)tabPageCollection[selectedIndex];
				}
			}
			set
			{
				// use SelectedIndex, not selectedIndex to do events
				SelectedIndex = tabPageCollection.IndexOf(value);
			}
		}

		public bool ShowToolTips
		{
			get
			{
				return showToolTips;
			}
			set
			{
				showToolTips = value;
			}
		}

		public TabSizeMode SizeMode
		{
			get
			{
				return sizeMode;
			}
			set
			{
				sizeMode = value;
				PerformLayout(this, "SizeMode");
			}
		}

		public int TabCount {
			get
			{
				return tabPageCollection.Count;
			}
		}

		public TabPageCollection TabPages
		{
			get
			{
				return tabPageCollection;
			}
		}
		
		protected override Control.ControlCollection CreateControlsInstance()
		{
			return new TabChildControls( this );
		}

		public override String ToString()
		{
			string ret =  base.ToString() + ", TabPages.Count: " + TabCount.ToString();
			if(TabPages.Count > 0)
			{
				for(int cnt = 0; cnt < TabPages.Count; cnt++)
				{
					ret += ", TabPages[" + cnt + "]: TabPage: " + TabPages[cnt].Text;
				}
			}

			return ret;
		}
	
		// Collection of child control TabPages.
		public class TabChildControls : Control.ControlCollection
		{
			// Owning tab control.
			private TabControl tabOwner;

			public TabChildControls(TabControl owner) : base(owner)
			{
				this.tabOwner = owner;
			}

			public override void Add(Control control)
			{
				base.Add(control);
				tabOwner.InvalidateTabs	();
				tabOwner.SetTabPageBounds();
			}

			public override void Remove(Control control)
			{
				int newIdx = -1;
				base.Remove(control);
				tabOwner.InvalidateTabs	();
				tabOwner.SetTabPageBounds();
			}

			public override void Clear()
			{
				base.Clear();
				tabOwner.InvalidateTabs	();
				tabOwner.SetTabPageBounds();
			}
			
		};

		// Collection of child control TabPages.
		public class TabPageCollection : ICollection, IEnumerable, IList
		{
			// Owning tab control.
			private TabControl tabOwner;

			public TabPageCollection(TabControl owner)
			{
				this.tabOwner = owner;
			}

			public TabPage this[int idx]
			{
				get
				{
					return (TabPage)(tabOwner.Controls[idx]);
				}
				set
				{
					ArrayList tabpages = new ArrayList();
					this.RemoveAt(idx);
					for(int i = 0; i < tabOwner.Controls.Count; i++)
					{
						if(i != idx)
						{
							tabpages.Add(tabOwner.Controls[i]);
						}
					}
					tabpages.Insert(idx, value);
					tabOwner.Controls.Clear();
					tabOwner.Controls.AddRange((TabPage[])tabpages.ToArray(typeof(TabPage)));
					if(tabOwner.selectedIndex == idx)
					{
						((Control)(value)).Visible = true;
					}
				}
			}

			public void Add(TabPage control)
			{
				if(control == null)
				{
					throw new ArgumentNullException("control");
				}
				tabOwner.Controls.Add(control);
			}

			public void Remove(TabPage control)
			{
				if(control == null)
				{
					throw new ArgumentNullException("control");
				}
				tabOwner.Controls.Remove(control);
			}

			public void AddRange(TabPage[] pages)
			{
				if(pages == null)
				{
					throw new ArgumentException("pages");
				}
				foreach(TabPage page in pages)
				{
					Add(page);
				}
			}

			public bool Controls(TabPage control)
			{
				return tabOwner.Controls.Contains(control);
			}

			public bool Contains(TabPage control)
			{
				return tabOwner.Controls.Contains(control);
			}

			public int IndexOf(TabPage control)
			{
				return tabOwner.Controls.IndexOf(control);
			}

			// Implement the ICollection interface.
			void ICollection.CopyTo(Array array, int index)
			{
				tabOwner.Controls.CopyTo(array, index);
			}
			public int Count
			{
				get
				{
					return tabOwner.Controls.Count;
				}
			}
			bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}
			Object ICollection.SyncRoot
			{
				get
				{
					return this;
				}
			}

			// Implement the IEnumerable interface.
			public IEnumerator GetEnumerator()
			{
				return tabOwner.Controls.GetEnumerator();
			}

			// Implement the IList interface.
			int IList.Add(Object value)
			{
				if(!(value is TabPage))
				{
					throw new ArgumentException("value");
				}
				Add((TabPage)value);
				return ((IList)(tabOwner.Controls)).IndexOf(value);
			}
			public void Clear()
			{
				// tabOwner.RemoveAll();	// Brubbel recursion endless !!!
				tabOwner.Controls.Clear();
				tabOwner.selectedIndex = -1;
			}
			bool IList.Contains(Object value)
			{
				return ((IList)(tabOwner.Controls)).Contains(value);
			}
			int IList.IndexOf(Object value)
			{
				return ((IList)(tabOwner.Controls)).IndexOf(value);
			}
			void IList.Insert(int index, Object value)
			{
				throw new NotSupportedException();
			}
			void IList.Remove(Object value)
			{
				if(!(value is TabPage))
				{
					throw new ArgumentException("value");
				}
				Remove((TabPage)value);
			}
			public void RemoveAt(int index)
			{
				Remove(this[index]);
			}
			bool IList.IsFixedSize
			{
				get
				{
					return false;
				}
			}
			bool IList.IsReadOnly
			{
				get
				{
					return false;
				}
			}
			Object IList.this[int index]
			{
				get
				{
					return this[index];
				}
				set
				{
					this[index] = (TabPage)value;
				}
			}

		};

		protected override void OnPaint(PaintEventArgs e)
		{
			Draw(e.Graphics);
			// Draw the visible TabPage (child controls)
			base.OnPaint (e);
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			base.OnPaintBackground (e);
			using (Brush back = new SolidBrush(BackColor))
			{
				e.Graphics.FillRectangle(back, 0, 0, Width, Height);
			}
		}


		private void Draw(Graphics g)
		{
			// Exclude the moveButtons from the draw to prevent flicker.
			if (moveButtonsShowing)
			{
				moveButtonLeftBounds = moveButtonRightBounds = new Rectangle( Width - moveButtonSize.Width *2, 1, moveButtonSize.Width, moveButtonSize.Height);
				moveButtonRightBounds.Offset( moveButtonLeftBounds.Width, 0);
				using (SolidBrush b = new SolidBrush( Color.Black ) )
				{
					ControlPaint.DrawButton(g, moveButtonLeftBounds, moveButtonLeftState);
					// Left Arrow
					g.FillPolygon(b, new Point[]
					{
						new Point(moveButtonLeftBounds.Left + 5, moveButtonLeftBounds.Top + 8),
						new Point(moveButtonLeftBounds.Left + 8, moveButtonLeftBounds.Top + 5),
						new Point(moveButtonLeftBounds.Left + 8, moveButtonLeftBounds.Top + 11)
					});
					ControlPaint.DrawButton(g, moveButtonRightBounds, moveButtonRightState);
					// Right Arrow
					g.FillPolygon(b, new Point[]
					{
						new Point(moveButtonRightBounds.Left + 11, moveButtonRightBounds.Top + 8),
						new Point(moveButtonRightBounds.Left + 8, moveButtonRightBounds.Top + 5),
						new Point(moveButtonRightBounds.Left + 8, moveButtonRightBounds.Top + 11)
					});
					
				}
				Region r = new Region(moveButtonLeftBounds);
				r.Union(moveButtonRightBounds);
				g.SetClip(r, Drawing.Drawing2D.CombineMode.Exclude);
			}
			Region clip = g.Clip.Clone();

			Rectangle newClient = GetTabBaseBounds();
			// Draw the tab edging
			ControlPaint.DrawBorder3D(g, newClient, Border3DStyle.RaisedInner, Border3DSide.Left);
			ControlPaint.DrawBorder3D(g, newClient, Border3DStyle.Raised, Border3DSide.Bottom);
			ControlPaint.DrawBorder3D(g, newClient, Border3DStyle.Raised, Border3DSide.Right);
			ControlPaint.DrawBorder3D(g, newClient, Border3DStyle.RaisedInner, Border3DSide.Top);

			// Draw each tab in the tabControl row 0 first, bottom row last, selected item very last
			for( int row = 0; row < PositionInfo.totalRows; row++ )
			{
				for( int i = 0; i < tabPageCollection.Count; i++ )
				{
					if (row == PositionInfo.positions[i].row && i != SelectedIndex)
					{
						Rectangle bounds = GetTabRect(i);
						// Remove bottom line off bounds if not selected so border isnt covered
						g.SetClip(new Rectangle(bounds.Left, bounds.Top, bounds.Width, bounds.Height -1), Drawing.Drawing2D.CombineMode.Intersect);
						OnDrawItem( new DrawItemEventArgs(g, Font, bounds, i, DrawItemState.None, ForeColor, BackColor));
						g.Clip = clip;
					}
				}
			}
			if (SelectedIndex < TabPages.Count && SelectedIndex >= 0)
			{
				Rectangle bounds = GetTabRect(SelectedIndex);
				g.SetClip(new Rectangle(bounds.Left, bounds.Top, bounds.Width, bounds.Height), Drawing.Drawing2D.CombineMode.Intersect);
				
				DrawItemState state;
				if (Focused)
					state = DrawItemState.Focus;// | DrawItemState.Selected;
				else
					state = DrawItemState.Default; //DrawItemState.Selected;
				OnDrawItem( new DrawItemEventArgs(g, Font, bounds, SelectedIndex, state, ForeColor, BackColor));
			}

		}

		public void DrawButton(int index)
		{
			if (index == -1 || index >= TabPages.Count)
				return;
			Invalidate(new Region( positionInfo.positions[index].bounds ));
		}

		public Rectangle GetTabRect( int index )
		{
			return PositionInfo.positions[index].bounds;
		}
			
		// Get the number of rows and the positions of each tab
		// This property is used everywhere for tab position and size info
		private TabPositionInfo PositionInfo
		{
			get
			{
				if (positionInfo == null)
				{
					if (tabPageCollection.Count == 0)
						positionInfo = TabPositionInfo.Empty;
					else
					{
						TabPosition[] tabs = new TabPosition[tabPageCollection.Count];

						// Find the width of a row
						int rowWidth;
						if (alignment == TabAlignment.Top || alignment == TabAlignment.Bottom)
							rowWidth = Width - indent * 2;
						else
							rowWidth = Height - indent * 2;
		
						int maxRow = 0;
						// Size the tabs based on the TabSizeMode
						using ( Graphics graphics = CreateGraphics() )
						{
							if (sizeMode == TabSizeMode.Normal || (sizeMode == TabSizeMode.FillToRight && !multiline ))
								SizeTabsNormal( ref tabs, graphics, rowWidth, out maxRow, false );
							else if (sizeMode == TabSizeMode.Fixed)
								SizeTabsNormal ( ref tabs, graphics, rowWidth, out maxRow, true );
							else if (sizeMode == TabSizeMode.FillToRight)
								SizeTabsFillRight ( ref tabs, graphics, rowWidth, out maxRow );
						}
						// Do we need to move the row that was selected to the bottom of the tabs?
						// why is bounds checked here again? its redundant.
						//if (SelectedIndex < TabCount)
						//{
							// Check to see if we have selected a tab that isnt on the last row and move the tab row down
						if( selectedIndex >= 0 && selectedIndex < tabs.Length) {
							if (tabs[selectedIndex].row != maxRow)
								RowToBottom(ref tabs, tabs[selectedIndex].row, maxRow);
						}
						//}

						// Find the actual bounds
						LayoutTabBounds( ref tabs, rowWidth );
						positionInfo = new TabPositionInfo(maxRow + 1, tabs);
					}
				}
				return positionInfo;
			}
		}

		// Size the tabs for SizeMode normal or fixed
		private void SizeTabsNormal(ref TabPosition[] tabs, Graphics g, int rowWidth, out int row, bool fixedSize)
		{
			int pos = 0;
			row = 0;
			
			for( int i=0;i < tabs.Length; i++)
			{
				if ((tabPageCollection[i] as TabPage).Visible)
				{
					int measuredWidth;
					if (fixedSize)
						measuredWidth = itemSize.Width;
					else
						measuredWidth = (int)g.MeasureString(tabPageCollection[i].Text, Font).Width + tabTextWidthMargin * 2;
					if (measuredWidth < minimumTabSize)
						measuredWidth = minimumTabSize;
					// Are we out of room on that row?
					if (pos + measuredWidth > rowWidth && multiline)
					{
						row++;
						pos = 0;
					}

					tabs[i]=new TabPosition( new Rectangle( pos, 0, measuredWidth, 0 ), row, false );
					pos += measuredWidth;
				}
			}
		}

		// Size the tabs for SizeMode fillRight
		private void SizeTabsFillRight(ref TabPosition[] tabs, Graphics g, int rowWidth, out int maxRow)
		{
			// Start by sizing as per normal
			SizeTabsNormal( ref tabs, g, rowWidth, out maxRow, false );

			// The details of the row/tab for the smallest padding + tab width
			// Used to "fill" the last tab if required for the optimal FillRight packing.
			int minPadded = int.MaxValue;
			int minTabWidth = 0;
			int minTabPos = -1;

			int currentPos = 0;
			for ( int row = 0; row <= maxRow; row++ )
			{
				int thisPadded;
				int thisMinTabPos;
				int thisMinTabWidth;
					
				// For the last row we might want to move a tab in to improve the packing
				if (row == maxRow)
				{
					// Make a copy of the last row of tabs in case we need to redo that line
					TabPosition[] tabsCopy = (TabPosition[])tabs.Clone();

					int startOfLastRow = currentPos;
					PadRow( ref tabs, ref currentPos, out thisPadded, row, rowWidth, out thisMinTabPos, out thisMinTabWidth );

					// Would it be optimal to pack with our best tab?
					if (minTabPos > -1 && thisPadded >= minPadded + minTabWidth)
					{
						// Find the beginning and end of the row that we are removing the tab from.
						int startPos = -1;
						int posEnd = -1;
						int currentRow = tabs[minTabPos].row;
						for (int i = 0; i < tabs.Length; i++)
						{
							if (tabs[i].row == currentRow)
							{
								if (startPos == -1)
									startPos = i;
								posEnd = i;
							}
						}

						// Create a temporary array for the row less the tab we are moving
						TabPosition[] tabs1 = new TabPosition[posEnd - startPos];
						int j = startPos;
						for (int i = 0; i < tabs1.Length; i++)
						{
							if (j == minTabPos)
								j++;
							tabs1[i] = tabs[j];
							j++;
						}

						// Recalculate this row
						PadTabs(ref tabs1, 0, tabs1.Length, tabs[minTabPos].bounds.Width);
						CalcTabPosFromWidths(ref tabs1, 0, tabs1.Length, currentRow);

						// Write this back into tabs.
						j = startPos;
						for (int i = 0; i < tabs1.Length; i++)
						{
							if (j == minTabPos)
								j++;
							tabs[j] = tabs1[i];
							j++;
						}
						
						// minTabPos now starts the last row
						tabsCopy[startOfLastRow - 1] = tabs[minTabPos];
						PadTabs(ref tabsCopy, startOfLastRow - 1, tabsCopy.Length, thisPadded - tabs[minTabPos].bounds.Width);
						CalcTabPosFromWidths(ref tabsCopy, startOfLastRow - 1, tabsCopy.Length, tabs[startOfLastRow].row);
						// Write the calculated values back to tabs.
						tabs[minTabPos] = tabsCopy[startOfLastRow - 1];
						tabs[minTabPos].row = tabs[startOfLastRow].row;
						for (int i = startOfLastRow; i < tabs.Length; i++)
							tabs[i] = tabsCopy[i];
						
					}
				}
				else
				{
					PadRow( ref tabs, ref currentPos, out thisPadded, row, rowWidth, out thisMinTabPos, out thisMinTabWidth );

					// Find the minimum Tabsize for this row
					if (thisPadded + thisMinTabWidth < minPadded + minTabWidth)
					{
						minPadded = thisPadded;
						minTabWidth = thisMinTabWidth;
						minTabPos = thisMinTabPos;
					}
				}
			}
		}

		// Pad tab widths evenly, from startPos and ending where total width is greater than the ideal
		private void PadRow (ref TabPosition[] tabs, ref int startPos, out int totalPadded, int newRow, int rowWidth, out int minTabPos, out int minTabWidth)
		{
			minTabWidth = int.MaxValue;
			minTabPos = -1;
			
			// Find the last tab that fits into this row and the number of pixels we have to fill.
			// Also find the smallest tab.
			totalPadded = rowWidth;
			int posEnd = startPos;
			for (; posEnd < tabs.Length; posEnd++)
			{
				if (!(tabPageCollection[posEnd] as TabPage).Visible)
					continue;

				int width = tabs[posEnd].bounds.Width;
				// We end the count if we have exceeded the ideal width
				if (totalPadded < width)
					break;
				totalPadded -= width;
				
				if (minTabWidth > width)
				{
					minTabWidth = width;
					minTabPos = posEnd;
				}		
			}

			PadTabs( ref tabs, startPos, posEnd, totalPadded);
			CalcTabPosFromWidths( ref tabs, startPos, posEnd, newRow);

			// We will start at this position next time
			startPos = posEnd;
		}

		// Write the x cooridinate of the tab from the width and write the row we want.
		private void CalcTabPosFromWidths(ref TabPosition[] tabs, int startPos, int posEnd, int newRow)
		{
			int posTotal = 0;
			for (int pos = startPos; pos < posEnd; pos++ )
			{
				if (!(tabPageCollection[pos] as TabPage).Visible)
					continue;
				int width = tabs[pos].bounds.Width;
				tabs[pos].bounds = new Rectangle(posTotal,0,width,0);
				tabs[pos].row = newRow;
				posTotal += width;
			}
		}

		// Add pixelsToPad to the widths of the tabs
		private void PadTabs(ref TabPosition[] tabs, int startPos, int posEnd, int pixelsToPad)
		{
			// Add 1 pixel to the smallest tab until the row is filled.
			while (pixelsToPad > 0)
			{
				// Find the smallest tab in the row
				int smallestPos = -1;
				int smallestWidth = int.MaxValue;
				for (int i = startPos; i < posEnd; i++ )
				{
					if (!(tabPageCollection[i] as TabPage).Visible)
						continue;

					if (tabs[i].bounds.Width < smallestWidth)
					{
						smallestPos = i;
						smallestWidth = tabs[i].bounds.Width;
					}
				}
				// If we have no visible tabs.
				if (smallestPos == -1)
					return;
				tabs[smallestPos].bounds = new Rectangle(0,0, smallestWidth + 1,0);
				pixelsToPad--;
			}
		}

		// Using the tabs row and the widths, calculate each tabs actual bounds
		private void LayoutTabBounds(ref TabPosition[] tabs, int rowWidth)
		{
			int down = ActualTabHeight;
			int top = 0;
			int yDirection = 1;
			// At the bottom, we start a row higher
			int extraRow = 0;
			if (alignment == TabAlignment.Bottom)
			{
				yDirection = -1;
				extraRow = 1;
				top = Height;
			}
			
			int lastVisible = -1;
			int leftOffset = 0;
			if (moveButtonsShowing)
				leftOffset = tabs[moveButtonsTabOffset].bounds.Left;
			int totalWidth = 0;
			for( int i=0;i < tabPageCollection.Count; i++)
			{
				if ((tabPageCollection[i] as TabPage).Visible)
				{
					// Move the tabs thats selected by 1
					int selDelta = 0;
					if (i != SelectedIndex)
						selDelta = 1;

					TabPosition tab = tabs[i];
					int width = tab.bounds.Width + (1-selDelta)*2*indent;
					// Draw the left edge of tabs that begin a row or are selected
					if (tab.bounds.Left - leftOffset == 0 || SelectedIndex == i)
						tab.leftExposed = true;
					tab.bounds = new Rectangle(tab.bounds.Left + indent - (1-selDelta)*2 - leftOffset, yDirection *((tab.row + extraRow ) * down + selDelta) + top, width, down + 1 - selDelta );
					tabs[i] = tab;
					lastVisible = i;
					totalWidth += width;
				}
			}
			moveButtonsShowing = !multiline && totalWidth > rowWidth;
			// Are the tab move buttons covered by a tab. This is used to disallow the tabs moving left anymore
			if (moveButtonsShowing)
				moveButtonsCovered = moveButtonLeftBounds.Left < tabs[lastVisible].bounds.Right;
					
		}

		// Move a particular row Row to the bottom of the tabs (lastRow)
		private void RowToBottom(ref TabPosition[] tabs, int row, int lastRow)
		{
			for (int i = 0; i < tabs.Length; i++)
			{
				if (tabs[i].row == row)
					tabs[i].row = lastRow;
				else if (tabs[i].row > row)
					tabs[i].row--;
			}
		}

		// All tab positions and the total rows
		private class TabPositionInfo
		{
			public int totalRows;
			public TabPosition[] positions;
			
			public TabPositionInfo( int totalRows, TabPosition[] positions)
			{
				this.totalRows = totalRows;
				this.positions = positions;
			}

			public static TabPositionInfo Empty
			{
				get 
				{
					return new TabPositionInfo(0, new TabPosition[0]);
				}
			}

		}

		// Each tab
		private struct TabPosition
		{
			public Rectangle bounds;
			public int row;
			public bool leftExposed;
			public TabPosition( Rectangle bounds, int row, bool leftExposed )
			{
				this.bounds = bounds;
				this.row = row;
				this.leftExposed = leftExposed;
			}
		}

		// The currently selected tab
		public int SelectedIndex {
			get
			{
				if( selectedIndex == -1 && tabPageCollection.Count > 0 ) {
					selectedIndex = 0;	// select first tab page without calling OnSelectedIndexChanged, like windows does.
					
					Control selectedPage = GetChildByIndex( selectedIndex );
					selectedPage.Visible = true;
					SetTabPageBounds();
					InvalidateTabs();
				}
				return selectedIndex;
			}
			set
			{
				if (value < 0 || value >= tabPageCollection.Count)
				{
					throw new IndexOutOfRangeException(value.ToString());
				}
				if (value != selectedIndex)
				{
					prevSelectedIndex = selectedIndex;
					selectedIndex = value;
					OnSelectedIndexChanged(EventArgs.Empty);
				}
			}
		}

		[TODO]
		// This occurs for each tab needing to be drawn
		protected virtual void OnDrawItem( DrawItemEventArgs e )
		{
			e.DrawBackground();
			Rectangle borderBounds = new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width, e.Bounds.Height+2);
			
			// Fix: If Appearance is tab then this ok, need to do for button & flat button
			// Draw tab edging & clip a border edge above the bottom
			
			DrawTab( e.Graphics, borderBounds, alignment, PositionInfo.positions[e.Index].leftExposed );

			// Owner Draw does their own drawing
			if (drawMode == TabDrawMode.Normal)
			{
				// Fix: If imageindex & imagelist then draw
				
				Rectangle textBounds = new Rectangle(e.Bounds.Left + tabTextWidthMargin, e.Bounds.Top + tabTextHeightMargin, e.Bounds.Width - tabTextWidthMargin*2, e.Bounds.Height - tabTextHeightMargin * 2);
				Rectangle focusBounds = textBounds;
				focusBounds.Inflate(1,1);

				if ((e.State & DrawItemState.Focus)>0)
					ControlPaint.DrawFocusRectangle(e.Graphics, focusBounds, ForeColor, BackColor);
				// Fix: Draw disabled
				DrawText( e.Graphics, tabPageCollection[e.Index].Text, textBounds, e.ForeColor, e.BackColor, hotTrack && e.Index == hotTrackIndex );
				
			}

			if(DrawItem != null)
			{
				DrawItem(this, e);
			}
		}

		// Draw the actual tab for each alignment.
		private void DrawTab( Graphics graphics, Rectangle bounds, TabAlignment alignment, bool leftEdge )
		{
			Border3DSide left = Border3DSide.Left;
			Border3DSide top = Border3DSide.Top;
			Border3DSide right = Border3DSide.Right;
			if (alignment == TabAlignment.Bottom)
				top = Border3DSide.Bottom;
			if ( leftEdge )
				ControlPaint.DrawBorder3D(graphics, bounds, Border3DStyle.RaisedInner, left);
			ControlPaint.DrawBorder3D(graphics, bounds, Border3DStyle.RaisedInner, top);
			ControlPaint.DrawBorder3D(graphics, bounds, Border3DStyle.Raised, right);
		}

		// Draw the text on the tab using the right colors
		private void DrawText(Graphics graphics, string text, Rectangle bounds, Color foreColor, Color backColor, bool hotTrack)
		{

			Color color = hotTrack ? SystemColors.HotTrack : ForeColor;
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;
			format.FormatFlags = StringFormatFlags.NoWrap;
			using (Brush brush = new SolidBrush( color )) 
			{
				graphics.DrawString( text, Font, brush, bounds, format );
			}
		}

		protected override bool IsInputKey(Keys keyData)
		{
			if ((keyData & Keys.Alt) == 0)
			{
				Keys key = keyData & Keys.KeyCode;
				if (key == Keys.Prior || key == Keys.Next || key == Keys.End || key == Keys.Home)
					return true;
			}
			return base.IsInputKey(keyData); 
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			// Ctrl Tab and Ctrl Shift Tab to cycle between tabs.
			if (e.KeyCode == Keys.Tab && (e.KeyData & Keys.Control) != 0)
			{
				if (SelectedIndex != -1)
				{
					if ((e.KeyData & Keys.Shift) == 0)
						SelectedIndex = (SelectedIndex + 1) % TabCount;
					else
						SelectedIndex = (SelectedIndex + TabCount - 1) % TabCount;
					e.Handled = true;
				}
			}
			base.OnKeyDown(e);
		}

		// Select the tab that is clicked
		protected override void OnMouseDown(MouseEventArgs e)
		{
			// select tabs only on left mouse button
			if( e.Button == MouseButtons.Left ) {
				if (moveButtonsShowing && moveButtonLeftBounds.Contains( e.X, e.Y ))
				{
					moveButtonLeftState = ButtonState.Pushed;
					if (moveButtonsTabOffset > 0)
					{
						moveButtonsTabOffset--;
						InvalidateTabs();
					}
				}
				else if (moveButtonsShowing && moveButtonRightBounds.Contains(e.X, e.Y))
				{
					moveButtonRightState = ButtonState.Pushed;
					if (moveButtonsTabOffset < tabPageCollection.Count - 1 && moveButtonsCovered)
					{
						moveButtonsTabOffset++;
						InvalidateTabs();
					}
				}
				else
				{
					moveButtonRightState = ButtonState.Normal;
					int newSelectedIndex =  GetMouseOverTab( e.X, e.Y );
					if (newSelectedIndex > -1)
					{
						SelectedIndex = newSelectedIndex;
						
						// Handle focus.
						if (!Focused)
						{
							if (!SelectedTab.SelectNextControl(null, true, true, false, false))
							{
								IContainerControl container = Parent.GetContainerControl();
								if (container != null)
								{
									container.ActiveControl = this;
								}
							}
						}
	
					}
				}
			}
			
			base.OnMouseDown (e);

		}

		protected override void OnLeave(EventArgs e)
		{
			// Redraw the selected button to clear the focus.
			DrawButton(selectedIndex);
			base.OnLeave (e);
		}

		protected override void OnEnter(EventArgs e)
		{
			// Redraw the selected button to set the focus.
			DrawButton(selectedIndex);
			base.OnEnter (e);
		}

		protected override bool ProcessKeyPreview(ref Message m)
		{
			if (ProcessKeyEventArgs(ref m))
			{
				return true; 
			}
			return base.ProcessKeyPreview(ref m); 
		}


		// Find which tab the mouse is over
		private int GetMouseOverTab(int x, int y)
		{
			TabPosition[] tabs = PositionInfo.positions;
			for( int i = 0; i < tabs.Length; i++ )
			{
				if ( tabs[i].bounds.Contains( x , y ) )
					return i;
			}
			return -1;
		}

		// Reset the tab move buttons
		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (moveButtonLeftState != ButtonState.Normal)
			{
				moveButtonLeftState = ButtonState.Normal;
				Invalidate(moveButtonLeftBounds);
			}
			if (moveButtonRightState != ButtonState.Normal)
			{
				moveButtonRightState = ButtonState.Normal;
				Invalidate(moveButtonRightBounds);
			}
			base.OnMouseUp (e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (hotTrack)
			{
				int prevHotTrackIndex = hotTrackIndex;
				hotTrackIndex = GetMouseOverTab( e.X, e.Y );
				// Dont draw more than we have to
				if (hotTrackIndex > -1 && hotTrackIndex != prevHotTrackIndex)
				{
					Region r = new Region(positionInfo.positions[hotTrackIndex].bounds);
					if (prevHotTrackIndex > -1)
						r.Union( positionInfo.positions[prevHotTrackIndex].bounds );
					Invalidate(r);
				}
				else if (hotTrackIndex == -1 && prevHotTrackIndex > -1)
					Invalidate(new Region( positionInfo.positions[prevHotTrackIndex].bounds ));

			}
			base.OnMouseMove (e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			if (hotTrack && hotTrackIndex > -1)
			{
				int prevHotTrackIndex = hotTrackIndex;
				hotTrackIndex = -1;
				Invalidate( new Region( positionInfo.positions[prevHotTrackIndex].bounds ));
			}
			base.OnMouseLeave (e);
		}

		// Display Area of the tab pages
		public override Rectangle DisplayRectangle 
		{
			get
			{
				if (toolkitWindow == null)
					return ClientRectangle;
				Rectangle rect = GetTabBaseBounds();
				return new Rectangle( rect.Left + padding.X, rect.Top + padding.Y, rect.Width - 2* padding.X, rect.Height - 2 * padding.Y );
			}
		}

		// Each tab height the excluding border
		private int ActualTabHeight
		{
			get
			{
				return  ItemSize.Height;
			}
		}

		// Returns the dimensions of the base (piece without tabs)
		private Rectangle GetTabBaseBounds()
		{
			int offset = 0;
			if (IsHandleCreated)
				offset = TabOffset;
			switch( alignment )
			{
				case TabAlignment.Left:
					return new Rectangle( offset, 0, Width - offset, Height );
				case TabAlignment.Top:
					return new Rectangle( 0, offset, Width, Height - offset );
				case TabAlignment.Right:
					return new Rectangle( 0, 0, Width - offset, Height );
				case TabAlignment.Bottom:
					return new Rectangle( 0, 0, Width, Height - offset );
				default:
					return Rectangle.Empty;
			}
		}

		// Height in pixels of tab
		private int TabOffset
		{
			get
			{
				return ActualTabHeight * PositionInfo.totalRows;
			}
		}

		// Returns the dimensions of the tab area
		private Rectangle GetTabsBounds()
		{
			int offset = 0;
			if (IsHandleCreated)
				offset = TabOffset;
			switch( alignment )
			{
				case TabAlignment.Left:
					return new Rectangle( 0, 0, offset, Height );
				case TabAlignment.Top:
					return new Rectangle( 0, 0, Width, offset );
				case TabAlignment.Right:
					return new Rectangle( Width - offset, 0, offset, Height );
				case TabAlignment.Bottom:
					return new Rectangle( 0, Height - offset, Width, offset );
				default:
					return Rectangle.Empty;
			}
		}

		internal void InvalidateTabs()
		{
			// Brubbel
			int newIndex = selectedIndex;
			if( newIndex != -1 ) {
				if( newIndex >= tabPageCollection.Count ) {
					newIndex = tabPageCollection.Count-1;
				}
				if( newIndex != -1 ) {
					this.SelectedIndex = newIndex;
				}
			} 

			positionInfo = null;
			if (!IsHandleCreated)
				return;
			Rectangle bounds = GetTabsBounds();
			// Because we overwrite the border of the tab base, we need to draw more than the tab base bounds
			switch (alignment)
			{
				case TabAlignment.Left:
					bounds = new Rectangle(bounds.Left, bounds.Top, bounds.Width + 1, bounds.Height);
					break;
				case TabAlignment.Top:
					bounds = new Rectangle(bounds.Left, bounds.Top, bounds.Width, bounds.Height + 1);
					break;
				case TabAlignment.Right:
					bounds =  new Rectangle(bounds.Left - 1, bounds.Top, bounds.Width + 1, bounds.Height);
					break;
				case TabAlignment.Bottom:
					bounds =  new Rectangle(bounds.Left, bounds.Top - 1, bounds.Width, bounds.Height + 1);
					break;
			}
			Invalidate(bounds);
			// Make sure the tabs are drawn before the tab page (looks better)
			Update();
		}

		// The event thats called when controls need to relayout their contents
		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout (e);
			SetTabPageBounds();
		}

		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			base.SetBoundsCore (x, y, width, height, specified);
			if (!IsHandleCreated)
			{
				return;
			}
			// Force the positions of the tabs to update if the size changes
			if ((specified & BoundsSpecified.Size) != 0)
			{
				InvalidateTabs();
				SetTabPageBounds();
				Invalidate(false);
			}
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			SetTabPageBounds();
		}

		// This causes the child tab pages to have their bounds set to DisplayRectangle.
		private void SetTabPageBounds()
		{
			foreach( TabPage tabPage in tabPageCollection)
			{
				if (tabPage.Visible)
				{
					tabPage.Bounds = Rectangle.Empty;
				}
			}
		}

	}
}
