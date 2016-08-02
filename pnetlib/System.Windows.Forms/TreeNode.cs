/*
 * TreeNode.cs - Implementation of the
 *			"System.Windows.Forms.TreeNode" class.
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
	using System.Text;
	using System.Globalization;
	using System.Runtime.Serialization;

	public class TreeNode : /*MarshalByRefObject,*/ ICloneable
#if CONFIG_SERIALIZATION
	, ISerializable
#endif
	{
		internal Color backColor;
		internal int childCount = 0;
		internal TreeNode[] children;
		internal bool expanded;
		internal Color foreColor;
		private int imageIndex;
		internal int index;
		internal bool isChecked;
		internal Font nodeFont;
		private TreeNodeCollection nodes;
		internal TreeNode parent;
		private int selectedImageIndex;
		private object tag;
		internal string text;
		internal TreeView treeView;

		// Must GO:
		internal int markerLineY;

		internal int AddSorted(TreeNode node)
		{
			int pos = 0;
			string text = node.Text;
			if (childCount > 0)
			{
				Globalization.CompareInfo compare = Application.CurrentCulture.CompareInfo;
				// Simple optimization if added in sort order
				if (compare.Compare(children[(childCount - 1)].Text, text) <= 0)
					pos = childCount;
				else
				{
					// Binary Search
					int i = 0;
					int j = childCount;
					while (i < j)
					{
						int mid = (i + j) / 2;
						if (compare.Compare(children[mid].Text, text) <= 0)
							i = mid + 1;
						else
							i = mid;
					}
					pos = i;
				}
			}

			node.SortChildren();
			InsertNodeAt(pos, node);
			if (treeView != null && node == treeView.selectedNode)
			{
				treeView.SelectedNode = node;
			}
			return pos;
		}

		public Color BackColor
		{
			get
			{
				// TODO:Property Bag
				return backColor;
			}
			set
			{
				// TODO:Property Bag
				if (value == backColor)
				{
					return;
				}
				backColor = value;
				Invalidate();
			}
		}

		public void BeginEdit()
		{
			if( null == treeView ) return;
			
			if (treeView.toolkitWindow != null)
			{
				if (!treeView.LabelEdit)
				{
					throw new Exception(S._("SWF_TreeNodeLabelEditFalse"));
				}
				if (!treeView.Focused)
				{
					treeView.Focus();
				}
				treeView.BeginEdit(this);
			}
		}

		public Rectangle Bounds
		{
			get
			{
				if( null == treeView ) return new Rectangle( 0,0,0,0 );
				return treeView.GetNodeBounds(this);
			}
		}

		public bool Checked
		{
			get
			{
				return isChecked;
			}
		
			set
			{
				if (treeView != null)
				{
					TreeViewCancelEventArgs e = new TreeViewCancelEventArgs(this, false, TreeViewAction.Unknown);
					treeView.OnBeforeCheck(e);
					if (e.Cancel)
					{
						return;
					}
					isChecked = value;
					Invalidate();
					treeView.OnAfterCheck(new TreeViewEventArgs(this, TreeViewAction.Unknown));
				}
				else
				{
					isChecked = value;
				}
			}
		}

		internal void Clear()
		{
			children = null;
			childCount = 0;
		}

		public virtual object Clone()
		{
			TreeNode node = new TreeNode(text, imageIndex, selectedImageIndex);
			if (childCount > 0)
			{
				node.children = new TreeNode[childCount];
				for (int i = 0; i < childCount; i++)
					node.Nodes.Add(children[i].Clone() as TreeNode);
			}
			node.Checked = Checked;
			node.Tag = Tag;
			return node;
		}

		public void Collapse()
		{
			CollapseInternal();
			if( null != treeView ) {
				treeView.InvalidateDown(this);
			}
		}

		// Collapse the children recursively but don't redraw.
		private void CollapseInternal()
		{
			bool selected = false;
			// Recursively collapse, if a child was selected, mark to select the parent.
			if (childCount > 0)
			{
				for (int i = 0; i < childCount; i++)
				{
					if( null != treeView ) {
						if (treeView.SelectedNode == children[i])
							selected = true;
					}
					children[i].CollapseInternal();
				}
			}
			if(expanded)
			{
				// Do the events.
				TreeViewCancelEventArgs eventArgs = new TreeViewCancelEventArgs(this, false, TreeViewAction.Collapse);
				if( null != treeView ) treeView.OnBeforeCollapse(eventArgs);
				if (!eventArgs.Cancel)
				{
					// The node is now collapsed.
					expanded = false;
					if( null != treeView ) treeView.OnAfterCollapse(new TreeViewEventArgs(this));
				}
			}
			if (selected)
			{
				if( null != treeView ) treeView.SelectedNode = this;
			}
		}

		public void EndEdit(bool cancel)
		{
			if (treeView != null)
			{
				treeView.EndEdit(cancel);
			}
		}

		public void EnsureVisible()
		{
			if( null == treeView ) return;
			
			TreeView.NodeEnumerator nodes;
			int nodeFromTop;
			int nodeNo;
			while (true)
			{
				// Find "this" node number and position from the top control.
				nodeFromTop = -1;
				nodeNo = 0;
				bool nodeFound = false;
				nodes = new TreeView.NodeEnumerator(treeView.nodes);
				while (nodes.MoveNext())
				{
					if (nodes.currentNode == treeView.topNode)
					{
						// We are at the top of the control.
						nodeFromTop = 0;
					}
					if (nodes.currentNode == this)
					{
						if (nodeFromTop < 0)
						{
							treeView.topNode = this;
							treeView.Invalidate();
							return;
						}
						nodeFound = true;
						break;
					}
					if (nodeFromTop >= 0)
					{
						nodeFromTop++;
					}
					nodeNo++;
				}
			

				if (nodeFound)
				{
					break;
				}
				else
				{
					// Make sure all parents are expanded and see if its now visible.
					TreeNode node = this;
					TreeNode highestNode = node;
					for (; node != null; node = node.Parent)
					{
						node.expanded = true;
						highestNode = node;
					}
					treeView.InvalidateDown(highestNode);
				}
			}

			int visibleNodes = treeView.VisibleCountActual;
			// See if its already visible.
			if (nodeFromTop < visibleNodes)
			{
				return;
			}

			// Set the top node no we want to make this node 1 up from the bottom.
			nodeFromTop = nodeNo - visibleNodes + 1;
			if (nodeFromTop < 0)
			{
				nodeFromTop = 0;
			}

			// Find the node corresponding to this node no.
			nodes.Reset();
			nodeNo = 0;
			while (nodes.MoveNext())
			{
				if (nodeFromTop == nodeNo)
				{
					treeView.topNode = nodes.currentNode;
					treeView.Invalidate();
					break;
				}
				nodeNo++;
			}
		}

		public void Expand()
		{
			if (expanded)
			{
				return;
			}
			if (treeView == null)
			{
				expanded = true;
				return;
			}

			TreeViewCancelEventArgs args = new TreeViewCancelEventArgs(this, false, TreeViewAction.Expand);
			treeView.OnBeforeExpand(args);
			if(args.Cancel)
			{
				return;
			}

			TreeNode node = this;
			TreeNode highestNode = node;
			for (; node != null; node = node.Parent)
			{
				node.expanded = true;
				highestNode = node;
			}
			treeView.InvalidateDown(highestNode);
			treeView.OnAfterExpand(new TreeViewEventArgs(this, TreeViewAction.Expand));
		}

		public void ExpandAll()
		{
			Expand();
			for (int i = 0; i < childCount; i++)
			{
				children[i].ExpandAll();
			}
		}

		public TreeNode FirstNode
		{
			get
			{
				if (childCount == 0)
				{
					return null;
				}
				else
				{
					return children[0];
				}
			}
		}

		public Color ForeColor
		{
			get
			{
				// TODO:Property Bag
				return foreColor;
			}
			set
			{
				// TODO:Property Bag
				if (value == foreColor)
				{
					return;
				}
				foreColor = value;
				Invalidate();
			}
		}
					
		// Not used in this implementation
		public static TreeNode FromHandle(TreeView tree, IntPtr handle)
		{
			return null;
		}

		public string FullPath
		{
			get
			{
				StringBuilder s = new StringBuilder();
				GetFullPath(s, TreeView.PathSeparator);
				return s.ToString();
			}
		}

		private void GetFullPath(StringBuilder path, string pathSeparator)
		{
			if (parent == null)
				return;
			parent.GetFullPath(path, pathSeparator);
			if (parent.parent != null)
			{
				path.Append(pathSeparator);
			}
			path.Append(text);
		}

		public int GetNodeCount(bool includeSubTrees)
		{
			int count = childCount;
			if (includeSubTrees)
			{
				for (int i = 0; i < childCount; i++)
				{
					count += children[i].GetNodeCount(true);
				}
			}
			return count;
		}

#if CONFIG_SERIALIZATION

		void System.Runtime.Serialization.ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
			si.AddValue("Text", text);
			si.AddValue("IsChecked", isChecked);
			si.AddValue("ImageIndex", imageIndex);
			si.AddValue("SelectedImageIndex", selectedImageIndex);
			si.AddValue("ChildCount", childCount);
			if (childCount > 0)
			{
				for (int i = 0; i < childCount; i++)
				{
					si.AddValue("children"+ i, children[i], typeof(TreeNode));
				}
			}
			if (tag != null && tag.GetType().IsSerializable)
			{
				si.AddValue("UserData", tag, tag.GetType());
			}
		}

