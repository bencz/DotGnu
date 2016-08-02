.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class public auto interface abstract ansi 'IBar'
{
.method public virtual hidebysig newslot abstract instance void 'Foo'() cil managed java 
{
} // method Foo
} // class IBar
.class public auto ansi 'FuBar' extends ['.library']'System'.'Object'
{
.method public hidebysig instance void 'Foo'() cil managed java 
{
	return
	.locals 1
	.maxstack 0
} // method Foo
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class FuBar
.class public auto ansi 'FooBar' extends 'FuBar' implements 'IBar'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void 'FuBar'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class FooBar
