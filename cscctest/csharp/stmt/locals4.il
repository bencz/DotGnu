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
.method private static hidebysig void 'm2'() cil managed 
{
	.locals init	(int32, int32, int32, int32, int32, int32, int32, int32)
	ldc.i4.1
	stloc.1
	ldc.i4.3
	stloc.3
	ldc.i4.4
	stloc.s	4
	ldc.i4.5
	stloc.s	5
	ldc.i4.6
	stloc.s	6
	ldc.i4.7
	stloc.s	7
	ret
	.maxstack 1
} // method m2
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
