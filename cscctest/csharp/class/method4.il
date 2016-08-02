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
.method private static hidebysig void 'm1'(int32[] 'y') cil managed 
{
.param[1]
.custom instance void ['.library']'System'.'ParamArrayAttribute'::'.ctor'() = (01 00 00 00)
	ret
	.maxstack 0
} // method m1
.method private static hidebysig void 'm2'(int32[] 'y') cil managed 
{
	ldarg.0
	call	void 'Test'::'m1'(int32[])
	ldc.i4.1
	newarr	['.library']'System'.'Int32'
	dup
	ldc.i4.0
	ldc.i4.3
	stelem.i4
	call	void 'Test'::'m1'(int32[])
	ldc.i4.5
	newarr	['.library']'System'.'Int32'
	dup
	ldc.i4.0
	ldc.i4.3
	stelem.i4
	dup
	ldc.i4.1
	ldc.i4.4
	stelem.i4
	dup
	ldc.i4.2
	ldc.i4.5
	stelem.i4
	dup
	ldc.i4.3
	ldc.i4.6
	stelem.i4
	dup
	ldc.i4.4
	ldc.i4.7
	stelem.i4
	call	void 'Test'::'m1'(int32[])
	ldc.i4.0
	newarr	['.library']'System'.'Int32'
	call	void 'Test'::'m1'(int32[])
	ret
	.maxstack 4
} // method m2
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
