/*
 * XmlDTDParserInput.cs - Implementation of the
 *			"System.Xml.Private.XmlDTDParserInput" class.
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
using System.IO;
using System.Text;
using System.Collections;

internal class XmlDTDParserInput : XmlParserInputBase
{
	// Internals.
#if !ECMA_COMPAT
	private bool valid;
	private bool started;
#endif
	private bool scanForPE;
	private int pePosition;
	private String peValue;
	private PEManager parameterEntities;
	private XmlParserInput input;


	// Constructor.
	public XmlDTDParserInput
				(XmlParserInput input,
				 XmlNameTable nameTable)
			: base(nameTable, null, input.ErrorHandler)
			{
				this.input = input;
				this.peValue = null;
				this.pePosition = -1;
				this.scanForPE = true;
				this.parameterEntities = new PEManager
					(nameTable, input.ErrorHandler);
			#if !ECMA_COMPAT
				this.valid = true;
				this.started = false;
			#endif
				base.logger = null;
			}


	// Get the current line number.
	public override int LineNumber
			{
				get { return input.LineNumber; }
			}

	// Get the current line position.
	public override int LinePosition
			{
				get { return input.LinePosition; }
			}

	// Get the logger.
	public override LogManager Logger
			{
				get { return input.Logger; }
			}

	// Get or set the list of parameter entities.
	public PEManager ParameterEntities
			{
				get { return parameterEntities; }
				set { parameterEntities = value; }
			}

	// Turn on/off pe scanning.
	public bool ScanForPE
			{
				get { return scanForPE; }
				set { scanForPE = value; }
			}

#if !ECMA_COMPAT
	// Valid DTDs require that all tags started in a pe, end in that pe.

	// Get the valid flag, which is set iff no invalid pe references were found.
	public bool Valid
			{
				get { return valid; }
			}


	//
	public void StartTag()
			{
				started = (peValue != null);
			}

	// 
	public void EndTag()
			{
				if(peValue != null)
				{
					if(!started)
					{
						valid = false;
					}
				}
				else if(started)
				{
					valid = false;
				}
				started = false;
			}
#endif

	// Move to the next character, returning false on EOF.
	public override bool NextChar()
			{
				bool retval;
				if(peValue == null)
				{
					retval = input.NextChar();
					currChar = input.currChar;
					if(!scanForPE) { return retval; }
					if(currChar == '%')
					{
						if(input.PeekChar() &&
						   XmlCharInfo.IsNameInit(input.peekChar))
						{
							String name = input.ReadName();
							input.Expect(';');
							peValue = parameterEntities[name];
							pePosition = 0;
							currChar = ' ';
						}
					}
				}
				else
				{
					retval = true;
					if(pePosition == -1)
					{
						pePosition++;
						currChar = ' ';
					}
					else if(pePosition < peValue.Length)
					{
						currChar = peValue[pePosition++];
					}
					else
					{
						pePosition = -1;
						peValue = null;
						currChar = ' ';
					}
				}
				return retval;
			}

	// Peek at the next character, returning false on EOF.
	public override bool PeekChar()
			{
				bool retval;
				if(peValue == null)
				{
					retval = input.PeekChar();
					peekChar = input.peekChar;
					if(!scanForPE) { return retval; }
					if(peekChar == '%')
					{
						if(input.ExtraPeekChar() &&
						   XmlCharInfo.IsNameInit(input.extraPeekChar))
						{
							input.NextChar();
							String name = input.ReadName();
							input.Expect(';');
							peValue = parameterEntities[name];
							pePosition = -1;
							peekChar = ' ';
						}
					}
				}
				else
				{
					retval = true;
					if(pePosition != -1 && pePosition < peValue.Length)
					{
						peekChar = peValue[pePosition];
					}
					else
					{
						peekChar = ' ';
					}
				}
				return retval;
			}

	// Exit the current pe, if any.
	public void ResetPE()
			{
				pePosition = -1;
				peValue = null;
			}




	// this is just a temporary hack until we come up
	// with something better for entity handling
	public sealed class PEManager : XmlErrorProcessor
	{
		// Internal state.
		private Hashtable table;
		private XmlParserInput input;


		// Constructor.
		public PEManager(XmlNameTable nameTable, ErrorHandler error)
				: base(error)
				{
					table = new Hashtable();
					input = new XmlParserInput(null, nameTable, error);
				}


		// Get or set the value for the given name.
		public String this[String name]
				{
					get
					{
						Object obj = table[name];
						if(obj == null) { return null; }
						PEValue pe = (PEValue)obj;
						if(!pe.expanded)
						{
							pe.value = Expand(name, pe.value);
							pe.expanded = true;
							table[name] = pe;
						}
						return pe.value;
					}
					set { table[name] = new PEValue(Expand(name, value)); }
				}


		// Check if the given entity is contained in the table.
		public bool Contains(String name)
				{
					return table.Contains(name);
				}

		// Expand the pe reference.
		private String Expand(String rootName, String rawValue)
				{
					// set up our reader
					input.Reader = new StringReader(rawValue);

					// create our log and push it onto the logger's log stack
					StringBuilder log = new StringBuilder();
					input.Logger.Push(log);

					// read until we consume the entire value
					while(input.PeekChar())
					{
						if(input.peekChar == '&')
						{
							int position = log.Length;
							char c;

							// move to the '&' character
							input.NextChar();

							// read the reference
							if(ReadCharacterReference(out c))
							{
								log.Length = position;
								log.Append(c);
							}
						}
						else if(input.peekChar == '%')
						{
							// pop the log while reading the pe reference
							input.Logger.Pop();

							// move to the '%' character
							input.NextChar();

							// read the pe reference name
							String name = input.ReadName();

							// give an error on recursion
							if(name == rootName) { Error(/* TODO */); }

							// the pe reference must end with ';' at this point
							input.Expect(';');

							// get the value for the entity
							String value = this[name];

							// check to make sure the entity has been defined
							if(value == null) { Error(/* TODO */); }

							// append the replacement text to the log
							log.Append(value);

							// push the log back onto the log stack
							input.Logger.Push(log);
						}
						else
						{
							input.NextChar();
						}
					}

					// close the reader
					input.Close();

					// pop the log from the log stack, and return the value
					return input.Logger.Pop().ToString();
				}

		// Read a character reference, returning false for general entity references.
		//
		// Already read: '&'
		private bool ReadCharacterReference(out char value)
				{
					// check for an empty reference
					if(!input.PeekChar()) { Error(/* TODO */); }
					if(input.peekChar == ';') { Error(/* TODO */); }

					// set the defaults
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
						input.ReadName();

						// the reference must end with ';' at this point
						input.Expect(';');

						// signal that a general entity reference was encountered
						return false;
					}
				}





		private struct PEValue
		{
			// Internal state.
			public bool expanded;
			public String value;

			// Constructor.
			public PEValue(String value)
					{
						this.expanded = false;
						this.value = value;
					}

		}; // struct PEValue


	}; // class PEManager

}; // class XmlDTDParserInput

}; // namespace System.Xml.Private
