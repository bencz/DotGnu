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
.method private hidebysig instance int32 'm1'(class ['.library']'System'.'String' 'str') cil managed 
{
	.locals init	(int32, char)
	ldc.i4.0
	stloc.0
	.locals init	(class ['.library']'System'.'String')
	.locals init	(int32)
	.locals init	(int32)
	ldarg.1
	dup
	stloc.2
	callvirt	instance int32 [.library]System.String::get_Length()
	stloc.s	4
	ldc.i4.0
	stloc.3
	br	?L1
?L2:
	ldloc.2
	ldloc.3
	callvirt	instance char [.library]System.String::get_Chars(int32)
	stloc.1
	ldloc.0
	ldloc.1
	add
	stloc.0
?L3:
	ldloc.3
	ldc.i4.1
	add
	stloc.3
?L1:
	ldloc.3
	ldloc.s	4
	blt	?L2
?L4:
	ldloc.0
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
