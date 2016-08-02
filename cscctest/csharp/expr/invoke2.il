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
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	ldnull
	callvirt	instance class ['.library']'System'.'String' ['.library']'System'.'Object'::'ToString'()
	stfld	class ['.library']'System'.'String' 'invoke2'::'s'
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 2
} // method .ctor
} // class invoke2
