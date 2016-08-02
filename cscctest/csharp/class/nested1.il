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
.class nested private auto ansi 'Test2' extends ['.library']'System'.'Object'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test2
.class nested private auto ansi 'Test3' extends 'Test'/'Test2'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void 'Test'/'Test2'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test3
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
.namespace 'X'
{
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.class nested public auto ansi 'Test2' extends ['.library']'System'.'Object'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test2
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
} // namespace X
.namespace 'X'
{
.class private auto ansi 'Test3' extends 'X'.'Test'/'Test2'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void 'X'.'Test'/'Test2'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test3
} // namespace X
