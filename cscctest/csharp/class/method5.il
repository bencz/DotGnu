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
.method private static hidebysig void 'm1'(int32 'x') cil managed 
{
	ret
	.maxstack 0
} // method m1
.method private static hidebysig void 'm1'(float64 'x') cil managed 
{
	ret
	.maxstack 0
} // method m1
.method private static hidebysig void 'm1'(valuetype ['.library']'System'.'Decimal' 'x') cil managed 
{
	ret
	.maxstack 0
} // method m1
.method private static hidebysig void 'm1'(unsigned int32 'x') cil managed 
{
	ret
	.maxstack 0
} // method m1
.method private static hidebysig void 'm2'() cil managed 
{
	ldc.i4.3
	call	void 'Test'::'m1'(int32)
	ldc.i4.3
	call	void 'Test'::'m1'(unsigned int32)
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
