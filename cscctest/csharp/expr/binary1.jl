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
	new	"System/Decimal"
	dup
	invokespecial	"System/Decimal" "<init>" "()V"
	astore	30
	new	"System/Decimal"
	dup
	invokespecial	"System/Decimal" "<init>" "()V"
	astore	45
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
	bipush	34
	i2b
	istore	16
	bipush	42
	sipush	255
	iand
	istore	17
	bipush	-126
	i2s
	istore	18
	bipush	67
	i2c
	istore	19
	sipush	-1234
	istore	20
	ldc	int32(54321)
	istore	21
	ldc2_w	int64(0xFFFFFFFDB34FE916)
	lstore	22
	ldc2_w	int64(0x000000024CB016EA)
	lstore	24
	bipush	65
	istore	26
	ldc	float32(0x3FC00000)
	fstore	27
	ldc2_w	float64(0x401ACCCCCCCCCCCD)
	dstore	28
	new	"System/Decimal"
	dup
	bipush	35
	iconst_0
	iconst_0
	iconst_0
	iconst_1
	invokespecial	"System/Decimal" "<init>__iiiB" "(IIIZI)"
	astore	30
	iload_1
	iload	16
	invokestatic	"System/Intrinsics/Operations" "iadd_ovf" "(II)I"
	istore	35
	iload_1
	iload	17
	invokestatic	"System/Intrinsics/Operations" "iadd_ovf" "(II)I"
	istore	35
	iload_1
	iload	18
	invokestatic	"System/Intrinsics/Operations" "iadd_ovf" "(II)I"
	istore	35
	iload_1
	iload	19
	invokestatic	"System/Intrinsics/Operations" "iadd_ovf" "(II)I"
	istore	35
	iload_1
	iload	20
	invokestatic	"System/Intrinsics/Operations" "iadd_ovf" "(II)I"
	istore	35
	iload_1
	i2l
	iload	21
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	invokestatic	"System/Intrinsics/Operations" "ladd_ovf" "(JJ)J"
	lstore	37
	iload_1
	i2l
	lload	22
	invokestatic	"System/Intrinsics/Operations" "ladd_ovf" "(JJ)J"
	lstore	37
	iload_1
	iload	26
	invokestatic	"System/Intrinsics/Operations" "iadd_ovf" "(II)I"
	istore	35
	iload_1
	i2f
	fload	27
	fadd
	fstore	42
	iload_1
	i2d
	dload	28
	dadd
	dstore	43
	iload_1
	invokestatic	"System/Decimal" "op_Implicit__V" "(B)LSystem/Decimal;"
	aload	30
	invokestatic	"System/Decimal" "op_Addition__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	iload_2
	iload	16
	invokestatic	"System/Intrinsics/Operations" "isub_ovf" "(II)I"
	istore	35
	iload_2
	iload	17
	invokestatic	"System/Intrinsics/Operations" "isub_ovf" "(II)I"
	istore	35
	iload_2
	iload	18
	invokestatic	"System/Intrinsics/Operations" "isub_ovf" "(II)I"
	istore	35
	iload_2
	iload	19
	invokestatic	"System/Intrinsics/Operations" "isub_ovf" "(II)I"
	istore	35
	iload_2
	iload	20
	invokestatic	"System/Intrinsics/Operations" "isub_ovf" "(II)I"
	istore	35
	iload_2
	iload	21
	invokestatic	"System/Intrinsics/Operations" "uisub_ovf" "(II)I"
	istore	36
	iload_2
	i2l
	lload	22
	invokestatic	"System/Intrinsics/Operations" "lsub_ovf" "(JJ)J"
	lstore	37
	iload_2
	i2l
	lload	24
	invokestatic	"System/Intrinsics/Operations" "ulsub_ovf" "(JJ)J"
	lstore	39
	iload_2
	iload	26
	invokestatic	"System/Intrinsics/Operations" "isub_ovf" "(II)I"
	istore	35
	iload_2
	i2f
	fload	27
	fsub
	fstore	42
	iload_2
	i2d
	dload	28
	dsub
	dstore	43
	iload_2
	invokestatic	"System/Decimal" "op_Implicit__BV" "(I)LSystem/Decimal;"
	aload	30
	invokestatic	"System/Decimal" "op_Subtraction__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	iload_3
	iload	16
	invokestatic	"System/Intrinsics/Operations" "imul_ovf" "(II)I"
	istore	35
	iload_3
	iload	17
	invokestatic	"System/Intrinsics/Operations" "imul_ovf" "(II)I"
	istore	35
	iload_3
	iload	18
	invokestatic	"System/Intrinsics/Operations" "imul_ovf" "(II)I"
	istore	35
	iload_3
	iload	19
	invokestatic	"System/Intrinsics/Operations" "imul_ovf" "(II)I"
	istore	35
	iload_3
	iload	20
	invokestatic	"System/Intrinsics/Operations" "imul_ovf" "(II)I"
	istore	35
	iload_3
	i2l
	iload	21
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	invokestatic	"System/Intrinsics/Operations" "lmul_ovf" "(JJ)J"
	lstore	37
	iload_3
	i2l
	lload	22
	invokestatic	"System/Intrinsics/Operations" "lmul_ovf" "(JJ)J"
	lstore	37
	iload_3
	iload	26
	invokestatic	"System/Intrinsics/Operations" "imul_ovf" "(II)I"
	istore	35
	iload_3
	i2f
	fload	27
	fmul
	fstore	42
	iload_3
	i2d
	dload	28
	dmul
	dstore	43
	iload_3
	invokestatic	"System/Decimal" "op_Implicit__V" "(S)LSystem/Decimal;"
	aload	30
	invokestatic	"System/Decimal" "op_Multiply__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	iload	4
	iload	16
	idiv
	istore	35
	iload	4
	iload	17
	idiv
	istore	35
	iload	4
	iload	18
	idiv
	istore	35
	iload	4
	iload	19
	idiv
	istore	35
	iload	4
	iload	20
	idiv
	istore	35
	iload	4
	iload	21
	invokestatic	"System/Intrinsics/Operations" "uidiv" "(II)I"
	istore	36
	iload	4
	i2l
	lload	22
	ldiv
	lstore	37
	iload	4
	i2l
	lload	24
	invokestatic	"System/Intrinsics/Operations" "uldiv" "(JJ)J"
	lstore	39
	iload	4
	iload	26
	idiv
	istore	35
	iload	4
	i2f
	fload	27
	fdiv
	fstore	42
	iload	4
	i2d
	dload	28
	ddiv
	dstore	43
	iload	4
	invokestatic	"System/Decimal" "op_Implicit__SV" "(I)LSystem/Decimal;"
	aload	30
	invokestatic	"System/Decimal" "op_Division__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	iload	5
	iload	16
	irem
	istore	35
	iload	5
	iload	17
	irem
	istore	35
	iload	5
	iload	18
	irem
	istore	35
	iload	5
	iload	19
	irem
	istore	35
	iload	5
	iload	20
	irem
	istore	35
	iload	5
	i2l
	iload	21
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	lrem
	lstore	37
	iload	5
	i2l
	lload	22
	lrem
	lstore	37
	iload	5
	iload	26
	irem
	istore	35
	iload	5
	i2f
	fload	27
	frem
	fstore	42
	iload	5
	i2d
	dload	28
	drem
	dstore	43
	iload	5
	invokestatic	"System/Decimal" "op_Implicit__iV" "(I)LSystem/Decimal;"
	aload	30
	invokestatic	"System/Decimal" "op_Modulus__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	iload	16
	i2l
	invokestatic	"System/Intrinsics/Operations" "ladd_ovf" "(JJ)J"
	lstore	37
	iload	6
	iload	17
	invokestatic	"System/Intrinsics/Operations" "uiadd_ovf" "(II)I"
	istore	36
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	iload	18
	i2l
	invokestatic	"System/Intrinsics/Operations" "ladd_ovf" "(JJ)J"
	lstore	37
	iload	6
	iload	19
	invokestatic	"System/Intrinsics/Operations" "uiadd_ovf" "(II)I"
	istore	36
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	iload	20
	i2l
	invokestatic	"System/Intrinsics/Operations" "ladd_ovf" "(JJ)J"
	lstore	37
	iload	6
	iload	21
	invokestatic	"System/Intrinsics/Operations" "uiadd_ovf" "(II)I"
	istore	36
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	lload	22
	invokestatic	"System/Intrinsics/Operations" "ladd_ovf" "(JJ)J"
	lstore	37
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	lload	24
	invokestatic	"System/Intrinsics/Operations" "uladd_ovf" "(JJ)J"
	lstore	39
	iload	6
	iload	26
	invokestatic	"System/Intrinsics/Operations" "uiadd_ovf" "(II)I"
	istore	36
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	l2f
	fload	27
	fadd
	fstore	42
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	l2d
	dload	28
	dadd
	dstore	43
	iload	6
	invokestatic	"System/Decimal" "op_Implicit__IV" "(I)LSystem/Decimal;"
	aload	30
	invokestatic	"System/Decimal" "op_Addition__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	lload	7
	iload	16
	i2l
	invokestatic	"System/Intrinsics/Operations" "lsub_ovf" "(JJ)J"
	lstore	37
	lload	7
	iload	17
	i2l
	invokestatic	"System/Intrinsics/Operations" "lsub_ovf" "(JJ)J"
	lstore	37
	lload	7
	iload	18
	i2l
	invokestatic	"System/Intrinsics/Operations" "lsub_ovf" "(JJ)J"
	lstore	37
	lload	7
	iload	19
	i2l
	invokestatic	"System/Intrinsics/Operations" "lsub_ovf" "(JJ)J"
	lstore	37
	lload	7
	iload	20
	i2l
	invokestatic	"System/Intrinsics/Operations" "lsub_ovf" "(JJ)J"
	lstore	37
	lload	7
	iload	21
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	invokestatic	"System/Intrinsics/Operations" "lsub_ovf" "(JJ)J"
	lstore	37
	lload	7
	lload	22
	invokestatic	"System/Intrinsics/Operations" "lsub_ovf" "(JJ)J"
	lstore	37
	lload	7
	iload	26
	i2l
	invokestatic	"System/Intrinsics/Operations" "lsub_ovf" "(JJ)J"
	lstore	37
	lload	7
	l2f
	fload	27
	fsub
	fstore	42
	lload	7
	l2d
	dload	28
	dsub
	dstore	43
	lload	7
	invokestatic	"System/Decimal" "op_Implicit__lV" "(J)LSystem/Decimal;"
	aload	30
	invokestatic	"System/Decimal" "op_Subtraction__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	lload	9
	iload	17
	i2l
	invokestatic	"System/Intrinsics/Operations" "ulmul_ovf" "(JJ)J"
	lstore	39
	lload	9
	iload	19
	i2l
	invokestatic	"System/Intrinsics/Operations" "ulmul_ovf" "(JJ)J"
	lstore	39
	lload	9
	iload	21
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	invokestatic	"System/Intrinsics/Operations" "ulmul_ovf" "(JJ)J"
	lstore	39
	lload	9
	lload	24
	invokestatic	"System/Intrinsics/Operations" "ulmul_ovf" "(JJ)J"
	lstore	39
	lload	9
	iload	26
	i2l
	invokestatic	"System/Intrinsics/Operations" "ulmul_ovf" "(JJ)J"
	lstore	39
	lload	9
	invokestatic	"System/Intrinsics/Operations" "ul2f" "(J)F"
	fload	27
	fmul
	fstore	42
	lload	9
	invokestatic	"System/Intrinsics/Operations" "ul2d" "(J)D"
	dload	28
	dmul
	dstore	43
	lload	9
	invokestatic	"System/Decimal" "op_Implicit__LV" "(J)LSystem/Decimal;"
	aload	30
	invokestatic	"System/Decimal" "op_Multiply__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	fload	12
	iload	16
	i2f
	fdiv
	fstore	42
	fload	12
	iload	17
	i2f
	fdiv
	fstore	42
	fload	12
	iload	18
	i2f
	fdiv
	fstore	42
	fload	12
	iload	19
	i2f
	fdiv
	fstore	42
	fload	12
	iload	20
	i2f
	fdiv
	fstore	42
	fload	12
	iload	21
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	l2f
	fdiv
	fstore	42
	fload	12
	lload	22
	l2f
	fdiv
	fstore	42
	fload	12
	lload	24
	invokestatic	"System/Intrinsics/Operations" "ul2f" "(J)F"
	fdiv
	fstore	42
	fload	12
	iload	26
	i2f
	fdiv
	fstore	42
	fload	12
	fload	27
	fdiv
	fstore	42
	fload	12
	f2d
	dload	28
	ddiv
	dstore	43
	dload	13
	iload	16
	i2d
	drem
	dstore	43
	dload	13
	iload	17
	i2d
	drem
	dstore	43
	dload	13
	iload	18
	i2d
	drem
	dstore	43
	dload	13
	iload	19
	i2d
	drem
	dstore	43
	dload	13
	iload	20
	i2d
	drem
	dstore	43
	dload	13
	iload	21
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	l2d
	drem
	dstore	43
	dload	13
	lload	22
	l2d
	drem
	dstore	43
	dload	13
	lload	24
	invokestatic	"System/Intrinsics/Operations" "ul2d" "(J)D"
	drem
	dstore	43
	dload	13
	iload	26
	i2d
	drem
	dstore	43
	dload	13
	fload	27
	f2d
	drem
	dstore	43
	dload	13
	dload	28
	drem
	dstore	43
	aload	15
	iload	16
	invokestatic	"System/Decimal" "op_Implicit__V" "(B)LSystem/Decimal;"
	invokestatic	"System/Decimal" "op_Addition__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	aload	15
	iload	17
	invokestatic	"System/Decimal" "op_Implicit__BV" "(I)LSystem/Decimal;"
	invokestatic	"System/Decimal" "op_Addition__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	aload	15
	iload	18
	invokestatic	"System/Decimal" "op_Implicit__V" "(S)LSystem/Decimal;"
	invokestatic	"System/Decimal" "op_Addition__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	aload	15
	iload	19
	invokestatic	"System/Decimal" "op_Implicit__SV" "(I)LSystem/Decimal;"
	invokestatic	"System/Decimal" "op_Addition__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	aload	15
	iload	20
	invokestatic	"System/Decimal" "op_Implicit__iV" "(I)LSystem/Decimal;"
	invokestatic	"System/Decimal" "op_Addition__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	aload	15
	iload	21
	invokestatic	"System/Decimal" "op_Implicit__IV" "(I)LSystem/Decimal;"
	invokestatic	"System/Decimal" "op_Addition__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	aload	15
	lload	22
	invokestatic	"System/Decimal" "op_Implicit__lV" "(J)LSystem/Decimal;"
	invokestatic	"System/Decimal" "op_Addition__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	aload	15
	lload	24
	invokestatic	"System/Decimal" "op_Implicit__LV" "(J)LSystem/Decimal;"
	invokestatic	"System/Decimal" "op_Addition__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	aload	15
	iload	26
	invokestatic	"System/Decimal" "op_Implicit__V" "(C)LSystem/Decimal;"
	invokestatic	"System/Decimal" "op_Addition__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	aload	15
	aload	30
	invokestatic	"System/Decimal" "op_Addition__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	iload_1
	iload	16
	iadd
	istore	35
	iload_1
	iload	17
	iadd
	istore	35
	iload_1
	iload	18
	iadd
	istore	35
	iload_1
	iload	19
	iadd
	istore	35
	iload_1
	iload	20
	iadd
	istore	35
	iload_1
	i2l
	iload	21
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	ladd
	lstore	37
	iload_1
	i2l
	lload	22
	ladd
	lstore	37
	iload_1
	iload	26
	iadd
	istore	35
	iload_1
	i2f
	fload	27
	fadd
	fstore	42
	iload_1
	i2d
	dload	28
	dadd
	dstore	43
	iload_1
	invokestatic	"System/Decimal" "op_Implicit__V" "(B)LSystem/Decimal;"
	aload	30
	invokestatic	"System/Decimal" "op_Addition__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	iload_2
	iload	16
	isub
	istore	35
	iload_2
	iload	17
	isub
	istore	35
	iload_2
	iload	18
	isub
	istore	35
	iload_2
	iload	19
	isub
	istore	35
	iload_2
	iload	20
	isub
	istore	35
	iload_2
	iload	21
	isub
	istore	36
	iload_2
	i2l
	lload	22
	lsub
	lstore	37
	iload_2
	i2l
	lload	24
	lsub
	lstore	39
	iload_2
	iload	26
	isub
	istore	35
	iload_2
	i2f
	fload	27
	fsub
	fstore	42
	iload_2
	i2d
	dload	28
	dsub
	dstore	43
	iload_2
	invokestatic	"System/Decimal" "op_Implicit__BV" "(I)LSystem/Decimal;"
	aload	30
	invokestatic	"System/Decimal" "op_Subtraction__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	iload_3
	iload	16
	imul
	istore	35
	iload_3
	iload	17
	imul
	istore	35
	iload_3
	iload	18
	imul
	istore	35
	iload_3
	iload	19
	imul
	istore	35
	iload_3
	iload	20
	imul
	istore	35
	iload_3
	i2l
	iload	21
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	lmul
	lstore	37
	iload_3
	i2l
	lload	22
	lmul
	lstore	37
	iload_3
	iload	26
	imul
	istore	35
	iload_3
	i2f
	fload	27
	fmul
	fstore	42
	iload_3
	i2d
	dload	28
	dmul
	dstore	43
	iload_3
	invokestatic	"System/Decimal" "op_Implicit__V" "(S)LSystem/Decimal;"
	aload	30
	invokestatic	"System/Decimal" "op_Multiply__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	iload	4
	iload	16
	idiv
	istore	35
	iload	4
	iload	17
	idiv
	istore	35
	iload	4
	iload	18
	idiv
	istore	35
	iload	4
	iload	19
	idiv
	istore	35
	iload	4
	iload	20
	idiv
	istore	35
	iload	4
	iload	21
	invokestatic	"System/Intrinsics/Operations" "uidiv" "(II)I"
	istore	36
	iload	4
	i2l
	lload	22
	ldiv
	lstore	37
	iload	4
	i2l
	lload	24
	invokestatic	"System/Intrinsics/Operations" "uldiv" "(JJ)J"
	lstore	39
	iload	4
	iload	26
	idiv
	istore	35
	iload	4
	i2f
	fload	27
	fdiv
	fstore	42
	iload	4
	i2d
	dload	28
	ddiv
	dstore	43
	iload	4
	invokestatic	"System/Decimal" "op_Implicit__SV" "(I)LSystem/Decimal;"
	aload	30
	invokestatic	"System/Decimal" "op_Division__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	iload	5
	iload	16
	irem
	istore	35
	iload	5
	iload	17
	irem
	istore	35
	iload	5
	iload	18
	irem
	istore	35
	iload	5
	iload	19
	irem
	istore	35
	iload	5
	iload	20
	irem
	istore	35
	iload	5
	i2l
	iload	21
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	lrem
	lstore	37
	iload	5
	i2l
	lload	22
	lrem
	lstore	37
	iload	5
	iload	26
	irem
	istore	35
	iload	5
	i2f
	fload	27
	frem
	fstore	42
	iload	5
	i2d
	dload	28
	drem
	dstore	43
	iload	5
	invokestatic	"System/Decimal" "op_Implicit__iV" "(I)LSystem/Decimal;"
	aload	30
	invokestatic	"System/Decimal" "op_Modulus__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	iload	16
	i2l
	ladd
	lstore	37
	iload	6
	iload	17
	iadd
	istore	36
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	iload	18
	i2l
	ladd
	lstore	37
	iload	6
	iload	19
	iadd
	istore	36
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	iload	20
	i2l
	ladd
	lstore	37
	iload	6
	iload	21
	iadd
	istore	36
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	lload	22
	ladd
	lstore	37
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	lload	24
	ladd
	lstore	39
	iload	6
	iload	26
	iadd
	istore	36
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	l2f
	fload	27
	fadd
	fstore	42
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	l2d
	dload	28
	dadd
	dstore	43
	iload	6
	invokestatic	"System/Decimal" "op_Implicit__IV" "(I)LSystem/Decimal;"
	aload	30
	invokestatic	"System/Decimal" "op_Addition__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	lload	7
	iload	16
	i2l
	lsub
	lstore	37
	lload	7
	iload	17
	i2l
	lsub
	lstore	37
	lload	7
	iload	18
	i2l
	lsub
	lstore	37
	lload	7
	iload	19
	i2l
	lsub
	lstore	37
	lload	7
	iload	20
	i2l
	lsub
	lstore	37
	lload	7
	iload	21
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	lsub
	lstore	37
	lload	7
	lload	22
	lsub
	lstore	37
	lload	7
	iload	26
	i2l
	lsub
	lstore	37
	lload	7
	l2f
	fload	27
	fsub
	fstore	42
	lload	7
	l2d
	dload	28
	dsub
	dstore	43
	lload	7
	invokestatic	"System/Decimal" "op_Implicit__lV" "(J)LSystem/Decimal;"
	aload	30
	invokestatic	"System/Decimal" "op_Subtraction__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	lload	9
	iload	17
	i2l
	lmul
	lstore	39
	lload	9
	iload	19
	i2l
	lmul
	lstore	39
	lload	9
	iload	21
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	lmul
	lstore	39
	lload	9
	lload	24
	lmul
	lstore	39
	lload	9
	iload	26
	i2l
	lmul
	lstore	39
	lload	9
	invokestatic	"System/Intrinsics/Operations" "ul2f" "(J)F"
	fload	27
	fmul
	fstore	42
	lload	9
	invokestatic	"System/Intrinsics/Operations" "ul2d" "(J)D"
	dload	28
	dmul
	dstore	43
	lload	9
	invokestatic	"System/Decimal" "op_Implicit__LV" "(J)LSystem/Decimal;"
	aload	30
	invokestatic	"System/Decimal" "op_Multiply__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	fload	12
	iload	16
	i2f
	fdiv
	fstore	42
	fload	12
	iload	17
	i2f
	fdiv
	fstore	42
	fload	12
	iload	18
	i2f
	fdiv
	fstore	42
	fload	12
	iload	19
	i2f
	fdiv
	fstore	42
	fload	12
	iload	20
	i2f
	fdiv
	fstore	42
	fload	12
	iload	21
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	l2f
	fdiv
	fstore	42
	fload	12
	lload	22
	l2f
	fdiv
	fstore	42
	fload	12
	lload	24
	invokestatic	"System/Intrinsics/Operations" "ul2f" "(J)F"
	fdiv
	fstore	42
	fload	12
	iload	26
	i2f
	fdiv
	fstore	42
	fload	12
	fload	27
	fdiv
	fstore	42
	fload	12
	f2d
	dload	28
	ddiv
	dstore	43
	dload	13
	iload	16
	i2d
	drem
	dstore	43
	dload	13
	iload	17
	i2d
	drem
	dstore	43
	dload	13
	iload	18
	i2d
	drem
	dstore	43
	dload	13
	iload	19
	i2d
	drem
	dstore	43
	dload	13
	iload	20
	i2d
	drem
	dstore	43
	dload	13
	iload	21
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	l2d
	drem
	dstore	43
	dload	13
	lload	22
	l2d
	drem
	dstore	43
	dload	13
	lload	24
	invokestatic	"System/Intrinsics/Operations" "ul2d" "(J)D"
	drem
	dstore	43
	dload	13
	iload	26
	i2d
	drem
	dstore	43
	dload	13
	fload	27
	f2d
	drem
	dstore	43
	dload	13
	dload	28
	drem
	dstore	43
	aload	15
	iload	16
	invokestatic	"System/Decimal" "op_Implicit__V" "(B)LSystem/Decimal;"
	invokestatic	"System/Decimal" "op_Addition__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	aload	15
	iload	17
	invokestatic	"System/Decimal" "op_Implicit__BV" "(I)LSystem/Decimal;"
	invokestatic	"System/Decimal" "op_Addition__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	aload	15
	iload	18
	invokestatic	"System/Decimal" "op_Implicit__V" "(S)LSystem/Decimal;"
	invokestatic	"System/Decimal" "op_Addition__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	aload	15
	iload	19
	invokestatic	"System/Decimal" "op_Implicit__SV" "(I)LSystem/Decimal;"
	invokestatic	"System/Decimal" "op_Addition__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	aload	15
	iload	20
	invokestatic	"System/Decimal" "op_Implicit__iV" "(I)LSystem/Decimal;"
	invokestatic	"System/Decimal" "op_Addition__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	aload	15
	iload	21
	invokestatic	"System/Decimal" "op_Implicit__IV" "(I)LSystem/Decimal;"
	invokestatic	"System/Decimal" "op_Addition__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	aload	15
	lload	22
	invokestatic	"System/Decimal" "op_Implicit__lV" "(J)LSystem/Decimal;"
	invokestatic	"System/Decimal" "op_Addition__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	aload	15
	lload	24
	invokestatic	"System/Decimal" "op_Implicit__LV" "(J)LSystem/Decimal;"
	invokestatic	"System/Decimal" "op_Addition__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	aload	15
	iload	26
	invokestatic	"System/Decimal" "op_Implicit__V" "(C)LSystem/Decimal;"
	invokestatic	"System/Decimal" "op_Addition__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	aload	15
	aload	30
	invokestatic	"System/Decimal" "op_Addition__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	astore	45
	return
	.locals 46
	.maxstack 7
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
