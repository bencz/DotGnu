/*
 * SymScope.cs - Implementation of the
 *			"System.Diagnostics.SymbolStore.SymScope" class.
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

namespace System.Diagnostics.SymbolStore
{

#if CONFIG_EXTENDED_DIAGNOSTICS

public class SymScope : ISymbolScope
{
	// Internal state.
	private unsafe ISymUnmanagedScope *pScope;
	private ISymbolMethod method;
	private ISymbolScope parent;
	private ISymbolScope[] children;
	private ISymbolVariable[] locals;
	private int endOffset;
	private int startOffset;

	// Constructors.
	public unsafe SymScope(ISymUnmanagedScope *pScope)
			{
				this.pScope = pScope;
				this.children = new ISymbolScope [0];
				this.locals = new ISymbolVariable [0];
			}
	internal SymScope(ISymbolMethod method)
			{
				// Create the root scope for a method.
				this.method = method;
				this.parent = null;
				this.children = new ISymbolScope [0];
				this.locals = new ISymbolVariable [0];
				this.endOffset = Int32.MaxValue;
				this.startOffset = 0;
			}
	private SymScope(SymScope parent, int endOffset, int startOffset)
			{
				// Create a new child scope.
				this.method = parent.method;
				this.parent = parent;
				this.children = new ISymbolScope [0];
				this.locals = new ISymbolVariable [0];
				this.endOffset = endOffset;
				this.startOffset = startOffset;
			}

	// Destructor (C++ style).
	~SymScope() {}
	public void __dtor()
			{
				GC.SuppressFinalize(this);
				Finalize();
			}

	// Implement the ISymbolScope interface.
	public virtual ISymbolScope[] GetChildren()
			{
				return children;
			}
	public virtual ISymbolVariable[] GetLocals()
			{
				return locals;
			}
	public virtual ISymbolNamespace[] GetNamespaces()
			{
				throw new NotSupportedException();
			}
	public virtual int EndOffset 
			{
 				get
				{
					return endOffset;
				}
 			}
	public virtual ISymbolMethod Method 
			{
 				get
				{
					return method;
				}
 			}
	public virtual ISymbolScope Parent 
			{
 				get
				{
					return parent;
				}
 			}
	public virtual int StartOffset 
			{
 				get
				{
					return startOffset;
				}
 			}

	// Insert a new scope into this one at a particular position.
	private SymScope InsertScope(int index, int endOffset, int startOffset)
			{
				SymScope scope = new SymScope(this, endOffset, startOffset);
				ISymbolScope[] newChildren;
				newChildren = new ISymbolScope [children.Length + 1];
				Array.Copy(children, 0, newChildren, 0, index);
				Array.Copy(children, index, newChildren, index + 1,
						   children.Length - index);
				newChildren[index] = scope;
				children = newChildren;
				return scope;
			}

	// Find or create a particular scope underneath this one.
	internal SymScope FindScope(int endOffset, int startOffset)
			{
				int posn = 0;
				while(posn < children.Length)
				{
					if(endOffset <= children[posn].StartOffset)
					{
						// Insert a new scope before this one.
						return InsertScope(posn, endOffset, startOffset);
					}
					else if(startOffset >= children[posn].EndOffset)
					{
						// Insert a new scope after this one.
						return InsertScope(posn + 1, endOffset, startOffset);
					}
					else if(startOffset == children[posn].StartOffset &&
							endOffset == children[posn].EndOffset)
					{
						// We already have this scope.
						return (SymScope)(children[posn]);
					}
					else if(startOffset <= children[posn].StartOffset &&
							endOffset >= children[posn].EndOffset)
					{
						// The child scope is completely contained,
						// so we need to insert a new scope level.
						SymScope scope = new SymScope
							(this, endOffset, startOffset);
						scope.children = new ISymbolScope [1];
						scope.children[0] = children[posn];
						children[posn] = scope;
						return scope;
					}
					else
					{
						// Insert within a child scope.
						return ((SymScope)(children[posn]))
							.FindScope(endOffset, startOffset);
					}
					++posn;
				}
				return InsertScope(children.Length, endOffset, startOffset);
			}

	// Find the scope that contains a particular offset.
	internal ISymbolScope FindOffset(int offset)
			{
				foreach(SymScope scope in children)
				{
					if(offset >= scope.startOffset && offset < scope.endOffset)
					{
						return scope.FindOffset(offset);
					}
				}
				return this;
			}

	// Add a local variable to this scope.
	internal void AddLocal(String name, int ilOffset)
			{
				SymVariable var = new SymVariable(name, ilOffset, this);
				ISymbolVariable[] newLocals;
				newLocals = new ISymbolVariable [locals.Length + 1];
				Array.Copy(locals, 0, newLocals, 0, locals.Length);
				newLocals[locals.Length] = var;
				locals = newLocals;
			}

}; // class SymScope

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics.SymbolStore
