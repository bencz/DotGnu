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
	.locals init	(int8, unsigned int8, int16, unsigned int16, int32, unsigned int32, int64, unsigned int64, char, float32, float64, valuetype ['.library']'System'.'Decimal', int8, unsigned int8, int16, unsigned int16, int32, unsigned int32, int64, unsigned int64, char, float32, float64, valuetype ['.library']'System'.'Decimal')
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
	ldc.r4	float32(0x3FC00000)
	stloc.s	9
	ldc.r8	float64(0x401ACCCCCCCCCCCD)
	stloc.s	10
	ldc.i4.s	35
	ldc.i4.0
	ldc.i4.0
	ldc.i4.0
	ldc.i4.1
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stloc.s	11
	ldc.i4.0
	ldloc.0
	sub.ovf
	stloc.s	16
	ldc.i4.0
	ldloc.1
	sub.ovf
	stloc.s	16
	ldc.i4.0
	ldloc.2
	sub.ovf
	stloc.s	16
	ldc.i4.0
	ldloc.3
	sub.ovf
	stloc.s	16
	ldc.i4.0
	ldloc.s	4
	sub.ovf
	stloc.s	16
	ldc.i4.0
	conv.i8
	ldloc.s	5
	conv.u8
	sub.ovf
	stloc.s	18
	ldc.i4.0
	conv.i8
	ldloc.s	6
	sub.ovf
	stloc.s	18
	ldc.i4.0
	ldloc.s	8
	sub.ovf
	stloc.s	16
	ldloc.s	9
	neg
	conv.r4
	stloc.s	21
	ldloc.s	10
	neg
	conv.r8
	stloc.s	22
	ldloc.s	11
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_UnaryNegation(valuetype [.library]System.Decimal)
	stloc.s	23
	ldloc.0
	neg
	stloc.s	16
	ldloc.1
	neg
	stloc.s	16
	ldloc.2
	neg
	stloc.s	16
	ldloc.3
	neg
	stloc.s	16
	ldloc.s	4
	neg
	stloc.s	16
	ldloc.s	5
	conv.u8
	neg
	stloc.s	18
	ldloc.s	6
	neg
	stloc.s	18
	ldloc.s	8
	neg
	stloc.s	16
	ldloc.s	9
	neg
	conv.r4
	stloc.s	21
	ldloc.s	10
	neg
	conv.r8
	stloc.s	22
	ldloc.s	11
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_UnaryNegation(valuetype [.library]System.Decimal)
	stloc.s	23
	ldloc.0
	not
	stloc.s	16
	ldloc.1
	not
	stloc.s	16
	ldloc.2
	not
	stloc.s	16
	ldloc.3
	not
	stloc.s	16
	ldloc.s	4
	not
	stloc.s	16
	ldloc.s	5
	not
	stloc.s	17
	ldloc.s	6
	not
	stloc.s	18
	ldloc.s	7
	not
	stloc.s	19
	ldloc.s	8
	not
	stloc.s	16
	ret
	.maxstack 5
} // method m1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
