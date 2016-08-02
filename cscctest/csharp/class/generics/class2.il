.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.namespace 'System'
{
.class public auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
} // namespace System
.namespace 'System'
{
.class public auto interface abstract ansi 'ITest'
{
} // class ITest
} // namespace System
.namespace 'System'
{
.class public auto ansi 'Test1`1'<'T'> extends ['.library']'System'.'Object'
{
.class nested public auto ansi 'Test1Nested'<'T'> extends ['.library']'System'.'Object'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test1Nested
.class nested public auto ansi 'Test1Nested1`1'<'T', 'U'> extends ['.library']'System'.'Object'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test1Nested1`1
.class nested public auto ansi 'Test1Nested2`1'<'<_P0>', 'T'> extends ['.library']'System'.'Object'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test1Nested2`1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test1`1
} // namespace System
.namespace 'System'
{
.class public auto ansi 'Test1`2'<.ctor valuetype 'T', 'U'> extends ['.library']'System'.'Object'
{
.class nested public auto ansi 'Test1Nested'<.ctor valuetype 'T', 'U'> extends ['.library']'System'.'Object'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test1Nested
.class nested public auto ansi 'Test1Nested1`1'<.ctor valuetype 'T', '<_P0>', 'U'> extends ['.library']'System'.'Object'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test1Nested1`1
.class nested public auto ansi 'Test1Nested1`2'<.ctor valuetype 'T', '<_P0>', 'U', 'V'> extends ['.library']'System'.'Object'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test1Nested1`2
.class nested public auto ansi 'Test1Nested2`1'<.ctor valuetype '<_P0>', 'U', 'T'> extends ['.library']'System'.'Object'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test1Nested2`1
.class nested public auto ansi 'Test1Nested2`2'<.ctor valuetype '<_P1>', '<_P0>', 'U', 'T'> extends ['.library']'System'.'Object'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test1Nested2`2
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test1`2
} // namespace System
