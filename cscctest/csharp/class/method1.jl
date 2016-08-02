.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private static hidebysig int32 'm1'() cil managed java 
{
	iconst_3
	ireturn
	.locals 0
	.maxstack 1
} // method m1
.method private static hidebysig int32 'm2'() cil managed java 
{
	invokestatic	int32 'Test'::'m1'()
	ireturn
	.locals 0
	.maxstack 1
} // method m2
.method private hidebysig instance int32 'm3'() cil managed java 
{
	invokestatic	int32 'Test'::'m1'()
	ireturn
	.locals 1
	.maxstack 1
} // method m3
.method private hidebysig instance int32 'm4'() cil managed java 
{
	aload_0
	invokespecial	instance int32 'Test'::'m3'()
	ireturn
	.locals 1
	.maxstack 1
} // method m4
.method private static hidebysig int32 'm5'(class 'Test' 't') cil managed java 
{
	aload_0
	invokespecial	instance int32 'Test'::'m3'()
	ireturn
	.locals 1
	.maxstack 1
} // method m5
.method private hidebysig instance int32 'm6'(class 'Test' 't') cil managed java 
{
	aload_1
	invokespecial	instance int32 'Test'::'m4'()
	ireturn
	.locals 2
	.maxstack 1
} // method m6
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test
