.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private auto ansi beforefieldinit 'Test' extends ['.library']'System'.'Object'
{
.field private static int32 'x'
.field private int32 'y'
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	ldc.i4.2
	stfld	int32 'Test'::'y'
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 2
} // method .ctor
.method private static hidebysig specialname rtspecialname void '.cctor'() cil managed 
{
	ldc.i4.1
	stsfld	int32 'Test'::'x'
	ret
	.maxstack 1
} // method .cctor
} // class Test
.class private auto ansi 'Test2' extends ['.library']'System'.'Object'
{
.field private int32 'z'
.method private hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	ldc.i4.3
	stfld	int32 'Test2'::'z'
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 2
} // method .ctor
.method private hidebysig specialname rtspecialname instance void '.ctor'(int32 'x') cil managed 
{
	ldarg.0
	ldc.i4.3
	stfld	int32 'Test2'::'z'
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 2
} // method .ctor
.method private hidebysig specialname rtspecialname instance void '.ctor'(int64 'y') cil managed 
{
	ldarg.0
	call	instance void 'Test2'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test2
