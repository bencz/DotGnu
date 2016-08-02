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
.method public hidebysig instance void 'show'() cil managed 
{
	ret
	.maxstack 0
} // method show
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	ldc.i4.0
	stfld	int32 'INHERIT'.'A'::'i'
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 2
} // method .ctor
} // class A
} // namespace INHERIT
.namespace 'INHERIT'
{
.class private auto ansi 'B' extends 'INHERIT'.'A'
{
.field private int32 'i'
.method public hidebysig specialname rtspecialname instance void '.ctor'(int32 'a', int32 'b') cil managed 
{
	ldarg.0
	call	instance void 'INHERIT'.'A'::'.ctor'()
	ldarg.0
	ldarg.1
	stfld	int32 'INHERIT'.'A'::'i'
	ldarg.0
	ldarg.2
	stfld	int32 'INHERIT'.'B'::'i'
	ret
	.maxstack 2
} // method .ctor
.method public hidebysig instance void 'show'() cil managed 
{
	ldarg.0
	call	instance void 'INHERIT'.'A'::'show'()
	ret
	.maxstack 1
} // method show
} // class B
} // namespace INHERIT
.namespace 'INHERIT'
{
.class private auto ansi 'UncoverName' extends ['.library']'System'.'Object'
{
.method public static hidebysig void 'Main'() cil managed 
{
	.locals init	(class 'INHERIT'.'B')
	ldc.i4.1
	ldc.i4.2
	newobj	instance void 'INHERIT'.'B'::'.ctor'(int32, int32)
	stloc.0
	ldloc.0
	callvirt	instance void 'INHERIT'.'B'::'show'()
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
} // class UncoverName
} // namespace INHERIT
