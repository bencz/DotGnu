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
.method public hidebysig instance void 't1'(void * 'x') cil managed 
{
	ret
	.maxstack 0
} // method t1
.method public hidebysig instance void 't2'(int32 * 'y') cil managed 
{
	ldarg.0
	ldarg.1
	callvirt	instance void 'Test'::'t1'(void *)
	ldarg.0
	ldc.i4.0
	conv.i
	callvirt	instance void 'Test'::'t1'(void *)
	ret
	.maxstack 2
} // method t2
.method public hidebysig instance void 't2'(char * 'y') cil managed 
{
	ldarg.0
	ldarg.1
	callvirt	instance void 'Test'::'t2'(int32 *)
	ldarg.0
	ldc.i4.3
	conv.i
	callvirt	instance void 'Test'::'t2'(int32 *)
	ldarg.0
	ldc.i4.7
	conv.u8
	conv.u
	callvirt	instance void 'Test'::'t2'(int32 *)
	ret
	.maxstack 2
} // method t2
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
