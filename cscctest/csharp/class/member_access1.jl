.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.namespace 'System'
{
.class public auto ansi 'Object'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	return
	.locals 1
	.maxstack 0
} // method .ctor
} // class Object
} // namespace System
.class public auto ansi 'Test' extends 'System'.'Object'
{
.class nested private auto ansi 'System' extends 'System'.'Object'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void 'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class System
.method public hidebysig instance void 'FooBar'() cil managed java 
{
	return
	.locals 1
	.maxstack 0
} // method FooBar
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void 'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test
