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
.field private static literal int32 'x' = int32(0x00000003)
.method private hidebysig instance int32 'm1'() cil managed 
{
	ldc.i4.3
	ret
	.maxstack 1
} // method m1
.method private static hidebysig int32 'm2'() cil managed 
{
	ldc.i4.3
	ret
	.maxstack 1
} // method m2
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
