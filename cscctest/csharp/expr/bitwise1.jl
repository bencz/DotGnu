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
	iload	12
	iand
	istore	27
	iload_1
	iload	13
	iand
	istore	27
	iload_1
	iload	14
	iand
	istore	27
	iload_1
	iload	15
	iand
	istore	27
	iload_1
	iload	16
	iand
	istore	27
	iload_1
	i2l
	iload	17
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	land
	lstore	29
	iload_1
	i2l
	lload	18
	land
	lstore	29
	iload_1
	iload	22
	iand
	istore	27
	iload_2
	iload	12
	ior
	istore	27
	iload_2
	iload	13
	ior
	istore	27
	iload_2
	iload	14
	ior
	istore	27
	iload_2
	iload	15
	ior
	istore	27
	iload_2
	iload	16
	ior
	istore	27
	iload_2
	iload	17
	ior
	istore	28
	iload_2
	i2l
	lload	18
	lor
	lstore	29
	iload_2
	i2l
	lload	20
	lor
	lstore	31
	iload_2
	iload	22
	ior
	istore	27
	iload_3
	iload	12
	ixor
	istore	27
	iload_3
	iload	13
	ixor
	istore	27
	iload_3
	iload	14
	ixor
	istore	27
	iload_3
	iload	15
	ixor
	istore	27
	iload_3
	iload	16
	ixor
	istore	27
	iload_3
	i2l
	iload	17
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	lxor
	lstore	29
	iload_3
	i2l
	lload	18
	lxor
	lstore	29
	iload_3
	iload	22
	ixor
	istore	27
	iload	4
	iload	12
	iand
	istore	27
	iload	4
	iload	13
	iand
	istore	27
	iload	4
	iload	14
	iand
	istore	27
	iload	4
	iload	15
	iand
	istore	27
	iload	4
	iload	16
	iand
	istore	27
	iload	4
	iload	17
	iand
	istore	28
	iload	4
	i2l
	lload	18
	land
	lstore	29
	iload	4
	i2l
	lload	20
	land
	lstore	31
	iload	4
	iload	22
	iand
	istore	27
	iload	5
	iload	12
	ior
	istore	27
	iload	5
	iload	13
	ior
	istore	27
	iload	5
	iload	14
	ior
	istore	27
	iload	5
	iload	15
	ior
	istore	27
	iload	5
	iload	16
	ior
	istore	27
	iload	5
	i2l
	iload	17
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	lor
	lstore	29
	iload	5
	i2l
	lload	18
	lor
	lstore	29
	iload	5
	iload	22
	ior
	istore	27
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	iload	12
	i2l
	lxor
	lstore	29
	iload	6
	iload	13
	ixor
	istore	28
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	iload	14
	i2l
	lxor
	lstore	29
	iload	6
	iload	15
	ixor
	istore	28
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	iload	16
	i2l
	lxor
	lstore	29
	iload	6
	iload	17
	ixor
	istore	28
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	lload	18
	lxor
	lstore	29
	iload	6
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	lload	20
	lxor
	lstore	31
	iload	6
	iload	22
	ixor
	istore	28
	lload	7
	iload	12
	i2l
	land
	lstore	29
	lload	7
	iload	13
	i2l
	land
	lstore	29
	lload	7
	iload	14
	i2l
	land
	lstore	29
	lload	7
	iload	15
	i2l
	land
	lstore	29
	lload	7
	iload	16
	i2l
	land
	lstore	29
	lload	7
	iload	17
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	land
	lstore	29
	lload	7
	lload	18
	land
	lstore	29
	lload	7
	iload	22
	i2l
	land
	lstore	29
	lload	9
	iload	13
	i2l
	lor
	lstore	31
	lload	9
	iload	15
	i2l
	lor
	lstore	31
	lload	9
	iload	17
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	lor
	lstore	31
	lload	9
	lload	20
	lor
	lstore	31
	lload	9
	iload	22
	i2l
	lor
	lstore	31
	return
	.locals 34
	.maxstack 6
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
