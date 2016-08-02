//
// This is a copy of the sample program from Annex A of the ECMA draft
// "Common Language Infrastructure (CLI) Part 5: Annexes", Draft 01,
// October 2000.  We use this to test that the assembler understands
// the syntax in the draft.
//

.assembly extern mscorlib
{
	.ver 1:0:2411:0
}

.assembly evenodd { }

//Global method

.method public static void main() il managed
{ .entrypoint
  .maxstack	1
  ldc.i4.5
  call			void EvenOdd::Test(int32)
  ldc.i4.2
  call			void EvenOdd::Test(int32)
  ldc.i4		100
  call			void EvenOdd::Test(int32)
  ldc.i4		1000001
  call			void EvenOdd::Test(int32)
  ret
} // end of global method `main'

.class EvenOdd extends [mscorlib]System.Object
{ .method private static bool IsEven(int32 N) il managed
  { .maxstack	2
	ldarg.0					// N
	ldc.i4.0
	bne.un		NonZero
	ldc.i4.1
	ret
NonZero:
	ldarg.0
	ldc.i4.1
	sub
	tail.
	call bool EvenOdd::IsOdd(int32)
	ret
  } // end of method `EvenOdd::IsEven'

  .method private static bool IsOdd(int32 N) il managed
  { .maxstack	2
  	// Demonstrates use of argument names and labels
	// Notice that the assembler does not convert these
	// automatically to their short versions
	//
	// Update: the above comment is from the draft and
	// refers to Microsoft's assembler.  Ours does
	// convert argument names and labels to shorter
	// versions unless "--no-short-insns" is supplied.
	ldarg		N
	ldc.i4.0
	bne.un		NonZero
	ldc.i4.0
	ret
NonZero:
	ldarg		N
	ldc.i4.1
	sub
	tail.
	call bool EvenOdd::IsEven(int32)
	ret
  } // end of method `EvenOdd::IsOdd'

  .method public static void Test(int32 N) il managed
  { .maxstack	1
  	ldarg		N
	call		void [mscorlib]System.Console::Write(int32)
	ldstr		" is "
	call		void [mscorlib]System.Console::Write(class [mscorlib]System.String)
	ldarg		N
	call		bool EvenOdd::IsEven(int32)
	brfalse		LoadOdd
	ldstr		"even"
Print:
	call		void [mscorlib]System.Console::WriteLine(class [mscorlib]System.String)
	ret
LoadOdd:
	ldstr		"odd"
	br			Print
  } // end of method `EvenOdd::Test'
} // end of class `EvenOdd'
