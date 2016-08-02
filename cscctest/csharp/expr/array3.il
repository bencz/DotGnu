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
.method private hidebysig instance void 'm1'(int32[] 'x', int32 'y') cil managed 
{
	ldarg.0
	ldarg.1
	ldarg.2
	ldelema	int32
	callvirt	instance void 'Test'::'m2'(int32 &)
	ldarg.0
	ldarg.1
	ldarg.2
	ldelema	int32
	callvirt	instance void 'Test'::'m3'(int32 &)
	ret
	.maxstack 3
} // method m1
.method private hidebysig instance void 'm2'(int32 & 'x') cil managed 
{
	ret
	.maxstack 0
} // method m2
.method private hidebysig instance void 'm3'([out] int32 & 'x') cil managed 
{
	ret
	.maxstack 0
} // method m3
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
