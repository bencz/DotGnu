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
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Nested
.field public class 'Parent'/'Nested' 'nested'
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	aconst_null
	putfield	class 'Parent'/'Nested' 'Parent'::'nested'
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 2
} // method .ctor
} // class Parent
.class public auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method public static hidebysig void 'Main'() cil managed java 
{
	aconst_null
	astore_0
	bipush	12
	anewarray class 'Parent'/'Nested'
	astore_1
	aload_0
	getfield	class 'Parent'/'Nested' 'Parent'::'nested'
	astore_2
	return
	.locals 3
	.maxstack 1
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
