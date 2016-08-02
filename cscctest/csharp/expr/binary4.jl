.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class public auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method public hidebysig instance void 'm1'(class 'Test' 't') cil managed java 
{
	ldc	"Hello"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	astore_2
	ldc	" World"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	astore_3
	aload_2
	aload_3
	invokestatic	"System/String" "Concat" "(LSystem/String;LSystem/String;)LSystem/String;"
	astore	4
	aload_2
	aload_1
	invokestatic	"System/String" "Concat" "(LSystem/Object;LSystem/Object;)LSystem/String;"
	astore	4
	aload	4
	aload_2
	invokestatic	"System/String" "Concat" "(LSystem/String;LSystem/String;)LSystem/String;"
	astore	4
	aload	4
	aload_1
	invokestatic	"System/String" "Concat" "(LSystem/Object;LSystem/Object;)LSystem/String;"
	astore	4
	return
	.locals 5
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
