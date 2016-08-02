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
.method private hidebysig instance void 'm1_int'(int32[] 'x', int32 'y') cil managed java 
{
	aload_1
	iload_2
	iaload
	istore_3
	aload_1
	iload_2
	iload_3
	iastore
	aload_1
	dup
	iload_2
	dup_x1
	iaload
	iload_3
	iadd
	iastore
	aload_1
	dup
	iload_2
	dup_x1
	iaload
	iconst_1
	iadd
	iastore
	istore_3
	aload_1
	dup
	iload_2
	dup_x1
	iaload
	iconst_1
	isub
	iastore
	aload_1
	dup
	iload_2
	dup_x1
	iaload
	dup_x2
	iconst_1
	isub
	iastore
	istore_3
	return
	.locals 4
	.maxstack 4
} // method m1_int
.method private hidebysig instance void 'm1_byte'(unsigned int8[] 'x', int32 'y') cil managed java 
{
	aload_1
	iload_2
	baload
	sipush	255
	iand
	istore_3
	aload_1
	iload_2
	iload_3
	bastore
	aload_1
	dup
	iload_2
	dup_x1
	baload
	sipush	255
	iand
	iconst_1
	iadd
	sipush	255
	iand
	bastore
	istore_3
	aload_1
	dup
	iload_2
	dup_x1
	baload
	sipush	255
	iand
	iconst_1
	isub
	sipush	255
	iand
	bastore
	aload_1
	dup
	iload_2
	dup_x1
	baload
	sipush	255
	iand
	dup_x2
	iconst_1
	isub
	sipush	255
	iand
	bastore
	istore_3
	return
	.locals 4
	.maxstack 4
} // method m1_byte
.method private hidebysig instance void 'm1_long'(int64[] 'x', int32 'y') cil managed java 
{
	aload_1
	iload_2
	laload
	lstore_3
	aload_1
	iload_2
	lload_3
	lastore
	aload_1
	dup
	iload_2
	dup_x1
	laload
	lload_3
	ladd
	lastore
	aload_1
	dup
	iload_2
	dup_x1
	laload
	lconst_1
	ladd
	lastore
	lstore_3
	aload_1
	dup
	iload_2
	dup_x1
	laload
	lconst_1
	lsub
	lastore
	aload_1
	dup
	iload_2
	dup_x1
	laload
	dup2_x2
	lconst_1
	lsub
	lastore
	lstore_3
	return
	.locals 5
	.maxstack 6
} // method m1_long
.method private hidebysig instance valuetype ['.library']'System'.'Decimal' 'm1_decimal'(valuetype ['.library']'System'.'Decimal'[] 'x', int32 'y') cil managed java 
{
	aload_1
	iload_2
	iconst_2
	invokestatic	"System/Decimal" "op_Implicit__iV" "(I)LSystem/Decimal;"
	aastore
	aload_1
	dup
	iload_2
	dup_x1
	aaload
	dup
	ifnonnull	?L1
	pop
	new	"System/Decimal"
	dup
	invokespecial	"System/Decimal" "<init>" "()V"
?L1:
	iconst_2
	invokestatic	"System/Decimal" "op_Implicit__iV" "(I)LSystem/Decimal;"
	invokestatic	"System/Decimal" "op_Addition__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	aastore
	aload_1
	iload_2
	aaload
	dup
	ifnonnull	?L2
	pop
	new	"System/Decimal"
	dup
	invokespecial	"System/Decimal" "<init>" "()V"
?L2:
	areturn
	.locals 3
	.maxstack 4
} // method m1_decimal
.method private hidebysig instance class ['.library']'System'.'String' 'm1_string'(class ['.library']'System'.'String'[] 'x', int32 'y') cil managed java 
{
	aload_1
	iload_2
	ldc	"Hello World"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	aastore
	aload_1
	iload_2
	aaload
	areturn
	.locals 3
	.maxstack 3
} // method m1_string
.method private hidebysig instance void 'm2_int'(int32[,] 'x', int32 'y', int32 'z') cil managed java 
{
	aload_1
	iload_2
	aaload
	iload_3
	iaload
	istore	4
	aload_1
	iload_2
	aaload
	iload_3
	iload	4
	iastore
	aload_1
	iload_2
	aaload
	dup
	iload_3
	dup_x1
	iaload
	iload	4
	iadd
	iastore
	aload_1
	iload_2
	aaload
	dup
	iload_3
	dup_x1
	iaload
	iconst_1
	iadd
	iastore
	istore	4
	aload_1
	iload_2
	aaload
	dup
	iload_3
	dup_x1
	iaload
	iconst_1
	isub
	iastore
	aload_1
	iload_2
	aaload
	dup
	iload_3
	dup_x1
	iaload
	dup_x2
	iconst_1
	isub
	iastore
	istore	4
	return
	.locals 5
	.maxstack 4
} // method m2_int
.method private hidebysig instance void 'm2_byte'(unsigned int8[,] 'x', int32 'y', int32 'z') cil managed java 
{
	aload_1
	iload_2
	aaload
	iload_3
	baload
	sipush	255
	iand
	istore	4
	aload_1
	iload_2
	aaload
	iload_3
	iload	4
	bastore
	aload_1
	iload_2
	aaload
	dup
	iload_3
	dup_x1
	baload
	sipush	255
	iand
	iconst_1
	iadd
	sipush	255
	iand
	bastore
	istore	4
	aload_1
	iload_2
	aaload
	dup
	iload_3
	dup_x1
	baload
	sipush	255
	iand
	iconst_1
	isub
	sipush	255
	iand
	bastore
	aload_1
	iload_2
	aaload
	dup
	iload_3
	dup_x1
	baload
	sipush	255
	iand
	dup_x2
	iconst_1
	isub
	sipush	255
	iand
	bastore
	istore	4
	return
	.locals 5
	.maxstack 4
} // method m2_byte
.method private hidebysig instance void 'm2_long'(int64[,] 'x', int32 'y', int32 'z') cil managed java 
{
	aload_1
	iload_2
	aaload
	iload_3
	laload
	lstore	4
	aload_1
	iload_2
	aaload
	iload_3
	lload	4
	lastore
	aload_1
	iload_2
	aaload
	dup
	iload_3
	dup_x1
	laload
	lload	4
	ladd
	lastore
	aload_1
	iload_2
	aaload
	dup
	iload_3
	dup_x1
	laload
	lconst_1
	ladd
	lastore
	lstore	4
	aload_1
	iload_2
	aaload
	dup
	iload_3
	dup_x1
	laload
	lconst_1
	lsub
	lastore
	aload_1
	iload_2
	aaload
	dup
	iload_3
	dup_x1
	laload
	dup2_x2
	lconst_1
	lsub
	lastore
	lstore	4
	return
	.locals 6
	.maxstack 6
} // method m2_long
.method private hidebysig instance valuetype ['.library']'System'.'Decimal' 'm2_decimal'(valuetype ['.library']'System'.'Decimal'[,] 'x', int32 'y', int32 'z') cil managed java 
{
	aload_1
	iload_2
	aaload
	iload_3
	iconst_2
	invokestatic	"System/Decimal" "op_Implicit__iV" "(I)LSystem/Decimal;"
	aastore
	aload_1
	iload_2
	aaload
	dup
	iload_3
	dup_x1
	aaload
	dup
	ifnonnull	?L3
	pop
	new	"System/Decimal"
	dup
	invokespecial	"System/Decimal" "<init>" "()V"
?L3:
	iconst_2
	invokestatic	"System/Decimal" "op_Implicit__iV" "(I)LSystem/Decimal;"
	invokestatic	"System/Decimal" "op_Addition__VVV" "(LSystem/Decimal;LSystem/Decimal;)LSystem/Decimal;"
	aastore
	aload_1
	iload_2
	aaload
	iload_3
	aaload
	dup
	ifnonnull	?L4
	pop
	new	"System/Decimal"
	dup
	invokespecial	"System/Decimal" "<init>" "()V"
?L4:
	areturn
	.locals 4
	.maxstack 4
} // method m2_decimal
.method private hidebysig instance class ['.library']'System'.'String' 'm2_string'(class ['.library']'System'.'String'[,] 'x', int32 'y', int32 'z') cil managed java 
{
	aload_1
	iload_2
	aaload
	iload_3
	ldc	"Hello World"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	aastore
	aload_1
	iload_2
	aaload
	iload_3
	aaload
	areturn
	.locals 4
	.maxstack 3
} // method m2_string
.method private hidebysig instance int32 'm3_uint'(int32[] 'x', unsigned int32 'y') cil managed java 
{
	aload_1
	iload_2
	iconst_1
	iadd
	invokestatic	"System/Intrinsics/Operations" "ui2i_ovf" "(I)I"
	iaload
	ireturn
	.locals 3
	.maxstack 3
} // method m3_uint
.method private hidebysig instance int32 'm3_long'(int32[] 'x', int64 'y') cil managed java 
{
	aload_1
	lload_2
	lconst_1
	ladd
	invokestatic	"System/Intrinsics/Operations" "l2i_ovf" "(J)I"
	iaload
	ireturn
	.locals 4
	.maxstack 5
} // method m3_long
.method private hidebysig instance int32 'm3_ulong'(int32[] 'x', unsigned int64 'y') cil managed java 
{
	aload_1
	lload_2
	lconst_1
	ladd
	invokestatic	"System/Intrinsics/Operations" "ul2i_ovf" "(J)I"
	iaload
	ireturn
	.locals 4
	.maxstack 5
} // method m3_ulong
.method private hidebysig instance int32 'm3_uint'(int32[] 'x', unsigned int16 'y') cil managed java 
{
	aload_1
	iload_2
	iaload
	ireturn
	.locals 3
	.maxstack 2
} // method m3_uint
.method private hidebysig instance int32 'm3_c_uint'(int32[] 'x', unsigned int32 'y') cil managed java 
{
	aload_1
	iload_2
	iconst_1
	invokestatic	"System/Intrinsics/Operations" "uiadd_ovf" "(II)I"
	invokestatic	"System/Intrinsics/Operations" "ui2i_ovf" "(I)I"
	iaload
	ireturn
	.locals 3
	.maxstack 3
} // method m3_c_uint
.method private hidebysig instance int32 'm3_c_long'(int32[] 'x', int64 'y') cil managed java 
{
	aload_1
	lload_2
	lconst_1
	invokestatic	"System/Intrinsics/Operations" "ladd_ovf" "(JJ)J"
	invokestatic	"System/Intrinsics/Operations" "l2i_ovf" "(J)I"
	iaload
	ireturn
	.locals 4
	.maxstack 5
} // method m3_c_long
.method private hidebysig instance int32 'm3_c_ulong'(int32[] 'x', unsigned int64 'y') cil managed java 
{
	aload_1
	lload_2
	lconst_1
	invokestatic	"System/Intrinsics/Operations" "uladd_ovf" "(JJ)J"
	invokestatic	"System/Intrinsics/Operations" "ul2i_ovf" "(J)I"
	iaload
	ireturn
	.locals 4
	.maxstack 5
} // method m3_c_ulong
.method private hidebysig instance int32 'm3_c_uint'(int32[] 'x', unsigned int16 'y') cil managed java 
{
	aload_1
	iload_2
	iaload
	ireturn
	.locals 3
	.maxstack 2
} // method m3_c_uint
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test
