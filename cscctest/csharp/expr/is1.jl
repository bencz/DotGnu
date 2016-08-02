.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private auto interface abstract ansi 'I'
{
} // class I
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
	aconst_null
	aconst_null
	if_acmpeq	?L1
	iconst_1
	goto	?L2
?L1:
	iconst_0
?L2:
	istore_1
	iconst_2
	istore	7
	aload_2
	aconst_null
	if_acmpeq	?L3
	iconst_1
	goto	?L4
?L3:
	iconst_0
?L4:
	istore_1
	aload_2
	aconst_null
	if_acmpeq	?L5
	iconst_1
	istore	7
?L5:
	aload_2
	aconst_null
	if_acmpne	?L6
	iconst_2
	istore	7
?L6:
	aload_3
	aconst_null
	if_acmpeq	?L7
	iconst_1
	goto	?L8
?L7:
	iconst_0
?L8:
	istore_1
	aload_3
	aconst_null
	if_acmpeq	?L9
	iconst_1
	istore	7
?L9:
	aload_3
	aconst_null
	if_acmpne	?L10
	iconst_2
	istore	7
?L10:
	aload	4
	aconst_null
	if_acmpeq	?L11
	iconst_1
	goto	?L12
?L11:
	iconst_0
?L12:
	istore_1
	aload	4
	aconst_null
	if_acmpeq	?L13
	iconst_1
	istore	7
?L13:
	aload	4
	aconst_null
	if_acmpne	?L14
	iconst_2
	istore	7
?L14:
	aload	4
	aconst_null
	if_acmpeq	?L15
	iconst_1
	goto	?L16
?L15:
	iconst_0
?L16:
	istore_1
	aload	4
	aconst_null
	if_acmpeq	?L17
	iconst_1
	istore	7
?L17:
	aload	4
	aconst_null
	if_acmpne	?L18
	iconst_2
	istore	7
?L18:
	iconst_1
	istore_1
	iconst_1
	istore	7
	iconst_1
	istore_1
	iconst_1
	istore	7
	iconst_1
	istore_1
	iconst_1
	istore	7
	iconst_1
	istore_1
	iconst_1
	istore	7
	aload_3
	instanceof	'Test2'
	istore_1
	aload_3
	instanceof	'Test2'
	ifeq	?L19
	iconst_1
	istore	7
?L19:
	aload_3
	instanceof	'Test2'
	ifne	?L20
	iconst_2
	istore	7
?L20:
	aload_3
	instanceof	'I'
	istore_1
	aload_3
	instanceof	'I'
	ifeq	?L21
	iconst_1
	istore	7
?L21:
	aload_3
	instanceof	'I'
	ifne	?L22
	iconst_2
	istore	7
?L22:
	aload_2
	instanceof	['.library']'System'.'Int32'
	istore_1
	aload_2
	instanceof	['.library']'System'.'Int32'
	ifeq	?L23
	iconst_1
	istore	7
?L23:
	aload_2
	instanceof	['.library']'System'.'Int32'
	ifne	?L24
	iconst_2
	istore	7
?L24:
	iconst_0
	istore_1
	goto	?L25
	iconst_1
	istore	7
?L25:
	iconst_2
	istore	7
?L26:
	return
	.locals 9
	.maxstack 2
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
.class private auto ansi 'Test2' extends 'Test' implements 'I'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void 'Test'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test2
