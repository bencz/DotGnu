.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.namespace 'INHERIT'
{
.class private auto ansi 'A' extends ['.library']'System'.'Object'
{
.field public int32 'i'
.method public hidebysig instance void 'show'() cil managed java 
{
	return
	.locals 1
	.maxstack 0
} // method show
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	iconst_0
	putfield	int32 'INHERIT'.'A'::'i'
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 2
} // method .ctor
} // class A
} // namespace INHERIT
.namespace 'INHERIT'
{
.class private auto ansi 'B' extends 'INHERIT'.'A'
{
.field private int32 'i'
.method public hidebysig specialname rtspecialname instance void '.ctor'(int32 'a', int32 'b') cil managed java 
{
	aload_0
	invokespecial	instance void 'INHERIT'.'A'::'.ctor'()
	aload_0
	iload_1
	putfield	int32 'INHERIT'.'A'::'i'
	aload_0
	iload_2
	putfield	int32 'INHERIT'.'B'::'i'
	return
	.locals 3
	.maxstack 2
} // method .ctor
.method public hidebysig instance void 'show'() cil managed java 
{
	aload_0
	invokespecial	instance void 'INHERIT'.'A'::'show'()
	return
	.locals 1
	.maxstack 1
} // method show
} // class B
} // namespace INHERIT
.namespace 'INHERIT'
{
.class private auto ansi 'UncoverName' extends ['.library']'System'.'Object'
{
.method public static hidebysig void 'Main'() cil managed java 
{
	new	'INHERIT'.'B'
	dup
	iconst_1
	iconst_2
	invokespecial	instance void 'INHERIT'.'B'::'.ctor'(int32, int32)
	astore_0
	aload_0
	invokespecial	instance void 'INHERIT'.'B'::'show'()
	return
	.locals 1
	.maxstack 4
} // method Main
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class UncoverName
} // namespace INHERIT
