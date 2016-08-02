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
.method private static hidebysig void 'm1'(int32[] 'y') cil managed java 
{
.param[1]
.custom instance void ['.library']'System'.'ParamArrayAttribute'::'.ctor'() = (01 00 00 00)
	return
	.locals 1
	.maxstack 0
} // method m1
.method private static hidebysig void 'm2'(int32[] 'y') cil managed java 
{
	aload_0
	invokestatic	void 'Test'::'m1'(int32[])
	iconst_1
	newarray int32
	dup
	iconst_0
	iconst_3
	iastore
	invokestatic	void 'Test'::'m1'(int32[])
	iconst_5
	newarray int32
	dup
	iconst_0
	iconst_3
	iastore
	dup
	iconst_1
	iconst_4
	iastore
	dup
	iconst_2
	iconst_5
	iastore
	dup
	iconst_3
	bipush	6
	iastore
	dup
	iconst_4
	bipush	7
	iastore
	invokestatic	void 'Test'::'m1'(int32[])
	iconst_0
	newarray int32
	invokestatic	void 'Test'::'m1'(int32[])
	return
	.locals 1
	.maxstack 4
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
