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
.class public auto ansi 'Test2' extends ['.library']'System'.'Object'
{
.method public static hidebysig class 'Test1'.'Test2' 'f1'() cil managed 
{
	newobj	instance void 'Test1'.'Test2'::'.ctor'()
	ret
	.maxstack 1
} // method f1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test2
} // namespace Test1
.namespace 'Test'
{
.class public auto ansi beforefieldinit 'Test1' extends ['.library']'System'.'Object'
{
.field private static int32 'i'
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
.field private static class 'Test1'.'Test2' 'test2'
.method private static hidebysig specialname rtspecialname void '.cctor'() cil managed 
{
	ldc.i4.s	12
	stsfld	int32 'Test'.'Test1'::'i'
	call	class 'Test1'.'Test2' 'Test1'.'Test2'::'f1'()
	stsfld	class 'Test1'.'Test2' 'Test'.'Test1'::'test2'
	ret
	.maxstack 1
} // method .cctor
} // class Test1
} // namespace Test
