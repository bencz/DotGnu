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
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Muncher
} // namespace Foo
.class public auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method public hidebysig instance void 'Bar'(class ['.library']'System'.'Object' 'Foo') cil managed 
{
	.locals init	(class 'Foo'.'Muncher')
	ldarg.1
	castclass	'Foo'.'Muncher'
	stloc.0
	ret
	.maxstack 1
} // method Bar
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
