.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.namespace 'Test1'
{
.class public auto ansi 'var' extends ['.library']'System'.'Object'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class var
} // namespace Test1
.namespace 'Test'
{
.class private auto ansi 'Test1' extends ['.library']'System'.'Object'
{
.method private static hidebysig int32 'i1'() cil managed java 
{
	bipush	10
	istore_0
	iload_0
	ireturn
	.locals 1
	.maxstack 1
} // method i1
.method private static hidebysig float32 'f1'() cil managed java 
{
	fconst_1
	fstore_0
	fload_0
	freturn
	.locals 1
	.maxstack 1
} // method f1
.method private static hidebysig float64 'd1'() cil managed java 
{
	dconst_1
	dstore_0
	dload_0
	dreturn
	.locals 2
	.maxstack 2
} // method d1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test1
} // namespace Test
.namespace 'Test'
{
.class private auto ansi 'Test2' extends ['.library']'System'.'Object'
{
.method private static hidebysig class 'Test1'.'var' 'v1'() cil managed java 
{
	new	'Test1'.'var'
	dup
	invokespecial	instance void 'Test1'.'var'::'.ctor'()
	astore_0
	aload_0
	areturn
	.locals 1
	.maxstack 2
} // method v1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test2
} // namespace Test
