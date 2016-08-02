.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class public sequential sealed serializable ansi 'X' extends ['.library']'System'.'ValueType'
{
.field private int32 'x'
.method public hidebysig specialname rtspecialname instance void '.ctor'(int32 '_x') cil managed 
{
	ldarg.0
	ldarg.1
	stfld	int32 'X'::'x'
	ret
	.maxstack 2
} // method .ctor
.method public hidebysig instance int32 'getX'() cil managed 
{
	ldarg.0
	ldfld	int32 'X'::'x'
	ret
	.maxstack 1
} // method getX
.method public virtual hidebysig newslot instance int32 'getX2'() cil managed 
{
	ldarg.0
	ldfld	int32 'X'::'x'
	ret
	.maxstack 1
} // method getX2
.method public hidebysig specialname instance int32 'get_XProp'() cil managed 
{
	ldarg.0
	ldfld	int32 'X'::'x'
	ret
	.maxstack 1
} // method get_XProp
.property int32 'XProp'()
{
	.get instance int32 'X'::'get_XProp'()
} // property XProp
.method public virtual hidebysig newslot specialname instance int32 'get_XProp2'() cil managed 
{
	ldarg.0
	ldfld	int32 'X'::'x'
	ret
	.maxstack 1
} // method get_XProp2
.property int32 'XProp2'()
{
	.get instance int32 'X'::'get_XProp2'()
} // property XProp2
} // class X
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private hidebysig instance int32 'm1'(valuetype 'X' 'x') cil managed 
{
	ldarga.s	1
	call	instance int32 'X'::'getX'()
	ret
	.maxstack 1
} // method m1
.method private hidebysig instance valuetype 'X' 'm2'() cil managed 
{
	ldc.i4.0
	newobj	instance void 'X'::'.ctor'(int32)
	ret
	.maxstack 1
} // method m2
.method private hidebysig instance int32 'm3'() cil managed 
{
	.locals init	(valuetype 'X')
	ldarg.0
	callvirt	instance valuetype 'X' 'Test'::'m2'()
	stloc.0
	ldloca.s	0
	call	instance int32 'X'::'getX'()
	ret
	.maxstack 1
} // method m3
.method private hidebysig instance int32 'm4'() cil managed 
{
	.locals init	(valuetype 'X')
	ldarg.0
	callvirt	instance valuetype 'X' 'Test'::'m2'()
	stloc.0
	ldloca.s	0
	call	instance int32 'X'::'getX2'()
	ret
	.maxstack 1
} // method m4
.method private hidebysig instance int32 'm5'(valuetype 'X' 'x') cil managed 
{
	ldarga.s	1
	call	instance int32 'X'::'get_XProp'()
	ret
	.maxstack 1
} // method m5
.method private hidebysig instance int32 'm6'() cil managed 
{
	.locals init	(valuetype 'X')
	ldarg.0
	callvirt	instance valuetype 'X' 'Test'::'m2'()
	stloc.0
	ldloca.s	0
	call	instance int32 'X'::'get_XProp'()
	ret
	.maxstack 1
} // method m6
.method private hidebysig instance int32 'm7'(valuetype 'X' 'x') cil managed 
{
	ldarga.s	1
	call	instance int32 'X'::'get_XProp2'()
	ret
	.maxstack 1
} // method m7
.method private hidebysig instance int32 'm8'() cil managed 
{
	.locals init	(valuetype 'X')
	ldarg.0
	callvirt	instance valuetype 'X' 'Test'::'m2'()
	stloc.0
	ldloca.s	0
	call	instance int32 'X'::'get_XProp2'()
	ret
	.maxstack 1
} // method m8
.method private hidebysig instance class ['.library']'System'.'String' 'm9'(valuetype 'X' 'x') cil managed 
{
	ldarg.1
	box	'X'
	callvirt	instance class ['.library']'System'.'String' ['.library']'System'.'Object'::'ToString'()
	ret
	.maxstack 1
} // method m9
.method private hidebysig instance class ['.library']'System'.'String' 'm10'() cil managed 
{
	ldarg.0
	callvirt	instance valuetype 'X' 'Test'::'m2'()
	box	'X'
	callvirt	instance class ['.library']'System'.'String' ['.library']'System'.'Object'::'ToString'()
	ret
	.maxstack 1
} // method m10
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
