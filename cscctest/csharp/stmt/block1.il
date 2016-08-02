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
.method private hidebysig instance void 'm1'() cil managed 
{
	.locals init	(int32, int32, int32)
	ret
	.maxstack 0
} // method m1
.method private hidebysig instance void 'm2'(int32 'x') cil managed 
{
	.locals init	(int32)
	ret
	.maxstack 0
} // method m2
.method private hidebysig instance void 'm3'() cil managed 
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
	ldc.i4.0
	stloc.1
	br	?L5
?L6:
?L7:
	ldloc.1
	ldc.i4.1
	add
	stloc.1
?L5:
	ldloc.1
	ldc.i4.s	10
	blt	?L6
?L8:
	ret
	.maxstack 2
} // method m3
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
