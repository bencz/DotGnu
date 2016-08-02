.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.namespace 'Test1'
{
.class public auto ansi 'var' extends ['.library']'System'.'Object' implements ['.library']'System'.'IDisposable'
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
} // class var
} // namespace Test1
.namespace 'Test1'
{
.class public auto ansi 'TestVar' extends 'Test1'.'var'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void 'Test1'.'var'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class TestVar
} // namespace Test1
.namespace 'Test'
{
.class private auto ansi 'TestDisposable' extends ['.library']'System'.'Object' implements ['.library']'System'.'IDisposable'
{
.field public int32 'i'
.method public hidebysig instance void 'Dispose'() cil managed 
{
	ret
	.maxstack 0
} // method Dispose
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	ldc.i4.1
	stfld	int32 'Test'.'TestDisposable'::'i'
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 2
} // method .ctor
} // class TestDisposable
} // namespace Test
.namespace 'Test'
{
.class private auto ansi 'TestDisposable1' extends ['.library']'System'.'Object' implements ['.library']'System'.'IDisposable'
{
.field public int32 'i'
.method public hidebysig instance void 'Dispose'() cil managed 
{
	ret
	.maxstack 0
} // method Dispose
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	ldc.i4.2
	stfld	int32 'Test'.'TestDisposable1'::'i'
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 2
} // method .ctor
} // class TestDisposable1
} // namespace Test
.namespace 'Test'
{
.class private auto ansi 'Test1' extends ['.library']'System'.'Object'
{
.method private static hidebysig void 't1'() cil managed 
{
	.locals init	(class 'Test'.'TestDisposable', class 'Test'.'TestDisposable1')
	newobj	instance void 'Test'.'TestDisposable'::'.ctor'()
	stloc.0
	.try {
	newobj	instance void 'Test'.'TestDisposable1'::'.ctor'()
	stloc.1
	.try {
	ldloc.0
	dup
	ldfld	int32 'Test'.'TestDisposable'::'i'
	ldloc.1
	ldfld	int32 'Test'.'TestDisposable1'::'i'
	add
	stfld	int32 'Test'.'TestDisposable'::'i'
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
	.maxstack 3
} // method t1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test1
} // namespace Test
.namespace 'Test'
{
.class private auto ansi 'Test2' extends ['.library']'System'.'Object'
{
.method private static hidebysig void 't1'() cil managed 
{
	.locals init	(class 'Test1'.'var')
	newobj	instance void 'Test1'.'TestVar'::'.ctor'()
	stloc.0
	.try {
	leave	?L5
	} finally {
	ldloc.0
	brfalse	?L6
	ldloc.0
	callvirt	instance void [.library]System.IDisposable::Dispose()
?L6:
	endfinally
	}
?L5:
	ret
	.maxstack 1
} // method t1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test2
} // namespace Test
