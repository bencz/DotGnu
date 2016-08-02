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
} // class TestDisposable
.method private static hidebysig void 'Test1'() cil managed java 
{
	new	'Test'/'TestDisposable'
	dup
	invokespecial	instance void 'Test'/'TestDisposable'::'.ctor'()
	astore_0
	return
	.locals 2
	.maxstack 2
} // method Test1
.method private static hidebysig void 'Test2'() cil managed java 
{
	return
	.locals 1
	.maxstack 0
} // method Test2
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test
