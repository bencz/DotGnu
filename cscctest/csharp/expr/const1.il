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
	.locals init	(class ['.library']'System'.'Object', bool, int32, unsigned int32, int64, unsigned int64, float32, float64, valuetype ['.library']'System'.'Decimal', class ['.library']'System'.'String')
	ldnull
	stloc.0
	ldc.i4.1
	stloc.1
	ldc.i4.0
	stloc.1
	ldc.i4.s	-128
	stloc.2
	ldc.i4.s	-2
	stloc.2
	ldc.i4.m1
	stloc.2
	ldc.i4.0
	stloc.2
	ldc.i4.1
	stloc.2
	ldc.i4.2
	stloc.2
	ldc.i4.3
	stloc.2
	ldc.i4.4
	stloc.2
	ldc.i4.5
	stloc.2
	ldc.i4.6
	stloc.2
	ldc.i4.7
	stloc.2
	ldc.i4.8
	stloc.2
	ldc.i4.s	9
	stloc.2
	ldc.i4.s	127
	stloc.2
	ldc.i4	2147483648
	stloc.2
	ldc.i4	4294967167
	stloc.2
	ldc.i4	128
	stloc.2
	ldc.i4	2147483647
	stloc.2
	ldc.i4.0
	stloc.3
	ldc.i4.1
	stloc.3
	ldc.i4.2
	stloc.3
	ldc.i4.3
	stloc.3
	ldc.i4.4
	stloc.3
	ldc.i4.5
	stloc.3
	ldc.i4.6
	stloc.3
	ldc.i4.7
	stloc.3
	ldc.i4.8
	stloc.3
	ldc.i4.s	9
	stloc.3
	ldc.i4.s	127
	stloc.3
	ldc.i4	128
	stloc.3
	ldc.i4	2147483647
	stloc.3
	ldc.i4	2147483648
	stloc.3
	ldc.i4.m1
	stloc.3
	ldc.i4.s	-128
	conv.i8
	stloc.s	4
	ldc.i4.s	-2
	conv.i8
	stloc.s	4
	ldc.i4.m1
	conv.i8
	stloc.s	4
	ldc.i4.0
	conv.i8
	stloc.s	4
	ldc.i4.1
	conv.i8
	stloc.s	4
	ldc.i4.2
	conv.i8
	stloc.s	4
	ldc.i4.3
	conv.i8
	stloc.s	4
	ldc.i4.4
	conv.i8
	stloc.s	4
	ldc.i4.5
	conv.i8
	stloc.s	4
	ldc.i4.6
	conv.i8
	stloc.s	4
	ldc.i4.7
	conv.i8
	stloc.s	4
	ldc.i4.8
	conv.i8
	stloc.s	4
	ldc.i4.s	9
	conv.i8
	stloc.s	4
	ldc.i4.s	127
	conv.i8
	stloc.s	4
	ldc.i4	2147483648
	conv.i8
	stloc.s	4
	ldc.i4	4294967167
	conv.i8
	stloc.s	4
	ldc.i4	128
	conv.i8
	stloc.s	4
	ldc.i4	2147483647
	conv.i8
	stloc.s	4
	ldc.i4	2147483648
	conv.u8
	stloc.s	4
	ldc.i4.m1
	conv.u8
	stloc.s	4
	ldc.i8	0x0000000100000000
	stloc.s	4
	ldc.i8	0x7fffffffffffffff
	stloc.s	4
	ldc.i8	0x8000000000000000
	stloc.s	4
	ldc.i4.0
	conv.u8
	stloc.s	5
	ldc.i4.1
	conv.u8
	stloc.s	5
	ldc.i4.2
	conv.u8
	stloc.s	5
	ldc.i4.3
	conv.u8
	stloc.s	5
	ldc.i4.4
	conv.u8
	stloc.s	5
	ldc.i4.5
	conv.u8
	stloc.s	5
	ldc.i4.6
	conv.u8
	stloc.s	5
	ldc.i4.7
	conv.u8
	stloc.s	5
	ldc.i4.8
	conv.u8
	stloc.s	5
	ldc.i4.s	9
	conv.u8
	stloc.s	5
	ldc.i4.s	127
	conv.u8
	stloc.s	5
	ldc.i4	128
	conv.u8
	stloc.s	5
	ldc.i4	2147483647
	conv.u8
	stloc.s	5
	ldc.i4	2147483648
	conv.u8
	stloc.s	5
	ldc.i4.m1
	conv.u8
	stloc.s	5
	ldc.i8	0x0000000100000000
	stloc.s	5
	ldc.i8	0x7fffffffffffffff
	stloc.s	5
	ldc.i8	0x8000000000000000
	stloc.s	5
	ldc.i4.s	-128
	conv.i8
	stloc.s	5
	ldc.i4.m1
	conv.i8
	stloc.s	5
	ldc.r4	float32(0x00000000)
	stloc.s	6
	ldc.r4	float32(0x3F800000)
	stloc.s	6
	ldc.r4	float32(0xBF800000)
	stloc.s	6
	ldc.r4	float32(0x78181A2D)
	stloc.s	6
	ldc.r4	float32(0x7F7FFFFF)
	stloc.s	6
	ldc.r4	float32(0xFF7FFFFF)
	stloc.s	6
	ldc.r4	float32(0x00000001)
	stloc.s	6
	ldc.i4.s	-128
	conv.r4
	stloc.s	6
	ldc.i4.m1
	conv.r4
	stloc.s	6
	ldc.i4.0
	conv.r4
	stloc.s	6
	ldc.i4.1
	conv.r4
	stloc.s	6
	ldc.i4.s	127
	conv.r4
	stloc.s	6
	ldc.i4	2147483648
	conv.r4
	stloc.s	6
	ldc.i4	2147483647
	conv.r4
	stloc.s	6
	ldc.i4.m1
	conv.r.un
	conv.r4
	stloc.s	6
	ldc.i8	0x0000000100000000
	conv.r4
	stloc.s	6
	ldc.i8	0x7fffffffffffffff
	conv.r4
	stloc.s	6
	ldc.i8	0x8000000000000000
	conv.r4
	stloc.s	6
	ldc.i8	0x8000000000000000
	conv.r.un
	conv.r4
	stloc.s	6
	ldc.i4.s	-128
	conv.i8
	conv.r.un
	conv.r4
	stloc.s	6
	ldc.i4.m1
	conv.i8
	conv.r.un
	conv.r4
	stloc.s	6
	ldc.r4	float32(0x00000000)
	stloc.s	7
	ldc.r4	float32(0x3F800000)
	stloc.s	7
	ldc.r4	float32(0xBF800000)
	stloc.s	7
	ldc.r4	float32(0x78181A2D)
	stloc.s	7
	ldc.r4	float32(0x7F7FFFFF)
	stloc.s	7
	ldc.r4	float32(0xFF7FFFFF)
	stloc.s	7
	ldc.r4	float32(0x00000001)
	stloc.s	7
	ldc.r4	float32(0x00000000)
	stloc.s	7
	ldc.r4	float32(0x3F800000)
	stloc.s	7
	ldc.r4	float32(0xBF800000)
	stloc.s	7
	ldc.r8	float64(0x4703034593B73C22)
	stloc.s	7
	ldc.r8	float64(0x7FEFFFFFFFFFFFFF)
	stloc.s	7
	ldc.r8	float64(0xFFEFFFFFFFFFFFFF)
	stloc.s	7
	ldc.r8	float64(0x0000000000000001)
	stloc.s	7
	ldc.r4	float32(0x00000000)
	stloc.s	7
	ldc.r4	float32(0x3F800000)
	stloc.s	7
	ldc.r4	float32(0xBF800000)
	stloc.s	7
	ldc.r8	float64(0x4703034593B73C22)
	stloc.s	7
	ldc.r8	float64(0x7FEFFFFFFFFFFFFF)
	stloc.s	7
	ldc.r8	float64(0xFFEFFFFFFFFFFFFF)
	stloc.s	7
	ldc.r8	float64(0x0000000000000001)
	stloc.s	7
	ldc.i4.s	-128
	conv.r8
	stloc.s	7
	ldc.i4.m1
	conv.r8
	stloc.s	7
	ldc.i4.0
	conv.r8
	stloc.s	7
	ldc.i4.1
	conv.r8
	stloc.s	7
	ldc.i4.s	127
	conv.r8
	stloc.s	7
	ldc.i4	2147483648
	conv.r8
	stloc.s	7
	ldc.i4	2147483647
	conv.r8
	stloc.s	7
	ldc.i4.m1
	conv.r.un
	stloc.s	7
	ldc.i8	0x0000000100000000
	conv.r8
	stloc.s	7
	ldc.i8	0x7fffffffffffffff
	conv.r8
	stloc.s	7
	ldc.i8	0x8000000000000000
	conv.r8
	stloc.s	7
	ldc.i8	0x8000000000000000
	conv.r.un
	conv.r8
	stloc.s	7
	ldc.i4.s	-128
	conv.i8
	conv.r.un
	conv.r8
	stloc.s	7
	ldc.i4.m1
	conv.i8
	conv.r.un
	conv.r8
	stloc.s	7
	ldc.i4.0
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	stloc.s	8
	ldc.i4.1
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	stloc.s	8
	ldc.i4.m1
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	stloc.s	8
	ldc.i4	2147483647
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	stloc.s	8
	ldc.i4	2147483648
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	stloc.s	8
	ldc.i4	2147483648
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int32)
	stloc.s	8
	ldc.i4.m1
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(unsigned int32)
	stloc.s	8
	ldc.i8	0x0000000100000000
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int64)
	stloc.s	8
	ldc.i8	0xffffffff00000001
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int64)
	stloc.s	8
	ldc.i8	0x7fffffffffffffff
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int64)
	stloc.s	8
	ldc.i8	0x8000000000000000
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int64)
	stloc.s	8
	ldc.i4.0
	ldc.i4	2147483648
	ldc.i4.0
	ldc.i4.0
	ldc.i4.0
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stloc.s	8
	ldc.i4.m1
	ldc.i4.m1
	ldc.i4.0
	ldc.i4.0
	ldc.i4.0
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stloc.s	8
	ldc.i4.m1
	ldc.i4.m1
	ldc.i4.m1
	ldc.i4.0
	ldc.i4.0
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stloc.s	8
	ldc.i4.m1
	ldc.i4.m1
	ldc.i4.m1
	ldc.i4.0
	ldc.i4.s	28
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stloc.s	8
	ldc.i4.m1
	ldc.i4.m1
	ldc.i4.m1
	ldc.i4.0
	ldc.i4.s	28
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stloc.s	8
	ldc.i4	2576980378
	ldc.i4	2576980377
	ldc.i4	429496729
	ldc.i4.0
	ldc.i4.s	27
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stloc.s	8
	ldc.i4	2550136832
	ldc.i4	1364693707
	ldc.i4	4282598581
	ldc.i4.0
	ldc.i4.0
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stloc.s	8
	ldc.i4	2550136832
	ldc.i4	1364693707
	ldc.i4	4282598581
	ldc.i4.0
	ldc.i4.0
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stloc.s	8
	ldc.i4.1
	ldc.i4.0
	ldc.i4.0
	ldc.i4.0
	ldc.i4.s	28
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stloc.s	8
	ldc.i4.0
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	stloc.s	8
	ldstr	"Hello World!"
	stloc.s	9
	ldstr	""
	stloc.s	9
	ldnull
	stloc.s	9
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
