.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private auto sealed serializable ansi 'Color' extends ['.library']'System'.'Enum'
{
.field public static literal valuetype 'Color' 'Red' = int32(0x00000000)
.field public static literal valuetype 'Color' 'Green' = int32(0x00000001)
.field public static literal valuetype 'Color' 'Blue' = int32(0x00000002)
.field public specialname rtspecialname int32 'value__'
} // class Color
.class private auto sealed serializable ansi 'ColorSmall' extends ['.library']'System'.'Enum'
{
.field public static literal valuetype 'ColorSmall' 'Red' = int8(0x00)
.field public static literal valuetype 'ColorSmall' 'Green' = int8(0x01)
.field public static literal valuetype 'ColorSmall' 'Blue' = int8(0x02)
.field public specialname rtspecialname int8 'value__'
} // class ColorSmall
.class public sequential sealed serializable ansi 'X' extends ['.library']'System'.'ValueType'
{
.size 1
.method public static hidebysig specialname valuetype 'X' 'op_Increment'(valuetype 'X' 'x') cil managed 
{
	ldarg.0
	ret
	.maxstack 1
} // method op_Increment
.method public static hidebysig specialname valuetype 'X' 'op_Decrement'(valuetype 'X' 'x') cil managed 
{
	ldarg.0
	ret
	.maxstack 1
} // method op_Decrement
} // class X
.class public auto ansi 'Y' extends ['.library']'System'.'Object'
{
.method public static hidebysig specialname class 'Y' 'op_Increment'(class 'Y' 'y') cil managed 
{
	ldarg.0
	ret
	.maxstack 1
} // method op_Increment
.method public static hidebysig specialname class 'Y' 'op_Decrement'(class 'Y' 'y') cil managed 
{
	ldarg.0
	ret
	.maxstack 1
} // method op_Decrement
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Y
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private hidebysig instance void 'm1'() cil managed 
{
	.locals init	(int8, unsigned int8, int16, unsigned int16, int32, unsigned int32, int64, unsigned int64, char, float32, float64, valuetype ['.library']'System'.'Decimal', valuetype 'Color', valuetype 'ColorSmall', valuetype 'X', class 'Y', int8, unsigned int8, int16, unsigned int16, int32, unsigned int32, int64, unsigned int64, char, float32, float64, valuetype ['.library']'System'.'Decimal', valuetype 'Color', valuetype 'ColorSmall', valuetype 'X', class 'Y')
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
	stloc.s	12
	ldc.i4.0
	stloc.s	13
	ldnull
	stloc.s	15
	ldloc.0
	ldc.i4.1
	add
	conv.i1
	stloc.0
	ldloc.1
	ldc.i4.1
	add
	conv.u1
	stloc.1
	ldloc.2
	ldc.i4.1
	add
	conv.i2
	stloc.2
	ldloc.3
	ldc.i4.1
	add
	conv.u2
	stloc.3
	ldloc.s	4
	ldc.i4.1
	add
	stloc.s	4
	ldloc.s	5
	ldc.i4.1
	add
	stloc.s	5
	ldloc.s	6
	ldc.i4.1
	conv.i8
	add
	stloc.s	6
	ldloc.s	7
	ldc.i4.1
	conv.u8
	add
	stloc.s	7
	ldloc.s	8
	ldc.i4.1
	add
	conv.u2
	stloc.s	8
	ldloc.s	9
	ldc.r4	float32(0x3F800000)
	add
	conv.r4
	stloc.s	9
	ldloc.s	10
	ldc.r4	float32(0x3F800000)
	add
	conv.r8
	stloc.s	10
	ldloc.s	11
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Increment(valuetype [.library]System.Decimal)
	stloc.s	11
	ldloc.s	12
	ldc.i4.1
	add
	stloc.s	12
	ldloc.s	13
	ldc.i4.1
	add
	conv.i1
	stloc.s	13
	ldloc.s	14
	call	valuetype 'X' 'X'::'op_Increment'(valuetype 'X')
	stloc.s	14
	ldloc.s	15
	call	class 'Y' 'Y'::'op_Increment'(class 'Y')
	stloc.s	15
	ldloc.0
	ldc.i4.1
	sub
	conv.i1
	stloc.0
	ldloc.1
	ldc.i4.1
	sub
	conv.u1
	stloc.1
	ldloc.2
	ldc.i4.1
	sub
	conv.i2
	stloc.2
	ldloc.3
	ldc.i4.1
	sub
	conv.u2
	stloc.3
	ldloc.s	4
	ldc.i4.1
	sub
	stloc.s	4
	ldloc.s	5
	ldc.i4.1
	sub
	stloc.s	5
	ldloc.s	6
	ldc.i4.1
	conv.i8
	sub
	stloc.s	6
	ldloc.s	7
	ldc.i4.1
	conv.u8
	sub
	stloc.s	7
	ldloc.s	8
	ldc.i4.1
	sub
	conv.u2
	stloc.s	8
	ldloc.s	9
	ldc.r4	float32(0x3F800000)
	sub
	conv.r4
	stloc.s	9
	ldloc.s	10
	ldc.r4	float32(0x3F800000)
	sub
	conv.r8
	stloc.s	10
	ldloc.s	11
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Decrement(valuetype System.Decimal)
	stloc.s	11
	ldloc.s	12
	ldc.i4.1
	sub
	stloc.s	12
	ldloc.s	13
	ldc.i4.1
	sub
	conv.i1
	stloc.s	13
	ldloc.s	14
	call	valuetype 'X' 'X'::'op_Decrement'(valuetype 'X')
	stloc.s	14
	ldloc.s	15
	call	class 'Y' 'Y'::'op_Decrement'(class 'Y')
	stloc.s	15
	ldloc.0
	ldc.i4.1
	add
	conv.i1
	stloc.0
	ldloc.1
	ldc.i4.1
	add
	conv.u1
	stloc.1
	ldloc.2
	ldc.i4.1
	add
	conv.i2
	stloc.2
	ldloc.3
	ldc.i4.1
	add
	conv.u2
	stloc.3
	ldloc.s	4
	ldc.i4.1
	add
	stloc.s	4
	ldloc.s	5
	ldc.i4.1
	add
	stloc.s	5
	ldloc.s	6
	ldc.i4.1
	conv.i8
	add
	stloc.s	6
	ldloc.s	7
	ldc.i4.1
	conv.u8
	add
	stloc.s	7
	ldloc.s	8
	ldc.i4.1
	add
	conv.u2
	stloc.s	8
	ldloc.s	9
	ldc.r4	float32(0x3F800000)
	add
	conv.r4
	stloc.s	9
	ldloc.s	10
	ldc.r4	float32(0x3F800000)
	add
	conv.r8
	stloc.s	10
	ldloc.s	11
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Increment(valuetype [.library]System.Decimal)
	stloc.s	11
	ldloc.s	12
	ldc.i4.1
	add
	stloc.s	12
	ldloc.s	13
	ldc.i4.1
	add
	conv.i1
	stloc.s	13
	ldloc.s	14
	call	valuetype 'X' 'X'::'op_Increment'(valuetype 'X')
	stloc.s	14
	ldloc.s	15
	call	class 'Y' 'Y'::'op_Increment'(class 'Y')
	stloc.s	15
	ldloc.0
	ldc.i4.1
	sub
	conv.i1
	stloc.0
	ldloc.1
	ldc.i4.1
	sub
	conv.u1
	stloc.1
	ldloc.2
	ldc.i4.1
	sub
	conv.i2
	stloc.2
	ldloc.3
	ldc.i4.1
	sub
	conv.u2
	stloc.3
	ldloc.s	4
	ldc.i4.1
	sub
	stloc.s	4
	ldloc.s	5
	ldc.i4.1
	sub
	stloc.s	5
	ldloc.s	6
	ldc.i4.1
	conv.i8
	sub
	stloc.s	6
	ldloc.s	7
	ldc.i4.1
	conv.u8
	sub
	stloc.s	7
	ldloc.s	8
	ldc.i4.1
	sub
	conv.u2
	stloc.s	8
	ldloc.s	9
	ldc.r4	float32(0x3F800000)
	sub
	conv.r4
	stloc.s	9
	ldloc.s	10
	ldc.r4	float32(0x3F800000)
	sub
	conv.r8
	stloc.s	10
	ldloc.s	11
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Decrement(valuetype System.Decimal)
	stloc.s	11
	ldloc.s	12
	ldc.i4.1
	sub
	stloc.s	12
	ldloc.s	13
	ldc.i4.1
	sub
	conv.i1
	stloc.s	13
	ldloc.s	14
	call	valuetype 'X' 'X'::'op_Decrement'(valuetype 'X')
	stloc.s	14
	ldloc.s	15
	call	class 'Y' 'Y'::'op_Decrement'(class 'Y')
	stloc.s	15
	ldloc.0
	ldc.i4.1
	add
	conv.i1
	dup
	stloc.0
	stloc.s	16
	ldloc.1
	ldc.i4.1
	add
	conv.u1
	dup
	stloc.1
	stloc.s	17
	ldloc.2
	ldc.i4.1
	add
	conv.i2
	dup
	stloc.2
	stloc.s	18
	ldloc.3
	ldc.i4.1
	add
	conv.u2
	dup
	stloc.3
	stloc.s	19
	ldloc.s	4
	ldc.i4.1
	add
	dup
	stloc.s	4
	stloc.s	20
	ldloc.s	5
	ldc.i4.1
	add
	dup
	stloc.s	5
	stloc.s	21
	ldloc.s	6
	ldc.i4.1
	conv.i8
	add
	dup
	stloc.s	6
	stloc.s	22
	ldloc.s	7
	ldc.i4.1
	conv.u8
	add
	dup
	stloc.s	7
	stloc.s	23
	ldloc.s	8
	ldc.i4.1
	add
	conv.u2
	dup
	stloc.s	8
	stloc.s	24
	ldloc.s	9
	ldc.r4	float32(0x3F800000)
	add
	conv.r4
	dup
	stloc.s	9
	stloc.s	25
	ldloc.s	10
	ldc.r4	float32(0x3F800000)
	add
	conv.r8
	dup
	stloc.s	10
	stloc.s	26
	ldloc.s	11
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Increment(valuetype [.library]System.Decimal)
	dup
	stloc.s	11
	stloc.s	27
	ldloc.s	12
	ldc.i4.1
	add
	dup
	stloc.s	12
	stloc.s	28
	ldloc.s	13
	ldc.i4.1
	add
	conv.i1
	dup
	stloc.s	13
	stloc.s	29
	ldloc.s	14
	call	valuetype 'X' 'X'::'op_Increment'(valuetype 'X')
	dup
	stloc.s	14
	stloc.s	30
	ldloc.s	15
	call	class 'Y' 'Y'::'op_Increment'(class 'Y')
	dup
	stloc.s	15
	stloc.s	31
	ldloc.0
	ldc.i4.1
	sub
	conv.i1
	dup
	stloc.0
	stloc.s	16
	ldloc.1
	ldc.i4.1
	sub
	conv.u1
	dup
	stloc.1
	stloc.s	17
	ldloc.2
	ldc.i4.1
	sub
	conv.i2
	dup
	stloc.2
	stloc.s	18
	ldloc.3
	ldc.i4.1
	sub
	conv.u2
	dup
	stloc.3
	stloc.s	19
	ldloc.s	4
	ldc.i4.1
	sub
	dup
	stloc.s	4
	stloc.s	20
	ldloc.s	5
	ldc.i4.1
	sub
	dup
	stloc.s	5
	stloc.s	21
	ldloc.s	6
	ldc.i4.1
	conv.i8
	sub
	dup
	stloc.s	6
	stloc.s	22
	ldloc.s	7
	ldc.i4.1
	conv.u8
	sub
	dup
	stloc.s	7
	stloc.s	23
	ldloc.s	8
	ldc.i4.1
	sub
	conv.u2
	dup
	stloc.s	8
	stloc.s	24
	ldloc.s	9
	ldc.r4	float32(0x3F800000)
	sub
	conv.r4
	dup
	stloc.s	9
	stloc.s	25
	ldloc.s	10
	ldc.r4	float32(0x3F800000)
	sub
	conv.r8
	dup
	stloc.s	10
	stloc.s	26
	ldloc.s	11
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Decrement(valuetype System.Decimal)
	dup
	stloc.s	11
	stloc.s	27
	ldloc.s	12
	ldc.i4.1
	sub
	dup
	stloc.s	12
	stloc.s	28
	ldloc.s	13
	ldc.i4.1
	sub
	conv.i1
	dup
	stloc.s	13
	stloc.s	29
	ldloc.s	14
	call	valuetype 'X' 'X'::'op_Decrement'(valuetype 'X')
	dup
	stloc.s	14
	stloc.s	30
	ldloc.s	15
	call	class 'Y' 'Y'::'op_Decrement'(class 'Y')
	dup
	stloc.s	15
	stloc.s	31
	ldloc.0
	dup
	ldc.i4.1
	add
	conv.i1
	stloc.0
	stloc.s	16
	ldloc.1
	dup
	ldc.i4.1
	add
	conv.u1
	stloc.1
	stloc.s	17
	ldloc.2
	dup
	ldc.i4.1
	add
	conv.i2
	stloc.2
	stloc.s	18
	ldloc.3
	dup
	ldc.i4.1
	add
	conv.u2
	stloc.3
	stloc.s	19
	ldloc.s	4
	dup
	ldc.i4.1
	add
	stloc.s	4
	stloc.s	20
	ldloc.s	5
	dup
	ldc.i4.1
	add
	stloc.s	5
	stloc.s	21
	ldloc.s	6
	dup
	ldc.i4.1
	conv.i8
	add
	stloc.s	6
	stloc.s	22
	ldloc.s	7
	dup
	ldc.i4.1
	conv.u8
	add
	stloc.s	7
	stloc.s	23
	ldloc.s	8
	dup
	ldc.i4.1
	add
	conv.u2
	stloc.s	8
	stloc.s	24
	ldloc.s	9
	dup
	ldc.r4	float32(0x3F800000)
	add
	conv.r4
	stloc.s	9
	stloc.s	25
	ldloc.s	10
	dup
	ldc.r4	float32(0x3F800000)
	add
	conv.r8
	stloc.s	10
	stloc.s	26
	ldloc.s	11
	dup
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Increment(valuetype [.library]System.Decimal)
	stloc.s	11
	stloc.s	27
	ldloc.s	12
	dup
	ldc.i4.1
	add
	stloc.s	12
	stloc.s	28
	ldloc.s	13
	dup
	ldc.i4.1
	add
	conv.i1
	stloc.s	13
	stloc.s	29
	ldloc.s	14
	dup
	call	valuetype 'X' 'X'::'op_Increment'(valuetype 'X')
	stloc.s	14
	stloc.s	30
	ldloc.s	15
	dup
	call	class 'Y' 'Y'::'op_Increment'(class 'Y')
	stloc.s	15
	stloc.s	31
	ldloc.0
	dup
	ldc.i4.1
	sub
	conv.i1
	stloc.0
	stloc.s	16
	ldloc.1
	dup
	ldc.i4.1
	sub
	conv.u1
	stloc.1
	stloc.s	17
	ldloc.2
	dup
	ldc.i4.1
	sub
	conv.i2
	stloc.2
	stloc.s	18
	ldloc.3
	dup
	ldc.i4.1
	sub
	conv.u2
	stloc.3
	stloc.s	19
	ldloc.s	4
	dup
	ldc.i4.1
	sub
	stloc.s	4
	stloc.s	20
	ldloc.s	5
	dup
	ldc.i4.1
	sub
	stloc.s	5
	stloc.s	21
	ldloc.s	6
	dup
	ldc.i4.1
	conv.i8
	sub
	stloc.s	6
	stloc.s	22
	ldloc.s	7
	dup
	ldc.i4.1
	conv.u8
	sub
	stloc.s	7
	stloc.s	23
	ldloc.s	8
	dup
	ldc.i4.1
	sub
	conv.u2
	stloc.s	8
	stloc.s	24
	ldloc.s	9
	dup
	ldc.r4	float32(0x3F800000)
	sub
	conv.r4
	stloc.s	9
	stloc.s	25
	ldloc.s	10
	dup
	ldc.r4	float32(0x3F800000)
	sub
	conv.r8
	stloc.s	10
	stloc.s	26
	ldloc.s	11
	dup
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Decrement(valuetype System.Decimal)
	stloc.s	11
	stloc.s	27
	ldloc.s	12
	dup
	ldc.i4.1
	sub
	stloc.s	12
	stloc.s	28
	ldloc.s	13
	dup
	ldc.i4.1
	sub
	conv.i1
	stloc.s	13
	stloc.s	29
	ldloc.s	14
	dup
	call	valuetype 'X' 'X'::'op_Decrement'(valuetype 'X')
	stloc.s	14
	stloc.s	30
	ldloc.s	15
	dup
	call	class 'Y' 'Y'::'op_Decrement'(class 'Y')
	stloc.s	15
	stloc.s	31
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
