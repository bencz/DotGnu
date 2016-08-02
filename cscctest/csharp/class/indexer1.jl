.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class public auto ansi 'TestIndexer' extends ['.library']'System'.'Object'
{
.custom instance void ['.library']'System.Reflection'.'DefaultMemberAttribute'::'.ctor'(class ['.library']'System'.'String') = (01 00 04 49 74 65 6D 00 00)
.method public hidebysig specialname instance int32 'get_Item'(int32 'x', class ['.library']'System'.'Object'[] 'args') cil managed java 
{
.param[2]
.custom instance void ['.library']'System'.'ParamArrayAttribute'::'.ctor'() = (01 00 00 00)
	iload_1
	ireturn
	.locals 3
	.maxstack 1
} // method get_Item
.method public hidebysig specialname instance void 'set_Item'(int32 'x', class ['.library']'System'.'Object'[] 'args', int32 'value') cil managed java 
{
.param[2]
.custom instance void ['.library']'System'.'ParamArrayAttribute'::'.ctor'() = (01 00 00 00)
	return
	.locals 4
	.maxstack 0
} // method set_Item
.property int32 'Item'(int32, class ['.library']'System'.'Object'[])
{
	.get instance int32 'TestIndexer'::'get_Item'(int32, class ['.library']'System'.'Object'[])
	.set instance void 'TestIndexer'::'set_Item'(int32, class ['.library']'System'.'Object'[], int32)
} // property Item
.method public hidebysig instance void 'test'() cil managed java 
{
	aload_0
	iconst_1
	bipush	8
	anewarray class ['.library']'System'.'Object'
	dup
	iconst_0
	iconst_2
	invokestatic	"System/Int32" "copyIn__" "(I)LSystem/Int32;"
	aastore
	dup
	iconst_1
	iconst_3
	invokestatic	"System/Int32" "copyIn__" "(I)LSystem/Int32;"
	aastore
	dup
	iconst_2
	iconst_4
	invokestatic	"System/Int32" "copyIn__" "(I)LSystem/Int32;"
	aastore
	dup
	iconst_3
	iconst_5
	invokestatic	"System/Int32" "copyIn__" "(I)LSystem/Int32;"
	aastore
	dup
	iconst_4
	bipush	6
	invokestatic	"System/Int32" "copyIn__" "(I)LSystem/Int32;"
	aastore
	dup
	iconst_5
	bipush	7
	invokestatic	"System/Int32" "copyIn__" "(I)LSystem/Int32;"
	aastore
	dup
	bipush	6
	bipush	8
	invokestatic	"System/Int32" "copyIn__" "(I)LSystem/Int32;"
	aastore
	dup
	bipush	7
	bipush	9
	invokestatic	"System/Int32" "copyIn__" "(I)LSystem/Int32;"
	aastore
	bipush	42
	invokespecial	instance void 'TestIndexer'::'set_Item'(int32, class ['.library']'System'.'Object'[], int32)
	aload_0
	bipush	12
	iconst_3
	anewarray class ['.library']'System'.'Object'
	dup
	iconst_0
	bipush	22
	invokestatic	"System/Int32" "copyIn__" "(I)LSystem/Int32;"
	aastore
	dup
	iconst_1
	bipush	34
	invokestatic	"System/Int32" "copyIn__" "(I)LSystem/Int32;"
	aastore
	dup
	iconst_2
	bipush	45
	invokestatic	"System/Int32" "copyIn__" "(I)LSystem/Int32;"
	aastore
	invokespecial	instance int32 'TestIndexer'::'get_Item'(int32, class ['.library']'System'.'Object'[])
	istore_1
	return
	.locals 2
	.maxstack 6
} // method test
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class TestIndexer
