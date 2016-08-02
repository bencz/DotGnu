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
	call	int32 [.library]System.String::CompareOrdinal(class [.library]System.String, class [.library]System.String)
	ldc.i4.0
	bge	?L1
	dup
	ldstr	"T"
	call	bool [.library]System.String::op_Equality(class [.library]System.String, class [.library]System.String)
	brtrue	?L2
	dup
	ldstr	"TO"
	call	bool [.library]System.String::op_Equality(class [.library]System.String, class [.library]System.String)
	brtrue	?L3
	br	?L4
?L1:
	dup
	ldstr	"TOK"
	call	bool [.library]System.String::op_Equality(class [.library]System.String, class [.library]System.String)
	brtrue	?L5
	dup
	ldstr	"TOKE"
	call	bool [.library]System.String::op_Equality(class [.library]System.String, class [.library]System.String)
	brtrue	?L6
	dup
	ldstr	"TOKEN"
	call	bool [.library]System.String::op_Equality(class [.library]System.String, class [.library]System.String)
	brtrue	?L7
	br	?L4
?L2:
	pop
	ldstr	"TO"
	br	?L3
?L3:
	pop
	ldstr	"TOK"
	br	?L5
?L5:
	pop
	ldstr	"TOKE"
	br	?L6
?L6:
	pop
	ldstr	"TOKEN"
	br	?L7
?L7:
	pop
	ldc.i4.5
	ret
?L4:
	pop
?L8:
	ldc.i4.0
	ret
	.maxstack 5
} // method StringSwitch
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class SW
