/*
 * TestSystemXml.cs - Tests for the "System" namespace.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

using CSUnit;
using System.Xml;

public class TestXml
{

	public static TestSuite Suite()
			{
				TestSuite fullSuite, suite;
				fullSuite = new TestSuite("System.Xml Tests");

				suite = new TestSuite("General Tests");
				suite.AddTests(typeof(TestNameTable));
				suite.AddTests(typeof(TestXmlConvert));
				suite.AddTests(typeof(TestXmlException));
				suite.AddTests(typeof(TestXmlNamespaceManager));
				suite.AddTests(typeof(TestXmlParserContext));
				fullSuite.AddTest(suite);

				suite = new TestSuite("Writer Tests");
				suite.AddTests(typeof(TestXmlTextWriter));
				fullSuite.AddTest(suite);

				suite = new TestSuite("Reader Tests");
				suite.AddTests(typeof(TestXmlTextReader));
				fullSuite.AddTest(suite);

			#if !ECMA_COMPAT
				suite = new TestSuite("Node Tests");
				suite.AddTests(typeof(TestXmlAttribute));
				suite.AddTests(typeof(TestXmlCDataSection));
				suite.AddTests(typeof(TestXmlComment));
				suite.AddTests(typeof(TestXmlDocument));
				suite.AddTests(typeof(TestXmlDocumentFragment));
				suite.AddTests(typeof(TestXmlDocumentType));
				suite.AddTests(typeof(TestXmlElement));
				suite.AddTests(typeof(TestXmlSignificantWhitespace));
				suite.AddTests(typeof(TestXmlText));
				suite.AddTests(typeof(TestXmlWhitespace));
				fullSuite.AddTest(suite);
			#endif

				return fullSuite;
			}

}; // class TestSystem
