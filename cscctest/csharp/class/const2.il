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
.method private hidebysig instance bool 'm1'() cil managed 
{
	ldc.i4.1
	ret
	.maxstack 1
} // method m1
.method private hidebysig instance bool 'm2'() cil managed 
{
	ldc.i4.0
	ret
	.maxstack 1
} // method m2
.method private hidebysig instance int32 'm3'() cil managed 
{
	ldc.i4	4294843840
	ret
	.maxstack 1
} // method m3
.method private hidebysig instance unsigned int32 'm4'() cil managed 
{
	ldc.i4	123456
	ret
	.maxstack 1
} // method m4
.method private hidebysig instance int64 'm5'() cil managed 
{
	ldc.i8	0xffffffff00000000
	ret
	.maxstack 1
} // method m5
.method private hidebysig instance unsigned int64 'm6'() cil managed 
{
	ldc.i8	0x7fffffffffffffff
	ret
	.maxstack 1
} // method m6
.method private hidebysig instance float32 'm7'() cil managed 
{
	ldc.r4	float32(0x6ACC3DA6)
	ret
	.maxstack 1
} // method m7
.method private hidebysig instance float64 'm8'() cil managed 
{
	ldc.r8	float64(0x4484D64651FE74C6)
	ret
	.maxstack 1
} // method m8
.method private hidebysig instance valuetype ['.library']'System'.'Decimal' 'm9'() cil managed 
{
	ldc.i4.m1
	ldc.i4.m1
	ldc.i4.m1
	ldc.i4.0
	ldc.i4.0
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	ret
	.maxstack 5
} // method m9
.method private hidebysig instance valuetype ['.library']'System'.'Decimal' 'm10'() cil managed 
{
	ldc.i4	2443182080
	ldc.i4	3397176012
	ldc.i4	669223
	ldc.i4.1
	ldc.i4.0
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	ret
	.maxstack 5
} // method m10
.method private hidebysig instance char 'm11'() cil managed 
{
	ldc.i4.s	97
	ret
	.maxstack 1
} // method m11
.method private hidebysig instance class ['.library']'System'.'String' 'm12'() cil managed 
{
	ldnull
	ret
	.maxstack 1
} // method m12
.method private hidebysig instance class ['.library']'System'.'String' 'm13'() cil managed 
{
	ldstr	""
	ret
	.maxstack 1
} // method m13
.method private hidebysig instance class ['.library']'System'.'String' 'm14'() cil managed 
{
	ldstr	"Hello World!"
	ret
	.maxstack 1
} // method m14
.method private hidebysig instance class ['.library']'System'.'Object' 'm15'() cil managed 
{
	ldnull
	ret
	.maxstack 1
} // method m15
.method private hidebysig instance class ['.library']'System'.'Object' 'm16'() cil managed 
{
	ldnull
	ret
	.maxstack 1
} // method m16
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
.method private static hidebysig specialname rtspecialname void '.cctor'() cil managed 
{
	ldc.i4.m1
	ldc.i4.m1
	ldc.i4.m1
	ldc.i4.0
	ldc.i4.0
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stsfld	valuetype ['.library']'System'.'Decimal' 'Test'::'dc1'
	ldc.i4	2443182080
	ldc.i4	3397176012
	ldc.i4	669223
	ldc.i4.1
	ldc.i4.0
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stsfld	valuetype ['.library']'System'.'Decimal' 'Test'::'dc2'
	ret
	.maxstack 5
} // method .cctor
} // class Test
