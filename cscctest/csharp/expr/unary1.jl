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
	invokestatic	"System/Intrinsics/Operations" "ineg_ovf" "(I)I"
	istore	20
	iload_2
	invokestatic	"System/Intrinsics/Operations" "ineg_ovf" "(I)I"
	istore	20
	iload_3
	invokestatic	"System/Intrinsics/Operations" "ineg_ovf" "(I)I"
	istore	20
	iload	4
	invokestatic	"System/Intrinsics/Operations" "ineg_ovf" "(I)I"
	istore	20
	iload	5
	invokestatic	"System/Intrinsics/Operations" "ineg_ovf" "(I)I"
	istore	20
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	invokestatic	"System/Intrinsics/Operations" "lneg_ovf" "(J)J"
	lstore	22
	lload	7
	invokestatic	"System/Intrinsics/Operations" "lneg_ovf" "(J)J"
	lstore	22
	iload	11
	invokestatic	"System/Intrinsics/Operations" "ineg_ovf" "(I)I"
	istore	20
	fload	12
	fneg
	fstore	27
	dload	13
	dneg
	dstore	28
	aload	15
	invokestatic	"System/Decimal" "op_UnaryNegation__VV" "(LSystem/Decimal;)LSystem/Decimal;"
	astore	30
	iload_1
	ineg
	istore	20
	iload_2
	ineg
	istore	20
	iload_3
	ineg
	istore	20
	iload	4
	ineg
	istore	20
	iload	5
	ineg
	istore	20
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	lneg
	lstore	22
	lload	7
	lneg
	lstore	22
	iload	11
	ineg
	istore	20
	fload	12
	fneg
	fstore	27
	dload	13
	dneg
	dstore	28
	aload	15
	invokestatic	"System/Decimal" "op_UnaryNegation__VV" "(LSystem/Decimal;)LSystem/Decimal;"
	astore	30
	iload_1
	iconst_m1
	ixor
	istore	20
	iload_2
	iconst_m1
	ixor
	istore	20
	iload_3
	iconst_m1
	ixor
	istore	20
	iload	4
	iconst_m1
	ixor
	istore	20
	iload	5
	iconst_m1
	ixor
	istore	20
	iload	6
	iconst_m1
	ixor
	istore	21
	lload	7
	iconst_m1
	i2l
	lxor
	lstore	22
	lload	9
	iconst_m1
	i2l
	lxor
	lstore	24
	iload	11
	iconst_m1
	ixor
	istore	20
	return
	.locals 31
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
