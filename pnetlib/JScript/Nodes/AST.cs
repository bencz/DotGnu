/*
 * AST.cs - Root of the abstract syntax tree structure.
 *
 * Copyright (C) 2003 Southern Storm Software, Pty Ltd.
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
 
namespace Microsoft.JScript
{

using System;

// The "AST" class is the root of the abstract syntax tree, as seen by
// API callers.  The "real" abstract syntax tree is managed by "treecc",
// rooted at the "JNode" class.  See "JNode.tc" for details.
//
// The "AST" class and its descendents should probably be "internal", but
// we have to declare them "public" for backwards-compatibility.  Because
// the constructors are "internal", there is no way for callers to inherit.

public abstract class AST
{
	// Private state - wrap up a JNode.
	internal JNode jnode;

	// Constructors.
	internal AST() {}
	internal AST(JNode jnode)
			{
				this.jnode = jnode;
			}

}; // class AST

}; // namespace Microsoft.JScript
