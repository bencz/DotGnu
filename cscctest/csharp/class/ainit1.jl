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
.field public static int32[] 'x'
.field public static int64[] 'y'
.method public static hidebysig void 'test'() cil managed java 
{
	iconst_4
	newarray int32
	dup
	iconst_0
	iconst_0
	iastore
	dup
	iconst_1
	iconst_1
	iastore
	dup
	iconst_2
	iconst_2
	iastore
	dup
	iconst_3
	iconst_3
	iastore
	astore_0
	return
	.locals 1
	.maxstack 4
} // method test
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
	iconst_4
	newarray int32
	dup
	iconst_0
	iconst_0
	iastore
	dup
	iconst_1
	iconst_1
	iastore
	dup
	iconst_2
	iconst_2
	iastore
	dup
	iconst_3
	iconst_3
	iastore
	putstatic	int32[] 'Test'::'x'
	iconst_4
	newarray int64
	dup
	iconst_0
	iconst_0
	i2l
	lastore
	dup
	iconst_1
	iconst_1
	i2l
	lastore
	dup
	iconst_2
	iconst_2
	i2l
	lastore
	dup
	iconst_3
	ldc2_w	int64(0x3000000000000000)
	lastore
	putstatic	int64[] 'Test'::'y'
	return
	.locals 0
	.maxstack 5
} // method .cctor
} // class Test
