.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class public auto ansi 'TestIndexer' extends ['.library']'System'.'Object'
{
.custom instance void ['.library']'System.Reflection'.'DefaultMemberAttribute'::'.ctor'(class ['.library']'System'.'String') = (01 00 04 49 74 65 6D 00 00)
.method public hidebysig specialname instance int32 'get_Item'(int32 'x', class ['.library']'System'.'Object'[] 'args') cil managed 
{
.param[2]
.custom instance void ['.library']'System'.'ParamArrayAttribute'::'.ctor'() = (01 00 00 00)
	ldarg.1
	ret
	.maxstack 1
} // method get_Item
.method public hidebysig specialname instance void 'set_Item'(int32 'x', class ['.library']'System'.'Object'[] 'args', int32 'value') cil managed 
{
.param[2]
.custom instance void ['.library']'System'.'ParamArrayAttribute'::'.ctor'() = (01 00 00 00)
	ret
	.maxstack 0
} // method set_Item
.property int32 'Item'(int32, class ['.library']'System'.'Object'[])
{
	.get instance int32 'TestIndexer'::'get_Item'(int32, class ['.library']'System'.'Object'[])
	.set instance void 'TestIndexer'::'set_Item'(int32, class ['.library']'System'.'Object'[], int32)
} // property Item
.method public hidebysig instance void 'test'() cil managed 
{
	.locals init	(int32)
	ldarg.0
	ldc.i4.1
	ldc.i4.8
	newarr	['.library']'System'.'Object'
	dup
	ldc.i4.0
	ldc.i4.2
	box	['.library']'System'.'Int32'
	stelem.ref
	dup
	ldc.i4.1
	ldc.i4.3
	box	['.library']'System'.'Int32'
	stelem.ref
	dup
	ldc.i4.2
	ldc.i4.4
	box	['.library']'System'.'Int32'
	stelem.ref
	dup
	ldc.i4.3
	ldc.i4.5
	box	['.library']'System'.'Int32'
	stelem.ref
	dup
	ldc.i4.4
	ldc.i4.6
	box	['.library']'System'.'Int32'
	stelem.ref
	dup
	ldc.i4.5
	ldc.i4.7
	box	['.library']'System'.'Int32'
	stelem.ref
	dup
	ldc.i4.6
	ldc.i4.8
	box	['.library']'System'.'Int32'
	stelem.ref
	dup
	ldc.i4.7
	ldc.i4.s	9
	box	['.library']'System'.'Int32'
	stelem.ref
	ldc.i4.s	42
	call	instance void 'TestIndexer'::'set_Item'(int32, class ['.library']'System'.'Object'[], int32)
	ldarg.0
	ldc.i4.s	12
	ldc.i4.3
	newarr	['.library']'System'.'Object'
	dup
	ldc.i4.0
	ldc.i4.s	22
	box	['.library']'System'.'Int32'
	stelem.ref
	dup
	ldc.i4.1
	ldc.i4.s	34
	box	['.library']'System'.'Int32'
	stelem.ref
	dup
	ldc.i4.2
	ldc.i4.s	45
	box	['.library']'System'.'Int32'
	stelem.ref
	call	instance int32 'TestIndexer'::'get_Item'(int32, class ['.library']'System'.'Object'[])
	stloc.0
	ret
	.maxstack 6
} // method test
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class TestIndexer
