/*
 * XmlNodeReader.cs - Implementation of the "System.Xml.XmlNodeReader" class.
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
 
namespace System.Xml
{

#if !ECMA_COMPAT

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Globalization;

public class XmlNodeReader : XmlReader
{
	// Internal state.
	private XmlNode startNode;
	private XmlNode currentNode;
	private XmlDocument doc;
	private ReadState readState;

	private bool isAttributeReader = false;
	private bool isEndElement = false;
	private bool skipThisNode = true;
	private int attributePosn;
	private int depth = 0;
	private bool inAttrValue = false;

	// Constructor.
	public XmlNodeReader(XmlNode node)
	{
		startNode = currentNode = node;
		if(node.NodeType == XmlNodeType.Document)
		{
			doc = (XmlDocument) (node);
		}
		else
		{
			doc = node.OwnerDocument;
		}
		if(node.NodeType == XmlNodeType.Attribute)
		{
			this.isAttributeReader = true;
		}
		if(node.NodeType != XmlNodeType.Document &&
			node.NodeType != XmlNodeType.DocumentFragment)
		{
			skipThisNode = false;
		}
		this.readState = ReadState.Initial;
	}

	// Clean up the resources that were used by this XML reader.
	public override void Close()
	{
		readState = ReadState.Closed;
	}

	// Returns the value of an attribute with a specific index.
	public override String GetAttribute(int attributeIndex)
	{
		if(this.isAttributeReader || (!this.InReadState))
		{
			return null;
		}

		if(attributeIndex < 0 || attributeIndex >= this.AttributeCount)
		{
			throw new ArgumentOutOfRangeException("attributeIndex");
		}

		switch(this.currentNode.NodeType)
		{
			case XmlNodeType.Element: 
			{
				return ((XmlElement)
						(this.currentNode)).Attributes[attributeIndex].Value;
			}
			break;

			case XmlNodeType.Attribute:
			{
				XmlElement element = (XmlElement)(this.currentNode.ParentNode);
				return element.Attributes[attributeIndex].Value;
			}
			break;
			
			case XmlNodeType.DocumentType:
			{
				// TODO
			}
			break;

			case XmlNodeType.XmlDeclaration:
			{
				// TODO
			}
			break;
		}
		
		return null;
	}
	// Returns the value of an attribute with a specific name.
	public override String GetAttribute(String name, String namespaceURI)
	{
		if(this.isAttributeReader || (!this.InReadState))
		{
			return null;
		}

		switch(this.currentNode.NodeType)
		{
			case XmlNodeType.Element: 
			{
				return ((XmlElement)
						(this.currentNode)).GetAttribute(name, namespaceURI);
			}
			break;

			case XmlNodeType.Attribute:
			{
				XmlElement element = (XmlElement)(this.currentNode.ParentNode);
				return element.GetAttribute(name, namespaceURI);
			}
			break;
			
			case XmlNodeType.DocumentType:
			{
				// TODO
			}
			break;

			case XmlNodeType.XmlDeclaration:
			{
				// TODO
			}
			break;
		}
		
		return null;
	}

	// Returns the value of an attribute with a specific qualified name.
	public override String GetAttribute(String name)
	{
		if(this.isAttributeReader || (!this.InReadState))
		{
			return null;
		}

		switch(this.currentNode.NodeType)
		{
			case XmlNodeType.Element: 
			{
				return ((XmlElement)
						(this.currentNode)).GetAttribute(name);
			}
			break;

			case XmlNodeType.Attribute:
			{
				XmlElement element = (XmlElement)(this.currentNode.ParentNode);
				return element.GetAttribute(name);
			}
			break;
			
			case XmlNodeType.DocumentType:
			{
				// TODO
			}
			break;

			case XmlNodeType.XmlDeclaration:
			{
				// TODO
			}
			break;
		}
		
		return null;
	}

	// Resolve a namespace in the scope of the current element.
	public override String LookupNamespace(String prefix)
	{
		if(!this.InReadState)
		{
			return null;
		}
		if(prefix == "xmlns")
		{
			return XmlDocument.xmlns; 
		}
		
		if(this.isAttributeReader)
		{
			// prefix lookups on attributes are supposed 
			// to work for 'xmlns'. w3rd
			return null;
		}

		XmlElement element = null;
		String attrName = "xmlns" + (prefix == String.Empty ? "" : ":"+prefix);
		String ns = null;

		for(XmlNode node = this.currentNode; 
			node != null; node = node.ParentNode)
		{
			element = (node as XmlElement);
			if(element == null)
			{
				continue;
			}
			ns = element.GetAttribute(attrName);
			if(ns != null)
			{
				return ns;
			}
		}
		return null;
	}

	// Move the current position to a particular attribute.
	public override void MoveToAttribute(int attributeIndex)
	{
		if(this.isAttributeReader || (!this.InReadState))
		{
			return ;
		}

		if(attributeIndex < 0 || attributeIndex >= this.AttributeCount)
		{
			// TODO: should this be only for the cases where we
			// handle the calls ?
			throw new ArgumentOutOfRangeException("attributeIndex");
		}
		
		XmlAttribute attribute = null;

		switch(this.currentNode.NodeType)
		{
			case XmlNodeType.Element: 
			{
				attribute = 
					((XmlElement)(this.currentNode)).Attributes[attributeIndex];
				MoveToAttribute(attribute, attributeIndex);
			}
			break;

			case XmlNodeType.Attribute:
			{
				XmlElement element = (XmlElement)(this.currentNode.ParentNode);
				attribute = element.Attributes[attributeIndex];
				MoveToAttribute(attribute, attributeIndex);
			}
			break;
			
			case XmlNodeType.DocumentType:
			case XmlNodeType.XmlDeclaration:
			{
				// TODO
			}
			break;
		}
	}

	// Move the current position to an attribute with a particular name.
	public override bool MoveToAttribute(String name, String ns)
	{
		if(this.isAttributeReader || (!this.InReadState))
		{
			return false;
		}

		XmlAttribute attribute = null;
		XmlElement element = null;

		switch(this.currentNode.NodeType)
		{
			case XmlNodeType.Element: 
			{
				element = (XmlElement)(this.currentNode);
				attribute =	element.GetAttributeNode(name,ns);
				if(attribute != null)
				{
					MoveToAttribute(attribute, -1);
					return true;
				}
			}
			break;

			case XmlNodeType.Attribute:
			{
				element = (XmlElement)(this.currentNode.ParentNode);
				attribute =	element.GetAttributeNode(name,ns);
				if(attribute != null)
				{
					MoveToAttribute(attribute, -1);
					return true;
				}
			}
			break;
			
			case XmlNodeType.DocumentType:
			{
				// TODO
			}
			break;

			case XmlNodeType.XmlDeclaration:
			{
				// TODO
			}
			break;
		}

		return false;	
	}

	// Move the current position to an attribute with a qualified name.
	public override bool MoveToAttribute(String name)
	{
		if(this.isAttributeReader || (!this.InReadState))
		{
			return false;
		}

		XmlAttribute attribute = null;
		XmlElement element = null;

		switch(this.currentNode.NodeType)
		{
			case XmlNodeType.Element: 
			{
				element = (XmlElement)(this.currentNode);
				attribute =	element.GetAttributeNode(name);
				if(attribute != null)
				{
					MoveToAttribute(attribute, -1);
					return true;
				}
			}
			break;

			case XmlNodeType.Attribute:
			{
				element = (XmlElement)(this.currentNode.ParentNode);
				attribute =	element.GetAttributeNode(name);
				if(attribute != null)
				{
					MoveToAttribute(attribute, -1);
					return true;
				}
			}
			break;
			
			case XmlNodeType.DocumentType:
			{
				// TODO
			}
			break;

			case XmlNodeType.XmlDeclaration:
			{
				// TODO
			}
			break;
		}

		return false;
	}

	private void MoveToAttribute(XmlAttribute attribute, int index)
	{
		if(index == -1)
		{
			index = attribute.OwnerElement.Attributes.IndexOf(attribute);
		}
		this.attributePosn = index;
		this.currentNode = attribute;
		this.readState = ReadState.Interactive;
		Debug.Assert(this.currentNode != null);
	}

	// Move to the element that owns the current attribute.
	public override bool MoveToElement()
	{
		if(this.isAttributeReader || (!this.InReadState))
		{
			return false;
		}
		if(this.currentNode.NodeType == XmlNodeType.Attribute)
		{
			XmlAttribute attribute = (XmlAttribute)(this.currentNode);
			this.currentNode = attribute.OwnerElement;
			this.attributePosn = 0;
			this.readState = ReadState.Interactive;
			Debug.Assert(this.currentNode != null);
			return true;
		}
		else if(this.currentNode.NodeType == XmlNodeType.DocumentType ||
				this.currentNode.NodeType == XmlNodeType.XmlDeclaration)
		{
			// TODO: DocumentType and XmlDeclaration
		}

		return false;
	}

	// Move to the first attribute owned by the current element.
	public override bool MoveToFirstAttribute()
	{
		if(this.InReadState && this.AttributeCount > 0)
		{
			this.MoveToAttribute(0);
			return true;
		}
		return false;
	}

	// Move to the next attribute owned by the current element.
	public override bool MoveToNextAttribute()
	{
		if(!this.InReadState)
		{
			return false;
		}
		int nextAttr = 0;
		if(this.currentNode.NodeType == XmlNodeType.Element ||
				this.currentNode.NodeType == XmlNodeType.Attribute)
		{
			nextAttr = this.attributePosn + 1; 
			if(nextAttr < this.AttributeCount)
			{
				this.MoveToAttribute(nextAttr);
				return true;
			}
		}
		else if(this.currentNode.NodeType == XmlNodeType.DocumentType ||
				this.currentNode.NodeType == XmlNodeType.XmlDeclaration)
		{
			// TODO:
		}
		return false;
	}

	// Read the next node in the input stream.
	public override bool Read()
	{
		if(EOF)
		{
			return false;
		}

		if(ReadState == ReadState.Initial)
		{
			currentNode = startNode;
			readState = ReadState.Interactive;
			// skip a Document or DocumentFragment node
			// sometimes I hate this API 
			if(skipThisNode)
			{
				currentNode = startNode.FirstChild;
			}
			else 
			{
				skipThisNode = true;
			}
			if(currentNode == null) 
			{
				this.readState = ReadState.Error;
				return false;
			}
			return true;
		}

		MoveToElement();

		if(!skipThisNode)
		{
			skipThisNode = true;
			return (currentNode != null);
		}

		bool eof = false;

		if (IsEmptyElement || this.isEndElement)
		{
			this.isEndElement = false;
			
			/* now that skipThisNode makes sense */
			if(currentNode.ParentNode == null 
				|| currentNode == startNode)
			{
				eof = true;
			}
			else if(currentNode.NextSibling == null)
			{
				this.depth--;
				currentNode = currentNode.ParentNode;
				this.isEndElement = true;
				return true;
			}
			else
			{
				currentNode = currentNode.NextSibling;
				return true;
			}
		}

		if(currentNode.NextSibling == null &&
					this.startNode.NodeType == XmlNodeType.EntityReference)
		{
			// One level only :)
			eof = true;
		}
		
		if(eof)
		{
			this.currentNode = null;
			this.readState = ReadState.EndOfFile;
			return false;
		}

		if(this.currentNode.FirstChild != null && !this.isEndElement)
		{
			this.depth++;
			this.isEndElement = false;
			currentNode = currentNode.FirstChild;
		}
		else if(this.currentNode.NodeType == XmlNodeType.Element) 
		{
			this.isEndElement = true;
			if(this.currentNode.FirstChild != null)
			{
				// empty elements do not increment depth
				this.depth--;
			}
		}
		else
		{
			if(!this.skipThisNode)
			{
				this.skipThisNode = true;
				return (this.currentNode != null);
			}
			if(this.currentNode.NextSibling != null)
			{
				this.isEndElement = false;
				this.currentNode = this.currentNode.NextSibling;
			}
			else
			{
				this.currentNode = this.currentNode.ParentNode;
				if(this.currentNode.NodeType == XmlNodeType.Element)
				{
					this.isEndElement = true;
				}
				this.depth--; // this node *has* children for sure :)
			}
		}

		if(this.currentNode == null)
		{
			this.readState = ReadState.EndOfFile;
			return false;
		}
		
		return true;
	}

	// Read the next attribute value in the input stream.
	public override bool ReadAttributeValue()
	{
		if(!this.InReadState)
		{
			return false;
		}

		this.readState = ReadState.Interactive;
		
		if(this.currentNode.NodeType == XmlNodeType.Attribute)
		{
			if(this.currentNode.FirstChild != null)
			{
				this.currentNode = this.currentNode.FirstChild;
				this.inAttrValue = true;
				return true;
			}
		}
		else if(this.inAttrValue)
		{
			Debug.Assert(this.currentNode.ParentNode.NodeType ==
							XmlNodeType.Attribute);
			if(this.currentNode.NextSibling != null)
			{
				this.currentNode = this.currentNode.NextSibling;
				return true;
			}
		}
		return false;
	}

	// Read the contents of the current node, including all markup.
	public override String ReadInnerXml()
	{
		// TODO: skip
		return currentNode.InnerXml;
	}

	// Read the current node, including all markup.
	public override String ReadOuterXml()
	{
		// TODO: skip
		return currentNode.OuterXml;
	}

	// Read the contents of an element or text node as a string.
	public override String ReadString()
	{
		// what about entities ?	
		return base.ReadString();
	}

	// Resolve an entity reference.
	public override void ResolveEntity()
	{
		// TODO: no idea what to do here :(
	}

	// Skip the current element in the input.
	public override void Skip()
	{
		this.readState = ReadState.Interactive;
		// what else ?
		base.Skip();
	}

	// Get the number of attributes on the current node.
	public override int AttributeCount
	{
		get
		{
			if(this.isAttributeReader)
			{
				return 0; // or -1 ?
			}

			switch(this.currentNode.NodeType)
			{
				case XmlNodeType.Element:
				{
					if(this.isEndElement)
					{
						return 0;
					}
					return ((XmlElement)this.currentNode).Attributes.Count;
				}
				break;

				case XmlNodeType.Attribute:
				{
					XmlElement element = (XmlElement)
											(this.currentNode.ParentNode);
					return element.Attributes.Count;
				}
				break;

				case XmlNodeType.XmlDeclaration:
				{
					// TODO: 
				}
				break;

				case XmlNodeType.DocumentType:
				{
					// TODO:
				}
				break;
			}
			return 0;
		}
	}

	// Get the base URI for the current node.
	public override String BaseURI
	{
		get
		{
			return currentNode.BaseURI;
		}
	}

	// Determine if this reader can parse and resolve entities.
	public override bool CanResolveEntity
	{
		get
		{
			return true;
		}
	}

	// Get the depth of the current node.
	public override int Depth
	{
		get
		{
			if(this.currentNode == null) 
			{
				return 0;
			}
			if(this.currentNode.NodeType == XmlNodeType.Attribute)
			{
				return this.depth + (this.inAttrValue ? 2 : 1);
			}
			return this.depth;
		}
	}

	// Determine if we have reached the end of the input stream.
	public override bool EOF
	{
		get
		{
			return (this.readState == ReadState.EndOfFile ||
						this.readState == ReadState.Error);
		}
	}

	// Determine if the current node can have an associated text value.
	public override bool HasValue
	{
		get
		{
			// confusing part is the 'can' part
			if(this.currentNode.Value == null &&
				this.currentNode.NodeType != XmlNodeType.DocumentType)
			{
				return false;
			}
			return true;
		}
	}

	// Determine if the current node's value was generated from a DTD default.
	public override bool IsDefault
	{
		get
		{
			if(this.currentNode.NodeType == XmlNodeType.Attribute)
			{
				// this is funny because Specified is defined as 
				// !isDefault in XmlAttribute.cs :)
				return !((XmlAttribute) this.currentNode).Specified;
			}
			return false;
		}
	}

	// Determine if the current node is an empty element.
	public override bool IsEmptyElement
	{
		get
		{
			if(this.currentNode.NodeType == XmlNodeType.Element)
			{
				// is element. now check if is empty
				return ((XmlElement) this.currentNode).IsEmpty;
			}
			return false;
		}
	}

	// Retrieve an attribute value with a specified index.
	public override String this[int i]
	{
		get
		{
			return GetAttribute(i);
		}
	}

	// Retrieve an attribute value with a specified name.
	public override String this[String localname, String namespaceURI]
	{
		get
		{
			return GetAttribute(localname, namespaceURI);
		}
	}

	// Retrieve an attribute value with a specified qualified name.
	public override String this[String name]
	{
		get
		{
			return GetAttribute(name);
		}
	}

	// Get the local name of the current node.
	public override String LocalName
	{
		get
		{
			return currentNode.LocalName;
		}
	}

	// Get the fully-qualified name of the current node.
	public override String Name
	{
		get
		{
			return currentNode.Name;
		}
	}

	// Get the name that that is used to look up and resolve names.
	public override XmlNameTable NameTable
	{
		get
		{
			return doc.NameTable;
		}
	}

	// Get the namespace URI associated with the current node.
	public override String NamespaceURI
	{
		get
		{
			return currentNode.NamespaceURI;
		}
	}

	// Get the type of the current node.
	public override XmlNodeType NodeType
	{
		get
		{
			if(!this.InReadState)
			{
				return XmlNodeType.None;
			}
			if(this.inAttrValue)
			{
				Debug.Assert(this.currentNode.NodeType 
								== XmlNodeType.Attribute);
				return XmlNodeType.Text;
			}
			if(this.isEndElement)
			{
				// DOM doesn't really mark endElements
				// we have to fake it.
				return XmlNodeType.EndElement;
			}
			return currentNode.NodeType;
		}
	}

	// Get the namespace prefix associated with the current node.
	public override String Prefix
	{
		get
		{
			return currentNode.Prefix;
		}
	}

	// Get the quote character that was used to enclose an attribute value.
	public override char QuoteChar
	{
		get
		{
			return '"';
		}
	}

	public override ReadState ReadState
	{
		get
		{
			return readState;
		}
	}

	// Get the text value of the current node.
	public override String Value
	{
		get
		{
			return currentNode.Value;
		}
	}

	// Get the current xml:lang scope.
	public override String XmlLang
	{
		get
		{
			XmlElement element = null;
			for(XmlNode node = this.currentNode; 
					node != null; node = node.ParentNode)
			{
				element = (node as XmlElement);
				if(element != null && element.HasAttribute("xml:lang")) 
				{
					return element.GetAttribute("xml:lang");
				}
			}
			return String.Empty;
		}
	}

	// Get the current xml:space scope.
	public override XmlSpace XmlSpace
	{
		get
		{
			XmlElement element = null;
			for(XmlNode node = this.currentNode; 
					node != null; node = node.ParentNode)
			{
				element = node as XmlElement;
				if(element != null && element.HasAttribute("xml:space"))
				{
					String xmlspace = element.GetAttribute("xml:space");
					if(String.Compare(xmlspace, "preserve", true) == 0)
					{
						return XmlSpace.Preserve;
					}
					else if(String.Compare(xmlspace, "default", true) == 0)
					{
						return XmlSpace.Default;
					}
				}
			}

			return XmlSpace.None;
		}
	}
		
	// helper functions
	// kinda obvious, isn't it ?
	private bool InReadState
	{
		get
		{
			return (this.readState == ReadState.Interactive);
		}
	}

}; // class XmlNodeReader

#endif // !ECMA_COMPAT

}; // namespace System.Xml
