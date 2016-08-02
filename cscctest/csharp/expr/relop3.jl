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
.method private hidebysig instance void 'm1'() cil managed java 
{
	ldc	"Hello"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	astore_1
	ldc	"Hello"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	astore_2
	aconst_null
	astore_3
	aconst_null
	astore	4
	aload_1
	aload_2
	invokestatic	bool ['.library']'System'.'String'::'op_Equality'(class ['.library']'System'.'String', class ['.library']'System'.'String')
	istore	5
	aload_1
	ifnull	?L1
	iconst_0
	goto	?L2
?L1:
	iconst_1
?L2:
	istore	5
	aload_1
	ifnull	?L3
	iconst_0
	goto	?L4
?L3:
	iconst_1
?L4:
	istore	5
	aload_3
	aload	4
	if_acmpne	?L5
	iconst_1
	goto	?L6
?L5:
	iconst_0
?L6:
	istore	5
	aload_3
	ifnull	?L7
	iconst_0
	goto	?L8
?L7:
	iconst_1
?L8:
	istore	5
	aload_3
	ifnull	?L9
	iconst_0
	goto	?L10
?L9:
	iconst_1
?L10:
	istore	5
	aload_1
	aload_2
	invokestatic	bool ['.library']'System'.'String'::'op_Inequality'(class ['.library']'System'.'String', class ['.library']'System'.'String')
	istore	5
	aload_1
	ifnonnull	?L11
	iconst_0
	goto	?L12
?L11:
	iconst_1
?L12:
	istore	5
	aload_1
	ifnonnull	?L13
	iconst_0
	goto	?L14
?L13:
	iconst_1
?L14:
	istore	5
	aload_3
	aload	4
	if_acmpeq	?L15
	iconst_1
	goto	?L16
?L15:
	iconst_0
?L16:
	istore	5
	aload_3
	ifnonnull	?L17
	iconst_0
	goto	?L18
?L17:
	iconst_1
?L18:
	istore	5
	aload_3
	ifnonnull	?L19
	iconst_0
	goto	?L20
?L19:
	iconst_1
?L20:
	istore	5
	aload_1
	aload_2
	invokestatic	bool ['.library']'System'.'String'::'op_Equality'(class ['.library']'System'.'String', class ['.library']'System'.'String')
	ifeq	?L21
	iconst_1
	istore	5
	goto	?L22
?L21:
	iconst_0
	istore	5
?L22:
	aload_1
	ifnonnull	?L23
	iconst_1
	istore	5
	goto	?L24
?L23:
	iconst_0
	istore	5
?L24:
	aload_1
	ifnonnull	?L25
	iconst_1
	istore	5
	goto	?L26
?L25:
	iconst_0
	istore	5
?L26:
	aload_3
	aload	4
	if_acmpne	?L27
	iconst_1
	istore	5
	goto	?L28
?L27:
	iconst_0
	istore	5
?L28:
	aload_3
	ifnonnull	?L29
	iconst_1
	istore	5
	goto	?L30
?L29:
	iconst_0
	istore	5
?L30:
	aload_3
	ifnonnull	?L31
	iconst_1
	istore	5
	goto	?L32
?L31:
	iconst_0
	istore	5
?L32:
	aload_1
	aload_2
	invokestatic	bool ['.library']'System'.'String'::'op_Inequality'(class ['.library']'System'.'String', class ['.library']'System'.'String')
	ifeq	?L33
	iconst_1
	istore	5
	goto	?L34
?L33:
	iconst_0
	istore	5
?L34:
	aload_1
	ifnull	?L35
	iconst_1
	istore	5
	goto	?L36
?L35:
	iconst_0
	istore	5
?L36:
	aload_1
	ifnull	?L37
	iconst_1
	istore	5
	goto	?L38
?L37:
	iconst_0
	istore	5
?L38:
	aload_3
	aload	4
	if_acmpeq	?L39
	iconst_1
	istore	5
	goto	?L40
?L39:
	iconst_0
	istore	5
?L40:
	aload_3
	ifnull	?L41
	iconst_1
	istore	5
	goto	?L42
?L41:
	iconst_0
	istore	5
?L42:
	aload_3
	ifnull	?L43
	iconst_1
	istore	5
	goto	?L44
?L43:
	iconst_0
	istore	5
?L44:
	return
	.locals 6
	.maxstack 2
} // method m1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test
