.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class public auto ansi 'A' extends ['.library']'System'.'Object'
{
.method family hidebysig instance void 'TestA'() cil managed java 
{
	return
	.locals 1
	.maxstack 0
} // method TestA
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class A
.class public auto ansi 'B' extends 'A'
{
.class nested public auto ansi 'C' extends ['.library']'System'.'Object'
{
.method public static hidebysig void 'F'(class 'B' 'b') cil managed java 
{
	aload_0
	invokespecial	instance void 'A'::'TestA'()
	return
	.locals 1
	.maxstack 1
} // method F
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class C
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void 'A'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class B
