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
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	class 'Test9'.'Test9' 'Test9'.'Test9'::'f'()
	stfld	class 'Test9'.'Test9' 'Test'.'Test1'::'T'
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 2
} // method .ctor
.field public static int32 'I'
.field public class 'Test9'.'Test9' 'T'
.method private static hidebysig specialname rtspecialname void '.cctor'() cil managed 
{
	ldsfld	int32 'Test9'.'Test9'::'I'
	stsfld	int32 'Test'.'Test1'::'I'
	ret
	.maxstack 1
} // method .cctor
} // class Test1
} // namespace Test
.namespace 'Test9'
{
.class public auto ansi beforefieldinit 'Test9' extends ['.library']'System'.'Object'
{
.field public static int32 'I'
.method public static hidebysig class 'Test9'.'Test9' 'f'() cil managed 
{
	newobj	instance void 'Test9'.'Test9'::'.ctor'()
	ret
	.maxstack 1
} // method f
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
.method private static hidebysig specialname rtspecialname void '.cctor'() cil managed 
{
	ldc.i4.s	9
	stsfld	int32 'Test9'.'Test9'::'I'
	ret
	.maxstack 1
} // method .cctor
} // class Test9
} // namespace Test9
