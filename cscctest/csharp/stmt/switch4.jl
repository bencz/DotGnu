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
	invokestatic	"System/String" "CompareOrdinal" "(LSystem/String;LSystem/String;)I"
	ifge	?L1
	dup
	ldc	"T"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	invokestatic	"System/String" "op_Equality" "(LSystem/String;LSystem/String;)Z"
	ifne	?L2
	dup
	ldc	"TO"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	invokestatic	"System/String" "op_Equality" "(LSystem/String;LSystem/String;)Z"
	ifne	?L3
	goto	?L4
?L1:
	dup
	ldc	"TOK"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	invokestatic	"System/String" "op_Equality" "(LSystem/String;LSystem/String;)Z"
	ifne	?L5
	dup
	ldc	"TOKE"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	invokestatic	"System/String" "op_Equality" "(LSystem/String;LSystem/String;)Z"
	ifne	?L6
	dup
	ldc	"TOKEN"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	invokestatic	"System/String" "op_Equality" "(LSystem/String;LSystem/String;)Z"
	ifne	?L7
	goto	?L4
?L2:
	pop
	ldc	"TO"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	goto	?L3
?L3:
	pop
	ldc	"TOK"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	goto	?L5
?L5:
	pop
	ldc	"TOKE"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	goto	?L6
?L6:
	pop
	ldc	"TOKEN"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	goto	?L7
?L7:
	pop
	iconst_5
	ireturn
?L4:
	pop
?L8:
	iconst_0
	ireturn
	.locals 2
	.maxstack 5
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
