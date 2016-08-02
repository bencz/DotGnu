.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.custom instance void OpenSystem.C.MemoryModelAttribute::.ctor(int32) = (01 00 40 00 00 00 00 00)
.method public static void 'm1'() cil managed 
{
	.locals init	(int8, unsigned int8, int16, unsigned int16, int32, unsigned int32, int64, unsigned int64, char, float32, float64, int8, unsigned int8, int16, unsigned int16, int32, unsigned int32, int64, unsigned int64, char, float32, float64, int8, unsigned int8, int16, unsigned int16, int32, unsigned int32, int64, unsigned int64, char, float32, float64)
	ldc.i4.s	34
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
	conv.u2
	stloc.s	8
	ldc.r4	float32(0x3FC00000)
	stloc.s	9
	ldc.r8	float64(0x401ACCCCCCCCCCCD)
	stloc.s	10
	ldc.i4.s	34
	stloc.s	11
	ldc.i4.s	42
	conv.u1
	stloc.s	12
	ldc.i4.s	-126
	conv.i2
	stloc.s	13
	ldc.i4.s	67
	conv.u2
	stloc.s	14
	ldc.i4	4294966062
	stloc.s	15
	ldc.i4	54321
	stloc.s	16
	ldc.i8	0xfffffffdb34fe916
	stloc.s	17
	ldc.i8	0x000000024cb016ea
	stloc.s	18
	ldc.i4.s	65
	conv.u2
	stloc.s	19
	ldc.r4	float32(0x3FC00000)
	stloc.s	20
	ldc.r8	float64(0x401ACCCCCCCCCCCD)
	stloc.s	21
	ldloc.0
	ldloc.s	11
	add
	conv.i1
	stloc.0
	ldloc.0
	ldloc.s	12
	add
	conv.i1
	stloc.0
	ldloc.0
	ldloc.s	13
	add
	conv.i1
	stloc.0
	ldloc.0
	ldloc.s	14
	add
	conv.i1
	stloc.0
	ldloc.0
	ldloc.s	15
	add
	conv.i1
	stloc.0
	ldloc.0
	ldloc.s	16
	add
	conv.i1
	stloc.0
	ldloc.0
	conv.i8
	ldloc.s	17
	add
	conv.i1
	stloc.0
	ldloc.0
	ldloc.s	19
	add
	conv.i1
	stloc.0
	ldloc.0
	conv.r8
	ldloc.s	20
	add
	conv.i1
	stloc.0
	ldloc.0
	conv.r8
	ldloc.s	21
	add
	conv.i1
	stloc.0
	ldloc.1
	ldloc.s	11
	sub
	conv.u1
	stloc.1
	ldloc.1
	ldloc.s	12
	sub
	conv.u1
	stloc.1
	ldloc.1
	ldloc.s	13
	sub
	conv.u1
	stloc.1
	ldloc.1
	ldloc.s	14
	sub
	conv.u1
	stloc.1
	ldloc.1
	ldloc.s	15
	sub
	conv.u1
	stloc.1
	ldloc.1
	ldloc.s	16
	sub
	conv.u1
	stloc.1
	ldloc.1
	conv.i8
	ldloc.s	17
	sub
	conv.u1
	stloc.1
	ldloc.1
	conv.u8
	ldloc.s	18
	sub
	conv.u1
	stloc.1
	ldloc.1
	ldloc.s	19
	sub
	conv.u1
	stloc.1
	ldloc.1
	conv.r.un
	ldloc.s	20
	sub
	conv.u1
	stloc.1
	ldloc.1
	conv.r.un
	ldloc.s	21
	sub
	conv.u1
	stloc.1
	ldloc.2
	ldloc.s	11
	mul
	conv.i2
	stloc.2
	ldloc.2
	ldloc.s	12
	mul
	conv.i2
	stloc.2
	ldloc.2
	ldloc.s	13
	mul
	conv.i2
	stloc.2
	ldloc.2
	ldloc.s	14
	mul
	conv.i2
	stloc.2
	ldloc.2
	ldloc.s	15
	mul
	conv.i2
	stloc.2
	ldloc.2
	ldloc.s	16
	mul
	conv.i2
	stloc.2
	ldloc.2
	conv.i8
	ldloc.s	17
	mul
	conv.i2
	stloc.2
	ldloc.2
	ldloc.s	19
	mul
	conv.i2
	stloc.2
	ldloc.2
	conv.r8
	ldloc.s	20
	mul
	conv.i2
	stloc.2
	ldloc.2
	conv.r8
	ldloc.s	21
	mul
	conv.i2
	stloc.2
	ldloc.3
	ldloc.s	11
	div
	conv.u2
	stloc.3
	ldloc.3
	ldloc.s	12
	div
	conv.u2
	stloc.3
	ldloc.3
	ldloc.s	13
	div
	conv.u2
	stloc.3
	ldloc.3
	ldloc.s	14
	div
	conv.u2
	stloc.3
	ldloc.3
	ldloc.s	15
	div
	conv.u2
	stloc.3
	ldloc.3
	ldloc.s	16
	div.un
	conv.u2
	stloc.3
	ldloc.3
	conv.i8
	ldloc.s	17
	div
	conv.u2
	stloc.3
	ldloc.3
	conv.u8
	ldloc.s	18
	div.un
	conv.u2
	stloc.3
	ldloc.3
	ldloc.s	19
	div
	conv.u2
	stloc.3
	ldloc.3
	conv.r.un
	ldloc.s	20
	div
	conv.u2
	stloc.3
	ldloc.3
	conv.r.un
	ldloc.s	21
	div
	conv.u2
	stloc.3
	ldloc.s	4
	ldloc.s	11
	rem
	stloc.s	4
	ldloc.s	4
	ldloc.s	12
	rem
	stloc.s	4
	ldloc.s	4
	ldloc.s	13
	rem
	stloc.s	4
	ldloc.s	4
	ldloc.s	14
	rem
	stloc.s	4
	ldloc.s	4
	ldloc.s	15
	rem
	stloc.s	4
	ldloc.s	4
	ldloc.s	16
	rem.un
	stloc.s	4
	ldloc.s	4
	conv.i8
	ldloc.s	17
	rem
	conv.i4
	stloc.s	4
	ldloc.s	4
	ldloc.s	19
	rem
	stloc.s	4
	ldloc.s	4
	conv.r8
	ldloc.s	20
	mul
	conv.i4
	stloc.s	4
	ldloc.s	4
	conv.r8
	ldloc.s	21
	mul
	conv.i4
	stloc.s	4
	ldloc.s	5
	ldloc.s	11
	add
	stloc.s	5
	ldloc.s	5
	ldloc.s	12
	add
	stloc.s	5
	ldloc.s	5
	ldloc.s	13
	add
	stloc.s	5
	ldloc.s	5
	ldloc.s	14
	add
	stloc.s	5
	ldloc.s	5
	ldloc.s	15
	add
	stloc.s	5
	ldloc.s	5
	ldloc.s	16
	add
	stloc.s	5
	ldloc.s	5
	conv.u8
	ldloc.s	17
	add
	conv.u4
	stloc.s	5
	ldloc.s	5
	conv.u8
	ldloc.s	18
	add
	conv.u4
	stloc.s	5
	ldloc.s	5
	ldloc.s	19
	add
	stloc.s	5
	ldloc.s	5
	conv.r.un
	ldloc.s	20
	add
	conv.u4
	stloc.s	5
	ldloc.s	5
	conv.r.un
	ldloc.s	21
	add
	conv.u4
	stloc.s	5
	ldloc.s	6
	ldloc.s	11
	conv.i8
	sub
	stloc.s	6
	ldloc.s	6
	ldloc.s	12
	conv.i8
	sub
	stloc.s	6
	ldloc.s	6
	ldloc.s	13
	conv.i8
	sub
	stloc.s	6
	ldloc.s	6
	ldloc.s	14
	conv.i8
	sub
	stloc.s	6
	ldloc.s	6
	ldloc.s	15
	conv.i8
	sub
	stloc.s	6
	ldloc.s	6
	ldloc.s	16
	conv.u8
	sub
	stloc.s	6
	ldloc.s	6
	ldloc.s	17
	sub
	stloc.s	6
	ldloc.s	6
	ldloc.s	19
	conv.i8
	sub
	stloc.s	6
	ldloc.s	6
	ldc.i8	0x8000000000000001
	dup
	conv.r.un
	conv.u8
	bne.un	?L1
	ldc.i8	0x8000000000000000
	add
	conv.r.un
	ldc.i8	0x8000000000000000
	conv.r.un
	sub
	br	?L2
?L1:
	conv.r8
?L2:
	ldloc.s	20
	sub
	conv.i8
	stloc.s	6
	ldloc.s	6
	ldc.i8	0x8000000000000001
	dup
	conv.r.un
	conv.u8
	bne.un	?L3
	ldc.i8	0x8000000000000000
	add
	conv.r.un
	ldc.i8	0x8000000000000000
	conv.r.un
	sub
	br	?L4
?L3:
	conv.r8
?L4:
	ldloc.s	21
	sub
	conv.i8
	stloc.s	6
	ldloc.s	7
	ldloc.s	12
	conv.u8
	mul
	stloc.s	7
	ldloc.s	7
	ldloc.s	14
	conv.u8
	mul
	stloc.s	7
	ldloc.s	7
	ldloc.s	16
	conv.u8
	mul
	stloc.s	7
	ldloc.s	7
	ldloc.s	18
	mul
	stloc.s	7
	ldloc.s	7
	ldloc.s	19
	conv.u8
	mul
	stloc.s	7
	ldloc.s	7
	conv.r.un
	ldloc.s	20
	mul
	conv.u8
	stloc.s	7
	ldloc.s	7
	conv.r.un
	ldloc.s	21
	mul
	conv.u8
	stloc.s	7
	ldloc.s	9
	ldloc.s	11
	conv.r8
	div
	conv.r4
	stloc.s	9
	ldloc.s	9
	ldloc.s	12
	conv.r.un
	div
	conv.r4
	stloc.s	9
	ldloc.s	9
	ldloc.s	13
	conv.r8
	div
	conv.r4
	stloc.s	9
	ldloc.s	9
	ldloc.s	14
	conv.r.un
	div
	conv.r4
	stloc.s	9
	ldloc.s	9
	ldloc.s	15
	conv.r8
	div
	conv.r4
	stloc.s	9
	ldloc.s	9
	ldloc.s	16
	conv.r.un
	div
	conv.r4
	stloc.s	9
	ldloc.s	9
	ldloc.s	17
	ldc.i8	0x8000000000000001
	dup
	conv.r.un
	conv.u8
	bne.un	?L5
	ldc.i8	0x8000000000000000
	add
	conv.r.un
	ldc.i8	0x8000000000000000
	conv.r.un
	sub
	br	?L6
?L5:
	conv.r8
?L6:
	div
	conv.r4
	stloc.s	9
	ldloc.s	9
	ldloc.s	18
	conv.r.un
	div
	conv.r4
	stloc.s	9
	ldloc.s	9
	ldloc.s	19
	conv.r.un
	div
	conv.r4
	stloc.s	9
	ldloc.s	9
	ldloc.s	20
	div
	conv.r4
	stloc.s	9
	ldloc.s	9
	ldloc.s	21
	div
	conv.r4
	stloc.s	9
	ldloc.s	10
	ldloc.s	11
	conv.r8
	add
	conv.r8
	stloc.s	10
	ldloc.s	10
	ldloc.s	12
	conv.r.un
	add
	conv.r8
	stloc.s	10
	ldloc.s	10
	ldloc.s	13
	conv.r8
	add
	conv.r8
	stloc.s	10
	ldloc.s	10
	ldloc.s	14
	conv.r.un
	add
	conv.r8
	stloc.s	10
	ldloc.s	10
	ldloc.s	15
	conv.r8
	add
	conv.r8
	stloc.s	10
	ldloc.s	10
	ldloc.s	16
	conv.r.un
	add
	conv.r8
	stloc.s	10
	ldloc.s	10
	ldloc.s	17
	ldc.i8	0x8000000000000001
	dup
	conv.r.un
	conv.u8
	bne.un	?L7
	ldc.i8	0x8000000000000000
	add
	conv.r.un
	ldc.i8	0x8000000000000000
	conv.r.un
	sub
	br	?L8
?L7:
	conv.r8
?L8:
	add
	conv.r8
	stloc.s	10
	ldloc.s	10
	ldloc.s	18
	conv.r.un
	add
	conv.r8
	stloc.s	10
	ldloc.s	10
	ldloc.s	19
	conv.r.un
	add
	conv.r8
	stloc.s	10
	ldloc.s	10
	ldloc.s	20
	add
	conv.r8
	stloc.s	10
	ldloc.s	10
	ldloc.s	21
	add
	conv.r8
	stloc.s	10
	ret
	.maxstack 4
} // method m1
