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
.class nested public auto ansi 'Nested' extends ['.library']'System'.'Object'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Nested
.field public class 'Parent'/'Nested' 'nested'
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	ldnull
	stfld	class 'Parent'/'Nested' 'Parent'::'nested'
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 2
} // method .ctor
} // class Parent
.class public auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method public static hidebysig void 'Main'() cil managed 
{
	.locals init	(class 'Parent', class 'Parent'/'Nested'[], class 'Parent'/'Nested')
	ldnull
	stloc.0
	ldc.i4.s	12
	newarr	'Parent'/'Nested'
	stloc.1
	ldloc.0
	ldfld	class 'Parent'/'Nested' 'Parent'::'nested'
	stloc.2
	ret
	.maxstack 1
} // method Main
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
