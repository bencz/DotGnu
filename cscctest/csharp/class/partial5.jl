.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.namespace 'Test'
{
.class public auto ansi beforefieldinit 'Test1' extends ['.library']'System'.'Object'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokestatic	class 'Test9'.'Test9' 'Test9'.'Test9'::'f'()
	putfield	class 'Test9'.'Test9' 'Test'.'Test1'::'T'
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 2
} // method .ctor
.field public static int32 'I'
.field public class 'Test9'.'Test9' 'T'
.method private static hidebysig specialname rtspecialname void '.cctor'() cil managed java 
{
	getstatic	int32 'Test9'.'Test9'::'I'
	putstatic	int32 'Test'.'Test1'::'I'
	return
	.locals 0
	.maxstack 1
} // method .cctor
} // class Test1
} // namespace Test
.namespace 'Test9'
{
.class public auto ansi beforefieldinit 'Test9' extends ['.library']'System'.'Object'
{
.field public static int32 'I'
.method public static hidebysig class 'Test9'.'Test9' 'f'() cil managed java 
{
	new	'Test9'.'Test9'
	dup
	invokespecial	instance void 'Test9'.'Test9'::'.ctor'()
	areturn
	.locals 0
	.maxstack 2
} // method f
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
.method private static hidebysig specialname rtspecialname void '.cctor'() cil managed java 
{
	bipush	9
	putstatic	int32 'Test9'.'Test9'::'I'
	return
	.locals 0
	.maxstack 1
} // method .cctor
} // class Test9
} // namespace Test9
