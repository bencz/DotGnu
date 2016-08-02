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
.class private sequential sealed serializable ansi 'Y' extends ['.library']'System'.'ValueType'
{
.field public valuetype 'X' 'x'
.method public hidebysig specialname instance valuetype 'X' 'get_X'() cil managed java 
{
	aload_0
	getfield	valuetype 'X' 'Y'::'x'
	dup
	ifnonnull	?L1
	pop
	new	"X"
	dup
	invokespecial	"X" "<init>" "()V"
?L1:
	areturn
	.locals 1
	.maxstack 2
} // method get_X
.method public hidebysig specialname instance void 'set_X'(valuetype 'X' 'value') cil managed java 
{
	aload_0
	aload_1
	putfield	valuetype 'X' 'Y'::'x'
	return
	.locals 2
	.maxstack 2
} // method set_X
.property valuetype 'X' 'X'()
{
	.get instance valuetype 'X' 'Y'::'get_X'()
	.set instance void 'Y'::'set_X'(valuetype 'X')
} // property X
} // class Y
.class private auto sealed serializable ansi 'Color' extends ['.library']'System'.'Enum'
{
.field public static literal valuetype 'Color' 'Red' = int32(0x00000000)
.field public static literal valuetype 'Color' 'Green' = int32(0x00000001)
.field public static literal valuetype 'Color' 'Blue' = int32(0x00000002)
.field public specialname rtspecialname int32 'value__'
} // class Color
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private hidebysig instance void 'm1'() cil managed java 
{
	new	"X"
	dup
	invokespecial	"X" "<init>" "()V"
	astore_2
	iconst_0
	istore_1
	new	'X'
	dup
	invokespecial	"X" "<init>" "()V"
	astore_2
	iconst_0
	istore_3
	new	'X'
	dup
	iconst_3
	invokespecial	instance void 'X'::'.ctor'(int32)
	astore_2
	new	'Test2'
	dup
	invokespecial	instance void 'Test2'::'.ctor'()
	astore	4
	new	'Test2'
	dup
	iconst_3
	invokespecial	instance void 'Test2'::'.ctor'(int32)
	astore	4
	new	'Test2'
	dup
	iconst_3
	i2l
	invokespecial	instance void 'Test2'::'.ctor'(int64)
	astore	4
	new	['.library']'System'.'Object'
	dup
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	astore	5
	return
	.locals 6
	.maxstack 4
} // method m1
.method private hidebysig instance void 'm2'(valuetype 'Y' 'y') cil managed java 
{
	aload_1
	invokestatic	"Y" "copyIn__" "(LY;)LY;"
	new	'X'
	dup
	iconst_1
	invokespecial	instance void 'X'::'.ctor'(int32)
	putfield	valuetype 'X' 'Y'::'x'
	return
	.locals 2
	.maxstack 4
} // method m2
.method private hidebysig instance void 'm3'(valuetype 'Y' 'y') cil managed java 
{
	aload_1
	invokestatic	"Y" "copyIn__" "(LY;)LY;"
	new	'X'
	dup
	iconst_1
	invokespecial	instance void 'X'::'.ctor'(int32)
	invokespecial	instance void 'Y'::'set_X'(valuetype 'X')
	return
	.locals 2
	.maxstack 4
} // method m3
.method private hidebysig instance valuetype 'X' 'm4'(valuetype 'Y' 'y') cil managed java 
{
	aload_1
	invokestatic	"Y" "copyIn__" "(LY;)LY;"
	new	'X'
	dup
	iconst_1
	invokespecial	instance void 'X'::'.ctor'(int32)
	dup_x1
	putfield	valuetype 'X' 'Y'::'x'
	areturn
	.locals 2
	.maxstack 4
} // method m4
.method private hidebysig instance void 'm4'() cil managed java 
{
	iconst_2
	anewarray valuetype 'X'
	astore_1
	aload_1
	iconst_0
	new	'X'
	dup
	iconst_0
	invokespecial	instance void 'X'::'.ctor'(int32)
	aastore
	aload_1
	iconst_1
	new	'X'
	dup
	invokespecial	"X" "<init>" "()V"
	aastore
	return
	.locals 2
	.maxstack 5
} // method m4
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test
.class private auto ansi 'Test2' extends ['.library']'System'.'Object'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
.method public hidebysig specialname rtspecialname instance void '.ctor'(int32 'x') cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 2
	.maxstack 1
} // method .ctor
.method public hidebysig specialname rtspecialname instance void '.ctor'(int64 'x') cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 3
	.maxstack 1
} // method .ctor
} // class Test2
