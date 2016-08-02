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
.class private auto sealed serializable ansi 'Size' extends ['.library']'System'.'Enum'
{
.field public static literal valuetype 'Size' 'Small' = int64(0x0000000000000000)
.field public static literal valuetype 'Size' 'Large' = int64(0x0000000000000001)
.field public specialname rtspecialname int64 'value__'
} // class Size
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private hidebysig instance void 'm1'() cil managed java 
{
	iconst_0
	istore_1
	iconst_1
	istore_2
	lconst_0
	lstore	4
	lconst_1
	lstore	6
	iload_1
	iconst_m1
	ixor
	istore_3
	lload	4
	iconst_m1
	i2l
	lxor
	lstore	8
	iload_1
	iconst_1
	iadd
	istore_3
	iconst_1
	iload_2
	iadd
	istore_3
	lload	4
	iconst_1
	i2l
	ladd
	lstore	8
	iconst_1
	i2l
	lload	6
	ladd
	lstore	8
	iload_1
	iconst_1
	isub
	istore_3
	iload_1
	iload_2
	isub
	istore	10
	lload	4
	iconst_1
	i2l
	lsub
	lstore	8
	lload	4
	lload	6
	lsub
	lstore	11
	iload_1
	iload_2
	if_icmpne	?L1
	iconst_1
	goto	?L2
?L1:
	iconst_0
?L2:
	istore	13
	iload_1
	iload_2
	if_icmpeq	?L3
	iconst_1
	goto	?L4
?L3:
	iconst_0
?L4:
	istore	13
	iload_1
	iload_2
	if_icmplt	?L5
	iconst_1
	goto	?L6
?L5:
	iconst_0
?L6:
	istore	13
	iload_1
	iload_2
	if_icmpgt	?L7
	iconst_1
	goto	?L8
?L7:
	iconst_0
?L8:
	istore	13
	iload_1
	iload_2
	if_icmple	?L9
	iconst_1
	goto	?L10
?L9:
	iconst_0
?L10:
	istore	13
	iload_1
	iload_2
	if_icmpge	?L11
	iconst_1
	goto	?L12
?L11:
	iconst_0
?L12:
	istore	13
	lload	4
	lload	6
	lcmp
	ifne	?L13
	iconst_1
	goto	?L14
?L13:
	iconst_0
?L14:
	istore	13
	lload	4
	lload	6
	lcmp
	ifeq	?L15
	iconst_1
	goto	?L16
?L15:
	iconst_0
?L16:
	istore	13
	lload	4
	lload	6
	lcmp
	iflt	?L17
	iconst_1
	goto	?L18
?L17:
	iconst_0
?L18:
	istore	13
	lload	4
	lload	6
	lcmp
	ifgt	?L19
	iconst_1
	goto	?L20
?L19:
	iconst_0
?L20:
	istore	13
	lload	4
	lload	6
	lcmp
	ifle	?L21
	iconst_1
	goto	?L22
?L21:
	iconst_0
?L22:
	istore	13
	lload	4
	lload	6
	lcmp
	ifge	?L23
	iconst_1
	goto	?L24
?L23:
	iconst_0
?L24:
	istore	13
	iload_1
	iload_2
	iand
	istore_3
	lload	4
	lload	6
	land
	lstore	8
	iload_1
	iload_2
	ior
	istore_3
	lload	4
	lload	6
	lor
	lstore	8
	iload_1
	iload_2
	ixor
	istore_3
	lload	4
	lload	6
	lxor
	lstore	8
	return
	.locals 14
	.maxstack 4
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
