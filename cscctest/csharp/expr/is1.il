.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private auto interface abstract ansi 'I'
{
} // class I
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
	.locals init	(bool, class ['.library']'System'.'Object', class 'Test', class 'Test2', class 'I', class ['.library']'System'.'ValueType', int32, valuetype 'Color')
	ldnull
	ldnull
	ceq
	ldc.i4.1
	xor
	stloc.0
	ldc.i4.2
	stloc.s	6
	ldloc.1
	ldnull
	ceq
	ldc.i4.1
	xor
	stloc.0
	ldloc.1
	ldnull
	beq	?L1
	ldc.i4.1
	stloc.s	6
?L1:
	ldloc.1
	ldnull
	bne.un	?L2
	ldc.i4.2
	stloc.s	6
?L2:
	ldloc.2
	ldnull
	ceq
	ldc.i4.1
	xor
	stloc.0
	ldloc.2
	ldnull
	beq	?L3
	ldc.i4.1
	stloc.s	6
?L3:
	ldloc.2
	ldnull
	bne.un	?L4
	ldc.i4.2
	stloc.s	6
?L4:
	ldloc.3
	ldnull
	ceq
	ldc.i4.1
	xor
	stloc.0
	ldloc.3
	ldnull
	beq	?L5
	ldc.i4.1
	stloc.s	6
?L5:
	ldloc.3
	ldnull
	bne.un	?L6
	ldc.i4.2
	stloc.s	6
?L6:
	ldloc.3
	ldnull
	ceq
	ldc.i4.1
	xor
	stloc.0
	ldloc.3
	ldnull
	beq	?L7
	ldc.i4.1
	stloc.s	6
?L7:
	ldloc.3
	ldnull
	bne.un	?L8
	ldc.i4.2
	stloc.s	6
?L8:
	ldc.i4.1
	stloc.0
	ldc.i4.1
	stloc.s	6
	ldc.i4.1
	stloc.0
	ldc.i4.1
	stloc.s	6
	ldc.i4.1
	stloc.0
	ldc.i4.1
	stloc.s	6
	ldc.i4.1
	stloc.0
	ldc.i4.1
	stloc.s	6
	ldloc.2
	isinst	'Test2'
	ldnull
	ceq
	ldc.i4.1
	xor
	stloc.0
	ldloc.2
	isinst	'Test2'
	brfalse	?L9
	ldc.i4.1
	stloc.s	6
?L9:
	ldloc.2
	isinst	'Test2'
	brtrue	?L10
	ldc.i4.2
	stloc.s	6
?L10:
	ldloc.2
	isinst	'I'
	ldnull
	ceq
	ldc.i4.1
	xor
	stloc.0
	ldloc.2
	isinst	'I'
	brfalse	?L11
	ldc.i4.1
	stloc.s	6
?L11:
	ldloc.2
	isinst	'I'
	brtrue	?L12
	ldc.i4.2
	stloc.s	6
?L12:
	ldloc.1
	isinst	['.library']'System'.'Int32'
	ldnull
	ceq
	ldc.i4.1
	xor
	stloc.0
	ldloc.1
	isinst	['.library']'System'.'Int32'
	brfalse	?L13
	ldc.i4.1
	stloc.s	6
?L13:
	ldloc.1
	isinst	['.library']'System'.'Int32'
	brtrue	?L14
	ldc.i4.2
	stloc.s	6
?L14:
	ldc.i4.0
	stloc.0
	br	?L15
	ldc.i4.1
	stloc.s	6
?L15:
	ldc.i4.2
	stloc.s	6
?L16:
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
.class private auto ansi 'Test2' extends 'Test' implements 'I'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void 'Test'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test2
