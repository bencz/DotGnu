.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.custom instance void OpenSystem.C.ModuleAttribute::.ctor() = (01 00 00 00)
.method public static int32 'func'(int32 'x') cil managed 
{
	ldarg.0
	ret
	.maxstack 1
} // method func
.method public static int32 'func2'(int32) cil managed 
{
	ldc.i4.0
	ret
	.maxstack 1
} // method func2
.field public static specialname int32 *(int32) 'func4-alias'
.method public static int32 'func4'(int32) cil managed
{
.custom instance void OpenSystem.C.WeakAliasForAttribute::.ctor(class [.library]System.String) = (01 00 05 66 75 6E 63 32 00 00)
	ldarg	0
	ldsfld	int32 *(int32) 'func4-alias'
	tail.
	calli	int32 (int32)
	ret
	.maxstack 2
}
.method private static specialname void '.init-func4'() cil managed
{
.custom instance void OpenSystem.C.InitializerAttribute::.ctor() = (01 00 00 00)
	ldftn	int32 'func2'(int32)
	stsfld	int32 *(int32) 'func4-alias'
	ret
	.maxstack 1
}
.method public static int32 'func5'(int32) cil managed
{
.custom instance void OpenSystem.C.StrongAliasForAttribute::.ctor(class [.library]System.String) = (01 00 05 66 75 6E 63 32 00 00)
	ldarg	0
	tail.
	call	int32 'func2'(int32)
	ret
	.maxstack 1
}
.method public static pinvokeimpl("module.so" ansi cdecl ) int32 'func6'(int32) cil managed {}