#endif // CONFIG_SERIALIZATION

		// This is not used in this implementation.
		public IntPtr Handle
		{
			get
			{
				return IntPtr.Zero;
			}
		}

		public int ImageIndex
		{
			get
			{
				return imageIndex;
			}
			set
			{
				if (imageIndex == value)
					return;
				imageIndex = value;
				Invalidate();
			}
		}

		public int Index
		{
			get
			{
				return index;
			}
		}

		internal void InsertNodeAt(int index, TreeNode node)
		{
			SizeChildrenArray();
			node.parent = this;
			node.index = index;
			node.treeView = treeView;
			for (int i = childCount; i > index; i--)
			{
				TreeNode node1 = children[i - 1];
				node1.index = i;
				children[i] = node1;
			}
			childCount++;
			children[index] = node;
			if (treeView != null)
			{
				if (childCount == 1 && IsVisible)
				{
					Invalidate();
				}
				else if ( index - 1 >= 0 )
				{
					if (expanded && children[index - 1].IsVisible)
					{
						treeView.InvalidateDown(node);
					}
				}
			}
		}

		internal void Invalidate()
		{
			if (treeView == null || !treeView.IsHandleCreated)
			{
				return;
			}
			Rectangle bounds = Bounds;
			if (bounds != Rectangle.Empty)
			{
				// Include the focus rectangle.
				bounds = new Rectangle(0, bounds.Y - 1, bounds.Right + 2, bounds.Height + 2);
				treeView.Invalidate(bounds);
			}
		}

		public bool IsEditing
		{
			get
			{
				if (treeView == null)
				{
					return false;
				}
				else
				{
					return (treeView.editNode == this);
				}
			}
		}

		public bool IsExpanded
		{
			get
			{
				return expanded;
			}
		}

		public bool IsSelected
		{
			get
			{
				if (treeView == null)
					return false;
				return treeView.selectedNode == this;
			}
		}

		public bool IsVisible
		{
			get
			{
				if (treeView == null || !treeView.Visible)
					return false;
				Rectangle bounds = Bounds;
				if (bounds == Rectangle.Empty)
					return false;
				return (treeView.ClientRectangle.IntersectsWith(bounds));
			}
		}

		public TreeNode LastNode
		{
			get
			{
				if (childCount == 0)
					return null;
				else
					return children[childCount - 1];
			}
		}

		public TreeNode NextNode
		{
			get
			{
				if (index < parent.Nodes.Count - 1)
					return parent.Nodes[index + 1];
				else
					return null;
			}
		}

		public TreeNode NextVisibleNode
		{
			get
			{
				if( null == treeView ) return null;
				
				bool pastThis = false;
				TreeView.NodeEnumerator nodes = new TreeView.NodeEnumerator(treeView.nodes);
				while (nodes.MoveNext())
				{
					if (pastThis)
					{
						if (nodes.currentNode.parent.expanded)
						{
							return nodes.currentNode;
						}
					}
					else if (nodes.currentNode == this)
					{
						pastThis = true;
					}
				}
				return null;
			}
		}

		public Font NodeFont
		{
			get
			{
				// TODO:Property Bag
				return nodeFont;
			}
			set
			{
				// TODO:Property Bag
				if (value == nodeFont)
				{
					return;
				}
				nodeFont = value;
				Invalidate();
			}
		}

		public TreeNodeCollection Nodes
		{
			get
			{
				if (nodes == null)
				{
					nodes = new TreeNodeCollection(this);
				}
				return nodes;
			}
		}

		public TreeNode Parent
		{
			get
			{
				if (treeView != null && parent == treeView.root)
				{
					return null;
				}
				return parent;
			}
		}

		public TreeNode PrevNode
		{
			get
			{
				if (index > 0 && index <= parent.Nodes.Count)
				{
					return parent.Nodes[index - 1];
				}
				else
				{
					return null;
				}
			}
		}

		public TreeNode PrevVisibleNode
		{
			get
			{
				if( null == treeView ) return null;
				
				TreeNode visibleNode = null;
				TreeView.NodeEnumerator nodes = new TreeView.NodeEnumerator(treeView.nodes);
				while (nodes.MoveNext())
				{
					if (nodes.currentNode == this)
					{
						break;
					}
					else if (nodes.currentNode.parent.expanded)
					{
						visibleNode = nodes.currentNode;
					}
				}
				return visibleNode;
			}
		}

		public void Remove()
		{
			// If we need to, redraw the parent.
			if (treeView != null)
			{
				treeView.InvalidateDown(this);
			}
			
			// When removing a node, we need to see if topNode is it or its children.
			// If so we find a new topNode.
			TreeNode node = treeView.topNode;
			while (node != null)
			{
				if (node == this)
				{
					treeView.topNode = PrevVisibleNode;
					break;
				}
				node = node.parent;
			}
			
			RemoveRecurse();

		}

		private void RemoveRecurse()
		{
			// Remove children.
			for (int i = 0; i < childCount; i++)
			{
				children[i].RemoveRecurse();
			}
			// Remove out of parent's children.
			for (int i = index; i < parent.childCount - 1; i++)
			{
				TreeNode node = parent.children[i + 1];
				node.index = i;
				parent.children[i] = node;
			}
			parent.childCount--;
			parent = null;
			treeView = null;
		}

		public int SelectedImageIndex
		{
			get
			{
				return selectedImageIndex;
			}
			set
			{
				selectedImageIndex = value;
				Invalidate();
			}
		}

		internal void SizeChildrenArray()
		{
			if (children == null)
			{
				children = new TreeNode[10];
			}
			else if (childCount == children.Length)
			{
				TreeNode[] copy = new TreeNode[childCount * 2];
				Array.Copy(children, 0, copy, 0, childCount);
				children = copy;
			}
		}

		private void SortChildren()
		{
			if (childCount > 0)
			{
				TreeNode[] sort = new TreeNode[childCount];
				CompareInfo compare = Application.CurrentCulture.CompareInfo;
				for (int i = 0; i < childCount; i++)
				{

					int pos = -1;
					for (int j = 0; j < childCount; j++)
					{
						if (children[j] != null)
						{
							if (pos == -1 || compare.Compare(children[j].Text, children[pos].Text) < 0)
							{
								pos = j;
							}
						}
					}
					sort[i] = children[pos];
					children[pos] = null;
					sort[i].index = i;
					sort[i].SortChildren();
				}
				children = sort;
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
				if (text == null)
					return String.Empty;
				else
					return text;
			}
			set
			{
				text = value;
				Invalidate();
			}
		}

		public void Toggle()
		{
			if (expanded)
			{
				Collapse();
			}
			else
			{
				Expand();
			}
		}

		public override string ToString()
		{
			String s = base.ToString();
			if (nodes != null)
			{
				s += ", Nodes.Count: " + childCount;
				if (childCount > 0)
				{
					s += ", Nodes[0]: " + nodes[0];
				}
			}
			return s;
		}

		public TreeNode()
		{
			imageIndex = -1;
			selectedImageIndex = -1;
			backColor = Color.Empty;
		}

		internal TreeNode(TreeView treeView) : this()
		{
			this.treeView = treeView;
		}

		public TreeNode(string text) : this()
		{
			this.text = text;
		}

		public TreeNode(string text, TreeNode[] children) : this()
		{
			this.text = text;
			Nodes.AddRange(children);
		}

		public TreeNode(string text, int imageIndex, int selectedImageIndex) : this()
		{
			this.text = text;
			this.imageIndex = imageIndex;
			this.selectedImageIndex = selectedImageIndex;
		}

		public TreeNode(string text, int imageIndex, int selectedImageIndex, TreeNode[] children) : this ()
		{
			this.text = text;
			this.imageIndex = imageIndex;
			this.selectedImageIndex = selectedImageIndex;
			Nodes.AddRange(children);
		}

		public TreeView TreeView
		{
			get
			{
				return treeView;
			}
		}
		
}; // class TreeNode

}; // namespace System.Windows.Forms
