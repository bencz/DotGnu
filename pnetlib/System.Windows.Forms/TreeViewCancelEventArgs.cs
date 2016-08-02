/*
 * TreeViewCancelEventArgs.cs - Implementation of the
 *			"System.Windows.Forms.TreeViewCancelEventArgs" class.
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

using System.ComponentModel;

public class TreeViewCancelEventArgs : CancelEventArgs
{
	// Internal state.
	private TreeNode node;
	private TreeViewAction action;

	// Constructor.
	public TreeViewCancelEventArgs
				(TreeNode node, bool cancel, TreeViewAction action)
			: base(cancel)
			{
				this.node = node;
				this.action = action;
			}

	// Get the action that caused the event.
	public TreeViewAction Action
			{
				get
				{
					return action;
				}
			}

	// Get the node that was affected by the event.
	public TreeNode Node
			{
				get
				{
					return node;
				}
			}

}; // class TreeViewCancelEventArgs

}; // namespace System.Windows.Forms
