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
	.locals init	(int8, unsigned int8, int16, unsigned int16, int32, unsigned int32, int64, unsigned int64, char, float32, float64, valuetype ['.library']'System'.'Decimal', int8, unsigned int8, int16, unsigned int16, int32, unsigned int32, int64, unsigned int64, char, float32, float64, valuetype ['.library']'System'.'Decimal', int8, unsigned int8, int16, unsigned int16, int32, unsigned int32, int64, unsigned int64, char, float32, float64, valuetype ['.library']'System'.'Decimal')
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
	ldc.i4.s	34
	conv.i1
	stloc.s	12
	ldc.i4.s	42
	conv.u1
	stloc.s	13
	ldc.i4.s	-126
	conv.i2
	stloc.s	14
	ldc.i4.s	67
	conv.u2
	stloc.s	15
	ldc.i4	4294966062
	stloc.s	16
	ldc.i4	54321
	stloc.s	17
	ldc.i8	0xfffffffdb34fe916
	stloc.s	18
	ldc.i8	0x000000024cb016ea
	stloc.s	19
	ldc.i4.s	65
	stloc.s	20
	ldc.r4	float32(0x3FC00000)
	stloc.s	21
	ldc.r8	float64(0x401ACCCCCCCCCCCD)
	stloc.s	22
	ldc.i4.s	35
	ldc.i4.0
	ldc.i4.0
	ldc.i4.0
	ldc.i4.1
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stloc.s	23
	ldloc.0
	ldloc.s	12
	add.ovf
	stloc.s	28
	ldloc.0
	ldloc.s	13
	add.ovf
	stloc.s	28
	ldloc.0
	ldloc.s	14
	add.ovf
	stloc.s	28
	ldloc.0
	ldloc.s	15
	add.ovf
	stloc.s	28
	ldloc.0
	ldloc.s	16
	add.ovf
	stloc.s	28
	ldloc.0
	conv.i8
	ldloc.s	17
	conv.u8
	add.ovf
	stloc.s	30
	ldloc.0
	conv.i8
	ldloc.s	18
	add.ovf
	stloc.s	30
	ldloc.0
	ldloc.s	20
	add.ovf
	stloc.s	28
	ldloc.0
	conv.r4
	ldloc.s	21
	add
	conv.r4
	stloc.s	33
	ldloc.0
	conv.r8
	ldloc.s	22
	add
	conv.r8
	stloc.s	34
	ldloc.0
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int8)
	ldloc.s	23
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Addition(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.1
	ldloc.s	12
	sub.ovf
	stloc.s	28
	ldloc.1
	ldloc.s	13
	sub.ovf
	stloc.s	28
	ldloc.1
	ldloc.s	14
	sub.ovf
	stloc.s	28
	ldloc.1
	ldloc.s	15
	sub.ovf
	stloc.s	28
	ldloc.1
	ldloc.s	16
	sub.ovf
	stloc.s	28
	ldloc.1
	ldloc.s	17
	sub.ovf.un
	stloc.s	29
	ldloc.1
	conv.i8
	ldloc.s	18
	sub.ovf
	stloc.s	30
	ldloc.1
	conv.u8
	ldloc.s	19
	sub.ovf.un
	stloc.s	31
	ldloc.1
	ldloc.s	20
	sub.ovf
	stloc.s	28
	ldloc.1
	conv.r.un
	ldloc.s	21
	sub
	conv.r4
	stloc.s	33
	ldloc.1
	conv.r.un
	ldloc.s	22
	sub
	conv.r8
	stloc.s	34
	ldloc.1
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int8)
	ldloc.s	23
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Subtraction(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.2
	ldloc.s	12
	mul.ovf
	stloc.s	28
	ldloc.2
	ldloc.s	13
	mul.ovf
	stloc.s	28
	ldloc.2
	ldloc.s	14
	mul.ovf
	stloc.s	28
	ldloc.2
	ldloc.s	15
	mul.ovf
	stloc.s	28
	ldloc.2
	ldloc.s	16
	mul.ovf
	stloc.s	28
	ldloc.2
	conv.i8
	ldloc.s	17
	conv.u8
	mul.ovf
	stloc.s	30
	ldloc.2
	conv.i8
	ldloc.s	18
	mul.ovf
	stloc.s	30
	ldloc.2
	ldloc.s	20
	mul.ovf
	stloc.s	28
	ldloc.2
	conv.r4
	ldloc.s	21
	mul
	conv.r4
	stloc.s	33
	ldloc.2
	conv.r8
	ldloc.s	22
	mul
	conv.r8
	stloc.s	34
	ldloc.2
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int16)
	ldloc.s	23
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Multiply(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.3
	ldloc.s	12
	div
	stloc.s	28
	ldloc.3
	ldloc.s	13
	div
	stloc.s	28
	ldloc.3
	ldloc.s	14
	div
	stloc.s	28
	ldloc.3
	ldloc.s	15
	div
	stloc.s	28
	ldloc.3
	ldloc.s	16
	div
	stloc.s	28
	ldloc.3
	ldloc.s	17
	div.un
	stloc.s	29
	ldloc.3
	conv.i8
	ldloc.s	18
	div
	stloc.s	30
	ldloc.3
	conv.u8
	ldloc.s	19
	div.un
	stloc.s	31
	ldloc.3
	ldloc.s	20
	div
	stloc.s	28
	ldloc.3
	conv.r.un
	ldloc.s	21
	div
	conv.r4
	stloc.s	33
	ldloc.3
	conv.r.un
	ldloc.s	22
	div
	conv.r8
	stloc.s	34
	ldloc.3
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int16)
	ldloc.s	23
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Division(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	4
	ldloc.s	12
	rem
	stloc.s	28
	ldloc.s	4
	ldloc.s	13
	rem
	stloc.s	28
	ldloc.s	4
	ldloc.s	14
	rem
	stloc.s	28
	ldloc.s	4
	ldloc.s	15
	rem
	stloc.s	28
	ldloc.s	4
	ldloc.s	16
	rem
	stloc.s	28
	ldloc.s	4
	conv.i8
	ldloc.s	17
	conv.u8
	rem
	stloc.s	30
	ldloc.s	4
	conv.i8
	ldloc.s	18
	rem
	stloc.s	30
	ldloc.s	4
	ldloc.s	20
	rem
	stloc.s	28
	ldloc.s	4
	conv.r4
	ldloc.s	21
	rem
	conv.r4
	stloc.s	33
	ldloc.s	4
	conv.r8
	ldloc.s	22
	rem
	conv.r8
	stloc.s	34
	ldloc.s	4
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	ldloc.s	23
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Modulus(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	5
	conv.u8
	ldloc.s	12
	conv.i8
	add.ovf
	stloc.s	30
	ldloc.s	5
	ldloc.s	13
	add.ovf.un
	stloc.s	29
	ldloc.s	5
	conv.u8
	ldloc.s	14
	conv.i8
	add.ovf
	stloc.s	30
	ldloc.s	5
	ldloc.s	15
	add.ovf.un
	stloc.s	29
	ldloc.s	5
	conv.u8
	ldloc.s	16
	conv.i8
	add.ovf
	stloc.s	30
	ldloc.s	5
	ldloc.s	17
	add.ovf.un
	stloc.s	29
	ldloc.s	5
	conv.u8
	ldloc.s	18
	add.ovf
	stloc.s	30
	ldloc.s	5
	conv.u8
	ldloc.s	19
	add.ovf.un
	stloc.s	31
	ldloc.s	5
	ldloc.s	20
	add.ovf.un
	stloc.s	29
	ldloc.s	5
	conv.r.un
	conv.r4
	ldloc.s	21
	add
	conv.r4
	stloc.s	33
	ldloc.s	5
	conv.r.un
	ldloc.s	22
	add
	conv.r8
	stloc.s	34
	ldloc.s	5
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int32)
	ldloc.s	23
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Addition(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	6
	ldloc.s	12
	conv.i8
	sub.ovf
	stloc.s	30
	ldloc.s	6
	ldloc.s	13
	conv.i8
	sub.ovf
	stloc.s	30
	ldloc.s	6
	ldloc.s	14
	conv.i8
	sub.ovf
	stloc.s	30
	ldloc.s	6
	ldloc.s	15
	conv.i8
	sub.ovf
	stloc.s	30
	ldloc.s	6
	ldloc.s	16
	conv.i8
	sub.ovf
	stloc.s	30
	ldloc.s	6
	ldloc.s	17
	conv.u8
	sub.ovf
	stloc.s	30
	ldloc.s	6
	ldloc.s	18
	sub.ovf
	stloc.s	30
	ldloc.s	6
	ldloc.s	20
	conv.i8
	sub.ovf
	stloc.s	30
	ldloc.s	6
	conv.r4
	ldloc.s	21
	sub
	conv.r4
	stloc.s	33
	ldloc.s	6
	conv.r8
	ldloc.s	22
	sub
	conv.r8
	stloc.s	34
	ldloc.s	6
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int64)
	ldloc.s	23
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Subtraction(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	7
	ldloc.s	13
	conv.u8
	mul.ovf.un
	stloc.s	31
	ldloc.s	7
	ldloc.s	15
	conv.u8
	mul.ovf.un
	stloc.s	31
	ldloc.s	7
	ldloc.s	17
	conv.u8
	mul.ovf.un
	stloc.s	31
	ldloc.s	7
	ldloc.s	19
	mul.ovf.un
	stloc.s	31
	ldloc.s	7
	ldloc.s	20
	conv.u8
	mul.ovf.un
	stloc.s	31
	ldloc.s	7
	conv.r.un
	conv.r4
	ldloc.s	21
	mul
	conv.r4
	stloc.s	33
	ldloc.s	7
	conv.r.un
	conv.r8
	ldloc.s	22
	mul
	conv.r8
	stloc.s	34
	ldloc.s	7
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int64)
	ldloc.s	23
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Multiply(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	9
	ldloc.s	12
	conv.r4
	div
	conv.r4
	stloc.s	33
	ldloc.s	9
	ldloc.s	13
	conv.r.un
	div
	conv.r4
	stloc.s	33
	ldloc.s	9
	ldloc.s	14
	conv.r4
	div
	conv.r4
	stloc.s	33
	ldloc.s	9
	ldloc.s	15
	conv.r.un
	div
	conv.r4
	stloc.s	33
	ldloc.s	9
	ldloc.s	16
	conv.r4
	div
	conv.r4
	stloc.s	33
	ldloc.s	9
	ldloc.s	17
	conv.r.un
	conv.r4
	div
	conv.r4
	stloc.s	33
	ldloc.s	9
	ldloc.s	18
	conv.r4
	div
	conv.r4
	stloc.s	33
	ldloc.s	9
	ldloc.s	19
	conv.r.un
	conv.r4
	div
	conv.r4
	stloc.s	33
	ldloc.s	9
	ldloc.s	20
	conv.r.un
	div
	conv.r4
	stloc.s	33
	ldloc.s	9
	ldloc.s	21
	div
	conv.r4
	stloc.s	33
	ldloc.s	9
	ldloc.s	22
	div
	conv.r8
	stloc.s	34
	ldloc.s	10
	ldloc.s	12
	conv.r8
	rem
	conv.r8
	stloc.s	34
	ldloc.s	10
	ldloc.s	13
	conv.r.un
	rem
	conv.r8
	stloc.s	34
	ldloc.s	10
	ldloc.s	14
	conv.r8
	rem
	conv.r8
	stloc.s	34
	ldloc.s	10
	ldloc.s	15
	conv.r.un
	rem
	conv.r8
	stloc.s	34
	ldloc.s	10
	ldloc.s	16
	conv.r8
	rem
	conv.r8
	stloc.s	34
	ldloc.s	10
	ldloc.s	17
	conv.r.un
	rem
	conv.r8
	stloc.s	34
	ldloc.s	10
	ldloc.s	18
	conv.r8
	rem
	conv.r8
	stloc.s	34
	ldloc.s	10
	ldloc.s	19
	conv.r.un
	conv.r8
	rem
	conv.r8
	stloc.s	34
	ldloc.s	10
	ldloc.s	20
	conv.r.un
	rem
	conv.r8
	stloc.s	34
	ldloc.s	10
	ldloc.s	21
	rem
	conv.r8
	stloc.s	34
	ldloc.s	10
	ldloc.s	22
	rem
	conv.r8
	stloc.s	34
	ldloc.s	11
	ldloc.s	12
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int8)
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Addition(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	11
	ldloc.s	13
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int8)
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Addition(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	11
	ldloc.s	14
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int16)
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Addition(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	11
	ldloc.s	15
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int16)
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Addition(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	11
	ldloc.s	16
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Addition(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	11
	ldloc.s	17
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int32)
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Addition(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	11
	ldloc.s	18
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int64)
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Addition(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	11
	ldloc.s	19
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int64)
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Addition(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	11
	ldloc.s	20
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(char)
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Addition(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	11
	ldloc.s	23
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Addition(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.0
	ldloc.s	12
	add
	stloc.s	28
	ldloc.0
	ldloc.s	13
	add
	stloc.s	28
	ldloc.0
	ldloc.s	14
	add
	stloc.s	28
	ldloc.0
	ldloc.s	15
	add
	stloc.s	28
	ldloc.0
	ldloc.s	16
	add
	stloc.s	28
	ldloc.0
	conv.i8
	ldloc.s	17
	conv.u8
	add
	stloc.s	30
	ldloc.0
	conv.i8
	ldloc.s	18
	add
	stloc.s	30
	ldloc.0
	ldloc.s	20
	add
	stloc.s	28
	ldloc.0
	conv.r4
	ldloc.s	21
	add
	conv.r4
	stloc.s	33
	ldloc.0
	conv.r8
	ldloc.s	22
	add
	conv.r8
	stloc.s	34
	ldloc.0
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int8)
	ldloc.s	23
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Addition(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.1
	ldloc.s	12
	sub
	stloc.s	28
	ldloc.1
	ldloc.s	13
	sub
	stloc.s	28
	ldloc.1
	ldloc.s	14
	sub
	stloc.s	28
	ldloc.1
	ldloc.s	15
	sub
	stloc.s	28
	ldloc.1
	ldloc.s	16
	sub
	stloc.s	28
	ldloc.1
	ldloc.s	17
	sub
	stloc.s	29
	ldloc.1
	conv.i8
	ldloc.s	18
	sub
	stloc.s	30
	ldloc.1
	conv.u8
	ldloc.s	19
	sub
	stloc.s	31
	ldloc.1
	ldloc.s	20
	sub
	stloc.s	28
	ldloc.1
	conv.r.un
	ldloc.s	21
	sub
	conv.r4
	stloc.s	33
	ldloc.1
	conv.r.un
	ldloc.s	22
	sub
	conv.r8
	stloc.s	34
	ldloc.1
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int8)
	ldloc.s	23
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Subtraction(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.2
	ldloc.s	12
	mul
	stloc.s	28
	ldloc.2
	ldloc.s	13
	mul
	stloc.s	28
	ldloc.2
	ldloc.s	14
	mul
	stloc.s	28
	ldloc.2
	ldloc.s	15
	mul
	stloc.s	28
	ldloc.2
	ldloc.s	16
	mul
	stloc.s	28
	ldloc.2
	conv.i8
	ldloc.s	17
	conv.u8
	mul
	stloc.s	30
	ldloc.2
	conv.i8
	ldloc.s	18
	mul
	stloc.s	30
	ldloc.2
	ldloc.s	20
	mul
	stloc.s	28
	ldloc.2
	conv.r4
	ldloc.s	21
	mul
	conv.r4
	stloc.s	33
	ldloc.2
	conv.r8
	ldloc.s	22
	mul
	conv.r8
	stloc.s	34
	ldloc.2
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int16)
	ldloc.s	23
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Multiply(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.3
	ldloc.s	12
	div
	stloc.s	28
	ldloc.3
	ldloc.s	13
	div
	stloc.s	28
	ldloc.3
	ldloc.s	14
	div
	stloc.s	28
	ldloc.3
	ldloc.s	15
	div
	stloc.s	28
	ldloc.3
	ldloc.s	16
	div
	stloc.s	28
	ldloc.3
	ldloc.s	17
	div.un
	stloc.s	29
	ldloc.3
	conv.i8
	ldloc.s	18
	div
	stloc.s	30
	ldloc.3
	conv.u8
	ldloc.s	19
	div.un
	stloc.s	31
	ldloc.3
	ldloc.s	20
	div
	stloc.s	28
	ldloc.3
	conv.r.un
	ldloc.s	21
	div
	conv.r4
	stloc.s	33
	ldloc.3
	conv.r.un
	ldloc.s	22
	div
	conv.r8
	stloc.s	34
	ldloc.3
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int16)
	ldloc.s	23
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Division(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	4
	ldloc.s	12
	rem
	stloc.s	28
	ldloc.s	4
	ldloc.s	13
	rem
	stloc.s	28
	ldloc.s	4
	ldloc.s	14
	rem
	stloc.s	28
	ldloc.s	4
	ldloc.s	15
	rem
	stloc.s	28
	ldloc.s	4
	ldloc.s	16
	rem
	stloc.s	28
	ldloc.s	4
	conv.i8
	ldloc.s	17
	conv.u8
	rem
	stloc.s	30
	ldloc.s	4
	conv.i8
	ldloc.s	18
	rem
	stloc.s	30
	ldloc.s	4
	ldloc.s	20
	rem
	stloc.s	28
	ldloc.s	4
	conv.r4
	ldloc.s	21
	rem
	conv.r4
	stloc.s	33
	ldloc.s	4
	conv.r8
	ldloc.s	22
	rem
	conv.r8
	stloc.s	34
	ldloc.s	4
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	ldloc.s	23
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Modulus(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	5
	conv.u8
	ldloc.s	12
	conv.i8
	add
	stloc.s	30
	ldloc.s	5
	ldloc.s	13
	add
	stloc.s	29
	ldloc.s	5
	conv.u8
	ldloc.s	14
	conv.i8
	add
	stloc.s	30
	ldloc.s	5
	ldloc.s	15
	add
	stloc.s	29
	ldloc.s	5
	conv.u8
	ldloc.s	16
	conv.i8
	add
	stloc.s	30
	ldloc.s	5
	ldloc.s	17
	add
	stloc.s	29
	ldloc.s	5
	conv.u8
	ldloc.s	18
	add
	stloc.s	30
	ldloc.s	5
	conv.u8
	ldloc.s	19
	add
	stloc.s	31
	ldloc.s	5
	ldloc.s	20
	add
	stloc.s	29
	ldloc.s	5
	conv.r.un
	conv.r4
	ldloc.s	21
	add
	conv.r4
	stloc.s	33
	ldloc.s	5
	conv.r.un
	ldloc.s	22
	add
	conv.r8
	stloc.s	34
	ldloc.s	5
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int32)
	ldloc.s	23
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Addition(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	6
	ldloc.s	12
	conv.i8
	sub
	stloc.s	30
	ldloc.s	6
	ldloc.s	13
	conv.i8
	sub
	stloc.s	30
	ldloc.s	6
	ldloc.s	14
	conv.i8
	sub
	stloc.s	30
	ldloc.s	6
	ldloc.s	15
	conv.i8
	sub
	stloc.s	30
	ldloc.s	6
	ldloc.s	16
	conv.i8
	sub
	stloc.s	30
	ldloc.s	6
	ldloc.s	17
	conv.u8
	sub
	stloc.s	30
	ldloc.s	6
	ldloc.s	18
	sub
	stloc.s	30
	ldloc.s	6
	ldloc.s	20
	conv.i8
	sub
	stloc.s	30
	ldloc.s	6
	conv.r4
	ldloc.s	21
	sub
	conv.r4
	stloc.s	33
	ldloc.s	6
	conv.r8
	ldloc.s	22
	sub
	conv.r8
	stloc.s	34
	ldloc.s	6
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int64)
	ldloc.s	23
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Subtraction(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	7
	ldloc.s	13
	conv.u8
	mul
	stloc.s	31
	ldloc.s	7
	ldloc.s	15
	conv.u8
	mul
	stloc.s	31
	ldloc.s	7
	ldloc.s	17
	conv.u8
	mul
	stloc.s	31
	ldloc.s	7
	ldloc.s	19
	mul
	stloc.s	31
	ldloc.s	7
	ldloc.s	20
	conv.u8
	mul
	stloc.s	31
	ldloc.s	7
	conv.r.un
	conv.r4
	ldloc.s	21
	mul
	conv.r4
	stloc.s	33
	ldloc.s	7
	conv.r.un
	conv.r8
	ldloc.s	22
	mul
	conv.r8
	stloc.s	34
	ldloc.s	7
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int64)
	ldloc.s	23
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Multiply(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	9
	ldloc.s	12
	conv.r4
	div
	conv.r4
	stloc.s	33
	ldloc.s	9
	ldloc.s	13
	conv.r.un
	div
	conv.r4
	stloc.s	33
	ldloc.s	9
	ldloc.s	14
	conv.r4
	div
	conv.r4
	stloc.s	33
	ldloc.s	9
	ldloc.s	15
	conv.r.un
	div
	conv.r4
	stloc.s	33
	ldloc.s	9
	ldloc.s	16
	conv.r4
	div
	conv.r4
	stloc.s	33
	ldloc.s	9
	ldloc.s	17
	conv.r.un
	conv.r4
	div
	conv.r4
	stloc.s	33
	ldloc.s	9
	ldloc.s	18
	conv.r4
	div
	conv.r4
	stloc.s	33
	ldloc.s	9
	ldloc.s	19
	conv.r.un
	conv.r4
	div
	conv.r4
	stloc.s	33
	ldloc.s	9
	ldloc.s	20
	conv.r.un
	div
	conv.r4
	stloc.s	33
	ldloc.s	9
	ldloc.s	21
	div
	conv.r4
	stloc.s	33
	ldloc.s	9
	ldloc.s	22
	div
	conv.r8
	stloc.s	34
	ldloc.s	10
	ldloc.s	12
	conv.r8
	rem
	conv.r8
	stloc.s	34
	ldloc.s	10
	ldloc.s	13
	conv.r.un
	rem
	conv.r8
	stloc.s	34
	ldloc.s	10
	ldloc.s	14
	conv.r8
	rem
	conv.r8
	stloc.s	34
	ldloc.s	10
	ldloc.s	15
	conv.r.un
	rem
	conv.r8
	stloc.s	34
	ldloc.s	10
	ldloc.s	16
	conv.r8
	rem
	conv.r8
	stloc.s	34
	ldloc.s	10
	ldloc.s	17
	conv.r.un
	rem
	conv.r8
	stloc.s	34
	ldloc.s	10
	ldloc.s	18
	conv.r8
	rem
	conv.r8
	stloc.s	34
	ldloc.s	10
	ldloc.s	19
	conv.r.un
	conv.r8
	rem
	conv.r8
	stloc.s	34
	ldloc.s	10
	ldloc.s	20
	conv.r.un
	rem
	conv.r8
	stloc.s	34
	ldloc.s	10
	ldloc.s	21
	rem
	conv.r8
	stloc.s	34
	ldloc.s	10
	ldloc.s	22
	rem
	conv.r8
	stloc.s	34
	ldloc.s	11
	ldloc.s	12
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int8)
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Addition(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	11
	ldloc.s	13
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int8)
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Addition(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	11
	ldloc.s	14
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int16)
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Addition(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	11
	ldloc.s	15
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int16)
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Addition(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	11
	ldloc.s	16
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Addition(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	11
	ldloc.s	17
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int32)
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Addition(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	11
	ldloc.s	18
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int64)
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Addition(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	11
	ldloc.s	19
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int64)
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Addition(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	11
	ldloc.s	20
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(char)
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Addition(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
	ldloc.s	11
	ldloc.s	23
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Addition(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.s	35
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
