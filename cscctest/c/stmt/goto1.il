.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.custom instance void OpenSystem.C.MemoryModelAttribute::.ctor(int32) = (01 00 40 00 00 00 00 00)
.method public static void 'func'(int32 'x') cil managed 
{
?L1:
	ldarg.0
	brtrue	?L2
	br	?L1
?L2:
	br	?L3
?L4:
	ret
?L3:
	ret
	.maxstack 1
} // method func
