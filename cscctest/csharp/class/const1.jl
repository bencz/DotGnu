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
.field private static literal int32 'x' = int32(0x00000003)
.method private hidebysig instance int32 'm1'() cil managed java 
{
	iconst_3
	ireturn
	.locals 1
	.maxstack 1
} // method m1
.method private static hidebysig int32 'm2'() cil managed java 
{
	iconst_3
	ireturn
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
