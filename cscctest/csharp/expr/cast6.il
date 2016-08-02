.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private auto interface abstract ansi 'I'
{
} // class I
.class private auto interface abstract ansi 'J' implements 'I'
{
} // class J
.class private auto interface abstract ansi 'K' implements 'I', 'J'
{
} // class K
.class private auto interface abstract ansi 'L'
{
} // class L
.class private sequential sealed serializable ansi 'X' extends ['.library']'System'.'ValueType'
{
.field private int32 'x'
} // class X
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method public hidebysig instance void 'm1'() cil managed 
{
	.locals init	(class ['.library']'System'.'Object', class 'Test', class 'Test2', class 'Test3', class 'I', class 'J', class 'K', class 'L', valuetype 'X', class ['.library']'System'.'Object'[], class 'Test'[], class 'I'[], valuetype 'X'[])
	ldnull
	stloc.0
	ldnull
	stloc.1
	ldnull
	stloc.s	4
	ldnull
	stloc.s	9
	ldnull
	stloc.s	10
	ldnull
	stloc.s	11
	ldnull
	stloc.s	12
	ldloc.1
	stloc.0
	ldloc.2
	stloc.0
	ldloc.s	4
	stloc.0
	ldloc.s	9
	stloc.0
	ldloc.s	10
	stloc.0
	ldloc.s	11
	stloc.0
	ldloc.s	12
	stloc.0
	ldloc.0
	castclass	'Test'
	stloc.1
	ldloc.0
	castclass	'Test2'
	stloc.2
	ldloc.0
	castclass	'I'
	stloc.s	4
	ldloc.0
	castclass	class ['.library']'System'.'Object'[]
	stloc.s	9
	ldloc.0
	castclass	class 'Test'[]
	stloc.s	10
	ldloc.0
	castclass	class 'I'[]
	stloc.s	11
	ldloc.0
	castclass	valuetype 'X'[]
	stloc.s	12
	ldloc.1
	stloc.0
	ldloc.2
	stloc.1
	ldloc.0
	castclass	'Test'
	stloc.1
	ldloc.0
	castclass	'Test2'
	stloc.2
	ldloc.1
	castclass	'Test2'
	stloc.2
	ldloc.2
	stloc.s	4
	ldloc.1
	castclass	'I'
	stloc.s	4
	ldloc.s	5
	stloc.s	4
	ldloc.s	6
	stloc.s	4
	ldloc.s	6
	stloc.s	5
	ldloc.s	5
	castclass	'Test2'
	stloc.2
	ldloc.s	5
	castclass	'Test3'
	stloc.3
	ldloc.s	6
	castclass	'L'
	stloc.s	7
	ldloc.s	10
	stloc.s	9
	ldloc.s	11
	stloc.s	9
	ldloc.s	9
	castclass	class 'Test'[]
	stloc.s	10
	ldloc.s	10
	castclass	class 'I'[]
	stloc.s	11
	ret
	.maxstack 1
} // method m1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
.class private auto ansi 'Test2' extends 'Test' implements 'I'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void 'Test'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test2
.class private auto sealed ansi 'Test3' extends ['.library']'System'.'Object' implements 'J'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test3
