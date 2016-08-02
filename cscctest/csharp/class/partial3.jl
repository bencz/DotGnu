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
.method public static hidebysig class 'Test1'.'Test2' 'f1'() cil managed java 
{
	new	'Test1'.'Test2'
	dup
	invokespecial	instance void 'Test1'.'Test2'::'.ctor'()
	areturn
	.locals 0
	.maxstack 2
} // method f1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test2
} // namespace Test1
.namespace 'Test'
{
.class public auto ansi beforefieldinit 'Test1' extends ['.library']'System'.'Object'
{
.field private static int32 'i'
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
.field private static class 'Test1'.'Test2' 'test2'
.method private static hidebysig specialname rtspecialname void '.cctor'() cil managed java 
{
	bipush	12
	putstatic	int32 'Test'.'Test1'::'i'
	invokestatic	class 'Test1'.'Test2' 'Test1'.'Test2'::'f1'()
	putstatic	class 'Test1'.'Test2' 'Test'.'Test1'::'test2'
	return
	.locals 0
	.maxstack 1
} // method .cctor
} // class Test1
} // namespace Test
