.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class public auto ansi 'Readonly' extends ['.library']'System'.'Object'
{
.field public static initonly bool 'flag'
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
	iconst_1
	putstatic	bool 'Readonly'::'flag'
	iconst_0
	putstatic	bool 'Readonly'::'flag'
	return
	.locals 0
	.maxstack 1
} // method .cctor
} // class Readonly
