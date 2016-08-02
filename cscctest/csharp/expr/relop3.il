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
.method private hidebysig instance void 'm1'() cil managed 
{
	.locals init	(class ['.library']'System'.'String', class ['.library']'System'.'String', class ['.library']'System'.'Object', class ['.library']'System'.'Object', bool)
	ldstr	"Hello"
	stloc.0
	ldstr	"Hello"
	stloc.1
	ldnull
	stloc.2
	ldnull
	stloc.3
	ldloc.0
	ldloc.1
	call	bool ['.library']'System'.'String'::'op_Equality'(class ['.library']'System'.'String', class ['.library']'System'.'String')
	stloc.s	4
	ldloc.0
	ldnull
	ceq
	stloc.s	4
	ldloc.0
	ldnull
	ceq
	stloc.s	4
	ldloc.2
	ldloc.3
	ceq
	stloc.s	4
	ldloc.2
	ldnull
	ceq
	stloc.s	4
	ldloc.2
	ldnull
	ceq
	stloc.s	4
	ldloc.0
	ldloc.1
	call	bool ['.library']'System'.'String'::'op_Inequality'(class ['.library']'System'.'String', class ['.library']'System'.'String')
	stloc.s	4
	ldloc.0
	ldnull
	ceq
	ldc.i4.1
	xor
	stloc.s	4
	ldloc.0
	ldnull
	ceq
	ldc.i4.1
	xor
	stloc.s	4
	ldloc.2
	ldloc.3
	ceq
	ldc.i4.1
	xor
	stloc.s	4
	ldloc.2
	ldnull
	ceq
	ldc.i4.1
	xor
	stloc.s	4
	ldloc.2
	ldnull
	ceq
	ldc.i4.1
	xor
	stloc.s	4
	ldloc.0
	ldloc.1
	call	bool ['.library']'System'.'String'::'op_Equality'(class ['.library']'System'.'String', class ['.library']'System'.'String')
	brfalse	?L1
	ldc.i4.1
	stloc.s	4
	br	?L2
?L1:
	ldc.i4.0
	stloc.s	4
?L2:
	ldloc.0
	brtrue	?L3
	ldc.i4.1
	stloc.s	4
	br	?L4
?L3:
	ldc.i4.0
	stloc.s	4
?L4:
	ldloc.0
	brtrue	?L5
	ldc.i4.1
	stloc.s	4
	br	?L6
?L5:
	ldc.i4.0
	stloc.s	4
?L6:
	ldloc.2
	ldloc.3
	bne.un	?L7
	ldc.i4.1
	stloc.s	4
	br	?L8
?L7:
	ldc.i4.0
	stloc.s	4
?L8:
	ldloc.2
	brtrue	?L9
	ldc.i4.1
	stloc.s	4
	br	?L10
?L9:
	ldc.i4.0
	stloc.s	4
?L10:
	ldloc.2
	brtrue	?L11
	ldc.i4.1
	stloc.s	4
	br	?L12
?L11:
	ldc.i4.0
	stloc.s	4
?L12:
	ldloc.0
	ldloc.1
	call	bool ['.library']'System'.'String'::'op_Inequality'(class ['.library']'System'.'String', class ['.library']'System'.'String')
	brfalse	?L13
	ldc.i4.1
	stloc.s	4
	br	?L14
?L13:
	ldc.i4.0
	stloc.s	4
?L14:
	ldloc.0
	brfalse	?L15
	ldc.i4.1
	stloc.s	4
	br	?L16
?L15:
	ldc.i4.0
	stloc.s	4
?L16:
	ldloc.0
	brfalse	?L17
	ldc.i4.1
	stloc.s	4
	br	?L18
?L17:
	ldc.i4.0
	stloc.s	4
?L18:
	ldloc.2
	ldloc.3
	beq	?L19
	ldc.i4.1
	stloc.s	4
	br	?L20
?L19:
	ldc.i4.0
	stloc.s	4
?L20:
	ldloc.2
	brfalse	?L21
	ldc.i4.1
	stloc.s	4
	br	?L22
?L21:
	ldc.i4.0
	stloc.s	4
?L22:
	ldloc.2
	brfalse	?L23
	ldc.i4.1
	stloc.s	4
	br	?L24
?L23:
	ldc.i4.0
	stloc.s	4
?L24:
	ret
	.maxstack 2
} // method m1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
