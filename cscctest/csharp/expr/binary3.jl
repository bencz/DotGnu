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
.field public int32 'x'
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
.field private int32 'y'
.method public hidebysig specialname rtspecialname instance void '.ctor'(int32 '_y') cil managed java 
{
	aload_0
	iload_1
	putfield	int32 'Y'::'y'
	return
	.locals 2
	.maxstack 2
} // method .ctor
.method public static hidebysig specialname valuetype 'Y' 'op_Subtraction'(valuetype 'Y' 'y', valuetype 'X' 'x') cil managed java 
{
	new	'Y'
	dup
	aload_0
	invokestatic	"Y" "copyIn__" "(LY;)LY;"
	getfield	int32 'Y'::'y'
	aload_1
	invokestatic	"X" "copyIn__" "(LX;)LX;"
	getfield	int32 'X'::'x'
	isub
	invokespecial	instance void 'Y'::'.ctor'(int32)
	areturn
	.locals 2
	.maxstack 4
} // method op_Subtraction
.method public static hidebysig specialname valuetype 'X' 'op_Subtraction'(valuetype 'Y' 'y1', valuetype 'Y' 'y2') cil managed java 
{
	new	'X'
	dup
	aload_0
	invokestatic	"Y" "copyIn__" "(LY;)LY;"
	getfield	int32 'Y'::'y'
	aload_1
	invokestatic	"Y" "copyIn__" "(LY;)LY;"
	getfield	int32 'Y'::'y'
	isub
	invokespecial	instance void 'X'::'.ctor'(int32)
	areturn
	.locals 2
	.maxstack 4
} // method op_Subtraction
} // class Y
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private hidebysig instance void 'm1'() cil managed java 
{
	new	"Y"
	dup
	invokespecial	"Y" "<init>" "()V"
	astore_1
	new	"Y"
	dup
	invokespecial	"Y" "<init>" "()V"
	astore_2
	new	"X"
	dup
	invokespecial	"X" "<init>" "()V"
	astore_3
	new	"Y"
	dup
	invokespecial	"Y" "<init>" "()V"
	astore	4
	new	'Y'
	dup
	iconst_1
	invokespecial	instance void 'Y'::'.ctor'(int32)
	astore_1
	new	'Y'
	dup
	iconst_2
	invokespecial	instance void 'Y'::'.ctor'(int32)
	astore_2
	aload_1
	aload_2
	invokestatic	valuetype 'X' 'Y'::'op_Subtraction'(valuetype 'Y', valuetype 'Y')
	astore_3
	aload_1
	aload_3
	invokestatic	valuetype 'Y' 'Y'::'op_Subtraction'(valuetype 'Y', valuetype 'X')
	astore	4
	return
	.locals 5
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
