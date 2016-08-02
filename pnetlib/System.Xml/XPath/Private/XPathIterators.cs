/*
 * XPathIterators.cs - implementation subclasses of XPathNodeIterator.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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
using System.Xml;
using System.Xml.XPath;
using System.Collections;

#if CONFIG_XPATH

namespace System.Xml.XPath.Private
{
	internal abstract class XPathBaseIterator : XPathNodeIterator
	{
		private int count = -1;
		/* reference hell :) */
		private XmlNamespaceManager nsManager;

		public XPathBaseIterator (XPathBaseIterator parent)
		{
			this.nsManager = parent.nsManager;
		}
		
		public XPathBaseIterator (XmlNamespaceManager nsmanager)
		{
			this.nsManager = nsManager;
		}

		public override int Count
		{
			get
			{
				lock(this)
				{
					if(count == -1)
					{
						count = 0;
						while(MoveNext())
						{
							count++;
						}
					}
				}
				return count;
			}
		}

		public XmlNamespaceManager NamespaceManager
		{
			get
			{
				return this.nsManager;
			}
		}
	}

	internal abstract class XPathSimpleIterator : XPathBaseIterator
	{
		protected readonly XPathNavigator navigator;
		protected readonly XPathBaseIterator parent;
		protected XPathNavigator current;
		protected int pos;

		public XPathSimpleIterator(XPathBaseIterator parent) : base (parent)
		{
			this.parent = parent;
			navigator = parent.Current.Clone();
			current = navigator.Clone();
			pos = 0;
		}

		public XPathSimpleIterator(XPathNavigator navigator, 
								   XmlNamespaceManager nsmanager) 
								   : base(nsmanager)
		{
			this.navigator = navigator.Clone();
			current = navigator.Clone();
		}
		
		public override XPathNavigator Current
		{
			get
			{
				return current;
			}
		}

		public override int CurrentPosition
		{
			get
			{
				return pos;
			}
		}
	}

	internal class XPathSelfIterator : XPathSimpleIterator
	{
		public XPathSelfIterator(XPathBaseIterator parent) : base(parent)
		{
		}

		public XPathSelfIterator(XPathNavigator navigator, 
								 XmlNamespaceManager nsmanager) 
								 : base(navigator, nsmanager)
		{
		}

		public XPathSelfIterator (XPathSelfIterator copy) : base(copy) 
		{
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathSelfIterator(this);
		}

		public override bool MoveNext()
		{
			if(pos == 0)
			{
				pos = 1;
				return true;
			}
			return false;
		}
	}

	internal class XPathChildIterator : XPathSimpleIterator
	{
		public XPathChildIterator (XPathBaseIterator iterator) : base (iterator)
		{
		}

		public XPathChildIterator(XPathChildIterator copy) : base (copy)
		{
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathChildIterator(this);
		}

		public override bool MoveNext()
		{
			if(pos == 0)
			{
				if(navigator.MoveToFirstChild())
				{
					pos++;
					current = navigator.Clone();
					return true;
				}
			}
			else if(navigator.MoveToNext())
			{
				pos++;
				current = navigator.Clone();
				return true;
			}

			return false;
		}
	}
	
	internal class XPathAttributeIterator : XPathSimpleIterator
	{
		public XPathAttributeIterator (XPathBaseIterator iterator) : base (iterator)
		{
		}

		public XPathAttributeIterator(XPathAttributeIterator copy) : base (copy)
		{
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathAttributeIterator(this);
		}

		public override bool MoveNext()
		{
			if(pos == 0)
			{
				if(navigator.MoveToFirstAttribute())
				{
					pos++;
					current = navigator.Clone();
					return true;
				}
			}
			else if(navigator.MoveToNextAttribute())
			{
				pos++;
				current = navigator.Clone();
				return true;
			}

			return false;
		}
	}
	
	internal class XPathAncestorIterator : XPathSimpleIterator
	{
		ArrayList parents = null;

		public XPathAncestorIterator (XPathBaseIterator iterator) : base (iterator)
		{
		}

		public XPathAncestorIterator(XPathAncestorIterator copy) : base (copy)
		{
			// TODO : do we need to clone this ?
			if(this.parents != null)
			{
				this.parents = (ArrayList)this.parents.Clone();
			}
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathAncestorIterator(this);
		}

		public override bool MoveNext()
		{
			if(parents == null)
			{
				parents = new ArrayList();
				while(navigator.MoveToParent())
				{
					if(navigator.NodeType == XPathNodeType.Root)
					{
						break;
					}
					// TODO: duplicate check
					parents.Add(navigator.Clone());
				}
				parents.Reverse();
			}

			if(pos < parents.Count)
			{
				current = (XPathNavigator)parents[pos];
				pos++;
				return true;
			}

			return false;
		}
	}
	
	internal class XPathAncestorOrSelfIterator : XPathSimpleIterator
	{
		ArrayList parents = null;

		public XPathAncestorOrSelfIterator (XPathBaseIterator iterator) : base (iterator)
		{
		}

		public XPathAncestorOrSelfIterator(XPathAncestorOrSelfIterator copy) : base (copy)
		{
			// TODO : do we need to clone this ?
			if(this.parents != null)
			{
				this.parents = (ArrayList)this.parents.Clone();
			}
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathAncestorOrSelfIterator(this);
		}

		public override bool MoveNext()
		{
			if(parents == null)
			{
				parents = new ArrayList();
				parents.Add(navigator.Clone());
				while(navigator.MoveToParent())
				{
					if(navigator.NodeType == XPathNodeType.Root)
					{
						break;
					}
					// TODO: duplicate check
					parents.Add(navigator.Clone());
				}
				parents.Reverse();
			}

			if(pos < parents.Count)
			{
				current = (XPathNavigator)parents[pos];
				pos++;
				return true;
			}

			return false;
		}
	}

	internal class XPathParentIterator : XPathSimpleIterator
	{
		public XPathParentIterator (XPathBaseIterator iterator) : base (iterator)
		{
		}

		public XPathParentIterator(XPathParentIterator copy) : base (copy)
		{
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathParentIterator(this);
		}

		public override bool MoveNext()
		{
			if(pos == 0)
			{
				if(navigator.MoveToParent())
				{
					pos++;
					current = navigator.Clone();
					return true;
				}
			}

			return false;
		}
	}

	internal class XPathDescendantIterator : XPathSimpleIterator
	{
		private int depth  = 0 ;
		private bool finished = false; 
		
		public XPathDescendantIterator (XPathBaseIterator iterator) : base (iterator)
		{
		}

		public XPathDescendantIterator(XPathDescendantIterator copy) : base (copy)
		{
			this.depth = copy.depth;
			this.finished = copy.finished;
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathDescendantIterator(this);
		}

		public override bool MoveNext()
		{
			// This is needed as at the end of the loop, the original will be restored 
			// as the navigator value.
			if(finished) 
			{
				return false;
			}

			if(navigator.MoveToFirstChild())
			{
				depth++;
				pos++;
				current = navigator.Clone();
				return true;
			}
			while(depth != 0)
			{
				if(navigator.MoveToNext())
				{
					pos++;
					current = navigator.Clone();
					return true;
				}
				if(!navigator.MoveToParent())
				{
					// TODO: resources
					throw new XPathException("there should be parent for depth != 0" , null);
				}
				depth--;
			}

			finished = true;

			return false;
		}
	}

	internal class XPathDescendantOrSelfIterator : XPathSimpleIterator
	{
		private int depth  = 0 ;
		private bool finished = false; 
		
		public XPathDescendantOrSelfIterator (XPathBaseIterator iterator) : base (iterator)
		{
		}

		public XPathDescendantOrSelfIterator(XPathDescendantOrSelfIterator copy) : base (copy)
		{
			this.depth = copy.depth;
			this.finished = copy.finished;
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathDescendantOrSelfIterator(this);
		}

		public override bool MoveNext()
		{
			// This is needed as at the end of the loop, the original 
			// will be restored as the navigator value.
			if(finished) 
			{
				return false;
			}

			if(pos == 0)
			{
				pos++;
				current = navigator.Clone();
				return true;
			}

			if(navigator.MoveToFirstChild())
			{
				depth++;
				pos++;
				current = navigator.Clone();
				return true;
			}
			while(depth != 0)
			{
				if(navigator.MoveToNext())
				{
					pos++;
					current = navigator.Clone();
					return true;
				}
				if(!navigator.MoveToParent())
				{
					// TODO: resources
					throw new XPathException("there should be parent for depth != 0" , null);
				}
				depth--;
			}

			finished = true;

			return false;
		}
	}

	internal class XPathPrecedingIterator : XPathSimpleIterator
	{
		private bool finished = false; 
		private XPathNavigator start = null;
		
		public XPathPrecedingIterator (XPathBaseIterator iterator) : base (iterator)
		{
			/* remember where we started from and go up the tree */
			start = iterator.Current.Clone();
			this.navigator.MoveToRoot();
		}

		public XPathPrecedingIterator(XPathPrecedingIterator copy) : base (copy)
		{
			this.finished = copy.finished;
			this.start = copy.start;
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathPrecedingIterator(this);
		}

		public override bool MoveNext()
		{
			if(finished) 
			{
				return false;
			}
			
			/* the lesson seems to be that you really
			   mean ::preceding-sibling half the time
			   you say ::preceding. This is expensive
			   as hell !!! */
			do
			{
				while(!navigator.MoveToFirstChild())
				{
					/* every sibling is traversed before any child */
					while(!navigator.MoveToNext())
					{
						if(!navigator.MoveToParent())
						{
							/* we hit the root node */
							finished = true;
							return false;
						}
					}
				}
			}
			while(navigator.IsDescendant(start));

			/* start is not a descendant of current node */
			if(navigator.ComparePosition(this.start) == XmlNodeOrder.Before)
			{
				pos++;
				current = navigator.Clone();	
				return true;
			}
			
			finished = true;
			return false;
		}
	}

	internal class XPathPrecedingSiblingIterator : XPathSimpleIterator
	{
		private bool finished = false; 
		private bool started = false;
		private XPathNavigator start = null;
		
		public XPathPrecedingSiblingIterator (XPathBaseIterator iterator) 
			: base (iterator)
		{
			start = iterator.Current.Clone();
			if(start.NodeType == XPathNodeType.Attribute ||
				start.NodeType == XPathNodeType.Namespace)
			{
				finished = true;
			}
			navigator.MoveToFirst();
		}

		public XPathPrecedingSiblingIterator(XPathPrecedingSiblingIterator copy)
			: base (copy)
		{
			this.finished = copy.finished;
			this.started = copy.started;
			this.start = copy.start;
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathPrecedingSiblingIterator(this);
		}

		public override bool MoveNext()
		{
			if(finished) 
			{
				return false;
			}

			if((!started) || navigator.MoveToNext())
			{
				started = true;
				if(navigator.ComparePosition(start) == XmlNodeOrder.Before)
				{
					pos++;
					current = navigator.Clone();
					return true;
				}
			}

			/* the code will never again  reach this line 
			   thanks to the first if(finished)
			   started = true; 
			*/
			finished = true;
			return false;
		}
	}

	internal class XPathFollowingIterator : XPathSimpleIterator
	{
		private bool finished = false; 
		
		public XPathFollowingIterator (XPathBaseIterator iterator) : base (iterator)
		{
		}

		public XPathFollowingIterator(XPathFollowingIterator copy) : base (copy)
		{
			this.finished = copy.finished;
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathFollowingIterator(this);
		}

		public override bool MoveNext()
		{
			// This is needed as at the end of the loop, the original 
			// will be restored as the navigator value.
			if(finished) 
			{
				return false;
			}
			bool movedToParent = false;

			if(this.pos == 0)
			{
				if(navigator.NodeType == XPathNodeType.Attribute ||
					navigator.NodeType == XPathNodeType.Namespace)
				{
					/* this is almost always true ? */
					movedToParent = navigator.MoveToParent();
				}
			}

			if(this.pos != 0 || movedToParent)
			{
				if(navigator.MoveToFirstChild())
				{
					pos++;
					current = navigator.Clone();
					return true;
				}
			}

			do 
			{
				if(navigator.MoveToNext())
				{
					pos++;
					current = navigator.Clone();
					return true;
				}
			}
			while(navigator.MoveToParent());

			finished = true;
			return false;
		}
	}

	internal class XPathFollowingSiblingIterator : XPathSimpleIterator
	{
		public XPathFollowingSiblingIterator (XPathBaseIterator iterator) 
			: base (iterator)
		{
		}

		public XPathFollowingSiblingIterator(XPathFollowingSiblingIterator copy)
			: base (copy)
		{
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathFollowingSiblingIterator(this);
		}

		public override bool MoveNext()
		{
			if(navigator.NodeType == XPathNodeType.Attribute ||
				navigator.NodeType == XPathNodeType.Namespace)
			{
				/* nodemaps not siblings */
				return false;
			}
			if(navigator.MoveToNext())
			{
				pos++;
				current = navigator.Clone();
				return true;
			}
			return false;
		}
	}
	
	internal class XPathNamespaceIterator : XPathSimpleIterator
	{
		public XPathNamespaceIterator (XPathBaseIterator iterator) 
			: base (iterator)
		{
		}

		public XPathNamespaceIterator(XPathNamespaceIterator copy)
			: base (copy)
		{
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathNamespaceIterator(this);
		}

		public override bool MoveNext()
		{
			if((pos == 0 && navigator.MoveToFirstNamespace()) ||
				navigator.MoveToNextNamespace())
			{
				pos++;
				current = navigator.Clone();
				return true;
			}
			return false;
		}
	}

	internal class XPathAxisIterator : XPathBaseIterator
	{
		protected XPathSimpleIterator iterator;
		protected NodeTest test;
		protected int pos;

		public XPathAxisIterator(XPathSimpleIterator iterator, 
									NodeTest test)
			: base(iterator)
		{
			this.iterator = iterator;
			this.test = test;
		}

		public XPathAxisIterator(XPathAxisIterator copy)
			: base(copy)
		{
			iterator = (XPathSimpleIterator) (copy.iterator.Clone());
			test = copy.test;
			pos = copy.pos;
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathAxisIterator(this);
		}

		public override bool MoveNext()
		{
			String name = null; 
			String ns = null;
			String nsURI = null;
			
			if(test.name != null)
			{
				name = test.name.Name;
				ns = test.name.Namespace;
				if(ns != null && this.NamespaceManager != null)
				{
					nsURI = this.NamespaceManager.LookupNamespace(ns);
				}
			}
			while(iterator.MoveNext())
			{
				if(test.nodeType != XPathNodeType.All && 
						test.nodeType != Current.NodeType)
				{
					continue;
				}
				if(nsURI != null && Current.NamespaceURI != nsURI)
				{
					continue;
				}
				if(name != null && Current.LocalName != name)
				{
					continue;
				}
				pos++;
				return true;
			}
			return false;
		}

		public override XPathNavigator Current 
		{
			get
			{
				return iterator.Current;
			}
		}

		public override int CurrentPosition
		{
			get
			{
				return pos;
			}
			
		}
	}

	internal class XPathSlashIterator : XPathBaseIterator 
	{
		protected XPathBaseIterator lhs;
		protected XPathBaseIterator rhs = null;
		protected Expression  expr;
		protected int pos;

		public XPathSlashIterator (XPathBaseIterator lhs, Expression expr)
			: base(lhs)
		{
			this.lhs = 	lhs;
			this.expr =  expr;
		}

		public XPathSlashIterator(XPathSlashIterator copy)
			: base(copy)
		{
			this.lhs = (XPathBaseIterator) copy.lhs.Clone();
			this.expr = copy.expr;
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathSlashIterator(this);
		}

		public override bool MoveNext()
		{
			//TODO: depth first traversal - fix when doing Evaluate
			while(rhs == null || !rhs.MoveNext())
			{
				if(!lhs.MoveNext())
				{
					return false;
				}
				rhs = (XPathBaseIterator)expr.Evaluate(lhs);
			}
			
			pos++;
			// We have already done an rhs.MoveNext()
			return true;
		}

		public override XPathNavigator Current
		{
			get
			{
				return rhs.Current;
			}
		}

		public override int CurrentPosition
		{
			get
			{
				return pos;
			}
		}
	}

	internal class XPathPredicateIterator : XPathBaseIterator
	{
		protected XPathBaseIterator iterator;
		protected Expression predicate;
		protected int pos;
		protected XPathResultType resultType;

		public XPathPredicateIterator(XPathBaseIterator iterator,
									  Expression predicate) 
									  : base (iterator)
		{
			this.iterator = iterator;
			this.predicate = predicate;
			this.resultType = predicate.ReturnType;
		}

		public XPathPredicateIterator(XPathPredicateIterator copy) : base(copy)
		{
			this.iterator = (XPathBaseIterator) copy.iterator.Clone();
			this.predicate = copy.predicate;
			this.resultType = copy.resultType;
			this.pos = copy.pos;
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathPredicateIterator(this);
		}

		public override bool MoveNext()
		{
			while(iterator.MoveNext())
			{
				bool match = false;
				switch(predicate.ReturnType)
				{
					case XPathResultType.String:
					case XPathResultType.NodeSet:
					case XPathResultType.Boolean:
					{
						match = (bool)predicate.EvaluateAs(iterator, XPathResultType.Boolean);
					}
					break;
					case XPathResultType.Number:
					{
						match = (iterator.CurrentPosition)  == (double)predicate.Evaluate(iterator);
					}
					break;
					default:
					{
						throw new NotSupportedException("TODO: " + predicate.ReturnType);
					}
					break;
				}
				if(match)
				{
					pos++;
					return true;
				}
			}
			return false;
		}

		public override XPathNavigator Current
		{
			get
			{
				return iterator.Current;
			}
		}

		public override int CurrentPosition
		{
			get
			{
				return pos;
			}
		}
	}

	internal class XPathUnionIterator : XPathSimpleIterator
	{
		XPathBaseIterator left;
		XPathBaseIterator right;
		bool moveLeft = true;
		bool moveRight = true;

		public XPathUnionIterator(XPathBaseIterator iterator, 
									XPathBaseIterator left,
									XPathBaseIterator right)
			: base(iterator)
		{
			this.left = left;
			this.right = right;
			this.current = null;
		}

		/* caveat: the base ctor gets the useless parent arg */
		public XPathUnionIterator(XPathUnionIterator copy) : base(copy.parent)
		{
			this.left = (XPathBaseIterator)copy.left.Clone();
			this.right = (XPathBaseIterator)copy.right.Clone();
			this.moveRight = copy.moveRight;
			this.moveLeft = copy.moveLeft;
		}

		public override bool MoveNext()
		{
			// TODO: investigate removing these variables
			bool movedRight = false;
			bool movedLeft = false;

			if(!moveLeft && !moveRight)
			{
				return false;
			}

			if(moveLeft)
			{
				movedLeft = left.MoveNext();
			}
			if(moveRight)
			{
				movedRight = right.MoveNext();
			}

			if(moveLeft && !movedLeft && moveRight && !movedRight)
			{
				return false;
			}

			if(moveLeft && !movedLeft)
			{
				moveLeft = false;
				current = right.Current.Clone();
				pos++;
				return true;
			}

			if(moveRight && !movedRight)
			{
				/* from next time, don't move right */
				moveRight = false;
				current = left.Current.Clone();
				pos++;
				return true;
			}

			// both moves were successful or we had leftover nodes from 
			// one side now we need to chose which side to output first
			
			XmlNodeOrder order = left.Current.ComparePosition(right.Current);

			switch(order)
			{
				case XmlNodeOrder.Same:
				{
					moveRight = moveLeft = true;
					current = left.Current.Clone();
					pos++;
					return true;
				}
				break;

				case XmlNodeOrder.Before:
				case XmlNodeOrder.Unknown:
				{
					moveLeft = true;
					moveRight = false;
					current = left.Current.Clone();
					pos++;
					return true;
				}
				break;

				case XmlNodeOrder.After:
				{
					moveLeft = false;
					moveRight = true;
					current = right.Current.Clone();
					pos++;
					return true;
				}
				break;
				default:
				{
					throw new XPathException("Could understand node relationship : "+order.ToString(), null);
				}
				break;
			}

			return false;
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathUnionIterator(this);
		}
	}

}

#endif // CONFIG_XPATH
