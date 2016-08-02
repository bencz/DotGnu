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
	iconst_4
	newarray int32
	dup
	iconst_0
	iconst_3
	iastore
	dup
	iconst_1
	iconst_4
	iastore
	dup
	iconst_2
	iconst_5
	iastore
	dup
	iconst_3
	bipush	6
	iastore
	astore_1
	iconst_5
	anewarray valuetype ['.library']'System'.'Decimal'
	dup
	iconst_0
	iconst_3
	invokestatic	"System/Decimal" "op_Implicit__iV" "(I)LSystem/Decimal;"
	aastore
	dup
	iconst_1
	new	"System/Decimal"
	dup
	bipush	45
	iconst_0
	iconst_0
	iconst_0
	iconst_1
	invokespecial	"System/Decimal" "<init>__iiiB" "(IIIZI)"
	aastore
	dup
	iconst_2
	iconst_5
	invokestatic	"System/Decimal" "op_Implicit__iV" "(I)LSystem/Decimal;"
	aastore
	dup
	iconst_3
	new	"System/Decimal"
	dup
	bipush	65
	iconst_0
	iconst_0
	iconst_0
	iconst_1
	invokespecial	"System/Decimal" "<init>__iiiB" "(IIIZI)"
	aastore
	dup
	iconst_4
	bipush	7
	invokestatic	"System/Decimal" "op_Implicit__iV" "(I)LSystem/Decimal;"
	aastore
	astore_2
	iconst_2
	iconst_2
	multianewarray	int32[,] 2
	dup
	iconst_0
	aaload
	dup
	iconst_0
	iconst_3
	iastore
	dup
	iconst_1
	iconst_4
	iastore
	pop
	dup
	iconst_1
	aaload
	dup
	iconst_0
	iconst_5
	iastore
	dup
	iconst_1
	bipush	6
	iastore
	pop
	astore_3
	iconst_3
	iconst_2
	multianewarray	int32[,] 2
	dup
	iconst_0
	aaload
	dup
	iconst_0
	iconst_3
	iastore
	dup
	iconst_1
	iconst_4
	iastore
	pop
	dup
	iconst_1
	aaload
	dup
	iconst_0
	iconst_5
	iastore
	dup
	iconst_1
	bipush	6
	iastore
	pop
	dup
	iconst_2
	aaload
	dup
	iconst_0
	bipush	7
	iastore
	dup
	iconst_1
	bipush	8
	iastore
	pop
	astore	4
	return
	.locals 5
	.maxstack 10
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
