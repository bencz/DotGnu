/*
 * XmlFragmentTextWriter.cs - Implementation of the
 *		"System.Xml.Private.XmlFragmentTextWriter" class.
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
 
namespace System.Xml.Private
{

using System;
using System.IO;
using System.Text;
using System.Globalization;

// This class is used for outputting XML fragments from the
// "InnerXml" and "OuterXml" properties in "XmlNode".

internal class XmlFragmentTextWriter : XmlTextWriter
{
	// Constructor.
	public XmlFragmentTextWriter()
			: base(new StringWriter())
			{
				// Make the writer automatically shift to the content
				// area of the document if it is in the start state.
				autoShiftToContent = true;
			}

	// Close the fragment and return the final string.
	public override String ToString()
			{
				Close();
				return ((StringWriter)writer).ToString();
			}

	// Override some of the XmlTextWriter methods to handle namespaces better.
	public override void WriteStartAttribute
				(String prefix, String localName, String ns)
			{
				if(ns == null || ns.Length == 0)
				{
					if(prefix != null && prefix.Length != 0)
					{
						prefix = "";
					}
				}
				base.WriteStartAttribute(prefix, localName, ns);
			}
	public override void WriteStartElement
				(String prefix, String localName, String ns)
			{
				if(ns == null || ns.Length == 0)
				{
					if(prefix != null && prefix.Length != 0)
					{
						prefix = "";
					}
				}
				base.WriteStartElement(prefix, localName, ns);
			}
			
	protected override void WriteXmlDeclaration(String text)
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

				bool haveEncoding = false;
#if !ECMA_COMPAT
				// make sure the encoding matches
				String encoding = r["encoding"];
				if(encoding != null && encoding.Length > 0 )
				{
					haveEncoding = true;
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
				StringBuilder start = new StringBuilder( "<?xml version=\"1.0\"" );
				if( haveEncoding ) {
					start.Append( " encoding=\"" );
					start.Append( encoding );
					start.Append( "\"" );
				}
				if( haveStandalone ) {
					start.Append( " standalone=\"" );
					start.Append( standalone );
					start.Append( "\"" );
				}
				start.Append( "?>" );
				
				writer.Write( start.ToString() );
				writeState = System.Xml.WriteState.Prolog;
			}
	

}; // class XmlFragmentTextWriter

}; // namespace System.Xml.Private
