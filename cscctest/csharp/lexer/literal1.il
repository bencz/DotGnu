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
.field public static literal int32 'i1' = int32(0x00000000)
.field public static literal int32 'i2' = int32(0x0000000C)
.field public static literal int32 'i3' = int32(0x0001E240)
.field public static literal int32 'i4' = int32(0xFFFE1DC0)
.field public static literal int32 'i5' = int32(0x00123456)
.field public static literal int32 'i6' = int32(0x7FFFFFFF)
.field public static literal int32 'i7' = int32(0x7ABCDEF0)
.field public static literal int32 'i8' = int32(0x7FFFFFFF)
.field public static literal int32 'i9' = int32(0x80000000)
.field public static literal unsigned int32 'ui1' = int32(0x00000000)
.field public static literal unsigned int32 'ui2' = int32(0x0001E240)
.field public static literal unsigned int32 'ui3' = int32(0x0001E240)
.field public static literal unsigned int32 'ui4' = int32(0x00123456)
.field public static literal unsigned int32 'ui5' = int32(0x7FFFFFFF)
.field public static literal unsigned int32 'ui6' = int32(0x80000000)
.field public static literal unsigned int32 'ui7' = int32(0xFFFFFFFF)
.field public static literal int64 'l1' = int64(0x0000000000000000)
.field public static literal int64 'l2' = int64(0x000000000001E240)
.field public static literal int64 'l3' = int64(0x000000007FFFFFFF)
.field public static literal int64 'l4' = int64(0x0000000080000000)
.field public static literal int64 'l5' = int64(0xFFFFFFFF80000000)
.field public static literal int64 'l6' = int64(0x00000000FFFFFFFF)
.field public static literal int64 'l7' = int64(0x0000000100000000)
.field public static literal int64 'l8' = int64(0xFFFFFFFF00000000)
.field public static literal int64 'l9' = int64(0x000000000001E240)
.field public static literal int64 'l10' = int64(0x000000000001E240)
.field public static literal int64 'l11' = int64(0x7FFFFFFFFFFFFFFF)
.field public static literal int64 'l12' = int64(0x8000000000000000)
.field public static literal unsigned int64 'ul1' = int64(0x0000000000000000)
.field public static literal unsigned int64 'ul2' = int64(0x000000000001E240)
.field public static literal unsigned int64 'ul3' = int64(0x000000007FFFFFFF)
.field public static literal unsigned int64 'ul4' = int64(0x0000000080000000)
.field public static literal unsigned int64 'ul5' = int64(0x00000000FFFFFFFF)
.field public static literal unsigned int64 'ul6' = int64(0x0000000100000000)
.field public static literal unsigned int64 'ul7' = int64(0x000000000001E240)
.field public static literal unsigned int64 'ul8' = int64(0x000000000001E240)
.field public static literal unsigned int64 'ul9' = int64(0x7FFFFFFFFFFFFFFF)
.field public static literal unsigned int64 'ul10' = int64(0x8000000000000000)
.field public static literal unsigned int64 'ul11' = int64(0xFFFFFFFFFFFFFFFF)
.field public static literal unsigned int64 'ul12' = int64(0x000000000001E240)
.field public static literal unsigned int64 'ul13' = int64(0x000000000001E240)
.field public static literal unsigned int64 'ul14' = int64(0x000000000001E240)
.field public static literal unsigned int64 'ul15' = int64(0x000000000001E240)
.field public static literal unsigned int64 'ul16' = int64(0x000000000001E240)
.field public static literal unsigned int64 'ul17' = int64(0x000000000001E240)
.field public static literal unsigned int64 'ul18' = int64(0x000000000001E240)
.field public static literal unsigned int64 'ul19' = int64(0x000000000001E240)
.field public static literal unsigned int64 'ul20' = int64(0x000000000001E240)
.field public static literal float32 'f1' = float32(0x00000000)
.field public static literal float32 'f2' = float32(0xBF800000)
.field public static literal float32 'f3' = float32(0x6ACC3DA6)
.field public static literal float32 'f4' = float32(0x6ACC3DA6)
.field public static literal float32 'f5' = float32(0x1E6933A9)
.field public static literal float32 'f6' = float32(0x3DFBE76D)
.field public static literal float32 'f7' = float32(0x6426B233)
.field public static literal float32 'f8' = float32(0x6922CA05)
.field public static literal float32 'f9' = float32(0x6922CA05)
.field public static literal float32 'f10' = float32(0x1CB9DF52)
.field public static literal float32 'f11' = float32(0x42F60000)
.field public static literal float64 'd1' = float64(0x0000000000000000)
.field public static literal float64 'd2' = float64(0xBFF0000000000000)
.field public static literal float64 'd3' = float64(0x455987B4CAE67FA8)
.field public static literal float64 'd4' = float64(0x455987B4CAE67FA8)
.field public static literal float64 'd5' = float64(0x3BCD267511916BC9)
.field public static literal float64 'd6' = float64(0x3FBF7CED916872B0)
.field public static literal float64 'd7' = float64(0x4484D64651FE74C6)
.field public static literal float64 'd8' = float64(0x45245940AC127E09)
.field public static literal float64 'd9' = float64(0x45245940AC127E09)
.field public static literal float64 'd10' = float64(0x3B973BEA3F91F75A)
.field public static literal float64 'd11' = float64(0x405EC00000000000)
.field public static initonly valuetype ['.library']'System'.'Decimal' 'dc1'
.custom instance void ['.library']'System.Runtime.CompilerServices'.'DecimalConstantAttribute'::'.ctor'(unsigned int8, unsigned int8, unsigned int32, unsigned int32, unsigned int32) = (01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00)
.field public static initonly valuetype ['.library']'System'.'Decimal' 'dc2'
.custom instance void ['.library']'System.Runtime.CompilerServices'.'DecimalConstantAttribute'::'.ctor'(unsigned int8, unsigned int8, unsigned int32, unsigned int32, unsigned int32) = (01 00 00 80 00 00 00 00 00 00 00 00 01 00 00 00 00 00)
.field public static initonly valuetype ['.library']'System'.'Decimal' 'dc3'
.custom instance void ['.library']'System.Runtime.CompilerServices'.'DecimalConstantAttribute'::'.ctor'(unsigned int8, unsigned int8, unsigned int32, unsigned int32, unsigned int32) = (01 00 00 00 D3 1E 66 00 A0 FE 99 2B 00 00 00 E8 00 00)
.field public static initonly valuetype ['.library']'System'.'Decimal' 'dc4'
.custom instance void ['.library']'System.Runtime.CompilerServices'.'DecimalConstantAttribute'::'.ctor'(unsigned int8, unsigned int8, unsigned int32, unsigned int32, unsigned int32) = (01 00 00 00 D3 1E 66 00 A0 FE 99 2B 00 00 00 E8 00 00)
.field public static initonly valuetype ['.library']'System'.'Decimal' 'dc5'
.custom instance void ['.library']'System.Runtime.CompilerServices'.'DecimalConstantAttribute'::'.ctor'(unsigned int8, unsigned int8, unsigned int32, unsigned int32, unsigned int32) = (01 00 19 00 00 00 00 00 00 00 00 00 40 E2 01 00 00 00)
.field public static initonly valuetype ['.library']'System'.'Decimal' 'dc6'
.custom instance void ['.library']'System.Runtime.CompilerServices'.'DecimalConstantAttribute'::'.ctor'(unsigned int8, unsigned int8, unsigned int32, unsigned int32, unsigned int32) = (01 00 03 00 00 00 00 00 00 00 00 00 7B 00 00 00 00 00)
.field public static initonly valuetype ['.library']'System'.'Decimal' 'dc7'
.custom instance void ['.library']'System.Runtime.CompilerServices'.'DecimalConstantAttribute'::'.ctor'(unsigned int8, unsigned int8, unsigned int32, unsigned int32, unsigned int32) = (01 00 00 00 9A 02 00 00 CE 3F CA C8 00 00 B0 98 00 00)
.field public static initonly valuetype ['.library']'System'.'Decimal' 'dc8'
.custom instance void ['.library']'System.Runtime.CompilerServices'.'DecimalConstantAttribute'::'.ctor'(unsigned int8, unsigned int8, unsigned int32, unsigned int32, unsigned int32) = (01 00 00 00 A0 2C 0A 00 04 3F 09 56 00 00 80 6F 00 00)
.field public static initonly valuetype ['.library']'System'.'Decimal' 'dc9'
.custom instance void ['.library']'System.Runtime.CompilerServices'.'DecimalConstantAttribute'::'.ctor'(unsigned int8, unsigned int8, unsigned int32, unsigned int32, unsigned int32) = (01 00 00 00 A0 2C 0A 00 04 3F 09 56 00 00 80 6F 00 00)
.field public static initonly valuetype ['.library']'System'.'Decimal' 'dc10'
.custom instance void ['.library']'System.Runtime.CompilerServices'.'DecimalConstantAttribute'::'.ctor'(unsigned int8, unsigned int8, unsigned int32, unsigned int32, unsigned int32) = (01 00 17 00 00 00 00 00 00 00 00 00 7B 00 00 00 00 00)
.field public static initonly valuetype ['.library']'System'.'Decimal' 'dc11'
.custom instance void ['.library']'System.Runtime.CompilerServices'.'DecimalConstantAttribute'::'.ctor'(unsigned int8, unsigned int8, unsigned int32, unsigned int32, unsigned int32) = (01 00 00 00 00 00 00 00 00 00 00 00 7B 00 00 00 00 00)
.field public static initonly valuetype ['.library']'System'.'Decimal' 'dc12'
.custom instance void ['.library']'System.Runtime.CompilerServices'.'DecimalConstantAttribute'::'.ctor'(unsigned int8, unsigned int8, unsigned int32, unsigned int32, unsigned int32) = (01 00 00 00 FF FF FF FF FF FF FF FF FF FF FF FF 00 00)
.field public static initonly valuetype ['.library']'System'.'Decimal' 'dc13'
.custom instance void ['.library']'System.Runtime.CompilerServices'.'DecimalConstantAttribute'::'.ctor'(unsigned int8, unsigned int8, unsigned int32, unsigned int32, unsigned int32) = (01 00 00 80 FF FF FF FF FF FF FF FF FF FF FF FF 00 00)
.field public static initonly valuetype ['.library']'System'.'Decimal' 'dc14'
.custom instance void ['.library']'System.Runtime.CompilerServices'.'DecimalConstantAttribute'::'.ctor'(unsigned int8, unsigned int8, unsigned int32, unsigned int32, unsigned int32) = (01 00 14 80 FF FF FF FF FF FF FF FF FF FF FF FF 00 00)
.field public static initonly valuetype ['.library']'System'.'Decimal' 'dc15'
.custom instance void ['.library']'System.Runtime.CompilerServices'.'DecimalConstantAttribute'::'.ctor'(unsigned int8, unsigned int8, unsigned int32, unsigned int32, unsigned int32) = (01 00 1C 80 FF FF FF FF FF FF FF FF FF FF FF FF 00 00)
.field public static initonly valuetype ['.library']'System'.'Decimal' 'dc16'
.custom instance void ['.library']'System.Runtime.CompilerServices'.'DecimalConstantAttribute'::'.ctor'(unsigned int8, unsigned int8, unsigned int32, unsigned int32, unsigned int32) = (01 00 1C 80 99 99 99 19 99 99 99 99 9A 99 99 99 00 00)
.field public static initonly valuetype ['.library']'System'.'Decimal' 'dc17'
.custom instance void ['.library']'System.Runtime.CompilerServices'.'DecimalConstantAttribute'::'.ctor'(unsigned int8, unsigned int8, unsigned int32, unsigned int32, unsigned int32) = (01 00 1C 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00)
.field public static initonly valuetype ['.library']'System'.'Decimal' 'dc18'
.custom instance void ['.library']'System.Runtime.CompilerServices'.'DecimalConstantAttribute'::'.ctor'(unsigned int8, unsigned int8, unsigned int32, unsigned int32, unsigned int32) = (01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00)
.field public static literal char 'c1' = char(0x0061)
.field public static literal char 'c2' = char(0x0027)
.field public static literal char 'c3' = char(0x0022)
.field public static literal char 'c4' = char(0x005C)
.field public static literal char 'c5' = char(0x0000)
.field public static literal char 'c6' = char(0x0007)
.field public static literal char 'c7' = char(0x0008)
.field public static literal char 'c8' = char(0x000C)
.field public static literal char 'c9' = char(0x000A)
.field public static literal char 'c10' = char(0x000D)
.field public static literal char 'c11' = char(0x0009)
.field public static literal char 'c12' = char(0x000B)
.field public static literal char 'c13' = char(0x0003)
.field public static literal char 'c14' = char(0x0003)
.field public static literal char 'c15' = char(0x0003)
.field public static literal char 'c16' = char(0x0003)
.field public static literal char 'c17' = char(0x0003)
.field public static literal char 'c18' = char(0x0003)
.field public static literal char 'c19' = char(0x001B)
.field public static literal class ['.library']'System'.'String' 's1' = nullref
.field public static literal class ['.library']'System'.'String' 's2' = ""
.field public static literal class ['.library']'System'.'String' 's3' = "Hello World!"
.field public static literal class ['.library']'System'.'String' 's4' = "Hello \\tWo\"rld!"
.field public static literal class ['.library']'System'.'String' 's5' = "line 1\nline 2\nline 3"
.field public static literal class ['.library']'System'.'String' 's6' = "\n#bad\n"
.field public static literal class ['.library']'System'.'String' 's7' = "\uDBC4\uDE34"
.field public static literal class ['.library']'System'.'Object' 'o1' = nullref
.field public static literal class 'Test' 'o2' = nullref
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
.method private static hidebysig specialname rtspecialname void '.cctor'() cil managed 
{
	ldc.i4.0
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	stsfld	valuetype ['.library']'System'.'Decimal' 'Test'::'dc1'
	ldc.i4.m1
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	stsfld	valuetype ['.library']'System'.'Decimal' 'Test'::'dc2'
	ldc.i4	3892314112
	ldc.i4	731512480
	ldc.i4	6692563
	ldc.i4.0
	ldc.i4.0
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stsfld	valuetype ['.library']'System'.'Decimal' 'Test'::'dc3'
	ldc.i4	3892314112
	ldc.i4	731512480
	ldc.i4	6692563
	ldc.i4.0
	ldc.i4.0
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stsfld	valuetype ['.library']'System'.'Decimal' 'Test'::'dc4'
	ldc.i4	123456
	ldc.i4.0
	ldc.i4.0
	ldc.i4.0
	ldc.i4.s	25
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stsfld	valuetype ['.library']'System'.'Decimal' 'Test'::'dc5'
	ldc.i4.s	123
	ldc.i4.0
	ldc.i4.0
	ldc.i4.0
	ldc.i4.3
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stsfld	valuetype ['.library']'System'.'Decimal' 'Test'::'dc6'
	ldc.i4	2561671168
	ldc.i4	3368697806
	ldc.i4	666
	ldc.i4.0
	ldc.i4.0
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stsfld	valuetype ['.library']'System'.'Decimal' 'Test'::'dc7'
	ldc.i4	1870659584
	ldc.i4	1443446532
	ldc.i4	666784
	ldc.i4.0
	ldc.i4.0
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stsfld	valuetype ['.library']'System'.'Decimal' 'Test'::'dc8'
	ldc.i4	1870659584
	ldc.i4	1443446532
	ldc.i4	666784
	ldc.i4.0
	ldc.i4.0
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stsfld	valuetype ['.library']'System'.'Decimal' 'Test'::'dc9'
	ldc.i4.s	123
	ldc.i4.0
	ldc.i4.0
	ldc.i4.0
	ldc.i4.s	23
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stsfld	valuetype ['.library']'System'.'Decimal' 'Test'::'dc10'
	ldc.i4.s	123
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	stsfld	valuetype ['.library']'System'.'Decimal' 'Test'::'dc11'
	ldc.i4.m1
	ldc.i4.m1
	ldc.i4.m1
	ldc.i4.0
	ldc.i4.0
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stsfld	valuetype ['.library']'System'.'Decimal' 'Test'::'dc12'
	ldc.i4.m1
	ldc.i4.m1
	ldc.i4.m1
	ldc.i4.1
	ldc.i4.0
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stsfld	valuetype ['.library']'System'.'Decimal' 'Test'::'dc13'
	ldc.i4.m1
	ldc.i4.m1
	ldc.i4.m1
	ldc.i4.1
	ldc.i4.s	20
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stsfld	valuetype ['.library']'System'.'Decimal' 'Test'::'dc14'
	ldc.i4.m1
	ldc.i4.m1
	ldc.i4.m1
	ldc.i4.1
	ldc.i4.s	28
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stsfld	valuetype ['.library']'System'.'Decimal' 'Test'::'dc15'
	ldc.i4	2576980378
	ldc.i4	2576980377
	ldc.i4	429496729
	ldc.i4.1
	ldc.i4.s	28
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stsfld	valuetype ['.library']'System'.'Decimal' 'Test'::'dc16'
	ldc.i4.1
	ldc.i4.0
	ldc.i4.0
	ldc.i4.0
	ldc.i4.s	28
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stsfld	valuetype ['.library']'System'.'Decimal' 'Test'::'dc17'
	ldc.i4.0
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	stsfld	valuetype ['.library']'System'.'Decimal' 'Test'::'dc18'
	ret
	.maxstack 5
} // method .cctor
} // class Test
