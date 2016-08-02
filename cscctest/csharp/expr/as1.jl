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
.class private auto sealed serializable ansi 'Color' extends ['.library']'System'.'Enum'
{
.field public static literal valuetype 'Color' 'Red' = int32(0x00000000)
.field public static literal valuetype 'Color' 'Green' = int32(0x00000001)
.field public static literal valuetype 'Color' 'Blue' = int32(0x00000002)
.field public specialname rtspecialname int32 'value__'
} // class Color
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private hidebysig instance void 'm1'() cil managed java 
{
	aconst_null
	astore_1
	aload_1
	astore_1
	aload_2
	astore_1
	aload_3
	astore_2
	aload_3
	astore	4
	iconst_3
	invokestatic	"System/Int32" "copyIn__" "(I)LSystem/Int32;"
	astore_1
	iconst_3
	invokestatic	"System/Decimal" "op_Implicit__iV" "(I)LSystem/Decimal;"
	invokestatic	"System/Decimal" "copyIn__" "(LSystem/Decimal;)LSystem/Decimal;"
	astore_1
	iconst_3
	invokestatic	"System/Int32" "copyIn__" "(I)LSystem/Int32;"
	astore	5
	iconst_2
	invokestatic	"Color" "copyIn__" "(I)LColor;"
	astore_1
	aload_2
	dup
	instanceof	'Test2'
	ifne	?L1
	pop
	aconst_null
?L1:
	astore_3
	aload_2
	dup
	instanceof	'I'
	ifne	?L2
	pop
	aconst_null
?L2:
	astore	4
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
.class private auto ansi 'Test2' extends 'Test' implements 'I'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void 'Test'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test2
