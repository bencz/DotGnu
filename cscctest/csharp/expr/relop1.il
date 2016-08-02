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
	.locals init	(bool, int8, unsigned int8, int16, unsigned int16, int32, unsigned int32, int64, unsigned int64, char, float32, float64, valuetype ['.library']'System'.'Decimal', class ['.library']'System'.'Object', int8, unsigned int8, int16, unsigned int16, int32, unsigned int32, int64, unsigned int64, char, float32, float64, valuetype ['.library']'System'.'Decimal', class ['.library']'System'.'Object')
	ldc.i4.s	34
	conv.i1
	stloc.1
	ldc.i4.s	42
	conv.u1
	stloc.2
	ldc.i4.s	-126
	conv.i2
	stloc.3
	ldc.i4.s	67
	conv.u2
	stloc.s	4
	ldc.i4	4294966062
	stloc.s	5
	ldc.i4	54321
	stloc.s	6
	ldc.i8	0xfffffffdb34fe916
	stloc.s	7
	ldc.i8	0x000000024cb016ea
	stloc.s	8
	ldc.i4.s	65
	stloc.s	9
	ldc.r4	float32(0x3FC00000)
	stloc.s	10
	ldc.r8	float64(0x401ACCCCCCCCCCCD)
	stloc.s	11
	ldc.i4.s	35
	ldc.i4.0
	ldc.i4.0
	ldc.i4.0
	ldc.i4.1
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stloc.s	12
	ldnull
	stloc.s	13
	ldc.i4.s	34
	conv.i1
	stloc.s	14
	ldc.i4.s	42
	conv.u1
	stloc.s	15
	ldc.i4.s	-126
	conv.i2
	stloc.s	16
	ldc.i4.s	67
	conv.u2
	stloc.s	17
	ldc.i4	4294966062
	stloc.s	18
	ldc.i4	54321
	stloc.s	19
	ldc.i8	0xfffffffdb34fe916
	stloc.s	20
	ldc.i8	0x000000024cb016ea
	stloc.s	21
	ldc.i4.s	65
	stloc.s	22
	ldc.r4	float32(0x3FC00000)
	stloc.s	23
	ldc.r8	float64(0x401ACCCCCCCCCCCD)
	stloc.s	24
	ldc.i4.s	35
	ldc.i4.0
	ldc.i4.0
	ldc.i4.0
	ldc.i4.1
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stloc.s	25
	ldnull
	stloc.s	26
	ldloc.1
	ldloc.s	14
	ceq
	stloc.0
	ldloc.1
	ldloc.s	15
	ceq
	stloc.0
	ldloc.1
	ldloc.s	16
	ceq
	stloc.0
	ldloc.1
	ldloc.s	17
	ceq
	stloc.0
	ldloc.1
	ldloc.s	18
	ceq
	stloc.0
	ldloc.1
	conv.i8
	ldloc.s	19
	conv.u8
	ceq
	stloc.0
	ldloc.1
	conv.i8
	ldloc.s	20
	ceq
	stloc.0
	ldloc.1
	ldloc.s	22
	ceq
	stloc.0
	ldloc.1
	conv.r4
	ldloc.s	23
	ceq
	stloc.0
	ldloc.1
	conv.r8
	ldloc.s	24
	ceq
	stloc.0
	ldloc.1
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int8)
	ldloc.s	25
	call	bool [.library]System.Decimal::op_Equality(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.0
	ldloc.2
	ldloc.s	14
	ceq
	ldc.i4.1
	xor
	stloc.0
	ldloc.2
	ldloc.s	15
	ceq
	ldc.i4.1
	xor
	stloc.0
	ldloc.2
	ldloc.s	16
	ceq
	ldc.i4.1
	xor
	stloc.0
	ldloc.2
	ldloc.s	17
	ceq
	ldc.i4.1
	xor
	stloc.0
	ldloc.2
	ldloc.s	18
	ceq
	ldc.i4.1
	xor
	stloc.0
	ldloc.2
	ldloc.s	19
	ceq
	ldc.i4.1
	xor
	stloc.0
	ldloc.2
	conv.i8
	ldloc.s	20
	ceq
	ldc.i4.1
	xor
	stloc.0
	ldloc.2
	conv.u8
	ldloc.s	21
	ceq
	ldc.i4.1
	xor
	stloc.0
	ldloc.2
	ldloc.s	22
	ceq
	ldc.i4.1
	xor
	stloc.0
	ldloc.2
	conv.r.un
	ldloc.s	23
	ceq
	ldc.i4.1
	xor
	stloc.0
	ldloc.2
	conv.r.un
	ldloc.s	24
	ceq
	ldc.i4.1
	xor
	stloc.0
	ldloc.2
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int8)
	ldloc.s	25
	call	bool [.library]System.Decimal::op_Inequality(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.0
	ldloc.3
	ldloc.s	14
	clt
	stloc.0
	ldloc.3
	ldloc.s	15
	clt
	stloc.0
	ldloc.3
	ldloc.s	16
	clt
	stloc.0
	ldloc.3
	ldloc.s	17
	clt
	stloc.0
	ldloc.3
	ldloc.s	18
	clt
	stloc.0
	ldloc.3
	conv.i8
	ldloc.s	19
	conv.u8
	clt
	stloc.0
	ldloc.3
	conv.i8
	ldloc.s	20
	clt
	stloc.0
	ldloc.3
	ldloc.s	22
	clt
	stloc.0
	ldloc.3
	conv.r4
	ldloc.s	23
	clt
	stloc.0
	ldloc.3
	conv.r8
	ldloc.s	24
	clt
	stloc.0
	ldloc.3
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int16)
	ldloc.s	25
	call	bool [.library]System.Decimal::op_LessThan(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.0
	ldloc.s	4
	ldloc.s	14
	cgt
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	4
	ldloc.s	15
	cgt
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	4
	ldloc.s	16
	cgt
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	4
	ldloc.s	17
	cgt
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	4
	ldloc.s	18
	cgt
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	4
	ldloc.s	19
	cgt.un
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	4
	conv.i8
	ldloc.s	20
	cgt
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	4
	conv.u8
	ldloc.s	21
	cgt.un
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	4
	ldloc.s	22
	cgt
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	4
	conv.r.un
	ldloc.s	23
	cgt.un
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	4
	conv.r.un
	ldloc.s	24
	cgt.un
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	4
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int16)
	ldloc.s	25
	call	bool [.library]System.Decimal::op_LessThanOrEqual(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.0
	ldloc.s	5
	ldloc.s	14
	cgt
	stloc.0
	ldloc.s	5
	ldloc.s	15
	cgt
	stloc.0
	ldloc.s	5
	ldloc.s	16
	cgt
	stloc.0
	ldloc.s	5
	ldloc.s	17
	cgt
	stloc.0
	ldloc.s	5
	ldloc.s	18
	cgt
	stloc.0
	ldloc.s	5
	conv.i8
	ldloc.s	19
	conv.u8
	cgt
	stloc.0
	ldloc.s	5
	conv.i8
	ldloc.s	20
	cgt
	stloc.0
	ldloc.s	5
	ldloc.s	22
	cgt
	stloc.0
	ldloc.s	5
	conv.r4
	ldloc.s	23
	cgt
	stloc.0
	ldloc.s	5
	conv.r8
	ldloc.s	24
	cgt
	stloc.0
	ldloc.s	5
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	ldloc.s	25
	call	bool [.library]System.Decimal::op_GreaterThan(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.0
	ldloc.s	6
	conv.u8
	ldloc.s	14
	conv.i8
	clt
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	6
	ldloc.s	15
	clt.un
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	6
	conv.u8
	ldloc.s	16
	conv.i8
	clt
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	6
	ldloc.s	17
	clt.un
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	6
	conv.u8
	ldloc.s	18
	conv.i8
	clt
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	6
	ldloc.s	19
	clt.un
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	6
	conv.u8
	ldloc.s	20
	clt
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	6
	conv.u8
	ldloc.s	21
	clt.un
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	6
	ldloc.s	22
	clt.un
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	6
	conv.r.un
	conv.r4
	ldloc.s	23
	clt.un
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	6
	conv.r.un
	ldloc.s	24
	clt.un
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	6
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int32)
	ldloc.s	25
	call	bool [.library]System.Decimal::op_GreaterThanOrEqual(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.0
	ldloc.s	7
	ldloc.s	14
	conv.i8
	ceq
	stloc.0
	ldloc.s	7
	ldloc.s	15
	conv.i8
	ceq
	stloc.0
	ldloc.s	7
	ldloc.s	16
	conv.i8
	ceq
	stloc.0
	ldloc.s	7
	ldloc.s	17
	conv.i8
	ceq
	stloc.0
	ldloc.s	7
	ldloc.s	18
	conv.i8
	ceq
	stloc.0
	ldloc.s	7
	ldloc.s	19
	conv.u8
	ceq
	stloc.0
	ldloc.s	7
	ldloc.s	20
	ceq
	stloc.0
	ldloc.s	7
	ldloc.s	22
	conv.i8
	ceq
	stloc.0
	ldloc.s	7
	conv.r4
	ldloc.s	23
	ceq
	stloc.0
	ldloc.s	7
	conv.r8
	ldloc.s	24
	ceq
	stloc.0
	ldloc.s	7
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int64)
	ldloc.s	25
	call	bool [.library]System.Decimal::op_Equality(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.0
	ldloc.s	8
	ldloc.s	15
	conv.u8
	ceq
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	8
	ldloc.s	17
	conv.u8
	ceq
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	8
	ldloc.s	19
	conv.u8
	ceq
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	8
	ldloc.s	21
	ceq
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	8
	ldloc.s	22
	conv.u8
	ceq
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	8
	conv.r.un
	conv.r4
	ldloc.s	23
	ceq
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	8
	conv.r.un
	conv.r8
	ldloc.s	24
	ceq
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	8
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int64)
	ldloc.s	25
	call	bool [.library]System.Decimal::op_Inequality(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.0
	ldloc.s	10
	ldloc.s	14
	conv.r4
	clt
	stloc.0
	ldloc.s	10
	ldloc.s	15
	conv.r.un
	clt
	stloc.0
	ldloc.s	10
	ldloc.s	16
	conv.r4
	clt
	stloc.0
	ldloc.s	10
	ldloc.s	17
	conv.r.un
	clt
	stloc.0
	ldloc.s	10
	ldloc.s	18
	conv.r4
	clt
	stloc.0
	ldloc.s	10
	ldloc.s	19
	conv.r.un
	conv.r4
	clt
	stloc.0
	ldloc.s	10
	ldloc.s	20
	conv.r4
	clt
	stloc.0
	ldloc.s	10
	ldloc.s	21
	conv.r.un
	conv.r4
	clt
	stloc.0
	ldloc.s	10
	ldloc.s	22
	conv.r.un
	clt
	stloc.0
	ldloc.s	10
	ldloc.s	23
	clt
	stloc.0
	ldloc.s	10
	ldloc.s	24
	clt
	stloc.0
	ldloc.s	11
	ldloc.s	14
	conv.r8
	cgt.un
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	11
	ldloc.s	15
	conv.r.un
	cgt.un
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	11
	ldloc.s	16
	conv.r8
	cgt.un
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	11
	ldloc.s	17
	conv.r.un
	cgt.un
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	11
	ldloc.s	18
	conv.r8
	cgt.un
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	11
	ldloc.s	19
	conv.r.un
	cgt.un
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	11
	ldloc.s	20
	conv.r8
	cgt.un
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	11
	ldloc.s	21
	conv.r.un
	conv.r8
	cgt.un
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	11
	ldloc.s	22
	conv.r.un
	cgt.un
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	11
	ldloc.s	23
	cgt.un
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	11
	ldloc.s	24
	cgt.un
	ldc.i4.1
	xor
	stloc.0
	ldloc.s	12
	ldloc.s	14
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int8)
	call	bool [.library]System.Decimal::op_GreaterThan(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.0
	ldloc.s	12
	ldloc.s	15
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int8)
	call	bool [.library]System.Decimal::op_GreaterThanOrEqual(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.0
	ldloc.s	12
	ldloc.s	16
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int16)
	call	bool [.library]System.Decimal::op_GreaterThan(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.0
	ldloc.s	12
	ldloc.s	17
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int16)
	call	bool [.library]System.Decimal::op_GreaterThanOrEqual(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.0
	ldloc.s	12
	ldloc.s	18
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	call	bool [.library]System.Decimal::op_GreaterThan(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.0
	ldloc.s	12
	ldloc.s	19
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int32)
	call	bool [.library]System.Decimal::op_GreaterThanOrEqual(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.0
	ldloc.s	12
	ldloc.s	20
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int64)
	call	bool [.library]System.Decimal::op_GreaterThan(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.0
	ldloc.s	12
	ldloc.s	21
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int64)
	call	bool [.library]System.Decimal::op_GreaterThanOrEqual(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.0
	ldloc.s	12
	ldloc.s	22
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(char)
	call	bool [.library]System.Decimal::op_GreaterThan(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.0
	ldloc.s	12
	ldloc.s	25
	call	bool [.library]System.Decimal::op_GreaterThanOrEqual(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stloc.0
	ldloc.s	13
	ldloc.s	26
	ceq
	stloc.0
	ldloc.s	13
	ldloc.s	26
	ceq
	ldc.i4.1
	xor
	stloc.0
	br	?L1
?L2:
	ldc.i4.1
	stloc.0
?L1:
	ldloc.1
	ldloc.s	14
	beq	?L2
?L3:
	br	?L4
?L5:
	ldc.i4.1
	stloc.0
?L4:
	ldloc.1
	ldloc.s	15
	beq	?L5
?L6:
	br	?L7
?L8:
	ldc.i4.1
	stloc.0
?L7:
	ldloc.1
	ldloc.s	16
	beq	?L8
?L9:
	br	?L10
?L11:
	ldc.i4.1
	stloc.0
?L10:
	ldloc.1
	ldloc.s	17
	beq	?L11
?L12:
	br	?L13
?L14:
	ldc.i4.1
	stloc.0
?L13:
	ldloc.1
	ldloc.s	18
	beq	?L14
?L15:
	br	?L16
?L17:
	ldc.i4.1
	stloc.0
?L16:
	ldloc.1
	conv.i8
	ldloc.s	19
	conv.u8
	beq	?L17
?L18:
	br	?L19
?L20:
	ldc.i4.1
	stloc.0
?L19:
	ldloc.1
	conv.i8
	ldloc.s	20
	beq	?L20
?L21:
	br	?L22
?L23:
	ldc.i4.1
	stloc.0
?L22:
	ldloc.1
	ldloc.s	22
	beq	?L23
?L24:
	br	?L25
?L26:
	ldc.i4.1
	stloc.0
?L25:
	ldloc.1
	conv.r4
	ldloc.s	23
	beq	?L26
?L27:
	br	?L28
?L29:
	ldc.i4.1
	stloc.0
?L28:
	ldloc.1
	conv.r8
	ldloc.s	24
	beq	?L29
?L30:
	br	?L31
?L32:
	ldc.i4.1
	stloc.0
?L31:
	ldloc.1
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int8)
	ldloc.s	25
	call	bool [.library]System.Decimal::op_Equality(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brtrue	?L32
?L33:
	br	?L34
?L35:
	ldc.i4.1
	stloc.0
?L34:
	ldloc.2
	ldloc.s	14
	bne.un	?L35
?L36:
	br	?L37
?L38:
	ldc.i4.1
	stloc.0
?L37:
	ldloc.2
	ldloc.s	15
	bne.un	?L38
?L39:
	br	?L40
?L41:
	ldc.i4.1
	stloc.0
?L40:
	ldloc.2
	ldloc.s	16
	bne.un	?L41
?L42:
	br	?L43
?L44:
	ldc.i4.1
	stloc.0
?L43:
	ldloc.2
	ldloc.s	17
	bne.un	?L44
?L45:
	br	?L46
?L47:
	ldc.i4.1
	stloc.0
?L46:
	ldloc.2
	ldloc.s	18
	bne.un	?L47
?L48:
	br	?L49
?L50:
	ldc.i4.1
	stloc.0
?L49:
	ldloc.2
	ldloc.s	19
	bne.un	?L50
?L51:
	br	?L52
?L53:
	ldc.i4.1
	stloc.0
?L52:
	ldloc.2
	conv.i8
	ldloc.s	20
	bne.un	?L53
?L54:
	br	?L55
?L56:
	ldc.i4.1
	stloc.0
?L55:
	ldloc.2
	conv.u8
	ldloc.s	21
	bne.un	?L56
?L57:
	br	?L58
?L59:
	ldc.i4.1
	stloc.0
?L58:
	ldloc.2
	ldloc.s	22
	bne.un	?L59
?L60:
	br	?L61
?L62:
	ldc.i4.1
	stloc.0
?L61:
	ldloc.2
	conv.r.un
	ldloc.s	23
	bne.un	?L62
?L63:
	br	?L64
?L65:
	ldc.i4.1
	stloc.0
?L64:
	ldloc.2
	conv.r.un
	ldloc.s	24
	bne.un	?L65
?L66:
	br	?L67
?L68:
	ldc.i4.1
	stloc.0
?L67:
	ldloc.2
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int8)
	ldloc.s	25
	call	bool [.library]System.Decimal::op_Inequality(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brtrue	?L68
?L69:
	br	?L70
?L71:
	ldc.i4.1
	stloc.0
?L70:
	ldloc.3
	ldloc.s	14
	blt	?L71
?L72:
	br	?L73
?L74:
	ldc.i4.1
	stloc.0
?L73:
	ldloc.3
	ldloc.s	15
	blt	?L74
?L75:
	br	?L76
?L77:
	ldc.i4.1
	stloc.0
?L76:
	ldloc.3
	ldloc.s	16
	blt	?L77
?L78:
	br	?L79
?L80:
	ldc.i4.1
	stloc.0
?L79:
	ldloc.3
	ldloc.s	17
	blt	?L80
?L81:
	br	?L82
?L83:
	ldc.i4.1
	stloc.0
?L82:
	ldloc.3
	ldloc.s	18
	blt	?L83
?L84:
	br	?L85
?L86:
	ldc.i4.1
	stloc.0
?L85:
	ldloc.3
	conv.i8
	ldloc.s	19
	conv.u8
	blt	?L86
?L87:
	br	?L88
?L89:
	ldc.i4.1
	stloc.0
?L88:
	ldloc.3
	conv.i8
	ldloc.s	20
	blt	?L89
?L90:
	br	?L91
?L92:
	ldc.i4.1
	stloc.0
?L91:
	ldloc.3
	ldloc.s	22
	blt	?L92
?L93:
	br	?L94
?L95:
	ldc.i4.1
	stloc.0
?L94:
	ldloc.3
	conv.r4
	ldloc.s	23
	blt	?L95
?L96:
	br	?L97
?L98:
	ldc.i4.1
	stloc.0
?L97:
	ldloc.3
	conv.r8
	ldloc.s	24
	blt	?L98
?L99:
	br	?L100
?L101:
	ldc.i4.1
	stloc.0
?L100:
	ldloc.3
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int16)
	ldloc.s	25
	call	bool [.library]System.Decimal::op_LessThan(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brtrue	?L101
?L102:
	br	?L103
?L104:
	ldc.i4.1
	stloc.0
?L103:
	ldloc.s	4
	ldloc.s	14
	ble	?L104
?L105:
	br	?L106
?L107:
	ldc.i4.1
	stloc.0
?L106:
	ldloc.s	4
	ldloc.s	15
	ble	?L107
?L108:
	br	?L109
?L110:
	ldc.i4.1
	stloc.0
?L109:
	ldloc.s	4
	ldloc.s	16
	ble	?L110
?L111:
	br	?L112
?L113:
	ldc.i4.1
	stloc.0
?L112:
	ldloc.s	4
	ldloc.s	17
	ble	?L113
?L114:
	br	?L115
?L116:
	ldc.i4.1
	stloc.0
?L115:
	ldloc.s	4
	ldloc.s	18
	ble	?L116
?L117:
	br	?L118
?L119:
	ldc.i4.1
	stloc.0
?L118:
	ldloc.s	4
	ldloc.s	19
	ble.un	?L119
?L120:
	br	?L121
?L122:
	ldc.i4.1
	stloc.0
?L121:
	ldloc.s	4
	conv.i8
	ldloc.s	20
	ble	?L122
?L123:
	br	?L124
?L125:
	ldc.i4.1
	stloc.0
?L124:
	ldloc.s	4
	conv.u8
	ldloc.s	21
	ble.un	?L125
?L126:
	br	?L127
?L128:
	ldc.i4.1
	stloc.0
?L127:
	ldloc.s	4
	ldloc.s	22
	ble	?L128
?L129:
	br	?L130
?L131:
	ldc.i4.1
	stloc.0
?L130:
	ldloc.s	4
	conv.r.un
	ldloc.s	23
	ble	?L131
?L132:
	br	?L133
?L134:
	ldc.i4.1
	stloc.0
?L133:
	ldloc.s	4
	conv.r.un
	ldloc.s	24
	ble	?L134
?L135:
	br	?L136
?L137:
	ldc.i4.1
	stloc.0
?L136:
	ldloc.s	4
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int16)
	ldloc.s	25
	call	bool [.library]System.Decimal::op_LessThanOrEqual(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brtrue	?L137
?L138:
	br	?L139
?L140:
	ldc.i4.1
	stloc.0
?L139:
	ldloc.s	5
	ldloc.s	14
	bgt	?L140
?L141:
	br	?L142
?L143:
	ldc.i4.1
	stloc.0
?L142:
	ldloc.s	5
	ldloc.s	15
	bgt	?L143
?L144:
	br	?L145
?L146:
	ldc.i4.1
	stloc.0
?L145:
	ldloc.s	5
	ldloc.s	16
	bgt	?L146
?L147:
	br	?L148
?L149:
	ldc.i4.1
	stloc.0
?L148:
	ldloc.s	5
	ldloc.s	17
	bgt	?L149
?L150:
	br	?L151
?L152:
	ldc.i4.1
	stloc.0
?L151:
	ldloc.s	5
	ldloc.s	18
	bgt	?L152
?L153:
	br	?L154
?L155:
	ldc.i4.1
	stloc.0
?L154:
	ldloc.s	5
	conv.i8
	ldloc.s	19
	conv.u8
	bgt	?L155
?L156:
	br	?L157
?L158:
	ldc.i4.1
	stloc.0
?L157:
	ldloc.s	5
	conv.i8
	ldloc.s	20
	bgt	?L158
?L159:
	br	?L160
?L161:
	ldc.i4.1
	stloc.0
?L160:
	ldloc.s	5
	ldloc.s	22
	bgt	?L161
?L162:
	br	?L163
?L164:
	ldc.i4.1
	stloc.0
?L163:
	ldloc.s	5
	conv.r4
	ldloc.s	23
	bgt	?L164
?L165:
	br	?L166
?L167:
	ldc.i4.1
	stloc.0
?L166:
	ldloc.s	5
	conv.r8
	ldloc.s	24
	bgt	?L167
?L168:
	br	?L169
?L170:
	ldc.i4.1
	stloc.0
?L169:
	ldloc.s	5
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	ldloc.s	25
	call	bool [.library]System.Decimal::op_GreaterThan(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brtrue	?L170
?L171:
	br	?L172
?L173:
	ldc.i4.1
	stloc.0
?L172:
	ldloc.s	6
	conv.u8
	ldloc.s	14
	conv.i8
	bge	?L173
?L174:
	br	?L175
?L176:
	ldc.i4.1
	stloc.0
?L175:
	ldloc.s	6
	ldloc.s	15
	bge.un	?L176
?L177:
	br	?L178
?L179:
	ldc.i4.1
	stloc.0
?L178:
	ldloc.s	6
	conv.u8
	ldloc.s	16
	conv.i8
	bge	?L179
?L180:
	br	?L181
?L182:
	ldc.i4.1
	stloc.0
?L181:
	ldloc.s	6
	ldloc.s	17
	bge.un	?L182
?L183:
	br	?L184
?L185:
	ldc.i4.1
	stloc.0
?L184:
	ldloc.s	6
	conv.u8
	ldloc.s	18
	conv.i8
	bge	?L185
?L186:
	br	?L187
?L188:
	ldc.i4.1
	stloc.0
?L187:
	ldloc.s	6
	ldloc.s	19
	bge.un	?L188
?L189:
	br	?L190
?L191:
	ldc.i4.1
	stloc.0
?L190:
	ldloc.s	6
	conv.u8
	ldloc.s	20
	bge	?L191
?L192:
	br	?L193
?L194:
	ldc.i4.1
	stloc.0
?L193:
	ldloc.s	6
	conv.u8
	ldloc.s	21
	bge.un	?L194
?L195:
	br	?L196
?L197:
	ldc.i4.1
	stloc.0
?L196:
	ldloc.s	6
	ldloc.s	22
	bge.un	?L197
?L198:
	br	?L199
?L200:
	ldc.i4.1
	stloc.0
?L199:
	ldloc.s	6
	conv.r.un
	conv.r4
	ldloc.s	23
	bge	?L200
?L201:
	br	?L202
?L203:
	ldc.i4.1
	stloc.0
?L202:
	ldloc.s	6
	conv.r.un
	ldloc.s	24
	bge	?L203
?L204:
	br	?L205
?L206:
	ldc.i4.1
	stloc.0
?L205:
	ldloc.s	6
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int32)
	ldloc.s	25
	call	bool [.library]System.Decimal::op_GreaterThanOrEqual(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brtrue	?L206
?L207:
	br	?L208
?L209:
	ldc.i4.1
	stloc.0
?L208:
	ldloc.s	7
	ldloc.s	14
	conv.i8
	beq	?L209
?L210:
	br	?L211
?L212:
	ldc.i4.1
	stloc.0
?L211:
	ldloc.s	7
	ldloc.s	15
	conv.i8
	beq	?L212
?L213:
	br	?L214
?L215:
	ldc.i4.1
	stloc.0
?L214:
	ldloc.s	7
	ldloc.s	16
	conv.i8
	beq	?L215
?L216:
	br	?L217
?L218:
	ldc.i4.1
	stloc.0
?L217:
	ldloc.s	7
	ldloc.s	17
	conv.i8
	beq	?L218
?L219:
	br	?L220
?L221:
	ldc.i4.1
	stloc.0
?L220:
	ldloc.s	7
	ldloc.s	18
	conv.i8
	beq	?L221
?L222:
	br	?L223
?L224:
	ldc.i4.1
	stloc.0
?L223:
	ldloc.s	7
	ldloc.s	19
	conv.u8
	beq	?L224
?L225:
	br	?L226
?L227:
	ldc.i4.1
	stloc.0
?L226:
	ldloc.s	7
	ldloc.s	20
	beq	?L227
?L228:
	br	?L229
?L230:
	ldc.i4.1
	stloc.0
?L229:
	ldloc.s	7
	ldloc.s	22
	conv.i8
	beq	?L230
?L231:
	br	?L232
?L233:
	ldc.i4.1
	stloc.0
?L232:
	ldloc.s	7
	conv.r4
	ldloc.s	23
	beq	?L233
?L234:
	br	?L235
?L236:
	ldc.i4.1
	stloc.0
?L235:
	ldloc.s	7
	conv.r8
	ldloc.s	24
	beq	?L236
?L237:
	br	?L238
?L239:
	ldc.i4.1
	stloc.0
?L238:
	ldloc.s	7
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int64)
	ldloc.s	25
	call	bool [.library]System.Decimal::op_Equality(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brtrue	?L239
?L240:
	br	?L241
?L242:
	ldc.i4.1
	stloc.0
?L241:
	ldloc.s	8
	ldloc.s	15
	conv.u8
	bne.un	?L242
?L243:
	br	?L244
?L245:
	ldc.i4.1
	stloc.0
?L244:
	ldloc.s	8
	ldloc.s	17
	conv.u8
	bne.un	?L245
?L246:
	br	?L247
?L248:
	ldc.i4.1
	stloc.0
?L247:
	ldloc.s	8
	ldloc.s	19
	conv.u8
	bne.un	?L248
?L249:
	br	?L250
?L251:
	ldc.i4.1
	stloc.0
?L250:
	ldloc.s	8
	ldloc.s	21
	bne.un	?L251
?L252:
	br	?L253
?L254:
	ldc.i4.1
	stloc.0
?L253:
	ldloc.s	8
	ldloc.s	22
	conv.u8
	bne.un	?L254
?L255:
	br	?L256
?L257:
	ldc.i4.1
	stloc.0
?L256:
	ldloc.s	8
	conv.r.un
	conv.r4
	ldloc.s	23
	bne.un	?L257
?L258:
	br	?L259
?L260:
	ldc.i4.1
	stloc.0
?L259:
	ldloc.s	8
	conv.r.un
	conv.r8
	ldloc.s	24
	bne.un	?L260
?L261:
	br	?L262
?L263:
	ldc.i4.1
	stloc.0
?L262:
	ldloc.s	8
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int64)
	ldloc.s	25
	call	bool [.library]System.Decimal::op_Inequality(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brtrue	?L263
?L264:
	br	?L265
?L266:
	ldc.i4.1
	stloc.0
?L265:
	ldloc.s	10
	ldloc.s	14
	conv.r4
	blt	?L266
?L267:
	br	?L268
?L269:
	ldc.i4.1
	stloc.0
?L268:
	ldloc.s	10
	ldloc.s	15
	conv.r.un
	blt	?L269
?L270:
	br	?L271
?L272:
	ldc.i4.1
	stloc.0
?L271:
	ldloc.s	10
	ldloc.s	16
	conv.r4
	blt	?L272
?L273:
	br	?L274
?L275:
	ldc.i4.1
	stloc.0
?L274:
	ldloc.s	10
	ldloc.s	17
	conv.r.un
	blt	?L275
?L276:
	br	?L277
?L278:
	ldc.i4.1
	stloc.0
?L277:
	ldloc.s	10
	ldloc.s	18
	conv.r4
	blt	?L278
?L279:
	br	?L280
?L281:
	ldc.i4.1
	stloc.0
?L280:
	ldloc.s	10
	ldloc.s	19
	conv.r.un
	conv.r4
	blt	?L281
?L282:
	br	?L283
?L284:
	ldc.i4.1
	stloc.0
?L283:
	ldloc.s	10
	ldloc.s	20
	conv.r4
	blt	?L284
?L285:
	br	?L286
?L287:
	ldc.i4.1
	stloc.0
?L286:
	ldloc.s	10
	ldloc.s	21
	conv.r.un
	conv.r4
	blt	?L287
?L288:
	br	?L289
?L290:
	ldc.i4.1
	stloc.0
?L289:
	ldloc.s	10
	ldloc.s	22
	conv.r.un
	blt	?L290
?L291:
	br	?L292
?L293:
	ldc.i4.1
	stloc.0
?L292:
	ldloc.s	10
	ldloc.s	23
	blt	?L293
?L294:
	br	?L295
?L296:
	ldc.i4.1
	stloc.0
?L295:
	ldloc.s	10
	ldloc.s	24
	blt	?L296
?L297:
	br	?L298
?L299:
	ldc.i4.1
	stloc.0
?L298:
	ldloc.s	11
	ldloc.s	14
	conv.r8
	ble	?L299
?L300:
	br	?L301
?L302:
	ldc.i4.1
	stloc.0
?L301:
	ldloc.s	11
	ldloc.s	15
	conv.r.un
	ble	?L302
?L303:
	br	?L304
?L305:
	ldc.i4.1
	stloc.0
?L304:
	ldloc.s	11
	ldloc.s	16
	conv.r8
	ble	?L305
?L306:
	br	?L307
?L308:
	ldc.i4.1
	stloc.0
?L307:
	ldloc.s	11
	ldloc.s	17
	conv.r.un
	ble	?L308
?L309:
	br	?L310
?L311:
	ldc.i4.1
	stloc.0
?L310:
	ldloc.s	11
	ldloc.s	18
	conv.r8
	ble	?L311
?L312:
	br	?L313
?L314:
	ldc.i4.1
	stloc.0
?L313:
	ldloc.s	11
	ldloc.s	19
	conv.r.un
	ble	?L314
?L315:
	br	?L316
?L317:
	ldc.i4.1
	stloc.0
?L316:
	ldloc.s	11
	ldloc.s	20
	conv.r8
	ble	?L317
?L318:
	br	?L319
?L320:
	ldc.i4.1
	stloc.0
?L319:
	ldloc.s	11
	ldloc.s	21
	conv.r.un
	conv.r8
	ble	?L320
?L321:
	br	?L322
?L323:
	ldc.i4.1
	stloc.0
?L322:
	ldloc.s	11
	ldloc.s	22
	conv.r.un
	ble	?L323
?L324:
	br	?L325
?L326:
	ldc.i4.1
	stloc.0
?L325:
	ldloc.s	11
	ldloc.s	23
	ble	?L326
?L327:
	br	?L328
?L329:
	ldc.i4.1
	stloc.0
?L328:
	ldloc.s	11
	ldloc.s	24
	ble	?L329
?L330:
	br	?L331
?L332:
	ldc.i4.1
	stloc.0
?L331:
	ldloc.s	12
	ldloc.s	14
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int8)
	call	bool [.library]System.Decimal::op_GreaterThan(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brtrue	?L332
?L333:
	br	?L334
?L335:
	ldc.i4.1
	stloc.0
?L334:
	ldloc.s	12
	ldloc.s	15
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int8)
	call	bool [.library]System.Decimal::op_GreaterThanOrEqual(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brtrue	?L335
?L336:
	br	?L337
?L338:
	ldc.i4.1
	stloc.0
?L337:
	ldloc.s	12
	ldloc.s	16
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int16)
	call	bool [.library]System.Decimal::op_GreaterThan(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brtrue	?L338
?L339:
	br	?L340
?L341:
	ldc.i4.1
	stloc.0
?L340:
	ldloc.s	12
	ldloc.s	17
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int16)
	call	bool [.library]System.Decimal::op_GreaterThanOrEqual(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brtrue	?L341
?L342:
	br	?L343
?L344:
	ldc.i4.1
	stloc.0
?L343:
	ldloc.s	12
	ldloc.s	18
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	call	bool [.library]System.Decimal::op_GreaterThan(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brtrue	?L344
?L345:
	br	?L346
?L347:
	ldc.i4.1
	stloc.0
?L346:
	ldloc.s	12
	ldloc.s	19
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int32)
	call	bool [.library]System.Decimal::op_GreaterThanOrEqual(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brtrue	?L347
?L348:
	br	?L349
?L350:
	ldc.i4.1
	stloc.0
?L349:
	ldloc.s	12
	ldloc.s	20
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int64)
	call	bool [.library]System.Decimal::op_GreaterThan(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brtrue	?L350
?L351:
	br	?L352
?L353:
	ldc.i4.1
	stloc.0
?L352:
	ldloc.s	12
	ldloc.s	21
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int64)
	call	bool [.library]System.Decimal::op_GreaterThanOrEqual(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brtrue	?L353
?L354:
	br	?L355
?L356:
	ldc.i4.1
	stloc.0
?L355:
	ldloc.s	12
	ldloc.s	22
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(char)
	call	bool [.library]System.Decimal::op_GreaterThan(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brtrue	?L356
?L357:
	br	?L358
?L359:
	ldc.i4.1
	stloc.0
?L358:
	ldloc.s	12
	ldloc.s	25
	call	bool [.library]System.Decimal::op_GreaterThanOrEqual(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brtrue	?L359
?L360:
	br	?L361
?L362:
	ldc.i4.1
	stloc.0
?L361:
	ldloc.s	13
	ldloc.s	26
	beq	?L362
?L363:
	br	?L364
?L365:
	ldc.i4.1
	stloc.0
?L364:
	ldloc.s	13
	ldloc.s	26
	bne.un	?L365
?L366:
	ldloc.1
	ldloc.s	14
	bne.un	?L367
	ldc.i4.1
	stloc.0
?L367:
	ldloc.1
	ldloc.s	15
	bne.un	?L368
	ldc.i4.1
	stloc.0
?L368:
	ldloc.1
	ldloc.s	16
	bne.un	?L369
	ldc.i4.1
	stloc.0
?L369:
	ldloc.1
	ldloc.s	17
	bne.un	?L370
	ldc.i4.1
	stloc.0
?L370:
	ldloc.1
	ldloc.s	18
	bne.un	?L371
	ldc.i4.1
	stloc.0
?L371:
	ldloc.1
	conv.i8
	ldloc.s	19
	conv.u8
	bne.un	?L372
	ldc.i4.1
	stloc.0
?L372:
	ldloc.1
	conv.i8
	ldloc.s	20
	bne.un	?L373
	ldc.i4.1
	stloc.0
?L373:
	ldloc.1
	ldloc.s	22
	bne.un	?L374
	ldc.i4.1
	stloc.0
?L374:
	ldloc.1
	conv.r4
	ldloc.s	23
	bne.un	?L375
	ldc.i4.1
	stloc.0
?L375:
	ldloc.1
	conv.r8
	ldloc.s	24
	bne.un	?L376
	ldc.i4.1
	stloc.0
?L376:
	ldloc.1
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int8)
	ldloc.s	25
	call	bool [.library]System.Decimal::op_Equality(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brfalse	?L377
	ldc.i4.1
	stloc.0
?L377:
	ldloc.2
	ldloc.s	14
	beq	?L378
	ldc.i4.1
	stloc.0
?L378:
	ldloc.2
	ldloc.s	15
	beq	?L379
	ldc.i4.1
	stloc.0
?L379:
	ldloc.2
	ldloc.s	16
	beq	?L380
	ldc.i4.1
	stloc.0
?L380:
	ldloc.2
	ldloc.s	17
	beq	?L381
	ldc.i4.1
	stloc.0
?L381:
	ldloc.2
	ldloc.s	18
	beq	?L382
	ldc.i4.1
	stloc.0
?L382:
	ldloc.2
	ldloc.s	19
	beq	?L383
	ldc.i4.1
	stloc.0
?L383:
	ldloc.2
	conv.i8
	ldloc.s	20
	beq	?L384
	ldc.i4.1
	stloc.0
?L384:
	ldloc.2
	conv.u8
	ldloc.s	21
	beq	?L385
	ldc.i4.1
	stloc.0
?L385:
	ldloc.2
	ldloc.s	22
	beq	?L386
	ldc.i4.1
	stloc.0
?L386:
	ldloc.2
	conv.r.un
	ldloc.s	23
	beq	?L387
	ldc.i4.1
	stloc.0
?L387:
	ldloc.2
	conv.r.un
	ldloc.s	24
	beq	?L388
	ldc.i4.1
	stloc.0
?L388:
	ldloc.2
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int8)
	ldloc.s	25
	call	bool [.library]System.Decimal::op_Inequality(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brfalse	?L389
	ldc.i4.1
	stloc.0
?L389:
	ldloc.3
	ldloc.s	14
	bge	?L390
	ldc.i4.1
	stloc.0
?L390:
	ldloc.3
	ldloc.s	15
	bge	?L391
	ldc.i4.1
	stloc.0
?L391:
	ldloc.3
	ldloc.s	16
	bge	?L392
	ldc.i4.1
	stloc.0
?L392:
	ldloc.3
	ldloc.s	17
	bge	?L393
	ldc.i4.1
	stloc.0
?L393:
	ldloc.3
	ldloc.s	18
	bge	?L394
	ldc.i4.1
	stloc.0
?L394:
	ldloc.3
	conv.i8
	ldloc.s	19
	conv.u8
	bge	?L395
	ldc.i4.1
	stloc.0
?L395:
	ldloc.3
	conv.i8
	ldloc.s	20
	bge	?L396
	ldc.i4.1
	stloc.0
?L396:
	ldloc.3
	ldloc.s	22
	bge	?L397
	ldc.i4.1
	stloc.0
?L397:
	ldloc.3
	conv.r4
	ldloc.s	23
	bge.un	?L398
	ldc.i4.1
	stloc.0
?L398:
	ldloc.3
	conv.r8
	ldloc.s	24
	bge.un	?L399
	ldc.i4.1
	stloc.0
?L399:
	ldloc.3
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int16)
	ldloc.s	25
	call	bool [.library]System.Decimal::op_LessThan(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brfalse	?L400
	ldc.i4.1
	stloc.0
?L400:
	ldloc.s	4
	ldloc.s	14
	bgt	?L401
	ldc.i4.1
	stloc.0
?L401:
	ldloc.s	4
	ldloc.s	15
	bgt	?L402
	ldc.i4.1
	stloc.0
?L402:
	ldloc.s	4
	ldloc.s	16
	bgt	?L403
	ldc.i4.1
	stloc.0
?L403:
	ldloc.s	4
	ldloc.s	17
	bgt	?L404
	ldc.i4.1
	stloc.0
?L404:
	ldloc.s	4
	ldloc.s	18
	bgt	?L405
	ldc.i4.1
	stloc.0
?L405:
	ldloc.s	4
	ldloc.s	19
	bgt.un	?L406
	ldc.i4.1
	stloc.0
?L406:
	ldloc.s	4
	conv.i8
	ldloc.s	20
	bgt	?L407
	ldc.i4.1
	stloc.0
?L407:
	ldloc.s	4
	conv.u8
	ldloc.s	21
	bgt.un	?L408
	ldc.i4.1
	stloc.0
?L408:
	ldloc.s	4
	ldloc.s	22
	bgt	?L409
	ldc.i4.1
	stloc.0
?L409:
	ldloc.s	4
	conv.r.un
	ldloc.s	23
	bgt.un	?L410
	ldc.i4.1
	stloc.0
?L410:
	ldloc.s	4
	conv.r.un
	ldloc.s	24
	bgt.un	?L411
	ldc.i4.1
	stloc.0
?L411:
	ldloc.s	4
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int16)
	ldloc.s	25
	call	bool [.library]System.Decimal::op_LessThanOrEqual(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brfalse	?L412
	ldc.i4.1
	stloc.0
?L412:
	ldloc.s	5
	ldloc.s	14
	ble	?L413
	ldc.i4.1
	stloc.0
?L413:
	ldloc.s	5
	ldloc.s	15
	ble	?L414
	ldc.i4.1
	stloc.0
?L414:
	ldloc.s	5
	ldloc.s	16
	ble	?L415
	ldc.i4.1
	stloc.0
?L415:
	ldloc.s	5
	ldloc.s	17
	ble	?L416
	ldc.i4.1
	stloc.0
?L416:
	ldloc.s	5
	ldloc.s	18
	ble	?L417
	ldc.i4.1
	stloc.0
?L417:
	ldloc.s	5
	conv.i8
	ldloc.s	19
	conv.u8
	ble	?L418
	ldc.i4.1
	stloc.0
?L418:
	ldloc.s	5
	conv.i8
	ldloc.s	20
	ble	?L419
	ldc.i4.1
	stloc.0
?L419:
	ldloc.s	5
	ldloc.s	22
	ble	?L420
	ldc.i4.1
	stloc.0
?L420:
	ldloc.s	5
	conv.r4
	ldloc.s	23
	ble.un	?L421
	ldc.i4.1
	stloc.0
?L421:
	ldloc.s	5
	conv.r8
	ldloc.s	24
	ble.un	?L422
	ldc.i4.1
	stloc.0
?L422:
	ldloc.s	5
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	ldloc.s	25
	call	bool [.library]System.Decimal::op_GreaterThan(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brfalse	?L423
	ldc.i4.1
	stloc.0
?L423:
	ldloc.s	6
	conv.u8
	ldloc.s	14
	conv.i8
	blt	?L424
	ldc.i4.1
	stloc.0
?L424:
	ldloc.s	6
	ldloc.s	15
	blt.un	?L425
	ldc.i4.1
	stloc.0
?L425:
	ldloc.s	6
	conv.u8
	ldloc.s	16
	conv.i8
	blt	?L426
	ldc.i4.1
	stloc.0
?L426:
	ldloc.s	6
	ldloc.s	17
	blt.un	?L427
	ldc.i4.1
	stloc.0
?L427:
	ldloc.s	6
	conv.u8
	ldloc.s	18
	conv.i8
	blt	?L428
	ldc.i4.1
	stloc.0
?L428:
	ldloc.s	6
	ldloc.s	19
	blt.un	?L429
	ldc.i4.1
	stloc.0
?L429:
	ldloc.s	6
	conv.u8
	ldloc.s	20
	blt	?L430
	ldc.i4.1
	stloc.0
?L430:
	ldloc.s	6
	conv.u8
	ldloc.s	21
	blt.un	?L431
	ldc.i4.1
	stloc.0
?L431:
	ldloc.s	6
	ldloc.s	22
	blt.un	?L432
	ldc.i4.1
	stloc.0
?L432:
	ldloc.s	6
	conv.r.un
	conv.r4
	ldloc.s	23
	blt.un	?L433
	ldc.i4.1
	stloc.0
?L433:
	ldloc.s	6
	conv.r.un
	ldloc.s	24
	blt.un	?L434
	ldc.i4.1
	stloc.0
?L434:
	ldloc.s	6
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int32)
	ldloc.s	25
	call	bool [.library]System.Decimal::op_GreaterThanOrEqual(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brfalse	?L435
	ldc.i4.1
	stloc.0
?L435:
	ldloc.s	7
	ldloc.s	14
	conv.i8
	bne.un	?L436
	ldc.i4.1
	stloc.0
?L436:
	ldloc.s	7
	ldloc.s	15
	conv.i8
	bne.un	?L437
	ldc.i4.1
	stloc.0
?L437:
	ldloc.s	7
	ldloc.s	16
	conv.i8
	bne.un	?L438
	ldc.i4.1
	stloc.0
?L438:
	ldloc.s	7
	ldloc.s	17
	conv.i8
	bne.un	?L439
	ldc.i4.1
	stloc.0
?L439:
	ldloc.s	7
	ldloc.s	18
	conv.i8
	bne.un	?L440
	ldc.i4.1
	stloc.0
?L440:
	ldloc.s	7
	ldloc.s	19
	conv.u8
	bne.un	?L441
	ldc.i4.1
	stloc.0
?L441:
	ldloc.s	7
	ldloc.s	20
	bne.un	?L442
	ldc.i4.1
	stloc.0
?L442:
	ldloc.s	7
	ldloc.s	22
	conv.i8
	bne.un	?L443
	ldc.i4.1
	stloc.0
?L443:
	ldloc.s	7
	conv.r4
	ldloc.s	23
	bne.un	?L444
	ldc.i4.1
	stloc.0
?L444:
	ldloc.s	7
	conv.r8
	ldloc.s	24
	bne.un	?L445
	ldc.i4.1
	stloc.0
?L445:
	ldloc.s	7
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int64)
	ldloc.s	25
	call	bool [.library]System.Decimal::op_Equality(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brfalse	?L446
	ldc.i4.1
	stloc.0
?L446:
	ldloc.s	8
	ldloc.s	15
	conv.u8
	beq	?L447
	ldc.i4.1
	stloc.0
?L447:
	ldloc.s	8
	ldloc.s	17
	conv.u8
	beq	?L448
	ldc.i4.1
	stloc.0
?L448:
	ldloc.s	8
	ldloc.s	19
	conv.u8
	beq	?L449
	ldc.i4.1
	stloc.0
?L449:
	ldloc.s	8
	ldloc.s	21
	beq	?L450
	ldc.i4.1
	stloc.0
?L450:
	ldloc.s	8
	ldloc.s	22
	conv.u8
	beq	?L451
	ldc.i4.1
	stloc.0
?L451:
	ldloc.s	8
	conv.r.un
	conv.r4
	ldloc.s	23
	beq	?L452
	ldc.i4.1
	stloc.0
?L452:
	ldloc.s	8
	conv.r.un
	conv.r8
	ldloc.s	24
	beq	?L453
	ldc.i4.1
	stloc.0
?L453:
	ldloc.s	8
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int64)
	ldloc.s	25
	call	bool [.library]System.Decimal::op_Inequality(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brfalse	?L454
	ldc.i4.1
	stloc.0
?L454:
	ldloc.s	10
	ldloc.s	14
	conv.r4
	bge.un	?L455
	ldc.i4.1
	stloc.0
?L455:
	ldloc.s	10
	ldloc.s	15
	conv.r.un
	bge.un	?L456
	ldc.i4.1
	stloc.0
?L456:
	ldloc.s	10
	ldloc.s	16
	conv.r4
	bge.un	?L457
	ldc.i4.1
	stloc.0
?L457:
	ldloc.s	10
	ldloc.s	17
	conv.r.un
	bge.un	?L458
	ldc.i4.1
	stloc.0
?L458:
	ldloc.s	10
	ldloc.s	18
	conv.r4
	bge.un	?L459
	ldc.i4.1
	stloc.0
?L459:
	ldloc.s	10
	ldloc.s	19
	conv.r.un
	conv.r4
	bge.un	?L460
	ldc.i4.1
	stloc.0
?L460:
	ldloc.s	10
	ldloc.s	20
	conv.r4
	bge.un	?L461
	ldc.i4.1
	stloc.0
?L461:
	ldloc.s	10
	ldloc.s	21
	conv.r.un
	conv.r4
	bge.un	?L462
	ldc.i4.1
	stloc.0
?L462:
	ldloc.s	10
	ldloc.s	22
	conv.r.un
	bge.un	?L463
	ldc.i4.1
	stloc.0
?L463:
	ldloc.s	10
	ldloc.s	23
	bge.un	?L464
	ldc.i4.1
	stloc.0
?L464:
	ldloc.s	10
	ldloc.s	24
	bge.un	?L465
	ldc.i4.1
	stloc.0
?L465:
	ldloc.s	11
	ldloc.s	14
	conv.r8
	bgt.un	?L466
	ldc.i4.1
	stloc.0
?L466:
	ldloc.s	11
	ldloc.s	15
	conv.r.un
	bgt.un	?L467
	ldc.i4.1
	stloc.0
?L467:
	ldloc.s	11
	ldloc.s	16
	conv.r8
	bgt.un	?L468
	ldc.i4.1
	stloc.0
?L468:
	ldloc.s	11
	ldloc.s	17
	conv.r.un
	bgt.un	?L469
	ldc.i4.1
	stloc.0
?L469:
	ldloc.s	11
	ldloc.s	18
	conv.r8
	bgt.un	?L470
	ldc.i4.1
	stloc.0
?L470:
	ldloc.s	11
	ldloc.s	19
	conv.r.un
	bgt.un	?L471
	ldc.i4.1
	stloc.0
?L471:
	ldloc.s	11
	ldloc.s	20
	conv.r8
	bgt.un	?L472
	ldc.i4.1
	stloc.0
?L472:
	ldloc.s	11
	ldloc.s	21
	conv.r.un
	conv.r8
	bgt.un	?L473
	ldc.i4.1
	stloc.0
?L473:
	ldloc.s	11
	ldloc.s	22
	conv.r.un
	bgt.un	?L474
	ldc.i4.1
	stloc.0
?L474:
	ldloc.s	11
	ldloc.s	23
	bgt.un	?L475
	ldc.i4.1
	stloc.0
?L475:
	ldloc.s	11
	ldloc.s	24
	bgt.un	?L476
	ldc.i4.1
	stloc.0
?L476:
	ldloc.s	12
	ldloc.s	14
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int8)
	call	bool [.library]System.Decimal::op_GreaterThan(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brfalse	?L477
	ldc.i4.1
	stloc.0
?L477:
	ldloc.s	12
	ldloc.s	15
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int8)
	call	bool [.library]System.Decimal::op_GreaterThanOrEqual(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brfalse	?L478
	ldc.i4.1
	stloc.0
?L478:
	ldloc.s	12
	ldloc.s	16
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int16)
	call	bool [.library]System.Decimal::op_GreaterThan(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brfalse	?L479
	ldc.i4.1
	stloc.0
?L479:
	ldloc.s	12
	ldloc.s	17
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int16)
	call	bool [.library]System.Decimal::op_GreaterThanOrEqual(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brfalse	?L480
	ldc.i4.1
	stloc.0
?L480:
	ldloc.s	12
	ldloc.s	18
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	call	bool [.library]System.Decimal::op_GreaterThan(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brfalse	?L481
	ldc.i4.1
	stloc.0
?L481:
	ldloc.s	12
	ldloc.s	19
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int32)
	call	bool [.library]System.Decimal::op_GreaterThanOrEqual(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brfalse	?L482
	ldc.i4.1
	stloc.0
?L482:
	ldloc.s	12
	ldloc.s	20
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int64)
	call	bool [.library]System.Decimal::op_GreaterThan(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brfalse	?L483
	ldc.i4.1
	stloc.0
?L483:
	ldloc.s	12
	ldloc.s	21
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int64)
	call	bool [.library]System.Decimal::op_GreaterThanOrEqual(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brfalse	?L484
	ldc.i4.1
	stloc.0
?L484:
	ldloc.s	12
	ldloc.s	22
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(char)
	call	bool [.library]System.Decimal::op_GreaterThan(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brfalse	?L485
	ldc.i4.1
	stloc.0
?L485:
	ldloc.s	12
	ldloc.s	25
	call	bool [.library]System.Decimal::op_GreaterThanOrEqual(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	brfalse	?L486
	ldc.i4.1
	stloc.0
?L486:
	ldloc.s	13
	ldloc.s	26
	bne.un	?L487
	ldc.i4.1
	stloc.0
?L487:
	ldloc.s	13
	ldloc.s	26
	beq	?L488
	ldc.i4.1
	stloc.0
?L488:
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
