/*
 * TreeView.cs - Implementation of the
 *			"System.Windows.Forms.TreeView" class.
 *
 * Copyright (C) 2003 Neil Cawse
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
	using System.Drawing.Drawing2D;

	public class TreeView : Control
	{
		internal bool checkBoxes;
		internal const int checkSize = 13;
		internal TreeNode editNode;
		private bool fullRowSelect;
		private bool hideSelection;
		private bool hotTracking;
		private const int hScrollBarPixelsScrolled = 3;
		internal int imageIndex = 0;
		internal ImageList imageList;
		// No of pixels on the right of an image.
		internal const int imagePad = 5;
		internal int indent;
		private int itemHeight;
		private bool labelEdit;
		private Timer mouseClickTimer;
		private const int mouseEditTimeout = 350;
		internal TreeNodeCollection nodes;
		private TreeNode nodeToEdit = null;
		private string pathSeparator = @"\";
		internal TreeNode root;
		private bool scrollable;
		internal int selectedImageIndex = 0;
		internal TreeNode selectedNode;
		private bool showLines;
		private bool showPlusMinus;
		private bool showRootLines;
		private bool sorted;
		private TextBox textBox;
		// The node currently at the top of the control
		internal TreeNode topNode;
		private int updating;
		private VScrollBar vScrollBar;
		private HScrollBar hScrollBar;
		// Offset of tree view by scrolling.
		internal int xOffset = 0;
		const int xPadding = 2;
		int xScrollValueBeforeEdit = 0;
		ClickEvent clickEvent;

		public event TreeViewEventHandler AfterCheck
		{
			add
			{
				AddHandler(EventId.AfterCheck, value);
			}
			remove
			{
				RemoveHandler(EventId.AfterCheck, value);
			}
		}

		public event TreeViewEventHandler AfterCollapse
		{
			add
			{
				AddHandler(EventId.AfterCollapse, value);
			}
			remove
			{
				RemoveHandler(EventId.AfterCollapse, value);
			}
		}

		public event TreeViewEventHandler AfterExpand
		{
			add
			{
				AddHandler(EventId.AfterExpand, value);
			}
			remove
			{
				RemoveHandler(EventId.AfterExpand, value);
			}
		}

		public event NodeLabelEditEventHandler AfterLabelEdit
		{
			add
			{
				AddHandler(EventId.AfterLabelEdit, value);
			}
			remove
			{
				AddHandler(EventId.AfterLabelEdit, value);
			}
		}

		public event TreeViewEventHandler AfterSelect
		{
			add
			{
				AddHandler(EventId.AfterSelect, value);
			}
			remove
			{
				RemoveHandler(EventId.AfterSelect, value);
			}
		}

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

		public new event EventHandler BackgroundImageChanged
		{
			add
			{
				base.BackgroundImageChanged += value;
			}
			remove
			{
				base.BackgroundImageChanged -= value;
			}
		}

		public event TreeViewCancelEventHandler BeforeCheck
		{
			add
			{
				AddHandler(EventId.BeforeCheck, value);
			}
			remove
			{
				RemoveHandler(EventId.BeforeCheck, value);
			}
		}

		public event TreeViewCancelEventHandler BeforeCollapse
		{
			add
			{
				AddHandler(EventId.BeforeCollapse, value);
			}
			remove
			{
				RemoveHandler(EventId.BeforeCollapse, value);
			}
		}

		public event TreeViewCancelEventHandler BeforeExpand
		{
			add
			{
				AddHandler(EventId.BeforeExpand, value);
			}
			remove
			{
				RemoveHandler(EventId.BeforeExpand, value);
			}
		}

		public event NodeLabelEditEventHandler BeforeLabelEdit
		{
			add
			{
				AddHandler(EventId.BeforeLabelEdit, value);
			}
			remove
			{
				RemoveHandler(EventId.BeforeLabelEdit, value);
			}
		}

		public event TreeViewCancelEventHandler BeforeSelect
		{
			add
			{
				AddHandler(EventId.BeforeSelect, value);
			}
			remove
			{
				RemoveHandler(EventId.BeforeSelect, value);
			}
		}

		internal void BeginEdit(TreeNode node)
		{
			editNode = node;
			if (textBox == null)
			{
				textBox = new TextBox();
				textBox.BorderStyle = BorderStyle.FixedSingle;
				textBox.Visible = false;
				Controls.Add(textBox);
				textBox.Leave +=new EventHandler(textBox_Leave);
				textBox.KeyUp +=new KeyEventHandler(textBox_KeyUp);
			}
			textBox.Text = editNode.Text;
			Rectangle nodeBounds = node.Bounds;
			nodeBounds.Y -= 2;
			int y = nodeBounds.Y + (nodeBounds.Height - textBox.Height) /2;
			// Resize the text bounds to cover the area we want to clear.
			nodeBounds.X -= 2;
			nodeBounds.Height += 4;
			nodeBounds.Width += 4;

			if (hScrollBar != null)
			{
				xScrollValueBeforeEdit = hScrollBar.Value;
			}
			int x = nodeBounds.X;
			int width = GetTextBoxWidth(ref x);
			// The height is fixed by the textbox.
			textBox.SetBounds(x, y, width, 0);
			textBox.Visible = true;
			textBox.Focus();
			textBox.SelectAll();
			// Redraw to hide the node we are editing.
			Draw(editNode);
			
		}

		public void BeginUpdate()
		{
			updating++;
		}

		public BorderStyle BorderStyle 
		{ 
			get
			{
				return BorderStyleInternal;
			}
			set
			{
				BorderStyleInternal = value;
			}
		}
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
					Invalidate();
				}
			}
		}

		public void CollapseAll()
		{
			root.Collapse();
		}

		protected override void CreateHandle()
		{
			base.CreateHandle ();
		}

		protected override CreateParams CreateParams
		{
			get
			{
				return base.CreateParams;
			}
		}

		protected override Size DefaultSize
		{
			get
			{
				return new Size(121, 97);
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose (disposing);
		}

		// Render the treeview starting from startingLine
		internal void Draw(Graphics g, TreeNode startNode)
		{
			if (updating > 0)
			{
				return;
			}

			Rectangle clientRectangle = ClientRectangle;
			int drawableHeight = clientRectangle.Height;
			int drawableWidth = clientRectangle.Width - xOffset;

			// We count the visible rows to see if we need the v scrollbar but we wait before deciding if we need the h scroll bar.
			bool needsHScrollBar = false;
			bool needsVScrollBar = GetNeedVScrollBar() && scrollable;
			bool createNewVScrollBar = false;
			bool createNewHScrollBar = false;

			if (needsVScrollBar)
			{
				// Don't allow drawing on the area that is going to be the scroll bar.
				// Create the scroll bar so we can get its width.
				if (vScrollBar == null)
				{
					vScrollBar = new VScrollBar();
					createNewVScrollBar = true;
				}
				drawableWidth -= vScrollBar.Width;
				Rectangle rect = new Rectangle(drawableWidth + xOffset, 0, vScrollBar.Width, clientRectangle.Height);
				g.ExcludeClip(rect);
			}
			else
			{
				// Check to see if the top node is not the first node and we have room for the whole tree.
				// If so, abandon the draw and redraw the whole tree from the top.
				if (topNode != null && topNode != this.nodes[0])
				{
					topNode = null;
					Invalidate();
					return;
				}
				if (vScrollBar != null)
				{
					// We don't need the scroll bar anymore.
					Controls.Remove(vScrollBar);
					vScrollBar.Dispose();
					vScrollBar = null;
				}
			}
			// Is the node being processed on the screen.
			bool drawing = false;
			// Start counting from the top.
			int nodeFromTop = -1;
			// Number of nodes.
			int nodeCount = 0;
			int topNodePosition = 0;
			// The maximum width of a displayed node.
			int maxWidth = 0;
			StringFormat format = new StringFormat(StringFormatFlags.NoWrap);
			if (topNode == null && this.nodes.Count > 0)
			{
				topNode = this.nodes[0];
			}
			Rectangle textBounds = Rectangle.Empty;

			NodeEnumerator nodes = new NodeEnumerator(this.nodes);
			using (Pen markerPen = new Pen(SystemColors.ControlLight))
			{
				markerPen.DashStyle = DashStyle.Dot;
				while (nodes.MoveNext())
				{
					// If we havnt started drawing yet, then see if we need to and if so clear the background.
					if (!drawing)
					{
						if (nodes.currentNode  == topNode)
						{
							// We are at the top node.
							nodeFromTop = 0;
							topNodePosition = nodeCount;
						}
					
						// Check to see if we must start drawing. Clear the background.
						if (nodeFromTop >= 0 && (nodes.currentNode == startNode || startNode == root))
						{
							// Clear background.
							int y = ItemHeight * nodeFromTop;
							using (SolidBrush b = new SolidBrush(BackColor))
							{
								g.FillRectangle(b, 0, y, ClientSize.Width, ClientSize.Height - y);
							}
							drawing = true;
						}
					}

					// Even if we arnt drawing nodes yet, we need to measure if the nodes are visible, for hscrollbar purposes.
					if (nodeFromTop >= 0 && drawableHeight > 0)
					{
						textBounds = GetTextBounds(g, nodes.currentNode, nodeFromTop, nodes.level);
						// Is the text too wide to fit in - if so we need an h scroll bar.
						if (textBounds.Right > drawableWidth && !needsHScrollBar && scrollable)
						{
							needsHScrollBar = true;
							if (hScrollBar == null)
							{
								hScrollBar = new HScrollBar();
								createNewHScrollBar = true;
							}
							drawableHeight -= hScrollBar.Height;
							// Don't allow drawing on the area that is going to be the scroll bar.
							Rectangle rect = new Rectangle(0, clientRectangle.Height - hScrollBar.Height, clientRectangle.Width, hScrollBar.Height);
							g.ExcludeClip(rect);
						}
						if (textBounds.Right > maxWidth)
						{
							maxWidth = textBounds.Right;
						}

					}

					// Draw the node if we still have space.
					if (drawing && drawableHeight > 0)
					{
						Rectangle bounds;
						// Draw the lines and the expander.
						DrawExpanderMarker(g, markerPen, nodes.currentNode, nodeFromTop, nodes.level);
						// Draw checkboxes.
						if (checkBoxes)
						{
							bounds = GetCheckBounds(nodeFromTop, nodes.level);
							ButtonState state;
							if (nodes.currentNode.isChecked)
							{
								state = ButtonState.Checked;
							}
							else
							{
								state = ButtonState.Normal;
							}
							ControlPaint.DrawCheckBox(g, bounds, state);
						}
						// Draw the node image.
						if (imageList != null)
						{
							bounds = GetImageBounds(nodeFromTop, nodes.level);
							int index = GetDisplayIndex(nodes.currentNode );
						
							if (index < imageList.Images.Count)
							{
								Image image = imageList.Images[index];
								g.DrawImage(image, bounds.X, bounds.Y);
							
							}
						}
						bounds = textBounds;
						// The height may be too small now.
						// If we are currently editing a node then dont draw it.
						if (drawableHeight > 0 && nodes.currentNode != editNode)
						{
							// Draw the node text.
							if (nodes.currentNode  == selectedNode && (Focused || !hideSelection))
							{
								Rectangle r;

								// **TODO**
								// Running into a chicken and egg issue here where if
								// we fill the rect here, we're overdrawing the
								// checkBoxes or ExpanderMarker of above

								// Draw FullRowSelect if we qualify
								if(fullRowSelect && !showLines)
								{
									int left = 1;
//									if(nodes.currentNode)
//									{
//										left = 10;
//									}
									g.FillRectangle(SystemBrushes.Highlight, new Rectangle(left, bounds.Y, drawableWidth - 1, bounds.Height));
								}
								else
								{
									g.FillRectangle(SystemBrushes.Highlight, bounds);
								}
								g.DrawString(nodes.currentNode.Text, Font, SystemBrushes.HighlightText, bounds, format);

								// Draw the focus rectangle.
								if(FullRowSelect && !ShowLines)
								{
									r = new Rectangle(0, bounds.Y - 1, drawableWidth, bounds.Height + 1);
								}
								else
								{
									r = new Rectangle(bounds.X - 1, bounds.Y - 1, bounds.Width + 1, bounds.Height + 1);
								}
								ControlPaint.DrawFocusRectangle(g, r);
							}
							else
							{
								g.DrawString(nodes.currentNode .Text, Font, SystemBrushes.ControlText, bounds,format);
							}
						}
						drawableHeight -= ItemHeight;
					}

					if (nodeFromTop >= 0)
					{
						nodeFromTop++;
					}
					nodeCount++;
				}
			}
			// If we need a v scroll bar, then set it up.
			if (needsVScrollBar)
			{
				SetupVScrollBar(nodeCount, needsHScrollBar, createNewVScrollBar, topNodePosition);
			}
			if (needsHScrollBar)
			{
				SetupHScrollBar(needsVScrollBar, maxWidth, createNewHScrollBar, g);
			}
			else if (hScrollBar != null)
			{
				// We dont need the scroll bar.
				// If we have scrolled then we need to reset the position.
				if (xOffset != 0)
				{
					xOffset = 0;
					Invalidate();
				}
				Controls.Remove(hScrollBar);
				hScrollBar.Dispose();
				hScrollBar = null;
			}
		}

		// Draw from startNode downwards
		internal void Draw(TreeNode startNode)
		{
			if (!Created || !Visible)
				return;
			using (Graphics g = CreateGraphics())
			{
				Draw(g, startNode);
			}
		}

		private void DrawExpanderMarker(Graphics g, Pen markerPen, TreeNode node, int nodeFromTop, int level)
		{
			Rectangle bounds = GetExpanderBounds(nodeFromTop, level);
			int midX = bounds.X + 4;
			int midY = bounds.Y + bounds.Height / 2;
			int lineRightStart = midX;
			int lineTopEnd = midY;
			if (node.Nodes.Count > 0 && showPlusMinus)
			{
				g.DrawRectangle(SystemPens.ControlText, midX - 4, midY - 4, 8, 8 );
				g.DrawLine(SystemPens.ControlText, midX - 2, midY, midX + 2, midY);
				if (!node.IsExpanded)
					g.DrawLine(SystemPens.ControlText, midX, midY - 2, midX, midY + 2);
				lineRightStart += 6;
				lineTopEnd -= 6;
			}
			if (!showLines)
			{
				return;
			}
			// Draw the right lead line
			if (bounds.Right > lineRightStart)
				g.DrawLine(markerPen, lineRightStart, midY, bounds.Right, midY);
			// Draw the top lead line
			TreeNode lineNode = node.PrevNode;
			if (lineNode == null)
				lineNode = node.Parent;
			if (lineNode != null)
				g.DrawLine(markerPen, midX, lineNode.markerLineY, midX, lineTopEnd);
			if (node.Nodes.Count > 0)
				node.markerLineY = midY + 6;
			else
				node.markerLineY = midY;
		}

		internal void EndEdit(bool cancel)
		{
			if (!cancel)
			{
				editNode.Text = textBox.Text;
			}
			textBox.Visible = false;
			editNode = null;
			if (hScrollBar != null)
			{
				hScrollBar.Value = xScrollValueBeforeEdit;
			}
		}

		public void EndUpdate()
		{
			if (updating < 2)
			{
				updating = 0;
				Invalidate();
			}
			else
			{
				updating--;
			}
		}

		public void ExpandAll()
		{
			root.ExpandAll();
		}

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
					Invalidate();
				}
			}
		}
		protected OwnerDrawPropertyBag GetItemRenderStyles(TreeNode node, int state)
		{
			// TODO: Property Bag
			return null;
		}

		// Get the node image index to display depending on what is set.
		private int GetDisplayIndex( TreeNode node)
		{
			int index = 0;
			if (node == selectedNode)
			{
				if (node.SelectedImageIndex > -1)
					index = node.SelectedImageIndex;
				else if (selectedImageIndex > -1)
					index = selectedImageIndex;
				else if (this.imageIndex > -1)
					index = this.imageIndex;
			}
			else
			{
				if (node.ImageIndex > -1)
					index = node.ImageIndex;
				else if (this.imageIndex > -1)
					index = this.imageIndex;
			}
			return index;
		}

		// Returns true if we dont have vertical space to draw all the items.
		private bool GetNeedVScrollBar()
		{
			int fullNodes = VisibleCount;
			NodeEnumerator nodes = new NodeEnumerator(Nodes);
			while (nodes.MoveNext())
			{
				if (--fullNodes == 0)
				{
					return true;
				}
			}
			return false;
		}

		public TreeNode GetNodeAt(int x, int y)
		{
			int height = ItemHeight;
			int nodeFromTop = -1;
			TreeView.NodeEnumerator nodes = new TreeView.NodeEnumerator(this.nodes);
			while (nodes.MoveNext())
			{
				if (nodes.currentNode == topNode)
				{
					// We are now at the top of the control.
					nodeFromTop = 1;
				}
				if (nodeFromTop > -1)
				{
					if (y < height * nodeFromTop)
					{
						return nodes.currentNode;
					}
					nodeFromTop++;
				}
			}
			return null;
		}

		public TreeNode GetNodeAt(Point pt)
		{
			return GetNodeAt(pt.X, pt.Y);
		}

		// Return the bounds of a check given the node from the top and an x level.
		internal Rectangle GetCheckBounds(int nodeFromTop, int level)
		{
			if (!checkBoxes)
				return Rectangle.Empty;
			int height = ItemHeight;
			int y = height * nodeFromTop + (height - checkSize) / 2;
			int x = (level + 1) * indent - xOffset + xPadding;
			return new Rectangle(x, y, checkSize, checkSize);
		}

		// Return the bounds of an expander given the node from the top and an x level.
		internal Rectangle GetExpanderBounds(int nodeFromTop, int level)
		{
			int height = ItemHeight;
			int y = height * nodeFromTop;
			int x = level* indent - xOffset + xPadding;
			return new Rectangle(x, y, indent, height);
		}

		// Return the bounds of an image given the node from the top and an x level.
		internal Rectangle GetImageBounds(int nodeFromTop, int level)
		{
			int height = ItemHeight;
			int y = height * nodeFromTop + (height - imageList.ImageSize.Height) / 2;
			int x = (level + 1) * indent - xOffset + xPadding;
			// Add on the width of the checkBoxes if applicable.
			if (checkBoxes)
			{
				x += checkSize;
			}
			return new Rectangle(x, y, imageList.ImageSize.Width + imagePad, imageList.ImageSize.Height);
		}

		public Rectangle GetNodeBounds(TreeNode node)
		{
			if (node.parent != null)
			{
				int nodeFromTop = -1;
				TreeView.NodeEnumerator nodes = new TreeView.NodeEnumerator(this.nodes);
				while (nodes.MoveNext())
				{
					if (nodes.currentNode == topNode)
					{
						// We are at the top of the control.
						nodeFromTop = 0;
					}
					if (nodes.currentNode == node)
					{
						using(Graphics g = CreateGraphics())
						{
							return GetTextBounds(g, node, nodeFromTop, nodes.level);
						}
					}
					if (nodeFromTop >= 0)
					{
						nodeFromTop++;
					}
				}
			}
			return Rectangle.Empty;
		}

		public int GetNodeCount(bool includeSubTrees)
		{
			return root.GetNodeCount(includeSubTrees);
		}

		// Get the bounds of a node. Supply a Graphics to measure the text, the node being measured, the number of the node being measured in the list of those being shown, the number of the node that is the first to be displayed (topNode) and the level of x indent.
		internal Rectangle GetTextBounds(Graphics g, TreeNode node, int nodeFromTop, int level)
		{
			int height = ItemHeight;
			int y = height * nodeFromTop;
			// Calculate the basic offset from the level and the indent.
			int x = (level + 1) * indent - xOffset;
			// Add on the width of the image if applicable.
			if (imageList != null)
			{
				x += imageList.ImageSize.Width + imagePad;
			}
			// Add on the width of the checkBoxes if applicable.
			if (checkBoxes)
			{
				x += checkSize;
			}

			Font font;
			if (node.nodeFont == null)
			{
				font = Font;
			}
			else
			{
				font = node.nodeFont;
			}

			int width = (int)g.MeasureString(node.text, font).Width;
			if (width < 5)
				width = 5;
								
			return new Rectangle(x, y, width, height);
		}

		// Returns the width the text box should be based on the width of the text.
		private int GetTextBoxWidth(ref int x)
		{
			const int minTextBoxWidth = 40;
			const int extraSpacing = 10;
			using (Graphics g = textBox.CreateGraphics())
			{
				SizeF size = g.MeasureString(textBox.Text, Font);
				int width = (int)size.Width + extraSpacing;
				if (width < minTextBoxWidth)
				{
					width = minTextBoxWidth;
				}
				int maxPossibleWidth = ClientRectangle.Width;
				if (vScrollBar != null)
				{
					maxPossibleWidth -= vScrollBar.Width;
				}
				int maxWidth = maxPossibleWidth - x;

				if (width > maxWidth)
				{
					width = maxWidth;
					// Try and move the control over to allow more space for the textbox.
					if (hScrollBar != null)
					{
						int offsetBefore = xOffset;
						hScrollBar.Value = hScrollBar.Maximum - hScrollBar.LargeChange + 1;
						int move = xOffset - offsetBefore;
						width += move;
						x -= move;
						if (x < 0)
						{
							x = 0;
						}
						if (width > maxPossibleWidth - x)
						{
							width = maxPossibleWidth - x;
						}
						
					}
				}
				return width;
			}
		}

		public bool HideSelection
		{
			get
			{
				return hideSelection;
			}
			set
			{
				if (hideSelection != value)
				{
					hideSelection = value;
					Invalidate();
				}
			}
		}

		public bool HotTracking
		{
			get
			{
				return hotTracking;
			}
			set
			{
				if (value != hotTracking)
				{
					hotTracking = value;
				}
			}
		}

		private void hScrollBar_ValueChanged(object sender, EventArgs e)
		{
			xOffset = hScrollBar.Value * hScrollBarPixelsScrolled;
			Invalidate();
		}

		public int ImageIndex
		{
			get
			{
				return imageIndex;
			}
			set
			{
				if (value != imageIndex)
				{
					imageIndex = value;
					Invalidate();
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
				if (value != imageList)
				{
					imageList = value;
					Invalidate();
				}
			}
		}

		public int Indent
		{
			get
			{
				return indent;
			}
			set
			{
				if (value != indent)
				{
					indent = value;
					Invalidate();
				}
			}
		}

		// Invalidate from startNode down.
		internal void InvalidateDown(TreeNode startNode)
		{
			if (updating > 0 || this.nodes == null)
			{
				return;
			}
			
			// Find the position of startNode relative to the top node.
			int nodeFromTop = -1;
			TreeView.NodeEnumerator nodes = new TreeView.NodeEnumerator(this.nodes);
			while (nodes.MoveNext())
			{
				if (nodes.currentNode == topNode)
				{
					// We are at the top of the control.
					nodeFromTop = 0;
				}
				if (nodes.currentNode == startNode)
				{
					break;
				}
				if (nodeFromTop >= 0)
				{
					nodeFromTop++;
				}
			}
			// Calculate the y position of startNode.
			int y = nodeFromTop * ItemHeight;
			// Invalidate from this position down.
			// Start one pixel higher to cover the focus rectangle.
			Invalidate(new Rectangle(0, y - 1, ClientRectangle.Width, ClientRectangle.Height - y + 1));
		}

		protected override bool IsInputKey(Keys keyData)
		{
			if (editNode != null && (keyData & Keys.Alt) == 0)
			{
				Keys key = keyData & Keys.KeyCode;
				if (key == Keys.Return || key == Keys.Escape || key == Keys.Prior || key == Keys.Next || key == Keys.Home || key == Keys.End)
					return true;
			}
			return base.IsInputKey(keyData);
		}

		public event ItemDragEventHandler ItemDrag
		{
			add
			{
				AddHandler(EventId.ItemDrag, value);
			}
			remove
			{
				RemoveHandler(EventId.ItemDrag, value);
			}
		}

		public int ItemHeight
		{
			get
			{
				if (itemHeight == -1)
					return FontHeight + 3;
				return itemHeight;
			}
			set
			{
				if (value != itemHeight)
				{
					itemHeight = value;
					Invalidate();
				}
			}
		}

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

		// This handles the timeout on the click timer and begins an edit.
		private void mouseClickTimer_Tick(object sender, EventArgs e)
		{
			mouseClickTimer.Stop();
			if (nodeToEdit != null)
			{
				nodeToEdit.BeginEdit();
				nodeToEdit = null;
			}
		}
		
		public TreeNodeCollection Nodes
		{
			get
			{
				return nodes;
			}
		}

		protected internal virtual void OnAfterCheck(TreeViewEventArgs e)
		{
			TreeViewEventHandler handler = (TreeViewEventHandler)GetHandler(EventId.AfterCheck);
			if (handler != null)
			{
				handler(this,e);
			}
		}

		protected internal virtual void OnAfterCollapse(TreeViewEventArgs e)
		{
			TreeViewEventHandler handler = (TreeViewEventHandler)GetHandler(EventId.AfterCollapse);
			if (handler != null)
			{
				handler(this,e);
			}
		}
		
		protected internal virtual void OnAfterExpand(TreeViewEventArgs e)
		{
			TreeViewEventHandler handler = (TreeViewEventHandler)GetHandler(EventId.AfterExpand);
			if (handler != null)
			{
				handler(this,e);
			}
		}

		protected virtual void OnAfterLabelEdit(NodeLabelEditEventArgs e)
		{
			EventHandler handler = GetHandler(EventId.AfterLabelEdit) as EventHandler;
			if (handler != null)
			{
				handler(this,e);
			}
		}

		protected virtual void OnAfterSelect(TreeViewEventArgs e)
		{
			TreeViewEventHandler handler = (TreeViewEventHandler)GetHandler(EventId.AfterSelect);
			if (handler != null)
			{
				handler(this,e);
			}
		}

		protected internal virtual void OnBeforeCheck(TreeViewCancelEventArgs e)
		{
			TreeViewCancelEventHandler handler = (TreeViewCancelEventHandler)GetHandler(EventId.BeforeCheck);
			if (handler != null)
			{
				handler(this,e);
			}
		}

		protected internal virtual void OnBeforeCollapse(TreeViewCancelEventArgs e)
		{
			TreeViewCancelEventHandler handler = (TreeViewCancelEventHandler)GetHandler(EventId.BeforeCollapse);
			if (handler != null)
			{
				handler(this,e);
			}
		}

		protected internal virtual void OnBeforeExpand(TreeViewCancelEventArgs e)
		{
			TreeViewCancelEventHandler handler = (TreeViewCancelEventHandler)GetHandler(EventId.BeforeExpand);
			if (handler != null)
			{
				handler(this,e);
			}
		}

		protected virtual void OnBeforeLabelEdit(NodeLabelEditEventArgs e)
		{
			NodeLabelEditEventHandler handler = (NodeLabelEditEventHandler)GetHandler(EventId.BeforeLabelEdit);
			if (handler != null)
			{
				handler(this,e);
			}
		}

		protected virtual void OnBeforeSelect(TreeViewCancelEventArgs e)
		{
			TreeViewCancelEventHandler handler = (TreeViewCancelEventHandler)GetHandler(EventId.BeforeSelect);
			if (handler != null)
			{
				handler(this,e);
			}
		}

		protected override void OnEnter(EventArgs e)
		{
			base.OnEnter (e);
			if (nodes.Count > 0)
			{
				SelectedNode.Invalidate();
			}
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated (e);
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			base.OnHandleDestroyed (e);
		}

		protected virtual void OnItemDrag(ItemDragEventArgs e)
		{
			ItemDragEventHandler handler = (ItemDragEventHandler)GetHandler(EventId.ItemDrag);
			if (handler != null)
			{
				handler(this,e);
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (!e.Handled && checkBoxes && selectedNode != null && (e.KeyData & Keys.KeyCode) == Keys.Space)
			{
				TreeViewCancelEventArgs args = new TreeViewCancelEventArgs(selectedNode, false, TreeViewAction.ByKeyboard);
				this.OnBeforeCheck(args);
				if (!args.Cancel)
				{
					selectedNode.isChecked = !selectedNode.isChecked;
					selectedNode.Invalidate();
					this.OnAfterCheck(new TreeViewEventArgs(selectedNode, TreeViewAction.ByKeyboard));
				}
				e.Handled = true;
			}
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			base.OnKeyPress(e);
			// Swallow the space
			if (e.KeyChar == ' ')
			{
				e.Handled = true;
			}
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp (e);
			// Swallow the space
			if ((e.KeyData & Keys.KeyCode) == Keys.Space)
			{
				e.Handled = true;
			}
		}

		// Non Microsoft member.
		protected override void OnMouseDown(MouseEventArgs e)
		{
			nodeToEdit = null;
			if (e.Button == MouseButtons.Left)
			{
				int nodeFromTop = -1;
				// Iterate through all the nodes, looking for the bounds that match.
				TreeView.NodeEnumerator nodes = new TreeView.NodeEnumerator(this.nodes);
				while (nodes.MoveNext())
				{
					if (nodes.currentNode == topNode)
					{
						// We are now at the top of the control.
						nodeFromTop = 0;
					}
					if (nodeFromTop > -1)
					{
						if (GetExpanderBounds(nodeFromTop, nodes.level).Contains(e.X, e.Y))
						{
							nodes.currentNode.Toggle();
							break;
						}
						else if (GetCheckBounds(nodeFromTop, nodes.level).Contains(e.X, e.Y))
						{
							TreeViewCancelEventArgs args = new TreeViewCancelEventArgs(nodes.currentNode, false, TreeViewAction.ByMouse);
							OnBeforeCheck(args);
							if (!args.Cancel)
							{
								nodes.currentNode.isChecked = !nodes.currentNode.isChecked;
								OnAfterCheck(new TreeViewEventArgs(nodes.currentNode, TreeViewAction.ByMouse));
							}

							Invalidate(GetCheckBounds(nodeFromTop, nodes.level));
							break;
			
						}
						nodeFromTop++;
					}
				}
			}
			else
			{
				ProcessClick(e.X, e.Y, true, ClickEvent.None);
			}
			base.OnMouseDown (e);
		}

		// Non Microsoft member.
		protected override void OnMouseMove(MouseEventArgs e)
		{
			//TODO: Hot tracking.
			base.OnMouseMove (e);
		}

		// Non Microsoft member.
		protected override void OnMouseLeave(EventArgs e)
		{
			//TODO: Hot tracking.
			base.OnMouseLeave (e);
		}

		// Non Microsoft member.
		protected override void OnMouseUp(MouseEventArgs e)
		{
			ProcessClick(e.X, e.Y, (e.Button == MouseButtons.Right), clickEvent);

			base.OnMouseUp (e);
		}

		void ProcessClick(int x, int y, bool rightMouse, ClickEvent clickEvent)
		{
			int nodeFromTop = -1;
			int height = ItemHeight;
			using(Graphics g = CreateGraphics())
			{
				// Iterate through all the nodes, looking for the bounds that match.
				TreeView.NodeEnumerator nodes = new TreeView.NodeEnumerator(this.nodes);
				while (nodes.MoveNext())
				{
					if (nodes.currentNode == topNode)
					{
						// We are now at the top of the control.
						nodeFromTop = 0;
					}
					if (nodeFromTop > -1)
					{
						// Check if the y matches this node.
						if (y < height * (nodeFromTop + 1))
						{							
							bool allowEdit = false;
							bool allowSelect = true;
							// Raise click or double click when called from MouseUp
							if(clickEvent == ClickEvent.Click)
							{
								OnClick(EventArgs.Empty);
							}
							else if(clickEvent == ClickEvent.DoubleClick)
							{
								OnDoubleClick(EventArgs.Empty);
							}
							// Clicking the image can be used to select.
							if (imageList == null || !GetImageBounds(nodeFromTop, nodes.level).Contains(x, y))
							{
								// Clicking the text can be used to edit and select.
								// if false then the hierarchy marker must have been clicked.
								if (GetTextBounds(g, nodes.currentNode, nodeFromTop, nodes.level).Contains(x, y))
								{
									allowEdit = true;
								}
								else
								{
									allowSelect = false;
								}
							}
							if (SelectedNode == nodes.Current && mouseClickTimer != null && mouseClickTimer.Enabled && (allowEdit || allowSelect))
							{
								mouseClickTimer.Stop();
								nodeToEdit = null;
								nodes.currentNode.Toggle();
								return;
							}
							if (allowSelect || rightMouse)
							{
								if (selectedNode == nodes.Current)
								{
									if (labelEdit && allowEdit && !rightMouse)
									{
										nodeToEdit = nodes.currentNode;
									}
								}
								else
								{
									nodeToEdit = null;
									// Do the events.
									TreeViewCancelEventArgs eventArgs = new TreeViewCancelEventArgs(nodes.currentNode, false, TreeViewAction.ByMouse);
									OnBeforeSelect(eventArgs);
									if (!eventArgs.Cancel)
									{
										SelectedNode = nodes.currentNode;
										OnAfterSelect(new TreeViewEventArgs(nodes.currentNode));
										Focus();
									}
								}
								if (rightMouse)
								{
									return;
								}
								if (mouseClickTimer == null)
								{
									mouseClickTimer = new Timer();
									mouseClickTimer.Tick +=new EventHandler(mouseClickTimer_Tick);
									mouseClickTimer.Interval = mouseEditTimeout;
								}
								mouseClickTimer.Start();
								break;
							}
						}
						nodeFromTop++;
					}
				}
			}
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			if ((keyData & Keys.Alt) == 0)
			{
				Keys key = keyData & Keys.KeyCode;
				bool shiftKey = (keyData & Keys.Shift) != 0;
				bool controlKey = (keyData & Keys.Control) != 0;
				TreeNode selectedNode = SelectedNode;

				switch (key)
				{
					case Keys.Left:
						if (selectedNode != null)
						{
							if (selectedNode.IsExpanded)
							{
								selectedNode.Collapse();
							}
							else if (selectedNode.Parent != null)
							{
								SelectedNode = selectedNode.Parent;
							}
						}
						return true;
					case Keys.Right:
						if (selectedNode != null && selectedNode.Nodes.Count != 0)
						{
							if (selectedNode.IsExpanded)
							{
								SelectedNode = selectedNode.NextVisibleNode;
							}
							else
							{
								selectedNode.Expand();
							}
						}
						return true;
					case Keys.Up:
						if (selectedNode != null)
						{
							selectedNode = selectedNode.PrevVisibleNode;
							if (selectedNode != null)
							{
								SelectedNode = selectedNode;
							}
						}
						return true;
					case Keys.Down:
						if (selectedNode != null)
						{
							selectedNode = selectedNode.NextVisibleNode;
							if (selectedNode != null)
							{
								SelectedNode = selectedNode;
							}
						}	
						return true;
					case Keys.Home:
						if (Nodes[0] != null)
						{
							SelectedNode = Nodes[0];
						}
						return true;
					case Keys.End:
					{
						NodeEnumerator nodes = new NodeEnumerator(this.nodes);
						while (nodes.MoveNext())
						{
						}
						SelectedNode = nodes.currentNode;
						return true;
					}
					case Keys.Prior:
					{
						int nodePosition = 0;
						// Get the position of the current selected node.
						NodeEnumerator nodes = new NodeEnumerator(this.nodes);
						while (nodes.MoveNext())
						{
							if (nodes.currentNode == selectedNode)
							{
								break;
							}
							nodePosition++;
						}

						nodePosition -= VisibleCountActual - 1;
						if (nodePosition < 0)
						{
							nodePosition = 0;
						}

						// Get the node that corresponds to the position.
						nodes.Reset();
						while (nodes.MoveNext())
						{
							if (nodePosition-- == 0)
							{
								break;
							}
						}

						// Set the selectedNode.
						SelectedNode = nodes.currentNode;

					}
						return true;
					case Keys.Next:
					{
						int rows = 0;
						int rowsPerPage = VisibleCountActual;
						NodeEnumerator nodes = new NodeEnumerator(this.nodes);
						while (nodes.MoveNext())
						{
							if (nodes.currentNode == selectedNode || rows > 0)
							{
								rows++;
								if (rows >= rowsPerPage)
								{
									break;
								}
							}
						}
						SelectedNode = nodes.currentNode;
						
						return true;
					}
				}
	
			}
			return base.ProcessDialogKey(keyData);
		}


		// Non Microsoft member.
		protected override void OnPaint(PaintEventArgs e)
		{
			Draw(e.Graphics, root);
		}

		// Non Microsoft member.
		protected override void OnLeave(EventArgs e)
		{
				base.OnLeave (e);
				if (selectedNode != null)
				{
					selectedNode.Invalidate();
				}
		}


		public new event PaintEventHandler Paint
		{
			add
			{
				base.Paint += value;
			}
			remove
			{
				base.Paint -= value;
			}
		}

		public string PathSeparator
		{
			get
			{
				return pathSeparator;
			}
			set
			{
				pathSeparator = value;
			}
		}
		// Emulate the Microsoft behaviour of resetting the control when they have to recreate the handle.
		internal void ResetView()
		{
			topNode = null;
			CollapseAll();
			Draw(root);
		}

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
		public int SelectedImageIndex
		{
			get
			{
				if (imageList ==null)
				{
					return -1;
				}
				if (selectedImageIndex >= imageList.Images.Count)
				{
					if (selectedImageIndex == -1)
					{
						return 0;
					}
					return imageList.Images.Count - 1;
				}
				return selectedImageIndex;
			}
			set
			{
				if (selectedImageIndex != value)
				{
					selectedImageIndex = value;
				}
			}
		}

		public TreeNode SelectedNode
		{
			get
			{
				if (selectedNode == null && nodes.Count > 0)
				{
					return nodes[0];
				}
				return selectedNode;
			}
			set
			{			
				if (value != selectedNode)
				{
					// Redraw the old item
					if (selectedNode != null)
					{
						TreeNode oldNode = selectedNode;
						selectedNode = value;
						oldNode.Invalidate();
					}
					else
					{
						selectedNode = value;
					}

					if (selectedNode != null)
					{
						selectedNode.Invalidate();
						selectedNode.EnsureVisible();
					}
				}
			}
		}

		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			base.SetBoundsCore (x, y, width, height, specified);
			if ((specified & BoundsSpecified.Size) != 0)
			{
				Invalidate();
			}
		}

		private void SetupHScrollBar(bool needsVScrollBar, int maxWidth, bool createNew, Graphics g)
		{
			Rectangle clientRectangle = ClientRectangle;
			int width = clientRectangle.Width;
			if (needsVScrollBar)
			{
				width -= vScrollBar.Width;
			}

			// If a node remove operation has caused the right most node to be removed, then the xoffset needs to be moved back.
			// The "hScrollBarPixelsScrolled" is because xOffset can only occur in increments of hScrollBarPixelsScrolled.
			if (maxWidth < width - hScrollBarPixelsScrolled * 2)
			{
				xOffset -= width - maxWidth;
				Invalidate();
				return;
			}

			//hScrollBar.Value = xOffset/hScrollBarPixelsScrolled;
			hScrollBar.Maximum = (maxWidth + xOffset) / hScrollBarPixelsScrolled;
			hScrollBar.LargeChange = width / hScrollBarPixelsScrolled;
			// Set the position of the H scroll bar but leave a hole if they are both visible.
			hScrollBar.SetBounds(0, clientRectangle.Height - hScrollBar.Height, width, hScrollBar.Height);
			// Force a redraw because if none of the hScrollBar values above change, we still want to make sure it is redrawn.
			hScrollBar.Invalidate();
				
			if (createNew)
			{
				hScrollBar.ValueChanged +=new EventHandler(hScrollBar_ValueChanged);
				Controls.Add(hScrollBar);
			}

			// Draw the gap between the two if needed.
			if (needsVScrollBar)
			{
				Rectangle gap = new Rectangle(hScrollBar.Right, hScrollBar.Top, vScrollBar.Width, hScrollBar.Height);
				g.FillRectangle(SystemBrushes.Control, gap);
			}
		}
		
		private void SetupVScrollBar(int nodeCount, bool needsHScrollBar, bool createNew, int selectedNodePosition)
		{
			Rectangle clientRectangle = ClientRectangle;
			vScrollBar.Maximum = nodeCount;
			// Set the position of the V scroll bar but leave a hole if they are both visible.
			int height = clientRectangle.Height;
			if (needsHScrollBar)
			{
				height -= hScrollBar.Height;
			}
			vScrollBar.LargeChange = height / ItemHeight;
			vScrollBar.Value = selectedNodePosition;
			vScrollBar.SetBounds(clientRectangle.Width - vScrollBar.Width, 0, vScrollBar.Width, height);
				
			if (createNew)
			{
				Controls.Add(vScrollBar);
				vScrollBar.ValueChanged+=new EventHandler(vScrollBar_ValueChanged);
			}
		}

		public bool ShowLines
		{
			get
			{
				return showLines;
			}
			set
			{
				if (value != showLines)
				{
					showLines = value;
				}
			}
		}

		public bool ShowPlusMinus
		{
			get
			{
				return showPlusMinus;
			}
			set
			{
				if (value != showPlusMinus)
				{
					showPlusMinus = value;
				}
			}
		}

		public bool ShowRootLines
		{
			get
			{
				return showRootLines;
			}
			set
			{
				if (value != showRootLines)
				{
					showRootLines = value;
				}
			}
		}

		public bool Sorted
		{
			get
			{
				return sorted;
			}
			set
			{
				if (value != sorted)
				{
					sorted = value;
					//TODO: could be done better!
					TreeNode[] nodes = new TreeNode[Nodes.Count];
					Nodes.CopyTo(nodes, 0);
					Nodes.Clear();
					Nodes.AddRange(nodes);
				}
			}
		}

		public override String Text
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


		private void textBox_KeyUp(Object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				textBox.Visible = false;
			}
			else
			{
				int x = textBox.Left;
				textBox.Width = GetTextBoxWidth(ref x);
				textBox.Left = x;
			}
		}

		private void textBox_Leave(object sender, EventArgs e)
		{	
			EndEdit(false);
		}

		public new event EventHandler TextChanged
		{
			add
			{
				base.TextChanged += value;
			}
			remove
			{
				base.TextChanged -= value;
			}
		}

		public TreeView() : base()
		{
			hideSelection = true;
			indent = 19;
			itemHeight = -1;
			scrollable = true;
			BorderStyleInternal = BorderStyle.Fixed3D;
			showLines = true;
			showPlusMinus = true;
			showRootLines = true;
			root = new TreeNode(this);
			root.Expand();
			nodes = new TreeNodeCollection(root);
			BackColor = SystemColors.Window;
			ForeColor = SystemColors.WindowText;
			SetStyle(ControlStyles.StandardClick, false);
			// Switch on double buffering.
			SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
		}

		protected internal override void ToolkitMouseDown(MouseButtons buttons, Keys modifiers, int clicks, int x, int y, int delta)
		{
			// We need to prevent the focus messages happening. These happen automatically in Controls ToolkitMouseDown.
			// We manually set the focus after the node is selected to make sure the events happen in the order ms does.
			if (Enabled)
			{
				OnMouseDown(new MouseEventArgs(buttons, clicks, x, y, delta));
			}
			// Set event to be raised on mouse up
			clickEvent = (clicks == 1) ? ClickEvent.Click : ClickEvent.DoubleClick;
		}

		public TreeNode TopNode 
		{
			get
			{
				return topNode;
			}
		}

		public override string ToString()
		{
			string s = base.ToString();
			if (Nodes != null)
			{
				s = s + ", Count: " + Nodes.Count;
				if (Nodes.Count > 0)
				{
					s = s + ", [0]: " + Nodes[0].ToString();
				}
			}
			return s; 

		}

		public int VisibleCount
		{
			get
			{
				return ClientRectangle.Height / ItemHeight;
			}
		}

		internal int VisibleCountActual
		{
			get
			{
				int height = ClientRectangle.Height;
				if (hScrollBar != null && hScrollBar.Visible)
				{
					height -= hScrollBar.Height;
				}
				return height / ItemHeight;
			}
		}

		private void vScrollBar_ValueChanged(object sender, EventArgs e)
		{
			int nodeFromTop = 0;
			TreeView.NodeEnumerator nodes = new TreeView.NodeEnumerator(this.nodes);
			while (nodes.MoveNext())
			{
				if (nodeFromTop == vScrollBar.Value)
				{
					topNode = nodes.currentNode;
					Invalidate();
					return;
				}
				nodeFromTop++;
			}
		}
		
		// Private enumerator class for all returning all expanded nodes.
		internal class NodeEnumerator : IEnumerator
		{
			// Internal state.
			private TreeNodeCollection nodes;
			internal TreeNode currentNode;
			private bool first;
			// level of node
			internal int level = 0;

			// Constructor.
			public NodeEnumerator(TreeNodeCollection nodes)
			{
				this.nodes = nodes;
				Reset();
			}

			// Move to the next element in the enumeration order.
			public bool MoveNext()
			{
				if (first)
				{
					if (nodes.Count == 0)
					{
						return false;
					}
					currentNode = nodes[0];
					first = false;
					return true;
				}
				if (currentNode == null)
				{
					return false;
				}
				if (currentNode.childCount > 0 && currentNode.expanded)
				{
					// If expanded climb into hierarchy.
					currentNode = currentNode.Nodes[0];
					level++;
				}
				else
				{
					TreeNode nextNode = currentNode.NextNode;
					TreeNode nextCurrentNode = currentNode;
					while (nextNode == null)
					{
						// We need to move back up.
						// Are we back at the top?
						if (nextCurrentNode.Parent == null)
						{
							// Leave the nextNode as the previous last node.
							nextNode = currentNode;
							return false;
						}
						else
						{
							nextCurrentNode = nextCurrentNode.Parent;
							if (nextCurrentNode.parent != null)
							{
								nextNode = nextCurrentNode.NextNode;
							}
							level--;
						}
					}
					currentNode = nextNode;
				}
				return true;

			}

			// Reset the enumeration.
			public void Reset()
			{
				first = true;
				currentNode = null;
				level = 0;
			}

			// Get the current value in the enumeration.
			public Object Current
			{
				get
				{
					if(currentNode == null)
					{
						throw new InvalidOperationException();
					}
					else
					{
						return currentNode;
					}
				}
			}
		}
		
		internal enum ClickEvent
		{
			Click,
			DoubleClick,
			None
		}
	}

}
