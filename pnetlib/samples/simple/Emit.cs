/*
 * Emit.cs - Sample program for System.Reflection.Emit.
 *
 * Copyright (C) 2003  Free Software Foundation, Inc.
 *
 * This program is free software, you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY, without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program, if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;

#if CONFIG_REFLECTION_EMIT

public class XYZ
{
	// Generates the following class dynamically and outputs to <name>.exe:
	//
	//	public class <name>Type
	//	{
	//		private int x, y, z;
	//
	//		public <name>Type(int x, int y, int z) : base()
	//		{
	//			this.x = x;
	//			this.y = y;
	//			this.z = z;
	//		}
	//
	//		public int <name>Method()
	//		{
	//			x = (x * y) % z;
	//			return x;
	//		}
	//
	//		public static void <name>EntryPoint()
	//		{
	//			int x, y, z;
	//			<name>Type <name>Local;
	//
	//			x = 1;
	//			y = 2;
	//			z = 3;
	//			<name>Local = new <name>Type(x, y, z);
	//			Console.WriteLine("(x:1 * y:2) % z:3 == " +
	//			                  <name>Local.<name>Method());
	//		}
	//	}
	//
	public static void xyz(String name)
	{
		AppDomain domain;
		AssemblyName asmName;
		AssemblyBuilder asmBuilder;
		ModuleBuilder modBuilder;
		TypeBuilder typeBuilder;

		// Begin building our dynamic assembly
		domain = Thread.GetDomain();
		asmName = new AssemblyName();
		asmName.Name = name+"Assembly";
		asmBuilder = domain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Save, "./");
		modBuilder = asmBuilder.DefineDynamicModule(name+"Module");
		typeBuilder = modBuilder.DefineType(name+"Type", TypeAttributes.Public);

		FieldBuilder fieldX;
		FieldBuilder fieldY;
		FieldBuilder fieldZ;

		// Create a few fields for our dynamic type
		fieldX = typeBuilder.DefineField("x", typeof(int), FieldAttributes.Private);
		fieldY = typeBuilder.DefineField("y", typeof(int), FieldAttributes.Private);
		fieldZ = typeBuilder.DefineField("z", typeof(int), FieldAttributes.Private);

		ConstructorBuilder ctorBuilder;
		ILGenerator ctorILGen;
		Type[] ctorParams = new Type[]
		{
			typeof(int),
			typeof(int),
			typeof(int)
		};

		// Build the constructor for our dynamic type
		ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public,
		                                            CallingConventions.Standard,
		                                            ctorParams);
		ctorILGen = ctorBuilder.GetILGenerator();
		ctorILGen.Emit(OpCodes.Ldarg_0);
		ctorILGen.Emit(OpCodes.Call, (typeof(object)).GetConstructor(new Type[0]));
		ctorILGen.Emit(OpCodes.Ldarg_0);
		ctorILGen.Emit(OpCodes.Ldarg_1);
		ctorILGen.Emit(OpCodes.Stfld, fieldX);
		ctorILGen.Emit(OpCodes.Ldarg_0);
		ctorILGen.Emit(OpCodes.Ldarg_2);
		ctorILGen.Emit(OpCodes.Stfld, fieldY);
		ctorILGen.Emit(OpCodes.Ldarg_0);
		ctorILGen.Emit(OpCodes.Ldarg_3);
		ctorILGen.Emit(OpCodes.Stfld, fieldZ);
		ctorILGen.Emit(OpCodes.Ret);

		MethodBuilder methodBuilder;
		ILGenerator methodILGen;
		Type[] methodParams = new Type[0];

		// Build an instance method for our dynamic type
		methodBuilder = typeBuilder.DefineMethod(name+"Method",
		                                         MethodAttributes.Public,
		                                         typeof(int),
		                                         methodParams);
		methodILGen = methodBuilder.GetILGenerator();
		methodILGen.Emit(OpCodes.Ldarg_0);
		methodILGen.Emit(OpCodes.Ldarg_0);
		methodILGen.Emit(OpCodes.Ldfld, fieldX);
		methodILGen.Emit(OpCodes.Ldarg_0);
		methodILGen.Emit(OpCodes.Ldfld, fieldY);
		methodILGen.Emit(OpCodes.Mul);
		methodILGen.Emit(OpCodes.Ldarg_0);
		methodILGen.Emit(OpCodes.Ldfld, fieldZ);
		methodILGen.Emit(OpCodes.Rem);
		methodILGen.Emit(OpCodes.Stfld, fieldX);
		methodILGen.Emit(OpCodes.Ldarg_0);
		methodILGen.Emit(OpCodes.Ldfld, fieldX);
		methodILGen.Emit(OpCodes.Ret);

		MethodBuilder entryBuilder;
		ILGenerator entryILGen;
		LocalBuilder localX;
		LocalBuilder localY;
		LocalBuilder localZ;
		LocalBuilder localT;
		MethodInfo writeln;
		MethodInfo concat;
		Type[] entryParams = new Type[0];
		Type[] writelnParams = new Type[]
		{
			typeof(string)
		};
		Type[] concatParams = new Type[]
		{
			typeof(object),
			typeof(object)
		};

		// Get some methods for use in our entry point
		writeln = (typeof(Console)).GetMethod("WriteLine", writelnParams);
		concat = (typeof(string)).GetMethod("Concat", concatParams);

		// Build the entry point for our dynamic assembly
		entryBuilder = typeBuilder.DefineMethod(name+"EntryPoint",
		                                        MethodAttributes.Public |
		                                        MethodAttributes.Static,
		                                        typeof(void),
		                                        entryParams);
		entryILGen = entryBuilder.GetILGenerator();
		localX = entryILGen.DeclareLocal(typeof(int));
		localX.SetLocalSymInfo("x");
		localY = entryILGen.DeclareLocal(typeof(int));
		localY.SetLocalSymInfo("y");
		localZ = entryILGen.DeclareLocal(typeof(int));
		localZ.SetLocalSymInfo("z");
		localT = entryILGen.DeclareLocal(typeBuilder);
		localT.SetLocalSymInfo(name+"Local");
		entryILGen.Emit(OpCodes.Ldc_I4_1);
		entryILGen.Emit(OpCodes.Stloc_0);
		entryILGen.Emit(OpCodes.Ldc_I4_2);
		entryILGen.Emit(OpCodes.Stloc_S, localY);
		entryILGen.Emit(OpCodes.Ldc_I4_3);
		entryILGen.Emit(OpCodes.Stloc_2);
		entryILGen.Emit(OpCodes.Ldloc_0);
		entryILGen.Emit(OpCodes.Ldloc_1);
		entryILGen.Emit(OpCodes.Ldloc_2);
		entryILGen.Emit(OpCodes.Newobj, ctorBuilder);
		entryILGen.Emit(OpCodes.Stloc_3);
		entryILGen.Emit(OpCodes.Ldstr, "(x:1 * y:2) % z:3 == ");
		entryILGen.Emit(OpCodes.Ldloc_S, localT);
		entryILGen.EmitCall(OpCodes.Callvirt, (MethodInfo)methodBuilder, null);
		entryILGen.Emit(OpCodes.Box, typeof(System.Int32));
		entryILGen.EmitCall(OpCodes.Call, concat, null);
		entryILGen.EmitCall(OpCodes.Call, writeln, null);
		entryILGen.Emit(OpCodes.Ret);

		// Set the entry point for our dynamic assembly
		asmBuilder.SetEntryPoint(entryBuilder, PEFileKinds.ConsoleApplication);

		// Finalize our type and save our assembly
		typeBuilder.CreateType();
		asmBuilder.Save(name+".exe");
	}

	public static void Main()
	{
		XYZ.xyz("MyEmitTest");
	}
}

#else

public class XYZ
{
	public static void Main()
	{
	}
}

#endif
