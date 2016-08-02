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
.method public virtual hidebysig newslot instance void 'print'() cil managed 
{
	ret
	.maxstack 0
} // method print
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class foo
.class private auto ansi 'test' extends 'foo'
{
.method public virtual hidebysig instance void 'print'() cil managed 
{
	ret
	.maxstack 0
} // method print
.method public hidebysig instance void 'callbase'() cil managed 
{
	ldarg.0
	call	instance void 'foo'::'print'()
	ldarg.0
	callvirt	instance void 'foo'::'print'()
	ret
	.maxstack 1
} // method callbase
.method public static hidebysig void 'Main'() cil managed 
{
	ret
	.maxstack 0
} // method Main
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void 'foo'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class test
