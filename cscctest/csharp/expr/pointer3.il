.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
	.custom instance void [.library]System.Security.Permissions.SecurityPermissionAttribute::.ctor(valuetype [.library]System.Security.Permissions.SecurityAction) =
		(01 00 08 00 00 00 01 00 54 02 10 53 6B 69 70 56
		 65 72 69 66 69 63 61 74 69 6F 6E 01)
}
.module '<Module>'
.custom instance void [.library]System.Security.UnverifiableCodeAttribute::.ctor() = (01 00 00 00)
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method public hidebysig instance void 't1'(unsigned int8 * 'x', unsigned int8 * 'y', int32 'i') cil managed 
{
	.locals init	(int64, unsigned int8 *)
	ldarg.1
	ldarg.2
	sub
	conv.i8
	stloc.0
	ldarg.1
	ldc.i4.4
	conv.i
	add
	stloc.1
	ldarg.1
	ldarg.3
	conv.i
	add
	stloc.1
	ldc.i4.4
	conv.i
	ldarg.1
	add
	stloc.1
	ldarg.3
	conv.i
	ldarg.1
	add
	stloc.1
	ldarg.1
	ldc.i4.4
	conv.i
	sub
	stloc.1
	ldloc.1
	ldc.i4.2
	conv.i
	add
	stloc.1
	ldloc.1
	ldc.i4.2
	conv.i
	sub
	stloc.1
	ret
	.maxstack 2
} // method t1
.method public hidebysig instance void 't1'(int32 * 'x', int32 * 'y', int32 'i') cil managed 
{
	.locals init	(int64, int32 *)
	ldarg.1
	ldarg.2
	sub
	conv.i8
	ldc.i4.4
	conv.i8
	div
	stloc.0
	ldarg.1
	ldc.i4.s	16
	conv.i
	add
	stloc.1
	ldarg.1
	ldarg.3
	conv.i
	ldc.i4.4
	conv.i
	mul
	add
	stloc.1
	ldc.i4.s	16
	conv.i
	ldarg.1
	add
	stloc.1
	ldarg.3
	conv.i
	ldc.i4.4
	conv.i
	mul
	ldarg.1
	add
	stloc.1
	ldarg.1
	ldc.i4.s	16
	conv.i
	sub
	stloc.1
	ldloc.1
	ldc.i4.8
	conv.i
	add
	stloc.1
	ldloc.1
	ldc.i4.8
	conv.i
	sub
	stloc.1
	ret
	.maxstack 3
} // method t1
.method public hidebysig instance void 't1'(int64 * 'x', int64 * 'y', int32 'i') cil managed 
{
	.locals init	(int64, int64 *)
	ldarg.1
	ldarg.2
	sub
	conv.i8
	ldc.i4.8
	conv.i8
	div
	stloc.0
	ldarg.1
	ldc.i4.s	32
	conv.i
	add
	stloc.1
	ldarg.1
	ldarg.3
	conv.i
	ldc.i4.8
	conv.i
	mul
	add
	stloc.1
	ldc.i4.s	32
	conv.i
	ldarg.1
	add
	stloc.1
	ldarg.3
	conv.i
	ldc.i4.8
	conv.i
	mul
	ldarg.1
	add
	stloc.1
	ldarg.1
	ldc.i4.s	32
	conv.i
	sub
	stloc.1
	ldloc.1
	ldc.i4.s	16
	conv.i
	add
	stloc.1
	ldloc.1
	ldc.i4.s	16
	conv.i
	sub
	stloc.1
	ret
	.maxstack 3
} // method t1
.method public hidebysig instance void 't1'(void * * 'x', void * * 'y', int32 'i') cil managed 
{
	.locals init	(int64, void * *)
	ldarg.1
	ldarg.2
	sub
	conv.i8
	sizeof	void *
	conv.i8
	div
	stloc.0
	ldarg.1
	ldc.i4.4
	conv.i
	sizeof	void *
	conv.i
	mul
	add
	stloc.1
	ldarg.1
	ldarg.3
	conv.i
	sizeof	void *
	conv.i
	mul
	add
	stloc.1
	ldc.i4.4
	conv.i
	sizeof	void *
	conv.i
	mul
	ldarg.1
	add
	stloc.1
	ldarg.3
	conv.i
	sizeof	void *
	conv.i
	mul
	ldarg.1
	add
	stloc.1
	ldarg.1
	ldc.i4.4
	conv.i
	sizeof	void *
	conv.i
	mul
	sub
	stloc.1
	ldloc.1
	ldc.i4.2
	conv.i
	sizeof	void *
	conv.i
	mul
	add
	stloc.1
	ldloc.1
	ldc.i4.2
	conv.i
	sizeof	void *
	conv.i
	mul
	sub
	stloc.1
	ret
	.maxstack 3
} // method t1
.method public hidebysig instance void 't2'() cil managed 
{
	.locals init	(unsigned int8 *, unsigned int8)
	ldloc.0
	ldc.i4.1
	conv.u
	add
	dup
	stloc.0
	ldind.u1
	stloc.1
	ldloc.0
	dup
	ldc.i4.1
	conv.u
	add
	stloc.0
	ldind.u1
	stloc.1
	ldloc.0
	ldc.i4.1
	conv.u
	sub
	dup
	stloc.0
	ldind.u1
	stloc.1
	ldloc.0
	dup
	ldc.i4.1
	conv.u
	sub
	stloc.0
	ldind.u1
	stloc.1
	ldloc.0
	ldc.i4.1
	conv.i
	add
	ldind.u1
	stloc.1
	ldloc.0
	ldind.u1
	stloc.1
	ret
	.maxstack 3
} // method t2
.method public hidebysig instance void 't3'() cil managed 
{
	.locals init	(int32 *, int32)
	ldloc.0
	ldc.i4.4
	conv.u
	add
	dup
	stloc.0
	ldind.i4
	stloc.1
	ldloc.0
	dup
	ldc.i4.4
	conv.u
	add
	stloc.0
	ldind.i4
	stloc.1
	ldloc.0
	ldc.i4.4
	conv.u
	sub
	dup
	stloc.0
	ldind.i4
	stloc.1
	ldloc.0
	dup
	ldc.i4.4
	conv.u
	sub
	stloc.0
	ldind.i4
	stloc.1
	ldloc.0
	ldc.i4.4
	conv.i
	add
	ldind.i4
	stloc.1
	ldloc.0
	ldind.i4
	stloc.1
	ret
	.maxstack 3
} // method t3
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
