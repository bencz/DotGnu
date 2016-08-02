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
.method private hidebysig instance int32 'm1'(class ['.library']'System'.'String' 'str') cil managed java 
{
	iconst_0
	istore_2
	aload_1
	dup
	astore	4
	invokevirtual	"System/String" "get_Length" "()I"
	istore	6
	iconst_0
	istore	5
	goto	?L1
?L2:
	aload	4
	iload	5
	invokevirtual	"System/String" "get_Chars" "(I)C"
	istore_3
	iload_2
	iload_3
	iadd
	istore_2
?L3:
	iinc	5 1
?L1:
	iload	5
	iload	6
	if_icmplt	?L2
?L4:
	iload_2
	ireturn
	.locals 7
	.maxstack 3
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
