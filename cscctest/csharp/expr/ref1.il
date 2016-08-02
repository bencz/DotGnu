.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private hidebysig instance void 'm1'(int32 & 'x') cil managed 
{
	.locals init	(int32)
	ldarg.1
	ldind.i4
	ldc.i4.1
	add
	stloc.0
	ldarg.1
	ldc.i4.3
	stind.i4
	ldarg.1
	dup
	ldind.i4
	ldloc.0
	add
	stind.i4
	ret
	.maxstack 3
} // method m1
.method private hidebysig instance void 'm2'([out] int32 & 'x') cil managed 
{
	ldarg.1
	ldc.i4.3
	stind.i4
	ret
	.maxstack 2
} // method m2
.method private hidebysig instance void 'm3'(int32 'x') cil managed 
{
	ldarg.0
	ldarga.s	1
	callvirt	instance void 'Test'::'m1'(int32 &)
	ldarg.0
	ldarga.s	1
	callvirt	instance void 'Test'::'m2'(int32 &)
	ret
	.maxstack 2
} // method m3
.method private hidebysig instance void 'm4'(int32 & 'x') cil managed 
{
	ldarg.0
	ldarg.1
	callvirt	instance void 'Test'::'m1'(int32 &)
	ldarg.0
	ldarg.1
	callvirt	instance void 'Test'::'m2'(int32 &)
	ret
	.maxstack 2
} // method m4
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
