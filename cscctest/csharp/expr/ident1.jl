.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private auto ansi 'foo' extends ['.library']'System'.'Object'
{
.field private static int32 '_x'
.method public hidebysig specialname rtspecialname instance void '.ctor'(int32 'x') cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	iload_1
	putstatic	int32 'foo'::'_x'
	return
	.locals 2
	.maxstack 1
} // method .ctor
.method public static hidebysig int32 'bar'() cil managed java 
{
	getstatic	int32 'foo'::'_x'
	ireturn
	.locals 0
	.maxstack 1
} // method bar
} // class foo
.class private auto ansi 'bar' extends ['.library']'System'.'Object'
{
.field private static int32 'foo'
.method private static hidebysig void 'Main'() cil managed java 
{
	iconst_1
	putstatic	int32 'bar'::'foo'
	new	'foo'
	dup
	getstatic	int32 'bar'::'foo'
	invokespecial	instance void 'foo'::'.ctor'(int32)
	astore_0
	invokestatic	int32 'foo'::'bar'()
	istore_1
	return
	.locals 2
	.maxstack 3
} // method Main
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class bar
