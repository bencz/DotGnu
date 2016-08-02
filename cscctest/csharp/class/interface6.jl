.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class public auto interface abstract ansi 'A'
{
.method public virtual hidebysig newslot abstract specialname instance int32 'get_t1'() cil managed java 
{
} // method get_t1
.property int32 't1'()
{
	.get instance int32 'A'::'get_t1'()
} // property t1
} // class A
.class public auto interface abstract ansi 'B' implements 'A'
{
} // class B
.class public auto interface abstract ansi 'C' implements 'B', 'A'
{
} // class C
.class public auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private hidebysig instance int32 't2'(class 'C' 'x') cil managed java 
{
	aload_1
	invokeinterface	instance int32 'A'::'get_t1'() 1
	ireturn
	.locals 2
	.maxstack 1
} // method t2
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test
