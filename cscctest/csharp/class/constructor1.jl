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
.method public hidebysig specialname rtspecialname instance void '.ctor'(int32 'x') cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 2
	.maxstack 1
} // method .ctor
} // class Test
.class private auto ansi 'Test2' extends 'Test'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void 'Test'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
.method public hidebysig specialname rtspecialname instance void '.ctor'(int32 'x') cil managed java 
{
	aload_0
	iload_1
	invokespecial	instance void 'Test'::'.ctor'(int32)
	return
	.locals 2
	.maxstack 2
} // method .ctor
} // class Test2
.class private auto ansi 'Test3' extends 'Test'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	iconst_3
	invokespecial	instance void 'Test'::'.ctor'(int32)
	return
	.locals 1
	.maxstack 2
} // method .ctor
.method public hidebysig specialname rtspecialname instance void '.ctor'(int32 'x') cil managed java 
{
	aload_0
	invokespecial	instance void 'Test3'::'.ctor'()
	return
	.locals 2
	.maxstack 1
} // method .ctor
} // class Test3
.class private auto abstract ansi 'Test4' extends ['.library']'System'.'Object'
{
.method family hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test4
