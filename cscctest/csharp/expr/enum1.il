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
.class private auto sealed serializable ansi 'Size' extends ['.library']'System'.'Enum'
{
.field public static literal valuetype 'Size' 'Small' = int64(0x0000000000000000)
.field public static literal valuetype 'Size' 'Large' = int64(0x0000000000000001)
.field public specialname rtspecialname int64 'value__'
} // class Size
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private hidebysig instance void 'm1'() cil managed 
{
	.locals init	(valuetype 'Color', valuetype 'Color', valuetype 'Color', valuetype 'Size', valuetype 'Size', valuetype 'Size', int32, int64, bool)
	ldc.i4.0
	stloc.0
	ldc.i4.1
	stloc.1
	ldc.i4.0
	conv.i8
	stloc.3
	ldc.i4.1
	conv.i8
	stloc.s	4
	ldloc.0
	not
	stloc.2
	ldloc.3
	not
	stloc.s	5
	ldloc.0
	ldc.i4.1
	add
	stloc.2
	ldc.i4.1
	ldloc.1
	add
	stloc.2
	ldloc.3
	ldc.i4.1
	conv.i8
	add
	stloc.s	5
	ldc.i4.1
	conv.i8
	ldloc.s	4
	add
	stloc.s	5
	ldloc.0
	ldc.i4.1
	sub
	stloc.2
	ldloc.0
	ldloc.1
	sub
	stloc.s	6
	ldloc.3
	ldc.i4.1
	conv.i8
	sub
	stloc.s	5
	ldloc.3
	ldloc.s	4
	sub
	stloc.s	7
	ldloc.0
	ldloc.1
	ceq
	stloc.s	8
	ldloc.0
	ldloc.1
	ceq
	ldc.i4.1
	xor
	stloc.s	8
	ldloc.0
	ldloc.1
	clt
	ldc.i4.1
	xor
	stloc.s	8
	ldloc.0
	ldloc.1
	cgt
	ldc.i4.1
	xor
	stloc.s	8
	ldloc.0
	ldloc.1
	cgt
	stloc.s	8
	ldloc.0
	ldloc.1
	clt
	stloc.s	8
	ldloc.3
	ldloc.s	4
	ceq
	stloc.s	8
	ldloc.3
	ldloc.s	4
	ceq
	ldc.i4.1
	xor
	stloc.s	8
	ldloc.3
	ldloc.s	4
	clt
	ldc.i4.1
	xor
	stloc.s	8
	ldloc.3
	ldloc.s	4
	cgt
	ldc.i4.1
	xor
	stloc.s	8
	ldloc.3
	ldloc.s	4
	cgt
	stloc.s	8
	ldloc.3
	ldloc.s	4
	clt
	stloc.s	8
	ldloc.0
	ldloc.1
	and
	stloc.2
	ldloc.3
	ldloc.s	4
	and
	stloc.s	5
	ldloc.0
	ldloc.1
	or
	stloc.2
	ldloc.3
	ldloc.s	4
	or
	stloc.s	5
	ldloc.0
	ldloc.1
	xor
	stloc.2
	ldloc.3
	ldloc.s	4
	xor
	stloc.s	5
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
