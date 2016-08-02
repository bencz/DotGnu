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
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.field public static literal valuetype 'Color' 'red' = int32(0x00000000)
.method private hidebysig instance void 'm1'() cil managed 
{
	.locals init	(valuetype 'Color', valuetype 'ColorSmall', int8, unsigned int8, int16, unsigned int16, int32, unsigned int32, int64, unsigned int64, char, float32, float64, valuetype ['.library']'System'.'Decimal')
	ldc.i4.3
	conv.i1
	stloc.2
	ldc.i4.3
	conv.u1
	stloc.3
	ldc.i4.3
	conv.i2
	stloc.s	4
	ldc.i4.3
	conv.u2
	stloc.s	5
	ldc.i4.3
	stloc.s	6
	ldc.i4.3
	stloc.s	7
	ldc.i4.3
	conv.i8
	stloc.s	8
	ldc.i4.3
	conv.u8
	stloc.s	9
	ldc.i4.s	51
	stloc.s	10
	ldc.r4	float32(0x40400000)
	stloc.s	11
	ldc.r4	float32(0x40400000)
	stloc.s	12
	ldc.i4.3
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	stloc.s	13
	ldc.i4.0
	stloc.0
	ldc.i4.0
	conv.i1
	stloc.1
	ldloc.2
	stloc.0
	ldloc.3
	stloc.0
	ldloc.s	4
	stloc.0
	ldloc.s	5
	stloc.0
	ldloc.s	6
	stloc.0
	ldloc.s	7
	conv.ovf.i4.un
	stloc.0
	ldloc.s	8
	conv.ovf.i4
	stloc.0
	ldloc.s	9
	conv.ovf.i4.un
	stloc.0
	ldloc.s	10
	stloc.0
	ldloc.s	11
	conv.ovf.i4
	stloc.0
	ldloc.s	12
	conv.ovf.i4
	stloc.0
	ldloc.s	13
	call	int32 [.library]System.Decimal::op_Explicit(valuetype [.library]System.Decimal)
	stloc.0
	ldloc.2
	stloc.1
	ldloc.3
	conv.ovf.i1.un
	stloc.1
	ldloc.s	4
	conv.ovf.i1
	stloc.1
	ldloc.s	5
	conv.ovf.i1.un
	stloc.1
	ldloc.s	6
	conv.ovf.i1
	stloc.1
	ldloc.s	7
	conv.ovf.i1.un
	stloc.1
	ldloc.s	8
	conv.ovf.i1
	stloc.1
	ldloc.s	9
	conv.ovf.i1.un
	stloc.1
	ldloc.s	10
	conv.ovf.i1.un
	stloc.1
	ldloc.s	11
	conv.ovf.i1
	stloc.1
	ldloc.s	12
	conv.ovf.i1
	stloc.1
	ldloc.s	13
	call	int8 [.library]System.Decimal::op_Explicit(valuetype [.library]System.Decimal)
	stloc.1
	ldloc.2
	stloc.0
	ldloc.3
	stloc.0
	ldloc.s	4
	stloc.0
	ldloc.s	5
	stloc.0
	ldloc.s	6
	stloc.0
	ldloc.s	7
	stloc.0
	ldloc.s	8
	conv.i4
	stloc.0
	ldloc.s	9
	conv.i4
	stloc.0
	ldloc.s	10
	stloc.0
	ldloc.s	11
	conv.i4
	stloc.0
	ldloc.s	12
	conv.i4
	stloc.0
	ldloc.s	13
	call	int32 [.library]System.Decimal::op_Explicit(valuetype [.library]System.Decimal)
	stloc.0
	ldloc.2
	stloc.1
	ldloc.3
	conv.i1
	stloc.1
	ldloc.s	4
	conv.i1
	stloc.1
	ldloc.s	5
	conv.i1
	stloc.1
	ldloc.s	6
	conv.i1
	stloc.1
	ldloc.s	7
	conv.i1
	stloc.1
	ldloc.s	8
	conv.i1
	stloc.1
	ldloc.s	9
	conv.i1
	stloc.1
	ldloc.s	10
	conv.i1
	stloc.1
	ldloc.s	11
	conv.i1
	stloc.1
	ldloc.s	12
	conv.i1
	stloc.1
	ldloc.s	13
	call	int8 [.library]System.Decimal::op_Explicit(valuetype [.library]System.Decimal)
	stloc.1
	ldloc.0
	conv.ovf.i1
	stloc.2
	ldloc.0
	conv.ovf.u1
	stloc.3
	ldloc.0
	conv.ovf.i2
	stloc.s	4
	ldloc.0
	conv.ovf.u2
	stloc.s	5
	ldloc.0
	stloc.s	6
	ldloc.0
	conv.ovf.u4
	stloc.s	7
	ldloc.0
	conv.i8
	stloc.s	8
	ldloc.0
	conv.ovf.u8
	stloc.s	9
	ldloc.0
	conv.ovf.u2
	stloc.s	10
	ldloc.0
	conv.r4
	stloc.s	11
	ldloc.0
	conv.r8
	stloc.s	12
	ldloc.0
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	stloc.s	13
	ldloc.1
	stloc.2
	ldloc.1
	conv.ovf.u1
	stloc.3
	ldloc.1
	stloc.s	4
	ldloc.1
	conv.ovf.u2
	stloc.s	5
	ldloc.1
	stloc.s	6
	ldloc.1
	conv.ovf.u4
	stloc.s	7
	ldloc.1
	conv.i8
	stloc.s	8
	ldloc.1
	conv.ovf.u8
	stloc.s	9
	ldloc.1
	conv.ovf.u2
	stloc.s	10
	ldloc.1
	conv.r4
	stloc.s	11
	ldloc.1
	conv.r8
	stloc.s	12
	ldloc.1
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int8)
	stloc.s	13
	ldloc.0
	conv.i1
	stloc.2
	ldloc.0
	conv.u1
	stloc.3
	ldloc.0
	conv.i2
	stloc.s	4
	ldloc.0
	conv.u2
	stloc.s	5
	ldloc.0
	stloc.s	6
	ldloc.0
	stloc.s	7
	ldloc.0
	conv.i8
	stloc.s	8
	ldloc.0
	conv.u8
	stloc.s	9
	ldloc.0
	conv.u2
	stloc.s	10
	ldloc.0
	conv.r4
	stloc.s	11
	ldloc.0
	conv.r8
	stloc.s	12
	ldloc.0
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	stloc.s	13
	ldloc.1
	stloc.2
	ldloc.1
	conv.u1
	stloc.3
	ldloc.1
	stloc.s	4
	ldloc.1
	conv.u2
	stloc.s	5
	ldloc.1
	stloc.s	6
	ldloc.1
	stloc.s	7
	ldloc.1
	conv.i8
	stloc.s	8
	ldloc.1
	conv.u8
	stloc.s	9
	ldloc.1
	conv.u2
	stloc.s	10
	ldloc.1
	conv.r4
	stloc.s	11
	ldloc.1
	conv.r8
	stloc.s	12
	ldloc.1
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int8)
	stloc.s	13
	ldloc.1
	stloc.0
	ldloc.0
	conv.ovf.i1
	stloc.1
	ldloc.1
	stloc.0
	ldloc.0
	conv.i1
	stloc.1
	ret
	.maxstack 1
} // method m1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
