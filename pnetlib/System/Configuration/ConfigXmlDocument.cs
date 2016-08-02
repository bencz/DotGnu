/*
 * ConfigXmlDocument.cs - Implementation of the
 *		"System.Configuration.ConfigXmlDocument" interface.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System.Configuration
{

#if !ECMA_COMPAT && SECOND_PASS

using System;
using System.IO;
using System.Xml;

// This class extends XmlDocument to keep track of line numbers.

public sealed class ConfigXmlDocument : XmlDocument, IConfigXmlNode
{
	// Internal state.
	private XmlTextReader reader;
	private String filename;
	private int line;

	// Constructor.
	public ConfigXmlDocument() : base() {}

	// Implement the IConfigXmlNode interface.
	String IConfigXmlNode.Filename
			{
				get
				{
					return filename;
				}
			}
	int IConfigXmlNode.LineNumber
			{
				get
				{
					if(reader != null)
					{
						if(line > 0)
						{
							return reader.LineNumber + line - 1;
						}
						else
						{
							return reader.LineNumber;
						}
					}
					else
					{
						return 0;
					}
				}
			}

	// Properties.
	public String Filename
			{
				get
				{
					return ((IConfigXmlNode)this).Filename;
				}
			}
	public int LineNumber
			{
				get
				{
					return ((IConfigXmlNode)this).LineNumber;
				}
			}

	// Load XML into this document.
	public override void Load(String filename)
			{
				this.filename = filename;
				try
				{
					reader = new XmlTextReader(filename);
					base.Load(reader);
				}
				finally
				{
					if(reader != null)
					{
						reader.Close();
						reader = null;
					}
				}
			}

	// Load a single element from an existing reader.
	public void LoadSingleElement(String filename, XmlTextReader sourceReader)
			{
				this.filename = filename;
				line = sourceReader.LineNumber;
				String xml = sourceReader.ReadOuterXml();
				try
				{
					reader = new XmlTextReader(new StringReader(xml),
											   sourceReader.NameTable);
					base.Load(reader);
				}
				catch
				{
					if(reader != null)
					{
						reader.Close();
						reader = null;
					}
				}
			}

	// Redirect the node creation routines to add line number information.
	public override XmlAttribute CreateAttribute
				(String prefix, String localName, String namespaceUri)
			{
				return new ConfigXmlAttribute
					(prefix, localName, namespaceUri, this,
					 Filename, LineNumber);
			}
	public override XmlElement CreateElement
				(String prefix, String localName, String namespaceUri)
			{
				return new ConfigXmlElement
					(prefix, localName, namespaceUri, this,
					 Filename, LineNumber);
			}
	public override XmlText CreateTextNode(String text)
			{
				return new ConfigXmlText(text, this, Filename, LineNumber);
			}
	public override XmlCDataSection CreateCDataSection(String data)
			{
				return new ConfigXmlCDataSection
					(data, this, Filename, LineNumber);
			}
	public override XmlComment CreateComment(String data)
			{
				return new ConfigXmlComment
					(data, this, Filename, LineNumber);
			}
	public override XmlSignificantWhitespace CreateSignificantWhitespace
				(String data)
			{
				return new ConfigXmlSignificantWhitespace
					(data, this, Filename, LineNumber);
			}
	public override XmlWhitespace CreateWhitespace(String data)
			{
				return new ConfigXmlWhitespace
					(data, this, Filename, LineNumber);
			}

	// Attribute that is augmented with line number information.
	private sealed class ConfigXmlAttribute : XmlAttribute, IConfigXmlNode
	{
		// Internal state.
		private String filename;
		private int line;

		// Constructor.
		public ConfigXmlAttribute(String prefix, String localName,
								  String namespaceUri, XmlDocument doc,
								  String filename, int line)
				: base(prefix, localName, namespaceUri, doc)
				{
					this.filename = filename;
					this.line = line;
				}

		// Implement IConfigXmlNode.
		public String Filename
				{
					get
					{
						return filename;
					}
				}
		public int LineNumber
				{
					get
					{
						return line;
					}
				}

		// Clone this node.
		public override XmlNode CloneNode(bool deep)
				{
					XmlNode node = base.CloneNode(deep);
					ConfigXmlAttribute cnode = (node as ConfigXmlAttribute);
					if(cnode != null)
					{
						cnode.filename = filename;
						cnode.line = line;
					}
					return node;
				}

	}; // class ConfigXmlAttribute

	// Element that is augmented with line number information.
	private sealed class ConfigXmlElement : XmlElement, IConfigXmlNode
	{
		// Internal state.
		private String filename;
		private int line;

		// Constructor.
		public ConfigXmlElement(String prefix, String localName,
								String namespaceUri, XmlDocument doc,
								String filename, int line)
				: base(prefix, localName, namespaceUri, doc)
				{
					this.filename = filename;
					this.line = line;
				}

		// Implement IConfigXmlNode.
		public String Filename
				{
					get
					{
						return filename;
					}
				}
		public int LineNumber
				{
					get
					{
						return line;
					}
				}

		// Clone this node.
		public override XmlNode CloneNode(bool deep)
				{
					XmlNode node = base.CloneNode(deep);
					ConfigXmlElement cnode = (node as ConfigXmlElement);
					if(cnode != null)
					{
						cnode.filename = filename;
						cnode.line = line;
					}
					return node;
				}

	}; // class ConfigXmlElement

	// Text node that is augmented with line number information.
	private sealed class ConfigXmlText : XmlText, IConfigXmlNode
	{
		// Internal state.
		private String filename;
		private int line;

		// Constructor.
		public ConfigXmlText(String text, XmlDocument doc,
							 String filename, int line)
				: base(text, doc)
				{
					this.filename = filename;
					this.line = line;
				}

		// Implement IConfigXmlNode.
		public String Filename
				{
					get
					{
						return filename;
					}
				}
		public int LineNumber
				{
					get
					{
						return line;
					}
				}

		// Clone this node.
		public override XmlNode CloneNode(bool deep)
				{
					XmlNode node = base.CloneNode(deep);
					ConfigXmlText cnode = (node as ConfigXmlText);
					if(cnode != null)
					{
						cnode.filename = filename;
						cnode.line = line;
					}
					return node;
				}

	}; // class ConfigXmlText

	// CDATA that is augmented with line number information.
	private sealed class ConfigXmlCDataSection : XmlCDataSection, IConfigXmlNode
	{
		// Internal state.
		private String filename;
		private int line;

		// Constructor.
		public ConfigXmlCDataSection(String data, XmlDocument doc,
							 		 String filename, int line)
				: base(data, doc)
				{
					this.filename = filename;
					this.line = line;
				}

		// Implement IConfigXmlNode.
		public String Filename
				{
					get
					{
						return filename;
					}
				}
		public int LineNumber
				{
					get
					{
						return line;
					}
				}

		// Clone this node.
		public override XmlNode CloneNode(bool deep)
				{
					XmlNode node = base.CloneNode(deep);
					ConfigXmlCDataSection cnode =
							(node as ConfigXmlCDataSection);
					if(cnode != null)
					{
						cnode.filename = filename;
						cnode.line = line;
					}
					return node;
				}

	}; // class ConfigXmlCDataSection

	// Comment that is augmented with line number information.
	private sealed class ConfigXmlComment : XmlComment, IConfigXmlNode
	{
		// Internal state.
		private String filename;
		private int line;

		// Constructor.
		public ConfigXmlComment(String data, XmlDocument doc,
						 		String filename, int line)
				: base(data, doc)
				{
					this.filename = filename;
					this.line = line;
				}

		// Implement IConfigXmlNode.
		public String Filename
				{
					get
					{
						return filename;
					}
				}
		public int LineNumber
				{
					get
					{
						return line;
					}
				}

		// Clone this node.
		public override XmlNode CloneNode(bool deep)
				{
					XmlNode node = base.CloneNode(deep);
					ConfigXmlComment cnode = (node as ConfigXmlComment);
					if(cnode != null)
					{
						cnode.filename = filename;
						cnode.line = line;
					}
					return node;
				}

	}; // class ConfigXmlComment

	// Significant whitespace that is augmented with line number information.
	private sealed class ConfigXmlSignificantWhitespace
			: XmlSignificantWhitespace, IConfigXmlNode
	{
		// Internal state.
		private String filename;
		private int line;

		// Constructor.
		public ConfigXmlSignificantWhitespace(String data, XmlDocument doc,
						 					  String filename, int line)
				: base(data, doc)
				{
					this.filename = filename;
					this.line = line;
				}

		// Implement IConfigXmlNode.
		public String Filename
				{
					get
					{
						return filename;
					}
				}
		public int LineNumber
				{
					get
					{
						return line;
					}
				}

		// Clone this node.
		public override XmlNode CloneNode(bool deep)
				{
					XmlNode node = base.CloneNode(deep);
					ConfigXmlSignificantWhitespace cnode =
							(node as ConfigXmlSignificantWhitespace);
					if(cnode != null)
					{
						cnode.filename = filename;
						cnode.line = line;
					}
					return node;
				}

	}; // class ConfigXmlSignificantWhitespace

	// Whitespace that is augmented with line number information.
	private sealed class ConfigXmlWhitespace : XmlWhitespace, IConfigXmlNode
	{
		// Internal state.
		private String filename;
		private int line;

		// Constructor.
		public ConfigXmlWhitespace(String data, XmlDocument doc,
			 					   String filename, int line)
				: base(data, doc)
				{
					this.filename = filename;
					this.line = line;
				}

		// Implement IConfigXmlNode.
		public String Filename
				{
					get
					{
						return filename;
					}
				}
		public int LineNumber
				{
					get
					{
						return line;
					}
				}

		// Clone this node.
		public override XmlNode CloneNode(bool deep)
				{
					XmlNode node = base.CloneNode(deep);
					ConfigXmlWhitespace cnode = (node as ConfigXmlWhitespace);
					if(cnode != null)
					{
						cnode.filename = filename;
						cnode.line = line;
					}
					return node;
				}

	}; // class ConfigXmlWhitespace

}; // class ConfigXmlDocument

#endif // !ECMA_COMPAT && SECOND_PASS

}; // namespace System.Configuration
