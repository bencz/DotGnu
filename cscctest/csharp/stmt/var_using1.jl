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
.method public hidebysig instance void 'Dispose'() cil managed java 
{
	return
	.locals 1
	.maxstack 0
} // method Dispose
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class var
} // namespace Test1
.namespace 'Test1'
{
.class public auto ansi 'TestVar' extends 'Test1'.'var'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void 'Test1'.'var'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class TestVar
} // namespace Test1
.namespace 'Test'
{
.class private auto ansi 'TestDisposable' extends ['.library']'System'.'Object' implements ['.library']'System'.'IDisposable'
{
.field public int32 'i'
.method public hidebysig instance void 'Dispose'() cil managed java 
{
	return
	.locals 1
	.maxstack 0
} // method Dispose
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	iconst_1
	putfield	int32 'Test'.'TestDisposable'::'i'
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 2
} // method .ctor
} // class TestDisposable
} // namespace Test
.namespace 'Test'
{
.class private auto ansi 'TestDisposable1' extends ['.library']'System'.'Object' implements ['.library']'System'.'IDisposable'
{
.field public int32 'i'
.method public hidebysig instance void 'Dispose'() cil managed java 
{
	return
	.locals 1
	.maxstack 0
} // method Dispose
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	iconst_2
	putfield	int32 'Test'.'TestDisposable1'::'i'
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 2
} // method .ctor
} // class TestDisposable1
} // namespace Test
.namespace 'Test'
{
.class private auto ansi 'Test1' extends ['.library']'System'.'Object'
{
.method private static hidebysig void 't1'() cil managed java 
{
	return
	.locals 2
	.maxstack 0
} // method t1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test1
} // namespace Test
.namespace 'Test'
{
.class private auto ansi 'Test2' extends ['.library']'System'.'Object'
{
.method private static hidebysig void 't1'() cil managed java 
{
	return
	.locals 1
	.maxstack 0
} // method t1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test2
} // namespace Test
