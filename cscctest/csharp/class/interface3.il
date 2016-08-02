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
.class private auto ansi 'Test' extends ['.library']'System'.'Object' implements 'IEnumerator'
{
.method public final virtual hidebysig newslot instance bool 'MoveNext'() cil managed 
{
	ldc.i4.0
	ret
	.maxstack 1
} // method MoveNext
.method public final virtual hidebysig newslot instance void 'Reset'() cil managed 
{
	ret
	.maxstack 0
} // method Reset
.method public final virtual hidebysig newslot specialname instance class ['.library']'System'.'Object' 'get_Current'() cil managed 
{
	ldnull
	ret
	.maxstack 1
} // method get_Current
.property class ['.library']'System'.'Object' 'Current'()
{
	.get instance class ['.library']'System'.'Object' 'Test'::'get_Current'()
} // property Current
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
.class private auto ansi 'Test2' extends ['.library']'System'.'Object' implements 'IEnumerator'
{
.method private final virtual hidebysig newslot instance bool 'IEnumerator.MoveNext'() cil managed 
{
	.override	'IEnumerator'::'MoveNext'
	ldc.i4.0
	ret
	.maxstack 1
} // method IEnumerator.MoveNext
.method private final virtual hidebysig newslot instance void 'IEnumerator.Reset'() cil managed 
{
	.override	'IEnumerator'::'Reset'
	ret
	.maxstack 0
} // method IEnumerator.Reset
.method private final virtual hidebysig newslot specialname instance class ['.library']'System'.'Object' 'IEnumerator.get_Current'() cil managed 
{
	.override	'IEnumerator'::'get_Current'
	ldnull
	ret
	.maxstack 1
} // method IEnumerator.get_Current
.property class ['.library']'System'.'Object' 'IEnumerator.Current'()
{
	.get instance class ['.library']'System'.'Object' 'Test2'::'IEnumerator.get_Current'()
} // property IEnumerator.Current
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test2
