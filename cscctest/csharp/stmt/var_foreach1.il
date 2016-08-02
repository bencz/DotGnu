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
.method private hidebysig instance int32 'm1'(int32[] 'array') cil managed 
{
	.locals init	(int32, int32)
	ldc.i4.0
	stloc.0
	.locals init	(int32[])
	.locals init	(int32)
	ldarg.1
	stloc.2
	ldc.i4.0
	stloc.3
	br	?L1
?L2:
	ldloc.2
	ldloc.3
	ldelem.i4
	stloc.1
	ldloc.0
	ldloc.1
	add
	stloc.0
?L3:
	ldloc.3
	ldc.i4.1
	add
	stloc.3
?L1:
	ldloc.3
	ldloc.2
	ldlen
	conv.i4
	blt	?L2
?L4:
	ldloc.0
	ret
	.maxstack 2
} // method m1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
