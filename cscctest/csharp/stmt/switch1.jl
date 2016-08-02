.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private auto sealed serializable ansi 'Color' extends ['.library']'System'.'Enum'
{
.field public static literal valuetype 'Color' 'Red' = int32(0x00000000)
.field public static literal valuetype 'Color' 'Green' = int32(0x00000001)
.field public static literal valuetype 'Color' 'Blue' = int32(0x00000002)
.field public static literal valuetype 'Color' 'Yellow' = int32(0x00000003)
.field public static literal valuetype 'Color' 'Cyan' = int32(0x00000004)
.field public static literal valuetype 'Color' 'Magenta' = int32(0x00000005)
.field public static literal valuetype 'Color' 'Black' = int32(0x00000006)
.field public static literal valuetype 'Color' 'White' = int32(0x00000007)
.field public specialname rtspecialname int32 'value__'
} // class Color
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private hidebysig instance void 'm1'(int32 'x') cil managed java 
{
	iload_1
	tableswitch ?L1 ( 0 :
		?L2,
		?L3,
		?L4,
		?L5
	)
?L2:
	iconst_0
	istore_2
	goto	?L6
?L3:
	iconst_1
	istore_2
	goto	?L6
?L4:
	iconst_2
	istore_2
	goto	?L6
?L5:
	iconst_3
	istore_2
	goto	?L6
?L1:
?L6:
	iload_1
	tableswitch ?L7 ( 0 :
		?L8,
		?L9,
		?L10,
		?L11,
		?L12,
		?L13,
		?L14,
		?L15
	)
?L8:
	iconst_0
	istore_2
	goto	?L16
?L9:
	iconst_1
	istore_2
	goto	?L16
?L10:
	iconst_2
	istore_2
	goto	?L16
?L11:
	iconst_3
	istore_2
	goto	?L16
?L12:
	iconst_4
	istore_2
	goto	?L16
?L13:
	iconst_5
	istore_2
	goto	?L16
?L14:
	bipush	6
	istore_2
	goto	?L16
?L15:
	bipush	7
	istore_2
	goto	?L16
?L7:
?L16:
	iload_1
	tableswitch ?L17 ( 1 :
		?L18,
		?L19,
		?L20,
		?L21,
		?L22,
		?L23,
		?L24,
		?L25
	)
?L18:
	iconst_1
	istore_2
	goto	?L26
?L19:
	iconst_2
	istore_2
	goto	?L26
?L20:
	iconst_3
	istore_2
	goto	?L26
?L21:
	iconst_4
	istore_2
	goto	?L26
?L22:
	iconst_5
	istore_2
	goto	?L26
?L23:
	bipush	6
	istore_2
	goto	?L26
?L24:
	bipush	7
	istore_2
	goto	?L26
?L25:
	bipush	8
	istore_2
	goto	?L26
?L17:
?L26:
	iload_1
	lookupswitch ?L27 (
		100 : ?L28,
		200 : ?L29,
		300 : ?L30,
		400 : ?L31,
		500 : ?L32,
		600 : ?L33,
		700 : ?L34,
		800 : ?L35,
		900 : ?L36,
		1000 : ?L37,
		1100 : ?L38,
		1200 : ?L39,
		1300 : ?L40,
		1400 : ?L41,
		1500 : ?L42,
		1600 : ?L43
	)
?L43:
	sipush	1600
	istore_2
	goto	?L44
?L42:
	sipush	1500
	istore_2
	goto	?L44
?L41:
	sipush	1400
	istore_2
	goto	?L44
?L40:
	sipush	1300
	istore_2
	goto	?L44
?L39:
	sipush	1200
	istore_2
	goto	?L44
?L38:
	sipush	1100
	istore_2
	goto	?L44
?L37:
	sipush	1000
	istore_2
	goto	?L44
?L36:
	sipush	900
	istore_2
	goto	?L44
?L35:
	sipush	800
	istore_2
	goto	?L44
?L34:
	sipush	700
	istore_2
	goto	?L44
?L33:
	sipush	600
	istore_2
	goto	?L44
?L32:
	sipush	500
	istore_2
	goto	?L44
?L31:
	sipush	400
	istore_2
	goto	?L44
?L30:
	sipush	300
	istore_2
	goto	?L44
?L29:
	sipush	200
	istore_2
	goto	?L44
?L28:
	bipush	100
	istore_2
	goto	?L44
?L27:
?L44:
	return
	.locals 3
	.maxstack 1
} // method m1
.method private hidebysig instance void 'm2'(int64 'x') cil managed java 
{
	lload_1
	dup2
	lconst_0
	lcmp
	ifeq	?L45
	dup2
	lconst_1
	lcmp
	ifeq	?L46
	dup2
	iconst_2
	i2l
	lcmp
	ifeq	?L47
	dup2
	iconst_3
	i2l
	lcmp
	ifeq	?L48
	goto	?L49
?L45:
	pop2
	iconst_0
	istore_3
	goto	?L50
?L46:
	pop2
	iconst_1
	istore_3
	goto	?L50
?L47:
	pop2
	iconst_2
	istore_3
	goto	?L50
?L48:
	pop2
	iconst_3
	istore_3
	goto	?L50
?L49:
	pop2
?L50:
	lload_1
	dup2
	iconst_4
	i2l
	lcmp
	ifge	?L51
	dup2
	lconst_0
	lcmp
	ifeq	?L52
	dup2
	lconst_1
	lcmp
	ifeq	?L53
	dup2
	iconst_2
	i2l
	lcmp
	ifeq	?L54
	dup2
	iconst_3
	i2l
	lcmp
	ifeq	?L55
	goto	?L56
?L51:
	dup2
	iconst_4
	i2l
	lcmp
	ifeq	?L57
	dup2
	iconst_5
	i2l
	lcmp
	ifeq	?L58
	dup2
	bipush	6
	i2l
	lcmp
	ifeq	?L59
	dup2
	bipush	7
	i2l
	lcmp
	ifeq	?L60
	goto	?L56
?L52:
	pop2
	iconst_0
	istore_3
	goto	?L61
?L53:
	pop2
	iconst_1
	istore_3
	goto	?L61
?L54:
	pop2
	iconst_2
	istore_3
	goto	?L61
?L55:
	pop2
	iconst_3
	istore_3
	goto	?L61
?L57:
	pop2
	iconst_4
	istore_3
	goto	?L61
?L58:
	pop2
	iconst_5
	istore_3
	goto	?L61
?L59:
	pop2
	bipush	6
	istore_3
	goto	?L61
?L60:
	pop2
	bipush	7
	istore_3
	goto	?L61
?L56:
	pop2
?L61:
	return
	.locals 4
	.maxstack 6
} // method m2
.method private hidebysig instance void 'm3'(class ['.library']'System'.'String' 'x') cil managed java 
{
	aload_1
	dup
	ifnull	?L62
	dup
	ldc	"1"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	invokestatic	"System/String" "op_Equality" "(LSystem/String;LSystem/String;)Z"
	ifne	?L63
	dup
	ldc	"2"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	invokestatic	"System/String" "op_Equality" "(LSystem/String;LSystem/String;)Z"
	ifne	?L64
	dup
	ldc	"3"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	invokestatic	"System/String" "op_Equality" "(LSystem/String;LSystem/String;)Z"
	ifne	?L65
	goto	?L66
?L62:
	pop
	iconst_0
	istore_2
	goto	?L67
?L63:
	pop
	iconst_1
	istore_2
	goto	?L67
?L64:
	pop
	iconst_2
	istore_2
	goto	?L67
?L65:
	pop
	iconst_3
	istore_2
	goto	?L67
?L66:
	pop
?L67:
	aload_1
	dup
	ldc	"4"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	invokestatic	"System/String" "CompareOrdinal" "(LSystem/String;LSystem/String;)I"
	ifge	?L68
	dup
	ifnull	?L69
	dup
	ldc	"1"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	invokestatic	"System/String" "op_Equality" "(LSystem/String;LSystem/String;)Z"
	ifne	?L70
	dup
	ldc	"2"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	invokestatic	"System/String" "op_Equality" "(LSystem/String;LSystem/String;)Z"
	ifne	?L71
	dup
	ldc	"3"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	invokestatic	"System/String" "op_Equality" "(LSystem/String;LSystem/String;)Z"
	ifne	?L72
	goto	?L73
?L68:
	dup
	ldc	"4"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	invokestatic	"System/String" "op_Equality" "(LSystem/String;LSystem/String;)Z"
	ifne	?L74
	dup
	ldc	"5"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	invokestatic	"System/String" "op_Equality" "(LSystem/String;LSystem/String;)Z"
	ifne	?L75
	dup
	ldc	"6"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	invokestatic	"System/String" "op_Equality" "(LSystem/String;LSystem/String;)Z"
	ifne	?L76
	dup
	ldc	"7"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	invokestatic	"System/String" "op_Equality" "(LSystem/String;LSystem/String;)Z"
	ifne	?L77
	goto	?L73
?L77:
	pop
	bipush	7
	istore_2
	goto	?L78
?L76:
	pop
	bipush	6
	istore_2
	goto	?L78
?L75:
	pop
	iconst_5
	istore_2
	goto	?L78
?L74:
	pop
	iconst_4
	istore_2
	goto	?L78
?L72:
	pop
	iconst_3
	istore_2
	goto	?L78
?L71:
	pop
	iconst_2
	istore_2
	goto	?L78
?L70:
	pop
	iconst_1
	istore_2
	goto	?L78
?L69:
	pop
	iconst_0
	istore_2
	goto	?L78
?L73:
	pop
?L78:
	return
	.locals 3
	.maxstack 3
} // method m3
.method private hidebysig instance void 'm4'(valuetype 'Color' 'x') cil managed java 
{
	iload_1
	tableswitch ?L79 ( 0 :
		?L80,
		?L81,
		?L82,
		?L83
	)
?L80:
	iconst_0
	istore_2
	goto	?L84
?L81:
	iconst_1
	istore_2
	goto	?L84
?L82:
	iconst_2
	istore_2
	goto	?L84
?L83:
	iconst_3
	istore_2
	goto	?L84
?L79:
?L84:
	iload_1
	tableswitch ?L85 ( 0 :
		?L86,
		?L87,
		?L88,
		?L89,
		?L90,
		?L91,
		?L92,
		?L93
	)
?L86:
	iconst_0
	istore_2
	goto	?L94
?L87:
	iconst_1
	istore_2
	goto	?L94
?L88:
	iconst_2
	istore_2
	goto	?L94
?L89:
	iconst_3
	istore_2
	goto	?L94
?L90:
	iconst_4
	istore_2
	goto	?L94
?L91:
	iconst_5
	istore_2
	goto	?L94
?L92:
	bipush	6
	istore_2
	goto	?L94
?L93:
	bipush	7
	istore_2
	goto	?L94
?L85:
?L94:
	return
	.locals 3
	.maxstack 1
} // method m4
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test
