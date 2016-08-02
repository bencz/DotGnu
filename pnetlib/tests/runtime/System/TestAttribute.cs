/*
 * TestAttribute.cs - Tests for the "System.Attribute" class.
 *
 * Copyright (C) 2002  Free Software Foundation, Inc.
 *
 * Authors : Jonathan Springer
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

using System;
using System.Reflection;
using CSUnit;


public class FieldTypeAttribute : Attribute
{
	private Type fieldType;

	public FieldTypeAttribute(Type fieldType): base()
	{
		this.fieldType = fieldType;
	}

	public Type FieldType
	{
		get
		{
			return fieldType;
		}
	}
}

public class TestAttribute : TestCase
{
	public class Nested
	{
	}

	[FieldType(typeof(TestAttribute.Nested))]
	public int SomeField;

	// Constructor.
	public TestAttribute(String name)
			: base(name)
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
	
	public void TestAttribute_Bug13433()
	{
		FieldInfo fi = typeof(TestAttribute).GetField("SomeField");
		FieldTypeAttribute[] attrs = (FieldTypeAttribute[]) fi.GetCustomAttributes(typeof(FieldTypeAttribute), true);

		AssertEquals("TestAttribute+Nested", attrs[0].FieldType.ToString());
	}

}; // class TestAttribute
