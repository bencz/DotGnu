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
.method private hidebysig instance void 'm1'(class 'Test' 't') cil managed java 
{
	aload_1
	dup
	astore_2
	monitorenter
	.try {
	aload_0
	aload_1
	invokespecial	instance void 'Test'::'m2'(class ['.library']'System'.'Object')
	aload_2
	monitorexit
	goto	?L1
	}
	catch {
	astore_3
	jsr	?L2
	aload_3
	athrow
	}
	finally {
?L2:
	astore	4
	aload_2
	monitorexit
	ret	4
	}
?L1:
	return
	.locals 5
	.maxstack 2
} // method m1
.method private hidebysig instance void 'm2'(class ['.library']'System'.'Object' 't') cil managed java 
{
	return
	.locals 2
	.maxstack 0
} // method m2
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test
