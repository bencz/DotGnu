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
.method private hidebysig instance void 'm1'() cil managed 
{
	.locals init	(class ['.library']'System'.'Object')
	ldtoken	['.library']'System'.'Int32'
	call	class [.library]System.Type [.library]System.Type::GetTypeFromHandle(valuetype [.library]System.RuntimeTypeHandle)
	stloc.0
	ldtoken	['.library']'System'.'Void'
	call	class [.library]System.Type [.library]System.Type::GetTypeFromHandle(valuetype [.library]System.RuntimeTypeHandle)
	stloc.0
	ldtoken	int32[]
	call	class [.library]System.Type [.library]System.Type::GetTypeFromHandle(valuetype [.library]System.RuntimeTypeHandle)
	stloc.0
	ldtoken	class 'Test'[]
	call	class [.library]System.Type [.library]System.Type::GetTypeFromHandle(valuetype [.library]System.RuntimeTypeHandle)
	stloc.0
	ret
	.maxstack 1
} // method m1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
