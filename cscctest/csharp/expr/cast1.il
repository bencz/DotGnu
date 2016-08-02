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
	.locals init	(int8, unsigned int8, int16, unsigned int16, int32, unsigned int32, int64, unsigned int64, char, float32, float64, valuetype ['.library']'System'.'Decimal')
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
	ldloc.0
	stloc.0
	ldloc.0
	conv.ovf.u1
	stloc.1
	ldloc.0
	stloc.2
	ldloc.0
	conv.ovf.u2
	stloc.3
	ldloc.0
	stloc.s	4
	ldloc.0
	conv.ovf.u4
	stloc.s	5
	ldloc.0
	conv.i8
	stloc.s	6
	ldloc.0
	conv.ovf.u8
	stloc.s	7
	ldloc.0
	conv.ovf.u2
	stloc.s	8
	ldloc.0
	conv.r4
	stloc.s	9
	ldloc.0
	conv.r8
	stloc.s	10
	ldloc.0
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int8)
	stloc.s	11
	ldloc.1
	conv.ovf.i1.un
	stloc.0
	ldloc.1
	stloc.1
	ldloc.1
	stloc.2
	ldloc.1
	stloc.3
	ldloc.1
	stloc.s	4
	ldloc.1
	stloc.s	5
	ldloc.1
	conv.i8
	stloc.s	6
	ldloc.1
	conv.u8
	stloc.s	7
	ldloc.1
	stloc.s	8
	ldloc.1
	conv.r.un
	stloc.s	9
	ldloc.1
	conv.r.un
	stloc.s	10
	ldloc.1
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int8)
	stloc.s	11
	ldloc.2
	conv.ovf.i1
	stloc.0
	ldloc.2
	conv.ovf.u1
	stloc.1
	ldloc.2
	stloc.2
	ldloc.2
	conv.ovf.u2
	stloc.3
	ldloc.2
	stloc.s	4
	ldloc.2
	conv.ovf.u4
	stloc.s	5
	ldloc.2
	conv.i8
	stloc.s	6
	ldloc.2
	conv.ovf.u8
	stloc.s	7
	ldloc.2
	conv.ovf.u2
	stloc.s	8
	ldloc.2
	conv.r4
	stloc.s	9
	ldloc.2
	conv.r8
	stloc.s	10
	ldloc.2
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int16)
	stloc.s	11
	ldloc.3
	conv.ovf.i1.un
	stloc.0
	ldloc.3
	conv.ovf.u1.un
	stloc.1
	ldloc.3
	conv.ovf.i2.un
	stloc.2
	ldloc.3
	stloc.3
	ldloc.3
	stloc.s	4
	ldloc.3
	stloc.s	5
	ldloc.3
	conv.i8
	stloc.s	6
	ldloc.3
	conv.u8
	stloc.s	7
	ldloc.3
	stloc.s	8
	ldloc.3
	conv.r.un
	stloc.s	9
	ldloc.3
	conv.r.un
	stloc.s	10
	ldloc.3
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int16)
	stloc.s	11
	ldloc.s	4
	conv.ovf.i1
	stloc.0
	ldloc.s	4
	conv.ovf.u1
	stloc.1
	ldloc.s	4
	conv.ovf.i2
	stloc.2
	ldloc.s	4
	conv.ovf.u2
	stloc.3
	ldloc.s	4
	stloc.s	4
	ldloc.s	4
	conv.ovf.u4
	stloc.s	5
	ldloc.s	4
	conv.i8
	stloc.s	6
	ldloc.s	4
	conv.ovf.u8
	stloc.s	7
	ldloc.s	4
	conv.ovf.u2
	stloc.s	8
	ldloc.s	4
	conv.r4
	stloc.s	9
	ldloc.s	4
	conv.r8
	stloc.s	10
	ldloc.s	4
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	stloc.s	11
	ldloc.s	5
	conv.ovf.i1.un
	stloc.0
	ldloc.s	5
	conv.ovf.u1.un
	stloc.1
	ldloc.s	5
	conv.ovf.i2.un
	stloc.2
	ldloc.s	5
	conv.ovf.u2.un
	stloc.3
	ldloc.s	5
	conv.ovf.i4.un
	stloc.s	4
	ldloc.s	5
	stloc.s	5
	ldloc.s	5
	conv.u8
	stloc.s	6
	ldloc.s	5
	conv.u8
	stloc.s	7
	ldloc.s	5
	conv.ovf.u2.un
	stloc.s	8
	ldloc.s	5
	conv.r.un
	conv.r4
	stloc.s	9
	ldloc.s	5
	conv.r.un
	stloc.s	10
	ldloc.s	5
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int32)
	stloc.s	11
	ldloc.s	6
	conv.ovf.i1
	stloc.0
	ldloc.s	6
	conv.ovf.u1
	stloc.1
	ldloc.s	6
	conv.ovf.i2
	stloc.2
	ldloc.s	6
	conv.ovf.u2
	stloc.3
	ldloc.s	6
	conv.ovf.i4
	stloc.s	4
	ldloc.s	6
	conv.ovf.u4
	stloc.s	5
	ldloc.s	6
	stloc.s	6
	ldloc.s	6
	conv.ovf.u8
	stloc.s	7
	ldloc.s	6
	conv.ovf.u2
	stloc.s	8
	ldloc.s	6
	conv.r4
	stloc.s	9
	ldloc.s	6
	conv.r8
	stloc.s	10
	ldloc.s	6
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int64)
	stloc.s	11
	ldloc.s	7
	conv.ovf.i1.un
	stloc.0
	ldloc.s	7
	conv.ovf.u1.un
	stloc.1
	ldloc.s	7
	conv.ovf.i2.un
	stloc.2
	ldloc.s	7
	conv.ovf.u2.un
	stloc.3
	ldloc.s	7
	conv.ovf.i4.un
	stloc.s	4
	ldloc.s	7
	conv.ovf.u4.un
	stloc.s	5
	ldloc.s	7
	conv.ovf.i8.un
	stloc.s	6
	ldloc.s	7
	stloc.s	7
	ldloc.s	7
	conv.ovf.u2.un
	stloc.s	8
	ldloc.s	7
	conv.r.un
	conv.r4
	stloc.s	9
	ldloc.s	7
	conv.r.un
	conv.r8
	stloc.s	10
	ldloc.s	7
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int64)
	stloc.s	11
	ldloc.s	8
	conv.ovf.i1.un
	stloc.0
	ldloc.s	8
	conv.ovf.u1.un
	stloc.1
	ldloc.s	8
	conv.ovf.i2.un
	stloc.2
	ldloc.s	8
	stloc.3
	ldloc.s	8
	stloc.s	4
	ldloc.s	8
	stloc.s	5
	ldloc.s	8
	conv.i8
	stloc.s	6
	ldloc.s	8
	conv.u8
	stloc.s	7
	ldloc.s	8
	stloc.s	8
	ldloc.s	8
	conv.r.un
	stloc.s	9
	ldloc.s	8
	conv.r.un
	stloc.s	10
	ldloc.s	8
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(char)
	stloc.s	11
	ldloc.s	9
	conv.ovf.i1
	stloc.0
	ldloc.s	9
	conv.ovf.u1
	stloc.1
	ldloc.s	9
	conv.ovf.i2
	stloc.2
	ldloc.s	9
	conv.ovf.u2
	stloc.3
	ldloc.s	9
	conv.ovf.i4
	stloc.s	4
	ldloc.s	9
	conv.ovf.u4
	stloc.s	5
	ldloc.s	9
	conv.ovf.i8
	stloc.s	6
	ldloc.s	9
	conv.ovf.u8
	stloc.s	7
	ldloc.s	9
	conv.ovf.u2
	stloc.s	8
	ldloc.s	9
	stloc.s	9
	ldloc.s	9
	stloc.s	10
	ldloc.s	9
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Explicit(float32)
	stloc.s	11
	ldloc.s	10
	conv.ovf.i1
	stloc.0
	ldloc.s	10
	conv.ovf.u1
	stloc.1
	ldloc.s	10
	conv.ovf.i2
	stloc.2
	ldloc.s	10
	conv.ovf.u2
	stloc.3
	ldloc.s	10
	conv.ovf.i4
	stloc.s	4
	ldloc.s	10
	conv.ovf.u4
	stloc.s	5
	ldloc.s	10
	conv.ovf.i8
	stloc.s	6
	ldloc.s	10
	conv.ovf.u8
	stloc.s	7
	ldloc.s	10
	conv.ovf.u2
	stloc.s	8
	ldloc.s	10
	conv.r4
	stloc.s	9
	ldloc.s	10
	stloc.s	10
	ldloc.s	10
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Explicit(float64)
	stloc.s	11
	ldloc.s	11
	call	int8 [.library]System.Decimal::op_Explicit(valuetype [.library]System.Decimal)
	stloc.0
	ldloc.s	11
	call	unsigned int8 [.library]System.Decimal::op_Explicit(valuetype [.library]System.Decimal)
	stloc.1
	ldloc.s	11
	call	int16 [.library]System.Decimal::op_Explicit(valuetype [.library]System.Decimal)
	stloc.2
	ldloc.s	11
	call	unsigned int16 [.library]System.Decimal::op_Explicit(valuetype [.library]System.Decimal)
	stloc.3
	ldloc.s	11
	call	int32 [.library]System.Decimal::op_Explicit(valuetype [.library]System.Decimal)
	stloc.s	4
	ldloc.s	11
	call	unsigned int32 [.library]System.Decimal::op_Explicit(valuetype [.library]System.Decimal)
	stloc.s	5
	ldloc.s	11
	call	int64 [.library]System.Decimal::op_Explicit(valuetype [.library]System.Decimal)
	stloc.s	6
	ldloc.s	11
	call	unsigned int64 [.library]System.Decimal::op_Explicit(valuetype [.library]System.Decimal)
	stloc.s	7
	ldloc.s	11
	call	char [.library]System.Decimal::op_Explicit(valuetype [.library]System.Decimal)
	stloc.s	8
	ldloc.s	11
	call	float32 [.library]System.Decimal::op_Explicit(valuetype [.library]System.Decimal)
	stloc.s	9
	ldloc.s	11
	call	float64 [.library]System.Decimal::op_Explicit(valuetype [.library]System.Decimal)
	stloc.s	10
	ldloc.s	11
	stloc.s	11
	ret
	.maxstack 5
} // method m1
.method private hidebysig instance void 'm2'() cil managed 
{
	.locals init	(int8, unsigned int8, int16, unsigned int16, int32, unsigned int32, int64, unsigned int64, char, float32, float64, valuetype ['.library']'System'.'Decimal')
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
	ldloc.0
	stloc.0
	ldloc.0
	conv.u1
	stloc.1
	ldloc.0
	stloc.2
	ldloc.0
	conv.u2
	stloc.3
	ldloc.0
	stloc.s	4
	ldloc.0
	stloc.s	5
	ldloc.0
	conv.i8
	stloc.s	6
	ldloc.0
	conv.u8
	stloc.s	7
	ldloc.0
	conv.u2
	stloc.s	8
	ldloc.0
	conv.r4
	stloc.s	9
	ldloc.0
	conv.r8
	stloc.s	10
	ldloc.0
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int8)
	stloc.s	11
	ldloc.1
	conv.i1
	stloc.0
	ldloc.1
	stloc.1
	ldloc.1
	stloc.2
	ldloc.1
	stloc.3
	ldloc.1
	stloc.s	4
	ldloc.1
	stloc.s	5
	ldloc.1
	conv.i8
	stloc.s	6
	ldloc.1
	conv.u8
	stloc.s	7
	ldloc.1
	stloc.s	8
	ldloc.1
	conv.r.un
	stloc.s	9
	ldloc.1
	conv.r.un
	stloc.s	10
	ldloc.1
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int8)
	stloc.s	11
	ldloc.2
	conv.i1
	stloc.0
	ldloc.2
	conv.u1
	stloc.1
	ldloc.2
	stloc.2
	ldloc.2
	conv.u2
	stloc.3
	ldloc.2
	stloc.s	4
	ldloc.2
	stloc.s	5
	ldloc.2
	conv.i8
	stloc.s	6
	ldloc.2
	conv.u8
	stloc.s	7
	ldloc.2
	conv.u2
	stloc.s	8
	ldloc.2
	conv.r4
	stloc.s	9
	ldloc.2
	conv.r8
	stloc.s	10
	ldloc.2
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int16)
	stloc.s	11
	ldloc.3
	conv.i1
	stloc.0
	ldloc.3
	conv.u1
	stloc.1
	ldloc.3
	conv.i2
	stloc.2
	ldloc.3
	stloc.3
	ldloc.3
	stloc.s	4
	ldloc.3
	stloc.s	5
	ldloc.3
	conv.i8
	stloc.s	6
	ldloc.3
	conv.u8
	stloc.s	7
	ldloc.3
	stloc.s	8
	ldloc.3
	conv.r.un
	stloc.s	9
	ldloc.3
	conv.r.un
	stloc.s	10
	ldloc.3
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int16)
	stloc.s	11
	ldloc.s	4
	conv.i1
	stloc.0
	ldloc.s	4
	conv.u1
	stloc.1
	ldloc.s	4
	conv.i2
	stloc.2
	ldloc.s	4
	conv.u2
	stloc.3
	ldloc.s	4
	stloc.s	4
	ldloc.s	4
	stloc.s	5
	ldloc.s	4
	conv.i8
	stloc.s	6
	ldloc.s	4
	conv.u8
	stloc.s	7
	ldloc.s	4
	conv.u2
	stloc.s	8
	ldloc.s	4
	conv.r4
	stloc.s	9
	ldloc.s	4
	conv.r8
	stloc.s	10
	ldloc.s	4
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	stloc.s	11
	ldloc.s	5
	conv.i1
	stloc.0
	ldloc.s	5
	conv.u1
	stloc.1
	ldloc.s	5
	conv.i2
	stloc.2
	ldloc.s	5
	conv.u2
	stloc.3
	ldloc.s	5
	stloc.s	4
	ldloc.s	5
	stloc.s	5
	ldloc.s	5
	conv.u8
	stloc.s	6
	ldloc.s	5
	conv.u8
	stloc.s	7
	ldloc.s	5
	conv.u2
	stloc.s	8
	ldloc.s	5
	conv.r.un
	conv.r4
	stloc.s	9
	ldloc.s	5
	conv.r.un
	stloc.s	10
	ldloc.s	5
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int32)
	stloc.s	11
	ldloc.s	6
	conv.i1
	stloc.0
	ldloc.s	6
	conv.u1
	stloc.1
	ldloc.s	6
	conv.i2
	stloc.2
	ldloc.s	6
	conv.u2
	stloc.3
	ldloc.s	6
	conv.i4
	stloc.s	4
	ldloc.s	6
	conv.u4
	stloc.s	5
	ldloc.s	6
	stloc.s	6
	ldloc.s	6
	stloc.s	7
	ldloc.s	6
	conv.u2
	stloc.s	8
	ldloc.s	6
	conv.r4
	stloc.s	9
	ldloc.s	6
	conv.r8
	stloc.s	10
	ldloc.s	6
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int64)
	stloc.s	11
	ldloc.s	7
	conv.i1
	stloc.0
	ldloc.s	7
	conv.u1
	stloc.1
	ldloc.s	7
	conv.i2
	stloc.2
	ldloc.s	7
	conv.u2
	stloc.3
	ldloc.s	7
	conv.i4
	stloc.s	4
	ldloc.s	7
	conv.u4
	stloc.s	5
	ldloc.s	7
	stloc.s	6
	ldloc.s	7
	stloc.s	7
	ldloc.s	7
	conv.u2
	stloc.s	8
	ldloc.s	7
	conv.r.un
	conv.r4
	stloc.s	9
	ldloc.s	7
	conv.r.un
	conv.r8
	stloc.s	10
	ldloc.s	7
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int64)
	stloc.s	11
	ldloc.s	8
	conv.i1
	stloc.0
	ldloc.s	8
	conv.u1
	stloc.1
	ldloc.s	8
	conv.i2
	stloc.2
	ldloc.s	8
	stloc.3
	ldloc.s	8
	stloc.s	4
	ldloc.s	8
	stloc.s	5
	ldloc.s	8
	conv.i8
	stloc.s	6
	ldloc.s	8
	conv.u8
	stloc.s	7
	ldloc.s	8
	stloc.s	8
	ldloc.s	8
	conv.r.un
	stloc.s	9
	ldloc.s	8
	conv.r.un
	stloc.s	10
	ldloc.s	8
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(char)
	stloc.s	11
	ldloc.s	9
	conv.i1
	stloc.0
	ldloc.s	9
	conv.u1
	stloc.1
	ldloc.s	9
	conv.i2
	stloc.2
	ldloc.s	9
	conv.u2
	stloc.3
	ldloc.s	9
	conv.i4
	stloc.s	4
	ldloc.s	9
	conv.u4
	stloc.s	5
	ldloc.s	9
	conv.i8
	stloc.s	6
	ldloc.s	9
	conv.u8
	stloc.s	7
	ldloc.s	9
	conv.u2
	stloc.s	8
	ldloc.s	9
	stloc.s	9
	ldloc.s	9
	stloc.s	10
	ldloc.s	9
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Explicit(float32)
	stloc.s	11
	ldloc.s	10
	conv.i1
	stloc.0
	ldloc.s	10
	conv.u1
	stloc.1
	ldloc.s	10
	conv.i2
	stloc.2
	ldloc.s	10
	conv.u2
	stloc.3
	ldloc.s	10
	conv.i4
	stloc.s	4
	ldloc.s	10
	conv.u4
	stloc.s	5
	ldloc.s	10
	conv.i8
	stloc.s	6
	ldloc.s	10
	conv.u8
	stloc.s	7
	ldloc.s	10
	conv.u2
	stloc.s	8
	ldloc.s	10
	conv.r4
	stloc.s	9
	ldloc.s	10
	stloc.s	10
	ldloc.s	10
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Explicit(float64)
	stloc.s	11
	ldloc.s	11
	call	int8 [.library]System.Decimal::op_Explicit(valuetype [.library]System.Decimal)
	stloc.0
	ldloc.s	11
	call	unsigned int8 [.library]System.Decimal::op_Explicit(valuetype [.library]System.Decimal)
	stloc.1
	ldloc.s	11
	call	int16 [.library]System.Decimal::op_Explicit(valuetype [.library]System.Decimal)
	stloc.2
	ldloc.s	11
	call	unsigned int16 [.library]System.Decimal::op_Explicit(valuetype [.library]System.Decimal)
	stloc.3
	ldloc.s	11
	call	int32 [.library]System.Decimal::op_Explicit(valuetype [.library]System.Decimal)
	stloc.s	4
	ldloc.s	11
	call	unsigned int32 [.library]System.Decimal::op_Explicit(valuetype [.library]System.Decimal)
	stloc.s	5
	ldloc.s	11
	call	int64 [.library]System.Decimal::op_Explicit(valuetype [.library]System.Decimal)
	stloc.s	6
	ldloc.s	11
	call	unsigned int64 [.library]System.Decimal::op_Explicit(valuetype [.library]System.Decimal)
	stloc.s	7
	ldloc.s	11
	call	char [.library]System.Decimal::op_Explicit(valuetype [.library]System.Decimal)
	stloc.s	8
	ldloc.s	11
	call	float32 [.library]System.Decimal::op_Explicit(valuetype [.library]System.Decimal)
	stloc.s	9
	ldloc.s	11
	call	float64 [.library]System.Decimal::op_Explicit(valuetype [.library]System.Decimal)
	stloc.s	10
	ldloc.s	11
	stloc.s	11
	ret
	.maxstack 5
} // method m2
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
