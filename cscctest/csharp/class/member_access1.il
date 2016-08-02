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
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ret
	.maxstack 0
} // method .ctor
} // class Object
} // namespace System
.class public auto ansi 'Test' extends 'System'.'Object'
{
.class nested private auto ansi 'System' extends 'System'.'Object'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void 'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class System
.method public hidebysig instance void 'FooBar'() cil managed 
{
	ret
	.maxstack 0
} // method FooBar
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void 'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
