/*
 * XmlWriterSettings.cs - Implementation of the "System.Xml.XmlWriterSettings" class.
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

#if !ECMA_COMPAT
#if CONFIG_FRAMEWORK_2_0
namespace System.Xml
{

using System;
using System.Text;

	public sealed class XmlWriterSettings
	{
		private Boolean checkCharacters;
		private Boolean closeOutput;
		private ConformanceLevel conformance;
		private Encoding encoding;
		private Boolean indent;
		private String indentChars;
		private String newLineChars;
		private Boolean newLineOnAttributes;
		private NewLineHandling newLineHandling;
		private Boolean normalizeNewLines;
		private Boolean omitXmlDeclaration;
		private XmlOutputMethod outputMethod;

		public XmlWriterSettings()
		{
			Reset ();
		}

		private XmlWriterSettings(XmlWriterSettings org)
		{
			checkCharacters = org.checkCharacters;
			closeOutput = org.closeOutput;
			conformance = org.conformance;
			encoding = org.encoding;
			indent = org.indent;
			indentChars = org.indentChars;
			newLineChars = org.newLineChars;
			newLineOnAttributes = org.newLineOnAttributes;
			newLineHandling = org.newLineHandling;
			normalizeNewLines = org.normalizeNewLines;
			outputMethod = org.outputMethod;
			omitXmlDeclaration = org.omitXmlDeclaration;
		}

		public XmlWriterSettings Clone()
		{
			return new XmlWriterSettings(this);
		}

		public void Reset()
		{
			checkCharacters = true;
			closeOutput = false;
			conformance = ConformanceLevel.Document;
			encoding = Encoding.UTF8;
			indent = false;
			indentChars = "  ";
			newLineChars = Environment.NewLine;
			newLineOnAttributes = false;
			newLineHandling = NewLineHandling.Replace;
			normalizeNewLines = true;
			omitXmlDeclaration = false;
			outputMethod = XmlOutputMethod.AutoDetect;
		}

		public Boolean CheckCharacters
		{
			get
			{
				return checkCharacters;
			}
			set
			{
				checkCharacters = value;
			}
		}

		public Boolean CloseOutput
		{
			get
			{
				return closeOutput;
			}
			set
			{
				closeOutput = value;
			}
		}

		public ConformanceLevel ConformanceLevel
		{
			get
			{
				return conformance;
			}
			set
			{
				conformance = value;
			}
		}

		public Encoding Encoding
		{
			get
			{
				return encoding;
			}
			set
			{
				encoding = value;
			}
		}

		public Boolean Indent
		{
			get
			{
				return indent;
			}
			set
			{
				indent = value;
			}
		}

		public String IndentChars
		{
			get
			{
				return indentChars;
			}
			set
			{
				indentChars = value;
			}
		}

		public String NewLineChars
		{
			get
			{
				return newLineChars;
			}
			set
			{
				newLineChars = value;
			}
		}

		public Boolean NewLineOnAttributes
		{
			get
			{
				return newLineOnAttributes;
			}
			set
			{
				newLineOnAttributes = value;
			}
		}

		public NewLineHandling NewLineHandling
		{
			get
			{
				return newLineHandling;
			}
			set
			{
				newLineHandling = value;
			}
		}

		public Boolean NormalizeNewLines
		{
			get
			{
				return normalizeNewLines;
			}
			set
			{
				normalizeNewLines = value;
			}
		}

		public Boolean OmitXmlDeclaration
		{
			get
			{
				return omitXmlDeclaration;
			}
			set
			{
				omitXmlDeclaration = value;
			}
		}

		public XmlOutputMethod OutputMethod
		{
			get
			{
				return outputMethod;
			}
			set
			{
				outputMethod = value;
			}
		}

	}; // class XmlWriterSettings
}; // namespace System.Xml
#endif // CONFIG_FRAMEWORK_2_0
#endif // !ECMA_COMPAT

