.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class public auto ansi 'Convertible' extends ['.library']'System'.'Object'
{
.method public static hidebysig specialname class 'Parent' 'op_Implicit'(class 'Convertible' 'a') cil managed 
{
	newobj	instance void 'Parent'::'.ctor'()
	ret
	.maxstack 1
} // method op_Implicit
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Convertible
.class public auto ansi 'Parent' extends ['.library']'System'.'Object'
{
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
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void 'Parent'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Child
.class public auto ansi 'Testing' extends ['.library']'System'.'Object'
{
.method public static hidebysig void 'Main'() cil managed 
{
	.locals init	(class 'Child')
	newobj	instance void 'Convertible'::'.ctor'()
	call	class 'Parent' 'Convertible'::'op_Implicit'(class 'Convertible')
	castclass	'Child'
	stloc.0
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
} // class Testing
