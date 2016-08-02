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
	bipush	34
	i2b
	istore	12
	bipush	42
	sipush	255
	iand
	istore	13
	bipush	-126
	i2s
	istore	14
	bipush	67
	i2c
	istore	15
	sipush	-1234
	istore	16
	ldc	int32(54321)
	istore	17
	ldc2_w	int64(0xFFFFFFFDB34FE916)
	lstore	18
	ldc2_w	int64(0x000000024CB016EA)
	lstore	20
	bipush	65
	istore	22
	iload_1
	iload	16
	ishl
	istore	27
	iload_2
	iload	16
	ishl
	istore	27
	iload_3
	iload	16
	ishl
	istore	27
	iload	4
	iload	16
	ishl
	istore	27
	iload	5
	iload	16
	ishl
	istore	27
	iload	6
	iload	16
	ishl
	istore	28
	lload	7
	iload	16
	lshl
	lstore	29
	lload	9
	iload	16
	lshl
	lstore	31
	iload	11
	iload	16
	ishl
	istore	27
	iload_1
	iload	16
	ishr
	istore	27
	iload_2
	iload	16
	ishr
	istore	27
	iload_3
	iload	16
	ishr
	istore	27
	iload	4
	iload	16
	ishr
	istore	27
	iload	5
	iload	16
	ishr
	istore	27
	iload	6
	iload	16
	iushr
	istore	28
	lload	7
	iload	16
	lshr
	lstore	29
	lload	9
	iload	16
	lushr
	lstore	31
	iload	11
	iload	16
	ishr
	istore	27
	iload	16
	iload_1
	ishl
	istore	27
	iload	16
	iload_2
	ishr
	istore	27
	iload	16
	iload_3
	ishl
	istore	27
	iload	16
	iload	4
	ishr
	istore	27
	iload	16
	iload	11
	ishl
	istore	27
	return
	.locals 34
	.maxstack 3
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
