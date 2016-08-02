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
.method public hidebysig instance int32 'StringSwitch'(class ['.library']'System'.'String' 's') cil managed java 
{
	aload_1
	dup
	ldc	"TOK"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	invokestatic	"System/String" "op_Equality" "(LSystem/String;LSystem/String;)Z"
	ifne	?L1
	dup
	ldc	"TOKEN"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	invokestatic	"System/String" "op_Equality" "(LSystem/String;LSystem/String;)Z"
	ifne	?L2
	goto	?L3
?L2:
	pop
	iconst_4
	ireturn
	goto	?L4
?L1:
	pop
	iconst_5
	ireturn
	goto	?L4
?L3:
	pop
?L4:
	iconst_0
	ireturn
	.locals 2
	.maxstack 3
} // method StringSwitch
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class SW
