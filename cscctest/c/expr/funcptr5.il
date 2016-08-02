.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.custom instance void OpenSystem.C.MemoryModelAttribute::.ctor(int32) = (01 00 40 00 00 00 00 00)
.method public static int32 'compare_pos'(float32 modopt(['OpenSystem.C']'OpenSystem.C'.'IsConst') * 'a', float32 modopt(['OpenSystem.C']'OpenSystem.C'.'IsConst') * 'b') cil managed 
{
	ldc.i4.1
	ret
	.maxstack 1
} // method compare_pos
.field private static float32 * 'save_array'
.method public static int32 'main'() cil managed 
{
	.locals init	(int32)
	ldtoken 'init-on-demand'
	call	void [.library]System.Runtime.CompilerServices.RuntimeHelpers::RunClassConstructor(valuetype [.library]System.RuntimeTypeHandle)
	ldc.i4.0
	stloc.0
	ldsfld	float32 * 'save_array'
	ldloc.0
	ldc.i4.4
	ldftn	int32 'compare_pos'(float32 modopt(['OpenSystem.C']'OpenSystem.C'.'IsConst') *, float32 modopt(['OpenSystem.C']'OpenSystem.C'.'IsConst') *)
	call	void 'qsort'(void *, unsigned int32, unsigned int32, method int32 *(void modopt(['OpenSystem.C']'OpenSystem.C'.'IsConst') *, void modopt(['OpenSystem.C']'OpenSystem.C'.'IsConst') *) modopt(['OpenSystem.C']'OpenSystem.C'.'IsFunctionPointer'))
	ldsfld	float32 * 'save_array'
	ldloc.0
	ldc.i4.4
	ldftn	int32 'compare_pos'(float32 modopt(['OpenSystem.C']'OpenSystem.C'.'IsConst') *, float32 modopt(['OpenSystem.C']'OpenSystem.C'.'IsConst') *)
	call	void 'qsort'(void *, unsigned int32, unsigned int32, method int32 *(void modopt(['OpenSystem.C']'OpenSystem.C'.'IsConst') *, void modopt(['OpenSystem.C']'OpenSystem.C'.'IsConst') *) modopt(['OpenSystem.C']'OpenSystem.C'.'IsFunctionPointer'))
	ldc.i4.0
	ret
	.maxstack 4
} // method main
.method private static specialname void '.init-0'() cil managed 
{
.custom instance void ['OpenSystem.C']'OpenSystem.C'.'InitializerAttribute'::'.ctor'() = (01 00 00 00)
	ldc.i4.0
	conv.i
	stsfld	float32 * 'save_array'
	ret
	.maxstack 1
} // method .init-0
.method public static void '.start'(class [.library]System.String[] args) cil managed
{
	.entrypoint
	.maxstack	3
	.locals (int32, int8 * *, int8 * *, class [.library]System.Object)
.try { 
	ldarg.0
	ldc.i4.8
	ldloca	0
	call	native int 'OpenSystem.C'.'Crt0'::GetArgV(class [.library]System.String[], int32, int32 &)
	stloc.1
	call	native int 'OpenSystem.C'.'Crt0'::GetEnvironment()
	stloc.2
	ldstr "libc64"
	call	void 'OpenSystem.C'.'Crt0'::Startup(class [.library]System.String)
	call	int32 '<Module>'::'main'()
	call	void 'OpenSystem.C'.'Crt0'::Shutdown(int32)
	leave	L1
} catch [.library]System.OutOfMemoryException {
	rethrow
} catch [.library]System.Object {
	stloc.3
	ldloc.3
	call	class [.library]System.Object 'OpenSystem.C'.'Crt0'::ShutdownWithException(class [.library]System.Object)
	throw
}
L1:
	ret
} // method .start
