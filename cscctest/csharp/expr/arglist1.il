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
.method private hidebysig instance vararg void 'm1'() cil managed 
{
	.locals init	(valuetype ['.library']'System'.'RuntimeArgumentHandle')
	arglist
	stloc.0
	ret
	.maxstack 1
} // method m1
.method private hidebysig instance vararg void 'm1'(class ['.library']'System'.'String' 'format') cil managed 
{
	.locals init	(valuetype ['.library']'System'.'RuntimeArgumentHandle')
	arglist
	stloc.0
	ret
	.maxstack 1
} // method m1
.method private hidebysig instance void 'm2'() cil managed 
{
	ldarg.0
	ldc.i4.1
	ldc.i4.2
	ldstr	"3"
	ldc.r4	float32(0x40800000)
	callvirt	instance vararg void 'Test'::'m1'(..., int32, int32, class ['.library']'System'.'String', float64)
	ldarg.0
	ldstr	"%d,%d,%s,%f"
	ldc.i4.1
	ldc.i4.2
	ldstr	"3"
	ldc.r4	float32(0x40800000)
	callvirt	instance vararg void 'Test'::'m1'(class ['.library']'System'.'String', ..., int32, int32, class ['.library']'System'.'String', float64)
	ret
	.maxstack 6
} // method m2
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
