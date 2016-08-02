.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class public auto ansi 'Test' extends ['.library']'System'.'Object'
{
.class nested private auto ansi 'TestDisposable' extends ['.library']'System'.'Object' implements ['.library']'System'.'IDisposable'
{
.method public hidebysig instance void 'Dispose'() cil managed 
{
	ret
	.maxstack 0
} // method Dispose
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class TestDisposable
.method private static hidebysig void 'Test1'() cil managed 
{
	.locals init	(class 'Test'/'TestDisposable', class 'Test'/'TestDisposable')
	newobj	instance void 'Test'/'TestDisposable'::'.ctor'()
	stloc.0
	ldloc.0
	stloc.1
	.try {
	leave	?L1
	} finally {
	ldloc.1
	brfalse	?L2
	ldloc.1
	callvirt	instance void [.library]System.IDisposable::Dispose()
?L2:
	endfinally
	}
?L1:
	ret
	.maxstack 1
} // method Test1
.method private static hidebysig void 'Test2'() cil managed 
{
	.locals init	(class 'Test'/'TestDisposable')
	newobj	instance void 'Test'/'TestDisposable'::'.ctor'()
	stloc.0
	.try {
	leave	?L3
	} finally {
	ldloc.0
	brfalse	?L4
	ldloc.0
	callvirt	instance void [.library]System.IDisposable::Dispose()
?L4:
	endfinally
	}
?L3:
	ret
	.maxstack 1
} // method Test2
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
