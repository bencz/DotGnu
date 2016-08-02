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
.method public virtual hidebysig newslot abstract instance void 'Foo'() cil managed 
{
} // method Foo
} // class IBar
.class public auto ansi 'FuBar' extends ['.library']'System'.'Object'
{
.method public hidebysig instance void 'Foo'() cil managed 
{
	ret
	.maxstack 0
} // method Foo
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class FuBar
.class public auto ansi 'FooBar' extends 'FuBar' implements 'IBar'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void 'FuBar'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
.method private final virtual newslot instance void 'IBar.Foo'() il managed
{
	.override	'IBar'::'Foo'
	ldarg.0
	call	instance void 'FuBar'::'Foo'()
	ret
	.maxstack 1
}
} // class FooBar
