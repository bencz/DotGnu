.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class public auto ansi 'ExternPropertyTest' extends ['.library']'System'.'Object'
{
.method public hidebysig specialname instance class ['.library']'System'.'String' 'get_MyValue'() cil managed java 
{
} // method get_MyValue
.method public hidebysig specialname instance void 'set_MyValue'(class ['.library']'System'.'String' 'value') cil managed java 
{
} // method set_MyValue
.property class ['.library']'System'.'String' 'MyValue'()
{
	.get instance class ['.library']'System'.'String' 'ExternPropertyTest'::'get_MyValue'()
	.set instance void 'ExternPropertyTest'::'set_MyValue'(class ['.library']'System'.'String')
} // property MyValue
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class ExternPropertyTest
