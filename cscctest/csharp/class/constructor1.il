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
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
.method public hidebysig specialname rtspecialname instance void '.ctor'(int32 'x') cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
.class private auto ansi 'Test2' extends 'Test'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void 'Test'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
.method public hidebysig specialname rtspecialname instance void '.ctor'(int32 'x') cil managed 
{
	ldarg.0
	ldarg.1
	call	instance void 'Test'::'.ctor'(int32)
	ret
	.maxstack 2
} // method .ctor
} // class Test2
.class private auto ansi 'Test3' extends 'Test'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	ldc.i4.3
	call	instance void 'Test'::'.ctor'(int32)
	ret
	.maxstack 2
} // method .ctor
.method public hidebysig specialname rtspecialname instance void '.ctor'(int32 'x') cil managed 
{
	ldarg.0
	call	instance void 'Test3'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test3
.class private auto abstract ansi 'Test4' extends ['.library']'System'.'Object'
{
.method family hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test4
