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
.method public virtual hidebysig newslot specialname instance int32 'get_x'() cil managed 
{
	ldc.i4.0
	ret
	.maxstack 1
} // method get_x
.method public virtual hidebysig newslot specialname instance void 'set_x'(int32 'value') cil managed 
{
	ret
	.maxstack 0
} // method set_x
.property int32 'x'()
{
	.get instance int32 'Test'::'get_x'()
	.set instance void 'Test'::'set_x'(int32)
} // property x
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
.class private auto ansi 'Test2' extends 'Test'
{
.method public virtual hidebysig specialname instance int32 'get_x'() cil managed 
{
	ldc.i4.0
	ret
	.maxstack 1
} // method get_x
.method public virtual hidebysig specialname instance void 'set_x'(int32 'value') cil managed 
{
	ret
	.maxstack 0
} // method set_x
.property int32 'x'()
{
	.get instance int32 'Test2'::'get_x'()
	.set instance void 'Test2'::'set_x'(int32)
} // property x
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void 'Test'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test2
