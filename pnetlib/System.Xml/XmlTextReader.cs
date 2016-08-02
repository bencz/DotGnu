/*
 * XmlTextReader.cs - Implementation of the "System.Xml.XmlTextReader" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2003, 2004  Free Software Foundation, Inc.
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

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections;
using System.Xml.Private;

public class XmlTextReader : XmlReader
#if !ECMA_COMPAT
	, IXmlLineInfo
#endif
{
	// Internal state.
	private bool contextSupport;
	private bool hasRoot;
	private bool incDepth;
	private bool namespaces;
	private bool normalize;
	private bool xmlPopScope;
	private bool xmlnsPopScope;
	private int depth;
	private int sawPreserve;
	private NodeManager nodes;
	private ReadState readState;
	private Stack elementNames;
	private State state;
	private WhitespaceHandling whitespace;
	private XmlDTDReader dtdReader;
	private XmlParserContext context;
	private XmlParserInput input;
	private XmlResolver resolver;

	private readonly Object xmlBaseName;
	private readonly Object xmlLangName;
	private readonly Object xmlSpaceName;
	private readonly Object xmlNSPrefix;
	private readonly Object xmlCompareQuick;


	// Possible Document States.
	private enum State
	{
		XmlDeclaration     = 0,
		DoctypeDeclaration = 1,
		Element            = 2,
		Content            = 3,
		Misc               = 4,
		Attribute          = 5,

	}; // enum State


	// Constructors.
	protected XmlTextReader()
			: this(new NameTable())
			{
				// Nothing to do here
			}
	protected XmlTextReader(XmlNameTable nt)
			{
				if(nt == null)
				{
					throw new ArgumentNullException("nt");
				}

				namespaces = true;
				normalize = false;
				depth = 0;
				readState = ReadState.Initial;
				whitespace = WhitespaceHandling.All;

				xmlBaseName = nt.Add("xml:base");
				xmlLangName = nt.Add("xml:lang");
				xmlSpaceName = nt.Add("xml:space");
				xmlNSPrefix = nt.Add("xmlns");
				xmlCompareQuick = nt.Add("xml");

				contextSupport = false;
				hasRoot = true;
				incDepth = false;
				xmlPopScope = false;
				xmlnsPopScope = false;
				sawPreserve = -1;
				state = State.XmlDeclaration;
				elementNames = new Stack();
				nodes = new NodeManager(nt, new ErrorHandler(Error));
				input = new XmlParserInput
					(null, nt, new EOFHandler(HandleEOF), new ErrorHandler(Error));
				context = new XmlParserContext
					(nt, new XmlNamespaceManager(nt), String.Empty, XmlSpace.None);
				resolver = new XmlUrlResolver();
				dtdReader = new XmlDTDReader(context);
			}
	public XmlTextReader(Stream input)
			: this(String.Empty, input, new NameTable())
			{
				// Nothing to do here
			}
	public XmlTextReader(String url, Stream input)
			: this(url, input, new NameTable())
			{
				// Nothing to do here
			}
	public XmlTextReader(Stream input, XmlNameTable nt)
			: this(String.Empty, input, nt)
			{
				// Nothing to do here
			}
	public XmlTextReader(String url, Stream input, XmlNameTable nt)
			: this(nt)
			{
				if(input == null)
				{
					throw new ArgumentNullException("input");
				}
				if(url == null)
				{
					context.BaseURI = String.Empty;
				}
				else
				{
					context.BaseURI = nt.Add(url);
				}
				XmlStreamReader sr = new XmlStreamReader(input, true);
				this.input.Reader = sr;
				context.Encoding = sr.CurrentEncoding;
			}
	public XmlTextReader(TextReader input)
			: this(String.Empty, input, new NameTable())
			{
				// Nothing to do here
			}
	public XmlTextReader(String url, TextReader input)
			: this(url, input, new NameTable())
			{
				// Nothing to do here
			}
	public XmlTextReader(TextReader input, XmlNameTable nt)
			: this(String.Empty, input, nt)
			{
				// Nothing to do here
			}
	public XmlTextReader(String url, TextReader input, XmlNameTable nt)
			: this(nt)
			{
				if(url == null)
				{
					context.BaseURI = String.Empty;
				}
				else
				{
					context.BaseURI = nt.Add(url);
				}
				this.input.Reader = input;
				if(input as StreamReader != null)
				{
					context.Encoding = ((StreamReader)input).CurrentEncoding;
				}
			}
	public XmlTextReader(Stream xmlFragment, XmlNodeType fragType,
						 XmlParserContext context)
			: this((context != null ? context.NameTable : new NameTable()))
			{
				if(xmlFragment == null)
				{
					throw new ArgumentNullException("xmlFragment");
				}
				if(fragType == XmlNodeType.Attribute)
				{
					state = State.Attribute;
					hasRoot = false;
				}
				else if(fragType == XmlNodeType.Element)
				{
					state = State.Content;
					hasRoot = false;
				}
				else if(fragType != XmlNodeType.Document)
				{
					throw new XmlException(S._("Xml_InvalidNodeType"));
				}

				if(context == null)
				{
					XmlStreamReader sr = new XmlStreamReader(xmlFragment, true);
					input.Reader = sr;
					this.context.Encoding = sr.CurrentEncoding;
					contextSupport = false;
				}
				else
				{
					this.context.BaseURI = context.BaseURI;
					this.context.DocTypeName = context.DocTypeName;
					this.context.InternalSubset = context.InternalSubset;
					this.context.NamespaceManager = context.NamespaceManager;
					this.context.PublicId = context.PublicId;
					this.context.SystemId = context.SystemId;
					this.context.XmlLang = context.XmlLang;
					this.context.XmlSpace = context.XmlSpace;
					dtdReader.Context = this.context;
					contextSupport = true;
					XmlStreamReader sr = new XmlStreamReader(xmlFragment, true);
					if(context.Encoding == null)
					{
						this.context.Encoding = sr.CurrentEncoding;
					}
					else
					{
						this.context.Encoding = context.Encoding;
					}
					input.Reader = sr;
				}
				namespaces = (fragType == XmlNodeType.Document);
			}
	public XmlTextReader(String xmlFragment, XmlNodeType fragType,
						 XmlParserContext context)
			: this((context != null ? context.NameTable : new NameTable()))
			{
				if(xmlFragment == null)
				{
					throw new ArgumentNullException("xmlFragment");
				}
				if(fragType == XmlNodeType.Attribute)
				{
					state = State.Attribute;
					hasRoot = false;
				}
				else if(fragType == XmlNodeType.Element)
				{
					state = State.Content;
					hasRoot = false;
				}
				else if(fragType != XmlNodeType.Document)
				{
					throw new XmlException(S._("Xml_InvalidNodeType"));
				}

				if(context == null)
				{
					this.context.BaseURI = String.Empty;
					XmlStreamReader sr = new XmlStreamReader(new StringReader(xmlFragment));
					input.Reader = sr.TxtReader;
					this.context.Encoding = sr.CurrentEncoding;
					contextSupport = false;
				}
				else
				{
					this.context.BaseURI = context.BaseURI;
					this.context.XmlLang = context.XmlLang;
					this.context.XmlSpace = context.XmlSpace;
					dtdReader.Context = this.context;
					contextSupport = true;
					XmlStreamReader sr = new XmlStreamReader(new StringReader(xmlFragment));
					if(context.Encoding == null)
					{
						this.context.Encoding = sr.CurrentEncoding;
					}
					else
					{
						this.context.Encoding = context.Encoding;
					}
					input.Reader = sr.TxtReader;
				}
				namespaces = (fragType == XmlNodeType.Document);
			}
	public XmlTextReader(String url)
			: this(url, new NameTable())
			{
				// Nothing to do here
			}
			
	public XmlTextReader(String url, XmlNameTable nt)
			: this(nt)
			{
				if(url == null) { throw new XmlException("url"); }
				
				Uri uri = resolver.ResolveUri(null, url);
				Stream stream = (Stream)resolver.GetEntity(uri, null, typeof(Stream));
				XmlStreamReader sr = new XmlStreamReader(stream, true);
				input.Reader = sr;
				context.BaseURI = nt.Add(url);
				context.Encoding = sr.CurrentEncoding;
				context.NameTable = nt;
			}

	// Get the number of attributes on the current node.
	public override int AttributeCount
			{
				get { return nodes.Current.AttributeCount; }
			}

	// Get the base URI for the current node.
	public override String BaseURI
			{
				get { return context.BaseURI; }
			}

	// Get the depth of the current node.
	public override int Depth
			{
				get { return depth + nodes.Current.DepthOffset; }
			}

	// Determine if we have reached the end of the input stream.
	public override bool EOF
			{
				get { return (readState == ReadState.EndOfFile); }
			}

	// Get the encoding that is in use by the XML stream.
	public Encoding Encoding
			{
				get
				{
					if(readState == ReadState.Interactive)
					{
						return context.Encoding;
					}
					else
					{
						return null;
					}
				}
			}

	// Determine if the current node can have an associated text value.
	public override bool HasValue
			{
				get
				{
					switch(NodeType)
					{
						case XmlNodeType.Attribute:
						case XmlNodeType.CDATA:
						case XmlNodeType.Comment:
						case XmlNodeType.DocumentType:
						case XmlNodeType.ProcessingInstruction:
						case XmlNodeType.SignificantWhitespace:
						case XmlNodeType.Text:
						case XmlNodeType.Whitespace:
						case XmlNodeType.XmlDeclaration:
							return true;
						default:
							return false;
					}
				}
			}

	// Determine if the current node's value was generated from a DTD default.
	public override bool IsDefault
			{
				get { return false; }
			}

	// Determine if the current node is an empty element.
	public override bool IsEmptyElement
			{
				get { return nodes.Current.IsEmptyElement; }
			}

	// Retrieve an attribute value with a specified index.
	public override String this[int i]
			{
				get { return GetAttribute(i); }
			}

	// Retrieve an attribute value with a specified name.
	public override String this[String localname, String namespaceURI]
			{
				get { return GetAttribute(localname, namespaceURI); }
			}

	// Retrieve an attribute value with a specified qualified name.
	public override String this[String name]
			{
				get { return GetAttribute(name); }
			}

#if !ECMA_COMPAT
	// Determine if we have line information.
	bool IXmlLineInfo.HasLineInfo()
			{
				return true;
			}
#endif

	// Get the current line number.
	public int LineNumber
			{
				get { return input.LineNumber; }
			}

	// Get the current line position.
	public int LinePosition
			{
				get { return input.LinePosition; }
			}

	// Get the local name of the current node.
	public override String LocalName
			{
				get { return nodes.Current.LocalName; }
			}

	// Get the fully-qualified name of the current node.
	public override String Name
			{
				get { return nodes.Current.Name; }
			}

	// Get the name table that is used to look up and resolve names.
	public override XmlNameTable NameTable
			{
				get { return context.NameTable; }
			}

	// Get the namespace URI associated with the current node.
	public override String NamespaceURI
			{
				get { return nodes.Current.NamespaceURI; }
			}

	// Get or set the "namespace support" flag for this reader.
	public bool Namespaces
			{
				get { return namespaces; }
				set
				{
					if(readState != ReadState.Initial)
					{
						throw new InvalidOperationException
							(S._("Xml_InvalidReadState"));
					}
					namespaces = value;
				}
			}

	// Get or set the "normalize" flag for this reader.
	public bool Normalization
			{
				get { return normalize; }
				set
				{
					if(readState == ReadState.Closed)
					{
						throw new InvalidOperationException
							(S._("Xml_InvalidReadState"));
					}
					normalize = value;
				}
			}

	// Get the type of the current node.
	public override XmlNodeType NodeType
			{
				get { return nodes.Current.NodeType; }
			}

	// Get the namespace prefix associated with the current node.
	public override String Prefix
			{
				get { return nodes.Current.Prefix; }
			}

	// Get the quote character that was used to enclose an attribute value.
	public override char QuoteChar
			{
				get { return nodes.Current.QuoteChar; }
			}

	// Get the current read state of the reader.
	public override ReadState ReadState
			{
				get { return readState; }
			}

	// Get the text value of the current node.
	public override String Value
			{
				get { return nodes.Current.Value; }
			}

	// Get or set the whitespace handling flag.
	public WhitespaceHandling WhitespaceHandling
			{
				get { return whitespace; }
				set
				{
					if(!Enum.IsDefined(typeof(WhitespaceHandling), value))
					{
						throw new ArgumentOutOfRangeException
							("value", S._("Xml_InvalidWhitespaceHandling"));
					}
					if(readState == ReadState.Closed)
					{
						throw new InvalidOperationException
							(S._("Xml_InvalidReadState"));
					}
					whitespace = value;
				}
			}

	// Get the current xml:lang scope.
	public override String XmlLang
			{
				get { return context.XmlLang; }
			}

	// Set the resolver to use to resolve DTD references.
	public XmlResolver XmlResolver
			{
				set { resolver = value; }
			}

	// Get the current xml:space scope.
	public override XmlSpace XmlSpace
			{
				get { return context.XmlSpace; }
			}

	internal XmlParserContext ParserContext 
			{
				get { return context; }
			}

	// Clean up the resources that were used by this XML reader.
	[TODO] // ********************************************************* TODO
	public override void Close()
			{
				context.SystemId = String.Empty;
				context.PublicId = String.Empty;
				depth = 0;
				contextSupport = false;
				input.Close();
			}

	// Returns the value of an attribute with a specific index.
	public override String GetAttribute(int i)
			{
				NodeInfo tmp = nodes.Current.GetAttribute(i);
				if(tmp == null)
				{
					throw new ArgumentOutOfRangeException
						("i", S._("Xml_InvalidAttributeIndex"));
				}
				return tmp.Value;
			}

	// Returns the value of an attribute with a specific name.
	public override String GetAttribute(String localName, String namespaceURI)
			{
				NodeInfo tmp = nodes.Current.GetAttribute(localName, namespaceURI);
				if(tmp == null) { return null; }
				return tmp.Value;
			}

	// Returns the value of an attribute with a specific qualified name.
	public override String GetAttribute(String name)
			{
				NodeInfo tmp = nodes.Current.GetAttribute(name);
				if(tmp == null) { return null; }
				return tmp.Value;
			}

	// Get the remainder of the current XML stream.
	public TextReader GetRemainder()
			{
				// clear the logger's log stack
				input.Logger.Clear();

				// create the log and push it onto the logger's log stack
				StringBuilder log = new StringBuilder();
				input.Logger.Push(log);

				// read until we consume all of the input
				while(input.NextChar()) {}

				// pop the log from the logger's log stack
				input.Logger.Pop();

				// close this reader
				Close();

				// return a new text reader using the logged data
				return new StringReader(log.ToString());
			}

	// Resolve a namespace in the scope of the current element.
	public override String LookupNamespace(String prefix)
			{
				if(!namespaces) { return String.Empty; }
				if(prefix == null) { throw new ArgumentNullException("prefix"); }
				return context.NamespaceManager.LookupNamespace(prefix);
			}

	// Move the current position to a particular attribute.
	public override void MoveToAttribute(int i)
			{
				if(!nodes.Current.MoveToAttribute(i))
				{
					throw new ArgumentOutOfRangeException
						("i", S._("Xml_InvalidAttributeIndex"));
				}
			}

	// Move the current position to an attribute with a particular name.
	public override bool MoveToAttribute(String localName, String namespaceURI)
			{
				return nodes.Current.MoveToAttribute(localName, namespaceURI);
			}

	// Move the current position to an attribute with a qualified name.
	public override bool MoveToAttribute(String name)
			{
				return nodes.Current.MoveToAttribute(name);
			}

	// Move to the element that owns the current attribute.
	public override bool MoveToElement()
			{
				return nodes.Current.MoveToElement();
			}

	// Move to the first attribute owned by the current element.
	public override bool MoveToFirstAttribute()
			{
				return nodes.Current.MoveToFirstAttribute();
			}

	// Move to the next attribute owned by the current element.
	public override bool MoveToNextAttribute()
			{
				return nodes.Current.MoveToNextAttribute();
			}

	// Read the next node in the input stream.
	public override bool Read()
			{
				// Validate the current state of the stream.
				switch(readState)
				{
					case ReadState.EndOfFile:
					{
						return false;
					}
					// Not reached.

					case ReadState.Closed:
					{
						throw new XmlException(S._("Xml_ReaderClosed"));
					}
					// Not reached.

					case ReadState.Error:
					{
						throw new XmlException(S._("Xml_ReaderError"));
					}
					// Not reached.

					case ReadState.Initial:
					{
						readState = ReadState.Interactive;
					}
					break;
				}
				return ReadDocument();
			}

	// Read the next attribute value in the input stream.
	public override bool ReadAttributeValue()
			{
				return nodes.Current.ReadAttributeValue();
			}

	// Read base64 data from the current element.
	[TODO]
	public int ReadBase64(byte[] array, int offset, int len)
			{
				// TODO
				return 0;
			}

	// Read binhex data from the current element.
	[TODO]
	public int ReadBinHex(byte[] array, int offset, int len)
			{
				// TODO
				return 0;
			}

	// Read character data from the current element.
	[TODO] // ********************************************************* TODO
	public int ReadChars(char[] buffer, int index, int count)
			{
				if(count > buffer.Length - index)
				{
					throw new ArgumentException
						(S._("ArgumentExceedsLength"), "count");
				}
				if(buffer == null)
				{
					throw new ArgumentNullException
						(S._("ArgumentIsNull"), "buffer");
				}
				if(index < 0 || count < 0)
				{
					throw new ArgumentOutOfRangeException
						(S._("ArgumentOutOfRange"));
				}

				if(NodeType != XmlNodeType.Element)
				{
					return 0;
				}

				/*
				if(nodeType != XmlNodeType.Text)
				{
					// if we are not at a text node, move to current element's content
					MoveToContent();
				}


				Read();

				int length = value.IndexOf("<");

				// go back to '<' position
				linePosition -= value.Length - length;

				String output = value.Substring(0, length);

				// read text now
				if(output.Length < count)
				{
					Array.Copy(output.ToCharArray(0, output.Length), 0, buffer, index, output.Length);
				}
				else
				{
					Array.Copy(output.ToCharArray(0, count), 0, buffer, index, count);
				}
				*/
				return 0;
			}

	// Reset to the initial state.
	public void ResetState()
			{
				if(contextSupport)
				{
					throw new InvalidOperationException
						(S._("Xml_ContextNotNull"));
				}

				context.Reset();
				elementNames.Clear();
				incDepth = false;
				xmlPopScope = false;
				xmlnsPopScope = false;
				depth = 0;
				if(hasRoot) { state = State.XmlDeclaration; }
				readState = ReadState.Initial;
			}

	// Resolve an entity reference.
	public override void ResolveEntity()
			{
				throw new InvalidOperationException
					(S._("Xml_CannotResolveEntity"));
			}

	// Check the node state against the current state.
	private void CheckState(State nodeState)
			{
				switch(nodeState)
				{
					case State.XmlDeclaration:
					{
						if(state != State.XmlDeclaration)
						{
							Error(/* TODO */);
						}
						state = State.DoctypeDeclaration;
					}
					break;

					case State.DoctypeDeclaration:
					{
						if(state != State.XmlDeclaration &&
						   state != State.DoctypeDeclaration)
						{
							Error(/* TODO */);
						}
						state = State.Element;
					}
					break;

					case State.Element:
					{
						if(state == State.Misc || state == State.Attribute)
						{
							Error(/* TODO */);
						}
						state = State.Content;
					}
					break;

					case State.Content:
					{
						if(state != State.Content)
						{
							Error(/* TODO */);
						}
					}
					break;

					case State.Misc:
					{
						if(state == State.XmlDeclaration)
						{
							state = State.DoctypeDeclaration;
						}
						else if(state == State.Attribute)
						{
							Error(/* TODO */);
						}
					}
					break;

					case State.Attribute:
					{
						if(state != State.Attribute)
						{
							Error(/* TODO */);
						}
					}
					break;
				}
			}

	// Enter the error state, and throw an XmlException.
	private void Error()
			{
				Error("Xml_ReaderError");
			}
	private void Error(String messageTag, params Object[] args)
			{
				readState = ReadState.Error;
				nodes.Reset();
				input.Logger.Clear();
				throw new XmlException(String.Format(S._(messageTag), args));
			}

	// Get the namespace information for a given name.
	private void GetNamespaceInfo
				(String name, out String localName, out String namespaceURI,
				 out String prefix)
			{
				XmlNameTable nt = context.NameTable;

				// set the defaults
				localName = name;
				prefix = String.Empty;
				namespaceURI = String.Empty;

				if(namespaces)
				{
					XmlNamespaceManager nm = context.NamespaceManager;

					// find the namespace separator
					int index = name.LastIndexOf(':');

					// give an error if there's no name before the separator
					if(index == 0)
					{
						Error(/* TODO */);
					}

					// set the namespace information
					if(index > 0)
					{
						// add the prefix and local name to the name table
						prefix = nt.Add(name.Substring(0, index));
						localName = nt.Add(name.Substring(index + 1));

						// check for a valid prefix
						if(prefix.IndexOf(':') != -1)
						{
							Error(/* TODO */);
						}
					}
					// set the namespace uri based on the prefix
					namespaceURI = nm.LookupNamespace(prefix);
				}
			}

	// Callback for eof in the input handler.
	private void HandleEOF()
			{
				readState = ReadState.EndOfFile;
			}

	// Process 'xml:' and 'xmlns' information for a list of attributes.
	private void ProcessAttributeInfo(Attributes attributes)
			{
				AttributeInfo info;
				int count = attributes.Count;
				for(int i = 0; i < count; ++i)
				{
					info = attributes[i];
					String tmpName = info.Name;
					String tmpValue = info.Value;
					if(((Object)tmpName) == xmlBaseName)
					{
						context.BaseURI = tmpValue;
					}
					else if(((Object)tmpName) == xmlLangName)
					{
						context.XmlLang = tmpValue;
					}
					else if(((Object)tmpName) == xmlSpaceName)
					{
						if(tmpValue == "default")
						{
							context.XmlSpace = XmlSpace.Default;
						}
						else if(tmpValue == "preserve")
						{
							context.XmlSpace = XmlSpace.Preserve;
							if(sawPreserve == -1)
							{
								sawPreserve = elementNames.Count;
							}
						}
						else
						{
							Error(/* TODO */);
						}
					}
					else if(namespaces)
					{
						XmlNamespaceManager ns = context.NamespaceManager;
						if(((Object)tmpName) == xmlNSPrefix)
						{
							ns.AddNamespace("", tmpValue);
						}
						else if(tmpName.StartsWith("xmlns:"))
						{
							String tmpLocalName = tmpName.Substring(6);
							ns.AddNamespace(tmpLocalName, tmpValue);
						}
					}
				}
			}

	// Read an attribute fragment.
	//
	// Already read: ''
	private void ReadAttributeFragment()
			{
				// create our value log
				StringBuilder log = new StringBuilder();

				// get the attribute and segment information nodes
				AttributeInfo att = nodes.Attribute;
				Segments segments = att.Segments;

				// read the attribute value
				if(normalize)
				{
					ReadAttributeValueNormalize(log, segments, '\0');
				}
				else
				{
					ReadAttributeValue(log, segments, '\0');
				}

				// set the attribute information
				att.SetInfo(segments, '"', String.Empty, log.ToString());
			}

	// Read an attribute value.
	//
	// Already read: ('"' | "'")
	private void ReadAttributeValue
				(StringBuilder log, Segments segments, char quoteChar)
			{
				int segLen = 0;
				int textStart = 0;
				SegmentInfo seg;
				XmlNameTable nt = context.NameTable;

				// push the log onto the logger's log stack
				input.Logger.Push(log);

				// read until we consume all the text
				while(input.PeekChar() && input.peekChar != quoteChar)
				{
					// perform error checks and process segments
					if(input.peekChar == '<')
					{
						Error(/* TODO */);
					}
					else if(input.peekChar == '&')
					{
						// if we have text, add a new text segment to the list
						if(log.Length > textStart)
						{
							int textEnd = (log.Length - textStart);
							seg = segments[segLen++];
							seg.SetInfo(true, log.ToString(textStart, textEnd));
						}

						// remeber position
						int position = log.Length;

						// move to the '&' character
						input.NextChar();

						// read character or entity reference
						String name;
						char ch;
						if(ReadReferenceNormalize(out name, out ch))
						{
							// character reference (e.g. &amp;)
							log.Length = position;
							log.Append(ch);
						}
						else
						{
							// we leave entity refence as it is
							log.Length = position;
							log.Append('&');
							log.Append(name);
							log.Append(';');
						}
						seg = segments[segLen++];
						seg.SetInfo(true, log.ToString(position, log.Length - position));

						// store the start index of the next segment
						textStart = log.Length;
					}
					else
					{
						input.NextChar();
					}
				}

				// if we have text, add a new text segment to the list
				if(log.Length > textStart)
				{
					int textEnd = (log.Length - textStart);
					seg = segments[segLen++];
					seg.SetInfo(true, log.ToString(textStart, textEnd));
				}

				// we need at least one segment
				if(segLen == 0)
				{
					seg = segments[segLen++];
					seg.SetInfo(true, "");
				}

				// store the number of segments
				segments.Count = segLen;

				// pop the log from the logger's log stack
				input.Logger.Pop();
			}

	// Read and normalize an attribute value.
	//
	// Already read: ('"' | "'")
	private void ReadAttributeValueNormalize
				(StringBuilder log, Segments segments, char quoteChar)
			{
				int segLen = 0;
				int textStart = 0;
				SegmentInfo seg;
				XmlNameTable nt = context.NameTable;

				// push the log onto the logger's log stack
				input.Logger.Push(log);

				// read until we consume all the text
				while(input.PeekChar() && input.peekChar != quoteChar)
				{
					// perform error checks and process segments
					if(input.peekChar == '<')
					{
						Error(/* TODO */);
					}
					else if(input.peekChar == '&')
					{
						bool haveText = (log.Length > textStart);
						int textEnd = (log.Length - textStart);
						int position = log.Length;
						String s; char c;

						// move to the '&' character
						input.NextChar();

						// read the reference
						if(ReadReferenceNormalize(out s, out c))
						{
							log.Length = position;
							log.Append(c);
						}
						else
						{
							// if we have text, add a new text segment to the list
							if(haveText)
							{
								seg = segments[segLen++];
								seg.SetInfo(true, log.ToString(textStart, textEnd));
							}

							// add a new reference segment to the list
							seg = segments[segLen++];
							seg.SetInfo(false, nt.Add(s));

							// store the start index of the next segment
							textStart = log.Length;
						}
					}
					else if(XmlCharInfo.IsWhitespace(input.peekChar))
					{
						// skip whitespace without logging
						input.Logger.Pop();
						input.SkipWhitespace();
						input.Logger.Push(log);

						// set whitespace to single space character
						log.Append(' ');
					}
					else
					{
						input.NextChar();
					}
				}

				// if we have text, add a new text segment to the list
				if(log.Length > textStart)
				{
					int textEnd = (log.Length - textStart);
					seg = segments[segLen++];
					seg.SetInfo(true, log.ToString(textStart, textEnd));
				}

				// we need at least one segment
				if(segLen == 0)
				{
					seg = segments[segLen++];
					seg.SetInfo(true, "");
				}

				// store the number of segments
				segments.Count = segLen;

				// pop the log from the logger's log stack
				input.Logger.Pop();
			}

	// Read the attributes for an element start tag or xml declaration tag.
	//
	// Already read: ''
	private void ReadAttributes(bool qmark)
			{
				// create our value log
				StringBuilder log = new StringBuilder();

				// get our attribute list
				int attLen = 0;
				Attributes attributes = nodes.Attributes;

				// read until we consume all of the attributes
				while(true)
				{
					// skip potentially optional whitespace
					bool hasWS = input.SkipWhitespace();

					// check for an end character
					if(!input.PeekChar()) { Error("Xml_UnexpectedEOF"); }
					if(qmark)
					{
						if(input.peekChar == '?')
						{
							attributes.Count = attLen;
							return;
						}
					}
					else if(input.peekChar == '/' || input.peekChar == '>')
					{
						attributes.Count = attLen;
						return;
					}

					// the attribute name must be preceded by whitespace
					if(!hasWS) { Error(/* TODO */); }

					// read the attribute name
					String name = input.ReadName();

					// skip optional whitespace and read the '=' character
					input.SkipWhitespace();
					input.Expect('=');
					input.SkipWhitespace();

					// scan for a valid quote character
					if(!input.NextChar()) { Error("Xml_UnexpectedEOF"); }
					char quoteChar = input.currChar;
					if(quoteChar != '"' && quoteChar != '\'')
					{
						Error(/* TODO */);
					}

					// get the attribute and segment information nodes
					AttributeInfo att = attributes[attLen++];
					Segments segments = att.Segments;

					// read the attribute value
					if(normalize && !qmark)
					{
						ReadAttributeValueNormalize(log, segments, quoteChar);
					}
					else
					{
						ReadAttributeValue(log, segments, quoteChar);
					}

					// the attribute value must be properly terminated
					input.Expect(quoteChar);

					// get the value from the log
					String value = log.ToString();

					// reset the log
					log.Length = 0;

					// set the attribute information
					att.SetInfo(segments, quoteChar, name, value);
				}
			}

	// Read a character data section.
	//
	// Already read: '<![CDATA['
	private void ReadCDATA()
			{
				// check the state
				CheckState(State.Content);

				// create our log and push it onto the logger's log stack
				StringBuilder log = new StringBuilder();
				input.Logger.Push(log);

				// read until we've consumed all of the character data content
				while(input.NextChar() && input.PeekChar())
				{
					// check for the ']]' sequence
					if(input.currChar == ']' && input.peekChar == ']')
					{
						input.NextChar();

						// check if we've got the ']]>' sequence
						if(!input.NextChar()) { Error("Xml_UnexpectedEOF"); }
						if(input.currChar == '>')
						{
							// erase the ']]>' sequence from the log
							log.Length -= 3;

							// get the cdata from the log and pop it from the logger
							String value = input.Logger.Pop().ToString();

							// set the current node information and return
							nodes.CDATA.SetInfo(value);
							return;
						}
					}
				}

				// if we make it here then we hit eof, so give an error
				Error("Xml_UnexpectedEOF");
			}

	// Read a comment.
	//
	// Already read: '<!--'
	private void ReadComment()
			{
				// check the state
				CheckState(State.Misc);

				// create our log and push it onto the logger's log stack
				StringBuilder log = new StringBuilder();
				input.Logger.Push(log);

				// read until we consume all of the comment content
				while(input.NextChar() && input.PeekChar())
				{
					// check for the '--' sequence
					if(input.currChar == '-' && input.peekChar == '-')
					{
						input.NextChar();

						// erase the '--' sequence from the log
						log.Length -= 2;

						// get the comment from the log and pop it from the logger
						String value = input.Logger.Pop().ToString();

						// the comment must end with '>' at this point
						input.Expect('>');

						// set the current node information and return
						nodes.Comment.SetInfo(value);
						return;
					}
				}

				// if we make it here then we hit eof, so give an error
				Error("Xml_UnexpectedEOF");
			}

	// Read the xml document.
	//
	// Already read: ''
	private bool ReadDocument()
			{
				// use the text node's next node if possible
				NodeInfo current = nodes.Current;
				if(current is TextInfo)
				{
					if(((TextInfo)current).Next())
					{
						return true;
					}
				}

				// handle the eof case
				if(!input.PeekChar())
				{
					// give an error if there are missing end tags
					if(elementNames.Count > 0)
					{
						Error(/* TODO */);
					}

					// give an error if document level rules have been broken
					if(hasRoot && state != State.Misc)
					{
						Error(/* TODO */);
					}

					// reset to default state (depth=0 and etc...)
					nodes.Reset();

					// return false if there are no nodes left to read
					return false;
				}

				// handle the attribute fragment case
				if(state == State.Attribute)
				{
					ReadAttributeFragment();
					return true;
				}

				// increase the depth if we last read an element start tag
				if(incDepth)
				{
					++depth;
					incDepth = false;
				}

				// pop the base/lang/space scope if we last read an element end tag
				if(xmlPopScope)
				{
					context.PopScope();
					xmlPopScope = false;
					if(sawPreserve == elementNames.Count)
					{
						sawPreserve = -1;
					}
				}

				// pop the namespace scope if we last read an element end tag
				if(xmlnsPopScope)
				{
					context.NamespaceManager.PopScope();
					xmlnsPopScope = false;
				}

				// handle all the possible node cases
				switch(input.peekChar)
				{
					// handle the tag node case
					case '<':
					{
						input.NextChar();

						// handle all the possible tag cases
						if(!input.PeekChar()) { Error(/* TODO */); }
						switch(input.peekChar)
						{
							// handle the qmark tag case
							case '?':
							{
								input.NextChar();
								ReadQMarkTag();
							}
							break;

							// handle the emark tag case
							case '!':
							{
								input.NextChar();
								ReadEMarkTag();
							}
							break;

							// handle the element end tag case
							case '/':
							{
								input.NextChar();
								ReadETag();
							}
							break;

							// handle the element start tag case
							default:
							{
								ReadSTag();
							}
							break;
						}
					}
					break;

					// handle the whitespace node case
					case '\r':
					case '\n':
					case '\t':
					case ' ':
					{
						if(whitespace == WhitespaceHandling.None)
						{
							input.SkipWhitespace();
							return ReadDocument();
						}
						if(whitespace == WhitespaceHandling.Significant)
						{
							if(sawPreserve != -1)
							{
								input.SkipWhitespace();
								return ReadDocument();
							}
						}
						ReadWhitespace();
					}
					break;

					// handle the text node case
					default:
					{
						ReadText();
					}
					break;
				}

				// if we made it this far, we must've read a node, so return true
				return true;
			}

	// Read a doctype declaration.
	//
	// Already read: '<!DOCTYPE'
	private void ReadDoctypeDeclaration()
			{
				// check the state
				CheckState(State.DoctypeDeclaration);

				// (re)initialize the dtd reader
				dtdReader.Init(input, resolver);

				// read the dtd
				dtdReader.Read();

				// get the SYSTEM and PUBLIC values
				char quoteSYS = '"';
				char quotePUB = '"';
				String sysID = context.SystemId;
				String pubID = context.PublicId;
				if(sysID != String.Empty)
				{
					int len = sysID.Length-1;
					quoteSYS = sysID[len];
					sysID = sysID.Substring(0, len);
					context.SystemId = sysID;
				}
				if(pubID != String.Empty)
				{
					int len = pubID.Length-1;
					quotePUB = pubID[len];
					pubID = pubID.Substring(0, len);
					context.PublicId = pubID;
				}

				// get the attribute list
				Attributes attributes = nodes.Attributes;

				// get the name table
				XmlNameTable nt = context.NameTable;

				// add the SYSTEM attribute
				AttributeInfo att = attributes[0];
				Segments segments = att.Segments;
				segments[0].SetInfo(true, sysID);
				segments.Count = 1;
				att.SetInfo(segments, quoteSYS, nt.Add("SYSTEM"), sysID);

				// add the SYSTEM attribute
				att = attributes[1];
				segments = att.Segments;
				segments[0].SetInfo(true, pubID);
				segments.Count = 1;
				att.SetInfo(segments, quotePUB, nt.Add("PUBLIC"), pubID);

				// set the length of the attributes list
				attributes.Count = 2;

				// update search information
				attributes.UpdateInfo(context.NameTable);

				// set the current node information
				nodes.DoctypeDeclaration.SetInfo
					(attributes, context.DocTypeName, context.InternalSubset);
			}

	// Read a '<!' tag.
	//
	// Already read: '<!'
	private void ReadEMarkTag()
			{
				// handle all the possible emark tag cases
				if(!input.NextChar()) { Error("Xml_UnexpectedEOF"); }
				switch(input.currChar)
				{
					// handle the DOCTYPE case
					case 'D':
					{
						input.Expect("OCTYPE");
						ReadDoctypeDeclaration();
					}
					break;

					// handle the comment case
					case '-':
					{
						input.Expect('-');
						ReadComment();
					}
					break;

					// handle the CDATA case
					case '[':
					{
						input.Expect("CDATA[");
						ReadCDATA();
					}
					break;

					// handle the unknown case
					default:
					{
						Error(/* TODO */);
					}
					break;
				}
			}

	// Read an element end tag.
	//
	// Already read: '</'
	private void ReadETag()
			{
				// check the state
				CheckState(State.Content);

				// read the element name
				String name = input.ReadName();

				// skip optional whitespace
				input.SkipWhitespace();

				// the element end tag must end with '>' at this point
				input.Expect('>');

				// decrease the depth
				--depth;

				// set a flag to pop the base/lang/space scope on the next read
				xmlPopScope = true;

				// set a flag to pop the namespace scope on the next read
				xmlnsPopScope = namespaces;

				// this end tag must match the last start tag
				if(elementNames.Count == 0 ||
				   elementNames.Pop() != (Object)name)
				{
					Error(/* TODO */);
				}

				// enforce document level rules
				if(hasRoot && elementNames.Count == 0)
				{
					state = State.Misc;
				}

				// get the namespace information
				String localName;
				String namespaceURI;
				String prefix;
				GetNamespaceInfo
					(name, out localName, out namespaceURI, out prefix);

				// set the current node information
				nodes.EndElement.SetInfo(localName, name, namespaceURI, prefix);
			}

	// Read a processing instruction.
	//
	// Already read: '<?' Name
	[TODO]
	private void ReadProcessingInstruction(String target)
			{
				// TODO: check target for ('X'|'x')('M'|'m')('L'|'l')

				// check the state
				CheckState(State.Misc);

				// skip potentially optional whitespace
				bool hasWS = input.SkipWhitespace();

				// check for the closing characters
				if(!input.NextChar()) { Error("Xml_UnexpectedEOF"); }
				if(!input.PeekChar()) { Error("Xml_UnexpectedEOF"); }
				if(input.currChar == '?' && input.peekChar == '>')
				{
					input.NextChar();

					// set the current node information and return
					nodes.ProcessingInstruction.SetInfo(target, String.Empty);
					return;
				}

				// pi content must be preceded by whitespace
				if(!hasWS) { Error(/* TODO */); }

				// create our log and push it onto the logger's log stack
				StringBuilder log = new StringBuilder();
				input.Logger.Push(log);

				// read until we consume all of the pi content
				while(input.NextChar() && input.PeekChar())
				{
					if(input.currChar == '?' && input.peekChar == '>')
					{
						input.NextChar();

						// erase the '?>' sequence from the log
						log.Length -= 2;

						// get the content from the log and pop it from the logger
						String value = input.Logger.Pop().ToString();

						// set the current node information and return
						nodes.ProcessingInstruction.SetInfo(target, value);
						return;
					}
				}

				// if we make it here then we hit eof, so give an error
				Error("Xml_UnexpectedEOF");
			}

	// Read a '<?' tag.
	//
	// Already read: '<?'
	private void ReadQMarkTag()
			{
				// read the pi target name
				String target = input.ReadName();

				// check if we have a pi or xml declaration
				if((Object)target == xmlCompareQuick)
				{
					ReadXmlDeclaration();
				}
				else
				{
					ReadProcessingInstruction(target);
				}
			}

	// Read an entity reference.
	//
	// Already read: '&'
	private String ReadReference()
			{
				// check for an empty reference
				if(!input.PeekChar()) { Error(/* TODO */); }
				if(input.peekChar == ';') { Error(/* TODO */); }

				// create our log and push it onto the logger's log stack
				StringBuilder log = new StringBuilder();
				input.Logger.Push(log);

				// handle character or general references
				if(input.peekChar == '#')
				{
					input.NextChar();

					// check for an empty character reference
					if(!input.PeekChar()) { Error(/* TODO */); }
					if(input.peekChar == ';') { Error(/* TODO */); }

					// handle a hex or decimal character reference
					if(input.peekChar == 'x')
					{
						input.NextChar();

						// check for an empty hex character reference
						if(!input.PeekChar()) { Error(/* TODO */); }
						if(input.peekChar == ';') { Error(/* TODO */); }

						// read until we consume all the digits
						while(input.NextChar() && input.currChar != ';')
						{
							if((input.currChar < '0' || input.currChar > '9') &&
							   (input.currChar < 'A' || input.currChar > 'F') &&
							   (input.currChar < 'a' || input.currChar > 'f'))
							{
								Error(/* TODO */);
							}
						}
					}
					else
					{
						// read until we consume all the digits
						while(input.NextChar() && input.currChar != ';')
						{
							if(input.currChar < '0' || input.currChar > '9')
							{
								Error(/* TODO */);
							}
						}
					}

					// we hit eof, otherwise we'd have ';', so give an error
					if(input.currChar != ';') { Error("Xml_UnexpectedEOF"); }
				}
				else
				{
					// read the reference name
					input.ReadName();

					// the reference must end with ';' at this point
					input.Expect(';');
				}

				// pop the log and return the reference name
				input.Logger.Pop();
				return context.NameTable.Add(log.ToString(0, log.Length-1));
			}

	// Read and normalize an entity reference, returning true if normalized.
	//
	// Already read: '&'
	private bool ReadReferenceNormalize(out String name, out char value)
			{
				// check for an empty reference
				if(!input.PeekChar()) { Error(/* TODO */); }
				if(input.peekChar == ';') { Error(/* TODO */); }

				// set the defaults
				name = null;
				value = (char)0;

				// handle character or general references
				if(input.peekChar == '#')
				{
					input.NextChar();

					// check for an empty character reference
					if(!input.PeekChar()) { Error(/* TODO */); }
					if(input.peekChar == ';') { Error(/* TODO */); }

					// handle a hex or decimal character reference
					if(input.peekChar == 'x')
					{
						input.NextChar();

						// check for an empty hex character reference
						if(!input.PeekChar()) { Error(/* TODO */); }
						if(input.peekChar == ';') { Error(/* TODO */); }

						// read until we consume all the digits
						while(input.NextChar() && input.currChar != ';')
						{
							value *= (char)0x10;
							if(input.currChar >= '0' && input.currChar <= '9')
							{
								value += (char)(input.currChar - '0');
							}
							else if(input.currChar >= 'A' && input.currChar <= 'F')
							{
								value += (char)((input.currChar - 'A') + 10);
							}
							else if(input.currChar >= 'a' && input.currChar <= 'f')
							{
								value += (char)((input.currChar - 'a') + 10);
							}
							else
							{
								Error(/* TODO */);
							}
						}
					}
					else
					{
						// read until we consume all the digits
						while(input.NextChar() && input.currChar != ';')
						{
							value *= (char)10;
							if(input.currChar >= '0' && input.currChar <= '9')
							{
								value += (char)(input.currChar - '0');
							}
							else
							{
								Error(/* TODO */);
							}
						}
					}

					// we hit eof, otherwise we'd have ';', so give an error
					if(input.currChar != ';') { Error("Xml_UnexpectedEOF"); }

					// check the range of the character
					if(!XmlCharInfo.IsChar(value))
					{
						Error(/* TODO */);
					}

					return true;
				}
				else
				{
					// read the reference name
					name = input.ReadName();

					// the reference must end with ';' at this point
					input.Expect(';');

					// check for builtins
					switch(name)
					{
						case "amp":
						{
							value = '&';
							return true;
						}
						// Not reached.

						case "apos":
						{
							value = '\'';
							return true;
						}
						// Not reached.

						case "gt":
						{
							value = '>';
							return true;
						}
						// Not reached.

						case "lt":
						{
							value = '<';
							return true;
						}
						// Not reached.

						case "quot":
						{
							value = '"';
							return true;
						}
						// Not reached.

						default:
						{
							return false;
						}
						// Not reached.
					}
				}
			}

	// Read an element start tag.
	//
	// Already read: '<'
	private void ReadSTag()
			{
				// check the state
				CheckState(State.Element);

				// read the element name
				String name = input.ReadName();

				// read the element's attributes
				ReadAttributes(false);

				// check if this is an empty element or not
				if(!input.NextChar()) { Error("Xml_UnexpectedEOF"); }
				bool empty = false;
				if(input.currChar == '/')
				{
					input.Expect('>');
					empty = true;
					xmlPopScope = true;
					xmlnsPopScope = namespaces;

					// enforce document level rules
					if(hasRoot && elementNames.Count == 0)
					{
						state = State.Misc;
					}
				}
				else
				{
					incDepth = true;
					elementNames.Push(name);
				}

				// push the "xml:" scope
				context.PushScope();

				// push the "xmlns" scope
				if(namespaces)
				{
					context.NamespaceManager.PushScope();
				}

				// process the attribute information
				Attributes attributes = nodes.Attributes;
				ProcessAttributeInfo(attributes);

				// get the namespace information
				String localName;
				String namespaceURI;
				String prefix;
				GetNamespaceInfo
					(name, out localName, out namespaceURI, out prefix);

				// update search information and check for duplicates
				if(namespaces)
				{
					attributes.UpdateInfo
						(context.NameTable, context.NamespaceManager);
				}
				else
				{
					attributes.UpdateInfo(context.NameTable);
				}

				// set the current node information
				nodes.Element.SetInfo
					(empty, attributes, localName, name, namespaceURI, prefix);
			}

	// Read a text node.
	//
	// Already read: ''
	private void ReadText()
			{
				ReadText(false);
			}
	private void ReadText(bool fromWhitespace)
			{
				// check the state
				CheckState(State.Content);

				// set things up
				int segLen = 0;
				SegmentInfo seg;
				TextInfo info = nodes.Text;
				Segments segments = info.Segments;
				XmlNameTable nt = context.NameTable;

				// handle log setup, based on if text was read as whitespace
				StringBuilder log;
				if(fromWhitespace)
				{
					// copy the whitespace log from the logger's log stack
					log = input.Logger.Peek();
				}
				else
				{
					// create a new log and push it onto the logger's log stack
					log = new StringBuilder();
					input.Logger.Push(log);
				}

				// read until we consume all the text
				while(input.PeekChar() && input.peekChar != '<')
				{
					if(input.peekChar == '&')
					{
						String s; char c;

						// skip the '&' character
						input.Logger.Pop();
						input.NextChar();

						// read the reference
						if(ReadReferenceNormalize(out s, out c))
						{
							log.Append(c);
						}
						else
						{
							// if we have text, add a new text segment to the list
							if(log.Length > 0)
							{
								seg = segments[segLen++];
								seg.SetInfo(true, log.ToString());
								log.Length = 0;
							}

							// add a new reference segment to the list
							seg = segments[segLen++];
							seg.SetInfo(false, nt.Add(s));
						}

						// push the log back onto the log stack
						input.Logger.Push(log);
					}
					else
					{
						input.NextChar();
					}
				}

				// if we have text, add a new text segment to the list
				if(log.Length > 0)
				{
					seg = segments[segLen++];
					seg.SetInfo(true, log.ToString());
					log.Length = 0;
				}

				// store the number of segments
				segments.Count = segLen;

				// pop the log from the logger's log stack
				input.Logger.Pop();

				// set the current node information
				info.SetInfo(segments);
			}

	// Read a whitespace node.
	//
	// Already read: ''
	private void ReadWhitespace()
			{
				// check the state
				CheckState(State.Misc);

				// set the significant whitespace flag
				bool significant = (sawPreserve != -1);

				// create our log and push it onto the logger's log stack
				StringBuilder log = new StringBuilder();
				input.Logger.Push(log);

				// skip the required whitespace
				if(!input.SkipWhitespace()) { Error(/* TODO */); }

				// check to see if we should treat the whitespace as text
				if(input.PeekChar() && input.peekChar != '<')
				{
					ReadText(true);
					return;
				}

				// get the whitespace from the log and pop it from the logger
				String value = input.Logger.Pop().ToString();

				// set the current node information
				nodes.Whitespace.SetInfo(significant, value);
			}

	// Read an xml declaration.
	//
	// Already read: '<?xml'
	[TODO]
	private void ReadXmlDeclaration()
			{
				// TODO: encoding checks

				// check the state
				CheckState(State.XmlDeclaration);

				// create our log and push it onto the logger's log stack
				StringBuilder log = new StringBuilder();
				input.Logger.Push(log);

				// read the xml declaration's attributes
				ReadAttributes(true);

				// get attributes from the log and pop it from the logger
				String value = input.Logger.Pop().ToString();

				// trim the leading and trailing whitespace from the value
				value = value.Trim(' ', '\t', '\n', '\r');

				// the xml declaration must end with '?>' at this point
				input.Expect("?>");

				// update search information and check for duplicates
				Attributes attributes = nodes.Attributes;
				attributes.UpdateInfo(context.NameTable);

				// check that we have the right number of attributes
				if(attributes.Count < 1 || attributes.Count > 3)
				{
					Error(/* TODO */);
				}

				// check the version attribute
				AttributeInfo att = attributes[0];
				if(att.Name != "version")
				{
					Error(/* TODO */);
				}
				if(att.Value != "1.0")
				{
					Error(/* TODO */);
				}

				// check the optional attributes
				if(attributes.Count > 1)
				{
					bool requireStandalone = true;
					att = attributes[1];
					if(att.Name == "encoding")
					{
						// TODO: encoding checks need to be done, but the
						//       XmlStreamReader also needs support for a
						//       larger range of encodings and should use the
						//       "<?xml" sequence to guesstimate the encoding
						//       if byte-order marks are not present... we will
						//       also need to determine if the guessed encoding
						//       is close enough to the actual to allow it
						//       without error
						if(attributes.Count > 2)
						{
							att = attributes[2];
						}
						else
						{
							requireStandalone = false;
						}
					}
					if(requireStandalone)
					{
						if(att.Name != "standalone")
						{
							Error(/* TODO */);
						}
						if(att.Value != "yes" && att.Value != "no")
						{
							Error(/* TODO */);
						}
					}
				}

				// set the current node information
				nodes.XmlDeclaration.SetInfo(attributes, value);
			}

}; // class XmlTextReader

}; // namespace System.Xml
