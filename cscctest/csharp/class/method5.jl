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
.method private static hidebysig void 'm1'(int32 'x') cil managed java 
{
	return
	.locals 1
	.maxstack 0
} // method m1
.method private static hidebysig void 'm1'(float64 'x') cil managed java 
{
	return
	.locals 2
	.maxstack 0
} // method m1
.method private static hidebysig void 'm1'(valuetype ['.library']'System'.'Decimal' 'x') cil managed java 
{
	return
	.locals 1
	.maxstack 0
} // method m1
.method private static hidebysig void 'm1'(unsigned int32 'x') cil managed java 
{
	return
	.locals 1
	.maxstack 0
} // method m1
.method private static hidebysig void 'm2'() cil managed java 
{
	iconst_3
	invokestatic	void 'Test'::'m1'(int32)
	iconst_3
	invokestatic	void 'Test'::'m1'(unsigned int32)
	return
	.locals 0
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
