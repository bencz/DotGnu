/*
 * XmlTextWriter.cs - Implementation of the "System.Xml.XmlTextWriter" class.
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

using System;
using System.IO;
using System.Text;
using System.Globalization;

public class XmlTextWriter : XmlWriter
{
	// Internal state.
	private bool                namespaces;
	private bool                needPrefixDecl;
	private int                 indentation;
	private int                 indentLevel;
	private char                indentChar;
	private char                quoteChar;
	private char[]              indentChars;
	private ElementScope        scope;
	private Formatting          formatting;
	private Special             special;
	private SpecialWriter       specialWriter;
	private String              nsPrefix;
	private String              xmlLang;
	protected WriteState        writeState; // need protected for XmlFragmentTextWriter
	protected XmlNameTable      nameTable;  // need protected for XmlFragmentTextWriter
	private XmlNamespaceManager namespaceManager;
	private XmlSpace            xmlSpace;
	internal bool               autoShiftToContent;
	internal TextWriter         writer;

	private enum Special
	{
		None      = 0,
		Lang      = 1,
		Space     = 2,
		Namespace = 3

	}; // enum Special


	// Constructors.
	public XmlTextWriter(Stream w, Encoding encoding)
			{
				writer = new StreamWriter
					(w, ((encoding != null) ? encoding : Encoding.UTF8));
				Initialize();
			}
	public XmlTextWriter(String filename, Encoding encoding)
			{
				writer = new StreamWriter
					(new FileStream(filename, FileMode.Create, FileAccess.Write),
					 ((encoding != null) ? encoding : Encoding.UTF8));
				Initialize();
			}
	public XmlTextWriter(TextWriter w)
			{
				if(w == null)
				{
					throw new ArgumentNullException("w");
				}
				writer = w;
				Initialize();
			}

	// Initialize the common writer fields.
	private void Initialize()
			{
				namespaces = true;
				needPrefixDecl = false;
				indentation = 2;
				indentLevel = 0;
				indentChar = ' ';
				quoteChar = '"';
				indentChars = new char[indentation];
				scope = null;
				formatting = System.Xml.Formatting.None;
				special = Special.None;
				specialWriter = new SpecialWriter(writer);
				nsPrefix = null;
				xmlLang = null;
				writeState = System.Xml.WriteState.Start;
				nameTable = new NameTable();
				namespaceManager = new XmlNamespaceManager(nameTable);
				xmlSpace = XmlSpace.None;
				autoShiftToContent = false;
				for(int i = 0; i < indentation; i++)
				{
					indentChars[i] = indentChar;
				}
			}

	// Push a new element scope.
	private void PushScope(String prefix, String localName, bool scopeShown, String xmlns)
			{
				scope = new ElementScope(scope);
				scope.prefix = (prefix != null ? nameTable.Add(prefix) : null);
				scope.localName =
					(localName != null ? nameTable.Add(localName) : null);
				scope.xmlns = (xmlns != null ? nameTable.Add(xmlns) : null);
				scope.scopeShown = scopeShown;
				scope.indentLevel = indentLevel;
				if(formatting == System.Xml.Formatting.Indented)
				{
					indentLevel++;
				}
				scope.indent = false;
				scope.xmlSpace = xmlSpace;
				scope.xmlLang = xmlLang;
				scope.genPrefixCount = 0;
				namespaceManager.PushScope();
			}

	// Pop the current element scope.
	private void PopScope()
			{
				indentLevel = scope.indentLevel;
				xmlSpace = scope.xmlSpace;
				xmlLang = scope.xmlLang;
				scope = scope.next;
				namespaceManager.PopScope();
			}

	// Add indentation before the current element.
	private void DoIndent(bool useScope)
			{
				if(xmlSpace != System.Xml.XmlSpace.Preserve)
				{
					if(formatting == System.Xml.Formatting.Indented)
					{
						int indent = useScope ? scope.indentLevel : indentLevel;
						writer.Write(Environment.NewLine);
						while(indent > 0)
						{
							writer.Write(indentChars);
							--indent;
						}
						// flag that perent element needs DoIndent on WriteEndElement
						if (!useScope && (scope != null))
						{
							scope.indent = true;
						}
					}
				}
			}

	// State flags for "Sync".
	[Flags]
	private enum WriteStateFlag
	{
		StartFlag     = (1<<0),
		PrologFlag    = (1<<1),
		ElementFlag   = (1<<2),
		AttributeFlag = (1<<3),
		ContentFlag   = (1<<4),
		ClosedFlag    = (1<<5)

	}; // enum WriteStateFlag


	// Generate a prefix for the given namespace.
	private String GeneratePrefix(String ns)
			{
				// Create the prefix builder.
				StringBuilder prefixBuilder = new StringBuilder();

				// Append the start of the prefix.
				prefixBuilder.Append("d").Append(indentLevel).Append("p");

				// Get the length of the start of the prefix.
				int len = prefixBuilder.Length;

				// Generate prefixes until an unused one is found.
				while(true)
				{
					// Update the generated prefix count.
					++scope.genPrefixCount;

					// Build the prefix.
					prefixBuilder.Append(scope.genPrefixCount);

					// Get the prefix from the builder.
					String prefix = prefixBuilder.ToString();

					// Get the mapping for the prefix.
					Object mapping = namespaceManager.LookupNamespace(prefix);

					// Return the prefix if it is unused.
					if(mapping == null)
					{
						// Add the mapping to the namespace manager.
						namespaceManager.AddNamespace(prefix, ns);

						// Return the prefix.
						return prefix;
					}

					// Reset the prefix builder.
					prefixBuilder.Length = len;
				}
			}

	// Handle the end of a special attribute.
	private void SpecialAttributeEnd()
			{
				switch(special)
				{
					case Special.Lang:
					{
						// Set the xml:lang value.
						xmlLang = specialWriter.AttributeValue;
					}
					break;

					case Special.Space:
					{
						// Get the attribute value.
						String value = specialWriter.AttributeValue;

						// Get the length of the attribute value.
						int len = value.Length;

						// Set the xml:space value.
						if(len == 7 && value == "default")
						{
							xmlSpace = System.Xml.XmlSpace.Default;
						}
						else if(len == 8 && value == "preserve")
						{
							xmlSpace = System.Xml.XmlSpace.Preserve;
						}
						else
						{
							// Reset the attribute value builder.
							specialWriter.ResetBuilder();

							// Reset the writer.
							writer = specialWriter.Writer;

							// Reset the special.
							special = Special.None;

							// Signal that the xml:space value is invalid.
							throw new ArgumentException(/* TODO */);
						}
					}
					break;

					case Special.Namespace:
					{
						// Get the attribute value.
						String value = specialWriter.AttributeValue;

						// Add the mapping to the namespace manager.
						namespaceManager.AddNamespace(nsPrefix, value);

						// Check for match to required prefix declaration.
						if(needPrefixDecl && nsPrefix == scope.prefix &&
						   value == scope.xmlns)
						{
							// Flag that required prefix declaration is present.
							needPrefixDecl = false;
						}

						// Reset the namespace prefix.
						nsPrefix = null;
					}
					break;
				}

				// Reset the attribute value builder.
				specialWriter.ResetBuilder();

				// Reset the writer.
				writer = specialWriter.Writer;

				// Reset the special.
				special = Special.None;
			}

	// Handle the start of a special attribute.
	private void SpecialAttributeStart
				(String prefix, String localName, Special type)
			{
				// Set the type of the special attribute.
				special = type;

				// Write the attribute prefix, if needed.
				if(((Object)prefix) == null)
				{
					// Write the name.
					writer.Write("xmlns");

					// Set the local name.
					localName = String.Empty;
				}
				else
				{
					// Write the prefixed attribute name.
					writer.Write(prefix);
					writer.Write(':');
					writer.Write(localName);
				}

				// Set the namespace prefix, if needed.
				if(special == Special.Namespace)
				{
					// Set the namespace prefix.
					nsPrefix = localName;
				}

				// Output the start of the attribute value.
				writer.Write('=');
				writer.Write(quoteChar);

				// Set the writer.
				writer = specialWriter;

				// We are now in the attribute state.
				writeState = System.Xml.WriteState.Attribute;
			}

	// Synchronize the output with a particular document area.
	private void Sync(WriteStateFlag flags, bool indent)
			{
				System.Xml.WriteState writeStateOnCalling = writeState;

				// Determine if the current write state is compatible
				// with the synchronisation flags, and shift to the
				// requested state if necessary.
				switch(writeState)
				{
					case WriteState.Element:
					{
						if((flags & WriteStateFlag.ContentFlag) != 0)
						{
							// Terminate the element.
							TerminateElement(false);

							// Switch to contents.
							writeState = System.Xml.WriteState.Content;
						}
						else
						{
							throw new InvalidOperationException
								(S._("Xml_InvalidWriteState"));
						}
					}
					break;

					case WriteState.Attribute:
					{
						if((flags & WriteStateFlag.AttributeFlag) != 0)
						{
							// We can write directly to the attribute.
						}
						else if((flags & WriteStateFlag.ContentFlag) != 0)
						{
							// Terminate the attribute.
							TerminateAttribute();

							// Terminate the element.
							TerminateElement(false);

							// Switch to contents.
							writeState = System.Xml.WriteState.Content;
						}
						else
						{
							throw new InvalidOperationException
								(S._("Xml_InvalidWriteState"));
						}
					}
					break;

					case WriteState.Content:
					{
						if((flags & WriteStateFlag.ContentFlag) == 0)
						{
							throw new InvalidOperationException
								(S._("Xml_InvalidWriteState"));
						}
					}
					break;

					case WriteState.Start:
					{
						// Automatically shift to the content mode
						// if we are outputting a document fragment
						// from "XmlNode.InnerXml" or "XmlNode.OuterXml".
						if(autoShiftToContent)
						{
							if((flags & WriteStateFlag.ContentFlag) == 0)
							{
								throw new InvalidOperationException
									(S._("Xml_InvalidWriteState"));
							}
							writeState = WriteState.Content;
						}
						else
						{
							if(((1 << (int)writeState) & (int)flags) == 0)
							{
								throw new InvalidOperationException
									(S._("Xml_InvalidWriteState"));
							}
							writeState = System.Xml.WriteState.Prolog;
						}
					}
					break;

					case WriteState.Prolog:
					case WriteState.Closed:
					{
						if(((1 << (int)writeState) & (int)flags) == 0)
						{
							throw new InvalidOperationException
								(S._("Xml_InvalidWriteState"));
						}
					}
					break;
				}
				if(indent) // do indent if possible
				{
					bool needIndent;

					if(scope != null)
					{
						needIndent = scope.indent;
					}
					else
					{
						needIndent = false;
					}
					if((writeStateOnCalling == System.Xml.WriteState.Prolog) ||
						(writeStateOnCalling == System.Xml.WriteState.Element) ||
						(writeStateOnCalling == System.Xml.WriteState.Attribute) ||
						needIndent)
					{
						DoIndent(false);
					}
				}
			}

	// Terminate an attribute.
	private void TerminateAttribute()
			{
				// Handle the end of a special attribute, if needed.
				if(special != Special.None) { SpecialAttributeEnd(); }

				// Terminate the attribute.
				writer.Write(quoteChar);
			}

	// Terminate a start element.
	private void TerminateElement(bool empty)
			{
				// Write the prefix declaration, if needed.
				if(needPrefixDecl)
				{
					writer.Write(" xmlns:");
					writer.Write(scope.prefix);
					writer.Write('=');
					writer.Write(quoteChar);
					WriteQuotedString(scope.xmlns);
					writer.Write(quoteChar);
					needPrefixDecl = false;
				}

				// Terminate the element.
				if(empty)
				{
					// Terminate the empty element.
					writer.Write(" />");
				}
				else
				{
					// Terminate the start element.
					writer.Write('>');
				}
			}

	private void WriteRawData(String value)
			{
				writer.Write(value);
			}

	// Write an xml declaration.
	virtual protected void WriteXmlDeclaration(String text)
			{
				// make sure we're in the start state
				if(writeState != System.Xml.WriteState.Start)
				{
					throw new InvalidOperationException
						(S._("Xml_InvalidWriteState"));
				}

				// create the reader
				XmlTextReader r = new XmlTextReader
					(new StringReader("<?xml "+text+"?>"), nameTable);

				// read the value
				r.Read();

				// read the version attribute
				if(r["version"] != "1.0")
				{
					throw new ArgumentException
						(S._("Xml_InvalidVersion"), "text");
				}

			#if !ECMA_COMPAT
				// make sure the encoding matches
				String encoding = r["encoding"];
				if(encoding != null && encoding.Length > 0 &&
				   writer.Encoding.WebName != encoding)
				{
					throw new ArgumentException
						(S._("Xml_InvalidEncoding"), "text");
				}
			#endif

				// make sure the standalone is valid
				String standalone = r["standalone"];
				bool haveStandalone = false;
				if(standalone != null && standalone.Length > 0)
				{
					if(standalone != "yes" && standalone != "no")
					{
						throw new ArgumentException
							(S._("Xml_InvalidEncoding"), "text");
					}
					haveStandalone = true;
				}

				// write the start of the document
				if(haveStandalone)
				{
					WriteStartDocument(standalone == "yes");
				}
				else
				{
					WriteStartDocument();
				}
			}

	// Close this writer and free all resources.
	public override void Close()
			{
				if(writeState != System.Xml.WriteState.Closed)
				{
					// Close all open element scopes.
					if(writeState == System.Xml.WriteState.Attribute)
					{
						// Terminate the attribute and the element start.
						writer.Write(quoteChar);
						writer.Write(" />");
						if(scope != null)
						{
							PopScope();
						}
					}
					else if(writeState == System.Xml.WriteState.Element)
					{
						// Terminate the element start.  We may need
						// to ignore this if writing in "auto-shift"
						// mode as we may have written a bare attribute.
						if(scope != null || !autoShiftToContent)
						{
							writer.Write(" />");
							PopScope();
						}
					}
					while(scope != null)
					{
						if (scope.indent)
						{
							DoIndent(true);
						}
						writer.Write("</");
						writer.Write(scope.localName);
						writer.Write('>');
						PopScope();
					}

					// write last newline
					DoIndent(false);
					// Flush and close the TextWriter stream.
					Flush();
					writer.Close();
					writeState = System.Xml.WriteState.Closed;
				}
			}

	// Flush the buffers that are used by this writer.
	public override void Flush()
			{
				writer.Flush();
			}

	// Look up the namespace URI for a specified namespace prefix.
	public override String LookupPrefix(String ns)
			{
				if(((Object)ns) == null || ns.Length == 0)
				{
					throw new ArgumentNullException("ns");
				}
				String prefix = namespaceManager.LookupPrefix(ns);
				return (prefix == String.Empty) ? null : prefix;
			}

	// Encode an array as base64 and write it out as text.
	public override void WriteBase64(byte[] buffer, int index, int count)
			{
			
				// Validate the parameters.
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				else if(index < 0)
				{
					throw new ArgumentOutOfRangeException
						("index", S._("ArgRange_Array"));
				}
				else if(count < 0 || (buffer.Length - index) < count)
				{
					throw new ArgumentOutOfRangeException
						("count", S._("ArgRange_Array"));
				}
				else if(writeState == WriteState.Closed)
				{
					throw new InvalidOperationException
						(S._("Xml_InvalidWriteState"));
				}
		
				// Synchronize to the content or attribute area.
				Sync(WriteStateFlag.ContentFlag |
					 WriteStateFlag.AttributeFlag, false);

				WriteRawData(XmlConvert.ToBase64String(buffer, index, count));
			}
	
	// Encode an array as BinHex and write it out as text.
	public override void WriteBinHex(byte[] buffer, int index, int count)
			{
				// Validate the parameters.
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				else if(index < 0)
				{
					throw new ArgumentOutOfRangeException
						("index", S._("ArgRange_Array"));
				}
				else if(count < 0)
				{
					throw new ArgumentOutOfRangeException
						("count", S._("ArgRange_Array"));
				}
				else if(buffer.Length - index < count)
				{
					throw new ArgumentException
						("index", S._("ArgRange_Array"));
				}
				else if(writeState == WriteState.Closed)
				{
					throw new InvalidOperationException
						(S._("Xml_InvalidWriteState"));
				}

				// Synchronize to the content or attribute area.
				Sync(WriteStateFlag.ContentFlag |
					 WriteStateFlag.AttributeFlag, false);

				WriteRawData(XmlConvert.ToHexString(buffer, index, count));	
			}

	// Write out a CDATA block.
	public override void WriteCData(String text)
			{
				if(text != null && text.IndexOf("]]>") != -1)
				{
					throw new ArgumentException
						(S._("Xml_InvalidXmlWritten"), "text");
				}
				Sync(WriteStateFlag.ContentFlag, false);
				writer.Write("<![CDATA[");

				if(text != null)
				{
					writer.Write(text);
				}
				writer.Write("]]>");
			}

	// Write a character entity.
	public override void WriteCharEntity(char ch)
			{
				Sync(WriteStateFlag.ContentFlag |
					 WriteStateFlag.AttributeFlag, false);
				writer.Write("&#x{0:X2};", (int)ch);
			}

	// Write a text buffer.
	public override void WriteChars(char[] buffer, int index, int count)
			{
				// Validate the parameters.
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				else if(index < 0)
				{
					throw new ArgumentOutOfRangeException
						("index", S._("ArgRange_Array"));
				}
				else if(count < 0 || (buffer.Length - index) < count)
				{
					throw new ArgumentOutOfRangeException
						("count", S._("ArgRange_Array"));
				}

				// Synchronize to the content or attribute area.
				Sync(WriteStateFlag.ContentFlag |
					 WriteStateFlag.AttributeFlag, false);

				// The buffer must not end in a low surrogate.
				if(count > 0 &&
				   buffer[index + count - 1] >= 0xD800 &&
				   buffer[index + count - 1] <= 0xDBFF)
				{
					throw new ArgumentException
						(S._("Xml_InvalidSurrogate"), "buffer");
				}

				// Quote the character buffer and output it.
				int posn, prev;
				char ch;
				prev = 0;
				for(posn = 0; posn < count; ++posn)
				{
					ch = buffer[index + posn];
					switch(ch)
					{
						case '\x09': case '\x0A': case '\x0D': break;

						case '<':
						{
							if(prev < posn)
							{
								writer.Write
									(buffer, index + prev, posn - prev);
							}
							writer.Write("&lt;");
							prev = posn + 1;
						}
						break;

						case '>':
						{
							if(prev < posn)
							{
								writer.Write
									(buffer, index + prev, posn - prev);
							}
							writer.Write("&gt;");
							prev = posn + 1;
						}
						break;

						case '&':
						{
							if(prev < posn)
							{
								writer.Write
									(buffer, index + prev, posn - prev);
							}
							writer.Write("&amp;");
							prev = posn + 1;
						}
						break;

						case '"':
						{
							if(writeState != System.Xml.WriteState.Attribute)
							{
								break;
							}
							if(prev < posn)
							{
								writer.Write
									(buffer, index + prev, posn - prev);
							}
							writer.Write("&quot;");
							prev = posn + 1;
						}
						break;

						case '\'':
						{
							if(writeState != System.Xml.WriteState.Attribute)
							{
								break;
							}
							if(prev < posn)
							{
								writer.Write
									(buffer, index + prev, posn - prev);
							}
							writer.Write("&apos;");
							prev = posn + 1;
						}
						break;

						default:
						{
							if(ch < 0x20)
							{
								// Quote a low-ASCII control character.
								if(prev < posn)
								{
									writer.Write
										(buffer, index + prev, posn - prev);
								}
								prev = posn + 1;
								writer.Write("&#");
								writer.Write
									(((int)ch).ToString
									  (null, NumberFormatInfo.InvariantInfo));
								writer.Write(';');
							}
						}
						break;
					}
				}
				if(prev < count)
				{
					writer.Write(buffer, index + prev, posn - prev);
				}
			}

	// Write a comment.
	public override void WriteComment(String text)
			{
				// Bail out if the comment text contains "--".
				if (text !=  null && text.Length > 0)
				{
					if(text.IndexOf("--") != -1)
					{
						throw new ArgumentException
							(S._("Xml_InvalidXmlWritten"), "text");
					}
				}
				// Synchronize to an area that allows comments.
				Sync(WriteStateFlag.StartFlag |
				     WriteStateFlag.ContentFlag |
					 WriteStateFlag.PrologFlag, true);

				// Write out the comment.
				writer.Write("<!--");
				if(text != null)
				{
					writer.Write(text);
				}
				writer.Write("-->");
			}

	// Write a document type specification.
	public override void WriteDocType(String name, String pubid,
									  String sysid, String subset)
			{
				// We must be in the prolog.
				if(autoShiftToContent)
				{
					if(writeState == System.Xml.WriteState.Closed)
					{
						throw new InvalidOperationException
							(S._("Xml_InvalidWriteState"));
					}
				}
				else
				{
					if(writeState != System.Xml.WriteState.Prolog)
					{
						throw new InvalidOperationException
							(S._("Xml_InvalidWriteState"));
					}
				}

				// Validate the name.
				if(!XmlReader.IsName(name))
				{
					throw new ArgumentException
						(S._("Xml_InvalidName"), "name");
				}

				// Write out the document type information.
				DoIndent(false);
				writer.Write("<!DOCTYPE ");
				writer.Write(name);
				writeState = System.Xml.WriteState.Attribute;
				if(pubid != null)
				{
					writer.Write(" PUBLIC ");
					writer.Write(quoteChar);
					WriteQuotedString(pubid);
					writer.Write(quoteChar);
					writer.Write(' ');
					writer.Write(quoteChar);
					WriteQuotedString(sysid);
					writer.Write(quoteChar);
				}
				else if(sysid != null)
				{
					writer.Write(" SYSTEM ");
					writer.Write(quoteChar);
					WriteQuotedString(sysid);
					writer.Write(quoteChar);
				}
				if(subset != null)
				{
					writer.Write(' ');
					writer.Write('[');
					writer.Write(subset);
					writer.Write(']');
				}
				writer.Write('>');

				// Return to the prolog.
				writeState = System.Xml.WriteState.Prolog;
			}

	// Write the end of an attribute.
	public override void WriteEndAttribute()
			{
				// We must be in the Attribute state.
				if(writeState != System.Xml.WriteState.Attribute)
				{
					throw new InvalidOperationException
						(S._("Xml_InvalidWriteState"));
				}

				// Terminate the attribute and return to the element state.
				TerminateAttribute();
				writeState = System.Xml.WriteState.Element;
			}

	// Write the document end information and reset to the start state.
	public override void WriteEndDocument()
			{
				if(writeState == System.Xml.WriteState.Start ||
				   writeState == System.Xml.WriteState.Prolog ||
				   writeState == System.Xml.WriteState.Closed)
				{
					throw new ArgumentException
						(S._("Xml_InvalidWriteState"), "WriteState");
				}
				if(writeState == System.Xml.WriteState.Attribute)
				{
					// Terminate the attribute and the element start.
					writer.Write(quoteChar);
					writer.Write(" />");
					PopScope();
				}
				else if(writeState == System.Xml.WriteState.Element)
				{
					// Terminate the element start.
					writer.Write(" />");
					PopScope();
				}
				while(scope != null)
				{
					if (scope.indent)
					{
						DoIndent(true);
					}
					writer.Write("</");
					writer.Write(scope.localName);
					writer.Write('>');
					PopScope();
				}
				writeState = System.Xml.WriteState.Start;
			}

	// Write the end of an element and pop the namespace scope.
	public override void WriteEndElement()
			{
				// We must be in the Element or Content state.
				if(writeState == System.Xml.WriteState.Element)
				{
					// Terminate the empty element.
					TerminateElement(true);
				}
				else if(writeState == System.Xml.WriteState.Content)
				{
					// Terminate the element with a full end tag.
					if(scope.indent)
					{
						DoIndent(true);
					}
					writer.Write("</");
					if(scope.scopeShown && scope.prefix != null && scope.prefix != String.Empty)
					{
						writer.Write(scope.prefix);
						writer.Write(':');
					}
					writer.Write(scope.localName);
					writer.Write(">");
				}
				else
				{
					throw new InvalidOperationException
						(S._("Xml_InvalidWriteState"));
				}

				// Pop the current scope.
				PopScope();
				writeState = System.Xml.WriteState.Content;
			}

	// Write an entity reference.
	public override void WriteEntityRef(String name)
			{
				if(!XmlReader.IsNameToken(name))
				{
					throw new ArgumentException
						(S._("Xml_InvalidEntityRef"), "name");
				}
				Sync(WriteStateFlag.ContentFlag |
					 WriteStateFlag.AttributeFlag, false);
				writer.Write("&{0};", name);
			}

	// Write a full end element tag, even if there is no content.
	public override void WriteFullEndElement()
			{
				// We must be in the Element or Content state.
				if(writeState == System.Xml.WriteState.Element)
				{
					// Terminate the start element.
					TerminateElement(false);
				}
				if(writeState == System.Xml.WriteState.Element ||
				   writeState == System.Xml.WriteState.Content)
				{
					// Terminate the element with a full end tag.
					if(scope.indent)
					{
						DoIndent(true);
					}
					writer.Write("</");
					if(scope.scopeShown && scope.prefix != null && scope.prefix != String.Empty)
					{
						writer.Write(scope.prefix);
						writer.Write(':');
					}
					writer.Write(scope.localName);
					writer.Write(">");
				}
				else
				{
					throw new InvalidOperationException
						(S._("Xml_InvalidWriteState"));
				}

				// Pop the current scope.
				PopScope();
				writeState = System.Xml.WriteState.Content;
			}

	// Write a name, as long as it is XML-compliant.
	public override void WriteName(String name)
			{
				if(!XmlReader.IsName(name))
				{
					throw new ArgumentException
						(S._("Xml_InvalidName"), "name");
				}
				Sync(WriteStateFlag.ContentFlag |
					 WriteStateFlag.AttributeFlag, false);
				writer.Write(name);
			}

	// Write a name token, as long as it is XML-compliant.
	public override void WriteNmToken(String name)
			{
				if(!XmlReader.IsNameToken(name))
				{
					throw new ArgumentException
						(S._("Xml_InvalidName"), "name");
				}
				Sync(WriteStateFlag.ContentFlag |
					 WriteStateFlag.AttributeFlag, false);
				writer.Write(name);
			}

	// Write a processing instruction. <?name text?>
	public override void WriteProcessingInstruction(String name, String text)
			{
				// Make sure we have a name.
				if(name == null || name.Length == 0)
				{
					throw new ArgumentException
						(S._("Xml_ArgumentException"), "name");
				}

				// validate and write the xml declaration value
				if(name == "xml")
				{
					WriteXmlDeclaration(text);
					return;
				}

				// Make sure we have valid processing instruction text.
				if(text != null && text.IndexOf("?>") != -1)
				{
					throw new ArgumentException
						(S._("Xml_InvalidXmlWritten"), "text");
				}

				// Synchronize to an area that allows processing instructions.
				Sync(WriteStateFlag.StartFlag |
				     WriteStateFlag.PrologFlag |
				     WriteStateFlag.ContentFlag, true);

				// Write out the processing instruction.
				if(text != null && text.Length != 0)
				{
					writer.Write("<?{0} {1}?>", name, text);
				}
				else
				{
					writer.Write("<?{0}?>",name);
				}
			}

	// Write a qualified name.
	public override void WriteQualifiedName(String localName, String ns)
			{
				if ((localName == null) || (localName == String.Empty))
				{
					throw new ArgumentException
						(S._("Xml_ArgumentException"), "localName");
				}
				
				if  ((Namespaces == false) && (ns != null) 
						&& (ns != String.Empty))
				{
					throw new ArgumentException
						(S._("Xml_ArgumentException"), "ns");
				}
				
				if(!XmlReader.IsName(localName))
				{
					throw new XmlException
						(S._("Xml_XmlException"));
				}

							
				if (writeState == System.Xml.WriteState.Closed)
				{
					throw new InvalidOperationException
						(S._("Xml_InvalidOperation"));
				}

				if (writeState == System.Xml.WriteState.Closed)
				{
						throw new InvalidOperationException
							(S._("Xml_InvalidOperation"));
				}

				if ((writeState == System.Xml.WriteState.Element) || (writeState == System.Xml.WriteState.Attribute))
				{

						if ((ns == null) && (writeState == System.Xml.WriteState.Element))
						{
							throw new ArgumentException
								(S._("Xml_ArgumentException"), "ns");
						}
						
						if (Namespaces == false) 
						{
							if ((ns == null) && (writeState == System.Xml.WriteState.Attribute))
							{
								writer.Write(localName);
							}
						} 
						else if (Namespaces == true) 
						{
							
							try 
							{
								Uri uri = new System.Uri(ns);
								String prefix = LookupPrefix(ns);

								if((Object)prefix == null && prefix == String.Empty)
								{
									throw new ArgumentException
										(S._("Xml_PrefixNotFound"), "ns");
								}
								else
								{

									if (writeState == System.Xml.WriteState.Attribute)
									{
										if (prefix != scope.prefix && prefix != String.Empty)
										{
											writer.Write("{0}:{1}", prefix, localName);
										}
										else
										{
											writer.Write("{0}", localName);
										}
									} 
									else if (writeState == System.Xml.WriteState.Element)
									{
										if((Object)ns == null && ns == String.Empty)
										{
											throw new ArgumentException
												(S._("Xml_NamespaceValueNull"));
										}
										else if (prefix != scope.prefix && prefix != String.Empty)
										{
											writer.Write("{0}:{1}", prefix, localName);
										}
										else if (prefix == scope.prefix)
										{
											writer.Write("{0}", localName);
										}
									}
								}	
							}	
							catch (UriFormatException)
							{
								throw new ArgumentException
									(S._("Xml_InvalidUriFormat"), "ns");
							}
						}
						
				}					
			}

	// Write raw string data.
	public override void WriteRaw(String data)
			{
				if(writeState == System.Xml.WriteState.Closed)
				{
					throw new InvalidOperationException
						(S._("Xml_WriteStateClosed"));
				}
				else
				{
					char[] buffer = data.ToCharArray();
					WriteChars(buffer, 0 , buffer.Length);
				}
			}

	// Write raw data from an array.
	public override void WriteRaw(char[] buffer, int index, int count)
			{
				// Validate the parameters.
				if(buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				else if(index < 0)
				{
					throw new ArgumentOutOfRangeException
						("index", S._("ArgRange_Array"));
				}
				else if(count < 0 || (buffer.Length - index) < count)
				{
					throw new ArgumentOutOfRangeException
						("count", S._("ArgRange_Array"));
				}
				else if(writeState == System.Xml.WriteState.Closed)
				{
					throw new InvalidOperationException
						(S._("Xml_InvalidWriteState"));
				}
				else 
				{
					WriteChars(buffer, index, count);
				}
			}

	// Write the start of an attribute with a full name.
	public override void WriteStartAttribute(String prefix, String localName,
										     String ns)
			{
				// Get the length of the prefix.
				int prefixLen = (((Object)prefix) == null ? 0 : prefix.Length);

				// Get the length of the namespace.
				int nsLen = (((Object)ns) == null ? 0 : ns.Length);

				// Validate the parameters.
				if(!namespaces && (prefixLen != 0 || nsLen != 0))
				{
					throw new ArgumentException
						(S._("Xml_NamespacesNotSupported"));
				}

				// Check the state and output delimiters.
				if(writeState == System.Xml.WriteState.Attribute)
				{
					// Terminate the attribute.
					TerminateAttribute();

					// Write a space before the start of the attribute.
					writer.Write(' ');
				}
				else if(writeState == System.Xml.WriteState.Element)
				{
					writer.Write(' ');
				}
				else if(writeState == System.Xml.WriteState.Closed)
				{
					throw new InvalidOperationException
						(S._("Xml_InvalidWriteState"));
				}

				// Set the special.
				special = Special.None;

				// Output the name of the attribute, with appropriate prefixes.
				if(prefixLen != 0)
				{
					if(prefixLen == 5 && prefix == "xmlns")
					{
						// Ensure the namespace is correct.
						if(nsLen != 0 &&
						   (nsLen != 29 ||
						    ns != XmlDocument.xmlns))
						{
							throw new ArgumentException(/* TODO */);
						}

						// Determine if the name starts with (X|x)(M|m)(L|l).
						bool startsWithXml = localName.ToLower
							(CultureInfo.InvariantCulture).StartsWith("xml");

						// Ensure that the name is valid.
						if(startsWithXml)
						{
							throw new ArgumentException(/* TODO */);
						}

						// Handle the start of the special attribute.
						SpecialAttributeStart
							(prefix, localName, Special.Namespace);

						// We're done here.
						return;
					}
					else if(prefixLen == 3 && prefix == "xml")
					{
						// Ensure the namespace is correct.
						if(nsLen != 0 &&
						   (nsLen != 36 ||
						    ns != XmlDocument.xmlnsXml))
						{
							throw new ArgumentException(/* TODO */);
						}

						// Get the length of the local name.
						int nameLen =
							(((Object)localName) == null ? 0 : localName.Length);

						// Set the special based on the local name.
						if(nameLen == 4 && localName == "lang")
						{
							// Handle the start of the special attribute.
							SpecialAttributeStart
								(prefix, localName, Special.Lang);

							// We're done here.
							return;
						}
						else if(nameLen == 5 && localName == "space")
						{
							// Handle the start of the special attribute.
							SpecialAttributeStart
								(prefix, localName, Special.Space);

							// We're done here.
							return;
						}
					}
					else if(nsLen != 0)
					{
						// Get the current mapping for the namespace.
						String currMapping = LookupPrefix(ns);

						// Ensure the correct mapping is in scope.
						if(currMapping != prefix)
						{
							// Add the mapping to the namespace manager.
							namespaceManager.AddNamespace(prefix, ns);

							// Write the namespace declaration.
							writer.Write("xmlns:");
							writer.Write(prefix);
							writer.Write('=');
							writer.Write(quoteChar);
							WriteQuotedString(ns);
							writer.Write(quoteChar);
							writer.Write(' ');
						}
					}
					else
					{
						// Lookup the namespace for the given prefix.
						ns = namespaceManager.LookupNamespace(prefix);

						// Ensure we have a namespace for the given prefix.
						if(((Object)ns) == null || ns.Length == 0)
						{
							throw new ArgumentException(/* TODO */);
						}
					}

					// Write the prefix.
					writer.Write(prefix);
					writer.Write(':');
				}
				else if(nsLen != 0)
				{
					if(((Object)localName) != null &&
					   (localName.Length == 5 && localName == "xmlns"))
					{
						// Ensure the namespace is correct.
						if(nsLen != 29 || ns != XmlDocument.xmlns)
						{
							throw new ArgumentException(/* TODO */);
						}

						// Handle the start of the special attribute.
						SpecialAttributeStart(null, null, Special.Namespace);

						// We're done here.
						return;
					}
					else if(nsLen == 29 &&
					        ns == XmlDocument.xmlns)
					{
						throw new ArgumentException(/* TODO */);
					}
					else if(nsLen == 36 &&
					        ns == XmlDocument.xmlnsXml)
					{
						throw new ArgumentException(/* TODO */);
					}
					else
					{
						// We were only given a namespace, so find the prefix.
						prefix = LookupPrefix(ns);

						// Ensure we have a prefix.
						if(((Object)prefix) == null || prefix.Length == 0)
						{
							// Generate a prefix.
							prefix = GeneratePrefix(ns);

							// Write the namespace declaration for the prefix.
							writer.Write("xmlns:");
							writer.Write(prefix);
							writer.Write('=');
							writer.Write(quoteChar);
							WriteQuotedString(ns);
							writer.Write(quoteChar);
							writer.Write(' ');
						}

						// Write the prefix.
						writer.Write(prefix);
						writer.Write(':');
					}
				}

				// Write the local name.
				writer.Write(localName);

				// Output the start of the attribute value.
				writer.Write('=');
				writer.Write(quoteChar);

				// We are now in the attribute state.
				writeState = System.Xml.WriteState.Attribute;
			}

	// Write the start of an XML document.
	public override void WriteStartDocument(bool standalone)
			{
				if(writeState != System.Xml.WriteState.Start)
				{
					throw new InvalidOperationException
						(S._("Xml_InvalidWriteState"));
				}
			#if !ECMA_COMPAT
				writer.Write
					("<?xml version=\"1.0\" encoding=\"{0}\" " +
					 "standalone=\"{1}\"?>",
					 writer.Encoding.WebName,
					 (standalone ? "yes" : "no"));
			#else
				writer.Write
					("<?xml version=\"1.0\" standalone=\"{0}\"?>",
					 (standalone ? "yes" : "no"));
			#endif
				writeState = System.Xml.WriteState.Prolog;
			}

	// Write the start of an XML document with no standalone attribute.
	public override void WriteStartDocument()
			{
				if(writeState != System.Xml.WriteState.Start)
				{
					throw new InvalidOperationException
						(S._("Xml_InvalidWriteState"));
				}
			#if !ECMA_COMPAT
				writer.Write("<?xml version=\"1.0\" encoding=\"{0}\"?>",
					 			 writer.Encoding.WebName);
			#else
				writer.Write("<?xml version=\"1.0\"?>");
			#endif
				writeState = System.Xml.WriteState.Prolog;
			}

	// Write the start of an element with a full name.
	public override void WriteStartElement(String prefix, String localName,
										   String ns)
			{
				// Validate the parameters.
				if(!namespaces &&
				   (((Object)prefix) != null || ((Object)ns) != null))
				{
					throw new ArgumentException
						(S._("Xml_NamespacesNotSupported"));
				}

				// We need to be in the Element or Content state.
				Sync(WriteStateFlag.ElementFlag |
					 WriteStateFlag.AttributeFlag |
					 WriteStateFlag.ContentFlag |
					 WriteStateFlag.StartFlag |
					 WriteStateFlag.PrologFlag, true);

				// Get the current scope prefix.
				String currPrefix;
				if(scope != null)
				{
					currPrefix = scope.prefix;
				}
				else
				{
					currPrefix = null;
				}

				// Output the name of the element, with appropriate prefixes.
				bool scopeShown = false;
				writer.Write('<');
				if(((Object)prefix) != null && prefix.Length != 0 &&
				   ((Object)ns) != null && ns.Length != 0)
				{
					// We need to associate a prefix with a namespace.
					String currMapping = LookupPrefix(ns);
					
					if(currMapping == prefix)
					{
						// always write prefix
						writer.Write(prefix);
						writer.Write(':');
						writer.Write(localName);
						scopeShown = true;
					}
					else
					{
						// Create a new pseudo-prefix for the URI.
						writer.Write(prefix);
						writer.Write(':');
						writer.Write(localName);
						writer.Write(' ');
						needPrefixDecl = true;
						scopeShown = true;
					}
				}
				else if(((Object)prefix) != null && prefix.Length != 0)
				{
					// We were only given a prefix, so output it directly.
					if(prefix != currPrefix)
					{
						writer.Write(prefix);
						writer.Write(':');
						scopeShown = true;
					}
					writer.Write(localName);
				}
				else if(((Object)ns) != null && ns.Length != 0)
				{
					// We were only given a namespace, so find the prefix.
					prefix = LookupPrefix(ns);
					if(((Object)prefix) == null || prefix.Length == 0)
					{
						writer.Write(localName);
						scopeShown = true;
					}
					else if(prefix != currPrefix)
					{
						// We know the prefix, but need to specify it.
						writer.Write(prefix);
						writer.Write(':');
						writer.Write(localName);
						scopeShown = true;
					}
					else
					{
						// We have the same prefix as our parent element.
						writer.Write(localName);
					}
				}
				else
				{
					// We only have a name.
					writer.Write(localName);
				}

				// Push a new scope record.
				PushScope(prefix, localName, scopeShown, ns);
				if(scopeShown && ((Object)prefix) != null && prefix.Length != 0)
				{
					namespaceManager.AddNamespace(prefix, ns);
				}

				// We are now in the element state.
				writeState = System.Xml.WriteState.Element;
			}

	// Write a quoted string.
	private void WriteQuotedString(String text)
			{
				int posn, prev, len;
				char ch;
				prev = 0;
				len = text.Length;
				for(posn = 0; posn < len; ++posn)
				{
					ch = text[posn];
					switch(ch)
					{
						case '\x09': case '\x0A': case '\x0D': break;

						case '<':
						{
							if(prev < posn)
							{
								writer.Write(text.Substring(prev, posn - prev));
							}
							writer.Write("&lt;");
							prev = posn + 1;
						}
						break;

						case '>':
						{
							if(prev < posn)
							{
								writer.Write(text.Substring(prev, posn - prev));
							}
							writer.Write("&gt;");
							prev = posn + 1;
						}
						break;

						case '&':
						{
							if(prev < posn)
							{
								writer.Write(text.Substring(prev, posn - prev));
							}
							writer.Write("&amp;");
							prev = posn + 1;
						}
						break;

						case '"':
						{
							if(writeState != System.Xml.WriteState.Attribute)
							{
								break;
							}
							if(quoteChar == '\'')
							{
								// no need to escape this
								break;
							}
							if(prev < posn)
							{
								writer.Write(text.Substring(prev, posn - prev));
							}
							writer.Write("&quot;");
							prev = posn + 1;
						}
						break;

						case '\'':
						{
							if(writeState != System.Xml.WriteState.Attribute)
							{
								break;
							}
							if(quoteChar == '"')
							{
								// no need to escape this
								break;
							}
							if(prev < posn)
							{
								writer.Write(text.Substring(prev, posn - prev));
							}
							writer.Write("&apos;");
							prev = posn + 1;
						}
						break;

						default:
						{
							if(ch < 0x20 && !Char.IsWhiteSpace(ch))
							{
								// Quote a low-ASCII control character.
								if(prev < posn)
								{
									writer.Write
										(text.Substring(prev, posn - prev));
								}
								prev = posn + 1;
								writer.Write("&#");
								writer.Write
									(((int)ch).ToString
									  (null, NumberFormatInfo.InvariantInfo));
								writer.Write(';');
							}
						}
						break;
					}
				}
				if(prev < len)
				{
					writer.Write(text.Substring(prev, len - prev));
				}
			}

	// Write a string.
	public override void WriteString(String text)
			{
				// We must not be in the closed state.
				if(writeState == System.Xml.WriteState.Closed)
				{
					if(((Object)text) != null && text.Length != 0)
					{
						throw new InvalidOperationException
							(S._("Xml_InvalidWriteState"));
					}
					return;
				}

				// If we are in the element state, then shift to content.
				Sync(WriteStateFlag.ContentFlag |
					 WriteStateFlag.AttributeFlag, false);

				// Bail out if the text is empty.
				if(((Object)text) == null || text.Length == 0)
				{
					return;
				}

				// Quote the string and output it.
				WriteQuotedString(text);
			}

	// Write a surrogate character entity.
	public override void WriteSurrogateCharEntity(char lowChar, char highChar)
			{
				if(lowChar < 0xDC00 || lowChar > 0xDFFF)
				{
					throw new ArgumentException
						(S._("Xml_InvalidSurrogate"), "lowChar");
				}
				if(highChar < 0xD800 || highChar > 0xDBFF)
				{
					throw new ArgumentException
						(S._("Xml_InvalidSurrogate"), "highChar");
				}
				Sync(WriteStateFlag.ContentFlag |
					 WriteStateFlag.AttributeFlag, false);
				int ch = 0x10000 + ((highChar - 0xD800) << 10) +
							(lowChar - 0xDC00);
				writer.Write("&#x{0:X5};", ch);
			}

	// Write a sequence of white space.
	public override void WriteWhitespace(String ws)
			{
				if(ws == null || ws == String.Empty)
				{
					throw new ArgumentException
						(S._("Xml_InvalidWhitespace"), "ws");
				}
				foreach(char ch in ws)
				{
					if(!Char.IsWhiteSpace(ch))
					{
						throw new ArgumentException
							(S._("Xml_InvalidWhitespace"), "ws");
					}
				}
				Sync(WriteStateFlag.ContentFlag |
					 WriteStateFlag.AttributeFlag |
					 WriteStateFlag.PrologFlag, false);
				writer.Write(ws);
			}

	// Get the base stream underlying the text writer.
	public Stream BaseStream
			{
				get
				{
					if(writer is StreamWriter)
					{
						return ((StreamWriter)writer).BaseStream;
					}
					else
					{
						return null;
					}
				}
			}

	// Get or set the output formatting mode.
	public Formatting Formatting
			{
				get
				{
					return formatting;
				}
				set
				{
					formatting = value;
				}
			}

	// Get or set the indent character.
	public char IndentChar
			{
				get
				{
					return indentChar;
				}
				set
				{
					indentChar = value;
					for (int i = 0; i < indentation; i++)
					{
						indentChars[i] = indentChar;
					}
				}
			}

	// Get or set the indentation level.
	public int Indentation
			{
				get
				{
					return indentation;
				}
				set
				{
					if(value < 0)
					{
						throw new ArgumentException
							(S._("ArgRange_NonNegative"), "value");
					}
					indentation = value;
					indentChars = new char[indentation];
					for (int i = 0; i < indentation; i++)
					{
						indentChars[i] = indentChar;
					}
				}
			}

	// Get or set the namespace support value.
	public bool Namespaces
			{
				get
				{
					return namespaces;
				}
				set
				{
					if(writeState != System.Xml.WriteState.Start)
					{
						throw new InvalidOperationException
							(S._("Xml_InvalidWriteState"));
					}
					namespaces = value;
				}
			}

	// Get or set the quote character.
	public char QuoteChar
			{
				get
				{
					return quoteChar;
				}
				set
				{
					if(value != '"' && value != '\'')
					{
						throw new ArgumentException
							(S._("Xml_InvalidQuoteChar"), "value");
					}
					quoteChar = value;
				}
			}

	// Get the current write state.
	public override WriteState WriteState
			{
				get
				{
					return writeState;
				}
			}

	// Get the xml:lang attribute.
	public override String XmlLang
			{
				get
				{
					return xmlLang;
				}
			}

	// Get the xml:space attribute.
	public override XmlSpace XmlSpace
			{
				get
				{
					return xmlSpace;
				}
			}

	// Class that keeps track of information for the open element scopes.
	private sealed class ElementScope
	{
		public String prefix;			// The prefix for the element.
		public String localName;		// The name of the element.
		public bool scopeShown;			// True if we output the prefix.
		public int indentLevel;			// Indent level for next higher scope.
		public XmlSpace xmlSpace;		// XmlSpace for next higher scope.
		public String xmlLang;			// XmlLang for next higher scope.
		public ElementScope next;		// The next higher scope.
		public String xmlns;			// Xmlnamespace in scope.
		public bool indent;				// flag that surrounding element needs DoIndent on end
		public int genPrefixCount;      // The count of generated prefixes.
		public ElementScope(ElementScope next)
				{
					this.next = next;
				}

	}; // class ElementScope

	// Class that keeps track of special attribute values, on output, as needed.
	private sealed class SpecialWriter : TextWriter
	{
		// Internal state.
		private StringBuilder builder;
		private TextWriter    writer;


		// Constructor.
		public SpecialWriter(TextWriter writer)
				: base()
				{
					this.builder = new StringBuilder();
					this.writer = writer;
				}


		// Get the attribute value.
		public String AttributeValue
				{
					get { return builder.ToString(); }
				}

		// Get the attribute value builder.
		public StringBuilder Builder
				{
					get { return builder; }
				}

		// Get the encoding for this writer.
		public override Encoding Encoding
				{
					get
					{
						if(writer == null) { return null; }
						return writer.Encoding;
					}
				}

		// Get the text writer.
		public TextWriter Writer
				{
					get { return writer; }
				}


		// Close this writer.
		public override void Close()
				{
					Dispose(false);
				}

		// Dispose of this writer.
		protected override void Dispose(bool disposing)
				{
					if(writer != null)
					{
						if(disposing)
						{
							((IDisposable)writer).Dispose();
						}
						else
						{
							writer.Close();
						}
						builder = null;
						writer = null;
					}
				}

		// Reset the attribute value builder.
		public void ResetBuilder()
				{
					builder.Length = 0;
				}

		// Write values to this writer.
		public override void Write(String value)
				{
					if(((Object)value) != null)
					{
						writer.Write(value);
						builder.Append(value);
					}
				}
		public override void Write(char[] buffer, int index, int count)
				{
					if(count > 0)
					{
						writer.Write(buffer, index, count);
						builder.Append(buffer, index, count);
					}
				}
		public override void Write(char ch)
				{
					writer.Write(ch);
					builder.Append(ch);
				}

	}; // class SpecialWriter

}; // class XmlTextWriter

}; // namespace System.Xml
