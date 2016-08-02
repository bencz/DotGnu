.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class public auto interface abstract ansi 'IEnumerator'
{
.method public virtual hidebysig newslot abstract instance bool 'MoveNext'() cil managed 
{
} // method MoveNext
.method public virtual hidebysig newslot abstract instance void 'Reset'() cil managed 
{
} // method Reset
.method public virtual hidebysig newslot abstract specialname instance class ['.library']'System'.'Object' 'get_Current'() cil managed 
{
} // method get_Current
.property class ['.library']'System'.'Object' 'Current'()
{
	.get instance class ['.library']'System'.'Object' 'IEnumerator'::'get_Current'()
} // property Current
} // class IEnumerator
.class public auto interface abstract ansi 'IEnumerator2'
{
.method public virtual hidebysig newslot abstract instance void 'Hello'() cil managed 
{
} // method Hello
} // class IEnumerator2
.class private auto ansi 'Test' extends ['.library']'System'.'Object' implements 'IEnumerator2'
{
.method public hidebysig instance bool 'MoveNext'() cil managed 
{
	ldc.i4.0
	ret
	.maxstack 1
} // method MoveNext
.method public hidebysig instance void 'Reset'() cil managed 
{
	ret
	.maxstack 0
} // method Reset
.method public hidebysig specialname instance class ['.library']'System'.'Object' 'get_Current'() cil managed 
{
	ldnull
	ret
	.maxstack 1
} // method get_Current
.property class ['.library']'System'.'Object' 'Current'()
{
	.get instance class ['.library']'System'.'Object' 'Test'::'get_Current'()
} // property Current
.method public virtual hidebysig newslot instance void 'Hello'() cil managed 
{
	ret
	.maxstack 0
} // method Hello
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
