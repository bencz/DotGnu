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
.method private static hidebysig int32 'm1'() cil managed 
{
	ldc.i4.3
	ret
	.maxstack 1
} // method m1
.method private static hidebysig int32 'm2'() cil managed 
{
	call	int32 'Test'::'m1'()
	ret
	.maxstack 1
} // method m2
.method private hidebysig instance int32 'm3'() cil managed 
{
	call	int32 'Test'::'m1'()
	ret
	.maxstack 1
} // method m3
.method private hidebysig instance int32 'm4'() cil managed 
{
	ldarg.0
	callvirt	instance int32 'Test'::'m3'()
	ret
	.maxstack 1
} // method m4
.method private static hidebysig int32 'm5'(class 'Test' 't') cil managed 
{
	ldarg.0
	callvirt	instance int32 'Test'::'m3'()
	ret
	.maxstack 1
} // method m5
.method private hidebysig instance int32 'm6'(class 'Test' 't') cil managed 
{
	ldarg.1
	callvirt	instance int32 'Test'::'m4'()
	ret
	.maxstack 1
} // method m6
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
