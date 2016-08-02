.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class public sequential sealed serializable ansi 'invoke3' extends ['.library']'System'.'ValueType'
{
.field private int32 'a'
.method public hidebysig specialname rtspecialname instance void '.ctor'(int32 'a') cil managed 
{
	ldarg.0
	ldarg.1
	stfld	int32 'invoke3'::'a'
	ret
	.maxstack 2
} // method .ctor
.method public virtual hidebysig instance class ['.library']'System'.'String' 'ToString'() cil managed 
{
	ldarg.0
	ldobj	'invoke3'
	box	'invoke3'
	call	instance class ['.library']'System'.'String' ['.library']'System'.'Object'::'ToString'()
	ret
	.maxstack 1
} // method ToString
.method public hidebysig instance class ['.library']'System'.'String' 'Test1'() cil managed 
{
	ldarg.0
	ldflda	int32 'invoke3'::'a'
	call	instance class ['.library']'System'.'String' ['.library']'System'.'Int32'::'ToString'()
	ret
	.maxstack 1
} // method Test1
.method public hidebysig instance class ['.library']'System'.'String' 'Test2'(int32[] 'b') cil managed 
{
	ldarg.1
	ldc.i4.0
	ldelema	int32
	call	instance class ['.library']'System'.'String' ['.library']'System'.'Int32'::'ToString'()
	ret
	.maxstack 2
} // method Test2
.method public hidebysig instance class ['.library']'System'.'String' 'Test3'(int32 'b') cil managed 
{
	ldarga.s	1
	call	instance class ['.library']'System'.'String' ['.library']'System'.'Int32'::'ToString'()
	ret
	.maxstack 1
} // method Test3
.method public hidebysig instance class ['.library']'System'.'String' 'Test4'() cil managed 
{
	.locals init	(int32)
	ldc.i4.0
	stloc.0
	ldloca.s	0
	call	instance class ['.library']'System'.'String' ['.library']'System'.'Int32'::'ToString'()
	ret
	.maxstack 1
} // method Test4
.method public hidebysig instance class ['.library']'System'.'String' 'Test5'() cil managed 
{
	.locals init	(int32)
	ldc.i4.0
	stloc.0
	ldloca.s	0
	call	instance class ['.library']'System'.'String' ['.library']'System'.'Int32'::'ToString'()
	ret
	.maxstack 1
} // method Test5
} // class invoke3
