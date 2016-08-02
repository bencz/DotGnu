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
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test
.class private auto ansi 'Test2' extends ['.library']'System'.'Object'
{
.field private class 'Test' 'x'
.field private int32 'Test'
.field private class 'Test' 'y'
.method private hidebysig instance int32 'm1'() cil managed java 
{
	aload_0
	getfield	int32 'Test2'::'Test'
	ireturn
	.locals 1
	.maxstack 1
} // method m1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test2
