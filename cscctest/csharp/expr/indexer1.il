.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class public auto ansi 'Dictionary' extends ['.library']'System'.'Object'
{
.custom instance void ['.library']'System.Reflection'.'DefaultMemberAttribute'::'.ctor'(class ['.library']'System'.'String') = (01 00 04 49 74 65 6D 00 00)
.method public hidebysig specialname instance class ['.library']'System'.'Object' 'get_Item'(class ['.library']'System'.'Object' 'key') cil managed 
{
	ldnull
	ret
	.maxstack 1
} // method get_Item
.method public hidebysig specialname instance void 'set_Item'(class ['.library']'System'.'Object' 'key', class ['.library']'System'.'Object' 'value') cil managed 
{
	ret
	.maxstack 0
} // method set_Item
.property class ['.library']'System'.'Object' 'Item'(class ['.library']'System'.'Object')
{
	.get instance class ['.library']'System'.'Object' 'Dictionary'::'get_Item'(class ['.library']'System'.'Object')
	.set instance void 'Dictionary'::'set_Item'(class ['.library']'System'.'Object', class ['.library']'System'.'Object')
} // property Item
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Dictionary
.class public auto ansi 'Counters' extends ['.library']'System'.'Object'
{
.custom instance void ['.library']'System.Reflection'.'DefaultMemberAttribute'::'.ctor'(class ['.library']'System'.'String') = (01 00 04 49 74 65 6D 00 00)
.field private int32[] 'counters'
.method public hidebysig specialname instance int32 'get_Item'(int32 'index') cil managed 
{
	ldarg.0
	ldfld	int32[] 'Counters'::'counters'
	ldarg.1
	ldelem.i4
	ret
	.maxstack 2
} // method get_Item
.method public hidebysig specialname instance void 'set_Item'(int32 'index', int32 'value') cil managed 
{
	ldarg.0
	ldfld	int32[] 'Counters'::'counters'
	ldarg.1
	ldarg.2
	stelem.i4
	ret
	.maxstack 3
} // method set_Item
.property int32 'Item'(int32)
{
	.get instance int32 'Counters'::'get_Item'(int32)
	.set instance void 'Counters'::'set_Item'(int32, int32)
} // property Item
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Counters
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private hidebysig instance class ['.library']'System'.'Object' 'm1'(class 'Dictionary' 'd', class ['.library']'System'.'Object' 'x') cil managed 
{
	ldarg.1
	ldarg.2
	ldstr	"Hello World"
	call	instance void 'Dictionary'::'set_Item'(class ['.library']'System'.'Object', class ['.library']'System'.'Object')
	ldarg.1
	ldarg.2
	call	instance class ['.library']'System'.'Object' 'Dictionary'::'get_Item'(class ['.library']'System'.'Object')
	ret
	.maxstack 3
} // method m1
.method private hidebysig instance class ['.library']'System'.'Object' 'm2'(class 'Dictionary' 'd', class ['.library']'System'.'Object' 'x') cil managed 
{
	ldarg.1
	ldarg.2
	ldstr	"Hello World"
	.locals init	(class ['.library']'System'.'Object')
	dup
	stloc.0
	call	instance void 'Dictionary'::'set_Item'(class ['.library']'System'.'Object', class ['.library']'System'.'Object')
	ldloc.0
	ret
	.maxstack 4
} // method m2
.method private hidebysig instance int32 'm3'(class 'Counters' 'c') cil managed 
{
	.locals init	(class 'Counters')
	.locals init	(int32)
	ldarg.1
	dup
	stloc.0
	ldc.i4.3
	dup
	stloc.1
	ldloc.0
	ldloc.1
	call	instance int32 'Counters'::'get_Item'(int32)
	ldc.i4.1
	add
	call	instance void 'Counters'::'set_Item'(int32, int32)
	ldarg.1
	dup
	stloc.0
	ldc.i4.3
	dup
	stloc.1
	ldloc.0
	ldloc.1
	call	instance int32 'Counters'::'get_Item'(int32)
	ldc.i4.1
	sub
	dup
	stloc.1
	call	instance void 'Counters'::'set_Item'(int32, int32)
	ldloc.1
	ret
	.maxstack 4
} // method m3
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
