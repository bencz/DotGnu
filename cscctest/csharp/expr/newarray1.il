.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private sequential sealed serializable ansi 'X' extends ['.library']'System'.'ValueType'
{
.field private int32 'x'
.method public hidebysig specialname rtspecialname instance void '.ctor'(int32 '_x') cil managed 
{
	ldarg.0
	ldarg.1
	stfld	int32 'X'::'x'
	ret
	.maxstack 2
} // method .ctor
} // class X
.class private auto sealed serializable ansi 'Color' extends ['.library']'System'.'Enum'
{
.field public static literal valuetype 'Color' 'Red' = int32(0x00000000)
.field public static literal valuetype 'Color' 'Green' = int32(0x00000001)
.field public static literal valuetype 'Color' 'Blue' = int32(0x00000002)
.field public specialname rtspecialname int32 'value__'
} // class Color
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private hidebysig instance void 'm1'(int32 'dim', unsigned int32 'dim2', int64 'dim3', unsigned int64 'dim4') cil managed 
{
	.locals init	(int32[], valuetype 'X'[], valuetype 'Color'[,], class 'Test'[], class 'Test'[][], class ['.library']'System'.'Object'[,,], class ['.library']'System'.'Object'[,,,][][,,], class ['.library']'System'.'Object'[,])
	ldc.i4.3
	newarr	['.library']'System'.'Int32'
	stloc.0
	ldarg.1
	newarr	'X'
	stloc.1
	ldc.i4.3
	ldarg.1
	newobj	instance void valuetype 'Color'[,]::.ctor(int32, int32)
	stloc.2
	ldc.i4.3
	newarr	'Test'
	stloc.3
	ldc.i4.3
	newarr	class 'Test'[]
	stloc.s	4
	ldarg.1
	ldarg.1
	ldarg.1
	newobj	instance void class ['.library']'System'.'Object'[,,]::.ctor(int32, int32, int32)
	stloc.s	5
	ldarg.1
	ldarg.1
	ldarg.1
	newobj	instance void class ['.library']'System'.'Object'[,,,][][,,]::.ctor(int32, int32, int32)
	stloc.s	6
	ldarg.2
	newarr	['.library']'System'.'Int32'
	stloc.0
	ldarg.3
	conv.ovf.u4.un
	newarr	['.library']'System'.'Int32'
	stloc.0
	ldarg.s	4
	conv.ovf.u4.un
	newarr	['.library']'System'.'Int32'
	stloc.0
	ldc.i4.3
	ldarg.2
	newobj	instance void class ['.library']'System'.'Object'[,]::.ctor(int32, int32)
	stloc.s	7
	ldc.i4.3
	ldarg.3
	conv.ovf.u4.un
	newobj	instance void class ['.library']'System'.'Object'[,]::.ctor(int32, int32)
	stloc.s	7
	ldc.i4.3
	ldarg.s	4
	conv.ovf.u4.un
	newobj	instance void class ['.library']'System'.'Object'[,]::.ctor(int32, int32)
	stloc.s	7
	ldarg.2
	ldc.i4.3
	newobj	instance void class ['.library']'System'.'Object'[,]::.ctor(int32, int32)
	stloc.s	7
	ldarg.3
	conv.ovf.u4.un
	ldc.i4.3
	newobj	instance void class ['.library']'System'.'Object'[,]::.ctor(int32, int32)
	stloc.s	7
	ldarg.s	4
	conv.ovf.u4.un
	ldc.i4.3
	newobj	instance void class ['.library']'System'.'Object'[,]::.ctor(int32, int32)
	stloc.s	7
	ret
	.maxstack 3
} // method m1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
