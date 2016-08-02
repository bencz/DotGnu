/*
 * TestEngine.cs - Tests for the "VsaEngine" class.
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

public class TestEngine : TestCase
{
	// Constructor.
	public TestEngine(String name)
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

	// Test engine creation.
	public void TestEngineCreate()
			{
				VsaEngine engine;
				VsaEngine engine2;

				// Create a new engine instance.
				engine = VsaEngine.CreateEngine();
				AssertNotNull("Create (1)", engine);
				AssertEquals("Create (2)", engine, Globals.contextEngine);

				// Try to create another (returns same instance).
				engine2 = VsaEngine.CreateEngine();
				AssertEquals("Create (3)", engine, engine2);

				// Close the engine.
				engine.Close();
				AssertNull("Create (4)", Globals.contextEngine);

				// Create a new engine, which should be different this time.
				engine = VsaEngine.CreateEngine();
				Assert("Create (5)", engine2 != engine);
				AssertNotNull("Create (6)", engine);
				engine.Close();
			}

	// Check that a script function object looks correct.
	private void CheckScriptFunction(String name, Object obj, int length)
			{
				Assert("CSF (" + name + ") (1)", obj is ScriptFunction);
				ScriptFunction func = (obj as ScriptFunction);
				AssertEquals("CSF (" + name + ") (2)",
							 "function " + name + "() {\n" +
							 "    [native code]\n}",
					   		 func.ToString());
				AssertEquals("CSF (" + name + ") (3)",
							 length, func.length);
			}

	// Test the global object properties.
	public void TestEngineGlobals()
			{
				// Create a new engine instance.
				VsaEngine engine = VsaEngine.CreateEngine();

				// Get the global object.
				LenientGlobalObject global = engine.LenientGlobalObject;
				AssertNotNull("Globals (1)", global);

				// Check that the "Object" and "Function" objects
				// appear to be of the right form.
				Object objectConstructor = global.Object;
				AssertNotNull("Globals (2)", objectConstructor);
				Assert("Globals (3)", objectConstructor is ObjectConstructor);
				Object functionConstructor = global.Function;
				AssertNotNull("Globals (4)", functionConstructor);
				Assert("Globals (5)",
					   functionConstructor is FunctionConstructor);

				// Check the type information.
				AssertSame("Type (1)", typeof(Boolean), global.boolean);
				AssertSame("Type (2)", typeof(Byte), global.@byte);
				AssertSame("Type (3)", typeof(SByte), global.@sbyte);
				AssertSame("Type (4)", typeof(Char), global.@char);
				AssertSame("Type (5)", typeof(Int16), global.@short);
				AssertSame("Type (6)", typeof(UInt16), global.@ushort);
				AssertSame("Type (7)", typeof(Int32), global.@int);
				AssertSame("Type (8)", typeof(UInt32), global.@uint);
				AssertSame("Type (9)", typeof(Int64), global.@long);
				AssertSame("Type (10)", typeof(UInt64), global.@ulong);
				AssertSame("Type (11)", typeof(Single), global.@float);
				AssertSame("Type (12)", typeof(Double), global.@double);
				AssertSame("Type (13)", typeof(Decimal), global.@decimal);
				AssertSame("Type (14)", typeof(Void), global.@void);

				// Check the global functions.
				CheckScriptFunction("CollectGarbage",
									global.CollectGarbage, 0);
				CheckScriptFunction("decodeURI",
									global.decodeURI, 1);
				CheckScriptFunction("decodeURIComponent",
									global.decodeURIComponent, 1);
				CheckScriptFunction("encodeURI",
									global.encodeURI, 1);
				CheckScriptFunction("encodeURIComponent",
									global.encodeURIComponent, 1);
				CheckScriptFunction("escape",
									global.escape, 1);
				CheckScriptFunction("eval",
									global.eval, 1);
				CheckScriptFunction("isFinite",
									global.isFinite, 1);
				CheckScriptFunction("isNaN",
									global.isNaN, 1);
				CheckScriptFunction("parseFloat",
									global.parseFloat, 1);
				CheckScriptFunction("parseInt",
									global.parseInt, 2);
				CheckScriptFunction("ScriptEngine",
									global.ScriptEngine, 0);
				CheckScriptFunction("ScriptEngineBuildVersion",
									global.ScriptEngineBuildVersion, 0);
				CheckScriptFunction("ScriptEngineMajorVersion",
									global.ScriptEngineMajorVersion, 0);
				CheckScriptFunction("ScriptEngineMinorVersion",
									global.ScriptEngineMinorVersion, 0);
				CheckScriptFunction("unescape",
									global.unescape, 1);

				// Check the global values.
				Assert("Value (1)",
					   Double.IsPositiveInfinity((double)(global.Infinity)));
				Assert("Value (2)", Double.IsNaN((double)(global.NaN)));
				AssertNull("Value (3)", global.undefined);

				// Close the engine and exit.
				engine.Close();
			}

}; // class TestEngine
