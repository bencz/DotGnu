.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private auto ansi beforefieldinit 'Test' extends ['.library']'System'.'Object'
{
.field public static literal bool 'True' = bool(true)
.field public static literal bool 'False' = bool(false)
.field public static literal int32 'i' = int32(0xFFFE1DC0)
.field public static literal unsigned int32 'ui' = int32(0x0001E240)
.field public static literal int64 'l' = int64(0xFFFFFFFF00000000)
.field public static literal unsigned int64 'ul' = int64(0x7FFFFFFFFFFFFFFF)
.field public static literal float32 'f' = float32(0x6ACC3DA6)
.field public static literal float64 'd' = float64(0x4484D64651FE74C6)
.field public static initonly valuetype ['.library']'System'.'Decimal' 'dc1'
.custom instance void ['.library']'System.Runtime.CompilerServices'.'DecimalConstantAttribute'::'.ctor'(unsigned int8, unsigned int8, unsigned int32, unsigned int32, unsigned int32) = (01 00 00 00 FF FF FF FF FF FF FF FF FF FF FF FF 00 00)
.field public static initonly valuetype ['.library']'System'.'Decimal' 'dc2'
.custom instance void ['.library']'System.Runtime.CompilerServices'.'DecimalConstantAttribute'::'.ctor'(unsigned int8, unsigned int8, unsigned int32, unsigned int32, unsigned int32) = (01 00 00 80 27 36 0A 00 CC CA 7C CA 00 00 A0 91 00 00)
.field public static literal char 'c' = char(0x0061)
.field public static literal class ['.library']'System'.'String' 's1' = nullref
.field public static literal class ['.library']'System'.'String' 's2' = ""
.field public static literal class ['.library']'System'.'String' 's3' = "Hello World!"
.field public static literal class ['.library']'System'.'Object' 'o1' = nullref
.field public static literal class 'Test' 'o2' = nullref
.method private hidebysig instance bool 'm1'() cil managed java 
{
	iconst_1
	ireturn
	.locals 1
	.maxstack 1
} // method m1
.method private hidebysig instance bool 'm2'() cil managed java 
{
	iconst_0
	ireturn
	.locals 1
	.maxstack 1
} // method m2
.method private hidebysig instance int32 'm3'() cil managed java 
{
	ldc	int32(-123456)
	ireturn
	.locals 1
	.maxstack 1
} // method m3
.method private hidebysig instance unsigned int32 'm4'() cil managed java 
{
	ldc	int32(123456)
	ireturn
	.locals 1
	.maxstack 1
} // method m4
.method private hidebysig instance int64 'm5'() cil managed java 
{
	ldc2_w	int64(0xFFFFFFFF00000000)
	lreturn
	.locals 1
	.maxstack 2
} // method m5
.method private hidebysig instance unsigned int64 'm6'() cil managed java 
{
	ldc2_w	int64(0x7FFFFFFFFFFFFFFF)
	lreturn
	.locals 1
	.maxstack 2
} // method m6
.method private hidebysig instance float32 'm7'() cil managed java 
{
	ldc	float32(0x6ACC3DA6)
	freturn
	.locals 1
	.maxstack 1
} // method m7
.method private hidebysig instance float64 'm8'() cil managed java 
{
	ldc2_w	float64(0x4484D64651FE74C6)
	dreturn
	.locals 1
	.maxstack 2
} // method m8
.method private hidebysig instance valuetype ['.library']'System'.'Decimal' 'm9'() cil managed java 
{
	new	"System/Decimal"
	dup
	iconst_m1
	iconst_m1
	iconst_m1
	iconst_0
	iconst_0
	invokespecial	"System/Decimal" "<init>__iiiB" "(IIIZI)"
	areturn
	.locals 1
	.maxstack 7
} // method m9
.method private hidebysig instance valuetype ['.library']'System'.'Decimal' 'm10'() cil managed java 
{
	new	"System/Decimal"
	dup
	ldc	int32(-1851785216)
	ldc	int32(-897791284)
	ldc	int32(669223)
	iconst_1
	iconst_0
	invokespecial	"System/Decimal" "<init>__iiiB" "(IIIZI)"
	areturn
	.locals 1
	.maxstack 7
} // method m10
.method private hidebysig instance char 'm11'() cil managed java 
{
	bipush	97
	ireturn
	.locals 1
	.maxstack 1
} // method m11
.method private hidebysig instance class ['.library']'System'.'String' 'm12'() cil managed java 
{
	aconst_null
	areturn
	.locals 1
	.maxstack 1
} // method m12
.method private hidebysig instance class ['.library']'System'.'String' 'm13'() cil managed java 
{
	ldc	""
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	areturn
	.locals 1
	.maxstack 1
} // method m13
.method private hidebysig instance class ['.library']'System'.'String' 'm14'() cil managed java 
{
	ldc	"Hello World!"
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	areturn
	.locals 1
	.maxstack 1
} // method m14
.method private hidebysig instance class ['.library']'System'.'Object' 'm15'() cil managed java 
{
	aconst_null
	areturn
	.locals 1
	.maxstack 1
} // method m15
.method private hidebysig instance class ['.library']'System'.'Object' 'm16'() cil managed java 
{
	aconst_null
	areturn
	.locals 1
	.maxstack 1
} // method m16
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
.method private static hidebysig specialname rtspecialname void '.cctor'() cil managed java 
{
	new	"System/Decimal"
	dup
	iconst_m1
	iconst_m1
	iconst_m1
	iconst_0
	iconst_0
	invokespecial	"System/Decimal" "<init>__iiiB" "(IIIZI)"
	putstatic	valuetype ['.library']'System'.'Decimal' 'Test'::'dc1'
	new	"System/Decimal"
	dup
	ldc	int32(-1851785216)
	ldc	int32(-897791284)
	ldc	int32(669223)
	iconst_1
	iconst_0
	invokespecial	"System/Decimal" "<init>__iiiB" "(IIIZI)"
	putstatic	valuetype ['.library']'System'.'Decimal' 'Test'::'dc2'
	return
	.locals 0
	.maxstack 7
} // method .cctor
} // class Test
