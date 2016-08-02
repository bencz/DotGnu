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
} // class Disposable
.class public auto ansi 'XYZ' extends ['.library']'System'.'Object'
{
.method public static hidebysig void 'Main'() cil managed java 
{
	return
	.locals 2
	.maxstack 0
} // method Main
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class XYZ
