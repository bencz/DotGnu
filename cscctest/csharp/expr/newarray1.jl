.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private sequential sealed serializable ansi 'X' extends ['.library']'System'.'ValueType'
{
.field private int32 'x'
.method public hidebysig specialname rtspecialname instance void '.ctor'(int32 '_x') cil managed java 
{
	aload_0
	iload_1
	putfield	int32 'X'::'x'
	return
	.locals 2
	.maxstack 2
} // method .ctor
} // class X
.class private auto sealed serializable ansi 'Color' extends ['.library']'System'.'Enum'
{
.field public static literal valuetype 'Color' 'Red' = int32(0x00000000)
.field public static literal valuetype 'Color' 'Green' = int32(0x00000001)
.field public static literal valuetype 'Color' 'Blue' = int32(0x00000002)
.field public specialname rtspecialname int32 'value__'
} // class Color
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private hidebysig instance void 'm1'(int32 'dim', unsigned int32 'dim2', int64 'dim3', unsigned int64 'dim4') cil managed java 
{
	iconst_3
	newarray int32
	astore	7
	iload_1
	anewarray valuetype 'X'
	astore	8
	iconst_3
	iload_1
	multianewarray	valuetype 'Color'[,] 2
	astore	9
	iconst_3
	anewarray class 'Test'
	astore	10
	iconst_3
	anewarray class 'Test'[]
	astore	11
	iload_1
	iload_1
	iload_1
	multianewarray	class ['.library']'System'.'Object'[,,] 3
	astore	12
	iload_1
	iload_1
	iload_1
	multianewarray	class ['.library']'System'.'Object'[,,,][][,,] 3
	astore	13
	iload_2
	newarray int32
	astore	7
	lload_3
	invokestatic	"System/Intrinsics/Operations" "l2ui_ovf" "(J)I"
	newarray int32
	astore	7
	lload	5
	invokestatic	"System/Intrinsics/Operations" "l2ui_ovf" "(J)I"
	newarray int32
	astore	7
	iconst_3
	iload_2
	multianewarray	class ['.library']'System'.'Object'[,] 2
	astore	14
	iconst_3
	lload_3
	invokestatic	"System/Intrinsics/Operations" "l2ui_ovf" "(J)I"
	multianewarray	class ['.library']'System'.'Object'[,] 2
	astore	14
	iconst_3
	lload	5
	invokestatic	"System/Intrinsics/Operations" "l2ui_ovf" "(J)I"
	multianewarray	class ['.library']'System'.'Object'[,] 2
	astore	14
	iload_2
	iconst_3
	multianewarray	class ['.library']'System'.'Object'[,] 2
	astore	14
	lload_3
	invokestatic	"System/Intrinsics/Operations" "l2ui_ovf" "(J)I"
	iconst_3
	multianewarray	class ['.library']'System'.'Object'[,] 2
	astore	14
	lload	5
	invokestatic	"System/Intrinsics/Operations" "l2ui_ovf" "(J)I"
	iconst_3
	multianewarray	class ['.library']'System'.'Object'[,] 2
	astore	14
	return
	.locals 15
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
