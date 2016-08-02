.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.namespace 'Foo'
{
.class public auto ansi 'Muncher' extends ['.library']'System'.'Object'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Muncher
} // namespace Foo
.class public auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method public hidebysig instance void 'Bar'(class ['.library']'System'.'Object' 'Foo') cil managed java 
{
	aload_1
	checkcast	'Foo'.'Muncher'
	astore_2
	return
	.locals 3
	.maxstack 1
} // method Bar
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test
