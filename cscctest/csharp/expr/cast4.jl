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
.field public specialname rtspecialname int32 'value__'
} // class Color
.class private auto sealed serializable ansi 'ColorSmall' extends ['.library']'System'.'Enum'
{
.field public static literal valuetype 'ColorSmall' 'Red' = int8(0x00)
.field public static literal valuetype 'ColorSmall' 'Green' = int8(0x01)
.field public static literal valuetype 'ColorSmall' 'Blue' = int8(0x02)
.field public specialname rtspecialname int8 'value__'
} // class ColorSmall
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.field public static literal valuetype 'Color' 'red' = int32(0x00000000)
.method private hidebysig instance void 'm1'() cil managed java 
{
	new	"System/Decimal"
	dup
	invokespecial	"System/Decimal" "<init>" "()V"
	astore	17
	iconst_3
	i2b
	istore_3
	iconst_3
	sipush	255
	iand
	istore	4
	iconst_3
	i2s
	istore	5
	iconst_3
	i2c
	istore	6
	iconst_3
	istore	7
	iconst_3
	istore	8
	iconst_3
	i2l
	lstore	9
	iconst_3
	i2l
	lstore	11
	bipush	51
	istore	13
	ldc	float32(0x40400000)
	fstore	14
	ldc2_w	float64(0x4008000000000000)
	dstore	15
	iconst_3
	invokestatic	"System/Decimal" "op_Implicit__iV" "(I)LSystem/Decimal;"
	astore	17
	iconst_0
	istore_1
	iconst_0
	i2b
	istore_2
	iload_3
	istore_1
	iload	4
	istore_1
	iload	5
	istore_1
	iload	6
	istore_1
	iload	7
	istore_1
	iload	8
	invokestatic	"System/Intrinsics/Operations" "ui2i_ovf" "(I)I"
	istore_1
	lload	9
	invokestatic	"System/Intrinsics/Operations" "l2i_ovf" "(J)I"
	istore_1
	lload	11
	invokestatic	"System/Intrinsics/Operations" "ul2i_ovf" "(J)I"
	istore_1
	iload	13
	istore_1
	fload	14
	f2d
	invokestatic	"System/Intrinsics/Operations" "d2i_ovf" "(D)I"
	istore_1
	dload	15
	invokestatic	"System/Intrinsics/Operations" "d2i_ovf" "(D)I"
	istore_1
	aload	17
	invokestatic	"System/Decimal" "op_Explicit__Vi" "(LSystem/Decimal;)I"
	istore_1
	iload_3
	istore_2
	iload	4
	invokestatic	"System/Intrinsics/Operations" "ui2b_ovf" "(I)B"
	istore_2
	iload	5
	invokestatic	"System/Intrinsics/Operations" "i2b_ovf" "(I)B"
	istore_2
	iload	6
	invokestatic	"System/Intrinsics/Operations" "ui2b_ovf" "(I)B"
	istore_2
	iload	7
	invokestatic	"System/Intrinsics/Operations" "i2b_ovf" "(I)B"
	istore_2
	iload	8
	invokestatic	"System/Intrinsics/Operations" "ui2b_ovf" "(I)B"
	istore_2
	lload	9
	invokestatic	"System/Intrinsics/Operations" "l2b_ovf" "(J)B"
	istore_2
	lload	11
	invokestatic	"System/Intrinsics/Operations" "ul2b_ovf" "(J)B"
	istore_2
	iload	13
	invokestatic	"System/Intrinsics/Operations" "ui2b_ovf" "(I)B"
	istore_2
	fload	14
	f2d
	invokestatic	"System/Intrinsics/Operations" "d2i_ovf" "(D)I"
	invokestatic	"System/Intrinsics/Operations" "i2b_ovf" "(I)B"
	istore_2
	dload	15
	invokestatic	"System/Intrinsics/Operations" "d2i_ovf" "(D)I"
	invokestatic	"System/Intrinsics/Operations" "i2b_ovf" "(I)B"
	istore_2
	aload	17
	invokestatic	"System/Decimal" "op_Explicit__V" "(LSystem/Decimal;)B"
	istore_2
	iload_3
	istore_1
	iload	4
	istore_1
	iload	5
	istore_1
	iload	6
	istore_1
	iload	7
	istore_1
	iload	8
	istore_1
	lload	9
	l2i
	istore_1
	lload	11
	l2i
	istore_1
	iload	13
	istore_1
	fload	14
	f2i
	istore_1
	dload	15
	d2i
	istore_1
	aload	17
	invokestatic	"System/Decimal" "op_Explicit__Vi" "(LSystem/Decimal;)I"
	istore_1
	iload_3
	istore_2
	iload	4
	i2b
	istore_2
	iload	5
	i2b
	istore_2
	iload	6
	i2b
	istore_2
	iload	7
	i2b
	istore_2
	iload	8
	i2b
	istore_2
	lload	9
	l2i
	i2b
	istore_2
	lload	11
	l2i
	i2b
	istore_2
	iload	13
	i2b
	istore_2
	fload	14
	f2i
	i2b
	istore_2
	dload	15
	d2i
	i2b
	istore_2
	aload	17
	invokestatic	"System/Decimal" "op_Explicit__V" "(LSystem/Decimal;)B"
	istore_2
	iload_1
	invokestatic	"System/Intrinsics/Operations" "i2b_ovf" "(I)B"
	istore_3
	iload_1
	invokestatic	"System/Intrinsics/Operations" "i2ub_ovf" "(I)I"
	istore	4
	iload_1
	invokestatic	"System/Intrinsics/Operations" "i2s_ovf" "(I)S"
	istore	5
	iload_1
	invokestatic	"System/Intrinsics/Operations" "i2us_ovf" "(I)C"
	istore	6
	iload_1
	istore	7
	iload_1
	invokestatic	"System/Intrinsics/Operations" "i2ui_ovf" "(I)I"
	istore	8
	iload_1
	i2l
	lstore	9
	iload_1
	invokestatic	"System/Intrinsics/Operations" "i2ul_ovf" "(I)J"
	lstore	11
	iload_1
	invokestatic	"System/Intrinsics/Operations" "i2us_ovf" "(I)C"
	istore	13
	iload_1
	i2f
	fstore	14
	iload_1
	i2d
	dstore	15
	iload_1
	invokestatic	"System/Decimal" "op_Implicit__iV" "(I)LSystem/Decimal;"
	astore	17
	iload_2
	istore_3
	iload_2
	invokestatic	"System/Intrinsics/Operations" "i2ub_ovf" "(I)I"
	istore	4
	iload_2
	istore	5
	iload_2
	invokestatic	"System/Intrinsics/Operations" "i2us_ovf" "(I)C"
	istore	6
	iload_2
	istore	7
	iload_2
	invokestatic	"System/Intrinsics/Operations" "i2ui_ovf" "(I)I"
	istore	8
	iload_2
	i2l
	lstore	9
	iload_2
	invokestatic	"System/Intrinsics/Operations" "i2ul_ovf" "(I)J"
	lstore	11
	iload_2
	invokestatic	"System/Intrinsics/Operations" "i2us_ovf" "(I)C"
	istore	13
	iload_2
	i2f
	fstore	14
	iload_2
	i2d
	dstore	15
	iload_2
	invokestatic	"System/Decimal" "op_Implicit__V" "(B)LSystem/Decimal;"
	astore	17
	iload_1
	i2b
	istore_3
	iload_1
	sipush	255
	iand
	istore	4
	iload_1
	i2s
	istore	5
	iload_1
	i2c
	istore	6
	iload_1
	istore	7
	iload_1
	istore	8
	iload_1
	i2l
	lstore	9
	iload_1
	i2l
	lstore	11
	iload_1
	i2c
	istore	13
	iload_1
	i2f
	fstore	14
	iload_1
	i2d
	dstore	15
	iload_1
	invokestatic	"System/Decimal" "op_Implicit__iV" "(I)LSystem/Decimal;"
	astore	17
	iload_2
	istore_3
	iload_2
	sipush	255
	iand
	istore	4
	iload_2
	istore	5
	iload_2
	i2c
	istore	6
	iload_2
	istore	7
	iload_2
	istore	8
	iload_2
	i2l
	lstore	9
	iload_2
	i2l
	lstore	11
	iload_2
	i2c
	istore	13
	iload_2
	i2f
	fstore	14
	iload_2
	i2d
	dstore	15
	iload_2
	invokestatic	"System/Decimal" "op_Implicit__V" "(B)LSystem/Decimal;"
	astore	17
	iload_2
	istore_1
	iload_1
	invokestatic	"System/Intrinsics/Operations" "i2b_ovf" "(I)B"
	istore_2
	iload_2
	istore_1
	iload_1
	i2b
	istore_2
	return
	.locals 18
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
