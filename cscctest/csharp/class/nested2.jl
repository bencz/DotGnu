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
.class nested private auto ansi 'Test2' extends ['.library']'System'.'Object'
{
.method public hidebysig instance void 'Foo'(int32 'x') cil managed java 
{
	bipush	10
	istore_1
	return
	.locals 2
	.maxstack 1
} // method Foo
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test2
.class nested private auto ansi 'Test3' extends 'Test'/'Test2'
{
.method public hidebysig instance void 'Foo'(class 'Test'/'Test2' 'x') cil managed java 
{
	aload_1
	bipush	10
	invokespecial	instance void 'Test'/'Test2'::'Foo'(int32)
	return
	.locals 2
	.maxstack 2
} // method Foo
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void 'Test'/'Test2'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test3
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test
.namespace 'X'
{
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.class nested public auto ansi 'Test2' extends ['.library']'System'.'Object'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test2
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test
} // namespace X
.namespace 'X'
{
.class private auto ansi 'Test3' extends 'X'.'Test'/'Test2'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void 'X'.'Test'/'Test2'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test3
} // namespace X
