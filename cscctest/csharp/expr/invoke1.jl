.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class public sequential sealed serializable ansi 'X' extends ['.library']'System'.'ValueType'
{
.field private int32 'x'
.method public hidebysig specialname rtspecialname instance void '.ctor'(int32 '_x') cil managed java 
{
	aload_0
	iload_1
	putfield	int32 'X'::'x'
	return
	.locals 2
	.maxstack 2
} // method .ctor
.method public hidebysig instance int32 'getX'() cil managed java 
{
	aload_0
	getfield	int32 'X'::'x'
	ireturn
	.locals 1
	.maxstack 1
} // method getX
.method public virtual hidebysig newslot instance int32 'getX2'() cil managed java 
{
	aload_0
	getfield	int32 'X'::'x'
	ireturn
	.locals 1
	.maxstack 1
} // method getX2
.method public hidebysig specialname instance int32 'get_XProp'() cil managed java 
{
	aload_0
	getfield	int32 'X'::'x'
	ireturn
	.locals 1
	.maxstack 1
} // method get_XProp
.property int32 'XProp'()
{
	.get instance int32 'X'::'get_XProp'()
} // property XProp
.method public virtual hidebysig newslot specialname instance int32 'get_XProp2'() cil managed java 
{
	aload_0
	getfield	int32 'X'::'x'
	ireturn
	.locals 1
	.maxstack 1
} // method get_XProp2
.property int32 'XProp2'()
{
	.get instance int32 'X'::'get_XProp2'()
} // property XProp2
} // class X
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private hidebysig instance int32 'm1'(valuetype 'X' 'x') cil managed java 
{
	aload_1
	invokestatic	"X" "copyIn__" "(LX;)LX;"
	invokespecial	instance int32 'X'::'getX'()
	ireturn
	.locals 2
	.maxstack 1
} // method m1
.method private hidebysig instance valuetype 'X' 'm2'() cil managed java 
{
	new	'X'
	dup
	iconst_0
	invokespecial	instance void 'X'::'.ctor'(int32)
	areturn
	.locals 1
	.maxstack 3
} // method m2
.method private hidebysig instance int32 'm3'() cil managed java 
{
	aload_0
	invokespecial	instance valuetype 'X' 'Test'::'m2'()
	invokestatic	"X" "copyIn__" "(LX;)LX;"
	invokespecial	instance int32 'X'::'getX'()
	ireturn
	.locals 1
	.maxstack 1
} // method m3
.method private hidebysig instance int32 'm4'() cil managed java 
{
	aload_0
	invokespecial	instance valuetype 'X' 'Test'::'m2'()
	invokestatic	"X" "copyIn__" "(LX;)LX;"
	invokevirtual	instance int32 'X'::'getX2'()
	ireturn
	.locals 1
	.maxstack 1
} // method m4
.method private hidebysig instance int32 'm5'(valuetype 'X' 'x') cil managed java 
{
	aload_1
	invokestatic	"X" "copyIn__" "(LX;)LX;"
	invokespecial	instance int32 'X'::'get_XProp'()
	ireturn
	.locals 2
	.maxstack 1
} // method m5
.method private hidebysig instance int32 'm6'() cil managed java 
{
	aload_0
	invokespecial	instance valuetype 'X' 'Test'::'m2'()
	invokestatic	"X" "copyIn__" "(LX;)LX;"
	invokespecial	instance int32 'X'::'get_XProp'()
	ireturn
	.locals 1
	.maxstack 1
} // method m6
.method private hidebysig instance int32 'm7'(valuetype 'X' 'x') cil managed java 
{
	aload_1
	invokestatic	"X" "copyIn__" "(LX;)LX;"
	invokevirtual	instance int32 'X'::'get_XProp2'()
	ireturn
	.locals 2
	.maxstack 1
} // method m7
.method private hidebysig instance int32 'm8'() cil managed java 
{
	aload_0
	invokespecial	instance valuetype 'X' 'Test'::'m2'()
	invokestatic	"X" "copyIn__" "(LX;)LX;"
	invokevirtual	instance int32 'X'::'get_XProp2'()
	ireturn
	.locals 1
	.maxstack 1
} // method m8
.method private hidebysig instance class ['.library']'System'.'String' 'm9'(valuetype 'X' 'x') cil managed java 
{
	aload_1
	invokestatic	"X" "copyIn__" "(LX;)LX;"
	invokevirtual	instance class ['.library']'System'.'String' ['.library']'System'.'Object'::'ToString'()
	areturn
	.locals 2
	.maxstack 1
} // method m9
.method private hidebysig instance class ['.library']'System'.'String' 'm10'() cil managed java 
{
	aload_0
	invokespecial	instance valuetype 'X' 'Test'::'m2'()
	invokestatic	"X" "copyIn__" "(LX;)LX;"
	invokevirtual	instance class ['.library']'System'.'String' ['.library']'System'.'Object'::'ToString'()
	areturn
	.locals 1
	.maxstack 1
} // method m10
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test
