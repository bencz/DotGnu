/*
 * TestString.cs - Tests for the "System.String" class.
 *
 * Copyright (C) 2002  Free Software Foundation, Inc.
 *
 * Authors : Stephen Compall, Gopal.V, & Richard Baumann
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

public class TestString : TestCase
{
	// Constructor.
	public TestString(String name)
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

	//Methods
	public void TestStringClone()
	{
		String fubar = "Foo Bar";
		AssertEquals("fubar.Clone() as String",fubar,(String)fubar.Clone());
		AssertEquals("fubar.Clone() as Object",(Object)fubar,fubar.Clone());
		AssertEquals("((ICloneable) fubar).Clone() as String",fubar,(String)((ICloneable) fubar).Clone());
		AssertEquals("((ICloneable) fubar).Clone() as Object",(Object)fubar,((ICloneable) fubar).Clone());
	}
	public void TestStringCompare()
	{
/*		String lesser = "abc";
		String medium = "abcd";
		String greater = "xyz";
		String caps = "ABC";*/

		AssertEquals("String.Compare(null,null))",0,String.Compare(null,null));
		AssertEquals("String.Compare(\"abc\",null)",1,
			String.Compare("abc", null));

		Assert("String.Compare(null,\"abc\")",
			String.Compare(null,"abc") < 0);

		Assert("String.Compare(\"abc\",\"xyz\")",
			String.Compare("abc","xyz") < 0);

		Assert("String.Compare(\"abc\",\"abcd\")",
			String.Compare("abc","abcd") < 0);

		Assert("String.Compare(\"xyz\",\"abc\")",
			String.Compare("xyz","abc") > 0);

		Assert("String.Compare(\"abc\",\"ABC\",true)",
			String.Compare("abc","ABC",true) == 0);

		Assert("String.Compare(\"abc\",\"ABC\",true)",
			String.Compare("abc","ABC",true) == 0);

		Assert("String.Compare(\"abc\",\"ABC\",false)",
			String.Compare("abc","ABC",false) != 0);

		Assert("String.Compare(\"a\",\"A\")",String.Compare("a","A") < 0);
		Assert("String.Compare(\"A\",\"a\")",String.Compare("A","a") > 0);
	
		AssertEquals("String.Compare(\"\",\"\")",String.Compare("",""),0);

		AssertEquals("String.Compare(\"abc\",0,\"xyz\",0,0)",
			String.Compare("abc",0,"xyz",0,0),0);

		AssertEquals("String.Compare(\"abcdabcd\",0,\"ab\",0,2)",
			String.Compare("abcdabcd",0,"ab",0,2),0);
			
		AssertEquals("String.Compare(\"abcdabcd\",4,\"ab\",0,2)",
			String.Compare("abcdabcd",4,"ab",0,2),0);
			
		Assert("String.Compare(\"abcdabcd\",1,\"ab\",0,2)",
			String.Compare("abcdabcd",1,"ab",0,2) > 0 );

		Assert("String.Compare(\"abcdabcd\",0,\"xyz\",0,2)",
			String.Compare("abcdabcd",0,"xyz",0,2) < 0 );
		
		AssertEquals("String.Compare(\"abcdabcd\",0,\"abd\",0,2)",
			String.Compare("abcdabcd",0,"abd",0,2),0);
		
		try
		{
			String.Compare("ab",3,"abc",1,7);
			Fail(" String.Compare(\"ab\",3,\"abc\",1,7) did not throw ArgumentOutOfRangeException");
		}
		catch(ArgumentOutOfRangeException err)
		{
			//Ok, that worked :)
			//move on folks
		}

		Assert("String.Compare(\"ABCDC\",1,\"bcd\",0,3,false)",
			String.Compare("ABCDC",1,"bcd",0,3,false) !=0);
			
		Assert("String.Compare(\"ABCDC\",1,\"bcd\",0,3,true)",
			String.Compare("ABCDC",1,"bcd",0,3,true) !=1);
		try
		{
			String.Compare("ab",3,"abc",1,7,true);
			Fail(" String.Compare(\"ab\",3,\"abc\",1,7,true) did not throw ArgumentOutOfRangeException");
		}
		catch(ArgumentOutOfRangeException err)
		{
			//Ok, that worked :)
			//move on folks
		}
			
		try
		{
			AssertEquals("String.Compare(\"ab\",1,\"abd\",1,7)",
				-1 , String.Compare("ab",1,"abd",1,7));
#if ECMA_COMPAT
			Fail(" String.Compare(\"ab\",1,\"abc\",1,7,true) did not throw ArgumentOutOfRangeException");
#endif
		}
		catch(ArgumentOutOfRangeException err)
		{
#if !ECMA_COMPAT
			Fail(" String.Compare(\"ab\",1,\"abc\",1,7,true) should NOT throw an ArgumentOutOfRangeException");
#endif
		}

		try
		{
			AssertEquals("String.Compare(\"ab\",1,\"abc\",3,7,true)",
				1 , String.Compare("ab",1,"abc",3,7,true));
#if ECMA_COMPAT
			Fail(" String.Compare(\"ab\",1,\"abc\",3,7,true) did not throw ArgumentOutOfRangeException");
#endif
		}
		catch(ArgumentOutOfRangeException err)
		{
#if !ECMA_COMPAT
			Fail(" String.Compare(\"ab\",1,\"abc\",3,7,true) should NOT throw an ArgumentOutOfRangeException");
#endif
		}
	/*
		TODO: put in a looped check for Compare ()
	*/
	}

	public void TestStringCompareOrdinal()
	{
	/*
		TODO: doesn't this need checks for all the I18n infos ?
		      what's the sense in doing it for English , which is
		      what Compare() does.
	*/
	}

	public void TestStringCompare1()
	{
		AssertEquals("String.Compare(null, 0, null , 0 , 0)",
			String.Compare(null, 0, null, 0, 0), 0);

		AssertEquals("String.Compare(null, 0, String.Empty , 0 , 0)",
			String.Compare(null, 0, String.Empty, 0, 0), -1);

		AssertEquals("String.Compare(String.Empty, 0, null , 0 , 0)",
			String.Compare(String.Empty, 0, null, 0, 0), 1);

		AssertEquals("String.Compare(String.Empty, 0, String.Empty , 0 , 0)",
			String.Compare(String.Empty, 0, String.Empty, 0, 0), 0);
	}

	public void TestStringCompareTo()
	{
	/*
		NOTE: doesn't this actually call the Compare(this,a) func ?
	*/
		String str="abc";
		Assert("str.CompareTo(\"xyz\")", str.CompareTo("xyz") < 0);
		Assert("str.CompareTo(\"abc\")", str.CompareTo("abc") == 0);
		Assert("str.CompareTo(\"0\")", str.CompareTo("0") > 0);	
	}

	public void TestStringConcat()
	{
		//String str1="Fu";
		//String str2="Bar";
		AssertEquals("String.Concat(\"Fu\",\"Bar\")",
			String.Concat("Fu","Bar"),"FuBar");
		AssertEquals("String.Concat(\"Foo\",\" \",\"Bar\")",
			String.Concat("Foo"," ","Bar"),"Foo Bar");
		// yup , F Up Beyond All Recognition
	}

	public void TestStringCopy()
	{
		String str1="Foo";
		String str2= String.Copy(str1);
		AssertEquals("String.Copy(str1)==str1",str2,str1);
		Assert("String.Copy(str1) as Object != str1 as Object",(Object)str2 != (Object)str1);
		try
		{
			String s = String.Copy(null);
			Fail(" String.Copy(null) should throw an ArgumentNullException");
		}
		catch(ArgumentNullException err)
		{
			//ummm... looks good 
		}
	}

	public void TestStringCopyTo()
	{
	/*TODO*/
		String str1 = "FuBar";
		try
		{
			str1.CopyTo(0,(char[])null,0,0);
			Fail("str1.CopyTo(0,(char[])null,0,0) should throw an ArgumentNullException");
		}
		catch(ArgumentNullException err)
		{
			//worked !
		}
		char[] c = new char[str1.Length];
		for (int i = 0; i < str1.Length ; i++) 
			str1.CopyTo(i, c, i, 1); // copy 1 char at a time
		String str2 = new String(c);
		AssertEquals("str1.CopyTo() char by char",str1,str2);
		try
		{
			str1.CopyTo(0, c, 0, -1);
			Fail("str1.CopyTo(0, char[], 0, -1) should throw an ArgumentOutOfRangeException");
		}
		catch(ArgumentOutOfRangeException e)
		{
			//worked !
		}
		try
		{
			str1.CopyTo(0, c, c.Length, 1);
			Fail("str1.CopyTo(0, char[], char[].Length, 1) should throw an ArgumentOutOfRangeException");
		}
		catch(ArgumentOutOfRangeException e)
		{
			//worked !
		}
		try
		{
			str1.CopyTo(str1.Length, c, 0, 1);
			Fail("str1.CopyTo(str1.Length, char[], 0, 1) should throw an ArgumentOutOfRangeException");
		}
		catch(ArgumentOutOfRangeException e)
		{
			//worked !
		}
		// must find a better error message :)
	}

	public void TestStringEndsWith()
	{
	/*TODO*/
		String str = "Foo Bar";
		try 
		{
			bool check = str.EndsWith(null);
			Fail("String.EndsWith(null) should throw an ArgumentNullException");
		} 
		catch (ArgumentNullException) 
		{
			//OK move on 
		}
		Assert("str.EndsWith(\"r\")", str.EndsWith("r"));
		Assert("str.EndsWith(\"Bar\")", str.EndsWith("Bar"));
		Assert("str.EndsWith(\"Foo Bar\")", str.EndsWith("Foo Bar"));
		Assert("!str.EndsWith(\"Foo\")", !str.EndsWith("Foo"));
	}

	public void TestStringEquals()
	{
	/*
		TODO: really need to see how to I18n here 
	*/
		String foobar = "FooBar";
		String fubar = "FooBar";
		String good_copy = foobar;
		String bad_copy = "I'm bad";
		String bad = null;
		try
		{
			bool q=bad.Equals("nothing");
			Fail("bad.Equals(\"nothing\") should have thrown a NullReferenceException");
		}
		catch(NullReferenceException err)
		{
			//great !
		}

		Assert("!foobar.Equals(null)", !foobar.Equals(null));
		Assert("foobar.Equals(fubar)", foobar.Equals(fubar));
		Assert("foobar.Equals(good_copy)", foobar.Equals(good_copy));
		Assert("!foobar.Equals(bad_copy)", !foobar.Equals(bad_copy));

		Assert("String.Equals(null,null)", String.Equals(null, null));
		Assert("String.Equals(foobar,fubar)", String.Equals(foobar, fubar));
		Assert("!String.Equals(foobar,bad_copy)", !String.Equals(foobar, bad_copy));
	}

	public void TestStringFormat()
	{
	/*TODO*/
		AssertEquals ("String.Format(\"\",0)", "", String.Format ("", 0));

		AssertEquals ("String.Format(\"{0}\",\"FooBar\")", 
				"FooBar", String.Format ("{0}","FooBar"));

		AssertEquals ("String.Format(\"{0}\",111)", 
				"111", String.Format ("{0}",111));

		try
		{
			String.Format(null,12);
			Fail("String.Format(null,12) should throw an ArgumentNullException");
		}
		catch(ArgumentNullException err)
		{
			// all's well 
		}

		try
		{
			String.Format("Er..",null);
			// all's well 
		}
		catch(ArgumentNullException err)
		{
			Fail("String.Format(\"Er...\",null) should not throw an ArgumentNullException");
		}

		try
		{
			String.Format("{-1}",12);
			Fail("String.Format(\"{-1}\",12) should throw an FormatException");
		}
		catch(FormatException err)
		{
			// all's well 
		}

		try
		{
			String.Format("{3}",12);
			Fail("String.Format(\"{3}\",12) should throw an FormatException");
		}
		catch(FormatException err)
		{
			// all's well 
		}

		try
		{
			String.Format("{}",12);
			Fail("String.Format(\"{}\",12) should throw an FormatException");
		}
		catch(FormatException err)
		{
			// all's well 
		}

		AssertEquals("String.Format (\"<{0,5}>\", 12)",
			"<   12>",String.Format ("<{0,5}>", 12));
		
		AssertEquals("String.Format (\"<{0,-5}>\", 12)",
			"<12   >",String.Format ("<{0,-5}>", 12));

		AssertEquals("String.Format (\"<{0,10}>\", 42)",
			"<        42>",String.Format ("<{0,10}>", 42));
		
		AssertEquals("String.Format (\"<{0,-10}>\", 42)",
			"<42        >",String.Format ("<{0,-10}>", 42));

		AssertEquals ("String.Format(\"The {1} of {1}\",\"Church\",\"Emacs\")",
			"The Church of Emacs",
			String.Format ("The {0} of {1}", "Church", "Emacs"));
		
		AssertEquals(
			"String.Format(\"G{0} N{1} U{2}\",\"nu's\",\"ot\",\"nix\")",
			"Gnu's Not Unix", 
			String.Format ("G{0} N{1} U{2}", "nu's", "ot", "nix"));

		AssertEquals ("String.Format (\"{0:x8}\", 0xcafebabe),\"cafebabe\")",
			"cafebabe", String.Format ("{0:x8}", 0xcafebabe));

		AssertEquals ("String.Format (\"{0:X8}\", 0xcafebabe),\"CAFEBABE\")",
			"CAFEBABE", String.Format ("{0:X8}", 0xcafebabe));

		AssertEquals ("String.Format (\"<{0,5:x3}>\", 0x55)",
			"<  055>", String.Format ("<{0,5:x3}>", 0x55));
	
		AssertEquals ("String.Format (\"<{0,-5:x3}>\", 0x55)",
			"<055  >", String.Format ("<{0,-5:x3}>", 0x55));
			
		AssertEquals ("String.Format (\"if({0}==0){{ .... }}\",\"i\")",
			"if(i==0){ .... }", String.Format ("if({0}==0){{ .... }}", "i"));

		/*  Some tests inspired by the mailing list  */
		AssertEquals ("String.Format (\"0x{0:X2}\", (byte)255)",
				"0xFF", String.Format("0x{0:X2}", (byte)255));

		AssertEquals ("String.Format (\"0x{0:X2}\", (byte)14)",
				"0x0E", String.Format("0x{0:X2}", (byte)14));

		AssertEquals ("String.Format (\"{0:D2}\", 9)",
				"09", String.Format("{0:D2}", 9));

		// Can't use these any more since cultures mess them up -- Rhys.
#if false
#if CONFIG_EXTENDED_NUMERICS
		AssertEquals ("String.Format (\"{0:F2}\", 1234567.89)",
				"1234567.89", String.Format("{0:F2}", 1234567.89));

		AssertEquals ("String.Format (\"{0:C2}\", 1234567)",
		 		"\u00a4"+"1,234,567.00", String.Format("{0:C2}", 1234567));

		AssertEquals ("String.Format (\"{0:E}\", 1234568)",
				"1.234568E+006", String.Format("{0:E}", 1234568));

		AssertEquals ("String.Format (\"{0:e}\", 0.325)",
				"3.25e-001", String.Format("{0:e}", 0.325));

		/*  Custom Format tests... */
		AssertEquals ("String.Format(\"{0:#,###,##0.00}\", 1234567.81)",
				"1,234,567.81", String.Format("{0:#,###,##0.00}", 1234567.81));

		AssertEquals ("String.Format(\"{0:#,##0,}M\", 1234000)",
				"1,234M", String.Format("{0:#,###,}M", 1234000));

		AssertEquals("String.Format(\"{0:####.##}\", 0)",
				".", String.Format("{0:####.##}", 0));

		AssertEquals("String.Format(\"{0:###0.0#}\", 0)",
				"0.0", String.Format("{0:###0.0#}", 0));
#endif
#endif
	}

	public void TestStringGetEnumerator()
	{
		String str = "foobar";
		char[] c = new char[str.Length];
		CharEnumerator en = str.GetEnumerator();
		AssertNotNull("CharEnumerator en!=null", en);
		
		for (int i = 0; i < str.Length; i++) 
		{
			en.MoveNext();
			c[i] = en.Current;
		}
		String copy = new String(c);
		AssertEquals("Copy by enumerator for string", str, copy);
	}

	public void TestStringGetHashCode()
	{
	/*
		TODO: what do I do here ?. (determinicity check ?)
		      s.GetHashCode() == s.GetHashCode() ?.
	*/
		String str1 = "foobar";
		String str2 = String.Copy(str1);
		AssertEquals("str1.GetHashCode() == str1.GetHashCode()",
			str1.GetHashCode(),str1.GetHashCode());
		AssertEquals("str1.GetHashCode() == str2.GetHashCode()",
			str1.GetHashCode(),str2.GetHashCode());
	}

	public void TestStringIndexOf()
	{
		String fubar = "Foo Bar";
		try
		{
			fubar.IndexOf(null);
			Fail("fubar.IndexOf(null) should throw an ArgumentNullException");
			
			fubar.IndexOf(null,0);
			Fail("fubar.IndexOf(null,0) should throw an ArgumentNullException");
			
			fubar.IndexOf(null,0,1);
			Fail("fubar.IndexOf(null,0,1) should throw an ArgumentNullException");
		}
		catch(ArgumentNullException err)
		{
			//all's well
		}
		try
		{
			fubar.IndexOf('f',fubar.Length+1);
			Fail("fubar.IndexOf('f',fubar.Length+1) should throw an ArgumentOutOfRangeException");

			fubar.IndexOf('f',fubar.Length+1,1);
			Fail("fubar.IndexOf('f',fubar.Length+1,1) should throw an ArgumentOutOfRangeException");
			
			fubar.IndexOf("foo",fubar.Length+1);
			Fail("fubar.IndexOf(\"foo\",fubar.Length+1) should throw an ArgumentOutOfRangeException");
			
			fubar.IndexOf("foo",fubar.Length+1,1);
			Fail("fubar.IndexOf(\"foo\",fubar.Length+1,1) should throw an ArgumentOutOfRangeException");
		}
		catch(ArgumentOutOfRangeException err)
		{
			//all's well 
		}
		AssertEquals("fubar.IndexOf('o')", fubar.IndexOf('o'),1);
		AssertEquals("fubar.IndexOf('F')", fubar.IndexOf('F'),0);
		AssertEquals("fubar.IndexOf('z')", fubar.IndexOf('z'),-1);

		AssertEquals("fubar.IndexOf(\"oo\")", fubar.IndexOf("oo"),1);
		AssertEquals("fubar.IndexOf(\"Bar\")", fubar.IndexOf("Bar"),4);
		AssertEquals("fubar.IndexOf(\"qwaz\")", fubar.IndexOf("qwaz"),-1);
		
		AssertEquals("fubar.IndexOf('o',1)", fubar.IndexOf('o',1),1);
		AssertEquals("fubar.IndexOf('o',3)", fubar.IndexOf('o',3),-1);
		AssertEquals("fubar.IndexOf('z',4)", fubar.IndexOf('z',4),-1);
		
		AssertEquals("fubar.IndexOf(\"oo\",1)", fubar.IndexOf("oo",1),1);
		AssertEquals("fubar.IndexOf(\"oo\",2)", fubar.IndexOf("oo",2),-1);

		AssertEquals("fubar.IndexOf('o',1,2)", fubar.IndexOf('o',1,1),1);
		AssertEquals("fubar.IndexOf('o',0,1)", fubar.IndexOf('o',0,1),-1);
		AssertEquals("fubar.IndexOf(' ',1,5)", fubar.IndexOf(' ',1,5),3);

		try
		{
			fubar.IndexOf('h', fubar.Length); // shouldn't throw per ECMA
		}
		catch (ArgumentOutOfRangeException)
		{
			Fail("IndexOf(char, int) should not throw when passed Length as the int");
		}
	/*
		TODO: I don't know any more test ideas , do you have one ?
	*/
	}

	public void TestStringIndexOfAny()
	{
		String fubar="mary had a little lamb ....";
		try
		{
			fubar.IndexOfAny(null);
			Fail("fubar.IndexOfAny(null) should throw an ArgumentNullException");
			fubar.IndexOfAny(null,0);
			Fail("fubar.IndexOfAny(null,0) should throw an ArgumentNullException");
			fubar.IndexOfAny(null,0,1);
			Fail("fubar.IndexOfAny(null,0,1) should throw an ArgumentNullException");
		}
		catch(ArgumentNullException err)
		{
			//all's A-OK !
		}
		char[] c={'a','e','i','o','u'};
		try
		{
			fubar.IndexOfAny(c,fubar.Length+1);
			Fail("fubar.IndexOfAny(c,fubar.Length+1) should throw an ArgumentOutOfRangeException");
			fubar.IndexOfAny(c,fubar.Length+1,1);
			Fail("fubar.IndexOfAny(c,fubar.Length+1,1) should throw an ArgumentOutOfRangeException");
			fubar.IndexOfAny(c,fubar.Length-3,5);
			Fail("fubar.IndexOfAny(c,fubar.Length-3,5) should throw an ArgumentOutOfRangeException");
		}
		catch(ArgumentOutOfRangeException)
		{
			//all's well in code and data :)
		}
		
		AssertEquals("fubar.IndexOfAny(c)",fubar.IndexOfAny(c),1); //m`a'
		AssertEquals("fubar.IndexOfAny(c,2)",fubar.IndexOfAny(c,2),6);//h`a
		AssertEquals("fubar.IndexOfAny(c,21)",fubar.IndexOfAny(c,21),-1);
		AssertEquals("fubar.IndexOfAny(c,2,5)",fubar.IndexOfAny(c,2,5),6);
		AssertEquals("fubar.IndexOfAny(c,2,3)",fubar.IndexOfAny(c,2,3),-1);			
	}

	public void TestStringInsert()
	{
		String fubar = "FooBar";
		AssertEquals("fubar.Insert(3,\" \")","Foo Bar",fubar.Insert(3," "));
		AssertEquals("fubar.Insert(0,\" \")"," FooBar",fubar.Insert(0," "));
		AssertEquals("fubar.Insert(fubar.Length,\" \")","FooBar ",fubar.Insert(fubar.Length," "));
		try
		{
			fubar.Insert(0,null);
			Fail("fubar.Insert(0,null) should throw an ArgumentNullException");
		}
		catch(ArgumentNullException)
		{
			// checks out ok
		}
		try
		{
			fubar.Insert(fubar.Length+1," ");
			Fail("fubar.Insert(fubar.Length+1,\" \") should throw an ArgumentOutOfRangeException");
		}
		catch(ArgumentOutOfRangeException)
		{
			// this works too
		}
	}

	public void TestStringIntern()
	{
		String foobar = "if the sun refused to shine, I don't mind, I don't mind... if the mountains fell in the sea, let it be, it aint me - hendrix";
		String fubar = foobar.Substring(0,10);
		fubar += foobar.Substring(10,foobar.Length-10);
		fubar = String.Intern(fubar);
		AssertEquals("String.Intern(fubar)",(Object)foobar,(Object)fubar);
		try
		{
			String.Intern(null);
			Fail("String.Intern(null) should throw an ArgumentNullException");
		}
		catch (ArgumentNullException)
		{
			// this works
		}
	}

	public void TestStringIsInterned()
	{
		// I can't imagine anyone using a string like this in pnetlib or the unit tests, so this should work
		char[] fu = new char[13] { 'q','w','e','r','t','y','u','i','o','p','\t','\0','\n' };
		String foobar = new String(fu);
		String fubar = String.IsInterned(foobar);
		Assert("String.IsInterned(foobar)",fubar == null);
		try
		{
			String.IsInterned(null);
			Fail("String.IsInterned(null) should throw an ArgumentNullException");
		}
		catch (ArgumentNullException)
		{
			// all is good
		}
	}

	public void TestStringJoin()
	{
		String fu = " fu ";
		String fu2 = "";
		String[] foo = new String[6] { "foo","ofo","oof","FOO","OFO","OOF" };
		String[] foo2 = new String[3] { "","","" };
		String[] foo3 = new String[0] { };
		AssertEquals("String.Join(fu,foo)","foo fu ofo fu oof fu FOO fu OFO fu OOF",String.Join(fu,foo));
		AssertEquals("String.Join(fu,foo,0,foo.Length)","foo fu ofo fu oof fu FOO fu OFO fu OOF",String.Join(fu,foo,0,foo.Length));
		AssertEquals("String.Join(fu,foo,0,1)","foo",String.Join(fu,foo,0,1));
		AssertEquals("String.Join(fu,foo,1,1)","ofo",String.Join(fu,foo,1,1));
		AssertEquals("String.Join(fu,foo,2,1)","oof",String.Join(fu,foo,2,1));
		AssertEquals("String.Join(fu,foo,3,1)","FOO",String.Join(fu,foo,3,1));
		AssertEquals("String.Join(fu,foo,4,1)","OFO",String.Join(fu,foo,4,1));
		AssertEquals("String.Join(fu,foo,5,1)","OOF",String.Join(fu,foo,5,1));
		AssertEquals("String.Join(fu,foo,0,2)","foo fu ofo",String.Join(fu,foo,0,2));
		AssertEquals("String.Join(fu,foo,1,2)","ofo fu oof",String.Join(fu,foo,1,2));
		AssertEquals("String.Join(fu,foo,2,2)","oof fu FOO",String.Join(fu,foo,2,2));
		AssertEquals("String.Join(fu,foo,3,2)","FOO fu OFO",String.Join(fu,foo,3,2));
		AssertEquals("String.Join(fu,foo,4,2)","OFO fu OOF",String.Join(fu,foo,4,2));
		AssertEquals("String.Join(fu,foo,0,3)","foo fu ofo fu oof",String.Join(fu,foo,0,3));
		AssertEquals("String.Join(fu,foo,1,3)","ofo fu oof fu FOO",String.Join(fu,foo,1,3));
		AssertEquals("String.Join(fu,foo,2,3)","oof fu FOO fu OFO",String.Join(fu,foo,2,3));
		AssertEquals("String.Join(fu,foo,3,3)","FOO fu OFO fu OOF",String.Join(fu,foo,3,3));
		AssertEquals("String.Join(fu,foo,0,4)","foo fu ofo fu oof fu FOO",String.Join(fu,foo,0,4));
		AssertEquals("String.Join(fu,foo,1,4)","ofo fu oof fu FOO fu OFO",String.Join(fu,foo,1,4));
		AssertEquals("String.Join(fu,foo,2,4)","oof fu FOO fu OFO fu OOF",String.Join(fu,foo,2,4));
		AssertEquals("String.Join(fu,foo,0,5)","foo fu ofo fu oof fu FOO fu OFO",String.Join(fu,foo,0,5));
		AssertEquals("String.Join(fu,foo,1,5)","ofo fu oof fu FOO fu OFO fu OOF",String.Join(fu,foo,1,5));
		AssertEquals("String.Join(fu,foo,0,0)","",String.Join(fu,foo,0,0));
		AssertEquals("String.Join(fu2,foo2,0,foo2.Length)","",String.Join(fu2,foo2,0,foo2.Length));
		AssertEquals("String.Join(fu,foo3,0,foo3.Length)","",String.Join(fu,foo3,0,foo3.Length));
		try
		{
			String.Join(fu,null);
			Fail("String.Join(fu,null) should throw an ArgumentNullException");
		}
		catch (ArgumentNullException)
		{
			// works
		}
		try
		{
			String.Join(fu,foo,0,foo.Length+1);
			Fail("String.Join(fu,foo,0,foo.Length+1) should throw an ArgumentOutOfRangeException");
		}
		catch (ArgumentOutOfRangeException)
		{
			// works
		}
	}

	public void TestStringLastIndexOf()
	{
		String foo = "Foo Bar foo bar fu bar Fu Bar";

		/* String.LastIndexOf(char) */
		AssertEquals("foo.LastIndexOf('r')",28,foo.LastIndexOf('r'));
		AssertEquals("foo.LastIndexOf('a')",27,foo.LastIndexOf('a'));
		AssertEquals("foo.LastIndexOf('B')",26,foo.LastIndexOf('B'));
		AssertEquals("foo.LastIndexOf(' ')",25,foo.LastIndexOf(' '));
		AssertEquals("foo.LastIndexOf('u')",24,foo.LastIndexOf('u'));
		AssertEquals("foo.LastIndexOf('F')",23,foo.LastIndexOf('F'));
		AssertEquals("foo.LastIndexOf('b')",19,foo.LastIndexOf('b'));
		AssertEquals("foo.LastIndexOf('f')",16,foo.LastIndexOf('f'));
		AssertEquals("foo.LastIndexOf('o')",10,foo.LastIndexOf('o'));
		AssertEquals("foo.LastIndexOf('c')",-1,foo.LastIndexOf('c'));

		/* String.LastIndexOf(char,int) */
		AssertEquals("foo.LastIndexOf('f',16)",16,foo.LastIndexOf('f',16));
		AssertEquals("foo.LastIndexOf('f',15)",8,foo.LastIndexOf('f',15));
		AssertEquals("foo.LastIndexOf('f',7)",-1,foo.LastIndexOf('f',7));
		AssertEquals("foo.LastIndexOf('f',foo.Length-1)",16,foo.LastIndexOf('f',foo.Length-1));
		try
		{
			AssertEquals("foo.LastIndexOf('f',foo.Length)",16,foo.LastIndexOf('f',foo.Length)); // don't ask me
		}
		catch (ArgumentOutOfRangeException)
		{
			// This looks like brain damage in the spec, but it's easy enough to implement.
			Fail("foo.LastIndexOf('f',foo.Length) should NOT throw an ArgumentOutOfRangeException");
		}
		try
		{
			foo.LastIndexOf('f',-1);
			Fail("foo.LastIndexOf('f',-1) should throw an ArgumentOutOfRangeException");
		}
		catch (ArgumentOutOfRangeException)
		{
			// nothing wrong here
		}
		try
		{
			foo.LastIndexOf('f',foo.Length+1);
			Fail("foo.LastIndexOf('f',foo.Length+1) should throw an ArgumentOutOfRangeException");
		}
		catch (ArgumentOutOfRangeException)
		{
			// or here
		}

		/* String.LastIndexOf(char,int,int) */
		AssertEquals("foo.LastIndexOf('f',16,17)",16,foo.LastIndexOf('f',16,17));
		AssertEquals("foo.LastIndexOf('f',15,16)",8,foo.LastIndexOf('f',15,16));
		AssertEquals("foo.LastIndexOf('f',15,7)",-1,foo.LastIndexOf('f',15,7));
		AssertEquals("foo.LastIndexOf('f',foo.Length-1,1)",-1,foo.LastIndexOf('f',foo.Length-1,1));
		AssertEquals("foo.LastIndexOf('r',foo.Length-1,1)",-1,foo.LastIndexOf('r',foo.Length-1,0));
		AssertEquals("foo.LastIndexOf('r',foo.Length-1,1)",28,foo.LastIndexOf('r',foo.Length-1,1));
		AssertEquals("foo.LastIndexOf('F',0,1)",0,foo.LastIndexOf('F',0,1));
		AssertEquals("foo.LastIndexOf('F',1,1)",-1,foo.LastIndexOf('F',1,1));
		try
		{
			AssertEquals("foo.LastIndexOf('r',foo.Length,0)",-1,foo.LastIndexOf('r',foo.Length,0)); // ask the ECMA
		}
		catch (ArgumentOutOfRangeException)
		{
			Fail("foo.LastIndexOf('r',foo.Length,0) should NOT throw an ArgumentOutOfRangeException");
		}
		try
		{
			AssertEquals("foo.LastIndexOf('r',foo.Length,1)",-1,foo.LastIndexOf('r',foo.Length,1)); // b/c these are
		}
		catch (ArgumentOutOfRangeException)
		{
			Fail("foo.LastIndexOf('r',foo.Length,1) should NOT throw an ArgumentOutOfRangeException");
		}
		try
		{
			AssertEquals("foo.LastIndexOf('r',foo.Length,2)",28,foo.LastIndexOf('r',foo.Length,2)); // all valid,
		}
		catch (ArgumentOutOfRangeException)
		{
			Fail("foo.LastIndexOf('r',foo.Length,2) should NOT throw an ArgumentOutOfRangeException");
		}
		try
		{
			AssertEquals("foo.LastIndexOf('f',foo.Length+10,0)",-1,foo.LastIndexOf('f',foo.Length+10,0)); // believe
		}
		catch (ArgumentOutOfRangeException)
		{
			Fail("foo.LastIndexOf('f',foo.Length+10,0) should NOT throw an ArgumentOutOfRangeException");
		}
		try
		{
			AssertEquals("foo.LastIndexOf('f',foo.Length+10,1)",-1,foo.LastIndexOf('f',foo.Length+10,1)); // it or not.
		}
		catch (ArgumentOutOfRangeException)
		{
			Fail("foo.LastIndexOf('f',foo.Length+10,1) should NOT throw an ArgumentOutOfRangeException");
		}
		try
		{
			AssertEquals("foo.LastIndexOf('f',foo.Length+10,11)",-1,foo.LastIndexOf('f',foo.Length+10,11)); // amazing,
		}
		catch (ArgumentOutOfRangeException)
		{
			Fail("foo.LastIndexOf('f',foo.Length+10,11) should NOT throw an ArgumentOutOfRangeException");
		}
		try
		{
			AssertEquals("foo.LastIndexOf('r',foo.Length+10,12)",28,foo.LastIndexOf('r',foo.Length+10,12)); // isn't it?
		}
		catch (ArgumentOutOfRangeException)
		{
			Fail("foo.LastIndexOf('r',foo.Length+10,12) should NOT throw an ArgumentOutOfRangeException");
		}	
		try
		{
			foo.LastIndexOf('f',-1,0);
			Fail("foo.LastIndexOf('f',-1,0) should throw an ArgumentOutOfRangeException");
		}
		catch (ArgumentOutOfRangeException)
		{
			// a-ok
		}
		try
		{
			foo.LastIndexOf('f',0,-1);
			Fail("foo.LastIndexOf('f',0,-1) should throw an ArgumentOutOfRangeException");
		}
		catch (ArgumentOutOfRangeException)
		{
			// no problems here
		}
		try
		{
			foo.LastIndexOf('f',0,2);
			Fail("foo.LastIndexOf('f',0,2) should throw an ArgumentOutOfRangeException");
		}
		catch (ArgumentOutOfRangeException)
		{
			// blah
		}

		/* String.LastIndexOf(String) */
		AssertEquals("foo.LastIndexOf(\"Bar\")",26,foo.LastIndexOf("Bar"));
		AssertEquals("foo.LastIndexOf(\"Fu\")",23,foo.LastIndexOf("Fu"));
		AssertEquals("foo.LastIndexOf(\"bar\")",19,foo.LastIndexOf("bar"));
		AssertEquals("foo.LastIndexOf(\"fu\")",16,foo.LastIndexOf("fu"));
		AssertEquals("foo.LastIndexOf(\"foo\")",8,foo.LastIndexOf("foo"));
		AssertEquals("foo.LastIndexOf(\"Foo\")",0,foo.LastIndexOf("Foo"));
		AssertEquals("foo.LastIndexOf(\"blah\")",-1,foo.LastIndexOf("blah"));
		AssertEquals("foo.LastIndexOf(\"\")",0,foo.LastIndexOf(""));
		try
		{
			foo.LastIndexOf((String)null);
			Fail("foo.LastIndexOf((String)null) should throw an ArgumentNullException");
		}
		catch (ArgumentNullException)
		{
			// looks good so far
		}

		/* String.LastIndexOf(String,int) */
		AssertEquals("foo.LastIndexOf(\"Bar\",foo.Length-1)",26,foo.LastIndexOf("Bar",foo.Length-1));
		AssertEquals("foo.LastIndexOf(\"Bar\",foo.Length-2)",4,foo.LastIndexOf("Bar",foo.Length-2));
		AssertEquals("foo.LastIndexOf(\"Fu\",foo.Length-3)",23,foo.LastIndexOf("Fu",foo.Length-3));
		AssertEquals("foo.LastIndexOf(\"Fu\",foo.Length-6)",-1,foo.LastIndexOf("Fu",foo.Length-6));
		try
		{
			AssertEquals("foo.LastIndexOf(\"\",0)",0,foo.LastIndexOf("",0)); // this is absurd,
		}
		catch (ArgumentOutOfRangeException)
		{
			Fail("foo.LastIndexOf(\"\",0) should NOT throw an ArgumentOutOfRangeException");
		}
		try
		{
			AssertEquals("foo.LastIndexOf(\"\",1)",0,foo.LastIndexOf("",1)); // as is this
		}
		catch (ArgumentOutOfRangeException)
		{
			Fail("foo.LastIndexOf(\"\",1) should NOT throw an ArgumentOutOfRangeException");
		}
		try
		{
			foo.LastIndexOf((String)null,0);
			Fail("foo.LastIndexOf((String)null,0) should throw an ArgumentNullException");
		}
		catch (ArgumentNullException)
		{
			// move along, nothing to see here
		}
		try
		{
			foo.LastIndexOf("foo",-1);
			Fail("foo.LastIndexOf(\"foo\",-1) should throw an ArgumentOutOfRangeException");
		}
		catch (ArgumentOutOfRangeException)
		{
			// this works
		}
		try
		{
			foo.LastIndexOf("foo",foo.Length);
			Fail("foo.LastIndexOf(\"foo\",foo.Length) should throw an ArgumentOutOfRangeException");
		}
		catch (ArgumentOutOfRangeException)
		{
			// this checks out
		}
		try
		{
			foo.LastIndexOf("foo",foo.Length+1);
			Fail("foo.LastIndexOf(\"foo\",foo.Length+1) should throw an ArgumentOutOfRangeException");
		}
		catch (ArgumentOutOfRangeException)
		{
			// this checks out
		}

		/* String.LastIndexOf(String,int,int) */
		AssertEquals("foo.LastIndexOf(\"Bar\",foo.Length-1,foo.Length)",26,foo.LastIndexOf("Bar",foo.Length-1,foo.Length));
		AssertEquals("foo.LastIndexOf(\"Bar\",foo.Length-2,foo.Length-1)",4,foo.LastIndexOf("Bar",foo.Length-2,foo.Length-1));
		AssertEquals("foo.LastIndexOf(\"Bar\",foo.Length-1,3)",26,foo.LastIndexOf("Bar",foo.Length-1,3));
		AssertEquals("foo.LastIndexOf(\"Bar\",foo.Length-2,3)",-1,foo.LastIndexOf("Bar",foo.Length-2,3));
		AssertEquals("foo.LastIndexOf(\"Bar\",foo.Length-1,2)",-1,foo.LastIndexOf("Bar",foo.Length-1,2));
		AssertEquals("foo.LastIndexOf(\"Fu\",foo.Length-4,foo.Length-3)",23,foo.LastIndexOf("Fu",foo.Length-4,foo.Length-3));
		AssertEquals("foo.LastIndexOf(\"Fu\",foo.Length-6,foo.Length-5)",-1,foo.LastIndexOf("Fu",foo.Length-6,foo.Length-5));
		AssertEquals("foo.LastIndexOf(\"Fu\",foo.Length-4,3)",23,foo.LastIndexOf("Fu",foo.Length-4,3));
		AssertEquals("foo.LastIndexOf(\"Fu\",foo.Length-6,3)",-1,foo.LastIndexOf("Fu",foo.Length-6,3));
		try
		{
			AssertEquals("foo.LastIndexOf(\"\",0,0)",0,foo.LastIndexOf("",0,0)); // and the absurdity continues
		}
		catch (ArgumentOutOfRangeException)
		{
			Fail("foo.LastIndexOf(\"\",0,0) should NOT throw an ArgumentOutOfRangeException");
		}
		try
		{
			AssertEquals("foo.LastIndexOf(\"\",0,1)",0,foo.LastIndexOf("",0,1)); // need I say more?
		}
		catch (ArgumentOutOfRangeException)
		{
			Fail("foo.LastIndexOf(\"\",0,1) should NOT throw an ArgumentOutOfRangeException");
		}
		try
		{
			AssertEquals("foo.LastIndexOf(\"\",1,0)",0,foo.LastIndexOf("",1,0)); // ok, "more"
		}
		catch (ArgumentOutOfRangeException)
		{
			Fail("foo.LastIndexOf(\"\",1,0) should NOT throw an ArgumentOutOfRangeException");
		}
		try
		{
			AssertEquals("foo.LastIndexOf(\"\",1,1)",0,foo.LastIndexOf("",1,1)); // and more
		}
		catch (ArgumentOutOfRangeException)
		{
			Fail("foo.LastIndexOf(\"\",1,1) should NOT throw an ArgumentOutOfRangeException");
		}
		try
		{
			foo.LastIndexOf((String)null,0,0);
			Fail("foo.LastIndexOf((String)null,0,0) should throw an ArgumentNullException");
		}
		catch (ArgumentNullException)
		{
			// doing good
		}
		try
		{
			foo.LastIndexOf("foo",-1,0);
			Fail("foo.LastIndexOf(\"foo\",-1,0) should throw an ArgumentOutOfRangeException");
		}
		catch (ArgumentOutOfRangeException)
		{
			// even better
		}
		try
		{
			foo.LastIndexOf("foo",0,-1);
			Fail("foo.LastIndexOf(\"foo\",0,-1) should throw an ArgumentOutOfRangeException");
		}
		catch (ArgumentOutOfRangeException)
		{
			// doing great, so far
		}
		try
		{
			foo.LastIndexOf("foo",0,2);
			Fail("foo.LastIndexOf(\"foo\",0,2) should throw an ArgumentOutOfRangeException");
		}
		catch (ArgumentOutOfRangeException)
		{
			// yay! we made it
		}
	}

	public void TestStringLastIndexOfAny()
	{
		//            00000000001111111111222222222
		//            01234567890123456789012345678
		String foo = "Foo Bar foo bar fu bar Fu Bar";

		/* String.LastIndexOfAny(char[]) */
		AssertEquals("foo.LastIndexOfAny(new char[] {'r','a','B'})",28,foo.LastIndexOfAny(new char[] {'r','a','B'}));
		AssertEquals("foo.LastIndexOfAny(new char[] {'f','a','B'})",27,foo.LastIndexOfAny(new char[] {'f','a','B'}));
		AssertEquals("foo.LastIndexOfAny(new char[] {'r'})",28,foo.LastIndexOfAny(new char[] {'r'}));
		AssertEquals("foo.LastIndexOfAny(new char[] {'B'})",26,foo.LastIndexOfAny(new char[] {'B'}));
		AssertEquals("foo.LastIndexOfAny(new char[] {'B','B'})",26,foo.LastIndexOfAny(new char[] {'B','B'}));
		AssertEquals("foo.LastIndexOfAny(new char[] {'B','u'})",26,foo.LastIndexOfAny(new char[] {'B','u'}));
		AssertEquals("foo.LastIndexOfAny(new char[] {'u','B'})",26,foo.LastIndexOfAny(new char[] {'u','B'}));
		AssertEquals("foo.LastIndexOfAny(new char[] {'u','f'})",24,foo.LastIndexOfAny(new char[] {'u','f'}));
		AssertEquals("foo.LastIndexOfAny(new char[] {'F','f'})",23,foo.LastIndexOfAny(new char[] {'F','f'}));
		AssertEquals("foo.LastIndexOfAny(new char[] {'F','q'})",23,foo.LastIndexOfAny(new char[] {'F','q'}));
		AssertEquals("foo.LastIndexOfAny(new char[] {'p','q'})",-1,foo.LastIndexOfAny(new char[] {'p','q'}));
		try
		{
			foo.LastIndexOfAny((char[])null);
			Fail("foo.LastIndexOfAny((char[])null) should throw an ArgumentNullException");
		}
		catch (ArgumentNullException)
		{
			// this is good
		}

		/* String.LastIndexOfAny(char[],int) */
		AssertEquals("foo.LastIndexOfAny(new char[] {'r','a','B'},foo.Length-1)",28,foo.LastIndexOfAny(new char[] {'r','a','B'},foo.Length-1));
		AssertEquals("foo.LastIndexOfAny(new char[] {'r','a','B'},foo.Length-2)",27,foo.LastIndexOfAny(new char[] {'r','a','B'},foo.Length-2));
		AssertEquals("foo.LastIndexOfAny(new char[] {'r','a','B'},4)",4,foo.LastIndexOfAny(new char[] {'r','a','B'},4));
		AssertEquals("foo.LastIndexOfAny(new char[] {'r','a','B'},5)",5,foo.LastIndexOfAny(new char[] {'r','a','B'},5));
		AssertEquals("foo.LastIndexOfAny(new char[] {'r','a','B'},6)",6,foo.LastIndexOfAny(new char[] {'r','a','B'},6));
		AssertEquals("foo.LastIndexOfAny(new char[] {'r','a','B'},1)",-1,foo.LastIndexOfAny(new char[] {'r','a','B'},1));
		AssertEquals("foo.LastIndexOfAny(new char[] {'r','a','B'},0)",-1,foo.LastIndexOfAny(new char[] {'r','a','B'},0));
		AssertEquals("foo.LastIndexOfAny(new char[] {'r','a','B'},3)",-1,foo.LastIndexOfAny(new char[] {'r','a','B'},3));
		try
		{
			foo.LastIndexOfAny((char[])null,0);
			Fail("foo.LastIndexOfAny((char[])null,0) should throw an ArgumentNullException");
		}
		catch (ArgumentNullException)
		{
			// caught it
		}
		try
		{
			foo.LastIndexOfAny(new char[] {'r','a','B'},-1);
			Fail("foo.LastIndexOfAny(new char[] {'r','a','B'},-1); should throw an ArgumentOutOfRangeException");
		}
		catch (ArgumentOutOfRangeException)
		{
			// caught another one
		}
		try
		{
			foo.LastIndexOfAny(new char[] {'r','a','B'},foo.Length);
			Fail("foo.LastIndexOfAny(new char[] {'r','a','B'},foo.Length); should throw an ArgumentOutOfRangeException");
		}
		catch (ArgumentOutOfRangeException)
		{
			// caught this one too
		}
		try
		{
			foo.LastIndexOfAny(new char[] {'r','a','B'},foo.Length+1);
			Fail("foo.LastIndexOfAny(new char[] {'r','a','B'},foo.Length+1); should throw an ArgumentOutOfRangeException");
		}
		catch (ArgumentOutOfRangeException)
		{
			// and this one
		}

		/* String.LastIndexOfAny(char[],int,int) */
		AssertEquals("foo.LastIndexOfAny(new char[] {'r','a','B'},foo.Length-1,1)",28,foo.LastIndexOfAny(new char[] {'r','a','B'},foo.Length-1,1));
		AssertEquals("foo.LastIndexOfAny(new char[] {'r','a','B'},foo.Length-2,2)",27,foo.LastIndexOfAny(new char[] {'r','a','B'},foo.Length-2,2));
		AssertEquals("foo.LastIndexOfAny(new char[] {'B'},foo.Length-1,foo.Length)",26,foo.LastIndexOfAny(new char[] {'B'},foo.Length-1,foo.Length));
		try
		{
			AssertEquals("foo.LastIndexOfAny(new char[] {'F'},foo.Length,foo.Length)",23, foo.LastIndexOfAny(new char[] {'F'},foo.Length,foo.Length));
		}
		catch (ArgumentOutOfRangeException)
		{
			Fail("foo.LastIndexOfAny(new char[] {'F'},foo.Length,foo.Length) should NOT throw an ArgumentOutOfRangeException");
			// flashing some leather
		}
		AssertEquals("foo.LastIndexOfAny(new char[] {'F'},foo.Length-11,foo.Length-10)",0,foo.LastIndexOfAny(new char[] {'F'},foo.Length-11,foo.Length-10));
		AssertEquals("foo.LastIndexOfAny(new char[] {'F'},foo.Length-10,foo.Length-10)",-1,foo.LastIndexOfAny(new char[] {'F'},foo.Length-10,foo.Length-10));
		AssertEquals("foo.LastIndexOfAny(new char[] {'F'},foo.Length+10,foo.Length)",23,foo.LastIndexOfAny(new char[] {'F'},foo.Length+10,foo.Length));
		AssertEquals("foo.LastIndexOfAny(new char[] {'F','o','B','a','r','f','b','u'},foo.Length+10,10)",-1,
		              foo.LastIndexOfAny(new char[] {'F','o','B','a','r','f','b','u'},foo.Length+10,10));
		try
		{
			foo.LastIndexOfAny((char[])null,0,0);
			Fail("foo.LastIndexOfAny((char[])null,0,0) should throw an ArgumentNullException");
		}
		catch (ArgumentNullException)
		{
			// all good here
		}
		try
		{
			foo.LastIndexOfAny(new char[] {'r','a','B'},-1,0);
			Fail("foo.LastIndexOfAny(new char[] {'r','a','B'},-1,0); should throw an ArgumentOutOfRangeException");
		}
		catch (ArgumentOutOfRangeException)
		{
			// and here
		}
		try
		{
			foo.LastIndexOfAny(new char[] {'r','a','B'},0,-1);
			Fail("foo.LastIndexOfAny(new char[] {'r','a','B'},0,-1); should throw an ArgumentOutOfRangeException");
		}
		catch (ArgumentOutOfRangeException)
		{
			// and here too
		}
		try
		{
			foo.LastIndexOfAny(new char[] {'r','a','B'},0,2);
			Fail("foo.LastIndexOfAny(new char[] {'r','a','B'},0,2); should throw an ArgumentOutOfRangeException");
		}
		catch (ArgumentOutOfRangeException)
		{
			// and here as well
		}
	}

	public void TestStringPadLeft()
	{
		String foo = "FooBar";

		/* String.PadLeft(int) */
		AssertEquals("foo.PadLeft(foo.Length)",foo,foo.PadLeft(foo.Length));
		AssertEquals("foo.PadLeft(foo.Length+1)",(" FooBar"),foo.PadLeft(foo.Length+1));
		AssertEquals("foo.PadLeft(foo.Length-1)",foo,foo.PadLeft(foo.Length-1));
		AssertEquals("foo.PadLeft(0)",foo,foo.PadLeft(0));
		try
		{
			foo.PadLeft(-1);
			Fail("foo.PadLeft(-1) should throw an ArgumentException");
		}
		catch (ArgumentException)
		{
			// range check works
		}

		/* String.PadLeft(int,char) */
		AssertEquals("foo.PadLeft(foo.Length,'_')",foo,foo.PadLeft(foo.Length,'_'));
		AssertEquals("foo.PadLeft(foo.Length+1,'_')",("_FooBar"),foo.PadLeft(foo.Length+1,'_'));
		AssertEquals("foo.PadLeft(foo.Length-1,'_')",foo,foo.PadLeft(foo.Length-1,'_'));
		AssertEquals("foo.PadLeft(0,'_')",foo,foo.PadLeft(0,'_'));
		try
		{
			foo.PadLeft(-1,'_');
			Fail("foo.PadLeft(-1,'_') should throw an ArgumentException");
		}
		catch (ArgumentException)
		{
			// range check works here too
		}
	}

	public void TestStringPadRight()
	{
		String foo = "FooBar";

		/* String.PadRight(int) */
		AssertEquals("foo.PadRight(foo.Length)",foo,foo.PadRight(foo.Length));
		AssertEquals("foo.PadRight(foo.Length+1)",("FooBar "),foo.PadRight(foo.Length+1));
		AssertEquals("foo.PadRight(foo.Length-1)",foo,foo.PadRight(foo.Length-1));
		AssertEquals("foo.PadRight(0)",foo,foo.PadRight(0));
		try
		{
			foo.PadRight(-1);
			Fail("foo.PadRight(-1) should throw an ArgumentException");
		}
		catch (ArgumentException)
		{
			// range check works
		}

		/* String.PadRight(int,char) */
		AssertEquals("foo.PadRight(foo.Length,'_')",foo,foo.PadRight(foo.Length,'_'));
		AssertEquals("foo.PadRight(foo.Length+1,'_')",("FooBar_"),foo.PadRight(foo.Length+1,'_'));
		AssertEquals("foo.PadRight(foo.Length-1,'_')",foo,foo.PadRight(foo.Length-1,'_'));
		AssertEquals("foo.PadRight(0,'_')",foo,foo.PadRight(0,'_'));
		try
		{
			foo.PadRight(-1,'_');
			Fail("foo.PadRight(-1,'_') should throw an ArgumentException");
		}
		catch (ArgumentException)
		{
			// range check works here too
		}
	}

	public void TestStringRemove()
	{
		String foo = "Foo Bar";
		AssertEquals("foo.Remove(0,foo.Length)","",foo.Remove(0,foo.Length));
		AssertEquals("foo.Remove(1,foo.Length-1)","F",foo.Remove(1,foo.Length-1));
		AssertEquals("foo.Remove(0,1)","oo Bar",foo.Remove(0,1));
		AssertEquals("foo.Remove(0,0)",foo,foo.Remove(0,0));
		AssertEquals("foo.Remove(foo.Length,0)",foo,foo.Remove(foo.Length,0));
		AssertEquals("foo.Remove(3,1)","FooBar",foo.Remove(3,1));
		AssertEquals("foo.Remove(foo.Length-1,1)","Foo Ba",foo.Remove(foo.Length-1,1));
		try
		{
			foo.Remove(-1,0);
			Fail("foo.Remove(-1,0) should throw an ArgumentOutOfRangeException");
		}
		catch (ArgumentOutOfRangeException)
		{
			// blah
		}
		try
		{
			foo.Remove(0,-1);
			Fail("foo.Remove(0,-1) should throw an ArgumentOutOfRangeException");
		}
		catch (ArgumentOutOfRangeException)
		{
			// blah blah
		}
		try
		{
			foo.Remove(0,foo.Length+1);
			Fail("foo.Remove(0,foo.Length+1) should throw an ArgumentOutOfRangeException");
		}
		catch (ArgumentOutOfRangeException)
		{
			// blah blah blah
		}
	}

	public void TestStringReplace()
	{
		String foo = "Foo Bar";

		/* String.Replace(char,char) */
		AssertEquals("foo.Replace('F','f')","foo Bar",foo.Replace('F','f'));
		AssertEquals("foo.Replace(' ','_')","Foo_Bar",foo.Replace(' ','_'));
		AssertEquals("foo.Replace('r','R')","Foo BaR",foo.Replace('r','R'));
		AssertEquals("foo.Replace('_',' ')",foo,foo.Replace('_',' '));

		/* String.Replace(String,String) */
		AssertEquals("foo.Replace(\"Foo\",\"Fu\")","Fu Bar",foo.Replace("Foo","Fu"));
		AssertEquals("foo.Replace(\"Fu\",\"Foo\")",foo,foo.Replace("Fu","Foo"));
		AssertEquals("foo.Replace(\"Foo Bar\",\"\")","",foo.Replace("Foo Bar",""));
		AssertEquals("foo.Replace(\"Foo Bar\",null)","",foo.Replace("Foo Bar",null));
		// Behavior changed by MS in 2.0 (Rich: they want to keep us busy)
		try
		{
			foo.Replace(null,"Foo Bar");
			Fail("foo.Replace(null,\"Foo Bar\") should have thrown an ArgumentNullException");
		}
		catch(ArgumentNullException)
		{
			// SUCCESS
		}
	}

	public void TestStringSubstring()
	{
		// if you pass Length, SHOULD NOT THROW!
		// per ECMA spec
		try
		{
			"x".Substring(1, 0);
			"x".Substring(1);
		}
		catch (ArgumentOutOfRangeException)
		{
			Fail("Substring should not throw when passed Length as the startIndex!");
		}
	}


	public void TestStringop_Equality()
	{
		String foo = "Foo Bar";
		String fu = "Fu Bar";
		Assert("!(foo == fu)",!(foo == fu));
		Assert("foo == foo",foo == foo);
		Assert("fu == fu",fu == fu);
		Assert("foo == String.Copy(foo)",foo == String.Copy(foo));
		Assert("fu == String.Copy(fu)",fu == String.Copy(fu));
	}

	public void TestStringop_Inequality()
	{
		String foo = "Foo Bar";
		String fu = "Fu Bar";
		Assert("foo != fu",foo != fu);
		Assert("!(foo != foo)",!(foo != foo));
		Assert("!(fu != fu)",!(fu != fu));
		Assert("!(foo != String.Copy(foo))",!(foo != String.Copy(foo)));
		Assert("!(fu != String.Copy(fu))",!(fu != String.Copy(fu)));
	}

	public void TestStringChars()
	{
		char[] fu = new char[] { 'F', 'o', 'o', ' ', 'B', 'a', 'r' };
		String foo = new String(fu);
		for (int i = 0; i < foo.Length; i++)
		{
			Assert("foo["+i+"] == fu["+i+"]",foo[i] == fu[i]);
		}
		try
		{
			int i = foo[-1];
			Fail("foo[-1] should throw an IndexOutOfRangeException");
		}
		catch (IndexOutOfRangeException)
		{
			// works here
		}
		try
		{
			int i = foo[foo.Length];
			Fail("foo[foo.Length] should throw an IndexOutOfRangeException");
		}
		catch (IndexOutOfRangeException)
		{
			// and here
		}
		try
		{
			int i = foo[foo.Length+1];
			Fail("foo[foo.Length+1] should throw an IndexOutOfRangeException");
		}
		catch (IndexOutOfRangeException)
		{
			// and here
		}
	}
	public void TestStringLength()
	{
		AssertEquals("\"Foo Bar\".Length","Foo Bar".Length,7);
		AssertEquals("\"\".Length","".Length,0);
	}
	public void TestStringToCharArray()
	{
		char[] foo;
		String bar;
		foo = String.Empty.ToCharArray();
		/* If length is 0 the startindex has to be ignored */
		foo = String.Empty.ToCharArray(10, 0);
		bar = "abc";
		foo = bar.ToCharArray(2, 0);
		AssertEquals("\"abc\".ToCharArray(2, 0)",foo.Length, 3);
	}
}; // class TestString
