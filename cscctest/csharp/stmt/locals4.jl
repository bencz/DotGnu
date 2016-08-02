.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private static hidebysig void 'm2'() cil managed java 
{
	iconst_1
	istore_1
	iconst_3
	istore_3
	iconst_4
	istore	4
	iconst_5
	istore	5
	bipush	6
	istore	6
	bipush	7
	istore	7
	return
	.locals 8
	.maxstack 1
} // method m2
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test
