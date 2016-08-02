.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class public auto ansi beforefieldinit 'Foo' extends ['.library']'System'.'Object'
{
.field public static int32 'c'
.field public static class 'Foo'/'Child' 'child'
.method public static hidebysig void 'Override1'() cil managed 
{
	ldc.i4.s	12
	stsfld	int32 'Foo'::'c'
	ret
	.maxstack 1
} // method Override1
.class nested public auto ansi 'Child' extends ['.library']'System'.'Object'
{
.method public hidebysig specialname instance int32 'get_i32'() cil managed 
{
	ldc.i4.s	12
	ret
	.maxstack 1
} // method get_i32
.property int32 'i32'()
{
	.get instance int32 'Foo'/'Child'::'get_i32'()
} // property i32
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Child
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
.method private static hidebysig specialname rtspecialname void '.cctor'() cil managed 
{
	newobj	instance void 'Foo'/'Child'::'.ctor'()
	stsfld	class 'Foo'/'Child' 'Foo'::'child'
	ret
	.maxstack 1
} // method .cctor
} // class Foo
.class private auto ansi 'Bar' extends ['.library']'System'.'Object'
{
.method public static hidebysig void 'NormalCase'() cil managed 
{
	.locals init	(int32, int32)
	ldc.i4.0
	stloc.0
	br	?L1
?L2:
?L3:
	ldloc.0
	ldc.i4.1
	add
	stloc.0
?L1:
	ldloc.0
	ldc.i4.s	10
	blt	?L2
?L4:
	ldsfld	class 'Foo'/'Child' 'Foo'::'child'
	call	instance int32 'Foo'/'Child'::'get_i32'()
	stloc.1
	call	void 'Foo'::'Override1'()
	ret
	.maxstack 2
} // method NormalCase
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Bar
