.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class public auto ansi 'invoke2' extends ['.library']'System'.'Object'
{
.field private class ['.library']'System'.'String' 's'
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	aconst_null
	invokevirtual	instance class ['.library']'System'.'String' ['.library']'System'.'Object'::'ToString'()
	putfield	class ['.library']'System'.'String' 'invoke2'::'s'
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 2
} // method .ctor
} // class invoke2
