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
	.locals init	(class ['.library']'System'.'Object', class 'Test', class 'Test2', class 'I', class ['.library']'System'.'ValueType')
	ldnull
	stloc.0
	ldloc.0
	stloc.0
	ldloc.1
	stloc.0
	ldloc.2
	stloc.1
	ldloc.2
	stloc.3
	ldc.i4.3
	box	['.library']'System'.'Int32'
	stloc.0
	ldc.i4.3
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	box	['.library']'System'.'Decimal'
	stloc.0
	ldc.i4.3
	box	['.library']'System'.'Int32'
	stloc.s	4
	ldc.i4.2
	box	'Color'
	stloc.0
	ldloc.1
	isinst	'Test2'
	stloc.2
	ldloc.1
	isinst	'I'
	stloc.3
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
