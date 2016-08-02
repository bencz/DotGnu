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
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
.method private static hidebysig specialname rtspecialname void '.cctor'() cil managed 
{
	ldc.i4.1
	stsfld	bool 'Readonly'::'flag'
	ldc.i4.0
	stsfld	bool 'Readonly'::'flag'
	ret
	.maxstack 1
} // method .cctor
} // class Readonly
