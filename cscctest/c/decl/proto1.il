.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.custom instance void OpenSystem.C.MemoryModelAttribute::.ctor(int32) = (01 00 40 00 00 00 00 00)
.method public static unsigned int64 'f1'(void * 'a', unsigned int64 'b') cil managed 
{
	ldc.i4.0
	conv.u8
	ret
	.maxstack 1
} // method f1
