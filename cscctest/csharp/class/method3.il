.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private auto ansi 'Test1' extends ['.library']'System'.'Object'
{
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
.method private static hidebysig void 'm7'(class 'Test1' 'x') cil managed 
{
	ldarg.0
	callvirt	instance void 'Test1'::'m3'()
	ldarg.0
	callvirt	instance void 'Test1'::'m4'()
	ret
	.maxstack 1
} // method m7
.method private static hidebysig void 'm8'(class 'Test2' 'x') cil managed 
{
	ldarg.0
	callvirt	instance void 'Test1'::'m3'()
	ret
	.maxstack 1
} // method m8
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test1
.class private auto ansi 'Test2' extends 'Test1'
{
.method public virtual hidebysig instance void 'm3'() cil managed 
{
	ret
	.maxstack 0
} // method m3
.method private static hidebysig void 'm5'(class 'Test1' 'x') cil managed 
{
	ldarg.0
	callvirt	instance void 'Test1'::'m3'()
	ldarg.0
	callvirt	instance void 'Test1'::'m4'()
	ret
	.maxstack 1
} // method m5
.method private static hidebysig void 'm6'(class 'Test2' 'x') cil managed 
{
	ldarg.0
	callvirt	instance void 'Test1'::'m3'()
	ret
	.maxstack 1
} // method m6
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void 'Test1'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test2
