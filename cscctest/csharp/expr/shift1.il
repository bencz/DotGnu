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
	.locals init	(int8, unsigned int8, int16, unsigned int16, int32, unsigned int32, int64, unsigned int64, char, int8, unsigned int8, int16, unsigned int16, int32, unsigned int32, int64, unsigned int64, char, int8, unsigned int8, int16, unsigned int16, int32, unsigned int32, int64, unsigned int64, char)
	ldc.i4.s	34
	conv.i1
	stloc.0
	ldc.i4.s	42
	conv.u1
	stloc.1
	ldc.i4.s	-126
	conv.i2
	stloc.2
	ldc.i4.s	67
	conv.u2
	stloc.3
	ldc.i4	4294966062
	stloc.s	4
	ldc.i4	54321
	stloc.s	5
	ldc.i8	0xfffffffdb34fe916
	stloc.s	6
	ldc.i8	0x000000024cb016ea
	stloc.s	7
	ldc.i4.s	65
	stloc.s	8
	ldc.i4.s	34
	conv.i1
	stloc.s	9
	ldc.i4.s	42
	conv.u1
	stloc.s	10
	ldc.i4.s	-126
	conv.i2
	stloc.s	11
	ldc.i4.s	67
	conv.u2
	stloc.s	12
	ldc.i4	4294966062
	stloc.s	13
	ldc.i4	54321
	stloc.s	14
	ldc.i8	0xfffffffdb34fe916
	stloc.s	15
	ldc.i8	0x000000024cb016ea
	stloc.s	16
	ldc.i4.s	65
	stloc.s	17
	ldloc.0
	ldloc.s	13
	conv.i
	shl
	stloc.s	22
	ldloc.1
	ldloc.s	13
	conv.i
	shl
	stloc.s	22
	ldloc.2
	ldloc.s	13
	conv.i
	shl
	stloc.s	22
	ldloc.3
	ldloc.s	13
	conv.i
	shl
	stloc.s	22
	ldloc.s	4
	ldloc.s	13
	conv.i
	shl
	stloc.s	22
	ldloc.s	5
	ldloc.s	13
	conv.i
	shl
	stloc.s	23
	ldloc.s	6
	ldloc.s	13
	conv.i
	shl
	stloc.s	24
	ldloc.s	7
	ldloc.s	13
	conv.i
	shl
	stloc.s	25
	ldloc.s	8
	ldloc.s	13
	conv.i
	shl
	stloc.s	22
	ldloc.0
	ldloc.s	13
	conv.i
	shr
	stloc.s	22
	ldloc.1
	ldloc.s	13
	conv.i
	shr
	stloc.s	22
	ldloc.2
	ldloc.s	13
	conv.i
	shr
	stloc.s	22
	ldloc.3
	ldloc.s	13
	conv.i
	shr
	stloc.s	22
	ldloc.s	4
	ldloc.s	13
	conv.i
	shr
	stloc.s	22
	ldloc.s	5
	ldloc.s	13
	conv.i
	shr.un
	stloc.s	23
	ldloc.s	6
	ldloc.s	13
	conv.i
	shr
	stloc.s	24
	ldloc.s	7
	ldloc.s	13
	conv.i
	shr.un
	stloc.s	25
	ldloc.s	8
	ldloc.s	13
	conv.i
	shr
	stloc.s	22
	ldloc.s	13
	ldloc.0
	conv.i
	shl
	stloc.s	22
	ldloc.s	13
	ldloc.1
	conv.i
	shr
	stloc.s	22
	ldloc.s	13
	ldloc.2
	conv.i
	shl
	stloc.s	22
	ldloc.s	13
	ldloc.3
	conv.i
	shr
	stloc.s	22
	ldloc.s	13
	ldloc.s	8
	conv.i
	shl
	stloc.s	22
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
