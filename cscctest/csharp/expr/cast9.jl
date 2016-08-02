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
.method public static hidebysig specialname class 'Parent' 'op_Implicit'(class 'Convertible' 'a') cil managed java 
{
	new	'Parent'
	dup
	invokespecial	instance void 'Parent'::'.ctor'()
	areturn
	.locals 1
	.maxstack 2
} // method op_Implicit
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Convertible
.class public auto ansi 'Parent' extends ['.library']'System'.'Object'
{
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
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void 'Parent'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Child
.class public auto ansi 'Testing' extends ['.library']'System'.'Object'
{
.method public static hidebysig void 'Main'() cil managed java 
{
	new	'Convertible'
	dup
	invokespecial	instance void 'Convertible'::'.ctor'()
	invokestatic	class 'Parent' 'Convertible'::'op_Implicit'(class 'Convertible')
	checkcast	'Child'
	astore_0
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
} // class Testing
