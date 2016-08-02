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
	astore	12
	aconst_null
	astore_1
	iconst_1
	istore_2
	iconst_0
	istore_2
	bipush	-128
	istore_3
	bipush	-2
	istore_3
	iconst_m1
	istore_3
	iconst_0
	istore_3
	iconst_1
	istore_3
	iconst_2
	istore_3
	iconst_3
	istore_3
	iconst_4
	istore_3
	iconst_5
	istore_3
	bipush	6
	istore_3
	bipush	7
	istore_3
	bipush	8
	istore_3
	bipush	9
	istore_3
	bipush	127
	istore_3
	ldc	int32(-2147483648)
	istore_3
	sipush	-129
	istore_3
	sipush	128
	istore_3
	ldc	int32(2147483647)
	istore_3
	iconst_0
	istore	4
	iconst_1
	istore	4
	iconst_2
	istore	4
	iconst_3
	istore	4
	iconst_4
	istore	4
	iconst_5
	istore	4
	bipush	6
	istore	4
	bipush	7
	istore	4
	bipush	8
	istore	4
	bipush	9
	istore	4
	bipush	127
	istore	4
	sipush	128
	istore	4
	ldc	int32(2147483647)
	istore	4
	ldc	int32(-2147483648)
	istore	4
	iconst_m1
	istore	4
	bipush	-128
	i2l
	lstore	5
	bipush	-2
	i2l
	lstore	5
	iconst_m1
	i2l
	lstore	5
	iconst_0
	i2l
	lstore	5
	iconst_1
	i2l
	lstore	5
	iconst_2
	i2l
	lstore	5
	iconst_3
	i2l
	lstore	5
	iconst_4
	i2l
	lstore	5
	iconst_5
	i2l
	lstore	5
	bipush	6
	i2l
	lstore	5
	bipush	7
	i2l
	lstore	5
	bipush	8
	i2l
	lstore	5
	bipush	9
	i2l
	lstore	5
	bipush	127
	i2l
	lstore	5
	ldc	int32(-2147483648)
	i2l
	lstore	5
	sipush	-129
	i2l
	lstore	5
	sipush	128
	i2l
	lstore	5
	ldc	int32(2147483647)
	i2l
	lstore	5
	ldc	int32(-2147483648)
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	lstore	5
	iconst_m1
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	lstore	5
	ldc2_w	int64(0x0000000100000000)
	lstore	5
	ldc2_w	int64(0x7FFFFFFFFFFFFFFF)
	lstore	5
	ldc2_w	int64(0x8000000000000000)
	lstore	5
	iconst_0
	i2l
	lstore	7
	iconst_1
	i2l
	lstore	7
	iconst_2
	i2l
	lstore	7
	iconst_3
	i2l
	lstore	7
	iconst_4
	i2l
	lstore	7
	iconst_5
	i2l
	lstore	7
	bipush	6
	i2l
	lstore	7
	bipush	7
	i2l
	lstore	7
	bipush	8
	i2l
	lstore	7
	bipush	9
	i2l
	lstore	7
	bipush	127
	i2l
	lstore	7
	sipush	128
	i2l
	lstore	7
	ldc	int32(2147483647)
	i2l
	lstore	7
	ldc	int32(-2147483648)
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	lstore	7
	iconst_m1
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	lstore	7
	ldc2_w	int64(0x0000000100000000)
	lstore	7
	ldc2_w	int64(0x7FFFFFFFFFFFFFFF)
	lstore	7
	ldc2_w	int64(0x8000000000000000)
	lstore	7
	bipush	-128
	i2l
	lstore	7
	iconst_m1
	i2l
	lstore	7
	fconst_0
	fstore	9
	fconst_1
	fstore	9
	ldc	float32(0xBF800000)
	fstore	9
	ldc	float32(0x78181A2D)
	fstore	9
	ldc	float32(0x7F7FFFFF)
	fstore	9
	ldc	float32(0xFF7FFFFF)
	fstore	9
	ldc	float32(0x00000001)
	fstore	9
	bipush	-128
	i2f
	fstore	9
	iconst_m1
	i2f
	fstore	9
	iconst_0
	i2f
	fstore	9
	iconst_1
	i2f
	fstore	9
	bipush	127
	i2f
	fstore	9
	ldc	int32(-2147483648)
	i2f
	fstore	9
	ldc	int32(2147483647)
	i2f
	fstore	9
	iconst_m1
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	l2f
	fstore	9
	ldc2_w	int64(0x0000000100000000)
	l2f
	fstore	9
	ldc2_w	int64(0x7FFFFFFFFFFFFFFF)
	l2f
	fstore	9
	ldc2_w	int64(0x8000000000000000)
	l2f
	fstore	9
	ldc2_w	int64(0x8000000000000000)
	invokestatic	"System/Intrinsics/Operations" "ul2f" "(J)F"
	fstore	9
	bipush	-128
	i2l
	invokestatic	"System/Intrinsics/Operations" "ul2f" "(J)F"
	fstore	9
	iconst_m1
	i2l
	invokestatic	"System/Intrinsics/Operations" "ul2f" "(J)F"
	fstore	9
	fconst_0
	f2d
	dstore	10
	fconst_1
	f2d
	dstore	10
	ldc	float32(0xBF800000)
	f2d
	dstore	10
	ldc	float32(0x78181A2D)
	f2d
	dstore	10
	ldc	float32(0x7F7FFFFF)
	f2d
	dstore	10
	ldc	float32(0xFF7FFFFF)
	f2d
	dstore	10
	ldc	float32(0x00000001)
	f2d
	dstore	10
	dconst_0
	dstore	10
	dconst_1
	dstore	10
	ldc2_w	float64(0xBFF0000000000000)
	dstore	10
	ldc2_w	float64(0x4703034593B73C22)
	dstore	10
	ldc2_w	float64(0x7FEFFFFFFFFFFFFF)
	dstore	10
	ldc2_w	float64(0xFFEFFFFFFFFFFFFF)
	dstore	10
	ldc2_w	float64(0x0000000000000001)
	dstore	10
	dconst_0
	dstore	10
	dconst_1
	dstore	10
	ldc2_w	float64(0xBFF0000000000000)
	dstore	10
	ldc2_w	float64(0x4703034593B73C22)
	dstore	10
	ldc2_w	float64(0x7FEFFFFFFFFFFFFF)
	dstore	10
	ldc2_w	float64(0xFFEFFFFFFFFFFFFF)
	dstore	10
	ldc2_w	float64(0x0000000000000001)
	dstore	10
	bipush	-128
	i2d
	dstore	10
	iconst_m1
	i2d
	dstore	10
	iconst_0
	i2d
	dstore	10
	iconst_1
	i2d
	dstore	10
	bipush	127
	i2d
	dstore	10
	ldc	int32(-2147483648)
	i2d
	dstore	10
	ldc	int32(2147483647)
	i2d
	dstore	10
	iconst_m1
	i2l
	ldc2_w	int64(0x00000000FFFFFFFF)
	land
	l2d
	dstore	10
	ldc2_w	int64(0x0000000100000000)
	l2d
	dstore	10
	ldc2_w	int64(0x7FFFFFFFFFFFFFFF)
	l2d
	dstore	10
	ldc2_w	int64(0x8000000000000000)
	l2d
	dstore	10
	ldc2_w	int64(0x8000000000000000)
	invokestatic	"System/Intrinsics/Operations" "ul2d" "(J)D"
	dstore	10
	bipush	-128
	i2l
	invokestatic	"System/Intrinsics/Operations" "ul2d" "(J)D"
	dstore	10
	iconst_m1
	i2l
	invokestatic	"System/Intrinsics/Operations" "ul2d" "(J)D"
	dstore	10
	iconst_0
	invokestatic	"System/Decimal" "op_Implicit__iV" "(I)LSystem/Decimal;"
	astore	12
	iconst_1
	invokestatic	"System/Decimal" "op_Implicit__iV" "(I)LSystem/Decimal;"
	astore	12
	iconst_m1
	invokestatic	"System/Decimal" "op_Implicit__iV" "(I)LSystem/Decimal;"
	astore	12
	ldc	int32(2147483647)
	invokestatic	"System/Decimal" "op_Implicit__iV" "(I)LSystem/Decimal;"
	astore	12
	ldc	int32(-2147483648)
	invokestatic	"System/Decimal" "op_Implicit__iV" "(I)LSystem/Decimal;"
	astore	12
	ldc	int32(-2147483648)
	invokestatic	"System/Decimal" "op_Implicit__IV" "(I)LSystem/Decimal;"
	astore	12
	iconst_m1
	invokestatic	"System/Decimal" "op_Implicit__IV" "(I)LSystem/Decimal;"
	astore	12
	ldc2_w	int64(0x0000000100000000)
	invokestatic	"System/Decimal" "op_Implicit__lV" "(J)LSystem/Decimal;"
	astore	12
	ldc2_w	int64(0xFFFFFFFF00000001)
	invokestatic	"System/Decimal" "op_Implicit__lV" "(J)LSystem/Decimal;"
	astore	12
	ldc2_w	int64(0x7FFFFFFFFFFFFFFF)
	invokestatic	"System/Decimal" "op_Implicit__lV" "(J)LSystem/Decimal;"
	astore	12
	ldc2_w	int64(0x8000000000000000)
	invokestatic	"System/Decimal" "op_Implicit__lV" "(J)LSystem/Decimal;"
	astore	12
	new	"System/Decimal"
	dup
	iconst_0
	ldc	int32(-2147483648)
	iconst_0
	iconst_0
	iconst_0
	invokespecial	"System/Decimal" "<init>__iiiB" "(IIIZI)"
	astore	12
	new	"System/Decimal"
	dup
	iconst_m1
	iconst_m1
	iconst_0
	iconst_0
	iconst_0
	invokespecial	"System/Decimal" "<init>__iiiB" "(IIIZI)"
	astore	12
	new	"System/Decimal"
	dup
	iconst_m1
	iconst_m1
	iconst_m1
	iconst_0
	iconst_0
	invokespecial	"System/Decimal" "<init>__iiiB" "(IIIZI)"
	astore	12
	new	"System/Decimal"
	dup
	iconst_m1
	iconst_m1
	iconst_m1
	iconst_0
	bipush	28
	invokespecial	"System/Decimal" "<init>__iiiB" "(IIIZI)"
	astore	12
	new	"System/Decimal"
	dup
	iconst_m1
	iconst_m1
	iconst_m1
	iconst_0
	bipush	28
	invokespecial	"System/Decimal" "<init>__iiiB" "(IIIZI)"
	astore	12
	new	"System/Decimal"
	dup
	ldc	int32(-1717986918)
	ldc	int32(-1717986919)
	ldc	int32(429496729)
	iconst_0
	bipush	27
	invokespecial	"System/Decimal" "<init>__iiiB" "(IIIZI)"
	astore	12
	new	"System/Decimal"
	dup
	ldc	int32(-1744830464)
	ldc	int32(1364693707)
	ldc	int32(-12368715)
	iconst_0
	iconst_0
	invokespecial	"System/Decimal" "<init>__iiiB" "(IIIZI)"
	astore	12
	new	"System/Decimal"
	dup
	ldc	int32(-1744830464)
	ldc	int32(1364693707)
	ldc	int32(-12368715)
	iconst_0
	iconst_0
	invokespecial	"System/Decimal" "<init>__iiiB" "(IIIZI)"
	astore	12
	new	"System/Decimal"
	dup
	iconst_1
	iconst_0
	iconst_0
	iconst_0
	bipush	28
	invokespecial	"System/Decimal" "<init>__iiiB" "(IIIZI)"
	astore	12
	iconst_0
	invokestatic	"System/Decimal" "op_Implicit__iV" "(I)LSystem/Decimal;"
	astore	12
	ldc	"Hello World!"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	astore	13
	ldc	""
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	astore	13
	aconst_null
	astore	13
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
