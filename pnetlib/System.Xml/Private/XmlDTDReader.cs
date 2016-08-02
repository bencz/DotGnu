/*
 * XmlDTDReader.cs - Implementation of the
 *			"System.Xml.Private.XmlDTDReader" class.
 *
 * Copyright (C) 2004  Free Software Foundation, Inc.
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

using System;
using System.Text;

internal sealed class XmlDTDReader : XmlErrorProcessor
{
	// Internals.
	private XmlParserContext context;
	private XmlDTDParserInput input;
	private XmlResolver resolver;


	// Constructor.
	public XmlDTDReader(XmlParserContext context)
			: base(null)
			{
				this.context = context;
				this.input = null;
				this.resolver = null;
			}


#if !ECMA_COMPAT
	//
	// NOTE: Any dtd rule handling should be done via objects accessible
	//       through parser context properties.
	//
	// Get the valid flag, which is set iff no invalid pe references were found.
	public bool Valid
			{
				get { return input.Valid; }
			}
#endif

	// Get or set the parser context.
	public XmlParserContext Context
			{
				get { return context; }
				set { context = value; }
			}


	// Initialize this dtd reader.
	public void Init
				(XmlParserInput input,
				 XmlResolver resolver)
			{
				base.ErrorHandler = input.ErrorHandler;
				this.input = new XmlDTDParserInput(input, context.NameTable);
				this.resolver = resolver;
			}

	// Read a doctype declaration.
	//
	// Already read: '<!DOCTYPE'
	public void Read()
			{
				// turn off pe scanning
				input.ScanForPE = false;

				// require whitespace, then read the name
				if(!input.SkipWhitespace()) { Error(/* TODO */); }
				context.DocTypeName = input.ReadName();

				// value of internal subset defaults to empty
				context.InternalSubset = String.Empty;

				// values of the public and system ids default to empty
				context.PublicId = String.Empty;
				context.SystemId = String.Empty;

				// check for an external id
				bool hasWS = input.SkipWhitespace();
				if(!input.PeekChar()) { Error("Xml_UnexpectedEOF"); }
				if(input.peekChar == 'S' || input.peekChar == 'P')
				{
					// external id must be preceded by whitespace
					if(!hasWS) { Error(/* TODO */); }

					// allow only an external id, not a public id
					ReadExternalOrPublicID(false, true);

					// skip potentially optional whitespace
					hasWS = input.SkipWhitespace();

					// get the peekChar ready for internal subset check
					if(!input.PeekChar()) { Error("Xml_UnexpectedEOF"); }
				}

				// check for an internal subset
				if(input.peekChar == '[')
				{
					// internal subset must be preceded by whitespace
					if(!hasWS) { Error(/* TODO */); }

					// move input to '['
					input.NextChar();

					// push the internal subset log onto the log stack
					input.Logger.Push(new StringBuilder());

					// read the internal subset
					ReadInternalSubset();

					// get the internal subset from the log and pop it from the logger
					context.InternalSubset = input.Logger.Pop().ToString();

					// skip optional whitespace
					input.SkipWhitespace();
				}

				// the dtd must end with '>' at this point
				input.Expect('>');
			}

	// Read attribute definitions.
	//
	// Already read: ''
	[TODO]
	private void ReadAttributeDefinitions()
			{
				// TODO: add support for a dtd rule handling object

				// read an arbitrary number of attribute definitions
				while(true)
				{
					// skip potentially optional whitespace
					bool hasWS = input.SkipWhitespace();

					// check for the end of the attribute list
					if(!input.PeekChar()) { Error("Xml_UnexpectedEOF"); }
					if(input.peekChar == '>') { return; }

					// the attribute name must be preceded by whitespace
					if(!hasWS) { Error(/* TODO */); }

					// read the attribute name
					input.ReadName();

					// the attribute type must be preceded by whitespace
					if(!input.SkipWhitespace()) { Error(/* TODO */); }

					// read the attribute type
					ReadAttributeType();

					// the default declaration must be preceded by whitespace
					if(!input.SkipWhitespace()) { Error(/* TODO */); }

					// read the default declaration
					ReadDefaultDeclaration();
				}
			}

	// Read an attribute list declaration.
	//
	// Already read: '<!ATTLIST'
	[TODO]
	private void ReadAttributeListDeclaration()
			{
				// TODO: add support for a dtd rule handling object

				// the element name must be preceded by whitespace
				if(!input.SkipWhitespace()) { Error(/* TODO */); }

				// read the element name
				input.ReadName();

				// read the attribute definitions
				ReadAttributeDefinitions();

			#if !ECMA_COMPAT
				// part of validity checks
				input.EndTag();
			#endif

				// the attribute list must end with '>' at this point
				input.Expect('>');
			}

	// Read an attribute type.
	//
	// Already read: ''
	[TODO]
	private void ReadAttributeType()
			{
				// TODO: add support for a dtd rule handling object

				// handle all the possible attribute types
				if(!input.NextChar()) { Error("Xml_UnexpectedEOF"); }
				switch(input.currChar)
				{
					// handle CDATA case
					case 'C':
					{
						// TODO: handle CDATA case here
						input.Expect("DATA");
					}
					break;

					// handle ID, IDREF, and IDREFS cases
					case 'I':
					{
						input.Expect('D');
						if(!input.PeekChar()) { Error("Xml_UnexpectedEOF"); }
						if(input.peekChar == 'R')
						{
							input.Expect("REF");
							if(!input.PeekChar()) { Error("Xml_UnexpectedEOF"); }
							if(input.peekChar == 'S')
							{
								// TODO: handle IDREFS case here
								input.NextChar();
							}
						#if !ECMA_COMPAT
							else
							{
								// TODO: handle IDREF case here
							}
						#endif
						}
					#if !ECMA_COMPAT
						else
						{
							// TODO: handle ID case here
						}
					#endif
					}
					break;

					// handle ENTITY and ENTITIES cases
					case 'E':
					{
						input.Expect("NTIT");
						if(!input.PeekChar()) { Error("Xml_UnexpectedEOF"); }
						if(input.peekChar == 'Y')
						{
							// TODO: handle ENTITY case here
							input.NextChar();
						}
						else
						{
							// TODO: handle ENTITIES case here
							input.Expect("IES");
						}
					}
					break;

					// handle NMTOKEN, NMTOKENS, and NOTATION cases
					case 'N':
					{
						if(!input.PeekChar()) { Error("Xml_UnexpectedEOF"); }
						if(input.peekChar == 'M')
						{
							input.Expect("MTOKEN");
							if(!input.PeekChar()) { Error("Xml_UnexpectedEOF"); }
							if(input.peekChar == 'S')
							{
								// TODO: handle NMTOKENS case here
								input.NextChar();
							}
						#if !ECMA_COMPAT
							else
							{
								// TODO: handle NMTOKEN case here
							}
						#endif
						}
						else
						{
							// TODO: handle NOTATION case here
							input.Expect("OTATION");

							// enumeration must be preceded by whitespace
							if(!input.SkipWhitespace()) { Error(/* TODO */); }

							// the enumeration starts with a '(' character
							input.Expect('(');

							// skip optional whitespace and read the first name
							input.SkipWhitespace();
							input.ReadName();
							input.SkipWhitespace();

							// read additional names
							while(input.NextChar() && input.currChar == '|')
							{
								// skip optional whitespace and read a name
								input.SkipWhitespace();
								input.ReadName();
								input.SkipWhitespace();
							}

							// the enumeration must end with ')' at this point
							if(input.currChar != ')') { Error(/* TODO */); }
						}
					}
					break;

					// handle enumeration case
					default:
					{
						// TODO: handle enumeration case here

						// the enumeration starts with a '(' character
						input.Expect('(');

						// skip optional whitespace and read the first name token
						input.SkipWhitespace();
						input.ReadNameToken();
						input.SkipWhitespace();

						// read additional name tokens
						while(input.NextChar() && input.currChar == '|')
						{
							// skip optional whitespace and read a name token
							input.SkipWhitespace();
							input.ReadNameToken();
							input.SkipWhitespace();
						}

						// the enumeration must end with ')' at this point
						if(input.currChar != ')') { Error(/* TODO */); }
					}
					break;
				}
			}

	// Read an attribute value literal.
	//
	// Already read: ''
	[TODO]
	private void ReadAttributeValue()
			{
				// TODO: add support for a dtd rule handling object

				// scan for a valid quote character
				if(!input.NextChar()) { Error("Xml_UnexpectedEOF"); }
				char quoteChar = input.currChar;
				if(quoteChar != '"' && quoteChar != '\'')
				{
					Error(/* TODO */);
				}

				// pe scanning on string literals isn't handled inline
				input.ScanForPE = false;

				// read until we hit the quote character
				while(input.NextChar() && input.currChar != quoteChar)
				{
					// TODO: do something with what we read here
				}

				// turn inline pe scanning back on
				input.ScanForPE = true;

				// we hit eof, otherwise we'd have quoteChar, so give an error
				if(input.currChar != quoteChar) { Error("Xml_UnexpectedEOF"); }
			}

	// Read a choice or sequence.
	//
	// Already read: '('
	[TODO]
	private void ReadChoiceOrSequence()
			{
				// TODO: add support for a dtd rule handling object

				// skip optional whitespace and read the first content particle
				input.SkipWhitespace();
				ReadContentParticle();
				input.SkipWhitespace();

				// scan for a separator or finish with the ')' character
				if(!input.NextChar()) { Error("Xml_UnexpectedEOF"); }
				char separator = (char)0;
				if(input.currChar == ')')
				{
					return;
				}
				else if(input.currChar == '|' || input.currChar == ',')
				{
					// store the separator
					separator = input.currChar;

					// skip optional whitespace and read the second cp
					input.SkipWhitespace();
					ReadContentParticle();
					input.SkipWhitespace();
				}
				else
				{
					Error(/* TODO */);
					// Shouldnt reach here.
					separator = (char)0;
				}

				// read until we've consumed all the content particles
				while(input.NextChar() && input.currChar == separator)
				{
					// skip optional whitespace and read a content particle
					input.SkipWhitespace();
					ReadContentParticle();
					input.SkipWhitespace();
				}

				// the choice or sequence must end with ')' at this point
				if(input.currChar != ')') { Error(/* TODO */); }
			}

	// Read a comment.
	//
	// Already read: '<!--'
	[TODO]
	private void ReadComment()
			{
				// turn pe scanning off inside comments
				input.ScanForPE = false;

				// read until we hit '-->'
				while(input.NextChar())
				{
					if(input.currChar == '-')
					{
						if(!input.NextChar()) { Error("Xml_UnexpectedEOF"); }
						if(input.currChar == '-')
						{
						#if !ECMA_COMPAT
							// part of validity checks
							input.EndTag();
						#endif
							input.Expect('>');

							// turn pe scanning back on
							input.ScanForPE = true;

							return;
						}
					}
				}
				Error("Xml_UnexpectedEOF");
			}

	// Read a content particle.
	//
	// Already read: ''
	[TODO]
	private void ReadContentParticle()
			{
				// TODO: add support for a dtd rule handling object

				// check for a choice or sequence, otherwise expect a name
				if(!input.PeekChar()) { Error("Xml_UnexpectedEOF"); }
				if(input.peekChar == '(')
				{
					input.NextChar();
					ReadChoiceOrSequence();
				}
				else
				{
					input.ReadName();
				}

				// check for optional occurance modifiers
				if(!input.PeekChar()) { Error("Xml_UnexpectedEOF"); }
				switch(input.peekChar)
				{
					case '?':
					case '*':
					case '+':
					{
						input.NextChar();
					}
					break;
				}
			}

	// Read a content specification.
	//
	// Already read: ''
	[TODO]
	private void ReadContentSpecification()
			{
				// TODO: add support for a dtd rule handling object

				// handle all the possible content specs
				if(!input.PeekChar()) { Error("Xml_UnexpectedEOF"); }
				switch(input.peekChar)
				{
					// handle EMPTY case
					case 'E':
					{
						// TODO: handle EMPTY case here
						input.Expect("EMPTY");
					}
					break;

					// handle ANY case
					case 'A':
					{
						// TODO: handle ANY case here
						input.Expect("ANY");
					}
					break;

					// handle Mixed and children cases
					default:
					{
						// both cases must start with a '(' character
						input.Expect('(');

						// skip optional whitespace
						input.SkipWhitespace();

						// check for Mixed or children
						if(!input.PeekChar()) { Error("Xml_UnexpectedEOF"); }
						if(input.peekChar == '#')
						{
							// TODO: handle Mixed case here

							// require '#PCDATA' at this point
							input.Expect("#PCDATA");

							// skip optional whitespace
							input.SkipWhitespace();

							// check for the finishing ')' character
							if(!input.PeekChar()) { Error("Xml_UnexpectedEOF"); }
							if(input.peekChar == ')')
							{
								input.NextChar();
								return;
							}

							// read until we've consumed all the names
							while(input.NextChar() && input.currChar == '|')
							{
								// skip optional whitespace and read a name
								input.SkipWhitespace();
								input.ReadName();
								input.SkipWhitespace();
							}

							// the Mixed case must end with ')*' at this point
							if(input.currChar != ')') { Error(/* TODO */); }
							input.Expect('*');
						}
						else
						{
							// TODO: handle children case here

							// read a choice or sequence
							ReadChoiceOrSequence();

							// read optional occurance modifiers
							if(!input.PeekChar()) { Error("Xml_UnexpectedEOF"); }
							switch(input.peekChar)
							{
								case '?':
								case '*':
								case '+':
								{
									input.NextChar();
								}
								break;
							}
						}
					}
					break;
				}
			}

	// Read a default declaration.
	//
	// Already read: ''
	[TODO]
	private void ReadDefaultDeclaration()
			{
				// TODO: add support for a dtd rule handling object

				// handle all the possible default declarations
				if(!input.PeekChar()) { Error("Xml_UnexpectedEOF"); }
				if(input.peekChar == '#')
				{
					input.NextChar();
					if(!input.PeekChar()) { Error("Xml_UnexpectedEOF"); }
					switch(input.peekChar)
					{
						// handle #REQUIRED case
						case 'R':
						{
							// TODO: handle #REQUIRED case here
							input.Expect("REQUIRED");
						}
						break;

						// handle #IMPLIED case
						case 'I':
						{
							// TODO: handle #IMPLIED case here
							input.Expect("IMPLIED");
						}
						break;

						// handle #FIXED case
						default:
						{
							// TODO: handle #FIXED case here
							input.Expect("FIXED");

							// require whitespace followed by an attribute value
							if(!input.SkipWhitespace()) { Error(/* TODO */); }
							ReadAttributeValue();
						}
						break;
					}
				}
				else
				{
					// TODO: handle attribute value case here
					ReadAttributeValue();
				}
			}

	// Read an element declaration.
	//
	// Already read: '<!ELEMENT'
	[TODO]
	private void ReadElementDeclaration()
			{
				// TODO: add support for a dtd rule handling object

				// require whitespace, then read the name
				if(!input.SkipWhitespace()) { Error(/* TODO */); }
				String name = input.ReadName();

				// require whitespace, then read the content specification
				if(!input.SkipWhitespace()) { Error(/* TODO */); }
				ReadContentSpecification();

				// skip optional whitespace
				input.SkipWhitespace();

			#if !ECMA_COMPAT
				// part of validity checks
				input.EndTag();
			#endif

				// the element declaration must end with '>' at this point
				input.Expect('>');
			}

	// Read an entity declaration.
	//
	// Already read: '<!ENTITY'
	[TODO]
	private void ReadEntityDeclaration()
			{
				// TODO: add support for a dtd rule handling object

				// skip required whitespace
				if(!input.SkipWhitespace()) { Error(/* TODO */); }

				// check for parameter or general entity
				if(!input.PeekChar()) { Error("Xml_UnexpectedEOF"); }
				if(input.peekChar == '%')
				{
					input.NextChar();

					// skip required whitespace and read the entity name
					if(!input.SkipWhitespace()) { Error(/* TODO */); }
					String name = input.ReadName();
					if(!input.SkipWhitespace()) { Error(/* TODO */); }

					// read the entity definition
					String value = ReadEntityDefinition(true);

					// add the pe to the pe table
					if(value != null)
					{
						input.ParameterEntities[name] = value;
					}
				}
				else
				{
					// read the entity name
					input.ReadName();

					// skip required whitespace
					if(!input.SkipWhitespace()) { Error(/* TODO */); }

					// read the entity definition
					ReadEntityDefinition(false);
				}

				// skip optional whitespace
				input.SkipWhitespace();

			#if !ECMA_COMPAT
				// part of validity checks
				input.EndTag();
			#endif

				// the entity declaration must end with '>' at this point
				input.Expect('>');
			}

	// Read a parameter or general entity definition.
	//
	// Already read: ''
	[TODO]
	private String ReadEntityDefinition(bool parameter)
			{
				// TODO: add support for a dtd rule handling object

				// check for an entity value or external id
				if(!input.PeekChar()) { Error("Xml_UnexpectedEOF"); }
				char quoteChar = input.peekChar;
				if(quoteChar != '"' && quoteChar != '\'')
				{
					// read external id
					ReadExternalOrPublicID(false);

					// read optional notation data declaration
					if(!parameter)
					{
						// skip potentially optional whitespace
						bool hasWS = input.SkipWhitespace();

						// check for and read the notation data declaration
						if(!input.PeekChar()) { Error("Xml_UnexpectedEOF"); }
						if(input.peekChar == 'N')
						{
							// the declaration must be preceded by whitespace
							if(!hasWS) { Error(/* TODO */); }

							// require 'NDATA' at this point
							input.Expect("NDATA");

							// skip required whitespace
							if(!input.SkipWhitespace()) { Error(/* TODO */); }

							// read the name
							input.ReadName();
						}
					}
					return null;
				}
				else
				{
					// move to quote character
					input.NextChar();

					// pe scanning on string literals isn't handled inline
					input.ScanForPE = false;

					// create our log and push it onto the logger's log stack
					StringBuilder log = new StringBuilder();
					input.Logger.Push(log);

					// read until we hit the quote character
					while(input.PeekChar() && input.peekChar != quoteChar)
					{
						input.NextChar();
					}

					// pop the log from the log stack
					input.Logger.Pop();

					// the entity value must be properly terminated
					input.Expect(quoteChar);

					// turn inline pe scanning back on
					input.ScanForPE = true;

					// return the entity value contents
					return log.ToString();
				}
			}

	// Read an external or public id.
	//
	// Already read: ''
	private void ReadExternalOrPublicID(bool allowPub)
			{
				ReadExternalOrPublicID(allowPub, false);
			}
	[TODO]
	private void ReadExternalOrPublicID(bool allowPub, bool setContext)
			{
				// TODO: load external subsets and parse them...
				//       remember not to log them in the internal subset log

				// handle 'SYSTEM' or 'PUBLIC'
				if(!input.NextChar()) { Error("Xml_UnexpectedEOF"); }
				if(input.currChar == 'S')
				{
					// require that the input match 'SYSTEM'
					input.Expect("YSTEM");

					// require whitespace
					if(!input.SkipWhitespace()) { Error(/* TODO */); }

					// scan for a valid quote character
					if(!input.NextChar()) { Error("Xml_UnexpectedEOF"); }
					char quoteChar = input.currChar;
					if(quoteChar != '"' && quoteChar != '\'')
					{
						Error(/* TODO */);
					}

					// pe scanning on string literals isn't handled inline
					input.ScanForPE = false;

					// create our log and push it onto the log stack
					StringBuilder log = new StringBuilder();
					input.Logger.Push(log);

					// read until we hit the quote character
					while(input.NextChar() && input.currChar != quoteChar) {}

					// we hit eof, otherwise we'd have quoteChar, so give an error
					if(input.currChar != quoteChar) { Error("Xml_UnexpectedEOF"); }

					// pop the log from the log stack
					input.Logger.Pop();

					// set the system id
					if(setContext) { context.SystemId = log.ToString(); }

					// turn inline pe scanning back on
					input.ScanForPE = true;
				}
				else if(input.currChar == 'P')
				{
					// require that the input match 'PUBLIC'
					input.Expect("UBLIC");

					// require whitespace
					if(!input.SkipWhitespace()) { Error(/* TODO */); }

					// scan for a valid quote character
					if(!input.NextChar()) { Error("Xml_UnexpectedEOF"); }
					char quoteChar = input.currChar;
					if(quoteChar != '"' && quoteChar != '\'')
					{
						Error(/* TODO */);
					}

					// pe scanning on string literals isn't handled inline
					input.ScanForPE = false;

					// create our log and push it onto the log stack
					StringBuilder log = new StringBuilder();
					input.Logger.Push(log);

					// read until we hit the quote character
					while(input.NextChar() && input.currChar != quoteChar)
					{
						// ensure we get valid public literal characters
						if(!XmlCharInfo.IsPublicId(input.currChar))
						{
							Error(/* TODO */);
						}
					}

					// we hit eof, otherwise we'd have quoteChar, so give an error
					if(input.currChar != quoteChar) { Error("Xml_UnexpectedEOF"); }

					// pop the log from the log stack
					input.Logger.Pop();

					// set the system id
					if(setContext) { context.PublicId = log.ToString(); }

					// reset the log
					log.Length = 0;

					// turn inline pe scanning back on
					input.ScanForPE = true;

					// skip potentially optional whitespace
					bool hasWS = input.SkipWhitespace();

					// scan for a valid quote character
					if(!input.PeekChar()) { Error("Xml_UnexpectedEOF"); }
					quoteChar = input.peekChar;
					if(quoteChar != '"' && quoteChar != '\'')
					{
						// this is permitted to end here only for public ids
						if(allowPub) { return; }
						Error(/* TODO */);
					}
					input.NextChar();

					// the system literal must be preceded by whitespace
					if(!hasWS) { Error(/* TODO */); }

					// pe scanning on string literals isn't handled inline
					input.ScanForPE = false;

					// push the log onto the log stack
					input.Logger.Push(log);

					// read until we hit the quote character
					while(input.NextChar() && input.currChar != quoteChar) {}

					// we hit eof, otherwise we'd have quoteChar, so give an error
					if(input.currChar != quoteChar) { Error("Xml_UnexpectedEOF"); }

					// pop the log from the log stack
					input.Logger.Pop();

					// set the system id
					if(setContext) { context.SystemId = log.ToString(); }

					// turn inline pe scanning back on
					input.ScanForPE = true;
				}
				else
				{
					// we didn't see 'SYSTEM' or 'PUBLIC' so give an error
					Error(/* TODO */);
				}
			}

	// Read the internal subset of a dtd.
	//
	// Already read: '['
	[TODO]
	private void ReadInternalSubset()
			{
				// turn on pe scanning
				input.ScanForPE = true;

				// read until we consume all of the internal subset
				while(input.PeekChar() && input.peekChar != ']')
				{
					// read a tag or skip whitespace
					if(input.peekChar == '<')
					{
						input.NextChar();

						// read a qmark or emark tag
						if(!input.NextChar()) { Error("Xml_UnexpectedEOF"); }
						if(input.currChar == '!')
						{
							// read a declaration or comment
							if(!input.NextChar()) { Error("Xml_UnexpectedEOF"); }
							switch(input.currChar)
							{
								// handle ELEMENT and ENTITY cases
								case 'E':
								{
									if(!input.NextChar()) { Error("Xml_UnexpectedEOF"); }
									if(input.currChar == 'L')
									{
										input.Expect("EMENT");
										ReadElementDeclaration();
									}
									else if(input.currChar == 'N')
									{
										input.Expect("TITY");
										ReadEntityDeclaration();
									}
									else
									{
										Error(/* TODO */);
									}
								}
								break;

								// handle ATTLIST case
								case 'A':
								{
									input.Expect("TTLIST");
									ReadAttributeListDeclaration();
								}
								break;

								// handle NOTATION case
								case 'N':
								{
									input.Expect("OTATION");
									ReadNotationDeclaration();
								}
								break;

								// handle comment case
								case '-':
								{
									input.Expect('-');
									ReadComment();
								}
								break;

								// handle unknown case
								default:
								{
									Error(/* TODO */);
								}
								break;
							}
						}
						else if(input.currChar == '?')
						{
							// read a processing instruction
							ReadProcessingInstruction();
						}
						else
						{
							Error(/* TODO */);
						}
					}
					else if(!input.SkipWhitespace())
					{
						Error(/* TODO */);
					}
				}

				// turn off pe scanning
				input.ScanForPE = false;

				// ensure that the ending ']' is not a part of a pe
				input.ResetPE();

				// the internal subset must end with ']' at this point
				StringBuilder log = input.Logger.Pop();
				input.Expect(']');
				input.Logger.Push(log);
			}

	// Read a notation declaration.
	//
	// Already read: '<!NOTATION'
	[TODO]
	private void ReadNotationDeclaration()
			{
				// TODO: add support for a dtd rule handling object

				// skip required whitespace and read the name
				if(!input.SkipWhitespace()) { Error(/* TODO */); }
				input.ReadName();
				if(!input.SkipWhitespace()) { Error(/* TODO */); }

				// read the external id
				ReadExternalOrPublicID(false);

				// skip optional whitespace
				input.SkipWhitespace();

			#if !ECMA_COMPAT
				// part of validity checks
				input.EndTag();
			#endif

				// the notation declaration must end with '>' at this point
				input.Expect('>');
			}

	// Read a processing instruction.
	//
	// Already read: '<?'
	[TODO]
	private void ReadProcessingInstruction()
			{
				// read the target name
				// TODO: check target for ('X'|'x')('M'|'m')('L'|'l')
				input.ReadName();

				// turn off pe scanning
				input.ScanForPE = false;

				// skip potentially optional whitespace
				bool hasWS = input.SkipWhitespace();

				// check for the closing characters
				if(!input.NextChar()) { Error("Xml_UnexpectedEOF"); }
				if(!input.PeekChar()) { Error("Xml_UnexpectedEOF"); }
				if(input.currChar == '?' && input.peekChar == '>')
				{
				#if !ECMA_COMPAT
					// part of validity checks
					input.EndTag();
				#endif

					input.NextChar();
					return;
				}

				// pi content must be preceded by whitespace
				if(!hasWS) { Error(/* TODO */); }

				// read until we consume all of the pi content
				while(input.NextChar() && input.PeekChar())
				{
					if(input.currChar == '?' && input.peekChar == '>')
					{
					#if !ECMA_COMPAT
						// part of validity checks
						input.EndTag();
					#endif

						// turn on pe scanning
						input.ScanForPE = true;

						input.NextChar();
						return;
					}
				}

				// if we make it this far, we hit eof
				Error("Xml_UnexpectedEOF");
			}

}; // class XmlDTDReader

}; // namespace System.Xml.Private
