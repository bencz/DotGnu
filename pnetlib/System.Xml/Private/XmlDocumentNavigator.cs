/*
 * XmlDocumentNavigator.cs - Implementation of the
 *		"System.Xml.Private.XmlDocumentNavigator" class.
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

namespace System.Xml.Private
{

#if CONFIG_XPATH

using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections;


internal class XmlDocumentNavigator : XPathNavigator, IHasXmlNode
{
	private XmlNode node;
	private XmlAttribute nsAttr = null;
	private XmlDocument document;

	/* xml:xmlns="http://www.w3.org/XML/1998/namespace" */
	private XmlAttribute xmlAttr = null;

	private ArrayList nsNames = null;

	public XmlDocumentNavigator(XmlNode node) : base()
	{
		this.node = node;
		this.document = (node is XmlDocument) ? 
							(XmlDocument)node : node.OwnerDocument;
		this.xmlAttr = document.CreateAttribute("xmlns", "xml", 
												XmlDocument.xmlns);
		this.xmlAttr.Value = XmlDocument.xmlnsXml;

		this.nsNames = null;
	}

	public XmlDocumentNavigator(XmlDocumentNavigator copy)
	{
		this.MoveTo(copy);
	}

	public override XPathNavigator Clone()
	{
		return new XmlDocumentNavigator(this);
	}

	public override String GetAttribute(String localName, String namespaceURI)
	{
		XmlAttribute attr = node.Attributes[localName, namespaceURI] ;

		if(attr != null)
		{
			return attr.Value;
		}
		return null;
	}


	public override String GetNamespace(String name)
	{
		return node.GetNamespaceOfPrefix(name);
	}

	public override bool IsSamePosition(XPathNavigator other)
	{
		XmlDocumentNavigator nav = (other as XmlDocumentNavigator);
		return ((nav != null) && (nav.node == node) && (nav.nsAttr == nsAttr));
	}
	
	public override bool MoveTo(XPathNavigator other)
	{
		XmlDocumentNavigator nav = (other as XmlDocumentNavigator);
		if(nav != null)
		{
			node = nav.node;
			nsAttr = nav.nsAttr;
			document = nav.document;
			xmlAttr = nav.xmlAttr;
			if(nav.nsNames == null || nav.nsNames.IsReadOnly)
			{
				nsNames = nav.nsNames;
			}
			else
			{
				nsNames = ArrayList.ReadOnly(nav.nsNames);
			}
			return true;
		}
		return false;
	}

	public override bool MoveToAttribute(String localName, String namespaceURI)
	{
		if(node.Attributes != null)
		{
			foreach(XmlAttribute attr in node.Attributes)
			{
				// TODO : can this be an Object Ref compare ?
				if(attr.LocalName == localName && 
					attr.NamespaceURI == namespaceURI)
				{
					node = attr;
					NamespaceAttribute = null;
					return true;
				}
			}
		}
		return false;
	}

	public override bool MoveToFirst()
	{
		// TODO : is this correct ?. Will a Text qualify as a first node ?
		if(node.NodeType != XmlNodeType.Attribute && node.ParentNode != null)
		{
			node = node.ParentNode.FirstChild;
			return true;
		}
		return false;
	}

	public override bool MoveToFirstAttribute()
	{
		if(NodeType == XPathNodeType.Element && node.Attributes != null)
		{
			foreach(XmlAttribute attr in node.Attributes)
			{
				if(attr.NamespaceURI != XmlDocument.xmlns)
				{
					node = attr;
					NamespaceAttribute = null;
					return true;
				}
			}
		}
		return false;
	}

	public override bool MoveToFirstChild()
	{
		if(!HasChildren)
		{
			return false;
		}

		XmlNode next = node.FirstChild;
		// TODO: implement normalization
		while(next!= null && 
				(next.NodeType == XmlNodeType.EntityReference || 
				next.NodeType == XmlNodeType.DocumentType ||
				next.NodeType == XmlNodeType.XmlDeclaration))
		{
			next = next.NextSibling;
		}

		if(next != null)
		{
			node = next;
			return true;
		}
		return false;
	}

	public override bool MoveToFirstNamespace(
								XPathNamespaceScope namespaceScope)
	{
		if(NodeType != XPathNodeType.Element)
		{
			return false;
		}

		XmlElement element = (XmlElement)node;
		while(element != null)
		{
			if(element.Attributes != null)
			{
				foreach(XmlAttribute attr in element.Attributes)
				{
					if(attr.NamespaceURI == XmlDocument.xmlns 
						&& !CheckForDuplicateNS(attr.Name, attr.Value))
					{
						NamespaceAttribute = attr;
						return true;
					}
				}
			}
			if(namespaceScope == XPathNamespaceScope.Local)
			{
				return false;
			}

			element = element.ParentNode as XmlElement;
		}

		if(namespaceScope == XPathNamespaceScope.All)
		{
			if(!CheckForDuplicateNS(xmlAttr.Name, xmlAttr.Value))
			{
				NamespaceAttribute = xmlAttr;
				return true;
			}
		}
		return false;
	}

	public override bool MoveToId(String id)
	{
		return false;
	}

	public override bool MoveToNamespace(String name)
	{
		if(name == "xml")
		{
			/* seems to be that xml namespaces are valid
			   wherever you are ? */
			NamespaceAttribute = xmlAttr;
			return true;
		}
		
		if(NodeType != XPathNodeType.Element)
		{
			return false;
		}

		XmlElement element = (XmlElement)node;
		while(element != null)
		{
			if(element.Attributes != null)
			{
				foreach(XmlAttribute attr in element.Attributes)
				{
					if(attr.NamespaceURI == XmlDocument.xmlns
						&& !CheckForDuplicateNS(attr.Name,attr.Value))
					{
						NamespaceAttribute = attr;
						return true;
					}
				}
			}
			element = element.ParentNode as XmlElement;
		}

		return false;
	}

	public override bool MoveToNext()
	{
		if(nsAttr != null)
		{
			return false;
		}

		XmlNode next = node.NextSibling;
		// TODO: implement normalization
		while(next!= null && 
				(next.NodeType == XmlNodeType.EntityReference || 
				next.NodeType == XmlNodeType.DocumentType ||
				next.NodeType == XmlNodeType.XmlDeclaration))
		{
			next = next.NextSibling;
		}
		
		if(next	!= null)
		{
			node = next;
			return true;	
		}
		
		return false;
	}

	public override bool MoveToNextAttribute()
	{
		if(NodeType == XPathNodeType.Attribute)
		{
			int i;
			XmlElement owner = ((XmlAttribute)node).OwnerElement;

			if(owner == null)
			{
				return false;
			}
			
			XmlAttributeCollection list = owner.Attributes;
			
			if(list == null)
			{
				return false;
			}

			for(i=0 ; i<list.Count ; i++)
			{
				// This should be a lot faster
				if(((Object)list[i]) == ((Object)node))
				{
					i++; /* Move to Next */
					break;
				}
			}

			if(i != list.Count)
			{
				node = list[i];
				NamespaceAttribute = null;
				return true;
			}
		}
		return false;
	}

	private XmlAttribute GetNextNamespace(XmlElement owner, 
											XmlAttribute current)
	{
		for(int i = 0; i < owner.Attributes.Count; i++)
		{
			XmlAttribute attr = owner.Attributes[i];
			if(((Object)attr) == ((Object)current) || current == null)
			{
				for(int j = i+1; j < owner.Attributes.Count; j++)
				{
					attr = owner.Attributes[j];
					if(attr.NamespaceURI == XmlDocument.xmlns 
						&& !CheckForDuplicateNS(attr.Name, attr.Value))
					{
						return attr;	
					}
				}
			}
		}

		return null;
	}

	public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
	{
		if(nsAttr == null)
		{
			return false;
		}
		
		XmlElement owner = nsAttr.OwnerElement;
		
		if(owner == null || ((Object)nsAttr) == ((Object)this.xmlAttr)) 
		{
			/* technically, I don't think we need the extra condition
			   because xmlAttr won't be attached to an element, but
			   it's there because it makes it clear :) */
			return false;
		}

		XmlAttribute nextNs = GetNextNamespace(owner, nsAttr);

		if(nextNs != null)
		{
			NamespaceAttribute = nextNs;
			return true;
		}

		if(namespaceScope == XPathNamespaceScope.Local)
		{
			return false;
		}
		
		for(XmlNode node = owner.ParentNode;
				node != null; node = node.ParentNode)
		{
			owner = (node as XmlElement);
			if(owner == null)
			{
				continue;
			}
			nextNs = GetNextNamespace(owner, null);
			if(nextNs != null)
			{
				NamespaceAttribute = nextNs;
				return true;
			}
		}
		
		if(namespaceScope == XPathNamespaceScope.All)
		{
			if(!CheckForDuplicateNS(xmlAttr.Name, xmlAttr.Value))
			{
				NamespaceAttribute = xmlAttr;
				return true;
			}
		}
		return false;
	}
	

	public override bool MoveToParent()
	{
		if(nsAttr != null)
		{
			/* the scary part is the MoveToNextNamespace
			   function where you just traverse up. So
			   there is no guarantee that parent node of
			   nsAttr is the next node you want */
			NamespaceAttribute = null;
			return true;
		}

		if(node.NodeType == XmlNodeType.Attribute)
		{
			XmlElement owner = ((XmlAttribute)node).OwnerElement;
			if(owner != null)
			{
				node = owner;
				NamespaceAttribute = null;
				return true;
			}
		}
		else if (node.ParentNode != null)
		{
			node = node.ParentNode;
			NamespaceAttribute = null;
			return true;
		}
		return false;
	}

	public override bool MoveToPrevious()
	{
		if(nsAttr != null)
		{
			return true;
		}

		if(node.PreviousSibling != null)
		{
			node = node.PreviousSibling;
			return true;
		}
		return false;
	}

	public override void MoveToRoot()
	{
		// TODO: make sure we don't use this for fragments
		if(document != null && document.DocumentElement != null)
		{
			node = document;
		}
		NamespaceAttribute = null;
		return;
	}
	
	public override String BaseURI 
	{
		get
		{
			return node.BaseURI;
		}
	}

	public override bool HasAttributes
	{
		get
		{
			if(nsAttr == null && node.Attributes != null)
			{
				return (node.Attributes.Count != 0);
			}
			return false;
		}
	}

	public override bool HasChildren
	{
		get
		{
			return (nsAttr == null && node.FirstChild != null);	
		}
	}

	public override bool IsEmptyElement
	{
		get
		{
			if(nsAttr == null && node.NodeType == XmlNodeType.Element)
			{
				return ((XmlElement)node).IsEmpty;
			}
			return false;
		}
	}

	public override String LocalName
	{
		get
		{
			XPathNodeType nodeType = NodeType;
			
			if(nodeType == XPathNodeType.Element ||
				nodeType == XPathNodeType.Attribute ||
				nodeType == XPathNodeType.ProcessingInstruction)
			{
				return node.LocalName;
			}
			else if(nodeType == XPathNodeType.Namespace)
			{
				return nsAttr.LocalName;
			}
			return String.Empty;
		}
	}

	public override String Name
	{
		get
		{
			XPathNodeType nodeType = NodeType;
			
			if(nodeType == XPathNodeType.Element ||
				nodeType == XPathNodeType.Attribute ||
				nodeType == XPathNodeType.ProcessingInstruction)
			{
				return node.Name;
			}
			else if(nodeType == XPathNodeType.Namespace)
			{
				return LocalName;
			}
			return String.Empty;
		}
	}

	public override XmlNameTable NameTable
	{
		get
		{
			return document.NameTable;
		}
	}

	public override String NamespaceURI
	{
		get
		{
			if(nsAttr != null) 
			{
				return String.Empty;
			}
			return node.NamespaceURI;
		}
	}

	public override XPathNodeType NodeType
	{
		get
		{
			if(nsAttr != null)
			{
				return XPathNodeType.Namespace;
			}

			switch(node.NodeType)
			{
				case XmlNodeType.Element:
				{
					return XPathNodeType.Element;
				}
				break;
				case XmlNodeType.Comment:
				{
					return XPathNodeType.Comment;
				}
				break;
				case XmlNodeType.Attribute:
				{
					return XPathNodeType.Attribute;
				}
				break;
				case XmlNodeType.Text:
				{
					return XPathNodeType.Text;
				}
				break;
				case XmlNodeType.Whitespace:
				{
					return XPathNodeType.Whitespace;
				}
				break;
				case XmlNodeType.SignificantWhitespace:
				{
					return XPathNodeType.SignificantWhitespace;
				}
				break;
				case XmlNodeType.ProcessingInstruction:
				{
					return XPathNodeType.ProcessingInstruction;
				}
				break;
				case XmlNodeType.Document:
				{
					return XPathNodeType.Root;
				}
				break;
			}
			// TODO resources
			throw new InvalidOperationException(
				String.Format("Invalid XPathNodeType for: {0}", 
							 node.NodeType)); 
		}
	}

	public override String Prefix
	{
		get
		{
			return node.Prefix;
		}
	}

	public override String Value
	{
		get
		{
			switch(NodeType)
			{
				case XPathNodeType.Attribute:
				case XPathNodeType.Comment:
				case XPathNodeType.ProcessingInstruction:
				{
					return node.Value;
				}
				break;
				case XPathNodeType.Text:
				case XPathNodeType.Whitespace:
				case XPathNodeType.SignificantWhitespace:
				{
					// TODO : normalize
					return node.Value;
				}
				break;
				case XPathNodeType.Element:
				case XPathNodeType.Root:
				{
					return node.InnerText;
				}
				break;
				case XPathNodeType.Namespace:
				{
					return nsAttr.Value; 
				}
				break;
			}
			return String.Empty;
		}
	}

	public override String XmlLang
	{
		get
		{
			return String.Empty;
		}
	}

	public override String ToString()
	{
		return String.Format("<XPathNavigator {0} , {1}>", node,document);
	}

	internal XmlNode CurrentNode
	{
		get
		{
			if(this.nsAttr != null) 
			{
				return this.nsAttr;
			}
			return this.node;
		}
	}

	internal XmlAttribute NamespaceAttribute 
	{
		get
		{
			return this.nsAttr;
		}
		set
		{
			this.nsAttr = value;

			if(value != null)
			{
				if(this.nsNames == null)
				{
					this.nsNames = new ArrayList();
				}
				else if(this.nsNames.IsReadOnly)
				{
					this.nsNames = new ArrayList(this.nsNames);
				}

				this.nsNames.Add(value.Value);
			}
			else
			{
				this.nsNames = null;
			}
		}
	}

	// return true if the namespace has been seen before 
	private bool CheckForDuplicateNS(String name, String ns)
	{
		// XmlNameTable to the rescue, we can compare names as objects
		if(this.nsNames != null && this.nsNames.Contains((Object)name))
		{
			// duplicate 
			return true;
		}

		/* tricky part: setting xmlns='' in your XML causes 
		   the default namespace to be *seen* but forgotten.
		*/
		
		if(ns == String.Empty)
		{
			if(this.nsNames == null)
			{
				this.nsNames = new ArrayList();
			}
			else if(this.nsNames.IsReadOnly)
			{
				this.nsNames = new ArrayList(this.nsNames);
			}
			this.nsNames.Add(NameTable.Get("xmlns"));

			return true;
		}

		return false;
	}

	XmlNode IHasXmlNode.GetNode()
	{
		return CurrentNode;
	}
}

#endif // CONFIG_XPATH

}
