.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private sequential sealed serializable ansi 'X' extends ['.library']'System'.'ValueType'
{
.field private int32 'x'
.method public hidebysig specialname rtspecialname instance void '.ctor'(int32 '_x') cil managed 
{
	ldarg.0
	ldarg.1
	stfld	int32 'X'::'x'
	ret
	.maxstack 2
} // method .ctor
} // class X
.class private sequential sealed serializable ansi 'Y' extends ['.library']'System'.'ValueType'
{
.field public valuetype 'X' 'x'
.method public hidebysig specialname instance valuetype 'X' 'get_X'() cil managed 
{
	ldarg.0
	ldfld	valuetype 'X' 'Y'::'x'
	ret
	.maxstack 1
} // method get_X
.method public hidebysig specialname instance void 'set_X'(valuetype 'X' 'value') cil managed 
{
	ldarg.0
	ldarg.1
	stfld	valuetype 'X' 'Y'::'x'
	ret
	.maxstack 2
} // method set_X
.property valuetype 'X' 'X'()
{
	.get instance valuetype 'X' 'Y'::'get_X'()
	.set instance void 'Y'::'set_X'(valuetype 'X')
} // property X
} // class Y
.class private auto sealed serializable ansi 'Color' extends ['.library']'System'.'Enum'
{
.field public static literal valuetype 'Color' 'Red' = int32(0x00000000)
.field public static literal valuetype 'Color' 'Green' = int32(0x00000001)
.field public static literal valuetype 'Color' 'Blue' = int32(0x00000002)
.field public specialname rtspecialname int32 'value__'
} // class Color
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private hidebysig instance void 'm1'() cil managed 
{
	.locals init	(int32, valuetype 'X', valuetype 'Color', class 'Test2', class ['.library']'System'.'Object')
	ldc.i4.0
	stloc.0
	ldloca.s	1
	initobj	'X'
	ldc.i4.0
	stloc.2
	ldloca.s	1
	ldc.i4.3
	call	instance void 'X'::'.ctor'(int32)
	newobj	instance void 'Test2'::'.ctor'()
	stloc.3
	ldc.i4.3
	newobj	instance void 'Test2'::'.ctor'(int32)
	stloc.3
	ldc.i4.3
	conv.i8
	newobj	instance void 'Test2'::'.ctor'(int64)
	stloc.3
	newobj	instance void ['.library']'System'.'Object'::'.ctor'()
	stloc.s	4
	ret
	.maxstack 2
} // method m1
.method private hidebysig instance void 'm2'(valuetype 'Y' 'y') cil managed 
{
	ldarga.s	1
	ldflda	valuetype 'X' 'Y'::'x'
	ldc.i4.1
	call	instance void 'X'::'.ctor'(int32)
	ret
	.maxstack 2
} // method m2
.method private hidebysig instance void 'm3'(valuetype 'Y' 'y') cil managed 
{
	ldarga.s	1
	ldc.i4.1
	newobj	instance void 'X'::'.ctor'(int32)
	call	instance void 'Y'::'set_X'(valuetype 'X')
	ret
	.maxstack 2
} // method m3
.method private hidebysig instance valuetype 'X' 'm4'(valuetype 'Y' 'y') cil managed 
{
	ldarga.s	1
	ldflda	valuetype 'X' 'Y'::'x'
	dup
	ldc.i4.1
	call	instance void 'X'::'.ctor'(int32)
	ldobj	'X'
	ret
	.maxstack 3
} // method m4
.method private hidebysig instance void 'm4'() cil managed 
{
	.locals init	(valuetype 'X'[])
	ldc.i4.2
	newarr	'X'
	stloc.0
	ldloc.0
	ldc.i4.0
	ldelema	'X'
	ldc.i4.0
	call	instance void 'X'::'.ctor'(int32)
	ldloc.0
	ldc.i4.1
	ldelema	'X'
	initobj	'X'
	ret
	.maxstack 2
} // method m4
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
.class private auto ansi 'Test2' extends ['.library']'System'.'Object'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
.method public hidebysig specialname rtspecialname instance void '.ctor'(int32 'x') cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
.method public hidebysig specialname rtspecialname instance void '.ctor'(int64 'x') cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test2
