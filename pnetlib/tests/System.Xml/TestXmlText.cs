/*
 * TestXmlText.cs - Tests for the "System.Xml.TestXmlText" class.
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

using CSUnit;
using System;
using System.Xml;

#if !ECMA_COMPAT

// Note: this class also tests the behaviour of "XmlCharacterData".

public class TestXmlText : TestCase
{
	// Internal state.
	private XmlDocument doc;
	private int changeBefore;
	private int changeAfter;
	private XmlNode expectedNode;
	private XmlNodeChangedEventHandler handleBefore;
	private XmlNodeChangedEventHandler handleAfter;

	// Constructor.
	public TestXmlText(String name)
			: base(name)
			{
				// Nothing to do here.
			}

	// Set up for the tests.
	protected override void Setup()
			{
				doc = new XmlDocument();
				handleBefore = new XmlNodeChangedEventHandler(WatchBefore);
				handleAfter = new XmlNodeChangedEventHandler(WatchAfter);
			}

	// Clean up after the tests.
	protected override void Cleanup()
			{
				// Nothing to do here.
			}

	// Check the properties on a newly constructed text node.
	private void CheckProperties(String msg, XmlText text,
								 String value, String xmlValue)
			{
				String temp;
				AssertEquals(msg + " [1]", "#text", text.LocalName);
				AssertEquals(msg + " [2]", "#text", text.Name);
				AssertEquals(msg + " [3]", String.Empty, text.Prefix);
				AssertEquals(msg + " [4]", String.Empty, text.NamespaceURI);
				AssertEquals(msg + " [5]", XmlNodeType.Text, text.NodeType);
				AssertEquals(msg + " [6]", value, text.Data);
				AssertEquals(msg + " [7]", value, text.Value);
				AssertEquals(msg + " [8]", value, text.InnerText);
				AssertEquals(msg + " [9]", value.Length, text.Length);
				AssertEquals(msg + " [10]", String.Empty, text.InnerXml);
				AssertEquals(msg + " [11]", xmlValue, text.OuterXml);
			}
	private void CheckProperties(String msg, XmlText text, String value)
			{
				CheckProperties(msg, text, value, value);
			}

	// Test the construction of text nodes.
	public void TestXmlTextConstruct()
			{
				CheckProperties("Construct (1)",
								doc.CreateTextNode(null),
								String.Empty);
				CheckProperties("Construct (2)",
								doc.CreateTextNode(String.Empty),
								String.Empty);
				CheckProperties("Construct (3)",
								doc.CreateTextNode("xyzzy"),
								"xyzzy");
				CheckProperties("Construct (4)",
								doc.CreateTextNode("<&>\"'"),
								"<&>\"'", "&lt;&amp;&gt;\"'");
			}

	// Watch for change events.
	private void WatchBefore(Object sender, XmlNodeChangedEventArgs args)
			{
				AssertEquals("Sender", expectedNode.OwnerDocument, sender);
				AssertEquals
					("Action", XmlNodeChangedAction.Change, args.Action);
				AssertEquals("Node", expectedNode, args.Node);
				AssertEquals("OldParent", expectedNode.ParentNode,
							 args.OldParent);
				AssertEquals("NewParent", expectedNode.ParentNode,
							 args.NewParent);
				++changeBefore;
			}
	private void WatchAfter(Object sender, XmlNodeChangedEventArgs args)
			{
				AssertEquals("Sender", expectedNode.OwnerDocument, sender);
				AssertEquals
					("Action", XmlNodeChangedAction.Change, args.Action);
				AssertEquals("Node", expectedNode, args.Node);
				AssertEquals("OldParent", expectedNode.ParentNode,
							 args.OldParent);
				AssertEquals("NewParent", expectedNode.ParentNode,
							 args.NewParent);
				++changeAfter;
			}

	// Register to watch for change events.
	private void RegisterWatchNeither(XmlNode node)
			{
				expectedNode = node;
				changeBefore = 0;
				changeAfter = 0;
			}
	private void RegisterWatchBoth(XmlNode node)
			{
				doc.NodeChanging += handleBefore;
				doc.NodeChanged += handleAfter;
				expectedNode = node;
				changeBefore = 0;
				changeAfter = 0;
			}
	private void RegisterWatchBefore(XmlNode node)
			{
				doc.NodeChanging += handleBefore;
				expectedNode = node;
				changeBefore = 0;
				changeAfter = 0;
			}
	private void RegisterWatchAfter(XmlNode node)
			{
				doc.NodeChanged += handleAfter;
				expectedNode = node;
				changeBefore = 0;
				changeAfter = 0;
			}

	// Check that change events were fired.
	private void CheckForChangeNeither()
			{
				doc.NodeChanging -= handleBefore;
				doc.NodeChanged -= handleAfter;
				AssertEquals("CheckForChange (Before)", 0, changeBefore);
				AssertEquals("CheckForChange (After)", 0, changeAfter);
			}
	private void CheckForChangeBoth()
			{
				doc.NodeChanging -= handleBefore;
				doc.NodeChanged -= handleAfter;
				AssertEquals("CheckForChange (Before)", 1, changeBefore);
				AssertEquals("CheckForChange (After)", 1, changeAfter);
			}
	private void CheckForChangeBefore()
			{
				doc.NodeChanging -= handleBefore;
				doc.NodeChanged -= handleAfter;
				AssertEquals("CheckForChange (Before)", 1, changeBefore);
				AssertEquals("CheckForChange (After)", 0, changeAfter);
			}
	private void CheckForChangeAfter()
			{
				doc.NodeChanging -= handleBefore;
				doc.NodeChanged -= handleAfter;
				AssertEquals("CheckForChange (Before)", 0, changeBefore);
				AssertEquals("CheckForChange (After)", 1, changeAfter);
			}

	// Test the appending of text data.
	public void TestXmlTextAppendData()
			{
				// Test simple appending.
				XmlText text = doc.CreateTextNode("hello");
				RegisterWatchNeither(text);
				text.AppendData(" and goodbye");
				CheckForChangeNeither();
				AssertEquals("AppendData (1)", 17, text.Length);
				AssertEquals("AppendData (2)", "hello and goodbye", text.Data);

				// Test event handling.
				RegisterWatchBoth(text);
				text.AppendData("blah");
				CheckForChangeBoth();

				RegisterWatchBefore(text);
				text.AppendData("blah2");
				CheckForChangeBefore();

				RegisterWatchAfter(text);
				text.AppendData("blah3");
				CheckForChangeAfter();

				AssertEquals("AppendData (3)",
							 "hello and goodbyeblahblah2blah3", text.Data);
			}

	// Test the deleting of text data.
	public void TestXmlTextDeleteData()
			{
				// Test simple deleting.
				XmlText text = doc.CreateTextNode("hello");
				RegisterWatchNeither(text);
				text.DeleteData(1, 3);
				AssertEquals("DeleteData (1)", 2, text.Length);
				AssertEquals("DeleteData (2)", "ho", text.Data);

				// Test out of range deletions.
				text.DeleteData(-10, 3);
				text.DeleteData(2, 30);
				AssertEquals("DeleteData (3)", "ho", text.Data);
				text.DeleteData(-1000, 4000);
				AssertEquals("DeleteData (4)", String.Empty, text.Data);
				CheckForChangeNeither();

				// Test event handling.
				text = doc.CreateTextNode("hello");
				RegisterWatchBoth(text);
				text.DeleteData(1, 3);
				CheckForChangeBoth();

				RegisterWatchBefore(text);
				text.DeleteData(-5, 1);
				CheckForChangeBefore();

				RegisterWatchAfter(text);
				text.DeleteData(0, 5);
				CheckForChangeAfter();
			}

	// Test the inserting of text data.
	public void TestXmlTextInsertData()
			{
				// Test simple insertions.
				XmlText text = doc.CreateTextNode("hello");
				RegisterWatchNeither(text);
				text.InsertData(3, "lll");
				AssertEquals("InsertData (1)", 8, text.Length);
				AssertEquals("InsertData (2)", "helllllo", text.Data);
				CheckForChangeNeither();

				// Test out of range insertions.  The before event will
				// fire, but not the after event, due to exceptions between.
				RegisterWatchBoth(text);
				try
				{
					text.InsertData(-1, "x");
					Fail("InsertData (3)");
				}
				catch(ArgumentOutOfRangeException)
				{
					// Success
				}
				CheckForChangeBefore();
				RegisterWatchBoth(text);
				try
				{
					text.InsertData(9, "x");
					Fail("InsertData (4)");
				}
				catch(ArgumentOutOfRangeException)
				{
					// Success
				}
				CheckForChangeBefore();

				// Test event handling.
				RegisterWatchBoth(text);
				text.InsertData(8, "x");
				AssertEquals("InsertData (5)", 9, text.Length);
				AssertEquals("InsertData (6)", "helllllox", text.Data);
				CheckForChangeBoth();

				RegisterWatchBefore(text);
				text.InsertData(8, "y");
				AssertEquals("InsertData (7)", 10, text.Length);
				AssertEquals("InsertData (8)", "hellllloyx", text.Data);
				CheckForChangeBefore();

				RegisterWatchBefore(text);
				text.InsertData(8, "z");
				AssertEquals("InsertData (9)", 11, text.Length);
				AssertEquals("InsertData (10)", "helllllozyx", text.Data);
				CheckForChangeBefore();
			}

	// Test the inserting of text data.
	public void TestXmlTextReplaceData()
			{
				// Test simple replacing.
				XmlText text = doc.CreateTextNode("hello");
				RegisterWatchNeither(text);
				text.ReplaceData(1, 3, "xyzzy");
				AssertEquals("ReplaceData (1)", 7, text.Length);
				AssertEquals("ReplaceData (2)", "hxyzzyo", text.Data);

				// Test out of range replaces.
				text.ReplaceData(-10, 3, "abc");
				text.ReplaceData(10, 30, "def");
				AssertEquals("ReplaceData (3)", "abchxyzzyodef", text.Data);
				text.ReplaceData(-1000, 4000, "");
				AssertEquals("ReplaceData (4)", String.Empty, text.Data);
				CheckForChangeNeither();

				// Test event handling.
				text = doc.CreateTextNode("hello");
				RegisterWatchBoth(text);
				text.ReplaceData(1, 3, "blah");
				CheckForChangeBoth();

				RegisterWatchBefore(text);
				text.ReplaceData(-5, 1, "blah2");
				CheckForChangeBefore();

				RegisterWatchAfter(text);
				text.ReplaceData(0, 5, "blah3");
				CheckForChangeAfter();
			}

	// Test the setting of text data.
	public void TestXmlTextSetData()
			{
				// Test simple setting.
				XmlText text = doc.CreateTextNode(null);
				AssertEquals("SetData (1)", String.Empty, text.Data);
				RegisterWatchNeither(text);
				text.Data = "hello";
				CheckForChangeNeither();
				AssertEquals("SetData (2)", "hello", text.Data);
				AssertEquals("SetData (3)", 5, text.Length);

				// Test event handling.
				RegisterWatchBoth(text);
				text.Data = "blah";
				CheckForChangeBoth();
				AssertEquals("SetData (4)", "blah", text.Data);
				AssertEquals("SetData (5)", 4, text.Length);

				RegisterWatchBefore(text);
				text.Data = "blah2";
				CheckForChangeBefore();
				AssertEquals("SetData (6)", "blah2", text.Data);
				AssertEquals("SetData (7)", 5, text.Length);

				RegisterWatchAfter(text);
				text.Data = "blah3";
				CheckForChangeAfter();
				AssertEquals("SetData (8)", "blah3", text.Data);
				AssertEquals("SetData (9)", 5, text.Length);
			}

	// Test the splitting of text nodes.
	public void TestXmlTextSplitText()
			{
				// Perform simple splits at various points.
				XmlAttribute attr = doc.CreateAttribute("foo");
				XmlText text = doc.CreateTextNode("hello and goodbye");
				RegisterWatchNeither(text);
				attr.AppendChild(text);
				XmlText split = text.SplitText(6);
				CheckForChangeNeither();
				Assert("SplitText (1)", text != split);
				AssertEquals("SplitText (2)", "hello ", text.Value);
				AssertEquals("SplitText (3)", "and goodbye", split.Value);

				attr = doc.CreateAttribute("foo");
				text = doc.CreateTextNode("hello and goodbye");
				attr.AppendChild(text);
				split = text.SplitText(-1);
				Assert("SplitText (4)", text != split);
				AssertEquals("SplitText (5)", "", text.Value);
				AssertEquals("SplitText (6)", "hello and goodbye", split.Value);

				attr = doc.CreateAttribute("foo");
				text = doc.CreateTextNode("hello and goodbye");
				attr.AppendChild(text);
				split = text.SplitText(100);
				Assert("SplitText (7)", text != split);
				AssertEquals("SplitText (8)", "hello and goodbye", text.Value);
				AssertEquals("SplitText (9)", "", split.Value);

				// Test event handling.
				attr = doc.CreateAttribute("foo");
				text = doc.CreateTextNode("hello and goodbye");
				attr.AppendChild(text);
				RegisterWatchBoth(text);
				text.SplitText(6);
				CheckForChangeBoth();

				attr = doc.CreateAttribute("foo");
				text = doc.CreateTextNode("hello and goodbye");
				attr.AppendChild(text);
				RegisterWatchBefore(text);
				text.SplitText(6);
				CheckForChangeBefore();

				attr = doc.CreateAttribute("foo");
				text = doc.CreateTextNode("hello and goodbye");
				attr.AppendChild(text);
				RegisterWatchAfter(text);
				text.SplitText(6);
				CheckForChangeAfter();
			}

	// Test the substring operator on text data.
	public void TestXmlTextSubstring()
			{
				XmlText text = doc.CreateTextNode("hello and goodbye");

				RegisterWatchNeither(text);
				AssertEquals("Substring (1)", String.Empty,
							 text.Substring(3, 0));
				AssertEquals("Substring (2)", "ello",
							 text.Substring(1, 4));
				AssertEquals("Substring (3)", "hello",
							 text.Substring(-1, 6));
				AssertEquals("Substring (4)", "llo and goodbye",
							 text.Substring(2, 2000));
				CheckForChangeNeither();
			}

}; // class TestXmlText

#endif // !ECMA_COMPAT
