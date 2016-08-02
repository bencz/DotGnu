.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private auto ansi beforefieldinit 'Test' extends ['.library']'System'.'Object'
{
.field private static int32 'x'
.field private int32 'y'
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	iconst_2
	putfield	int32 'Test'::'y'
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 2
} // method .ctor
.method private static hidebysig specialname rtspecialname void '.cctor'() cil managed java 
{
	iconst_1
	putstatic	int32 'Test'::'x'
	return
	.locals 0
	.maxstack 1
} // method .cctor
} // class Test
.class private auto ansi 'Test2' extends ['.library']'System'.'Object'
{
.field private int32 'z'
.method private hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	iconst_3
	putfield	int32 'Test2'::'z'
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 2
} // method .ctor
.method private hidebysig specialname rtspecialname instance void '.ctor'(int32 'x') cil managed java 
{
	aload_0
	iconst_3
	putfield	int32 'Test2'::'z'
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 2
	.maxstack 2
} // method .ctor
.method private hidebysig specialname rtspecialname instance void '.ctor'(int64 'y') cil managed java 
{
	aload_0
	invokespecial	instance void 'Test2'::'.ctor'()
	return
	.locals 3
	.maxstack 1
} // method .ctor
} // class Test2
