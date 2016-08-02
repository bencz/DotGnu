.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.custom instance void OpenSystem.C.MemoryModelAttribute::.ctor(int32) = (01 00 40 00 00 00 00 00)
.field public static valuetype 'array int[20]' 'a'
.field public static valuetype 'array int[1]' 'a2'
.field public static valuetype 'array int[0]' 'a3'
.field public static valuetype 'array int[3][5]' 'a4'
.field public static valuetype 'array int *[20]' 'a5'
.field public static valuetype 'array int *[3][5]' 'a6'
.field public static valuetype 'array int[3]' * modopt(['OpenSystem.C']'OpenSystem.C'.'IsConst') modreq(['.library']'System.Runtime.CompilerServices'.'IsVolatile') 'a7'
.method public static int8 * 'a8'(int32 'x') cil managed 
{
	ldc.i4.0
	conv.i
	ret
	.maxstack 1
} // method a8
.field public static int8 * 'p'
.field public static int8 modopt(['OpenSystem.C']'OpenSystem.C'.'IsConst') * 'p2'
.field public static int8 modopt(['OpenSystem.C']'OpenSystem.C'.'IsConst') * 'p3'
.field public static int8 * modopt(['OpenSystem.C']'OpenSystem.C'.'IsConst') 'p4'
.field public static char * modreq(['.library']'System.Runtime.CompilerServices'.'IsVolatile') 'p5'
.field public static method int32 *() modopt(['OpenSystem.C']'OpenSystem.C'.'IsFunctionPointer') 'f'
.field public static method int32 *(int32, int8 *) modopt(['OpenSystem.C']'OpenSystem.C'.'IsFunctionPointer') * 'f2'
.method public static void 'f3'(method int32 *(int8 *) modopt(['OpenSystem.C']'OpenSystem.C'.'IsFunctionPointer')) cil managed 
{
	ret
	.maxstack 0
} // method f3
.method public static void 'f4'(method int32 *(int8 *) modopt(['OpenSystem.C']'OpenSystem.C'.'IsFunctionPointer') * 'x') cil managed 
{
	ret
	.maxstack 0
} // method f4
.class public explicit sealed serializable ansi 'array int[20]' extends ['.library']'System'.'ValueType'
{
.size 80
.pack 4
.field [0] public specialname int32 'elem__'
} // class array int[20]
.class public explicit sealed serializable ansi 'array int[1]' extends ['.library']'System'.'ValueType'
{
.size 4
.pack 4
.field [0] public specialname int32 'elem__'
} // class array int[1]
.class public explicit sealed serializable ansi 'array int[0]' extends ['.library']'System'.'ValueType'
{
.size 0
.pack 4
.field public static specialname int32 'elem__'
} // class array int[0]
.class public explicit sealed serializable ansi 'array int[3][5]' extends ['.library']'System'.'ValueType'
{
.size 60
.pack 4
.field [0] public specialname valuetype 'array int[5]' 'elem__'
} // class array int[3][5]
.class public explicit sealed serializable ansi 'array int *[20]' extends ['.library']'System'.'ValueType'
{
.size 160
.pack 8
.field [0] public specialname int32 * 'elem__'
} // class array int *[20]
.class public explicit sealed serializable ansi 'array int *[3][5]' extends ['.library']'System'.'ValueType'
{
.size 120
.pack 8
.field [0] public specialname valuetype 'array int *[5]' 'elem__'
} // class array int *[3][5]
.class public explicit sealed serializable ansi 'array int[3]' extends ['.library']'System'.'ValueType'
{
.size 12
.pack 4
.field [0] public specialname int32 'elem__'
} // class array int[3]
.class public explicit sealed serializable ansi 'array int[5]' extends ['.library']'System'.'ValueType'
{
.size 20
.pack 4
.field [0] public specialname int32 'elem__'
} // class array int[5]
.class public explicit sealed serializable ansi 'array int *[5]' extends ['.library']'System'.'ValueType'
{
.size 40
.pack 8
.field [0] public specialname int32 * 'elem__'
} // class array int *[5]
