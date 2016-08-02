.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.custom instance void OpenSystem.C.MemoryModelAttribute::.ctor(int32) = (01 00 40 00 00 00 00 00)
.method public static void 'func'(int32 * 'x') cil managed 
{
	ret
	.maxstack 0
} // method func
.method public static void 'func3'(valuetype 'array int[100]' * 'x') cil managed 
{
	ret
	.maxstack 0
} // method func3
.method public static void 'func2'() cil managed 
{
	.locals init	(valuetype 'array int[100]')
	ldloca.s	0
	call	void 'func'(int32 *)
	ldloca.s	0
	call	void 'func3'(valuetype 'array int[100]' *)
	ret
	.maxstack 1
} // method func2
.class public explicit sealed serializable ansi 'array int[100]' extends ['.library']'System'.'ValueType'
{
.size 400
.pack 4
.field [0] public specialname int32 'elem__'
} // class array int[100]
