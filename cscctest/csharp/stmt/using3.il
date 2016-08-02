.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class public auto ansi 'Disposable' extends ['.library']'System'.'Object' implements ['.library']'System'.'IDisposable'
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
} // class Disposable
.class public auto ansi 'XYZ' extends ['.library']'System'.'Object'
{
.method public static hidebysig void 'Main'() cil managed 
{
	.locals init	(class 'Disposable', class 'Disposable')
	newobj	instance void 'Disposable'::'.ctor'()
	stloc.0
	.try {
	leave	?L1
	} finally {
	ldloc.0
	brfalse	?L2
	ldloc.0
	callvirt	instance void [.library]System.IDisposable::Dispose()
?L2:
	endfinally
	}
?L1:
	newobj	instance void 'Disposable'::'.ctor'()
	stloc.1
	.try {
	leave	?L3
	} finally {
	ldloc.1
	brfalse	?L4
	ldloc.1
	callvirt	instance void [.library]System.IDisposable::Dispose()
?L4:
	endfinally
	}
?L3:
	ret
	.maxstack 1
} // method Main
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class XYZ
