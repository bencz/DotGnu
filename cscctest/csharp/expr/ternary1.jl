.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private auto sealed serializable ansi 'Dino' extends ['.library']'System'.'Enum'
{
.field public static literal valuetype 'Dino' 'None' = int32(0x00000000)
.field public static literal valuetype 'Dino' 'Trex' = int32(0x00000001)
.field public specialname rtspecialname int32 'value__'
} // class Dino
.class private auto ansi 'FooBar' extends ['.library']'System'.'Object'
{
.method private static hidebysig void 'Foo'(bool 'x') cil managed java 
{
	iload_0
	ifeq	?L1
	iconst_1
	goto	?L2
?L1:
	iconst_0
?L2:
	invokestatic	"Dino" "copyIn__" "(I)LDino;"
	invokevirtual	instance class ['.library']'System'.'String' ['.library']'System'.'Object'::'ToString'()
	astore_1
	iload_0
	ifeq	?L3
	iconst_0
	goto	?L4
?L3:
	iconst_1
?L4:
	invokestatic	"Dino" "copyIn__" "(I)LDino;"
	invokevirtual	instance class ['.library']'System'.'String' ['.library']'System'.'Object'::'ToString'()
	astore_1
	return
	.locals 2
	.maxstack 1
} // method Foo
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class FooBar
