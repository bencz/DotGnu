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
	ldc	"System.Int32"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	invokestatic	"System/Type" "GetType" "(LSystem/String;)LSystem/Type;"
	astore_1
	ldc	"System.Void"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	invokestatic	"System/Type" "GetType" "(LSystem/String;)LSystem/Type;"
	astore_1
	ldc	"int[]"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	invokestatic	"System/Type" "GetType" "(LSystem/String;)LSystem/Type;"
	astore_1
	ldc	"Test[]"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	invokestatic	"System/Type" "GetType" "(LSystem/String;)LSystem/Type;"
	astore_1
	return
	.locals 2
	.maxstack 1
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
