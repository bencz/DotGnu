/*
 * TreeBase.cs - Base class for generic tree implementations.
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

namespace Generics
{

using System;

// The algorithm used here is based on the red-black tree implementation
// in the C++ Standard Template Library.
//
// Normally, programmers should use "TreeSet" or "TreeDictionary"
// instead of inheriting from this class.

public abstract class TreeBase<KeyT, ValueT>
{
	// Structure of a tree node.
	private sealed class TreeNode<KeyT, ValueT>
	{
		public TreeNode<KeyT, ValueT> parent;	// Parent of this node.
		public TreeNode<KeyT, ValueT> left;		// Left child.
		public TreeNode<KeyT, ValueT> right;	// Right child.
		public KeyT					  key;		// Key stored in the node.
		public ValueT				  value;	// Value stored in the node.
		public bool		   			  red;		// True if the node is "red".

	}; // class TreeNode<KeyT, ValueT>

	// Internal state.
	protected IComparer<KeyT> cmp;
	private TreeNode<KeyT, ValueT> root;
	private TreeNode<KeyT, ValueT> leftMost;
	protected int count;

	// Constructors.
	protected TreeBase() : this(null) {}
	protected TreeBase(IComparer<KeyT> cmp)
			{
				if(cmp == null)
				{
					this.cmp = new Comparer<KeyT>();
				}
				else
				{
					this.cmp = cmp;
				}
				root = null;
				leftMost = null;
				count = 0;
			}

	// Rotate the tree left around a node.
	private void RotateLeft(TreeNode<KeyT, ValueT> x)
			{
				TreeNode<KeyT, ValueT> y = x.right;
				x.right = y.left;
				if(y.left != null)
				{
					y.left.parent = x;
				}
				y.parent = x.parent;
				if(x == root)
				{
					root = y;
				}
				else if(x == x.parent.left)
				{
					x.parent.left = y;
				}
				else
				{
					x.parent.right = y;
				}
				y.left = x;
				x.parent = y;
			}

	// Rotate the tree right around a node.
	private void RotateRight(TreeNode<KeyT, ValueT> x)
			{
				TreeNode<KeyT, ValueT> y = x.left;
				x.left = y.right;
				if(y.right != null)
				{
					y.right.parent = x;
				}
				y.parent = x.parent;
				if(x == root)
				{
					root = y;
				}
				else if(x == x.parent.right)
				{
					x.parent.right = y;
				}
				else
				{
					x.parent.left = y;
				}
				y.right = x;
				x.parent = y;
			}

	// Rebalance the tree around a particular inserted node.
	private void Rebalance(TreeNode<KeyT, ValueT> x)
			{
				TreeNode<KeyT, ValueT> y;

				// Set the inserted node's color initially to "red".
				x.red = true;

				// Split and rotate sub-trees as necessary to rebalance.
				while(x != root && x.parent.red)
				{
					if(x.parent == x.parent.parent.left)
					{
						y = x.parent.parent.right;
						if(y != null && y.red)
						{
							x.parent.red = false;
							y.red = false;
							x.parent.parent.red = true;
							x = x.parent.parent;
						}
						else
						{
							if(x == x.parent.right)
							{
								x = x.parent;
								RotateLeft(x);
							}
							x.parent.red = false;
							x.parent.parent.red = true;
							RotateRight(x.parent.parent);
						}
					}
					else
					{
						y = x.parent.parent.left;
						if(y != null && y.red)
						{
							x.parent.red = false;
							y.red = false;
							x.parent.parent.red = true;
							x = x.parent.parent;
						}
						else
						{
							if(x == x.parent.left)
							{
								x = x.parent;
								RotateRight(x);
							}
							x.parent.red = false;
							x.parent.parent.red = true;
							RotateLeft(x.parent.parent);
						}
					}
				}

				// Set the root color to black.
				root.red = false;
			}

	// Remove a specific node and rebalance the tree afterwards.
	private void RemoveNode(TreeNode<KeyT, ValueT> z)
			{
				TreeNode<KeyT, ValueT> y = z;
				TreeNode<KeyT, ValueT> x = null;
				TreeNode<KeyT, ValueT> x_parent = null;
				TreeNode<KeyT, ValueT> w;
				bool tempRed;

				// There will be one less item once we are finished.
				--count;

				// Determine the starting position for the rebalance.
				if(y.left == null)
				{
					x = y.right;
				}
				else if(y.right == null)
				{
					x = y.left;
				}
				else
				{
					y = y.right;
					while(y.left != null)
					{
						y = y.left;
					}
					x = y.right;
				}

				// Re-link the nodes to remove z from the tree.
				if(y != z)
				{
					z.left.parent = y;
					y.left = z.left;
					if(y != z.right)
					{
						x_parent = y.parent;
						if(x != null)
						{
							x.parent = y.parent;
						}
						y.parent.left = x;
						y.right = z.right;
						z.right.parent = y;
					}
					else
					{
						x_parent = y;
					}
					if(root == z)
					{
						root = y;
					}
					else if(z.parent.left == z)
					{
						z.parent.left = y;
					}
					else
					{
						z.parent.right = y;
					}
					y.parent = z.parent;
					tempRed = y.red;
					y.red = z.red;
					z.red = tempRed;
					y = z;
				}
				else
				{
					x_parent = y.parent;
					if(x != null)
					{
						x.parent = y.parent;
					}
					if(root == z)
					{
						root = x;
					}
					else if(z.parent.left == z)
					{
						z.parent.left = x;
					}
					else
					{
						z.parent.right = x;
					}
					if(leftMost == z)
					{
						if(z.right == null)
						{
							leftMost = z.parent;
						}
						else
						{
							leftMost = x;
							while(leftMost != null && leftMost.left != null)
							{
								leftMost = leftMost.left;
							}
						}
					}
				}

				// If the y node is "red", then the tree is still balanced.
				if(y.red)
				{
					return;
				}

				// Rotate nodes within the tree to bring it back into balance.
				while(x != root && (x == null || !(x.red)))
				{
					if(x == x_parent.left)
					{
						w = x_parent.right;
						if(w.red)
						{
							w.red = false;
							x_parent.red = true;
							RotateLeft(x_parent);
							w = x_parent.right;
						}
						if((w.left == null || !(w.left.red)) &&
						   (w.right == null || !(w.right.red)))
						{
							w.red = true;
							x = x_parent;
							x_parent = x_parent.parent;
						}
						else
						{
							if(w.right == null || !(w.right.red))
							{
								if(w.left != null)
								{
									w.left.red = false;
								}
								w.red = true;
								RotateRight(w);
								w = x_parent.right;
							}
							w.red = x_parent.red;
							x_parent.red = false;
							if(w.right != null)
							{
								w.right.red = false;
							}
							RotateLeft(x_parent);
							break;
						}
					}
					else
					{
						w = x_parent.left;
						if(w.red)
						{
							w.red = false;
							x_parent.red = true;
							RotateRight(x_parent);
							w = x_parent.left;
						}
						if((w.right == null || !(w.right.red)) &&
						   (w.left == null || !(w.left.red)))
						{
							w.red = true;
							x = x_parent;
							x_parent = x_parent.parent;
						}
						else
						{
							if(w.left == null || !(w.left.red))
							{
								if(w.right != null)
								{
									w.right.red = false;
								}
								w.red = true;
								RotateLeft(w);
								w = x_parent.left;
							}
							w.red = x_parent.red;
							x_parent.red = false;
							if(w.left != null)
							{
								w.left.red = false;
							}
							RotateRight(x_parent);
							break;
						}
					}
					if(x != null)
					{
						x.red = false;
					}
				}
			}

	// Add an item to this tree.
	protected void AddItem(KeyT key, ValueT value, bool throwIfExists)
			{
				TreeNode<KeyT, ValueT> y;
				TreeNode<KeyT, ValueT> x;
				TreeNode<KeyT, ValueT> z;
				int cmpValue;

				// Find the insert position.
				y = null;
				x = root;
				while(x != null)
				{
					y = x;
					if((cmpValue = cmp.Compare(key, x.key)) < 0)
					{
						x = x.left;
					}
					else if(cmpValue > 0)
					{
						x = x.right;
					}
					else if(throwIfExists)
					{
						throw new ArgumentException(S._("Arg_ExistingEntry"));
					}
					else
					{
						x.value = value;
						return;
					}
				}

				// Create a new node to insert.
				z = new TreeNode<KeyT, ValueT>();
				z.key = key;
				z.value = value;

				// Determine how to insert the node.
				if(y == null)
				{
					// The tree is empty, so add the initial node.
					root = z;
					leftMost = z;
				}
				else if(x != null || cmp.Compare(key, y.key) < 0)
				{
					// Insert on the left.
					y.left = z;
					if(y == leftMost)
					{
						leftMost = z;
					}
				}
				else
				{
					// Insert on the right.
					y.right = z;
				}
				z.parent = y;

				// Rebalance the tree around the inserted node.
				Rebalance(z);

				// We have one more element in the tree.
				++count;
			}

	// Clear the contents of this tree.
	protected void ClearAllItems()
			{
				root = null;
				leftMost = null;
				count = 0;
			}

	// Determine if this tree contains a specific key.
	protected bool ContainsItem(KeyT key)
			{
				TreeNode<KeyT, ValueT> current = root;
				int cmpValue;
				while(current != null)
				{
					if((cmpValue = cmp.Compare(key, current.key)) < 0)
					{
						current = current.left;
					}
					else if(cmpValue > 0)
					{
						current = current.right;
					}
					else
					{
						return true;
					}
				}
				return false;
			}

	// Look up the value associated with a specific key.
	protected T LookupItem(KeyT key)
			{
				TreeNode<KeyT, ValueT> current = root;
				int cmpValue;
				while(current != null)
				{
					if((cmpValue = cmp.Compare(key, current.key)) < 0)
					{
						current = current.left;
					}
					else if(cmpValue > 0)
					{
						current = current.right;
					}
					else
					{
						return current.value;
					}
				}
				throw new ArgumentException(S._("Arg_NotInDictionary"));
			}

	// Remove the node for a specific key.
	protected void RemoveItem(KeyT key)
			{
				TreeNode<KeyT, ValueT> current = root;
				int cmpValue;
				while(current != null)
				{
					if((cmpValue = cmp.Compare(key, current.key)) < 0)
					{
						current = current.left;
					}
					else if(cmpValue > 0)
					{
						current = current.right;
					}
					else
					{
						RemoveNode(current);
						return;
					}
				}
			}

	// Get an iterator for this tree.
	protected TreeBaseIterator<KeyT, ValueT> GetInOrderIterator()
			{
				return new TreeBaseIterator<KeyT, ValueT>(this);
			}

	// Iterator class that implements in-order traversal of a tree.
	protected sealed class TreeBaseIterator<KeyT, ValueT>
	{
		// Internal state.
		private Tree<KeyT, ValueT> tree;
		private TreeNode<KeyT, ValueT> current;
		private bool reset;
		private bool removed;

		// Constructor.
		public TreeBaseIterator(Tree<KeyT, ValueT> tree)
				{
					this.tree = tree;
					this.current = null;
					this.reset = true;
					this.removed = false;
				}

		// Move to the next item in the iteration order.
		public bool MoveNext()
				{
					if(removed)
					{
						// The last node was removed, so we are already
						// positioned on the next node to be visited.
						removed = false;
					}
					else if(reset)
					{
						// Start with the left-most node in the tree.
						current = tree.leftMost;
						reset = false;
					}
					else if(current == null)
					{
						// We already reached the end of the tree.
						return false;
					}
					else if(current.right != null)
					{
						// Move to the left-most node in the right sub-tree.
						current = current.right;
						while(current.left != null)
						{
							current = current.left;
						}
					}
					else
					{
						// Move up ancestors until we are no longer
						// the right-most child of our parent.
						TreeNode<T> parent = current.parent;
						while(parent != null && parent.right == current)
						{
							current = parent;
							parent = current.parent;
						}
						current = parent;
					}
					return (current != null);
				}

		// Reset the iterator to the start.
		public void Reset()
				{
					current = null;
					reset = true;
					removed = false;
				}

		// Remove the current item in the iteration order.
		public void Remove()
				{
					// Bail out if we are not currently positioned on a node.
					if(current == null || removed)
					{
						throw new InvalidOperationException
							(S._("Invalid_BadIteratorPosition"));
					}

					// Save the current node.
					TreeNode<T> node = current;

					// Move on to the next node in the traversal order.
					MoveNext();

					// Remove the node from the tree.
					tree.RemoveNode(node);

					// Record that we have removed "node".
					removed = true;
				}

		// Get the key from the current item.
		public KeyT Key
				{
					get
					{
						if(current != null && !removed)
						{
							return current.key;
						}
						throw new InvalidOperationException
							(S._("Invalid_BadIteratorPosition"));
					}
				}

		// Get the value from the current item.
		public ValueT Value
				{
					get
					{
						if(current != null && !removed)
						{
							return current.value;
						}
						throw new InvalidOperationException
							(S._("Invalid_BadIteratorPosition"));
					}
					set
					{
						if(current != null && !removed)
						{
							current.value = value;
						}
						else
						{
							throw new InvalidOperationException
								(S._("Invalid_BadIteratorPosition"));
						}
					}
				}

	}; // class TreeBaseIterator<KeyT, ValueT>

}; // class TreeBase<KeyT, ValueT>

}; // namespace Generics
