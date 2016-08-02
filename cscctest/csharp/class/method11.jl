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
.method public virtual hidebysig newslot instance void 'print'() cil managed java 
{
	return
	.locals 1
	.maxstack 0
} // method print
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class foo
.class private auto ansi 'test' extends 'foo'
{
.method public virtual hidebysig instance void 'print'() cil managed java 
{
	return
	.locals 1
	.maxstack 0
} // method print
.method public hidebysig instance void 'callbase'() cil managed java 
{
	aload_0
	invokespecial	instance void 'foo'::'print'()
	aload_0
	invokevirtual	instance void 'foo'::'print'()
	return
	.locals 1
	.maxstack 1
} // method callbase
.method public static hidebysig void 'Main'() cil managed java 
{
	return
	.locals 0
	.maxstack 0
} // method Main
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void 'foo'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class test
