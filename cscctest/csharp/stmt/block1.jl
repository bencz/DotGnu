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
	return
	.locals 4
	.maxstack 0
} // method m1
.method private hidebysig instance void 'm2'(int32 'x') cil managed java 
{
	return
	.locals 3
	.maxstack 0
} // method m2
.method private hidebysig instance void 'm3'() cil managed java 
{
	iconst_0
	istore_1
	goto	?L1
?L2:
?L3:
	iload_1
	iconst_1
	iadd
	istore_1
?L1:
	iload_1
	bipush	10
	if_icmplt	?L2
?L4:
	iconst_0
	istore_2
	goto	?L5
?L6:
?L7:
	iload_2
	iconst_1
	iadd
	istore_2
?L5:
	iload_2
	bipush	10
	if_icmplt	?L6
?L8:
	return
	.locals 3
	.maxstack 2
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
