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
.class public sequential sealed serializable ansi 'X' extends ['.library']'System'.'ValueType'
{
.size 1
.method public static hidebysig specialname valuetype 'X' 'op_Increment'(valuetype 'X' 'x') cil managed java 
{
	aload_0
	areturn
	.locals 1
	.maxstack 1
} // method op_Increment
.method public static hidebysig specialname valuetype 'X' 'op_Decrement'(valuetype 'X' 'x') cil managed java 
{
	aload_0
	areturn
	.locals 1
	.maxstack 1
} // method op_Decrement
} // class X
.class public auto ansi 'Y' extends ['.library']'System'.'Object'
{
.method public static hidebysig specialname class 'Y' 'op_Increment'(class 'Y' 'y') cil managed java 
{
	aload_0
	areturn
	.locals 1
	.maxstack 1
} // method op_Increment
.method public static hidebysig specialname class 'Y' 'op_Decrement'(class 'Y' 'y') cil managed java 
{
	aload_0
	areturn
	.locals 1
	.maxstack 1
} // method op_Decrement
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Y
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private hidebysig instance void 'm1'() cil managed java 
{
	new	"System/Decimal"
	dup
	invokespecial	"System/Decimal" "<init>" "()V"
	astore	15
	new	"X"
	dup
	invokespecial	"X" "<init>" "()V"
	astore	18
	new	"System/Decimal"
	dup
	invokespecial	"System/Decimal" "<init>" "()V"
	astore	34
	new	"X"
	dup
	invokespecial	"X" "<init>" "()V"
	astore	37
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
	iconst_0
	istore	16
	iconst_0
	istore	17
	aconst_null
	astore	19
	iload_1
	iconst_1
	iadd
	i2b
	istore_1
	iload_2
	iconst_1
	iadd
	sipush	255
	iand
	istore_2
	iload_3
	iconst_1
	iadd
	i2s
	istore_3
	iload	4
	iconst_1
	iadd
	i2c
	istore	4
	iload	5
	iconst_1
	iadd
	istore	5
	iload	6
	iconst_1
	iadd
	istore	6
	lload	7
	lconst_1
	ladd
	lstore	7
	lload	9
	lconst_1
	ladd
	lstore	9
	iload	11
	iconst_1
	iadd
	i2c
	istore	11
	fload	12
	fconst_1
	fadd
	fstore	12
	dload	13
	dconst_1
	dadd
	dstore	13
	aload	15
	invokestatic	"System/Decimal" "op_Increment__VV" "(LSystem/Decimal;)LSystem/Decimal;"
	astore	15
	iload	16
	iconst_1
	iadd
	istore	16
	iload	17
	iconst_1
	iadd
	i2b
	istore	17
	aload	18
	invokestatic	valuetype 'X' 'X'::'op_Increment'(valuetype 'X')
	astore	18
	aload	19
	invokestatic	class 'Y' 'Y'::'op_Increment'(class 'Y')
	astore	19
	iload_1
	iconst_1
	isub
	i2b
	istore_1
	iload_2
	iconst_1
	isub
	sipush	255
	iand
	istore_2
	iload_3
	iconst_1
	isub
	i2s
	istore_3
	iload	4
	iconst_1
	isub
	i2c
	istore	4
	iload	5
	iconst_1
	isub
	istore	5
	iload	6
	iconst_1
	isub
	istore	6
	lload	7
	lconst_1
	lsub
	lstore	7
	lload	9
	lconst_1
	lsub
	lstore	9
	iload	11
	iconst_1
	isub
	i2c
	istore	11
	fload	12
	fconst_1
	fsub
	fstore	12
	dload	13
	dconst_1
	dsub
	dstore	13
	aload	15
	invokestatic	"System/Decimal" "op_Decrement__VV" "(LSystem/Decimal;)LSystem/Decimal;"
	astore	15
	iload	16
	iconst_1
	isub
	istore	16
	iload	17
	iconst_1
	isub
	i2b
	istore	17
	aload	18
	invokestatic	valuetype 'X' 'X'::'op_Decrement'(valuetype 'X')
	astore	18
	aload	19
	invokestatic	class 'Y' 'Y'::'op_Decrement'(class 'Y')
	astore	19
	iload_1
	iconst_1
	iadd
	i2b
	istore_1
	iload_2
	iconst_1
	iadd
	sipush	255
	iand
	istore_2
	iload_3
	iconst_1
	iadd
	i2s
	istore_3
	iload	4
	iconst_1
	iadd
	i2c
	istore	4
	iload	5
	iconst_1
	iadd
	istore	5
	iload	6
	iconst_1
	iadd
	istore	6
	lload	7
	lconst_1
	ladd
	lstore	7
	lload	9
	lconst_1
	ladd
	lstore	9
	iload	11
	iconst_1
	iadd
	i2c
	istore	11
	fload	12
	fconst_1
	fadd
	fstore	12
	dload	13
	dconst_1
	dadd
	dstore	13
	aload	15
	invokestatic	"System/Decimal" "op_Increment__VV" "(LSystem/Decimal;)LSystem/Decimal;"
	astore	15
	iload	16
	iconst_1
	iadd
	istore	16
	iload	17
	iconst_1
	iadd
	i2b
	istore	17
	aload	18
	invokestatic	valuetype 'X' 'X'::'op_Increment'(valuetype 'X')
	astore	18
	aload	19
	invokestatic	class 'Y' 'Y'::'op_Increment'(class 'Y')
	astore	19
	iload_1
	iconst_1
	isub
	i2b
	istore_1
	iload_2
	iconst_1
	isub
	sipush	255
	iand
	istore_2
	iload_3
	iconst_1
	isub
	i2s
	istore_3
	iload	4
	iconst_1
	isub
	i2c
	istore	4
	iload	5
	iconst_1
	isub
	istore	5
	iload	6
	iconst_1
	isub
	istore	6
	lload	7
	lconst_1
	lsub
	lstore	7
	lload	9
	lconst_1
	lsub
	lstore	9
	iload	11
	iconst_1
	isub
	i2c
	istore	11
	fload	12
	fconst_1
	fsub
	fstore	12
	dload	13
	dconst_1
	dsub
	dstore	13
	aload	15
	invokestatic	"System/Decimal" "op_Decrement__VV" "(LSystem/Decimal;)LSystem/Decimal;"
	astore	15
	iload	16
	iconst_1
	isub
	istore	16
	iload	17
	iconst_1
	isub
	i2b
	istore	17
	aload	18
	invokestatic	valuetype 'X' 'X'::'op_Decrement'(valuetype 'X')
	astore	18
	aload	19
	invokestatic	class 'Y' 'Y'::'op_Decrement'(class 'Y')
	astore	19
	iload_1
	iconst_1
	iadd
	i2b
	dup
	istore_1
	istore	20
	iload_2
	iconst_1
	iadd
	sipush	255
	iand
	dup
	istore_2
	istore	21
	iload_3
	iconst_1
	iadd
	i2s
	dup
	istore_3
	istore	22
	iload	4
	iconst_1
	iadd
	i2c
	dup
	istore	4
	istore	23
	iload	5
	iconst_1
	iadd
	dup
	istore	5
	istore	24
	iload	6
	iconst_1
	iadd
	dup
	istore	6
	istore	25
	lload	7
	lconst_1
	ladd
	dup2
	lstore	7
	lstore	26
	lload	9
	lconst_1
	ladd
	dup2
	lstore	9
	lstore	28
	iload	11
	iconst_1
	iadd
	i2c
	dup
	istore	11
	istore	30
	fload	12
	fconst_1
	fadd
	dup
	fstore	12
	fstore	31
	dload	13
	dconst_1
	dadd
	dup2
	dstore	13
	dstore	32
	aload	15
	invokestatic	"System/Decimal" "op_Increment__VV" "(LSystem/Decimal;)LSystem/Decimal;"
	dup
	astore	15
	astore	34
	iload	16
	iconst_1
	iadd
	dup
	istore	16
	istore	35
	iload	17
	iconst_1
	iadd
	i2b
	dup
	istore	17
	istore	36
	aload	18
	invokestatic	valuetype 'X' 'X'::'op_Increment'(valuetype 'X')
	dup
	astore	18
	astore	37
	aload	19
	invokestatic	class 'Y' 'Y'::'op_Increment'(class 'Y')
	dup
	astore	19
	astore	38
	iload_1
	iconst_1
	isub
	i2b
	dup
	istore_1
	istore	20
	iload_2
	iconst_1
	isub
	sipush	255
	iand
	dup
	istore_2
	istore	21
	iload_3
	iconst_1
	isub
	i2s
	dup
	istore_3
	istore	22
	iload	4
	iconst_1
	isub
	i2c
	dup
	istore	4
	istore	23
	iload	5
	iconst_1
	isub
	dup
	istore	5
	istore	24
	iload	6
	iconst_1
	isub
	dup
	istore	6
	istore	25
	lload	7
	lconst_1
	lsub
	dup2
	lstore	7
	lstore	26
	lload	9
	lconst_1
	lsub
	dup2
	lstore	9
	lstore	28
	iload	11
	iconst_1
	isub
	i2c
	dup
	istore	11
	istore	30
	fload	12
	fconst_1
	fsub
	dup
	fstore	12
	fstore	31
	dload	13
	dconst_1
	dsub
	dup2
	dstore	13
	dstore	32
	aload	15
	invokestatic	"System/Decimal" "op_Decrement__VV" "(LSystem/Decimal;)LSystem/Decimal;"
	dup
	astore	15
	astore	34
	iload	16
	iconst_1
	isub
	dup
	istore	16
	istore	35
	iload	17
	iconst_1
	isub
	i2b
	dup
	istore	17
	istore	36
	aload	18
	invokestatic	valuetype 'X' 'X'::'op_Decrement'(valuetype 'X')
	dup
	astore	18
	astore	37
	aload	19
	invokestatic	class 'Y' 'Y'::'op_Decrement'(class 'Y')
	dup
	astore	19
	astore	38
	iload_1
	dup
	iconst_1
	iadd
	i2b
	istore_1
	istore	20
	iload_2
	dup
	iconst_1
	iadd
	sipush	255
	iand
	istore_2
	istore	21
	iload_3
	dup
	iconst_1
	iadd
	i2s
	istore_3
	istore	22
	iload	4
	dup
	iconst_1
	iadd
	i2c
	istore	4
	istore	23
	iload	5
	dup
	iconst_1
	iadd
	istore	5
	istore	24
	iload	6
	dup
	iconst_1
	iadd
	istore	6
	istore	25
	lload	7
	dup2
	lconst_1
	ladd
	lstore	7
	lstore	26
	lload	9
	dup2
	lconst_1
	ladd
	lstore	9
	lstore	28
	iload	11
	dup
	iconst_1
	iadd
	i2c
	istore	11
	istore	30
	fload	12
	dup
	fconst_1
	fadd
	fstore	12
	fstore	31
	dload	13
	dup2
	dconst_1
	dadd
	dstore	13
	dstore	32
	aload	15
	dup
	invokestatic	"System/Decimal" "op_Increment__VV" "(LSystem/Decimal;)LSystem/Decimal;"
	astore	15
	astore	34
	iload	16
	dup
	iconst_1
	iadd
	istore	16
	istore	35
	iload	17
	dup
	iconst_1
	iadd
	i2b
	istore	17
	istore	36
	aload	18
	dup
	invokestatic	valuetype 'X' 'X'::'op_Increment'(valuetype 'X')
	astore	18
	astore	37
	aload	19
	dup
	invokestatic	class 'Y' 'Y'::'op_Increment'(class 'Y')
	astore	19
	astore	38
	iload_1
	dup
	iconst_1
	isub
	i2b
	istore_1
	istore	20
	iload_2
	dup
	iconst_1
	isub
	sipush	255
	iand
	istore_2
	istore	21
	iload_3
	dup
	iconst_1
	isub
	i2s
	istore_3
	istore	22
	iload	4
	dup
	iconst_1
	isub
	i2c
	istore	4
	istore	23
	iload	5
	dup
	iconst_1
	isub
	istore	5
	istore	24
	iload	6
	dup
	iconst_1
	isub
	istore	6
	istore	25
	lload	7
	dup2
	lconst_1
	lsub
	lstore	7
	lstore	26
	lload	9
	dup2
	lconst_1
	lsub
	lstore	9
	lstore	28
	iload	11
	dup
	iconst_1
	isub
	i2c
	istore	11
	istore	30
	fload	12
	dup
	fconst_1
	fsub
	fstore	12
	fstore	31
	dload	13
	dup2
	dconst_1
	dsub
	dstore	13
	dstore	32
	aload	15
	dup
	invokestatic	"System/Decimal" "op_Decrement__VV" "(LSystem/Decimal;)LSystem/Decimal;"
	astore	15
	astore	34
	iload	16
	dup
	iconst_1
	isub
	istore	16
	istore	35
	iload	17
	dup
	iconst_1
	isub
	i2b
	istore	17
	istore	36
	aload	18
	dup
	invokestatic	valuetype 'X' 'X'::'op_Decrement'(valuetype 'X')
	astore	18
	astore	37
	aload	19
	dup
	invokestatic	class 'Y' 'Y'::'op_Decrement'(class 'Y')
	astore	19
	astore	38
	return
	.locals 39
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
