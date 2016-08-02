.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private auto ansi 'Test1' extends ['.library']'System'.'Object'
{
.method public virtual hidebysig newslot instance void 'm3'() cil managed java 
{
	return
	.locals 1
	.maxstack 0
} // method m3
.method public hidebysig instance void 'm4'() cil managed java 
{
	return
	.locals 1
	.maxstack 0
} // method m4
.method private static hidebysig void 'm7'(class 'Test1' 'x') cil managed java 
{
	aload_0
	invokevirtual	instance void 'Test1'::'m3'()
	aload_0
	invokespecial	instance void 'Test1'::'m4'()
	return
	.locals 1
	.maxstack 1
} // method m7
.method private static hidebysig void 'm8'(class 'Test2' 'x') cil managed java 
{
	aload_0
	invokevirtual	instance void 'Test1'::'m3'()
	return
	.locals 1
	.maxstack 1
} // method m8
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test1
.class private auto ansi 'Test2' extends 'Test1'
{
.method public virtual hidebysig instance void 'm3'() cil managed java 
{
	return
	.locals 1
	.maxstack 0
} // method m3
.method private static hidebysig void 'm5'(class 'Test1' 'x') cil managed java 
{
	aload_0
	invokevirtual	instance void 'Test1'::'m3'()
	aload_0
	invokespecial	instance void 'Test1'::'m4'()
	return
	.locals 1
	.maxstack 1
} // method m5
.method private static hidebysig void 'm6'(class 'Test2' 'x') cil managed java 
{
	aload_0
	invokevirtual	instance void 'Test1'::'m3'()
	return
	.locals 1
	.maxstack 1
} // method m6
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void 'Test1'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test2
