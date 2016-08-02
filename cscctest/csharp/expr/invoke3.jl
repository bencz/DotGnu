.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class public sequential sealed serializable ansi 'invoke3' extends ['.library']'System'.'ValueType'
{
.field private int32 'a'
.method public hidebysig specialname rtspecialname instance void '.ctor'(int32 'a') cil managed java 
{
	aload_0
	iload_1
	putfield	int32 'invoke3'::'a'
	return
	.locals 2
	.maxstack 2
} // method .ctor
.method public virtual hidebysig instance class ['.library']'System'.'String' 'ToString'() cil managed java 
{
	aload_0
	invokespecial	instance class ['.library']'System'.'String' ['.library']'System'.'Object'::'ToString'()
	areturn
	.locals 1
	.maxstack 1
} // method ToString
.method public hidebysig instance class ['.library']'System'.'String' 'Test1'() cil managed java 
{
	aload_0
	getfield	int32 'invoke3'::'a'
	invokestatic	"System/Int32" "copyIn__" "(I)LSystem/Int32;"
	invokevirtual	instance class ['.library']'System'.'String' ['.library']'System'.'Int32'::'ToString'()
	areturn
	.locals 1
	.maxstack 1
} // method Test1
.method public hidebysig instance class ['.library']'System'.'String' 'Test2'(int32[] 'b') cil managed java 
{
	aload_1
	iconst_0
	iaload
	invokestatic	"System/Int32" "copyIn__" "(I)LSystem/Int32;"
	invokevirtual	instance class ['.library']'System'.'String' ['.library']'System'.'Int32'::'ToString'()
	areturn
	.locals 2
	.maxstack 2
} // method Test2
.method public hidebysig instance class ['.library']'System'.'String' 'Test3'(int32 'b') cil managed java 
{
	iload_1
	invokestatic	"System/Int32" "copyIn__" "(I)LSystem/Int32;"
	invokevirtual	instance class ['.library']'System'.'String' ['.library']'System'.'Int32'::'ToString'()
	areturn
	.locals 2
	.maxstack 1
} // method Test3
.method public hidebysig instance class ['.library']'System'.'String' 'Test4'() cil managed java 
{
	iconst_0
	istore_1
	iload_1
	invokestatic	"System/Int32" "copyIn__" "(I)LSystem/Int32;"
	invokevirtual	instance class ['.library']'System'.'String' ['.library']'System'.'Int32'::'ToString'()
	areturn
	.locals 2
	.maxstack 1
} // method Test4
.method public hidebysig instance class ['.library']'System'.'String' 'Test5'() cil managed java 
{
	iconst_0
	invokestatic	"System/Int32" "copyIn__" "(I)LSystem/Int32;"
	invokevirtual	instance class ['.library']'System'.'String' ['.library']'System'.'Int32'::'ToString'()
	areturn
	.locals 1
	.maxstack 1
} // method Test5
} // class invoke3
