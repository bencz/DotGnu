/*
 * XPathNavigator.cs - Implementation of the
 *		"System.Xml.XPath.XPathNavigator" class.
 *
 * Copyright (C) 2002 Southern Storm Software, Pty Ltd.
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
using System.Xml.XPath.Private;

namespace System.Xml.XPath
{

#if CONFIG_XPATH

[TODO]
#if ECMA_COMPAT
internal
#else
public
#endif
abstract class XPathNavigator : ICloneable
{
	// TODO

	// Constructor.
	protected XPathNavigator() {}

	// Implement the ICloneable interface.
	public abstract XPathNavigator Clone();

	Object ICloneable.Clone()
			{
				return Clone();
			}

	public virtual XmlNodeOrder ComparePosition(XPathNavigator nav)
			{
				if(IsSamePosition(nav))
				{
					return XmlNodeOrder.Same;
				}

				if(IsDescendant(nav))
				{
					return XmlNodeOrder.Before;
				}
				else if(nav.IsDescendant(this))
				{
					return XmlNodeOrder.After;
				}
				
				XPathNavigator copy = this.Clone();
				XPathNavigator other = nav.Clone();
				
				/* now, it gets expensive - we find the 
				   closest common ancestor. But these two
				   might be from totally different places.
				   
				   Someone should re-implement this somewhere,
				   so that it is faster for XmlDocument.
				 */
				int common = 0;
				int otherDepth = 0;
				int copyDepth = 0;
				
				copy.MoveToRoot();
				other.MoveToRoot();

				if(!copy.IsSamePosition(other))
				{
					return XmlNodeOrder.Unknown;
				}

				/* what do you think ? I'm made of GC space ? */
				copy.MoveTo(this);
				other.MoveTo(nav);	

				while(other.MoveToParent())
				{
					otherDepth++;
				}

				while(copy.MoveToParent())
				{
					copyDepth++;
				}

				common = (otherDepth > copyDepth) ? copyDepth : otherDepth;

				other.MoveTo(nav);
				copy.MoveTo(this);

				// traverse both till you get to depth == common
				for(;otherDepth > common; otherDepth--)
				{
					other.MoveToParent();
				}
				for(;copyDepth > common; copyDepth--)
				{
					copy.MoveToParent();
				}

				other.MoveTo(nav);
				copy.MoveTo(this);

				XPathNavigator copy1 = copy.Clone();
				XPathNavigator other1 = other.Clone();

				while(copy.IsSamePosition(other))
				{
					copy1.MoveTo(copy);
					other1.MoveTo(other);

					copy.MoveToParent();
					other.MoveToParent();
				}

				copy.MoveTo(copy1);
				other.MoveTo(other1);

				// Now copy & other are siblings and can be compared
				while(copy.MoveToNext())
				{
					if(copy.IsSamePosition(other))
					{
						return XmlNodeOrder.Before;
					}
				}

				return XmlNodeOrder.After;
			}

	public virtual XPathExpression Compile(String xpath)
			{
				XPathParser parser = new XPathParser();
				return parser.Parse(xpath);
			}

	public virtual Object Evaluate(XPathExpression expr)
			{
				return Evaluate(expr, new XPathSelfIterator(this,null));
			}

	public virtual Object Evaluate(XPathExpression expr, 
									XPathNodeIterator context)
			{
				if(expr is XPathExpressionBase)
				{
					return ((XPathExpressionBase)expr).Evaluate(context);
				}
				return null;
			}

	public virtual Object Evaluate(String xpath)
			{
				XPathExpression expr = Compile(xpath);
				return Evaluate(expr);
			}

	public abstract String GetAttribute(String localName, String namespaceURI);

	public abstract String GetNamespace(String name);

	public virtual bool IsDescendant(XPathNavigator nav)
			{
				if(nav != null)
				{
					nav = nav.Clone();
					while(nav.MoveToParent())
					{
						if(IsSamePosition(nav))
						{
							return true;
						}
					}
				}
				return false;
			}

	public abstract bool IsSamePosition(XPathNavigator other);

	[TODO]
	public virtual bool Matches(XPathExpression expr)
			{
				 throw new NotImplementedException("Matches");
			}

	[TODO]
	public virtual bool Matches(String xpath)
			{
				 throw new NotImplementedException("Matches");
			}

	public abstract bool MoveTo(XPathNavigator other);

	public abstract bool MoveToAttribute(String localName, String namespaceURI);

	public abstract bool MoveToFirst();

	public abstract bool MoveToFirstAttribute();

	public abstract bool MoveToFirstChild();

	public bool MoveToFirstNamespace()
			{
				return MoveToFirstNamespace(XPathNamespaceScope.All);
			}

	public abstract bool MoveToFirstNamespace(
								XPathNamespaceScope namespaceScope);

	public abstract bool MoveToId(String id);

	public abstract bool MoveToNamespace(String name);

	public abstract bool MoveToNext();

	public abstract bool MoveToNextAttribute();

	public bool MoveToNextNamespace()
			{
				return MoveToNextNamespace(XPathNamespaceScope.All);
			}

	public abstract bool MoveToNextNamespace(XPathNamespaceScope namespaceScope);

	public abstract bool MoveToParent();

	public abstract bool MoveToPrevious();

	public abstract void MoveToRoot();

	public virtual XPathNodeIterator Select(XPathExpression expr)
			{
				return (Evaluate(expr) as XPathNodeIterator);
			}

	public virtual XPathNodeIterator Select(String xpath)
			{
				return (Evaluate(xpath) as XPathNodeIterator);
			}

	[TODO]
	public virtual XPathNodeIterator SelectAncestors(XPathNodeType type, 
													bool matchSelf)
			{
				 throw new NotImplementedException("SelectAncestors");
			}

	[TODO]
	public virtual XPathNodeIterator SelectAncestors(String name, 
													String namespaceURI, 
													bool matchSelf)
			{
				 throw new NotImplementedException("SelectAncestors");
			}

	[TODO]
	public virtual XPathNodeIterator SelectChildren(XPathNodeType type)
			{
				 throw new NotImplementedException("SelectChildren");
			}

	[TODO]
	public virtual XPathNodeIterator SelectChildren(String name, 
													String namespaceURI)
			{
				 throw new NotImplementedException("SelectChildren");
			}

	[TODO]
	public virtual XPathNodeIterator SelectDescendants(XPathNodeType type, 
														bool matchSelf)
			{
				 throw new NotImplementedException("SelectDescendants");
			}

	[TODO]
	public virtual XPathNodeIterator SelectDescendants(String name, 
														String namespaceURI, 
														bool matchSelf)
			{
				 throw new NotImplementedException("SelectDescendants");
			}

	[TODO]
	public override String ToString()
			{
				 throw new NotImplementedException("ToString");
			}

	public abstract String BaseURI 
			{
				get;
			}

	public abstract bool HasAttributes 
			{
				get;
			}

	public abstract bool HasChildren 
			{
				get;
			}

	public abstract bool IsEmptyElement 
			{
				get;
			}

	public abstract String LocalName 
			{
				get;
			}

	public abstract String Name 
			{
				get;
			}

	public abstract XmlNameTable NameTable 
			{
				get;
			}

	public abstract String NamespaceURI 
			{
				get;
			}

	public abstract XPathNodeType NodeType 
			{
				get;
			}

	public abstract String Prefix 
			{
				get;
			}

	public abstract String Value 
			{
				get;
			}

	public abstract String XmlLang 
			{
				get;
			}


}; // class XPathNavigator

#endif // CONFIG_XPATH

}; // namespace System.Xml.XPath
