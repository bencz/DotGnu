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
.method public hidebysig instance void 'Foo'(int32 'x') cil managed 
{
	ret
	.maxstack 0
} // method Foo
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Parent
.class public auto ansi 'Child' extends 'Parent'
{
.method public hidebysig instance void 'Foo'(float32 'x') cil managed 
{
	ret
	.maxstack 0
} // method Foo
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void 'Parent'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Child
.class public auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method public static hidebysig void 'Main'() cil managed 
{
	.locals init	(class 'Child')
	newobj	instance void 'Child'::'.ctor'()
	stloc.0
	ldloc.0
	ldc.i4.s	42
	conv.r4
	callvirt	instance void 'Child'::'Foo'(float32)
	ret
	.maxstack 2
} // method Main
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
