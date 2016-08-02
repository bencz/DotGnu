.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private auto sealed serializable ansi 'Dino' extends ['.library']'System'.'Enum'
{
.field public static literal valuetype 'Dino' 'None' = int32(0x00000000)
.field public static literal valuetype 'Dino' 'Trex' = int32(0x00000001)
.field public specialname rtspecialname int32 'value__'
} // class Dino
.class private auto ansi 'FooBar' extends ['.library']'System'.'Object'
{
.method private static hidebysig void 'Foo'(bool 'x') cil managed 
{
	.locals init	(class ['.library']'System'.'String')
	ldarg.0
	brfalse	?L1
	ldc.i4.1
	br	?L2
?L1:
	ldc.i4.0
?L2:
	box	'Dino'
	callvirt	instance class ['.library']'System'.'String' ['.library']'System'.'Object'::'ToString'()
	stloc.0
	ldarg.0
	brfalse	?L3
	ldc.i4.0
	br	?L4
?L3:
	ldc.i4.1
?L4:
	box	'Dino'
	callvirt	instance class ['.library']'System'.'String' ['.library']'System'.'Object'::'ToString'()
	stloc.0
	ret
	.maxstack 1
} // method Foo
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class FooBar
