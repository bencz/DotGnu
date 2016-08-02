.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.namespace 'Test1'
{
.class public auto ansi 'var' extends ['.library']'System'.'Object'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class var
} // namespace Test1
.namespace 'Test'
{
.class private auto ansi 'Test1' extends ['.library']'System'.'Object'
{
.method private static hidebysig int32 'i1'() cil managed 
{
	.locals init	(int32)
	ldc.i4.s	10
	stloc.0
	ldloc.0
	ret
	.maxstack 1
} // method i1
.method private static hidebysig float32 'f1'() cil managed 
{
	.locals init	(float32)
	ldc.r4	float32(0x3F800000)
	stloc.0
	ldloc.0
	ret
	.maxstack 1
} // method f1
.method private static hidebysig float64 'd1'() cil managed 
{
	.locals init	(float64)
	ldc.r4	float32(0x3F800000)
	stloc.0
	ldloc.0
	ret
	.maxstack 1
} // method d1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test1
} // namespace Test
.namespace 'Test'
{
.class private auto ansi 'Test2' extends ['.library']'System'.'Object'
{
.method private static hidebysig class 'Test1'.'var' 'v1'() cil managed 
{
	.locals init	(class 'Test1'.'var')
	newobj	instance void 'Test1'.'var'::'.ctor'()
	stloc.0
	ldloc.0
	ret
	.maxstack 1
} // method v1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test2
} // namespace Test
