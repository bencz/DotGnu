/*
 * TestGlobal.cs - Tests for the global object.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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
using Microsoft.JScript;
using Microsoft.JScript.Vsa;
using Microsoft.Vsa;

public class TestGlobal : TestCase
{
	// Constructor.
	public TestGlobal(String name)
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

	// Test the "escape" function.
	public void TestGlobalEscape()
			{
				AssertEquals("Escape (1)", "http%3A//foo.com",
							 GlobalObject.escape("http://foo.com"));
				AssertEquals("Escape (2)", "foo.com/nowhere.html",
							 GlobalObject.escape("foo.com/nowhere.html"));
				AssertEquals("Escape (3)", "%AB%uABCD",
							 GlobalObject.escape("\u00AB\uABCD"));
				AssertEquals("Escape (4)",
							 "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
							 "abcdefghijklmnopqrstuvwxyz" +
							 "0123456789@*_+-./",
							 GlobalObject.escape
							 	("ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
							 	 "abcdefghijklmnopqrstuvwxyz" +
							 	 "0123456789@*_+-./"));
			}

	// Test the "unescape" function.
	public void TestGlobalUnescape()
			{
				AssertEquals("Unescape (1)", "http://foo.com",
							 GlobalObject.unescape("http%3A//foo.com"));
				AssertEquals("Unescape (2)", "foo.com/nowhere.html",
							 GlobalObject.unescape("foo.com/nowhere.html"));
				AssertEquals("Unescape (3)", "\u00AB\uABCD",
							 GlobalObject.unescape("%AB%uABCD"));
				AssertEquals("Unescape (4)",
							 "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
							 "abcdefghijklmnopqrstuvwxyz" +
							 "0123456789@*_+-./",
							 GlobalObject.unescape
							 	("ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
							 	 "abcdefghijklmnopqrstuvwxyz" +
							 	 "0123456789@*_+-./"));
			}

	// Test the "encodeURI" function.
	public void TestGlobalEncodeURI()
			{
				AssertEquals("EncodeURI (1)", "http://foo.com",
							 GlobalObject.encodeURI("http://foo.com"));
				AssertEquals("EncodeURI (2)", "foo.com/nowhere.html",
							 GlobalObject.encodeURI("foo.com/nowhere.html"));
				AssertEquals("EncodeURI (3)", "%C2%AB%EA%AF%8D",
							 GlobalObject.encodeURI("\u00AB\uABCD"));
				AssertEquals("EncodeURI (4)",
							 "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
							 "abcdefghijklmnopqrstuvwxyz" +
							 "0123456789@*_+-./",
							 GlobalObject.encodeURI
							 	("ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
							 	 "abcdefghijklmnopqrstuvwxyz" +
							 	 "0123456789@*_+-./"));
			}

	// Test the "encodeURIComponent" function.
	public void TestGlobalEncodeURIComponent()
			{
				AssertEquals("EncodeURIComponent (1)", "http%3A%2F%2Ffoo.com",
							 GlobalObject.encodeURIComponent
							 	("http://foo.com"));
				AssertEquals("EncodeURIComponent (2)", "foo.com%2Fnowhere.html",
							 GlobalObject.encodeURIComponent
							 	("foo.com/nowhere.html"));
				AssertEquals("EncodeURIComponent (3)", "%C2%AB%EA%AF%8D",
							 GlobalObject.encodeURIComponent("\u00AB\uABCD"));
				AssertEquals("EncodeURIComponent (4)",
							 "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
							 "abcdefghijklmnopqrstuvwxyz" +
							 "0123456789%40*_%2B-.%2F",
							 GlobalObject.encodeURIComponent
							 	("ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
							 	 "abcdefghijklmnopqrstuvwxyz" +
							 	 "0123456789@*_+-./"));
			}

	// Test the "decodeURI" function.
	public void TestGlobalDecodeURI()
			{
				AssertEquals("DecodeURI (1)", "http%3A//foo.com",
							 GlobalObject.decodeURI("http%3A//foo.com"));
				AssertEquals("DecodeURI (2)", "foo.com/nowhere.html",
							 GlobalObject.decodeURI("foo.com/nowhere.html"));
				AssertEquals("DecodeURI (3)", "\u00AB\uABCD",
							 GlobalObject.decodeURI("%C2%AB%EA%AF%8D"));
				AssertEquals("DecodeURI (4)",
							 "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
							 "abcdefghijklmnopqrstuvwxyz" +
							 "0123456789@*_+-./",
							 GlobalObject.decodeURI
							 	("ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
							 	 "abcdefghijklmnopqrstuvwxyz" +
							 	 "0123456789@*_+-./"));
			}

	// Test the "decodeURIComponent" function.
	public void TestGlobalDecodeURIComponent()
			{
				AssertEquals("DecodeURIComponent (1)", "http://foo.com",
							 GlobalObject.decodeURIComponent
							 		("http%3A//foo.com"));
				AssertEquals("DecodeURIComponent (2)", "foo.com/nowhere.html",
							 GlobalObject.decodeURIComponent
							 		("foo.com/nowhere.html"));
				AssertEquals("DecodeURIComponent (3)", "\u00AB\uABCD",
							 GlobalObject.decodeURIComponent
							 		("%C2%AB%EA%AF%8D"));
				AssertEquals("DecodeURIComponent (4)",
							 "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
							 "abcdefghijklmnopqrstuvwxyz" +
							 "0123456789@*_+-./",
							 GlobalObject.decodeURIComponent
							 	("ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
							 	 "abcdefghijklmnopqrstuvwxyz" +
							 	 "0123456789@*_+-./"));
			}

	// Test the "isFinite" function.
	public void TestGlobalIsFinite()
			{
				Assert("IsFinite (1)", !GlobalObject.isFinite(Double.NaN));
				Assert("IsFinite (2)",
					   !GlobalObject.isFinite(Double.PositiveInfinity));
				Assert("IsFinite (3)",
					   !GlobalObject.isFinite(Double.NegativeInfinity));
				Assert("IsFinite (4)",
					   GlobalObject.isFinite(Double.MaxValue));
				Assert("IsFinite (5)",
					   GlobalObject.isFinite(Double.MinValue));
				Assert("IsFinite (6)",
					   GlobalObject.isFinite(Double.Epsilon));
				Assert("IsFinite (7)",
					   GlobalObject.isFinite(-1.234));
			}

	// Test the "isNaN" function.
	public void TestGlobalIsNaN()
			{
				Assert("IsNaN (1)", GlobalObject.isNaN(Double.NaN));
				Assert("IsNaN (2)",
					   !GlobalObject.isNaN(Double.PositiveInfinity));
				Assert("IsNaN (3)",
					   !GlobalObject.isNaN(Double.NegativeInfinity));
				Assert("IsNaN (4)",
					   !GlobalObject.isNaN(Double.MaxValue));
				Assert("IsNaN (5)",
					   !GlobalObject.isNaN(Double.MinValue));
				Assert("IsNaN (6)",
					   !GlobalObject.isNaN(Double.Epsilon));
				Assert("IsNaN (7)",
					   !GlobalObject.isNaN(-1.234));
			}

	// Test the "parseFloat" function.
	public void TestGlobalParseFloat()
			{
				AssertEquals("ParseFloat (1)", 0.123,
							 GlobalObject.parseFloat("0.123"), 0.0001);
				AssertEquals("ParseFloat (2)", 0.0e12,
							 GlobalObject.parseFloat("0.e12"), 0.0001);
				AssertEquals("ParseFloat (3)", 154.01e-12,
							 GlobalObject.parseFloat("154.01e-12"), 0.0001);
				Assert("ParseFloat (4)",
					   Double.IsPositiveInfinity
					   		(GlobalObject.parseFloat(" Infinityx")));
				Assert("ParseFloat (5)",
					   Double.IsNegativeInfinity
					   		(GlobalObject.parseFloat(" -Infinity %")));
				Assert("ParseFloat (6)",
					   Double.IsNaN
					   		(GlobalObject.parseFloat("hello")));
				Assert("ParseFloat (7)",
					   Double.IsNaN
					   		(GlobalObject.parseFloat("NaN")));
				Assert("ParseFloat (8)",
					   Double.IsNaN
					   		(GlobalObject.parseFloat("154.e")));
			}

	// Test the "parseInt" function.
	public void TestGlobalParseInt()
			{
				AssertEquals("ParseInt (1)", 123,
							 GlobalObject.parseInt("123", 10), 0.0001);
				AssertEquals("ParseInt (2)", 123,
							 GlobalObject.parseInt("123.5", 0), 0.0001);
				AssertEquals("ParseInt (3)", 0x123,
							 GlobalObject.parseInt("0x123", 0), 0.0001);
				AssertEquals("ParseInt (4)", -0x123,
							 GlobalObject.parseInt("-0x123", 0), 0.0001);
				AssertEquals("ParseInt (5)", 0,
							 GlobalObject.parseInt("0x123", 6), 0.0001);
				Assert("ParseInt (6)",
					   Double.IsNaN(GlobalObject.parseInt("", 0)));
				Assert("ParseInt (7)",
					   Double.IsNaN(GlobalObject.parseInt("abc", 4)));
			}

	// Test the "parseInt" function inside an evaluation.
	public void TestGlobalEvalParseInt()
			{
				VsaEngine engine;
				IVsaCodeItem item;

				// Create a new engine instance.
				engine = VsaEngine.CreateEngine();

				// Compile an empty script.
				item = (IVsaCodeItem)(engine.Items.CreateItem
						("script1", VsaItemType.Code, VsaItemFlag.None));
				item.SourceText = "";
				Assert("Compile", engine.Compile());
				engine.Run();

				// Evaluate a "parseInt" call within the engine context.
				AssertEquals("EvalParseInt (1)", 123.0,
							 (double)(Eval.JScriptEvaluate
							 			("parseInt(\"123\", 0)", engine)),
							 0.0001);
				AssertEquals("EvalParseInt (2)", 123.0,
							 (double)(Eval.JScriptEvaluate
							 			("parseInt(\"123\")", engine)),
							 0.0001);

				// Close the engine.
				engine.Close();
			}

}; // class TestGlobal
