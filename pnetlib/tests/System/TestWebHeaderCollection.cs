/*
 * TestWebHeaderCollection.cs - Test class for "System.Net.WebHeaderCollection" 
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2002  FSF.
 * 
 * Authors : Jeff Post
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
using System.Net;

public class TestWebHeaderCollection : TestCase
 {
	// Constructor.
	public TestWebHeaderCollection(String name)	: base(name)
	{
		// Nothing to do here.
	}

	// Set up for the tests.
	protected override void Setup()
	{
		// Nothing to do here.
	}

	// Clean up after the tests.
	protected override void Cleanup()
	{
		// Nothing to do here.
	}

	public void TestConstructor()
	{
		WebHeaderCollection whc = new WebHeaderCollection();
		if (whc == null)
			Fail("Constructor failed");
	}

	public void TestWebHeaderCollectionAdd()
	{
		WebHeaderCollection whc = new WebHeaderCollection();
		whc.Add("phony:header");
		whc.Add("some", "stuff");
		try
		{
			whc.Add("nonsenseheader");
			Fail("Add: failed to throw exception for missing colon");
		}
		catch (ArgumentException)
		{
			// So far, so good.
		}
		try
		{
			whc.Add(null);
			Fail("Add: failed to throw exception for null header");
		}
		catch (ArgumentNullException)
		{
			// Still ok...
		}
		try
		{
			whc.Add(null, "theOtherCtor");
			Fail("Add: failed to throw header for null name");
		}
		catch (ArgumentNullException)
		{
			// Onward and upward...
		}
		try
		{
			whc.Add("accept:betterNot");
			Fail("Add: failed to throw exception for restricted header");
		}
		catch (ArgumentException)
		{
			// Add looks good...
		}
	}

	public void TestWebHeaderCollectionAddWithoutValidate()
	{
		// Nothing to do here. - AddWithoutValidate is a protected method
	}

	public void TestWebHeaderCollectionGetValues()
	{
		WebHeaderCollection whc = new WebHeaderCollection();
		string[] strArray1;
		try
		{
			strArray1 = whc.GetValues(null);
			Fail("GetValues: failed to throw exception for null argument");
		}
		catch(ArgumentNullException)
		{
			// Ok
		}
		whc.Add("phony:junk");
		whc.Add("more", "stuff");
		string[] strArray = whc.GetValues("phony");
		if (strArray[0] != "junk")
			Fail("GetValues: returned incorrect data for 'phony:junk'");
		strArray1 = whc.GetValues("more");
		if (strArray1[0] != "stuff")
			Fail("GetValues: returned incorrect data for 'more:stuff'");
		string[] strArray2 = whc.GetValues("notThere");
		if (strArray2 != null)
			Fail("GetValues: did not return null for name:value not in collection");
	}

	public void TestWebHeaderCollectionIsRestricted()
	{
		AssertEquals("IsRestricted(\"phony\")",false,
			WebHeaderCollection.IsRestricted("phony"));
		AssertEquals("IsRestricted(\"accept\")",true,
			WebHeaderCollection.IsRestricted("accept"));
	}

	public void TestWebHeaderCollectionRemove()
	{
		WebHeaderCollection whc = new WebHeaderCollection();
		whc.Add("some:stuff");
		whc.Remove("some");
		try
		{
			whc.Remove(null);
			Fail("Remove: failed to throw exception for null argument");
		}
		catch (ArgumentNullException)
		{
			// Ok
		}
		try
		{
			whc.Remove("[NotValidHeader?]");
			Fail("Remove: failed to throw exception for invalid header name: '[NotValidHeader?]'");
		}
		catch (ArgumentException)
		{
			// Yep...
		}
		try
		{
			whc.Remove("accept");
			Fail("Remove: failed to throw exception for restricted header 'accept'");
		}
		catch (ArgumentException)
		{
			// Still moving along...
		}
	}

	public void TestWebHeaderCollectionSet()
	{
		WebHeaderCollection whc = new WebHeaderCollection();
		whc.Set("more", "junk");
		try
		{
			whc.Set(null, "value");
			Fail("Set: failed to throw exception for null name");
		}
		catch (ArgumentNullException)
		{
			// Whizzing right along...
		}
		try
		{
			whc.Set("accept", "NoNo");
			Fail("Set: failed to throw exception for restricted header");
		}
		catch (ArgumentException)
		{
			// goodie!
		}
		try
		{
			whc.Set("@this is not right!", "junk");
			Fail("Set: failed to throw exception for invalid header name");
		}
		catch (ArgumentException)
		{
			// Tada!
		}
	}

	public void TestWebHeaderCollectionToString()
	{
		WebHeaderCollection whc = new WebHeaderCollection();
		whc.Add("test", "entry");
		whc.Add("more:junk");
		AssertEquals("ToString()","test: entry\r\nmore: junk\r\n",
						whc.ToString());
	}
}
