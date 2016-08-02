.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.namespace 'System.Collections'
{
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
	.get instance class ['.library']'System'.'Object' 'System.Collections'.'IEnumerator'::'get_Current'()
} // property Current
} // class IEnumerator
} // namespace System.Collections
.namespace 'System.Collections'
{
.class public auto interface abstract ansi 'IEnumerable'
{
.method public virtual hidebysig newslot abstract instance class 'System.Collections'.'IEnumerator' 'GetEnumerator'() cil managed 
{
} // method GetEnumerator
} // class IEnumerable
} // namespace System.Collections
.class private auto ansi 'TestEnumerator' extends ['.library']'System'.'Object' implements 'System.Collections'.'IEnumerator'
{
.method private final virtual hidebysig newslot instance bool 'System.Collections.IEnumerator.MoveNext'() cil managed 
{
	.override	'System.Collections'.'IEnumerator'::'MoveNext'
	ldc.i4.0
	ret
	.maxstack 1
} // method System.Collections.IEnumerator.MoveNext
.method public hidebysig instance bool 'MoveNext'() cil managed 
{
	ldc.i4.0
	ret
	.maxstack 1
} // method MoveNext
.method private final virtual hidebysig newslot instance void 'System.Collections.IEnumerator.Reset'() cil managed 
{
	.override	'System.Collections'.'IEnumerator'::'Reset'
	ret
	.maxstack 0
} // method System.Collections.IEnumerator.Reset
.method public hidebysig instance void 'Reset'() cil managed 
{
	ret
	.maxstack 0
} // method Reset
.method private final virtual hidebysig newslot specialname instance class ['.library']'System'.'Object' 'System.Collections.IEnumerator.get_Current'() cil managed 
{
	.override	'System.Collections'.'IEnumerator'::'get_Current'
	ldc.i4.0
	box	['.library']'System'.'Int32'
	ret
	.maxstack 1
} // method System.Collections.IEnumerator.get_Current
.property class ['.library']'System'.'Object' 'System.Collections.IEnumerator.Current'()
{
	.get instance class ['.library']'System'.'Object' 'TestEnumerator'::'System.Collections.IEnumerator.get_Current'()
} // property System.Collections.IEnumerator.Current
.method public hidebysig specialname instance unsigned int8 'get_Current'() cil managed 
{
	ldc.i4.0
	conv.u1
	ret
	.maxstack 1
} // method get_Current
.property unsigned int8 'Current'()
{
	.get instance unsigned int8 'TestEnumerator'::'get_Current'()
} // property Current
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class TestEnumerator
.class private auto ansi 'TestEnumerable' extends ['.library']'System'.'Object' implements 'System.Collections'.'IEnumerable'
{
.method private final virtual hidebysig newslot instance class 'System.Collections'.'IEnumerator' 'System.Collections.IEnumerable.GetEnumerator'() cil managed 
{
	.override	'System.Collections'.'IEnumerable'::'GetEnumerator'
	ldnull
	ret
	.maxstack 1
} // method System.Collections.IEnumerable.GetEnumerator
.method public hidebysig instance class 'TestEnumerator' 'GetEnumerator'() cil managed 
{
	ldnull
	ret
	.maxstack 1
} // method GetEnumerator
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class TestEnumerable
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private hidebysig instance int32 'm1'(class 'TestEnumerable' 'en') cil managed 
{
	.locals init	(class ['.library']'System'.'String', char)
	ldstr	""
	stloc.0
	.locals init	(class 'TestEnumerator')
	.locals init	(class ['.library']'System'.'IDisposable')
	ldarg.1
	call	instance class 'TestEnumerator' 'TestEnumerable'::'GetEnumerator'()
	stloc.2
	.try {
	br	?L1
?L2:
	ldloc.2
	call	instance unsigned int8 'TestEnumerator'::'get_Current'()
	stloc.1
	ldloc.0
	ldloc.1
	call	class [.library]System.String [.library]System.Char::ToString(char)
	call	class [.library]System.String [.library]System.String::Concat(class [.library]System.String, class [.library]System.String)
	stloc.0
?L1:
	ldloc.2
	call	instance bool 'TestEnumerator'::'MoveNext'()
	brtrue	?L2
?L3:
	leave	?L4
	}
	finally {
	ldloc.2
	isinst	[.library]System.IDisposable
	stloc.3
	ldloc.3
	brfalse	?L5
	ldloc.3
	callvirt	instance void [.library]System.IDisposable::Dispose()
?L5:
	endfinally
	}
?L4:
	ldc.i4.0
	ret
	.maxstack 2
} // method m1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
