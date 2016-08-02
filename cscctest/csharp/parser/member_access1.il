.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class public auto abstract ansi 'Test' extends ['.library']'System'.'Object'
{
.field public int32 'f1'
.field private int32 'f2'
.field family int32 'f3'
.field famorassem int32 'f4'
.field assembly int32 'f5'
.field private int32 'f6'
.field private static int32 'f7'
.field private static initonly int32 'f8'
.field public initonly int32 'f9'
.field family int32 modreq(['.library']'System.Runtime.CompilerServices'.'IsVolatile') 'f10'
.method public static hidebysig void 'm1'() cil managed 
{
	ret
	.maxstack 0
} // method m1
.method public virtual hidebysig newslot abstract instance void 'm2'() cil managed 
{
} // method m2
.method public virtual hidebysig newslot instance void 'm3'() cil managed 
{
	ret
	.maxstack 0
} // method m3
.method public hidebysig instance void 'm4'() cil managed 
{
	ret
	.maxstack 0
} // method m4
.method public virtual hidebysig newslot instance void 'm5'() cil managed 
{
	ret
	.maxstack 0
} // method m5
.method public virtual hidebysig newslot instance void 'm6'() cil managed 
{
	ret
	.maxstack 0
} // method m6
.method public hidebysig instance void 'm7'() cil managed 
{
} // method m7
.method family hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
.class public auto abstract ansi 'Test2' extends 'Test'
{
.field public int32 'f1'
.field public int32 'f11'
.method public virtual hidebysig abstract instance void 'm2'() cil managed 
{
} // method m2
.method public virtual hidebysig instance void 'm3'() cil managed 
{
	ret
	.maxstack 0
} // method m3
.method public hidebysig instance void 'm4'() cil managed 
{
	ret
	.maxstack 0
} // method m4
.method public virtual hidebysig newslot instance void 'm5'() cil managed 
{
	ret
	.maxstack 0
} // method m5
.method public final virtual hidebysig instance void 'm6'() cil managed 
{
	ret
	.maxstack 0
} // method m6
.method family hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void 'Test'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test2
