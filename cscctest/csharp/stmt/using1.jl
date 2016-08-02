.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class public auto ansi beforefieldinit 'Foo' extends ['.library']'System'.'Object'
{
.field public static int32 'c'
.field public static class 'Foo'/'Child' 'child'
.method public static hidebysig void 'Override1'() cil managed java 
{
	bipush	12
	putstatic	int32 'Foo'::'c'
	return
	.locals 0
	.maxstack 1
} // method Override1
.class nested public auto ansi 'Child' extends ['.library']'System'.'Object'
{
.method public hidebysig specialname instance int32 'get_i32'() cil managed java 
{
	bipush	12
	ireturn
	.locals 1
	.maxstack 1
} // method get_i32
.property int32 'i32'()
{
	.get instance int32 'Foo'/'Child'::'get_i32'()
} // property i32
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Child
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
	new	'Foo'/'Child'
	dup
	invokespecial	instance void 'Foo'/'Child'::'.ctor'()
	putstatic	class 'Foo'/'Child' 'Foo'::'child'
	return
	.locals 0
	.maxstack 2
} // method .cctor
} // class Foo
.class private auto ansi 'Bar' extends ['.library']'System'.'Object'
{
.method public static hidebysig void 'NormalCase'() cil managed java 
{
	iconst_0
	istore_0
	goto	?L1
?L2:
?L3:
	iload_0
	iconst_1
	iadd
	istore_0
?L1:
	iload_0
	bipush	10
	if_icmplt	?L2
?L4:
	getstatic	class 'Foo'/'Child' 'Foo'::'child'
	invokespecial	instance int32 'Foo'/'Child'::'get_i32'()
	istore_1
	invokestatic	void 'Foo'::'Override1'()
	return
	.locals 2
	.maxstack 2
} // method NormalCase
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Bar
