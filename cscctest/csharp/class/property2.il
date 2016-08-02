.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private auto abstract ansi 'Test' extends ['.library']'System'.'Object'
{
.method public virtual hidebysig newslot abstract specialname instance int32 'get_x'() cil managed 
{
} // method get_x
.method public virtual hidebysig newslot abstract specialname instance void 'set_x'(int32 'value') cil managed 
{
} // method set_x
.property int32 'x'()
{
	.get instance int32 'Test'::'get_x'()
	.set instance void 'Test'::'set_x'(int32)
} // property x
.method private hidebysig instance int32 'm1'() cil managed 
{
	ldarg.0
	callvirt	instance int32 'Test'::'get_x'()
	ret
	.maxstack 1
} // method m1
.method private static hidebysig int32 'm2'(class 'Test' 't') cil managed 
{
	ldarg.0
	callvirt	instance int32 'Test'::'get_x'()
	ret
	.maxstack 1
} // method m2
.method private hidebysig instance int32 'm3'(class 'Test' 't') cil managed 
{
	ldarg.1
	callvirt	instance int32 'Test'::'get_x'()
	ret
	.maxstack 1
} // method m3
.method private hidebysig instance void 'm4'(int32 'value') cil managed 
{
	ldarg.0
	ldarg.1
	callvirt	instance void 'Test'::'set_x'(int32)
	ret
	.maxstack 2
} // method m4
.method private static hidebysig void 'm5'(class 'Test' 't', int32 'value') cil managed 
{
	ldarg.0
	ldarg.1
	callvirt	instance void 'Test'::'set_x'(int32)
	ret
	.maxstack 2
} // method m5
.method private hidebysig instance void 'm6'(class 'Test' 't', int32 'value') cil managed 
{
	ldarg.1
	ldarg.2
	callvirt	instance void 'Test'::'set_x'(int32)
	ret
	.maxstack 2
} // method m6
.method family hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
