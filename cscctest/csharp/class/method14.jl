.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class public auto ansi 'Parent' extends ['.library']'System'.'Object'
{
.method public hidebysig instance void 'Foo'(int32 'x') cil managed java 
{
	return
	.locals 2
	.maxstack 0
} // method Foo
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Parent
.class public auto ansi 'Child' extends 'Parent'
{
.method public hidebysig instance void 'Foo'(float32 'x') cil managed java 
{
	return
	.locals 2
	.maxstack 0
} // method Foo
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void 'Parent'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Child
.class public auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method public static hidebysig void 'Main'() cil managed java 
{
	new	'Child'
	dup
	invokespecial	instance void 'Child'::'.ctor'()
	astore_0
	aload_0
	bipush	42
	i2f
	invokespecial	instance void 'Child'::'Foo'(float32)
	return
	.locals 1
	.maxstack 2
} // method Main
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test
