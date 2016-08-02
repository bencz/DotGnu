.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class public auto ansi 'SW' extends ['.library']'System'.'Object'
{
.method public hidebysig instance int32 'StringSwitch'(class ['.library']'System'.'String' 's') cil managed 
{
	ldarg.1
	dup
	ldstr	"TOK"
	call	bool [.library]System.String::op_Equality(class [.library]System.String, class [.library]System.String)
	brtrue	?L1
	dup
	ldstr	"TOKEN"
	call	bool [.library]System.String::op_Equality(class [.library]System.String, class [.library]System.String)
	brtrue	?L2
	br	?L3
?L2:
	pop
	ldc.i4.4
	ret
	br	?L4
?L1:
	pop
	ldc.i4.5
	ret
	br	?L4
?L3:
	pop
?L4:
	ldc.i4.0
	ret
	.maxstack 3
} // method StringSwitch
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class SW
