/*
 * TestEmit.cs - Test class for "System.Reflection.Emit" 
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
 * 
 * Authors : Jonas Printzen <jonas@printzen.net> 
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
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

#if CONFIG_REFLECTION_EMIT
interface ISomeMethods
{
	void nop();
	int inc( int i );
	int dec( int i );

	void   SetTag( string tag );
	string GetTag();

	void ThrowMe( bool bThrow );
}

public class TestEmit : TestCase
{
	private string timestamp=null;
 
	AppDomain       appdomain;
	AssemblyBuilder assemblybuilder;
	ModuleBuilder   modulebuilder;
	TypeBuilder     typebuilder;

	// Constructor.
	public TestEmit(String name)
		: base(name)
	{
		timestamp=DateTime.Now.ToString( "yyyy-MM-dd_HH:mm", null );
	}

	// Set up for the tests.
	protected override void Setup()
	{
		// Remove saved assembly
		File.Delete("Simple.dll");
		File.Delete("SomeMethods.dll");
	}

	// Clean up after the tests.
	protected override void Cleanup()
	{
		// Remove saved assembly
		File.Delete("Simple.dll");
		File.Delete("SomeMethods.dll");
	}

	//==== Tests =====================================================

	public void TestSimpleSave()
	{
		// Create Simple assembly
		CreateSimple( true );

		// Save the assembly
		assemblybuilder.Save("Simple.dll");

		// OK! Built simple assembly, let's use it...
		Assembly SimpleAsm = Assembly.LoadFrom("Simple.dll");
		Type simple = SimpleAsm.GetType("Simple");

		simple.InvokeMember
		(
			"Run", 
			BindingFlags.InvokeMethod|BindingFlags.Public|BindingFlags.Static,
			null,null,null 
		);
	}

#if FALSE
	public void TestSimpleRun()
	{
		Type simple = CreateSimple( false );

		simple.InvokeMember
			(
			"Run", 
			BindingFlags.InvokeMethod|BindingFlags.Public|BindingFlags.Static,
			null,null,null 
			);
	}
#endif

	// Test SomeMethods using interface-access, Save
	public void TestSomeMethodsSave()
	{
		// Create for save
		CreateSomeMethods( true );

		// Save the assembly
		assemblybuilder.Save("SomeMethods.dll");

		// OK! Built the assembly, let's use it...
		Assembly SomeAsm = Assembly.LoadFrom("SomeMethods.dll");
		Type t = SomeAsm.GetType("SomeMethods");

		// Make an instance...
		ISomeMethods i = (ISomeMethods)(SomeAsm.CreateInstance("SomeMethods"));

		// Test the generated code...
		VerifySomeMethods( i );
	}

#if FALSE
	// Test SomeMethods using interface-access, Run
	public void TestSomeMethodsRun()
	{
		Type t = CreateSomeMethods( false );

		// Create instance
		object io = Activator.CreateInstance(t);
		ISomeMethods i = (ISomeMethods)io; 

		// Test the generated code...
		VerifySomeMethods( i );
	}
#endif

	//==== Helpers ===================================================

	// Prepare an assembly
	private void PrepareAssembly( string typeName, bool bSave )
	{
		AssemblyBuilderAccess acc = bSave ? AssemblyBuilderAccess.Save:AssemblyBuilderAccess.Run;
		AssemblyName aName = new AssemblyName();
		aName.Name = typeName;

		appdomain = AppDomain.CurrentDomain;
		assemblybuilder = appdomain.DefineDynamicAssembly( aName, acc );
		modulebuilder = assemblybuilder.DefineDynamicModule( typeName+".dll" );
		typebuilder = modulebuilder.DefineType( typeName, TypeAttributes.Public );
	}

	private Type CreateSimple( bool bSave )
	{
		// Setup an assembly...
		PrepareAssembly( "Simple", bSave );

		// Create a default constructor
		AddConstr( typebuilder );

		// Create one public static method
		MethodBuilder method = 
			typebuilder.DefineMethod
			(
			"Run",
			MethodAttributes.Static|MethodAttributes.Public,
			typeof(void),
			new Type[0]
			);

		// Method body
		ILGenerator il = method.GetILGenerator();

		il.Emit( OpCodes.Nop );
		il.Emit( OpCodes.Ret );

		// Bake
		return typebuilder.CreateType();
	}

	private Type CreateSomeMethods( bool bSave )
	{
		// Setup an assembly...
		PrepareAssembly( "SomeMethods", bSave );

		// Create a default constructor
		AddConstr( typebuilder );

		// Implement the methods in ISomeMethods
		ImplementSomeMethods( true );

		// Bake
		return typebuilder.CreateType();
	}

	// Create dummy constructor in given type
	private void AddConstr( TypeBuilder t )
	{
		// Base is Object...
		Type obj = Type.GetType("System.Object");
		// Constructor in Object to call...
		ConstructorInfo oci = obj.GetConstructor(new Type[0]);

		// Start on the actual constructor..
		ConstructorBuilder cb = t.DefineConstructor
		(
			MethodAttributes.Public,
			CallingConventions.Standard,
			new Type[0]
		);

		// Generate code...
		ILGenerator il = cb.GetILGenerator();
		il.Emit(OpCodes.Ldarg_0);
		il.Emit(OpCodes.Call, oci);
		il.Emit(OpCodes.Ret);
	}

	// Implement ISomeMethods in the prepared assembly
	private void ImplementSomeMethods( bool bIF )
	{
		ILGenerator il=null;
		MethodBuilder method=null;

		// Create a private string m_txt
		FieldBuilder fb = typebuilder.DefineField("m_txt",typeof(string),FieldAttributes.Private );

		MethodAttributes attr = MethodAttributes.Public;
		// Implement as interface
		if( bIF )
		{
			typebuilder.AddInterfaceImplementation(typeof(ISomeMethods));
			attr |= MethodAttributes.Virtual;
		}

		// void nop();
		method = typebuilder.DefineMethod( "nop", attr, typeof(void), new Type[0] );
		il = method.GetILGenerator();
		il.Emit(OpCodes.Nop);
		il.Emit(OpCodes.Ret);

		// int inc( int )
		method = typebuilder.DefineMethod( "inc", attr, typeof(int), new Type[]{typeof(int)} );
		il = method.GetILGenerator();
		il.Emit(OpCodes.Ldarg_1);
		il.Emit(OpCodes.Ldc_I4_1 );
		il.Emit(OpCodes.Add );
		il.Emit(OpCodes.Ret );

		// int dec( int )
		method = typebuilder.DefineMethod( "dec", attr, typeof(int), new Type[]{typeof(int)} );
		il = method.GetILGenerator();
		il.Emit(OpCodes.Ldarg_1);
		il.Emit(OpCodes.Ldc_I4_1 );
		il.Emit(OpCodes.Sub );
		il.Emit(OpCodes.Ret );

		// void SetTag( string )
		method = typebuilder.DefineMethod( "SetTag", attr, typeof(void), new Type[]{typeof(string)} );
		il = method.GetILGenerator();
		il.Emit(OpCodes.Ldarg_0);
		il.Emit(OpCodes.Ldarg_1);
		il.Emit(OpCodes.Stfld, fb );
		il.Emit(OpCodes.Ret );

		// void GetTag( string )
		method = typebuilder.DefineMethod( "GetTag", attr, typeof(string), new Type[0] );
		il = method.GetILGenerator();
		il.Emit(OpCodes.Ldarg_0);
		il.Emit(OpCodes.Ldfld, fb );
		il.Emit(OpCodes.Ret );

		// void ThrowMe( bool )
		method = typebuilder.DefineMethod( "ThrowMe", attr, typeof(void), new Type[]{typeof(bool)} );
		il = method.GetILGenerator();
		Label skipException = il.DefineLabel();
		il.Emit(OpCodes.Ldarg_1);
		il.Emit(OpCodes.Brfalse_S, skipException );
		il.Emit(OpCodes.Ldstr, "BenignException" );
		il.Emit(OpCodes.Newobj, typeof(Exception).GetConstructor(new Type[]{typeof(string)}) );
		il.Emit(OpCodes.Throw );
		il.MarkLabel(skipException);
		il.Emit(OpCodes.Ret);
	}

	private void VerifySomeMethods( ISomeMethods ism )
	{
		ism.nop();

		AssertEquals( "inc(0) => 1", 1, ism.inc(0) );
		AssertEquals( "dec(1) => 0", 0, ism.dec(1) );

		ism.SetTag("Hello");
		AssertEquals("GetTag() => 'Hello'", "Hello", ism.GetTag() );

		ism.ThrowMe( false );

		try
		{
			ism.ThrowMe( true );
			Fail("ThrowMe(true) EXPECTED Exception!");
		}
		catch( Exception )
		{
			//OK
		}
	}
}
#endif // CONFIG_REFLECTION_EMIT

