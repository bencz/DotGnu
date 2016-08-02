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
.method private static hidebysig void 'm1'(class ['.library']'System'.'String' 'x', class ['.library']'System'.'Object'[] 'y') cil managed 
{
.param[2]
.custom instance void ['.library']'System'.'ParamArrayAttribute'::'.ctor'() = (01 00 00 00)
	ret
	.maxstack 0
} // method m1
.method private static hidebysig void 'm1'(class ['.library']'System'.'String' 'x') cil managed 
{
	ret
	.maxstack 0
} // method m1
.method private static hidebysig void 'm1'(class ['.library']'System'.'Object' 'x') cil managed 
{
	ret
	.maxstack 0
} // method m1
.method private static hidebysig void 'm2'() cil managed 
{
	ldstr	"hello"
	call	void 'Test'::'m1'(class ['.library']'System'.'String')
	ldstr	"hello"
	ldc.i4.1
	newarr	['.library']'System'.'Object'
	dup
	ldc.i4.0
	ldstr	"there"
	stelem.ref
	call	void 'Test'::'m1'(class ['.library']'System'.'String', class ['.library']'System'.'Object'[])
	ret
	.maxstack 5
} // method m2
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
