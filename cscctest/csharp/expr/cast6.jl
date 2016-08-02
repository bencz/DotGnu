.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private auto interface abstract ansi 'I'
{
} // class I
.class private auto interface abstract ansi 'J' implements 'I'
{
} // class J
.class private auto interface abstract ansi 'K' implements 'I', 'J'
{
} // class K
.class private auto interface abstract ansi 'L'
{
} // class L
.class private sequential sealed serializable ansi 'X' extends ['.library']'System'.'ValueType'
{
.field private int32 'x'
} // class X
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method public hidebysig instance void 'm1'() cil managed java 
{
	new	"X"
	dup
	invokespecial	"X" "<init>" "()V"
	astore	9
	aconst_null
	astore_1
	aconst_null
	astore_2
	aconst_null
	astore	5
	aconst_null
	astore	10
	aconst_null
	astore	11
	aconst_null
	astore	12
	aconst_null
	astore	13
	aload_2
	astore_1
	aload_3
	astore_1
	aload	5
	astore_1
	aload	10
	astore_1
	aload	11
	astore_1
	aload	12
	astore_1
	aload	13
	astore_1
	aload_1
	checkcast	'Test'
	astore_2
	aload_1
	checkcast	'Test2'
	astore_3
	aload_1
	checkcast	'I'
	astore	5
	aload_1
	checkcast	class ['.library']'System'.'Object'[]
	astore	10
	aload_1
	checkcast	class 'Test'[]
	astore	11
	aload_1
	checkcast	class 'I'[]
	astore	12
	aload_1
	checkcast	valuetype 'X'[]
	astore	13
	aload_2
	astore_1
	aload_3
	astore_2
	aload_1
	checkcast	'Test'
	astore_2
	aload_1
	checkcast	'Test2'
	astore_3
	aload_2
	checkcast	'Test2'
	astore_3
	aload_3
	astore	5
	aload_2
	checkcast	'I'
	astore	5
	aload	6
	astore	5
	aload	7
	astore	5
	aload	7
	astore	6
	aload	6
	checkcast	'Test2'
	astore_3
	aload	6
	checkcast	'Test3'
	astore	4
	aload	7
	checkcast	'L'
	astore	8
	aload	11
	astore	10
	aload	12
	astore	10
	aload	10
	checkcast	class 'Test'[]
	astore	11
	aload	11
	checkcast	class 'I'[]
	astore	12
	return
	.locals 14
	.maxstack 2
} // method m1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test
.class private auto ansi 'Test2' extends 'Test' implements 'I'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void 'Test'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test2
.class private auto sealed ansi 'Test3' extends ['.library']'System'.'Object' implements 'J'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test3
