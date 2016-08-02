.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private auto ansi 'foo' extends ['.library']'System'.'Object'
{
.field private static int32 '_x'
.method public hidebysig specialname rtspecialname instance void '.ctor'(int32 'x') cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ldarg.1
	stsfld	int32 'foo'::'_x'
	ret
	.maxstack 1
} // method .ctor
.method public static hidebysig int32 'bar'() cil managed 
{
	ldsfld	int32 'foo'::'_x'
	ret
	.maxstack 1
} // method bar
} // class foo
.class private auto ansi 'bar' extends ['.library']'System'.'Object'
{
.field private static int32 'foo'
.method private static hidebysig void 'Main'() cil managed 
{
	.locals init	(class 'foo', int32)
	ldc.i4.1
	stsfld	int32 'bar'::'foo'
	ldsfld	int32 'bar'::'foo'
	newobj	instance void 'foo'::'.ctor'(int32)
	stloc.0
	call	int32 'foo'::'bar'()
	stloc.1
	ret
	.maxstack 1
} // method Main
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class bar
