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
.method public virtual hidebysig newslot abstract instance bool 'MoveNext'() cil managed java 
{
} // method MoveNext
.method public virtual hidebysig newslot abstract instance void 'Reset'() cil managed java 
{
} // method Reset
.method public virtual hidebysig newslot abstract specialname instance class ['.library']'System'.'Object' 'get_Current'() cil managed java 
{
} // method get_Current
.property class ['.library']'System'.'Object' 'Current'()
{
	.get instance class ['.library']'System'.'Object' 'IEnumerator'::'get_Current'()
} // property Current
} // class IEnumerator
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.class nested public auto ansi 'Test2' extends ['.library']'System'.'Object' implements 'IEnumerator'
{
.method public final virtual hidebysig newslot instance bool 'MoveNext'() cil managed java 
{
	iconst_0
	ireturn
	.locals 1
	.maxstack 1
} // method MoveNext
.method public final virtual hidebysig newslot instance void 'Reset'() cil managed java 
{
	return
	.locals 1
	.maxstack 0
} // method Reset
.method public final virtual hidebysig newslot specialname instance class ['.library']'System'.'Object' 'get_Current'() cil managed java 
{
	aconst_null
	areturn
	.locals 1
	.maxstack 1
} // method get_Current
.property class ['.library']'System'.'Object' 'Current'()
{
	.get instance class ['.library']'System'.'Object' 'Test'/'Test2'::'get_Current'()
} // property Current
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test2
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test
