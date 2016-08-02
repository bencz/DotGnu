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
	new	"System/Decimal"
	dup
	invokespecial	"System/Decimal" "<init>" "()V"
	astore	15
	bipush	34
	i2b
	istore_1
	bipush	42
	sipush	255
	iand
	istore_2
	bipush	-126
	i2s
	istore_3
	bipush	67
	i2c
	istore	4
	sipush	-1234
	istore	5
	ldc	int32(54321)
	istore	6
	ldc2_w	int64(0xFFFFFFFDB34FE916)
	lstore	7
	ldc2_w	int64(0x000000024CB016EA)
	lstore	9
	bipush	65
	istore	11
	ldc	float32(0x3FC00000)
	fstore	12
	ldc2_w	float64(0x401ACCCCCCCCCCCD)
	dstore	13
	new	"System/Decimal"
	dup
	bipush	35
	iconst_0
	iconst_0
	iconst_0
	iconst_1
	invokespecial	"System/Decimal" "<init>__iiiB" "(IIIZI)"
	astore	15
	iload_1
	istore_1
	iload_1
	invokestatic	"System/Intrinsics/Operations" "i2ub_ovf" "(I)I"
	istore_2
	iload_1
	istore_3
	iload_1
	invokestatic	"System/Intrinsics/Operations" "i2us_ovf" "(I)C"
	istore	4
	iload_1
	istore	5
	iload_1
	invokestatic	"System/Intrinsics/Operations" "i2ui_ovf" "(I)I"
	istore	6
	iload_1
	i2l
	lstore	7
	iload_1
	invokestatic	"System/Intrinsics/Operations" "i2ul_ovf" "(I)J"
	lstore	9
	iload_1
	invokestatic	"System/Intrinsics/Operations" "i2us_ovf" "(I)C"
	istore	11
	iload_1
	i2f
	fstore	12
	iload_1
	i2d
	dstore	13
	iload_1
	invokestatic	"System/Decimal" "op_Implicit__V" "(B)LSystem/Decimal;"
	astore	15
	iload_2
	invokestatic	"System/Intrinsics/Operations" "ui2b_ovf" "(I)B"
	istore_1
	iload_2
	istore_2
	iload_2
	istore_3
	iload_2
	istore	4
	iload_2
	istore	5
	iload_2
	istore	6
	iload_2
	i2l
	lstore	7
	iload_2
	i2l
	lstore	9
	iload_2
	istore	11
	iload_2
	i2f
	fstore	12
	iload_2
	i2d
	dstore	13
	iload_2
	invokestatic	"System/Decimal" "op_Implicit__BV" "(I)LSystem/Decimal;"
	astore	15
	iload_3
	invokestatic	"System/Intrinsics/Operations" "i2b_ovf" "(I)B"
	istore_1
	iload_3
	invokestatic	"System/Intrinsics/Operations" "i2ub_ovf" "(I)I"
	istore_2
	iload_3
	istore_3
	iload_3
	invokestatic	"System/Intrinsics/Operations" "i2us_ovf" "(I)C"
	istore	4
	iload_3
	istore	5
	iload_3
	invokestatic	"System/Intrinsics/Operations" "i2ui_ovf" "(I)I"
	istore	6
	iload_3
	i2l
	lstore	7
	iload_3
	invokestatic	"System/Intrinsics/Operations" "i2ul_ovf" "(I)J"
	lstore	9
	iload_3
	invokestatic	"System/Intrinsics/Operations" "i2us_ovf" "(I)C"
	istore	11
	iload_3
	i2f
	fstore	12
	iload_3
	i2d
	dstore	13
	iload_3
	invokestatic	"System/Decimal" "op_Implicit__V" "(S)LSystem/Decimal;"
	astore	15
	iload	4
	invokestatic	"System/Intrinsics/Operations" "ui2b_ovf" "(I)B"
	istore_1
	iload	4
	invokestatic	"System/Intrinsics/Operations" "i2ub_ovf" "(I)I"
	istore_2
	iload	4
	invokestatic	"System/Intrinsics/Operations" "ui2s_ovf" "(I)S"
	istore_3
	iload	4
	istore	4
	iload	4
	istore	5
	iload	4
	istore	6
	iload	4
	i2l
	lstore	7
	iload	4
	i2l
	lstore	9
	iload	4
	istore	11
	iload	4
	i2f
	fstore	12
	iload	4
	i2d
	dstore	13
	iload	4
	invokestatic	"System/Decimal" "op_Implicit__SV" "(I)LSystem/Decimal;"
	astore	15
	iload	5
	invokestatic	"System/Intrinsics/Operations" "i2b_ovf" "(I)B"
	istore_1
	iload	5
	invokestatic	"System/Intrinsics/Operations" "i2ub_ovf" "(I)I"
	istore_2
	iload	5
	invokestatic	"System/Intrinsics/Operations" "i2s_ovf" "(I)S"
	istore_3
	iload	5
	invokestatic	"System/Intrinsics/Operations" "i2us_ovf" "(I)C"
	istore	4
	iload	5
	istore	5
	iload	5
	invokestatic	"System/Intrinsics/Operations" "i2ui_ovf" "(I)I"
	istore	6
	iload	5
	i2l
	lstore	7
	iload	5
	invokestatic	"System/Intrinsics/Operations" "i2ul_ovf" "(I)J"
	lstore	9
	iload	5
	invokestatic	"System/Intrinsics/Operations" "i2us_ovf" "(I)C"
	istore	11
	iload	5
	i2f
	fstore	12
	iload	5
	i2d
	dstore	13
	iload	5
	invokestatic	"System/Decimal" "op_Implicit__iV" "(I)LSystem/Decimal;"
	astore	15
	iload	6
	invokestatic	"System/Intrinsics/Operations" "ui2b_ovf" "(I)B"
	istore_1
	iload	6
	invokestatic	"System/Intrinsics/Operations" "i2ub_ovf" "(I)I"
	istore_2
	iload	6
	invokestatic	"System/Intrinsics/Operations" "ui2s_ovf" "(I)S"
	istore_3
	iload	6
	invokestatic	"System/Intrinsics/Operations" "i2us_ovf" "(I)C"
	istore	4
	iload	6
	invokestatic	"System/Intrinsics/Operations" "ui2i_ovf" "(I)I"
	istore	5
	iload	6
	istore	6
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	lstore	7
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	lstore	9
	iload	6
	invokestatic	"System/Intrinsics/Operations" "i2us_ovf" "(I)C"
	istore	11
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	l2f
	fstore	12
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	l2d
	dstore	13
	iload	6
	invokestatic	"System/Decimal" "op_Implicit__IV" "(I)LSystem/Decimal;"
	astore	15
	lload	7
	invokestatic	"System/Intrinsics/Operations" "l2b_ovf" "(J)B"
	istore_1
	lload	7
	invokestatic	"System/Intrinsics/Operations" "l2ub_ovf" "(J)I"
	istore_2
	lload	7
	invokestatic	"System/Intrinsics/Operations" "l2s_ovf" "(J)S"
	istore_3
	lload	7
	invokestatic	"System/Intrinsics/Operations" "l2us_ovf" "(J)C"
	istore	4
	lload	7
	invokestatic	"System/Intrinsics/Operations" "l2i_ovf" "(J)I"
	istore	5
	lload	7
	invokestatic	"System/Intrinsics/Operations" "l2ui_ovf" "(J)I"
	istore	6
	lload	7
	lstore	7
	lload	7
	invokestatic	"System/Intrinsics/Operations" "l2ul_ovf" "(J)J"
	lstore	9
	lload	7
	invokestatic	"System/Intrinsics/Operations" "l2us_ovf" "(J)C"
	istore	11
	lload	7
	l2f
	fstore	12
	lload	7
	l2d
	dstore	13
	lload	7
	invokestatic	"System/Decimal" "op_Implicit__lV" "(J)LSystem/Decimal;"
	astore	15
	lload	9
	invokestatic	"System/Intrinsics/Operations" "ul2b_ovf" "(J)B"
	istore_1
	lload	9
	invokestatic	"System/Intrinsics/Operations" "l2ub_ovf" "(J)I"
	istore_2
	lload	9
	invokestatic	"System/Intrinsics/Operations" "ul2s_ovf" "(J)S"
	istore_3
	lload	9
	invokestatic	"System/Intrinsics/Operations" "l2us_ovf" "(J)C"
	istore	4
	lload	9
	invokestatic	"System/Intrinsics/Operations" "ul2i_ovf" "(J)I"
	istore	5
	lload	9
	invokestatic	"System/Intrinsics/Operations" "l2ui_ovf" "(J)I"
	istore	6
	lload	9
	invokestatic	"System/Intrinsics/Operations" "ul2l_ovf" "(J)J"
	lstore	7
	lload	9
	lstore	9
	lload	9
	invokestatic	"System/Intrinsics/Operations" "l2us_ovf" "(J)C"
	istore	11
	lload	9
	invokestatic	"System/Intrinsics/Operations" "ul2f" "(J)F"
	fstore	12
	lload	9
	invokestatic	"System/Intrinsics/Operations" "ul2d" "(J)D"
	dstore	13
	lload	9
	invokestatic	"System/Decimal" "op_Implicit__LV" "(J)LSystem/Decimal;"
	astore	15
	iload	11
	invokestatic	"System/Intrinsics/Operations" "ui2b_ovf" "(I)B"
	istore_1
	iload	11
	invokestatic	"System/Intrinsics/Operations" "i2ub_ovf" "(I)I"
	istore_2
	iload	11
	invokestatic	"System/Intrinsics/Operations" "ui2s_ovf" "(I)S"
	istore_3
	iload	11
	istore	4
	iload	11
	istore	5
	iload	11
	istore	6
	iload	11
	i2l
	lstore	7
	iload	11
	i2l
	lstore	9
	iload	11
	istore	11
	iload	11
	i2f
	fstore	12
	iload	11
	i2d
	dstore	13
	iload	11
	invokestatic	"System/Decimal" "op_Implicit__V" "(C)LSystem/Decimal;"
	astore	15
	fload	12
	f2d
	invokestatic	"System/Intrinsics/Operations" "d2i_ovf" "(D)I"
	invokestatic	"System/Intrinsics/Operations" "i2b_ovf" "(I)B"
	istore_1
	fload	12
	f2d
	invokestatic	"System/Intrinsics/Operations" "d2ui_ovf" "(D)I"
	invokestatic	"System/Intrinsics/Operations" "i2ub_ovf" "(I)I"
	istore_2
	fload	12
	f2d
	invokestatic	"System/Intrinsics/Operations" "d2i_ovf" "(D)I"
	invokestatic	"System/Intrinsics/Operations" "i2s_ovf" "(I)S"
	istore_3
	fload	12
	f2d
	invokestatic	"System/Intrinsics/Operations" "d2i_ovf" "(D)I"
	invokestatic	"System/Intrinsics/Operations" "i2us_ovf" "(I)C"
	istore	4
	fload	12
	f2d
	invokestatic	"System/Intrinsics/Operations" "d2i_ovf" "(D)I"
	istore	5
	fload	12
	f2d
	invokestatic	"System/Intrinsics/Operations" "d2ui_ovf" "(D)I"
	istore	6
	fload	12
	f2d
	invokestatic	"System/Intrinsics/Operations" "d2l_ovf" "(D)J"
	lstore	7
	fload	12
	f2d
	invokestatic	"System/Intrinsics/Operations" "d2ul_ovf" "(D)J"
	lstore	9
	fload	12
	f2d
	invokestatic	"System/Intrinsics/Operations" "d2i_ovf" "(D)I"
	invokestatic	"System/Intrinsics/Operations" "i2us_ovf" "(I)C"
	istore	11
	fload	12
	fstore	12
	fload	12
	f2d
	dstore	13
	fload	12
	invokestatic	"System/Decimal" "op_Explicit__V" "(F)LSystem/Decimal;"
	astore	15
	dload	13
	invokestatic	"System/Intrinsics/Operations" "d2i_ovf" "(D)I"
	invokestatic	"System/Intrinsics/Operations" "i2b_ovf" "(I)B"
	istore_1
	dload	13
	invokestatic	"System/Intrinsics/Operations" "d2ui_ovf" "(D)I"
	invokestatic	"System/Intrinsics/Operations" "i2ub_ovf" "(I)I"
	istore_2
	dload	13
	invokestatic	"System/Intrinsics/Operations" "d2i_ovf" "(D)I"
	invokestatic	"System/Intrinsics/Operations" "i2s_ovf" "(I)S"
	istore_3
	dload	13
	invokestatic	"System/Intrinsics/Operations" "d2i_ovf" "(D)I"
	invokestatic	"System/Intrinsics/Operations" "i2us_ovf" "(I)C"
	istore	4
	dload	13
	invokestatic	"System/Intrinsics/Operations" "d2i_ovf" "(D)I"
	istore	5
	dload	13
	invokestatic	"System/Intrinsics/Operations" "d2ui_ovf" "(D)I"
	istore	6
	dload	13
	invokestatic	"System/Intrinsics/Operations" "d2l_ovf" "(D)J"
	lstore	7
	dload	13
	invokestatic	"System/Intrinsics/Operations" "d2ul_ovf" "(D)J"
	lstore	9
	dload	13
	invokestatic	"System/Intrinsics/Operations" "d2i_ovf" "(D)I"
	invokestatic	"System/Intrinsics/Operations" "i2us_ovf" "(I)C"
	istore	11
	dload	13
	d2f
	fstore	12
	dload	13
	dstore	13
	dload	13
	invokestatic	"System/Decimal" "op_Explicit__V" "(D)LSystem/Decimal;"
	astore	15
	aload	15
	invokestatic	"System/Decimal" "op_Explicit__V" "(LSystem/Decimal;)B"
	istore_1
	aload	15
	invokestatic	"System/Decimal" "op_Explicit__VB" "(LSystem/Decimal;)I"
	istore_2
	aload	15
	invokestatic	"System/Decimal" "op_Explicit__V" "(LSystem/Decimal;)S"
	istore_3
	aload	15
	invokestatic	"System/Decimal" "op_Explicit__VS" "(LSystem/Decimal;)I"
	istore	4
	aload	15
	invokestatic	"System/Decimal" "op_Explicit__Vi" "(LSystem/Decimal;)I"
	istore	5
	aload	15
	invokestatic	"System/Decimal" "op_Explicit__VI" "(LSystem/Decimal;)I"
	istore	6
	aload	15
	invokestatic	"System/Decimal" "op_Explicit__Vl" "(LSystem/Decimal;)J"
	lstore	7
	aload	15
	invokestatic	"System/Decimal" "op_Explicit__VL" "(LSystem/Decimal;)J"
	lstore	9
	aload	15
	invokestatic	"System/Decimal" "op_Explicit__V" "(LSystem/Decimal;)C"
	istore	11
	aload	15
	invokestatic	"System/Decimal" "op_Explicit__V" "(LSystem/Decimal;)F"
	fstore	12
	aload	15
	invokestatic	"System/Decimal" "op_Explicit__V" "(LSystem/Decimal;)D"
	dstore	13
	aload	15
	astore	15
	return
	.locals 16
	.maxstack 7
} // method m1
.method private hidebysig instance void 'm2'() cil managed java 
{
	new	"System/Decimal"
	dup
	invokespecial	"System/Decimal" "<init>" "()V"
	astore	15
	bipush	34
	i2b
	istore_1
	bipush	42
	sipush	255
	iand
	istore_2
	bipush	-126
	i2s
	istore_3
	bipush	67
	i2c
	istore	4
	sipush	-1234
	istore	5
	ldc	int32(54321)
	istore	6
	ldc2_w	int64(0xFFFFFFFDB34FE916)
	lstore	7
	ldc2_w	int64(0x000000024CB016EA)
	lstore	9
	bipush	65
	istore	11
	ldc	float32(0x3FC00000)
	fstore	12
	ldc2_w	float64(0x401ACCCCCCCCCCCD)
	dstore	13
	new	"System/Decimal"
	dup
	bipush	35
	iconst_0
	iconst_0
	iconst_0
	iconst_1
	invokespecial	"System/Decimal" "<init>__iiiB" "(IIIZI)"
	astore	15
	iload_1
	istore_1
	iload_1
	sipush	255
	iand
	istore_2
	iload_1
	istore_3
	iload_1
	i2c
	istore	4
	iload_1
	istore	5
	iload_1
	istore	6
	iload_1
	i2l
	lstore	7
	iload_1
	i2l
	lstore	9
	iload_1
	i2c
	istore	11
	iload_1
	i2f
	fstore	12
	iload_1
	i2d
	dstore	13
	iload_1
	invokestatic	"System/Decimal" "op_Implicit__V" "(B)LSystem/Decimal;"
	astore	15
	iload_2
	i2b
	istore_1
	iload_2
	istore_2
	iload_2
	istore_3
	iload_2
	istore	4
	iload_2
	istore	5
	iload_2
	istore	6
	iload_2
	i2l
	lstore	7
	iload_2
	i2l
	lstore	9
	iload_2
	istore	11
	iload_2
	i2f
	fstore	12
	iload_2
	i2d
	dstore	13
	iload_2
	invokestatic	"System/Decimal" "op_Implicit__BV" "(I)LSystem/Decimal;"
	astore	15
	iload_3
	i2b
	istore_1
	iload_3
	sipush	255
	iand
	istore_2
	iload_3
	istore_3
	iload_3
	i2c
	istore	4
	iload_3
	istore	5
	iload_3
	istore	6
	iload_3
	i2l
	lstore	7
	iload_3
	i2l
	lstore	9
	iload_3
	i2c
	istore	11
	iload_3
	i2f
	fstore	12
	iload_3
	i2d
	dstore	13
	iload_3
	invokestatic	"System/Decimal" "op_Implicit__V" "(S)LSystem/Decimal;"
	astore	15
	iload	4
	i2b
	istore_1
	iload	4
	sipush	255
	iand
	istore_2
	iload	4
	i2s
	istore_3
	iload	4
	istore	4
	iload	4
	istore	5
	iload	4
	istore	6
	iload	4
	i2l
	lstore	7
	iload	4
	i2l
	lstore	9
	iload	4
	istore	11
	iload	4
	i2f
	fstore	12
	iload	4
	i2d
	dstore	13
	iload	4
	invokestatic	"System/Decimal" "op_Implicit__SV" "(I)LSystem/Decimal;"
	astore	15
	iload	5
	i2b
	istore_1
	iload	5
	sipush	255
	iand
	istore_2
	iload	5
	i2s
	istore_3
	iload	5
	i2c
	istore	4
	iload	5
	istore	5
	iload	5
	istore	6
	iload	5
	i2l
	lstore	7
	iload	5
	i2l
	lstore	9
	iload	5
	i2c
	istore	11
	iload	5
	i2f
	fstore	12
	iload	5
	i2d
	dstore	13
	iload	5
	invokestatic	"System/Decimal" "op_Implicit__iV" "(I)LSystem/Decimal;"
	astore	15
	iload	6
	i2b
	istore_1
	iload	6
	sipush	255
	iand
	istore_2
	iload	6
	i2s
	istore_3
	iload	6
	i2c
	istore	4
	iload	6
	istore	5
	iload	6
	istore	6
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	lstore	7
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	lstore	9
	iload	6
	i2c
	istore	11
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	l2f
	fstore	12
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	l2d
	dstore	13
	iload	6
	invokestatic	"System/Decimal" "op_Implicit__IV" "(I)LSystem/Decimal;"
	astore	15
	lload	7
	l2i
	i2b
	istore_1
	lload	7
	l2i
	sipush	255
	iand
	istore_2
	lload	7
	l2i
	i2s
	istore_3
	lload	7
	l2i
	i2c
	istore	4
	lload	7
	l2i
	istore	5
	lload	7
	l2i
	istore	6
	lload	7
	lstore	7
	lload	7
	lstore	9
	lload	7
	l2i
	i2c
	istore	11
	lload	7
	l2f
	fstore	12
	lload	7
	l2d
	dstore	13
	lload	7
	invokestatic	"System/Decimal" "op_Implicit__lV" "(J)LSystem/Decimal;"
	astore	15
	lload	9
	l2i
	i2b
	istore_1
	lload	9
	l2i
	sipush	255
	iand
	istore_2
	lload	9
	l2i
	i2s
	istore_3
	lload	9
	l2i
	i2c
	istore	4
	lload	9
	l2i
	istore	5
	lload	9
	l2i
	istore	6
	lload	9
	lstore	7
	lload	9
	lstore	9
	lload	9
	l2i
	i2c
	istore	11
	lload	9
	invokestatic	"System/Intrinsics/Operations" "ul2f" "(J)F"
	fstore	12
	lload	9
	invokestatic	"System/Intrinsics/Operations" "ul2d" "(J)D"
	dstore	13
	lload	9
	invokestatic	"System/Decimal" "op_Implicit__LV" "(J)LSystem/Decimal;"
	astore	15
	iload	11
	i2b
	istore_1
	iload	11
	sipush	255
	iand
	istore_2
	iload	11
	i2s
	istore_3
	iload	11
	istore	4
	iload	11
	istore	5
	iload	11
	istore	6
	iload	11
	i2l
	lstore	7
	iload	11
	i2l
	lstore	9
	iload	11
	istore	11
	iload	11
	i2f
	fstore	12
	iload	11
	i2d
	dstore	13
	iload	11
	invokestatic	"System/Decimal" "op_Implicit__V" "(C)LSystem/Decimal;"
	astore	15
	fload	12
	f2i
	i2b
	istore_1
	fload	12
	f2i
	sipush	255
	iand
	istore_2
	fload	12
	f2i
	i2s
	istore_3
	fload	12
	f2i
	i2c
	istore	4
	fload	12
	f2i
	istore	5
	fload	12
	f2d
	invokestatic	"System/Intrinsics/Operations" "d2ui" "(D)I"
	istore	6
	fload	12
	f2l
	lstore	7
	fload	12
	f2d
	invokestatic	"System/Intrinsics/Operations" "d2ul" "(D)J"
	lstore	9
	fload	12
	f2i
	i2c
	istore	11
	fload	12
	fstore	12
	fload	12
	f2d
	dstore	13
	fload	12
	invokestatic	"System/Decimal" "op_Explicit__V" "(F)LSystem/Decimal;"
	astore	15
	dload	13
	d2i
	i2b
	istore_1
	dload	13
	d2i
	sipush	255
	iand
	istore_2
	dload	13
	d2i
	i2s
	istore_3
	dload	13
	d2i
	i2c
	istore	4
	dload	13
	d2i
	istore	5
	dload	13
	invokestatic	"System/Intrinsics/Operations" "d2ui" "(D)I"
	istore	6
	dload	13
	d2l
	lstore	7
	dload	13
	invokestatic	"System/Intrinsics/Operations" "d2ul" "(D)J"
	lstore	9
	dload	13
	d2i
	i2c
	istore	11
	dload	13
	d2f
	fstore	12
	dload	13
	dstore	13
	dload	13
	invokestatic	"System/Decimal" "op_Explicit__V" "(D)LSystem/Decimal;"
	astore	15
	aload	15
	invokestatic	"System/Decimal" "op_Explicit__V" "(LSystem/Decimal;)B"
	istore_1
	aload	15
	invokestatic	"System/Decimal" "op_Explicit__VB" "(LSystem/Decimal;)I"
	istore_2
	aload	15
	invokestatic	"System/Decimal" "op_Explicit__V" "(LSystem/Decimal;)S"
	istore_3
	aload	15
	invokestatic	"System/Decimal" "op_Explicit__VS" "(LSystem/Decimal;)I"
	istore	4
	aload	15
	invokestatic	"System/Decimal" "op_Explicit__Vi" "(LSystem/Decimal;)I"
	istore	5
	aload	15
	invokestatic	"System/Decimal" "op_Explicit__VI" "(LSystem/Decimal;)I"
	istore	6
	aload	15
	invokestatic	"System/Decimal" "op_Explicit__Vl" "(LSystem/Decimal;)J"
	lstore	7
	aload	15
	invokestatic	"System/Decimal" "op_Explicit__VL" "(LSystem/Decimal;)J"
	lstore	9
	aload	15
	invokestatic	"System/Decimal" "op_Explicit__V" "(LSystem/Decimal;)C"
	istore	11
	aload	15
	invokestatic	"System/Decimal" "op_Explicit__V" "(LSystem/Decimal;)F"
	fstore	12
	aload	15
	invokestatic	"System/Decimal" "op_Explicit__V" "(LSystem/Decimal;)D"
	dstore	13
	aload	15
	astore	15
	return
	.locals 16
	.maxstack 7
} // method m2
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test
