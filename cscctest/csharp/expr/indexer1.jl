.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class public auto ansi 'Dictionary' extends ['.library']'System'.'Object'
{
.custom instance void ['.library']'System.Reflection'.'DefaultMemberAttribute'::'.ctor'(class ['.library']'System'.'String') = (01 00 04 49 74 65 6D 00 00)
.method public hidebysig specialname instance class ['.library']'System'.'Object' 'get_Item'(class ['.library']'System'.'Object' 'key') cil managed java 
{
	aconst_null
	areturn
	.locals 2
	.maxstack 1
} // method get_Item
.method public hidebysig specialname instance void 'set_Item'(class ['.library']'System'.'Object' 'key', class ['.library']'System'.'Object' 'value') cil managed java 
{
	return
	.locals 3
	.maxstack 0
} // method set_Item
.property class ['.library']'System'.'Object' 'Item'(class ['.library']'System'.'Object')
{
	.get instance class ['.library']'System'.'Object' 'Dictionary'::'get_Item'(class ['.library']'System'.'Object')
	.set instance void 'Dictionary'::'set_Item'(class ['.library']'System'.'Object', class ['.library']'System'.'Object')
} // property Item
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Dictionary
.class public auto ansi 'Counters' extends ['.library']'System'.'Object'
{
.custom instance void ['.library']'System.Reflection'.'DefaultMemberAttribute'::'.ctor'(class ['.library']'System'.'String') = (01 00 04 49 74 65 6D 00 00)
.field private int32[] 'counters'
.method public hidebysig specialname instance int32 'get_Item'(int32 'index') cil managed java 
{
	aload_0
	getfield	int32[] 'Counters'::'counters'
	iload_1
	iaload
	ireturn
	.locals 2
	.maxstack 2
} // method get_Item
.method public hidebysig specialname instance void 'set_Item'(int32 'index', int32 'value') cil managed java 
{
	aload_0
	getfield	int32[] 'Counters'::'counters'
	iload_1
	iload_2
	iastore
	return
	.locals 3
	.maxstack 3
} // method set_Item
.property int32 'Item'(int32)
{
	.get instance int32 'Counters'::'get_Item'(int32)
	.set instance void 'Counters'::'set_Item'(int32, int32)
} // property Item
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Counters
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private hidebysig instance class ['.library']'System'.'Object' 'm1'(class 'Dictionary' 'd', class ['.library']'System'.'Object' 'x') cil managed java 
{
	aload_1
	aload_2
	ldc	"Hello World"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	invokespecial	instance void 'Dictionary'::'set_Item'(class ['.library']'System'.'Object', class ['.library']'System'.'Object')
	aload_1
	aload_2
	invokespecial	instance class ['.library']'System'.'Object' 'Dictionary'::'get_Item'(class ['.library']'System'.'Object')
	areturn
	.locals 3
	.maxstack 3
} // method m1
.method private hidebysig instance class ['.library']'System'.'Object' 'm2'(class 'Dictionary' 'd', class ['.library']'System'.'Object' 'x') cil managed java 
{
	aload_1
	aload_2
	ldc	"Hello World"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	dup
	astore_3
	invokespecial	instance void 'Dictionary'::'set_Item'(class ['.library']'System'.'Object', class ['.library']'System'.'Object')
	aload_3
	areturn
	.locals 4
	.maxstack 4
} // method m2
.method private hidebysig instance int32 'm3'(class 'Counters' 'c') cil managed java 
{
	aload_1
	dup
	astore_2
	iconst_3
	dup
	istore_3
	aload_2
	iload_3
	invokespecial	instance int32 'Counters'::'get_Item'(int32)
	iconst_1
	iadd
	invokespecial	instance void 'Counters'::'set_Item'(int32, int32)
	aload_1
	dup
	astore_2
	iconst_3
	dup
	istore_3
	aload_2
	iload_3
	invokespecial	instance int32 'Counters'::'get_Item'(int32)
	iconst_1
	isub
	dup
	istore_3
	invokespecial	instance void 'Counters'::'set_Item'(int32, int32)
	iload_3
	ireturn
	.locals 4
	.maxstack 4
} // method m3
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test
